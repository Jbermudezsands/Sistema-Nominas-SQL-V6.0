''' <summary>
''' Cálculo del 13vo mes (aguinaldo) de un empleado, portado de <c>CmdCal13_Click</c>, <c>SalMayorVariable</c> y
''' <c>CalcularHistorialMes</c> de Frm13VacaMes (V.5.0) sin acceso a datos: quien llama entrega los datos del empleado y de
''' sus meses (<see cref="CsAguinaldoDAL"/>) y recibe los montos a grabar.
''' </summary>
Public Class CalculadoraAguinaldo

    Public Const PeriodoCatorcenal As String = "Catorcenal los Sabados"

    ''' <summary>Parámetros del período que se calcula; son los mismos para todos los empleados.</summary>
    Public Class Contexto
        Public Property Fecha_Ini As Date
        Public Property Fecha_Fin As Date
        ''' <summary>Primer y último mes (1 a 12) de los seis que entran al cálculo.</summary>
        Public Property Mes_Ini As Integer
        Public Property Mes_Fin As Integer
        ''' <summary>Periodo del tipo de nómina ("Quincenal", "Mensual", "Catorcenal los Sabados", ...).</summary>
        Public Property Periodo As String = ""
        Public Property Tipo_Pago As String = ""
        Public Property Dias_Mes As Double = 30
        ''' <summary>Solo se paga a quien tiene al menos estos días acumulados.</summary>
        Public Property Antiguedad_Minima As Double
        ''' <summary>Cantidad de períodos de planilla por mes (índice 1 a 12); solo importa en nómina catorcenal.</summary>
        Public ReadOnly Property Cant_Planilla As Integer() = New Integer(12) {}
    End Class

    ''' <summary>Datos del empleado y de lo que cobró cada mes del período.</summary>
    Public Class DatosEmpleado
        Public Property Cod_Empleado As String = ""
        Public Property Cod_Empleado1 As String = ""
        ''' <summary>Empleado.SalarioFijo = "N" (destajo, comisiones u horas variables: se toma el mes más alto).</summary>
        Public Property Salario_Variable As Boolean
        Public Property Sueldo_Periodo As Double
        Public Property Tarifa_Horaria As Double
        Public Property Monto_Pension As Double
        Public Property Porcentaje_Pension As Double
        Public Property Fecha_Contrato As Date?
        Public Property Sueldo_Actual As Double?
        Public Property Sueldo_Actual_Basico As Boolean
        Public Property Sal_Porcentaje As Double
        Public Property Adelanto As Double
        Public Property Dias_Suspension As Double
        ''' <summary>Días de suspensión con fecha de inicio entre dos fechas (restan días de aguinaldo).</summary>
        Public Property Suspensiones As Func(Of Date, Date, Double) = Function(d, h) 0
        ' Por mes (índice 1 a 12)
        Public ReadOnly Property Devengado As Double() = New Double(12) {}
        Public ReadOnly Property Horas_Extra As Double() = New Double(12) {}
        Public ReadOnly Property Ingresos As Double() = New Double(12) {}
        Public ReadOnly Property Subsidio As Double() = New Double(12) {}
    End Class

    Public Class Resultado
        ''' <summary>Falso si el empleado no llega a la antigüedad mínima: no se calcula nada.</summary>
        Public Property Procede As Boolean
        ''' <summary>Verdadero si hay monto neto distinto de cero y por tanto se graba el detalle.</summary>
        Public Property Graba As Boolean
        Public Property Dias As Double
        Public Property Salario_Mensual As Double
        Public Property Neto_Mensual As Double
        Public Property Salario_A_Pagar As Double
        Public Property Monto_Pension As Double
        Public Property Monto_Porcentaje As Double
        Public Property Monto_Suspension As Double
        Public Property Dias_Suspension As Double
        Public Property Adelanto As Double
        ''' <summary>Salario de cada mes (índice 1 a 12) que se guarda en HistorialSalarioMes.</summary>
        Public ReadOnly Property Salarios As Double() = New Double(12) {}
    End Class

    Public Shared Function Calcular(e As DatosEmpleado, c As Contexto) As Resultado

        Dim r As New Resultado With {.Dias = DiasAguinaldo(e, c)}

        If r.Dias < c.Antiguedad_Minima Then Return r

        r.Procede = True

        Dim tarifa As Double = e.Tarifa_Horaria
        If c.Periodo = PeriodoCatorcenal AndAlso c.Tipo_Pago = "Salario Fijo" Then tarifa = e.Sueldo_Periodo / 112

        Dim salMayor As Double = Math.Round(c.Dias_Mes * 8 * tarifa, 2)

        If e.Salario_Variable Then
            salMayor = SalMayorVariable(e, c, salMayor)
        Else
            salMayor = SalarioFijo(e, c)
        End If

        Dim montoPct As Double = If(e.Porcentaje_Pension > 0, salMayor * (e.Porcentaje_Pension / 100), 0)

        r.Salario_Mensual = salMayor
        r.Monto_Pension = e.Monto_Pension
        r.Monto_Porcentaje = montoPct
        r.Neto_Mensual = salMayor - e.Monto_Pension - montoPct

        HistorialMes(e, c, r.Salarios)

        If r.Neto_Mensual = 0 Then Return r

        r.Graba = True

        If r.Dias <> 0 AndAlso r.Dias < 30 Then
            r.Salario_A_Pagar = (r.Neto_Mensual * Math.Round(r.Dias, 2)) / c.Dias_Mes
        ElseIf r.Dias >= 30 Then
            r.Salario_A_Pagar = r.Neto_Mensual
        End If

        r.Adelanto = e.Adelanto
        r.Dias_Suspension = e.Dias_Suspension
        r.Monto_Suspension = (Math.Round(salMayor, 2) / 30) * Math.Round(e.Dias_Suspension * 0.08333333, 2)

        Return r

    End Function

    ''' <summary>
    ''' Días de aguinaldo ganados en el período (CalcularDiasAguinaldo). Contrato anterior al período: desde el inicio del
    ''' período; contrato de hasta 2 días antes del cierre: los días más 0.08333; si no, desde el contrato.
    ''' </summary>
    Public Shared Function DiasAguinaldo(e As DatosEmpleado, c As Contexto) As Double

        If Not e.Fecha_Contrato.HasValue Then Return 0

        Dim contrato As Date = e.Fecha_Contrato.Value.Date
        Dim fin As Date = c.Fecha_Fin.Date
        Dim esMes30 As Boolean = (c.Dias_Mes = 30)

        If contrato < c.Fecha_Ini.Date Then
            Return CalculadoraLiquidacion.DiasAguinaldo(c.Fecha_Ini.Date, fin, esMes30, e.Suspensiones)
        End If

        Dim diferencia As Integer = (fin - contrato).Days

        If diferencia <= 2 Then Return diferencia + 0.08333

        Return CalculadoraLiquidacion.DiasAguinaldo(contrato, fin, esMes30, e.Suspensiones)

    End Function

    ''' <summary>Empleado con salario fijo: sueldo del período (con su porcentaje), o el sueldo actual si así está marcado.</summary>
    Private Shared Function SalarioFijo(e As DatosEmpleado, c As Contexto) As Double

        Dim salMayor As Double = e.Sueldo_Periodo * (1 + e.Sal_Porcentaje / 100)

        If c.Periodo = PeriodoCatorcenal Then salMayor = c.Dias_Mes * (e.Sueldo_Periodo / 14)

        If e.Sueldo_Actual_Basico Then
            If Not e.Sueldo_Actual.HasValue Then
                salMayor = 0
            Else
                Select Case c.Periodo
                    Case "Quincenal", "Mensual" : salMayor = e.Sueldo_Actual.Value
                    Case PeriodoCatorcenal : salMayor = (e.Sueldo_Actual.Value / 14) * c.Dias_Mes
                End Select
            End If
        End If

        Return salMayor

    End Function

    ''' <summary>
    ''' Empleado con salario variable: el mes más alto de los seis, con sus ingresos y subsidio. Si cobró horas extra todos los
    ''' meses, el mejor mes con horas extra puede reemplazarlo (SalMayorVariable).
    ''' </summary>
    Private Shared Function SalMayorVariable(e As DatosEmpleado, c As Contexto, inicial As Double) As Double

        Dim salMayor As Double = inicial
        Dim salMayorConHE As Double = 0
        Dim calculaHE As Boolean = True

        For mes As Integer = c.Mes_Ini To c.Mes_Fin
            Dim salMes As Double = e.Devengado(mes)
            Dim salMesHE As Double = e.Devengado(mes) + e.Horas_Extra(mes)

            If c.Periodo = PeriodoCatorcenal AndAlso c.Cant_Planilla(mes) > 0 Then
                salMes = ProporcionCatorcenal(salMes, c.Cant_Planilla(mes))
            End If

            If e.Horas_Extra(mes) <= 0 Then calculaHE = False

            If e.Sueldo_Actual_Basico Then salMes = If(e.Sueldo_Actual, 0) + e.Ingresos(mes)

            salMes += e.Subsidio(mes)

            If salMayor < salMes Then salMayor = salMes
            If salMayorConHE < salMesHE Then salMayorConHE = salMesHE
        Next

        If calculaHE AndAlso salMayorConHE > salMayor Then salMayor = salMayorConHE

        Return salMayor

    End Function

    ''' <summary>Lo que se guarda en HistorialSalarioMes: devengado + horas extra de cada mes (sueldo actual si está marcado), más subsidio.</summary>
    Private Shared Sub HistorialMes(e As DatosEmpleado, c As Contexto, salarios As Double())

        For mes As Integer = c.Mes_Ini To c.Mes_Fin

            Dim v As Double = e.Devengado(mes) + e.Horas_Extra(mes)

            If c.Periodo = PeriodoCatorcenal AndAlso c.Cant_Planilla(mes) > 0 Then
                v = ProporcionCatorcenal(v, c.Cant_Planilla(mes))
            End If

            If e.Sueldo_Actual_Basico Then v = If(e.Sueldo_Actual, 0) + e.Ingresos(mes)

            salarios(mes) = v + e.Subsidio(mes)

        Next

    End Sub

    ''' <summary>Un mes catorcenal con 3 pagos son 42 días; con 2, 28: se lleva a 30 días.</summary>
    Private Shared Function ProporcionCatorcenal(monto As Double, pagosEnElMes As Integer) As Double
        Return If(pagosEnElMes = 3, (monto / 42) * 30, (monto / 28) * 30)
    End Function

End Class
