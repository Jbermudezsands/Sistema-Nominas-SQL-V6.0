<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmCargaMasiva
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
        Me.LblNomina = New DevExpress.XtraEditors.LabelControl()
        Me.CmbNomina = New DevExpress.XtraEditors.LookUpEdit()
        Me.LblInfo = New DevExpress.XtraEditors.LabelControl()
        Me.groupDeduccion = New DevExpress.XtraEditors.GroupControl()
        Me.BtnProcesarDeduccion = New DevExpress.XtraEditors.SimpleButton()
        Me.SpnDeduccion = New DevExpress.XtraEditors.SpinEdit()
        Me.LblMontoDeduccion = New DevExpress.XtraEditors.LabelControl()
        Me.CmbDeduccion = New DevExpress.XtraEditors.LookUpEdit()
        Me.LblTipoDeduccion = New DevExpress.XtraEditors.LabelControl()
        Me.groupIncentivo = New DevExpress.XtraEditors.GroupControl()
        Me.BtnProcesarIncentivo = New DevExpress.XtraEditors.SimpleButton()
        Me.SpnIncentivo = New DevExpress.XtraEditors.SpinEdit()
        Me.LblMontoIncentivo = New DevExpress.XtraEditors.LabelControl()
        Me.CmbIncentivo = New DevExpress.XtraEditors.LookUpEdit()
        Me.LblTipoIncentivo = New DevExpress.XtraEditors.LabelControl()
        Me.Progreso = New DevExpress.XtraEditors.ProgressBarControl()
        Me.LblProcesando = New DevExpress.XtraEditors.LabelControl()
        Me.panelBotones = New DevExpress.XtraEditors.PanelControl()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        CType(Me.CmbNomina.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.groupDeduccion, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupDeduccion.SuspendLayout()
        CType(Me.SpnDeduccion.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CmbDeduccion.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.groupIncentivo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupIncentivo.SuspendLayout()
        CType(Me.SpnIncentivo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CmbIncentivo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Progreso.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        Me.SuspendLayout()
        '
        'LblNomina
        '
        Me.LblNomina.Location = New System.Drawing.Point(13, 18)
        Me.LblNomina.Name = "LblNomina"
        Me.LblNomina.Size = New System.Drawing.Size(76, 13)
        Me.LblNomina.TabIndex = 0
        Me.LblNomina.Text = "Nómina activa:"
        '
        'CmbNomina
        '
        Me.CmbNomina.Location = New System.Drawing.Point(110, 15)
        Me.CmbNomina.Name = "CmbNomina"
        Me.CmbNomina.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.CmbNomina.Properties.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo("CodTipoNomina", "Código", 60), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Nomina", "Nómina", 200)})
        Me.CmbNomina.Properties.DisplayMember = "Nomina"
        Me.CmbNomina.Properties.NullText = ""
        Me.CmbNomina.Properties.ValueMember = "CodTipoNomina"
        Me.CmbNomina.Size = New System.Drawing.Size(280, 20)
        Me.CmbNomina.TabIndex = 1
        '
        'LblInfo
        '
        Me.LblInfo.Location = New System.Drawing.Point(110, 42)
        Me.LblInfo.Name = "LblInfo"
        Me.LblInfo.Size = New System.Drawing.Size(0, 13)
        Me.LblInfo.TabIndex = 2
        '
        'groupDeduccion
        '
        Me.groupDeduccion.Controls.Add(Me.BtnProcesarDeduccion)
        Me.groupDeduccion.Controls.Add(Me.SpnDeduccion)
        Me.groupDeduccion.Controls.Add(Me.LblMontoDeduccion)
        Me.groupDeduccion.Controls.Add(Me.CmbDeduccion)
        Me.groupDeduccion.Controls.Add(Me.LblTipoDeduccion)
        Me.groupDeduccion.Location = New System.Drawing.Point(13, 70)
        Me.groupDeduccion.Name = "groupDeduccion"
        Me.groupDeduccion.Size = New System.Drawing.Size(614, 100)
        Me.groupDeduccion.TabIndex = 3
        Me.groupDeduccion.Text = "Deducción para todos los empleados del tipo de nómina"
        '
        'BtnProcesarDeduccion
        '
        Me.BtnProcesarDeduccion.Location = New System.Drawing.Point(480, 40)
        Me.BtnProcesarDeduccion.Name = "BtnProcesarDeduccion"
        Me.BtnProcesarDeduccion.Size = New System.Drawing.Size(120, 35)
        Me.BtnProcesarDeduccion.TabIndex = 4
        Me.BtnProcesarDeduccion.Text = "Procesar deducción"
        '
        'SpnDeduccion
        '
        Me.SpnDeduccion.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.SpnDeduccion.Location = New System.Drawing.Point(97, 62)
        Me.SpnDeduccion.Name = "SpnDeduccion"
        Me.SpnDeduccion.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight)})
        Me.SpnDeduccion.Properties.DisplayFormat.FormatString = "n2"
        Me.SpnDeduccion.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.SpnDeduccion.Properties.EditFormat.FormatString = "n2"
        Me.SpnDeduccion.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.SpnDeduccion.Properties.MaxValue = New Decimal(New Integer() {100000000, 0, 0, 0})
        Me.SpnDeduccion.Size = New System.Drawing.Size(110, 20)
        Me.SpnDeduccion.TabIndex = 3
        '
        'LblMontoDeduccion
        '
        Me.LblMontoDeduccion.Location = New System.Drawing.Point(13, 65)
        Me.LblMontoDeduccion.Name = "LblMontoDeduccion"
        Me.LblMontoDeduccion.Size = New System.Drawing.Size(34, 13)
        Me.LblMontoDeduccion.TabIndex = 2
        Me.LblMontoDeduccion.Text = "Monto:"
        '
        'CmbDeduccion
        '
        Me.CmbDeduccion.Location = New System.Drawing.Point(97, 33)
        Me.CmbDeduccion.Name = "CmbDeduccion"
        Me.CmbDeduccion.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.CmbDeduccion.Properties.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo("CodTipoDeduccion", "Código", 60), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Deduccion", "Deducción", 220)})
        Me.CmbDeduccion.Properties.DisplayMember = "Deduccion"
        Me.CmbDeduccion.Properties.NullText = ""
        Me.CmbDeduccion.Properties.ValueMember = "CodTipoDeduccion"
        Me.CmbDeduccion.Size = New System.Drawing.Size(280, 20)
        Me.CmbDeduccion.TabIndex = 1
        '
        'LblTipoDeduccion
        '
        Me.LblTipoDeduccion.Location = New System.Drawing.Point(13, 36)
        Me.LblTipoDeduccion.Name = "LblTipoDeduccion"
        Me.LblTipoDeduccion.Size = New System.Drawing.Size(75, 13)
        Me.LblTipoDeduccion.TabIndex = 0
        Me.LblTipoDeduccion.Text = "Tipo deducción:"
        '
        'groupIncentivo
        '
        Me.groupIncentivo.Controls.Add(Me.BtnProcesarIncentivo)
        Me.groupIncentivo.Controls.Add(Me.SpnIncentivo)
        Me.groupIncentivo.Controls.Add(Me.LblMontoIncentivo)
        Me.groupIncentivo.Controls.Add(Me.CmbIncentivo)
        Me.groupIncentivo.Controls.Add(Me.LblTipoIncentivo)
        Me.groupIncentivo.Location = New System.Drawing.Point(13, 180)
        Me.groupIncentivo.Name = "groupIncentivo"
        Me.groupIncentivo.Size = New System.Drawing.Size(614, 100)
        Me.groupIncentivo.TabIndex = 4
        Me.groupIncentivo.Text = "Incentivo para todos los empleados del tipo de nómina"
        '
        'BtnProcesarIncentivo
        '
        Me.BtnProcesarIncentivo.Location = New System.Drawing.Point(480, 40)
        Me.BtnProcesarIncentivo.Name = "BtnProcesarIncentivo"
        Me.BtnProcesarIncentivo.Size = New System.Drawing.Size(120, 35)
        Me.BtnProcesarIncentivo.TabIndex = 4
        Me.BtnProcesarIncentivo.Text = "Procesar incentivo"
        '
        'SpnIncentivo
        '
        Me.SpnIncentivo.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.SpnIncentivo.Location = New System.Drawing.Point(97, 62)
        Me.SpnIncentivo.Name = "SpnIncentivo"
        Me.SpnIncentivo.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight)})
        Me.SpnIncentivo.Properties.DisplayFormat.FormatString = "n2"
        Me.SpnIncentivo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.SpnIncentivo.Properties.EditFormat.FormatString = "n2"
        Me.SpnIncentivo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.SpnIncentivo.Properties.MaxValue = New Decimal(New Integer() {100000000, 0, 0, 0})
        Me.SpnIncentivo.Size = New System.Drawing.Size(110, 20)
        Me.SpnIncentivo.TabIndex = 3
        '
        'LblMontoIncentivo
        '
        Me.LblMontoIncentivo.Location = New System.Drawing.Point(13, 65)
        Me.LblMontoIncentivo.Name = "LblMontoIncentivo"
        Me.LblMontoIncentivo.Size = New System.Drawing.Size(34, 13)
        Me.LblMontoIncentivo.TabIndex = 2
        Me.LblMontoIncentivo.Text = "Monto:"
        '
        'CmbIncentivo
        '
        Me.CmbIncentivo.Location = New System.Drawing.Point(97, 33)
        Me.CmbIncentivo.Name = "CmbIncentivo"
        Me.CmbIncentivo.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.CmbIncentivo.Properties.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo("CodTipoIncentivo", "Código", 60), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Incentivo", "Incentivo", 220)})
        Me.CmbIncentivo.Properties.DisplayMember = "Incentivo"
        Me.CmbIncentivo.Properties.NullText = ""
        Me.CmbIncentivo.Properties.ValueMember = "CodTipoIncentivo"
        Me.CmbIncentivo.Size = New System.Drawing.Size(280, 20)
        Me.CmbIncentivo.TabIndex = 1
        '
        'LblTipoIncentivo
        '
        Me.LblTipoIncentivo.Location = New System.Drawing.Point(13, 36)
        Me.LblTipoIncentivo.Name = "LblTipoIncentivo"
        Me.LblTipoIncentivo.Size = New System.Drawing.Size(70, 13)
        Me.LblTipoIncentivo.TabIndex = 0
        Me.LblTipoIncentivo.Text = "Tipo incentivo:"
        '
        'Progreso
        '
        Me.Progreso.Location = New System.Drawing.Point(13, 296)
        Me.Progreso.Name = "Progreso"
        Me.Progreso.Size = New System.Drawing.Size(614, 18)
        Me.Progreso.TabIndex = 5
        '
        'LblProcesando
        '
        Me.LblProcesando.Location = New System.Drawing.Point(13, 322)
        Me.LblProcesando.Name = "LblProcesando"
        Me.LblProcesando.Size = New System.Drawing.Size(0, 13)
        Me.LblProcesando.TabIndex = 6
        '
        'panelBotones
        '
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Location = New System.Drawing.Point(0, 355)
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(640, 55)
        Me.panelBotones.TabIndex = 7
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(13, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 0
        Me.BtnSalir.Text = "Salir"
        '
        'FrmCargaMasiva
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(640, 410)
        Me.Controls.Add(Me.panelBotones)
        Me.Controls.Add(Me.LblProcesando)
        Me.Controls.Add(Me.Progreso)
        Me.Controls.Add(Me.groupIncentivo)
        Me.Controls.Add(Me.groupDeduccion)
        Me.Controls.Add(Me.LblInfo)
        Me.Controls.Add(Me.CmbNomina)
        Me.Controls.Add(Me.LblNomina)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.Name = "FrmCargaMasiva"
        Me.Text = "Carga Masiva de Deducciones e Incentivos"
        CType(Me.CmbNomina.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.groupDeduccion, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupDeduccion.ResumeLayout(False)
        Me.groupDeduccion.PerformLayout()
        CType(Me.SpnDeduccion.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CmbDeduccion.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.groupIncentivo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupIncentivo.ResumeLayout(False)
        Me.groupIncentivo.PerformLayout()
        CType(Me.SpnIncentivo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CmbIncentivo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Progreso.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LblNomina As DevExpress.XtraEditors.LabelControl
    Friend WithEvents CmbNomina As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents LblInfo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents groupDeduccion As DevExpress.XtraEditors.GroupControl
    Friend WithEvents BtnProcesarDeduccion As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents SpnDeduccion As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents LblMontoDeduccion As DevExpress.XtraEditors.LabelControl
    Friend WithEvents CmbDeduccion As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents LblTipoDeduccion As DevExpress.XtraEditors.LabelControl
    Friend WithEvents groupIncentivo As DevExpress.XtraEditors.GroupControl
    Friend WithEvents BtnProcesarIncentivo As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents SpnIncentivo As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents LblMontoIncentivo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents CmbIncentivo As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents LblTipoIncentivo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents Progreso As DevExpress.XtraEditors.ProgressBarControl
    Friend WithEvents LblProcesando As DevExpress.XtraEditors.LabelControl
    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
End Class
