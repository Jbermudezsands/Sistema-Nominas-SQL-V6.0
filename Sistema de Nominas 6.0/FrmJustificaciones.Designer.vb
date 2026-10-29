<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmJustificaciones
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
        Me.TxtNombre = New DevExpress.XtraEditors.TextEdit()
        Me.cmbEmpleado = New DevExpress.XtraEditors.SearchLookUpEdit()
        Me.cmbEmpleadoView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.LblEmpleado = New DevExpress.XtraEditors.LabelControl()
        Me.groupNueva = New DevExpress.XtraEditors.GroupControl()
        Me.BtnAgregar = New DevExpress.XtraEditors.SimpleButton()
        Me.TxtMotivo = New DevExpress.XtraEditors.TextEdit()
        Me.LblMotivo = New DevExpress.XtraEditors.LabelControl()
        Me.BtnTipo = New DevExpress.XtraEditors.SimpleButton()
        Me.CmbTipo = New DevExpress.XtraEditors.LookUpEdit()
        Me.LblTipo = New DevExpress.XtraEditors.LabelControl()
        Me.TmeFin = New DevExpress.XtraEditors.TimeEdit()
        Me.DtpFin = New DevExpress.XtraEditors.DateEdit()
        Me.LblFin = New DevExpress.XtraEditors.LabelControl()
        Me.TmeInicio = New DevExpress.XtraEditors.TimeEdit()
        Me.DtpInicio = New DevExpress.XtraEditors.DateEdit()
        Me.LblInicio = New DevExpress.XtraEditors.LabelControl()
        Me.panelBotones = New DevExpress.XtraEditors.PanelControl()
        Me.BtnEliminar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnExcel = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.gridJustificaciones = New DevExpress.XtraGrid.GridControl()
        Me.gridViewJustificaciones = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.panelEmpleado, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelEmpleado.SuspendLayout()
        CType(Me.TxtNombre.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbEmpleado.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbEmpleadoView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.groupNueva, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupNueva.SuspendLayout()
        CType(Me.TxtMotivo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CmbTipo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TmeFin.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtpFin.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtpFin.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TmeInicio.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtpInicio.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtpInicio.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        CType(Me.gridJustificaciones, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewJustificaciones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelEmpleado
        '
        Me.panelEmpleado.Controls.Add(Me.TxtNombre)
        Me.panelEmpleado.Controls.Add(Me.cmbEmpleado)
        Me.panelEmpleado.Controls.Add(Me.LblEmpleado)
        Me.panelEmpleado.Dock = System.Windows.Forms.DockStyle.Top
        Me.panelEmpleado.Location = New System.Drawing.Point(0, 0)
        Me.panelEmpleado.Name = "panelEmpleado"
        Me.panelEmpleado.Size = New System.Drawing.Size(720, 50)
        Me.panelEmpleado.TabIndex = 0
        '
        'TxtNombre
        '
        Me.TxtNombre.Location = New System.Drawing.Point(215, 15)
        Me.TxtNombre.Name = "TxtNombre"
        Me.TxtNombre.Properties.ReadOnly = True
        Me.TxtNombre.Size = New System.Drawing.Size(490, 20)
        Me.TxtNombre.TabIndex = 2
        Me.TxtNombre.TabStop = False
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
        'groupNueva
        '
        Me.groupNueva.Controls.Add(Me.BtnAgregar)
        Me.groupNueva.Controls.Add(Me.TxtMotivo)
        Me.groupNueva.Controls.Add(Me.LblMotivo)
        Me.groupNueva.Controls.Add(Me.BtnTipo)
        Me.groupNueva.Controls.Add(Me.CmbTipo)
        Me.groupNueva.Controls.Add(Me.LblTipo)
        Me.groupNueva.Controls.Add(Me.TmeFin)
        Me.groupNueva.Controls.Add(Me.DtpFin)
        Me.groupNueva.Controls.Add(Me.LblFin)
        Me.groupNueva.Controls.Add(Me.TmeInicio)
        Me.groupNueva.Controls.Add(Me.DtpInicio)
        Me.groupNueva.Controls.Add(Me.LblInicio)
        Me.groupNueva.Dock = System.Windows.Forms.DockStyle.Top
        Me.groupNueva.Location = New System.Drawing.Point(0, 50)
        Me.groupNueva.Name = "groupNueva"
        Me.groupNueva.Size = New System.Drawing.Size(720, 175)
        Me.groupNueva.TabIndex = 1
        Me.groupNueva.Text = "Nueva justificación"
        '
        'BtnAgregar
        '
        Me.BtnAgregar.Location = New System.Drawing.Point(75, 130)
        Me.BtnAgregar.Name = "BtnAgregar"
        Me.BtnAgregar.Size = New System.Drawing.Size(95, 35)
        Me.BtnAgregar.TabIndex = 8
        Me.BtnAgregar.Text = "Guardar"
        '
        'TxtMotivo
        '
        Me.TxtMotivo.Location = New System.Drawing.Point(75, 98)
        Me.TxtMotivo.Name = "TxtMotivo"
        Me.TxtMotivo.Properties.MaxLength = 250
        Me.TxtMotivo.Size = New System.Drawing.Size(630, 20)
        Me.TxtMotivo.TabIndex = 7
        '
        'LblMotivo
        '
        Me.LblMotivo.Location = New System.Drawing.Point(13, 101)
        Me.LblMotivo.Name = "LblMotivo"
        Me.LblMotivo.Size = New System.Drawing.Size(36, 13)
        Me.LblMotivo.TabIndex = 11
        Me.LblMotivo.Text = "Motivo:"
        '
        'BtnTipo
        '
        Me.BtnTipo.Location = New System.Drawing.Point(395, 65)
        Me.BtnTipo.Name = "BtnTipo"
        Me.BtnTipo.Size = New System.Drawing.Size(95, 22)
        Me.BtnTipo.TabIndex = 6
        Me.BtnTipo.Text = "Nuevo tipo..."
        '
        'CmbTipo
        '
        Me.CmbTipo.Location = New System.Drawing.Point(75, 66)
        Me.CmbTipo.Name = "CmbTipo"
        Me.CmbTipo.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.CmbTipo.Properties.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Classname", "Tipo")})
        Me.CmbTipo.Properties.DisplayMember = "Classname"
        Me.CmbTipo.Properties.NullText = ""
        Me.CmbTipo.Properties.ShowHeader = False
        Me.CmbTipo.Properties.ValueMember = "Classid"
        Me.CmbTipo.Size = New System.Drawing.Size(310, 20)
        Me.CmbTipo.TabIndex = 5
        '
        'LblTipo
        '
        Me.LblTipo.Location = New System.Drawing.Point(13, 69)
        Me.LblTipo.Name = "LblTipo"
        Me.LblTipo.Size = New System.Drawing.Size(24, 13)
        Me.LblTipo.TabIndex = 10
        Me.LblTipo.Text = "Tipo:"
        '
        'TmeFin
        '
        Me.TmeFin.EditValue = New Date(2000, 1, 1, 0, 0, 0, 0)
        Me.TmeFin.Location = New System.Drawing.Point(470, 34)
        Me.TmeFin.Name = "TmeFin"
        Me.TmeFin.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.TmeFin.Properties.Mask.EditMask = "HH:mm"
        Me.TmeFin.Size = New System.Drawing.Size(75, 20)
        Me.TmeFin.TabIndex = 4
        '
        'DtpFin
        '
        Me.DtpFin.EditValue = Nothing
        Me.DtpFin.Location = New System.Drawing.Point(340, 34)
        Me.DtpFin.Name = "DtpFin"
        Me.DtpFin.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DtpFin.Properties.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DtpFin.Size = New System.Drawing.Size(120, 20)
        Me.DtpFin.TabIndex = 3
        '
        'LblFin
        '
        Me.LblFin.Location = New System.Drawing.Point(305, 37)
        Me.LblFin.Name = "LblFin"
        Me.LblFin.Size = New System.Drawing.Size(18, 13)
        Me.LblFin.TabIndex = 9
        Me.LblFin.Text = "Fin:"
        '
        'TmeInicio
        '
        Me.TmeInicio.EditValue = New Date(2000, 1, 1, 0, 0, 0, 0)
        Me.TmeInicio.Location = New System.Drawing.Point(205, 34)
        Me.TmeInicio.Name = "TmeInicio"
        Me.TmeInicio.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.TmeInicio.Properties.Mask.EditMask = "HH:mm"
        Me.TmeInicio.Size = New System.Drawing.Size(75, 20)
        Me.TmeInicio.TabIndex = 2
        '
        'DtpInicio
        '
        Me.DtpInicio.EditValue = Nothing
        Me.DtpInicio.Location = New System.Drawing.Point(75, 34)
        Me.DtpInicio.Name = "DtpInicio"
        Me.DtpInicio.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DtpInicio.Properties.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DtpInicio.Size = New System.Drawing.Size(120, 20)
        Me.DtpInicio.TabIndex = 1
        '
        'LblInicio
        '
        Me.LblInicio.Location = New System.Drawing.Point(13, 37)
        Me.LblInicio.Name = "LblInicio"
        Me.LblInicio.Size = New System.Drawing.Size(29, 13)
        Me.LblInicio.TabIndex = 0
        Me.LblInicio.Text = "Inicio:"
        '
        'panelBotones
        '
        Me.panelBotones.Controls.Add(Me.BtnEliminar)
        Me.panelBotones.Controls.Add(Me.BtnExcel)
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Location = New System.Drawing.Point(0, 414)
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(720, 56)
        Me.panelBotones.TabIndex = 3
        '
        'BtnEliminar
        '
        Me.BtnEliminar.Location = New System.Drawing.Point(13, 10)
        Me.BtnEliminar.Name = "BtnEliminar"
        Me.BtnEliminar.Size = New System.Drawing.Size(95, 35)
        Me.BtnEliminar.TabIndex = 0
        Me.BtnEliminar.Text = "Eliminar"
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
        'gridJustificaciones
        '
        Me.gridJustificaciones.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridJustificaciones.Location = New System.Drawing.Point(0, 225)
        Me.gridJustificaciones.MainView = Me.gridViewJustificaciones
        Me.gridJustificaciones.Name = "gridJustificaciones"
        Me.gridJustificaciones.Size = New System.Drawing.Size(720, 189)
        Me.gridJustificaciones.TabIndex = 2
        Me.gridJustificaciones.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewJustificaciones})
        '
        'gridViewJustificaciones
        '
        Me.gridViewJustificaciones.GridControl = Me.gridJustificaciones
        Me.gridViewJustificaciones.Name = "gridViewJustificaciones"
        Me.gridViewJustificaciones.OptionsBehavior.Editable = False
        Me.gridViewJustificaciones.OptionsView.ShowGroupPanel = False
        '
        'FrmJustificaciones
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(720, 470)
        Me.Controls.Add(Me.gridJustificaciones)
        Me.Controls.Add(Me.panelBotones)
        Me.Controls.Add(Me.groupNueva)
        Me.Controls.Add(Me.panelEmpleado)
        Me.Name = "FrmJustificaciones"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Justificaciones de Asistencia"
        CType(Me.panelEmpleado, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelEmpleado.ResumeLayout(False)
        Me.panelEmpleado.PerformLayout()
        CType(Me.TxtNombre.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmbEmpleado.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmbEmpleadoView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.groupNueva, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupNueva.ResumeLayout(False)
        Me.groupNueva.PerformLayout()
        CType(Me.TxtMotivo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CmbTipo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TmeFin.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtpFin.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtpFin.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TmeInicio.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtpInicio.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtpInicio.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        CType(Me.gridJustificaciones, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewJustificaciones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents panelEmpleado As DevExpress.XtraEditors.PanelControl
    Friend WithEvents TxtNombre As DevExpress.XtraEditors.TextEdit
    Friend WithEvents cmbEmpleado As DevExpress.XtraEditors.SearchLookUpEdit
    Friend WithEvents cmbEmpleadoView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents LblEmpleado As DevExpress.XtraEditors.LabelControl
    Friend WithEvents groupNueva As DevExpress.XtraEditors.GroupControl
    Friend WithEvents BtnAgregar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents TxtMotivo As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LblMotivo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents BtnTipo As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents CmbTipo As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents LblTipo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TmeFin As DevExpress.XtraEditors.TimeEdit
    Friend WithEvents DtpFin As DevExpress.XtraEditors.DateEdit
    Friend WithEvents LblFin As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TmeInicio As DevExpress.XtraEditors.TimeEdit
    Friend WithEvents DtpInicio As DevExpress.XtraEditors.DateEdit
    Friend WithEvents LblInicio As DevExpress.XtraEditors.LabelControl
    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnEliminar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnExcel As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents gridJustificaciones As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewJustificaciones As DevExpress.XtraGrid.Views.Grid.GridView
End Class
