Public Class Main
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub RibbonControl_Click(sender As Object, e As EventArgs) Handles RibbonControl.Click

    End Sub

    Private Sub BarButtonIEmpleado_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonIEmpleado.ItemClick
        AbrirFormulario(My.Forms.FrmEmpleados)
    End Sub

    Private Sub BarButtonCargo_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonCargo.ItemClick
        AbrirFormulario(My.Forms.FrmCargo)
    End Sub

    Private Sub BarButtonDepartamento_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonDepartamento.ItemClick
        AbrirFormulario(My.Forms.FrmDepartamento)
    End Sub

    Private Sub BarButtonDeduccion_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonDeduccion.ItemClick
        AbrirFormulario(My.Forms.FrmDeduccion)
    End Sub

    Private Sub BarButtonControles_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonControles.ItemClick
        AbrirFormulario(My.Forms.FrmControles)
    End Sub

    Private Sub BarButtonTipoNomina_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonTipoNomina.ItemClick
        AbrirFormulario(My.Forms.FrmTipoNomina)
    End Sub

    Private Sub BarButtonPeriodoNomina_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonPeriodoNomina.ItemClick
        AbrirFormulario(New FrmPeriodo(TipoCalendario.Planilla))
    End Sub

    Private Sub BarButtonPeriodoFiscal_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonPeriodoFiscal.ItemClick
        AbrirFormulario(New FrmPeriodo(TipoCalendario.Fiscal))
    End Sub

    Private Sub BarButtonSalir_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonSalir.ItemClick
        Me.Close()
    End Sub

    ''' <summary>Muestra el formulario como hijo MDI; si ya está abierto lo trae al frente.</summary>
    Private Sub AbrirFormulario(frm As Form)
        frm.MdiParent = Me
        frm.Show()
        frm.Activate()
    End Sub
End Class
