''' <summary>
''' IR de la liquidación, port de CalcularMontoIrBajas / BuscarIrAnual (FrmBajas.frm, V.5.0) sin acceso a datos.
''' La tabla IR es anual; el primer tramo (exento) se omite como hacía la V.5.0 con MoveNext.
''' </summary>
Public Class CalculadoraIRBaja

    Public Class TramoIR
        Public Property Desde As Double
        Public Property Hasta As Double
        Public Property Impuesto_Base As Double
        Public Property Porciento As Double
        Public Property Sobre_Exceso As Double

        Public Function Contiene(monto As Double) As Boolean
            Return Desde <= monto AndAlso Hasta >= monto
        End Function

        Public Function Impuesto(monto As Double) As Double
            Return (monto - Sobre_Exceso) * (Porciento / 100) + Impuesto_Base
        End Function
    End Class

    ''' <summary>IR a retener en la liquidación.</summary>
    ''' <param name="periodo">TipoNomina.Periodo del empleado.</param>
    ''' <param name="tramos">Tabla IR ordenada por Desde (incluye el tramo exento).</param>
    ''' <param name="brutoMensual">Ingreso del mes sin INSS (nómina del mes + ingresos de la baja).</param>
    ''' <param name="vacaciones">Vacaciones sin INSS; en nómina quincenal se calcula el IR del incremento anual que producen.</param>
    ''' <param name="semanasMes">Viernes/sábados del mes de la baja (nóminas semanales).</param>
    Public Shared Function Calcular(periodo As String, tramos As IList(Of TramoIR), brutoMensual As Double, vacaciones As Double, semanasMes As Integer) As Double

        If tramos Is Nothing OrElse tramos.Count < 2 Then Return 0

        Dim gravables As List(Of TramoIR) = tramos.Skip(1).ToList()
        Dim minimo As Double = (gravables(0).Desde - 1) / 12
        Dim anual As Double = brutoMensual * 12

        If brutoMensual < minimo Then Return 0

        Dim tramo As TramoIR = gravables.FirstOrDefault(Function(t) t.Contiene(anual))
        If tramo Is Nothing Then Return 0

        Dim impuestoAnual As Double = tramo.Impuesto(anual)

        Select Case periodo
            Case "Semanal Viernes", "Semanal Sabado"
                Return If(semanasMes > 0, R(impuestoAnual / semanasMes / 12), 0)
            Case "Catorcenal los Viernes", "Catorcenal los Sabados", "Mensual"
                Return R(impuestoAnual / 12)
            Case "Quincenal"
                If vacaciones <> 0 Then
                    Dim conVacaciones As TramoIR = gravables.FirstOrDefault(Function(t) t.Contiene(anual + vacaciones))
                    Dim impuestoConVacaciones As Double = If(conVacaciones Is Nothing, 0, conVacaciones.Impuesto(anual + vacaciones))
                    Return impuestoConVacaciones - impuestoAnual
                End If
                ' La V.5.0 devolvía aquí el impuesto anual completo; se toma la parte del mes.
                Return R(impuestoAnual / 12)
            Case "Trimestral"
                Return R(impuestoAnual / 4)
            Case "Semestral"
                Return R(impuestoAnual / 2)
        End Select

        Return 0

    End Function

    ''' <summary>Cantidad de viernes (Semanal Viernes) o sábados (resto) del mes de la fecha.</summary>
    Public Shared Function SemanasDelMes(fecha As Date, periodo As String) As Integer
        Dim dia As DayOfWeek = If(periodo = "Semanal Viernes", DayOfWeek.Friday, DayOfWeek.Saturday)
        Return Enumerable.Range(1, Date.DaysInMonth(fecha.Year, fecha.Month)).Count(Function(d) New Date(fecha.Year, fecha.Month, d).DayOfWeek = dia)
    End Function

    Private Shared Function R(valor As Double) As Double
        Return Math.Round(valor, 2, MidpointRounding.AwayFromZero)
    End Function

End Class
