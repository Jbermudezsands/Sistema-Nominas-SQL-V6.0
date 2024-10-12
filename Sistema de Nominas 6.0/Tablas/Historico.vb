Public Class Historico
    '////////////////HISTORICO EMPLEADOS //////////////////////////////
    Private Id As Integer
    Private FechaBaja As Date
    Private MotivoBaja As String
    Private MotivoAumento As String
    Private FechaInicialSusp As Date
    Private FechaFinalSusp As Date
    Private MotivoSuspencion As String
    Private FechaNacimiento As Date
    Private FechaContrato As Date
    Private FechaContratoVac As Date
    Private CargoInicial As String
    Private CargoActual As String
    Private CargoAnterior As String
    Private SueldoInicial As Double
    Private SueldoAnterior As Double
    Private SueldoActual As Double
    Private CuentaDebito As String
    Private CuentaCredito As String
    Private CuentaPrestamo As String
    Private CuentaOtrosIngresos As String
    Private CuentaINSS As String
    Private CuentaIR As String
    Private CuentaSueldos As String
    Private ProvAguinaldo As String
    Private ProvVacaciones As String
    Private INSSPatronal As String
    Private INATEC As String
    Private AguinaldoxPagar As String
    Private VacacionesxPagar As String
    Private INSSxPagar As String
    Private INATECxPagar As String
    Private IRxPagar As String
    Private PrestamoxPagar As String
    Private NominaxPagar As String
    Private CuentaHorasExtra As String
    Private INSSPatronalPagar As String
    Private CuentaSubsidio As String

    Public Property Empleado_Id As Integer
        Get
            Return Id
        End Get
        Set(value As Integer)
            Id = value
        End Set
    End Property

    Public Property Fecha_Baja As Date
        Get
            Return FechaBaja
        End Get
        Set(value As Date)
            FechaBaja = value
        End Set
    End Property

    Public Property Motivo_Baja As String
        Get
            Return MotivoBaja
        End Get
        Set(value As String)
            MotivoBaja = value
        End Set
    End Property

    Public Property Motivo_Aumento As String
        Get
            Return MotivoAumento
        End Get
        Set(value As String)
            MotivoAumento = value
        End Set
    End Property

    Public Property FechaInicial_Susp As Date
        Get
            Return FechaInicialSusp
        End Get
        Set(value As Date)
            FechaInicialSusp = value
        End Set
    End Property

    Public Property FechaFinal_Susp As Date
        Get
            Return FechaFinalSusp
        End Get
        Set(value As Date)
            FechaFinalSusp = value
        End Set
    End Property

    Public Property Motivo_Suspencion As String
        Get
            Return MotivoSuspencion
        End Get
        Set(value As String)
            MotivoSuspencion = value
        End Set
    End Property

    Public Property Fecha_Nacimiento As Date
        Get
            Return FechaNacimiento
        End Get
        Set(value As Date)
            FechaNacimiento = value
        End Set
    End Property

    Public Property Fecha_Contrato As Date
        Get
            Return FechaContrato
        End Get
        Set(value As Date)
            FechaContrato = value
        End Set
    End Property

    Public Property Fecha_ContratoVac As Date
        Get
            Return FechaContratoVac
        End Get
        Set(value As Date)
            FechaContratoVac = value
        End Set
    End Property

    Public Property Cargo_Inicial As String
        Get
            Return CargoInicial
        End Get
        Set(value As String)
            CargoInicial = value
        End Set
    End Property

    Public Property Cargo_Actual As String
        Get
            Return CargoActual
        End Get
        Set(value As String)
            CargoActual = value
        End Set
    End Property

    Public Property Cargo_Anterior As String
        Get
            Return CargoAnterior
        End Get
        Set(value As String)
            CargoAnterior = value
        End Set
    End Property

    Public Property Sueldo_Inicial As Double
        Get
            Return SueldoInicial
        End Get
        Set(value As Double)
            SueldoInicial = value
        End Set
    End Property

    Public Property Sueldo_Anterior As Double
        Get
            Return SueldoAnterior
        End Get
        Set(value As Double)
            SueldoAnterior = value
        End Set
    End Property

    Public Property Sueldo_Actual As Double
        Get
            Return SueldoActual
        End Get
        Set(value As Double)
            SueldoActual = value
        End Set
    End Property

    Public Property Cuenta_Debito As String
        Get
            Return CuentaDebito
        End Get
        Set(value As String)
            CuentaDebito = value
        End Set
    End Property

    Public Property Cuenta_Credito As String
        Get
            Return CuentaCredito
        End Get
        Set(value As String)
            CuentaCredito = value
        End Set
    End Property

    Public Property Cuenta_Prestamo As String
        Get
            Return CuentaPrestamo
        End Get
        Set(value As String)
            CuentaPrestamo = value
        End Set
    End Property

    Public Property CuentaOtros_Ingresos As String
        Get
            Return CuentaOtrosIngresos
        End Get
        Set(value As String)
            CuentaOtrosIngresos = value
        End Set
    End Property

    Public Property Cuenta_INSS As String
        Get
            Return CuentaINSS
        End Get
        Set(value As String)
            CuentaINSS = value
        End Set
    End Property

    Public Property Cuenta_IR As String
        Get
            Return CuentaIR
        End Get
        Set(value As String)
            CuentaIR = value
        End Set
    End Property

    Public Property Cuenta_Sueldos As String
        Get
            Return CuentaSueldos
        End Get
        Set(value As String)
            CuentaSueldos = value
        End Set
    End Property

    Public Property Prov_Aguinaldo As String
        Get
            Return ProvAguinaldo
        End Get
        Set(value As String)
            ProvAguinaldo = value
        End Set
    End Property

    Public Property Prov_Vacaciones As String
        Get
            Return ProvVacaciones
        End Get
        Set(value As String)
            ProvVacaciones = value
        End Set
    End Property

    Public Property INSS_Patronal As String
        Get
            Return INSSPatronal
        End Get
        Set(value As String)
            INSSPatronal = value
        End Set
    End Property

    Public Property Empleado_INATEC As String
        Get
            Return INATEC
        End Get
        Set(value As String)
            INATEC = value
        End Set
    End Property

    Public Property Aguinaldo_Pagar As String
        Get
            Return AguinaldoxPagar
        End Get
        Set(value As String)
            AguinaldoxPagar = value
        End Set
    End Property

    Public Property Vacaciones_Pagar As String
        Get
            Return VacacionesxPagar
        End Get
        Set(value As String)
            VacacionesxPagar = value
        End Set
    End Property

    Public Property INSS_Pagar As String
        Get
            Return INSSxPagar
        End Get
        Set(value As String)
            INSSxPagar = value
        End Set
    End Property

    Public Property INATEC_Pagar As String
        Get
            Return INATECxPagar
        End Get
        Set(value As String)
            INATECxPagar = value
        End Set
    End Property

    Public Property IR_Pagar As String
        Get
            Return IRxPagar
        End Get
        Set(value As String)
            IRxPagar = value
        End Set
    End Property

    Public Property Prestamo_Pagar As String
        Get
            Return PrestamoxPagar
        End Get
        Set(value As String)
            PrestamoxPagar = value
        End Set
    End Property

    Public Property Nomina_Pagar As String
        Get
            Return NominaxPagar
        End Get
        Set(value As String)
            NominaxPagar = value
        End Set
    End Property

    Public Property CuentaHoras_Extra As String
        Get
            Return CuentaHorasExtra
        End Get
        Set(value As String)
            CuentaHorasExtra = value
        End Set
    End Property

    Public Property INSSPatronal_Pagar As String
        Get
            Return INSSPatronalPagar
        End Get
        Set(value As String)
            INSSPatronalPagar = value
        End Set
    End Property

    Public Property Cuenta_Subsidio As String
        Get
            Return CuentaSubsidio
        End Get
        Set(value As String)
            CuentaSubsidio = value
        End Set
    End Property
End Class
