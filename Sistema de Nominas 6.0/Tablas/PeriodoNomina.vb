''' <summary>Un período del calendario de una nómina (filas de Fecha_Planilla y PeriodoFiscal).</summary>
Public Class PeriodoNomina

    Public Property Periodo As Integer
    Public Property Anio As Integer

    ''' <summary>Mes de dos dígitos ("01" a "12").</summary>
    Public Property Mes As String = ""
    Public Property Inicio As Date
    Public Property Final As Date
    Public Property Actual As Boolean
    Public Property Calculada As Boolean

    ''' <summary>Número de la nómina generada para el período; 0 = sin nómina.</summary>
    Public Property Num_Nomina As Integer

    ''' <summary>Un período con nómina calculada ya no debe cambiar de fechas.</summary>
    Public ReadOnly Property TieneNomina As Boolean
        Get
            Return Calculada OrElse Num_Nomina > 0
        End Get
    End Property

End Class
