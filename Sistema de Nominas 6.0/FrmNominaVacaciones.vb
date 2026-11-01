Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Nómina de vacaciones: la mitad "Vacaciones" de Frm13VacaMes de la V.5.0, separada del 13vo mes. Calcula, muestra el detalle,
''' imprime colillas y nómina, exporta a Excel y a los bancos, desglosa billetes/monedas, abre el historial salarial y cierra la
''' nómina. Solo UI: datos en <see cref="CsNominaVacacionesDAL"/>, reglas en <see cref="CalculadoraNominaVacaciones"/>. La interfaz se
''' arma por código (no hay Designer).
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmNominaVacaciones
    Inherits XtraForm

    Private ReadOnly _dal As New CsNominaVacacionesDAL
    Private _nomina As NominaPrestacion
    Private _detalle As DataTable
    Private _cargando As Boolean
    Private _calculando As Boolean

    Private CmbTipo As LookUpEdit
    Private TxtNumero As SpinEdit
    Private TxtIni, TxtFin, TxtPlanillaIni, TxtPlanillaFin As DateEdit
    Private TxtDescuento, TxtAntiguedad As SpinEdit
    Private LblEstado As LabelControl
    Private ChkSinTransferir, ChkRestarImpuestos, ChkPorDepartamento As CheckEdit
    Private Grid As GridControl
    Private Vista As GridView
    Private BtnCalcular, BtnCerrarNomina As SimpleButton
    Private BtnColilla, BtnNomina, BtnExcel, BtnBac, BtnBanpro, BtnBacentro, BtnMonedas, BtnHistorial, BtnSalir As SimpleButton

    Public Sub New()

        Text = "Nómina de Vacaciones"
        ClientSize = New Size(1040, 470)
        StartPosition = FormStartPosition.CenterParent

        CrearInterfaz()
        AddHandler Load, AddressOf FrmNominaVacaciones_Load

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        Dim superior As New PanelControl With {.Dock = DockStyle.Top, .Height = 112}
        EstiloGrid.AplicarBarra(superior)

        superior.Controls.Add(New LabelControl With {.Text = "Tipo de nómina:", .Location = New Point(12, 14)})
        CmbTipo = New LookUpEdit With {.Location = New Point(105, 11), .Size = New Size(220, 20)}
        CmbTipo.Properties.NullText = ""
        CmbTipo.Properties.Columns.Add(New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Nomina", "Nómina"))
        CmbTipo.Properties.ShowHeader = False
        CmbTipo.Properties.DisplayMember = "Nomina"
        CmbTipo.Properties.ValueMember = "CodTipoNomina"
        AddHandler CmbTipo.EditValueChanged, AddressOf CmbTipo_EditValueChanged

        superior.Controls.Add(New LabelControl With {.Text = "Nómina No.:", .Location = New Point(345, 14)})
        TxtNumero = Spin(New Point(415, 11), 70, 1, 999999)
        AddHandler TxtNumero.Validated, AddressOf TxtNumero_Validated

        superior.Controls.Add(New LabelControl With {.Text = "Vacaciones desde:", .Location = New Point(505, 14)})
        TxtIni = New DateEdit With {.Location = New Point(610, 11), .Size = New Size(100, 20)}
        superior.Controls.Add(New LabelControl With {.Text = "hasta:", .Location = New Point(722, 14)})
        TxtFin = New DateEdit With {.Location = New Point(762, 11), .Size = New Size(100, 20)}

        superior.Controls.Add(New LabelControl With {.Text = "Salarios de planillas desde:", .Location = New Point(12, 44)})
        TxtPlanillaIni = New DateEdit With {.Location = New Point(165, 41), .Size = New Size(100, 20)}
        superior.Controls.Add(New LabelControl With {.Text = "hasta:", .Location = New Point(277, 44)})
        TxtPlanillaFin = New DateEdit With {.Location = New Point(317, 41), .Size = New Size(100, 20)}

        superior.Controls.Add(New LabelControl With {.Text = "Días de descuento:", .Location = New Point(440, 44)})
        TxtDescuento = Spin(New Point(545, 41), 55, 0, 15)
        superior.Controls.Add(New LabelControl With {.Text = "Antigüedad mayor a (días):", .Location = New Point(620, 44)})
        TxtAntiguedad = Spin(New Point(765, 41), 55, 0, 15)

        BtnCalcular = New SimpleButton With {.Text = "Calcular", .Location = New Point(12, 72), .Size = New Size(110, 28)}
        BtnCalcular.ImageOptions.ImageUri.Uri = "Refresh"
        AddHandler BtnCalcular.Click, AddressOf BtnCalcular_Click

        BtnCerrarNomina = New SimpleButton With {.Text = "Cerrar nómina", .Location = New Point(132, 72), .Size = New Size(120, 28)}
        BtnCerrarNomina.ImageOptions.ImageUri.Uri = "Apply"
        AddHandler BtnCerrarNomina.Click, AddressOf BtnCerrarNomina_Click

        ChkSinTransferir = New CheckEdit With {.Text = "Cerrar sin transferir a la nómina", .Location = New Point(270, 75), .Width = 220}
        ChkRestarImpuestos = New CheckEdit With {.Text = "Restar INSS e IR", .Location = New Point(495, 75), .Width = 130}
        AddHandler ChkRestarImpuestos.CheckedChanged, Sub(s, e) If Not _cargando AndAlso _nomina IsNot Nothing Then CargarDetalle()

        LblEstado = New LabelControl With {.Location = New Point(640, 79), .AutoSizeMode = LabelAutoSizeMode.None, .Size = New Size(440, 16)}

        superior.Controls.AddRange(New Control() {CmbTipo, TxtNumero, TxtIni, TxtFin, TxtPlanillaIni, TxtPlanillaFin, TxtDescuento,
                                                  TxtAntiguedad, BtnCalcular, BtnCerrarNomina, ChkSinTransferir, ChkRestarImpuestos, LblEstado})

        Dim barra As New FlowLayoutPanel With {.Dock = DockStyle.Bottom, .Height = 86, .Padding = New Padding(8, 6, 8, 4), .WrapContents = True}
        BtnColilla = Boton("Colillas", "Print", 100)
        BtnNomina = Boton("Imprimir nómina", "Print", 135)
        BtnExcel = Boton("Planilla a Excel", "ExportToXLSX", 135)
        BtnBac = Boton("Banco BAC", "ExportToXLSX", 110)
        BtnBanpro = Boton("Banpro", "ExportToXLSX", 95)
        BtnBacentro = Boton("Bacentro", "ExportToXLSX", 100)
        BtnMonedas = Boton("Denominaciones", "Currency", 130)
        BtnHistorial = Boton("Historial salarial", "History", 135)
        BtnSalir = Boton("Salir", "Close", 80)
        ChkPorDepartamento = New CheckEdit With {.Text = "Colilla por departamento", .Width = 170, .Margin = New Padding(3, 8, 3, 3)}

        AddHandler BtnColilla.Click, AddressOf BtnColilla_Click
        AddHandler BtnNomina.Click, AddressOf BtnNomina_Click
        AddHandler BtnExcel.Click, AddressOf BtnExcel_Click
        AddHandler BtnBac.Click, AddressOf BtnBac_Click
        AddHandler BtnBanpro.Click, Sub(s, e) ExportarCreditoDebito(True)
        AddHandler BtnBacentro.Click, Sub(s, e) ExportarCreditoDebito(False)
        AddHandler BtnMonedas.Click, AddressOf BtnMonedas_Click
        AddHandler BtnHistorial.Click, AddressOf BtnHistorial_Click
        AddHandler BtnSalir.Click, Sub(s, e) Close()

        barra.Controls.AddRange(New Control() {BtnColilla, ChkPorDepartamento, BtnNomina, BtnExcel, BtnBac, BtnBanpro, BtnBacentro,
                                               BtnMonedas, BtnHistorial, BtnSalir})

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

    Private Shared Function Spin(ubicacion As Point, ancho As Integer, minimo As Decimal, maximo As Decimal) As SpinEdit
        Dim s As New SpinEdit With {.Location = ubicacion, .Size = New Size(ancho, 20)}
        s.Properties.MinValue = minimo
        s.Properties.MaxValue = maximo
        s.Properties.IsFloatValue = False
        Return s
    End Function

    Private Shared Function Boton(texto As String, icono As String, ancho As Integer) As SimpleButton
        Dim b As New SimpleButton With {.Text = texto, .Size = New Size(ancho, 30)}
        b.ImageOptions.ImageUri.Uri = icono
        Return b
    End Function

#End Region

#Region "CARGA"

    Private Sub FrmNominaVacaciones_Load(sender As Object, e As EventArgs)

        Try
            _cargando = True
            Dim tipos As DataTable = _dal.ListarTiposNomina()
            CmbTipo.Properties.DataSource = tipos
            If tipos.Rows.Count > 0 Then CmbTipo.EditValue = tipos.Rows(0)("CodTipoNomina")
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo leer los tipos de nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _cargando = False
        End Try

        CargarPropuesta()

    End Sub

    Private Sub CmbTipo_EditValueChanged(sender As Object, e As EventArgs)
        If Not _cargando Then CargarPropuesta()
    End Sub

    Private Sub TxtNumero_Validated(sender As Object, e As EventArgs)

        If _cargando Then Return

        Try
            Dim existente As NominaPrestacion = _dal.Buscar(Convert.ToInt32(TxtNumero.Value))
            Mostrar(If(existente, New NominaPrestacion With {.Numero = Convert.ToInt32(TxtNumero.Value), .Cod_Tipo_Nomina = TipoSeleccionado(),
                                                             .Fecha_Ini = TxtIni.DateTime.Date, .Fecha_Fin = TxtFin.DateTime.Date}))
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo leer la nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Function TipoSeleccionado() As String
        Return If(CmbTipo.EditValue Is Nothing, "", CStr(CmbTipo.EditValue))
    End Function

    Private Sub CargarPropuesta()

        If TipoSeleccionado() = "" Then Return

        Try
            Mostrar(_dal.Proponer(TipoSeleccionado()))
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo leer la nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub Mostrar(nomina As NominaPrestacion)

        _cargando = True
        Try
            _nomina = nomina
            TxtNumero.Value = nomina.Numero
            TxtIni.DateTime = nomina.Fecha_Ini
            TxtFin.DateTime = nomina.Fecha_Fin
            ' El rango de planillas propone el mismo período de las vacaciones; se puede cambiar antes de calcular.
            TxtPlanillaIni.DateTime = nomina.Fecha_Ini
            TxtPlanillaFin.DateTime = nomina.Fecha_Fin
            ChkSinTransferir.Checked = nomina.Transferir
            If nomina.Existe Then CmbTipo.EditValue = nomina.Cod_Tipo_Nomina
        Finally
            _cargando = False
        End Try

        CargarDetalle()
        ActualizarEstado()

    End Sub

    Private Sub CargarDetalle()

        _detalle = If(_nomina.Existe, _dal.ListarDetalle(_nomina.Numero, ChkRestarImpuestos.Checked), New DataTable)
        Grid.DataSource = _detalle

        Vista.PopulateColumns()
        If Vista.Columns.Count = 0 Then Return

        Vista.Columns("Codigo").Caption = "Código"
        Vista.Columns("Codigo").Width = 90
        Vista.Columns("Nombres").Caption = "Nombre del empleado"
        Vista.Columns("Nombres").Width = 260
        Formatear("SalarioMensual", "Salario promedio", 110)
        Formatear("DiasAPagar", "Días a pagar", 80)
        Formatear("DiasDescuento", "Días desc.", 80)
        Formatear("Adelanto", "Adelanto vac.", 100)
        Formatear("Inss", "INSS vac.", 90)
        Formatear("Ir", "IR vac.", 90)
        Formatear("TotalDevengado", "Total devengado", 110)
        Formatear("MontoPagar", "Monto a pagar", 110)

        Vista.Columns("MontoPagar").Summary.Add(DevExpress.Data.SummaryItemType.Sum, "MontoPagar", "{0:#,##0.00}")
        Vista.Columns("TotalDevengado").Summary.Add(DevExpress.Data.SummaryItemType.Sum, "TotalDevengado", "{0:#,##0.00}")

    End Sub

    Private Sub Formatear(campo As String, titulo As String, ancho As Integer)
        Dim c As GridColumn = Vista.Columns(campo)
        c.Caption = titulo
        c.Width = ancho
        c.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        c.DisplayFormat.FormatString = "#,##0.00"
    End Sub

    Private Sub ActualizarEstado()

        Dim abierta As Boolean = Not _nomina.Existe OrElse _nomina.Activa
        BtnCalcular.Enabled = abierta AndAlso Not _calculando
        BtnCerrarNomina.Enabled = _nomina.Existe AndAlso _nomina.Activa AndAlso Not _calculando
        For Each c As Control In {TxtIni, TxtFin, TxtPlanillaIni, TxtPlanillaFin, TxtDescuento, TxtAntiguedad, ChkSinTransferir}
            c.Enabled = abierta
        Next

        Dim hayDatos As Boolean = _nomina.Existe AndAlso _detalle IsNot Nothing AndAlso _detalle.Rows.Count > 0
        For Each b As SimpleButton In {BtnColilla, BtnNomina, BtnExcel, BtnBac, BtnBanpro, BtnBacentro, BtnMonedas}
            b.Enabled = hayDatos
        Next
        BtnHistorial.Enabled = _nomina.Existe

        LblEstado.Text = If(Not _nomina.Existe, "Nómina nueva: aún no calculada.",
                            If(_nomina.Activa, $"Nómina abierta - {_detalle.Rows.Count} empleados - salarios {_nomina.Monto_Pagado:#,##0.00}",
                               $"Nómina cerrada - {_detalle.Rows.Count} empleados"))

    End Sub

#End Region

#Region "CALCULAR Y CERRAR"

    Private Async Sub BtnCalcular_Click(sender As Object, e As EventArgs)

        If TipoSeleccionado() = "" Then Return

        Dim p As New CsNominaVacacionesDAL.ParametrosCalculo With {
            .Cod_Tipo_Nomina = TipoSeleccionado(), .Numero = Convert.ToInt32(TxtNumero.Value),
            .Fecha_Ini = TxtIni.DateTime.Date, .Fecha_Fin = TxtFin.DateTime.Date,
            .Planilla_Ini = TxtPlanillaIni.DateTime.Date, .Planilla_Fin = TxtPlanillaFin.DateTime.Date,
            .Dias_Descuento = Convert.ToDouble(TxtDescuento.Value), .Antiguedad_Minima = Convert.ToDouble(TxtAntiguedad.Value),
            .Transferir = ChkSinTransferir.Checked}

        Dim mensaje As String = If(_nomina IsNot Nothing AndAlso _nomina.Existe,
                                   $"Se recalculará la nómina de vacaciones {p.Numero} y se reemplazará su detalle.",
                                   $"Se calculará la nómina de vacaciones {p.Numero} del {p.Fecha_Ini:dd/MM/yyyy} al {p.Fecha_Fin:dd/MM/yyyy}.")
        If XtraMessageBox.Show(mensaje & vbCrLf & "¿Continuar?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        _calculando = True
        ActualizarEstado()
        Cursor = Cursors.WaitCursor

        Try
            Dim progreso As Action(Of Integer, Integer, String) =
                Sub(actual, total, texto)
                    BeginInvoke(Sub() LblEstado.Text = If(total > 0, $"Calculando {actual} de {total}...", texto))
                End Sub

            Dim r As CsNominaVacacionesDAL.ResultadoCalculo = Await Task.Run(Function() _dal.Calcular(p, progreso))

            _calculando = False
            Mostrar(_dal.Buscar(p.Numero))
            XtraMessageBox.Show($"Nómina calculada: {r.Grabados} empleados con vacaciones, de {r.Empleados} revisados.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            _calculando = False
            ActualizarEstado()
            XtraMessageBox.Show("No se pudo calcular la nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnCerrarNomina_Click(sender As Object, e As EventArgs)

        If XtraMessageBox.Show($"Al cerrar la nómina de vacaciones {_nomina.Numero} ya no se podrá recalcular." & vbCrLf & "¿Cerrarla?",
                               Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Try
            If Not _dal.Cerrar(_nomina.Numero, Not ChkSinTransferir.Checked) Then
                XtraMessageBox.Show("La nómina no existe o ya estaba cerrada.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            Mostrar(_dal.Buscar(_nomina.Numero))
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo cerrar la nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

#End Region

#Region "REPORTES Y EXPORTACION"

    Private Function Periodo() As String
        Return $"Vacaciones No. {_nomina.Numero}  -  Del {_nomina.Fecha_Ini:dd/MM/yyyy} al {_nomina.Fecha_Fin:dd/MM/yyyy}"
    End Function

    Private Sub BtnColilla_Click(sender As Object, e As EventArgs)

        Try
            Dim restar As Boolean = ChkRestarImpuestos.Checked
            Dim datos As DataTable = _dal.ListarParaReporte(_nomina.Numero, restar, "")

            Dim detalle As New List(Of ConceptoColilla) From {
                New ConceptoColilla("Salario promedio mensual", "SalarioMensual"),
                New ConceptoColilla("Días a pagar", "DiasAPagar", "{0:0.00}"),
                New ConceptoColilla("Días de descuento", "DiasDescuento", "{0:0.00}"),
                New ConceptoColilla("Días pagados", "TotalDias", "{0:0.00}")}
            Dim deducciones As New List(Of ConceptoColilla) From {New ConceptoColilla("Adelanto de vacaciones", "Adelanto")}
            If restar Then
                deducciones.Add(New ConceptoColilla("INSS de vacaciones", "Inss"))
                deducciones.Add(New ConceptoColilla("IR de vacaciones", "Ir"))
            End If

            Dim reporte As New RptColillaPago(New CsControlesDAL().ObtenerDatosEmpresa(), "Colilla de Vacaciones", Periodo(), datos,
                                              detalle, deducciones, ChkPorDepartamento.Checked)
            FrmVisorReporte.Mostrar(reporte, $"Colillas Vacaciones {_nomina.Numero}", Me)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo generar las colillas: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnNomina_Click(sender As Object, e As EventArgs)

        Try
            Dim columnas As New List(Of ColumnaReporte) From {
                New ColumnaReporte("Código", "Codigo", 60),
                New ColumnaReporte("Nombres", "Nombres", 190),
                New ColumnaReporte("Cédula", "Cedula", 85),
                New ColumnaReporte("No. INSS", "NumeroInss", 70),
                New ColumnaReporte("Salario promedio", "SalarioMensual", 85, "#,##0.00", True),
                New ColumnaReporte("Días pagar", "DiasAPagar", 55, "0.00"),
                New ColumnaReporte("Días desc.", "DiasDescuento", 55, "0.00"),
                New ColumnaReporte("Adelanto", "Adelanto", 75, "#,##0.00", True),
                New ColumnaReporte("Total devengado", "TotalDevengado", 85, "#,##0.00", True),
                New ColumnaReporte("INSS", "Inss", 70, "#,##0.00", True),
                New ColumnaReporte("IR", "Ir", 70, "#,##0.00", True),
                New ColumnaReporte("Monto a pagar", "MontoPagar", 85, "#,##0.00", True)}

            Dim reporte As New RptTabular(New CsControlesDAL().ObtenerDatosEmpresa(), "Nómina de Vacaciones", Periodo(),
                                          _dal.ListarParaReporte(_nomina.Numero, ChkRestarImpuestos.Checked, ""), columnas, "Departamento", "Departamento")
            FrmVisorReporte.Mostrar(reporte, $"Nomina Vacaciones {_nomina.Numero}", Me)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo generar la nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs)

        Try
            Dim ruta As String = ExportadorGrid.ExportarAExcel(Vista, $"Nomina Vacaciones {_nomina.Numero}")
            If ruta <> "" Then XtraMessageBox.Show("Archivo exportado:" & vbCrLf & ruta, Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo exportar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnBac_Click(sender As Object, e As EventArgs)

        Using dlg As New FrmDatosBanco(FrmDatosBanco.Formato.Bac, "Exportar a BAC", "Vacaciones")

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

                Dim tabla As DataTable = ExportadorBancos.FormatoBac(_dal.ListarParaBanco(_nomina.Numero, ChkRestarImpuestos.Checked), dlg.CodigoCliente,
                                                                     dlg.Consecutivo, dlg.Fecha, dlg.Concepto, tasa)
                Guardar(tabla, $"BAC Vacaciones {_nomina.Numero}")
            Catch ex As Exception
                XtraMessageBox.Show("No se pudo exportar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End Using

    End Sub

    Private Sub ExportarCreditoDebito(esBanpro As Boolean)

        Dim nombre As String = If(esBanpro, "Banpro", "Bacentro")

        Using dlg As New FrmDatosBanco(FrmDatosBanco.Formato.CreditoDebito, "Exportar a " & nombre, "Vacaciones")

            If dlg.ShowDialog(Me) <> DialogResult.OK Then Return

            Try
                Dim columna As String = If(esBanpro, ExportadorBancos.CuentaBanpro, ExportadorBancos.CuentaBac)
                Dim empresa As DatosEmpresa = New CsControlesDAL().ObtenerDatosEmpresa()
                Dim tabla As DataTable = ExportadorBancos.FormatoCreditoDebito(_dal.ListarParaBanco(_nomina.Numero, ChkRestarImpuestos.Checked), columna,
                                                                               dlg.Concepto, empresa.Nombre_Empresa, dlg.CuentaEmpresa)
                Guardar(tabla, $"{nombre} Vacaciones {_nomina.Numero}")
            Catch ex As Exception
                XtraMessageBox.Show("No se pudo exportar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End Using

    End Sub

    Private Sub Guardar(tabla As DataTable, nombre As String)
        Dim ruta As String = ExportadorGrid.ExportarTabla(tabla, nombre)
        If ruta <> "" Then XtraMessageBox.Show("Archivo exportado:" & vbCrLf & ruta, Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub BtnMonedas_Click(sender As Object, e As EventArgs)

        Try
            Using frm As New FrmMonedas13vo($"Vacaciones No. {_nomina.Numero}", _dal.ListarMontos(_nomina.Numero, ChkRestarImpuestos.Checked))
                frm.ShowDialog(Me)
            End Using
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo abrir las denominaciones: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnHistorial_Click(sender As Object, e As EventArgs)
        Using frm As New FrmSalarioHistorial(CsHistorialSalarialDAL.TipoVacaciones, 0, _nomina.Numero)
            frm.ShowDialog(Me)
        End Using
    End Sub

#End Region

End Class
