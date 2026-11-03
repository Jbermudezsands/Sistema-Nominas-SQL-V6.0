Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Reportes de nómina (FrmReportes de la V.5.0, rediseñado): se elige el reporte de una lista, el tipo de nómina y el rango de fechas,
''' se consulta en pantalla y se imprime con DevExpress (<see cref="RptTabular"/>) o se exporta a Excel. Solo UI: las consultas y las
''' columnas de cada reporte están en <see cref="CsReportesNominaDAL"/>. La interfaz se arma por código (no hay Designer).
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmReportesNomina
    Inherits XtraForm

    Private ReadOnly _dal As New CsReportesNominaDAL
    Private _datos As DataTable
    Private _consultado As CsReportesNominaDAL.DefinicionReporte
    Private _cargando As Boolean

    Private LstReportes As ListBoxControl
    Private LblDescripcion As LabelControl
    Private CmbTipo As LookUpEdit
    Private FechaInicio, FechaFin As DateEdit
    Private ChkSaldo As CheckEdit
    Private TxtSaldo, TxtNumero As SpinEdit
    Private LblNumero As LabelControl
    Private Grid As GridControl
    Private Vista As GridView
    Private BtnConsultar, BtnImprimir, BtnExcel, BtnSalir As SimpleButton
    Private LblResultado As LabelControl

    Public Sub New()

        Text = "Reportes de Nómina"
        ClientSize = New Size(1080, 500)
        MinimumSize = New Size(700, 400)
        StartPosition = FormStartPosition.CenterParent

        CrearInterfaz()
        AddHandler Load, AddressOf FrmReportesNomina_Load

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        Dim superior As New PanelControl With {.Dock = DockStyle.Top, .Height = 48}
        EstiloGrid.AplicarBarra(superior)

        superior.Controls.Add(New LabelControl With {.Text = "Tipo de nómina:", .Location = New Point(12, 16)})
        CmbTipo = New LookUpEdit With {.Location = New Point(105, 13), .Size = New Size(230, 20)}
        CmbTipo.Properties.NullText = ""
        CmbTipo.Properties.Columns.Add(New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Nomina", ""))
        CmbTipo.Properties.ShowHeader = False
        CmbTipo.Properties.DisplayMember = "Nomina"
        CmbTipo.Properties.ValueMember = "CodTipoNomina"
        AddHandler CmbTipo.EditValueChanged, Sub(s, e) If Not _cargando Then ProponerFechas()

        superior.Controls.Add(New LabelControl With {.Text = "Desde:", .Location = New Point(355, 16)})
        FechaInicio = New DateEdit With {.Location = New Point(395, 13), .Size = New Size(110, 20)}
        superior.Controls.Add(New LabelControl With {.Text = "Hasta:", .Location = New Point(520, 16)})
        FechaFin = New DateEdit With {.Location = New Point(558, 13), .Size = New Size(110, 20)}

        BtnConsultar = Boton("Consultar", "Refresh", 110)
        BtnConsultar.Location = New Point(690, 9)
        AddHandler BtnConsultar.Click, AddressOf BtnConsultar_Click

        ChkSaldo = New CheckEdit With {.Text = "Saldo mayor a:", .Location = New Point(815, 14), .Width = 100, .Visible = False}
        TxtSaldo = New SpinEdit With {.Location = New Point(918, 13), .Size = New Size(70, 20), .Visible = False}
        TxtSaldo.Properties.DisplayFormat.FormatString = "0.00"
        TxtSaldo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric

        LblNumero = New LabelControl With {.Text = "Nómina No.:", .Location = New Point(815, 16), .Visible = False}
        TxtNumero = New SpinEdit With {.Location = New Point(885, 13), .Size = New Size(70, 20), .Visible = False}
        TxtNumero.Properties.IsFloatValue = False
        TxtNumero.Properties.MinValue = 0

        superior.Controls.AddRange(New Control() {CmbTipo, FechaInicio, FechaFin, BtnConsultar, ChkSaldo, TxtSaldo, LblNumero, TxtNumero})

        Dim izquierda As New PanelControl With {.Dock = DockStyle.Left, .Width = 270}
        EstiloGrid.AplicarBarra(izquierda)
        LblDescripcion = New LabelControl With {.Dock = DockStyle.Bottom, .AutoSizeMode = LabelAutoSizeMode.Vertical, .Padding = New Padding(8)}
        LblDescripcion.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
        LblDescripcion.Height = 70
        LstReportes = New ListBoxControl With {.Dock = DockStyle.Fill}
        AddHandler LstReportes.SelectedIndexChanged, AddressOf LstReportes_SelectedIndexChanged
        AddHandler LstReportes.DoubleClick, AddressOf BtnConsultar_Click
        izquierda.Controls.Add(LstReportes)
        izquierda.Controls.Add(LblDescripcion)

        Dim barra As New FlowLayoutPanel With {.Dock = DockStyle.Bottom, .Height = 46, .Padding = New Padding(8, 6, 8, 4)}
        BtnImprimir = Boton("Ver / imprimir", "Print", 130)
        BtnExcel = Boton("Exportar a Excel", "ExportToXLSX", 140)
        BtnSalir = Boton("Salir", "Close", 80)
        AddHandler BtnImprimir.Click, AddressOf BtnImprimir_Click
        AddHandler BtnExcel.Click, AddressOf BtnExcel_Click
        AddHandler BtnSalir.Click, Sub(s, e) Close()
        LblResultado = New LabelControl With {.AutoSizeMode = LabelAutoSizeMode.None, .Size = New Size(520, 16), .Margin = New Padding(12, 9, 0, 0)}
        barra.Controls.AddRange(New Control() {BtnImprimir, BtnExcel, BtnSalir, LblResultado})

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
        Controls.Add(izquierda)
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

    Private Sub FrmReportesNomina_Load(sender As Object, e As EventArgs)

        If MdiParent IsNot Nothing Then WindowState = FormWindowState.Maximized

        Try
            _cargando = True

            For Each d As CsReportesNominaDAL.DefinicionReporte In CsReportesNominaDAL.Catalogo()
                LstReportes.Items.Add(d.Nombre)
            Next
            LstReportes.SelectedIndex = 0

            Dim tipos As DataTable = _dal.ListarTipos()
            Dim todos As DataRow = tipos.NewRow()
            todos("CodTipoNomina") = ""
            todos("Nomina") = "(Todos los tipos)"
            tipos.Rows.InsertAt(todos, 0)
            CmbTipo.Properties.DataSource = tipos
            CmbTipo.EditValue = ""
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo preparar los reportes: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _cargando = False
        End Try

        ProponerFechas()
        ActualizarBotones()

    End Sub

    Private Function TipoSeleccionado() As String
        Return If(CmbTipo.EditValue Is Nothing, "", CStr(CmbTipo.EditValue))
    End Function

    Private Function ReporteSeleccionado() As CsReportesNominaDAL.DefinicionReporte
        Return CsReportesNominaDAL.Catalogo()(Math.Max(0, LstReportes.SelectedIndex))
    End Function

    ''' <summary>Propone como rango el de todas las nóminas calculadas del tipo elegido.</summary>
    Private Sub ProponerFechas()

        Try
            Dim rango As Tuple(Of Date, Date) = _dal.RangoDisponible(TipoSeleccionado())
            If rango IsNot Nothing Then
                FechaInicio.DateTime = rango.Item1
                FechaFin.DateTime = rango.Item2
            Else
                FechaInicio.DateTime = Date.Today
                FechaFin.DateTime = Date.Today
            End If
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo leer las fechas de las nóminas: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub LstReportes_SelectedIndexChanged(sender As Object, e As EventArgs)

        If LstReportes.SelectedIndex < 0 Then Return

        Dim d As CsReportesNominaDAL.DefinicionReporte = ReporteSeleccionado()
        LblDescripcion.Text = d.Descripcion
        LblNumero.Visible = d.UsaNumeroNomina
        TxtNumero.Visible = d.UsaNumeroNomina
        If d.UsaNumeroNomina AndAlso TxtNumero.Value = 0 Then
            Try
                TxtNumero.Value = _dal.UltimaNomina(TipoSeleccionado())
            Catch
                ' El usuario puede escribir el número.
            End Try
        End If
        CmbTipo.Enabled = d.UsaRango OrElse d.UsaFechaCorte OrElse d.UsaNumeroNomina
        FechaInicio.Enabled = d.UsaRango
        FechaFin.Enabled = d.UsaRango OrElse d.UsaFechaCorte
        ChkSaldo.Visible = d.UsaSaldoMinimo
        TxtSaldo.Visible = d.UsaSaldoMinimo

    End Sub

    Private Sub ActualizarBotones()
        Dim hay As Boolean = _datos IsNot Nothing AndAlso _datos.Rows.Count > 0
        BtnImprimir.Enabled = hay
        BtnExcel.Enabled = hay
    End Sub

#End Region

#Region "CONSULTAR, IMPRIMIR Y EXPORTAR"

    Private Sub BtnConsultar_Click(sender As Object, e As EventArgs)

        Dim d As CsReportesNominaDAL.DefinicionReporte = ReporteSeleccionado()

        If d.UsaRango AndAlso FechaInicio.DateTime.Date > FechaFin.DateTime.Date Then
            XtraMessageBox.Show("La fecha inicial es mayor que la final.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor
            Dim saldoMinimo As Double? = If(d.UsaSaldoMinimo AndAlso ChkSaldo.Checked, CType(Convert.ToDouble(TxtSaldo.Value), Double?), Nothing)
            _datos = _dal.Consultar(d.Clave, TipoSeleccionado(), FechaInicio.DateTime, FechaFin.DateTime, saldoMinimo, Convert.ToInt32(TxtNumero.Value))
            _consultado = d
            MostrarResultado()
        Catch ex As Exception
            _datos = Nothing
            XtraMessageBox.Show("No se pudo consultar el reporte: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try

        ActualizarBotones()

    End Sub

    Private Sub MostrarResultado()

        Vista.Columns.Clear()
        Grid.DataSource = _datos
        Vista.PopulateColumns()

        For Each c As ColumnaReporte In _consultado.Columnas
            Dim col As DevExpress.XtraGrid.Columns.GridColumn = Vista.Columns.ColumnByFieldName(c.Campo)
            If col Is Nothing Then Continue For
            col.Caption = c.Titulo
            col.Width = CInt(c.Ancho * 1.3)
            If c.Formato <> "" Then
                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom
                col.DisplayFormat.FormatString = c.Formato
            End If
            If c.Sumar Then
                col.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum
                col.SummaryItem.DisplayFormat = "{0:" & c.Formato & "}"
            End If
        Next

        ' Las columnas que el reporte no muestra (por ejemplo Departamento, si es el grupo) quedan ocultas.
        For Each col As DevExpress.XtraGrid.Columns.GridColumn In Vista.Columns
            If Not _consultado.Columnas.Any(Function(c) String.Equals(c.Campo, col.FieldName, StringComparison.OrdinalIgnoreCase)) Then col.Visible = (col.FieldName = _consultado.CampoGrupo)
        Next

        LblResultado.Text = $"{_datos.Rows.Count} filas - {_consultado.Nombre}"

    End Sub

    Private Function PeriodoTexto() As String
        Dim tipo As String = If(CmbTipo.Text = "" OrElse TipoSeleccionado() = "", "Todos los tipos de nómina", CmbTipo.Text)
        If _consultado.UsaNumeroNomina Then Return $"Nómina No. {TxtNumero.Value:0}"
        If _consultado.UsaFechaCorte Then Return $"{tipo}  -  Al {FechaFin.DateTime:dd/MM/yyyy}"
        If Not _consultado.UsaRango Then Return $"Al {Date.Today:dd/MM/yyyy}"
        Return $"{tipo}  -  Del {FechaInicio.DateTime:dd/MM/yyyy} al {FechaFin.DateTime:dd/MM/yyyy}"
    End Function

    Private Sub BtnImprimir_Click(sender As Object, e As EventArgs)

        If _datos Is Nothing OrElse _consultado Is Nothing Then Return

        Try
            Dim reporte As New RptTabular(New CsControlesDAL().ObtenerDatosEmpresa(), _consultado.Nombre, PeriodoTexto(), _datos,
                                          _consultado.Columnas, _consultado.CampoGrupo, _consultado.TituloGrupo)
            FrmVisorReporte.Mostrar(reporte, _consultado.Nombre, Me)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo generar el reporte: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs)

        If _datos Is Nothing OrElse _consultado Is Nothing Then Return

        Try
            Dim ruta As String = ExportadorGrid.ExportarAExcel(Vista, _consultado.Nombre)
            If ruta <> "" Then XtraMessageBox.Show("Archivo exportado:" & vbCrLf & ruta, Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo exportar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

#End Region

End Class
