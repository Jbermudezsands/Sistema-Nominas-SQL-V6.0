<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmDeduccionPorciento
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
        Me.BtnEliminar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnGuardar = New DevExpress.XtraEditors.SimpleButton()
        Me.Grid = New DevExpress.XtraGrid.GridControl()
        Me.Vista = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.ColRango1 = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.ColRango2 = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.ColPorciento = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.LblAyuda = New DevExpress.XtraEditors.LabelControl()
        CType(Me.PanelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelBotones.SuspendLayout()
        CType(Me.Grid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Vista, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PanelBotones
        '
        Me.PanelBotones.Controls.Add(Me.BtnSalir)
        Me.PanelBotones.Controls.Add(Me.BtnEliminar)
        Me.PanelBotones.Controls.Add(Me.BtnGuardar)
        Me.PanelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBotones.Name = "PanelBotones"
        Me.PanelBotones.Size = New System.Drawing.Size(560, 55)
        Me.PanelBotones.TabIndex = 1
        '
        'BtnGuardar
        '
        Me.BtnGuardar.Location = New System.Drawing.Point(13, 10)
        Me.BtnGuardar.Name = "BtnGuardar"
        Me.BtnGuardar.Size = New System.Drawing.Size(95, 35)
        Me.BtnGuardar.TabIndex = 0
        Me.BtnGuardar.Text = "Grabar"
        '
        'BtnEliminar
        '
        Me.BtnEliminar.Location = New System.Drawing.Point(119, 10)
        Me.BtnEliminar.Name = "BtnEliminar"
        Me.BtnEliminar.Size = New System.Drawing.Size(95, 35)
        Me.BtnEliminar.TabIndex = 1
        Me.BtnEliminar.Text = "Eliminar fila"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(225, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 2
        Me.BtnSalir.Text = "Salir"
        '
        'LblAyuda
        '
        Me.LblAyuda.Dock = System.Windows.Forms.DockStyle.Top
        Me.LblAyuda.Name = "LblAyuda"
        Me.LblAyuda.Padding = New System.Windows.Forms.Padding(8, 6, 8, 6)
        Me.LblAyuda.Size = New System.Drawing.Size(560, 25)
        Me.LblAyuda.TabIndex = 2
        Me.LblAyuda.Text = "Escriba en la fila vacía para agregar un rango. Presione Grabar para guardar los cambios."
        '
        'Grid
        '
        Me.Grid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Grid.MainView = Me.Vista
        Me.Grid.Name = "Grid"
        Me.Grid.TabIndex = 0
        Me.Grid.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.Vista})
        '
        'Vista
        '
        Me.Vista.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() {Me.ColRango1, Me.ColRango2, Me.ColPorciento})
        Me.Vista.GridControl = Me.Grid
        Me.Vista.Name = "Vista"
        Me.Vista.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top
        Me.Vista.OptionsView.ShowGroupPanel = False
        '
        'ColRango1
        '
        Me.ColRango1.Caption = "Desde"
        Me.ColRango1.DisplayFormat.FormatString = "N2"
        Me.ColRango1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.ColRango1.FieldName = "Rango1"
        Me.ColRango1.Name = "ColRango1"
        Me.ColRango1.Visible = True
        Me.ColRango1.VisibleIndex = 0
        '
        'ColRango2
        '
        Me.ColRango2.Caption = "Hasta"
        Me.ColRango2.DisplayFormat.FormatString = "N2"
        Me.ColRango2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.ColRango2.FieldName = "Rango2"
        Me.ColRango2.Name = "ColRango2"
        Me.ColRango2.Visible = True
        Me.ColRango2.VisibleIndex = 1
        '
        'ColPorciento
        '
        Me.ColPorciento.Caption = "Porciento"
        Me.ColPorciento.DisplayFormat.FormatString = "N2"
        Me.ColPorciento.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.ColPorciento.FieldName = "Porciento"
        Me.ColPorciento.Name = "ColPorciento"
        Me.ColPorciento.Visible = True
        Me.ColPorciento.VisibleIndex = 2
        '
        'FrmDeduccionPorciento
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(560, 380)
        Me.Controls.Add(Me.Grid)
        Me.Controls.Add(Me.LblAyuda)
        Me.Controls.Add(Me.PanelBotones)
        Me.Name = "FrmDeduccionPorciento"
        Me.Text = "Deducción por Porciento"
        CType(Me.PanelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelBotones.ResumeLayout(False)
        CType(Me.Grid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Vista, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnEliminar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnGuardar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents Grid As DevExpress.XtraGrid.GridControl
    Friend WithEvents Vista As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents ColRango1 As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents ColRango2 As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents ColPorciento As DevExpress.XtraGrid.Columns.GridColumn
    Friend WithEvents LblAyuda As DevExpress.XtraEditors.LabelControl
End Class
