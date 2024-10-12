Public Class FrmEntrada
    Dim Compañia As New Compañias

    Private Sub BtnAceptar_Click(sender As Object, e As EventArgs) Handles BtnAceptar.Click
        If (TxtUsuario.Text = "") Then
            MessageBox.Show("Necesita Digitar el Usuario", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        If (TxtContraseña.Text = "") Then
            MessageBox.Show("Necesita Digitar una Contraseña", "Zeus Contable", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        If (Compañia.ContraseñaCorrecta(TxtUsuario.Text, TxtContraseña.Text) = True) Then
            Me.Hide()
            Main.ShowDialog()
        Else
            MessageBox.Show("Constraseña o Usuario Incorrecto!!", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If


    End Sub

    Private Sub TxtContraseña_TextChanged(sender As Object, e As EventArgs) Handles TxtContraseña.TextChanged

    End Sub

    Private Sub TxtContraseña_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtContraseña.KeyDown
        If (e.KeyCode = Keys.Enter) Then
            BtnAceptar_Click(sender, e)
        End If

    End Sub

    Private Sub BtnCancelar_Click(sender As Object, e As EventArgs) Handles BtnCancelar.Click
        Me.Close()
    End Sub
End Class