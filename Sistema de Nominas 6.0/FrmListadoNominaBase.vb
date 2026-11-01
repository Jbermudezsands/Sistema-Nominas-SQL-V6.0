Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Base de los listados históricos de nóminas de prestaciones (V.5.0 FrmListadoNominaVacaciones, una pestaña por tipo; aquí un
''' formulario por tipo). Arriba las nóminas, abajo el detalle de la seleccionada y una barra con colillas, nómina impresa, Excel,
''' denominaciones e historial salarial. Solo UI: cada formulario hijo aporta los datos y los reportes. La interfaz se arma por código.
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public MustInherit Class FrmListadoNominaBase
    Inherits XtraForm

    Private _cargando As Boolean
    Private _fila As DataRow

    Protected GridNominas As GridControl
    Protected VistaNominas As GridView
    Protected GridDetalle As GridControl
    Protected VistaDetalle As GridView
    Protected ChkRestarImpuestos As CheckEdit
    Protected ChkPorDepartamento As CheckEdit
    Private BtnColilla, BtnNomina, BtnExcel, BtnMonedas, BtnHistorial, BtnSalir As SimpleButton

    Protected Sub New(titulo As String)

        Text = titulo
        ClientSize = New Size(1040, 560)
        StartPosition = FormStartPosition.CenterParent

        CrearInterfaz()
        AddHandler Load, AddressOf FrmListadoNominaBase_Load

    End Sub

#Region "A DEFINIR POR CADA LISTADO"

    ''' <summary>Nóminas: Numero, CodTipoNomina, Tipo, FechaIni, FechaFin, FechaAplica, MontoPagado, Estado.</summary>
    Protected MustOverride Function ListarNominas() As DataTable
    Protected MustOverride Function ListarDetalle(numero As Integer) As DataTable
    Protected MustOverride Sub ConfigurarDetalle()
    Protected MustOverride Sub Colillas(numero As Integer, periodo As String)
    Protected MustOverride Sub NominaImpresa(numero As Integer, periodo As String)
    Protected MustOverride Function ListarMontos(numero As Integer) As DataTable
    Protected MustOverride Function EtiquetaNomina() As String
    Protected MustOverride Sub AbrirHistorial(numero As Integer)

    ''' <summary>Solo vacaciones: "Restar INSS e IR" cambia el monto a pagar.</summary>
    Protected Overridable ReadOnly Property MuestraRestarImpuestos As Boolean = False

#End Region

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        GridNominas = New GridControl With {.Dock = DockStyle.Top, .Height = 190}
        VistaNominas = New GridView(GridNominas)
        GridNominas.MainView = VistaNominas
        GridNominas.ViewCollection.Add(VistaNominas)
        VistaNominas.OptionsBehavior.Editable = False
        VistaNominas.OptionsView.ShowGroupPanel = False
        VistaNominas.OptionsView.ColumnAutoWidth = False
        EstiloGrid.Aplicar(VistaNominas)
        AddHandler VistaNominas.FocusedRowChanged, AddressOf VistaNominas_FocusedRowChanged

        Dim separador As New SplitterControl With {.Dock = DockStyle.Top}

        GridDetalle = New GridControl With {.Dock = DockStyle.Fill}
        VistaDetalle = New GridView(GridDetalle)
        GridDetalle.MainView = VistaDetalle
        GridDetalle.ViewCollection.Add(VistaDetalle)
        VistaDetalle.OptionsBehavior.Editable = False
        VistaDetalle.OptionsView.ShowAutoFilterRow = True
        VistaDetalle.OptionsView.ShowFooter = True
        VistaDetalle.OptionsView.ColumnAutoWidth = False
        EstiloGrid.Aplicar(VistaDetalle)

        Dim barra As New FlowLayoutPanel With {.Dock = DockStyle.Bottom, .Height = 86, .Padding = New Padding(8, 6, 8, 4), .WrapContents = True}
        BtnColilla = Boton("Colillas", "Print", 100)
        BtnNomina = Boton("Imprimir nómina", "Print", 135)
        BtnExcel = Boton("Detalle a Excel", "ExportToXLSX", 135)
        BtnMonedas = Boton("Denominaciones", "Currency", 130)
        BtnHistorial = Boton("Historial salarial", "History", 135)
        BtnSalir = Boton("Salir", "Close", 80)
        ChkPorDepartamento = New CheckEdit With {.Text = "Colilla por departamento", .Width = 170, .Margin = New Padding(3, 8, 3, 3)}
        ChkRestarImpuestos = New CheckEdit With {.Text = "Restar INSS e IR", .Width = 130, .Margin = New Padding(3, 8, 3, 3), .Visible = MuestraRestarImpuestos}
        AddHandler ChkRestarImpuestos.CheckedChanged, Sub(s, e) MostrarDetalle()

        AddHandler BtnColilla.Click, Sub(s, e) Ejecutar(Sub(n) Colillas(n, Periodo()), "generar las colillas")
        AddHandler BtnNomina.Click, Sub(s, e) Ejecutar(Sub(n) NominaImpresa(n, Periodo()), "generar la nómina")
        AddHandler BtnExcel.Click, AddressOf BtnExcel_Click
        AddHandler BtnMonedas.Click, Sub(s, e) Ejecutar(AddressOf MostrarMonedas, "abrir las denominaciones")
        AddHandler BtnHistorial.Click, Sub(s, e) Ejecutar(AddressOf AbrirHistorial, "abrir el historial salarial")
        AddHandler BtnSalir.Click, Sub(s, e) Close()

        barra.Controls.AddRange(New Control() {BtnColilla, ChkPorDepartamento, ChkRestarImpuestos, BtnNomina, BtnExcel, BtnMonedas, BtnHistorial, BtnSalir})

        Controls.Add(GridDetalle)
        Controls.Add(separador)
        Controls.Add(GridNominas)
        Controls.Add(barra)

    End Sub

    Private Shared Function Boton(texto As String, icono As String, ancho As Integer) As SimpleButton
        Dim b As New SimpleButton With {.Text = texto, .Size = New Size(ancho, 30)}
        b.ImageOptions.ImageUri.Uri = icono
        Return b
    End Function

#End Region

#Region "CARGA"

    Private Sub FrmListadoNominaBase_Load(sender As Object, e As EventArgs)
        Recargar()
    End Sub

    ''' <summary>Vuelve a leer las nóminas (p. ej. después de calcular o cerrar una).</summary>
    Public Sub Recargar()

        Try
            Cursor = Cursors.WaitCursor
            _cargando = True
            Try
                GridNominas.DataSource = ListarNominas()
                ConfigurarNominas()
            Finally
                _cargando = False
            End Try
            MostrarDetalle()
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo leer las nóminas: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub ConfigurarNominas()

        VistaNominas.PopulateColumns()
        If VistaNominas.Columns.Count = 0 Then Return

        Encabezado("Numero", "Nómina", 70)
        Encabezado("CodTipoNomina", "Cód. tipo", 70)
        Encabezado("Tipo", "Tipo de nómina", 160)
        Encabezado("FechaIni", "Desde", 90, "dd/MM/yyyy")
        Encabezado("FechaFin", "Hasta", 90, "dd/MM/yyyy")
        Encabezado("FechaAplica", "Fecha aplicada", 100, "dd/MM/yyyy")
        Encabezado("MontoPagado", "Monto pagado", 110, "#,##0.00")
        Encabezado("Estado", "Estado", 80)

    End Sub

    Private Sub Encabezado(campo As String, titulo As String, ancho As Integer, Optional formato As String = "")

        Dim c As GridColumn = VistaNominas.Columns(campo)
        If c Is Nothing Then Return

        c.Caption = titulo
        c.Width = ancho

        If formato <> "" Then
            c.DisplayFormat.FormatType = If(formato.Contains("/"), DevExpress.Utils.FormatType.DateTime, DevExpress.Utils.FormatType.Numeric)
            c.DisplayFormat.FormatString = formato
        End If

    End Sub

    Private Sub VistaNominas_FocusedRowChanged(sender As Object, e As FocusedRowChangedEventArgs)
        If Not _cargando Then MostrarDetalle()
    End Sub

    Private Sub MostrarDetalle()

        _fila = TryCast(VistaNominas.GetFocusedDataRow(), DataRow)

        For Each b As SimpleButton In {BtnColilla, BtnNomina, BtnExcel, BtnMonedas, BtnHistorial}
            b.Enabled = _fila IsNot Nothing
        Next

        If _fila Is Nothing Then
            GridDetalle.DataSource = Nothing
            Return
        End If

        Try
            GridDetalle.DataSource = ListarDetalle(Numero())
            ConfigurarDetalle()
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo leer el detalle: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Protected Sub Formatear(campo As String, titulo As String, ancho As Integer, Optional formato As String = "#,##0.00", Optional sumar As Boolean = False)

        Dim c As GridColumn = VistaDetalle.Columns(campo)
        If c Is Nothing Then Return

        c.Caption = titulo
        c.Width = ancho
        c.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        c.DisplayFormat.FormatString = formato
        If sumar Then c.Summary.Add(DevExpress.Data.SummaryItemType.Sum, campo, "{0:#,##0.00}")

    End Sub

    Private Function Numero() As Integer
        Return Convert.ToInt32(_fila("Numero"))
    End Function

    Private Function Periodo() As String
        Return $"{EtiquetaNomina()} No. {Numero()}  -  Del {CDate(_fila("FechaIni")):dd/MM/yyyy} al {CDate(_fila("FechaFin")):dd/MM/yyyy}"
    End Function

#End Region

#Region "ACCIONES"

    Private Sub Ejecutar(accion As Action(Of Integer), descripcion As String)

        If _fila Is Nothing Then Return

        Try
            accion(Numero())
        Catch ex As Exception
            XtraMessageBox.Show($"No se pudo {descripcion}: {ex.Message}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub MostrarMonedas(numero As Integer)
        Using frm As New FrmMonedas13vo($"{EtiquetaNomina()} No. {numero}", ListarMontos(numero))
            frm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs)

        Ejecutar(Sub(n)
                     Dim ruta As String = ExportadorGrid.ExportarAExcel(VistaDetalle, $"Detalle {EtiquetaNomina()} {n}")
                     If ruta <> "" Then XtraMessageBox.Show("Archivo exportado:" & vbCrLf & ruta, Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
                 End Sub, "exportar")

    End Sub

#End Region

End Class
