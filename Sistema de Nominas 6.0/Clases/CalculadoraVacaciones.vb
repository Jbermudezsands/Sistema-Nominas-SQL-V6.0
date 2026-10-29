''' <summary>
''' Saldo de días de vacaciones de un empleado, portado de CalculoDiasVacaciones (Funciones.bas, V.5.0): recorre mes a mes desde
''' la fecha de contrato para vacaciones acumulando 0.0833 días por día (2.5 por mes completo con 30 días por mes, 2.58 con
''' meses de 31) y resta los días solicitados en cada mes. Sin acceso a datos: los días solicitados los entrega quien llama.
''' </summary>
Public Class CalculadoraVacaciones

    ''' <summary>Días solicitados con FechaInicio entre <c>desde</c> y <c>hasta</c> (ambos inclusive).</summary>
    Public Delegate Function DiasSolicitados(desde As Date, hasta As Date) As Double

    ''' <param name="diasMes30">Controles.DiasMes = 30; cualquier otro valor se trata como mes calendario.</param>
    Public Shared Function Saldo(contratoVacaciones As Date, hasta As Date, diasMes30 As Boolean, solicitados As DiasSolicitados) As Double

        Dim inicio As Date = contratoVacaciones.Date
        Dim fin As Date = hasta.Date

        Dim tInicio As Date = inicio
        Dim tFin As Date = FinDeMes(inicio)
        Dim saldoActual As Double = 0

        Do While inicio < fin

            If diasMes30 AndAlso tFin.Day > 30 Then tFin = tFin.AddDays(-1)

            saldoActual += Acumulado(tInicio, tFin, diasMes30, False) - Solicitado(tInicio, tFin, solicitados)

            If tFin.AddDays(2) >= fin Then

                ' Lo que queda de fecha final cae en este mes (o a 2 días de su cierre): termina el recorrido.
                inicio = fin

            Else

                tFin = tFin.AddDays(2)
                tInicio = New Date(tFin.Year, tFin.Month, 1)
                tFin = FinDeMes(tFin)
                inicio = tInicio

                ' El mes siguiente contiene la fecha final: se acumula solo hasta ella y termina el recorrido.
                If tFin >= fin Then

                    tFin = fin
                    If diasMes30 AndAlso tFin.Day > 30 Then tFin = tFin.AddDays(-1)

                    saldoActual += Acumulado(tInicio, tFin, diasMes30, True) - Solicitado(tInicio, tFin, solicitados)

                    inicio = fin

                End If

            End If

        Loop

        Return saldoActual

    End Function

    Private Shared Function Solicitado(tInicio As Date, tFin As Date, solicitados As DiasSolicitados) As Double

        ' En meses de 31 días la V.5.0 incluía el día siguiente al cierre del período.
        Dim limite As Date = If(Date.DaysInMonth(tInicio.Year, tInicio.Month) = 31, tFin.AddDays(1), tFin)

        Return solicitados(tInicio, limite)

    End Function

    ''' <summary>
    ''' Días ganados en el tramo. En febrero la V.5.0 sumaba días extra para equipararlo a 30 días; en el último tramo
    ''' (<paramref name="ultimoTramo"/>) solo sumaba 1.
    ''' </summary>
    Private Shared Function Acumulado(tInicio As Date, tFin As Date, diasMes30 As Boolean, ultimoTramo As Boolean) As Double

        Dim d As Integer = CInt((tFin - tInicio).TotalDays)
        Dim diasMes As Integer = Date.DaysInMonth(tInicio.Year, tInicio.Month)

        If tInicio.Month = 2 Then

            Dim extra As Integer

            If ultimoTramo Then
                extra = 1
            ElseIf diasMes30 Then
                extra = If(diasMes = 28, 3, 2)
            Else
                extra = If(diasMes = 28, 4, 3)
            End If

            Return If(d + 1 < diasMes, Redondear((d + extra) * 0.0833), 2.5)

        End If

        If diasMes30 Then
            Return If(d + 1 < 30, Redondear((d + 1) * 0.0833), 2.5)
        End If

        Return If(d + 1 <= 30, Redondear((d + 1) * 0.0833), 2.58)

    End Function

    Private Shared Function Redondear(valor As Double) As Double
        Return Math.Round(valor, 2, MidpointRounding.AwayFromZero)
    End Function

    Private Shared Function FinDeMes(fecha As Date) As Date
        Return New Date(fecha.Year, fecha.Month, 1).AddMonths(1).AddDays(-1)
    End Function

End Class
