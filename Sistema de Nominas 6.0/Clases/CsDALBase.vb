Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Base de los DAL de catálogos: toma la cadena de conexión elegida en runtime
''' y ofrece helpers parametrizados (nunca se concatenan valores del usuario).
''' </summary>
Public MustInherit Class CsDALBase

    Protected ReadOnly CadenaConexion As String

    Protected Sub New()
        CadenaConexion = New Conexion().ConexionSQL
    End Sub

    Protected Function ConsultarTabla(sql As String, ParamArray parametros() As SqlParameter) As DataTable

        Dim dt As New DataTable

        Using cn As New SqlConnection(CadenaConexion)
            Using cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddRange(parametros)
                Using da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
        End Using

        Return dt

    End Function

    Protected Function Escalar(sql As String, ParamArray parametros() As SqlParameter) As Object

        Using cn As New SqlConnection(CadenaConexion)
            Using cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddRange(parametros)
                cn.Open()
                Return cmd.ExecuteScalar()
            End Using
        End Using

    End Function

    Protected Function Ejecutar(sql As String, ParamArray parametros() As SqlParameter) As Integer

        Using cn As New SqlConnection(CadenaConexion)
            Using cmd As New SqlCommand(sql, cn)
                cmd.Parameters.AddRange(parametros)
                cn.Open()
                Return cmd.ExecuteNonQuery()
            End Using
        End Using

    End Function

    ''' <summary>Máximo código numérico + 1, rellenado con ceros a <paramref name="ancho"/>.
    ''' Tabla y columna son constantes del código, no entrada del usuario.</summary>
    Protected Function SiguienteCodigo(tabla As String, columna As String, ancho As Integer) As String

        Dim sql As String =
            "SELECT MAX(CASE WHEN " & columna & " NOT LIKE '%[^0-9]%' AND LEN(" & columna & ") <= 9 " &
            "THEN CAST(" & columna & " AS int) END) FROM " & tabla

        Dim r As Object = Escalar(sql)
        Dim n As Integer = If(IsDBNull(r) OrElse r Is Nothing, 0, Convert.ToInt32(r))

        Return (n + 1).ToString().PadLeft(ancho, "0"c)

    End Function

    Protected Shared Function ValorODbNull(valor As String) As Object

        If String.IsNullOrWhiteSpace(valor) Then Return DBNull.Value

        Return valor.Trim

    End Function

    ''' <summary>Traduce la violación de llave foránea (547) a un mensaje claro.</summary>
    Protected Shared Function EsEnUso(ex As SqlException) As Boolean
        Return ex.Number = 547
    End Function

End Class
