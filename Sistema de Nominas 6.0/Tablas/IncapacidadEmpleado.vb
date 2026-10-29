''' <summary>Una incapacidad registrada a un empleado (tabla Incapacidad).</summary>
Public Class IncapacidadEmpleado

    ''' <summary>Llave interna del empleado (Empleado.CodEmpleado), no el código visible.</summary>
    Public Property Cod_Empleado As Decimal
    Public Property Cod_Incapacidad As String = ""
    Public Property Fecha As Date

End Class
