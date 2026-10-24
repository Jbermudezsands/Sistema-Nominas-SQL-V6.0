''' <summary>Fila única de la tabla Controles (parámetros de cálculo del sistema).</summary>
Public Class Controles

    Private _diasMes As Double = 30
    Private _diasSemana As Double = 7
    Private _verificarTasa As Boolean
    Private _salarioPromedioReal As Boolean
    Private _antiguedadMenor As Boolean
    Private _calcularRedondeado As Boolean

    ''' <summary>25, 30 o 365/12.</summary>
    Public Property Dias_Mes As Double
        Get
            Return _diasMes
        End Get
        Set(value As Double)
            _diasMes = value
        End Set
    End Property

    ''' <summary>6 o 7.</summary>
    Public Property Dias_Semana As Double
        Get
            Return _diasSemana
        End Get
        Set(value As Double)
            _diasSemana = value
        End Set
    End Property

    Public Property Verificar_Tasa As Boolean
        Get
            Return _verificarTasa
        End Get
        Set(value As Boolean)
            _verificarTasa = value
        End Set
    End Property

    ''' <summary>True = salario promedio por diferencia de días; False = entre períodos.</summary>
    Public Property Salario_Promedio_Real As Boolean
        Get
            Return _salarioPromedioReal
        End Get
        Set(value As Boolean)
            _salarioPromedioReal = value
        End Set
    End Property

    Public Property Antiguedad_Menor As Boolean
        Get
            Return _antiguedadMenor
        End Get
        Set(value As Boolean)
            _antiguedadMenor = value
        End Set
    End Property

    Public Property Calcular_Redondeado As Boolean
        Get
            Return _calcularRedondeado
        End Get
        Set(value As Boolean)
            _calcularRedondeado = value
        End Set
    End Property
End Class
