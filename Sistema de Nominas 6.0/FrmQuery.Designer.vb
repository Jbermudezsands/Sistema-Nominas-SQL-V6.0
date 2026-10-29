<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmQuery
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
        Me.panelConsulta = New DevExpress.XtraEditors.PanelControl()
        Me.TxtConsulta = New DevExpress.XtraEditors.MemoEdit()
        Me.panelBotones = New DevExpress.XtraEditors.PanelControl()
        Me.LblEstado = New DevExpress.XtraEditors.LabelControl()
        Me.BtnEjecutar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnExcel = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnInstalarSP = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnEstructura = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.gridConsulta = New DevExpress.XtraGrid.GridControl()
        Me.gridViewConsulta = New DevExpress.XtraGrid.Views.Grid.GridView()
        CType(Me.panelConsulta, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelConsulta.SuspendLayout()
        CType(Me.TxtConsulta.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        CType(Me.gridConsulta, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.gridViewConsulta, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelConsulta
        '
        Me.panelConsulta.Controls.Add(Me.TxtConsulta)
        Me.panelConsulta.Dock = System.Windows.Forms.DockStyle.Top
        Me.panelConsulta.Location = New System.Drawing.Point(0, 0)
        Me.panelConsulta.Name = "panelConsulta"
        Me.panelConsulta.Size = New System.Drawing.Size(900, 170)
        Me.panelConsulta.TabIndex = 0
        '
        'TxtConsulta
        '
        Me.TxtConsulta.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TxtConsulta.Location = New System.Drawing.Point(2, 2)
        Me.TxtConsulta.Name = "TxtConsulta"
        Me.TxtConsulta.Properties.Appearance.Font = New System.Drawing.Font("Consolas", 9.75!)
        Me.TxtConsulta.Properties.Appearance.Options.UseFont = True
        Me.TxtConsulta.Size = New System.Drawing.Size(896, 166)
        Me.TxtConsulta.TabIndex = 0
        '
        'panelBotones
        '
        Me.panelBotones.Controls.Add(Me.LblEstado)
        Me.panelBotones.Controls.Add(Me.BtnEjecutar)
        Me.panelBotones.Controls.Add(Me.BtnExcel)
        Me.panelBotones.Controls.Add(Me.BtnInstalarSP)
        Me.panelBotones.Controls.Add(Me.BtnEstructura)
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Location = New System.Drawing.Point(0, 407)
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(900, 55)
        Me.panelBotones.TabIndex = 2
        '
        'LblEstado
        '
        Me.LblEstado.Location = New System.Drawing.Point(640, 21)
        Me.LblEstado.Name = "LblEstado"
        Me.LblEstado.Size = New System.Drawing.Size(0, 13)
        Me.LblEstado.TabIndex = 5
        '
        'BtnEjecutar
        '
        Me.BtnEjecutar.Location = New System.Drawing.Point(13, 10)
        Me.BtnEjecutar.Name = "BtnEjecutar"
        Me.BtnEjecutar.Size = New System.Drawing.Size(95, 35)
        Me.BtnEjecutar.TabIndex = 0
        Me.BtnEjecutar.Text = "Ejecutar"
        '
        'BtnExcel
        '
        Me.BtnExcel.Location = New System.Drawing.Point(119, 10)
        Me.BtnExcel.Name = "BtnExcel"
        Me.BtnExcel.Size = New System.Drawing.Size(95, 35)
        Me.BtnExcel.TabIndex = 1
        Me.BtnExcel.Text = "Excel"
        '
        'BtnInstalarSP
        '
        Me.BtnInstalarSP.Location = New System.Drawing.Point(225, 10)
        Me.BtnInstalarSP.Name = "BtnInstalarSP"
        Me.BtnInstalarSP.Size = New System.Drawing.Size(135, 35)
        Me.BtnInstalarSP.TabIndex = 2
        Me.BtnInstalarSP.Text = "Instalar SP asistencia"
        '
        'BtnEstructura
        '
        Me.BtnEstructura.Location = New System.Drawing.Point(366, 10)
        Me.BtnEstructura.Name = "BtnEstructura"
        Me.BtnEstructura.Size = New System.Drawing.Size(135, 35)
        Me.BtnEstructura.TabIndex = 3
        Me.BtnEstructura.Text = "Actualizar estructura"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(507, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 4
        Me.BtnSalir.Text = "Salir"
        '
        'gridConsulta
        '
        Me.gridConsulta.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridConsulta.Location = New System.Drawing.Point(0, 170)
        Me.gridConsulta.MainView = Me.gridViewConsulta
        Me.gridConsulta.Name = "gridConsulta"
        Me.gridConsulta.Size = New System.Drawing.Size(900, 237)
        Me.gridConsulta.TabIndex = 1
        Me.gridConsulta.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gridViewConsulta})
        '
        'gridViewConsulta
        '
        Me.gridViewConsulta.GridControl = Me.gridConsulta
        Me.gridViewConsulta.Name = "gridViewConsulta"
        Me.gridViewConsulta.OptionsBehavior.Editable = False
        Me.gridViewConsulta.OptionsView.ShowAutoFilterRow = True
        Me.gridViewConsulta.OptionsView.ShowGroupPanel = False
        '
        'FrmQuery
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 462)
        Me.Controls.Add(Me.gridConsulta)
        Me.Controls.Add(Me.panelBotones)
        Me.Controls.Add(Me.panelConsulta)
        Me.Name = "FrmQuery"
        Me.Text = "Consultas SQL"
        CType(Me.panelConsulta, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelConsulta.ResumeLayout(False)
        CType(Me.TxtConsulta.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        Me.panelBotones.PerformLayout()
        CType(Me.gridConsulta, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.gridViewConsulta, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents panelConsulta As DevExpress.XtraEditors.PanelControl
    Friend WithEvents TxtConsulta As DevExpress.XtraEditors.MemoEdit
    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents LblEstado As DevExpress.XtraEditors.LabelControl
    Friend WithEvents BtnEjecutar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnExcel As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnInstalarSP As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnEstructura As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents gridConsulta As DevExpress.XtraGrid.GridControl
    Friend WithEvents gridViewConsulta As DevExpress.XtraGrid.Views.Grid.GridView
End Class
