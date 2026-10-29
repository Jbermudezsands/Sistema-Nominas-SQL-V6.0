<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmAnotaciones
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
        Me.panelBotones = New DevExpress.XtraEditors.PanelControl()
        Me.BtnGrabar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnBorrar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.LblCodigo = New DevExpress.XtraEditors.LabelControl()
        Me.TxtCodigo = New DevExpress.XtraEditors.TextEdit()
        Me.LblNombre = New DevExpress.XtraEditors.LabelControl()
        Me.TxtNombre = New DevExpress.XtraEditors.TextEdit()
        Me.LblJustifica = New DevExpress.XtraEditors.LabelControl()
        Me.TxtJustificaFalta = New DevExpress.XtraEditors.MemoEdit()
        Me.LblRecord = New DevExpress.XtraEditors.LabelControl()
        Me.TxtDatosRecord = New DevExpress.XtraEditors.MemoEdit()
        Me.LblCursos = New DevExpress.XtraEditors.LabelControl()
        Me.TxtCursos = New DevExpress.XtraEditors.MemoEdit()
        Me.LblRazones = New DevExpress.XtraEditors.LabelControl()
        Me.TxtRazones = New DevExpress.XtraEditors.MemoEdit()
        Me.LblTrabajos = New DevExpress.XtraEditors.LabelControl()
        Me.TxtTrabAnteriores = New DevExpress.XtraEditors.MemoEdit()
        Me.LblRecomendaciones = New DevExpress.XtraEditors.LabelControl()
        Me.TxtRecomendaciones = New DevExpress.XtraEditors.MemoEdit()
        Me.LblSalida = New DevExpress.XtraEditors.LabelControl()
        Me.TxtSalida = New DevExpress.XtraEditors.MemoEdit()
        Me.LblFaltas = New DevExpress.XtraEditors.LabelControl()
        Me.CmbFaltas = New DevExpress.XtraEditors.ComboBoxEdit()
        Me.LblIdiomas = New DevExpress.XtraEditors.LabelControl()
        Me.TxtIdiomas = New DevExpress.XtraEditors.TextEdit()
        Me.LblTelefono = New DevExpress.XtraEditors.LabelControl()
        Me.TxtTelEmergencia = New DevExpress.XtraEditors.TextEdit()
        Me.LblContratacion = New DevExpress.XtraEditors.LabelControl()
        Me.DtpContratacion = New DevExpress.XtraEditors.DateEdit()
        Me.LblFinaliza = New DevExpress.XtraEditors.LabelControl()
        Me.DtpFinaliza = New DevExpress.XtraEditors.DateEdit()
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panelBotones.SuspendLayout()
        CType(Me.TxtCodigo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtNombre.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtJustificaFalta.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtDatosRecord.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtCursos.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtRazones.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtTrabAnteriores.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtRecomendaciones.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtSalida.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CmbFaltas.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtIdiomas.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtTelEmergencia.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtpContratacion.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtpContratacion.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtpFinaliza.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DtpFinaliza.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelBotones
        '
        Me.panelBotones.Controls.Add(Me.BtnGrabar)
        Me.panelBotones.Controls.Add(Me.BtnBorrar)
        Me.panelBotones.Controls.Add(Me.BtnSalir)
        Me.panelBotones.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panelBotones.Location = New System.Drawing.Point(0, 585)
        Me.panelBotones.Name = "panelBotones"
        Me.panelBotones.Size = New System.Drawing.Size(700, 55)
        Me.panelBotones.TabIndex = 30
        '
        'BtnGrabar
        '
        Me.BtnGrabar.Location = New System.Drawing.Point(13, 10)
        Me.BtnGrabar.Name = "BtnGrabar"
        Me.BtnGrabar.Size = New System.Drawing.Size(95, 35)
        Me.BtnGrabar.TabIndex = 0
        Me.BtnGrabar.Text = "Grabar"
        '
        'BtnBorrar
        '
        Me.BtnBorrar.Location = New System.Drawing.Point(119, 10)
        Me.BtnBorrar.Name = "BtnBorrar"
        Me.BtnBorrar.Size = New System.Drawing.Size(95, 35)
        Me.BtnBorrar.TabIndex = 1
        Me.BtnBorrar.Text = "Borrar"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(225, 10)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 2
        Me.BtnSalir.Text = "Salir"
        '
        'LblCodigo
        '
        Me.LblCodigo.Location = New System.Drawing.Point(12, 18)
        Me.LblCodigo.Name = "LblCodigo"
        Me.LblCodigo.Size = New System.Drawing.Size(37, 13)
        Me.LblCodigo.TabIndex = 31
        Me.LblCodigo.Text = "Código:"
        '
        'TxtCodigo
        '
        Me.TxtCodigo.Location = New System.Drawing.Point(60, 15)
        Me.TxtCodigo.Name = "TxtCodigo"
        Me.TxtCodigo.Properties.ReadOnly = True
        Me.TxtCodigo.Size = New System.Drawing.Size(90, 20)
        Me.TxtCodigo.TabIndex = 32
        Me.TxtCodigo.TabStop = False
        '
        'LblNombre
        '
        Me.LblNombre.Location = New System.Drawing.Point(170, 18)
        Me.LblNombre.Name = "LblNombre"
        Me.LblNombre.Size = New System.Drawing.Size(40, 13)
        Me.LblNombre.TabIndex = 33
        Me.LblNombre.Text = "Nombre:"
        '
        'TxtNombre
        '
        Me.TxtNombre.Location = New System.Drawing.Point(220, 15)
        Me.TxtNombre.Name = "TxtNombre"
        Me.TxtNombre.Properties.ReadOnly = True
        Me.TxtNombre.Size = New System.Drawing.Size(460, 20)
        Me.TxtNombre.TabIndex = 34
        Me.TxtNombre.TabStop = False
        '
        'LblFaltas
        '
        Me.LblFaltas.Location = New System.Drawing.Point(355, 62)
        Me.LblFaltas.Name = "LblFaltas"
        Me.LblFaltas.Size = New System.Drawing.Size(88, 13)
        Me.LblFaltas.TabIndex = 35
        Me.LblFaltas.Text = "Tuvo faltas:"
        '
        'CmbFaltas
        '
        Me.CmbFaltas.Location = New System.Drawing.Point(355, 78)
        Me.CmbFaltas.Name = "CmbFaltas"
        Me.CmbFaltas.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.CmbFaltas.Properties.Items.AddRange(New Object() {"Si", "No"})
        Me.CmbFaltas.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        Me.CmbFaltas.Size = New System.Drawing.Size(80, 20)
        Me.CmbFaltas.TabIndex = 0
        '
        'LblJustifica
        '
        Me.LblJustifica.Location = New System.Drawing.Point(12, 62)
        Me.LblJustifica.Name = "LblJustifica"
        Me.LblJustifica.Size = New System.Drawing.Size(118, 13)
        Me.LblJustifica.TabIndex = 36
        Me.LblJustifica.Text = "Justificación de faltas:"
        '
        'TxtJustificaFalta
        '
        Me.TxtJustificaFalta.Location = New System.Drawing.Point(12, 78)
        Me.TxtJustificaFalta.Name = "TxtJustificaFalta"
        Me.TxtJustificaFalta.Size = New System.Drawing.Size(325, 48)
        Me.TxtJustificaFalta.TabIndex = 1
        '
        'LblRecord
        '
        Me.LblRecord.Location = New System.Drawing.Point(12, 134)
        Me.LblRecord.Name = "LblRecord"
        Me.LblRecord.Size = New System.Drawing.Size(86, 13)
        Me.LblRecord.TabIndex = 37
        Me.LblRecord.Text = "Datos del récord:"
        '
        'TxtDatosRecord
        '
        Me.TxtDatosRecord.Location = New System.Drawing.Point(12, 150)
        Me.TxtDatosRecord.Name = "TxtDatosRecord"
        Me.TxtDatosRecord.Size = New System.Drawing.Size(325, 48)
        Me.TxtDatosRecord.TabIndex = 2
        '
        'LblIdiomas
        '
        Me.LblIdiomas.Location = New System.Drawing.Point(355, 134)
        Me.LblIdiomas.Name = "LblIdiomas"
        Me.LblIdiomas.Size = New System.Drawing.Size(41, 13)
        Me.LblIdiomas.TabIndex = 38
        Me.LblIdiomas.Text = "Idiomas:"
        '
        'TxtIdiomas
        '
        Me.TxtIdiomas.Location = New System.Drawing.Point(355, 150)
        Me.TxtIdiomas.Name = "TxtIdiomas"
        Me.TxtIdiomas.Size = New System.Drawing.Size(325, 20)
        Me.TxtIdiomas.TabIndex = 3
        '
        'LblCursos
        '
        Me.LblCursos.Location = New System.Drawing.Point(12, 206)
        Me.LblCursos.Name = "LblCursos"
        Me.LblCursos.Size = New System.Drawing.Size(37, 13)
        Me.LblCursos.TabIndex = 39
        Me.LblCursos.Text = "Cursos:"
        '
        'TxtCursos
        '
        Me.TxtCursos.Location = New System.Drawing.Point(12, 222)
        Me.TxtCursos.Name = "TxtCursos"
        Me.TxtCursos.Size = New System.Drawing.Size(325, 48)
        Me.TxtCursos.TabIndex = 5
        '
        'LblTelefono
        '
        Me.LblTelefono.Location = New System.Drawing.Point(355, 206)
        Me.LblTelefono.Name = "LblTelefono"
        Me.LblTelefono.Size = New System.Drawing.Size(145, 13)
        Me.LblTelefono.TabIndex = 40
        Me.LblTelefono.Text = "Teléfono en caso de emergencia:"
        '
        'TxtTelEmergencia
        '
        Me.TxtTelEmergencia.Location = New System.Drawing.Point(355, 222)
        Me.TxtTelEmergencia.Name = "TxtTelEmergencia"
        Me.TxtTelEmergencia.Size = New System.Drawing.Size(325, 20)
        Me.TxtTelEmergencia.TabIndex = 4
        '
        'LblRazones
        '
        Me.LblRazones.Location = New System.Drawing.Point(12, 278)
        Me.LblRazones.Name = "LblRazones"
        Me.LblRazones.Size = New System.Drawing.Size(121, 13)
        Me.LblRazones.TabIndex = 41
        Me.LblRazones.Text = "Razones de contratación:"
        '
        'TxtRazones
        '
        Me.TxtRazones.Location = New System.Drawing.Point(12, 294)
        Me.TxtRazones.Name = "TxtRazones"
        Me.TxtRazones.Size = New System.Drawing.Size(325, 48)
        Me.TxtRazones.TabIndex = 6
        '
        'LblContratacion
        '
        Me.LblContratacion.Location = New System.Drawing.Point(355, 278)
        Me.LblContratacion.Name = "LblContratacion"
        Me.LblContratacion.Size = New System.Drawing.Size(108, 13)
        Me.LblContratacion.TabIndex = 42
        Me.LblContratacion.Text = "Fecha de contratación:"
        '
        'DtpContratacion
        '
        Me.DtpContratacion.EditValue = Nothing
        Me.DtpContratacion.Location = New System.Drawing.Point(355, 294)
        Me.DtpContratacion.Name = "DtpContratacion"
        Me.DtpContratacion.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DtpContratacion.Properties.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DtpContratacion.Size = New System.Drawing.Size(130, 20)
        Me.DtpContratacion.TabIndex = 7
        '
        'LblTrabajos
        '
        Me.LblTrabajos.Location = New System.Drawing.Point(12, 350)
        Me.LblTrabajos.Name = "LblTrabajos"
        Me.LblTrabajos.Size = New System.Drawing.Size(98, 13)
        Me.LblTrabajos.TabIndex = 43
        Me.LblTrabajos.Text = "Trabajos anteriores:"
        '
        'TxtTrabAnteriores
        '
        Me.TxtTrabAnteriores.Location = New System.Drawing.Point(12, 366)
        Me.TxtTrabAnteriores.Name = "TxtTrabAnteriores"
        Me.TxtTrabAnteriores.Size = New System.Drawing.Size(325, 48)
        Me.TxtTrabAnteriores.TabIndex = 8
        '
        'LblFinaliza
        '
        Me.LblFinaliza.Location = New System.Drawing.Point(355, 350)
        Me.LblFinaliza.Name = "LblFinaliza"
        Me.LblFinaliza.Size = New System.Drawing.Size(105, 13)
        Me.LblFinaliza.TabIndex = 44
        Me.LblFinaliza.Text = "Fecha de finalización:"
        '
        'DtpFinaliza
        '
        Me.DtpFinaliza.EditValue = Nothing
        Me.DtpFinaliza.Location = New System.Drawing.Point(355, 366)
        Me.DtpFinaliza.Name = "DtpFinaliza"
        Me.DtpFinaliza.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DtpFinaliza.Properties.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DtpFinaliza.Size = New System.Drawing.Size(130, 20)
        Me.DtpFinaliza.TabIndex = 9
        '
        'LblRecomendaciones
        '
        Me.LblRecomendaciones.Location = New System.Drawing.Point(12, 422)
        Me.LblRecomendaciones.Name = "LblRecomendaciones"
        Me.LblRecomendaciones.Size = New System.Drawing.Size(92, 13)
        Me.LblRecomendaciones.TabIndex = 45
        Me.LblRecomendaciones.Text = "Recomendaciones:"
        '
        'TxtRecomendaciones
        '
        Me.TxtRecomendaciones.Location = New System.Drawing.Point(12, 438)
        Me.TxtRecomendaciones.Name = "TxtRecomendaciones"
        Me.TxtRecomendaciones.Size = New System.Drawing.Size(325, 48)
        Me.TxtRecomendaciones.TabIndex = 10
        '
        'LblSalida
        '
        Me.LblSalida.Location = New System.Drawing.Point(12, 494)
        Me.LblSalida.Name = "LblSalida"
        Me.LblSalida.Size = New System.Drawing.Size(85, 13)
        Me.LblSalida.TabIndex = 46
        Me.LblSalida.Text = "Causa de salida:"
        '
        'TxtSalida
        '
        Me.TxtSalida.Location = New System.Drawing.Point(12, 510)
        Me.TxtSalida.Name = "TxtSalida"
        Me.TxtSalida.Size = New System.Drawing.Size(325, 48)
        Me.TxtSalida.TabIndex = 11
        '
        'FrmAnotaciones
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(700, 640)
        Me.Controls.Add(Me.TxtSalida)
        Me.Controls.Add(Me.LblSalida)
        Me.Controls.Add(Me.TxtRecomendaciones)
        Me.Controls.Add(Me.LblRecomendaciones)
        Me.Controls.Add(Me.DtpFinaliza)
        Me.Controls.Add(Me.LblFinaliza)
        Me.Controls.Add(Me.TxtTrabAnteriores)
        Me.Controls.Add(Me.LblTrabajos)
        Me.Controls.Add(Me.DtpContratacion)
        Me.Controls.Add(Me.LblContratacion)
        Me.Controls.Add(Me.TxtRazones)
        Me.Controls.Add(Me.LblRazones)
        Me.Controls.Add(Me.TxtTelEmergencia)
        Me.Controls.Add(Me.LblTelefono)
        Me.Controls.Add(Me.TxtCursos)
        Me.Controls.Add(Me.LblCursos)
        Me.Controls.Add(Me.TxtIdiomas)
        Me.Controls.Add(Me.LblIdiomas)
        Me.Controls.Add(Me.TxtDatosRecord)
        Me.Controls.Add(Me.LblRecord)
        Me.Controls.Add(Me.TxtJustificaFalta)
        Me.Controls.Add(Me.LblJustifica)
        Me.Controls.Add(Me.CmbFaltas)
        Me.Controls.Add(Me.LblFaltas)
        Me.Controls.Add(Me.TxtNombre)
        Me.Controls.Add(Me.LblNombre)
        Me.Controls.Add(Me.TxtCodigo)
        Me.Controls.Add(Me.LblCodigo)
        Me.Controls.Add(Me.panelBotones)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmAnotaciones"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Anotaciones del Empleado"
        CType(Me.panelBotones, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panelBotones.ResumeLayout(False)
        CType(Me.TxtCodigo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtNombre.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtJustificaFalta.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtDatosRecord.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtCursos.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtRazones.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtTrabAnteriores.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtRecomendaciones.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtSalida.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CmbFaltas.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtIdiomas.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtTelEmergencia.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtpContratacion.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtpContratacion.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtpFinaliza.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DtpFinaliza.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents panelBotones As DevExpress.XtraEditors.PanelControl
    Friend WithEvents BtnGrabar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnBorrar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents LblCodigo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtCodigo As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LblNombre As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtNombre As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LblJustifica As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtJustificaFalta As DevExpress.XtraEditors.MemoEdit
    Friend WithEvents LblRecord As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtDatosRecord As DevExpress.XtraEditors.MemoEdit
    Friend WithEvents LblCursos As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtCursos As DevExpress.XtraEditors.MemoEdit
    Friend WithEvents LblRazones As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtRazones As DevExpress.XtraEditors.MemoEdit
    Friend WithEvents LblTrabajos As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtTrabAnteriores As DevExpress.XtraEditors.MemoEdit
    Friend WithEvents LblRecomendaciones As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtRecomendaciones As DevExpress.XtraEditors.MemoEdit
    Friend WithEvents LblSalida As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtSalida As DevExpress.XtraEditors.MemoEdit
    Friend WithEvents LblFaltas As DevExpress.XtraEditors.LabelControl
    Friend WithEvents CmbFaltas As DevExpress.XtraEditors.ComboBoxEdit
    Friend WithEvents LblIdiomas As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtIdiomas As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LblTelefono As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtTelEmergencia As DevExpress.XtraEditors.TextEdit
    Friend WithEvents LblContratacion As DevExpress.XtraEditors.LabelControl
    Friend WithEvents DtpContratacion As DevExpress.XtraEditors.DateEdit
    Friend WithEvents LblFinaliza As DevExpress.XtraEditors.LabelControl
    Friend WithEvents DtpFinaliza As DevExpress.XtraEditors.DateEdit
End Class
