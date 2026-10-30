Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraEditors.Mask
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Solicitud y autorización de días de vacaciones, ausencias, subsidios, feriados, etc. (V.5.0 FrmSolicitud).
''' Solo UI: los datos pasan por <see cref="CsSolicitudDAL"/> y el saldo por <see cref="CsVacacionesDAL"/>.
''' <see cref="CargarEmpleado"/> y <see cref="CargarSolicitud"/> son públicos para abrir la pantalla desde otros formularios.
''' El botón de impresión es <c>BtnImprimir</c> (en la V.5.0 se llamaba <c>CmdBorrar</c> aunque imprimía).
''' La interfaz se arma por código (no hay Designer).
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmSolicitud
    Inherits XtraForm

    Private Shared ReadOnly TiposSolicitud As String() = {
        "Vacaciones", "Vacaciones Pagadas", "Vacaciones Programadas", "Subsidio", "Ausente", "AusenteBasico",
        "AusenteBasicoExcento", "AusenteBasicoDINSS", "Feriado", "Permiso Programado", "Suspension"}

    Private Shared ReadOnly Clasificaciones As String() = {
        "Riesgo Laboral", "Accidente Comun", "Embarazo", "Enfermedades", "Ausente", "Vacaciones", "Vacaciones Pagadas",
        "Vacaciones Programadas", "Feriado", "Permiso Programado", "Suspension"}

    ''' <summary>Más de estos días se imprime el formato largo (<see cref="RptSolicitudVacaLargo"/>); si no, el corto (<see cref="RptSolicitudVacaCorto"/>).</summary>
    Private Const DiasFormatoLargo As Double = 3

    Private _dal As CsSolicitudDAL
    Private _vacaciones As CsVacacionesDAL
    Private _empleado As EmpleadoSolicitud
    Private _resumen As ResumenVacaciones
    ''' <summary>Solicitud abierta para modificar; Nothing = solicitud nueva.</summary>
    Private _cargada As SolicitudVacaciones
    ''' <summary>Días de la solicitud abierta que ya están contados en <see cref="_resumen"/>.</summary>
    Private _diasGuardados As Double
    Private _disfrutados, _disponibles As Double
    Private _cargando As Boolean

    ' Encabezado
    Private TxtNumero As TextEdit
    Private DtpFechaSolicitud As DateEdit
    Private CmbTipoSolicitud As ComboBoxEdit
    ' Empleado
    Private CmbEmpleado As SearchLookUpEdit
    Private ChkNomina As CheckEdit
    Private CmbNomina As ComboBoxEdit
    Private TxtNombre, TxtDepartamento, TxtCargo, TxtFechaIngreso As TextEdit
    Private TxtDiasVacaciones, TxtDisfrutados, TxtDisponibles As TextEdit
    Private LblDiasVacaciones, LblDisfrutados, LblDisponibles As LabelControl
    ' Datos de la solicitud
    Private CmbClasificacion As ComboBoxEdit
    Private DtpFechaInicio, DtpFechaFin As DateEdit
    Private TxtDiasSolicitud, TxtHorasSolicitud As TextEdit
    Private TxtObservaciones As MemoEdit
    ' Solicitudes del empleado y botones
    Private GridSolicitudes As GridControl
    Private VistaSolicitudes As GridView
    Private BtnNuevo, BtnBuscar, BtnSeleccionar, BtnGrabar, BtnAnular, BtnImprimir, BtnSalir As SimpleButton

    Public Sub New()

        Text = "Solicitud y Autorización de Días de Vacaciones"
        ClientSize = New Size(900, 462)
        StartPosition = FormStartPosition.CenterParent
        AutoScroll = True

        CrearInterfaz()

        AddHandler Load, AddressOf FrmSolicitud_Load

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        Dim encabezado As New PanelControl With {.Location = New Point(8, 6), .Size = New Size(884, 36)}
        encabezado.Controls.Add(Etiqueta("Número:", 10, 10))
        TxtNumero = New TextEdit With {.Location = New Point(65, 7), .Size = New Size(90, 20)}
        TxtNumero.Properties.ReadOnly = True
        encabezado.Controls.Add(Etiqueta("Fecha solicitud:", 175, 10))
        DtpFechaSolicitud = New DateEdit With {.Location = New Point(265, 7), .Size = New Size(110, 20), .DateTime = Date.Today}
        encabezado.Controls.Add(Etiqueta("Tipo de solicitud:", 395, 10))
        CmbTipoSolicitud = Combo(TiposSolicitud, 495, 7, 190)
        AddHandler CmbTipoSolicitud.EditValueChanged, AddressOf CmbTipoSolicitud_EditValueChanged
        BtnBuscar = New SimpleButton With {.Text = "Buscar solicitud", .Location = New Point(745, 4), .Size = New Size(130, 27)}
        BtnBuscar.ImageOptions.ImageUri.Uri = "Find"
        AddHandler BtnBuscar.Click, AddressOf BtnBuscar_Click
        encabezado.Controls.AddRange(New Control() {TxtNumero, DtpFechaSolicitud, CmbTipoSolicitud, BtnBuscar})

        Dim gEmpleado As GroupControl = Grupo("Empleado", 8, 48, 432, 182)
        CmbEmpleado = New SearchLookUpEdit With {.Location = New Point(100, 28), .Size = New Size(120, 20)}
        CmbEmpleado.Properties.DisplayMember = "CodEmpleado1"
        CmbEmpleado.Properties.ValueMember = "CodEmpleado1"
        CmbEmpleado.Properties.NullText = ""
        CmbEmpleado.Properties.PopupFormSize = New Size(450, 350)
        AddHandler CmbEmpleado.EditValueChanged, AddressOf CmbEmpleado_EditValueChanged
        gEmpleado.Controls.Add(Etiqueta("Código:", 10, 31))
        gEmpleado.Controls.Add(CmbEmpleado)
        ChkNomina = New CheckEdit With {.Text = "Toda la nómina:", .Location = New Point(228, 28), .Size = New Size(105, 20)}
        CmbNomina = Combo(New String() {}, 335, 28, 90)
        AddHandler ChkNomina.CheckedChanged, AddressOf ChkNomina_CheckedChanged
        gEmpleado.Controls.AddRange(New Control() {ChkNomina, CmbNomina})
        TxtNombre = Lectura(gEmpleado, "Nombres:", 10, 57, 100, 322)
        TxtDepartamento = Lectura(gEmpleado, "Departamento:", 10, 83, 100, 322)
        TxtCargo = Lectura(gEmpleado, "Cargo:", 10, 109, 100, 322)
        TxtFechaIngreso = Lectura(gEmpleado, "Fecha ingreso:", 10, 135, 100, 100)
        LblDisponibles = Etiqueta("Días disponibles:", 215, 135)
        TxtDisponibles = SoloLectura(315, 132, 107)
        TxtDisponibles.Properties.Appearance.Font = New Font(TxtDisponibles.Font, FontStyle.Bold)
        LblDiasVacaciones = Etiqueta("Días vacaciones:", 10, 161)
        TxtDiasVacaciones = SoloLectura(100, 158, 100)
        LblDisfrutados = Etiqueta("Días disfrutados:", 215, 161)
        TxtDisfrutados = SoloLectura(315, 158, 107)
        gEmpleado.Controls.AddRange(New Control() {LblDiasVacaciones, TxtDiasVacaciones, LblDisfrutados, TxtDisfrutados, LblDisponibles, TxtDisponibles})
        gEmpleado.Height = 188

        Dim gDatos As GroupControl = Grupo("Datos de la solicitud", 448, 48, 444, 188)
        gDatos.Controls.Add(Etiqueta("Clasificación:", 10, 31))
        CmbClasificacion = Combo(Clasificaciones, 100, 28, 200)
        gDatos.Controls.Add(CmbClasificacion)
        DtpFechaInicio = Fecha(gDatos, "Fecha inicio:", 10, 57, 100)
        DtpFechaFin = Fecha(gDatos, "Fecha fin:", 225, 57, 295)
        TxtDiasSolicitud = Numero(gDatos, "Días a disfrutar:", 10, 83, 100, 110)
        TxtHorasSolicitud = Numero(gDatos, "Horas a disfrutar:", 225, 83, 325, 95)
        gDatos.Controls.Add(Etiqueta("Observaciones:", 10, 109))
        TxtObservaciones = New MemoEdit With {.Location = New Point(10, 128), .Size = New Size(424, 52)}
        gDatos.Controls.Add(TxtObservaciones)
        AddHandler DtpFechaInicio.EditValueChanged, AddressOf Fechas_EditValueChanged
        AddHandler DtpFechaFin.EditValueChanged, AddressOf Fechas_EditValueChanged
        AddHandler TxtDiasSolicitud.EditValueChanged, AddressOf TxtDiasSolicitud_EditValueChanged
        AddHandler TxtHorasSolicitud.EditValueChanged, Sub(s, e) If Not _cargando Then MostrarTotales()

        Dim gLista As GroupControl = Grupo("Solicitudes del empleado", 8, 242, 884, 168)
        GridSolicitudes = New GridControl With {.Dock = DockStyle.Fill}
        VistaSolicitudes = New GridView(GridSolicitudes)
        GridSolicitudes.MainView = VistaSolicitudes
        GridSolicitudes.ViewCollection.Add(VistaSolicitudes)
        VistaSolicitudes.OptionsBehavior.Editable = False
        VistaSolicitudes.OptionsView.ShowGroupPanel = False
        VistaSolicitudes.OptionsView.ColumnAutoWidth = True
        EstiloGrid.Aplicar(VistaSolicitudes)
        AddHandler VistaSolicitudes.DoubleClick, Sub(s, e) SeleccionarDelGrid()
        gLista.Controls.Add(GridSolicitudes)

        BtnNuevo = Boton("Nuevo", "New", 8)
        BtnSeleccionar = Boton("Seleccionar", "Edit", 109)
        BtnGrabar = Boton("Autorizar", "Save", 210)
        BtnAnular = Boton("Anular", "Delete", 311)
        BtnImprimir = Boton("Imprimir", "Print", 412)
        BtnSalir = Boton("Salir", "Close", 797)
        AddHandler BtnNuevo.Click, Sub(s, e) Limpiar()
        AddHandler BtnSeleccionar.Click, Sub(s, e) SeleccionarDelGrid()
        AddHandler BtnGrabar.Click, AddressOf BtnGrabar_Click
        AddHandler BtnAnular.Click, AddressOf BtnAnular_Click
        AddHandler BtnImprimir.Click, AddressOf BtnImprimir_Click
        AddHandler BtnSalir.Click, Sub(s, e) Close()

        Controls.AddRange(New Control() {encabezado, gEmpleado, gDatos, gLista,
                                         BtnNuevo, BtnSeleccionar, BtnGrabar, BtnAnular, BtnImprimir, BtnSalir})

    End Sub

    Private Shared Function Grupo(titulo As String, x As Integer, y As Integer, ancho As Integer, alto As Integer) As GroupControl
        Return New GroupControl With {.Text = titulo, .Location = New Point(x, y), .Size = New Size(ancho, alto)}
    End Function

    Private Shared Function Etiqueta(texto As String, x As Integer, y As Integer) As LabelControl
        Return New LabelControl With {.Text = texto, .Location = New Point(x, y)}
    End Function

    Private Shared Function SoloLectura(x As Integer, y As Integer, ancho As Integer) As TextEdit
        Dim t As New TextEdit With {.Location = New Point(x, y), .Size = New Size(ancho, 20)}
        t.Properties.ReadOnly = True
        Return t
    End Function

    Private Shared Function Lectura(g As GroupControl, texto As String, xEt As Integer, y As Integer, x As Integer, ancho As Integer) As TextEdit
        Dim t As TextEdit = SoloLectura(x, y - 3, ancho)
        g.Controls.Add(Etiqueta(texto, xEt, y))
        g.Controls.Add(t)
        Return t
    End Function

    Private Shared Function Numero(g As GroupControl, texto As String, xEt As Integer, y As Integer, x As Integer, ancho As Integer) As TextEdit
        Dim t As New TextEdit With {.Location = New Point(x, y - 3), .Size = New Size(ancho, 20)}
        t.Properties.Mask.MaskType = MaskType.Numeric
        t.Properties.Mask.EditMask = "n2"
        t.Properties.Mask.UseMaskAsDisplayFormat = True
        g.Controls.Add(Etiqueta(texto, xEt, y))
        g.Controls.Add(t)
        Return t
    End Function

    Private Shared Function Fecha(g As GroupControl, texto As String, xEt As Integer, y As Integer, x As Integer) As DateEdit
        Dim d As New DateEdit With {.Location = New Point(x, y - 3), .Size = New Size(110, 20), .DateTime = Date.Today}
        g.Controls.Add(Etiqueta(texto, xEt, y))
        g.Controls.Add(d)
        Return d
    End Function

    Private Shared Function Combo(items As String(), x As Integer, y As Integer, ancho As Integer) As ComboBoxEdit
        Dim c As New ComboBoxEdit With {.Location = New Point(x, y), .Size = New Size(ancho, 20)}
        c.Properties.TextEditStyle = TextEditStyles.DisableTextEditor
        c.Properties.Items.AddRange(items)
        Return c
    End Function

    Private Function Boton(texto As String, icono As String, x As Integer) As SimpleButton
        Dim b As New SimpleButton With {.Text = texto, .Location = New Point(x, 418), .Size = New Size(95, 35)}
        b.ImageOptions.ImageUri.Uri = icono
        Return b
    End Function

#End Region

#Region "CARGA"

    Private Sub FrmSolicitud_Load(sender As Object, e As EventArgs)

        Try
            _dal = New CsSolicitudDAL()
            _vacaciones = New CsVacacionesDAL()
            RecargarEmpleados()
            FrmConsulta.AgregarBotonBuscar(CmbEmpleado, TipoBusqueda.Empleado, Me)
            CmbNomina.Properties.Items.AddRange(_dal.ListarNominas())
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo abrir la pantalla de solicitudes: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Limpiar()

    End Sub

    ''' <summary>Vuelve a leer los empleados activos del buscador.</summary>
    Public Sub RecargarEmpleados()
        CmbEmpleado.Properties.DataSource = _dal.ListarEmpleados()
        CmbEmpleado.Properties.View.Columns.Clear()
        CmbEmpleado.Properties.View.PopulateColumns(CmbEmpleado.Properties.DataSource)
    End Sub

    ''' <summary>Deja la pantalla lista para una solicitud nueva (V.5.0 BackToLife).</summary>
    Public Sub Limpiar()

        _cargando = True
        Try
            _cargada = Nothing
            _empleado = Nothing
            _resumen = Nothing
            _diasGuardados = 0
            CmbEmpleado.EditValue = Nothing
            ChkNomina.Checked = False
            For Each t As TextEdit In {TxtNombre, TxtDepartamento, TxtCargo, TxtFechaIngreso}
                t.Text = ""
            Next
            DtpFechaSolicitud.DateTime = Date.Today
            CmbTipoSolicitud.EditValue = "Vacaciones"
            CmbClasificacion.EditValue = "Vacaciones"
            DtpFechaInicio.DateTime = Date.Today
            DtpFechaFin.DateTime = Date.Today
            TxtDiasSolicitud.EditValue = 1D
            TxtHorasSolicitud.EditValue = 0D
            TxtObservaciones.Text = ""
            TxtNumero.Text = If(_dal Is Nothing, "", _dal.ProximoNumero())
            GridSolicitudes.DataSource = Nothing
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo preparar una solicitud nueva: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _cargando = False
        End Try

        AjustarDias()
        MostrarTotales()
        ActualizarBotones()

    End Sub

    ''' <summary>Carga el empleado por su código visible: datos, solicitudes y saldo de vacaciones a hoy.</summary>
    Public Sub CargarEmpleado(codigo1 As String)

        If _dal Is Nothing OrElse String.IsNullOrWhiteSpace(codigo1) Then Return

        Dim previo As Boolean = _cargando
        _cargando = True
        Try
            Cursor = Cursors.WaitCursor
            If Not Equals(CmbEmpleado.EditValue, codigo1) Then CmbEmpleado.EditValue = codigo1

            ' Otro empleado: la solicitud abierta deja de aplicar y se pasa a una nueva.
            If _cargada IsNot Nothing AndAlso _cargada.Codigo_Empleado <> codigo1 Then
                _cargada = Nothing
                _diasGuardados = 0
                TxtNumero.Text = _dal.ProximoNumero()
            End If

            _empleado = _dal.BuscarEmpleado(codigo1)

            If _empleado Is Nothing Then
                _resumen = Nothing
                TxtNombre.Text = ""
                TxtDepartamento.Text = ""
                TxtCargo.Text = ""
                TxtFechaIngreso.Text = ""
                XtraMessageBox.Show("El empleado " & codigo1 & " no existe o está inactivo.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                TxtNombre.Text = _empleado.Nombre_Completo
                TxtDepartamento.Text = _empleado.Departamento
                TxtCargo.Text = _empleado.Cargo
                TxtFechaIngreso.Text = If(_empleado.Fecha_Contrato_Vac.HasValue, _empleado.Fecha_Contrato_Vac.Value.ToString("dd/MM/yyyy"), "")
                _resumen = If(_empleado.Fecha_Contrato_Vac.HasValue, _vacaciones.CalcularResumen(codigo1, Date.Now), Nothing)
            End If

            MostrarSolicitudes(codigo1)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo cargar el empleado: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
            _cargando = previo
        End Try

        MostrarTotales()
        ActualizarBotones()

    End Sub

    ''' <summary>Abre una solicitud existente para verla, modificarla, anularla o imprimirla. False si no existe.</summary>
    Public Function CargarSolicitud(numero As String) As Boolean

        If _dal Is Nothing Then Return False

        Dim s As SolicitudVacaciones

        Try
            s = _dal.Buscar(numero)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo leer la solicitud: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try

        If s Is Nothing Then
            XtraMessageBox.Show("Esta solicitud no existe o fue eliminada.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        _cargando = True
        Try
            ChkNomina.Checked = False
            CargarEmpleado(s.Codigo_Empleado)
            _cargada = s
            _diasGuardados = If(CsVacacionesDAL.DescuentaVacaciones(s.Tipo_Solicitud) AndAlso s.Fecha_Inicio.Date <= Date.Today, s.Dias_Disfrutar, 0)
            TxtNumero.Text = s.Numero_Solicitud
            DtpFechaSolicitud.DateTime = s.Fecha_Solicitud
            CmbTipoSolicitud.EditValue = s.Tipo_Solicitud
            CmbClasificacion.EditValue = If(s.Clasificacion = "", Nothing, s.Clasificacion)
            DtpFechaInicio.DateTime = s.Fecha_Inicio.Date
            DtpFechaFin.DateTime = s.Fecha_Fin.Date
            TxtDiasSolicitud.EditValue = CDec(s.Dias_Solicitud)
            TxtHorasSolicitud.EditValue = CDec(s.Horas_Solicitud)
            TxtObservaciones.Text = s.Observaciones
        Finally
            _cargando = False
        End Try

        AjustarDias()
        MostrarTotales()
        ActualizarBotones()

        If s.Num_Nomina <> 0 Then
            XtraMessageBox.Show("La solicitud " & s.Numero_Solicitud & " ya fue procesada en la nómina " & s.Num_Nomina & ": solo se puede consultar o imprimir.",
                                Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        Return True

    End Function

    Private Sub MostrarSolicitudes(codigo1 As String)

        GridSolicitudes.DataSource = _dal.ListarPorEmpleado(codigo1)
        VistaSolicitudes.PopulateColumns()

        Dim titulos As New Dictionary(Of String, String) From {
            {"NumeroSolicitud", "Número"}, {"FechaSolicitud", "Fecha solicitud"}, {"TipoSolicitud", "Tipo"},
            {"Clasificacion", "Clasificación"}, {"FechaInicio", "Inicio"}, {"FechaFin", "Fin"},
            {"DiasDisfrutar", "Días"}, {"Pagado", "Pagado"}, {"NumNomina", "Nómina"}}

        For Each c As DevExpress.XtraGrid.Columns.GridColumn In VistaSolicitudes.Columns
            If titulos.ContainsKey(c.FieldName) Then c.Caption = titulos(c.FieldName)
            If c.FieldName.StartsWith("Fecha") Then
                c.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
                c.DisplayFormat.FormatString = "dd/MM/yyyy"
            ElseIf c.FieldName = "DiasDisfrutar" Then
                c.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                c.DisplayFormat.FormatString = "n2"
            End If
        Next

    End Sub

#End Region

#Region "EVENTOS"

    Private Sub CmbEmpleado_EditValueChanged(sender As Object, e As EventArgs)
        If _cargando OrElse CmbEmpleado.EditValue Is Nothing Then Return
        CargarEmpleado(Convert.ToString(CmbEmpleado.EditValue))
    End Sub

    ''' <summary>"Toda la nómina": la solicitud se graba para cada empleado activo de la nómina elegida (V.5.0: código = nombre de nómina).</summary>
    Private Sub ChkNomina_CheckedChanged(sender As Object, e As EventArgs)
        If ChkNomina.Checked AndAlso Not _cargando Then
            _cargando = True
            Try
                _cargada = Nothing
                _empleado = Nothing
                _resumen = Nothing
                _diasGuardados = 0
                CmbEmpleado.EditValue = Nothing
                For Each t As TextEdit In {TxtNombre, TxtDepartamento, TxtCargo, TxtFechaIngreso}
                    t.Text = ""
                Next
                GridSolicitudes.DataSource = Nothing
                TxtNumero.Text = _dal.ProximoNumero()
            Finally
                _cargando = False
            End Try
            MostrarTotales()
        End If
        ActualizarBotones()
    End Sub

    ''' <summary>La clasificación sigue al tipo de solicitud (V.5.0 CmbTipoSolicitud_Click).</summary>
    Private Sub CmbTipoSolicitud_EditValueChanged(sender As Object, e As EventArgs)

        If _cargando Then Return

        Select Case Convert.ToString(CmbTipoSolicitud.EditValue)
            Case "Vacaciones", "Vacaciones Pagadas", "Vacaciones Programadas", "Feriado", "Permiso Programado", "Suspension"
                CmbClasificacion.EditValue = CmbTipoSolicitud.EditValue
            Case "Ausente", "AusenteBasico", "AusenteBasicoExcento", "AusenteBasicoDINSS"
                CmbClasificacion.EditValue = "Ausente"
            Case Else
                CmbClasificacion.EditValue = "Accidente Comun"
        End Select

        MostrarTotales()

    End Sub

    ''' <summary>Los días se recalculan con el rango de fechas (fin - inicio + 1).</summary>
    Private Sub Fechas_EditValueChanged(sender As Object, e As EventArgs)
        If _cargando Then Return
        _cargando = True
        Try
            TxtDiasSolicitud.EditValue = CDec(Math.Max(1, (DtpFechaFin.DateTime.Date - DtpFechaInicio.DateTime.Date).Days + 1))
        Finally
            _cargando = False
        End Try
        AjustarDias()
        MostrarTotales()
    End Sub

    Private Sub TxtDiasSolicitud_EditValueChanged(sender As Object, e As EventArgs)
        If _cargando Then Return
        AjustarDias()
        MostrarTotales()
    End Sub

    ''' <summary>
    ''' Un solo día: los días quedan fijos y se pueden pedir horas. Varios días: se pueden rebajar los días (p. ej. sin
    ''' fines de semana) y las horas quedan en 0 (V.5.0 DtpFechaInicio_Change / TxtDiasSolicitud_Change).
    ''' </summary>
    Private Sub AjustarDias()
        Dim variosDias As Boolean = (DtpFechaFin.DateTime.Date - DtpFechaInicio.DateTime.Date).Days >= 1
        TxtDiasSolicitud.Properties.ReadOnly = Not variosDias
        Dim porHoras As Boolean = Valor(TxtDiasSolicitud) <= 1
        TxtHorasSolicitud.Enabled = porHoras
        If Not porHoras AndAlso Valor(TxtHorasSolicitud) <> 0 Then
            Dim previo As Boolean = _cargando
            _cargando = True
            TxtHorasSolicitud.EditValue = 0D
            _cargando = previo
        End If
    End Sub

    ''' <summary>
    ''' Días acumulados, disfrutados (incluida esta solicitud) y disponibles. Ausente y Suspensión no muestran los días
    ''' (V.5.0 CmbTipoSolicitud_LostFocus); los tipos que no descuentan vacaciones no suman a los disfrutados.
    ''' </summary>
    Private Sub MostrarTotales()

        Dim tipo As String = Convert.ToString(CmbTipoSolicitud.EditValue)
        Dim visibles As Boolean = tipo <> "Ausente" AndAlso tipo <> "Suspension"

        For Each c As Control In New Control() {LblDiasVacaciones, TxtDiasVacaciones, LblDisfrutados, TxtDisfrutados, LblDisponibles, TxtDisponibles}
            c.Visible = visibles
        Next

        If _resumen Is Nothing Then
            _disfrutados = 0
            _disponibles = 0
            TxtDiasVacaciones.Text = ""
            TxtDisfrutados.Text = ""
            TxtDisponibles.Text = ""
            Return
        End If

        Dim actual As Double = If(CsVacacionesDAL.DescuentaVacaciones(tipo),
                                  CsSolicitudDAL.DiasDisfrutar(LeerSolicitud(), If(_empleado Is Nothing, 0, _empleado.Horas_Dia)), 0)

        _disfrutados = _resumen.Solicitado - _diasGuardados + actual
        _disponibles = _resumen.Acumulado - _disfrutados

        TxtDiasVacaciones.Text = _resumen.Acumulado.ToString("#,##0.00")
        TxtDisfrutados.Text = _disfrutados.ToString("#,##0.00")
        TxtDisponibles.Text = _disponibles.ToString("#,##0.00")
        TxtDisponibles.Properties.Appearance.ForeColor = If(_disponibles < 0, Color.Red, Color.Empty)

    End Sub

    Private Sub ActualizarBotones()
        Dim procesada As Boolean = _cargada IsNot Nothing AndAlso _cargada.Num_Nomina <> 0
        CmbEmpleado.Enabled = Not ChkNomina.Checked
        CmbNomina.Visible = ChkNomina.Checked
        BtnGrabar.Enabled = Not procesada
        BtnAnular.Enabled = _cargada IsNot Nothing AndAlso Not procesada
        BtnImprimir.Enabled = _empleado IsNot Nothing AndAlso Not ChkNomina.Checked
        BtnSeleccionar.Enabled = VistaSolicitudes.RowCount > 0
    End Sub

#End Region

#Region "ACCIONES"

    Private Sub SeleccionarDelGrid()
        If VistaSolicitudes.FocusedRowHandle < 0 Then Return
        Dim numero As Object = VistaSolicitudes.GetFocusedRowCellValue("NumeroSolicitud")
        If numero IsNot Nothing Then CargarSolicitud(Convert.ToString(numero))
    End Sub

    ''' <summary>Abre una solicitud por su número (V.5.0 PushButton1 + FrmConsulta "Solicitud").</summary>
    Private Sub BtnBuscar_Click(sender As Object, e As EventArgs)

        Dim elegida As ResultadoBusqueda = FrmConsulta.Elegir(TipoBusqueda.Solicitud, Me)

        If elegida Is Nothing Then Return

        CargarSolicitud(elegida.Codigo)

    End Sub

    ''' <summary>Autorizar (V.5.0 CmdGrabar): crea o modifica la solicitud, o la crea para toda la nómina.</summary>
    Private Sub BtnGrabar_Click(sender As Object, e As EventArgs)

        Dim sol As SolicitudVacaciones = LeerSolicitud()
        sol.Dias_Vacaciones = If(_resumen Is Nothing, 0, _resumen.Acumulado)
        sol.Dias_Disfrutados = _disfrutados

        Try
            Cursor = Cursors.WaitCursor

            If ChkNomina.Checked Then

                Dim nomina As String = Convert.ToString(CmbNomina.EditValue)
                If nomina = "" Then
                    XtraMessageBox.Show("Elija la nómina.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
                If XtraMessageBox.Show("¿Crear la solicitud " & sol.Tipo_Solicitud & " para todos los empleados activos de la nómina " & nomina & "?",
                                       Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

                sol.Numero_Solicitud = ""
                sol.Dias_Vacaciones = 0
                sol.Dias_Disfrutados = 0
                Dim total As Integer = _dal.GuardarParaNomina(sol, nomina)
                Cursor = Cursors.Default
                XtraMessageBox.Show(total & " solicitudes grabadas.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Limpiar()

            Else

                If _empleado Is Nothing Then
                    XtraMessageBox.Show("Tiene que elegir un empleado.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                ' Una solicitud nueva toma el número al grabar (otro usuario pudo usar el que se mostraba).
                If _cargada Is Nothing Then sol.Numero_Solicitud = ""
                Dim numero As String = _dal.Guardar(sol)
                Cursor = Cursors.Default
                XtraMessageBox.Show("Solicitud " & numero & " grabada.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
                CargarSolicitud(numero)

            End If

        Catch ex As InvalidOperationException
            Cursor = Cursors.Default
            XtraMessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Catch ex As Exception
            Cursor = Cursors.Default
            XtraMessageBox.Show("No se pudo grabar la solicitud: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ''' <summary>Anular (V.5.0 CmdAnular): elimina la solicitud abierta y sus días.</summary>
    Private Sub BtnAnular_Click(sender As Object, e As EventArgs)

        If _cargada Is Nothing Then Return

        If XtraMessageBox.Show("¿Desea eliminar la solicitud " & _cargada.Numero_Solicitud & "?", Text,
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Try
            _dal.Eliminar(_cargada.Numero_Solicitud)
        Catch ex As InvalidOperationException
            XtraMessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo eliminar la solicitud: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Limpiar()

    End Sub

    ''' <summary>
    ''' Imprimir (V.5.0 <c>cmdborrar_Click</c>) con los datos de la pantalla: con más de 3 días la constancia larga
    ''' <see cref="RptSolicitudVacaLargo"/>; con 3 o menos el formato corto <see cref="RptSolicitudVacaCorto"/>.
    ''' </summary>
    Private Sub BtnImprimir_Click(sender As Object, e As EventArgs)

        If _empleado Is Nothing Then
            XtraMessageBox.Show("Tiene que elegir un empleado.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim sol As SolicitudVacaciones = LeerSolicitud()

        Try
            Cursor = Cursors.WaitCursor
            Dim empresa As DatosEmpresa = New CsControlesDAL().ObtenerDatosEmpresa()
            Dim acumulado As Double = If(_resumen Is Nothing, 0, _resumen.Acumulado)
            Dim rpt As DevExpress.XtraReports.UI.XtraReport =
                If(sol.Dias_Solicitud > DiasFormatoLargo,
                   CType(New RptSolicitudVacaLargo(empresa, sol, _empleado, acumulado, _disfrutados, _disponibles), DevExpress.XtraReports.UI.XtraReport),
                   New RptSolicitudVacaCorto(empresa, sol, _empleado, acumulado, _disfrutados, _disponibles))
            Cursor = Cursors.Default
            FrmVisorReporte.Mostrar(rpt, "Solicitud vacaciones " & sol.Numero_Solicitud & " " & _empleado.Codigo_Empleado1, Me)
        Catch ex As Exception
            Cursor = Cursors.Default
            XtraMessageBox.Show("No se pudo generar el reporte: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    ''' <summary>Pasa los datos de la pantalla a la entidad (V.5.0 LeerSolicitud).</summary>
    Private Function LeerSolicitud() As SolicitudVacaciones
        Return New SolicitudVacaciones With {
            .Numero_Solicitud = TxtNumero.Text.Trim,
            .Codigo_Empleado = If(_empleado Is Nothing, Convert.ToString(CmbEmpleado.EditValue), _empleado.Codigo_Empleado1),
            .Tipo_Solicitud = Convert.ToString(CmbTipoSolicitud.EditValue),
            .Clasificacion = Convert.ToString(CmbClasificacion.EditValue),
            .Fecha_Solicitud = DtpFechaSolicitud.DateTime.Date,
            .Fecha_Inicio = DtpFechaInicio.DateTime.Date,
            .Fecha_Fin = DtpFechaFin.DateTime.Date,
            .Dias_Solicitud = Valor(TxtDiasSolicitud),
            .Horas_Solicitud = If(TxtHorasSolicitud.Enabled, Valor(TxtHorasSolicitud), 0),
            .Observaciones = TxtObservaciones.Text}
    End Function

    Private Shared Function Valor(t As TextEdit) As Double
        If t.EditValue Is Nothing OrElse IsDBNull(t.EditValue) Then Return 0
        Dim d As Double
        Return If(Double.TryParse(Convert.ToString(t.EditValue), d), d, 0)
    End Function

#End Region

End Class
