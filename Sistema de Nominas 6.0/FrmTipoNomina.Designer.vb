<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmTipoNomina
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
        Me.groupGenerales = New DevExpress.XtraEditors.GroupControl()
        Me.lblCodigo = New DevExpress.XtraEditors.LabelControl()
        Me.cmbCodigo = New DevExpress.XtraEditors.SearchLookUpEdit()
        Me.cmbCodigoView = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.TxtCodigo = New System.Windows.Forms.TextBox()
        Me.lblDesc = New DevExpress.XtraEditors.LabelControl()
        Me.TxtDescripcion = New System.Windows.Forms.TextBox()
        Me.lblPeriodo = New DevExpress.XtraEditors.LabelControl()
        Me.CmbPeriodo = New DevExpress.XtraEditors.ComboBoxEdit()
        Me.lblTarifa = New DevExpress.XtraEditors.LabelControl()
        Me.TxtTarifa = New DevExpress.XtraEditors.TextEdit()
        Me.lblHoras = New DevExpress.XtraEditors.LabelControl()
        Me.TxtHoras = New DevExpress.XtraEditors.TextEdit()
        Me.groupPago = New DevExpress.XtraEditors.GroupControl()
        Me.RgTipoPago = New DevExpress.XtraEditors.RadioGroup()
        Me.groupMoneda = New DevExpress.XtraEditors.GroupControl()
        Me.RgMoneda = New DevExpress.XtraEditors.RadioGroup()
        Me.ChkMantValor = New DevExpress.XtraEditors.CheckEdit()
        Me.groupInss = New DevExpress.XtraEditors.GroupControl()
        Me.RgInss = New DevExpress.XtraEditors.RadioGroup()
        Me.lblInssLab = New DevExpress.XtraEditors.LabelControl()
        Me.TxtInssLaboral = New DevExpress.XtraEditors.TextEdit()
        Me.lblInssPat = New DevExpress.XtraEditors.LabelControl()
        Me.TxtInssPatronal = New DevExpress.XtraEditors.TextEdit()
        Me.groupIr = New DevExpress.XtraEditors.GroupControl()
        Me.RgIr = New DevExpress.XtraEditors.RadioGroup()
        Me.lblIrPct = New DevExpress.XtraEditors.LabelControl()
        Me.TxtIr = New DevExpress.XtraEditors.TextEdit()
        Me.groupOpc = New DevExpress.XtraEditors.GroupControl()
        Me.ChkEmpleadoCero = New DevExpress.XtraEditors.CheckEdit()
        Me.ChkConstruccion = New DevExpress.XtraEditors.CheckEdit()
        Me.ChkIrUltima = New DevExpress.XtraEditors.CheckEdit()
        Me.ChkHorasTrabajadas = New DevExpress.XtraEditors.CheckEdit()
        Me.BtnNuevo = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnGuardar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnEliminar = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnAnterior = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSiguiente = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnSalir = New DevExpress.XtraEditors.SimpleButton()
        Me.btnCancelar = New DevExpress.XtraEditors.SimpleButton()
        CType(Me.groupGenerales, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbCodigo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmbCodigoView, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.CmbPeriodo.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtTarifa.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtHoras.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.groupPago, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RgTipoPago.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.groupMoneda, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RgMoneda.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ChkMantValor.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.groupInss, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RgInss.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtInssLaboral.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtInssPatronal.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.groupIr, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RgIr.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TxtIr.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.groupOpc, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ChkEmpleadoCero.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ChkConstruccion.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ChkIrUltima.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ChkHorasTrabajadas.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupGenerales.SuspendLayout()
        Me.groupPago.SuspendLayout()
        Me.groupMoneda.SuspendLayout()
        Me.groupInss.SuspendLayout()
        Me.groupIr.SuspendLayout()
        Me.groupOpc.SuspendLayout()
        Me.SuspendLayout()
        '
        'groupGenerales
        '
        Me.groupGenerales.Controls.Add(Me.lblCodigo)
        Me.groupGenerales.Controls.Add(Me.cmbCodigo)
        Me.groupGenerales.Controls.Add(Me.TxtCodigo)
        Me.groupGenerales.Controls.Add(Me.lblDesc)
        Me.groupGenerales.Controls.Add(Me.TxtDescripcion)
        Me.groupGenerales.Controls.Add(Me.lblPeriodo)
        Me.groupGenerales.Controls.Add(Me.CmbPeriodo)
        Me.groupGenerales.Controls.Add(Me.lblTarifa)
        Me.groupGenerales.Controls.Add(Me.TxtTarifa)
        Me.groupGenerales.Controls.Add(Me.lblHoras)
        Me.groupGenerales.Controls.Add(Me.TxtHoras)
        Me.groupGenerales.Location = New System.Drawing.Point(12, 12)
        Me.groupGenerales.Name = "groupGenerales"
        Me.groupGenerales.Size = New System.Drawing.Size(676, 112)
        Me.groupGenerales.TabIndex = 0
        Me.groupGenerales.Text = "Datos generales"
        '
        'lblCodigo
        '
        Me.lblCodigo.Location = New System.Drawing.Point(15, 38)
        Me.lblCodigo.Name = "lblCodigo"
        Me.lblCodigo.Size = New System.Drawing.Size(70, 13)
        Me.lblCodigo.TabIndex = 1
        Me.lblCodigo.Text = "Codigo Nomina:"
        '
        'cmbCodigo
        '
        Me.cmbCodigo.Location = New System.Drawing.Point(120, 35)
        Me.cmbCodigo.Name = "cmbCodigo"
        Me.cmbCodigo.Size = New System.Drawing.Size(140, 20)
        Me.cmbCodigo.TabIndex = 2
        Me.cmbCodigo.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.cmbCodigo.Properties.NullText = ""
        Me.cmbCodigo.Properties.PopupSizeable = False
        Me.cmbCodigo.Properties.PopupView = Me.cmbCodigoView
        '
        'cmbCodigoView
        '
        Me.cmbCodigoView.Name = "cmbCodigoView"
        Me.cmbCodigoView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus
        Me.cmbCodigoView.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.cmbCodigoView.OptionsView.ShowGroupPanel = False
        '
        'TxtCodigo
        '
        Me.TxtCodigo.Location = New System.Drawing.Point(120, 35)
        Me.TxtCodigo.Name = "TxtCodigo"
        Me.TxtCodigo.Size = New System.Drawing.Size(140, 21)
        Me.TxtCodigo.TabIndex = 4
        Me.TxtCodigo.Visible = False
        Me.TxtCodigo.MaxLength = 20
        '
        'lblDesc
        '
        Me.lblDesc.Location = New System.Drawing.Point(285, 38)
        Me.lblDesc.Name = "lblDesc"
        Me.lblDesc.Size = New System.Drawing.Size(66, 13)
        Me.lblDesc.TabIndex = 5
        Me.lblDesc.Text = "Descripción:"
        '
        'TxtDescripcion
        '
        Me.TxtDescripcion.Location = New System.Drawing.Point(360, 35)
        Me.TxtDescripcion.Name = "TxtDescripcion"
        Me.TxtDescripcion.Size = New System.Drawing.Size(300, 21)
        Me.TxtDescripcion.TabIndex = 6
        Me.TxtDescripcion.MaxLength = 50
        '
        'lblPeriodo
        '
        Me.lblPeriodo.Location = New System.Drawing.Point(15, 68)
        Me.lblPeriodo.Name = "lblPeriodo"
        Me.lblPeriodo.Size = New System.Drawing.Size(41, 13)
        Me.lblPeriodo.TabIndex = 7
        Me.lblPeriodo.Text = "Período:"
        '
        'CmbPeriodo
        '
        Me.CmbPeriodo.Location = New System.Drawing.Point(120, 65)
        Me.CmbPeriodo.Name = "CmbPeriodo"
        Me.CmbPeriodo.Size = New System.Drawing.Size(200, 20)
        Me.CmbPeriodo.TabIndex = 8
        Me.CmbPeriodo.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.CmbPeriodo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        Me.CmbPeriodo.Properties.Items.AddRange(New Object() {"Semanal Viernes", "Semanal Sabado", "Catorcenal los Viernes", "Catorcenal los Sabados", "Quincenal", "Mensual", "Trimestral", "Semestral"})
        '
        'lblTarifa
        '
        Me.lblTarifa.Location = New System.Drawing.Point(340, 68)
        Me.lblTarifa.Name = "lblTarifa"
        Me.lblTarifa.Size = New System.Drawing.Size(73, 13)
        Me.lblTarifa.TabIndex = 9
        Me.lblTarifa.Text = "Tarifa Básica:"
        '
        'TxtTarifa
        '
        Me.TxtTarifa.Location = New System.Drawing.Point(420, 65)
        Me.TxtTarifa.Name = "TxtTarifa"
        Me.TxtTarifa.Size = New System.Drawing.Size(90, 20)
        Me.TxtTarifa.TabIndex = 10
        Me.TxtTarifa.EditValue = 0
        Me.TxtTarifa.Properties.Mask.EditMask = "n2"
        Me.TxtTarifa.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtTarifa.Properties.Mask.UseMaskAsDisplayFormat = True
        '
        'lblHoras
        '
        Me.lblHoras.Location = New System.Drawing.Point(525, 68)
        Me.lblHoras.Name = "lblHoras"
        Me.lblHoras.Size = New System.Drawing.Size(87, 13)
        Me.lblHoras.TabIndex = 11
        Me.lblHoras.Text = "Horas laboradas:"
        '
        'TxtHoras
        '
        Me.TxtHoras.Location = New System.Drawing.Point(620, 65)
        Me.TxtHoras.Name = "TxtHoras"
        Me.TxtHoras.Size = New System.Drawing.Size(40, 20)
        Me.TxtHoras.TabIndex = 12
        Me.TxtHoras.EditValue = 8
        Me.TxtHoras.Properties.Mask.EditMask = "n0"
        Me.TxtHoras.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtHoras.Properties.Mask.UseMaskAsDisplayFormat = True
        '
        'groupPago
        '
        Me.groupPago.Controls.Add(Me.RgTipoPago)
        Me.groupPago.Location = New System.Drawing.Point(12, 130)
        Me.groupPago.Name = "groupPago"
        Me.groupPago.Size = New System.Drawing.Size(330, 135)
        Me.groupPago.TabIndex = 13
        Me.groupPago.Text = "Tipo de Pago"
        '
        'RgTipoPago
        '
        Me.RgTipoPago.Location = New System.Drawing.Point(10, 30)
        Me.RgTipoPago.Name = "RgTipoPago"
        Me.RgTipoPago.Size = New System.Drawing.Size(310, 95)
        Me.RgTipoPago.TabIndex = 14
        Me.RgTipoPago.EditValue = "Salario Fijo"
        Me.RgTipoPago.Properties.Columns = 1
        Me.RgTipoPago.Properties.Items.AddRange(New DevExpress.XtraEditors.Controls.RadioGroupItem() {New DevExpress.XtraEditors.Controls.RadioGroupItem("Salario Fijo", "Sueldo Fijo"), New DevExpress.XtraEditors.Controls.RadioGroupItem("Salario Destajo", "Salario al Destajo"), New DevExpress.XtraEditors.Controls.RadioGroupItem("Salario Fijo y Comision", "Salario Fijo y Comisión"), New DevExpress.XtraEditors.Controls.RadioGroupItem("Salario Destajo y Comision", "Salario al Destajo y Comisión"), New DevExpress.XtraEditors.Controls.RadioGroupItem("Salario Fijo,Destajo y Comision", "Salario Fijo, Comisión, Destajo")})
        '
        'groupMoneda
        '
        Me.groupMoneda.Controls.Add(Me.RgMoneda)
        Me.groupMoneda.Controls.Add(Me.ChkMantValor)
        Me.groupMoneda.Location = New System.Drawing.Point(352, 130)
        Me.groupMoneda.Name = "groupMoneda"
        Me.groupMoneda.Size = New System.Drawing.Size(336, 135)
        Me.groupMoneda.TabIndex = 15
        Me.groupMoneda.Text = "Moneda"
        '
        'RgMoneda
        '
        Me.RgMoneda.Location = New System.Drawing.Point(10, 30)
        Me.RgMoneda.Name = "RgMoneda"
        Me.RgMoneda.Size = New System.Drawing.Size(316, 26)
        Me.RgMoneda.TabIndex = 16
        Me.RgMoneda.EditValue = "CS"
        Me.RgMoneda.Properties.Columns = 2
        Me.RgMoneda.Properties.Items.AddRange(New DevExpress.XtraEditors.Controls.RadioGroupItem() {New DevExpress.XtraEditors.Controls.RadioGroupItem("CS", "Córdobas"), New DevExpress.XtraEditors.Controls.RadioGroupItem("US", "Dólares")})
        '
        'ChkMantValor
        '
        Me.ChkMantValor.Location = New System.Drawing.Point(12, 70)
        Me.ChkMantValor.Name = "ChkMantValor"
        Me.ChkMantValor.Size = New System.Drawing.Size(250, 20)
        Me.ChkMantValor.TabIndex = 17
        Me.ChkMantValor.Properties.Caption = "Mantenimiento de Valor"
        '
        'groupInss
        '
        Me.groupInss.Controls.Add(Me.RgInss)
        Me.groupInss.Controls.Add(Me.lblInssLab)
        Me.groupInss.Controls.Add(Me.TxtInssLaboral)
        Me.groupInss.Controls.Add(Me.lblInssPat)
        Me.groupInss.Controls.Add(Me.TxtInssPatronal)
        Me.groupInss.Location = New System.Drawing.Point(12, 272)
        Me.groupInss.Name = "groupInss"
        Me.groupInss.Size = New System.Drawing.Size(330, 120)
        Me.groupInss.TabIndex = 18
        Me.groupInss.Text = "Cálculo del INSS"
        '
        'RgInss
        '
        Me.RgInss.Location = New System.Drawing.Point(10, 30)
        Me.RgInss.Name = "RgInss"
        Me.RgInss.Size = New System.Drawing.Size(310, 50)
        Me.RgInss.TabIndex = 19
        Me.RgInss.EditValue = False
        Me.RgInss.Properties.Columns = 1
        Me.RgInss.Properties.Items.AddRange(New DevExpress.XtraEditors.Controls.RadioGroupItem() {New DevExpress.XtraEditors.Controls.RadioGroupItem(True, "Calcular por Porcentaje"), New DevExpress.XtraEditors.Controls.RadioGroupItem(False, "Calcular por la Tabla")})
        '
        'lblInssLab
        '
        Me.lblInssLab.Location = New System.Drawing.Point(15, 92)
        Me.lblInssLab.Name = "lblInssLab"
        Me.lblInssLab.Size = New System.Drawing.Size(80, 13)
        Me.lblInssLab.TabIndex = 20
        Me.lblInssLab.Text = "% INSS Laboral:"
        '
        'TxtInssLaboral
        '
        Me.TxtInssLaboral.Location = New System.Drawing.Point(100, 89)
        Me.TxtInssLaboral.Name = "TxtInssLaboral"
        Me.TxtInssLaboral.Size = New System.Drawing.Size(60, 20)
        Me.TxtInssLaboral.TabIndex = 21
        Me.TxtInssLaboral.EditValue = 0
        Me.TxtInssLaboral.Properties.Mask.EditMask = "n2"
        Me.TxtInssLaboral.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtInssLaboral.Properties.Mask.UseMaskAsDisplayFormat = True
        '
        'lblInssPat
        '
        Me.lblInssPat.Location = New System.Drawing.Point(175, 92)
        Me.lblInssPat.Name = "lblInssPat"
        Me.lblInssPat.Size = New System.Drawing.Size(85, 13)
        Me.lblInssPat.TabIndex = 22
        Me.lblInssPat.Text = "% INSS Patronal:"
        '
        'TxtInssPatronal
        '
        Me.TxtInssPatronal.Location = New System.Drawing.Point(262, 89)
        Me.TxtInssPatronal.Name = "TxtInssPatronal"
        Me.TxtInssPatronal.Size = New System.Drawing.Size(58, 20)
        Me.TxtInssPatronal.TabIndex = 23
        Me.TxtInssPatronal.EditValue = 0
        Me.TxtInssPatronal.Properties.Mask.EditMask = "n2"
        Me.TxtInssPatronal.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtInssPatronal.Properties.Mask.UseMaskAsDisplayFormat = True
        '
        'groupIr
        '
        Me.groupIr.Controls.Add(Me.RgIr)
        Me.groupIr.Controls.Add(Me.lblIrPct)
        Me.groupIr.Controls.Add(Me.TxtIr)
        Me.groupIr.Location = New System.Drawing.Point(352, 272)
        Me.groupIr.Name = "groupIr"
        Me.groupIr.Size = New System.Drawing.Size(336, 120)
        Me.groupIr.TabIndex = 24
        Me.groupIr.Text = "Cálculo del IR"
        '
        'RgIr
        '
        Me.RgIr.Location = New System.Drawing.Point(10, 30)
        Me.RgIr.Name = "RgIr"
        Me.RgIr.Size = New System.Drawing.Size(316, 50)
        Me.RgIr.TabIndex = 25
        Me.RgIr.EditValue = False
        Me.RgIr.Properties.Columns = 1
        Me.RgIr.Properties.Items.AddRange(New DevExpress.XtraEditors.Controls.RadioGroupItem() {New DevExpress.XtraEditors.Controls.RadioGroupItem(True, "Calcular por Porcentaje"), New DevExpress.XtraEditors.Controls.RadioGroupItem(False, "Calcular por la Tabla")})
        '
        'lblIrPct
        '
        Me.lblIrPct.Location = New System.Drawing.Point(15, 92)
        Me.lblIrPct.Name = "lblIrPct"
        Me.lblIrPct.Size = New System.Drawing.Size(90, 13)
        Me.lblIrPct.TabIndex = 26
        Me.lblIrPct.Text = "Porcentaje a calcular:"
        '
        'TxtIr
        '
        Me.TxtIr.Location = New System.Drawing.Point(125, 89)
        Me.TxtIr.Name = "TxtIr"
        Me.TxtIr.Size = New System.Drawing.Size(60, 20)
        Me.TxtIr.TabIndex = 27
        Me.TxtIr.EditValue = 0
        Me.TxtIr.Properties.Mask.EditMask = "n2"
        Me.TxtIr.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric
        Me.TxtIr.Properties.Mask.UseMaskAsDisplayFormat = True
        '
        'groupOpc
        '
        Me.groupOpc.Controls.Add(Me.ChkEmpleadoCero)
        Me.groupOpc.Controls.Add(Me.ChkConstruccion)
        Me.groupOpc.Controls.Add(Me.ChkIrUltima)
        Me.groupOpc.Controls.Add(Me.ChkHorasTrabajadas)
        Me.groupOpc.Location = New System.Drawing.Point(12, 398)
        Me.groupOpc.Name = "groupOpc"
        Me.groupOpc.Size = New System.Drawing.Size(676, 72)
        Me.groupOpc.TabIndex = 28
        Me.groupOpc.Text = "Opciones"
        '
        'ChkEmpleadoCero
        '
        Me.ChkEmpleadoCero.Location = New System.Drawing.Point(12, 28)
        Me.ChkEmpleadoCero.Name = "ChkEmpleadoCero"
        Me.ChkEmpleadoCero.Size = New System.Drawing.Size(300, 20)
        Me.ChkEmpleadoCero.TabIndex = 29
        Me.ChkEmpleadoCero.Properties.Caption = "Mostrar empleados con salario cero"
        '
        'ChkConstruccion
        '
        Me.ChkConstruccion.Location = New System.Drawing.Point(340, 28)
        Me.ChkConstruccion.Name = "ChkConstruccion"
        Me.ChkConstruccion.Size = New System.Drawing.Size(300, 20)
        Me.ChkConstruccion.TabIndex = 30
        Me.ChkConstruccion.Properties.Caption = "Empleados de construcción"
        '
        'ChkIrUltima
        '
        Me.ChkIrUltima.Location = New System.Drawing.Point(12, 48)
        Me.ChkIrUltima.Name = "ChkIrUltima"
        Me.ChkIrUltima.Size = New System.Drawing.Size(300, 20)
        Me.ChkIrUltima.TabIndex = 31
        Me.ChkIrUltima.Properties.Caption = "Calcular IR en la última semana"
        '
        'ChkHorasTrabajadas
        '
        Me.ChkHorasTrabajadas.Location = New System.Drawing.Point(340, 48)
        Me.ChkHorasTrabajadas.Name = "ChkHorasTrabajadas"
        Me.ChkHorasTrabajadas.Size = New System.Drawing.Size(300, 20)
        Me.ChkHorasTrabajadas.TabIndex = 32
        Me.ChkHorasTrabajadas.Properties.Caption = "Calcular por horas trabajadas"
        '
        'BtnNuevo
        '
        Me.BtnNuevo.Location = New System.Drawing.Point(13, 478)
        Me.BtnNuevo.Name = "BtnNuevo"
        Me.BtnNuevo.Size = New System.Drawing.Size(95, 35)
        Me.BtnNuevo.TabIndex = 33
        Me.BtnNuevo.Text = "Nuevo"
        '
        'BtnGuardar
        '
        Me.BtnGuardar.Location = New System.Drawing.Point(119, 478)
        Me.BtnGuardar.Name = "BtnGuardar"
        Me.BtnGuardar.Size = New System.Drawing.Size(95, 35)
        Me.BtnGuardar.TabIndex = 34
        Me.BtnGuardar.Text = "Grabar"
        '
        'BtnEliminar
        '
        Me.BtnEliminar.Location = New System.Drawing.Point(225, 478)
        Me.BtnEliminar.Name = "BtnEliminar"
        Me.BtnEliminar.Size = New System.Drawing.Size(95, 35)
        Me.BtnEliminar.TabIndex = 35
        Me.BtnEliminar.Text = "Eliminar"
        '
        'BtnAnterior
        '
        Me.BtnAnterior.Location = New System.Drawing.Point(380, 478)
        Me.BtnAnterior.Name = "BtnAnterior"
        Me.BtnAnterior.Size = New System.Drawing.Size(95, 35)
        Me.BtnAnterior.TabIndex = 36
        Me.BtnAnterior.Text = "Anterior"
        '
        'BtnSiguiente
        '
        Me.BtnSiguiente.Location = New System.Drawing.Point(481, 478)
        Me.BtnSiguiente.Name = "BtnSiguiente"
        Me.BtnSiguiente.Size = New System.Drawing.Size(95, 35)
        Me.BtnSiguiente.TabIndex = 37
        Me.BtnSiguiente.Text = "Siguiente"
        '
        'BtnSalir
        '
        Me.BtnSalir.Location = New System.Drawing.Point(592, 478)
        Me.BtnSalir.Name = "BtnSalir"
        Me.BtnSalir.Size = New System.Drawing.Size(95, 35)
        Me.BtnSalir.TabIndex = 38
        Me.BtnSalir.Text = "Salir"
        '
        'btnCancelar
        '
        Me.btnCancelar.Location = New System.Drawing.Point(13, 478)
        Me.btnCancelar.Name = "btnCancelar"
        Me.btnCancelar.Size = New System.Drawing.Size(95, 35)
        Me.btnCancelar.TabIndex = 39
        Me.btnCancelar.Visible = False
        Me.btnCancelar.Text = "Cancelar"
        '
        'FrmTipoNomina
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(700, 525)
        Me.Controls.Add(Me.groupGenerales)
        Me.Controls.Add(Me.groupPago)
        Me.Controls.Add(Me.groupMoneda)
        Me.Controls.Add(Me.groupInss)
        Me.Controls.Add(Me.groupIr)
        Me.Controls.Add(Me.groupOpc)
        Me.Controls.Add(Me.BtnNuevo)
        Me.Controls.Add(Me.BtnGuardar)
        Me.Controls.Add(Me.BtnEliminar)
        Me.Controls.Add(Me.BtnAnterior)
        Me.Controls.Add(Me.BtnSiguiente)
        Me.Controls.Add(Me.BtnSalir)
        Me.Controls.Add(Me.btnCancelar)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmTipoNomina"
        Me.Text = "Tipos de Nómina"
        CType(Me.groupGenerales, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmbCodigo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmbCodigoView, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.CmbPeriodo.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtTarifa.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtHoras.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.groupPago, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RgTipoPago.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.groupMoneda, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RgMoneda.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ChkMantValor.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.groupInss, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RgInss.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtInssLaboral.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtInssPatronal.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.groupIr, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RgIr.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TxtIr.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.groupOpc, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ChkEmpleadoCero.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ChkConstruccion.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ChkIrUltima.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ChkHorasTrabajadas.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupGenerales.ResumeLayout(False)
        Me.groupGenerales.PerformLayout()
        Me.groupPago.ResumeLayout(False)
        Me.groupPago.PerformLayout()
        Me.groupMoneda.ResumeLayout(False)
        Me.groupMoneda.PerformLayout()
        Me.groupInss.ResumeLayout(False)
        Me.groupInss.PerformLayout()
        Me.groupIr.ResumeLayout(False)
        Me.groupIr.PerformLayout()
        Me.groupOpc.ResumeLayout(False)
        Me.groupOpc.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents groupGenerales As DevExpress.XtraEditors.GroupControl
    Friend WithEvents lblCodigo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents cmbCodigo As DevExpress.XtraEditors.SearchLookUpEdit
    Friend WithEvents cmbCodigoView As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents TxtCodigo As System.Windows.Forms.TextBox
    Friend WithEvents lblDesc As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtDescripcion As System.Windows.Forms.TextBox
    Friend WithEvents lblPeriodo As DevExpress.XtraEditors.LabelControl
    Friend WithEvents CmbPeriodo As DevExpress.XtraEditors.ComboBoxEdit
    Friend WithEvents lblTarifa As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtTarifa As DevExpress.XtraEditors.TextEdit
    Friend WithEvents lblHoras As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtHoras As DevExpress.XtraEditors.TextEdit
    Friend WithEvents groupPago As DevExpress.XtraEditors.GroupControl
    Friend WithEvents RgTipoPago As DevExpress.XtraEditors.RadioGroup
    Friend WithEvents groupMoneda As DevExpress.XtraEditors.GroupControl
    Friend WithEvents RgMoneda As DevExpress.XtraEditors.RadioGroup
    Friend WithEvents ChkMantValor As DevExpress.XtraEditors.CheckEdit
    Friend WithEvents groupInss As DevExpress.XtraEditors.GroupControl
    Friend WithEvents RgInss As DevExpress.XtraEditors.RadioGroup
    Friend WithEvents lblInssLab As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtInssLaboral As DevExpress.XtraEditors.TextEdit
    Friend WithEvents lblInssPat As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtInssPatronal As DevExpress.XtraEditors.TextEdit
    Friend WithEvents groupIr As DevExpress.XtraEditors.GroupControl
    Friend WithEvents RgIr As DevExpress.XtraEditors.RadioGroup
    Friend WithEvents lblIrPct As DevExpress.XtraEditors.LabelControl
    Friend WithEvents TxtIr As DevExpress.XtraEditors.TextEdit
    Friend WithEvents groupOpc As DevExpress.XtraEditors.GroupControl
    Friend WithEvents ChkEmpleadoCero As DevExpress.XtraEditors.CheckEdit
    Friend WithEvents ChkConstruccion As DevExpress.XtraEditors.CheckEdit
    Friend WithEvents ChkIrUltima As DevExpress.XtraEditors.CheckEdit
    Friend WithEvents ChkHorasTrabajadas As DevExpress.XtraEditors.CheckEdit
    Friend WithEvents BtnNuevo As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnGuardar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnEliminar As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnAnterior As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSiguiente As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents BtnSalir As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents btnCancelar As DevExpress.XtraEditors.SimpleButton
End Class
