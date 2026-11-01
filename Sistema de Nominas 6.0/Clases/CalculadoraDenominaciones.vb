''' <summary>
''' Desglose de un monto en billetes y monedas (V.5.0 FrmMonedas13vo.CmdOtorga_Click): se reparte de mayor a menor
''' denominación y los centavos se reparten aparte. Sin acceso a datos; trabaja en centavos enteros para no
''' arrastrar errores de punto flotante.
''' </summary>
Public Class CalculadoraDenominaciones

    ''' <summary>Billetes (1000 a 1) y monedas de centavo (0.50 a 0.01), en el orden en que se reparte.</summary>
    Public Shared ReadOnly Valores As Decimal() = {1000D, 500D, 200D, 100D, 50D, 20D, 10D, 5D, 1D, 0.5D, 0.25D, 0.1D, 0.05D, 0.01D}

    ''' <summary>Cantidad de cada denominación de <see cref="Valores"/> necesaria para pagar <paramref name="monto"/>.</summary>
    Public Shared Function Desglosar(monto As Double) As Integer()

        Dim cantidades(Valores.Length - 1) As Integer
        Dim centavos As Long = CLng(Math.Round(monto * 100, MidpointRounding.AwayFromZero))

        If centavos <= 0 Then Return cantidades

        For i As Integer = 0 To Valores.Length - 1
            Dim valor As Long = CLng(Valores(i) * 100D)
            cantidades(i) = CInt(centavos \ valor)
            centavos -= cantidades(i) * valor
        Next

        Return cantidades

    End Function

    ''' <summary>Suma el desglose de varios montos (el total de billetes y monedas que hay que retirar del banco).</summary>
    Public Shared Function Acumular(montos As IEnumerable(Of Double)) As Integer()

        Dim total(Valores.Length - 1) As Integer

        For Each monto As Double In montos
            Dim parcial As Integer() = Desglosar(monto)
            For i As Integer = 0 To total.Length - 1
                total(i) += parcial(i)
            Next
        Next

        Return total

    End Function

    ''' <summary>Importe que representa <paramref name="cantidad"/> unidades de la denominación en la posición <paramref name="indice"/>.</summary>
    Public Shared Function Importe(indice As Integer, cantidad As Integer) As Double
        Return CDbl(Valores(indice) * cantidad)
    End Function

    ''' <summary>Texto de la denominación como lo imprimía la V.5.0 ("1000 *", "0.50 *").</summary>
    Public Shared Function Etiqueta(indice As Integer) As String
        Dim v As Decimal = Valores(indice)
        Return If(v >= 1D, v.ToString("#,##0"), v.ToString("0.00")) & " *"
    End Function

End Class
