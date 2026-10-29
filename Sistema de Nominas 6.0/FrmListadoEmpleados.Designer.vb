<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmListadoEmpleados
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
        Me.LblRegistros = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl1 = New DevExpress.XtraEditors.LabelControl()
        Me.BtnNuevo = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnEditar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnExcel = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.gridEmpleados = New DevExpress.XtraGrid.GridControl()
        Me.gridViewEmpleados = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        CType(Me.gridEmpleados, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewEmpleados, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelBotones
        '
        Me.panelBotones.Controls.Add(Me.LblRegistros)
        Me.panelBotones.Controls.Add(Me.labelControl1)
        Me.panelBotones.Controls.Add(Me.BtnNuevo)
        Me.panelBotones.Controls.Add(Me.BtnEditar)
        Me.panelBotones.Controls.Add(Me.BtnExcel)
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Location = New System.Drawing.Point(0, 386)
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(1000, 55)
        Me.panelBotones.TabIndex = 1
        '
        'LblRegistros
        '
        Me.LblRegistros.Location = New System.Drawing.Point(660, 21)
        Me.LblRegistros.Name = "LblRegistros"
        Me.LblRegistros.Size = New System.Drawing.Size(6, 13)
        Me.LblRegistros.TabIndex = 5
        Me.LblRegistros.Text = "0"
        '
        'labelControl1
        '
        Me.labelControl1.Location = New System.Drawing.Point(600, 21)
        Me.labelControl1.Name = "labelControl1"
        Me.labelControl1.Size = New System.Drawing.Size(49, 13)
        Me.labelControl1.TabIndex = 4
        Me.labelControl1.Text = "Registros:"
        '
        'BtnNuevo
        '
        Me.BtnNuevo.Location = New System.Drawing.Point(13, 10)
        Me.BtnNuevo.Name = "BtnNuevo"
        Me.BtnNuevo.Size = New System.Drawing.Size(95, 35)
        Me.BtnNuevo.TabIndex = 0
        Me.BtnNuevo.Text = "Nuevo"
        '
        'BtnEditar
        '
        Me.BtnEditar.Location = New System.Drawing.Point(119, 10)
        Me.BtnEditar.Name = "BtnEditar"
        Me.BtnEditar.Size = New System.Drawing.Size(95, 35)
        Me.BtnEditar.TabIndex = 1
        Me.BtnEditar.Text = "Editar"
        '
        'BtnExcel
        '
        Me.BtnExcel.Location = New System.Drawing.Point(225, 10)
        Me.BtnExcel.Name = "BtnExcel"
        Me.BtnExcel.Size = New System.Drawing.Size(95, 35)
        Me.BtnExcel.TabIndex = 2
        Me.BtnExcel.Text = "Excel"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(331, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 3
        Me.BtnSalir.Text = "Salir"
        '
        'gridEmpleados
        '
        Me.gridEmpleados.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridEmpleados.Location = New System.Drawing.Point(0, 0)
        Me.gridEmpleados.MainView = Me.gridViewEmpleados
        Me.gridEmpleados.Name = "gridEmpleados"
        Me.gridEmpleados.Size = New System.Drawing.Size(1000, 386)
        Me.gridEmpleados.TabIndex = 0
        Me.gridEmpleados.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewEmpleados})
        '
        'gridViewEmpleados
        '
        Me.gridViewEmpleados.GridControl = Me.gridEmpleados
        Me.gridViewEmpleados.Name = "gridViewEmpleados"
        Me.gridViewEmpleados.OptionsBehavior.Editable = False
        Me.gridViewEmpleados.OptionsView.ShowGroupPanel = False
        '
        'FrmListadoEmpleados
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1000, 441)
        Me.Controls.Add(Me.gridEmpleados)
        Me.Controls.Add(Me.panelBotones)
        Me.Name = "FrmListadoEmpleados"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Listado de Empleados"
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        Me.panelBotones.PerformLayout()
        CType(Me.gridEmpleados, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewEmpleados, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LblRegistros As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents BtnNuevo As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnEditar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnExcel As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents gridEmpleados As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewEmpleados As DevExpress.XtraGrid.Views.Grid.GridView
End Class
