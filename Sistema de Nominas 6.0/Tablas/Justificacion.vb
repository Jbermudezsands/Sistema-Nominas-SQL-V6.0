''' <summary>Justificación (permiso, subsidio, etc.) de un empleado (tabla UserLeave).</summary>
Public Class Justificacion

    Public Property Id As Integer
    Public Property Cod_Empleado1 As String = ""
    Public Property Cod_Tipo As Integer
    Public Property Inicio As DateTime = DateTime.Now
    Public Property Fin As DateTime = DateTime.Now
    Public Property Motivo As String = ""

End Class
