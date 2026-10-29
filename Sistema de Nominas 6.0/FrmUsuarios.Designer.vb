<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmUsuarios
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
        Me.BtnNuevo = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnGuardar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnEliminar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnAnterior = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSiguiente = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.btnCancelar = New DevExpress.XtraEditors.SimpleButton()
        Me.groupControl1 = New DevExpress.XtraEditors.GroupControl()
        Me.TxtClave2 = New System.Windows.Forms.TextBox()
        Me.TxtClave = New System.Windows.Forms.TextBox()
        Me.TxtNivel = New DevExpress.XtraEditors.SpinEdit()
        Me.TxtNombre = New System.Windows.Forms.TextBox()
        Me.cmbUsuario = New DevExpress.XtraEditors.SearchLookUpEdit()
        Me.cmbUsuarioView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.labelControl5 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl4 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl1 = New DevExpress.XtraEditors.LabelControl()
        CType(Me.groupControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupControl1.SuspendLayout()
        CType(Me.TxtNivel.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbUsuario.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbUsuarioView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnNuevo
        '
        Me.BtnNuevo.Location = New System.Drawing.Point(13, 218)
        Me.BtnNuevo.Name = "BtnNuevo"
        Me.BtnNuevo.Size = New System.Drawing.Size(95, 35)
        Me.BtnNuevo.TabIndex = 3
        Me.BtnNuevo.Text = "Nuevo"
        '
        'BtnGuardar
        '
        Me.BtnGuardar.Location = New System.Drawing.Point(119, 218)
        Me.BtnGuardar.Name = "BtnGuardar"
        Me.BtnGuardar.Size = New System.Drawing.Size(95, 35)
        Me.BtnGuardar.TabIndex = 4
        Me.BtnGuardar.Text = "Grabar"
        '
        'BtnEliminar
        '
        Me.BtnEliminar.Location = New System.Drawing.Point(225, 218)
        Me.BtnEliminar.Name = "BtnEliminar"
        Me.BtnEliminar.Size = New System.Drawing.Size(95, 35)
        Me.BtnEliminar.TabIndex = 5
        Me.BtnEliminar.Text = "Eliminar"
        '
        'BtnAnterior
        '
        Me.BtnAnterior.Location = New System.Drawing.Point(340, 218)
        Me.BtnAnterior.Name = "BtnAnterior"
        Me.BtnAnterior.Size = New System.Drawing.Size(95, 35)
        Me.BtnAnterior.TabIndex = 6
        Me.BtnAnterior.Text = "Anterior"
        '
        'BtnSiguiente
        '
        Me.BtnSiguiente.Location = New System.Drawing.Point(441, 218)
        Me.BtnSiguiente.Name = "BtnSiguiente"
        Me.BtnSiguiente.Size = New System.Drawing.Size(95, 35)
        Me.BtnSiguiente.TabIndex = 7
        Me.BtnSiguiente.Text = "Siguiente"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(552, 218)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 8
        Me.BtnSalir.Text = "Salir"
        '
        'btnCancelar
        '
        Me.btnCancelar.Location = New System.Drawing.Point(13, 218)
        Me.btnCancelar.Name = "btnCancelar"
        Me.btnCancelar.Size = New System.Drawing.Size(95, 35)
        Me.btnCancelar.TabIndex = 9
        Me.btnCancelar.Text = "Cancelar"
        Me.btnCancelar.Visible = False
        '
        'groupControl1
        '
        Me.groupControl1.Controls.Add(Me.TxtClave2)
        Me.groupControl1.Controls.Add(Me.TxtClave)
        Me.groupControl1.Controls.Add(Me.TxtNivel)
        Me.groupControl1.Controls.Add(Me.TxtNombre)
        Me.groupControl1.Controls.Add(Me.cmbUsuario)
        Me.groupControl1.Controls.Add(Me.labelControl5)
        Me.groupControl1.Controls.Add(Me.labelControl4)
        Me.groupControl1.Controls.Add(Me.labelControl3)
        Me.groupControl1.Controls.Add(Me.labelControl2)
        Me.groupControl1.Controls.Add(Me.labelControl1)
        Me.groupControl1.Location = New System.Drawing.Point(12, 12)
        Me.groupControl1.Name = "groupControl1"
        Me.groupControl1.Size = New System.Drawing.Size(635, 190)
        Me.groupControl1.TabIndex = 0
        Me.groupControl1.Text = "Datos del usuario"
        '
        'TxtClave2
        '
        Me.TxtClave2.Location = New System.Drawing.Point(140, 155)
        Me.TxtClave2.Name = "TxtClave2"
        Me.TxtClave2.Size = New System.Drawing.Size(250, 21)
        Me.TxtClave2.TabIndex = 4
        Me.TxtClave2.UseSystemPasswordChar = True
        '
        'TxtClave
        '
        Me.TxtClave.Location = New System.Drawing.Point(140, 125)
        Me.TxtClave.Name = "TxtClave"
        Me.TxtClave.Size = New System.Drawing.Size(250, 21)
        Me.TxtClave.TabIndex = 3
        Me.TxtClave.UseSystemPasswordChar = True
        '
        'TxtNivel
        '
        Me.TxtNivel.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.TxtNivel.Location = New System.Drawing.Point(140, 95)
        Me.TxtNivel.Name = "TxtNivel"
        Me.TxtNivel.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinUp)})
        Me.TxtNivel.Properties.IsFloatValue = False
        Me.TxtNivel.Properties.Mask.EditMask = "N00"
        Me.TxtNivel.Properties.MaxValue = New Decimal(New Integer() {99, 0, 0, 0})
        Me.TxtNivel.Size = New System.Drawing.Size(90, 20)
        Me.TxtNivel.TabIndex = 2
        '
        'TxtNombre
        '
        Me.TxtNombre.Location = New System.Drawing.Point(140, 65)
        Me.TxtNombre.Name = "TxtNombre"
        Me.TxtNombre.Size = New System.Drawing.Size(250, 21)
        Me.TxtNombre.TabIndex = 1
        '
        'cmbUsuario
        '
        Me.cmbUsuario.Location = New System.Drawing.Point(140, 35)
        Me.cmbUsuario.Name = "cmbUsuario"
        Me.cmbUsuario.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cmbUsuario.Properties.NullText = ""
        Me.cmbUsuario.Properties.PopupSizeable = False
        Me.cmbUsuario.Properties.PopupView = Me.cmbUsuarioView
        Me.cmbUsuario.Size = New System.Drawing.Size(250, 20)
        Me.cmbUsuario.TabIndex = 0
        '
        'cmbUsuarioView
        '
        Me.cmbUsuarioView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.cmbUsuarioView.Name = "cmbUsuarioView"
        Me.cmbUsuarioView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.cmbUsuarioView.OptionsView.ShowGroupPanel = False
        '
        'labelControl5
        '
        Me.labelControl5.Location = New System.Drawing.Point(15, 158)
        Me.labelControl5.Name = "labelControl5"
        Me.labelControl5.Size = New System.Drawing.Size(80, 13)
        Me.labelControl5.TabIndex = 8
        Me.labelControl5.Text = "Repita la Clave:"
        '
        'labelControl4
        '
        Me.labelControl4.Location = New System.Drawing.Point(15, 128)
        Me.labelControl4.Name = "labelControl4"
        Me.labelControl4.Size = New System.Drawing.Size(86, 13)
        Me.labelControl4.TabIndex = 7
        Me.labelControl4.Text = "Clave de Acceso:"
        '
        'labelControl3
        '
        Me.labelControl3.Location = New System.Drawing.Point(15, 98)
        Me.labelControl3.Name = "labelControl3"
        Me.labelControl3.Size = New System.Drawing.Size(88, 13)
        Me.labelControl3.TabIndex = 6
        Me.labelControl3.Text = "Nivel del Usuario:"
        '
        'labelControl2
        '
        Me.labelControl2.Location = New System.Drawing.Point(15, 68)
        Me.labelControl2.Name = "labelControl2"
        Me.labelControl2.Size = New System.Drawing.Size(107, 13)
        Me.labelControl2.TabIndex = 5
        Me.labelControl2.Text = "Nombre del Usuario:"
        '
        'labelControl1
        '
        Me.labelControl1.Location = New System.Drawing.Point(15, 38)
        Me.labelControl1.Name = "labelControl1"
        Me.labelControl1.Size = New System.Drawing.Size(42, 13)
        Me.labelControl1.TabIndex = 4
        Me.labelControl1.Text = "Usuario:"
        '
        'FrmUsuarios
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(660, 265)
        Me.Controls.Add(Me.btnCancelar)
        Me.Controls.Add(Me.BtnNuevo)
        Me.Controls.Add(Me.BtnGuardar)
        Me.Controls.Add(Me.BtnEliminar)
        Me.Controls.Add(Me.BtnAnterior)
        Me.Controls.Add(Me.BtnSiguiente)
        Me.Controls.Add(Me.BtnSalir)
        Me.Controls.Add(Me.groupControl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmUsuarios"
        Me.Text = "Usuarios"
        CType(Me.groupControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupControl1.ResumeLayout(False)
        Me.groupControl1.PerformLayout()
        CType(Me.TxtNivel.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmbUsuario.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmbUsuarioView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents BtnNuevo As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnGuardar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnEliminar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnAnterior As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSiguiente As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents btnCancelar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents groupControl1 As DevExpress.XtraEditors.GroupControl
    Friend WithEvents TxtClave2 As System.Windows.Forms.TextBox
    Friend WithEvents TxtClave As System.Windows.Forms.TextBox
    Friend WithEvents TxtNivel As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents TxtNombre As System.Windows.Forms.TextBox
    Friend WithEvents cmbUsuario As DevExpress.XtraEditors.SearchLookUpEdit
    Friend WithEvents cmbUsuarioView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents labelControl5 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl4 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl1 As DevExpress.XtraEditors.LabelControl
End Class
