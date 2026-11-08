<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmRespaldar
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
        Me.panelDatos = New DevExpress.XtraEditors.PanelControl()
        Me.TxtRuta = New DevExpress.XtraEditors.ButtonEdit()
        Me.LblRuta = New DevExpress.XtraEditors.LabelControl()
        Me.TxtBaseDatos = New DevExpress.XtraEditors.TextEdit()
        Me.LblBaseDatos = New DevExpress.XtraEditors.LabelControl()
        Me.panelBotones = New DevExpress.XtraEditors.PanelControl()
        Me.BtnRespaldar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        CType(Me.panelDatos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelDatos.SuspendLayout()
        CType(Me.TxtRuta.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtBaseDatos.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        Me.SuspendLayout()
        '
        'panelDatos
        '
        Me.panelDatos.Controls.Add(Me.TxtRuta)
        Me.panelDatos.Controls.Add(Me.LblRuta)
        Me.panelDatos.Controls.Add(Me.TxtBaseDatos)
        Me.panelDatos.Controls.Add(Me.LblBaseDatos)
        Me.panelDatos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.panelDatos.Location = New System.Drawing.Point(0, 0)
        Me.panelDatos.Name = "panelDatos"
        Me.panelDatos.Size = New System.Drawing.Size(484, 112)
        Me.panelDatos.TabIndex = 0
        '
        'TxtRuta
        '
        Me.TxtRuta.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtRuta.Location = New System.Drawing.Point(15, 78)
        Me.TxtRuta.Name = "TxtRuta"
        Me.TxtRuta.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)})
        Me.TxtRuta.Size = New System.Drawing.Size(454, 20)
        Me.TxtRuta.TabIndex = 3
        '
        'LblRuta
        '
        Me.LblRuta.Location = New System.Drawing.Point(15, 59)
        Me.LblRuta.Name = "LblRuta"
        Me.LblRuta.Size = New System.Drawing.Size(86, 13)
        Me.LblRuta.TabIndex = 2
        Me.LblRuta.Text = "Ruta del respaldo:"
        '
        'TxtBaseDatos
        '
        Me.TxtBaseDatos.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtBaseDatos.Location = New System.Drawing.Point(15, 32)
        Me.TxtBaseDatos.Name = "TxtBaseDatos"
        Me.TxtBaseDatos.Properties.ReadOnly = True
        Me.TxtBaseDatos.Size = New System.Drawing.Size(454, 20)
        Me.TxtBaseDatos.TabIndex = 1
        Me.TxtBaseDatos.TabStop = False
        '
        'LblBaseDatos
        '
        Me.LblBaseDatos.Location = New System.Drawing.Point(15, 13)
        Me.LblBaseDatos.Name = "LblBaseDatos"
        Me.LblBaseDatos.Size = New System.Drawing.Size(121, 13)
        Me.LblBaseDatos.TabIndex = 0
        Me.LblBaseDatos.Text = "Base de datos a respaldar:"
        '
        'panelBotones
        '
        Me.panelBotones.Controls.Add(Me.BtnRespaldar)
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Location = New System.Drawing.Point(0, 112)
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(484, 55)
        Me.panelBotones.TabIndex = 1
        '
        'BtnRespaldar
        '
        Me.BtnRespaldar.Location = New System.Drawing.Point(13, 10)
        Me.BtnRespaldar.Name = "BtnRespaldar"
        Me.BtnRespaldar.Size = New System.Drawing.Size(110, 35)
        Me.BtnRespaldar.TabIndex = 0
        Me.BtnRespaldar.Text = "Respaldar"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(129, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 1
        Me.BtnSalir.Text = "Salir"
        '
        'FrmRespaldar
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(484, 167)
        Me.Controls.Add(Me.panelDatos)
        Me.Controls.Add(Me.panelBotones)
        Me.MaximizeBox = False
        Me.Name = "FrmRespaldar"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Respaldar Base de Datos"
        CType(Me.panelDatos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelDatos.ResumeLayout(False)
        Me.panelDatos.PerformLayout()
        CType(Me.TxtRuta.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtBaseDatos.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents panelDatos As DevExpress.XtraEditors.PanelControl
    Friend WithEvents TxtRuta As DevExpress.XtraEditors.ButtonEdit
    Friend WithEvents LblRuta As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtBaseDatos As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LblBaseDatos As DevExpress.XtraEditors.LabelControl
    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnRespaldar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
End Class
