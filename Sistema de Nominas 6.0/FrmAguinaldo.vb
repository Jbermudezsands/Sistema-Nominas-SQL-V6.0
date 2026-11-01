Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Nómina de 13vo mes (aguinaldo): la mitad "13vo" de Frm13VacaMes de la V.5.0, separada de vacaciones. Calcula, muestra el
''' detalle, imprime colillas y nómina, exporta a Excel y a los bancos, desglosa billetes/monedas, abre el historial salarial y
''' cierra la nómina. Solo UI: datos en <see cref="CsAguinaldoDAL"/>, reglas en <see cref="CalculadoraAguinaldo"/>. La interfaz se
''' arma por código (no hay Designer).
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmAguinaldo
    Inherits XtraForm

    Private ReadOnly _dal As New CsAguinaldoDAL
    Private _nomina As NominaPrestacion
    Private _detalle As DataTable
    Private _cargando As Boolean
    Private _calculando As Boolean

    Private CmbTipo As LookUpEdit
    Private TxtNumero As SpinEdit
    Private TxtIni, TxtFin As DateEdit
    Private TxtAntiguedad As SpinEdit
    Private LblEstado As LabelControl
    Private ChkPorDepartamento As CheckEdit
    Private Grid As GridControl
    Private Vista As GridView
    Private BtnCalcular, BtnCerrarNomina As SimpleButton
    Private BtnColilla, BtnNomina, BtnExcel, BtnBac, BtnBanpro, BtnBacentro, BtnMonedas, BtnHistorial, BtnSalir As SimpleButton

    Public Sub New()

        Text = "Nómina de 13vo Mes"
        ClientSize = New Size(1040, 440)
        StartPosition = FormStartPosition.CenterParent

        CrearInterfaz()
        AddHandler Load, AddressOf FrmAguinaldo_Load

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        Dim superior As New PanelControl With {.Dock = DockStyle.Top, .Height = 76}
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
        TxtNumero = New SpinEdit With {.Location = New Point(415, 11), .Size = New Size(70, 20)}
        TxtNumero.Properties.IsFloatValue = False
        TxtNumero.Properties.MinValue = 1
        TxtNumero.Properties.MaxValue = 999999
        AddHandler TxtNumero.Validated, AddressOf TxtNumero_Validated

        superior.Controls.Add(New LabelControl With {.Text = "Desde:", .Location = New Point(505, 14)})
        TxtIni = New DateEdit With {.Location = New Point(548, 11), .Size = New Size(100, 20)}
        superior.Controls.Add(New LabelControl With {.Text = "Hasta:", .Location = New Point(660, 14)})
        TxtFin = New DateEdit With {.Location = New Point(702, 11), .Size = New Size(100, 20)}

        superior.Controls.Add(New LabelControl With {.Text = "Antigüedad mayor a (días):", .Location = New Point(820, 14)})
        TxtAntiguedad = New SpinEdit With {.Location = New Point(975, 11), .Size = New Size(60, 20)}
        TxtAntiguedad.Properties.MinValue = 0
        TxtAntiguedad.Properties.MaxValue = 365

        BtnCalcular = New SimpleButton With {.Text = "Calcular", .Location = New Point(12, 40), .Size = New Size(110, 28)}
        BtnCalcular.ImageOptions.ImageUri.Uri = "Refresh"
        AddHandler BtnCalcular.Click, AddressOf BtnCalcular_Click

        BtnCerrarNomina = New SimpleButton With {.Text = "Cerrar nómina", .Location = New Point(132, 40), .Size = New Size(120, 28)}
        BtnCerrarNomina.ImageOptions.ImageUri.Uri = "Apply"
        AddHandler BtnCerrarNomina.Click, AddressOf BtnCerrarNomina_Click

        LblEstado = New LabelControl With {.Location = New Point(270, 47), .AutoSizeMode = LabelAutoSizeMode.None, .Size = New Size(700, 16)}

        superior.Controls.AddRange(New Control() {CmbTipo, TxtNumero, TxtIni, TxtFin, TxtAntiguedad, BtnCalcular, BtnCerrarNomina, LblEstado})

        ' Franja inferior de botones: se acomodan en varias filas si la ventana es angosta.
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
        AddHandler BtnBanpro.Click, Sub(s, e) ExportarCreditoDebito(CsAguinaldoBancos.Banpro)
        AddHandler BtnBacentro.Click, Sub(s, e) ExportarCreditoDebito(CsAguinaldoBancos.Bacentro)
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

    Private Shared Function Boton(texto As String, icono As String, ancho As Integer) As SimpleButton
        Dim b As New SimpleButton With {.Text = texto, .Size = New Size(ancho, 30)}
        b.ImageOptions.ImageUri.Uri = icono
        Return b
    End Function

#End Region

#Region "CARGA"

    Private Sub FrmAguinaldo_Load(sender As Object, e As EventArgs)

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
            If existente IsNot Nothing Then
                Mostrar(existente)
            Else
                ' Número nuevo: conserva las fechas, la nómina se crea al calcular.
                _nomina = New NominaPrestacion With {.Numero = Convert.ToInt32(TxtNumero.Value), .Cod_Tipo_Nomina = TipoSeleccionado(),
                                                     .Fecha_Ini = TxtIni.DateTime.Date, .Fecha_Fin = TxtFin.DateTime.Date}
                Mostrar(_nomina)
            End If
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo leer la nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Function TipoSeleccionado() As String
        Return If(CmbTipo.EditValue Is Nothing, "", CStr(CmbTipo.EditValue))
    End Function

    ''' <summary>Muestra la nómina abierta del tipo de nómina elegido, o la propuesta (siguiente número y fechas) si no hay.</summary>
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
            If nomina.Existe Then CmbTipo.EditValue = nomina.Cod_Tipo_Nomina
        Finally
            _cargando = False
        End Try

        CargarDetalle()
        ActualizarEstado()

    End Sub

    Private Sub CargarDetalle()

        _detalle = If(_nomina.Existe, _dal.ListarDetalle(_nomina.Numero), New DataTable)
        Grid.DataSource = _detalle

        Vista.PopulateColumns()
        If Vista.Columns.Count = 0 Then Return

        Vista.Columns("Codigo").Caption = "Código"
        Vista.Columns("Codigo").Width = 90
        Vista.Columns("Nombres").Caption = "Nombre del empleado"
        Vista.Columns("Nombres").Width = 280
        Formatear("SalarioMensual", "Salario mensual", 120)
        Formatear("DiasAPagar", "Días a pagar", 90)
        Formatear("SalarioAPagar", "Salario a pagar", 120)
        Formatear("Adelanto", "Adelanto 13vo", 110)
        Formatear("MontoPagar", "Monto a pagar", 120)

        Vista.Columns("MontoPagar").Summary.Add(DevExpress.Data.SummaryItemType.Sum, "MontoPagar", "{0:#,##0.00}")
        Vista.Columns("SalarioAPagar").Summary.Add(DevExpress.Data.SummaryItemType.Sum, "SalarioAPagar", "{0:#,##0.00}")

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
        TxtIni.Enabled = abierta
        TxtFin.Enabled = abierta
        TxtAntiguedad.Enabled = abierta

        Dim hayDatos As Boolean = _nomina.Existe AndAlso _detalle IsNot Nothing AndAlso _detalle.Rows.Count > 0
        For Each b As SimpleButton In {BtnColilla, BtnNomina, BtnExcel, BtnBac, BtnBanpro, BtnBacentro, BtnMonedas, BtnHistorial}
            b.Enabled = hayDatos
        Next
        BtnHistorial.Enabled = _nomina.Existe

        LblEstado.Text = If(Not _nomina.Existe, "Nómina nueva: aún no calculada.",
                            If(_nomina.Activa, $"Nómina abierta - {_detalle.Rows.Count} empleados - monto calculado {_nomina.Monto_Pagado:#,##0.00}",
                               $"Nómina cerrada - {_detalle.Rows.Count} empleados"))

    End Sub

#End Region

#Region "CALCULAR Y CERRAR"

    Private Async Sub BtnCalcular_Click(sender As Object, e As EventArgs)

        If TipoSeleccionado() = "" Then Return

        Dim numero As Integer = Convert.ToInt32(TxtNumero.Value)
        Dim ini As Date = TxtIni.DateTime.Date
        Dim fin As Date = TxtFin.DateTime.Date
        Dim tipo As String = TipoSeleccionado()
        Dim antiguedad As Double = Convert.ToDouble(TxtAntiguedad.Value)

        If fin < ini Then
            XtraMessageBox.Show("La fecha final es menor que la inicial.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim mensaje As String = If(_nomina IsNot Nothing AndAlso _nomina.Existe,
                                   $"Se recalculará la nómina de 13vo mes {numero} y se reemplazará su detalle.",
                                   $"Se calculará la nómina de 13vo mes {numero} del {ini:dd/MM/yyyy} al {fin:dd/MM/yyyy}.")
        If XtraMessageBox.Show(mensaje & vbCrLf & "¿Continuar?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        _calculando = True
        ActualizarEstado()
        Cursor = Cursors.WaitCursor

        Try
            Dim progreso As Action(Of Integer, Integer, String) =
                Sub(actual, total, texto)
                    BeginInvoke(Sub() LblEstado.Text = If(total > 0, $"Calculando {actual} de {total}...", texto))
                End Sub

            Dim r As CsAguinaldoDAL.ResultadoCalculo = Await Task.Run(Function() _dal.Calcular(tipo, numero, ini, fin, antiguedad, progreso))

            _calculando = False
            Mostrar(_dal.Buscar(numero))
            XtraMessageBox.Show($"Nómina calculada: {r.Grabados} empleados con pago, de {r.Empleados} revisados.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            _calculando = False
            ActualizarEstado()
            XtraMessageBox.Show("No se pudo calcular la nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnCerrarNomina_Click(sender As Object, e As EventArgs)

        If XtraMessageBox.Show($"Al cerrar la nómina de 13vo mes {_nomina.Numero} ya no se podrá recalcular." & vbCrLf & "¿Cerrarla?",
                               Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Try
            If Not _dal.Cerrar(_nomina.Numero) Then
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
        Return $"13vo Mes No. {_nomina.Numero}  -  Desde {_nomina.Fecha_Ini:dd/MM/yyyy} hasta {_nomina.Fecha_Fin:dd/MM/yyyy}"
    End Function

    Private Sub BtnColilla_Click(sender As Object, e As EventArgs)

        Try
            Dim datos As DataTable = _dal.ListarParaReporte(_nomina.Numero, "")
            Dim detalle As New List(Of ConceptoColilla) From {
                New ConceptoColilla("Salario mensual", "SalarioMensual"),
                New ConceptoColilla("Días a pagar", "DiasAPagar", "{0:0.00}")}
            Dim deducciones As New List(Of ConceptoColilla) From {New ConceptoColilla("Adelanto de 13vo mes", "Adelanto")}

            Dim reporte As New RptColillaPago(New CsControlesDAL().ObtenerDatosEmpresa(), "Colilla de 13vo Mes", Periodo(), datos,
                                              detalle, deducciones, ChkPorDepartamento.Checked)
            FrmVisorReporte.Mostrar(reporte, $"Colillas 13vo Mes {_nomina.Numero}", Me)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo generar las colillas: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnNomina_Click(sender As Object, e As EventArgs)

        Try
            Dim columnas As New List(Of ColumnaReporte) From {
                New ColumnaReporte("Código", "Codigo", 70),
                New ColumnaReporte("Nombres", "Nombres", 260),
                New ColumnaReporte("Fecha ingreso", "FechaIngreso", 80, "dd/MM/yyyy"),
                New ColumnaReporte("Salario mensual", "SalarioMensual", 100, "#,##0.00", True),
                New ColumnaReporte("Días a pagar", "DiasAPagar", 70, "0.00"),
                New ColumnaReporte("Total devengado", "TotalDevengado", 100, "#,##0.00", True),
                New ColumnaReporte("Deducciones", "TotalDeducir", 90, "#,##0.00", True),
                New ColumnaReporte("Monto a pagar", "MontoPagar", 100, "#,##0.00", True)}

            Dim reporte As New RptTabular(New CsControlesDAL().ObtenerDatosEmpresa(), "Nómina de 13vo Mes", Periodo(),
                                          _dal.ListarParaReporte(_nomina.Numero, ""), columnas, "Departamento", "Departamento")
            FrmVisorReporte.Mostrar(reporte, $"Nomina 13vo Mes {_nomina.Numero}", Me)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo generar la nómina: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs)

        Try
            Dim ruta As String = ExportadorGrid.ExportarAExcel(Vista, $"Nomina 13vo Mes {_nomina.Numero}")
            If ruta <> "" Then XtraMessageBox.Show("Archivo exportado:" & vbCrLf & ruta, Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo exportar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnBac_Click(sender As Object, e As EventArgs)

        Using dlg As New FrmDatosBanco(FrmDatosBanco.Formato.Bac, "Exportar a BAC", "13vo Mes")

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

                Dim tabla As DataTable = ExportadorBancos.FormatoBac(_dal.ListarParaBanco(_nomina.Numero), dlg.CodigoCliente, dlg.Consecutivo,
                                                                     dlg.Fecha, dlg.Concepto, tasa)
                Guardar(tabla, $"BAC 13vo Mes {_nomina.Numero}")
            Catch ex As Exception
                XtraMessageBox.Show("No se pudo exportar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End Using

    End Sub

    Private Sub ExportarCreditoDebito(banco As CsAguinaldoBancos)

        Dim nombre As String = If(banco = CsAguinaldoBancos.Banpro, "Banpro", "Bacentro")

        Using dlg As New FrmDatosBanco(FrmDatosBanco.Formato.CreditoDebito, "Exportar a " & nombre, "13vo Mes")

            If dlg.ShowDialog(Me) <> DialogResult.OK Then Return

            Try
                Dim columna As String = If(banco = CsAguinaldoBancos.Banpro, ExportadorBancos.CuentaBanpro, ExportadorBancos.CuentaBac)
                Dim empresa As DatosEmpresa = New CsControlesDAL().ObtenerDatosEmpresa()
                Dim tabla As DataTable = ExportadorBancos.FormatoCreditoDebito(_dal.ListarParaBanco(_nomina.Numero), columna, dlg.Concepto,
                                                                               empresa.Nombre_Empresa, dlg.CuentaEmpresa)
                Guardar(tabla, $"{nombre} 13vo Mes {_nomina.Numero}")
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
            Using frm As New FrmMonedas13vo($"13vo Mes No. {_nomina.Numero}", _dal.ListarMontos(_nomina.Numero))
                frm.ShowDialog(Me)
            End Using
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo abrir las denominaciones: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnHistorial_Click(sender As Object, e As EventArgs)
        Using frm As New FrmSalarioHistorial(CsHistorialSalarialDAL.TipoAguinaldo, _nomina.Numero, 0)
            frm.ShowDialog(Me)
        End Using
    End Sub

#End Region

    Private Enum CsAguinaldoBancos
        Banpro
        Bacentro
    End Enum

End Class
