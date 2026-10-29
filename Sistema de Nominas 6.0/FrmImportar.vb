Imports DevExpress.XtraBars.Navigation

''' <summary>
''' Importación desde Excel (empleados, saldo de vacaciones, movimiento salarial, horas extra, deducciones, ingresos, solicitudes y
''' solicitud total), consola SQL y eliminación de movimientos pendientes. Menú lateral de secciones, cada una con su descripción y las
''' columnas que espera el archivo. Acceso a datos por CsImportacionDAL y LectorExcel; <see cref="Importar"/> y <see cref="AbrirArchivo"/>
''' quedan públicos para llamarlos desde otro formulario.
''' </summary>
Public Class FrmImportar

    Private ReadOnly _dal As New CsImportacionDAL
    Private ReadOnly _catalogos As New CsCargaMasivaDAL
    Private ReadOnly _vacaciones As New CsVacacionesDAL
    Private ReadOnly _consulta As New CsConsultaDAL

    Private _datos As DataTable
    Private ChkDolarizado As DevExpress.XtraEditors.CheckEdit
    Private ChkSueldoActual As DevExpress.XtraEditors.CheckEdit
    Private LblSaldoAl As DevExpress.XtraEditors.LabelControl
    Private DtpSaldoAl As DevExpress.XtraEditors.DateEdit
    Private ChkSegunNomina As DevExpress.XtraEditors.CheckEdit
    Private LblSolTipo As DevExpress.XtraEditors.LabelControl
    Private CmbSolTipo As DevExpress.XtraEditors.ComboBoxEdit
    Private LblSolClasif As DevExpress.XtraEditors.LabelControl
    Private CmbSolClasif As DevExpress.XtraEditors.ComboBoxEdit
    Private LblSolFecha As DevExpress.XtraEditors.LabelControl
    Private DtpSolFecha As DevExpress.XtraEditors.DateEdit
    Private CmbElimSol As DevExpress.XtraEditors.ComboBoxEdit
    Private WithEvents BtnElimSol As DevExpress.XtraEditors.SimpleButton
    Private WithEvents BtnElimEmp As DevExpress.XtraEditors.SimpleButton
    Private PnlSolEmp As DevExpress.XtraEditors.GroupControl
    Private PnlSql As DevExpress.XtraEditors.GroupControl
    Private TxtSql As DevExpress.XtraEditors.TextEdit
    Private WithEvents BtnConsultar As DevExpress.XtraEditors.SimpleButton
    Private WithEvents BtnEjecutarSql As DevExpress.XtraEditors.SimpleButton

    ''' <summary>Sección del menú. Tipo = el de <see cref="Importar"/>, -1 = consola SQL, -2 = eliminar.</summary>
    Private Class Seccion
        Public Titulo As String
        Public Descripcion As String
        Public Formato As String
        Public Tipo As Integer
    End Class

    Private ReadOnly _secciones As New List(Of Seccion) From {
        New Seccion With {.Titulo = "Empleados (catálogo)", .Tipo = 3,
            .Descripcion = "Crea o actualiza el catálogo de empleados. Los cargos, departamentos y grupos que no existan se crean.",
            .Formato = "Columnas por encabezado: Codigo, Nombre1, Nombre2, Apellido1, Apellido2, Cargo, Departamento, TipoNomina, GrupoNomina, SueldoPeriodo, FechaIngreso, RUC, INSS, Cedula… El tipo de nómina debe existir."},
        New Seccion With {.Titulo = "Saldo de vacaciones", .Tipo = 4,
            .Descripcion = "Ajusta el saldo de vacaciones del sistema al saldo del Excel registrando una solicitud de ajuste automático.",
            .Formato = "Columnas: Código del empleado, Nombre, Saldo según Excel (negativo = días a favor que se suman)."},
        New Seccion With {.Titulo = "Movimiento salarial", .Tipo = 2,
            .Descripcion = "Actualiza el salario básico y el aumento en córdobas de los empleados.",
            .Formato = "Columnas: Código, Nombre, Salario básico + nivelación (0 = no cambiarlo), Aumento en córdobas."},
        New Seccion With {.Titulo = "Horas extra", .Tipo = 0,
            .Descripcion = "Registra las horas extra de los empleados en la nómina activa de su tipo de nómina.",
            .Formato = "Columnas: Código del empleado, Nombre, Horas."},
        New Seccion With {.Titulo = "Deducciones", .Tipo = 1,
            .Descripcion = "Agrega deducciones a los empleados en su nómina activa.",
            .Formato = "Columnas: INSS, Código del empleado, Nombre, Monto, Código de la deducción."},
        New Seccion With {.Titulo = "Ingresos (incentivos)", .Tipo = 5,
            .Descripcion = "Agrega o corrige incentivos de los empleados en su nómina activa.",
            .Formato = "Columnas: INSS, Código del empleado, Nombre, Monto, Código del tipo de incentivo."},
        New Seccion With {.Titulo = "Solicitudes", .Tipo = 6,
            .Descripcion = "Crea solicitudes (ausencias, vacaciones, subsidios…) con el tipo, la clasificación y la fecha que elija.",
            .Formato = "Columnas: Código del empleado, Nombre, Cantidad (días u horas según el tipo). Importar dos veces el mismo archivo duplica las solicitudes."},
        New Seccion With {.Titulo = "Solicitud total", .Tipo = 7,
            .Descripcion = "Asistencia total del período: horas extra, llegadas tarde, vacaciones, ausencias y subsidios. Si algo no valida, no se importa nada.",
            .Formato = "Columnas: CodEmpleado, HorasExtra, LlegadasTarde (min), DiasVacaciones, DiasVacaPagadas, DiasAusencia, DiasSubsidio, FechaIniNomina, FechaFinNomina, FechaAplica."},
        New Seccion With {.Titulo = "Consultas SQL", .Tipo = -1,
            .Descripcion = "Ejecuta una consulta SQL sobre la base de la compañía y muestra el resultado en la tabla.",
            .Formato = ""},
        New Seccion With {.Titulo = "Eliminar pendientes", .Tipo = -2,
            .Descripcion = "Borra movimientos que aún no se han pagado. Estas acciones no se pueden deshacer.",
            .Formato = ""}
    }

    Private _actual As Seccion

#Region "CARGA"

    Private Sub FrmImportar_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            CmbElimDed.Properties.DataSource = _catalogos.ListarTiposDeduccion()
            CmbElimInc.Properties.DataSource = _catalogos.ListarTiposIncentivo()
        Catch ex As Exception
            MostrarError(ex)
        End Try

        ' El combo oculto conserva los índices de tipo que usa Importar.
        CmbTipo.Properties.Items.AddRange(New Object() {"Horas extra", "Deducciones", "Movimiento salarial", "Empleados (catálogo)",
                                                        "Saldo de vacaciones", "Ingresos (incentivos)", "Solicitudes (ausencias, vacaciones)",
                                                        "Solicitud total (asistencia del período)"})

        CrearOpciones()
        CrearPanelesAvanzados()
        CrearMenu()

        AddHandler panelArchivo.DragEnter, AddressOf Archivo_DragEnter
        AddHandler panelArchivo.DragDrop, AddressOf Archivo_DragDrop
        AddHandler gridPrevia.DragEnter, AddressOf Archivo_DragEnter
        AddHandler gridPrevia.DragDrop, AddressOf Archivo_DragDrop
        gridPrevia.AllowDrop = True

        BtnImportar.Appearance.BackColor = Color.FromArgb(37, 99, 235)
        BtnImportar.Appearance.ForeColor = Color.White
        BtnImportar.Appearance.Options.UseBackColor = True
        BtnImportar.Appearance.Options.UseForeColor = True

        WindowState = FormWindowState.Maximized

    End Sub

    ''' <summary>Opciones propias de cada sección; se agregan al panel de opciones y se muestran según el tipo.</summary>
    Private Sub CrearOpciones()

        ChkDolarizado = New DevExpress.XtraEditors.CheckEdit With {.Size = New Size(110, 20)}
        ChkDolarizado.Properties.Caption = "Dolarizado"
        ChkSueldoActual = New DevExpress.XtraEditors.CheckEdit With {.Size = New Size(330, 20)}
        ChkSueldoActual.Properties.Caption = "Sueldo actual = sueldo x última tasa de cambio"

        LblSaldoAl = New DevExpress.XtraEditors.LabelControl With {.Text = "Saldo al:", .Margin = New Padding(3, 6, 3, 3)}
        DtpSaldoAl = New DevExpress.XtraEditors.DateEdit With {.Size = New Size(110, 20), .EditValue = Date.Today}
        ChkSegunNomina = New DevExpress.XtraEditors.CheckEdit With {.Size = New Size(320, 20)}
        ChkSegunNomina.Properties.Caption = "Según nómina de vacaciones activa (máx. 15 días)"

        LblSolTipo = New DevExpress.XtraEditors.LabelControl With {.Text = "Tipo:", .Margin = New Padding(3, 6, 3, 3)}
        CmbSolTipo = New DevExpress.XtraEditors.ComboBoxEdit With {.Size = New Size(170, 20)}
        CmbSolTipo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        CmbSolTipo.Properties.Items.AddRange(New Object() {"Vacaciones", "Vacaciones Pagadas", "Vacaciones Programadas", "Subsidio", "Ausente",
                                                           "AusenteBasico", "AusenteBasicoExcento", "AusenteBasicoDINSS", "Feriado",
                                                           "Permiso Programado", "Suspension"})
        LblSolClasif = New DevExpress.XtraEditors.LabelControl With {.Text = "Clasificación:", .Margin = New Padding(12, 6, 3, 3)}
        CmbSolClasif = New DevExpress.XtraEditors.ComboBoxEdit With {.Size = New Size(150, 20)}
        CmbSolClasif.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        CmbSolClasif.Properties.Items.AddRange(New Object() {"Riesgo Laboral", "Accidente Comun", "Embarazo", "Enfermedades", "Ausente",
                                                             "Vacaciones", "Vacaciones Pagadas", "Vacaciones Programadas", "Feriado",
                                                             "Permiso Programado", "Suspension"})
        LblSolFecha = New DevExpress.XtraEditors.LabelControl With {.Text = "Fecha de solicitud:", .Margin = New Padding(12, 6, 3, 3)}
        DtpSolFecha = New DevExpress.XtraEditors.DateEdit With {.Size = New Size(110, 20), .EditValue = Date.Today}
        AddHandler CmbSolTipo.SelectedIndexChanged, AddressOf CmbSolTipo_SelectedIndexChanged
        CmbSolTipo.SelectedIndex = 0

        panelOpciones.Controls.AddRange(New Control() {ChkDolarizado, ChkSueldoActual, LblSaldoAl, DtpSaldoAl, ChkSegunNomina,
                                                       LblSolTipo, CmbSolTipo, LblSolClasif, CmbSolClasif, LblSolFecha, DtpSolFecha})

    End Sub

    ''' <summary>Paneles de "Eliminar" (solicitudes y empleados) y "Consultas SQL"; se agregan por código, no están en el Designer.</summary>
    Private Sub CrearPanelesAvanzados()

        PnlSolEmp = New DevExpress.XtraEditors.GroupControl With {.Dock = DockStyle.Top, .Height = 130, .Text = "Solicitudes y empleados"}
        PnlSql = New DevExpress.XtraEditors.GroupControl With {.Dock = DockStyle.Top, .Height = 90, .Text = "Consulta SQL"}

        PnlSolEmp.Controls.Add(New DevExpress.XtraEditors.LabelControl With {.Text = "Tipo de solicitud (de la nómina activa indicada arriba):", .Location = New Point(20, 46)})

        CmbElimSol = New DevExpress.XtraEditors.ComboBoxEdit With {.Location = New Point(310, 43), .Size = New Size(190, 20)}
        CmbElimSol.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        CmbElimSol.Properties.Items.AddRange(CmbSolTipo.Properties.Items)
        CmbElimSol.SelectedIndex = 0

        BtnElimSol = New DevExpress.XtraEditors.SimpleButton With {.Text = "Eliminar solicitudes", .Location = New Point(20, 85), .Size = New Size(190, 30)}
        BtnElimEmp = New DevExpress.XtraEditors.SimpleButton With {.Text = "Eliminar TODOS los empleados", .Location = New Point(510, 85), .Size = New Size(220, 30)}
        BtnElimEmp.Appearance.ForeColor = Color.FromArgb(185, 28, 28)
        BtnElimEmp.Appearance.Options.UseForeColor = True

        TxtSql = New DevExpress.XtraEditors.TextEdit With {.Location = New Point(20, 42), .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right, .Size = New Size(560, 22)}
        TxtSql.Properties.NullValuePrompt = "SELECT * FROM Empleado WHERE …"
        TxtSql.Properties.NullValuePromptShowForEmptyValue = True
        BtnConsultar = New DevExpress.XtraEditors.SimpleButton With {.Text = "Consultar", .Anchor = AnchorStyles.Top Or AnchorStyles.Right, .Location = New Point(600, 39), .Size = New Size(110, 28)}
        BtnEjecutarSql = New DevExpress.XtraEditors.SimpleButton With {.Text = "Ejecutar", .Anchor = AnchorStyles.Top Or AnchorStyles.Right, .Location = New Point(720, 39), .Size = New Size(110, 28)}

        PnlSolEmp.Controls.AddRange(New Control() {CmbElimSol, BtnElimSol, BtnElimEmp})
        PnlSql.Controls.AddRange(New Control() {TxtSql, BtnConsultar, BtnEjecutarSql})

        panelContenido.Controls.Add(PnlSolEmp)
        panelContenido.Controls.Add(PnlSql)
        panelContenido.Controls.SetChildIndex(PnlSolEmp, panelContenido.Controls.GetChildIndex(panelEliminar))
        panelContenido.Controls.SetChildIndex(PnlSql, panelContenido.Controls.GetChildIndex(panelEliminar))

    End Sub

    Private Sub CrearMenu()

        Dim importar As New AccordionControlElement With {.Text = "IMPORTAR DESDE EXCEL", .Style = ElementStyle.Group, .Expanded = True}
        Dim herramientas As New AccordionControlElement With {.Text = "HERRAMIENTAS", .Style = ElementStyle.Group, .Expanded = True}

        For Each s In _secciones
            Dim item As New AccordionControlElement With {.Text = s.Titulo, .Style = ElementStyle.Item, .Tag = s}
            If s.Tipo >= 0 Then importar.Elements.Add(item) Else herramientas.Elements.Add(item)
        Next

        Menu.Elements.AddRange(New AccordionControlElement() {importar, herramientas})
        AddHandler Menu.ElementClick, Sub(s, ev) If ev.Element.Style = ElementStyle.Item Then MostrarSeccion(DirectCast(ev.Element.Tag, Seccion))

        MostrarSeccion(_secciones(3))
        Menu.SelectedElement = importar.Elements(3)

    End Sub

    ''' <summary>Cambia de sección: título, descripción, columnas esperadas y los controles que le corresponden. Descarta el archivo cargado.</summary>
    Private Sub MostrarSeccion(s As Seccion)

        _actual = s
        LblTitulo.Text = s.Titulo
        LblDescripcion.Text = s.Descripcion
        LblFormato.Text = s.Formato

        Dim importa As Boolean = s.Tipo >= 0

        If importa Then CmbTipo.SelectedIndex = s.Tipo

        LimpiarArchivo()

        panelArchivo.Visible = importa
        panelBotones.Visible = importa
        panelEliminar.Visible = s.Tipo = -2
        PnlSolEmp.Visible = s.Tipo = -2
        PnlSql.Visible = s.Tipo = -1
        gridPrevia.Visible = s.Tipo <> -2
        TxtResultado.Text = ""

    End Sub

    Private Sub LimpiarArchivo()
        _datos = Nothing
        TxtRuta.Text = ""
        gridPrevia.DataSource = Nothing
        BtnImportar.Enabled = False
    End Sub

    Private Sub CmbTipo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbTipo.SelectedIndexChanged

        Dim t As Integer = CmbTipo.SelectedIndex

        ChkAcumular.Visible = t = 0 OrElse t = 7
        ChkDolarizado.Visible = t = 3
        ChkSueldoActual.Visible = t = 3
        LblSaldoAl.Visible = t = 4
        DtpSaldoAl.Visible = t = 4
        ChkSegunNomina.Visible = t = 4
        LblSolTipo.Visible = t = 6
        CmbSolTipo.Visible = t = 6
        LblSolClasif.Visible = t = 6
        CmbSolClasif.Visible = t = 6
        LblSolFecha.Visible = t = 6
        DtpSolFecha.Visible = t = 6

        panelOpciones.Visible = t = 0 OrElse t >= 3

    End Sub

    Private Sub Archivo_DragEnter(sender As Object, e As DragEventArgs)

        If e.Data.GetDataPresent(DataFormats.FileDrop) AndAlso _actual IsNot Nothing AndAlso _actual.Tipo >= 0 Then
            e.Effect = DragDropEffects.Copy
        End If

    End Sub

    Private Sub Archivo_DragDrop(sender As Object, e As DragEventArgs)

        Dim archivos() As String = TryCast(e.Data.GetData(DataFormats.FileDrop), String())

        If archivos Is Nothing OrElse archivos.Length = 0 Then Exit Sub

        Dim ext As String = IO.Path.GetExtension(archivos(0)).ToLowerInvariant()

        If ext <> ".xls" AndAlso ext <> ".xlsx" Then
            MessageBox.Show("Solo se pueden importar archivos de Excel (.xls o .xlsx).", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        AbrirArchivo(archivos(0))

    End Sub

    ''' <summary>Propone la clasificación según el tipo, como la V.5.0; se puede cambiar.</summary>
    Private Sub CmbSolTipo_SelectedIndexChanged(sender As Object, e As EventArgs)

        Select Case Convert.ToString(CmbSolTipo.EditValue)
            Case "Vacaciones", "Vacaciones Pagadas", "Vacaciones Programadas", "Feriado", "Permiso Programado", "Suspension", "Ausente"
                CmbSolClasif.EditValue = CmbSolTipo.EditValue
            Case "AusenteBasico", "AusenteBasicoExcento", "AusenteBasicoDINSS"
                CmbSolClasif.EditValue = "Ausente"
            Case Else
                CmbSolClasif.EditValue = "Accidente Comun"
        End Select

    End Sub

    Private Sub BtnExaminar_Click(sender As Object, e As EventArgs) Handles BtnExaminar.Click

        Using dlg As New OpenFileDialog With {.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx", .Title = "Seleccione el archivo a importar"}

            If dlg.ShowDialog(Me) <> DialogResult.OK Then Exit Sub

            AbrirArchivo(dlg.FileName)

        End Using

    End Sub

    ''' <summary>Lee la primera hoja del libro y la muestra como vista previa.</summary>
    Public Sub AbrirArchivo(ruta As String)

        Try

            Cursor = Cursors.WaitCursor

            _datos = LectorExcel.Leer(ruta)
            TxtRuta.Text = ruta
            gridPrevia.DataSource = _datos
            gridViewPrevia.PopulateColumns()
            TxtResultado.Text = "Archivo cargado: " & _datos.Rows.Count & " filas. Revise la vista previa y pulse Importar."
            BtnImportar.Enabled = _datos.Rows.Count > 0

        Catch ex As Exception
            LimpiarArchivo()
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

#End Region

#Region "IMPORTAR"

    Private Sub BtnImportar_Click(sender As Object, e As EventArgs) Handles BtnImportar.Click

        If _datos Is Nothing OrElse _datos.Rows.Count = 0 Then
            MessageBox.Show("Seleccione primero un archivo con datos.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("Se importarán " & _datos.Rows.Count & " filas (" & CmbTipo.Text & "). ¿Desea continuar?", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Importar(CmbTipo.SelectedIndex, _datos)

    End Sub

    ''' <summary>Importa la tabla según el tipo (0 = horas extra, 1 = deducciones, 2 = movimiento salarial, 3 = catálogo de empleados, 4 = saldo de vacaciones, 5 = incentivos, 6 = solicitudes, 7 = solicitud total) y muestra lo ocurrido por fila.</summary>
    Public Function Importar(tipo As Integer, filas As DataTable) As Integer

        Try

            Cursor = Cursors.WaitCursor

            Dim res As CsImportacionDAL.ResultadoImportacion

            Select Case tipo
                Case 0 : res = _dal.ImportarHorasExtra(filas, ChkAcumular.Checked)
                Case 1 : res = _dal.ImportarDeducciones(filas)
                Case 3 : res = _dal.ImportarEmpleados(filas, ChkDolarizado.Checked, ChkSueldoActual.Checked)
                Case 5 : res = _dal.ImportarIncentivos(filas)
                Case 6 : res = _dal.ImportarSolicitudes(filas, Convert.ToString(CmbSolTipo.EditValue), Convert.ToString(CmbSolClasif.EditValue),
                                                        Convert.ToDateTime(DtpSolFecha.EditValue))
                Case 7 : res = _dal.ImportarSolicitudTotal(filas, ChkAcumular.Checked)
                Case 4 : res = _vacaciones.ImportarSaldos(filas, Convert.ToDateTime(DtpSaldoAl.EditValue), ChkSegunNomina.Checked)
                Case Else : res = _dal.ImportarMovimientoSalarial(filas)
            End Select

            TxtResultado.Text = "Filas importadas: " & res.Importadas & vbCrLf & String.Join(vbCrLf, res.Mensajes)

            Return res.Importadas

        Catch ex As Exception
            TxtResultado.Text = "La importación falló y no se guardó ningún cambio."
            MostrarError(ex)
            Return 0
        Finally
            Cursor = Cursors.Default
        End Try

    End Function

#End Region

#Region "ELIMINAR PENDIENTES"

    Private Sub BtnElimHE_Click(sender As Object, e As EventArgs) Handles BtnElimHE.Click

        If SpnNomina.Value <= 0 Then
            MessageBox.Show("Digite el número de nómina.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Eliminar("las horas extra pendientes de la nómina " & SpnNomina.Value,
                 Function() _dal.EliminarHorasExtraPendientes(Convert.ToInt32(SpnNomina.Value)))

    End Sub

    Private Sub BtnElimDed_Click(sender As Object, e As EventArgs) Handles BtnElimDed.Click

        If SpnNomina.Value <= 0 OrElse Convert.ToString(CmbElimDed.EditValue) = "" Then
            MessageBox.Show("Digite el número de nómina y elija la deducción.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Eliminar("las deducciones pendientes de tipo " & CmbElimDed.Text & " de la nómina " & SpnNomina.Value,
                 Function() _dal.EliminarDeduccionesPendientes(Convert.ToInt32(SpnNomina.Value), Convert.ToString(CmbElimDed.EditValue)))

    End Sub

    Private Sub BtnElimInc_Click(sender As Object, e As EventArgs) Handles BtnElimInc.Click

        If Convert.ToString(CmbElimInc.EditValue) = "" Then
            MessageBox.Show("Elija el incentivo.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Eliminar("los incentivos pendientes de tipo " & CmbElimInc.Text,
                 Function() _dal.EliminarIncentivosPendientes(Convert.ToString(CmbElimInc.EditValue)))

    End Sub

    Private Sub BtnElimSol_Click(sender As Object, e As EventArgs) Handles BtnElimSol.Click

        If SpnNomina.Value <= 0 Then
            MessageBox.Show("Digite el número de nómina.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Dim tipo As String = Convert.ToString(CmbElimSol.EditValue)

        Eliminar("las solicitudes de tipo " & tipo & " de los empleados de la nómina activa " & SpnNomina.Value,
                 Function() _dal.EliminarSolicitudesPendientes(Convert.ToInt32(SpnNomina.Value), tipo))

    End Sub

    Private Sub BtnElimEmp_Click(sender As Object, e As EventArgs) Handles BtnElimEmp.Click

        If MessageBox.Show("Se eliminarán TODOS los empleados con sus horas extra, deducciones, incentivos y solicitudes. ¿Está seguro?", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) <> DialogResult.Yes Then
            Exit Sub
        End If

        Eliminar("todos los empleados y sus movimientos", Function() _dal.EliminarTodosLosEmpleados())

    End Sub

    Private Sub BtnConsultar_Click(sender As Object, e As EventArgs) Handles BtnConsultar.Click

        If Not CsConsultaDAL.DevuelveFilas(TxtSql.Text) Then
            MessageBox.Show("Consultar solo admite SELECT; use Ejecutar para otras sentencias.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        EjecutarSql(False)

    End Sub

    Private Sub BtnEjecutarSql_Click(sender As Object, e As EventArgs) Handles BtnEjecutarSql.Click

        If String.IsNullOrWhiteSpace(TxtSql.Text) Then
            MessageBox.Show("Es obligatorio escribir la consulta SQL.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        EjecutarSql(Not CsConsultaDAL.SoloLee(TxtSql.Text))

    End Sub

    Private Sub EjecutarSql(confirmar As Boolean)

        If confirmar AndAlso MessageBox.Show("¿Está seguro de ejecutar la consulta?", "Zeus Nominas", MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try
            Cursor = Cursors.WaitCursor

            Dim r = _consulta.EjecutarSentencia(TxtSql.Text)

            If r.EsConsulta Then
                gridPrevia.DataSource = r.Tabla
                gridViewPrevia.PopulateColumns()
                TxtResultado.Text = "Filas: " & r.Tabla.Rows.Count
            Else
                TxtResultado.Text = "Filas afectadas: " & r.FilasAfectadas
            End If

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub Eliminar(que As String, accion As Func(Of Integer))

        If MessageBox.Show("Se eliminarán " & que & ". Esta acción no se puede deshacer. ¿Desea continuar?", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try
            Cursor = Cursors.WaitCursor
            Dim n As Integer = accion()
            TxtResultado.Text = "Registros eliminados: " & n
        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

#End Region

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
