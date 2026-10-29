Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraEditors.Mask
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Despidos y renuncias (V.5.0 FrmBajas): carga el empleado, calcula y graba su liquidación, muestra el reporte
''' (vista previa con exportación a PDF/Excel) y procesa la baja. Solo UI: todo pasa por <see cref="CsLiquidacion"/>.
''' <see cref="CargarEmpleado"/> es público para abrir la pantalla con un empleado desde otros formularios.
''' La interfaz se arma por código (no hay Designer).
''' </summary>
Public Class FrmBajas
    Inherits XtraForm

    Private _svc As CsLiquidacion
    Private _estado As CsLiquidacion.Estado
    Private _cargando As Boolean

    ' Empleado
    Private CmbEmpleado As SearchLookUpEdit
    Private TxtNombre, TxtCargo, TxtDepartamento, TxtSexo, TxtDireccion As TextEdit
    ' Tiempo laborado
    Private TxtFechaContrato As TextEdit
    Private DtpEgreso, DtpHistorial, DtpIniVaca, DtpIniAgui As DateEdit
    Private TxtAnnos, TxtMeses, TxtDiasTrabajados, TxtDiasPlanilla As TextEdit
    ' Salario
    Private TxtSalarioBasico, TxtTarifa, TxtSalarioPromedio, TxtSalarioAlto, TxtAntiguedad, TxtDiasVaca As TextEdit
    Private LblTipoSalario As LabelControl
    Private ChkPromedioReal, ChkSueldoActual, ChkAntiguedadBase, ChkPagarDias As CheckEdit
    ' Tipo de baja
    Private RdgTipo As RadioGroup
    Private TxtMotivo As MemoEdit
    ' Prestaciones
    Private ChkIncentivos, ChkOtroPlanilla, Chk13Mes, ChkVaca, ChkAntigue, ChkExtra, ChkViaticos, ChkOtro As CheckEdit
    Private TxtDescuentoDias, TxtConceptoOtro, TxtMontoOtro As TextEdit
    Private LblDescuento, LblConceptoOtro, LblMontoOtro As LabelControl
    ' Deducciones
    Private ChkIr, ChkPrestamo, ChkDeducciones, ChkOtrasDed As CheckEdit
    Private TxtConceptoOtrasDed, TxtMontoOtrasDed As TextEdit
    Private LblConceptoOtrasDed, LblMontoOtrasDed As LabelControl
    ' Historial y botones
    Private GridSalarios As GridControl
    Private VistaSalarios As GridView
    Private BtnCalcular, BtnImprimir, BtnProcesar, BtnCerrar As SimpleButton

    Public Sub New()

        Text = "Despidos y Renuncias"
        ClientSize = New Size(1110, 660)
        StartPosition = FormStartPosition.CenterParent

        CrearInterfaz()

        AddHandler Load, AddressOf FrmBajas_Load

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        Dim gEmpleado As GroupControl = Grupo("Empleado", 8, 8, 545, 150)
        CmbEmpleado = New SearchLookUpEdit With {.Location = New Point(105, 30), .Size = New Size(150, 20)}
        CmbEmpleado.Properties.DisplayMember = "CodEmpleado1"
        CmbEmpleado.Properties.ValueMember = "CodEmpleado1"
        CmbEmpleado.Properties.NullText = ""
        CmbEmpleado.Properties.PopupFormSize = New Size(450, 350)
        AddHandler CmbEmpleado.EditValueChanged, AddressOf CmbEmpleado_EditValueChanged
        gEmpleado.Controls.Add(Etiqueta("Código:", 10, 33))
        gEmpleado.Controls.Add(CmbEmpleado)
        TxtNombre = Lectura(gEmpleado, "Nombre:", 10, 58, 105, 425)
        TxtCargo = Lectura(gEmpleado, "Cargo:", 10, 83, 105, 175)
        TxtDepartamento = Lectura(gEmpleado, "Departamento:", 290, 83, 375, 155)
        TxtSexo = Lectura(gEmpleado, "Sexo:", 270, 33, 310, 60)
        TxtDireccion = Lectura(gEmpleado, "Dirección:", 10, 108, 105, 425)

        Dim gTiempo As GroupControl = Grupo("Tiempo laborado", 8, 162, 545, 135)
        TxtFechaContrato = Lectura(gTiempo, "Fecha ingreso:", 10, 30, 105, 100)
        DtpEgreso = Fecha(gTiempo, "Fecha egreso:", 215, 30, 300)
        gTiempo.Controls.Add(Etiqueta("Fecha historial:", 420, 33))
        DtpHistorial = New DateEdit With {.Location = New Point(420, 51), .Size = New Size(115, 20), .DateTime = Date.Today}
        gTiempo.Controls.Add(DtpHistorial)
        TxtAnnos = Lectura(gTiempo, "Años trabajados:", 10, 56, 105, 100)
        TxtMeses = Lectura(gTiempo, "Meses:", 215, 56, 300, 100)
        TxtDiasTrabajados = Numero(gTiempo, "Días trabajados:", 10, 82, 105, 100, "n0")
        TxtDiasPlanilla = Numero(gTiempo, "Días últ. nómina:", 215, 82, 300, 100, "n0")
        DtpIniVaca = Fecha(gTiempo, "Inicio vacaciones:", 10, 108, 105)
        DtpIniAgui = Fecha(gTiempo, "Inicio aguinaldo:", 215, 108, 300)
        AddHandler DtpEgreso.EditValueChanged, AddressOf DtpEgreso_EditValueChanged
        AddHandler DtpHistorial.EditValueChanged, AddressOf DtpHistorial_EditValueChanged

        Dim gSalario As GroupControl = Grupo("Cálculos básicos del salario", 8, 301, 545, 160)
        TxtSalarioBasico = Numero(gSalario, "Salario mínimo:", 10, 30, 105, 100)
        TxtTarifa = Lectura(gSalario, "Tarifa:", 215, 30, 300, 100)
        TxtSalarioPromedio = Numero(gSalario, "Salario promedio:", 10, 56, 105, 100)
        TxtSalarioAlto = Numero(gSalario, "Salario más alto:", 215, 56, 300, 100)
        TxtAntiguedad = Lectura(gSalario, "% antigüedad:", 10, 82, 105, 100)
        TxtDiasVaca = Numero(gSalario, "Días vacaciones:", 215, 82, 300, 100)
        TxtDiasVaca.Properties.NullValuePrompt = "calculados"
        TxtDiasVaca.EditValue = Nothing
        LblTipoSalario = New LabelControl With {.Location = New Point(410, 33), .Text = "", .AutoSizeMode = LabelAutoSizeMode.None, .Size = New Size(130, 30)}
        LblTipoSalario.Appearance.ForeColor = Color.DarkBlue
        LblTipoSalario.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
        gSalario.Controls.Add(LblTipoSalario)
        ChkPromedioReal = Casilla(gSalario, "Salario prom. diferencia fecha", 10, 110, False, 250)
        ChkSueldoActual = Casilla(gSalario, "Sueldo actual -> básico en liquidación", 270, 110, False, 260)
        ChkAntiguedadBase = Casilla(gSalario, "Antigüedad base para cálculo", 10, 133, False, 250)
        ChkPagarDias = Casilla(gSalario, "Pagar días trabajados en planilla", 270, 133, True, 260)
        AddHandler ChkAntiguedadBase.CheckedChanged, AddressOf Recargar_Opcion

        Dim gTipo As GroupControl = Grupo("Tipo de baja", 8, 465, 545, 140)
        RdgTipo = New RadioGroup With {.Location = New Point(10, 28), .Size = New Size(525, 28)}
        RdgTipo.Properties.Columns = 3
        RdgTipo.Properties.Items.AddRange(New RadioGroupItem() {
            New RadioGroupItem("Despido", "Despido"), New RadioGroupItem("Renuncia", "Renuncia"), New RadioGroupItem("Fin", "Final. contrato")})
        RdgTipo.EditValue = "Despido"
        TxtMotivo = New MemoEdit With {.Location = New Point(105, 62), .Size = New Size(430, 70)}
        TxtMotivo.Properties.MaxLength = 255
        gTipo.Controls.Add(RdgTipo)
        gTipo.Controls.Add(Etiqueta("Observaciones:", 10, 64))
        gTipo.Controls.Add(TxtMotivo)

        Dim gPrest As GroupControl = Grupo("Prestaciones", 561, 8, 541, 150)
        ChkIncentivos = Casilla(gPrest, "Incentivos", 10, 28, True)
        ChkOtroPlanilla = Casilla(gPrest, "Otros ingresos planilla", 10, 51, False)
        Chk13Mes = Casilla(gPrest, "13vo mes", 10, 74, True)
        ChkVaca = Casilla(gPrest, "Vacaciones", 10, 97, True)
        ChkAntigue = Casilla(gPrest, "Antigüedad", 180, 28, True)
        ChkExtra = Casilla(gPrest, "Horas extra", 180, 51, False)
        ChkViaticos = Casilla(gPrest, "Viáticos", 180, 74, False)
        ChkOtro = Casilla(gPrest, "Otros ingresos", 180, 97, False)
        LblDescuento = Etiqueta("Días a descontar:", 10, 124)
        TxtDescuentoDias = New TextEdit With {.Location = New Point(110, 121), .Size = New Size(60, 20)}
        Mascara(TxtDescuentoDias, "n2")
        gPrest.Controls.AddRange(New Control() {LblDescuento, TxtDescuentoDias})
        LblConceptoOtro = Etiqueta("Concepto:", 345, 31)
        TxtConceptoOtro = New TextEdit With {.Location = New Point(400, 28), .Size = New Size(130, 20)}
        TxtConceptoOtro.Properties.MaxLength = 25
        LblMontoOtro = Etiqueta("Monto:", 345, 57)
        TxtMontoOtro = New TextEdit With {.Location = New Point(400, 54), .Size = New Size(130, 20)}
        Mascara(TxtMontoOtro, "n2")
        gPrest.Controls.AddRange(New Control() {LblConceptoOtro, TxtConceptoOtro, LblMontoOtro, TxtMontoOtro})
        AddHandler ChkVaca.CheckedChanged, Sub(s, e) MostrarOpcionales()
        AddHandler ChkOtro.CheckedChanged, Sub(s, e) MostrarOpcionales()
        AddHandler ChkIncentivos.CheckedChanged, AddressOf Recargar_Opcion
        AddHandler ChkExtra.CheckedChanged, AddressOf Recargar_Opcion

        Dim gDed As GroupControl = Grupo("Deducciones", 561, 162, 541, 105)
        ChkIr = Casilla(gDed, "IR", 10, 28, True)
        ChkPrestamo = Casilla(gDed, "Préstamos", 10, 51, False)
        ChkDeducciones = Casilla(gDed, "Deducciones", 10, 74, False)
        ChkOtrasDed = Casilla(gDed, "Otras deducciones", 180, 28, False)
        LblConceptoOtrasDed = Etiqueta("Concepto:", 345, 31)
        TxtConceptoOtrasDed = New TextEdit With {.Location = New Point(400, 28), .Size = New Size(130, 20)}
        TxtConceptoOtrasDed.Properties.MaxLength = 50
        LblMontoOtrasDed = Etiqueta("Monto:", 345, 57)
        TxtMontoOtrasDed = New TextEdit With {.Location = New Point(400, 54), .Size = New Size(130, 20)}
        Mascara(TxtMontoOtrasDed, "n2")
        gDed.Controls.AddRange(New Control() {LblConceptoOtrasDed, TxtConceptoOtrasDed, LblMontoOtrasDed, TxtMontoOtrasDed})
        AddHandler ChkOtrasDed.CheckedChanged, Sub(s, e) MostrarOpcionales()

        Dim gHist As GroupControl = Grupo("Salarios de los últimos 6 meses", 561, 271, 541, 334)
        GridSalarios = New GridControl With {.Dock = DockStyle.Fill}
        VistaSalarios = New GridView(GridSalarios)
        GridSalarios.MainView = VistaSalarios
        GridSalarios.ViewCollection.Add(VistaSalarios)
        VistaSalarios.OptionsBehavior.Editable = False
        VistaSalarios.OptionsView.ShowGroupPanel = False
        VistaSalarios.OptionsView.ShowFooter = True
        VistaSalarios.OptionsView.ColumnAutoWidth = True
        gHist.Controls.Add(GridSalarios)

        BtnCalcular = Boton("Calcular", "CalculateNow", 8)
        BtnImprimir = Boton("Imprimir", "Print", 109)
        BtnProcesar = Boton("Procesar", "Apply", 210)
        BtnCerrar = Boton("Cerrar", "Close", 1007)
        AddHandler BtnCalcular.Click, AddressOf BtnCalcular_Click
        AddHandler BtnImprimir.Click, Sub(s, e) MostrarReporte()
        AddHandler BtnProcesar.Click, AddressOf BtnProcesar_Click
        AddHandler BtnCerrar.Click, Sub(s, e) Close()

        Controls.AddRange(New Control() {gEmpleado, gTiempo, gSalario, gTipo, gPrest, gDed, gHist, BtnCalcular, BtnImprimir, BtnProcesar, BtnCerrar})

        MostrarOpcionales()

    End Sub

    Private Function Grupo(titulo As String, x As Integer, y As Integer, ancho As Integer, alto As Integer) As GroupControl
        Return New GroupControl With {.Text = titulo, .Location = New Point(x, y), .Size = New Size(ancho, alto)}
    End Function

    Private Shared Function Etiqueta(texto As String, x As Integer, y As Integer) As LabelControl
        Return New LabelControl With {.Text = texto, .Location = New Point(x, y)}
    End Function

    Private Shared Function Lectura(g As GroupControl, texto As String, xEt As Integer, y As Integer, x As Integer, ancho As Integer) As TextEdit
        Dim t As New TextEdit With {.Location = New Point(x, y - 3), .Size = New Size(ancho, 20)}
        t.Properties.ReadOnly = True
        g.Controls.Add(Etiqueta(texto, xEt, y))
        g.Controls.Add(t)
        Return t
    End Function

    Private Shared Function Numero(g As GroupControl, texto As String, xEt As Integer, y As Integer, x As Integer, ancho As Integer,
                                   Optional formato As String = "n2") As TextEdit
        Dim t As New TextEdit With {.Location = New Point(x, y - 3), .Size = New Size(ancho, 20)}
        Mascara(t, formato)
        g.Controls.Add(Etiqueta(texto, xEt, y))
        g.Controls.Add(t)
        Return t
    End Function

    Private Shared Sub Mascara(t As TextEdit, formato As String)
        t.Properties.Mask.MaskType = MaskType.Numeric
        t.Properties.Mask.EditMask = formato
        t.Properties.Mask.UseMaskAsDisplayFormat = True
    End Sub

    Private Shared Function Fecha(g As GroupControl, texto As String, xEt As Integer, y As Integer, x As Integer) As DateEdit
        Dim d As New DateEdit With {.Location = New Point(x, y - 3), .Size = New Size(100, 20), .DateTime = Date.Today}
        g.Controls.Add(Etiqueta(texto, xEt, y))
        g.Controls.Add(d)
        Return d
    End Function

    Private Shared Function Casilla(g As GroupControl, texto As String, x As Integer, y As Integer, marcada As Boolean,
                                    Optional ancho As Integer = 160) As CheckEdit
        Dim c As New CheckEdit With {.Text = texto, .Location = New Point(x, y), .Size = New Size(ancho, 20), .Checked = marcada}
        g.Controls.Add(c)
        Return c
    End Function

    Private Function Boton(texto As String, icono As String, x As Integer) As SimpleButton
        Dim b As New SimpleButton With {.Text = texto, .Location = New Point(x, 615), .Size = New Size(95, 35)}
        b.ImageOptions.ImageUri.Uri = icono
        Return b
    End Function

    ''' <summary>Los campos de días a descontar, otros ingresos y otras deducciones solo se ven con su casilla marcada.</summary>
    Private Sub MostrarOpcionales()
        LblDescuento.Visible = ChkVaca.Checked
        TxtDescuentoDias.Visible = ChkVaca.Checked
        For Each c As Control In New Control() {LblConceptoOtro, TxtConceptoOtro, LblMontoOtro, TxtMontoOtro}
            c.Visible = ChkOtro.Checked
        Next
        For Each c As Control In New Control() {LblConceptoOtrasDed, TxtConceptoOtrasDed, LblMontoOtrasDed, TxtMontoOtrasDed}
            c.Visible = ChkOtrasDed.Checked
        Next
    End Sub

#End Region

#Region "CARGA"

    Private Sub FrmBajas_Load(sender As Object, e As EventArgs)

        Try
            _svc = New CsLiquidacion()
            ChkPromedioReal.Checked = _svc.Salario_Promedio_Real
            ChkExtra.Text = If(_svc.Es_Formato_Bono, "Horas extra (en salario)", "Horas extra")
            RecargarEmpleados()
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo abrir la pantalla de bajas: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Limpiar()

    End Sub

    ''' <summary>Vuelve a leer los empleados activos no liquidados del buscador.</summary>
    Public Sub RecargarEmpleados()
        CmbEmpleado.Properties.DataSource = _svc.ListarEmpleados()
        CmbEmpleado.Properties.View.Columns.Clear()
        CmbEmpleado.Properties.View.PopulateColumns(CmbEmpleado.Properties.DataSource)
    End Sub

    ''' <summary>Carga un empleado por su código visible (también lo usan otros formularios).</summary>
    Public Sub CargarEmpleado(codigo1 As String)

        If _svc Is Nothing Then Return

        _cargando = True
        Try
            Cursor = Cursors.WaitCursor
            If Not Equals(CmbEmpleado.EditValue, codigo1) Then CmbEmpleado.EditValue = codigo1
            _estado = _svc.CargarEmpleado(codigo1, ChkIncentivos.Checked, ChkAntiguedadBase.Checked, ChkExtra.Checked, ChkPromedioReal.Checked)
            MostrarEmpleado()
            MostrarEstado()
        Catch ex As InvalidOperationException
            XtraMessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Limpiar()
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo cargar el empleado: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Limpiar()
        Finally
            Cursor = Cursors.Default
            _cargando = False
        End Try

    End Sub

    Private Sub MostrarEmpleado()
        Dim emp As EmpleadoBaja = _svc.Empleado
        TxtNombre.Text = emp.Nombre_Completo
        TxtCargo.Text = emp.Cargo
        TxtDepartamento.Text = emp.Departamento
        TxtSexo.Text = emp.Sexo
        TxtDireccion.Text = emp.Direccion
        TxtFechaContrato.Text = emp.Fecha_Contrato.Value.ToString("dd/MM/yyyy")
        Text = "Despidos y Renuncias - " & emp.Cod_Empleado1 & " " & emp.Nombre_Completo
    End Sub

    ''' <summary>Pasa a la pantalla los valores calculados por el servicio (sin disparar los recálculos).</summary>
    Private Sub MostrarEstado()

        Dim previo As Boolean = _cargando
        _cargando = True

        DtpEgreso.DateTime = _estado.Fecha_Egreso
        DtpIniVaca.DateTime = _estado.Fecha_Ini_Vacaciones
        DtpIniAgui.DateTime = _estado.Fecha_Ini_Aguinaldo
        TxtAnnos.Text = _estado.Annos.ToString("n2")
        TxtMeses.Text = _estado.Meses_Texto
        TxtDiasTrabajados.EditValue = _estado.Dias_Trabajados
        TxtDiasPlanilla.EditValue = _estado.Dias_Planilla
        TxtSalarioBasico.EditValue = Math.Round(_estado.Salario_Basico, 2)
        TxtTarifa.Text = _estado.Tarifa.ToString("n6")
        TxtSalarioPromedio.EditValue = Math.Round(_estado.Salario_Promedio, 2)
        TxtSalarioAlto.EditValue = Math.Round(_estado.Salario_Alto, 2)
        TxtAntiguedad.Text = _estado.Porcentaje_Antiguedad.ToString("n2")
        ChkSueldoActual.Checked = _estado.Sueldo_Actual_Basico
        LblTipoSalario.Text = If(_estado.Salario_Fijo, "Empleado con salario fijo", "Empleado con salario variable")

        MostrarSalarios()
        HabilitarAcciones(False)

        _cargando = previo

    End Sub

    Private Sub MostrarSalarios()

        GridSalarios.DataSource = _svc.Salarios.Select(Function(s) New With {
            .Mes = s.Periodo, .Basico = s.Salario_Basico, .Septimo = s.Septimo, .Produccion = s.Destajo + s.Bono_Produccion,
            .Reembolso = s.Reembolso, .Incentivos = s.Incentivos, .Otros = s.Otros, .Total = s.Total_Ingresos}).ToList()
        VistaSalarios.PopulateColumns()

        For Each col As DevExpress.XtraGrid.Columns.GridColumn In VistaSalarios.Columns
            If col.FieldName = "Mes" Then Continue For
            col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
            col.DisplayFormat.FormatString = "n2"
            col.Summary.Clear()
            col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n2}")
        Next
        If VistaSalarios.Columns("Basico") IsNot Nothing Then VistaSalarios.Columns("Basico").Caption = "Salario Básico"
        If VistaSalarios.Columns("Septimo") IsNot Nothing Then VistaSalarios.Columns("Septimo").Caption = "Séptimo"
        If VistaSalarios.Columns("Produccion") IsNot Nothing Then VistaSalarios.Columns("Produccion").Caption = "Producción"

    End Sub

    Private Sub Limpiar()

        _cargando = True
        _estado = Nothing
        For Each t As TextEdit In New TextEdit() {TxtNombre, TxtCargo, TxtDepartamento, TxtSexo, TxtDireccion, TxtFechaContrato, TxtAnnos, TxtMeses, TxtTarifa, TxtAntiguedad}
            t.Text = ""
        Next
        For Each t As TextEdit In New TextEdit() {TxtDiasTrabajados, TxtDiasPlanilla, TxtSalarioBasico, TxtSalarioPromedio, TxtSalarioAlto, TxtDiasVaca}
            t.EditValue = Nothing
        Next
        DtpEgreso.DateTime = Date.Today
        DtpHistorial.DateTime = Date.Today
        LblTipoSalario.Text = "No se ha definido el empleado"
        GridSalarios.DataSource = Nothing
        Text = "Despidos y Renuncias"
        HabilitarAcciones(False)
        BtnCalcular.Enabled = False
        _cargando = False

    End Sub

    Private Sub HabilitarAcciones(calculada As Boolean)
        BtnCalcular.Enabled = _estado IsNot Nothing
        BtnImprimir.Enabled = calculada
        BtnProcesar.Enabled = calculada
    End Sub

    Private Sub CmbEmpleado_EditValueChanged(sender As Object, e As EventArgs)
        If _cargando Then Return
        Dim codigo As String = If(CmbEmpleado.EditValue, "").ToString()
        If codigo = "" Then
            Limpiar()
        Else
            CargarEmpleado(codigo)
        End If
    End Sub

    ''' <summary>Incentivos, horas extra y antigüedad base cambian el historial de salarios: se vuelve a cargar el empleado.</summary>
    Private Sub Recargar_Opcion(sender As Object, e As EventArgs)
        If _cargando OrElse _svc?.Empleado Is Nothing Then Return
        CargarEmpleado(_svc.Empleado.Cod_Empleado1)
    End Sub

    Private Sub DtpEgreso_EditValueChanged(sender As Object, e As EventArgs)
        If _cargando OrElse _estado Is Nothing OrElse DtpEgreso.EditValue Is Nothing Then Return
        Try
            _svc.CambiarFechaEgreso(DtpEgreso.DateTime, _estado)
            MostrarEstado()
        Catch ex As Exception
            XtraMessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub DtpHistorial_EditValueChanged(sender As Object, e As EventArgs)
        If _cargando OrElse _estado Is Nothing OrElse DtpHistorial.EditValue Is Nothing Then Return
        Try
            _estado.Fecha_Ini_Vacaciones = DtpIniVaca.DateTime
            _estado.Sueldo_Actual_Basico = ChkSueldoActual.Checked
            _svc.CambiarFechaHistorial(DtpHistorial.DateTime, _estado, ChkAntiguedadBase.Checked, ChkPromedioReal.Checked)
            MostrarEstado()
        Catch ex As Exception
            XtraMessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

#End Region

#Region "CALCULAR / IMPRIMIR / PROCESAR"

    Private Function LeerOpciones() As OpcionesLiquidacion

        Return New OpcionesLiquidacion With {
            .Fecha_Egreso = DtpEgreso.DateTime.Date,
            .Fecha_Ini_Vacaciones = DtpIniVaca.DateTime.Date,
            .Fecha_Ini_Aguinaldo = DtpIniAgui.DateTime.Date,
            .Salario_Basico = Valor(TxtSalarioBasico),
            .Salario_Promedio = Valor(TxtSalarioPromedio),
            .Salario_Alto = Valor(TxtSalarioAlto),
            .Dias_Planilla = Valor(TxtDiasPlanilla),
            .Dias_Trabajados = Valor(TxtDiasTrabajados),
            .Annos = _estado.Annos,
            .Meses_Texto = TxtMeses.Text,
            .Dias_Vacaciones_Manual = If(TxtDiasVaca.EditValue Is Nothing OrElse TxtDiasVaca.Text.Trim() = "", CType(Nothing, Double?), Valor(TxtDiasVaca)),
            .Dias_Descuento_Vacaciones = Valor(TxtDescuentoDias),
            .Pagar_Dias_Planilla = ChkPagarDias.Checked,
            .Antiguedad_Base = ChkAntiguedadBase.Checked,
            .Sueldo_Actual_Basico = ChkSueldoActual.Checked,
            .Incentivos = ChkIncentivos.Checked,
            .Salario_Promedio_Real = ChkPromedioReal.Checked,
            .Aguinaldo = Chk13Mes.Checked,
            .Vacaciones = ChkVaca.Checked,
            .Antiguedad = ChkAntigue.Checked,
            .Horas_Extra = ChkExtra.Checked,
            .Viaticos = ChkViaticos.Checked,
            .Otros_Ingresos_Planilla = ChkOtroPlanilla.Checked,
            .Otros_Ingresos = ChkOtro.Checked,
            .Concepto_Otros_Ingresos = TxtConceptoOtro.Text,
            .Monto_Otros_Ingresos = Valor(TxtMontoOtro),
            .Ir = ChkIr.Checked,
            .Prestamos = ChkPrestamo.Checked,
            .Deducciones = ChkDeducciones.Checked,
            .Otras_Deducciones = ChkOtrasDed.Checked,
            .Concepto_Otras_Deducciones = TxtConceptoOtrasDed.Text,
            .Monto_Otras_Deducciones = Valor(TxtMontoOtrasDed),
            .Tipo_Baja = If(RdgTipo.EditValue, "Despido").ToString(),
            .Motivo = TxtMotivo.Text}

    End Function

    Private Shared Function Valor(t As TextEdit) As Double
        If t.EditValue Is Nothing OrElse IsDBNull(t.EditValue) Then Return 0
        Dim d As Double
        Return If(Double.TryParse(t.EditValue.ToString(), d), d, 0)
    End Function

    Private Sub BtnCalcular_Click(sender As Object, e As EventArgs)

        If _estado Is Nothing Then Return

        If DtpEgreso.DateTime.Date < _svc.Empleado.Fecha_Contrato.Value.Date Then
            XtraMessageBox.Show("La fecha de egreso no puede ser anterior a la fecha de ingreso.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor
            Dim liq As Liquidacion = _svc.Calcular(LeerOpciones(), DtpHistorial.DateTime, Nombre_Usuario)
            Cursor = Cursors.Default
            HabilitarAcciones(True)

            If XtraMessageBox.Show("Liquidación calculada y grabada." & vbCrLf & vbCrLf &
                                   "Total ingresos:  " & liq.Total_Ingresos.ToString("n2") & vbCrLf &
                                   "Total egresos:   " & liq.Total_Egresos.ToString("n2") & vbCrLf &
                                   "Neto a pagar:    " & liq.Neto_Pagar.ToString("n2") & vbCrLf & vbCrLf &
                                   "¿Desea ver el reporte de la liquidación?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                MostrarReporte()
            End If
        Catch ex As InvalidOperationException
            XtraMessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo calcular la liquidación: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    ''' <summary>Vista previa del reporte de la liquidación calculada, con impresión y exportación a PDF/Excel.</summary>
    Public Sub MostrarReporte()

        If _svc?.Liquidacion Is Nothing Then Return

        Try
            Dim diasPlanilla As String = If(ChkPagarDias.Checked, "", Valor(TxtDiasPlanilla).ToString("n0") & " días")
            Using rpt As RptLiquidacion = _svc.CrearReporte(TxtMeses.Text, diasPlanilla)
                FrmVisorReporte.Mostrar(rpt, "Liquidacion " & _svc.Empleado.Cod_Empleado1, Me)
            End Using
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo mostrar el reporte: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnProcesar_Click(sender As Object, e As EventArgs)

        If _svc?.Liquidacion Is Nothing Then Return

        Dim aviso As String = If(ChkPagarDias.Checked,
            "El empleado quedará liquidado pero activo para pagarle en planilla los días trabajados.",
            "El empleado quedará inactivo y se quitará de la nómina activa.")
        If XtraMessageBox.Show("¿Desea realmente dar de baja a " & _svc.Empleado.Nombre_Completo & "?" & vbCrLf & vbCrLf & aviso,
                               Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Try
            _svc.Procesar(Nombre_Usuario)
            XtraMessageBox.Show("Se procesó con éxito.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            _cargando = True
            CmbEmpleado.EditValue = Nothing
            _cargando = False
            RecargarEmpleados()
            Limpiar()
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo procesar la baja: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

#End Region

End Class
