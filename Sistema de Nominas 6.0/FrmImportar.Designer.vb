<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmImportar
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
        Me.Menu = New DevExpress.XtraBars.Navigation.AccordionControl()
        Me.panelContenido = New DevExpress.XtraEditors.PanelControl()
        Me.gridPrevia = New DevExpress.XtraGrid.GridControl()
        Me.gridViewPrevia = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.panelEliminar = New DevExpress.XtraEditors.GroupControl()
        Me.BtnElimInc = New DevExpress.XtraEditors.SimpleButton()
        Me.CmbElimInc = New DevExpress.XtraEditors.LookUpEdit()
        Me.BtnElimDed = New DevExpress.XtraEditors.SimpleButton()
        Me.CmbElimDed = New DevExpress.XtraEditors.LookUpEdit()
        Me.BtnElimHE = New DevExpress.XtraEditors.SimpleButton()
        Me.SpnNomina = New DevExpress.XtraEditors.SpinEdit()
        Me.LblNomina = New DevExpress.XtraEditors.LabelControl()
        Me.panelArchivo = New DevExpress.XtraEditors.PanelControl()
        Me.panelOpciones = New System.Windows.Forms.FlowLayoutPanel()
        Me.ChkAcumular = New DevExpress.XtraEditors.CheckEdit()
        Me.LblFormato = New DevExpress.XtraEditors.LabelControl()
        Me.BtnExaminar = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtRuta = New DevExpress.XtraEditors.TextEdit()
        Me.LblRuta = New DevExpress.XtraEditors.LabelControl()
        Me.TxtResultado = New DevExpress.XtraEditors.MemoEdit()
        Me.panelBotones = New DevExpress.XtraEditors.PanelControl()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnImportar = New DevExpress.XtraEditors.SimpleButton()
        Me.panelEncabezado = New DevExpress.XtraEditors.PanelControl()
        Me.LblDescripcion = New DevExpress.XtraEditors.LabelControl()
        Me.LblTitulo = New DevExpress.XtraEditors.LabelControl()
        Me.CmbTipo = New DevExpress.XtraEditors.ComboBoxEdit()
        Me.LblTipo = New DevExpress.XtraEditors.LabelControl()
        CType(Me.Menu, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.panelContenido, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelContenido.SuspendLayout()
        CType(Me.gridPrevia, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewPrevia, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.panelEliminar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelEliminar.SuspendLayout()
        CType(Me.CmbElimInc.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CmbElimDed.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SpnNomina.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.panelArchivo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelArchivo.SuspendLayout()
        Me.panelOpciones.SuspendLayout()
        CType(Me.ChkAcumular.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtRuta.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtResultado.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        CType(Me.panelEncabezado, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelEncabezado.SuspendLayout()
        CType(Me.CmbTipo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Menu
        '
        Me.Menu.Dock = System.Windows.Forms.DockStyle.Left
        Me.Menu.Name = "Menu"
        Me.Menu.Size = New System.Drawing.Size(250, 700)
        Me.Menu.TabIndex = 0
        '
        'panelContenido
        '
        Me.panelContenido.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.panelContenido.Controls.Add(Me.gridPrevia)
        Me.panelContenido.Controls.Add(Me.panelEliminar)
        Me.panelContenido.Controls.Add(Me.panelArchivo)
        Me.panelContenido.Controls.Add(Me.TxtResultado)
        Me.panelContenido.Controls.Add(Me.panelBotones)
        Me.panelContenido.Controls.Add(Me.panelEncabezado)
        Me.panelContenido.Dock = System.Windows.Forms.DockStyle.Fill
        Me.panelContenido.Name = "panelContenido"
        Me.panelContenido.TabIndex = 1
        '
        'gridPrevia
        '
        Me.gridPrevia.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridPrevia.MainView = Me.gridViewPrevia
        Me.gridPrevia.Name = "gridPrevia"
        Me.gridPrevia.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewPrevia})
        '
        'gridViewPrevia
        '
        Me.gridViewPrevia.GridControl = Me.gridPrevia
        Me.gridViewPrevia.Name = "gridViewPrevia"
        Me.gridViewPrevia.OptionsBehavior.Editable = False
        Me.gridViewPrevia.OptionsView.ShowGroupPanel = False
        Me.gridViewPrevia.OptionsView.EnableAppearanceEvenRow = True
        '
        'panelEliminar
        '
        Me.panelEliminar.Controls.Add(Me.BtnElimInc)
        Me.panelEliminar.Controls.Add(Me.CmbElimInc)
        Me.panelEliminar.Controls.Add(Me.BtnElimDed)
        Me.panelEliminar.Controls.Add(Me.CmbElimDed)
        Me.panelEliminar.Controls.Add(Me.BtnElimHE)
        Me.panelEliminar.Controls.Add(Me.SpnNomina)
        Me.panelEliminar.Controls.Add(Me.LblNomina)
        Me.panelEliminar.Dock = System.Windows.Forms.DockStyle.Top
        Me.panelEliminar.Name = "panelEliminar"
        Me.panelEliminar.Size = New System.Drawing.Size(930, 130)
        Me.panelEliminar.TabIndex = 4
        Me.panelEliminar.Text = "Eliminar movimientos pendientes (sin pagar)"
        '
        'LblNomina
        '
        Me.LblNomina.Location = New System.Drawing.Point(20, 46)
        Me.LblNomina.Name = "LblNomina"
        Me.LblNomina.Text = "N.º de nómina:"
        '
        'SpnNomina
        '
        Me.SpnNomina.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.SpnNomina.Location = New System.Drawing.Point(110, 43)
        Me.SpnNomina.Name = "SpnNomina"
        Me.SpnNomina.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.SpnNomina.Properties.MaxValue = New Decimal(New Integer() {99999999, 0, 0, 0})
        Me.SpnNomina.Size = New System.Drawing.Size(100, 20)
        '
        'BtnElimHE
        '
        Me.BtnElimHE.Location = New System.Drawing.Point(20, 85)
        Me.BtnElimHE.Name = "BtnElimHE"
        Me.BtnElimHE.Size = New System.Drawing.Size(190, 30)
        Me.BtnElimHE.Text = "Eliminar horas extra"
        '
        'CmbElimDed
        '
        Me.CmbElimDed.Location = New System.Drawing.Point(250, 43)
        Me.CmbElimDed.Name = "CmbElimDed"
        Me.CmbElimDed.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.CmbElimDed.Properties.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Deduccion", "Tipo de deducción")})
        Me.CmbElimDed.Properties.DisplayMember = "Deduccion"
        Me.CmbElimDed.Properties.NullText = "Elija el tipo de deducción"
        Me.CmbElimDed.Properties.ShowHeader = False
        Me.CmbElimDed.Properties.ValueMember = "CodTipoDeduccion"
        Me.CmbElimDed.Size = New System.Drawing.Size(220, 20)
        '
        'BtnElimDed
        '
        Me.BtnElimDed.Location = New System.Drawing.Point(250, 85)
        Me.BtnElimDed.Name = "BtnElimDed"
        Me.BtnElimDed.Size = New System.Drawing.Size(220, 30)
        Me.BtnElimDed.Text = "Eliminar deducciones del tipo"
        '
        'CmbElimInc
        '
        Me.CmbElimInc.Location = New System.Drawing.Point(510, 43)
        Me.CmbElimInc.Name = "CmbElimInc"
        Me.CmbElimInc.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.CmbElimInc.Properties.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Incentivo", "Tipo de incentivo")})
        Me.CmbElimInc.Properties.DisplayMember = "Incentivo"
        Me.CmbElimInc.Properties.NullText = "Elija el tipo de incentivo"
        Me.CmbElimInc.Properties.ShowHeader = False
        Me.CmbElimInc.Properties.ValueMember = "CodTipoIncentivo"
        Me.CmbElimInc.Size = New System.Drawing.Size(220, 20)
        '
        'BtnElimInc
        '
        Me.BtnElimInc.Location = New System.Drawing.Point(510, 85)
        Me.BtnElimInc.Name = "BtnElimInc"
        Me.BtnElimInc.Size = New System.Drawing.Size(220, 30)
        Me.BtnElimInc.Text = "Eliminar incentivos del tipo"
        '
        'panelArchivo
        '
        Me.panelArchivo.AllowDrop = True
        Me.panelArchivo.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.panelArchivo.Controls.Add(Me.panelOpciones)
        Me.panelArchivo.Controls.Add(Me.LblFormato)
        Me.panelArchivo.Controls.Add(Me.BtnExaminar)
        Me.panelArchivo.Controls.Add(Me.TxtRuta)
        Me.panelArchivo.Controls.Add(Me.LblRuta)
        Me.panelArchivo.Dock = System.Windows.Forms.DockStyle.Top
        Me.panelArchivo.Name = "panelArchivo"
        Me.panelArchivo.Size = New System.Drawing.Size(930, 150)
        Me.panelArchivo.TabIndex = 3
        '
        'LblRuta
        '
        Me.LblRuta.Location = New System.Drawing.Point(24, 10)
        Me.LblRuta.Name = "LblRuta"
        Me.LblRuta.Text = "Archivo de Excel (también puede arrastrarlo aquí)"
        '
        'TxtRuta
        '
        Me.TxtRuta.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtRuta.Location = New System.Drawing.Point(24, 32)
        Me.TxtRuta.Name = "TxtRuta"
        Me.TxtRuta.Properties.NullValuePrompt = "Ningún archivo seleccionado"
        Me.TxtRuta.Properties.NullValuePromptShowForEmptyValue = True
        Me.TxtRuta.Properties.ReadOnly = True
        Me.TxtRuta.Size = New System.Drawing.Size(740, 22)
        '
        'BtnExaminar
        '
        Me.BtnExaminar.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnExaminar.Location = New System.Drawing.Point(776, 29)
        Me.BtnExaminar.Name = "BtnExaminar"
        Me.BtnExaminar.Size = New System.Drawing.Size(130, 28)
        Me.BtnExaminar.Text = "Examinar..."
        '
        'LblFormato
        '
        Me.LblFormato.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LblFormato.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(110, Byte), Integer), CType(CType(118, Byte), Integer), CType(CType(130, Byte), Integer))
        Me.LblFormato.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LblFormato.Location = New System.Drawing.Point(24, 64)
        Me.LblFormato.Name = "LblFormato"
        Me.LblFormato.Size = New System.Drawing.Size(882, 34)
        Me.LblFormato.Text = ""
        '
        'panelOpciones
        '
        Me.panelOpciones.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.panelOpciones.BackColor = System.Drawing.Color.Transparent
        Me.panelOpciones.Controls.Add(Me.ChkAcumular)
        Me.panelOpciones.Location = New System.Drawing.Point(24, 104)
        Me.panelOpciones.Name = "panelOpciones"
        Me.panelOpciones.Size = New System.Drawing.Size(882, 38)
        '
        'ChkAcumular
        '
        Me.ChkAcumular.Location = New System.Drawing.Point(3, 3)
        Me.ChkAcumular.Name = "ChkAcumular"
        Me.ChkAcumular.Properties.Caption = "Acumular horas si ya existen"
        Me.ChkAcumular.Size = New System.Drawing.Size(220, 20)
        '
        'TxtResultado
        '
        Me.TxtResultado.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.TxtResultado.Name = "TxtResultado"
        Me.TxtResultado.Properties.NullValuePrompt = "El resultado de la operación aparecerá aquí"
        Me.TxtResultado.Properties.NullValuePromptShowForEmptyValue = True
        Me.TxtResultado.Properties.ReadOnly = True
        Me.TxtResultado.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TxtResultado.Size = New System.Drawing.Size(930, 130)
        Me.TxtResultado.TabIndex = 2
        '
        'panelBotones
        '
        Me.panelBotones.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Controls.Add(Me.BtnImportar)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(930, 64)
        Me.panelBotones.TabIndex = 1
        '
        'BtnImportar
        '
        Me.BtnImportar.Appearance.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!, System.Drawing.FontStyle.Bold)
        Me.BtnImportar.Appearance.Options.UseFont = True
        Me.BtnImportar.Location = New System.Drawing.Point(24, 12)
        Me.BtnImportar.Name = "BtnImportar"
        Me.BtnImportar.Size = New System.Drawing.Size(170, 42)
        Me.BtnImportar.Text = "Importar"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(206, 12)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(110, 42)
        Me.BtnSalir.Text = "Salir"
        '
        'panelEncabezado
        '
        Me.panelEncabezado.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.panelEncabezado.Controls.Add(Me.LblDescripcion)
        Me.panelEncabezado.Controls.Add(Me.LblTitulo)
        Me.panelEncabezado.Dock = System.Windows.Forms.DockStyle.Top
        Me.panelEncabezado.Name = "panelEncabezado"
        Me.panelEncabezado.Size = New System.Drawing.Size(930, 80)
        Me.panelEncabezado.TabIndex = 0
        '
        'LblTitulo
        '
        Me.LblTitulo.Appearance.Font = New System.Drawing.Font("Segoe UI Semibold", 16.0!, System.Drawing.FontStyle.Bold)
        Me.LblTitulo.Appearance.Options.UseFont = True
        Me.LblTitulo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LblTitulo.Location = New System.Drawing.Point(24, 14)
        Me.LblTitulo.Name = "LblTitulo"
        Me.LblTitulo.Size = New System.Drawing.Size(880, 30)
        Me.LblTitulo.Text = "Importar desde Excel"
        '
        'LblDescripcion
        '
        Me.LblDescripcion.Appearance.Font = New System.Drawing.Font("Segoe UI", 9.5!)
        Me.LblDescripcion.Appearance.ForeColor = System.Drawing.Color.FromArgb(CType(CType(110, Byte), Integer), CType(CType(118, Byte), Integer), CType(CType(130, Byte), Integer))
        Me.LblDescripcion.Appearance.Options.UseFont = True
        Me.LblDescripcion.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None
        Me.LblDescripcion.Location = New System.Drawing.Point(24, 48)
        Me.LblDescripcion.Name = "LblDescripcion"
        Me.LblDescripcion.Size = New System.Drawing.Size(880, 22)
        Me.LblDescripcion.Text = ""
        '
        'CmbTipo
        '
        Me.CmbTipo.Name = "CmbTipo"
        Me.CmbTipo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        Me.CmbTipo.Visible = False
        '
        'LblTipo
        '
        Me.LblTipo.Name = "LblTipo"
        Me.LblTipo.Visible = False
        '
        'FrmImportar
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1180, 700)
        Me.Controls.Add(Me.panelContenido)
        Me.Controls.Add(Me.Menu)
        Me.Controls.Add(Me.CmbTipo)
        Me.Controls.Add(Me.LblTipo)
        Me.Name = "FrmImportar"
        Me.Text = "Importar desde Excel"
        CType(Me.Menu, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.panelContenido, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelContenido.ResumeLayout(False)
        CType(Me.gridPrevia, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewPrevia, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.panelEliminar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelEliminar.ResumeLayout(False)
        Me.panelEliminar.PerformLayout()
        CType(Me.CmbElimInc.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CmbElimDed.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SpnNomina.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.panelArchivo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelArchivo.ResumeLayout(False)
        Me.panelArchivo.PerformLayout()
        Me.panelOpciones.ResumeLayout(False)
        CType(Me.ChkAcumular.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtRuta.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtResultado.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        CType(Me.panelEncabezado, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelEncabezado.ResumeLayout(False)
        CType(Me.CmbTipo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Menu As DevExpress.XtraBars.Navigation.AccordionControl
    Friend WithEvents panelContenido As DevExpress.XtraEditors.PanelControl
    Friend WithEvents panelEncabezado As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LblTitulo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LblDescripcion As DevExpress.XtraEditors.LabelControl
    Friend WithEvents panelArchivo As DevExpress.XtraEditors.PanelControl
    Friend WithEvents panelOpciones As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents ChkAcumular As DevExpress.XtraEditors.CheckEdit
    Friend WithEvents LblFormato As DevExpress.XtraEditors.LabelControl
    Friend WithEvents BtnExaminar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtRuta As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LblRuta As DevExpress.XtraEditors.LabelControl
    Friend WithEvents CmbTipo As DevExpress.XtraEditors.ComboBoxEdit
    Friend WithEvents LblTipo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnImportar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents panelEliminar As DevExpress.XtraEditors.GroupControl
    Friend WithEvents BtnElimInc As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmbElimInc As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents BtnElimDed As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmbElimDed As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents BtnElimHE As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents SpnNomina As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents LblNomina As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtResultado As DevExpress.XtraEditors.MemoEdit
    Friend WithEvents gridPrevia As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewPrevia As DevExpress.XtraGrid.Views.Grid.GridView
End Class
