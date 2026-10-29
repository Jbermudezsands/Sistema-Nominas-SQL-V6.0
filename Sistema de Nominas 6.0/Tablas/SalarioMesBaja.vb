''' <summary>Ingresos de un mes de nómina del empleado (historial de salarios de la liquidación).</summary>
Public Class SalarioMesBaja

    Public Property Mes As Integer
    Public Property Anio As Integer
    Public Property Fecha_Inicio As Date
    Public Property Fecha_Fin As Date
    Public Property Salario_Basico As Double
    Public Property Destajo As Double
    Public Property Septimo As Double
    Public Property Otros As Double
    Public Property Incentivos As Double
    Public Property Reembolso As Double
    Public Property Bono_Produccion As Double
    Public Property Horas_Extras As Double
    Public Property Total_Ingresos As Double

    Public ReadOnly Property Periodo As String
        Get
            Return Mes.ToString("00") & "/" & Anio.ToString()
        End Get
    End Property

End Class
