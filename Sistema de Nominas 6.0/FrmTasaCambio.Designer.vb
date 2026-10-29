<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmTasaCambio
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
        Me.panelDatos = New DevExpress.XtraEditors.PanelControl()
        Me.LblHoy = New DevExpress.XtraEditors.LabelControl()
        Me.SpnMonto = New DevExpress.XtraEditors.SpinEdit()
        Me.LblMonto = New DevExpress.XtraEditors.LabelControl()
        Me.DtpFecha = New DevExpress.XtraEditors.DateEdit()
        Me.LblFecha = New DevExpress.XtraEditors.LabelControl()
        Me.panelBotones = New DevExpress.XtraEditors.PanelControl()
        Me.BtnNuevo = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnGrabar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnEliminar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.gridTasas = New DevExpress.XtraGrid.GridControl()
        Me.gridViewTasas = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.panelDatos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelDatos.SuspendLayout()
        CType(Me.SpnMonto.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtpFecha.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtpFecha.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        CType(Me.gridTasas, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewTasas, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelDatos
        '
        Me.panelDatos.Controls.Add(Me.LblHoy)
        Me.panelDatos.Controls.Add(Me.SpnMonto)
        Me.panelDatos.Controls.Add(Me.LblMonto)
        Me.panelDatos.Controls.Add(Me.DtpFecha)
        Me.panelDatos.Controls.Add(Me.LblFecha)
        Me.panelDatos.Dock = System.Windows.Forms.DockStyle.Top
        Me.panelDatos.Location = New System.Drawing.Point(0, 0)
        Me.panelDatos.Name = "panelDatos"
        Me.panelDatos.Size = New System.Drawing.Size(456, 84)
        Me.panelDatos.TabIndex = 0
        '
        'LblHoy
        '
        Me.LblHoy.Appearance.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold)
        Me.LblHoy.Appearance.Options.UseFont = True
        Me.LblHoy.Location = New System.Drawing.Point(13, 56)
        Me.LblHoy.Name = "LblHoy"
        Me.LblHoy.Size = New System.Drawing.Size(0, 13)
        Me.LblHoy.TabIndex = 4
        '
        'SpnMonto
        '
        Me.SpnMonto.EditValue = New Decimal(New Integer() {0, 0, 0, 0})
        Me.SpnMonto.Location = New System.Drawing.Point(316, 15)
        Me.SpnMonto.Name = "SpnMonto"
        Me.SpnMonto.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight)})
        Me.SpnMonto.Properties.DisplayFormat.FormatString = "n4"
        Me.SpnMonto.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.SpnMonto.Properties.EditFormat.FormatString = "n4"
        Me.SpnMonto.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        Me.SpnMonto.Properties.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.SpnMonto.Properties.MaxValue = New Decimal(New Integer() {100000, 0, 0, 0})
        Me.SpnMonto.Size = New System.Drawing.Size(110, 20)
        Me.SpnMonto.TabIndex = 3
        '
        'LblMonto
        '
        Me.LblMonto.Location = New System.Drawing.Point(248, 18)
        Me.LblMonto.Name = "LblMonto"
        Me.LblMonto.Size = New System.Drawing.Size(51, 13)
        Me.LblMonto.TabIndex = 2
        Me.LblMonto.Text = "Tasa (C$):"
        '
        'DtpFecha
        '
        Me.DtpFecha.EditValue = Nothing
        Me.DtpFecha.Location = New System.Drawing.Point(65, 15)
        Me.DtpFecha.Name = "DtpFecha"
        Me.DtpFecha.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DtpFecha.Properties.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DtpFecha.Size = New System.Drawing.Size(130, 20)
        Me.DtpFecha.TabIndex = 1
        '
        'LblFecha
        '
        Me.LblFecha.Location = New System.Drawing.Point(13, 18)
        Me.LblFecha.Name = "LblFecha"
        Me.LblFecha.Size = New System.Drawing.Size(33, 13)
        Me.LblFecha.TabIndex = 0
        Me.LblFecha.Text = "Fecha:"
        '
        'panelBotones
        '
        Me.panelBotones.Controls.Add(Me.BtnNuevo)
        Me.panelBotones.Controls.Add(Me.BtnGrabar)
        Me.panelBotones.Controls.Add(Me.BtnEliminar)
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Location = New System.Drawing.Point(0, 381)
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(456, 55)
        Me.panelBotones.TabIndex = 2
        '
        'BtnNuevo
        '
        Me.BtnNuevo.Location = New System.Drawing.Point(13, 10)
        Me.BtnNuevo.Name = "BtnNuevo"
        Me.BtnNuevo.Size = New System.Drawing.Size(95, 35)
        Me.BtnNuevo.TabIndex = 0
        Me.BtnNuevo.Text = "Nuevo"
        '
        'BtnGrabar
        '
        Me.BtnGrabar.Location = New System.Drawing.Point(119, 10)
        Me.BtnGrabar.Name = "BtnGrabar"
        Me.BtnGrabar.Size = New System.Drawing.Size(95, 35)
        Me.BtnGrabar.TabIndex = 1
        Me.BtnGrabar.Text = "Grabar"
        '
        'BtnEliminar
        '
        Me.BtnEliminar.Location = New System.Drawing.Point(225, 10)
        Me.BtnEliminar.Name = "BtnEliminar"
        Me.BtnEliminar.Size = New System.Drawing.Size(95, 35)
        Me.BtnEliminar.TabIndex = 2
        Me.BtnEliminar.Text = "Eliminar"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(331, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 3
        Me.BtnSalir.Text = "Salir"
        '
        'gridTasas
        '
        Me.gridTasas.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridTasas.Location = New System.Drawing.Point(0, 84)
        Me.gridTasas.MainView = Me.gridViewTasas
        Me.gridTasas.Name = "gridTasas"
        Me.gridTasas.Size = New System.Drawing.Size(456, 297)
        Me.gridTasas.TabIndex = 1
        Me.gridTasas.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewTasas})
        '
        'gridViewTasas
        '
        Me.gridViewTasas.GridControl = Me.gridTasas
        Me.gridViewTasas.Name = "gridViewTasas"
        Me.gridViewTasas.OptionsBehavior.Editable = False
        Me.gridViewTasas.OptionsView.ShowGroupPanel = False
        '
        'FrmTasaCambio
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(456, 436)
        Me.Controls.Add(Me.gridTasas)
        Me.Controls.Add(Me.panelBotones)
        Me.Controls.Add(Me.panelDatos)
        Me.Name = "FrmTasaCambio"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Tasa de Cambio"
        CType(Me.panelDatos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelDatos.ResumeLayout(False)
        Me.panelDatos.PerformLayout()
        CType(Me.SpnMonto.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtpFecha.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtpFecha.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        CType(Me.gridTasas, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewTasas, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents panelDatos As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LblHoy As DevExpress.XtraEditors.LabelControl
    Friend WithEvents SpnMonto As DevExpress.XtraEditors.SpinEdit
    Friend WithEvents LblMonto As DevExpress.XtraEditors.LabelControl
    Friend WithEvents DtpFecha As DevExpress.XtraEditors.DateEdit
    Friend WithEvents LblFecha As DevExpress.XtraEditors.LabelControl
    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnNuevo As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnGrabar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnEliminar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents gridTasas As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewTasas As DevExpress.XtraGrid.Views.Grid.GridView
End Class
