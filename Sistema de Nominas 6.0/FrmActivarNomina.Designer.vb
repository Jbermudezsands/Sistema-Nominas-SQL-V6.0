<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmActivarNomina
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
        Me.BtnActivar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.groupTipos = New DevExpress.XtraEditors.GroupControl()
        Me.gridTipos = New DevExpress.XtraGrid.GridControl()
        Me.gridViewTipos = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.groupPeriodo = New DevExpress.XtraEditors.GroupControl()
        Me.LblAviso = New DevExpress.XtraEditors.LabelControl()
        Me.LblFechaLarga = New DevExpress.XtraEditors.LabelControl()
        Me.dtFinal = New DevExpress.XtraEditors.DateEdit()
        Me.dtInicio = New DevExpress.XtraEditors.DateEdit()
        Me.cmbPeriodo = New DevExpress.XtraEditors.LookUpEdit()
        Me.TxtAno = New System.Windows.Forms.TextBox()
        Me.labelControl4 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl1 = New DevExpress.XtraEditors.LabelControl()
        CType(Me.groupTipos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupTipos.SuspendLayout()
        CType(Me.gridTipos, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewTipos, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.groupPeriodo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupPeriodo.SuspendLayout()
        CType(Me.dtFinal.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dtFinal.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dtInicio.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dtInicio.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbPeriodo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnActivar
        '
        Me.BtnActivar.Location = New System.Drawing.Point(13, 330)
        Me.BtnActivar.Name = "BtnActivar"
        Me.BtnActivar.Size = New System.Drawing.Size(95, 35)
        Me.BtnActivar.TabIndex = 2
        Me.BtnActivar.Text = "Activar"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(119, 330)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 3
        Me.BtnSalir.Text = "Salir"
        '
        'groupTipos
        '
        Me.groupTipos.Controls.Add(Me.gridTipos)
        Me.groupTipos.Location = New System.Drawing.Point(12, 12)
        Me.groupTipos.Name = "groupTipos"
        Me.groupTipos.Size = New System.Drawing.Size(560, 130)
        Me.groupTipos.TabIndex = 0
        Me.groupTipos.Text = "Tipos de nómina por activar"
        '
        'gridTipos
        '
        Me.gridTipos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridTipos.Location = New System.Drawing.Point(2, 23)
        Me.gridTipos.MainView = Me.gridViewTipos
        Me.gridTipos.Name = "gridTipos"
        Me.gridTipos.Size = New System.Drawing.Size(556, 105)
        Me.gridTipos.TabIndex = 0
        Me.gridTipos.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewTipos})
        '
        'gridViewTipos
        '
        Me.gridViewTipos.GridControl = Me.gridTipos
        Me.gridViewTipos.Name = "gridViewTipos"
        Me.gridViewTipos.OptionsBehavior.Editable = False
        Me.gridViewTipos.OptionsView.ShowGroupPanel = False
        '
        'groupPeriodo
        '
        Me.groupPeriodo.Controls.Add(Me.LblAviso)
        Me.groupPeriodo.Controls.Add(Me.LblFechaLarga)
        Me.groupPeriodo.Controls.Add(Me.dtFinal)
        Me.groupPeriodo.Controls.Add(Me.dtInicio)
        Me.groupPeriodo.Controls.Add(Me.cmbPeriodo)
        Me.groupPeriodo.Controls.Add(Me.TxtAno)
        Me.groupPeriodo.Controls.Add(Me.labelControl4)
        Me.groupPeriodo.Controls.Add(Me.labelControl3)
        Me.groupPeriodo.Controls.Add(Me.labelControl2)
        Me.groupPeriodo.Controls.Add(Me.labelControl1)
        Me.groupPeriodo.Location = New System.Drawing.Point(12, 152)
        Me.groupPeriodo.Name = "groupPeriodo"
        Me.groupPeriodo.Size = New System.Drawing.Size(560, 165)
        Me.groupPeriodo.TabIndex = 1
        Me.groupPeriodo.Text = "Período para la activación"
        '
        'LblAviso
        '
        Me.LblAviso.Appearance.ForeColor = System.Drawing.Color.Firebrick
        Me.LblAviso.Appearance.Options.UseForeColor = True
        Me.LblAviso.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap
        Me.LblAviso.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LblAviso.Location = New System.Drawing.Point(15, 125)
        Me.LblAviso.Name = "LblAviso"
        Me.LblAviso.Size = New System.Drawing.Size(530, 32)
        Me.LblAviso.TabIndex = 9
        '
        'LblFechaLarga
        '
        Me.LblFechaLarga.Location = New System.Drawing.Point(15, 100)
        Me.LblFechaLarga.Name = "LblFechaLarga"
        Me.LblFechaLarga.Size = New System.Drawing.Size(0, 13)
        Me.LblFechaLarga.TabIndex = 8
        '
        'dtFinal
        '
        Me.dtFinal.EditValue = Nothing
        Me.dtFinal.Location = New System.Drawing.Point(415, 65)
        Me.dtFinal.Name = "dtFinal"
        Me.dtFinal.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.dtFinal.Properties.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.dtFinal.Properties.DisplayFormat.FormatString = "dd/MM/yyyy"
        Me.dtFinal.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
        Me.dtFinal.Properties.EditFormat.FormatString = "dd/MM/yyyy"
        Me.dtFinal.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime
        Me.dtFinal.Properties.Mask.EditMask = "dd/MM/yyyy"
        Me.dtFinal.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.dtFinal.Size = New System.Drawing.Size(130, 20)
        Me.dtFinal.TabIndex = 3
        '
        'dtInicio
        '
        Me.dtInicio.EditValue = Nothing
        Me.dtInicio.Location = New System.Drawing.Point(150, 65)
        Me.dtInicio.Name = "dtInicio"
        Me.dtInicio.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.dtInicio.Properties.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.dtInicio.Properties.DisplayFormat.FormatString = "dd/MM/yyyy"
        Me.dtInicio.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
        Me.dtInicio.Properties.EditFormat.FormatString = "dd/MM/yyyy"
        Me.dtInicio.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime
        Me.dtInicio.Properties.Mask.EditMask = "dd/MM/yyyy"
        Me.dtInicio.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.dtInicio.Properties.ReadOnly = True
        Me.dtInicio.Size = New System.Drawing.Size(130, 20)
        Me.dtInicio.TabIndex = 2
        '
        'cmbPeriodo
        '
        Me.cmbPeriodo.Location = New System.Drawing.Point(415, 35)
        Me.cmbPeriodo.Name = "cmbPeriodo"
        Me.cmbPeriodo.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cmbPeriodo.Properties.NullText = ""
        Me.cmbPeriodo.Size = New System.Drawing.Size(130, 20)
        Me.cmbPeriodo.TabIndex = 1
        '
        'TxtAno
        '
        Me.TxtAno.Location = New System.Drawing.Point(150, 35)
        Me.TxtAno.Name = "TxtAno"
        Me.TxtAno.ReadOnly = True
        Me.TxtAno.Size = New System.Drawing.Size(130, 21)
        Me.TxtAno.TabIndex = 0
        Me.TxtAno.TabStop = False
        '
        'labelControl4
        '
        Me.labelControl4.Location = New System.Drawing.Point(300, 68)
        Me.labelControl4.Name = "labelControl4"
        Me.labelControl4.Size = New System.Drawing.Size(110, 13)
        Me.labelControl4.TabIndex = 7
        Me.labelControl4.Text = "Fecha final (de pago):"
        '
        'labelControl3
        '
        Me.labelControl3.Location = New System.Drawing.Point(15, 68)
        Me.labelControl3.Name = "labelControl3"
        Me.labelControl3.Size = New System.Drawing.Size(66, 13)
        Me.labelControl3.TabIndex = 6
        Me.labelControl3.Text = "Fecha inicial:"
        '
        'labelControl2
        '
        Me.labelControl2.Location = New System.Drawing.Point(300, 38)
        Me.labelControl2.Name = "labelControl2"
        Me.labelControl2.Size = New System.Drawing.Size(42, 13)
        Me.labelControl2.TabIndex = 5
        Me.labelControl2.Text = "Período:"
        '
        'labelControl1
        '
        Me.labelControl1.Location = New System.Drawing.Point(15, 38)
        Me.labelControl1.Name = "labelControl1"
        Me.labelControl1.Size = New System.Drawing.Size(22, 13)
        Me.labelControl1.TabIndex = 4
        Me.labelControl1.Text = "Año:"
        '
        'FrmActivarNomina
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 377)
        Me.Controls.Add(Me.BtnActivar)
        Me.Controls.Add(Me.BtnSalir)
        Me.Controls.Add(Me.groupTipos)
        Me.Controls.Add(Me.groupPeriodo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmActivarNomina"
        Me.Text = "Activar Nóminas"
        CType(Me.groupTipos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupTipos.ResumeLayout(False)
        CType(Me.gridTipos, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewTipos, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.groupPeriodo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupPeriodo.ResumeLayout(False)
        Me.groupPeriodo.PerformLayout()
        CType(Me.dtFinal.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dtFinal.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dtInicio.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dtInicio.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmbPeriodo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents BtnActivar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents groupTipos As DevExpress.XtraEditors.GroupControl
    Friend WithEvents gridTipos As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewTipos As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents groupPeriodo As DevExpress.XtraEditors.GroupControl
    Friend WithEvents LblAviso As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LblFechaLarga As DevExpress.XtraEditors.LabelControl
    Friend WithEvents dtFinal As DevExpress.XtraEditors.DateEdit
    Friend WithEvents dtInicio As DevExpress.XtraEditors.DateEdit
    Friend WithEvents cmbPeriodo As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents TxtAno As System.Windows.Forms.TextBox
    Friend WithEvents labelControl4 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl1 As DevExpress.XtraEditors.LabelControl
End Class
