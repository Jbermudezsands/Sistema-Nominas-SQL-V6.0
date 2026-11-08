<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmRegistroJustifica
    Inherits DevExpress.XtraEditors.XtraForm

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.PanelBotones = New DevExpress.XtraEditors.PanelControl()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSiguiente = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnAnterior = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnEliminar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnGuardar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnNuevo = New DevExpress.XtraEditors.SimpleButton()
        Me.btnCancelar = New DevExpress.XtraEditors.SimpleButton()
        Me.groupControl1 = New DevExpress.XtraEditors.GroupControl()
        Me.TxtNombre = New System.Windows.Forms.TextBox()
        Me.TxtCodigo = New System.Windows.Forms.TextBox()
        Me.cmbCodigo = New DevExpress.XtraEditors.LookUpEdit()
        Me.labelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl1 = New DevExpress.XtraEditors.LabelControl()
        CType(Me.PanelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelBotones.SuspendLayout()
        CType(Me.groupControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupControl1.SuspendLayout()
        CType(Me.cmbCodigo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelBotones
        '
        Me.PanelBotones.Controls.Add(Me.btnCancelar)
        Me.PanelBotones.Controls.Add(Me.BtnSalir)
        Me.PanelBotones.Controls.Add(Me.BtnSiguiente)
        Me.PanelBotones.Controls.Add(Me.BtnAnterior)
        Me.PanelBotones.Controls.Add(Me.BtnEliminar)
        Me.PanelBotones.Controls.Add(Me.BtnGuardar)
        Me.PanelBotones.Controls.Add(Me.BtnNuevo)
        Me.PanelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBotones.Name = "PanelBotones"
        Me.PanelBotones.Size = New System.Drawing.Size(660, 55)
        Me.PanelBotones.TabIndex = 1
        '
        'BtnNuevo
        '
        Me.BtnNuevo.Location = New System.Drawing.Point(13, 10)
        Me.BtnNuevo.Name = "BtnNuevo"
        Me.BtnNuevo.Size = New System.Drawing.Size(95, 35)
        Me.BtnNuevo.TabIndex = 3
        Me.BtnNuevo.Text = "Nuevo"
        '
        'btnCancelar
        '
        Me.btnCancelar.Location = New System.Drawing.Point(13, 10)
        Me.btnCancelar.Name = "btnCancelar"
        Me.btnCancelar.Size = New System.Drawing.Size(95, 35)
        Me.btnCancelar.TabIndex = 9
        Me.btnCancelar.Text = "Cancelar"
        Me.btnCancelar.Visible = False
        '
        'BtnGuardar
        '
        Me.BtnGuardar.Location = New System.Drawing.Point(119, 10)
        Me.BtnGuardar.Name = "BtnGuardar"
        Me.BtnGuardar.Size = New System.Drawing.Size(95, 35)
        Me.BtnGuardar.TabIndex = 4
        Me.BtnGuardar.Text = "Grabar"
        '
        'BtnEliminar
        '
        Me.BtnEliminar.Location = New System.Drawing.Point(225, 10)
        Me.BtnEliminar.Name = "BtnEliminar"
        Me.BtnEliminar.Size = New System.Drawing.Size(95, 35)
        Me.BtnEliminar.TabIndex = 5
        Me.BtnEliminar.Text = "Eliminar"
        '
        'BtnAnterior
        '
        Me.BtnAnterior.Location = New System.Drawing.Point(340, 10)
        Me.BtnAnterior.Name = "BtnAnterior"
        Me.BtnAnterior.Size = New System.Drawing.Size(95, 35)
        Me.BtnAnterior.TabIndex = 6
        Me.BtnAnterior.Text = "Anterior"
        '
        'BtnSiguiente
        '
        Me.BtnSiguiente.Location = New System.Drawing.Point(441, 10)
        Me.BtnSiguiente.Name = "BtnSiguiente"
        Me.BtnSiguiente.Size = New System.Drawing.Size(95, 35)
        Me.BtnSiguiente.TabIndex = 7
        Me.BtnSiguiente.Text = "Siguiente"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(552, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 8
        Me.BtnSalir.Text = "Salir"
        '
        'groupControl1
        '
        Me.groupControl1.Controls.Add(Me.TxtNombre)
        Me.groupControl1.Controls.Add(Me.TxtCodigo)
        Me.groupControl1.Controls.Add(Me.cmbCodigo)
        Me.groupControl1.Controls.Add(Me.labelControl2)
        Me.groupControl1.Controls.Add(Me.labelControl1)
        Me.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.groupControl1.Name = "groupControl1"
        Me.groupControl1.TabIndex = 0
        Me.groupControl1.Text = "Tipo de justificación"
        '
        'TxtNombre
        '
        Me.TxtNombre.Location = New System.Drawing.Point(110, 65)
        Me.TxtNombre.MaxLength = 50
        Me.TxtNombre.Name = "TxtNombre"
        Me.TxtNombre.Size = New System.Drawing.Size(500, 21)
        Me.TxtNombre.TabIndex = 1
        '
        'TxtCodigo
        '
        Me.TxtCodigo.Location = New System.Drawing.Point(110, 35)
        Me.TxtCodigo.Name = "TxtCodigo"
        Me.TxtCodigo.ReadOnly = True
        Me.TxtCodigo.Size = New System.Drawing.Size(140, 21)
        Me.TxtCodigo.TabIndex = 10
        Me.TxtCodigo.Text = "(nuevo)"
        Me.TxtCodigo.Visible = False
        '
        'cmbCodigo
        '
        Me.cmbCodigo.Location = New System.Drawing.Point(110, 35)
        Me.cmbCodigo.Name = "cmbCodigo"
        Me.cmbCodigo.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cmbCodigo.Properties.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Classid", "Código", 40), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Classname", "Descripción")})
        Me.cmbCodigo.Properties.DisplayMember = "Classname"
        Me.cmbCodigo.Properties.NullText = ""
        Me.cmbCodigo.Properties.ValueMember = "Classid"
        Me.cmbCodigo.Size = New System.Drawing.Size(300, 20)
        Me.cmbCodigo.TabIndex = 0
        '
        'labelControl2
        '
        Me.labelControl2.Location = New System.Drawing.Point(15, 68)
        Me.labelControl2.Name = "labelControl2"
        Me.labelControl2.TabIndex = 5
        Me.labelControl2.Text = "Descripción:"
        '
        'labelControl1
        '
        Me.labelControl1.Location = New System.Drawing.Point(15, 38)
        Me.labelControl1.Name = "labelControl1"
        Me.labelControl1.TabIndex = 4
        Me.labelControl1.Text = "Tipo:"
        '
        'FrmRegistroJustifica
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(660, 190)
        Me.Controls.Add(Me.groupControl1)
        Me.Controls.Add(Me.PanelBotones)
        Me.Name = "FrmRegistroJustifica"
        Me.Text = "Tipos de Justificación"
        CType(Me.PanelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelBotones.ResumeLayout(False)
        CType(Me.groupControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupControl1.ResumeLayout(False)
        Me.groupControl1.PerformLayout()
        CType(Me.cmbCodigo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnNuevo As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnGuardar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnEliminar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnAnterior As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSiguiente As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents btnCancelar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents groupControl1 As DevExpress.XtraEditors.GroupControl
    Friend WithEvents TxtNombre As System.Windows.Forms.TextBox
    Friend WithEvents TxtCodigo As System.Windows.Forms.TextBox
    Friend WithEvents cmbCodigo As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents labelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl1 As DevExpress.XtraEditors.LabelControl
End Class
