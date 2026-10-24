<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmPeriodo
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
        Me.groupDatos = New DevExpress.XtraEditors.GroupControl()
        Me.lblTipo = New DevExpress.XtraEditors.LabelControl()
        Me.cmbTipoNomina = New DevExpress.XtraEditors.LookUpEdit()
        Me.lblPeriodo = New DevExpress.XtraEditors.LabelControl()
        Me.LblPeriodoValor = New DevExpress.XtraEditors.LabelControl()
        Me.lblAnio = New DevExpress.XtraEditors.LabelControl()
        Me.TxtAnio = New DevExpress.XtraEditors.TextEdit()
        Me.lblInicio = New DevExpress.XtraEditors.LabelControl()
        Me.dtpInicio = New DevExpress.XtraEditors.DateEdit()
        Me.lblFinal = New DevExpress.XtraEditors.LabelControl()
        Me.dtpFinal = New DevExpress.XtraEditors.DateEdit()
        Me.gridPeriodos = New DevExpress.XtraGrid.GridControl()
        Me.gridView1 = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.BtnGenerar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnGuardar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        CType(Me.groupDatos, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbTipoNomina.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtAnio.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dtpInicio.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dtpFinal.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridPeriodos, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupDatos.SuspendLayout()
        Me.SuspendLayout()
        '
        'groupDatos
        '
        Me.groupDatos.Controls.Add(Me.lblTipo)
        Me.groupDatos.Controls.Add(Me.cmbTipoNomina)
        Me.groupDatos.Controls.Add(Me.lblPeriodo)
        Me.groupDatos.Controls.Add(Me.LblPeriodoValor)
        Me.groupDatos.Controls.Add(Me.lblAnio)
        Me.groupDatos.Controls.Add(Me.TxtAnio)
        Me.groupDatos.Controls.Add(Me.lblInicio)
        Me.groupDatos.Controls.Add(Me.dtpInicio)
        Me.groupDatos.Controls.Add(Me.lblFinal)
        Me.groupDatos.Controls.Add(Me.dtpFinal)
        Me.groupDatos.Location = New System.Drawing.Point(12, 12)
        Me.groupDatos.Name = "groupDatos"
        Me.groupDatos.Size = New System.Drawing.Size(736, 100)
        Me.groupDatos.TabIndex = 0
        Me.groupDatos.Text = "Planilla"
        '
        'lblTipo
        '
        Me.lblTipo.Location = New System.Drawing.Point(15, 38)
        Me.lblTipo.Name = "lblTipo"
        Me.lblTipo.Size = New System.Drawing.Size(80, 13)
        Me.lblTipo.TabIndex = 1
        Me.lblTipo.Text = "Tipo de nómina:"
        '
        'cmbTipoNomina
        '
        Me.cmbTipoNomina.Location = New System.Drawing.Point(120, 35)
        Me.cmbTipoNomina.Name = "cmbTipoNomina"
        Me.cmbTipoNomina.Size = New System.Drawing.Size(260, 20)
        Me.cmbTipoNomina.TabIndex = 2
        Me.cmbTipoNomina.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cmbTipoNomina.Properties.NullText = ""
        '
        'lblPeriodo
        '
        Me.lblPeriodo.Location = New System.Drawing.Point(400, 38)
        Me.lblPeriodo.Name = "lblPeriodo"
        Me.lblPeriodo.Size = New System.Drawing.Size(41, 13)
        Me.lblPeriodo.TabIndex = 3
        Me.lblPeriodo.Text = "Período:"
        '
        'LblPeriodoValor
        '
        Me.LblPeriodoValor.Location = New System.Drawing.Point(450, 38)
        Me.LblPeriodoValor.Name = "LblPeriodoValor"
        Me.LblPeriodoValor.Size = New System.Drawing.Size(10, 13)
        Me.LblPeriodoValor.TabIndex = 4
        Me.LblPeriodoValor.Appearance.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold)
        Me.LblPeriodoValor.Appearance.Options.UseFont = True
        '
        'lblAnio
        '
        Me.lblAnio.Location = New System.Drawing.Point(15, 70)
        Me.lblAnio.Name = "lblAnio"
        Me.lblAnio.Size = New System.Drawing.Size(25, 13)
        Me.lblAnio.TabIndex = 5
        Me.lblAnio.Text = "Año:"
        '
        'TxtAnio
        '
        Me.TxtAnio.Location = New System.Drawing.Point(120, 67)
        Me.TxtAnio.Name = "TxtAnio"
        Me.TxtAnio.Size = New System.Drawing.Size(70, 20)
        Me.TxtAnio.TabIndex = 6
        Me.TxtAnio.EditValue = 2000
        Me.TxtAnio.Properties.Mask.EditMask = "0000"
        Me.TxtAnio.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        '
        'lblInicio
        '
        Me.lblInicio.Location = New System.Drawing.Point(215, 70)
        Me.lblInicio.Name = "lblInicio"
        Me.lblInicio.Size = New System.Drawing.Size(34, 13)
        Me.lblInicio.TabIndex = 7
        Me.lblInicio.Text = "Desde:"
        '
        'dtpInicio
        '
        Me.dtpInicio.Location = New System.Drawing.Point(260, 67)
        Me.dtpInicio.Name = "dtpInicio"
        Me.dtpInicio.Size = New System.Drawing.Size(110, 20)
        Me.dtpInicio.TabIndex = 8
        Me.dtpInicio.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        '
        'lblFinal
        '
        Me.lblFinal.Location = New System.Drawing.Point(395, 70)
        Me.lblFinal.Name = "lblFinal"
        Me.lblFinal.Size = New System.Drawing.Size(33, 13)
        Me.lblFinal.TabIndex = 9
        Me.lblFinal.Text = "Hasta:"
        '
        'dtpFinal
        '
        Me.dtpFinal.Location = New System.Drawing.Point(440, 67)
        Me.dtpFinal.Name = "dtpFinal"
        Me.dtpFinal.Size = New System.Drawing.Size(110, 20)
        Me.dtpFinal.TabIndex = 10
        Me.dtpFinal.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        '
        'gridPeriodos
        '
        Me.gridPeriodos.Location = New System.Drawing.Point(12, 120)
        Me.gridPeriodos.Name = "gridPeriodos"
        Me.gridPeriodos.Size = New System.Drawing.Size(736, 380)
        Me.gridPeriodos.TabIndex = 11
        Me.gridPeriodos.MainView = Me.gridView1
        Me.gridPeriodos.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridView1})
        '
        'gridView1
        '
        Me.gridView1.Name = "gridView1"
        Me.gridView1.GridControl = Me.gridPeriodos
        Me.gridView1.OptionsBehavior.Editable = False
        Me.gridView1.OptionsView.ShowGroupPanel = False
        '
        'BtnGenerar
        '
        Me.BtnGenerar.Location = New System.Drawing.Point(13, 512)
        Me.BtnGenerar.Name = "BtnGenerar"
        Me.BtnGenerar.Size = New System.Drawing.Size(95, 35)
        Me.BtnGenerar.TabIndex = 13
        Me.BtnGenerar.Text = "Generar"
        '
        'BtnGuardar
        '
        Me.BtnGuardar.Location = New System.Drawing.Point(119, 512)
        Me.BtnGuardar.Name = "BtnGuardar"
        Me.BtnGuardar.Size = New System.Drawing.Size(95, 35)
        Me.BtnGuardar.TabIndex = 14
        Me.BtnGuardar.Text = "Guardar"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(653, 512)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 15
        Me.BtnSalir.Text = "Salir"
        '
        'FrmPeriodo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(760, 555)
        Me.Controls.Add(Me.groupDatos)
        Me.Controls.Add(Me.gridPeriodos)
        Me.Controls.Add(Me.BtnGenerar)
        Me.Controls.Add(Me.BtnGuardar)
        Me.Controls.Add(Me.BtnSalir)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmPeriodo"
        Me.Text = "Periodos"
        CType(Me.groupDatos, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmbTipoNomina.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtAnio.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dtpInicio.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dtpFinal.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridPeriodos, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupDatos.ResumeLayout(False)
        Me.groupDatos.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents groupDatos As DevExpress.XtraEditors.GroupControl
    Friend WithEvents lblTipo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents cmbTipoNomina As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents lblPeriodo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LblPeriodoValor As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lblAnio As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtAnio As DevExpress.XtraEditors.TextEdit
    Friend WithEvents lblInicio As DevExpress.XtraEditors.LabelControl
    Friend WithEvents dtpInicio As DevExpress.XtraEditors.DateEdit
    Friend WithEvents lblFinal As DevExpress.XtraEditors.LabelControl
    Friend WithEvents dtpFinal As DevExpress.XtraEditors.DateEdit
    Friend WithEvents gridPeriodos As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridView1 As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents BtnGenerar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnGuardar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
End Class
