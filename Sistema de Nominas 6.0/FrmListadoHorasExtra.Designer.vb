<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmListadoHorasExtra
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
        Me.LblTotalHoras = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.LblRegistros = New DevExpress.XtraEditors.LabelControl()
        Me.labelControl1 = New DevExpress.XtraEditors.LabelControl()
        Me.BtnEliminar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnExcel = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.gridHorasExtra = New DevExpress.XtraGrid.GridControl()
        Me.gridViewHorasExtra = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        CType(Me.gridHorasExtra, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewHorasExtra, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelBotones
        '
        Me.panelBotones.Controls.Add(Me.LblTotalHoras)
        Me.panelBotones.Controls.Add(Me.labelControl2)
        Me.panelBotones.Controls.Add(Me.LblRegistros)
        Me.panelBotones.Controls.Add(Me.labelControl1)
        Me.panelBotones.Controls.Add(Me.BtnEliminar)
        Me.panelBotones.Controls.Add(Me.BtnExcel)
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Location = New System.Drawing.Point(0, 364)
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(720, 55)
        Me.panelBotones.TabIndex = 1
        '
        'LblTotalHoras
        '
        Me.LblTotalHoras.Location = New System.Drawing.Point(620, 21)
        Me.LblTotalHoras.Name = "LblTotalHoras"
        Me.LblTotalHoras.Size = New System.Drawing.Size(22, 13)
        Me.LblTotalHoras.TabIndex = 6
        Me.LblTotalHoras.Text = "0.00"
        '
        'labelControl2
        '
        Me.labelControl2.Location = New System.Drawing.Point(550, 21)
        Me.labelControl2.Name = "labelControl2"
        Me.labelControl2.Size = New System.Drawing.Size(58, 13)
        Me.labelControl2.TabIndex = 5
        Me.labelControl2.Text = "Total horas:"
        '
        'LblRegistros
        '
        Me.LblRegistros.Location = New System.Drawing.Point(500, 21)
        Me.LblRegistros.Name = "LblRegistros"
        Me.LblRegistros.Size = New System.Drawing.Size(6, 13)
        Me.LblRegistros.TabIndex = 4
        Me.LblRegistros.Text = "0"
        '
        'labelControl1
        '
        Me.labelControl1.Location = New System.Drawing.Point(440, 21)
        Me.labelControl1.Name = "labelControl1"
        Me.labelControl1.Size = New System.Drawing.Size(49, 13)
        Me.labelControl1.TabIndex = 3
        Me.labelControl1.Text = "Registros:"
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
        'gridHorasExtra
        '
        Me.gridHorasExtra.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridHorasExtra.Location = New System.Drawing.Point(0, 0)
        Me.gridHorasExtra.MainView = Me.gridViewHorasExtra
        Me.gridHorasExtra.Name = "gridHorasExtra"
        Me.gridHorasExtra.Size = New System.Drawing.Size(720, 364)
        Me.gridHorasExtra.TabIndex = 0
        Me.gridHorasExtra.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewHorasExtra})
        '
        'gridViewHorasExtra
        '
        Me.gridViewHorasExtra.GridControl = Me.gridHorasExtra
        Me.gridViewHorasExtra.Name = "gridViewHorasExtra"
        Me.gridViewHorasExtra.OptionsBehavior.Editable = False
        Me.gridViewHorasExtra.OptionsView.ShowGroupPanel = False
        '
        'FrmListadoHorasExtra
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(720, 419)
        Me.Controls.Add(Me.gridHorasExtra)
        Me.Controls.Add(Me.panelBotones)
        Me.Name = "FrmListadoHorasExtra"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Listado de Horas Extra"
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        Me.panelBotones.PerformLayout()
        CType(Me.gridHorasExtra, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewHorasExtra, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LblTotalHoras As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl2 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents LblRegistros As DevExpress.XtraEditors.LabelControl
    Friend WithEvents labelControl1 As DevExpress.XtraEditors.LabelControl
    Friend WithEvents BtnEliminar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnExcel As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents gridHorasExtra As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewHorasExtra As DevExpress.XtraGrid.Views.Grid.GridView
End Class
