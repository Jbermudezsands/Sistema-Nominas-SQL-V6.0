<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmInssIr
    Inherits DevExpress.XtraEditors.XtraForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.TabControl = New DevExpress.XtraTab.XtraTabControl()
        Me.PageInss = New DevExpress.XtraTab.XtraTabPage()
        Me.gridInss = New DevExpress.XtraGrid.GridControl()
        Me.gridViewInss = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.PageIr = New DevExpress.XtraTab.XtraTabPage()
        Me.gridIr = New DevExpress.XtraGrid.GridControl()
        Me.gridViewIr = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.panelBotones = New DevExpress.XtraEditors.PanelControl()
        Me.LblAyuda = New DevExpress.XtraEditors.LabelControl()
        Me.BtnGuardar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnRecargar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnExcel = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        CType(Me.TabControl, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl.SuspendLayout()
        Me.PageInss.SuspendLayout()
        CType(Me.gridInss, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewInss, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PageIr.SuspendLayout()
        CType(Me.gridIr, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewIr, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl
        '
        Me.TabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl.Location = New System.Drawing.Point(0, 0)
        Me.TabControl.Name = "TabControl"
        Me.TabControl.SelectedTabPage = Me.PageInss
        Me.TabControl.Size = New System.Drawing.Size(900, 369)
        Me.TabControl.TabIndex = 0
        Me.TabControl.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.PageInss, Me.PageIr})
        '
        'PageInss
        '
        Me.PageInss.Controls.Add(Me.gridInss)
        Me.PageInss.Name = "PageInss"
        Me.PageInss.Size = New System.Drawing.Size(898, 344)
        Me.PageInss.Text = "INSS"
        '
        'gridInss
        '
        Me.gridInss.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridInss.Location = New System.Drawing.Point(0, 0)
        Me.gridInss.MainView = Me.gridViewInss
        Me.gridInss.Name = "gridInss"
        Me.gridInss.Size = New System.Drawing.Size(898, 344)
        Me.gridInss.TabIndex = 0
        Me.gridInss.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewInss})
        '
        'gridViewInss
        '
        Me.gridViewInss.GridControl = Me.gridInss
        Me.gridViewInss.Name = "gridViewInss"
        Me.gridViewInss.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.[True]
        Me.gridViewInss.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top
        Me.gridViewInss.OptionsView.ShowGroupPanel = False
        '
        'PageIr
        '
        Me.PageIr.Controls.Add(Me.gridIr)
        Me.PageIr.Name = "PageIr"
        Me.PageIr.Size = New System.Drawing.Size(898, 344)
        Me.PageIr.Text = "IR"
        '
        'gridIr
        '
        Me.gridIr.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridIr.Location = New System.Drawing.Point(0, 0)
        Me.gridIr.MainView = Me.gridViewIr
        Me.gridIr.Name = "gridIr"
        Me.gridIr.Size = New System.Drawing.Size(898, 344)
        Me.gridIr.TabIndex = 0
        Me.gridIr.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewIr})
        '
        'gridViewIr
        '
        Me.gridViewIr.GridControl = Me.gridIr
        Me.gridViewIr.Name = "gridViewIr"
        Me.gridViewIr.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.[True]
        Me.gridViewIr.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top
        Me.gridViewIr.OptionsView.ShowGroupPanel = False
        '
        'panelBotones
        '
        Me.panelBotones.Controls.Add(Me.LblAyuda)
        Me.panelBotones.Controls.Add(Me.BtnGuardar)
        Me.panelBotones.Controls.Add(Me.BtnRecargar)
        Me.panelBotones.Controls.Add(Me.BtnExcel)
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Location = New System.Drawing.Point(0, 369)
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(900, 55)
        Me.panelBotones.TabIndex = 1
        '
        'LblAyuda
        '
        Me.LblAyuda.Location = New System.Drawing.Point(450, 21)
        Me.LblAyuda.Name = "LblAyuda"
        Me.LblAyuda.Size = New System.Drawing.Size(385, 13)
        Me.LblAyuda.TabIndex = 4
        Me.LblAyuda.Text = "Fila superior = nueva fila.  Tecla Supr = borrar la fila.  Grabar aplica los camb" &
    "ios."
        '
        'BtnGuardar
        '
        Me.BtnGuardar.Location = New System.Drawing.Point(13, 10)
        Me.BtnGuardar.Name = "BtnGuardar"
        Me.BtnGuardar.Size = New System.Drawing.Size(95, 35)
        Me.BtnGuardar.TabIndex = 0
        Me.BtnGuardar.Text = "Grabar"
        '
        'BtnRecargar
        '
        Me.BtnRecargar.Location = New System.Drawing.Point(119, 10)
        Me.BtnRecargar.Name = "BtnRecargar"
        Me.BtnRecargar.Size = New System.Drawing.Size(95, 35)
        Me.BtnRecargar.TabIndex = 1
        Me.BtnRecargar.Text = "Recargar"
        '
        'BtnExcel
        '
        Me.BtnExcel.Location = New System.Drawing.Point(225, 10)
        Me.BtnExcel.Name = "BtnExcel"
        Me.BtnExcel.Size = New System.Drawing.Size(95, 35)
        Me.BtnExcel.TabIndex = 2
        Me.BtnExcel.Text = "Excel"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(331, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 3
        Me.BtnSalir.Text = "Salir"
        '
        'FrmInssIr
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 424)
        Me.Controls.Add(Me.TabControl)
        Me.Controls.Add(Me.panelBotones)
        Me.Name = "FrmInssIr"
        Me.Text = "Tabla del INSS / IR"
        CType(Me.TabControl, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl.ResumeLayout(False)
        Me.PageInss.ResumeLayout(False)
        CType(Me.gridInss, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewInss, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PageIr.ResumeLayout(False)
        CType(Me.gridIr, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewIr, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        Me.panelBotones.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TabControl As DevExpress.XtraTab.XtraTabControl
    Friend WithEvents PageInss As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents gridInss As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewInss As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents PageIr As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents gridIr As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewIr As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LblAyuda As DevExpress.XtraEditors.LabelControl
    Friend WithEvents BtnGuardar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnRecargar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnExcel As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
End Class
