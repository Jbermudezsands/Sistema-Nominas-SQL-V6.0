Imports System.Globalization
Imports System.IO

''' <summary>
''' Lee tasas de cambio de un archivo de texto separado por tabuladores: una línea de encabezado (FECHA, USXCS) y luego
''' "dd/MM/yyyy" + tab + tasa (con punto decimal), por ejemplo <c>01/01/2025 [tab] 36.6243</c>. Sin acceso a datos.
''' </summary>
Public Class ImportadorTasas

    Public Class Resultado
        Public ReadOnly Property Tasas As New List(Of TasaCambio)
        ''' <summary>Líneas que no se pudieron leer (número de línea y motivo), sin contar el encabezado ni las vacías.</summary>
        Public ReadOnly Property Rechazadas As New List(Of String)
    End Class

    Private Shared ReadOnly Formatos As String() = {"dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "dd-MM-yyyy"}

    Public Shared Function Leer(ruta As String) As Resultado

        Dim res As New Resultado
        Dim porFecha As New Dictionary(Of Date, Integer) ' fecha -> posición en la lista (si se repite gana la última)
        Dim lineas As String() = File.ReadAllLines(ruta, Text.Encoding.UTF8)

        For i As Integer = 0 To lineas.Length - 1

            Dim linea As String = lineas(i).Trim()
            If linea = "" Then Continue For

            Dim partes As String() = linea.Split({vbTab, ";"c}, StringSplitOptions.RemoveEmptyEntries)
            If partes.Length < 2 Then
                If i > 0 Then res.Rechazadas.Add($"Línea {i + 1}: se esperaba fecha y tasa separadas por tabulador.")
                Continue For
            End If

            Dim fecha As Date
            If Not Date.TryParseExact(partes(0).Trim(), Formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, fecha) Then
                ' La primera línea es el encabezado (FECHA / USXCS).
                If i > 0 Then res.Rechazadas.Add($"Línea {i + 1}: fecha no válida ""{partes(0).Trim()}"".")
                Continue For
            End If

            Dim monto As Decimal
            Dim texto As String = partes(1).Trim().Replace(",", ".")
            If Not Decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, monto) OrElse monto <= 0 Then
                res.Rechazadas.Add($"Línea {i + 1}: tasa no válida ""{partes(1).Trim()}"".")
                Continue For
            End If

            Dim t As New TasaCambio With {.Fecha = fecha.Date, .Monto = monto}

            If porFecha.ContainsKey(t.Fecha) Then
                res.Tasas(porFecha(t.Fecha)) = t
            Else
                porFecha(t.Fecha) = res.Tasas.Count
                res.Tasas.Add(t)
            End If

        Next

        Return res

    End Function

End Class
