Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraTab

''' <summary>
''' Historial salarial de una nómina de aguinaldo o de vacaciones (V.5.0 FrmSalarioHistorial). Una sola pantalla y una
''' sola cuadrícula para las dos pestañas: al cambiar de pestaña solo cambian los datos. Exporta a Excel lo que muestra la
''' cuadrícula (con sus filtros) e imprime <see cref="RptHistorialSalarial"/>. Solo UI; los datos vienen de
''' <see cref="CsHistorialSalarialDAL"/>. La interfaz se arma por código (no hay Designer).
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmSalarioHistorial
    Inherits XtraForm

    Private Shared ReadOnly Tipos As String() = {CsHistorialSalarialDAL.TipoAguinaldo, CsHistorialSalarialDAL.TipoVacaciones}

    Private ReadOnly _dal As New CsHistorialSalarialDAL
    ''' <summary>Número de nómina de cada pestaña (0 = aguinaldo, 1 = vacaciones).</summary>
    Private ReadOnly _numeros(1) As Integer
    Private _pestanaActual As Integer
    Private _cargando As Boolean
    Private _historial As DataTable

    Private Pestanas As XtraTabControl
    Private TxtNumero As SpinEdit
    Private Grid As GridControl
    Private Vista As GridView
    Private LblCuenta As LabelControl
    Private BtnCargar, BtnExcel, BtnImprimir, BtnCerrar As SimpleButton

    ''' <param name="tipoInicial">Pestaña que se abre: "Aguinaldo" o "Vacaciones".</param>
    ''' <param name="numAguinaldo">Nómina de aguinaldo (13vo) a mostrar; 0 si no se conoce.</param>
    ''' <param name="numVacaciones">Nómina de vacaciones a mostrar; 0 si no se conoce.</param>
    Public Sub New(tipoInicial As String, numAguinaldo As Integer, numVacaciones As Integer)

        _numeros(0) = numAguinaldo
        _numeros(1) = numVacaciones
        _pestanaActual = If(tipoInicial = CsHistorialSalarialDAL.TipoVacaciones, 1, 0)

        Text = "Historial Salarial"
        ClientSize = New Size(1040, 440)
        StartPosition = FormStartPosition.CenterParent
        MinimizeBox = False

        CrearInterfaz()
        AddHandler Load, AddressOf FrmSalarioHistorial_Load

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        Pestanas = New XtraTabControl With {.Dock = DockStyle.Top, .Height = 30}
        Pestanas.TabPages.Add("Aguinaldo (13vo Mes)")
        Pestanas.TabPages.Add("Vacaciones")
        Pestanas.SelectedTabPageIndex = _pestanaActual
        AddHandler Pestanas.SelectedPageChanged, AddressOf Pestanas_SelectedPageChanged

        Dim superior As New PanelControl With {.Dock = DockStyle.Top, .Height = 40}
        EstiloGrid.AplicarBarra(superior)
        superior.Controls.Add(New LabelControl With {.Text = "Nómina No.:", .Location = New Point(12, 13)})
        TxtNumero = New SpinEdit With {.Location = New Point(85, 10), .Size = New Size(80, 20)}
        TxtNumero.Properties.IsFloatValue = False
        TxtNumero.Properties.MinValue = 0
        TxtNumero.Properties.MaxValue = 999999
        BtnCargar = New SimpleButton With {.Text = "Cargar", .Location = New Point(175, 7), .Size = New Size(90, 26)}
        BtnCargar.ImageOptions.ImageUri.Uri = "Refresh"
        AddHandler BtnCargar.Click, Sub(s, e) Cargar()
        LblCuenta = New LabelControl With {.Location = New Point(285, 13)}
        superior.Controls.AddRange(New Control() {TxtNumero, BtnCargar, LblCuenta})

        Dim barra As New PanelControl With {.Dock = DockStyle.Bottom, .Height = 46}
        EstiloGrid.AplicarBarra(barra)
        BtnCerrar = Boton("Cerrar", "Close", barra.Width - 107, 95)
        BtnImprimir = Boton("Imprimir", "Print", barra.Width - 227, 110)
        BtnExcel = Boton("Exportar a Excel", "ExportToXLSX", barra.Width - 397, 160)
        AddHandler BtnCerrar.Click, Sub(s, e) Close()
        AddHandler BtnImprimir.Click, AddressOf BtnImprimir_Click
        AddHandler BtnExcel.Click, AddressOf BtnExcel_Click
        barra.Controls.AddRange(New Control() {BtnExcel, BtnImprimir, BtnCerrar})

        Grid = New GridControl With {.Dock = DockStyle.Fill}
        Vista = New GridView(Grid)
        Grid.MainView = Vista
        Grid.ViewCollection.Add(Vista)
        Vista.OptionsView.ShowAutoFilterRow = True
        Vista.OptionsView.ColumnAutoWidth = False
        Vista.OptionsBehavior.Editable = False
        EstiloGrid.Aplicar(Vista)

        ' El orden de Dock: lo último agregado se acopla primero.
        Controls.Add(Grid)
        Controls.Add(barra)
        Controls.Add(superior)
        Controls.Add(Pestanas)

    End Sub

    Private Shared Function Boton(texto As String, icono As String, izquierda As Integer, ancho As Integer) As SimpleButton
        Dim b As New SimpleButton With {.Text = texto, .Size = New Size(ancho, 30), .Location = New Point(izquierda, 8),
                                        .Anchor = AnchorStyles.Top Or AnchorStyles.Right}
        b.ImageOptions.ImageUri.Uri = icono
        Return b
    End Function

#End Region

    Private Sub FrmSalarioHistorial_Load(sender As Object, e As EventArgs)
        MostrarNumero()
        Cargar()
    End Sub

    Private Sub Pestanas_SelectedPageChanged(sender As Object, e As TabPageChangedEventArgs)
        If _cargando Then Return
        _numeros(_pestanaActual) = Convert.ToInt32(TxtNumero.Value)
        _pestanaActual = Pestanas.SelectedTabPageIndex
        MostrarNumero()
        Cargar()
    End Sub

    Private Sub MostrarNumero()
        TxtNumero.Value = _numeros(_pestanaActual)
    End Sub

    ''' <summary>Vuelve a leer el historial de la nómina indicada en la pestaña actual.</summary>
    Public Sub Cargar()

        _cargando = True
        Try
            _numeros(_pestanaActual) = Convert.ToInt32(TxtNumero.Value)
            _historial = _dal.Listar(Tipos(_pestanaActual), _numeros(_pestanaActual))
            Grid.DataSource = _historial
            ConfigurarColumnas()
            LblCuenta.Text = $"{_historial.Rows.Count} empleados"
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo leer el historial: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _cargando = False
        End Try

    End Sub

    Private Sub ConfigurarColumnas()

        Vista.PopulateColumns()
        If Vista.Columns.Count = 0 Then Return

        Vista.Columns("NumNomina").Visible = False
        Vista.Columns("Codigo").Caption = "Código"
        Vista.Columns("Codigo").Width = 90
        Vista.Columns("Nombres").Caption = "Nombre del empleado"
        Vista.Columns("Nombres").Width = 250
        Vista.Columns("Cargo").Width = 140
        Vista.Columns("Departamento").Width = 140
        Vista.Columns("FechaIni").Caption = "Fecha inicial"
        Vista.Columns("FechaFin").Caption = "Fecha corte"

        For Each c As String In {"FechaIni", "FechaFin"}
            Vista.Columns(c).DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
            Vista.Columns(c).DisplayFormat.FormatString = "dd/MM/yyyy"
            Vista.Columns(c).Width = 90
        Next

        ' Solo los meses con datos (el aguinaldo usa seis, las vacaciones los de su período).
        For Each mes As String In CsHistorialSalarialDAL.Meses
            Dim col As GridColumn = Vista.Columns(mes)
            col.Visible = RptHistorialSalarial.TieneDatos(_historial, mes)
            col.Width = 90
            col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
            col.DisplayFormat.FormatString = "#,##0.00"
        Next

    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs)

        If _historial Is Nothing OrElse _historial.Rows.Count = 0 Then
            XtraMessageBox.Show("No hay historial para exportar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim ruta As String = ExportadorGrid.ExportarAExcel(Vista, $"Historial {Tipos(_pestanaActual)} {_numeros(_pestanaActual)}")
        If ruta <> "" Then XtraMessageBox.Show("Archivo exportado:" & vbCrLf & ruta, Text, MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub

    Private Sub BtnImprimir_Click(sender As Object, e As EventArgs)

        If _historial Is Nothing OrElse _historial.Rows.Count = 0 Then
            XtraMessageBox.Show("No hay historial para imprimir.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim empresa As DatosEmpresa = New CsControlesDAL().ObtenerDatosEmpresa()
        Dim reporte As RptTabular = RptHistorialSalarial.Crear(empresa, Tipos(_pestanaActual), _numeros(_pestanaActual), _historial)
        FrmVisorReporte.Mostrar(reporte, $"Historial {Tipos(_pestanaActual)} {_numeros(_pestanaActual)}", Me)

    End Sub

End Class
