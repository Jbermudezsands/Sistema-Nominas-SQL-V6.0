''' <summary>Liquidación calculada de un empleado (tabla Bajas) más los datos que solo usa el reporte.</summary>
Public Class Liquidacion

    Public Property Id As Integer
    Public Property Cod_Empleado As Decimal
    Public Property Fecha_Baja As Date
    Public Property Annos_Trabajados As Double
    Public Property Meses_Trabajados As Double
    ''' <summary>Días de indemnización por antigüedad (Bajas.DiasTrabajados).</summary>
    Public Property Dias_Antiguedad As Double
    Public Property Monto_Nom_Propor As Double
    Public Property Monto_Vacaciones As Double
    Public Property Monto_Aguinaldo As Double
    Public Property Monto_Antiguedad As Double
    Public Property Horas_Extra As Double
    Public Property Monto_Horas_Extra As Double
    Public Property Otro As String = ""
    Public Property Monto_Otro As Double
    Public Property Fecha_Ini_Vacaciones As Date
    Public Property Fecha_Ini_Aguinaldo As Date
    Public Property Dias_Aguinaldo As Double
    Public Property Dias_Vacaciones As Double
    Public Property Dias_Menos_Vacaciones As Double
    Public Property Salario_Mensual As Double
    Public Property Fecha_Historial As Date
    Public Property Tipo_Baja As String = ""
    Public Property Motivo_Baja As String = ""
    Public Property Prestamo As Double
    Public Property Deducciones As Double
    Public Property Monto_Inss As Double
    Public Property Monto_Ir As Double
    Public Property Concepto_Otras_Deducciones As String = ""
    Public Property Monto_Otras_Deducciones As Double
    Public Property Dias_Laborados As Double

    Public Property Sueldo_Actual_Opt As Boolean
    Public Property Prestamo_Opt As Boolean
    Public Property Deducciones_Opt As Boolean
    Public Property Aguinaldo_Opt As Boolean
    Public Property Antiguedad_Opt As Boolean
    Public Property Viaticos_Opt As Boolean
    Public Property Vacaciones_Opt As Boolean
    Public Property Horas_Extra_Opt As Boolean
    Public Property Otros_Ingresos_Opt As Boolean
    Public Property Otros_Ingresos_Planilla_Opt As Boolean
    Public Property Pagar_Dias_Planilla_Opt As Boolean

    ' ---- Solo para el reporte (no se guardan) ----
    Public Property Dias_Vacaciones_Acumulados As Double
    Public Property Salario_Basico As Double
    Public Property Salario_Alto As Double
    Public Property Salario_Por_Dia As Double
    ''' <summary>Detalle de la base usada para el IR (V.5.0 FrmBajas.Referencia).</summary>
    Public Property Referencia As String = ""

    Public ReadOnly Property Total_Ingresos As Double
        Get
            Return Monto_Vacaciones + Monto_Nom_Propor + Monto_Aguinaldo + Monto_Antiguedad + Monto_Horas_Extra + Monto_Otro
        End Get
    End Property

    Public ReadOnly Property Total_Egresos As Double
        Get
            Return Monto_Inss + Monto_Ir + Prestamo + Deducciones + Monto_Otras_Deducciones
        End Get
    End Property

    Public ReadOnly Property Neto_Pagar As Double
        Get
            Return Total_Ingresos - Total_Egresos
        End Get
    End Property

End Class
