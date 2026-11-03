Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Calcular nómina (FrmCalcularNomina de la V.5.0): calcula la nómina activa de un tipo, muestra la planilla, la exporta a Excel,
''' a los bancos (BAC, Banpro, Bancentro) y a CSV, desglosa billetes/monedas y cierra la nómina. Solo UI: el cálculo está en
''' <see cref="CsCalculoNominaDAL"/>, el cierre en <see cref="CsCierreNominaDAL"/> y las consultas en <see cref="CsPlanillaNominaDAL"/>.
''' La interfaz se arma por código (no hay Designer).
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmCalcularNomina
    Inherits XtraForm

    Private ReadOnly _planilla As New CsPlanillaNominaDAL
    Private ReadOnly _calculo As New CsCalculoNominaDAL
    Private ReadOnly _cierre As New CsCierreNominaDAL
    Private _estado As CsPlanillaNominaDAL.EstadoNomina
    Private _detalle As DataTable
    Private _cargando As Boolean
    Private _ocupado As Boolean

    Private CmbTipo, CmbColilla, CmbNominaRpt As LookUpEdit
    Private ChkPorDepartamento As CheckEdit
    Private BarraProgreso As ProgressBarControl
    Private LblProgreso As LabelControl
    Private LblNomina, LblEstado As LabelControl
    Private Grid As GridControl
    Private Vista As GridView
    Private BtnCalcular, BtnCerrarNomina, BtnExcel, BtnBac, BtnBanpro, BtnBancentro, BtnCsv, BtnMonedas, BtnSalir, BtnColillas, BtnNominaRpt, BtnReportes As SimpleButton

    Public Sub New()

        Text = "Calcular Nómina"
        ClientSize = New Size(1040, 500)
        MinimumSize = New Size(700, 400)
        StartPosition = FormStartPosition.CenterParent

        CrearInterfaz()
        AddHandler Load, AddressOf FrmCalcularNomina_Load

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        Dim superior As New PanelControl With {.Dock = DockStyle.Top, .Height = 148}
        EstiloGrid.AplicarBarra(superior)

        superior.Controls.Add(New LabelControl With {.Text = "Tipo de nómina:", .Location = New Point(12, 14)})
        CmbTipo = New LookUpEdit With {.Location = New Point(105, 11), .Size = New Size(240, 20)}
        CmbTipo.Properties.NullText = ""
        CmbTipo.Properties.Columns.Add(New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Nomina", "Nómina"))
        CmbTipo.Properties.ShowHeader = False
        CmbTipo.Properties.DisplayMember = "Nomina"
        CmbTipo.Properties.ValueMember = "CodTipoNomina"
        AddHandler CmbTipo.EditValueChanged, Sub(s, e) If Not _cargando Then CargarNomina()

        LblNomina = New LabelControl With {.Location = New Point(365, 14), .AutoSizeMode = LabelAutoSizeMode.None, .Size = New Size(560, 16)}
        LblNomina.Appearance.Font = New Font(LblNomina.Appearance.Font, FontStyle.Bold)

        BtnCalcular = Boton("Calcular", "Refresh", 110)
        BtnCalcular.Location = New Point(12, 44)
        AddHandler BtnCalcular.Click, AddressOf BtnCalcular_Click

        BtnCerrarNomina = Boton("Cerrar nómina", "Apply", 125)
        BtnCerrarNomina.Location = New Point(132, 44)
        AddHandler BtnCerrarNomina.Click, AddressOf BtnCerrarNomina_Click

        LblEstado = New LabelControl With {.Location = New Point(275, 52), .AutoSizeMode = LabelAutoSizeMode.None, .Size = New Size(740, 16)}

        BarraProgreso = New ProgressBarControl With {.Location = New Point(275, 49), .Size = New Size(260, 22), .Visible = False}
        BarraProgreso.Properties.ShowTitle = True
        BarraProgreso.Properties.PercentView = False
        LblProgreso = New LabelControl With {.Location = New Point(545, 52), .AutoSizeMode = LabelAutoSizeMode.None, .Size = New Size(480, 18), .Visible = False, .AllowHtmlString = True}

        superior.Controls.AddRange(New Control() {CmbTipo, LblNomina, BtnCalcular, BtnCerrarNomina, LblEstado, BarraProgreso, LblProgreso})

        CmbColilla = CrearSelectorFormato(CatalogoReportesNomina.FormatosColilla(), New Point(105, 82))
        BtnColillas = Boton("Colillas", "Print", 100)
        BtnColillas.Location = New Point(365, 77)
        ChkPorDepartamento = New CheckEdit With {.Text = "Una hoja por departamento", .Location = New Point(475, 82), .Width = 170}
        CmbNominaRpt = CrearSelectorFormato(CatalogoReportesNomina.FormatosNomina(), New Point(105, 112))
        BtnNominaRpt = Boton("Nómina", "Print", 100)
        BtnNominaRpt.Location = New Point(365, 107)
        AddHandler BtnColillas.Click, AddressOf BtnColillas_Click
        AddHandler BtnNominaRpt.Click, AddressOf BtnNominaRpt_Click

        superior.Controls.AddRange(New Control() {
            New LabelControl With {.Text = "Formato colilla:", .Location = New Point(12, 85)}, CmbColilla, BtnColillas, ChkPorDepartamento,
            New LabelControl With {.Text = "Formato nómina:", .Location = New Point(12, 115)}, CmbNominaRpt, BtnNominaRpt})

        Dim barra As New FlowLayoutPanel With {.Dock = DockStyle.Bottom, .Height = 46, .Padding = New Padding(8, 6, 8, 4), .WrapContents = True}
        BtnExcel = Boton("Planilla a Excel", "ExportToXLSX", 140)
        BtnBac = BotonBanco("BAC", My.Resources.BancoBAC, 105)
        BtnBanpro = BotonBanco("Banpro", My.Resources.BancoBanpro, 115)
        BtnBancentro = BotonBanco("Bancentro", My.Resources.BancoBancentro, 125)
        BtnCsv = Boton("Exportar CSV", "ExportToCSV", 130)
        BtnMonedas = Boton("Denominaciones", "Currency", 140)
        BtnReportes = Boton("Reportes", "Report", 110)
        AddHandler BtnReportes.Click, AddressOf BtnReportes_Click
        BtnSalir = Boton("Salir", "Close", 80)

        AddHandler BtnExcel.Click, AddressOf BtnExcel_Click
        AddHandler BtnBac.Click, AddressOf BtnBac_Click
        AddHandler BtnBanpro.Click, Sub(s, e) ExportarCreditoDebito(True)
        AddHandler BtnBancentro.Click, Sub(s, e) ExportarCreditoDebito(False)
        AddHandler BtnCsv.Click, AddressOf BtnCsv_Click
        AddHandler BtnMonedas.Click, AddressOf BtnMonedas_Click
        AddHandler BtnSalir.Click, Sub(s, e) Close()

        barra.Controls.AddRange(New Control() {BtnExcel, BtnBac, BtnBanpro, BtnBancentro, BtnCsv, BtnMonedas, BtnReportes, BtnSalir})

        Grid = New GridControl With {.Dock = DockStyle.Fill}
        Vista = New GridView(Grid)
        Grid.MainView = Vista
        Grid.ViewCollection.Add(Vista)
        Vista.OptionsView.ShowAutoFilterRow = True
        Vista.OptionsView.ShowFooter = True
        Vista.OptionsView.ColumnAutoWidth = False
        Vista.OptionsBehavior.Editable = False
        EstiloGrid.Aplicar(Vista)

        Controls.Add(Grid)
        Controls.Add(barra)
        Controls.Add(superior)

    End Sub

    Private Shared Function Boton(texto As String, icono As String, ancho As Integer) As SimpleButton
        Dim b As New SimpleButton With {.Text = texto, .Size = New Size(ancho, 30)}
        b.ImageOptions.ImageUri.Uri = icono
        Return b
    End Function

    ''' <summary>Botón con el logo del banco, escalado a 22 px de alto.</summary>
    Private Shared Function BotonBanco(texto As String, logo As Image, ancho As Integer) As SimpleButton
        Dim b As New SimpleButton With {.Text = texto, .Size = New Size(ancho, 30)}
        Dim alto As Integer = 22
        Dim anchoLogo As Integer = Math.Max(1, CInt(logo.Width * alto / logo.Height))
        b.ImageOptions.Image = New Bitmap(logo, New Size(anchoLogo, alto))
        Return b
    End Function

#End Region

#Region "CARGA"

    Private Sub FrmCalcularNomina_Load(sender As Object, e As EventArgs)

        If MdiParent IsNot Nothing Then WindowState = FormWindowState.Maximized

        Try
            _cargando = True
            Dim tipos As DataTable = _planilla.ListarTipos()
            CmbTipo.Properties.DataSource = tipos
            If tipos.Rows.Count > 0 Then CmbTipo.EditValue = tipos.Rows(0)("CodTipoNomina")
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo leer los tipos de nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _cargando = False
        End Try

        SeleccionarFormatosDeEmpresa()
        CargarNomina()

    End Sub

    Private Function TipoSeleccionado() As String
        Return If(CmbTipo.EditValue Is Nothing, "", CStr(CmbTipo.EditValue))
    End Function

    Private Sub CargarNomina()

        _estado = Nothing
        _detalle = Nothing

        If TipoSeleccionado() <> "" Then
            Try
                _estado = _planilla.BuscarNomina(TipoSeleccionado())
                _detalle = If(_estado Is Nothing, New DataTable, _planilla.ListarPlanilla(_estado.Numero))
            Catch ex As Exception
                XtraMessageBox.Show("No se pudo leer la nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

        MostrarDetalle()
        ActualizarEstado()

    End Sub

    Private Sub MostrarDetalle()

        Grid.DataSource = _detalle
        Vista.PopulateColumns()
        If _detalle Is Nothing OrElse Vista.Columns.Count = 0 Then Return

        Texto("Codigo", "Código", 80)
        Texto("Nombres", "Nombre del empleado", 240)
        Texto("Departamento", "Departamento", 140)
        Texto("Cargo", "Cargo", 140)
        Texto("NumeroInss", "No. INSS", 90)
        Texto("NumCedula", "Cédula", 110)

        Numero("SalarioBasico", "Salario básico", 100)
        Numero("SeptimoDia", "Séptimo día", 90)
        Numero("IncetivoProduccion", "Incent. producción", 100)
        Numero("BonoProduccion", "Bono producción", 100)
        Numero("Destajo", "Destajo", 90)
        Numero("HE", "Horas extra", 70)
        Numero("HorasExtras", "Monto horas extra", 100)
        Numero("Comisiones", "Comisiones", 90)
        Numero("Incentivos", "Incentivos", 90)
        Numero("OtrosIngresos", "Otros ingresos", 95)
        Numero("VacacionesPagadas", "Vacaciones pagadas", 105)
        Numero("Antiguedad", "Antigüedad", 90)
        Numero("TotalSubsidio", "Subsidio", 90)
        Numero("Viaticos", "Viáticos", 90)
        Numero("TotalDevengado", "Total devengado", 110, True)
        Numero("MontoINSS", "INSS", 85, True)
        Numero("MontoIR", "IR", 85, True)
        Numero("Prestamo", "Préstamo", 90, True)
        Numero("Deducciones", "Deducciones", 95, True)
        Numero("Adelantos", "Adelantos", 85)
        Numero("MontoPension", "Pensión", 85)
        Numero("EmbargoJudicial", "Embargo", 85)
        Numero("TotalDeducir", "Total deducir", 105, True)
        Numero("NetoPagar", "Neto a pagar", 110, True)
        Numero("INSSPatronal", "INSS patronal", 95, True)
        Numero("INATEC", "INATEC", 85, True)
        Numero("Mes13", "13vo mes", 90, True)
        Numero("Vacaciones", "Vacaciones", 90)

        Vista.Columns("HE").DisplayFormat.FormatString = "#,##0.##"

    End Sub

    Private Sub Texto(campo As String, titulo As String, ancho As Integer)
        Dim c As GridColumn = Vista.Columns(campo)
        c.Caption = titulo
        c.Width = ancho
    End Sub

    Private Sub Numero(campo As String, titulo As String, ancho As Integer, Optional sumar As Boolean = False)
        Dim c As GridColumn = Vista.Columns(campo)
        c.Caption = titulo
        c.Width = ancho
        c.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        c.DisplayFormat.FormatString = "#,##0.00"
        If sumar Then c.Summary.Add(DevExpress.Data.SummaryItemType.Sum, campo, "{0:#,##0.00}")
    End Sub

    Private Sub ActualizarEstado()

        Dim hayNomina As Boolean = _estado IsNot Nothing
        Dim abierta As Boolean = hayNomina AndAlso _estado.Activa AndAlso Not _estado.Cerrada
        Dim hayDatos As Boolean = hayNomina AndAlso _detalle IsNot Nothing AndAlso _detalle.Rows.Count > 0

        BtnCalcular.Enabled = abierta AndAlso Not _ocupado
        BtnCerrarNomina.Enabled = abierta AndAlso _estado.Procesada AndAlso hayDatos AndAlso Not _ocupado
        For Each b As SimpleButton In {BtnExcel, BtnBac, BtnBanpro, BtnBancentro, BtnCsv, BtnMonedas, BtnColillas, BtnNominaRpt, BtnReportes}
            b.Enabled = hayDatos AndAlso Not _ocupado
        Next
        CmbTipo.Enabled = Not _ocupado

        If Not hayNomina Then
            LblNomina.Text = "El tipo de nómina no tiene ninguna nómina. Actívela primero."
            LblEstado.Text = ""
            Return
        End If

        LblNomina.Text = $"Nómina No. {_estado.Numero}  -  Del {_estado.Inicio:dd/MM/yyyy} al {_estado.Fin:dd/MM/yyyy}"
        LblEstado.Text = If(_estado.Cerrada, "Nómina cerrada.",
                            If(Not _estado.Procesada, "Nómina abierta, aún sin calcular.",
                               $"Nómina abierta y calculada - {If(_detalle IsNot Nothing, _detalle.Rows.Count, 0)} empleados."))

    End Sub

#End Region

#Region "CALCULAR Y CERRAR"

    Private Async Sub BtnCalcular_Click(sender As Object, e As EventArgs)

        If _estado Is Nothing Then Return

        Dim mensaje As String = If(_estado.Procesada,
                                   $"Se recalculará la nómina {_estado.Numero} y se reemplazará su detalle.",
                                   $"Se calculará la nómina {_estado.Numero} del {_estado.Inicio:dd/MM/yyyy} al {_estado.Fin:dd/MM/yyyy}.")
        If XtraMessageBox.Show(mensaje & vbCrLf & "¿Continuar?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Dim tipo As String = TipoSeleccionado()
        _ocupado = True
        ActualizarEstado()
        Cursor = Cursors.WaitCursor

        Try
            BarraProgreso.Properties.Maximum = 100
            BarraProgreso.Position = 0
            MostrarProgreso(True)

            ' La capa de datos entrega "código<tab>nombre"; el código y el nombre se resaltan con otra fuente.
            Dim progreso As Action(Of Integer, Integer, String) =
                Sub(actual, total, empleado)
                    BeginInvoke(Sub()
                                    Dim partes As String() = empleado.Split(ControlChars.Tab)
                                    BarraProgreso.Properties.Maximum = Math.Max(1, total)
                                    BarraProgreso.Position = actual
                                    LblProgreso.Text = $"{actual} de {total}   <font=Consolas><b><color=0,70,160>{System.Net.WebUtility.HtmlEncode(partes(0))}</color></b></font>   " &
                                                       $"<font=Georgia><i><b>{System.Net.WebUtility.HtmlEncode(If(partes.Length > 1, partes(1), ""))}</b></i></font>"
                                End Sub)
                End Sub

            Dim r As CsCalculoNominaDAL.ResumenCalculo = Await Task.Run(Function() _calculo.Calcular(tipo, progreso))

            Try
                Registrar($"Se Calculo la Nomina: {r.NumNomina} ({r.Empleados} empleados)")
            Catch
                ' El rastro no debe deshacer un cálculo ya grabado.
            End Try

            _ocupado = False
            CargarNomina()
            XtraMessageBox.Show($"Nómina calculada: {r.Empleados} empleados.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As NotSupportedException
            _ocupado = False
            ActualizarEstado()
            XtraMessageBox.Show("Esta nómina usa una opción que todavía no está migrada:" & vbCrLf & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Catch ex As Exception
            _ocupado = False
            ActualizarEstado()
            XtraMessageBox.Show("No se pudo calcular la nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            MostrarProgreso(False)
            Cursor = Cursors.Default
        End Try

    End Sub

    ''' <summary>Muestra la barra y el empleado en proceso en lugar del texto de estado.</summary>
    Private Sub MostrarProgreso(visible As Boolean)
        BarraProgreso.Visible = visible
        LblProgreso.Visible = visible
        LblEstado.Visible = Not visible
        If visible Then LblProgreso.Text = "Preparando el cálculo..."
    End Sub

    Private Async Sub BtnCerrarNomina_Click(sender As Object, e As EventArgs)

        If _estado Is Nothing Then Return

        If XtraMessageBox.Show($"Al cerrar la nómina {_estado.Numero} se pagan las deducciones, incentivos y cuotas de préstamo, y ya no se podrá recalcular." & vbCrLf & "¿Cerrarla?",
                               Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Dim tipo As String = TipoSeleccionado()
        Dim usuario As String = New Compañias().Nombre_Usuario
        _ocupado = True
        ActualizarEstado()
        Cursor = Cursors.WaitCursor

        Try
            Dim r As CsCierreNominaDAL.ResultadoCierre = Await Task.Run(Function() _cierre.Cerrar(tipo, usuario))
            _ocupado = False
            CargarNomina()
            XtraMessageBox.Show($"La nómina {r.NumNomina} ha sido cerrada." & vbCrLf &
                                $"Deducciones pagadas: {r.DeduccionesPagadas}, incentivos: {r.IncentivosPagados}, cuotas de préstamo: {r.CuotasPrestamo}.",
                                Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            _ocupado = False
            ActualizarEstado()
            XtraMessageBox.Show("No se pudo cerrar la nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

#End Region

#Region "EXPORTACION"

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs)

        Try
            Dim ruta As String = ExportadorGrid.ExportarAExcel(Vista, $"Nomina {_estado.Numero}")
            If ruta = "" Then Return
            Process.Start(New ProcessStartInfo(ruta) With {.UseShellExecute = True})
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo exportar o abrir el archivo: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnCsv_Click(sender As Object, e As EventArgs)

        Try
            Using dlg As New SaveFileDialog With {.Filter = "Archivo CSV (*.csv)|*.csv", .FileName = $"Nomina {_estado.Numero}", .Title = "Exportar a CSV"}
                If dlg.ShowDialog(Me) <> DialogResult.OK Then Return

                Dim columnas As New List(Of KeyValuePair(Of String, String)) From {
                    Par("CodEmpleado", "Codigo"), Par("Nombres", "Nombres"), Par("SalarioBasico", "SalarioBasico"), Par("Destajo", "Destajo"),
                    Par("HorasExtras", "HorasExtras"), Par("Comisiones", "Comisiones"), Par("Incentivos", "Incentivos"),
                    Par("VacacionesPagadas", "VacacionesPagadas"), Par("SeptimoDia", "SeptimoDia"), Par("IncetivoProduccion", "IncetivoProduccion"),
                    Par("OtrosIngresos", "OtrosIngresos"), Par("TotalDevengado", "TotalDevengado"), Par("Prestamo", "Prestamo"),
                    Par("MontoINSS", "MontoINSS"), Par("MontoIr", "MontoIR"), Par("Deducciones", "Deducciones"), Par("INSSPatronal", "INSSPatronal")}

                ExportadorCsv.Escribir(_detalle, columnas, dlg.FileName)
                XtraMessageBox.Show("Archivo exportado:" & vbCrLf & dlg.FileName, Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo exportar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Shared Function Par(encabezado As String, campo As String) As KeyValuePair(Of String, String)
        Return New KeyValuePair(Of String, String)(encabezado, campo)
    End Function

    Private Sub BtnBac_Click(sender As Object, e As EventArgs)

        Using dlg As New FrmDatosBanco(FrmDatosBanco.Formato.Bac, "Exportar a BAC", "Nómina")

            If dlg.ShowDialog(Me) <> DialogResult.OK Then Return

            Try
                Dim tasa As Double = 0
                If dlg.EnDolares Then
                    tasa = New CsBajasDAL().TasaCambio(dlg.Fecha)
                    If tasa <= 0 Then
                        XtraMessageBox.Show("No hay tasa de cambio para la fecha de pago.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                End If

                Dim tabla As DataTable = ExportadorBancos.FormatoBac(_planilla.ListarParaBanco(_estado.Numero), dlg.CodigoCliente,
                                                                     dlg.Consecutivo, dlg.Fecha, dlg.Concepto, tasa)
                Guardar(tabla, $"BAC Nomina {_estado.Numero}")
                Registrar($"Se Exporto la Nomina BAC: {_estado.Numero}")
            Catch ex As Exception
                XtraMessageBox.Show("No se pudo exportar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End Using

    End Sub

    Private Sub ExportarCreditoDebito(esBanpro As Boolean)

        Dim nombre As String = If(esBanpro, "Banpro", "Bancentro")

        Using dlg As New FrmDatosBanco(FrmDatosBanco.Formato.CreditoDebito, "Exportar a " & nombre, "Nómina")

            If dlg.ShowDialog(Me) <> DialogResult.OK Then Return

            Try
                Dim columna As String = If(esBanpro, ExportadorBancos.CuentaBanpro, ExportadorBancos.CuentaBac)
                Dim empresa As DatosEmpresa = New CsControlesDAL().ObtenerDatosEmpresa()
                Dim tabla As DataTable = ExportadorBancos.FormatoCreditoDebito(_planilla.ListarParaBanco(_estado.Numero), columna,
                                                                               dlg.Concepto, empresa.Nombre_Empresa, dlg.CuentaEmpresa)
                Guardar(tabla, $"{nombre} Nomina {_estado.Numero}")
                Registrar($"Se Exporto la Nomina {nombre}: {_estado.Numero}")
            Catch ex As Exception
                XtraMessageBox.Show("No se pudo exportar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End Using

    End Sub

    Private Sub Guardar(tabla As DataTable, nombre As String)
        Dim ruta As String = ExportadorGrid.ExportarTabla(tabla, nombre)
        If ruta <> "" Then XtraMessageBox.Show("Archivo exportado:" & vbCrLf & ruta, Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub Registrar(accion As String)
        Try
            _cierre.Registrar(New Compañias().Nombre_Usuario, accion)
        Catch
            ' El rastro de una exportación no debe estorbar al usuario.
        End Try
    End Sub

#Region "COLILLAS Y REPORTES"

    Private Shared Function CrearSelectorFormato(formatos As IList(Of CatalogoReportesNomina.Formato), ubicacion As Point) As LookUpEdit
        Dim c As New LookUpEdit With {.Location = ubicacion, .Size = New Size(240, 20)}
        c.Properties.NullText = ""
        c.Properties.Columns.Add(New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Nombre", ""))
        c.Properties.ShowHeader = False
        c.Properties.DisplayMember = "Nombre"
        c.Properties.ValueMember = "Clave"
        c.Properties.DataSource = formatos
        c.EditValue = formatos(0).Clave
        Return c
    End Function

    ''' <summary>Deja elegidos los formatos que la empresa tiene configurados en la V.5.0 (Controles: FormatoColilla / FormatoNomina).</summary>
    Private Sub SeleccionarFormatosDeEmpresa()
        Try
            Dim empresa As DatosEmpresa = New CsControlesDAL().ObtenerDatosEmpresa()
            CmbColilla.EditValue = CatalogoReportesNomina.ClaveDe(CatalogoReportesNomina.FormatosColilla(), empresa.Formato_Colilla)
            CmbNominaRpt.EditValue = CatalogoReportesNomina.ClaveDe(CatalogoReportesNomina.FormatosNomina(), empresa.Formato_Nomina)
        Catch
            ' Sin configuración se queda el primer formato de cada lista.
        End Try
    End Sub

    Private Function MostrarEmpleadosEnCero() As Boolean
        Dim fila As DataRowView = TryCast(CmbTipo.GetSelectedDataRow(), DataRowView)
        Return fila IsNot Nothing AndAlso Not IsDBNull(fila("MostraEmpleadoCero")) AndAlso Convert.ToBoolean(fila("MostraEmpleadoCero"))
    End Function

    Private Function PeriodoTexto() As String
        Return $"Nómina No. {_estado.Numero}  -  Del {_estado.Inicio:dd/MM/yyyy} al {_estado.Fin:dd/MM/yyyy}"
    End Function

    Private Sub BtnColillas_Click(sender As Object, e As EventArgs)

        If _estado Is Nothing Then Return

        Try
            Dim clave As String = CStr(CmbColilla.EditValue)
            Dim datos As DataTable = _planilla.ListarParaReporte(_estado.Numero, If(ChkPorDepartamento.Checked, "Departamento", ""), MostrarEmpleadosEnCero())
            Dim reporte As DevExpress.XtraReports.UI.XtraReport =CatalogoReportesNomina.Colilla(clave, New CsControlesDAL().ObtenerDatosEmpresa(), "Colilla de Pago",
                                                                          PeriodoTexto(), datos, ChkPorDepartamento.Checked)
            Registrar($"Se imprimio Colillas de la nomina: {_estado.Numero}")
            FrmVisorReporte.Mostrar(reporte, $"Colillas de la nómina {_estado.Numero}", Me)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo generar las colillas: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    ''' <summary>Botón "Reportes" de la V.5.0: abre FrmNominaActiva con la nómina que se está viendo.</summary>
    Private Sub BtnReportes_Click(sender As Object, e As EventArgs)

        If _estado Is Nothing Then Return

        Try
            Using frm As New FrmNominaActiva(_estado.Numero, PeriodoTexto(), MostrarEmpleadosEnCero())
                frm.ShowDialog(Me)
            End Using
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo abrir los reportes: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnNominaRpt_Click(sender As Object, e As EventArgs)

        If _estado Is Nothing Then Return

        Try
            Dim clave As String = CStr(CmbNominaRpt.EditValue)
            Dim datos As DataTable = _planilla.ListarParaReporte(_estado.Numero, CatalogoReportesNomina.OrdenNomina(clave), MostrarEmpleadosEnCero())
            Dim reporte As RptTabular = CatalogoReportesNomina.Nomina(clave, New CsControlesDAL().ObtenerDatosEmpresa(), "Nómina", PeriodoTexto(), datos)
            Registrar($"Se imprimio la nomina: {_estado.Numero}")
            FrmVisorReporte.Mostrar(reporte, $"Nómina {_estado.Numero}", Me)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo generar la nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

#End Region

    Private Sub BtnMonedas_Click(sender As Object, e As EventArgs)

        Try
            Using frm As New FrmMonedas13vo($"Nómina No. {_estado.Numero}", _planilla.ListarMontos(_estado.Numero))
                frm.ShowDialog(Me)
            End Using
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo abrir las denominaciones: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

#End Region

End Class
