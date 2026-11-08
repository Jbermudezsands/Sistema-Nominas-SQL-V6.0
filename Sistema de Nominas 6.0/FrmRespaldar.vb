''' <summary>
''' Respaldo de la base de datos de la compañía conectada. Acceso a datos por CsRespaldoDAL;
''' el formulario solo pide la ruta y muestra el resultado.
''' </summary>
Public Class FrmRespaldar

    Private ReadOnly _dal As New CsRespaldoDAL

    Private Sub FrmRespaldar_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        EstiloGrid.AplicarBarra(panelBotones)

        Try
            TxtBaseDatos.Text = _dal.NombreBaseDatos()
            TxtRuta.Text = _dal.RutaSugerida()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub TxtRuta_ButtonClick(sender As Object, e As DevExpress.XtraEditors.Controls.ButtonPressedEventArgs) Handles TxtRuta.ButtonClick

        Using dlg As New SaveFileDialog
            dlg.Title = "Nombre del respaldo"
            dlg.Filter = "Respaldos de SQL Server (*.bak)|*.bak|Todos los archivos (*.*)|*.*"
            dlg.DefaultExt = "bak"
            dlg.FileName = IO.Path.GetFileName(TxtRuta.Text)
            dlg.InitialDirectory = CarpetaInicial()

            If dlg.ShowDialog(Me) = DialogResult.OK Then
                TxtRuta.Text = dlg.FileName
            End If
        End Using

    End Sub

    Private Sub BtnRespaldar_Click(sender As Object, e As EventArgs) Handles BtnRespaldar.Click

        If String.IsNullOrWhiteSpace(TxtRuta.Text) Then
            MessageBox.Show("Debe indicar la ruta donde se guardará el respaldo.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtRuta.Focus()
            Return
        End If

        Try

            Cursor = Cursors.WaitCursor
            panelBotones.Enabled = False
            TxtRuta.Enabled = False

            _dal.Respaldar(TxtRuta.Text.Trim())

            MessageBox.Show("Base de datos respaldada con éxito.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Close()

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
            panelBotones.Enabled = True
            TxtRuta.Enabled = True
        End Try

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

    Private Function CarpetaInicial() As String

        Try
            Dim carpeta As String = IO.Path.GetDirectoryName(TxtRuta.Text)
            If Not String.IsNullOrEmpty(carpeta) AndAlso IO.Directory.Exists(carpeta) Then Return carpeta
        Catch ex As ArgumentException
            ' Ruta con caracteres no válidos: se usa la carpeta por defecto.
        End Try

        Return If(IO.Directory.Exists(CsRespaldoDAL.CarpetaPorDefecto()), CsRespaldoDAL.CarpetaPorDefecto(), My.Application.Info.DirectoryPath)

    End Function

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
