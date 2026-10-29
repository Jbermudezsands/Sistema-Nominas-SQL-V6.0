<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmListNomina
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
        Me.panelBotones = New DevExpress.XtraEditors.PanelControl()
        Me.BtnAnular = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnExcel = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.gridNominas = New DevExpress.XtraGrid.GridControl()
        Me.gridViewNominas = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.groupTotales = New DevExpress.XtraEditors.GroupControl()
        Me.TxtSalBasico = New DevExpress.XtraEditors.TextEdit()
        Me.TxtDestajo = New DevExpress.XtraEditors.TextEdit()
        Me.TxtHE = New DevExpress.XtraEditors.TextEdit()
        Me.TxtComisiones = New DevExpress.XtraEditors.TextEdit()
        Me.TxtOtIngre = New DevExpress.XtraEditors.TextEdit()
        Me.TxtIncentivos = New DevExpress.XtraEditors.TextEdit()
        Me.TxtDeducciones = New DevExpress.XtraEditors.TextEdit()
        Me.TxtPrestamo = New DevExpress.XtraEditors.TextEdit()
        Me.TxtInss = New DevExpress.XtraEditors.TextEdit()
        Me.TxtIR = New DevExpress.XtraEditors.TextEdit()
        Me.lbl1 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl2 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl3 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl4 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl5 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl6 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl7 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl8 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl9 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl10 = New DevExpress.XtraEditors.LabelControl()
        Me.gridDetalle = New DevExpress.XtraGrid.GridControl()
        Me.gridViewDetalle = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        CType(Me.gridNominas, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewNominas, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.groupTotales, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupTotales.SuspendLayout()
        CType(Me.TxtSalBasico.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtDestajo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtHE.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtComisiones.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtOtIngre.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtIncentivos.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtDeducciones.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtPrestamo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtInss.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtIR.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridDetalle, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewDetalle, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelBotones
        '
        Me.panelBotones.Controls.Add(Me.BtnAnular)
        Me.panelBotones.Controls.Add(Me.BtnExcel)
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Location = New System.Drawing.Point(0, 466)
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(1040, 55)
        Me.panelBotones.TabIndex = 3
        '
        'BtnAnular
        '
        Me.BtnAnular.Location = New System.Drawing.Point(13, 10)
        Me.BtnAnular.Name = "BtnAnular"
        Me.BtnAnular.Size = New System.Drawing.Size(95, 35)
        Me.BtnAnular.TabIndex = 0
        Me.BtnAnular.Text = "Anular"
        '
        'BtnExcel
        '
        Me.BtnExcel.Location = New System.Drawing.Point(119, 10)
        Me.BtnExcel.Name = "BtnExcel"
        Me.BtnExcel.Size = New System.Drawing.Size(95, 35)
        Me.BtnExcel.TabIndex = 1
        Me.BtnExcel.Text = "Excel"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(225, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 2
        Me.BtnSalir.Text = "Salir"
        '
        'gridNominas
        '
        Me.gridNominas.Dock = System.Windows.Forms.DockStyle.Top
        Me.gridNominas.Location = New System.Drawing.Point(0, 0)
        Me.gridNominas.MainView = Me.gridViewNominas
        Me.gridNominas.Name = "gridNominas"
        Me.gridNominas.Size = New System.Drawing.Size(1040, 180)
        Me.gridNominas.TabIndex = 0
        Me.gridNominas.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewNominas})
        '
        'gridViewNominas
        '
        Me.gridViewNominas.GridControl = Me.gridNominas
        Me.gridViewNominas.Name = "gridViewNominas"
        Me.gridViewNominas.OptionsBehavior.Editable = False
        Me.gridViewNominas.OptionsView.ShowGroupPanel = False
        '
        'groupTotales
        '
        Me.groupTotales.Controls.Add(Me.TxtSalBasico)
        Me.groupTotales.Controls.Add(Me.TxtDestajo)
        Me.groupTotales.Controls.Add(Me.TxtHE)
        Me.groupTotales.Controls.Add(Me.TxtComisiones)
        Me.groupTotales.Controls.Add(Me.TxtOtIngre)
        Me.groupTotales.Controls.Add(Me.TxtIncentivos)
        Me.groupTotales.Controls.Add(Me.TxtDeducciones)
        Me.groupTotales.Controls.Add(Me.TxtPrestamo)
        Me.groupTotales.Controls.Add(Me.TxtInss)
        Me.groupTotales.Controls.Add(Me.TxtIR)
        Me.groupTotales.Controls.Add(Me.lbl1)
        Me.groupTotales.Controls.Add(Me.lbl2)
        Me.groupTotales.Controls.Add(Me.lbl3)
        Me.groupTotales.Controls.Add(Me.lbl4)
        Me.groupTotales.Controls.Add(Me.lbl5)
        Me.groupTotales.Controls.Add(Me.lbl6)
        Me.groupTotales.Controls.Add(Me.lbl7)
        Me.groupTotales.Controls.Add(Me.lbl8)
        Me.groupTotales.Controls.Add(Me.lbl9)
        Me.groupTotales.Controls.Add(Me.lbl10)
        Me.groupTotales.Dock = System.Windows.Forms.DockStyle.Top
        Me.groupTotales.Location = New System.Drawing.Point(0, 180)
        Me.groupTotales.Name = "groupTotales"
        Me.groupTotales.Size = New System.Drawing.Size(1040, 105)
        Me.groupTotales.TabIndex = 1
        Me.groupTotales.Text = "Totales de la nómina seleccionada"
        '
        'TxtSalBasico
        '
        Me.TxtSalBasico.EditValue = "0.00"
        Me.TxtSalBasico.Location = New System.Drawing.Point(100, 38)
        Me.TxtSalBasico.Name = "TxtSalBasico"
        Me.TxtSalBasico.Properties.Mask.EditMask = "n2"
        Me.TxtSalBasico.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtSalBasico.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.TxtSalBasico.Properties.ReadOnly = True
        Me.TxtSalBasico.Size = New System.Drawing.Size(100, 20)
        Me.TxtSalBasico.TabIndex = 0
        '
        'TxtDestajo
        '
        Me.TxtDestajo.EditValue = "0.00"
        Me.TxtDestajo.Location = New System.Drawing.Point(305, 38)
        Me.TxtDestajo.Name = "TxtDestajo"
        Me.TxtDestajo.Properties.Mask.EditMask = "n2"
        Me.TxtDestajo.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtDestajo.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.TxtDestajo.Properties.ReadOnly = True
        Me.TxtDestajo.Size = New System.Drawing.Size(100, 20)
        Me.TxtDestajo.TabIndex = 1
        '
        'TxtHE
        '
        Me.TxtHE.EditValue = "0.00"
        Me.TxtHE.Location = New System.Drawing.Point(510, 38)
        Me.TxtHE.Name = "TxtHE"
        Me.TxtHE.Properties.Mask.EditMask = "n2"
        Me.TxtHE.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtHE.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.TxtHE.Properties.ReadOnly = True
        Me.TxtHE.Size = New System.Drawing.Size(100, 20)
        Me.TxtHE.TabIndex = 2
        '
        'TxtComisiones
        '
        Me.TxtComisiones.EditValue = "0.00"
        Me.TxtComisiones.Location = New System.Drawing.Point(715, 38)
        Me.TxtComisiones.Name = "TxtComisiones"
        Me.TxtComisiones.Properties.Mask.EditMask = "n2"
        Me.TxtComisiones.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtComisiones.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.TxtComisiones.Properties.ReadOnly = True
        Me.TxtComisiones.Size = New System.Drawing.Size(100, 20)
        Me.TxtComisiones.TabIndex = 3
        '
        'TxtOtIngre
        '
        Me.TxtOtIngre.EditValue = "0.00"
        Me.TxtOtIngre.Location = New System.Drawing.Point(920, 38)
        Me.TxtOtIngre.Name = "TxtOtIngre"
        Me.TxtOtIngre.Properties.Mask.EditMask = "n2"
        Me.TxtOtIngre.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtOtIngre.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.TxtOtIngre.Properties.ReadOnly = True
        Me.TxtOtIngre.Size = New System.Drawing.Size(100, 20)
        Me.TxtOtIngre.TabIndex = 4
        '
        'TxtIncentivos
        '
        Me.TxtIncentivos.EditValue = "0.00"
        Me.TxtIncentivos.Location = New System.Drawing.Point(100, 68)
        Me.TxtIncentivos.Name = "TxtIncentivos"
        Me.TxtIncentivos.Properties.Mask.EditMask = "n2"
        Me.TxtIncentivos.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtIncentivos.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.TxtIncentivos.Properties.ReadOnly = True
        Me.TxtIncentivos.Size = New System.Drawing.Size(100, 20)
        Me.TxtIncentivos.TabIndex = 5
        '
        'TxtDeducciones
        '
        Me.TxtDeducciones.EditValue = "0.00"
        Me.TxtDeducciones.Location = New System.Drawing.Point(305, 68)
        Me.TxtDeducciones.Name = "TxtDeducciones"
        Me.TxtDeducciones.Properties.Mask.EditMask = "n2"
        Me.TxtDeducciones.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtDeducciones.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.TxtDeducciones.Properties.ReadOnly = True
        Me.TxtDeducciones.Size = New System.Drawing.Size(100, 20)
        Me.TxtDeducciones.TabIndex = 6
        '
        'TxtPrestamo
        '
        Me.TxtPrestamo.EditValue = "0.00"
        Me.TxtPrestamo.Location = New System.Drawing.Point(510, 68)
        Me.TxtPrestamo.Name = "TxtPrestamo"
        Me.TxtPrestamo.Properties.Mask.EditMask = "n2"
        Me.TxtPrestamo.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtPrestamo.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.TxtPrestamo.Properties.ReadOnly = True
        Me.TxtPrestamo.Size = New System.Drawing.Size(100, 20)
        Me.TxtPrestamo.TabIndex = 7
        '
        'TxtInss
        '
        Me.TxtInss.EditValue = "0.00"
        Me.TxtInss.Location = New System.Drawing.Point(715, 68)
        Me.TxtInss.Name = "TxtInss"
        Me.TxtInss.Properties.Mask.EditMask = "n2"
        Me.TxtInss.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtInss.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.TxtInss.Properties.ReadOnly = True
        Me.TxtInss.Size = New System.Drawing.Size(100, 20)
        Me.TxtInss.TabIndex = 8
        '
        'TxtIR
        '
        Me.TxtIR.EditValue = "0.00"
        Me.TxtIR.Location = New System.Drawing.Point(920, 68)
        Me.TxtIR.Name = "TxtIR"
        Me.TxtIR.Properties.Mask.EditMask = "n2"
        Me.TxtIR.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtIR.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.TxtIR.Properties.ReadOnly = True
        Me.TxtIR.Size = New System.Drawing.Size(100, 20)
        Me.TxtIR.TabIndex = 9
        '
        'lbl1
        '
        Me.lbl1.Location = New System.Drawing.Point(15, 41)
        Me.lbl1.Name = "lbl1"
        Me.lbl1.Size = New System.Drawing.Size(65, 13)
        Me.lbl1.TabIndex = 10
        Me.lbl1.Text = "Salario Básico"
        '
        'lbl2
        '
        Me.lbl2.Location = New System.Drawing.Point(220, 41)
        Me.lbl2.Name = "lbl2"
        Me.lbl2.Size = New System.Drawing.Size(37, 13)
        Me.lbl2.TabIndex = 11
        Me.lbl2.Text = "Destajo"
        '
        'lbl3
        '
        Me.lbl3.Location = New System.Drawing.Point(425, 41)
        Me.lbl3.Name = "lbl3"
        Me.lbl3.Size = New System.Drawing.Size(62, 13)
        Me.lbl3.TabIndex = 12
        Me.lbl3.Text = "Horas Extras"
        '
        'lbl4
        '
        Me.lbl4.Location = New System.Drawing.Point(630, 41)
        Me.lbl4.Name = "lbl4"
        Me.lbl4.Size = New System.Drawing.Size(53, 13)
        Me.lbl4.TabIndex = 13
        Me.lbl4.Text = "Comisiones"
        '
        'lbl5
        '
        Me.lbl5.Location = New System.Drawing.Point(835, 41)
        Me.lbl5.Name = "lbl5"
        Me.lbl5.Size = New System.Drawing.Size(72, 13)
        Me.lbl5.TabIndex = 14
        Me.lbl5.Text = "Otros Ingresos"
        '
        'lbl6
        '
        Me.lbl6.Location = New System.Drawing.Point(15, 71)
        Me.lbl6.Name = "lbl6"
        Me.lbl6.Size = New System.Drawing.Size(50, 13)
        Me.lbl6.TabIndex = 15
        Me.lbl6.Text = "Incentivos"
        '
        'lbl7
        '
        Me.lbl7.Location = New System.Drawing.Point(220, 71)
        Me.lbl7.Name = "lbl7"
        Me.lbl7.Size = New System.Drawing.Size(60, 13)
        Me.lbl7.TabIndex = 16
        Me.lbl7.Text = "Deducciones"
        '
        'lbl8
        '
        Me.lbl8.Location = New System.Drawing.Point(425, 71)
        Me.lbl8.Name = "lbl8"
        Me.lbl8.Size = New System.Drawing.Size(50, 13)
        Me.lbl8.TabIndex = 17
        Me.lbl8.Text = "Préstamos"
        '
        'lbl9
        '
        Me.lbl9.Location = New System.Drawing.Point(630, 71)
        Me.lbl9.Name = "lbl9"
        Me.lbl9.Size = New System.Drawing.Size(23, 13)
        Me.lbl9.TabIndex = 18
        Me.lbl9.Text = "INSS"
        '
        'lbl10
        '
        Me.lbl10.Location = New System.Drawing.Point(835, 71)
        Me.lbl10.Name = "lbl10"
        Me.lbl10.Size = New System.Drawing.Size(11, 13)
        Me.lbl10.TabIndex = 19
        Me.lbl10.Text = "IR"
        '
        'gridDetalle
        '
        Me.gridDetalle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridDetalle.Location = New System.Drawing.Point(0, 285)
        Me.gridDetalle.MainView = Me.gridViewDetalle
        Me.gridDetalle.Name = "gridDetalle"
        Me.gridDetalle.Size = New System.Drawing.Size(1040, 181)
        Me.gridDetalle.TabIndex = 2
        Me.gridDetalle.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewDetalle})
        '
        'gridViewDetalle
        '
        Me.gridViewDetalle.GridControl = Me.gridDetalle
        Me.gridViewDetalle.Name = "gridViewDetalle"
        Me.gridViewDetalle.OptionsBehavior.Editable = False
        Me.gridViewDetalle.OptionsView.ShowGroupPanel = False
        '
        'FrmListNomina
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1040, 521)
        Me.Controls.Add(Me.gridDetalle)
        Me.Controls.Add(Me.groupTotales)
        Me.Controls.Add(Me.gridNominas)
        Me.Controls.Add(Me.panelBotones)
        Me.Name = "FrmListNomina"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Listado de Nóminas"
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        CType(Me.gridNominas, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewNominas, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.groupTotales, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupTotales.ResumeLayout(False)
        Me.groupTotales.PerformLayout()
        CType(Me.TxtSalBasico.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtDestajo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtHE.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtComisiones.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtOtIngre.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtIncentivos.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtDeducciones.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtPrestamo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtInss.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtIR.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridDetalle, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewDetalle, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnAnular As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnExcel As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents gridNominas As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewNominas As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents groupTotales As DevExpress.XtraEditors.GroupControl
    Friend WithEvents TxtSalBasico As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TxtDestajo As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TxtHE As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TxtComisiones As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TxtOtIngre As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TxtIncentivos As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TxtDeducciones As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TxtPrestamo As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TxtInss As DevExpress.XtraEditors.TextEdit
    Friend WithEvents TxtIR As DevExpress.XtraEditors.TextEdit
    Friend WithEvents lbl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl4 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl5 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl6 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl7 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl8 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl9 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl10 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents gridDetalle As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewDetalle As DevExpress.XtraGrid.Views.Grid.GridView
End Class
