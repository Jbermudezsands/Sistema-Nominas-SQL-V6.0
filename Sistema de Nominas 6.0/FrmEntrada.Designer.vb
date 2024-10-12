<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmEntrada
    Inherits DevExpress.XtraEditors.XtraForm

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmEntrada))
        Me.TxtContraseña = New System.Windows.Forms.TextBox()
        Me.BtnCancelar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnAceptar = New DevExpress.XtraEditors.SimpleButton()
        Me.labelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.TxtUsuario = New DevExpress.XtraEditors.TextEdit()
        Me.labelControl1 = New DevExpress.XtraEditors.LabelControl()
        CType(Me.TxtUsuario.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TxtContraseña
        '
        Me.TxtContraseña.Location = New System.Drawing.Point(93, 39)
        Me.TxtContraseña.Name = "TxtContraseña"
        Me.TxtContraseña.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.TxtContraseña.Size = New System.Drawing.Size(161, 21)
        Me.TxtContraseña.TabIndex = 7
        '
        'BtnCancelar
        '
        Me.BtnCancelar.ImageOptions.Image = CType(resources.GetObject("BtnCancelar.ImageOptions.Image"), System.Drawing.Image)
        Me.BtnCancelar.Location = New System.Drawing.Point(179, 79)
        Me.BtnCancelar.Name = "BtnCancelar"
        Me.BtnCancelar.Size = New System.Drawing.Size(100, 36)
        Me.BtnCancelar.TabIndex = 10
        Me.BtnCancelar.Text = "Cancelar"
        '
        'BtnAceptar
        '
        Me.BtnAceptar.ImageOptions.Image = CType(resources.GetObject("BtnAceptar.ImageOptions.Image"), System.Drawing.Image)
        Me.BtnAceptar.Location = New System.Drawing.Point(5, 79)
        Me.BtnAceptar.Name = "BtnAceptar"
        Me.BtnAceptar.Size = New System.Drawing.Size(100, 36)
        Me.BtnAceptar.TabIndex = 9
        Me.BtnAceptar.Text = "Aceptar"
        '
        'labelControl2
        '
        Me.labelControl2.Location = New System.Drawing.Point(19, 45)
        Me.labelControl2.Name = "labelControl2"
        Me.labelControl2.Size = New System.Drawing.Size(56, 13)
        Me.labelControl2.TabIndex = 8
        Me.labelControl2.Text = "Contraseña"
        '
        'TxtUsuario
        '
        Me.TxtUsuario.Location = New System.Drawing.Point(93, 12)
        Me.TxtUsuario.Name = "TxtUsuario"
        Me.TxtUsuario.Size = New System.Drawing.Size(161, 20)
        Me.TxtUsuario.TabIndex = 6
        '
        'labelControl1
        '
        Me.labelControl1.Location = New System.Drawing.Point(35, 15)
        Me.labelControl1.Name = "labelControl1"
        Me.labelControl1.Size = New System.Drawing.Size(40, 13)
        Me.labelControl1.TabIndex = 5
        Me.labelControl1.Text = "Usuario:"
        '
        'FrmEntrada
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(294, 132)
        Me.Controls.Add(Me.TxtContraseña)
        Me.Controls.Add(Me.BtnCancelar)
        Me.Controls.Add(Me.BtnAceptar)
        Me.Controls.Add(Me.labelControl2)
        Me.Controls.Add(Me.TxtUsuario)
        Me.Controls.Add(Me.labelControl1)
        Me.IconOptions.LargeImage = CType(resources.GetObject("FrmEntrada.IconOptions.LargeImage"), System.Drawing.Image)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmEntrada"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Registro de Usuarios"
        CType(Me.TxtUsuario.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents TxtContraseña As TextBox
    Private WithEvents BtnCancelar As DevExpress.XtraEditors.SimpleButton
    Private WithEvents BtnAceptar As DevExpress.XtraEditors.SimpleButton
    Private WithEvents labelControl2 As DevExpress.XtraEditors.LabelControl
    Private WithEvents TxtUsuario As DevExpress.XtraEditors.TextEdit
    Private WithEvents labelControl1 As DevExpress.XtraEditors.LabelControl
End Class
