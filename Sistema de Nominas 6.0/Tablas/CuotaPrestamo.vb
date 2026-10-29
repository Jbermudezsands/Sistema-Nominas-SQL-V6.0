''' <summary>Una cuota de la libreta de un préstamo (fila de MovPrestamo).</summary>
Public Class CuotaPrestamo

    Public Property Num_Cuota As Integer
    Public Property Monto As Double
    Public Property Interes As Double
    Public Property Saldo_Cuota As Double

End Class

''' <summary>Cabecera del préstamo (fila de Prestamo) más su libreta.</summary>
Public Class PrestamoEmpleado

    Public Property Num_Prestamo As Integer
    Public Property Cod_Empleado As Double
    Public Property Monto As Double
    Public Property Cant_Cuotas As Integer
    Public Property Interes As Double
    Public Property Saldo As Double
    Public Property Moneda As String = "CS"
    Public Property Cuotas_Iguales As Boolean = True
    Public Property Cuenta_Debito As String = ""
    Public Property Cuenta_Credito As String = ""
    Public Property Cuota_Igual As Double
    Public Property Cuotas As New List(Of CuotaPrestamo)

End Class
