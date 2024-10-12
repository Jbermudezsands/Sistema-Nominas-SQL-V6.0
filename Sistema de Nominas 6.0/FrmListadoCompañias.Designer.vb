<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmListadoCompañias
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmListadoCompañias))
        Me.BtnCancelar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSeleccionar = New DevExpress.XtraEditors.SimpleButton()
        Me.listBoxCompañia = New DevExpress.XtraEditors.ListBoxControl()
        CType(Me.listBoxCompañia, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnCancelar
        '
        Me.BtnCancelar.ImageOptions.Image = CType(resources.GetObject("BtnCancelar.ImageOptions.Image"), System.Drawing.Image)
        Me.BtnCancelar.Location = New System.Drawing.Point(247, 304)
        Me.BtnCancelar.Name = "BtnCancelar"
        Me.BtnCancelar.Size = New System.Drawing.Size(97, 39)
        Me.BtnCancelar.TabIndex = 5
        Me.BtnCancelar.Text = "Cancelar"
        '
        'BtnSeleccionar
        '
        Me.BtnSeleccionar.ImageOptions.Image = CType(resources.GetObject("BtnSeleccionar.ImageOptions.Image"), System.Drawing.Image)
        Me.BtnSeleccionar.Location = New System.Drawing.Point(13, 304)
        Me.BtnSeleccionar.Name = "BtnSeleccionar"
        Me.BtnSeleccionar.Size = New System.Drawing.Size(96, 39)
        Me.BtnSeleccionar.TabIndex = 4
        Me.BtnSeleccionar.Text = "Seleccionar"
        '
        'listBoxCompañia
        '
        Me.listBoxCompañia.Location = New System.Drawing.Point(12, 12)
        Me.listBoxCompañia.Name = "listBoxCompañia"
        Me.listBoxCompañia.Size = New System.Drawing.Size(332, 285)
        Me.listBoxCompañia.TabIndex = 3
        '
        'FrmListadoCompañias
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(355, 361)
        Me.Controls.Add(Me.BtnCancelar)
        Me.Controls.Add(Me.BtnSeleccionar)
        Me.Controls.Add(Me.listBoxCompañia)
        Me.IconOptions.LargeImage = CType(resources.GetObject("FrmListadoCompañias.IconOptions.LargeImage"), System.Drawing.Image)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmListadoCompañias"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Listado de Compañias"
        CType(Me.listBoxCompañia, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents BtnCancelar As DevExpress.XtraEditors.SimpleButton
    Private WithEvents BtnSeleccionar As DevExpress.XtraEditors.SimpleButton
    Private WithEvents listBoxCompañia As DevExpress.XtraEditors.ListBoxControl
End Class
