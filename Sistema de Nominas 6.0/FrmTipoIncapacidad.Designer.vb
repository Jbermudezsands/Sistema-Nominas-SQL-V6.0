<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmTipoIncapacidad
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
        Me.TxtIncapacidad = New System.Windows.Forms.TextBox()
        Me.TxtCodigo = New System.Windows.Forms.TextBox()
        Me.cmbCodigo = New DevExpress.XtraEditors.SearchLookUpEdit()
        Me.cmbCodigoView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.labelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl1 = New DevExpress.XtraEditors.LabelControl()
        CType(Me.groupControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupControl1.SuspendLayout()
        CType(Me.cmbCodigo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbCodigoView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnNuevo
        '
        Me.BtnNuevo.Location = New System.Drawing.Point(13, 128)
        Me.BtnNuevo.Name = "BtnNuevo"
        Me.BtnNuevo.Size = New System.Drawing.Size(95, 35)
        Me.BtnNuevo.TabIndex = 2
        Me.BtnNuevo.Text = "Nuevo"
        '
        'BtnGuardar
        '
        Me.BtnGuardar.Location = New System.Drawing.Point(119, 128)
        Me.BtnGuardar.Name = "BtnGuardar"
        Me.BtnGuardar.Size = New System.Drawing.Size(95, 35)
        Me.BtnGuardar.TabIndex = 3
        Me.BtnGuardar.Text = "Grabar"
        '
        'BtnEliminar
        '
        Me.BtnEliminar.Location = New System.Drawing.Point(225, 128)
        Me.BtnEliminar.Name = "BtnEliminar"
        Me.BtnEliminar.Size = New System.Drawing.Size(95, 35)
        Me.BtnEliminar.TabIndex = 4
        Me.BtnEliminar.Text = "Eliminar"
        '
        'BtnAnterior
        '
        Me.BtnAnterior.Location = New System.Drawing.Point(340, 128)
        Me.BtnAnterior.Name = "BtnAnterior"
        Me.BtnAnterior.Size = New System.Drawing.Size(95, 35)
        Me.BtnAnterior.TabIndex = 5
        Me.BtnAnterior.Text = "Anterior"
        '
        'BtnSiguiente
        '
        Me.BtnSiguiente.Location = New System.Drawing.Point(441, 128)
        Me.BtnSiguiente.Name = "BtnSiguiente"
        Me.BtnSiguiente.Size = New System.Drawing.Size(95, 35)
        Me.BtnSiguiente.TabIndex = 6
        Me.BtnSiguiente.Text = "Siguiente"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(552, 128)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 7
        Me.BtnSalir.Text = "Salir"
        '
        'btnCancelar
        '
        Me.btnCancelar.Location = New System.Drawing.Point(13, 128)
        Me.btnCancelar.Name = "btnCancelar"
        Me.btnCancelar.Size = New System.Drawing.Size(95, 35)
        Me.btnCancelar.TabIndex = 8
        Me.btnCancelar.Text = "Cancelar"
        Me.btnCancelar.Visible = False
        '
        'groupControl1
        '
        Me.groupControl1.Controls.Add(Me.TxtIncapacidad)
        Me.groupControl1.Controls.Add(Me.TxtCodigo)
        Me.groupControl1.Controls.Add(Me.cmbCodigo)
        Me.groupControl1.Controls.Add(Me.labelControl2)
        Me.groupControl1.Controls.Add(Me.labelControl1)
        Me.groupControl1.Location = New System.Drawing.Point(12, 12)
        Me.groupControl1.Name = "groupControl1"
        Me.groupControl1.Size = New System.Drawing.Size(635, 100)
        Me.groupControl1.TabIndex = 0
        Me.groupControl1.Text = "Datos del tipo de incapacidad"
        '
        'TxtIncapacidad
        '
        Me.TxtIncapacidad.Location = New System.Drawing.Point(125, 65)
        Me.TxtIncapacidad.Name = "TxtIncapacidad"
        Me.TxtIncapacidad.Size = New System.Drawing.Size(485, 21)
        Me.TxtIncapacidad.TabIndex = 1
        '
        'TxtCodigo
        '
        Me.TxtCodigo.Location = New System.Drawing.Point(125, 35)
        Me.TxtCodigo.Name = "TxtCodigo"
        Me.TxtCodigo.Size = New System.Drawing.Size(140, 21)
        Me.TxtCodigo.TabIndex = 9
        Me.TxtCodigo.Visible = False
        '
        'cmbCodigo
        '
        Me.cmbCodigo.Location = New System.Drawing.Point(125, 35)
        Me.cmbCodigo.Name = "cmbCodigo"
        Me.cmbCodigo.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cmbCodigo.Properties.NullText = ""
        Me.cmbCodigo.Properties.PopupSizeable = False
        Me.cmbCodigo.Properties.PopupView = Me.cmbCodigoView
        Me.cmbCodigo.Size = New System.Drawing.Size(140, 20)
        Me.cmbCodigo.TabIndex = 0
        '
        'cmbCodigoView
        '
        Me.cmbCodigoView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.cmbCodigoView.Name = "cmbCodigoView"
        Me.cmbCodigoView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.cmbCodigoView.OptionsView.ShowGroupPanel = False
        '
        'labelControl2
        '
        Me.labelControl2.Location = New System.Drawing.Point(15, 68)
        Me.labelControl2.Name = "labelControl2"
        Me.labelControl2.Size = New System.Drawing.Size(60, 13)
        Me.labelControl2.TabIndex = 5
        Me.labelControl2.Text = "Incapacidad:"
        '
        'labelControl1
        '
        Me.labelControl1.Location = New System.Drawing.Point(15, 38)
        Me.labelControl1.Name = "labelControl1"
        Me.labelControl1.Size = New System.Drawing.Size(104, 13)
        Me.labelControl1.TabIndex = 4
        Me.labelControl1.Text = "Código Incapacidad:"
        '
        'FrmTipoIncapacidad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(660, 175)
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
        Me.Name = "FrmTipoIncapacidad"
        Me.Text = "Tipos de Incapacidad"
        CType(Me.groupControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupControl1.ResumeLayout(False)
        Me.groupControl1.PerformLayout()
        CType(Me.cmbCodigo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmbCodigoView, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents TxtIncapacidad As System.Windows.Forms.TextBox
    Friend WithEvents TxtCodigo As System.Windows.Forms.TextBox
    Friend WithEvents cmbCodigo As DevExpress.XtraEditors.SearchLookUpEdit
    Friend WithEvents cmbCodigoView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents labelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl1 As DevExpress.XtraEditors.LabelControl
End Class
