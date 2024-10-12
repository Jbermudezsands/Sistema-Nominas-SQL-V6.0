<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SplashScreen
    Inherits DevExpress.XtraSplashScreen.SplashScreen

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SplashScreen))
        Me.peImage = New DevExpress.XtraEditors.PictureEdit()
        Me.labelStatus = New DevExpress.XtraEditors.LabelControl()
        Me.labelCopyright = New DevExpress.XtraEditors.LabelControl()
        Me.progressBarControl = New DevExpress.XtraEditors.MarqueeProgressBarControl()
        Me.peLogo = New DevExpress.XtraEditors.PictureEdit()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        CType(Me.peImage.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.progressBarControl.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.peLogo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'peImage
        '
        Me.peImage.Cursor = System.Windows.Forms.Cursors.Default
        Me.peImage.Dock = System.Windows.Forms.DockStyle.Top
        Me.peImage.EditValue = CType(resources.GetObject("peImage.EditValue"), Object)
        Me.peImage.Location = New System.Drawing.Point(1, 1)
        Me.peImage.Name = "peImage"
        Me.peImage.Properties.AllowFocused = False
        Me.peImage.Properties.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.peImage.Properties.Appearance.Options.UseBackColor = True
        Me.peImage.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.peImage.Properties.ShowMenu = False
        Me.peImage.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch
        Me.peImage.Properties.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None
        Me.peImage.Size = New System.Drawing.Size(448, 200)
        Me.peImage.TabIndex = 14
        '
        'labelStatus
        '
        Me.labelStatus.Location = New System.Drawing.Point(24, 215)
        Me.labelStatus.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.labelStatus.Name = "labelStatus"
        Me.labelStatus.Size = New System.Drawing.Size(50, 13)
        Me.labelStatus.TabIndex = 12
        Me.labelStatus.Text = "Starting..."
        '
        'labelCopyright
        '
        Me.labelCopyright.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.labelCopyright.Location = New System.Drawing.Point(24, 287)
        Me.labelCopyright.Name = "labelCopyright"
        Me.labelCopyright.Size = New System.Drawing.Size(47, 13)
        Me.labelCopyright.TabIndex = 11
        Me.labelCopyright.Text = "Copyright"
        '
        'progressBarControl
        '
        Me.progressBarControl.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.progressBarControl.EditValue = 0
        Me.progressBarControl.Location = New System.Drawing.Point(24, 232)
        Me.progressBarControl.Name = "progressBarControl"
        Me.progressBarControl.Size = New System.Drawing.Size(402, 12)
        Me.progressBarControl.TabIndex = 10
        '
        'peLogo
        '
        Me.peLogo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.peLogo.EditValue = CType(resources.GetObject("peLogo.EditValue"), Object)
        Me.peLogo.Location = New System.Drawing.Point(226, 254)
        Me.peLogo.Name = "peLogo"
        Me.peLogo.Properties.AllowFocused = False
        Me.peLogo.Properties.Appearance.BackColor = System.Drawing.Color.Transparent
        Me.peLogo.Properties.Appearance.Options.UseBackColor = True
        Me.peLogo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.peLogo.Properties.ShowMenu = False
        Me.peLogo.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze
        Me.peLogo.Size = New System.Drawing.Size(214, 59)
        Me.peLogo.TabIndex = 15
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 1
        '
        'SplashScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(450, 320)
        Me.Controls.Add(Me.peLogo)
        Me.Controls.Add(Me.peImage)
        Me.Controls.Add(Me.labelStatus)
        Me.Controls.Add(Me.labelCopyright)
        Me.Controls.Add(Me.progressBarControl)
        Me.Name = "SplashScreen"
        Me.Padding = New System.Windows.Forms.Padding(1)
        Me.Text = "SplashScreen"
        CType(Me.peImage.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.progressBarControl.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.peLogo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents peImage As DevExpress.XtraEditors.PictureEdit
    Private WithEvents labelStatus As DevExpress.XtraEditors.LabelControl
    Private WithEvents labelCopyright As DevExpress.XtraEditors.LabelControl
    Private WithEvents progressBarControl As DevExpress.XtraEditors.MarqueeProgressBarControl
    Private WithEvents peLogo As DevExpress.XtraEditors.PictureEdit
    Friend WithEvents Timer1 As Timer
End Class
