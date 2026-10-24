Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Public Class CsEmpleadosDAL

    Private ReadOnly CadenaConexion As String

    Public Sub New()

        Dim cn As New Conexion

        CadenaConexion = cn.ConexionSQL

    End Sub

#Region "GUARDAR"

    Public Sub Guardar(emp As Empleados)

        Try

            If emp.Cod_Empleado > 0 Then
                Actualizar(emp)
            Else
                If Existe(emp.Cod_Empleado1) Then
                    Throw New Exception(
                        "Ya existe un empleado con el código " &
                        emp.Cod_Empleado1 & ".")
                End If
                Insertar(emp)
            End If

        Catch ex As Exception

            Throw New Exception(
                "Error al guardar empleado: " &
                ex.Message)

        End Try

    End Sub

#End Region

#Region "EXISTE"

    Public Function Existe(
        CodigoEmpleado As String) As Boolean

        Dim Sql As String =
"
SELECT COUNT(*)
FROM Empleado
WHERE CodEmpleado1 = @CodEmpleado1
"

        Using cn As New SqlConnection(CadenaConexion)

            Using cmd As New SqlCommand(Sql, cn)

                cmd.Parameters.AddWithValue(
                    "@CodEmpleado1",
                    CodigoEmpleado)

                cn.Open()

                Return Convert.ToInt32(
                    cmd.ExecuteScalar()) > 0

            End Using

        End Using

    End Function

#End Region

#Region "INSERTAR"

    '    Private Sub Insertar(emp As Empleados)

    '        Using cn As New SqlConnection(CadenaConexion)

    '            cn.Open()

    '            Dim trans As SqlTransaction =
    '                cn.BeginTransaction()

    '            Try

    '                Dim Sql As String =
    '"
    'INSERT INTO Empleado
    '(
    'CodEmpleado1,
    'Nombre1,
    'Nombre2,
    'Apellido1,
    'Apellido2,
    'Direccion,
    'Telefono,
    'Nacionalidad,
    'CodigoPostal,
    'Sexo,
    'NumeroRuc,
    'NumCedula,
    'NumeroInss,
    'NumHijos,
    'Sindicalista,
    'CodDepartamento,
    'CodCargo,
    'CuentaBanco,
    'Correo_Electronico,
    'NumeroCelular,
    'CodTipoNomina,
    'DiasDescuento,
    'SalPorcentaje,
    'SueldoPeriodo,
    'TarifaHoraria,
    'PorcentajeComision,
    'OtrosIngresos,
    'DescripOtrIngre,
    'ExentoInss,
    'ExentoIr,
    'PagoInssPatronal,
    'SalarioMinimo,
    'Activo,
    'Liquidado,
    'Ausente,
    'SalarioFijo,
    'SumarSubsidio,
    'PorcientoIncentivo,
    'Gravidez,
    'VacacionesBasico,
    'Dolarizado,
    'FechaAntiguedad,
    'Antiguedad,
    'SueldoActualBasico,
    'DiasBasico,
    'AumentoBasico,
    'Foto
    ')
    'VALUES
    '(
    '@CodEmpleado1,
    '@Nombre1,
    '@Nombre2,
    '@Apellido1,
    '@Apellido2,
    '@Direccion,
    '@Telefono,
    '@Nacionalidad,
    '@CodigoPostal,
    '@Sexo,
    '@NumeroRuc,
    '@NumCedula,
    '@NumeroInss,
    '@NumHijos,
    '@Sindicalista,
    '@CodDepartamento,
    '@CodCargo,
    '@CuentaBanco,
    '@Correo_Electronico,
    '@NumeroCelular,
    '@CodTipoNomina,
    '@DiasDescuento,
    '@SalPorcentaje,
    '@SueldoPeriodo,
    '@TarifaHoraria,
    '@PorcentajeComision,
    '@OtrosIngresos,
    '@DescripOtrIngre,
    '@ExentoInss,
    '@ExentoIr,
    '@PagoInssPatronal,
    '@SalarioMinimo,
    '@Activo,
    '@Liquidado,
    '@Ausente,
    '@SalarioFijo,
    '@SumarSubsidio,
    '@PorcientoIncentivo,
    '@Gravidez,
    '@VacacionesBasico,
    '@Dolarizado,
    '@FechaAntiguedad,
    '@Antiguedad,
    '@SueldoActualBasico,
    '@DiasBasico,
    '@AumentoBasico,
    '@Foto
    ')

    'SELECT SCOPE_IDENTITY()
    '"

    '                Using cmd As New SqlCommand(
    '                    Sql,
    '                    cn,
    '                    trans)

    '                    AgregarParametrosEmpleado(
    '                        cmd,
    '                        emp)

    '                    emp.Cod_Empleado =
    '                        Convert.ToInt32(
    '                            cmd.ExecuteScalar())

    '                End Using

    '                InsertarHistorico(
    '                    emp,
    '                    cn,
    '                    trans)

    '                trans.Commit()

    '            Catch ex As Exception

    '                trans.Rollback()

    '                Throw

    '            End Try

    '        End Using

    '    End Sub


    Private Sub Insertar(emp As Empleados)

        Using cn As New SqlConnection(CadenaConexion)

            cn.Open()

            Dim trans As SqlTransaction =
            cn.BeginTransaction()

            Try

                Dim Sql As String =
"
INSERT INTO Empleado
(
    CodEmpleado1,
    Nombre1,
    Nombre2,
    Apellido1,
    Apellido2,
    Direccion,
    Telefono,
    Nacionalidad,
    CodigoPostal,
    Sexo,
    NumeroRuc,
    NumCedula,
    NumeroInss,
    NumHijos,
    Sindicalista,
    CodDepartamento,
    CodCargo,
    CuentaBanco,
    Correo_Electronico,
    NumeroCelular,
    CodTipoNomina,
    DiasDescuento,
    SalPorcentaje,
    SueldoPeriodo,
    TarifaHoraria,
    PorcentajeComision,
    OtrosIngresos,
    DescripOtrIngre,
    ExentoInss,
    ExentoIr,
    PagoInssPatronal,
    SalarioMinimo,
    Activo,
    Liquidado,
    Ausente,
    SalarioFijo,
    SumarSubsidio,
    PorcientoIncentivo,
    Gravidez,
    VacacionesBasico,
    Dolarizado,
    FechaAntiguedad,
    Antiguedad,
    SueldoActualBasico,
    DiasBasico,
    AumentoBasico,
    Foto,
    ViaticoxDia,
    Reembolso,
    DeducirPorciento,
    HorasTurno,
    CuentaBancoBANPRO,
    CuentaBancoBDF,
    CuentaBancoLAFISE
)
VALUES
(
    @CodEmpleado1,
    @Nombre1,
    @Nombre2,
    @Apellido1,
    @Apellido2,
    @Direccion,
    @Telefono,
    @Nacionalidad,
    @CodigoPostal,
    @Sexo,
    @NumeroRuc,
    @NumCedula,
    @NumeroInss,
    @NumHijos,
    @Sindicalista,
    @CodDepartamento,
    @CodCargo,
    @CuentaBanco,
    @Correo_Electronico,
    @NumeroCelular,
    @CodTipoNomina,
    @DiasDescuento,
    @SalPorcentaje,
    @SueldoPeriodo,
    @TarifaHoraria,
    @PorcentajeComision,
    @OtrosIngresos,
    @DescripOtrIngre,
    @ExentoInss,
    @ExentoIr,
    @PagoInssPatronal,
    @SalarioMinimo,
    @Activo,
    @Liquidado,
    @Ausente,
    @SalarioFijo,
    @SumarSubsidio,
    @PorcientoIncentivo,
    @Gravidez,
    @VacacionesBasico,
    @Dolarizado,
    @FechaAntiguedad,
    @Antiguedad,
    @SueldoActualBasico,
    @DiasBasico,
    @AumentoBasico,
    @Foto,
    @ViaticoxDia,
    @Reembolso,
    @DeducirPorciento,
    @HorasTurno,
    @CuentaBancoBANPRO,
    @CuentaBancoBDF,
    @CuentaBancoLAFISE
)

SELECT CAST(SCOPE_IDENTITY() AS INT)
"

                Using cmd As New SqlCommand(
                Sql,
                cn,
                trans)

                    AgregarParametrosEmpleado(
                    cmd,
                    emp)

                    '========================================
                    ' OBTENER AUTONUMERICO
                    '========================================

                    emp.Cod_Empleado =
                    Convert.ToInt32(
                        cmd.ExecuteScalar())

                End Using

                '========================================
                ' VALIDAR AUTONUMERICO
                '========================================

                If emp.Cod_Empleado <= 0 Then

                    Throw New Exception(
                    "No se pudo obtener el código del empleado.")

                End If

                '========================================
                ' INSERTAR HISTORICO
                '========================================

                InsertarHistorico(
                emp,
                cn,
                trans)

                trans.Commit()

            Catch ex As Exception

                trans.Rollback()

                Throw

            End Try

        End Using

    End Sub
#End Region

#Region "ACTUALIZAR"

    Private Sub Actualizar(emp As Empleados)

        Using cn As New SqlConnection(CadenaConexion)

            cn.Open()

            Dim trans As SqlTransaction =
                cn.BeginTransaction()

            Try

                Dim Sql As String =
"
UPDATE Empleado SET

Nombre1 = @Nombre1,
Nombre2 = @Nombre2,
Apellido1 = @Apellido1,
Apellido2 = @Apellido2,
Direccion = @Direccion,
Telefono = @Telefono,
Nacionalidad = @Nacionalidad,
CodigoPostal = @CodigoPostal,
Sexo = @Sexo,
NumeroRuc = @NumeroRuc,
NumCedula = @NumCedula,
NumeroInss = @NumeroInss,
NumHijos = @NumHijos,
Sindicalista = @Sindicalista,
CodDepartamento = @CodDepartamento,
CodCargo = @CodCargo,
CuentaBanco = @CuentaBanco,
Correo_Electronico = @Correo_Electronico,
NumeroCelular = @NumeroCelular,
CodTipoNomina = @CodTipoNomina,
DiasDescuento = @DiasDescuento,
SalPorcentaje = @SalPorcentaje,
SueldoPeriodo = @SueldoPeriodo,
TarifaHoraria = @TarifaHoraria,
PorcentajeComision = @PorcentajeComision,
OtrosIngresos = @OtrosIngresos,
DescripOtrIngre = @DescripOtrIngre,
ExentoInss = @ExentoInss,
ExentoIr = @ExentoIr,
PagoInssPatronal = @PagoInssPatronal,
SalarioMinimo = @SalarioMinimo,
SalarioFijo = @SalarioFijo,
Dolarizado = @Dolarizado,
SueldoActualBasico = @SueldoActualBasico,
DiasBasico = @DiasBasico,
AumentoBasico = @AumentoBasico,
Foto = @Foto,
ViaticoxDia = @ViaticoxDia,
Reembolso = @Reembolso,
DeducirPorciento = @DeducirPorciento,
HorasTurno = @HorasTurno,
CuentaBancoBANPRO = @CuentaBancoBANPRO,
CuentaBancoBDF = @CuentaBancoBDF,
CuentaBancoLAFISE = @CuentaBancoLAFISE

WHERE CodEmpleado = @CodEmpleado
"

                Using cmd As New SqlCommand(
                    Sql,
                    cn,
                    trans)

                    AgregarParametrosEmpleado(
                        cmd,
                        emp)

                    cmd.Parameters.AddWithValue(
                        "@CodEmpleado",
                        emp.Cod_Empleado)

                    cmd.ExecuteNonQuery()

                End Using

                ActualizarHistorico(
                    emp,
                    cn,
                    trans)

                trans.Commit()

            Catch ex As Exception

                trans.Rollback()

                Throw

            End Try

        End Using

    End Sub

#End Region

#Region "BUSCAR"

    Public Function Buscar(
        CodigoEmpleado As String) As Empleados

        Dim emp As Empleados = Nothing

        Dim Sql As String =
"
SELECT
E.*,
E.CuentaBancoBANPRO AS CuentaBanco_BANPRO,
E.CuentaBancoBDF AS CuentaBanco_BDF,
E.CuentaBancoLAFISE AS CuentaBanco_LAFISE,
H.FechaBaja, H.MotivoBaja,
H.FechaAumento, H.MotivoAumento,
H.FechaInicialSusp, H.FechaFinalSusp, H.MotivoSuspencion,
H.FechaNacimiento, H.FechaContrato, H.FechaContratoVac,
H.CargoInicial, H.CargoActual, H.CargoAnterior,
H.SueldoInicial, H.SueldoAnterior, H.SueldoActual,
H.CuentaSueldos, H.ProvAguinaldo, H.ProvVacaciones,
H.INSSPatronal, H.INATEC, H.CuentaHorasExtra, H.CuentaOtrosIngresos,
H.AguinaldoxPagar, H.VacacionesxPagar, H.INSSxPagar, H.IRxPagar,
H.INSSPatronalPagar, H.NominaxPagar, H.INATECxPagar, H.PrestamoxPagar,
H.CuentaBanco AS CuentaBanco_BAC
FROM Empleado E
LEFT JOIN Historico H
ON E.CodEmpleado = H.CodEmpleado
WHERE E.CodEmpleado1 = @CodEmpleado1
"

        Using cn As New SqlConnection(CadenaConexion)

            Using cmd As New SqlCommand(Sql, cn)

                cmd.Parameters.AddWithValue(
                    "@CodEmpleado1",
                    CodigoEmpleado)

                cn.Open()

                Using dr As SqlDataReader =
                    cmd.ExecuteReader()

                    If dr.Read() Then

                        emp =
                            MapearEmpleado(dr)

                    End If

                End Using

            End Using

        End Using

        Return emp

    End Function

#End Region

#Region "ELIMINAR"

    ''' <summary>
    ''' No borra: marca el empleado como inactivo (Activo = 0).
    ''' </summary>
    Public Sub Eliminar(
        CodEmpleado As Integer)

        Dim Sql As String =
"
UPDATE Empleado
SET Activo = 0
WHERE CodEmpleado = @CodEmpleado
"

        Using cn As New SqlConnection(CadenaConexion)

            Using cmd As New SqlCommand(Sql, cn)

                cmd.Parameters.AddWithValue(
                    "@CodEmpleado",
                    CodEmpleado)

                cn.Open()

                cmd.ExecuteNonQuery()

            End Using

        End Using

    End Sub

#End Region

#Region "CATALOGOS"

    Private Function Consultar(Sql As String) As DataTable

        Dim dt As New DataTable

        Using cn As New SqlConnection(CadenaConexion)

            Using da As New SqlDataAdapter(Sql, cn)

                da.Fill(dt)

            End Using

        End Using

        Return dt

    End Function

    Public Function ListarDepartamentos() As DataTable

        Return Consultar(
            "SELECT CodDepartamento, Departamento " &
            "FROM Departamento ORDER BY Departamento")

    End Function

    Public Function ListarCargos() As DataTable

        Return Consultar(
            "SELECT CodCargo, Cargo " &
            "FROM Cargo ORDER BY Cargo")

    End Function

    Public Function ListarTiposNomina() As DataTable

        Return Consultar(
            "SELECT CodTipoNomina, Nomina, Periodo, TipoPago " &
            "FROM TipoNomina ORDER BY CodTipoNomina")

    End Function

    ''' <summary>
    ''' Empleados activos para el buscador (no exige cargo/departamento/histórico).
    ''' </summary>
    Public Function ListarParaCombo() As DataTable

        Return Consultar(
            "SELECT CodEmpleado1, " &
            "LTRIM(RTRIM(ISNULL(Nombre1,'') + ' ' + ISNULL(Nombre2,'') + ' ' + " &
            "ISNULL(Apellido1,'') + ' ' + ISNULL(Apellido2,''))) AS Nombres, " &
            "NumCedula " &
            "FROM Empleado WHERE Activo = 1 ORDER BY CodEmpleado1")

    End Function

    ''' <summary>
    ''' Siguiente código numérico libre (máximo numérico + 1).
    ''' </summary>
    Public Function ProximoCodigo() As String

        Using cn As New SqlConnection(CadenaConexion)

            Using cmd As New SqlCommand(
                "SELECT ISNULL(MAX(TRY_CAST(CodEmpleado1 AS BIGINT)), 0) + 1 " &
                "FROM Empleado", cn)

                cn.Open()

                Return Convert.ToString(cmd.ExecuteScalar())

            End Using

        End Using

    End Function

#End Region

#Region "LISTAR"

    Public Function Listar() As DataTable

        Dim dt As New DataTable

        Dim Sql As String =
"
SELECT
CodEmpleado,
CodEmpleado1,
Nombre1,
Nombre2,
Apellido1,
Apellido2,
NumeroInss,
NumCedula,
Telefono
FROM Empleado
WHERE Activo = 1
ORDER BY CodEmpleado1
"

        Using cn As New SqlConnection(CadenaConexion)

            Using da As New SqlDataAdapter(
                Sql,
                cn)

                da.Fill(dt)

            End Using

        End Using

        Return dt

    End Function

#End Region

#Region "HISTORICO"

    Private Sub InsertarHistorico(
        emp As Empleados,
        cn As SqlConnection,
        trans As SqlTransaction)

        Dim Sql As String =
"
INSERT INTO Historico
(
Id,
CodEmpleado,
FechaBaja,
MotivoBaja,
FechaAumento,
MotivoAumento,
FechaInicialSusp,
FechaFinalSusp,
MotivoSuspencion,
FechaNacimiento,
FechaContrato,
FechaContratoVac,
CargoInicial,
CargoActual,
CargoAnterior,
SueldoInicial,
SueldoAnterior,
SueldoActual,

CuentaSueldos,
ProvAguinaldo,
ProvVacaciones,
INSSPatronal,
INATEC,
CuentaHorasExtra,
CuentaOtrosIngresos,

AguinaldoxPagar,
VacacionesxPagar,
INSSxPagar,
IRxPagar,
INSSPatronalPagar,
NominaxPagar,
INATECxPagar,
PrestamoxPagar,

CuentaBanco
)
VALUES
(
(SELECT ISNULL(MAX(Id), 0) + 1 FROM Historico WITH (UPDLOCK, HOLDLOCK)),
@CodEmpleado,
@FechaBaja,
@MotivoBaja,
@FechaAumento,
@MotivoAumento,
@FechaInicialSusp,
@FechaFinalSusp,
@MotivoSuspencion,
@FechaNacimiento,
@FechaContrato,
@FechaContratoVac,
@CargoInicial,
@CargoActual,
@CargoAnterior,
@SueldoInicial,
@SueldoAnterior,
@SueldoActual,

@CuentaSueldos,
@ProvAguinaldo,
@ProvVacaciones,
@INSSPatronal,
@INATEC,
@CuentaHorasExtra,
@CuentaOtrosIngresos,

@AguinaldoxPagar,
@VacacionesxPagar,
@INSSxPagar,
@IRxPagar,
@INSSPatronalPagar,
@NominaxPagar,
@INATECxPagar,
@PrestamoxPagar,

@CuentaBanco_BAC
)
"

        Using cmd As New SqlCommand(
            Sql,
            cn,
            trans)

            AgregarParametrosHistorico(
                cmd,
                emp)

            cmd.ExecuteNonQuery()

        End Using

    End Sub

    Private Sub ActualizarHistorico(
        emp As Empleados,
        cn As SqlConnection,
        trans As SqlTransaction)

        Dim Sql As String =
"
UPDATE Historico SET

FechaBaja = @FechaBaja,
MotivoBaja = @MotivoBaja,
FechaAumento = @FechaAumento,
MotivoAumento = @MotivoAumento,
FechaInicialSusp = @FechaInicialSusp,
FechaFinalSusp = @FechaFinalSusp,
MotivoSuspencion = @MotivoSuspencion,
FechaNacimiento = @FechaNacimiento,
FechaContrato = @FechaContrato,
FechaContratoVac = @FechaContratoVac,

CargoInicial = @CargoInicial,
CargoActual = @CargoActual,
CargoAnterior = @CargoAnterior,

SueldoInicial = @SueldoInicial,
SueldoAnterior = @SueldoAnterior,
SueldoActual = @SueldoActual,

CuentaSueldos = @CuentaSueldos,
ProvAguinaldo = @ProvAguinaldo,
ProvVacaciones = @ProvVacaciones,
INSSPatronal = @INSSPatronal,
INATEC = @INATEC,
CuentaHorasExtra = @CuentaHorasExtra,
CuentaOtrosIngresos = @CuentaOtrosIngresos,

AguinaldoxPagar = @AguinaldoxPagar,
VacacionesxPagar = @VacacionesxPagar,
INSSxPagar = @INSSxPagar,
IRxPagar = @IRxPagar,
INSSPatronalPagar = @INSSPatronalPagar,
NominaxPagar = @NominaxPagar,
INATECxPagar = @INATECxPagar,
PrestamoxPagar = @PrestamoxPagar,

CuentaBanco = @CuentaBanco_BAC

WHERE CodEmpleado = @CodEmpleado
"

        Using cmd As New SqlCommand(
            Sql,
            cn,
            trans)

            AgregarParametrosHistorico(
                cmd,
                emp)

            If cmd.ExecuteNonQuery() = 0 Then
                InsertarHistorico(emp, cn, trans)
            End If

        End Using

    End Sub

#End Region

#Region "PARAMETROS"

    Private Sub AgregarParametrosEmpleado(
        cmd As SqlCommand,
        emp As Empleados)

        cmd.Parameters.AddWithValue("@CodEmpleado1", emp.Cod_Empleado1)
        cmd.Parameters.AddWithValue("@Nombre1", emp.Primer_Nombre)
        cmd.Parameters.AddWithValue("@Nombre2", emp.Segundo_Nombre)
        cmd.Parameters.AddWithValue("@Apellido1", emp.Primer_Apellido)
        cmd.Parameters.AddWithValue("@Apellido2", emp.Segundo_Apellido)
        cmd.Parameters.AddWithValue("@Direccion", emp.Empleado_Direccion)
        cmd.Parameters.AddWithValue("@Telefono", emp.Empleado_Telefono)
        cmd.Parameters.AddWithValue("@Nacionalidad", emp.Empleado_Nacionalidad)
        cmd.Parameters.AddWithValue("@CodigoPostal", emp.Codigo_Postal)
        cmd.Parameters.AddWithValue("@Sexo", emp.Empleado_Sexo)
        cmd.Parameters.AddWithValue("@NumeroRuc", emp.Numero_Ruc)
        cmd.Parameters.AddWithValue("@NumCedula", emp.Num_Cedula)
        cmd.Parameters.AddWithValue("@NumeroInss", emp.Numero_Inss)
        cmd.Parameters.AddWithValue("@NumHijos", emp.Num_Hijos)
        cmd.Parameters.AddWithValue("@Sindicalista", emp.Empleado_Sindicalista)
        cmd.Parameters.AddWithValue("@CodDepartamento", emp.Cod_Departamento)
        cmd.Parameters.AddWithValue("@CodCargo", emp.Cod_Cargo)
        cmd.Parameters.AddWithValue("@CuentaBanco", emp.Empleado_CuentaBanco)
        cmd.Parameters.AddWithValue("@Correo_Electronico", emp.Correo_Electronico)
        cmd.Parameters.AddWithValue("@NumeroCelular", emp.Numero_Celular)
        cmd.Parameters.AddWithValue("@CodTipoNomina", emp.Cod_TipoNomina)
        cmd.Parameters.AddWithValue("@DiasDescuento", emp.Dias_Descuento)
        cmd.Parameters.AddWithValue("@SalPorcentaje", emp.Sal_Porcentaje)
        cmd.Parameters.AddWithValue("@SueldoPeriodo", emp.Sueldo_Periodo)
        cmd.Parameters.AddWithValue("@TarifaHoraria", emp.Tarifa_Horaria)
        cmd.Parameters.AddWithValue("@PorcentajeComision", emp.Porcentaje_Comision)
        cmd.Parameters.AddWithValue("@OtrosIngresos", emp.Otros_Ingresos)
        cmd.Parameters.AddWithValue("@DescripOtrIngre", emp.Descrip_OtrIngre)
        cmd.Parameters.AddWithValue("@ExentoInss", emp.Exento_Inss)
        cmd.Parameters.AddWithValue("@ExentoIr", emp.Exento_Ir)
        cmd.Parameters.AddWithValue("@PagoInssPatronal", emp.PagoInss_Patronal)
        cmd.Parameters.AddWithValue("@SalarioMinimo", emp.Salario_Minimo)
        cmd.Parameters.AddWithValue("@Activo", emp.Empleado_Activo)
        cmd.Parameters.AddWithValue("@Liquidado", emp.Empleado_Liquidado)
        cmd.Parameters.AddWithValue("@Ausente", emp.Empleado_Ausente)
        cmd.Parameters.AddWithValue("@SalarioFijo", emp.Salario_Fijo)
        cmd.Parameters.AddWithValue("@SumarSubsidio", emp.Sumar_Subsidio)
        cmd.Parameters.AddWithValue("@PorcientoIncentivo", emp.Porciento_Incentivo)
        cmd.Parameters.AddWithValue("@Gravidez", emp.Empleado_Gravidez)
        cmd.Parameters.AddWithValue("@VacacionesBasico", emp.Vacaciones_Basico)
        cmd.Parameters.AddWithValue("@Dolarizado", emp.Empleado_Dolarizado)
        cmd.Parameters.AddWithValue("@FechaAntiguedad",
            If(IsDate(emp.Fecha_Antiguedad),
               CDate(emp.Fecha_Antiguedad),
               CObj(DBNull.Value)))
        cmd.Parameters.AddWithValue("@Antiguedad", emp.Empleado_Antiguedad)
        cmd.Parameters.AddWithValue("@SueldoActualBasico", emp.SueldoActual_Basico)
        cmd.Parameters.AddWithValue("@DiasBasico", emp.Dias_Basico)
        cmd.Parameters.AddWithValue("@AumentoBasico", emp.Aumento_Basico)
        cmd.Parameters.AddWithValue("@Foto", emp.Empleado_Foto)
        cmd.Parameters.AddWithValue("@ViaticoxDia", emp.Viatico_Dia)
        cmd.Parameters.AddWithValue("@Reembolso", emp.Empleado_Reembolso)
        cmd.Parameters.AddWithValue("@DeducirPorciento", emp.Deducir_Porciento)
        cmd.Parameters.AddWithValue("@HorasTurno", emp.Horas_Turno)
        cmd.Parameters.AddWithValue("@CuentaBancoBANPRO", emp.CuentaBanco_BANPRO)
        cmd.Parameters.AddWithValue("@CuentaBancoBDF", emp.CuentaBanco_BDF)
        cmd.Parameters.AddWithValue("@CuentaBancoLAFISE", emp.CuentaBanco_LAFISE)

        NormalizarParametros(cmd)

    End Sub

    Private Sub AgregarParametrosHistorico(
        cmd As SqlCommand,
        emp As Empleados)

        cmd.Parameters.AddWithValue("@CodEmpleado", emp.Cod_Empleado)

        cmd.Parameters.AddWithValue("@FechaBaja",
            If(emp.Fecha_Baja = Date.MinValue,
               DBNull.Value,
               emp.Fecha_Baja))

        cmd.Parameters.AddWithValue("@MotivoBaja",
            emp.Motivo_Baja)

        cmd.Parameters.AddWithValue("@MotivoSuspencion",
            emp.Motivo_Suspencion)

        cmd.Parameters.AddWithValue("@FechaAumento",
            If(emp.Fecha_Aumento = Date.MinValue,
               DBNull.Value,
               emp.Fecha_Aumento))

        cmd.Parameters.AddWithValue("@MotivoAumento",
            emp.Motivo_Aumento)

        cmd.Parameters.AddWithValue("@FechaInicialSusp",
            If(emp.FechaInicial_Susp = Date.MinValue,
               DBNull.Value,
               emp.FechaInicial_Susp))

        cmd.Parameters.AddWithValue("@FechaFinalSusp",
            If(emp.FechaFinal_Susp = Date.MinValue,
               DBNull.Value,
               emp.FechaFinal_Susp))

        cmd.Parameters.AddWithValue("@FechaNacimiento",
            emp.Fecha_Nacimiento)

        cmd.Parameters.AddWithValue("@FechaContrato",
            emp.Fecha_Contrato)

        cmd.Parameters.AddWithValue("@FechaContratoVac",
            emp.Fecha_ContratoVac)

        cmd.Parameters.AddWithValue("@CargoInicial",
            emp.Cargo_Inicial)

        cmd.Parameters.AddWithValue("@CargoActual",
            emp.Cargo_Actual)

        cmd.Parameters.AddWithValue("@CargoAnterior",
            emp.Cargo_Anterior)

        cmd.Parameters.AddWithValue("@SueldoInicial",
            emp.Sueldo_Inicial)

        cmd.Parameters.AddWithValue("@SueldoAnterior",
            emp.Sueldo_Anterior)

        cmd.Parameters.AddWithValue("@SueldoActual",
            emp.Sueldo_Actual)

        cmd.Parameters.AddWithValue("@CuentaSueldos",
            emp.Cuenta_Sueldos)

        cmd.Parameters.AddWithValue("@ProvAguinaldo",
            emp.Prov_Aguinaldo)

        cmd.Parameters.AddWithValue("@ProvVacaciones",
            emp.Prov_Vacaciones)

        cmd.Parameters.AddWithValue("@INSSPatronal",
            emp.INSS_Patronal)

        cmd.Parameters.AddWithValue("@INATEC",
            emp.Empleado_INATEC)

        cmd.Parameters.AddWithValue("@CuentaHorasExtra",
            emp.CuentaHoras_Extra)

        cmd.Parameters.AddWithValue("@CuentaOtrosIngresos",
            emp.CuentaOtros_Ingresos)

        cmd.Parameters.AddWithValue("@AguinaldoxPagar",
            emp.Aguinaldo_Pagar)

        cmd.Parameters.AddWithValue("@VacacionesxPagar",
            emp.Vacaciones_Pagar)

        cmd.Parameters.AddWithValue("@INSSxPagar",
            emp.INSS_Pagar)

        cmd.Parameters.AddWithValue("@IRxPagar",
            emp.IR_Pagar)

        cmd.Parameters.AddWithValue("@INSSPatronalPagar",
            emp.INSSPatronal_Pagar)

        cmd.Parameters.AddWithValue("@NominaxPagar",
            emp.Nomina_Pagar)

        cmd.Parameters.AddWithValue("@INATECxPagar",
            emp.INATEC_Pagar)

        cmd.Parameters.AddWithValue("@PrestamoxPagar",
            emp.Prestamo_Pagar)

        cmd.Parameters.AddWithValue("@CuentaBanco_BAC",
            emp.Cuenta_Banco)

        NormalizarParametros(cmd)

    End Sub

    ''' <summary>
    ''' Nothing -> NULL, y fechas fuera del rango de smalldatetime
    ''' (p.ej. Date.MinValue de un campo vacío) -> NULL.
    ''' </summary>
    Private Sub NormalizarParametros(cmd As SqlCommand)

        For Each p As SqlParameter In cmd.Parameters

            If p.Value Is Nothing Then

                p.Value = DBNull.Value

            ElseIf TypeOf p.Value Is Date Then

                Dim d As Date = CDate(p.Value)

                If d < New Date(1900, 1, 1) OrElse
                   d > New Date(2079, 6, 6) Then
                    p.Value = DBNull.Value
                End If

            End If

        Next

    End Sub

#End Region

#Region "MAPEAR"

    Private Function MapearEmpleado(
    dr As SqlDataReader) As Empleados

        Dim emp As New Empleados

        '====================================================
        ' EMPLEADO
        '====================================================

        emp.Cod_Empleado =
            If(IsDBNull(dr("CodEmpleado")),
               0,
               Convert.ToInt32(dr("CodEmpleado")))

        emp.Cod_Empleado1 =
            dr("CodEmpleado1").ToString()

        emp.Primer_Nombre =
            dr("Nombre1").ToString()

        emp.Segundo_Nombre =
            dr("Nombre2").ToString()

        emp.Primer_Apellido =
            dr("Apellido1").ToString()

        emp.Segundo_Apellido =
            dr("Apellido2").ToString()

        emp.Empleado_Direccion =
            dr("Direccion").ToString()

        emp.Empleado_Telefono =
            dr("Telefono").ToString()

        emp.Empleado_Nacionalidad =
            dr("Nacionalidad").ToString()

        emp.Codigo_Postal =
            dr("CodigoPostal").ToString()

        emp.Empleado_Sexo =
            dr("Sexo").ToString()

        emp.Numero_Ruc =
            dr("NumeroRuc").ToString()

        emp.Num_Cedula =
            dr("NumCedula").ToString()

        emp.Numero_Inss =
            dr("NumeroInss").ToString()

        emp.Num_Hijos =
            If(IsDBNull(dr("NumHijos")),
               0,
               Convert.ToInt32(dr("NumHijos")))

        emp.Empleado_Sindicalista =
            dr("Sindicalista").ToString()

        emp.Cod_Departamento =
            dr("CodDepartamento").ToString()

        emp.Cod_Cargo =
            dr("CodCargo").ToString()

        emp.Empleado_CuentaBanco =
            dr("CuentaBanco").ToString()

        emp.Correo_Electronico =
            dr("Correo_Electronico").ToString()

        emp.Numero_Celular =
            dr("NumeroCelular").ToString()

        emp.Cod_TipoNomina =
            dr("CodTipoNomina").ToString()

        emp.Dias_Descuento =
            If(IsDBNull(dr("DiasDescuento")),
               0,
               Convert.ToDecimal(dr("DiasDescuento")))

        emp.Sal_Porcentaje =
            If(IsDBNull(dr("SalPorcentaje")),
               0,
               Convert.ToDecimal(dr("SalPorcentaje")))

        emp.Sueldo_Periodo =
            If(IsDBNull(dr("SueldoPeriodo")),
               0,
               Convert.ToDecimal(dr("SueldoPeriodo")))

        emp.Tarifa_Horaria =
            If(IsDBNull(dr("TarifaHoraria")),
               0,
               Convert.ToDecimal(dr("TarifaHoraria")))

        emp.Porcentaje_Comision =
            If(IsDBNull(dr("PorcentajeComision")),
               0,
               Convert.ToDecimal(dr("PorcentajeComision")))

        emp.Otros_Ingresos =
            If(IsDBNull(dr("OtrosIngresos")),
               0,
               Convert.ToDecimal(dr("OtrosIngresos")))

        emp.Descrip_OtrIngre =
            dr("DescripOtrIngre").ToString()

        emp.Exento_Inss =
            If(IsDBNull(dr("ExentoInss")),
               0,
               Convert.ToInt32(dr("ExentoInss")))

        emp.Exento_Ir =
            If(IsDBNull(dr("ExentoIr")),
               0,
               Convert.ToInt32(dr("ExentoIr")))

        emp.PagoInss_Patronal =
            If(IsDBNull(dr("PagoInssPatronal")),
               0,
               Convert.ToInt32(dr("PagoInssPatronal")))

        emp.Salario_Minimo =
            If(IsDBNull(dr("SalarioMinimo")),
               0,
               Convert.ToInt32(dr("SalarioMinimo")))

        emp.Empleado_Activo =
            If(IsDBNull(dr("Activo")),
               0,
               Convert.ToInt32(dr("Activo")))

        emp.Empleado_Liquidado =
            If(IsDBNull(dr("Liquidado")),
               0,
               Convert.ToInt32(dr("Liquidado")))

        emp.Empleado_Ausente =
            If(IsDBNull(dr("Ausente")),
               0,
               Convert.ToInt32(dr("Ausente")))

        emp.Salario_Fijo =
            dr("SalarioFijo").ToString()

        emp.Sumar_Subsidio =
            If(IsDBNull(dr("SumarSubsidio")),
               0,
               Convert.ToInt32(dr("SumarSubsidio")))

        emp.Porciento_Incentivo =
            If(IsDBNull(dr("PorcientoIncentivo")),
               0,
               Convert.ToDecimal(dr("PorcientoIncentivo")))

        emp.Empleado_Gravidez =
            If(IsDBNull(dr("Gravidez")),
               0,
               Convert.ToInt32(dr("Gravidez")))

        emp.Vacaciones_Basico =
            If(IsDBNull(dr("VacacionesBasico")),
               0,
               Convert.ToInt32(dr("VacacionesBasico")))

        emp.Empleado_Dolarizado =
            If(IsDBNull(dr("Dolarizado")),
               0,
               Convert.ToInt32(dr("Dolarizado")))

        emp.Fecha_Antiguedad =
            If(IsDBNull(dr("FechaAntiguedad")),
               Date.MinValue,
               Convert.ToDateTime(dr("FechaAntiguedad")))

        emp.Empleado_Antiguedad =
            If(IsDBNull(dr("Antiguedad")),
               0,
               Convert.ToDecimal(dr("Antiguedad")))

        emp.SueldoActual_Basico =
            If(IsDBNull(dr("SueldoActualBasico")),
               0,
               Convert.ToInt32(dr("SueldoActualBasico")))

        emp.Dias_Basico =
            If(IsDBNull(dr("DiasBasico")),
               0,
               Convert.ToDecimal(dr("DiasBasico")))

        emp.Aumento_Basico =
            If(IsDBNull(dr("AumentoBasico")),
               0,
               Convert.ToDecimal(dr("AumentoBasico")))

        emp.Viatico_Dia =
            If(IsDBNull(dr("ViaticoxDia")),
               0,
               Convert.ToDouble(dr("ViaticoxDia")))

        emp.Empleado_Reembolso =
            If(IsDBNull(dr("Reembolso")),
               0,
               Convert.ToDouble(dr("Reembolso")))

        emp.Deducir_Porciento =
            If(IsDBNull(dr("DeducirPorciento")),
               0,
               Convert.ToInt32(dr("DeducirPorciento")))

        emp.Horas_Turno =
            If(IsDBNull(dr("HorasTurno")),
               0,
               Convert.ToInt32(dr("HorasTurno")))

        If Not IsDBNull(dr("Foto")) Then
            emp.Empleado_Foto =
                dr("Foto").ToString()
        End If

        '====================================================
        ' HISTORICO
        '====================================================

        emp.Cargo_Inicial =
            dr("CargoInicial").ToString()

        emp.Cargo_Actual =
            dr("CargoActual").ToString()

        emp.Cargo_Anterior =
            dr("CargoAnterior").ToString()

        emp.Sueldo_Inicial =
            If(IsDBNull(dr("SueldoInicial")),
               0,
               Convert.ToDecimal(dr("SueldoInicial")))

        emp.Sueldo_Anterior =
            If(IsDBNull(dr("SueldoAnterior")),
               0,
               Convert.ToDecimal(dr("SueldoAnterior")))

        emp.Sueldo_Actual =
            If(IsDBNull(dr("SueldoActual")),
               0,
               Convert.ToDecimal(dr("SueldoActual")))

        emp.Fecha_Baja =
            If(IsDBNull(dr("FechaBaja")),
               Date.MinValue,
               Convert.ToDateTime(dr("FechaBaja")))

        emp.Motivo_Baja =
            dr("MotivoBaja").ToString()

        emp.Motivo_Suspencion =
            dr("MotivoSuspencion").ToString()

        emp.Fecha_Aumento =
            If(IsDBNull(dr("FechaAumento")),
               Date.MinValue,
               Convert.ToDateTime(dr("FechaAumento")))

        emp.Motivo_Aumento =
            dr("MotivoAumento").ToString()

        emp.FechaInicial_Susp =
            If(IsDBNull(dr("FechaInicialSusp")),
               Date.MinValue,
               Convert.ToDateTime(dr("FechaInicialSusp")))

        emp.FechaFinal_Susp =
            If(IsDBNull(dr("FechaFinalSusp")),
               Date.MinValue,
               Convert.ToDateTime(dr("FechaFinalSusp")))

        emp.Fecha_Nacimiento =
            If(IsDBNull(dr("FechaNacimiento")),
               Date.MinValue,
               Convert.ToDateTime(dr("FechaNacimiento")))

        emp.Fecha_Contrato =
            If(IsDBNull(dr("FechaContrato")),
               Date.MinValue,
               Convert.ToDateTime(dr("FechaContrato")))

        emp.Fecha_ContratoVac =
            If(IsDBNull(dr("FechaContratoVac")),
               Date.MinValue,
               Convert.ToDateTime(dr("FechaContratoVac")))

        '====================================================
        ' CUENTAS
        '====================================================

        emp.Cuenta_Sueldos =
            dr("CuentaSueldos").ToString()

        emp.Prov_Aguinaldo =
            dr("ProvAguinaldo").ToString()

        emp.Prov_Vacaciones =
            dr("ProvVacaciones").ToString()

        emp.INSS_Patronal =
            dr("INSSPatronal").ToString()

        emp.Empleado_INATEC =
            dr("INATEC").ToString()

        emp.CuentaHoras_Extra =
            dr("CuentaHorasExtra").ToString()

        emp.CuentaOtros_Ingresos =
            dr("CuentaOtrosIngresos").ToString()

        emp.Aguinaldo_Pagar =
            dr("AguinaldoxPagar").ToString()

        emp.Vacaciones_Pagar =
            dr("VacacionesxPagar").ToString()

        emp.INSS_Pagar =
            dr("INSSxPagar").ToString()

        emp.IR_Pagar =
            dr("IRxPagar").ToString()

        emp.INSSPatronal_Pagar =
            dr("INSSPatronalPagar").ToString()

        emp.Nomina_Pagar =
            dr("NominaxPagar").ToString()

        emp.INATEC_Pagar =
            dr("INATECxPagar").ToString()

        emp.Prestamo_Pagar =
            dr("PrestamoxPagar").ToString()

        '====================================================
        ' BANCOS
        '====================================================

        emp.CuentaBanco_BANPRO =
            dr("CuentaBanco_BANPRO").ToString()

        emp.CuentaBanco_BDF =
            dr("CuentaBanco_BDF").ToString()

        emp.CuentaBanco_LAFISE =
            dr("CuentaBanco_LAFISE").ToString()

        emp.Cuenta_Banco =
            dr("CuentaBanco_BAC").ToString()

        Return emp

    End Function

#End Region

End Class





