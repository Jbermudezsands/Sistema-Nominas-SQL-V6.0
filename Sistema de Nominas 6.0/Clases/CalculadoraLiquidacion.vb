''' <summary>
''' Cálculos de la liquidación (bajas), portados de FrmBajas.frm y Funciones.bas de la V.5.0 sin acceso a datos:
''' días de antigüedad (CalcularDiasAntiguedad), días de aguinaldo/vacaciones (CalcularDiasAguinaldo), días de
''' planilla (CalcularDiasPlanilla), salario promedio/alto y los montos de CmdCalculos_Click. Los valores de la base
''' de datos los entrega quien llama (<see cref="CsLiquidacion"/>). Los redondeos imitan el Format() de la V.5.0.
''' </summary>
Public Class CalculadoraLiquidacion

    ''' <summary>Valores de la base de datos que necesita <see cref="Calcular"/>.</summary>
    Public Class DatosCalculo
        Public Property Dias_Mes As Double = 30
        Public Property Antiguedad_Menor As Boolean
        Public Property Tasa_Cambio As Double = 1
        Public Property Adelanto_Vacaciones As Double
        Public Property Horas_Extra_Pendientes As Double
        Public Property Otros_Ingresos_Planilla As Double
        Public Property Cuotas_Prestamo As Double
        Public Property Deducciones_Pendientes As Double
        ''' <summary>Días de suspensión del empleado con FechaInicio entre las dos fechas (para el aguinaldo).</summary>
        Public Property Suspensiones As Func(Of Date, Date, Double) = Function(d, h) 0
    End Class

    ''' <summary>Liquidación calculada sin IR, más las bases que necesita <see cref="CalculadoraIRBaja"/>.</summary>
    Public Class Resultado
        Public Property Liquidacion As Liquidacion
        ''' <summary>Ingresos del mes de la baja ya sin INSS (horas extra + otros + días de planilla).</summary>
        Public Property Base_Ir_Mensual As Double
        ''' <summary>Vacaciones antes del redondeo final y menos adelantos (la V.5.0 usaba este valor para INSS e IR).</summary>
        Public Property Vacaciones_Base As Double
    End Class

#Region "DIAS"

    ''' <summary>Días de antigüedad acumulados (2.5 por mes completo), port de CalcularDiasAntiguedad.</summary>
    Public Shared Function DiasAntiguedad(fechaInicio As Date, fechaFin As Date, diasMes30 As Boolean) As Double
        Return Recorrer(fechaInicio, fechaFin, diasMes30, False, Nothing)
    End Function

    ''' <summary>Días de aguinaldo/vacaciones ganados menos suspensiones, port de CalcularDiasAguinaldo.</summary>
    Public Shared Function DiasAguinaldo(fechaInicio As Date, fechaFin As Date, diasMes30 As Boolean, suspensiones As Func(Of Date, Date, Double)) As Double
        Return Recorrer(fechaInicio, fechaFin, diasMes30, True, suspensiones)
    End Function

    ''' <summary>
    ''' Recorrido mes a mes común a las dos funciones de la V.5.0; solo cambian el acumulado de febrero, la condición
    ''' de cierre y que el aguinaldo resta suspensiones. Se conserva el tramo extra que la V.5.0 sumaba al terminar.
    ''' </summary>
    Private Shared Function Recorrer(fechaInicio As Date, fechaFin As Date, diasMes30 As Boolean, esAguinaldo As Boolean,
                                     suspensiones As Func(Of Date, Date, Double)) As Double

        Dim inicio As Date = fechaInicio.Date
        Dim fin As Date = fechaFin.Date
        Dim tInicio As Date = inicio
        Dim tFin As Date = If((fin - inicio).TotalDays > 30, FinDeMes(inicio), fin)
        Dim total As Double = 0
        Dim vueltas As Integer = 0

        Do While inicio < fin AndAlso vueltas < 2000

            vueltas += 1
            If diasMes30 AndAlso tFin.Day > 30 Then tFin = tFin.AddDays(-1)

            total += Acumulado(tInicio, tFin, diasMes30, esAguinaldo, False) - Suspendido(tInicio, tFin, esAguinaldo, suspensiones, 0.0833)

            If tFin.AddDays(2) >= fin Then
                Dim restante As Double = (fin - tFin).TotalDays
                If (esAguinaldo AndAlso restante = 1) OrElse (Not esAguinaldo AndAlso restante <= 1) Then
                    tInicio = fin
                    tFin = fin
                Else
                    tFin = tFin.AddDays(2)
                    tInicio = New Date(tFin.Year, tFin.Month, 1)
                    tFin = FinDeMes(tFin)
                End If
                inicio = fin
            Else
                tFin = tFin.AddDays(2)
                tInicio = New Date(tFin.Year, tFin.Month, 1)
                tFin = FinDeMes(tFin)
                inicio = tInicio
            End If

            If FinDeMes(tFin) >= fin Then
                tFin = fin
                If diasMes30 AndAlso tFin.Day > 30 Then tFin = tFin.AddDays(-1)

                total += Acumulado(tInicio, tFin, diasMes30, esAguinaldo, True) - Suspendido(tInicio, tFin, esAguinaldo, suspensiones, 0.0833333)

                tFin = tFin.AddDays(2)
                tInicio = New Date(tFin.Year, tFin.Month, 1)
                tFin = FinDeMes(tFin)
                inicio = tInicio.AddMonths(1)
            End If

        Loop

        Return total

    End Function

    Private Shared Function Suspendido(tInicio As Date, tFin As Date, esAguinaldo As Boolean, suspensiones As Func(Of Date, Date, Double), factor As Double) As Double
        If Not esAguinaldo OrElse suspensiones Is Nothing Then Return 0
        Dim hasta As Date = If(Date.DaysInMonth(tInicio.Year, tInicio.Month) = 31, tFin.AddDays(1), tFin)
        Return suspensiones(tInicio, hasta) * factor
    End Function

    ''' <summary>Días ganados en un tramo, tal como los calculaban CalcularDiasAntiguedad / CalcularDiasAguinaldo.</summary>
    Private Shared Function Acumulado(tInicio As Date, tFin As Date, diasMes30 As Boolean, esAguinaldo As Boolean, ultimoTramo As Boolean) As Double

        Dim d As Integer = CInt((tFin - tInicio).TotalDays)
        Dim diasMes As Integer = Date.DaysInMonth(tInicio.Year, tInicio.Month)

        If tInicio.Month = 2 Then

            If d + 1 >= diasMes Then Return 2.5

            Dim extra As Integer
            If esAguinaldo Then
                ' Solo se completa febrero cuando el tramo llega al 28 o 29.
                If tFin.Day >= 28 AndAlso tFin.Month = 2 Then
                    If diasMes30 Then
                        extra = If(diasMes = 28, 3, 2)
                    ElseIf ultimoTramo Then
                        extra = If(diasMes = 28, 3, 2)
                    Else
                        extra = If(diasMes = 28, 4, 3)
                    End If
                Else
                    extra = 1
                End If
            ElseIf ultimoTramo Then
                extra = 1
            ElseIf diasMes30 Then
                extra = If(diasMes = 28, 3, 2)
            Else
                extra = If(diasMes = 28, 4, 3)
            End If

            Return R((d + extra) * 0.0833, 2)

        End If

        If diasMes30 Then Return If(d + 1 < 30, R((d + 1) * 0.0833, 2), 2.5)

        ' La V.5.0 usaba 0.033 en el último tramo de la antigüedad con meses calendario (se conserva).
        Dim factor As Double = If(ultimoTramo AndAlso Not esAguinaldo, 0.033, 0.0833)
        Return If(d + 1 <= 30, R((d + 1) * factor, 2), 2.58)

    End Function

    ''' <summary>Días entre dos fechas contando meses de 30 días (CalcularDiasVaca).</summary>
    Public Shared Function DiasVacaciones(fechaInicio As Date, fechaFin As Date, diasMes30 As Boolean) As Double

        Dim ini As Date = fechaInicio.Date
        Dim fin As Date = fechaFin.Date
        If Not diasMes30 Then Return (fin - ini).TotalDays

        ' La V.5.0 dejaba sin asignar el fin del primer mes cuando no empezaba el día 1; aquí se toma el fin de ese mes.
        Dim finPrimerMes As Date = FinDeMes(ini)
        Dim dias1 As Double = If(ini.Day > 1, (finPrimerMes - ini).TotalDays + 1, 30)
        Dim finMesAnterior As Date = New Date(fin.Year, fin.Month, 1).AddDays(-1)

        If finMesAnterior > finPrimerMes.AddDays(1) Then
            Dim meses As Integer = MesesEntre(finPrimerMes.AddDays(1), finMesAnterior) + 1
            Dim dias2 As Double = Math.Min(30, (fin - New Date(fin.Year, fin.Month, 1)).TotalDays + 1)
            If fin.Month = 2 AndAlso dias2 >= 28 Then dias2 = 30
            Return dias1 + meses * 30 + dias2
        End If

        If fin > finPrimerMes.AddDays(1) Then
            Dim dias2 As Double = (fin - finPrimerMes.AddDays(1)).TotalDays + 1
            If fin.Month = 2 Then
                If dias2 >= 28 Then dias2 = 30
            ElseIf dias2 >= 30 Then
                dias2 = 30
            End If
            Return dias1 + dias2
        End If

        Return (fin - ini).TotalDays

    End Function

    ''' <summary>Días a pagar de la nómina activa hasta la fecha de egreso (CalcularDiasPlanilla). 0 si no hay nómina activa.</summary>
    Public Shared Function DiasPlanilla(fechaIngreso As Date, fechaEgreso As Date, inicioNomina As Date?, finNomina As Date?) As Double

        If Not inicioNomina.HasValue OrElse Not finNomina.HasValue Then Return 0

        Dim laborados As Double = (finNomina.Value.Date - fechaIngreso.Date).TotalDays + 1
        Dim planilla As Double = (finNomina.Value.Date - inicioNomina.Value.Date).TotalDays + 1
        Dim aPagar As Double = (fechaEgreso.Date - inicioNomina.Value.Date).TotalDays + 1

        If planilla > laborados Then aPagar = laborados

        Return aPagar

    End Function

    ''' <summary>Tiempo trabajado al cargar el empleado (txtCodEmpleado_Change): días, años (÷360) y "m meses y d días".</summary>
    Public Shared Function TiempoAlCargar(contrato As Date, egreso As Date, diasMes As Double, ByRef annos As Double, ByRef mesesTexto As String) As Double

        Dim dias As Double
        If (egreso.Date - contrato.Date).TotalDays + 1 <= 30 Then
            dias = (egreso.Date - contrato.Date).TotalDays + 1
        Else
            dias = R(DiasAntiguedad(contrato, egreso, diasMes = 30) * 12, 0)
        End If

        annos = R(dias / 360, 2)
        mesesTexto = TextoMeses(dias, diasMes)
        Return dias

    End Function

    ''' <summary>Tiempo trabajado al cambiar la fecha de egreso (TxtUltFechaNomina_Change): años ÷358 como en la V.5.0.</summary>
    Public Shared Function TiempoAlCambiarEgreso(contrato As Date, egreso As Date, diasMes As Double, ByRef annos As Double, ByRef mesesTexto As String) As Double

        Dim c As Date = contrato.Date
        Dim e As Date = egreso.Date
        Dim dias As Double

        If (e - c).TotalDays <= 31 Then
            If c.Month = e.Month OrElse c.Month <> 2 Then
                dias = (e - c).TotalDays + 1
            Else
                ' Contrato en febrero: se cuenta febrero completo hasta su último día más los días del mes de egreso.
                dias = ((FinDeMes(c) - c).TotalDays + 1) + ((e - New Date(e.Year, e.Month, 1)).TotalDays + 1)
            End If
        Else
            dias = R(DiasAntiguedad(c, e, diasMes = 30) * 12, 0)
        End If

        annos = R(dias / 358, 2)
        mesesTexto = TextoMeses(dias, diasMes)
        Return dias

    End Function

    Private Shared Function TextoMeses(dias As Double, diasMes As Double) As String
        If diasMes <= 0 Then Return ""
        Dim meses As Double = dias / diasMes
        Dim mes As Integer = CInt(Math.Floor(meses))
        Dim dia As Double = R((meses - mes) * diasMes, 0)
        Return mes & " m y " & dia & " d"
    End Function

#End Region

#Region "SALARIOS"

    ''' <summary>Salario promedio y más alto de los meses (rama "salario variable" de la V.5.0).</summary>
    ''' <param name="sueldoActualSiPocos">Con "salario promedio real" y menos de 6 meses, la V.5.0 usaba el sueldo actual.</param>
    Public Shared Sub PromedioYAlto(meses As IList(Of SalarioMesBaja), promedioReal As Boolean, sueldoActualSiPocos As Double,
                                    ByRef promedio As Double, ByRef alto As Double)

        promedio = 0
        alto = 0
        If meses Is Nothing OrElse meses.Count = 0 Then Return

        Dim total As Double = meses.Sum(Function(m) m.Total_Ingresos)
        alto = Math.Max(0, meses.Max(Function(m) m.Total_Ingresos))

        If promedioReal AndAlso meses.Count < 6 Then
            promedio = sueldoActualSiPocos
        Else
            promedio = total / meses.Count
        End If

    End Sub

    ''' <summary>Sueldo mensual de un empleado de salario fijo según el tipo de nómina (último mes del historial).</summary>
    Public Shared Function SueldoMensualFijo(emp As EmpleadoBaja, ultimoMes As SalarioMesBaja, sueldoActualBasico As Boolean, diasMes As Double) As Double

        If ultimoMes Is Nothing Then Return 0

        Select Case emp.Periodo
            Case "Quincenal"
                Return emp.Sueldo_Periodo * 2
            Case "Mensual"
                Return emp.Sueldo_Periodo
            Case "Catorcenal los Sabados"
                Return If(sueldoActualBasico, ultimoMes.Salario_Basico, emp.Sueldo_Periodo / 14 * diasMes)
            Case Else
                ' La V.5.0 no tenía caso para los demás períodos y dejaba el sueldo en 0.
                Return 0
        End Select

    End Function

#End Region

#Region "LIQUIDACION"

    ''' <summary>Montos de la liquidación (CmdCalculos_Click de la V.5.0), sin el IR.</summary>
    Public Shared Function Calcular(emp As EmpleadoBaja, opc As OpcionesLiquidacion, datos As DatosCalculo) As Resultado

        Dim diasMes As Double = If(datos.Dias_Mes > 0, datos.Dias_Mes, 30)
        Dim mes30 As Boolean = diasMes = 30
        Dim contrato As Date = emp.Fecha_Contrato.Value.Date
        Dim egreso As Date = opc.Fecha_Egreso.Date
        Dim basico As Double = opc.Salario_Basico
        Dim promedio As Double = opc.Salario_Promedio
        Dim alto As Double = opc.Salario_Alto

        Dim sueldoPeriodo As Double = emp.Sueldo_Periodo * If(emp.Dolarizado, datos.Tasa_Cambio, 1)
        Dim sab As Boolean? = emp.Sueldo_Actual_Basico
        Dim sueldoActual As Double = If(sab.GetValueOrDefault(), emp.Sueldo_Actual, 0)

        ' ---- Salario base de aguinaldo y de vacaciones ----
        Dim salarioMesAgui As Double = If(alto > basico, alto, basico)
        Dim mayorDe As Func(Of Double) = Function() If(promedio > basico, promedio, basico)
        Dim salarioMesVaca As Double
        Dim antiguedadDias As Double = (egreso - contrato).TotalDays

        If opc.Antiguedad_Base Then
            If antiguedadDias >= 180 Then
                If emp.Salario_Fijo = "N" Then
                    salarioMesVaca = If(promedio > sueldoPeriodo, promedio, basico)
                Else
                    salarioMesVaca = sueldoPeriodo
                End If
            Else
                salarioMesVaca = If(sab.HasValue, mayorDe(), basico)
            End If
        ElseIf antiguedadDias >= 180 Then
            salarioMesVaca = If(sab.GetValueOrDefault(), mayorDe(), sueldoPeriodo)
        Else
            salarioMesVaca = If(sab.HasValue, mayorDe(), basico)
        End If

        Dim diarioAgui As Double = salarioMesAgui / diasMes
        Dim diarioVaca As Double = salarioMesVaca / diasMes
        Dim horaVaca As Double = diarioVaca / 8

        ' ---- Días desde la última nómina ----
        Dim montoPropor As Double = 0
        If Not opc.Pagar_Dias_Planilla AndAlso opc.Dias_Planilla > 0 Then
            If sab.HasValue Then
                montoPropor = If(sueldoActual = 0, sueldoPeriodo, sueldoActual) / diasMes * opc.Dias_Planilla
            Else
                montoPropor = basico / diasMes * opc.Dias_Planilla
            End If
        End If

        ' ---- Días ganados de antigüedad, vacaciones y aguinaldo ----
        Dim fc As Date = AjusteFebrero(contrato, contrato)
        Dim diasAntigue, diasVacaTrab, diasAguiTrab As Double
        Dim totalDias As Double = (egreso - fc).TotalDays + 1

        If totalDias < 30 Then
            diasAntigue = 0 : diasVacaTrab = 0 : diasAguiTrab = 0
        ElseIf totalDias <= 31 Then
            diasAntigue = 2.5 : diasVacaTrab = 2.5 : diasAguiTrab = 2.5
        Else
            fc = contrato
            diasAntigue = DiasAntiguedad(contrato, egreso, mes30)
            If (egreso - opc.Fecha_Ini_Vacaciones.Date).TotalDays < 30 Then
                diasVacaTrab = ((egreso - opc.Fecha_Ini_Vacaciones.Date).TotalDays + 1) * 0.0833
            Else
                diasVacaTrab = DiasAguinaldo(opc.Fecha_Ini_Vacaciones, egreso, mes30, datos.Suspensiones)
            End If
            diasAguiTrab = DiasAguinaldo(opc.Fecha_Ini_Aguinaldo, egreso, mes30, datos.Suspensiones)
        End If

        ' ---- Aguinaldo ----
        Dim diasAgui As Double = 0
        If opc.Aguinaldo Then
            Dim fa As Date = If(fc.Month = 2, AjusteFebrero(fc, contrato), opc.Fecha_Ini_Aguinaldo.Date)
            If (egreso - fa).TotalDays < 30 Then
                diasAgui = Math.Max(0, (egreso - fa).TotalDays + 1) * 0.08333
            Else
                diasAgui = diasAguiTrab
            End If
        End If
        ' La V.5.0 restaba los adelantos de 13vo y luego recalculaba el monto sin restarlos: se conserva el monto recalculado.
        Dim totalAgui As Double = R(R(diasAgui, 2) * R(diarioAgui, 4), 2)

        ' ---- Vacaciones ----
        Dim diasVac As Double = 0
        Dim vacacionesBase As Double = 0
        If opc.Vacaciones Then
            If opc.Dias_Vacaciones_Manual.HasValue Then diasVacaTrab = opc.Dias_Vacaciones_Manual.Value
            diasVacaTrab = R(diasVacaTrab, 2)
            diasVac = diasVacaTrab - R(opc.Dias_Descuento_Vacaciones, 2)
            vacacionesBase = diasVac * diarioVaca - datos.Adelanto_Vacaciones
        End If
        Dim totalVac As Double = R(R(diasVac, 2) * R(diarioVaca, 2), 2)

        ' ---- Horas extra ----
        Dim he As Double = 0, montoHe As Double = 0
        If opc.Horas_Extra Then
            he = datos.Horas_Extra_Pendientes
            montoHe = he * horaVaca * 2
        End If

        ' ---- Indemnización por antigüedad ----
        Dim totalAntig As Double = 0
        If opc.Antiguedad Then
            Dim diasA As Double = opc.Dias_Trabajados
            Dim meses As Double = diasA / diasMes
            Dim mes As Integer = CInt(Math.Floor(meses))
            Dim dia As Double = R((meses - mes) * diasMes, 0)

            If meses / 12 >= 3 Then
                If Math.Floor(meses / 12) >= 6 Then
                    diasAntigue = diasMes * 5
                    totalAntig = diarioVaca * diasMes * 5
                Else
                    diasAntigue = 90 + (mes - 36) * 1.67 + (dia / diasMes) * 1.67
                    totalAntig = R(R(diarioVaca, 4) * R(diasAntigue, 2), 2)
                End If
            ElseIf Math.Floor(meses / 12) >= 1 Then
                totalAntig = R(R(diarioVaca, 4) * R(diasAntigue, 2), 2)
            ElseIf datos.Antiguedad_Menor AndAlso diasA > diasMes Then
                totalAntig = diarioVaca * diasAntigue
            End If
        End If

        ' ---- Otros ingresos ----
        Dim otros As Double = If(opc.Otros_Ingresos, opc.Monto_Otros_Ingresos, 0)
        If opc.Otros_Ingresos_Planilla Then otros += datos.Otros_Ingresos_Planilla

        ' ---- Egresos ----
        Dim prestamo As Double = If(opc.Prestamos, datos.Cuotas_Prestamo, 0)
        Dim deducciones As Double = If(opc.Deducciones, datos.Deducciones_Pendientes, 0)
        Dim otrasDed As Double = If(opc.Otras_Deducciones, opc.Monto_Otras_Deducciones, 0)
        Dim inss As Double = (vacacionesBase + montoHe + otros + montoPropor) * emp.Tasa_Inss
        Dim baseIr As Double = (montoHe + otros + montoPropor) * (1 - emp.Tasa_Inss)

        Dim liq As New Liquidacion With {
            .Cod_Empleado = emp.Cod_Empleado,
            .Fecha_Baja = egreso,
            .Annos_Trabajados = opc.Annos,
            .Meses_Trabajados = MesesDelTexto(opc.Meses_Texto),
            .Dias_Antiguedad = R(diasAntigue, 2),
            .Monto_Nom_Propor = R(montoPropor, 2),
            .Monto_Vacaciones = totalVac,
            .Monto_Aguinaldo = totalAgui,
            .Monto_Antiguedad = R(totalAntig, 2),
            .Horas_Extra = he,
            .Monto_Horas_Extra = R(montoHe, 2),
            .Otro = Recortar(opc.Concepto_Otros_Ingresos, 25),
            .Monto_Otro = R(otros, 2),
            .Fecha_Ini_Vacaciones = opc.Fecha_Ini_Vacaciones.Date,
            .Fecha_Ini_Aguinaldo = opc.Fecha_Ini_Aguinaldo.Date,
            .Dias_Aguinaldo = R(diasAgui, 2),
            .Dias_Vacaciones = R(diasVac, 2),
            .Dias_Menos_Vacaciones = opc.Dias_Descuento_Vacaciones,
            .Salario_Mensual = R(salarioMesVaca, 2),
            .Tipo_Baja = opc.Tipo_Baja,
            .Motivo_Baja = Recortar(opc.Motivo, 255),
            .Prestamo = R(prestamo, 2),
            .Deducciones = R(deducciones, 2),
            .Monto_Inss = R(inss, 2),
            .Concepto_Otras_Deducciones = Recortar(opc.Concepto_Otras_Deducciones, 50),
            .Monto_Otras_Deducciones = R(otrasDed, 2),
            .Dias_Laborados = opc.Dias_Planilla,
            .Sueldo_Actual_Opt = opc.Sueldo_Actual_Basico,
            .Prestamo_Opt = opc.Prestamos,
            .Deducciones_Opt = opc.Deducciones,
            .Aguinaldo_Opt = opc.Aguinaldo,
            .Antiguedad_Opt = opc.Antiguedad,
            .Viaticos_Opt = opc.Viaticos,
            .Vacaciones_Opt = opc.Vacaciones,
            .Horas_Extra_Opt = opc.Horas_Extra,
            .Otros_Ingresos_Opt = opc.Otros_Ingresos,
            .Otros_Ingresos_Planilla_Opt = opc.Otros_Ingresos_Planilla,
            .Pagar_Dias_Planilla_Opt = opc.Pagar_Dias_Planilla,
            .Dias_Vacaciones_Acumulados = R(diasVacaTrab, 2),
            .Salario_Basico = basico,
            .Salario_Alto = alto,
            .Salario_Por_Dia = R(alto / 30, 2)
        }

        Return New Resultado With {.Liquidacion = liq, .Base_Ir_Mensual = baseIr, .Vacaciones_Base = vacacionesBase}

    End Function

    ''' <summary>La V.5.0 retrocedía 2 días (febrero de 28) o 1 (de 29) la fecha cuando el contrato era en febrero.</summary>
    Private Shared Function AjusteFebrero(fecha As Date, contrato As Date) As Date
        If fecha.Month <> 2 Then Return fecha
        Select Case FinDeMes(fecha).Day
            Case 28 : Return contrato.AddDays(-2)
            Case 29 : Return contrato.AddDays(-1)
        End Select
        Return fecha
    End Function

    ''' <summary>Número inicial de "40 m y 5 d" (la V.5.0 grababa Val(TxtMeses)).</summary>
    Private Shared Function MesesDelTexto(texto As String) As Double
        Dim digitos As String = New String((If(texto, "")).TakeWhile(Function(c) Char.IsDigit(c)).ToArray())
        Return If(digitos = "", 0, CDbl(digitos))
    End Function

#End Region

    Friend Shared Function R(valor As Double, decimales As Integer) As Double
        Return Math.Round(valor, decimales, MidpointRounding.AwayFromZero)
    End Function

    Private Shared Function Recortar(texto As String, largo As Integer) As String
        texto = If(texto, "").Trim()
        Return If(texto.Length > largo, texto.Substring(0, largo), texto)
    End Function

    Private Shared Function FinDeMes(fecha As Date) As Date
        Return New Date(fecha.Year, fecha.Month, 1).AddMonths(1).AddDays(-1)
    End Function

    Private Shared Function MesesEntre(desde As Date, hasta As Date) As Integer
        Return (hasta.Year - desde.Year) * 12 + hasta.Month - desde.Month
    End Function

End Class
