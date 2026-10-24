Public Class Empleados

    Inherits Historico

    Private CodEmpleado As Double
    Private CodEmpleado1 As String
    Private Nombre1 As String
    Private Nombre2 As String
    Private Apellido1 As String
    Private Apellido2 As String
    Private NumHijos As Double
    Private Direccion As String
    Private Nacionalidad As String
    Private CodigoPostal As String
    Private Sexo As String
    Private CodInss As String
    Private CodIr As String
    Private NumCedula As String
    Private Sindicalista As String
    Private CodDepartamento As String
    Private CodGrupo As String
    Private CodCargo As String
    Private NumeroInss As String
    Private NumeroRuc As String
    Private CodTipoNomina As String
    Private DiasDescuento As Double
    Private SalPorcentaje As Double
    Private CantPts As Double
    Private SueldoPeriodo As Double
    Private TarifaHoraria As Double
    Private PorcentajeComision As Double
    Private OtrosIngresos As Double
    Private DescripOtrIngre As String
    Private ExentoInss As Integer
    Private ExentoIr As Integer
    Private PagoInssPatronal As Integer
    Private SalarioMinimo As Integer
    Private Observaciones As String
    Private Activo As Integer
    Private Foto As String
    Private Liquidado As Integer
    Private Ausente As Integer
    Private SalarioFijo As String
    Private SumarSubsidio As Integer
    Private PorcientoIncentivo As Double
    Private Gravidez As Integer
    Private VacacionesBasico As Integer
    Private Dolarizado As Integer
    Private FechaAntiguedad As String
    Private Antiguedad As Integer
    Private CuentaBanco As String
    Private SueldoActualBasico As Integer
    Private HorasTurno As Integer
    Private DiasBasico As Double
    Private AumentoBasico As Double
    Private EmpleadoConstruccion As Integer
    Private MontoPension As Double
    Private PorcentajePension As Double
    Private ViaticoxDia As Double
    Private DeducirPorciento As Integer
    Private Reembolso As Double
    Private Telefono As String
    Private MontoViatico As Double
    Private CorreoElectronico As String
    Private NumeroCelular As String
    Private CelularEmergencia As String
    Private Profesion As String
    Private EstadoCivil As String
    Private JefeInmediato As String
    Private Incentivo As String
    Private CuentaBancoLAFISE As String
    Private CuentaBancoBDF As String
    Private CuentaBancoBANPRO As String
    Private MontoEmbargo As Double
    Private PorcentajeEmbargo As Double
    Private PagoCheque As Boolean



    Public Property Cod_Empleado As Double
        Get
            Return CodEmpleado
        End Get
        Set(value As Double)
            CodEmpleado = value
        End Set
    End Property

    Public Property Cod_Empleado1 As String
        Get
            Return CodEmpleado1
        End Get
        Set(value As String)
            CodEmpleado1 = value
        End Set
    End Property

    Public Property Primer_Nombre As String
        Get
            Return Nombre1
        End Get
        Set(value As String)
            Nombre1 = value
        End Set
    End Property

    Public Property Segundo_Nombre As String
        Get
            Return Nombre2
        End Get
        Set(value As String)
            Nombre2 = value
        End Set
    End Property

    Public Property Primer_Apellido As String
        Get
            Return Apellido1
        End Get
        Set(value As String)
            Apellido1 = value
        End Set
    End Property

    Public Property Segundo_Apellido As String
        Get
            Return Apellido2
        End Get
        Set(value As String)
            Apellido2 = value
        End Set
    End Property

    Public Property Num_Hijos As Double
        Get
            Return NumHijos
        End Get
        Set(value As Double)
            NumHijos = value
        End Set
    End Property

    Public Property Empleado_Direccion As String
        Get
            Return Direccion
        End Get
        Set(value As String)
            Direccion = value
        End Set
    End Property

    Public Property Empleado_Nacionalidad As String
        Get
            Return Nacionalidad
        End Get
        Set(value As String)
            Nacionalidad = value
        End Set
    End Property

    Public Property Codigo_Postal As String
        Get
            Return CodigoPostal
        End Get
        Set(value As String)
            CodigoPostal = value
        End Set
    End Property

    Public Property Empleado_Sexo As String
        Get
            Return Sexo
        End Get
        Set(value As String)
            Sexo = value
        End Set
    End Property

    Public Property Cod_Inss As String
        Get
            Return CodInss
        End Get
        Set(value As String)
            CodInss = value
        End Set
    End Property

    Public Property Cod_Ir As String
        Get
            Return CodIr
        End Get
        Set(value As String)
            CodIr = value
        End Set
    End Property

    Public Property Num_Cedula As String
        Get
            Return NumCedula
        End Get
        Set(value As String)
            NumCedula = value
        End Set
    End Property

    Public Property Empleado_Sindicalista As String
        Get
            Return Sindicalista
        End Get
        Set(value As String)
            Sindicalista = value
        End Set
    End Property

    Public Property Cod_Departamento As String
        Get
            Return CodDepartamento
        End Get
        Set(value As String)
            CodDepartamento = value
        End Set
    End Property

    Public Property Cod_Grupo As String
        Get
            Return CodGrupo
        End Get
        Set(value As String)
            CodGrupo = value
        End Set
    End Property

    Public Property Cod_Cargo As String
        Get
            Return CodCargo
        End Get
        Set(value As String)
            CodCargo = value
        End Set
    End Property

    Public Property Numero_Inss As String
        Get
            Return NumeroInss
        End Get
        Set(value As String)
            NumeroInss = value
        End Set
    End Property

    Public Property Numero_Ruc As String
        Get
            Return NumeroRuc
        End Get
        Set(value As String)
            NumeroRuc = value
        End Set
    End Property

    Public Property Cod_TipoNomina As String
        Get
            Return CodTipoNomina
        End Get
        Set(value As String)
            CodTipoNomina = value
        End Set
    End Property

    Public Property Dias_Descuento As Double
        Get
            Return DiasDescuento
        End Get
        Set(value As Double)
            DiasDescuento = value
        End Set
    End Property

    Public Property Sal_Porcentaje As Double
        Get
            Return SalPorcentaje
        End Get
        Set(value As Double)
            SalPorcentaje = value
        End Set
    End Property

    Public Property Cant_Pts As Double
        Get
            Return CantPts
        End Get
        Set(value As Double)
            CantPts = value
        End Set
    End Property

    Public Property Sueldo_Periodo As Double
        Get
            Return SueldoPeriodo
        End Get
        Set(value As Double)
            SueldoPeriodo = value
        End Set
    End Property

    Public Property Tarifa_Horaria As Double
        Get
            Return TarifaHoraria
        End Get
        Set(value As Double)
            TarifaHoraria = value
        End Set
    End Property

    Public Property Porcentaje_Comision As Double
        Get
            Return PorcentajeComision
        End Get
        Set(value As Double)
            PorcentajeComision = value
        End Set
    End Property

    Public Property Otros_Ingresos As Double
        Get
            Return OtrosIngresos
        End Get
        Set(value As Double)
            OtrosIngresos = value
        End Set
    End Property

    Public Property Descrip_OtrIngre As String
        Get
            Return DescripOtrIngre
        End Get
        Set(value As String)
            DescripOtrIngre = value
        End Set
    End Property

    Public Property Exento_Inss As Integer
        Get
            Return ExentoInss
        End Get
        Set(value As Integer)
            ExentoInss = value
        End Set
    End Property

    Public Property Exento_Ir As Integer
        Get
            Return ExentoIr
        End Get
        Set(value As Integer)
            ExentoIr = value
        End Set
    End Property

    Public Property PagoInss_Patronal As Integer
        Get
            Return PagoInssPatronal
        End Get
        Set(value As Integer)
            PagoInssPatronal = value
        End Set
    End Property

    Public Property Salario_Minimo As Integer
        Get
            Return SalarioMinimo
        End Get
        Set(value As Integer)
            SalarioMinimo = value
        End Set
    End Property

    Public Property Empleado_Observaciones As String
        Get
            Return Observaciones
        End Get
        Set(value As String)
            Observaciones = value
        End Set
    End Property

    Public Property Empleado_Activo As Integer
        Get
            Return Activo
        End Get
        Set(value As Integer)
            Activo = value
        End Set
    End Property

    Public Property Empleado_Liquidado As Integer
        Get
            Return Liquidado
        End Get
        Set(value As Integer)
            Liquidado = value
        End Set
    End Property

    Public Property Empleado_Ausente As Integer
        Get
            Return Ausente
        End Get
        Set(value As Integer)
            Ausente = value
        End Set
    End Property

    Public Property Salario_Fijo As String
        Get
            Return SalarioFijo
        End Get
        Set(value As String)
            SalarioFijo = value
        End Set
    End Property

    Public Property Sumar_Subsidio As Integer
        Get
            Return SumarSubsidio
        End Get
        Set(value As Integer)
            SumarSubsidio = value
        End Set
    End Property

    Public Property Porciento_Incentivo As Double
        Get
            Return PorcientoIncentivo
        End Get
        Set(value As Double)
            PorcientoIncentivo = value
        End Set
    End Property

    Public Property Empleado_Gravidez As Integer
        Get
            Return Gravidez
        End Get
        Set(value As Integer)
            Gravidez = value
        End Set
    End Property

    Public Property Vacaciones_Basico As Integer
        Get
            Return VacacionesBasico
        End Get
        Set(value As Integer)
            VacacionesBasico = value
        End Set
    End Property

    Public Property Empleado_Dolarizado As Integer
        Get
            Return Dolarizado
        End Get
        Set(value As Integer)
            Dolarizado = value
        End Set
    End Property

    Public Property Fecha_Antiguedad As String
        Get
            Return FechaAntiguedad
        End Get
        Set(value As String)
            FechaAntiguedad = value
        End Set
    End Property

    Public Property Empleado_Antiguedad As Integer
        Get
            Return Antiguedad
        End Get
        Set(value As Integer)
            Antiguedad = value
        End Set
    End Property

    Public Property Empleado_CuentaBanco As String
        Get
            Return CuentaBanco
        End Get
        Set(value As String)
            CuentaBanco = value
        End Set
    End Property

    Public Property SueldoActual_Basico As Integer
        Get
            Return SueldoActualBasico
        End Get
        Set(value As Integer)
            SueldoActualBasico = value
        End Set
    End Property

    Public Property Horas_Turno As Integer
        Get
            Return HorasTurno
        End Get
        Set(value As Integer)
            HorasTurno = value
        End Set
    End Property

    Public Property Dias_Basico As Double
        Get
            Return DiasBasico
        End Get
        Set(value As Double)
            DiasBasico = value
        End Set
    End Property

    Public Property Aumento_Basico As Double
        Get
            Return AumentoBasico
        End Get
        Set(value As Double)
            AumentoBasico = value
        End Set
    End Property

    Public Property Empleado_Construccion As Integer
        Get
            Return EmpleadoConstruccion
        End Get
        Set(value As Integer)
            EmpleadoConstruccion = value
        End Set
    End Property

    Public Property Monto_Pension As Double
        Get
            Return MontoPension
        End Get
        Set(value As Double)
            MontoPension = value
        End Set
    End Property

    Public Property Porcentaje_Pension As Double
        Get
            Return PorcentajePension
        End Get
        Set(value As Double)
            PorcentajePension = value
        End Set
    End Property

    Public Property Viatico_Dia As Double
        Get
            Return ViaticoxDia
        End Get
        Set(value As Double)
            ViaticoxDia = value
        End Set
    End Property

    Public Property Deducir_Porciento As Integer
        Get
            Return DeducirPorciento
        End Get
        Set(value As Integer)
            DeducirPorciento = value
        End Set
    End Property

    Public Property Empleado_Reembolso As Double
        Get
            Return Reembolso
        End Get
        Set(value As Double)
            Reembolso = value
        End Set
    End Property

    Public Property Empleado_Telefono As String
        Get
            Return Telefono
        End Get
        Set(value As String)
            Telefono = value
        End Set
    End Property

    Public Property Monto_Viatico As Double
        Get
            Return MontoViatico
        End Get
        Set(value As Double)
            MontoViatico = value
        End Set
    End Property

    Public Property Correo_Electronico As String
        Get
            Return CorreoElectronico
        End Get
        Set(value As String)
            CorreoElectronico = value
        End Set
    End Property

    Public Property Numero_Celular As String
        Get
            Return NumeroCelular
        End Get
        Set(value As String)
            NumeroCelular = value
        End Set
    End Property

    Public Property Celular_Emergencia As String
        Get
            Return CelularEmergencia
        End Get
        Set(value As String)
            CelularEmergencia = value
        End Set
    End Property

    Public Property Empleado_Profesion As String
        Get
            Return Profesion
        End Get
        Set(value As String)
            Profesion = value
        End Set
    End Property

    Public Property Estado_Civil As String
        Get
            Return EstadoCivil
        End Get
        Set(value As String)
            EstadoCivil = value
        End Set
    End Property

    Public Property Jefe_Inmediato As String
        Get
            Return JefeInmediato
        End Get
        Set(value As String)
            JefeInmediato = value
        End Set
    End Property

    Public Property Empleado_Incentivo As String
        Get
            Return Incentivo
        End Get
        Set(value As String)
            Incentivo = value
        End Set
    End Property

    Public Property CuentaBanco_LAFISE As String
        Get
            Return CuentaBancoLAFISE
        End Get
        Set(value As String)
            CuentaBancoLAFISE = value
        End Set
    End Property

    Public Property CuentaBanco_BDF As String
        Get
            Return CuentaBancoBDF
        End Get
        Set(value As String)
            CuentaBancoBDF = value
        End Set
    End Property

    Public Property CuentaBanco_BANPRO As String
        Get
            Return CuentaBancoBANPRO
        End Get
        Set(value As String)
            CuentaBancoBANPRO = value
        End Set
    End Property

    Public Property Empleado_Foto As String
        Get
            Return Foto
        End Get
        Set(value As String)
            Foto = value
        End Set
    End Property

    Public Property Monto_Embargo As Double
        Get
            Return MontoEmbargo
        End Get
        Set(value As Double)
            MontoEmbargo = value
        End Set
    End Property

    Public Property Porcentaje_Embargo As Double
        Get
            Return PorcentajeEmbargo
        End Get
        Set(value As Double)
            PorcentajeEmbargo = value
        End Set
    End Property

    Public Property Pago_Cheque As Boolean
        Get
            Return PagoCheque
        End Get
        Set(value As Boolean)
            PagoCheque = value
        End Set
    End Property
End Class
