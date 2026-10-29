Imports System.IO
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls

''' <summary>
''' Ficha de empleados. Todo el acceso a datos pasa por CsEmpleadosDAL
''' (consultas parametrizadas). "Eliminar" solo inactiva al empleado.
''' </summary>
Public Class FrmEmpleados

    Private ReadOnly _dal As New CsEmpleadosDAL

    ' Autonumérico (Empleado.CodEmpleado) del empleado cargado; 0 = empleado nuevo.
    Private _codEmpleado As Double = 0
    Private _fechaAntiguedad As String = ""
    Private _tieneFoto As Boolean = False
    Private _cargando As Boolean = False
    Private _modoNuevo As Boolean = False
    Private _listaEmpleados As DataTable

    ' Pestañas Préstamo / Incentivos / Deducciones / Subsidios: operan sobre el empleado ya guardado.
    Private _ucPrestamo As UcPrestamoEmpleado
    Private _ucIncentivos As UcMovimientosEmpleado
    Private _ucDeducciones As UcMovimientosEmpleado
    Private _ucSubsidios As UcMovimientosEmpleado

    Private ReadOnly Property EsNuevo As Boolean
        Get
            Return _modoNuevo
        End Get
    End Property

#Region "CARGA INICIAL"

    Private Sub FrmEmpleados_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            CrearPestanasMovimientos()
            ConfigurarLongitudes()
            CargarCatalogos()
            RefrescarListaEmpleados()
            Limpiar_Empleados()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    ''' <summary>Agrega las pestañas de movimientos (controles propios, fuera del diseñador).</summary>
    Private Sub CrearPestanasMovimientos()

        _ucPrestamo = New UcPrestamoEmpleado
        _ucIncentivos = New UcMovimientosEmpleado(TipoMovimiento.Incentivo)
        _ucDeducciones = New UcMovimientosEmpleado(TipoMovimiento.Deduccion)
        _ucSubsidios = New UcMovimientosEmpleado(TipoMovimiento.Subsidio)

        AgregarPestana("TabPrestamo", "Préstamo", _ucPrestamo)
        AgregarPestana("TabIncentivos", "Incentivos", _ucIncentivos)
        AgregarPestana("TabDeducciones", "Deducciones", _ucDeducciones)
        AgregarPestana("TabSubsidios", "Subsidios", _ucSubsidios)

    End Sub

    Private Sub AgregarPestana(nombre As String, texto As String, contenido As Control)

        Dim pagina As New DevExpress.XtraTab.XtraTabPage With {.Name = nombre, .Text = texto}

        pagina.Controls.Add(contenido)
        TabEmpleados.TabPages.Add(pagina)

    End Sub

    ''' <summary>Recarga las cuatro pestañas para el empleado cargado (0 = ninguno guardado).</summary>
    Private Sub CargarMovimientos()

        _ucPrestamo.Cargar(_codEmpleado)
        _ucIncentivos.Cargar(_codEmpleado)
        _ucDeducciones.Cargar(_codEmpleado)
        _ucSubsidios.Cargar(_codEmpleado)

    End Sub

    ''' <summary>Evita que un texto largo falle al guardar (tamaños reales de las columnas).</summary>
    Private Sub ConfigurarLongitudes()

        txtNombre1.MaxLength = 20
        txtNombre2.MaxLength = 20
        txtApellido.MaxLength = 20
        txtApellido2.MaxLength = 20
        txtDireccion.MaxLength = 200
        txtNacionalidad.MaxLength = 12
        txtCodigoPostal.MaxLength = 10
        txtNumeroCedula.MaxLength = 30
        txtNumeroInss.MaxLength = 20
        txtNumeroRuc.MaxLength = 20
        txtTelefono.MaxLength = 50
        txtNumeroCelular.MaxLength = 50
        txtCorreoElectronico.MaxLength = 50
        txtCuentaBanco.MaxLength = 50
        txtDescripOtrIngre.MaxLength = 20
        TxtCodEmpleado.MaxLength = 50
        txtMotivoBaja.MaxLength = 150
        txtMotivoAumento.MaxLength = 150
        txtMotivo.MaxLength = 150

        For Each t As TextBox In {txtCtaSueldos, txtCtaProvAguinaldo, txtCtaProvVacaciones,
                                  txtCtaInssPatronal, txtCtaInatec, txtCtaHorasExtra,
                                  txtCtaOtrosIngresos, txtCtaAguinaldo, txtCtaVacaciones,
                                  txtCtaInss, txtCtaIR, TxtCtaInssPagPatronal,
                                  txtCtaNominaPagar, txtINATECXpagar, txtCtaPrestamosxPagar,
                                  TxtCtaBanpro, TxtCtaBDF, TxtCtaLAFISE, txtCtaBAC}
            t.MaxLength = 50
        Next

    End Sub

    Private Sub CargarCatalogos()

        ConfigurarLookUp(cmbDepartamento, _dal.ListarDepartamentos(),
                         "CodDepartamento", "Departamento")

        Dim cargos As DataTable = _dal.ListarCargos()

        ConfigurarLookUp(cmbCargo, cargos, "CodCargo", "Cargo")

        ' Los cargos del histórico se guardan por nombre.
        ConfigurarLookUp(CboCargoInicialH, cargos, "Cargo", "Cargo")
        ConfigurarLookUp(cboCargoAnteriorH, cargos, "Cargo", "Cargo")
        ConfigurarLookUp(cboCargoActualH, cargos, "Cargo", "Cargo")

        With cmbTipoNomina.Properties
            .DataSource = _dal.ListarTiposNomina()
            .ValueMember = "CodTipoNomina"
            .DisplayMember = "CodTipoNomina"
        End With

        cmbExcentoInss.Text = "No"
        cmbExcentoIr.Text = "No"
        cmbSalarioMinimo.Text = "No"
        cmbPagoInssPatronal.Text = "No"

    End Sub

    Private Sub ConfigurarLookUp(
        combo As LookUpEdit,
        datos As DataTable,
        valor As String,
        texto As String)

        With combo.Properties
            .Columns.Clear()
            .Columns.Add(New LookUpColumnInfo(texto, ""))
            .ShowHeader = False
            .DataSource = datos
            .ValueMember = valor
            .DisplayMember = texto
            .NullText = ""
        End With

    End Sub

    ''' <summary>Recarga el buscador de empleados activos.</summary>
    Private Sub RefrescarListaEmpleados()

        _listaEmpleados = _dal.ListarParaCombo()

        With cmbCodigoEmpleado.Properties
            .DataSource = _listaEmpleados
            .ValueMember = "CodEmpleado1"
            .DisplayMember = "CodEmpleado1"
            .TextEditStyle = TextEditStyles.Standard
            .NullText = ""
        End With

    End Sub

#End Region

#Region "FORMULARIO -> ENTIDAD"

    Private Function ObtenerEmpleadoFormulario() As Empleados

        Dim emp As New Empleados

        '====================================================
        ' DATOS GENERALES
        '====================================================

        emp.Cod_Empleado = _codEmpleado
        emp.Cod_Empleado1 = CodigoDigitado()

        emp.Primer_Nombre = txtNombre1.Text.Trim
        emp.Segundo_Nombre = txtNombre2.Text.Trim
        emp.Primer_Apellido = txtApellido.Text.Trim
        emp.Segundo_Apellido = txtApellido2.Text.Trim
        emp.Empleado_Direccion = txtDireccion.Text.Trim
        emp.Empleado_Telefono = txtTelefono.Text.Trim
        emp.Empleado_Nacionalidad = txtNacionalidad.Text.Trim
        emp.Codigo_Postal = txtCodigoPostal.Text.Trim
        emp.Empleado_Sexo = SexoABd(cmbSexo.Text.Trim)
        emp.Numero_Ruc = txtNumeroRuc.Text.Trim
        emp.Num_Cedula = txtNumeroCedula.Text.Trim
        emp.Numero_Inss = txtNumeroInss.Text.Trim
        emp.Num_Hijos = txtnumeroHijos.Value
        emp.Empleado_Sindicalista = cmbSindicalista.Text.Trim
        emp.Cod_Departamento = Convert.ToString(cmbDepartamento.EditValue)
        emp.Cod_Cargo = Convert.ToString(cmbCargo.EditValue)
        emp.Empleado_CuentaBanco = txtCuentaBanco.Text.Trim
        emp.Correo_Electronico = txtCorreoElectronico.Text.Trim
        emp.Numero_Celular = txtNumeroCelular.Text.Trim

        ' Foto: se guarda solo si el usuario cargó una (la silueta es un marcador).
        emp.Empleado_Foto =
            If(_tieneFoto AndAlso Not IsNothing(ImgFotoEmpleado.Image),
               bytesToString(ImagenToBytes(ImgFotoEmpleado.Image)),
               "")

        '====================================================
        ' NOMINA
        '====================================================

        emp.Cod_TipoNomina = Convert.ToString(cmbTipoNomina.EditValue)
        emp.Dias_Descuento = Numero(txtDiasDescuento)
        emp.Sal_Porcentaje = Numero(txtSalarioPorciento)
        emp.Sueldo_Periodo = Numero(txtSueldoPeriodo)
        emp.Tarifa_Horaria = Numero(txtTarifaHoraria)
        emp.Porcentaje_Comision = Numero(txtComision)
        emp.Otros_Ingresos = Numero(txtOtrosIngresos)
        emp.Descrip_OtrIngre = txtDescripOtrIngre.Text.Trim

        emp.Exento_Inss = SiNo(cmbExcentoInss)
        emp.Exento_Ir = SiNo(cmbExcentoIr)
        emp.PagoInss_Patronal = SiNo(cmbPagoInssPatronal)
        emp.Salario_Minimo = SiNo(cmbSalarioMinimo)

        emp.Salario_Fijo = If(ChkSalarioFijo.Checked, "S", "N")
        emp.Sumar_Subsidio = If(ChkSubsidio.Checked, 1, 0)
        emp.Empleado_Dolarizado = If(ChkSalarioDolarizado.Checked, 1, 0)
        emp.SueldoActual_Basico = If(ChkSueldoActualBasico.Checked, 1, 0)
        emp.Deducir_Porciento = If(ChkDeducirPorcentaje.Checked, 1, 0)
        emp.Horas_Turno = If(ChkHorasTurno.Checked, 1, 0)

        emp.Dias_Basico = Numero(txtDiasBasico)
        emp.Aumento_Basico = Numero(txtAumentoBasico)
        emp.Viatico_Dia = Numero(txtViatico)
        emp.Empleado_Reembolso = Numero(txtReembolso)

        emp.Fecha_Antiguedad =
            If(_fechaAntiguedad <> "", _fechaAntiguedad, Now.ToString("yyyy-MM-dd HH:mm:ss"))

        emp.Empleado_Activo = 1
        emp.Empleado_Liquidado = 0
        emp.Empleado_Ausente = 0

        '====================================================
        ' HISTORICO
        '====================================================

        emp.Cargo_Inicial = Convert.ToString(CboCargoInicialH.EditValue)
        emp.Cargo_Actual = Convert.ToString(cboCargoActualH.EditValue)
        emp.Cargo_Anterior = Convert.ToString(cboCargoAnteriorH.EditValue)

        emp.Sueldo_Inicial = Numero(TxtSueldoInicial)
        emp.Sueldo_Anterior = Numero(TxtSueldoAnterior)
        emp.Sueldo_Actual = Numero(TxtSueldoActual)

        emp.Fecha_Contrato = LeerFecha(dtpFechaContrato)
        emp.Fecha_Nacimiento = LeerFecha(dtpFechaNacimiento)
        emp.Fecha_ContratoVac = LeerFecha(dtpFechaVacaciones)
        emp.Fecha_Baja = LeerFecha(CboFechaBaja)
        emp.Fecha_Aumento = LeerFecha(cboFechaAumento)
        emp.FechaInicial_Susp = LeerFecha(cboInicioSuspencion)
        emp.FechaFinal_Susp = LeerFecha(cboFinSuspencion)

        emp.Motivo_Baja = txtMotivoBaja.Text.Trim
        emp.Motivo_Aumento = txtMotivoAumento.Text.Trim
        emp.Motivo_Suspencion = txtMotivo.Text.Trim

        ' Cuentas de débito
        emp.Cuenta_Sueldos = txtCtaSueldos.Text.Trim
        emp.Prov_Aguinaldo = txtCtaProvAguinaldo.Text.Trim
        emp.Prov_Vacaciones = txtCtaProvVacaciones.Text.Trim
        emp.INSS_Patronal = txtCtaInssPatronal.Text.Trim
        emp.Empleado_INATEC = txtCtaInatec.Text.Trim
        emp.CuentaHoras_Extra = txtCtaHorasExtra.Text.Trim
        emp.CuentaOtros_Ingresos = txtCtaOtrosIngresos.Text.Trim

        ' Cuentas de crédito
        emp.Aguinaldo_Pagar = txtCtaAguinaldo.Text.Trim
        emp.Vacaciones_Pagar = txtCtaVacaciones.Text.Trim
        emp.INSS_Pagar = txtCtaInss.Text.Trim
        emp.IR_Pagar = txtCtaIR.Text.Trim
        emp.INSSPatronal_Pagar = TxtCtaInssPagPatronal.Text.Trim
        emp.Nomina_Pagar = txtCtaNominaPagar.Text.Trim
        emp.INATEC_Pagar = txtINATECXpagar.Text.Trim
        emp.Prestamo_Pagar = txtCtaPrestamosxPagar.Text.Trim

        ' Cuentas bancarias
        emp.CuentaBanco_BANPRO = TxtCtaBanpro.Text.Trim
        emp.CuentaBanco_BDF = TxtCtaBDF.Text.Trim
        emp.CuentaBanco_LAFISE = TxtCtaLAFISE.Text.Trim
        emp.Cuenta_Banco = txtCtaBAC.Text.Trim

        Return emp

    End Function

    ''' <summary>Código visible del empleado: el nuevo (si está en modo Nuevo) o el seleccionado.</summary>
    Private Function CodigoDigitado() As String

        If EsNuevo Then
            Return TxtCodEmpleado.Text.Trim
        End If

        Return Convert.ToString(cmbCodigoEmpleado.EditValue).Trim

    End Function

    Private Shared Function Numero(t As TextBox) As Double

        Dim v As Double

        If Double.TryParse(t.Text.Trim, v) Then Return v

        Return 0

    End Function

    ' En la BD el sexo se guarda como "M" / "F" (datos existentes).
    Private Shared Function SexoABd(texto As String) As String

        Select Case texto.ToUpperInvariant
            Case "MASCULINO" : Return "M"
            Case "FEMENINO" : Return "F"
            Case Else : Return texto
        End Select

    End Function

    Private Shared Function SexoAPantalla(valor As String) As String

        Select Case If(valor, "").Trim.ToUpperInvariant
            Case "M" : Return "Masculino"
            Case "F" : Return "Femenino"
            Case Else : Return If(valor, "")
        End Select

    End Function

    Private Shared Function SiNo(c As System.Windows.Forms.ComboBox) As Integer
        Return If(String.Equals(c.Text.Trim, "Si", StringComparison.OrdinalIgnoreCase), 1, 0)
    End Function

    ''' <summary>Fecha vacía => Date.MinValue (el DAL la guarda como NULL).</summary>
    Private Shared Function LeerFecha(d As DateEdit) As Date

        If IsNothing(d.EditValue) OrElse Convert.ToString(d.EditValue).Trim = "" Then
            Return Date.MinValue
        End If

        Return d.DateTime

    End Function

    Private Shared Sub PonerFecha(d As DateEdit, valor As Date)

        If valor = Date.MinValue OrElse valor.Year < 1900 Then
            d.EditValue = Nothing
        Else
            d.DateTime = valor
        End If

    End Sub

#End Region

#Region "ENTIDAD -> FORMULARIO"

    Private Sub LlenarFormulario(emp As Empleados)

        _cargando = True

        Try

            _codEmpleado = emp.Cod_Empleado
            _fechaAntiguedad = If(IsDate(emp.Fecha_Antiguedad), emp.Fecha_Antiguedad, "")

            ' Generales
            TxtCodEmpleado.Text = emp.Cod_Empleado1
            cmbCodigoEmpleado.EditValue = emp.Cod_Empleado1
            txtNombre1.Text = emp.Primer_Nombre
            txtNombre2.Text = emp.Segundo_Nombre
            txtApellido.Text = emp.Primer_Apellido
            txtApellido2.Text = emp.Segundo_Apellido
            txtDireccion.Text = emp.Empleado_Direccion
            txtTelefono.Text = emp.Empleado_Telefono
            txtNacionalidad.Text = emp.Empleado_Nacionalidad
            txtCodigoPostal.Text = emp.Codigo_Postal
            cmbSexo.Text = SexoAPantalla(emp.Empleado_Sexo)
            txtNumeroRuc.Text = emp.Numero_Ruc
            txtNumeroCedula.Text = emp.Num_Cedula
            txtNumeroInss.Text = emp.Numero_Inss
            txtnumeroHijos.Value = Math.Max(txtnumeroHijos.Minimum,
                                   Math.Min(txtnumeroHijos.Maximum, Convert.ToDecimal(emp.Num_Hijos)))
            cmbSindicalista.Text = emp.Empleado_Sindicalista
            cmbDepartamento.EditValue = emp.Cod_Departamento
            cmbCargo.EditValue = emp.Cod_Cargo
            txtCuentaBanco.Text = emp.Empleado_CuentaBanco
            txtCorreoElectronico.Text = emp.Correo_Electronico
            txtNumeroCelular.Text = emp.Numero_Celular

            ' Nómina
            cmbTipoNomina.EditValue = emp.Cod_TipoNomina
            txtDiasDescuento.Text = emp.Dias_Descuento.ToString
            txtSalarioPorciento.Text = emp.Sal_Porcentaje.ToString
            txtSueldoPeriodo.Text = emp.Sueldo_Periodo.ToString
            txtTarifaHoraria.Text = emp.Tarifa_Horaria.ToString
            txtComision.Text = emp.Porcentaje_Comision.ToString
            txtOtrosIngresos.Text = emp.Otros_Ingresos.ToString
            txtDescripOtrIngre.Text = emp.Descrip_OtrIngre
            txtDiasBasico.Text = emp.Dias_Basico.ToString
            txtAumentoBasico.Text = emp.Aumento_Basico.ToString
            txtViatico.Text = emp.Viatico_Dia.ToString
            txtReembolso.Text = emp.Empleado_Reembolso.ToString

            cmbExcentoInss.Text = If(emp.Exento_Inss <> 0, "Si", "No")
            cmbExcentoIr.Text = If(emp.Exento_Ir <> 0, "Si", "No")
            cmbPagoInssPatronal.Text = If(emp.PagoInss_Patronal <> 0, "Si", "No")
            cmbSalarioMinimo.Text = If(emp.Salario_Minimo <> 0, "Si", "No")

            ChkSalarioFijo.Checked = (emp.Salario_Fijo = "S")
            ChkSubsidio.Checked = (emp.Sumar_Subsidio <> 0)
            ChkSalarioDolarizado.Checked = (emp.Empleado_Dolarizado <> 0)
            ChkSueldoActualBasico.Checked = (emp.SueldoActual_Basico <> 0)
            ChkDeducirPorcentaje.Checked = (emp.Deducir_Porciento <> 0)
            ChkHorasTurno.Checked = (emp.Horas_Turno <> 0)

            ' Histórico
            CboCargoInicialH.EditValue = emp.Cargo_Inicial
            cboCargoActualH.EditValue = emp.Cargo_Actual
            cboCargoAnteriorH.EditValue = emp.Cargo_Anterior

            TxtSueldoInicial.Text = emp.Sueldo_Inicial.ToString
            TxtSueldoAnterior.Text = emp.Sueldo_Anterior.ToString
            TxtSueldoActual.Text = emp.Sueldo_Actual.ToString

            PonerFecha(dtpFechaContrato, emp.Fecha_Contrato)
            PonerFecha(dtpFechaNacimiento, emp.Fecha_Nacimiento)
            PonerFecha(dtpFechaVacaciones, emp.Fecha_ContratoVac)
            PonerFecha(CboFechaBaja, emp.Fecha_Baja)
            PonerFecha(cboFechaAumento, emp.Fecha_Aumento)
            PonerFecha(cboInicioSuspencion, emp.FechaInicial_Susp)
            PonerFecha(cboFinSuspencion, emp.FechaFinal_Susp)

            txtMotivoBaja.Text = emp.Motivo_Baja
            txtMotivoAumento.Text = emp.Motivo_Aumento
            txtMotivo.Text = emp.Motivo_Suspencion

            ' Cuentas
            txtCtaSueldos.Text = emp.Cuenta_Sueldos
            txtCtaProvAguinaldo.Text = emp.Prov_Aguinaldo
            txtCtaProvVacaciones.Text = emp.Prov_Vacaciones
            txtCtaInssPatronal.Text = emp.INSS_Patronal
            txtCtaInatec.Text = emp.Empleado_INATEC
            txtCtaHorasExtra.Text = emp.CuentaHoras_Extra
            txtCtaOtrosIngresos.Text = emp.CuentaOtros_Ingresos

            txtCtaAguinaldo.Text = emp.Aguinaldo_Pagar
            txtCtaVacaciones.Text = emp.Vacaciones_Pagar
            txtCtaInss.Text = emp.INSS_Pagar
            txtCtaIR.Text = emp.IR_Pagar
            TxtCtaInssPagPatronal.Text = emp.INSSPatronal_Pagar
            txtCtaNominaPagar.Text = emp.Nomina_Pagar
            txtINATECXpagar.Text = emp.INATEC_Pagar
            txtCtaPrestamosxPagar.Text = emp.Prestamo_Pagar

            TxtCtaBanpro.Text = emp.CuentaBanco_BANPRO
            TxtCtaBDF.Text = emp.CuentaBanco_BDF
            TxtCtaLAFISE.Text = emp.CuentaBanco_LAFISE
            txtCtaBAC.Text = emp.Cuenta_Banco

            MostrarFoto(emp.Empleado_Foto)

            Me.Text = If(emp.Empleado_Activo = 0, "Empleados - INACTIVO", "Empleados")

            CargarMovimientos()

        Finally
            _cargando = False
        End Try

    End Sub

    Private Sub MostrarFoto(foto As String)

        Dim img As Image = Nothing

        If Not String.IsNullOrWhiteSpace(foto) Then
            img = BytesToImagen(StringtoByte(foto))
        End If

        _tieneFoto = Not IsNothing(img)
        ImgFotoEmpleado.Image = If(_tieneFoto, img, My.Resources.Silueta)

    End Sub

#End Region

#Region "BUSCAR / NAVEGAR"

    ''' <summary>Carga un empleado por su código visible (CodEmpleado1).</summary>
    Public Sub CargarEmpleado(CodigoEmpleado As String)

        Try

            Dim emp As Empleados = _dal.Buscar(CodigoEmpleado)

            If IsNothing(emp) Then
                MessageBox.Show("No existe el empleado " & CodigoEmpleado & ".",
                                "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            SalirModoNuevo()
            LlenarFormulario(emp)
            ActualizarBotones()

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub cmbCodigoEmpleado_EditValueChanged(sender As Object, e As EventArgs) _
        Handles cmbCodigoEmpleado.EditValueChanged

        If _cargando OrElse EsNuevo Then Exit Sub

        Dim codigo As String = Convert.ToString(cmbCodigoEmpleado.EditValue).Trim

        If codigo = "" Then Exit Sub

        ' Solo carga si el código está en la lista (evita cargar mientras se escribe).
        If Not IsNothing(_listaEmpleados) AndAlso
           _listaEmpleados.Select("CodEmpleado1 = '" & codigo.Replace("'", "''") & "'").Length > 0 Then
            CargarEmpleado(codigo)
        End If

    End Sub

    ''' <summary>Al elegir el tipo de nómina se llena "Tipo Pago" con el de esa nómina.</summary>
    Private Sub cmbTipoNomina_EditValueChanged(sender As Object, e As EventArgs) _
        Handles cmbTipoNomina.EditValueChanged

        Dim codigo As String = Convert.ToString(cmbTipoNomina.EditValue)
        Dim tipoPago As String = ""

        Dim tabla As DataTable = TryCast(cmbTipoNomina.Properties.DataSource, DataTable)

        If Not IsNothing(tabla) AndAlso codigo <> "" Then
            Dim filas() As DataRow =
                tabla.Select("CodTipoNomina = '" & codigo.Replace("'", "''") & "'")

            If filas.Length > 0 Then tipoPago = Convert.ToString(filas(0)("TipoPago"))
        End If

        cmbTipoPago.Text = tipoPago

    End Sub

    Private Sub BtnBuscarEmpleado_Click(sender As Object, e As EventArgs) Handles BtnBuscarEmpleado.Click

        Dim codigo As String = CodigoDigitado()

        If codigo = "" Then
            MessageBox.Show("Digite o seleccione el código del empleado.",
                            "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        CargarEmpleado(codigo)

    End Sub

    Private Sub BtnSiguiente_Click(sender As Object, e As EventArgs) Handles BtnSiguiente.Click
        Mover(1)
    End Sub

    Private Sub BtnAnterior_Click(sender As Object, e As EventArgs) Handles BtnAnterior.Click
        Mover(-1)
    End Sub

    Private Sub Mover(paso As Integer)

        If IsNothing(_listaEmpleados) OrElse _listaEmpleados.Rows.Count = 0 Then Exit Sub

        Dim actual As String = Convert.ToString(cmbCodigoEmpleado.EditValue).Trim
        Dim indice As Integer = -1

        For i As Integer = 0 To _listaEmpleados.Rows.Count - 1
            If Convert.ToString(_listaEmpleados.Rows(i)("CodEmpleado1")).Trim = actual Then
                indice = i
                Exit For
            End If
        Next

        indice = If(indice = -1,
                    If(paso > 0, 0, _listaEmpleados.Rows.Count - 1),
                    indice + paso)

        If indice < 0 OrElse indice >= _listaEmpleados.Rows.Count Then Exit Sub

        CargarEmpleado(Convert.ToString(_listaEmpleados.Rows(indice)("CodEmpleado1")).Trim)

    End Sub

#End Region

#Region "NUEVO / CANCELAR / LIMPIAR"

    Private Sub BtnNuevo_Click(sender As Object, e As EventArgs) Handles BtnNuevo.Click
        NuevoEmpleado()
    End Sub

    ''' <summary>Deja la ficha lista para un empleado nuevo (la usa también el listado de empleados).</summary>
    Public Sub NuevoEmpleado()

        Try
            Limpiar_Empleados()

            _modoNuevo = True
            btnCancelar.Visible = True
            TxtCodEmpleado.Visible = True
            TxtCodEmpleado.BringToFront()
            TxtCodEmpleado.Text = _dal.ProximoCodigo()
            TxtCodEmpleado.Focus()

            ActualizarBotones()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnNumeroDisponible_Click(sender As Object, e As EventArgs) Handles BtnNumeroDisponible.Click

        If Not EsNuevo Then Exit Sub

        Try
            TxtCodEmpleado.Text = _dal.ProximoCodigo()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        Limpiar_Empleados()
    End Sub

    Private Sub SalirModoNuevo()
        _modoNuevo = False
        btnCancelar.Visible = False
        TxtCodEmpleado.Visible = False
    End Sub

    Private Sub Limpiar_Empleados()

        _cargando = True

        Try

            SalirModoNuevo()

            _codEmpleado = 0
            _fechaAntiguedad = ""
            Me.Text = "Empleados"

            TxtCodEmpleado.Text = ""
            cmbCodigoEmpleado.EditValue = Nothing

            For Each t As TextBox In {txtNombre1, txtNombre2, txtApellido, txtApellido2, txtDireccion,
                                      txtTelefono, txtCodigoPostal, txtNumeroRuc, txtNumeroCedula,
                                      txtNumeroInss, txtCorreoElectronico, txtCuentaBanco,
                                      txtNumeroCelular, txtDescripOtrIngre,
                                      txtMotivoBaja, txtMotivoAumento, txtMotivo,
                                      txtCtaSueldos, txtCtaProvAguinaldo, txtCtaProvVacaciones,
                                      txtCtaInssPatronal, txtCtaInatec, txtCtaHorasExtra,
                                      txtCtaOtrosIngresos, txtCtaAguinaldo, txtCtaVacaciones,
                                      txtCtaInss, txtCtaIR, TxtCtaInssPagPatronal,
                                      txtCtaNominaPagar, txtINATECXpagar, txtCtaPrestamosxPagar,
                                      TxtCtaBanpro, TxtCtaBDF, TxtCtaLAFISE, txtCtaBAC}
                t.Text = ""
            Next

            For Each t As TextBox In {txtDiasDescuento, txtSalarioPorciento, txtSueldoPeriodo,
                                      txtTarifaHoraria, txtComision, txtOtrosIngresos,
                                      txtDiasBasico, txtAumentoBasico, txtViatico, txtReembolso,
                                      TxtSueldoInicial, TxtSueldoAnterior, TxtSueldoActual}
                t.Text = "0"
            Next

            txtNacionalidad.Text = "Nicaraguense"
            txtnumeroHijos.Value = 0
            cmbSexo.Text = ""
            cmbSindicalista.Text = "No"

            cmbDepartamento.EditValue = Nothing
            cmbCargo.EditValue = Nothing
            cmbTipoNomina.EditValue = Nothing
            CboCargoInicialH.EditValue = Nothing
            cboCargoActualH.EditValue = Nothing
            cboCargoAnteriorH.EditValue = Nothing

            cmbExcentoInss.Text = "No"
            cmbExcentoIr.Text = "No"
            cmbSalarioMinimo.Text = "No"
            cmbPagoInssPatronal.Text = "No"

            For Each c As CheckEdit In {ChkSubsidio, ChkSalarioDolarizado, ChkSalarioFijo,
                                        ChkSueldoActualBasico, ChkDeducirPorcentaje, ChkHorasTurno}
                c.Checked = False
            Next

            dtpFechaContrato.DateTime = Today
            dtpFechaNacimiento.EditValue = Nothing
            dtpFechaVacaciones.EditValue = Nothing
            CboFechaBaja.EditValue = Nothing
            cboFechaAumento.EditValue = Nothing
            cboInicioSuspencion.EditValue = Nothing
            cboFinSuspencion.EditValue = Nothing

            MostrarFoto("")

            CargarMovimientos()

        Finally
            _cargando = False
        End Try

        ActualizarBotones()

    End Sub

    Private Sub ActualizarBotones()
        ' Solo se puede anular un empleado ya guardado.
        BtnAnular.Enabled = (_codEmpleado > 0)
    End Sub

#End Region

#Region "GUARDAR"

    Private Sub BtnGuardar_Click(sender As Object, e As EventArgs) Handles BtnGuardar.Click

        Try

            Cursor = Cursors.WaitCursor

            If Not Validar() Then Exit Sub

            Dim emp As Empleados = ObtenerEmpleadoFormulario()

            _dal.Guardar(emp)

            MessageBox.Show("Empleado guardado correctamente.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Refresca el buscador y deja el empleado guardado en pantalla.
            Dim codigo As String = emp.Cod_Empleado1

            _cargando = True
            Try
                RefrescarListaEmpleados()
            Finally
                _cargando = False
            End Try

            CargarEmpleado(codigo)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Function Validar() As Boolean

        If CodigoDigitado() = "" Then
            Return Avisar("Debe ingresar el código del empleado.", If(EsNuevo, TxtCodEmpleado, cmbCodigoEmpleado))
        End If

        If txtNombre1.Text.Trim = "" Then
            Return Avisar("Debe ingresar el primer nombre.", txtNombre1)
        End If

        If txtApellido.Text.Trim = "" Then
            Return Avisar("Debe ingresar el primer apellido.", txtApellido)
        End If

        If IsNothing(cmbDepartamento.EditValue) OrElse Convert.ToString(cmbDepartamento.EditValue) = "" Then
            Return Avisar("Debe seleccionar el departamento.", cmbDepartamento)
        End If

        If IsNothing(cmbCargo.EditValue) OrElse Convert.ToString(cmbCargo.EditValue) = "" Then
            Return Avisar("Debe seleccionar el cargo.", cmbCargo)
        End If

        If IsNothing(cmbTipoNomina.EditValue) OrElse Convert.ToString(cmbTipoNomina.EditValue) = "" Then
            Return Avisar("Debe seleccionar el tipo de nómina.", cmbTipoNomina)
        End If

        Return True

    End Function

    Private Shared Function Avisar(mensaje As String, control As Control) As Boolean

        MessageBox.Show(mensaje, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        control.Focus()

        Return False

    End Function

#End Region

#Region "ANULAR (INACTIVAR)"

    ''' <summary>No borra el registro: solo lo marca como inactivo.</summary>
    Private Sub BtnAnular_Click(sender As Object, e As EventArgs) Handles BtnAnular.Click

        If _codEmpleado <= 0 Then
            MessageBox.Show("Seleccione un empleado guardado.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Dim nombre As String = (txtNombre1.Text & " " & txtApellido.Text).Trim

        If MessageBox.Show("¿Desea inactivar al empleado " & nombre & "?" & vbCrLf &
                           "El registro no se elimina; solo dejará de aparecer como activo.",
                           "Zeus Nominas", MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try

            Cursor = Cursors.WaitCursor

            _dal.Eliminar(Convert.ToInt32(_codEmpleado))

            _cargando = True
            Try
                RefrescarListaEmpleados()
            Finally
                _cargando = False
            End Try

            Limpiar_Empleados()

            MessageBox.Show("Empleado inactivado.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

#End Region

#Region "FOTO"

    Private Sub BtnAgregar_Click(sender As Object, e As EventArgs) Handles BtnAgregar.Click

        OpenFileDialog1.Title = "Seleccione la foto del empleado"
        OpenFileDialog1.Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Todos los archivos|*.*"
        OpenFileDialog1.FileName = ""

        If OpenFileDialog1.ShowDialog() <> DialogResult.OK Then Exit Sub

        Try

            ' Se copia a memoria para no dejar el archivo bloqueado.
            Using original As Image = Image.FromStream(
                    New MemoryStream(File.ReadAllBytes(OpenFileDialog1.FileName)))

                ImgFotoEmpleado.Image = CambiarTamañoImage(original)
            End Using

            _tieneFoto = True

        Catch ex As Exception
            MessageBox.Show("No se pudo cargar la imagen: " & ex.Message, "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try

    End Sub

    ''' <summary>Quita la foto de la pantalla; se borra de la BD al grabar.</summary>
    Private Sub BtnBorrar_Click(sender As Object, e As EventArgs) Handles BtnBorrar.Click

        If Not _tieneFoto Then Exit Sub

        If MessageBox.Show("¿Está seguro de quitar la foto?", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        MostrarFoto("")

    End Sub

#End Region

#Region "ANOTACIONES / CARNET"

    Private Sub BtnAnotar_Click(sender As Object, e As EventArgs) Handles BtnAnotar.Click
        AbrirAnotaciones()
    End Sub

    Private Sub BtnCarnet_Click(sender As Object, e As EventArgs) Handles BtnCarnet.Click
        AbrirCarnet()
    End Sub

    ''' <summary>Abre las anotaciones (currículum) del empleado que está en pantalla.</summary>
    Public Sub AbrirAnotaciones()

        If Not HayEmpleadoGuardado() Then Exit Sub

        Using f As New FrmAnotaciones
            f.CargarEmpleado(Convert.ToDecimal(_codEmpleado), TxtCodEmpleado.Text)
            f.ShowDialog(Me)
        End Using

    End Sub

    ''' <summary>Abre el carnet del empleado que está en pantalla.</summary>
    Public Sub AbrirCarnet()

        If Not HayEmpleadoGuardado() Then Exit Sub

        Using f As New FrmCarnet
            f.CargarEmpleado(Convert.ToDecimal(_codEmpleado))
            f.ShowDialog(Me)
        End Using

    End Sub

    Private Function HayEmpleadoGuardado() As Boolean

        If _codEmpleado > 0 Then Return True

        MessageBox.Show("Seleccione o guarde primero un empleado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Return False

    End Function

#End Region
    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Me.Close()
    End Sub

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
