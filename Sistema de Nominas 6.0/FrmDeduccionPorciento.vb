''' <summary>
''' Porcentajes de deducción por rango. Cuadrícula editable (fila nueva arriba); Grabar aplica altas,
''' cambios y bajas en una transacción mediante CsDeduccionPorcientoDAL.
''' </summary>
Public Class FrmDeduccionPorciento

    Private ReadOnly _dal As New CsDeduccionPorcientoDAL
    Private _tabla As DataTable

    Private Sub FrmDeduccionPorciento_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Recargar()
        Catch ex As Exception
            MostrarError(ex)
        End Try
    End Sub

    Public Sub Recargar()
        _tabla = _dal.Listar()
        Grid.DataSource = _tabla
        If Vista.Columns("IdDeduccionPorciento") IsNot Nothing Then Vista.Columns("IdDeduccionPorciento").Visible = False
    End Sub

    Private Sub BtnGuardar_Click(sender As Object, e As EventArgs) Handles BtnGuardar.Click

        Try
            Vista.CloseEditor()
            Vista.UpdateCurrentRow()

            Dim error1 As String = CsDeduccionPorcientoDAL.Validar(_tabla)
            If error1 <> "" Then
                MessageBox.Show(error1, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            Cursor = Cursors.WaitCursor
            _dal.Guardar(_tabla)
            Recargar()
            MessageBox.Show("Cambios guardados correctamente.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnEliminar_Click(sender As Object, e As EventArgs) Handles BtnEliminar.Click

        If Vista.FocusedRowHandle < 0 Then
            MessageBox.Show("Seleccione la fila que desea eliminar.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("¿Está seguro de eliminar el registro seleccionado?", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Exit Sub

        ' Se marca como eliminada en la tabla; se aplica en la base al presionar Grabar.
        Vista.DeleteRow(Vista.FocusedRowHandle)

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
