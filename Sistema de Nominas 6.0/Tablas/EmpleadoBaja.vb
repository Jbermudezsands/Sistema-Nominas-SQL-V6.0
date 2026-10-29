''' <summary>Datos del empleado que usa la liquidación (Empleado + Cargo + Departamento + Historico + TipoNomina).</summary>
Public Class EmpleadoBaja

    Public Property Cod_Empleado As Decimal
    Public Property Cod_Empleado1 As String = ""
    Public Property Nombre1 As String = ""
    Public Property Nombre2 As String = ""
    Public Property Apellido1 As String = ""
    Public Property Apellido2 As String = ""
    Public Property Direccion As String = ""
    Public Property Cargo As String = ""
    Public Property Departamento As String = ""
    Public Property Sexo As String = ""
    Public Property Num_Cedula As String = ""
    Public Property Numero_Inss As String = ""
    Public Property Activo As Boolean
    Public Property Liquidado As Boolean

    ''' <summary>Sueldo del período tal como está en Empleado (en dólares si <see cref="Dolarizado"/>).</summary>
    Public Property Sueldo_Periodo As Double
    Public Property Dolarizado As Boolean
    ''' <summary>"S" salario fijo / "N" variable (NULL se toma como "S", igual que la V.5.0).</summary>
    Public Property Salario_Fijo As String = "S"
    ''' <summary>Empleado.SueldoActualBasico; Nothing cuando la columna es NULL (la V.5.0 distinguía NULL de False).</summary>
    Public Property Sueldo_Actual_Basico As Boolean?
    ''' <summary>Historico.SueldoActual (mensual, en córdobas).</summary>
    Public Property Sueldo_Actual As Double
    Public Property Fecha_Contrato As Date?

    Public Property Cod_Tipo_Nomina As String = ""
    Public Property Periodo As String = ""
    ''' <summary>TipoNomina.TasaInss en fracción (7 % = 0.07).</summary>
    Public Property Tasa_Inss As Double
    Public Property Ir_Ultima_Semana As Boolean

    Public ReadOnly Property Nombre_Completo As String
        Get
            Return String.Join(" ", {Nombre1, Nombre2, Apellido1, Apellido2}.Where(Function(s) Not String.IsNullOrWhiteSpace(s)).Select(Function(s) s.Trim()))
        End Get
    End Property

End Class
