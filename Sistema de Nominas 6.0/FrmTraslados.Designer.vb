<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmTraslados
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
        Me.BtnTrasladar = New DevExpress.XtraEditors.SimpleButton()
        Me.gpDatos = New DevExpress.XtraEditors.GroupControl()
        Me.BtnConsultar = New DevExpress.XtraEditors.SimpleButton()
        Me.DtHasta = New DevExpress.XtraEditors.DateEdit()
        Me.DtDesde = New DevExpress.XtraEditors.DateEdit()
        Me.CmbDestino = New DevExpress.XtraEditors.SearchLookUpEdit()
        Me.CmbDestinoView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.CmbOrigen = New DevExpress.XtraEditors.SearchLookUpEdit()
        Me.CmbOrigenView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.lbl4 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl3 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl2 = New DevExpress.XtraEditors.LabelControl()
        Me.lbl1 = New DevExpress.XtraEditors.LabelControl()
        Me.Grid = New DevExpress.XtraGrid.GridControl()
        Me.Vista = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.PanelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelBotones.SuspendLayout()
        CType(Me.gpDatos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gpDatos.SuspendLayout()
        CType(Me.DtHasta.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtHasta.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtDesde.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtDesde.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CmbDestino.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CmbDestinoView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CmbOrigen.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CmbOrigenView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Grid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Vista, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelBotones
        '
        Me.PanelBotones.Controls.Add(Me.BtnSalir)
        Me.PanelBotones.Controls.Add(Me.BtnTrasladar)
        Me.PanelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBotones.Name = "PanelBotones"
        Me.PanelBotones.Size = New System.Drawing.Size(900, 55)
        Me.PanelBotones.TabIndex = 2
        '
        'BtnTrasladar
        '
        Me.BtnTrasladar.Location = New System.Drawing.Point(13, 10)
        Me.BtnTrasladar.Name = "BtnTrasladar"
        Me.BtnTrasladar.Size = New System.Drawing.Size(105, 35)
        Me.BtnTrasladar.TabIndex = 0
        Me.BtnTrasladar.Text = "Trasladar"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(130, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 1
        Me.BtnSalir.Text = "Salir"
        '
        'gpDatos
        '
        Me.gpDatos.Controls.Add(Me.BtnConsultar)
        Me.gpDatos.Controls.Add(Me.DtHasta)
        Me.gpDatos.Controls.Add(Me.DtDesde)
        Me.gpDatos.Controls.Add(Me.CmbDestino)
        Me.gpDatos.Controls.Add(Me.CmbOrigen)
        Me.gpDatos.Controls.Add(Me.lbl4)
        Me.gpDatos.Controls.Add(Me.lbl3)
        Me.gpDatos.Controls.Add(Me.lbl2)
        Me.gpDatos.Controls.Add(Me.lbl1)
        Me.gpDatos.Dock = System.Windows.Forms.DockStyle.Top
        Me.gpDatos.Name = "gpDatos"
        Me.gpDatos.Size = New System.Drawing.Size(900, 130)
        Me.gpDatos.TabIndex = 0
        Me.gpDatos.Text = "Traslado de historial de pagos"
        '
        'lbl1
        '
        Me.lbl1.Location = New System.Drawing.Point(15, 38)
        Me.lbl1.Name = "lbl1"
        Me.lbl1.Text = "Empleado origen:"
        '
        'lbl2
        '
        Me.lbl2.Location = New System.Drawing.Point(15, 68)
        Me.lbl2.Name = "lbl2"
        Me.lbl2.Text = "Empleado destino:"
        '
        'lbl3
        '
        Me.lbl3.Location = New System.Drawing.Point(15, 98)
        Me.lbl3.Name = "lbl3"
        Me.lbl3.Text = "Desde:"
        '
        'lbl4
        '
        Me.lbl4.Location = New System.Drawing.Point(255, 98)
        Me.lbl4.Name = "lbl4"
        Me.lbl4.Text = "Hasta:"
        '
        'CmbOrigen
        '
        Me.CmbOrigen.Location = New System.Drawing.Point(120, 35)
        Me.CmbOrigen.Name = "CmbOrigen"
        Me.CmbOrigen.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.CmbOrigen.Properties.DisplayMember = "Nombres"
        Me.CmbOrigen.Properties.NullText = ""
        Me.CmbOrigen.Properties.PopupView = Me.CmbOrigenView
        Me.CmbOrigen.Properties.ValueMember = "CodEmpleado"
        Me.CmbOrigen.Size = New System.Drawing.Size(450, 20)
        Me.CmbOrigen.TabIndex = 0
        '
        'CmbOrigenView
        '
        Me.CmbOrigenView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.CmbOrigenView.Name = "CmbOrigenView"
        Me.CmbOrigenView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.CmbOrigenView.OptionsView.ShowGroupPanel = False
        '
        'CmbDestino
        '
        Me.CmbDestino.Location = New System.Drawing.Point(120, 65)
        Me.CmbDestino.Name = "CmbDestino"
        Me.CmbDestino.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.CmbDestino.Properties.DisplayMember = "Nombres"
        Me.CmbDestino.Properties.NullText = ""
        Me.CmbDestino.Properties.PopupView = Me.CmbDestinoView
        Me.CmbDestino.Properties.ValueMember = "CodEmpleado"
        Me.CmbDestino.Size = New System.Drawing.Size(450, 20)
        Me.CmbDestino.TabIndex = 1
        '
        'CmbDestinoView
        '
        Me.CmbDestinoView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.CmbDestinoView.Name = "CmbDestinoView"
        Me.CmbDestinoView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.CmbDestinoView.OptionsView.ShowGroupPanel = False
        '
        'DtDesde
        '
        Me.DtDesde.EditValue = Nothing
        Me.DtDesde.Location = New System.Drawing.Point(120, 95)
        Me.DtDesde.Name = "DtDesde"
        Me.DtDesde.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DtDesde.Properties.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.DtDesde.Size = New System.Drawing.Size(120, 20)
        Me.DtDesde.TabIndex = 2
        '
        'DtHasta
        '
        Me.DtHasta.EditValue = Nothing
        Me.DtHasta.Location = New System.Drawing.Point(300, 95)
        Me.DtHasta.Name = "DtHasta"
        Me.DtHasta.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DtHasta.Properties.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton()})
        Me.DtHasta.Size = New System.Drawing.Size(120, 20)
        Me.DtHasta.TabIndex = 3
        '
        'BtnConsultar
        '
        Me.BtnConsultar.Location = New System.Drawing.Point(440, 88)
        Me.BtnConsultar.Name = "BtnConsultar"
        Me.BtnConsultar.Size = New System.Drawing.Size(130, 30)
        Me.BtnConsultar.TabIndex = 4
        Me.BtnConsultar.Text = "Consultar historial"
        '
        'Grid
        '
        Me.Grid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Grid.MainView = Me.Vista
        Me.Grid.Name = "Grid"
        Me.Grid.TabIndex = 1
        Me.Grid.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.Vista})
        '
        'Vista
        '
        Me.Vista.GridControl = Me.Grid
        Me.Vista.Name = "Vista"
        Me.Vista.OptionsBehavior.Editable = False
        Me.Vista.OptionsView.ColumnAutoWidth = False
        Me.Vista.OptionsView.ShowGroupPanel = False
        '
        'FrmTraslados
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 520)
        Me.Controls.Add(Me.Grid)
        Me.Controls.Add(Me.gpDatos)
        Me.Controls.Add(Me.PanelBotones)
        Me.Name = "FrmTraslados"
        Me.Text = "Traslado de Historial"
        CType(Me.PanelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelBotones.ResumeLayout(False)
        CType(Me.gpDatos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gpDatos.ResumeLayout(False)
        Me.gpDatos.PerformLayout()
        CType(Me.DtHasta.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtHasta.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtDesde.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtDesde.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CmbDestino.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CmbDestinoView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CmbOrigen.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CmbOrigenView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Grid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Vista, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnTrasladar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents gpDatos As DevExpress.XtraEditors.GroupControl
    Friend WithEvents BtnConsultar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents DtHasta As DevExpress.XtraEditors.DateEdit
    Friend WithEvents DtDesde As DevExpress.XtraEditors.DateEdit
    Friend WithEvents CmbDestino As DevExpress.XtraEditors.SearchLookUpEdit
    Friend WithEvents CmbDestinoView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents CmbOrigen As DevExpress.XtraEditors.SearchLookUpEdit
    Friend WithEvents CmbOrigenView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents lbl4 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents lbl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents Grid As DevExpress.XtraGrid.GridControl
    Friend WithEvents Vista As DevExpress.XtraGrid.Views.Grid.GridView
End Class
