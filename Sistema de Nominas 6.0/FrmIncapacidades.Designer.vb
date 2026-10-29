<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmIncapacidades
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
        Me.BtnAgregar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnEliminar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnAnterior = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSiguiente = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.groupEmpleado = New DevExpress.XtraEditors.GroupControl()
        Me.TxtNombre = New System.Windows.Forms.TextBox()
        Me.cmbEmpleado = New DevExpress.XtraEditors.SearchLookUpEdit()
        Me.cmbEmpleadoView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.labelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl1 = New DevExpress.XtraEditors.LabelControl()
        Me.groupIncapacidades = New DevExpress.XtraEditors.GroupControl()
        Me.dtFecha = New DevExpress.XtraEditors.DateEdit()
        Me.cmbTipo = New DevExpress.XtraEditors.LookUpEdit()
        Me.labelControl4 = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.gridIncapacidades = New DevExpress.XtraGrid.GridControl()
        Me.gridViewIncapacidades = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.groupEmpleado, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupEmpleado.SuspendLayout()
        CType(Me.cmbEmpleado.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbEmpleadoView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.groupIncapacidades, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupIncapacidades.SuspendLayout()
        CType(Me.dtFecha.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dtFecha.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbTipo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridIncapacidades, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewIncapacidades, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'BtnAgregar
        '
        Me.BtnAgregar.Location = New System.Drawing.Point(13, 400)
        Me.BtnAgregar.Name = "BtnAgregar"
        Me.BtnAgregar.Size = New System.Drawing.Size(95, 35)
        Me.BtnAgregar.TabIndex = 2
        Me.BtnAgregar.Text = "Agregar"
        '
        'BtnEliminar
        '
        Me.BtnEliminar.Location = New System.Drawing.Point(119, 400)
        Me.BtnEliminar.Name = "BtnEliminar"
        Me.BtnEliminar.Size = New System.Drawing.Size(95, 35)
        Me.BtnEliminar.TabIndex = 3
        Me.BtnEliminar.Text = "Eliminar"
        '
        'BtnAnterior
        '
        Me.BtnAnterior.Location = New System.Drawing.Point(340, 400)
        Me.BtnAnterior.Name = "BtnAnterior"
        Me.BtnAnterior.Size = New System.Drawing.Size(95, 35)
        Me.BtnAnterior.TabIndex = 4
        Me.BtnAnterior.Text = "Anterior"
        '
        'BtnSiguiente
        '
        Me.BtnSiguiente.Location = New System.Drawing.Point(441, 400)
        Me.BtnSiguiente.Name = "BtnSiguiente"
        Me.BtnSiguiente.Size = New System.Drawing.Size(95, 35)
        Me.BtnSiguiente.TabIndex = 5
        Me.BtnSiguiente.Text = "Siguiente"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(552, 400)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 6
        Me.BtnSalir.Text = "Salir"
        '
        'groupEmpleado
        '
        Me.groupEmpleado.Controls.Add(Me.TxtNombre)
        Me.groupEmpleado.Controls.Add(Me.cmbEmpleado)
        Me.groupEmpleado.Controls.Add(Me.labelControl2)
        Me.groupEmpleado.Controls.Add(Me.labelControl1)
        Me.groupEmpleado.Location = New System.Drawing.Point(12, 12)
        Me.groupEmpleado.Name = "groupEmpleado"
        Me.groupEmpleado.Size = New System.Drawing.Size(635, 70)
        Me.groupEmpleado.TabIndex = 0
        Me.groupEmpleado.Text = "Empleado"
        '
        'TxtNombre
        '
        Me.TxtNombre.Location = New System.Drawing.Point(285, 35)
        Me.TxtNombre.Name = "TxtNombre"
        Me.TxtNombre.ReadOnly = True
        Me.TxtNombre.Size = New System.Drawing.Size(335, 21)
        Me.TxtNombre.TabIndex = 1
        Me.TxtNombre.TabStop = False
        '
        'cmbEmpleado
        '
        Me.cmbEmpleado.Location = New System.Drawing.Point(110, 35)
        Me.cmbEmpleado.Name = "cmbEmpleado"
        Me.cmbEmpleado.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cmbEmpleado.Properties.NullText = ""
        Me.cmbEmpleado.Properties.PopupView = Me.cmbEmpleadoView
        Me.cmbEmpleado.Size = New System.Drawing.Size(100, 20)
        Me.cmbEmpleado.TabIndex = 0
        '
        'cmbEmpleadoView
        '
        Me.cmbEmpleadoView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.cmbEmpleadoView.Name = "cmbEmpleadoView"
        Me.cmbEmpleadoView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.cmbEmpleadoView.OptionsView.ShowGroupPanel = False
        '
        'labelControl2
        '
        Me.labelControl2.Location = New System.Drawing.Point(225, 38)
        Me.labelControl2.Name = "labelControl2"
        Me.labelControl2.Size = New System.Drawing.Size(47, 13)
        Me.labelControl2.TabIndex = 3
        Me.labelControl2.Text = "Nombre:"
        '
        'labelControl1
        '
        Me.labelControl1.Location = New System.Drawing.Point(15, 38)
        Me.labelControl1.Name = "labelControl1"
        Me.labelControl1.Size = New System.Drawing.Size(86, 13)
        Me.labelControl1.TabIndex = 2
        Me.labelControl1.Text = "Número Empleado:"
        '
        'groupIncapacidades
        '
        Me.groupIncapacidades.Controls.Add(Me.dtFecha)
        Me.groupIncapacidades.Controls.Add(Me.cmbTipo)
        Me.groupIncapacidades.Controls.Add(Me.labelControl4)
        Me.groupIncapacidades.Controls.Add(Me.labelControl3)
        Me.groupIncapacidades.Controls.Add(Me.gridIncapacidades)
        Me.groupIncapacidades.Location = New System.Drawing.Point(12, 92)
        Me.groupIncapacidades.Name = "groupIncapacidades"
        Me.groupIncapacidades.Size = New System.Drawing.Size(635, 296)
        Me.groupIncapacidades.TabIndex = 1
        Me.groupIncapacidades.Text = "Incapacidades del empleado"
        '
        'dtFecha
        '
        Me.dtFecha.EditValue = Nothing
        Me.dtFecha.Location = New System.Drawing.Point(480, 262)
        Me.dtFecha.Name = "dtFecha"
        Me.dtFecha.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.dtFecha.Properties.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.dtFecha.Properties.DisplayFormat.FormatString = "dd/MM/yyyy"
        Me.dtFecha.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
        Me.dtFecha.Properties.EditFormat.FormatString = "dd/MM/yyyy"
        Me.dtFecha.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime
        Me.dtFecha.Properties.Mask.EditMask = "dd/MM/yyyy"
        Me.dtFecha.Properties.Mask.UseMaskAsDisplayFormat = True
        Me.dtFecha.Size = New System.Drawing.Size(130, 20)
        Me.dtFecha.TabIndex = 2
        '
        'cmbTipo
        '
        Me.cmbTipo.Location = New System.Drawing.Point(110, 262)
        Me.cmbTipo.Name = "cmbTipo"
        Me.cmbTipo.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cmbTipo.Properties.NullText = ""
        Me.cmbTipo.Size = New System.Drawing.Size(280, 20)
        Me.cmbTipo.TabIndex = 1
        '
        'labelControl4
        '
        Me.labelControl4.Location = New System.Drawing.Point(415, 265)
        Me.labelControl4.Name = "labelControl4"
        Me.labelControl4.Size = New System.Drawing.Size(32, 13)
        Me.labelControl4.TabIndex = 4
        Me.labelControl4.Text = "Fecha:"
        '
        'labelControl3
        '
        Me.labelControl3.Location = New System.Drawing.Point(15, 265)
        Me.labelControl3.Name = "labelControl3"
        Me.labelControl3.Size = New System.Drawing.Size(89, 13)
        Me.labelControl3.TabIndex = 3
        Me.labelControl3.Text = "Tipo Incapacidad:"
        '
        'gridIncapacidades
        '
        Me.gridIncapacidades.Location = New System.Drawing.Point(10, 28)
        Me.gridIncapacidades.MainView = Me.gridViewIncapacidades
        Me.gridIncapacidades.Name = "gridIncapacidades"
        Me.gridIncapacidades.Size = New System.Drawing.Size(615, 220)
        Me.gridIncapacidades.TabIndex = 0
        Me.gridIncapacidades.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewIncapacidades})
        '
        'gridViewIncapacidades
        '
        Me.gridViewIncapacidades.GridControl = Me.gridIncapacidades
        Me.gridViewIncapacidades.Name = "gridViewIncapacidades"
        Me.gridViewIncapacidades.OptionsBehavior.Editable = False
        Me.gridViewIncapacidades.OptionsView.ShowGroupPanel = False
        '
        'FrmIncapacidades
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(660, 447)
        Me.Controls.Add(Me.BtnAgregar)
        Me.Controls.Add(Me.BtnEliminar)
        Me.Controls.Add(Me.BtnAnterior)
        Me.Controls.Add(Me.BtnSiguiente)
        Me.Controls.Add(Me.BtnSalir)
        Me.Controls.Add(Me.groupEmpleado)
        Me.Controls.Add(Me.groupIncapacidades)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmIncapacidades"
        Me.Text = "Incapacidades"
        CType(Me.groupEmpleado, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupEmpleado.ResumeLayout(False)
        Me.groupEmpleado.PerformLayout()
        CType(Me.cmbEmpleado.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmbEmpleadoView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.groupIncapacidades, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupIncapacidades.ResumeLayout(False)
        Me.groupIncapacidades.PerformLayout()
        CType(Me.dtFecha.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dtFecha.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmbTipo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridIncapacidades, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewIncapacidades, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents BtnAgregar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnEliminar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnAnterior As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSiguiente As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents groupEmpleado As DevExpress.XtraEditors.GroupControl
    Friend WithEvents TxtNombre As System.Windows.Forms.TextBox
    Friend WithEvents cmbEmpleado As DevExpress.XtraEditors.SearchLookUpEdit
    Friend WithEvents cmbEmpleadoView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents labelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents groupIncapacidades As DevExpress.XtraEditors.GroupControl
    Friend WithEvents dtFecha As DevExpress.XtraEditors.DateEdit
    Friend WithEvents cmbTipo As DevExpress.XtraEditors.LookUpEdit
    Friend WithEvents labelControl4 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl3 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents gridIncapacidades As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewIncapacidades As DevExpress.XtraGrid.Views.Grid.GridView
End Class
