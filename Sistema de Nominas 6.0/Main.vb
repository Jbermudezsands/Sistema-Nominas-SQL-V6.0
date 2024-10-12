Public Class Main
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub RibbonControl_Click(sender As Object, e As EventArgs) Handles RibbonControl.Click

    End Sub

    Private Sub BarButtonIEmpleado_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonIEmpleado.ItemClick
        My.Forms.FrmEmpleados.MdiParent = Me
        My.Forms.FrmEmpleados.Show()
    End Sub

    Private Sub BarButtonSalir_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonSalir.ItemClick
        Me.Close()
    End Sub
End Class