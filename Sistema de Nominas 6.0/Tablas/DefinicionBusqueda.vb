''' <summary>
''' Qué consulta muestra <see cref="FrmConsulta"/>: título, SQL (siempre constante del código, sin parámetros del usuario),
''' columnas y la columna que identifica el registro elegido.
''' </summary>
Public Class DefinicionBusqueda

    Public Property Titulo As String
    Public Property Sql As String
    Public Property Clave As String
    Public Property Columnas As New List(Of ColumnaBusqueda)

    Public Sub New(titulo As String, sql As String, clave As String, ParamArray columnas() As ColumnaBusqueda)
        Me.Titulo = titulo
        Me.Sql = sql
        Me.Clave = clave
        Me.Columnas.AddRange(columnas)
    End Sub

End Class
