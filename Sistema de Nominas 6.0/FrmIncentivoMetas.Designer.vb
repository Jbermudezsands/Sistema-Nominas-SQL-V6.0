<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmIncentivoMetas
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
        Me.Tabs = New DevExpress.XtraTab.XtraTabControl()
        Me.TabConfiguracion = New DevExpress.XtraTab.XtraTabPage()
        Me.TabMetas = New DevExpress.XtraTab.XtraTabPage()
        Me.TabProduccion = New DevExpress.XtraTab.XtraTabPage()
        Me.gpPuntualidad = New DevExpress.XtraEditors.GroupControl()
        Me.TxtMonto = New DevExpress.XtraEditors.SpinEdit()
        Me.TxtHoras = New DevExpress.XtraEditors.SpinEdit()
        Me.lbl2 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl1 = New DevExpress.XtraEditors.LabelControl()
        Me.gpSeptimo = New DevExpress.XtraEditors.GroupControl()
        Me.TxtHorasSeptimo = New DevExpress.XtraEditors.SpinEdit()
        Me.lbl3 = New DevExpress.XtraEditors.LabelControl()
        Me.gpBasico = New DevExpress.XtraEditors.GroupControl()
        Me.TxtHorasBasico = New DevExpress.XtraEditors.SpinEdit()
        Me.lbl4 = New DevExpress.XtraEditors.LabelControl()
        Me.gpViaticos = New DevExpress.XtraEditors.GroupControl()
        Me.TxtMontoViatico = New DevExpress.XtraEditors.SpinEdit()
        Me.lbl5 = New DevExpress.XtraEditors.LabelControl()
        Me.BtnGrabarConfig = New DevExpress.XtraEditors.SimpleButton()
        Me.PanelMetas = New DevExpress.XtraEditors.PanelControl()
        Me.BtnEliminarMeta = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnGrabarMetas = New DevExpress.XtraEditors.SimpleButton()
        Me.GridMetas = New DevExpress.XtraGrid.GridControl()
        Me.VistaMetas = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.gpProduccion = New DevExpress.XtraEditors.GroupControl()
        Me.BtnEliminarProd = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnAgregar = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtValor = New DevExpress.XtraEditors.SpinEdit()
        Me.TxtCantidadHoras = New DevExpress.XtraEditors.TextEdit()
        Me.CmbDepartamento = New DevExpress.XtraEditors.LookUpEdit()
        Me.lbl8 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl7 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl6 = New DevExpress.XtraEditors.LabelControl()
        Me.GridProduccion = New DevExpress.XtraGrid.GridControl()
        Me.VistaProduccion = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.PanelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelBotones.SuspendLayout()
        CType(Me.Tabs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Tabs.SuspendLayout()
        Me.TabConfiguracion.SuspendLayout()
        Me.TabMetas.SuspendLayout()
        Me.TabProduccion.SuspendLayout()
        CType(Me.gpPuntualidad, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpPuntualidad.SuspendLayout()
        CType(Me.TxtMonto.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtHoras.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gpSeptimo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpSeptimo.SuspendLayout()
        CType(Me.TxtHorasSeptimo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gpBasico, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpBasico.SuspendLayout()
        CType(Me.TxtHorasBasico.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gpViaticos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpViaticos.SuspendLayout()
        CType(Me.TxtMontoViatico.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PanelMetas, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelMetas.SuspendLayout()
        CType(Me.GridMetas, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.VistaMetas, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gpProduccion, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpProduccion.SuspendLayout()
        CType(Me.TxtValor.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtCantidadHoras.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CmbDepartamento.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridProduccion, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.VistaProduccion, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelBotones
        '
        Me.PanelBotones.Controls.Add(Me.BtnSalir)
        Me.PanelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBotones.Name = "PanelBotones"
        Me.PanelBotones.Size = New System.Drawing.Size(760, 55)
        Me.PanelBotones.TabIndex = 1
        '
        'BtnSalir
        '
        Me.BtnSalir.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnSalir.Location = New System.Drawing.Point(652, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 0
        Me.BtnSalir.Text = "Salir"
        '
        'Tabs
        '
        Me.Tabs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Tabs.Name = "Tabs"
        Me.Tabs.SelectedTabPage = Me.TabConfiguracion
        Me.Tabs.TabIndex = 0
        Me.Tabs.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.TabConfiguracion, Me.TabMetas, Me.TabProduccion})
        '
        'TabConfiguracion
        '
        Me.TabConfiguracion.Controls.Add(Me.BtnGrabarConfig)
        Me.TabConfiguracion.Controls.Add(Me.gpViaticos)
        Me.TabConfiguracion.Controls.Add(Me.gpBasico)
        Me.TabConfiguracion.Controls.Add(Me.gpSeptimo)
        Me.TabConfiguracion.Controls.Add(Me.gpPuntualidad)
        Me.TabConfiguracion.Name = "TabConfiguracion"
        Me.TabConfiguracion.Text = "Puntualidad / Séptimo"
        '
        'gpPuntualidad
        '
        Me.gpPuntualidad.Controls.Add(Me.TxtMonto)
        Me.gpPuntualidad.Controls.Add(Me.TxtHoras)
        Me.gpPuntualidad.Controls.Add(Me.lbl2)
        Me.gpPuntualidad.Controls.Add(Me.lbl1)
        Me.gpPuntualidad.Location = New System.Drawing.Point(15, 15)
        Me.gpPuntualidad.Name = "gpPuntualidad"
        Me.gpPuntualidad.Size = New System.Drawing.Size(340, 110)
        Me.gpPuntualidad.TabIndex = 0
        Me.gpPuntualidad.Text = "Configuración para el cálculo de puntualidad"
        '
        'lbl1
        '
        Me.lbl1.Location = New System.Drawing.Point(15, 43)
        Me.lbl1.Name = "lbl1"
        Me.lbl1.Text = "Cantidad de horas:"
        '
        'lbl2
        '
        Me.lbl2.Location = New System.Drawing.Point(15, 73)
        Me.lbl2.Name = "lbl2"
        Me.lbl2.Text = "Monto:"
        '
        'TxtHoras
        '
        Me.TxtHoras.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.TxtHoras.Location = New System.Drawing.Point(150, 40)
        Me.TxtHoras.Name = "TxtHoras"
        Me.TxtHoras.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.TxtHoras.Properties.MaxValue = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.TxtHoras.Size = New System.Drawing.Size(170, 20)
        Me.TxtHoras.TabIndex = 0
        '
        'TxtMonto
        '
        Me.TxtMonto.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.TxtMonto.Location = New System.Drawing.Point(150, 70)
        Me.TxtMonto.Name = "TxtMonto"
        Me.TxtMonto.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.TxtMonto.Properties.DisplayFormat.FormatString = "N2"
        Me.TxtMonto.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.TxtMonto.Properties.MaxValue = New Decimal(New Integer() {99999999, 0, 0, 0})
        Me.TxtMonto.Size = New System.Drawing.Size(170, 20)
        Me.TxtMonto.TabIndex = 1
        '
        'gpSeptimo
        '
        Me.gpSeptimo.Controls.Add(Me.TxtHorasSeptimo)
        Me.gpSeptimo.Controls.Add(Me.lbl3)
        Me.gpSeptimo.Location = New System.Drawing.Point(370, 15)
        Me.gpSeptimo.Name = "gpSeptimo"
        Me.gpSeptimo.Size = New System.Drawing.Size(340, 110)
        Me.gpSeptimo.TabIndex = 1
        Me.gpSeptimo.Text = "Cálculo del séptimo día"
        '
        'lbl3
        '
        Me.lbl3.Location = New System.Drawing.Point(15, 43)
        Me.lbl3.Name = "lbl3"
        Me.lbl3.Text = "Cantidad de horas:"
        '
        'TxtHorasSeptimo
        '
        Me.TxtHorasSeptimo.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.TxtHorasSeptimo.Location = New System.Drawing.Point(150, 40)
        Me.TxtHorasSeptimo.Name = "TxtHorasSeptimo"
        Me.TxtHorasSeptimo.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.TxtHorasSeptimo.Properties.DisplayFormat.FormatString = "N2"
        Me.TxtHorasSeptimo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.TxtHorasSeptimo.Properties.MaxValue = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.TxtHorasSeptimo.Size = New System.Drawing.Size(170, 20)
        Me.TxtHorasSeptimo.TabIndex = 0
        '
        'gpBasico
        '
        Me.gpBasico.Controls.Add(Me.TxtHorasBasico)
        Me.gpBasico.Controls.Add(Me.lbl4)
        Me.gpBasico.Location = New System.Drawing.Point(15, 140)
        Me.gpBasico.Name = "gpBasico"
        Me.gpBasico.Size = New System.Drawing.Size(340, 80)
        Me.gpBasico.TabIndex = 2
        Me.gpBasico.Text = "Salario básico por antigüedad"
        '
        'lbl4
        '
        Me.lbl4.Location = New System.Drawing.Point(15, 43)
        Me.lbl4.Name = "lbl4"
        Me.lbl4.Text = "Cantidad de horas:"
        '
        'TxtHorasBasico
        '
        Me.TxtHorasBasico.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.TxtHorasBasico.Location = New System.Drawing.Point(150, 40)
        Me.TxtHorasBasico.Name = "TxtHorasBasico"
        Me.TxtHorasBasico.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.TxtHorasBasico.Properties.MaxValue = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.TxtHorasBasico.Size = New System.Drawing.Size(170, 20)
        Me.TxtHorasBasico.TabIndex = 0
        '
        'gpViaticos
        '
        Me.gpViaticos.Controls.Add(Me.TxtMontoViatico)
        Me.gpViaticos.Controls.Add(Me.lbl5)
        Me.gpViaticos.Location = New System.Drawing.Point(370, 140)
        Me.gpViaticos.Name = "gpViaticos"
        Me.gpViaticos.Size = New System.Drawing.Size(340, 80)
        Me.gpViaticos.TabIndex = 3
        Me.gpViaticos.Text = "Viáticos"
        '
        'lbl5
        '
        Me.lbl5.Location = New System.Drawing.Point(15, 43)
        Me.lbl5.Name = "lbl5"
        Me.lbl5.Text = "Monto:"
        '
        'TxtMontoViatico
        '
        Me.TxtMontoViatico.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.TxtMontoViatico.Location = New System.Drawing.Point(150, 40)
        Me.TxtMontoViatico.Name = "TxtMontoViatico"
        Me.TxtMontoViatico.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.TxtMontoViatico.Properties.DisplayFormat.FormatString = "N2"
        Me.TxtMontoViatico.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.TxtMontoViatico.Properties.MaxValue = New Decimal(New Integer() {99999999, 0, 0, 0})
        Me.TxtMontoViatico.Size = New System.Drawing.Size(170, 20)
        Me.TxtMontoViatico.TabIndex = 0
        '
        'BtnGrabarConfig
        '
        Me.BtnGrabarConfig.Location = New System.Drawing.Point(15, 235)
        Me.BtnGrabarConfig.Name = "BtnGrabarConfig"
        Me.BtnGrabarConfig.Size = New System.Drawing.Size(95, 35)
        Me.BtnGrabarConfig.TabIndex = 4
        Me.BtnGrabarConfig.Text = "Grabar"
        '
        'TabMetas
        '
        Me.TabMetas.Controls.Add(Me.GridMetas)
        Me.TabMetas.Controls.Add(Me.PanelMetas)
        Me.TabMetas.Name = "TabMetas"
        Me.TabMetas.Text = "Incentivo por Metas"
        '
        'PanelMetas
        '
        Me.PanelMetas.Controls.Add(Me.BtnEliminarMeta)
        Me.PanelMetas.Controls.Add(Me.BtnGrabarMetas)
        Me.PanelMetas.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelMetas.Name = "PanelMetas"
        Me.PanelMetas.Size = New System.Drawing.Size(754, 50)
        Me.PanelMetas.TabIndex = 1
        '
        'BtnGrabarMetas
        '
        Me.BtnGrabarMetas.Location = New System.Drawing.Point(10, 8)
        Me.BtnGrabarMetas.Name = "BtnGrabarMetas"
        Me.BtnGrabarMetas.Size = New System.Drawing.Size(95, 35)
        Me.BtnGrabarMetas.TabIndex = 0
        Me.BtnGrabarMetas.Text = "Grabar"
        '
        'BtnEliminarMeta
        '
        Me.BtnEliminarMeta.Location = New System.Drawing.Point(115, 8)
        Me.BtnEliminarMeta.Name = "BtnEliminarMeta"
        Me.BtnEliminarMeta.Size = New System.Drawing.Size(95, 35)
        Me.BtnEliminarMeta.TabIndex = 1
        Me.BtnEliminarMeta.Text = "Eliminar fila"
        '
        'GridMetas
        '
        Me.GridMetas.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GridMetas.MainView = Me.VistaMetas
        Me.GridMetas.Name = "GridMetas"
        Me.GridMetas.TabIndex = 0
        Me.GridMetas.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.VistaMetas})
        '
        'VistaMetas
        '
        Me.VistaMetas.GridControl = Me.GridMetas
        Me.VistaMetas.Name = "VistaMetas"
        Me.VistaMetas.OptionsView.ColumnAutoWidth = False
        Me.VistaMetas.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top
        Me.VistaMetas.OptionsView.ShowGroupPanel = False
        '
        'TabProduccion
        '
        Me.TabProduccion.Controls.Add(Me.GridProduccion)
        Me.TabProduccion.Controls.Add(Me.gpProduccion)
        Me.TabProduccion.Name = "TabProduccion"
        Me.TabProduccion.Text = "Incentivo x Producción"
        '
        'gpProduccion
        '
        Me.gpProduccion.Controls.Add(Me.BtnEliminarProd)
        Me.gpProduccion.Controls.Add(Me.BtnAgregar)
        Me.gpProduccion.Controls.Add(Me.TxtValor)
        Me.gpProduccion.Controls.Add(Me.TxtCantidadHoras)
        Me.gpProduccion.Controls.Add(Me.CmbDepartamento)
        Me.gpProduccion.Controls.Add(Me.lbl8)
        Me.gpProduccion.Controls.Add(Me.lbl7)
        Me.gpProduccion.Controls.Add(Me.lbl6)
        Me.gpProduccion.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpProduccion.Name = "gpProduccion"
        Me.gpProduccion.Size = New System.Drawing.Size(754, 110)
        Me.gpProduccion.TabIndex = 1
        Me.gpProduccion.Text = "Datos de los incentivos"
        '
        'lbl6
        '
        Me.lbl6.Location = New System.Drawing.Point(15, 43)
        Me.lbl6.Name = "lbl6"
        Me.lbl6.Text = "Departamento:"
        '
        'lbl7
        '
        Me.lbl7.Location = New System.Drawing.Point(15, 73)
        Me.lbl7.Name = "lbl7"
        Me.lbl7.Text = "Cantidad de horas:"
        '
        'lbl8
        '
        Me.lbl8.Location = New System.Drawing.Point(300, 73)
        Me.lbl8.Name = "lbl8"
        Me.lbl8.Text = "Valor:"
        '
        'CmbDepartamento
        '
        Me.CmbDepartamento.Location = New System.Drawing.Point(125, 40)
        Me.CmbDepartamento.Name = "CmbDepartamento"
        Me.CmbDepartamento.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.CmbDepartamento.Properties.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo("CodDepartamento", "Código", 50), New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Departamento", "Departamento")})
        Me.CmbDepartamento.Properties.DisplayMember = "Departamento"
        Me.CmbDepartamento.Properties.NullText = ""
        Me.CmbDepartamento.Properties.ValueMember = "CodDepartamento"
        Me.CmbDepartamento.Size = New System.Drawing.Size(280, 20)
        Me.CmbDepartamento.TabIndex = 0
        '
        'TxtCantidadHoras
        '
        Me.TxtCantidadHoras.Location = New System.Drawing.Point(125, 70)
        Me.TxtCantidadHoras.Name = "TxtCantidadHoras"
        Me.TxtCantidadHoras.Properties.MaxLength = 10
        Me.TxtCantidadHoras.Size = New System.Drawing.Size(150, 20)
        Me.TxtCantidadHoras.TabIndex = 1
        '
        'TxtValor
        '
        Me.TxtValor.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.TxtValor.Location = New System.Drawing.Point(340, 70)
        Me.TxtValor.Name = "TxtValor"
        Me.TxtValor.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.TxtValor.Properties.DisplayFormat.FormatString = "N2"
        Me.TxtValor.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.TxtValor.Properties.MaxValue = New Decimal(New Integer() {99999999, 0, 0, 0})
        Me.TxtValor.Size = New System.Drawing.Size(120, 20)
        Me.TxtValor.TabIndex = 2
        '
        'BtnAgregar
        '
        Me.BtnAgregar.Location = New System.Drawing.Point(480, 55)
        Me.BtnAgregar.Name = "BtnAgregar"
        Me.BtnAgregar.Size = New System.Drawing.Size(95, 35)
        Me.BtnAgregar.TabIndex = 3
        Me.BtnAgregar.Text = "Agregar"
        '
        'BtnEliminarProd
        '
        Me.BtnEliminarProd.Location = New System.Drawing.Point(585, 55)
        Me.BtnEliminarProd.Name = "BtnEliminarProd"
        Me.BtnEliminarProd.Size = New System.Drawing.Size(95, 35)
        Me.BtnEliminarProd.TabIndex = 4
        Me.BtnEliminarProd.Text = "Eliminar"
        '
        'GridProduccion
        '
        Me.GridProduccion.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GridProduccion.MainView = Me.VistaProduccion
        Me.GridProduccion.Name = "GridProduccion"
        Me.GridProduccion.TabIndex = 0
        Me.GridProduccion.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.VistaProduccion})
        '
        'VistaProduccion
        '
        Me.VistaProduccion.GridControl = Me.GridProduccion
        Me.VistaProduccion.Name = "VistaProduccion"
        Me.VistaProduccion.OptionsBehavior.Editable = False
        Me.VistaProduccion.OptionsView.ShowGroupPanel = False
        '
        'FrmIncentivoMetas
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(760, 460)
        Me.Controls.Add(Me.Tabs)
        Me.Controls.Add(Me.PanelBotones)
        Me.Name = "FrmIncentivoMetas"
        Me.Text = "Incentivo por Metas"
        CType(Me.PanelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelBotones.ResumeLayout(False)
        CType(Me.Tabs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Tabs.ResumeLayout(False)
        Me.TabConfiguracion.ResumeLayout(False)
        Me.TabMetas.ResumeLayout(False)
        Me.TabProduccion.ResumeLayout(False)
        CType(Me.gpPuntualidad, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpPuntualidad.ResumeLayout(False)
        Me.gpPuntualidad.PerformLayout()
        CType(Me.TxtMonto.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtHoras.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gpSeptimo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpSeptimo.ResumeLayout(False)
        Me.gpSeptimo.PerformLayout()
        CType(Me.TxtHorasSeptimo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gpBasico, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpBasico.ResumeLayout(False)
        Me.gpBasico.PerformLayout()
        CType(Me.TxtHorasBasico.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gpViaticos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpViaticos.ResumeLayout(False)
        Me.gpViaticos.PerformLayout()
        CType(Me.TxtMontoViatico.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PanelMetas, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelMetas.ResumeLayout(False)
        CType(Me.GridMetas, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.VistaMetas, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gpProduccion, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpProduccion.ResumeLayout(False)
        Me.gpProduccion.PerformLayout()
        CType(Me.TxtValor.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtCantidadHoras.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CmbDepartamento.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridProduccion, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.VistaProduccion, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents Tabs As DevExpress.XtraTab.XtraTabControl
    Friend WithEvents TabConfiguracion As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents TabMetas As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents TabProduccion As DevExpress.XtraTab.XtraTabPage
    Friend WithEvents gpPuntualidad As DevExpress.XtraEditors.GroupControl
    Friend WithEvents TxtMonto As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents TxtHoras As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents lbl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents gpSeptimo As DevExpress.XtraEditors.GroupControl
    Friend WithEvents TxtHorasSeptimo As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents lbl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents gpBasico As DevExpress.XtraEditors.GroupControl
    Friend WithEvents TxtHorasBasico As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents lbl4 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents gpViaticos As DevExpress.XtraEditors.GroupControl
    Friend WithEvents TxtMontoViatico As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents lbl5 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents BtnGrabarConfig As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents PanelMetas As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnEliminarMeta As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnGrabarMetas As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents GridMetas As DevExpress.XtraGrid.GridControl
    Friend WithEvents VistaMetas As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents gpProduccion As DevExpress.XtraEditors.GroupControl
    Friend WithEvents BtnEliminarProd As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnAgregar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtValor As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents TxtCantidadHoras As DevExpress.XtraEditors.TextEdit
    Friend WithEvents CmbDepartamento As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents lbl8 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl7 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl6 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents GridProduccion As DevExpress.XtraGrid.GridControl
    Friend WithEvents VistaProduccion As DevExpress.XtraGrid.Views.Grid.GridView
End Class
