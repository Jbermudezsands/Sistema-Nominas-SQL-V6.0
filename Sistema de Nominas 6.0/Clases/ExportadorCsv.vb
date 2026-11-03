Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Text

''' <summary>Escribe una tabla como archivo .csv (separador coma, números con punto y dos decimales, UTF-8).</summary>
Public Class ExportadorCsv

    ''' <param name="columnas">Pares (encabezado del archivo, campo de la tabla) en el orden de salida.</param>
    Public Shared Sub Escribir(tabla As DataTable, columnas As IList(Of KeyValuePair(Of String, String)), ruta As String)

        Using w As New StreamWriter(ruta, False, New UTF8Encoding(True))

            w.WriteLine(String.Join(",", columnas.Select(Function(c) c.Key)))

            For Each fila As DataRow In tabla.Rows
                w.WriteLine(String.Join(",", columnas.Select(Function(c) Valor(fila(c.Value)))))
            Next

        End Using

    End Sub

    Private Shared Function Valor(o As Object) As String

        If o Is Nothing OrElse IsDBNull(o) Then Return ""

        If TypeOf o Is Double OrElse TypeOf o Is Decimal OrElse TypeOf o Is Single Then
            Return Convert.ToDecimal(o).ToString("0.00", CultureInfo.InvariantCulture)
        End If

        Dim s As String = Convert.ToString(o, CultureInfo.InvariantCulture)
        If s.IndexOfAny({","c, """"c, vbCr(0), vbLf(0)}) >= 0 Then s = """" & s.Replace("""", """""") & """"
        Return s

    End Function

End Class
