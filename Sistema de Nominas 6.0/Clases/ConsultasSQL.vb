Public Class ConsultasSQL
    Dim MiConexion As New Conexion

    Public Function CaseListadoSistemaNomina(Quien As String) As DataTable
        Dim SqlString As String = ""

        Select Case Quien
            Case "ListadoEmpleados"
                SqlString = "SELECT Empleado.CodEmpleado1, Empleado.Nombre1 + ' ' + Empleado.Nombre2 + ' ' + Empleado.Apellido1 + ' ' + Empleado.Apellido2 AS Nombres, Empleado.CodEmpleado, Empleado.NumCedula, Empleado.NumeroInss, Departamento.Departamento, Cargo.Cargo, Empleado.SueldoPeriodo, Empleado.Sexo, Empleado.NumHijos, Empleado.TarifaHoraria , Empleado.Dolarizado, Historico.FechaNacimiento, Historico.FechaContrato, Empleado.CuentaBanco, Empleado.NumeroInss FROM Empleado INNER JOIN Departamento ON Empleado.CodDepartamento = Departamento.CodDepartamento INNER JOIN Cargo ON Empleado.CodCargo = Cargo.CodCargo INNER JOIN Historico ON Empleado.CodEmpleado = Historico.Codempleado Where(Empleado.Activo = 1) ORDER BY Empleado.CodEmpleado1"

            Case "ConsultaEmpleados"
                SqlString = "SELECT Empleado.CodEmpleado1, Empleado.Nombre1 + ' ' + Empleado.Nombre2 + ' ' + Empleado.Apellido1 + ' ' + Empleado.Apellido2 AS Nombres, Empleado.CodEmpleado, Empleado.NumCedula, Empleado.NumeroInss, Departamento.Departamento, Cargo.Cargo, Empleado.SueldoPeriodo, Empleado.Sexo, Empleado.NumHijos, Empleado.TarifaHoraria , Empleado.Dolarizado, Historico.FechaNacimiento, Historico.FechaContrato, Empleado.CuentaBanco, Empleado.NumeroInss FROM Empleado INNER JOIN Departamento ON Empleado.CodDepartamento = Departamento.CodDepartamento INNER JOIN Cargo ON Empleado.CodCargo = Cargo.CodCargo INNER JOIN Historico ON Empleado.CodEmpleado = Historico.Codempleado Where(Empleado.Activo = 1) ORDER BY Empleado.CodEmpleado1"

            Case "ComboEmpleados"
                SqlString = "SELECT Empleado.CodEmpleado1, Empleado.Nombre1 + ' ' + Empleado.Nombre2 + ' ' + Empleado.Apellido1 + ' ' + Empleado.Apellido2 AS Nombres, Empleado.NumCedula  FROM Empleado INNER JOIN Departamento ON Empleado.CodDepartamento = Departamento.CodDepartamento INNER JOIN Cargo ON Empleado.CodCargo = Cargo.CodCargo INNER JOIN Historico ON Empleado.CodEmpleado = Historico.Codempleado Where(Empleado.Activo = 1) ORDER BY Empleado.CodEmpleado1"

            Case "ListadoCargo"
                SqlString = "SELECT  CodCargo, Cargo FROM Cargo"

            Case "ListadoDepartamento"
                SqlString = "SELECT  CodDepartamento, Departamento FROM Departamento"

            Case "ListadoTipoNomina"
                SqlString = "SELECT CodTipoNomina, Nomina, Periodo FROM TipoNomina"

        End Select

        Return FillDataTableSQL(SqlString, Quien)

    End Function


    Public Sub InsertarEmpleados(TablaEmpleados As Empleados)
        Dim SqlString As String

        SqlString = "INSERT INTO Empleado  ((CodEmpleado1),(Nombre1),(Nombre2),(Apellido1),(Apellido2),(NumHijos),(Direccion) ,(Nacionalidad) ,(CodigoPostal) ,(Sexo) ,(CodInss) ,(CodIr) ,(NumCedula) ,(Sindicalista),(CodDepartamento),(CodGrupo),(CodCargo),(NumeroInss),(NumeroRuc),(CodTipoNomina),(DiasDescuento),(SalPorcentaje),(CantPts),(SueldoPeriodo) ,(TarifaHoraria),(PorcentajeComision),(OtrosIngresos),(DescripOtrIngre),(ExentoInss),(ExentoIr),(PagoInssPatronal),(SalarioMinimo) ,(Observaciones),(Activo),(Liquidado),(Ausente),(SalarioFijo),(SumarSubsidio),(PorcientoIncentivo),(Gravidez) ,(VacacionesBasico) ,(Dolarizado) ,(FechaAntiguedad),(Antiguedad) ,(CuentaBanco) ,(SueldoActualBasico) ,(HorasTurno) ,(DiasBasico) ,(AumentoBasico) ,(EmpleadoConstruccion) ,(MontoPension) ,(PorcentajePension) ,(ViaticoxDia) ,(DeducirPorciento)  ,(Reembolso) ,(Telefono) ,(MontoViatico)) " +
                               "VALUES ('" + TablaEmpleados.Cod_Empleado1 + "','" + TablaEmpleados.Primer_Nombre + "','" + TablaEmpleados.Segundo_Nombre + "','" + TablaEmpleados.Primer_Apellido + "' ,'" + TablaEmpleados.Segundo_Apellido + "'," + TablaEmpleados.Num_Hijos + ",'" + TablaEmpleados.Empleado_Direccion + "' ,'" + TablaEmpleados.Empleado_Nacionalidad + "' ,'" + TablaEmpleados.Codigo_Postal + "' ,'" + TablaEmpleados.Empleado_Sexo + "' ,'" + TablaEmpleados.Cod_Inss + "' ,'" + TablaEmpleados.Cod_Ir + "' ,'" + TablaEmpleados.Num_Cedula + "','" + TablaEmpleados.Empleado_Sindicalista + "' ,'" + TablaEmpleados.Cod_Departamento + "' ,'" + TablaEmpleados.Cod_Grupo + "' ,'" + TablaEmpleados.Cod_Cargo + "' ,'" + TablaEmpleados.Numero_Inss + "' ,'" + TablaEmpleados.Numero_Ruc + "' ,'" + TablaEmpleados.Cod_TipoNomina + "' ," + TablaEmpleados.Dias_Descuento + " ," + TablaEmpleados.Sal_Porcentaje + " ," + TablaEmpleados.Cant_Pts + "  ," + TablaEmpleados.Sueldo_Periodo + " ," + TablaEmpleados.Tarifa_Horaria + " ," + TablaEmpleados.Porcentaje_Comision + " ," + TablaEmpleados.Otros_Ingresos + ",'" + TablaEmpleados.Descrip_OtrIngre + "' ," + TablaEmpleados.Exento_Inss + " ," + TablaEmpleados.Exento_Ir + "  ," + TablaEmpleados.PagoInss_Patronal + " ," + TablaEmpleados.Salario_Minimo + " ,'" + TablaEmpleados.Empleado_Observaciones + "'  ," + TablaEmpleados.Empleado_Activo + ",'" + TablaEmpleados.Empleado_Liquidado + "'," + TablaEmpleados.Empleado_Ausente + " ,'" + TablaEmpleados.Salario_Fijo + "'," + TablaEmpleados.Sumar_Subsidio + "," + TablaEmpleados.Porciento_Incentivo + " ," + TablaEmpleados.Empleado_Gravidez + " ," + TablaEmpleados.Vacaciones_Basico + " ," + TablaEmpleados.Empleado_Dolarizado + " ," + TablaEmpleados.Fecha_Antiguedad + " ," + TablaEmpleados.Empleado_Antiguedad + ",'" + TablaEmpleados.Empleado_CuentaBanco + "'," + TablaEmpleados.SueldoActual_Basico + " ," + TablaEmpleados.Horas_Turno + " ," + TablaEmpleados.Dias_Basico + " ," + TablaEmpleados.Aumento_Basico + " ," + TablaEmpleados.Empleado_Construccion + " ," + TablaEmpleados.Monto_Pension + " ," + TablaEmpleados.Porcentaje_Pension + " , " + TablaEmpleados.Viatico_Dia + " ," + TablaEmpleados.Deducir_Porciento + " ,'" + TablaEmpleados.Empleado_Telefono + "'," + TablaEmpleados.Monto_Viatico + ") "
        EjecutarConsulta(SqlString)

    End Sub
    Public Sub ActualizarEmpleado(TablaEmpleado As Empleados)
        Dim SqlString As String
        SqlString = "UPDATE Empleado  SET(CodEmpleado1) = '" + TablaEmpleado.Cod_Empleado1 + "' ,(Nombre1) = '" + TablaEmpleado.Primer_Nombre + "',(Nombre2) = '" + TablaEmpleado.Segundo_Nombre + "' ,(Apellido1) = '" + TablaEmpleado.Primer_Apellido + "' ,(Apellido2) = '" + TablaEmpleado.Segundo_Apellido + "',(NumHijos) = " + TablaEmpleado.Num_Hijos + " ,(Direccion) = '" + TablaEmpleado.Empleado_Direccion + "',(Nacionalidad) = '" + TablaEmpleado.Empleado_Nacionalidad + "',(CodigoPostal) = '" + TablaEmpleado.Codigo_Postal + "' ,(Sexo) = '" + TablaEmpleado.Empleado_Sexo + "' ,(CodInss) = '" + TablaEmpleado.Codigo_Postal + "' ,(CodIr) = '" + TablaEmpleado.Cod_Ir + "',(NumCedula) = '" + TablaEmpleado.Num_Cedula + "' ,(Sindicalista) = '" + TablaEmpleado.Empleado_Sindicalista + "',(CodDepartamento) = '" + TablaEmpleado.Cod_Departamento + "' ,(CodGrupo) = '" + TablaEmpleado.Cod_Grupo + "' ,(CodCargo) = '" + TablaEmpleado.Cod_Cargo + "' ,(NumeroInss) = '" + TablaEmpleado.Numero_Inss + "' ,(NumeroRuc) = '" + TablaEmpleado.Numero_Ruc + "' ,(CodTipoNomina) = '" + TablaEmpleado.Cod_TipoNomina + "' ,(DiasDescuento) = " + TablaEmpleado.Dias_Descuento + " ,(SalPorcentaje) = " + TablaEmpleado.Sal_Porcentaje + ",(CantPts) = " + TablaEmpleado.Cant_Pts + " ,(SueldoPeriodo) = " + TablaEmpleado.Sueldo_Periodo + " ,(TarifaHoraria) = " + TablaEmpleado.Tarifa_Horaria + " ,(PorcentajeComision) = " + TablaEmpleado.Porcentaje_Comision + " ,(OtrosIngresos) = " + TablaEmpleado.Otros_Ingresos + " ,(DescripOtrIngre) = '" + TablaEmpleado.Descrip_OtrIngre + "' ,(ExentoInss) = " + TablaEmpleado.Exento_Inss + " ,(ExentoIr) = " + TablaEmpleado.Exento_Ir + " ,(PagoInssPatronal) = " + TablaEmpleado.PagoInss_Patronal + " ,(SalarioMinimo) = " + TablaEmpleado.Salario_Minimo + ",(Observaciones) = '" + TablaEmpleado.Empleado_Observaciones + "' ,(Activo) = '" + TablaEmpleado.Empleado_Activo + "',(Liquidado) = " + TablaEmpleado.Empleado_Liquidado + " ,(Ausente) = '" + TablaEmpleado.Empleado_Ausente + "' ,(SalarioFijo) = '" + TablaEmpleado.Salario_Fijo + "',(SumarSubsidio) = " + TablaEmpleado.Sumar_Subsidio + " ,(PorcientoIncentivo) = " + TablaEmpleado.Porciento_Incentivo + "  ,(Gravidez) = " + TablaEmpleado.Empleado_Gravidez + " ,(VacacionesBasico) = " + TablaEmpleado.Vacaciones_Basico + " ,(Dolarizado) = " + TablaEmpleado.Empleado_Dolarizado + " ,(FechaAntiguedad) = '" + TablaEmpleado.Fecha_Antiguedad + "' ,(Antiguedad) = " + TablaEmpleado.Empleado_Antiguedad + " ,(CuentaBanco) = '" + TablaEmpleado.Empleado_CuentaBanco + "' ,(SueldoActualBasico) = " + TablaEmpleado.SueldoActual_Basico + " ,(HorasTurno) = '" + TablaEmpleado.Horas_Turno + "' ,(DiasBasico) = " + TablaEmpleado.Dias_Basico + " ,(AumentoBasico) = " + TablaEmpleado.Aumento_Basico + " ,(EmpleadoConstruccion) = " + TablaEmpleado.Empleado_Construccion + " ,(MontoPension) = " + TablaEmpleado.Monto_Pension + " ,(PorcentajePension) = " + TablaEmpleado.Porcentaje_Comision + " ,(ViaticoxDia) = " + TablaEmpleado.Viatico_Dia + " ,(DeducirPorciento) = " + TablaEmpleado.Deducir_Porciento + " ,(Reembolso) = " + TablaEmpleado.Empleado_Reembolso + " ,(Telefono) = '" + TablaEmpleado.Empleado_Telefono + "' ,(MontoViatico) = " + TablaEmpleado.Monto_Viatico + "  + " +
                               "WHERE (CodEmpleado = " + TablaEmpleado.Cod_Empleado + ") "
        EjecutarConsulta(SqlString)
    End Sub

    Public Function BuscarEmpleado(CodigoEmpleado As String) As Empleados
        Dim TEmpleado As New DataTable, SqlString As String
        Dim TablaEmpleado As New Empleados, TablaHistorico As New Historico

        TEmpleado = New DataTable
        SqlString = "SELECT * FROM Empleado WHERE (CodEmpleado1 = '" + CodigoEmpleado + "') AND (Activo = 1)"
        TEmpleado = FillDataTableSQL(SqlString, "Empleado")
        If TEmpleado.Rows.Count <> 0 Then
            If Not IsDBNull(TEmpleado.Rows(0)("CodEmpleado")) Then
                TablaEmpleado.Cod_Empleado = Convert.ToDouble(TEmpleado.Rows(0)("CodEmpleado"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("CodEmpleado1")) Then
                TablaEmpleado.Cod_Empleado1 = TEmpleado.Rows(0)("CodEmpleado1").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Nombre1")) Then
                TablaEmpleado.Primer_Nombre = TEmpleado.Rows(0)("Nombre1").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Nombre2")) Then
                TablaEmpleado.Segundo_Nombre = TEmpleado.Rows(0)("Nombre2").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Apellido1")) Then
                TablaEmpleado.Primer_Apellido = TEmpleado.Rows(0)("Apellido1").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Apellido2")) Then
                TablaEmpleado.Segundo_Apellido = TEmpleado.Rows(0)("Apellido2").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("NumHijos")) Then
                TablaEmpleado.Num_Hijos = Convert.ToInt32((TEmpleado.Rows(0)("NumHijos")))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Nacionalidad")) Then
                TablaEmpleado.Empleado_Nacionalidad = TEmpleado.Rows(0)("Nacionalidad").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Direccion")) Then
                TablaEmpleado.Empleado_Direccion = TEmpleado.Rows(0)("Direccion").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("CodigoPostal")) Then
                TablaEmpleado.Codigo_Postal = TEmpleado.Rows(0)("CodigoPostal").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Sexo")) Then
                TablaEmpleado.Empleado_Sexo = TEmpleado.Rows(0)("Sexo").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("CodInss")) Then
                TablaEmpleado.Cod_Inss = TEmpleado.Rows(0)("CodInss").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Codir")) Then
                TablaEmpleado.Cod_Ir = TEmpleado.Rows(0)("Codir").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("NumCedula")) Then
                TablaEmpleado.Num_Cedula = TEmpleado.Rows(0)("NumCedula").ToString()
            End If


            If Not IsDBNull(TEmpleado.Rows(0)("Sindicalista")) Then
                TablaEmpleado.Empleado_Sindicalista = TEmpleado.Rows(0)("Sindicalista").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("CodDepartamento")) Then
                TablaEmpleado.Cod_Departamento = TEmpleado.Rows(0)("CodDepartamento").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("CodGrupo")) Then
                TablaEmpleado.Cod_Grupo = TEmpleado.Rows(0)("CodGrupo").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("CodCargo")) Then
                TablaEmpleado.Cod_Cargo = TEmpleado.Rows(0)("CodCargo").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("NumeroInss")) Then
                TablaEmpleado.Numero_Inss = TEmpleado.Rows(0)("NumeroInss").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("NumeroRuc")) Then
                TablaEmpleado.Numero_Ruc = TEmpleado.Rows(0)("NumeroRuc").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("CodTipoNomina")) Then
                TablaEmpleado.Cod_TipoNomina = TEmpleado.Rows(0)("CodTipoNomina").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("DiasDescuento")) Then
                TablaEmpleado.Dias_Descuento = Convert.ToDouble(TEmpleado.Rows(0)("DiasDescuento"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("SalPorcentaje")) Then
                TablaEmpleado.Sal_Porcentaje = Convert.ToInt32(TEmpleado.Rows(0)("SalPorcentaje"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("SueldoPeriodo")) Then
                TablaEmpleado.Sueldo_Periodo = Convert.ToDouble(TEmpleado.Rows(0)("SueldoPeriodo"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("TarifaHoraria")) Then
                TablaEmpleado.Tarifa_Horaria = Convert.ToDouble(TEmpleado.Rows(0)("TarifaHoraria"))
            End If


            If Not IsDBNull(TEmpleado.Rows(0)("PorcentajeComision")) Then
                TablaEmpleado.Porcentaje_Comision = Convert.ToDouble(TEmpleado.Rows(0)("PorcentajeComision"))
            End If


            If Not IsDBNull(TEmpleado.Rows(0)("OtrosIngresos")) Then
                TablaEmpleado.Otros_Ingresos = Convert.ToDouble(TEmpleado.Rows(0)("OtrosIngresos"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("DescripOtrIngre")) Then
                TablaEmpleado.Descrip_OtrIngre = TEmpleado.Rows(0)("DescripOtrIngre").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("ExentoInss")) Then
                TablaEmpleado.Exento_Inss = Convert.ToInt32(TEmpleado.Rows(0)("ExentoInss"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("ExentoIr")) Then
                TablaEmpleado.Exento_Ir = Convert.ToInt32(TEmpleado.Rows(0)("ExentoIr"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("PagoInssPatronal")) Then
                TablaEmpleado.PagoInss_Patronal = Convert.ToInt32(TEmpleado.Rows(0)("PagoInssPatronal"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("SalarioMinimo")) Then
                TablaEmpleado.Salario_Minimo = Convert.ToInt32(TEmpleado.Rows(0)("SalarioMinimo"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Observaciones")) Then
                TablaEmpleado.Empleado_Observaciones = TEmpleado.Rows(0)("Observaciones").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Activo")) Then
                TablaEmpleado.Empleado_Activo = Convert.ToInt32(TEmpleado.Rows(0)("Activo"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Liquidado")) Then
                TablaEmpleado.Empleado_Liquidado = Convert.ToInt32(TEmpleado.Rows(0)("Liquidado"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Ausente")) Then
                TablaEmpleado.Empleado_Ausente = Convert.ToInt32(TEmpleado.Rows(0)("Ausente"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("SalarioFijo")) Then
                TablaEmpleado.Salario_Fijo = TEmpleado.Rows(0)("SalarioFijo").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("SumarSubsidio")) Then
                TablaEmpleado.Sumar_Subsidio = Convert.ToInt32(TEmpleado.Rows(0)("SumarSubsidio"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("PorcientoIncentivo")) Then
                TablaEmpleado.Porciento_Incentivo = Convert.ToDouble(TEmpleado.Rows(0)("PorcientoIncentivo"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Gravidez")) Then
                TablaEmpleado.Empleado_Gravidez = Convert.ToInt32(TEmpleado.Rows(0)("Gravidez"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("VacacionesBasico")) Then
                TablaEmpleado.Vacaciones_Basico = Convert.ToInt32(TEmpleado.Rows(0)("VacacionesBasico"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Dolarizado")) Then
                TablaEmpleado.Empleado_Dolarizado = Convert.ToInt32(TEmpleado.Rows(0)("Dolarizado"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("FechaAntiguedad")) Then
                TablaEmpleado.Fecha_Antiguedad = TEmpleado.Rows(0)("FechaAntiguedad").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Antiguedad")) Then
                TablaEmpleado.Empleado_Antiguedad = Convert.ToInt32(TEmpleado.Rows(0)("Antiguedad"))
            End If


            If Not IsDBNull(TEmpleado.Rows(0)("CuentaBanco")) Then
                TablaEmpleado.Empleado_CuentaBanco = TEmpleado.Rows(0)("CuentaBanco").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("SueldoActualBasico")) Then
                TablaEmpleado.SueldoActual_Basico = Convert.ToInt32(TEmpleado.Rows(0)("SueldoActualBasico"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("HorasTurno")) Then
                TablaEmpleado.Horas_Turno = Convert.ToInt32(TEmpleado.Rows(0)("HorasTurno"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("DiasBasico")) Then
                TablaEmpleado.Dias_Basico = Convert.ToDouble(TEmpleado.Rows(0)("DiasBasico"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("AumentoBasico")) Then
                TablaEmpleado.Aumento_Basico = Convert.ToDouble(TEmpleado.Rows(0)("AumentoBasico"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("EmpleadoConstruccion")) Then
                TablaEmpleado.Empleado_Construccion = Convert.ToInt32(TEmpleado.Rows(0)("EmpleadoConstruccion"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("EmpleadoConstruccion")) Then
                TablaEmpleado.Monto_Pension = Convert.ToDouble(TEmpleado.Rows(0)("EmpleadoConstruccion"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("PorcentajePension")) Then
                TablaEmpleado.Porcentaje_Pension = Convert.ToDouble(TEmpleado.Rows(0)("PorcentajePension"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("ViaticoxDia")) Then
                TablaEmpleado.Viatico_Dia = Convert.ToDouble(TEmpleado.Rows(0)("ViaticoxDia"))
            End If


            If Not IsDBNull(TEmpleado.Rows(0)("DeducirPorciento")) Then
                TablaEmpleado.Deducir_Porciento = Convert.ToInt32(TEmpleado.Rows(0)("DeducirPorciento"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("Reembolso")) Then
                TablaEmpleado.Empleado_Reembolso = Convert.ToDouble(TEmpleado.Rows(0)("Reembolso"))
            End If


            If Not IsDBNull(TEmpleado.Rows(0)("Telefono")) Then
                TablaEmpleado.Empleado_Telefono = TEmpleado.Rows(0)("Telefono").ToString()
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("MontoViatico")) Then
                TablaEmpleado.Monto_Viatico = Convert.ToDouble(TEmpleado.Rows(0)("MontoViatico"))
            End If

            '///////////////////////////////////////////////////
            '////////////////REGISTROS HISTORICOS //////////////////
            '//////////////////////////////////////////////////////////

            If Not IsDBNull(TEmpleado.Rows(0)("FechaBaja")) Then
                TablaHistorico.Fecha_Baja = Convert.ToDateTime(TEmpleado.Rows(0)("FechaBaja"))
            End If

            If Not IsDBNull(TEmpleado.Rows(0)("MotivoBaja")) Then
                Tablahistorico.Motivo_Baja = TEmpleado.Rows(0)("MotivoBaja").ToString()
            End If

            Return TablaEmpleado
        Else

            Return Nothing

        End If

    End Function
    Public Sub BorrarEmpleado(CodigoEmpleado As String)
        Dim SqlString As String
        SqlString = "DELETE FROM Empleado  WHERE (CodEmpleado = " + CodigoEmpleado + ") "
        EjecutarConsulta(SqlString)
    End Sub

    Private Sub EjecutarConsulta(ByVal SqlString As String)

        Dim ComandoUpdate As New SqlClient.SqlCommand, iResultado As Integer

        MiConexion.MiConexion_SQl.Open()
        ComandoUpdate = New SqlClient.SqlCommand(SqlString, MiConexion.MiConexion_SQl)
        iResultado = ComandoUpdate.ExecuteNonQuery
        MiConexion.MiConexion_SQl.Close()


    End Sub

    Private Function FillDataSetSQL(SQlString As String, Nombre As String) As DataSet

        Dim Dataset As New DataSet, DataAdapter As New SqlClient.SqlDataAdapter

        DataAdapter = New SqlClient.SqlDataAdapter(SQlString, MiConexion.MiConexion_SQl)
        DataAdapter.Fill(Dataset, Nombre)

        Return Dataset

    End Function
    Private Function FillDataTableSQL(SQlString As String, Nombre As String) As DataTable
        Dim DataSetDatos As New DataSet, DataAdapter As New SqlClient.SqlDataAdapter

        MiConexion.MiConexion_SQl.Open()
        DataAdapter = New SqlClient.SqlDataAdapter(SQlString, MiConexion.MiConexion_SQl)
        DataSetDatos = New DataSet()
        DataAdapter.Fill(DataSetDatos, Nombre)
        MiConexion.MiConexion_SQl.Close()
        Return DataSetDatos.Tables(Nombre)
    End Function



    Public Function BuscarCargo(CodigoCargo As String) As Cargos
        Dim TablaCargo = New Cargos
        Dim TCargo = New DataTable()
        Dim SqlString As String

        SqlString = "SELECT CodCargo, Cargo, Monto FROM Cargo WHERE (CodCargo = '" + CodigoCargo + "')"
        TCargo = FillDataTableSQL(SqlString, "TablaCargo")

        If TCargo.Rows.Count <> 0 Then


            TablaCargo.Cod_Cargo = TCargo.Rows(0)("CodCargo").ToString()
            TablaCargo.Descripcion_Cargo = TCargo.Rows(0)("Cargo").ToString()
            TablaCargo.Monto_Cargo = 0
            Return TablaCargo
        Else
            Return TablaCargo
        End If

    End Function


    Public Function BuscarDepartamento(CodigoDepartamento As String) As Departamento
        Dim TDepartamento = New DataTable()
        Dim TablaDepartamento = New Departamento

        Dim SqlString As String
        SqlString = "SELECT CodDepartamento, Departamento FROM Departamento WHERE (CodDepartamento = '" + CodigoDepartamento + "')"
        TDepartamento = FillDataTableSQL(SqlString, "Departamento")
        If TDepartamento.Rows.Count <> 0 Then

            If Not IsDBNull(TDepartamento.Rows(0)("CodDepartamento")) Then
                TablaDepartamento.Cod_Departamento = TDepartamento.Rows(0)("CodDepartamento").ToString()
            End If


            If Not IsDBNull(TDepartamento.Rows(0)("Departamento")) Then
                TablaDepartamento.Descripcion_Departamento = TDepartamento.Rows(0)("Departamento").ToString()
            End If

            Return TablaDepartamento
        Else
            Return TablaDepartamento
        End If



    End Function


End Class
