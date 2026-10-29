''' <summary>
''' Proceso de baja/liquidación de un empleado (V.5.0 FrmBajas): carga del empleado con su historial de salarios,
''' recálculos al cambiar fechas, cálculo y grabado de la liquidación (con IR) y proceso final de la baja.
''' Une CsBajasDAL (datos), CalculadoraLiquidacion y CalculadoraIRBaja (negocio); el formulario solo muestra.
''' </summary>
Public Class CsLiquidacion

    Private Const FormatoBono As String = "Nomina Bono Produccion"

    Private ReadOnly _dal As New CsBajasDAL
    Private ReadOnly _controles As Controles
    Private _suspensiones As List(Of KeyValuePair(Of Date, Double))

    Public ReadOnly Property Empresa As CsBajasDAL.DatosEmpresaBaja
    Public ReadOnly Property Empleado As EmpleadoBaja
    ''' <summary>Meses usados para el salario promedio/alto (se muestran en pantalla y en el reporte).</summary>
    Public ReadOnly Property Salarios As New List(Of SalarioMesBaja)
    Public ReadOnly Property Liquidacion As Liquidacion

    ''' <summary>Valores que la pantalla muestra después de cargar o recalcular.</summary>
    Public Class Estado
        Public Property Fecha_Egreso As Date
        Public Property Fecha_Ini_Vacaciones As Date
        Public Property Fecha_Ini_Aguinaldo As Date
        Public Property Annos As Double
        Public Property Meses_Texto As String = ""
        Public Property Dias_Trabajados As Double
        Public Property Dias_Planilla As Double
        Public Property Salario_Promedio As Double
        Public Property Salario_Alto As Double
        Public Property Salario_Basico As Double
        Public Property Tarifa As Double
        Public Property Porcentaje_Antiguedad As Double
        Public Property Sueldo_Actual_Basico As Boolean
        Public Property Salario_Fijo As Boolean
    End Class

    Public Sub New()
        _controles = New CsControlesDAL().ObtenerControles()
        _Empresa = _dal.ObtenerDatosEmpresa()
    End Sub

    Public ReadOnly Property Dias_Mes As Double
        Get
            Return If(_controles IsNot Nothing AndAlso _controles.Dias_Mes > 0, _controles.Dias_Mes, 30)
        End Get
    End Property

    ''' <summary>Controles.SalarioPromedioReal: valor inicial de la casilla "Salario prom. diferencia fecha".</summary>
    Public ReadOnly Property Salario_Promedio_Real As Boolean
        Get
            Return _controles IsNot Nothing AndAlso _controles.Salario_Promedio_Real
        End Get
    End Property

    Public ReadOnly Property Es_Formato_Bono As Boolean
        Get
            Return String.Equals(Empresa.Formato_Nomina, FormatoBono, StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    Public Function ListarEmpleados() As DataTable
        Return _dal.ListarEmpleadosActivos()
    End Function

#Region "CARGAR / RECALCULAR"

    ''' <summary>
    ''' Carga el empleado (txtCodEmpleado_Change de la V.5.0). Lanza InvalidOperationException si no existe, ya fue dado
    ''' de baja o no tiene fecha de contrato. Como la V.5.0, primero recalcula Historico.SueldoActual.
    ''' </summary>
    Public Function CargarEmpleado(codigo1 As String, incentivos As Boolean, antiguedadBase As Boolean, horasExtra As Boolean,
                                   promedioReal As Boolean) As Estado

        _Empleado = Nothing
        _Liquidacion = Nothing
        Salarios.Clear()

        Dim emp As EmpleadoBaja = _dal.BuscarEmpleado(codigo1)
        If emp Is Nothing Then Throw New InvalidOperationException("Este empleado no existe.")
        If Not emp.Activo OrElse emp.Liquidado Then Throw New InvalidOperationException("Este empleado ya fue dado de baja.")
        If Not emp.Fecha_Contrato.HasValue Then
            Throw New InvalidOperationException("No ha sido grabada la fecha de contrato: no puede realizarse la baja.")
        End If

        _dal.ActualizarSueldoActual(emp.Cod_Empleado, Dias_Mes)
        emp = _dal.BuscarEmpleado(codigo1)
        _Empleado = emp
        _suspensiones = _dal.Suspensiones(emp.Cod_Empleado1)

        Dim est As New Estado With {
            .Sueldo_Actual_Basico = emp.Sueldo_Actual_Basico.GetValueOrDefault(),
            .Salario_Fijo = emp.Salario_Fijo = "S",
            .Salario_Basico = Empresa.Salario_Minimo,
            .Tarifa = Empresa.Salario_Minimo / 30 / 8}

        Dim contrato As Date = emp.Fecha_Contrato.Value.Date
        est.Fecha_Egreso = If(_dal.UltimaFechaNomina(emp.Cod_Empleado), contrato).Date

        Dim annos As Double, meses As String = ""
        CalculadoraLiquidacion.TiempoAlCargar(contrato, est.Fecha_Egreso, Dias_Mes, annos, meses)
        est.Annos = annos
        est.Meses_Texto = meses
        est.Dias_Trabajados = CalculadoraLiquidacion.R(CalculadoraLiquidacion.DiasAntiguedad(contrato, est.Fecha_Egreso, Dias_Mes = 30) * 12, 0)

        ' Ventana de los últimos 6 meses con nómina.
        Dim desde, hasta As Date
        Ventana(_dal.MesesConNomina(emp.Cod_Empleado, Nothing), desde, hasta)

        Dim variante As CsBajasDAL.VarianteSalarios =
            If(Es_Formato_Bono, CsBajasDAL.VarianteSalarios.BonoProduccion,
               If(est.Sueldo_Actual_Basico, CsBajasDAL.VarianteSalarios.SueldoActual, CsBajasDAL.VarianteSalarios.Normal))
        Salarios.AddRange(_dal.ListarSalarios(emp.Cod_Empleado, desde, hasta, variante, incentivos, horasExtra))

        If est.Salario_Fijo Then
            Dim sueldo As Double = CalculadoraLiquidacion.SueldoMensualFijo(emp, Salarios.LastOrDefault(), est.Sueldo_Actual_Basico, Dias_Mes)
            Dim factor As Double = FactorAntiguedad(antiguedadBase, annos, est)
            est.Salario_Promedio = sueldo * factor
            est.Salario_Alto = sueldo * factor
            If Not antiguedadBase AndAlso est.Sueldo_Actual_Basico Then
                est.Salario_Promedio = emp.Sueldo_Actual
                est.Salario_Alto = emp.Sueldo_Actual
            End If
        Else
            CalculadoraLiquidacion.PromedioYAlto(Salarios, promedioReal, If(est.Sueldo_Actual_Basico, emp.Sueldo_Actual, 0),
                                                 est.Salario_Promedio, est.Salario_Alto)
        End If

        FechasInicio(emp, est.Fecha_Egreso, est.Fecha_Egreso, est)
        est.Dias_Planilla = DiasPlanilla(est.Fecha_Egreso)

        Return est

    End Function

    ''' <summary>Nueva fecha de egreso (TxtUltFechaNomina_Change): solo cambia años, meses, días trabajados y días de planilla.</summary>
    Public Function CambiarFechaEgreso(fechaEgreso As Date, est As Estado) As Estado

        RequiereEmpleado()

        Dim annos As Double, meses As String = ""
        est.Fecha_Egreso = fechaEgreso.Date
        est.Dias_Trabajados = CalculadoraLiquidacion.TiempoAlCambiarEgreso(Empleado.Fecha_Contrato.Value, fechaEgreso, Dias_Mes, annos, meses)
        est.Annos = annos
        est.Meses_Texto = meses
        est.Dias_Planilla = DiasPlanilla(fechaEgreso)
        _Liquidacion = Nothing

        Return est

    End Function

    ''' <summary>
    ''' Recalcula salario promedio/alto con los 6 meses anteriores a la fecha de historial (TxtFechaHistorial_Change).
    ''' Esta rama de la V.5.0 siempre suma incentivos y no distingue el formato bono producción.
    ''' </summary>
    Public Function CambiarFechaHistorial(fechaHistorial As Date, est As Estado, antiguedadBase As Boolean, promedioReal As Boolean) As Estado

        RequiereEmpleado()
        Salarios.Clear()
        _Liquidacion = Nothing

        Dim desde, hasta As Date
        Ventana(_dal.MesesConNomina(Empleado.Cod_Empleado, fechaHistorial), desde, hasta)

        Dim variante As CsBajasDAL.VarianteSalarios =
            If(est.Sueldo_Actual_Basico, CsBajasDAL.VarianteSalarios.SueldoActual, CsBajasDAL.VarianteSalarios.Normal)
        Salarios.AddRange(_dal.ListarSalarios(Empleado.Cod_Empleado, desde, hasta, variante, True, False))

        If est.Salario_Fijo Then
            Dim sueldo As Double = 0
            If Salarios.Count > 0 Then
                If Empleado.Periodo = "Quincenal" Then sueldo = Empleado.Sueldo_Periodo * 2
                If Empleado.Periodo = "Mensual" Then sueldo = Empleado.Sueldo_Periodo
            End If
            Dim factor As Double = FactorAntiguedad(antiguedadBase, est.Annos, est)
            est.Salario_Promedio = sueldo * factor
            est.Salario_Alto = sueldo * factor
        Else
            Dim total As Double = Salarios.Sum(Function(s) s.Total_Ingresos)
            est.Salario_Alto = If(Salarios.Count = 0, 0, Math.Max(0, Salarios.Max(Function(s) s.Total_Ingresos)))
            est.Salario_Promedio = 0
            If Salarios.Count > 0 Then
                If promedioReal Then
                    ' Divide entre el tiempo real trabajado (meses de 30 días desde el inicio de vacaciones).
                    Dim mesReal As Double = CalculadoraLiquidacion.DiasVacaciones(est.Fecha_Ini_Vacaciones, hasta, Dias_Mes = 30) / Dias_Mes
                    est.Salario_Promedio = If(mesReal > 0, total / mesReal, total / Salarios.Count)
                Else
                    est.Salario_Promedio = total / Salarios.Count
                End If
            End If
        End If

        FechasInicio(Empleado, fechaHistorial.Date, est.Fecha_Egreso, est)

        Return est

    End Function

    ''' <summary>De la lista ordenada de meses toma el inicio del 6.º desde el final y el fin del último (hoy si no hay).</summary>
    Private Shared Sub Ventana(meses As List(Of SalarioMesBaja), ByRef desde As Date, ByRef hasta As Date)
        If meses.Count = 0 Then
            desde = Date.Today
            hasta = Date.Today
            Return
        End If
        hasta = meses.Last().Fecha_Fin
        desde = meses(Math.Max(0, meses.Count - 6)).Fecha_Inicio
    End Sub

    ''' <summary>1 + porcentaje de la tabla Antiguedad cuando se marca "Antigüedad base para cálculo".</summary>
    Private Function FactorAntiguedad(antiguedadBase As Boolean, annos As Double, est As Estado) As Double
        est.Porcentaje_Antiguedad = 0
        If Not antiguedadBase Then Return 1
        ' Sin fila en la tabla la V.5.0 multiplicaba por 0 (salario 0); aquí se deja el salario sin recargo.
        Dim pct As Double? = _dal.PorcentajeAntiguedad(CInt(Math.Floor(annos)))
        est.Porcentaje_Antiguedad = pct.GetValueOrDefault()
        Return 1 + pct.GetValueOrDefault()
    End Function

    ''' <summary>
    ''' Inicio de aguinaldo y vacaciones: si el contrato es anterior al año de planilla, aguinaldo desde el 1 de diciembre
    ''' anterior y vacaciones desde el 1 de junio (egreso después de mayo) o el 1 de diciembre anterior; si no, desde el contrato.
    ''' </summary>
    Private Sub FechasInicio(emp As EmpleadoBaja, fechaReferencia As Date, fechaEgreso As Date, est As Estado)

        Dim contrato As Date = emp.Fecha_Contrato.Value.Date
        Dim inicioAnio As Date? = _dal.InicioAnioPlanilla(emp.Cod_Tipo_Nomina, fechaEgreso.Year)
        Dim y As Integer = fechaReferencia.Year

        If inicioAnio.HasValue AndAlso contrato < inicioAnio.Value Then
            est.Fecha_Ini_Aguinaldo = New Date(y - 1, 12, 1)
            est.Fecha_Ini_Vacaciones = If(fechaReferencia > New Date(y, 5, 31), New Date(y, 6, 1), New Date(y - 1, 12, 1))
        Else
            est.Fecha_Ini_Aguinaldo = contrato
            est.Fecha_Ini_Vacaciones = contrato
        End If

    End Sub

    Private Function DiasPlanilla(fechaEgreso As Date) As Double
        Dim nomina As CsBajasDAL.NominaActivaBaja = _dal.NominaActiva(Empleado.Cod_Empleado)
        Return CalculadoraLiquidacion.DiasPlanilla(Empleado.Fecha_Contrato.Value, fechaEgreso,
                                                   nomina?.Inicio, nomina?.Fin)
    End Function

#End Region

#Region "CALCULAR / PROCESAR"

    ''' <summary>Calcula la liquidación con IR, la graba en Bajas y registra la bitácora (CmdCalculos_Click).</summary>
    Public Function Calcular(opc As OpcionesLiquidacion, fechaHistorial As Date, usuario As String) As Liquidacion

        RequiereEmpleado()

        Dim emp As EmpleadoBaja = _dal.BuscarEmpleado(Empleado.Cod_Empleado1)
        If emp Is Nothing OrElse Not emp.Activo OrElse emp.Liquidado Then
            Throw New InvalidOperationException("Este empleado ya no está activo o ya fue liquidado.")
        End If

        _dal.RegistrarBitacora(usuario, "Se Calculo Liquidacion: " & emp.Cod_Empleado1 & " " & emp.Nombre1)

        Dim egreso As Date = opc.Fecha_Egreso.Date
        Dim tasa As Double = _dal.TasaCambio(egreso)
        Dim inicioAnio As Date = If(_dal.InicioAnioPlanilla(emp.Cod_Tipo_Nomina, egreso.Year), Date.Today)
        Dim susp As List(Of KeyValuePair(Of Date, Double)) = _suspensiones

        Dim datos As New CalculadoraLiquidacion.DatosCalculo With {
            .Dias_Mes = Dias_Mes,
            .Antiguedad_Menor = _controles IsNot Nothing AndAlso _controles.Antiguedad_Menor,
            .Tasa_Cambio = tasa,
            .Adelanto_Vacaciones = _dal.AdelantoVacaciones(emp.Cod_Empleado, inicioAnio, egreso),
            .Horas_Extra_Pendientes = If(opc.Horas_Extra, _dal.HorasExtraPendientes(emp.Cod_Empleado), 0),
            .Otros_Ingresos_Planilla = If(opc.Otros_Ingresos_Planilla, _dal.OtrosIngresosPlanilla(emp.Cod_Empleado), 0),
            .Cuotas_Prestamo = If(opc.Prestamos, _dal.CuotasPrestamoPendientes(emp.Cod_Empleado, tasa), 0),
            .Deducciones_Pendientes = If(opc.Deducciones, _dal.DeduccionesPendientes(emp.Cod_Empleado), 0),
            .Suspensiones = Function(d, h) susp.Where(Function(s) s.Key.Date >= d.Date AndAlso s.Key.Date <= h.Date).Sum(Function(s) s.Value)}

        Dim res As CalculadoraLiquidacion.Resultado = CalculadoraLiquidacion.Calcular(emp, opc, datos)
        Dim liq As Liquidacion = res.Liquidacion

        If opc.Ir Then CalcularIr(emp, opc, res)

        liq.Fecha_Historial = fechaHistorial.Date
        _dal.Guardar(liq, fechaHistorial)

        _Empleado = emp
        _Liquidacion = liq
        Return liq

    End Function

    ''' <summary>
    ''' Base del IR (CalcularMontoIrBajas): con "IR última semana" suma lo ya devengado en el mes; si no hay nómina del mes
    ''' pero sí vacaciones, toma el salario promedio como nómina del mes y descuenta el INSS de las vacaciones.
    ''' </summary>
    Private Sub CalcularIr(emp As EmpleadoBaja, opc As OpcionesLiquidacion, res As CalculadoraLiquidacion.Resultado)

        Dim montoNomina As Double = 0
        Dim inss As Double = 0
        Dim vacaciones As Double = res.Vacaciones_Base

        If emp.Ir_Ultima_Semana Then
            Dim dev As CsBajasDAL.DevengadoMes = _dal.DevengadoDelMes(emp.Cod_Empleado, emp.Cod_Tipo_Nomina, opc.Fecha_Egreso)
            If dev IsNot Nothing Then
                inss = dev.Inss
                montoNomina = dev.Devengado - dev.Inss - dev.Viaticos
            End If
        End If

        If vacaciones <> 0 AndAlso montoNomina = 0 Then
            inss = opc.Salario_Promedio * emp.Tasa_Inss
            montoNomina = opc.Salario_Promedio - inss
            vacaciones -= vacaciones * emp.Tasa_Inss
        End If

        res.Liquidacion.Referencia = "Monto Nomina " & (montoNomina + inss).ToString("#,##0.00") & "   Monto INSS  " & inss.ToString("#,##0.00")

        Dim ir As Double = CalculadoraIRBaja.Calcular(emp.Periodo, _dal.TablaIR(), montoNomina + res.Base_Ir_Mensual, vacaciones,
                                                      CalculadoraIRBaja.SemanasDelMes(opc.Fecha_Egreso, emp.Periodo))
        res.Liquidacion.Monto_Ir = CalculadoraLiquidacion.R(ir, 2)

    End Sub

    ''' <summary>Da de baja al empleado con la última liquidación calculada (CmdEfectuar_Click).</summary>
    Public Sub Procesar(usuario As String)
        RequiereEmpleado()
        If Liquidacion Is Nothing Then Throw New InvalidOperationException("Primero calcule la liquidación.")
        _dal.Procesar(Empleado, Liquidacion, usuario)
        _Empleado.Liquidado = True
        _Empleado.Activo = Liquidacion.Pagar_Dias_Planilla_Opt
    End Sub

    Private Sub RequiereEmpleado()
        If Empleado Is Nothing Then Throw New InvalidOperationException("Seleccione un empleado.")
    End Sub

#End Region

#Region "REPORTE"

    ''' <summary>Reporte de la última liquidación calculada (V.5.0 ARBaja).</summary>
    Public Function CrearReporte(mesesTexto As String, diasPlanillaTexto As String) As RptLiquidacion

        RequiereEmpleado()
        If Liquidacion Is Nothing Then Throw New InvalidOperationException("Primero calcule la liquidación.")

        Return New RptLiquidacion(Empresa, Empleado, Liquidacion, Salarios, mesesTexto, diasPlanillaTexto, RutaFotoEmpleado())

    End Function

    ''' <summary>Foto del empleado en DatosEmpresa.RutaFoto (código visible + .jpg/.gif/.bmp); "" si no hay.</summary>
    Private Function RutaFotoEmpleado() As String
        If String.IsNullOrWhiteSpace(Empresa.Ruta_Foto) Then Return ""
        For Each ext As String In {".jpg", ".gif", ".bmp"}
            Try
                Dim ruta As String = Empresa.Ruta_Foto & Empleado.Cod_Empleado1 & ext
                If IO.File.Exists(ruta) Then Return ruta
            Catch ex As ArgumentException
                Return ""
            End Try
        Next
        Return ""
    End Function

#End Region

End Class
