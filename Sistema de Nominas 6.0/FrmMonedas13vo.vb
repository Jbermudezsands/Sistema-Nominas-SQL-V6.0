Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Denominaciones de billetes y monedas para pagar una nómina de 13vo mes o de vacaciones (V.5.0 FrmMonedas13vo,
''' que servía a las dos según la variable global <c>Quien</c>). Recibe los montos a pagar por empleado y calcula el
''' desglose con <see cref="CalculadoraDenominaciones"/>; el botón Imprimir abre <see cref="RptDenominaciones"/>.
''' La interfaz se arma por código (no hay Designer).
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmMonedas13vo
    Inherits XtraForm

    Private ReadOnly _titulo As String
    Private ReadOnly _montos As DataTable
    Private _totales As Integer()
    Private _granTotal As Double

    Private Grid As GridControl
    Private Vista As GridView
    Private LblGranTotal As LabelControl
    Private BtnImprimir, BtnCerrar As SimpleButton

    ''' <param name="titulo">Nómina que se paga, por ejemplo "Aguinaldo No. 3".</param>
    ''' <param name="montos">Una fila por empleado con las columnas <c>Codigo</c>, <c>Nombres</c> y <c>Monto</c>.</param>
    Public Sub New(titulo As String, montos As DataTable)

        _titulo = titulo
        _montos = montos

        Text = "Denominaciones - " & titulo
        ClientSize = New Size(1040, 440)
        StartPosition = FormStartPosition.CenterParent
        MinimizeBox = False

        CrearInterfaz()
        AddHandler Load, AddressOf FrmMonedas13vo_Load

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        Dim barra As New PanelControl With {.Dock = DockStyle.Bottom, .Height = 46}
        EstiloGrid.AplicarBarra(barra)

        LblGranTotal = New LabelControl With {.Location = New Point(12, 14), .AutoSizeMode = LabelAutoSizeMode.None, .Size = New Size(520, 20)}
        LblGranTotal.Appearance.Font = New Font(LblGranTotal.Appearance.Font, FontStyle.Bold)

        BtnCerrar = New SimpleButton With {.Text = "Cerrar", .Size = New Size(95, 30), .Anchor = AnchorStyles.Top Or AnchorStyles.Right}
        BtnCerrar.ImageOptions.ImageUri.Uri = "Close"
        BtnCerrar.Location = New Point(barra.Width - 107, 8)
        BtnImprimir = New SimpleButton With {.Text = "Imprimir", .Size = New Size(110, 30), .Anchor = AnchorStyles.Top Or AnchorStyles.Right}
        BtnImprimir.ImageOptions.ImageUri.Uri = "Print"
        BtnImprimir.Location = New Point(barra.Width - 227, 8)
        AddHandler BtnImprimir.Click, AddressOf BtnImprimir_Click
        AddHandler BtnCerrar.Click, Sub(s, e) Close()
        barra.Controls.AddRange(New Control() {LblGranTotal, BtnImprimir, BtnCerrar})

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

    End Sub

#End Region

    Private Sub FrmMonedas13vo_Load(sender As Object, e As EventArgs)

        Dim detalle As New DataTable
        detalle.Columns.Add("Codigo", GetType(String))
        detalle.Columns.Add("Nombres", GetType(String))
        detalle.Columns.Add("Monto", GetType(Double))
        For i As Integer = 0 To CalculadoraDenominaciones.Valores.Length - 1
            detalle.Columns.Add(NombreColumna(i), GetType(Integer))
        Next

        Dim montos As New List(Of Double)

        For Each f As DataRow In _montos.Rows
            Dim monto As Double = If(IsDBNull(f("Monto")), 0, Convert.ToDouble(f("Monto")))
            Dim cantidades As Integer() = CalculadoraDenominaciones.Desglosar(monto)
            Dim fila As DataRow = detalle.NewRow()
            fila("Codigo") = f("Codigo")
            fila("Nombres") = f("Nombres")
            fila("Monto") = monto
            For i As Integer = 0 To cantidades.Length - 1
                fila(NombreColumna(i)) = cantidades(i)
            Next
            detalle.Rows.Add(fila)
            montos.Add(monto)
        Next

        _totales = CalculadoraDenominaciones.Acumular(montos)
        _granTotal = montos.Sum()

        Grid.DataSource = detalle
        ConfigurarColumnas()

        LblGranTotal.Text = $"{detalle.Rows.Count} empleados    Total a pagar: {_granTotal:#,##0.00}"

    End Sub

    Private Sub ConfigurarColumnas()

        Vista.PopulateColumns()

        Dim codigo As GridColumn = Vista.Columns("Codigo")
        codigo.Caption = "Código"
        codigo.Width = 90
        Vista.Columns("Nombres").Width = 260
        FormatoMonto(Vista.Columns("Monto"), "Monto a pagar", 100)

        For i As Integer = 0 To CalculadoraDenominaciones.Valores.Length - 1
            Dim col As GridColumn = Vista.Columns(NombreColumna(i))
            col.Caption = CalculadoraDenominaciones.Etiqueta(i).Replace(" *", "")
            col.Width = 60
            col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:#,##0}")
        Next

    End Sub

    Private Shared Sub FormatoMonto(col As GridColumn, titulo As String, ancho As Integer)
        col.Caption = titulo
        col.Width = ancho
        col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        col.DisplayFormat.FormatString = "#,##0.00"
        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:#,##0.00}")
    End Sub

    Private Shared Function NombreColumna(indice As Integer) As String
        Return "D" & indice.ToString("00")
    End Function

    Private Sub BtnImprimir_Click(sender As Object, e As EventArgs)

        Dim empresa As DatosEmpresa = New CsControlesDAL().ObtenerDatosEmpresa()
        FrmVisorReporte.Mostrar(New RptDenominaciones(empresa, _titulo, _totales), "Denominaciones " & _titulo, Me)

    End Sub

End Class
