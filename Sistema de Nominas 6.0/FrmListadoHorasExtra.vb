''' <summary>
''' Listado de horas extra pendientes de pago. Acceso a datos por CsHorasExtraDAL; <see cref="Recargar"/>
''' queda público para que otros formularios (p. ej. el de movimientos) refresquen el listado.
''' </summary>
Public Class FrmListadoHorasExtra

    Private ReadOnly _dal As New CsHorasExtraDAL

#Region "CARGA"

    Private WithEvents BtnNuevo As DevExpress.XtraEditors.SimpleButton
    Private WithEvents BtnEditar As DevExpress.XtraEditors.SimpleButton

    Private Sub FrmListadoHorasExtra_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' V.5.0: "Nuevo" y "Editar" abren FrmMovimientos. Se agregan por código; los botones del Designer se acortan para que quepan.
        Dim x As Integer = 8

        BtnNuevo = New DevExpress.XtraEditors.SimpleButton With {.Text = "Nuevo", .Size = New Size(80, 35)}
        BtnEditar = New DevExpress.XtraEditors.SimpleButton With {.Text = "Editar", .Size = New Size(80, 35)}
        EstiloGrid.AplicarBarra(panelBotones)
        panelBotones.Controls.Add(BtnNuevo)
        panelBotones.Controls.Add(BtnEditar)

        For Each b As DevExpress.XtraEditors.SimpleButton In {BtnNuevo, BtnEditar, BtnEliminar, BtnExcel, BtnSalir}
            b.Size = New Size(80, 35)
            b.Location = New Point(x, 10)
            x += 85
        Next

        Recargar()

    End Sub

    Private Sub BtnNuevo_Click(sender As Object, e As EventArgs) Handles BtnNuevo.Click
        AbrirMovimientos(Nothing, Nothing)
    End Sub

    Private Sub BtnEditar_Click(sender As Object, e As EventArgs) Handles BtnEditar.Click

        Dim fila As DataRow = TryCast(gridViewHorasExtra.GetFocusedDataRow(), DataRow)

        If fila Is Nothing Then
            MessageBox.Show("Seleccione el registro de horas extra que desea editar.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        AbrirMovimientos(Convert.ToString(fila("CodEmpleado1")), Convert.ToDouble(fila("CantHoras")))

    End Sub

    Private Sub AbrirMovimientos(codigo As String, horas As Double?)

        Dim frm As New FrmMovimientos
        AddHandler frm.MovimientoGuardado, AddressOf Recargar
        AddHandler frm.FormClosed, Sub(s, ev) Recargar()

        frm.MdiParent = MdiParent
        frm.Show()

        If codigo IsNot Nothing Then frm.CargarEmpleado(codigo, horas)

    End Sub

    Public Sub Recargar()

        Try

            Cursor = Cursors.WaitCursor

            gridHorasExtra.DataSource = _dal.ListarPendientes()
            ConfigurarColumnas()
            ActualizarTotales()

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub ConfigurarColumnas()

        With gridViewHorasExtra

            .OptionsBehavior.Editable = False
            .OptionsView.ShowGroupPanel = False
            .OptionsView.ShowAutoFilterRow = True
            EstiloGrid.Aplicar(gridViewHorasExtra)

            If .Columns("Id") IsNot Nothing Then .Columns("Id").Visible = False

            Encabezado("CodEmpleado1", "Código", 80)
            Encabezado("Nombres", "Nombres", 420)
            Encabezado("NumNomina", "Nómina", 80)
            Encabezado("CantHoras", "Horas", 80)

            If .Columns("CantHoras") IsNot Nothing Then
                .Columns("CantHoras").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .Columns("CantHoras").DisplayFormat.FormatString = "n2"
            End If

        End With

    End Sub

    Private Sub Encabezado(columna As String, titulo As String, ancho As Integer)

        Dim col As DevExpress.XtraGrid.Columns.GridColumn = gridViewHorasExtra.Columns(columna)

        If col Is Nothing Then Exit Sub

        col.Caption = titulo
        col.Width = ancho

    End Sub

    Private Sub ActualizarTotales()

        Dim total As Double = 0

        For i As Integer = 0 To gridViewHorasExtra.DataRowCount - 1
            total += Convert.ToDouble(gridViewHorasExtra.GetRowCellValue(i, "CantHoras"))
        Next

        LblRegistros.Text = gridViewHorasExtra.DataRowCount.ToString()
        LblTotalHoras.Text = total.ToString("N2")

    End Sub

    Private Sub gridViewHorasExtra_ColumnFilterChanged(sender As Object, e As EventArgs) Handles gridViewHorasExtra.ColumnFilterChanged
        ActualizarTotales()
    End Sub

#End Region

#Region "ACCIONES"

    Private Sub BtnEliminar_Click(sender As Object, e As EventArgs) Handles BtnEliminar.Click

        Dim fila As DataRow = TryCast(gridViewHorasExtra.GetFocusedDataRow(), DataRow)

        If fila Is Nothing Then
            MessageBox.Show("Seleccione el registro de horas extra que desea eliminar.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("¿Está seguro de borrar las horas extra del empleado " & Convert.ToString(fila("CodEmpleado1")) & "?",
                           "Zeus Nominas", MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try

            Cursor = Cursors.WaitCursor

            If Not _dal.EliminarPendiente(Convert.ToInt32(fila("Id"))) Then
                MessageBox.Show("El registro ya no existe o ya fue pagado.", "Zeus Nominas",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            Recargar()

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs) Handles BtnExcel.Click

        Try
            Dim ruta As String = ExportadorGrid.ExportarAExcel(gridViewHorasExtra, "HorasExtraPendientes")

            If ruta <> "" Then
                MessageBox.Show("Listado exportado a " & ruta, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

#End Region

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
