Imports System.Data
Imports System.Data.OleDb
Imports System.IO

''' <summary>Lee la primera hoja de un libro de Excel (.xls o .xlsx) como DataTable; la primera fila son los encabezados.</summary>
Public Class LectorExcel

    Public Shared Function Leer(ruta As String) As DataTable

        If Not File.Exists(ruta) Then
            Throw New FileNotFoundException("No se encontró el archivo " & ruta)
        End If

        Dim esXls As Boolean = String.Equals(Path.GetExtension(ruta), ".xls", StringComparison.OrdinalIgnoreCase)

        ' IMEX=1 lee las columnas mezcladas como texto; los códigos de empleado pueden traer ceros a la izquierda.
        Dim cadena As String =
            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & ruta & ";Extended Properties=""" &
            If(esXls, "Excel 8.0", "Excel 12.0 Xml") & ";HDR=YES;IMEX=1"""

        Dim dt As New DataTable

        Try

            Using cn As New OleDbConnection(cadena)

                cn.Open()

                Dim hojas As DataTable = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)
                Dim hoja As String = Nothing

                For Each fila As DataRow In hojas.Rows
                    Dim nombre As String = Convert.ToString(fila("TABLE_NAME"))
                    If nombre.EndsWith("$") OrElse nombre.EndsWith("$'") Then
                        hoja = nombre
                        Exit For
                    End If
                Next

                If hoja Is Nothing Then
                    Throw New InvalidOperationException("El libro no tiene hojas con datos.")
                End If

                Using da As New OleDbDataAdapter("SELECT * FROM [" & hoja.Trim("'"c) & "]", cn)
                    da.Fill(dt)
                End Using

            End Using

        Catch ex As InvalidOperationException When ex.Message.Contains("ACE.OLEDB")
            Throw New InvalidOperationException(
                "No se puede leer Excel: instale el componente 'Microsoft Access Database Engine' (ACE OLEDB 12) con la misma arquitectura (32/64 bits) que el sistema.", ex)
        End Try

        Return dt

    End Function

End Class
