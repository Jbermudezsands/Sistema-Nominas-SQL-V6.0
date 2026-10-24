''' <summary>Fila única (Numero = 1) de la tabla DatosEmpresa.</summary>
Public Class DatosEmpresa

    Public Property Nombre_Empresa As String = ""
    Public Property Numero_Ruc As String = ""
    Public Property Direccion As String = ""
    Public Property Telefono As String = ""
    Public Property Fax As String = ""
    Public Property Email As String = ""
    Public Property Ruta_Logo As String = ""
    Public Property Ruta_Foto As String = ""
    Public Property Formato_Colilla As String = ""
    Public Property Formato_Nomina As String = ""
    Public Property Metodo_Vacaciones As String = "Vacaciones Mensuales"
    Public Property Tipo_Calculo_Ir As String = "Calcular IR x 12"
    Public Property Conexion_Sistema_Contable As String = ""
    Public Property Porciento_Subsidio As Double = 0
    Public Property Horas_Extra As String = "2"
    Public Property Calcular_7mo As Boolean
    Public Property Restar_3_Dias_Subsidio As Boolean

End Class
