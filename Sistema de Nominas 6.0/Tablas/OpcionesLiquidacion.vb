''' <summary>Lo que el usuario fija en la pantalla de bajas antes de calcular la liquidación (V.5.0 FrmBajas).</summary>
Public Class OpcionesLiquidacion

    Public Property Fecha_Egreso As Date
    Public Property Fecha_Ini_Vacaciones As Date
    Public Property Fecha_Ini_Aguinaldo As Date

    ''' <summary>Salario básico de referencia (la V.5.0 lo llenaba con DatosEmpresa.SalarioMinimo).</summary>
    Public Property Salario_Basico As Double
    Public Property Salario_Promedio As Double
    Public Property Salario_Alto As Double

    ''' <summary>Días a pagar desde la última nómina (TxtDias).</summary>
    Public Property Dias_Planilla As Double
    ''' <summary>Días trabajados para antigüedad (TxtDiasTrabajados).</summary>
    Public Property Dias_Trabajados As Double
    Public Property Annos As Double
    Public Property Meses_Texto As String = ""

    ''' <summary>Días de vacaciones digitados (TxtDiasVaca); Nothing = se calculan.</summary>
    Public Property Dias_Vacaciones_Manual As Double?
    Public Property Dias_Descuento_Vacaciones As Double

    Public Property Pagar_Dias_Planilla As Boolean = True
    Public Property Antiguedad_Base As Boolean
    Public Property Sueldo_Actual_Basico As Boolean
    Public Property Incentivos As Boolean = True
    Public Property Salario_Promedio_Real As Boolean

    Public Property Aguinaldo As Boolean = True
    Public Property Vacaciones As Boolean = True
    Public Property Antiguedad As Boolean = True
    Public Property Horas_Extra As Boolean
    Public Property Viaticos As Boolean
    Public Property Otros_Ingresos_Planilla As Boolean
    Public Property Otros_Ingresos As Boolean
    Public Property Concepto_Otros_Ingresos As String = ""
    Public Property Monto_Otros_Ingresos As Double

    Public Property Ir As Boolean = True
    Public Property Prestamos As Boolean
    Public Property Deducciones As Boolean
    Public Property Otras_Deducciones As Boolean
    Public Property Concepto_Otras_Deducciones As String = ""
    Public Property Monto_Otras_Deducciones As Double

    ''' <summary>"Despido", "Renuncia" o "Fin" (Bajas.TipoBaja es nvarchar(10)).</summary>
    Public Property Tipo_Baja As String = "Despido"
    Public Property Motivo As String = ""

End Class
