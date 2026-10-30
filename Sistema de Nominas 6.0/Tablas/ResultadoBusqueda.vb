''' <summary>
''' La fila que el usuario eligió en <see cref="FrmConsulta"/>. Reemplaza las propiedades públicas del FrmConsulta de la V.5.0
''' (CodigoEmpleado1, NumeroSolicitud, FechaSolicitud, TipoSolicitud, ...): se leen por nombre de campo con <see cref="Texto"/>.
''' </summary>
Public Class ResultadoBusqueda

    Private ReadOnly _valores As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)

    ''' <summary>Valor de la columna clave de la consulta.</summary>
    Public ReadOnly Property Codigo As String

    Public Sub New(fila As DataRow, clave As String)
        For Each c As DataColumn In fila.Table.Columns
            _valores(c.ColumnName) = If(IsDBNull(fila(c)), Nothing, fila(c))
        Next
        Codigo = Texto(clave)
    End Sub

    Public Function Valor(campo As String) As Object
        Dim v As Object = Nothing
        _valores.TryGetValue(campo, v)
        Return v
    End Function

    Public Function Texto(campo As String) As String
        Dim v As Object = Valor(campo)
        Return If(v Is Nothing, "", Convert.ToString(v).Trim)
    End Function

    Public Function Fecha(campo As String) As Date?
        Dim v As Object = Valor(campo)
        If v Is Nothing Then Return Nothing
        Return Convert.ToDateTime(v)
    End Function

End Class
