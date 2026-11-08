Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Adelantos de 13vo mes y vacaciones por empleado (V.5.0 FrmAdelantos13vo): se registran aquí y se descuentan en
''' <see cref="FrmAguinaldo"/> y <see cref="FrmNominaVacaciones"/>. La foto viene de la base de datos (Empleado.Foto),
''' igual que en <see cref="FrmEmpleados"/>. Datos en <see cref="CsAdelantosDAL"/>; solo UI, armada por código.
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmAdelantos13vo
    Inherits XtraForm

    Private ReadOnly _dal As New CsAdelantosDAL

    Private _ficha As CsAdelantosDAL.FichaAdelanto
    Private _tabla As DataTable
    Private _cargando As Boolean
    Private _sinGuardar As Boolean

    Private CmbEmpleado As SearchLookUpEdit
    Private TxtNombre, TxtCargo As TextEdit
    Private ImgFoto As PictureEdit
    Private Grid As GridControl
    Private Vista As GridView
    Private BtnNuevo, BtnGrabar, BtnBorrar, BtnSalir As SimpleButton
    Private LblTotal As LabelControl

    Public Sub New()

        Text = "Adelanto de 13vo Mes y Vacaciones"
        ClientSize = New Size(900, 480)
        MinimumSize = New Size(640, 420)
        StartPosition = FormStartPosition.CenterParent

        CrearInterfaz()
        AddHandler Load, AddressOf FrmAdelantos13vo_Load
        AddHandler FormClosing, AddressOf FrmAdelantos13vo_FormClosing

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        ' Botones en columna acoplada a la derecha (ver CLAUDE.md: calzar dentro del MDI).
        Dim barra As New XtraScrollableControl With {.Dock = DockStyle.Right, .Width = 150}
        barra.Appearance.BackColor = EstiloGrid.ColorBarra
        barra.Appearance.Options.UseBackColor = True

        BtnNuevo = Boton("Nuevo adelanto", "Add")
        BtnGrabar = Boton("Grabar", "Save")
        BtnBorrar = Boton("Borrar línea", "Delete")
        BtnSalir = Boton("Cerrar", "Close")

        Dim y As Integer = 10
        For Each b As SimpleButton In {BtnNuevo, BtnGrabar, BtnBorrar, BtnSalir}
            b.Location = New Point(10, y)
            barra.Controls.Add(b)
            y += 38
        Next

        AddHandler BtnNuevo.Click, AddressOf BtnNuevo_Click
        AddHandler BtnGrabar.Click, AddressOf BtnGrabar_Click
        AddHandler BtnBorrar.Click, AddressOf BtnBorrar_Click
        AddHandler BtnSalir.Click, Sub(s, e) Close()

        ' Datos generales + foto
        Dim cabecera As New GroupControl With {.Text = "Datos Generales", .Dock = DockStyle.Top, .Height = 160}

        cabecera.Controls.Add(New LabelControl With {.Text = "Número empleado:", .Location = New Point(12, 36)})
        CmbEmpleado = New SearchLookUpEdit With {.Location = New Point(125, 33), .Size = New Size(150, 20)}
        CmbEmpleado.Properties.DisplayMember = "CodEmpleado1"
        CmbEmpleado.Properties.ValueMember = "CodEmpleado1"
        CmbEmpleado.Properties.NullText = ""
        CmbEmpleado.Properties.PopupFormSize = New Size(450, 350)
        AddHandler CmbEmpleado.EditValueChanged, AddressOf CmbEmpleado_EditValueChanged
        cabecera.Controls.Add(CmbEmpleado)

        TxtNombre = SoloLectura(cabecera, "Nombre:", 12, 66, 125, 300)
        TxtCargo = SoloLectura(cabecera, "Cargo:", 12, 96, 125, 300)

        ImgFoto = New PictureEdit With {.Location = New Point(450, 28), .Size = New Size(120, 124)}
        ImgFoto.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom
        ImgFoto.Properties.ReadOnly = True
        ImgFoto.Properties.ShowMenu = False
        cabecera.Controls.Add(ImgFoto)

        LblTotal = New LabelControl With {.Dock = DockStyle.Bottom, .Padding = New Padding(6, 4, 0, 4)}

        Grid = New GridControl With {.Dock = DockStyle.Fill}
        Vista = New GridView(Grid)
        Grid.MainView = Vista
        Grid.ViewCollection.Add(Vista)
        Vista.OptionsView.ShowGroupPanel = False
        Vista.OptionsView.ColumnAutoWidth = True
        Vista.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom
        Vista.OptionsBehavior.Editable = True
        EstiloGrid.Aplicar(Vista)
        AddHandler Vista.InitNewRow, AddressOf Vista_InitNewRow
        AddHandler Vista.RowUpdated, Sub(s, e)
                                         _sinGuardar = True
                                         ActualizarTotal()
                                     End Sub

        Controls.Add(Grid)
        Controls.Add(LblTotal)
        Controls.Add(cabecera)
        Controls.Add(barra)
        Grid.BringToFront()

    End Sub

    Private Shared Function Boton(texto As String, icono As String) As SimpleButton
        Dim b As New SimpleButton With {.Text = texto, .Size = New Size(130, 30)}
        b.ImageOptions.ImageUri.Uri = icono
        Return b
    End Function

    Private Shared Function SoloLectura(padre As Control, titulo As String, x As Integer, y As Integer, xCaja As Integer, ancho As Integer) As TextEdit
        padre.Controls.Add(New LabelControl With {.Text = titulo, .Location = New Point(x, y + 3)})
        Dim t As New TextEdit With {.Location = New Point(xCaja, y), .Size = New Size(ancho, 20)}
        t.Properties.ReadOnly = True
        padre.Controls.Add(t)
        Return t
    End Function

#End Region

#Region "CARGA"

    Private Sub FrmAdelantos13vo_Load(sender As Object, e As EventArgs)

        If MdiParent IsNot Nothing Then WindowState = FormWindowState.Maximized

        Try
            FrmConsulta.AgregarBotonBuscar(CmbEmpleado, TipoBusqueda.Empleado, Me)
            CmbEmpleado.Properties.DataSource = _dal.ListarEmpleados()
            CmbEmpleado.Properties.View.Columns.Clear()
            CmbEmpleado.Properties.View.PopulateColumns(CmbEmpleado.Properties.DataSource)
            MostrarFoto("")
            PrepararTabla(Nothing)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo cargar la lista de empleados: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub CmbEmpleado_EditValueChanged(sender As Object, e As EventArgs)

        If _cargando Then Return

        If Not ConfirmarDescarte() Then
            _cargando = True
            CmbEmpleado.EditValue = If(_ficha Is Nothing, Nothing, _ficha.Codigo)
            _cargando = False
            Return
        End If

        If CmbEmpleado.EditValue Is Nothing Then
            LimpiarEmpleado()
            Return
        End If

        Try
            Dim ficha As CsAdelantosDAL.FichaAdelanto = _dal.BuscarEmpleado(Convert.ToString(CmbEmpleado.EditValue))

            If ficha Is Nothing Then
                LimpiarEmpleado()
                Return
            End If

            _ficha = ficha
            TxtNombre.Text = ficha.Nombres
            TxtCargo.Text = ficha.Cargo
            MostrarFoto(ficha.Foto)
            PrepararTabla(_dal.Listar(ficha.Cod_Empleado))

        Catch ex As Exception
            XtraMessageBox.Show("No se pudo leer el empleado: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub LimpiarEmpleado()
        _ficha = Nothing
        TxtNombre.Text = ""
        TxtCargo.Text = ""
        MostrarFoto("")
        PrepararTabla(Nothing)
    End Sub

    ''' <summary>Misma lectura que FrmEmpleados.MostrarFoto: la foto se guarda como texto "b1,b2,..." en Empleado.Foto.</summary>
    Private Sub MostrarFoto(foto As String)

        Dim img As Image = Nothing

        If Not String.IsNullOrWhiteSpace(foto) Then img = BytesToImagen(StringtoByte(foto))

        ImgFoto.Image = If(img, My.Resources.Silueta)

    End Sub

    Private Sub PrepararTabla(datos As DataTable)

        If datos Is Nothing Then
            datos = New DataTable
            datos.Columns.Add("Fecha", GetType(Date))
            datos.Columns.Add("Monto", GetType(Double))
            datos.Columns.Add("Referencia", GetType(String))
            datos.Columns.Add("Tipo", GetType(String))
        End If

        _tabla = datos
        _sinGuardar = False
        Grid.DataSource = _tabla
        Vista.PopulateColumns()

        Dim fecha As New RepositoryItemDateEdit
        fecha.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
        fecha.DisplayFormat.FormatString = "dd/MM/yyyy"
        fecha.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime
        fecha.EditFormat.FormatString = "dd/MM/yyyy"
        fecha.Mask.EditMask = "dd/MM/yyyy"
        fecha.Mask.UseMaskAsDisplayFormat = True
        Grid.RepositoryItems.Add(fecha)
        Vista.Columns("Fecha").ColumnEdit = fecha
        Vista.Columns("Fecha").Width = 100

        With Vista.Columns("Monto")
            .Width = 110
            .DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
            .DisplayFormat.FormatString = "n2"
            .Summary.Add(DevExpress.Data.SummaryItemType.Sum, "Monto", "{0:n2}")
        End With

        Vista.Columns("Referencia").Caption = "Ref/Cheque"

        Dim tipos As New RepositoryItemComboBox With {.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor}
        tipos.Items.AddRange(New String() {CsAdelantosDAL.Tipo13vo, CsAdelantosDAL.TipoVacaciones})
        Grid.RepositoryItems.Add(tipos)
        Vista.Columns("Tipo").ColumnEdit = tipos
        Vista.Columns("Tipo").Width = 120

        Vista.OptionsView.ShowFooter = True

        Dim hayEmpleado As Boolean = (_ficha IsNot Nothing)
        Grid.Enabled = hayEmpleado
        BtnNuevo.Enabled = hayEmpleado
        BtnGrabar.Enabled = hayEmpleado
        BtnBorrar.Enabled = hayEmpleado
        ActualizarTotal()

    End Sub

    Private Sub ActualizarTotal()
        Dim total As Double = 0
        If _tabla IsNot Nothing Then
            For Each f As DataRow In _tabla.Rows
                If f.RowState <> DataRowState.Deleted AndAlso Not IsDBNull(f("Monto")) Then total += Convert.ToDouble(f("Monto"))
            Next
        End If
        LblTotal.Text = "Total de adelantos: " & total.ToString("n2") & If(_sinGuardar, "   (cambios sin grabar)", "")
    End Sub

#End Region

#Region "ACCIONES"

    Private Sub Vista_InitNewRow(sender As Object, e As DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs)
        Vista.SetRowCellValue(e.RowHandle, "Fecha", Date.Today)
        Vista.SetRowCellValue(e.RowHandle, "Tipo", CsAdelantosDAL.Tipo13vo)
    End Sub

    Private Sub BtnNuevo_Click(sender As Object, e As EventArgs)
        Vista.FocusedRowHandle = GridControl.NewItemRowHandle
        Vista.FocusedColumn = Vista.Columns("Monto")
        Vista.ShowEditor()
    End Sub

    Private Sub BtnBorrar_Click(sender As Object, e As EventArgs)

        If Vista.FocusedRowHandle < 0 Then Return

        If XtraMessageBox.Show("¿Desea eliminar la línea seleccionada?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Vista.DeleteRow(Vista.FocusedRowHandle)
        _sinGuardar = True
        ActualizarTotal()

    End Sub

    Private Sub BtnGrabar_Click(sender As Object, e As EventArgs)

        If _ficha Is Nothing Then Return

        Try
            Vista.CloseEditor()
            Vista.UpdateCurrentRow()
            Cursor = Cursors.WaitCursor
            _dal.Guardar(_ficha.Cod_Empleado, _tabla)
            PrepararTabla(_dal.Listar(_ficha.Cod_Empleado))
            XtraMessageBox.Show("Los adelantos se guardaron correctamente.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As InvalidOperationException
            XtraMessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudieron guardar los adelantos: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Function ConfirmarDescarte() As Boolean
        If Not _sinGuardar Then Return True
        Return XtraMessageBox.Show("Hay cambios sin grabar. ¿Desea descartarlos?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes
    End Function

    Private Sub FrmAdelantos13vo_FormClosing(sender As Object, e As FormClosingEventArgs)
        If Not ConfirmarDescarte() Then e.Cancel = True
    End Sub

#End Region

End Class
