<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Main
    Inherits DevExpress.XtraBars.Ribbon.RibbonForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.RibbonControl = New DevExpress.XtraBars.Ribbon.RibbonControl()
        Me.RibbonPage1 = New DevExpress.XtraBars.Ribbon.RibbonPage()
        Me.RibbonPageProcesos = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonStatusBar = New DevExpress.XtraBars.Ribbon.RibbonStatusBar()
        Me.RibbonEmpleados = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonIEmpleado = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonListaEmpleado = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonActivarNomina = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonHorasExtra = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonCalcularNomina = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonVacaciones = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonDespidos = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonListaLiquidacion = New DevExpress.XtraBars.BarButtonItem()
        Me.RibbonPageGroup1 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.BarButtonPeriodoNomina = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonPeriodoFiscal = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonTipoNomina = New DevExpress.XtraBars.BarButtonItem()
        Me.RibbonPageGroup2 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.BarButtonListaNomina = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonHistorialSalarial = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonListaVacaciones = New DevExpress.XtraBars.BarButtonItem()
        Me.RibbonPageGroup3 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.BarButtonTasaCambio = New DevExpress.XtraBars.BarButtonItem()
        Me.BarToggleSwitchItem1 = New DevExpress.XtraBars.BarToggleSwitchItem()
        Me.BarMdiChildrenListItem1 = New DevExpress.XtraBars.BarMdiChildrenListItem()
        Me.BarButtonUsuarios = New DevExpress.XtraBars.BarButtonItem()
        Me.RibbonPageGroup4 = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.BarButtonItem16 = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonSalir = New DevExpress.XtraBars.BarButtonItem()
        Me.BarButtonRespaldar = New DevExpress.XtraBars.BarButtonItem()
        CType(Me.RibbonControl, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RibbonControl
        '
        Me.RibbonControl.ExpandCollapseItem.Id = 0
        Me.RibbonControl.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.RibbonControl.ExpandCollapseItem, Me.RibbonControl.SearchEditItem, Me.BarButtonIEmpleado, Me.BarButtonListaEmpleado, Me.BarButtonActivarNomina, Me.BarButtonHorasExtra, Me.BarButtonCalcularNomina, Me.BarButtonVacaciones, Me.BarButtonDespidos, Me.BarButtonListaLiquidacion, Me.BarButtonPeriodoNomina, Me.BarButtonPeriodoFiscal, Me.BarButtonTipoNomina, Me.BarButtonListaNomina, Me.BarButtonHistorialSalarial, Me.BarButtonListaVacaciones, Me.BarButtonTasaCambio, Me.BarToggleSwitchItem1, Me.BarMdiChildrenListItem1, Me.BarButtonUsuarios, Me.BarButtonItem16, Me.BarButtonSalir, Me.BarButtonRespaldar})
        Me.RibbonControl.Location = New System.Drawing.Point(0, 0)
        Me.RibbonControl.MaxItemId = 23
        Me.RibbonControl.Name = "RibbonControl"
        Me.RibbonControl.Pages.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPage() {Me.RibbonPage1})
        Me.RibbonControl.Size = New System.Drawing.Size(1260, 158)
        Me.RibbonControl.StatusBar = Me.RibbonStatusBar
        '
        'RibbonPage1
        '
        Me.RibbonPage1.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.RibbonPageProcesos, Me.RibbonPageGroup1, Me.RibbonPageGroup2, Me.RibbonPageGroup3, Me.RibbonPageGroup4})
        Me.RibbonPage1.Name = "RibbonPage1"
        Me.RibbonPage1.Text = "Acceso"
        '
        'RibbonPageProcesos
        '
        Me.RibbonPageProcesos.ItemLinks.Add(Me.BarButtonIEmpleado)
        Me.RibbonPageProcesos.ItemLinks.Add(Me.BarButtonListaEmpleado)
        Me.RibbonPageProcesos.ItemLinks.Add(Me.BarButtonActivarNomina)
        Me.RibbonPageProcesos.ItemLinks.Add(Me.BarButtonHorasExtra)
        Me.RibbonPageProcesos.ItemLinks.Add(Me.BarButtonCalcularNomina)
        Me.RibbonPageProcesos.ItemLinks.Add(Me.BarButtonVacaciones)
        Me.RibbonPageProcesos.ItemLinks.Add(Me.BarButtonDespidos)
        Me.RibbonPageProcesos.Name = "RibbonPageProcesos"
        Me.RibbonPageProcesos.Text = "Procesos"
        '
        'RibbonStatusBar
        '
        Me.RibbonStatusBar.ItemLinks.Add(Me.BarMdiChildrenListItem1)
        Me.RibbonStatusBar.Location = New System.Drawing.Point(0, 512)
        Me.RibbonStatusBar.Name = "RibbonStatusBar"
        Me.RibbonStatusBar.Ribbon = Me.RibbonControl
        Me.RibbonStatusBar.Size = New System.Drawing.Size(1260, 24)
        '
        'RibbonEmpleados
        '
        Me.RibbonEmpleados.Caption = "Empleados"
        Me.RibbonEmpleados.Id = 3
        Me.RibbonEmpleados.ImageOptions.Image = CType(resources.GetObject("RibbonEmpleados.ImageOptions.Image"), System.Drawing.Image)
        Me.RibbonEmpleados.ImageOptions.LargeImage = CType(resources.GetObject("RibbonEmpleados.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.RibbonEmpleados.Name = "RibbonEmpleados"
        '
        'BarButtonIEmpleado
        '
        Me.BarButtonIEmpleado.Caption = "Empleados"
        Me.BarButtonIEmpleado.Id = 1
        Me.BarButtonIEmpleado.ImageOptions.Image = CType(resources.GetObject("BarButtonIEmpleado.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonIEmpleado.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonIEmpleado.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonIEmpleado.Name = "BarButtonIEmpleado"
        '
        'BarButtonListaEmpleado
        '
        Me.BarButtonListaEmpleado.Caption = "Lista Empleados"
        Me.BarButtonListaEmpleado.Id = 2
        Me.BarButtonListaEmpleado.ImageOptions.Image = CType(resources.GetObject("BarButtonItem1.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonListaEmpleado.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem1.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonListaEmpleado.Name = "BarButtonListaEmpleado"
        '
        'BarButtonActivarNomina
        '
        Me.BarButtonActivarNomina.Caption = "Activar Nomina"
        Me.BarButtonActivarNomina.Id = 3
        Me.BarButtonActivarNomina.ImageOptions.Image = CType(resources.GetObject("BarButtonItem2.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonActivarNomina.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem2.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonActivarNomina.Name = "BarButtonActivarNomina"
        '
        'BarButtonHorasExtra
        '
        Me.BarButtonHorasExtra.Caption = "Horas Extra"
        Me.BarButtonHorasExtra.Id = 4
        Me.BarButtonHorasExtra.ImageOptions.Image = CType(resources.GetObject("BarButtonItem3.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonHorasExtra.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem3.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonHorasExtra.Name = "BarButtonHorasExtra"
        '
        'BarButtonCalcularNomina
        '
        Me.BarButtonCalcularNomina.Caption = "Calcular Nomina"
        Me.BarButtonCalcularNomina.Id = 5
        Me.BarButtonCalcularNomina.ImageOptions.Image = CType(resources.GetObject("BarButtonItem4.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonCalcularNomina.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem4.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonCalcularNomina.Name = "BarButtonCalcularNomina"
        '
        'BarButtonVacaciones
        '
        Me.BarButtonVacaciones.Caption = "Vacaciones"
        Me.BarButtonVacaciones.Id = 6
        Me.BarButtonVacaciones.ImageOptions.Image = CType(resources.GetObject("BarButtonItem5.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonVacaciones.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem5.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonVacaciones.Name = "BarButtonVacaciones"
        '
        'BarButtonDespidos
        '
        Me.BarButtonDespidos.Caption = "Despidos y Renuncias"
        Me.BarButtonDespidos.Id = 7
        Me.BarButtonDespidos.ImageOptions.Image = CType(resources.GetObject("BarButtonItem6.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonDespidos.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem6.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonDespidos.Name = "BarButtonDespidos"
        '
        'BarButtonListaLiquidacion
        '
        Me.BarButtonListaLiquidacion.Caption = "Lista Liquidacion"
        Me.BarButtonListaLiquidacion.Id = 8
        Me.BarButtonListaLiquidacion.ImageOptions.Image = CType(resources.GetObject("BarButtonItem7.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonListaLiquidacion.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem7.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonListaLiquidacion.Name = "BarButtonListaLiquidacion"
        '
        'RibbonPageGroup1
        '
        Me.RibbonPageGroup1.ItemLinks.Add(Me.BarButtonPeriodoNomina)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.BarButtonPeriodoFiscal)
        Me.RibbonPageGroup1.ItemLinks.Add(Me.BarButtonTipoNomina)
        Me.RibbonPageGroup1.Name = "RibbonPageGroup1"
        Me.RibbonPageGroup1.Text = "Catalogo"
        '
        'BarButtonPeriodoNomina
        '
        Me.BarButtonPeriodoNomina.Caption = "Periodo Nomina"
        Me.BarButtonPeriodoNomina.Id = 9
        Me.BarButtonPeriodoNomina.ImageOptions.Image = CType(resources.GetObject("BarButtonItem8.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonPeriodoNomina.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem8.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonPeriodoNomina.Name = "BarButtonPeriodoNomina"
        '
        'BarButtonPeriodoFiscal
        '
        Me.BarButtonPeriodoFiscal.Caption = "Periodo Fiscal"
        Me.BarButtonPeriodoFiscal.Id = 10
        Me.BarButtonPeriodoFiscal.ImageOptions.Image = CType(resources.GetObject("BarButtonItem9.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonPeriodoFiscal.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem9.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonPeriodoFiscal.Name = "BarButtonPeriodoFiscal"
        '
        'BarButtonTipoNomina
        '
        Me.BarButtonTipoNomina.Caption = "Tipo Nomina"
        Me.BarButtonTipoNomina.Id = 11
        Me.BarButtonTipoNomina.ImageOptions.Image = CType(resources.GetObject("BarButtonItem10.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonTipoNomina.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem10.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonTipoNomina.Name = "BarButtonTipoNomina"
        '
        'RibbonPageGroup2
        '
        Me.RibbonPageGroup2.ItemLinks.Add(Me.BarButtonListaNomina)
        Me.RibbonPageGroup2.ItemLinks.Add(Me.BarButtonHistorialSalarial)
        Me.RibbonPageGroup2.ItemLinks.Add(Me.BarButtonListaLiquidacion)
        Me.RibbonPageGroup2.ItemLinks.Add(Me.BarButtonListaVacaciones)
        Me.RibbonPageGroup2.Name = "RibbonPageGroup2"
        Me.RibbonPageGroup2.Text = "Historicos"
        '
        'BarButtonListaNomina
        '
        Me.BarButtonListaNomina.Caption = "Listado de Nominas"
        Me.BarButtonListaNomina.Id = 12
        Me.BarButtonListaNomina.ImageOptions.Image = CType(resources.GetObject("BarButtonItem11.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonListaNomina.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem11.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonListaNomina.Name = "BarButtonListaNomina"
        '
        'BarButtonHistorialSalarial
        '
        Me.BarButtonHistorialSalarial.Caption = "Historial Salarial"
        Me.BarButtonHistorialSalarial.Id = 13
        Me.BarButtonHistorialSalarial.ImageOptions.Image = CType(resources.GetObject("BarButtonItem12.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonHistorialSalarial.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem12.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonHistorialSalarial.Name = "BarButtonHistorialSalarial"
        '
        'BarButtonListaVacaciones
        '
        Me.BarButtonListaVacaciones.Caption = "Listado de Vacaciones"
        Me.BarButtonListaVacaciones.Id = 14
        Me.BarButtonListaVacaciones.ImageOptions.Image = CType(resources.GetObject("BarButtonItem13.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonListaVacaciones.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem13.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonListaVacaciones.Name = "BarButtonListaVacaciones"
        '
        'RibbonPageGroup3
        '
        Me.RibbonPageGroup3.ItemLinks.Add(Me.BarButtonTasaCambio)
        Me.RibbonPageGroup3.ItemLinks.Add(Me.BarButtonUsuarios)
        Me.RibbonPageGroup3.ItemLinks.Add(Me.BarButtonRespaldar)
        Me.RibbonPageGroup3.Name = "RibbonPageGroup3"
        Me.RibbonPageGroup3.Text = "Opciones"
        '
        'BarButtonTasaCambio
        '
        Me.BarButtonTasaCambio.Caption = "Tasas de Cambio"
        Me.BarButtonTasaCambio.Id = 16
        Me.BarButtonTasaCambio.ImageOptions.Image = CType(resources.GetObject("BarButtonItem14.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonTasaCambio.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem14.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonTasaCambio.Name = "BarButtonTasaCambio"
        '
        'BarToggleSwitchItem1
        '
        Me.BarToggleSwitchItem1.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right
        Me.BarToggleSwitchItem1.Caption = "BarToggleSwitchItem1"
        Me.BarToggleSwitchItem1.Id = 17
        Me.BarToggleSwitchItem1.Name = "BarToggleSwitchItem1"
        '
        'BarMdiChildrenListItem1
        '
        Me.BarMdiChildrenListItem1.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right
        Me.BarMdiChildrenListItem1.Caption = "Ventanas Activas"
        Me.BarMdiChildrenListItem1.Id = 18
        Me.BarMdiChildrenListItem1.Name = "BarMdiChildrenListItem1"
        '
        'BarButtonUsuarios
        '
        Me.BarButtonUsuarios.Caption = "Usuarios"
        Me.BarButtonUsuarios.Id = 19
        Me.BarButtonUsuarios.ImageOptions.Image = CType(resources.GetObject("BarButtonItem15.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonUsuarios.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem15.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonUsuarios.Name = "BarButtonUsuarios"
        '
        'RibbonPageGroup4
        '
        Me.RibbonPageGroup4.ItemLinks.Add(Me.BarButtonSalir)
        Me.RibbonPageGroup4.Name = "RibbonPageGroup4"
        Me.RibbonPageGroup4.Text = "Ayuda"
        '
        'BarButtonItem16
        '
        Me.BarButtonItem16.ActAsDropDown = True
        Me.BarButtonItem16.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown
        Me.BarButtonItem16.Caption = "Respaldar"
        Me.BarButtonItem16.Id = 20
        Me.BarButtonItem16.ImageOptions.AllowStubGlyph = DevExpress.Utils.DefaultBoolean.[False]
        Me.BarButtonItem16.ImageOptions.Image = CType(resources.GetObject("BarButtonItem16.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonItem16.Name = "BarButtonItem16"
        '
        'BarButtonSalir
        '
        Me.BarButtonSalir.Caption = "Salir"
        Me.BarButtonSalir.Id = 21
        Me.BarButtonSalir.ImageOptions.Image = CType(resources.GetObject("BarButtonItem17.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonSalir.ImageOptions.LargeImage = CType(resources.GetObject("BarButtonItem17.ImageOptions.LargeImage"), System.Drawing.Image)
        Me.BarButtonSalir.Name = "BarButtonSalir"
        Me.BarButtonSalir.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BarButtonRespaldar
        '
        Me.BarButtonRespaldar.Caption = "Respaldar"
        Me.BarButtonRespaldar.Id = 22
        Me.BarButtonRespaldar.ImageOptions.Image = CType(resources.GetObject("BarButtonItem18.ImageOptions.Image"), System.Drawing.Image)
        Me.BarButtonRespaldar.Name = "BarButtonRespaldar"
        Me.BarButtonRespaldar.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1260, 536)
        Me.Controls.Add(Me.RibbonStatusBar)
        Me.Controls.Add(Me.RibbonControl)
        Me.IconOptions.LargeImage = CType(resources.GetObject("Main.IconOptions.LargeImage"), System.Drawing.Image)
        Me.IsMdiContainer = True
        Me.Name = "Main"
        Me.Ribbon = Me.RibbonControl
        Me.StatusBar = Me.RibbonStatusBar
        Me.Text = "Zeus Nominas - Licencia; Systems And Solutions"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        CType(Me.RibbonControl, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents RibbonControl As DevExpress.XtraBars.Ribbon.RibbonControl
    Friend WithEvents RibbonPage1 As DevExpress.XtraBars.Ribbon.RibbonPage
    Friend WithEvents RibbonPageProcesos As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents RibbonStatusBar As DevExpress.XtraBars.Ribbon.RibbonStatusBar
    Friend WithEvents BarButtonIEmpleado As DevExpress.XtraBars.BarButtonItem
    Private WithEvents RibbonEmpleados As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonListaEmpleado As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonActivarNomina As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonHorasExtra As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonCalcularNomina As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonVacaciones As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonDespidos As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonListaLiquidacion As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonPeriodoNomina As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonPeriodoFiscal As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonTipoNomina As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonListaNomina As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonHistorialSalarial As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonListaVacaciones As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonTasaCambio As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarToggleSwitchItem1 As DevExpress.XtraBars.BarToggleSwitchItem
    Friend WithEvents BarMdiChildrenListItem1 As DevExpress.XtraBars.BarMdiChildrenListItem
    Friend WithEvents BarButtonUsuarios As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonItem16 As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents BarButtonSalir As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents RibbonPageGroup1 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents RibbonPageGroup2 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents RibbonPageGroup3 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents RibbonPageGroup4 As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Friend WithEvents BarButtonRespaldar As DevExpress.XtraBars.BarButtonItem
End Class
