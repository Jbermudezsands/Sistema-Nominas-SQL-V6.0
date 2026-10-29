''' <summary>Datos que se imprimen en el carnet de un empleado.</summary>
Public Class DatosCarnet

    Public Property Codigo As String = ""
    Public Property Nombres As String = ""
    Public Property Apellidos As String = ""
    Public Property Cargo As String = ""

    ''' <summary>Foto guardada en Empleado.Foto (texto); vacía si el empleado no tiene.</summary>
    Public Property Foto As String = ""

End Class
