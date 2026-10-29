''' <summary>
''' Libreta de un préstamo (V.5.0 CmdAfectuar_Click): capital en cuotas iguales,
''' interés sobre el saldo pendiente de cada cuota y CuotaIgual = (capital + interés total) / cuotas.
''' Sin acceso a BD.
''' </summary>
Public Class CalculadoraPrestamo

    ''' <param name="interesPorcentaje">Tasa por cuota en porcentaje (5 = 5%).</param>
    Public Shared Function Calcular(monto As Double, cuotas As Integer, interesPorcentaje As Double) As PrestamoEmpleado

        If cuotas <= 0 Then Throw New ArgumentException("Digite la cantidad de cuotas.")
        If monto <= 0 Then Throw New ArgumentException("Digite el monto del préstamo.")

        Dim p As New PrestamoEmpleado With {
            .Monto = monto,
            .Saldo = monto,
            .Cant_Cuotas = cuotas,
            .Interes = interesPorcentaje
        }

        Dim tasa As Double = interesPorcentaje / 100
        Dim capital As Double = monto / cuotas
        Dim saldo As Double = monto
        Dim totalInteres As Double = 0

        For i As Integer = 1 To cuotas
            Dim interes As Double = saldo * tasa
            totalInteres += interes

            p.Cuotas.Add(New CuotaPrestamo With {
                .Num_Cuota = i,
                .Monto = capital,
                .Interes = interes,
                .Saldo_Cuota = saldo
            })

            saldo -= capital
        Next

        p.Cuota_Igual = (monto + totalInteres) / cuotas

        Return p

    End Function

End Class
