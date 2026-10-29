<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmHistorial
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
        Me.panelEmpleado = New DevExpress.XtraEditors.PanelControl()
        Me.LblEstado = New DevExpress.XtraEditors.LabelControl()
        Me.LblNombre = New DevExpress.XtraEditors.LabelControl()
        Me.cmbEmpleado = New DevExpress.XtraEditors.SearchLookUpEdit()
        Me.cmbEmpleadoView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.LblEmpleado = New DevExpress.XtraEditors.LabelControl()
        Me.panelBotones = New DevExpress.XtraEditors.PanelControl()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnExcel = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnDuplicados = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnCopiar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnDesactivar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnActivar = New DevExpress.XtraEditors.SimpleButton()
        Me.gridHistorial = New DevExpress.XtraGrid.GridControl()
        Me.gridViewHistorial = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.panelEmpleado, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelEmpleado.SuspendLayout()
        CType(Me.cmbEmpleado.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbEmpleadoView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        CType(Me.gridHistorial, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewHistorial, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelEmpleado
        '
        Me.panelEmpleado.Controls.Add(Me.LblEstado)
        Me.panelEmpleado.Controls.Add(Me.LblNombre)
        Me.panelEmpleado.Controls.Add(Me.cmbEmpleado)
        Me.panelEmpleado.Controls.Add(Me.LblEmpleado)
        Me.panelEmpleado.Dock = System.Windows.Forms.DockStyle.Top
        Me.panelEmpleado.Location = New System.Drawing.Point(0, 0)
        Me.panelEmpleado.Name = "panelEmpleado"
        Me.panelEmpleado.Size = New System.Drawing.Size(1000, 50)
        Me.panelEmpleado.TabIndex = 0
        '
        'LblEstado
        '
        Me.LblEstado.Location = New System.Drawing.Point(700, 18)
        Me.LblEstado.Name = "LblEstado"
        Me.LblEstado.Size = New System.Drawing.Size(0, 13)
        Me.LblEstado.TabIndex = 3
        '
        'LblNombre
        '
        Me.LblNombre.Location = New System.Drawing.Point(225, 18)
        Me.LblNombre.Name = "LblNombre"
        Me.LblNombre.Size = New System.Drawing.Size(0, 13)
        Me.LblNombre.TabIndex = 2
        '
        'cmbEmpleado
        '
        Me.cmbEmpleado.Location = New System.Drawing.Point(75, 15)
        Me.cmbEmpleado.Name = "cmbEmpleado"
        Me.cmbEmpleado.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cmbEmpleado.Properties.NullText = ""
        Me.cmbEmpleado.Properties.PopupView = Me.cmbEmpleadoView
        Me.cmbEmpleado.Size = New System.Drawing.Size(130, 20)
        Me.cmbEmpleado.TabIndex = 1
        '
        'cmbEmpleadoView
        '
        Me.cmbEmpleadoView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.cmbEmpleadoView.Name = "cmbEmpleadoView"
        Me.cmbEmpleadoView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.cmbEmpleadoView.OptionsView.ShowGroupPanel = False
        '
        'LblEmpleado
        '
        Me.LblEmpleado.Location = New System.Drawing.Point(13, 18)
        Me.LblEmpleado.Name = "LblEmpleado"
        Me.LblEmpleado.Size = New System.Drawing.Size(50, 13)
        Me.LblEmpleado.TabIndex = 0
        Me.LblEmpleado.Text = "Empleado:"
        '
        'panelBotones
        '
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Controls.Add(Me.BtnExcel)
        Me.panelBotones.Controls.Add(Me.BtnDuplicados)
        Me.panelBotones.Controls.Add(Me.BtnCopiar)
        Me.panelBotones.Controls.Add(Me.BtnDesactivar)
        Me.panelBotones.Controls.Add(Me.BtnActivar)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Location = New System.Drawing.Point(0, 384)
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(1000, 55)
        Me.panelBotones.TabIndex = 2
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(560, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 5
        Me.BtnSalir.Text = "Salir"
        '
        'BtnExcel
        '
        Me.BtnExcel.Location = New System.Drawing.Point(455, 10)
        Me.BtnExcel.Name = "BtnExcel"
        Me.BtnExcel.Size = New System.Drawing.Size(95, 35)
        Me.BtnExcel.TabIndex = 4
        Me.BtnExcel.Text = "Excel"
        '
        'BtnDuplicados
        '
        Me.BtnDuplicados.Location = New System.Drawing.Point(350, 10)
        Me.BtnDuplicados.Name = "BtnDuplicados"
        Me.BtnDuplicados.Size = New System.Drawing.Size(95, 35)
        Me.BtnDuplicados.TabIndex = 3
        Me.BtnDuplicados.Text = "Duplicados"
        '
        'BtnCopiar
        '
        Me.BtnCopiar.Location = New System.Drawing.Point(245, 10)
        Me.BtnCopiar.Name = "BtnCopiar"
        Me.BtnCopiar.Size = New System.Drawing.Size(95, 35)
        Me.BtnCopiar.TabIndex = 2
        Me.BtnCopiar.Text = "Copiar perfil"
        '
        'BtnDesactivar
        '
        Me.BtnDesactivar.Location = New System.Drawing.Point(129, 10)
        Me.BtnDesactivar.Name = "BtnDesactivar"
        Me.BtnDesactivar.Size = New System.Drawing.Size(106, 35)
        Me.BtnDesactivar.TabIndex = 1
        Me.BtnDesactivar.Text = "Desactivar"
        '
        'BtnActivar
        '
        Me.BtnActivar.Location = New System.Drawing.Point(13, 10)
        Me.BtnActivar.Name = "BtnActivar"
        Me.BtnActivar.Size = New System.Drawing.Size(106, 35)
        Me.BtnActivar.TabIndex = 0
        Me.BtnActivar.Text = "Activar"
        '
        'gridHistorial
        '
        Me.gridHistorial.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridHistorial.Location = New System.Drawing.Point(0, 50)
        Me.gridHistorial.MainView = Me.gridViewHistorial
        Me.gridHistorial.Name = "gridHistorial"
        Me.gridHistorial.Size = New System.Drawing.Size(1000, 334)
        Me.gridHistorial.TabIndex = 1
        Me.gridHistorial.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewHistorial})
        '
        'gridViewHistorial
        '
        Me.gridViewHistorial.GridControl = Me.gridHistorial
        Me.gridViewHistorial.Name = "gridViewHistorial"
        Me.gridViewHistorial.OptionsBehavior.Editable = False
        Me.gridViewHistorial.OptionsView.ColumnAutoWidth = False
        Me.gridViewHistorial.OptionsView.ShowFooter = True
        Me.gridViewHistorial.OptionsView.ShowGroupPanel = False
        '
        'FrmHistorial
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1000, 439)
        Me.Controls.Add(Me.gridHistorial)
        Me.Controls.Add(Me.panelBotones)
        Me.Controls.Add(Me.panelEmpleado)
        Me.Name = "FrmHistorial"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Historial del Empleado"
        CType(Me.panelEmpleado, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelEmpleado.ResumeLayout(False)
        Me.panelEmpleado.PerformLayout()
        CType(Me.cmbEmpleado.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmbEmpleadoView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        CType(Me.gridHistorial, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewHistorial, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents panelEmpleado As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LblEstado As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LblNombre As DevExpress.XtraEditors.LabelControl
    Friend WithEvents cmbEmpleado As DevExpress.XtraEditors.SearchLookUpEdit
    Friend WithEvents cmbEmpleadoView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents LblEmpleado As DevExpress.XtraEditors.LabelControl
    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnExcel As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnDuplicados As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnCopiar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnDesactivar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnActivar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents gridHistorial As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewHistorial As DevExpress.XtraGrid.Views.Grid.GridView
End Class
