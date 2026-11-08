''' <summary>Lo que aparece en el encabezado de los reportes de historial y detalle de salarios de una baja (V.5.0 ArepHistorialLiquida / ArepDetalleLiquida).</summary>
Public Class DatosHistorialBaja

    Public Property Codigo As String = ""
    ''' <summary>Nombre completo del empleado (siempre se imprime, junto al código).</summary>
    Public Property Nombre_Completo As String = ""
    Public Property Departamento As String = ""
    Public Property Cargo As String = ""
    Public Property Annos As String = ""
    Public Property Meses As String = ""
    Public Property Dias As String = ""
    Public Property Salario_Alto As Double
    Public Property Salario_Basico As Double
    Public Property Salario_Promedio As Double
    Public Property Tarifa As Double
    Public Property Fecha_Ini_Vacaciones As Date
    Public Property Fecha_Ini_Aguinaldo As Date

End Class
