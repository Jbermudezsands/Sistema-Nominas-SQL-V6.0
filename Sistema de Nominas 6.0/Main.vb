Public Class Main
    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub RibbonControl_Click(sender As Object, e As EventArgs) Handles RibbonControl.Click

    End Sub

    Private Sub BarButtonIEmpleado_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonIEmpleado.ItemClick
        AbrirFormulario(My.Forms.FrmEmpleados)
    End Sub

    Private Sub BarButtonListaEmpleado_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonListaEmpleado.ItemClick
        AbrirFormulario(My.Forms.FrmListadoEmpleados)
    End Sub

    Private Sub BarButtonHorasExtra_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonHorasExtra.ItemClick
        AbrirFormulario(My.Forms.FrmListadoHorasExtra)
    End Sub

    Private Sub BarButtonActivarNomina_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonActivarNomina.ItemClick
        AbrirFormulario(My.Forms.FrmActivarNomina)
    End Sub

    Private Sub BarButtonListaNomina_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonListaNomina.ItemClick
        AbrirFormulario(My.Forms.FrmListNomina)
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

    Private Sub BarButtonTasaCambio_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonTasaCambio.ItemClick
        AbrirFormulario(My.Forms.FrmTasaCambio)
    End Sub

    Private Sub BarButtonCargaMasiva_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonCargaMasiva.ItemClick
        AbrirFormulario(My.Forms.FrmCargaMasiva)
    End Sub

    Private Sub BarButtonHistorial_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonHistorial.ItemClick
        AbrirFormulario(My.Forms.FrmHistorial)
    End Sub

    Private Sub BarButtonImportar_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonImportar.ItemClick
        AbrirFormulario(My.Forms.FrmImportar)
    End Sub

    Private Sub BarButtonJustificaciones_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonJustificaciones.ItemClick
        AbrirFormulario(My.Forms.FrmJustificaciones)
    End Sub

    Private Sub BarButtonQuery_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonQuery.ItemClick
        AbrirFormulario(My.Forms.FrmQuery)
    End Sub

    Private Sub BarButtonInssIr_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonInssIr.ItemClick
        AbrirFormulario(My.Forms.FrmInssIr)
    End Sub

    Private Sub BarButtonIncapacidades_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonIncapacidades.ItemClick
        AbrirFormulario(My.Forms.FrmIncapacidades)
    End Sub

    Private Sub BarButtonTipoComision_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonTipoComision.ItemClick
        AbrirFormulario(My.Forms.FrmTipoComision)
    End Sub

    Private Sub BarButtonTipoDestajo_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonTipoDestajo.ItemClick
        AbrirFormulario(My.Forms.FrmTipoDestajo)
    End Sub

    Private Sub BarButtonTipoIncapacidad_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonTipoIncapacidad.ItemClick
        AbrirFormulario(My.Forms.FrmTipoIncapacidad)
    End Sub
    ''' <summary>Página "Procesos" > Bajas; "Despidos y Renuncias" de la página Acceso abre la misma pantalla.</summary>
    Private Sub BarButtonBajas_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonBajas.ItemClick, BarButtonDespidos.ItemClick
        AbrirFormulario(My.Forms.FrmBajas)
    End Sub

    ''' <summary>Página "Procesos" > Vacaciones: solicitudes de vacaciones, ausencias, subsidios, etc. (V.5.0 FrmSolicitud).</summary>
    Private Sub BarButtonVacaciones_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonVacaciones.ItemClick

    End Sub

    Private Sub BarButtonUsuarios_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonUsuarios.ItemClick
        AbrirFormulario(My.Forms.FrmUsuarios)
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

    Private Sub BarButtonSolicitud_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonSolicitud.ItemClick
        AbrirFormulario(My.Forms.FrmSolicitud)
    End Sub
End Class
