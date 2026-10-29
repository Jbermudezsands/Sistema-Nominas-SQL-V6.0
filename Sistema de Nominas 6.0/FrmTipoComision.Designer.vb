<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmTipoComision
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
        Me.TxtPorcentaje = New DevExpress.XtraEditors.TextEdit()
        Me.TxtCuenta = New System.Windows.Forms.TextBox()
        Me.TxtComision = New System.Windows.Forms.TextBox()
        Me.TxtCodigo = New System.Windows.Forms.TextBox()
        Me.cmbCodigo = New DevExpress.XtraEditors.SearchLookUpEdit()
        Me.cmbCodigoView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.labelControl4 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl1 = New DevExpress.XtraEditors.LabelControl()
        CType(Me.groupControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupControl1.SuspendLayout()
        CType(Me.TxtPorcentaje.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbCodigo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbCodigoView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnNuevo
        '
        Me.BtnNuevo.Location = New System.Drawing.Point(13, 188)
        Me.BtnNuevo.Name = "BtnNuevo"
        Me.BtnNuevo.Size = New System.Drawing.Size(95, 35)
        Me.BtnNuevo.TabIndex = 2
        Me.BtnNuevo.Text = "Nuevo"
        '
        'BtnGuardar
        '
        Me.BtnGuardar.Location = New System.Drawing.Point(119, 188)
        Me.BtnGuardar.Name = "BtnGuardar"
        Me.BtnGuardar.Size = New System.Drawing.Size(95, 35)
        Me.BtnGuardar.TabIndex = 3
        Me.BtnGuardar.Text = "Grabar"
        '
        'BtnEliminar
        '
        Me.BtnEliminar.Location = New System.Drawing.Point(225, 188)
        Me.BtnEliminar.Name = "BtnEliminar"
        Me.BtnEliminar.Size = New System.Drawing.Size(95, 35)
        Me.BtnEliminar.TabIndex = 4
        Me.BtnEliminar.Text = "Eliminar"
        '
        'BtnAnterior
        '
        Me.BtnAnterior.Location = New System.Drawing.Point(340, 188)
        Me.BtnAnterior.Name = "BtnAnterior"
        Me.BtnAnterior.Size = New System.Drawing.Size(95, 35)
        Me.BtnAnterior.TabIndex = 5
        Me.BtnAnterior.Text = "Anterior"
        '
        'BtnSiguiente
        '
        Me.BtnSiguiente.Location = New System.Drawing.Point(441, 188)
        Me.BtnSiguiente.Name = "BtnSiguiente"
        Me.BtnSiguiente.Size = New System.Drawing.Size(95, 35)
        Me.BtnSiguiente.TabIndex = 6
        Me.BtnSiguiente.Text = "Siguiente"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(552, 188)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 7
        Me.BtnSalir.Text = "Salir"
        '
        'btnCancelar
        '
        Me.btnCancelar.Location = New System.Drawing.Point(13, 188)
        Me.btnCancelar.Name = "btnCancelar"
        Me.btnCancelar.Size = New System.Drawing.Size(95, 35)
        Me.btnCancelar.TabIndex = 8
        Me.btnCancelar.Text = "Cancelar"
        Me.btnCancelar.Visible = False
        '
        'groupControl1
        '
        Me.groupControl1.Controls.Add(Me.TxtPorcentaje)
        Me.groupControl1.Controls.Add(Me.TxtCuenta)
        Me.groupControl1.Controls.Add(Me.TxtComision)
        Me.groupControl1.Controls.Add(Me.TxtCodigo)
        Me.groupControl1.Controls.Add(Me.cmbCodigo)
        Me.groupControl1.Controls.Add(Me.labelControl4)
        Me.groupControl1.Controls.Add(Me.labelControl3)
        Me.groupControl1.Controls.Add(Me.labelControl2)
        Me.groupControl1.Controls.Add(Me.labelControl1)
        Me.groupControl1.Location = New System.Drawing.Point(12, 12)
        Me.groupControl1.Name = "groupControl1"
        Me.groupControl1.Size = New System.Drawing.Size(635, 160)
        Me.groupControl1.TabIndex = 0
        Me.groupControl1.Text = "Datos del tipo de comisión"
        '
        'TxtComision
        '
        Me.TxtComision.Location = New System.Drawing.Point(125, 65)
        Me.TxtComision.Name = "TxtComision"
        Me.TxtComision.Size = New System.Drawing.Size(485, 21)
        Me.TxtComision.TabIndex = 1
        '
        'TxtPorcentaje
        '
        Me.TxtPorcentaje.EditValue = "0.00"
        Me.TxtPorcentaje.Location = New System.Drawing.Point(125, 95)
        Me.TxtPorcentaje.Name = "TxtPorcentaje"
        Me.TxtPorcentaje.Properties.Mask.EditMask = "n2"
        Me.TxtPorcentaje.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtPorcentaje.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.TxtPorcentaje.Size = New System.Drawing.Size(140, 20)
        Me.TxtPorcentaje.TabIndex = 2
        '
        'TxtCuenta
        '
        Me.TxtCuenta.Location = New System.Drawing.Point(125, 125)
        Me.TxtCuenta.Name = "TxtCuenta"
        Me.TxtCuenta.Size = New System.Drawing.Size(485, 21)
        Me.TxtCuenta.TabIndex = 3
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
        Me.labelControl2.Text = "Comisión:"
        '
        'labelControl1
        '
        Me.labelControl1.Location = New System.Drawing.Point(15, 38)
        Me.labelControl1.Name = "labelControl1"
        Me.labelControl1.Size = New System.Drawing.Size(104, 13)
        Me.labelControl1.TabIndex = 4
        Me.labelControl1.Text = "Código Comisión:"
        '
        'labelControl3
        '
        Me.labelControl3.Location = New System.Drawing.Point(15, 98)
        Me.labelControl3.Name = "labelControl3"
        Me.labelControl3.Size = New System.Drawing.Size(80, 13)
        Me.labelControl3.TabIndex = 6
        Me.labelControl3.Text = "Porcentaje (%):"
        '
        'labelControl4
        '
        Me.labelControl4.Location = New System.Drawing.Point(15, 128)
        Me.labelControl4.Name = "labelControl4"
        Me.labelControl4.Size = New System.Drawing.Size(90, 13)
        Me.labelControl4.TabIndex = 7
        Me.labelControl4.Text = "Cuenta Contable:"
        '
        'FrmTipoComision
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(660, 235)
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
        Me.Name = "FrmTipoComision"
        Me.Text = "Tipos de Comisión"
        CType(Me.groupControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupControl1.ResumeLayout(False)
        Me.groupControl1.PerformLayout()
        CType(Me.TxtPorcentaje.Properties, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents TxtComision As System.Windows.Forms.TextBox
    Friend WithEvents TxtCodigo As System.Windows.Forms.TextBox
    Friend WithEvents cmbCodigo As DevExpress.XtraEditors.SearchLookUpEdit
    Friend WithEvents cmbCodigoView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents labelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtPorcentaje As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TxtCuenta As System.Windows.Forms.TextBox
    Friend WithEvents labelControl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl4 As DevExpress.XtraEditors.LabelControl
End Class
