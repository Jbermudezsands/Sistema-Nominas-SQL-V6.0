Imports DevExpress.Data
Imports DevExpress.Utils
Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo

''' <summary>
''' Consulta de registros (V.5.0 FrmConsulta): ventana modal con un grid y barra de filtros que elige un registro para otro formulario.
''' Se usa con <see cref="Elegir(TipoBusqueda, IWin32Window)"/>, que devuelve el registro elegido (<see cref="ResultadoBusqueda"/>)
''' o Nothing si se cancela. Qué consulta se muestra lo define <see cref="CatalogoBusquedas"/>; los datos vienen de <see cref="CsBuscadorDAL"/>.
''' La interfaz se arma por código (no hay Designer).
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmConsulta
    Inherits XtraForm

    Private ReadOnly _definicion As DefinicionBusqueda
    Private _dal As CsBuscadorDAL

    Private GridConsulta As GridControl
    Private VistaConsulta As GridView
    Private BtnPegar, BtnCancelar As SimpleButton

    ''' <summary>El registro elegido; Nothing mientras no se haya pegado nada.</summary>
    Public ReadOnly Property Resultado As ResultadoBusqueda

    Public Sub New(definicion As DefinicionBusqueda)

        _definicion = definicion

        Text = "Consulta de Registros - " & definicion.Titulo
        ClientSize = New Size(900, 460)
        StartPosition = FormStartPosition.CenterParent
        MinimizeBox = False
        ShowInTaskbar = False

        CrearInterfaz()

        AddHandler Load, AddressOf FrmConsulta_Load

    End Sub

    ''' <summary>Abre la consulta como diálogo y devuelve el registro elegido (Nothing si se cancela o falla la carga).</summary>
    Public Shared Function Elegir(tipo As TipoBusqueda, Optional propietario As IWin32Window = Nothing) As ResultadoBusqueda
        Return Elegir(CatalogoBusquedas.Obtener(tipo), propietario)
    End Function

    ''' <summary>Igual, con una definición propia (columnas y SQL de la consulta).</summary>
    Public Shared Function Elegir(definicion As DefinicionBusqueda, Optional propietario As IWin32Window = Nothing) As ResultadoBusqueda

        Using frm As New FrmConsulta(definicion)
            If frm.ShowDialog(propietario) <> DialogResult.OK Then Return Nothing
            Return frm.Resultado
        End Using

    End Function

    ''' <summary>
    ''' Agrega al combo de código de un formulario el botón de lupa (V.5.0 botón "Consultar" junto al código) que abre esta consulta
    ''' y deja el código elegido en el combo. Se llama una vez al cargar el formulario.
    ''' </summary>
    Public Shared Sub AgregarBotonBuscar(combo As SearchLookUpEdit, tipo As TipoBusqueda, propietario As IWin32Window)

        Dim boton As New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
        boton.ImageOptions.ImageUri.Uri = "Find"
        boton.ToolTip = "Buscar en la lista de registros"
        combo.Properties.Buttons.Add(boton)

        AddHandler combo.ButtonClick,
            Sub(s, e)
                If e.Button IsNot boton Then Return
                Dim elegido As ResultadoBusqueda = Elegir(tipo, propietario)
                If elegido IsNot Nothing Then combo.EditValue = elegido.Codigo
            End Sub

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        GridConsulta = New GridControl With {.Dock = DockStyle.Fill}
        VistaConsulta = New GridView(GridConsulta)
        GridConsulta.MainView = VistaConsulta
        GridConsulta.ViewCollection.Add(VistaConsulta)

        With VistaConsulta
            .OptionsBehavior.Editable = False
            .OptionsSelection.EnableAppearanceFocusedCell = False
            .OptionsView.ShowGroupPanel = False
            .OptionsView.ShowAutoFilterRow = True
            .OptionsView.ColumnAutoWidth = False
        End With
        EstiloGrid.Aplicar(VistaConsulta)

        AddHandler VistaConsulta.DoubleClick, AddressOf Vista_DoubleClick
        AddHandler VistaConsulta.KeyDown, AddressOf Vista_KeyDown
        AddHandler VistaConsulta.RowCountChanged, Sub(s, e) MostrarTotal()
        AddHandler VistaConsulta.ColumnFilterChanged, Sub(s, e) MostrarTotal()

        BtnPegar = New SimpleButton With {.Text = "Pegar", .Location = New Point(8, 7), .Size = New Size(95, 35)}
        BtnPegar.ImageOptions.ImageUri.Uri = "Apply"
        BtnCancelar = New SimpleButton With {.Text = "Cancelar", .Dock = DockStyle.Right, .Size = New Size(95, 35)}
        BtnCancelar.ImageOptions.ImageUri.Uri = "Cancel"
        AddHandler BtnPegar.Click, Sub(s, e) Pegar()
        AddHandler BtnCancelar.Click, Sub(s, e) DialogResult = DialogResult.Cancel

        Dim barra As New PanelControl With {.Dock = DockStyle.Bottom, .Height = 50, .BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder}
        barra.Padding = New Padding(8, 7, 8, 8)
        barra.Controls.AddRange(New Control() {BtnPegar, BtnCancelar})
        EstiloGrid.AplicarBarra(barra)

        Controls.Add(GridConsulta)
        Controls.Add(barra)

        CancelButton = BtnCancelar

    End Sub

#End Region

#Region "CARGA"

    Private Sub FrmConsulta_Load(sender As Object, e As EventArgs)

        Try
            Cursor = Cursors.WaitCursor
            _dal = New CsBuscadorDAL()
            GridConsulta.DataSource = _dal.Listar(_definicion)
        Catch ex As Exception
            Cursor = Cursors.Default
            XtraMessageBox.Show("No se pudo abrir la consulta: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            DialogResult = DialogResult.Cancel
            Return
        Finally
            Cursor = Cursors.Default
        End Try

        ConfigurarColumnas()
        MostrarTotal()

        ' El cursor empieza en la barra de filtros: se escribe y la lista se reduce.
        VistaConsulta.FocusedRowHandle = GridControl.AutoFilterRowHandle
        If VistaConsulta.VisibleColumns.Count > 0 Then VistaConsulta.FocusedColumn = VistaConsulta.VisibleColumns(0)
        GridConsulta.Select()

    End Sub

    Private Sub ConfigurarColumnas()

        VistaConsulta.PopulateColumns()

        For Each def As ColumnaBusqueda In _definicion.Columnas
            Dim col As GridColumn = VistaConsulta.Columns.ColumnByFieldName(def.Campo)
            If col Is Nothing Then Continue For

            col.Caption = def.Titulo
            col.Width = def.Ancho
            col.Visible = def.Visible
            col.VisibleIndex = _definicion.Columnas.IndexOf(def)

            If def.Formato <> "" Then
                col.DisplayFormat.FormatType = If(col.ColumnType Is GetType(Date) OrElse col.ColumnType Is GetType(DateTime),
                                                  FormatType.DateTime, FormatType.Numeric)
                col.DisplayFormat.FormatString = def.Formato
            End If

            ' Texto: contiene lo escrito (V.5.0 LIKE '%texto%'); número y fecha: igual al valor.
            col.OptionsFilter.AutoFilterCondition = If(col.ColumnType Is GetType(String), AutoFilterCondition.Contains, AutoFilterCondition.Equals)
        Next

    End Sub

    Private Sub MostrarTotal()
        Text ="Consulta de Registros - " & _definicion.Titulo & " (" & VistaConsulta.RowCount & ")"
    End Sub

#End Region

#Region "SELECCION"

    Private Sub Vista_DoubleClick(sender As Object, e As EventArgs)

        ' Solo una fila de datos: no el encabezado, la barra de filtros ni el pie.
        Dim pos As Point = VistaConsulta.GridControl.PointToClient(Control.MousePosition)
        Dim info As GridHitInfo = VistaConsulta.CalcHitInfo(pos)

        If info.InRow AndAlso info.RowHandle >= 0 Then Pegar()

    End Sub

    Private Sub Vista_KeyDown(sender As Object, e As KeyEventArgs)

        ' Enter sobre una fila la elige; en la barra de filtros solo aplica el filtro.
        If e.KeyCode = Keys.Enter AndAlso VistaConsulta.FocusedRowHandle >= 0 Then
            e.Handled = True
            Pegar()
        End If

    End Sub

    ''' <summary>Pegar (V.5.0 CmdPegar): guarda la fila actual como resultado y cierra.</summary>
    Private Sub Pegar()

        Dim fila As DataRowView = TryCast(VistaConsulta.GetFocusedRow(), DataRowView)

        If fila Is Nothing Then
            XtraMessageBox.Show("Elija un registro de la lista.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        _Resultado = New ResultadoBusqueda(fila.Row, _definicion.Clave)
        DialogResult = DialogResult.OK

    End Sub

#End Region

End Class
