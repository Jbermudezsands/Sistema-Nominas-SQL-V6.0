Public Class SplashScreen


    Sub New()
        InitializeComponent()
        Me.labelCopyright.Text = "Copyright © 2010 -" & DateTime.Now.Year.ToString()
    End Sub

    Public Overrides Sub ProcessCommand(ByVal cmd As System.Enum, ByVal arg As Object)
        MyBase.ProcessCommand(cmd, arg)
    End Sub

    Public Enum SplashScreenCommand
        SomeCommandId
    End Enum

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Dim Compañia As New Compañias, frmListaCompañias As New FrmListadoCompañias

        If Timer1.Interval < 40 Then
            Me.Timer1.Interval = Me.Timer1.Interval + 1
        ElseIf Me.Timer1.Interval <> 200 Then

            If (Compañia.ConsultaCompañia() > 1) Then
                Timer1.Enabled = False
                Me.Hide()
                frmListaCompañias.ShowDialog()
                Me.Close()
            Else
                Main.Show()
            End If

        End If
    End Sub

    Private Sub SplashScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub peImage_EditValueChanged(sender As Object, e As EventArgs) Handles peImage.EditValueChanged

    End Sub
End Class
