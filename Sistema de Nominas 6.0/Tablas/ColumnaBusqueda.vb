''' <summary>Una columna del grid de <see cref="FrmConsulta"/>.</summary>
Public Class ColumnaBusqueda

    Public Property Campo As String
    Public Property Titulo As String
    Public Property Ancho As Integer = 100
    Public Property Visible As Boolean = True

    ''' <summary>Formato numérico o de fecha (p. ej. "n2", "dd/MM/yyyy"); vacío = sin formato.</summary>
    Public Property Formato As String = ""

    Public Sub New(campo As String, titulo As String, ancho As Integer,
                   Optional visible As Boolean = True, Optional formato As String = "")
        Me.Campo = campo
        Me.Titulo = titulo
        Me.Ancho = ancho
        Me.Visible = visible
        Me.Formato = formato
    End Sub

End Class
