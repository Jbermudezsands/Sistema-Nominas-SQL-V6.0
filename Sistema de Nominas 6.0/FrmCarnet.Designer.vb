<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmCarnet
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
        Me.components = New System.ComponentModel.Container()
        Me.PanelCarnet = New DevExpress.XtraEditors.PanelControl()
        Me.PicFoto = New System.Windows.Forms.PictureBox()
        Me.LblNombres = New DevExpress.XtraEditors.LabelControl()
        Me.LblApellidos = New DevExpress.XtraEditors.LabelControl()
        Me.LblCargo = New DevExpress.XtraEditors.LabelControl()
        Me.LblCodigo = New DevExpress.XtraEditors.LabelControl()
        Me.PicBarras = New System.Windows.Forms.PictureBox()
        Me.LblCodigoBarras = New DevExpress.XtraEditors.LabelControl()
        Me.panelBotones = New DevExpress.XtraEditors.PanelControl()
        Me.BtnImprimir = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        CType(Me.PanelCarnet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelCarnet.SuspendLayout()
        CType(Me.PicFoto, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PicBarras, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelCarnet
        '
        Me.PanelCarnet.Appearance.BackColor = System.Drawing.Color.White
        Me.PanelCarnet.Appearance.Options.UseBackColor = True
        Me.PanelCarnet.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.PanelCarnet.Controls.Add(Me.LblCodigoBarras)
        Me.PanelCarnet.Controls.Add(Me.PicBarras)
        Me.PanelCarnet.Controls.Add(Me.LblCodigo)
        Me.PanelCarnet.Controls.Add(Me.LblCargo)
        Me.PanelCarnet.Controls.Add(Me.LblApellidos)
        Me.PanelCarnet.Controls.Add(Me.LblNombres)
        Me.PanelCarnet.Controls.Add(Me.PicFoto)
        Me.PanelCarnet.Location = New System.Drawing.Point(12, 12)
        Me.PanelCarnet.Name = "PanelCarnet"
        Me.PanelCarnet.Size = New System.Drawing.Size(360, 240)
        Me.PanelCarnet.TabIndex = 0
        '
        'PicFoto
        '
        Me.PicFoto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicFoto.Location = New System.Drawing.Point(12, 12)
        Me.PicFoto.Name = "PicFoto"
        Me.PicFoto.Size = New System.Drawing.Size(110, 130)
        Me.PicFoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PicFoto.TabIndex = 0
        Me.PicFoto.TabStop = False
        '
        'LblNombres
        '
        Me.LblNombres.Appearance.Font = New System.Drawing.Font("Tahoma", 11.0!, System.Drawing.FontStyle.Bold)
        Me.LblNombres.Appearance.Options.UseFont = True
        Me.LblNombres.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LblNombres.Location = New System.Drawing.Point(132, 14)
        Me.LblNombres.Name = "LblNombres"
        Me.LblNombres.Size = New System.Drawing.Size(216, 22)
        Me.LblNombres.TabIndex = 1
        '
        'LblApellidos
        '
        Me.LblApellidos.Appearance.Font = New System.Drawing.Font("Tahoma", 11.0!, System.Drawing.FontStyle.Bold)
        Me.LblApellidos.Appearance.Options.UseFont = True
        Me.LblApellidos.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LblApellidos.Location = New System.Drawing.Point(132, 42)
        Me.LblApellidos.Name = "LblApellidos"
        Me.LblApellidos.Size = New System.Drawing.Size(216, 22)
        Me.LblApellidos.TabIndex = 2
        '
        'LblCargo
        '
        Me.LblCargo.Appearance.Font = New System.Drawing.Font("Tahoma", 9.0!)
        Me.LblCargo.Appearance.Options.UseFont = True
        Me.LblCargo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LblCargo.Location = New System.Drawing.Point(132, 76)
        Me.LblCargo.Name = "LblCargo"
        Me.LblCargo.Size = New System.Drawing.Size(216, 18)
        Me.LblCargo.TabIndex = 3
        '
        'LblCodigo
        '
        Me.LblCodigo.Appearance.Font = New System.Drawing.Font("Tahoma", 14.0!, System.Drawing.FontStyle.Bold)
        Me.LblCodigo.Appearance.Options.UseFont = True
        Me.LblCodigo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LblCodigo.Location = New System.Drawing.Point(132, 108)
        Me.LblCodigo.Name = "LblCodigo"
        Me.LblCodigo.Size = New System.Drawing.Size(216, 30)
        Me.LblCodigo.TabIndex = 4
        '
        'PicBarras
        '
        Me.PicBarras.Location = New System.Drawing.Point(12, 152)
        Me.PicBarras.Name = "PicBarras"
        Me.PicBarras.Size = New System.Drawing.Size(336, 56)
        Me.PicBarras.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PicBarras.TabIndex = 5
        Me.PicBarras.TabStop = False
        '
        'LblCodigoBarras
        '
        Me.LblCodigoBarras.Appearance.Options.UseTextOptions = True
        Me.LblCodigoBarras.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
        Me.LblCodigoBarras.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LblCodigoBarras.Location = New System.Drawing.Point(12, 212)
        Me.LblCodigoBarras.Name = "LblCodigoBarras"
        Me.LblCodigoBarras.Size = New System.Drawing.Size(336, 16)
        Me.LblCodigoBarras.TabIndex = 6
        '
        'panelBotones
        '
        Me.panelBotones.Controls.Add(Me.BtnImprimir)
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Location = New System.Drawing.Point(0, 264)
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(384, 55)
        Me.panelBotones.TabIndex = 1
        '
        'BtnImprimir
        '
        Me.BtnImprimir.Location = New System.Drawing.Point(13, 10)
        Me.BtnImprimir.Name = "BtnImprimir"
        Me.BtnImprimir.Size = New System.Drawing.Size(95, 35)
        Me.BtnImprimir.TabIndex = 0
        Me.BtnImprimir.Text = "Imprimir"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(119, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 1
        Me.BtnSalir.Text = "Salir"
        '
        'FrmCarnet
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(384, 319)
        Me.Controls.Add(Me.panelBotones)
        Me.Controls.Add(Me.PanelCarnet)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmCarnet"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Carnet del Empleado"
        CType(Me.PanelCarnet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelCarnet.ResumeLayout(False)
        Me.PanelCarnet.PerformLayout()
        CType(Me.PicFoto, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PicBarras, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelCarnet As DevExpress.XtraEditors.PanelControl
    Friend WithEvents PicFoto As System.Windows.Forms.PictureBox
    Friend WithEvents LblNombres As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LblApellidos As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LblCargo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LblCodigo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents PicBarras As System.Windows.Forms.PictureBox
    Friend WithEvents LblCodigoBarras As DevExpress.XtraEditors.LabelControl
    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnImprimir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
End Class
