Public Class FrmEmpleados
    Dim ConsultasSQl As New ConsultasSQL
    Dim Cod_Empleado As Double = 0
    Public Sub CargarEmpleado(CodigoEmpleado As String)
        Dim TablaEmpleados As New Empleados, TablaDepartamento As New Departamento, TablaCargo As New Cargos

        Dim CodDepartamento As String = "", CodCargo As String = ""

        TablaEmpleados = ConsultasSQl.BuscarEmpleado(CodigoEmpleado)
        If Not IsNothing(TablaEmpleados) Then
            txtNombre1.Text = TablaEmpleados.Primer_Nombre
            txtNombre2.Text = TablaEmpleados.Segundo_Apellido
            txtApellido.Text = TablaEmpleados.Primer_Apellido
            txtApellido2.Text = TablaEmpleados.Segundo_Apellido
            txtDireccion.Text = TablaEmpleados.Empleado_Direccion
            txtTelefono.Text = TablaEmpleados.Empleado_Telefono
            txtNacionalidad.Text = TablaEmpleados.Empleado_Nacionalidad
            txtCodigoPostal.Text = TablaEmpleados.Codigo_Postal
            cmbSexo.Text = TablaEmpleados.Empleado_Sexo
            txtNumeroRuc.Text = TablaEmpleados.Numero_Ruc
            txtNumeroCedula.Text = TablaEmpleados.Num_Cedula
            txtNumeroInss.Text = TablaEmpleados.Numero_Inss
            txtnumeroHijos.Value = TablaEmpleados.Num_Hijos
            cmbSindicalista.Text = TablaEmpleados.Empleado_Sindicalista
            CodDepartamento = TablaEmpleados.Cod_Departamento
            CodCargo = TablaEmpleados.Cod_Cargo
            txtCuentaBanco.Text = TablaEmpleados.Empleado_CuentaBanco
            Cod_Empleado = TablaEmpleados.Cod_Empleado
            '///////////////BUSCO EN NOMBRE DEL DEPARTAMENTO /////////////////////

            TablaDepartamento = ConsultasSQl.BuscarDepartamento(CodDepartamento)
            If Not IsDBNull(TablaDepartamento) Then
                cmbDepartamento.Text = TablaDepartamento.Descripcion_Departamento
            End If

            TablaCargo = ConsultasSQl.BuscarCargo(CodCargo)
            If Not IsDBNull(TablaCargo) Then
                cmbCargo.Text = TablaCargo.Descripcion_Cargo
            End If
        Else
            MsgBox("No Existe el Empleado", vbOK, "Zeus Nominas")

        End If
    End Sub



    Private Sub Limpiar_Empleados()
        Me.TxtCodEmpleado.Text = ""
        Me.cmbCodigoEmpleado.Text = ""
        txtNombre1.Text = ""
        txtNombre2.Text = ""
        txtApellido.Text = ""
        txtApellido2.Text = ""
        txtDireccion.Text = ""
        txtTelefono.Text = ""
        txtNacionalidad.Text = ""
        txtCodigoPostal.Text = ""
        cmbSexo.Text = ""
        txtNumeroRuc.Text = ""
        txtNumeroCedula.Text = ""
        txtNumeroInss.Text = ""
        txtnumeroHijos.Value = 0
        cmbSindicalista.Text = ""
        Me.txtCorreoElectronico.Text = ""
        Me.txtCuentaBanco.Text = ""
        Me.ChkSubsidio.Checked = False
        Me.ChkSalarioDolarizado.Checked = False

    End Sub

    Private Sub FrmEmpleados_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cmbCodigoEmpleado.Properties.DataSource = ConsultasSQl.CaseListadoSistemaNomina("ComboEmpleados")
        cmbCodigoEmpleado.Properties.DisplayMember = "CodEmpleado1"
        cmbCodigoEmpleado.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard

        cmbDepartamento.Properties.DataSource = ConsultasSQl.CaseListadoSistemaNomina("ListadoDepartamento")
        cmbDepartamento.Properties.DisplayMember = "Departamento"

        cmbCargo.Properties.DataSource = ConsultasSQl.CaseListadoSistemaNomina("ListadoCargo")
        cmbCargo.Properties.DisplayMember = "Cargo"

        cmbTipoNomina.Properties.DataSource = ConsultasSQl.CaseListadoSistemaNomina("ListadoTipoNomina")
        cmbTipoNomina.Properties.DisplayMember = "CodTipoNomina"
        cmbTipoNomina.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard

        cmbExcentoInss.Text = "No"
        cmbExcentoIr.Text = "No"
        cmbSalarioMinimo.Text = "No"
        cmbPagoInssPatronal.Text = "No"

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Me.Close()
    End Sub

    Private Sub simpleButton16_Click(sender As Object, e As EventArgs) Handles BtnNuevo.Click

        Me.BtnCancelar.Visible = True
        Me.TxtCodEmpleado.Visible = True
        Limpiar_Empleados()


    End Sub

    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        Me.btnCancelar.Visible = False
        Me.TxtCodEmpleado.Visible = False
        Limpiar_Empleados()
    End Sub

    Private Sub cmbCodigoEmpleado_EditValueChanged(sender As Object, e As EventArgs) Handles cmbCodigoEmpleado.EditValueChanged
        CargarEmpleado(cmbCodigoEmpleado.Text)
    End Sub

    Private Sub BtnGuardar_Click(sender As Object, e As EventArgs) Handles BtnGuardar.Click
        Dim TablaEmpleados As New Empleados, Codigo_Empleado As String = ""
        Dim TablaHistorico As New Historico

        If TxtCodEmpleado.Text <> "" Then
            Codigo_Empleado = Me.TxtCodEmpleado.Text
        ElseIf cmbCodigoEmpleado.Text <> "" Then
            Codigo_Empleado = cmbCodigoEmpleado.Text
        End If

        If Me.cmbTipoNomina.Text = "" Then
            MsgBox("Es Necesario el tipo de nomina", MsgBoxStyle.Critical, "Zeus Nominas")
            Exit Sub
        End If

        TablaEmpleados.Cod_Empleado = Cod_Empleado
        TablaEmpleados.Cod_Empleado1 = Codigo_Empleado
        TablaEmpleados.Primer_Nombre = txtNombre1.Text
        TablaEmpleados.Segundo_Apellido = txtNombre2.Text
        TablaEmpleados.Primer_Apellido = txtApellido.Text
        TablaEmpleados.Segundo_Apellido = txtApellido2.Text
        TablaEmpleados.Empleado_Direccion = txtDireccion.Text
        TablaEmpleados.Empleado_Telefono = txtTelefono.Text
        TablaEmpleados.Empleado_Nacionalidad = txtNacionalidad.Text
        TablaEmpleados.Codigo_Postal = txtCodigoPostal.Text
        TablaEmpleados.Empleado_Sexo = cmbSexo.Text
        TablaEmpleados.Numero_Ruc = txtNumeroRuc.Text
        TablaEmpleados.Num_Cedula = txtNumeroCedula.Text
        TablaEmpleados.Numero_Inss = txtNumeroInss.Text
        TablaEmpleados.Num_Hijos = txtnumeroHijos.Value
        TablaEmpleados.Empleado_Sindicalista = cmbSindicalista.Text
        TablaEmpleados.Cod_Departamento = cmbDepartamento.Text
        TablaEmpleados.Cod_Cargo = cmbCargo.Text
        TablaEmpleados.Empleado_CuentaBanco = txtCuentaBanco.Text
        TablaEmpleados.Empleado_Telefono = Me.txtTelefono.Text
        TablaEmpleados.Codigo_Postal = Me.txtCodigoPostal.Text
        TablaEmpleados.Cod_TipoNomina = cmbTipoNomina.Text
        TablaEmpleados.Dias_Descuento = txtDiasDescuento.Text
        TablaEmpleados.Sal_Porcentaje = txtSalarioPorciento.Text
        TablaEmpleados.Dias_Descuento = txtDiasDescuento.Text
        TablaEmpleados.Sueldo_Periodo = txtSueldoPeriodo.Text
        TablaEmpleados.Tarifa_Horaria = txtTarifaHoraria.Text
        TablaEmpleados.Porcentaje_Comision = txtComision.Text
        TablaEmpleados.Otros_Ingresos = txtOtrosIngresos.Text
        TablaEmpleados.Descrip_OtrIngre = txtDescripOtrIngre.Text

        If Me.cmbExcentoInss.Text = "Si" Then
            TablaEmpleados.Exento_Inss = 1
        Else
            TablaEmpleados.Exento_Inss = 0
        End If

        If cmbExcentoIr.Text = "Si" Then
            TablaEmpleados.Exento_Ir = 1
        Else
            TablaEmpleados.Exento_Ir = 0
        End If

        If cmbPagoInssPatronal.Text = "Si" Then
            TablaEmpleados.PagoInss_Patronal = 1
        Else
            TablaEmpleados.PagoInss_Patronal = 0
        End If

        If cmbSalarioMinimo.Text = "Si" Then
            TablaEmpleados.Salario_Minimo = 1
        Else
            TablaEmpleados.Salario_Minimo = 0
        End If

        TablaEmpleados.Empleado_Activo = 1
        TablaEmpleados.Empleado_Liquidado = 0
        TablaEmpleados.Empleado_Ausente = 0

        If ChkSalarioFijo.Checked = True Then
            TablaEmpleados.Salario_Fijo = "S"
        Else
            TablaEmpleados.Salario_Fijo = "N"
        End If

        TablaEmpleados.Sumar_Subsidio = 0
        TablaEmpleados.Sal_Porcentaje = txtSalarioPorciento.Text
        TablaEmpleados.Porciento_Incentivo = 0
        TablaEmpleados.Empleado_Gravidez = 0
        TablaEmpleados.Vacaciones_Basico = 0

        If ChkSalarioDolarizado.Checked = True Then
            TablaEmpleados.Empleado_Dolarizado = 1
        Else
            TablaEmpleados.Empleado_Dolarizado = 0
        End If

        TablaEmpleados.Fecha_Antiguedad = Format(Now, "dd/MM/yyyy")
        TablaEmpleados.Empleado_Antiguedad = 0
        TablaEmpleados.Empleado_CuentaBanco = txtCuentaBanco.Text
        TablaEmpleados.Correo_Electronico = txtCorreoElectronico.Text

        If ChkSueldoActualBasico.Checked = True Then
            TablaEmpleados.SueldoActual_Basico = 1
        Else
            TablaEmpleados.SueldoActual_Basico = 0
        End If

        TablaEmpleados.Dias_Basico = txtDiasBasico.Text
        TablaEmpleados.Aumento_Basico = txtAumentoBasico.Text





        If IsNothing(ConsultasSQl.BuscarEmpleado(Codigo_Empleado)) Then
            ConsultasSQl.InsertarEmpleados(TablaEmpleados)
        Else
            ConsultasSQl.ActualizarEmpleado(TablaEmpleados)
        End If

        Limpiar_Empleados()

    End Sub

    Private Sub TxtCodEmpleado_TextChanged(sender As Object, e As EventArgs) Handles TxtCodEmpleado.TextChanged

    End Sub

    Private Sub TabGenerales_Paint(sender As Object, e As PaintEventArgs) Handles TabGenerales.Paint

    End Sub

    Private Sub labelControl21_Click(sender As Object, e As EventArgs) Handles labelControl21.Click

    End Sub
End Class