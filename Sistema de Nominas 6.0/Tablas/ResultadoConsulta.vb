Imports System.Data

''' <summary>Resultado de ejecutar una sentencia SQL libre (ver CsConsultaDAL).</summary>
Public Class ResultadoConsulta

    ''' <summary>True si la sentencia devolvió un conjunto de filas (SELECT, WITH, EXEC).</summary>
    Public Property EsConsulta As Boolean

    Public Property Tabla As DataTable

    ''' <summary>Filas afectadas por una sentencia que no es consulta (INSERT, UPDATE, DELETE...).</summary>
    Public Property FilasAfectadas As Integer

End Class
