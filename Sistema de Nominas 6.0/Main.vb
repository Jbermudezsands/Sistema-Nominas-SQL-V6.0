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

    ''' <summary>Página "Procesos" > Vacaciones: nómina de vacaciones (V.5.0 Frm13VacaMes, parte de vacaciones). Las solicitudes tienen su propio botón.</summary>
    Private Sub BarButtonVacaciones_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonVacaciones.ItemClick
        AbrirUnico(Of FrmNominaVacaciones)(Function() New FrmNominaVacaciones)
    End Sub

    ''' <summary>Página "Procesos" > Aguinaldo: nómina de 13vo mes (V.5.0 Frm13VacaMes, parte del 13vo).</summary>
    Private Sub BarButtonAguinaldo_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonAguinaldo.ItemClick
        AbrirUnico(Of FrmAguinaldo)(Function() New FrmAguinaldo)
    End Sub

    ''' <summary>Página "Historicos": listado de nóminas de vacaciones (V.5.0 FrmListadoNominaVacaciones, pestaña de vacaciones).</summary>
    Private Sub BarButtonListadoVacaciones_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonListadoVacaciones.ItemClick
        AbrirUnico(Of FrmListadoNominaVacaciones)(Function() New FrmListadoNominaVacaciones)
    End Sub

    ''' <summary>Página "Historicos": listado de nóminas de 13vo mes (pestaña de 13vo de la V.5.0).</summary>
    Private Sub BarButtonListado13vo_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonListado13vo.ItemClick
        AbrirUnico(Of FrmListadoNomina13vo)(Function() New FrmListadoNomina13vo)
    End Sub

    ''' <summary>Historial salarial: se abre en la pestaña del aguinaldo y el usuario indica el número de nómina.</summary>
    Private Sub BarButtonHistorialSalarial_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonHistorialSalarial.ItemClick
        AbrirUnico(Of FrmSalarioHistorial)(Function() New FrmSalarioHistorial(CsHistorialSalarialDAL.TipoAguinaldo, 0, 0))
    End Sub

    ''' <summary>Para formularios que se crean con <c>New</c> (no con My.Forms): trae al frente el ya abierto o crea uno nuevo como hijo MDI.</summary>
    Private Sub AbrirUnico(Of T As Form)(crear As Func(Of T))
        Dim abierto As Form = MdiChildren.FirstOrDefault(Function(f) TypeOf f Is T AndAlso Not f.IsDisposed)
        AbrirFormulario(If(abierto, crear()))
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
