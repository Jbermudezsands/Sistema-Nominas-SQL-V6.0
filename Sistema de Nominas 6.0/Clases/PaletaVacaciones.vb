''' <summary>
''' Color y orden de cada tipo de día del calendario de vacaciones. Una sola fuente para la pantalla y el reporte
''' (la V.5.0 leía el color de cada fila de DescuentoDiasVacaciones, y algunos tipos venían sin color).
''' </summary>
Public NotInheritable Class PaletaVacaciones

    Private Shared ReadOnly Tipos As New List(Of Tuple(Of String, Color)) From {
        Tuple.Create("Vacaciones", Color.FromArgb(192, 192, 255)),
        Tuple.Create("Vacaciones Pagadas", Color.FromArgb(0, 192, 0)),
        Tuple.Create("Vacaciones Programadas", Color.FromArgb(255, 255, 128)),
        Tuple.Create("Permiso Programado", Color.FromArgb(0, 255, 255)),
        Tuple.Create("Ausente", Color.FromArgb(255, 128, 255)),
        Tuple.Create("AusenteBasico", Color.FromArgb(200, 200, 200)),
        Tuple.Create("Feriado", Color.FromArgb(251, 136, 136)),
        Tuple.Create("Subsidio", Color.FromArgb(255, 192, 128)),
        Tuple.Create("Suspension", Color.FromArgb(170, 170, 170))}

    Private Sub New()
    End Sub

    ''' <summary>Tipos conocidos en el orden en que se muestran.</summary>
    Public Shared ReadOnly Property TiposConocidos As IEnumerable(Of String)
        Get
            Return Tipos.Select(Function(t) t.Item1)
        End Get
    End Property

    Public Shared Function Orden(tipo As String) As Integer
        Dim i As Integer = Tipos.FindIndex(Function(t) t.Item1 = tipo)
        Return If(i < 0, Tipos.Count, i)
    End Function

    ''' <summary>Color del tipo; un tipo desconocido sale en gris claro.</summary>
    Public Shared Function ColorDe(tipo As String) As Color
        Dim t As Tuple(Of String, Color) = Tipos.FirstOrDefault(Function(x) x.Item1 = tipo)
        Return If(t Is Nothing, Color.Gainsboro, t.Item2)
    End Function

    ''' <summary>Texto negro o blanco, según cuál se lee mejor sobre <paramref name="fondo"/>.</summary>
    Public Shared Function ColorTexto(fondo As Color) As Color
        Dim luz As Double = 0.299 * fondo.R + 0.587 * fondo.G + 0.114 * fondo.B
        Return If(luz > 140, Color.Black, Color.White)
    End Function

End Class
