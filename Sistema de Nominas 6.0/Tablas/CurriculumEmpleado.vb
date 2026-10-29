''' <summary>Anotaciones / currículum de un empleado (tabla Curriculum).</summary>
Public Class CurriculumEmpleado

    Public Property Cod_Empleado As Decimal
    Public Property Fecha_Contratacion As DateTime = DateTime.Today
    Public Property Fecha_Finalizacion As DateTime = DateTime.Today
    Public Property Causa_Salida As String = ""
    Public Property Faltas As String = ""
    Public Property Justificacion_Faltas As String = ""
    Public Property Datos_Record As String = ""
    Public Property Idiomas As String = ""
    Public Property Telefono_Emergencia As String = ""
    Public Property Cursos As String = ""
    Public Property Razones_Contratacion As String = ""
    Public Property Trabajo_Anterior As String = ""
    Public Property Recomendaciones As String = ""

End Class
