Public Class TipoNomina

    Public Property Cod_Tipo_Nomina As String = ""
    Public Property Descripcion_Nomina As String = ""

    ''' <summary>Semanal Viernes, Semanal Sabado, Catorcenal los Viernes, Catorcenal los Sabados,
    ''' Quincenal, Mensual, Trimestral o Semestral.</summary>
    Public Property Periodo As String = "Quincenal"

    ''' <summary>Salario Fijo, Salario Destajo, Salario Fijo y Comision,
    ''' Salario Destajo y Comision o Salario Fijo,Destajo y Comision.</summary>
    Public Property Tipo_Pago As String = "Salario Fijo"

    ''' <summary>"CS" córdobas, "US" dólares.</summary>
    Public Property Moneda As String = "CS"
    Public Property Mantenimiento_Valor As Boolean

    ''' <summary>True = INSS por porcentaje fijo; False = por tabla.</summary>
    Public Property Inss_Por_Porcentaje As Boolean
    Public Property Tasa_Inss As Decimal
    Public Property Tasa_Inss_Patronal As Decimal

    ''' <summary>True = IR por porcentaje fijo; False = por tabla.</summary>
    Public Property Ir_Por_Porcentaje As Boolean
    Public Property Tasa_Ir As Decimal

    Public Property Tarifa_Horaria As Double
    Public Property Horas As Double = 8
    Public Property Calcular_Hora_Trabajada As Boolean
    Public Property Ir_Ultima_Semana As Boolean
    Public Property Empleado_Construccion As Boolean
    Public Property Mostrar_Empleado_Cero As Boolean

End Class
