''' <summary>
''' Cálculo de la nómina de vacaciones de un empleado, portado de <c>CmdCalVaca_Click</c> de Frm13VacaMes (V.5.0) sin acceso a
''' datos: <see cref="CsNominaVacacionesDAL"/> entrega los datos del empleado y de sus meses y recibe los montos a grabar.
''' </summary>
Public Class CalculadoraNominaVacaciones

    Public Const PeriodoCatorcenal As String = "Catorcenal los Sabados"

    ''' <summary>Parámetros de la nómina; son los mismos para todos los empleados.</summary>
    Public Class Contexto
        Public Property Dias_Mes As Double = 30
        Public Property Periodo As String = ""
        Public Property Tasa_Cambio As Double = 1
        ''' <summary>TipoNomina.TasaInss cuando el tipo de nómina calcula INSS por porcentaje; si no, 0.</summary>
        Public Property Tasa_Inss As Double
        ''' <summary>Solo entra quien tiene al menos estos días de vacaciones.</summary>
        Public Property Antiguedad_Minima As Double
        ''' <summary>Días de descuento que se digitan para todos los empleados (máximo 15).</summary>
        Public Property Dias_Descuento As Double
    End Class

    ''' <summary>Lo que ganó el empleado en un mes de planilla del rango de salarios.</summary>
    Public Class MesIngreso
        Public Property Mes As Integer
        Public Property Ano As Integer
        ''' <summary>Salario, destajo, séptimo, otros ingresos, incentivos y horas extra del mes; 0 si no cobró.</summary>
        Public Property Total As Double
        ''' <summary>Cantidad de períodos de planilla del mes (solo importa en nómina catorcenal).</summary>
        Public Property Cant_Planilla As Integer
    End Class

    Public Class DatosEmpleado
        Public Property Cod_Empleado As String = ""
        Public Property Cod_Empleado1 As String = ""
        ''' <summary>Empleado.SalarioFijo = "N": el salario es el promedio de lo cobrado en los meses del rango.</summary>
        Public Property Salario_Variable As Boolean
        Public Property Sueldo_Periodo As Double
        Public Property Tarifa_Horaria As Double
        Public Property Sueldo_Actual_Basico As Boolean
        Public Property Sueldo_Actual As Double?
        ''' <summary>Saldo de vacaciones al cierre del período (CalculoDiasVacaciones); 0 si el empleado no tiene fecha de contrato.</summary>
        Public Property Dias_Saldo As Double
        ''' <summary>Días acumulados sin restar solicitudes (CalculoDiasVacaSFicha).</summary>
        Public Property Dias_Acumulados As Double
        ''' <summary>Meses del rango de salarios, en orden; el mes sin nómina trae Total = 0.</summary>
        Public ReadOnly Property Meses As New List(Of MesIngreso)
        ''' <summary>Días de vacaciones ya aprobados dentro del período de la nómina.</summary>
        Public Property Dias_Solicitados As Double
        Public Property Adelanto As Double
        ''' <summary>Del mes más reciente con nómina: lo que ganó, el INSS y el IR que se le retuvieron.</summary>
        Public Property Ingreso_Ultimo_Mes As Double
        Public Property Inss_Ultimo_Mes As Double
        Public Property Ir_Ultimo_Mes As Double
        ''' <summary>IR que corresponde a un salario mensual; Nothing = sin IR.</summary>
        Public Property Calcular_Ir As Func(Of Double, Double)
    End Class

    Public Class Resultado
        ''' <summary>Falso si el empleado no llega a la antigüedad mínima.</summary>
        Public Property Procede As Boolean
        ''' <summary>Verdadero si tiene salario y por tanto se graba el detalle.</summary>
        Public Property Graba As Boolean
        Public Property Salario_Mensual As Double
        Public Property Dias_A_Pagar As Double
        Public Property Dias_Descuento As Double
        Public Property Adelanto As Double
        Public Property Inss As Double
        Public Property Ir As Double
        Public Property Total_Devengado As Double
        ''' <summary>Promedio de los meses que se guarda en HistorialSalarioMes (índice 1 a 12); solo salario variable.</summary>
        Public ReadOnly Property Salarios As Double() = New Double(12) {}
    End Class

    Public Shared Function Calcular(e As DatosEmpleado, c As Contexto) As Resultado

        Dim r As New Resultado

        Dim dias As Double = Math.Min(Math.Max(e.Dias_Saldo, 0), 15)
        Dim diasNomina As Double = e.Dias_Acumulados
        If diasNomina > 15 Then
            diasNomina = 15
            dias = 15
        End If

        If dias < c.Antiguedad_Minima Then Return r
        r.Procede = True

        Dim tarifa As Double = e.Tarifa_Horaria
        If tarifa = 0 Then tarifa = TarifaDelSueldo(e, c, tarifa)
        Dim salarioBasico As Double = Math.Round(c.Dias_Mes * 8 * tarifa, 2)

        Dim salMayor As Double
        Dim sueldoActual As Boolean = False

        If e.Salario_Variable Then

            If dias > c.Antiguedad_Minima AndAlso e.Meses.Count > 0 Then
                If e.Sueldo_Actual_Basico Then
                    sueldoActual = True
                    salarioBasico = If(e.Sueldo_Actual, 0)
                End If
                salMayor = PromedioMeses(e, c, r.Salarios)
            End If

        Else

            ' Salario fijo: lo que corresponde a un mes según el sueldo del período.
            salMayor = Math.Round(c.Dias_Mes * 8 * TarifaDelSueldo(e, c, tarifa), 2)
            Select Case c.Periodo
                Case "Trimestral" : salMayor *= 3
                Case "Semestral" : salMayor *= 6
                Case "Semanal Viernes" : salMayor *= 2
            End Select

        End If

        salMayor = If(sueldoActual, salarioBasico, Math.Max(salarioBasico, salMayor))

        If salMayor <= 0 Then Return r

        r.Graba = True
        r.Salario_Mensual = salMayor
        r.Dias_A_Pagar = diasNomina
        r.Dias_Descuento = c.Dias_Descuento + e.Dias_Solicitados
        r.Adelanto = e.Adelanto

        Dim diasPagados As Double = Math.Max(diasNomina - r.Dias_Descuento, 0)
        r.Total_Devengado = salMayor * diasPagados / c.Dias_Mes
        r.Inss = r.Total_Devengado * (c.Tasa_Inss / 100)

        If e.Calcular_Ir IsNot Nothing AndAlso diasPagados > 0 Then
            Dim parte As Double = (salMayor / c.Dias_Mes) * (diasPagados / 12)
            Dim acumulado As Double = e.Ingreso_Ultimo_Mes + parte - e.Inss_Ultimo_Mes - parte * (c.Tasa_Inss / 100)
            r.Ir = Math.Max(e.Calcular_Ir(acumulado) - e.Ir_Ultimo_Mes, 0)
        End If

        Return r

    End Function

    ''' <summary>Tarifa por hora que sale del sueldo del período (por la tasa de cambio) según el período del tipo de nómina.</summary>
    Private Shared Function TarifaDelSueldo(e As DatosEmpleado, c As Contexto, porDefecto As Double) As Double

        Dim sueldo As Double = e.Sueldo_Periodo * c.Tasa_Cambio

        Select Case c.Periodo
            Case PeriodoCatorcenal : Return Math.Round(sueldo / 112, 2)
            Case "Quincenal" : Return Math.Round(sueldo / ((c.Dias_Mes * 8) / 2), 2)
            Case "Mensual" : Return Math.Round(sueldo / (c.Dias_Mes * 8), 2)
            Case "Trimestral" : Return Math.Round(sueldo / (c.Dias_Mes * 8 * 3), 2)
            Case "Semestral" : Return Math.Round(sueldo / (c.Dias_Mes * 8 * 6), 2)
        End Select

        Return porDefecto

    End Function

    ''' <summary>Promedio de los meses con nómina; también deja en <paramref name="salarios"/> lo de cada mes para el historial.</summary>
    Private Shared Function PromedioMeses(e As DatosEmpleado, c As Contexto, salarios As Double()) As Double

        Dim suma As Double = 0
        Dim cantidad As Integer = 0

        For Each m As MesIngreso In e.Meses

            If m.Total = 0 Then Continue For

            Dim v As Double = m.Total
            If c.Periodo = PeriodoCatorcenal AndAlso m.Cant_Planilla > 0 Then
                v = If(m.Cant_Planilla = 3, (v / 42) * 30, (v / 28) * 30)
            End If

            cantidad += 1
            suma += v
            If m.Mes >= 1 AndAlso m.Mes <= 12 Then salarios(m.Mes) = v

        Next

        If suma < 0 Then suma = 0

        Return If(cantidad <> 0, suma / cantidad, 0)

    End Function

End Class
