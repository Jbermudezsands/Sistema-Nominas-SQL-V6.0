Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Importación de movimientos desde tablas leídas de Excel (horas extra, deducciones, movimiento salarial) y
''' limpieza de movimientos pendientes. Las filas con problema no detienen la importación: se devuelven en
''' el resultado para que el usuario las revise; un error inesperado de base de datos sí deshace todo.
''' </summary>
Public Class CsImportacionDAL
    Inherits CsDALBase

    ''' <summary>Líneas "código, mensaje" de lo ocurrido con cada fila.</summary>
    Public Class ResultadoImportacion
        Public ReadOnly Property Mensajes As New List(Of String)
        Public Property Importadas As Integer
    End Class

#Region "HORAS EXTRA"

    ''' <summary>Columnas por posición: 0 = código del empleado, 1 = nombre, 2 = horas.</summary>
    Public Function ImportarHorasExtra(filas As DataTable, acumular As Boolean) As ResultadoImportacion

        Dim res As New ResultadoImportacion

        EnTransaccion(
            Sub(cn, tr)

                Dim siguienteId As Integer = Convert.ToInt32(EscalarEn(cn, tr,
                    "SELECT ISNULL(MAX(Id), 0) + 1 FROM HorasExtras WITH (TABLOCKX)"))

                For Each fila As DataRow In filas.Rows

                    Dim codigo As String = Texto(fila, 0)

                    If codigo = "" Then Continue For

                    Dim horas As Double
                    If Not Double.TryParse(Texto(fila, 2), horas) Then
                        res.Mensajes.Add(codigo & ", horas no válidas")
                        Continue For
                    End If

                    Dim emp As Decimal, numNomina As Integer
                    Dim problema As String = BuscarEmpleadoYNomina(cn, tr, codigo, emp, numNomina)

                    If problema <> "" Then
                        res.Mensajes.Add(codigo & ", " & problema)
                        Continue For
                    End If

                    Dim pEmp As New SqlParameter("@Emp", SqlDbType.Decimal) With {.Value = emp}
                    Dim pNom As New SqlParameter("@Nom", numNomina)
                    Dim pHoras As New SqlParameter("@Horas", horas)

                    Dim actualizadas As Integer = EjecutarEn(cn, tr,
                        If(acumular,
                           "UPDATE HorasExtras SET CantHoras = ISNULL(CantHoras, 0) + @Horas WHERE CodEmpleado = @Emp AND NumNomina = @Nom",
                           "UPDATE HorasExtras SET CantHoras = @Horas, Pagada = 0 WHERE CodEmpleado = @Emp AND NumNomina = @Nom"),
                        pEmp, pNom, pHoras)

                    If actualizadas > 0 Then
                        res.Mensajes.Add(codigo & ", horas actualizadas")
                    Else
                        EjecutarEn(cn, tr,
                            "INSERT INTO HorasExtras (Id, CodEmpleado, NumNomina, CantHoras, Pagada) VALUES (@Id, @Emp2, @Nom2, @Horas2, 0)",
                            New SqlParameter("@Id", siguienteId),
                            New SqlParameter("@Emp2", SqlDbType.Decimal) With {.Value = emp},
                            New SqlParameter("@Nom2", numNomina),
                            New SqlParameter("@Horas2", horas))
                        siguienteId += 1
                        res.Mensajes.Add(codigo & ", horas importadas")
                    End If

                    res.Importadas += 1

                Next

            End Sub)

        Return res

    End Function

#End Region

#Region "DEDUCCIONES"

    ''' <summary>Columnas por posición: 0 = INSS, 1 = código del empleado, 2 = nombre, 3 = monto, 4 = código de deducción.</summary>
    Public Function ImportarDeducciones(filas As DataTable) As ResultadoImportacion

        Dim res As New ResultadoImportacion

        EnTransaccion(
            Sub(cn, tr)

                For Each fila As DataRow In filas.Rows

                    Dim codigo As String = Texto(fila, 1)

                    If codigo = "" Then Continue For

                    Dim monto As Double
                    If Not Double.TryParse(Texto(fila, 3), monto) Then
                        res.Mensajes.Add(codigo & ", monto no válido")
                        Continue For
                    End If

                    Dim codDeduccion As String = Texto(fila, 4)

                    If Convert.ToInt32(EscalarEn(cn, tr, "SELECT COUNT(*) FROM TipoDeduccion WHERE CodTipoDeduccion = @C",
                                                 New SqlParameter("@C", codDeduccion))) = 0 Then
                        res.Mensajes.Add(codigo & ", tipo de deducción '" & codDeduccion & "' no existe")
                        Continue For
                    End If

                    Dim emp As Decimal, numNomina As Integer
                    Dim problema As String = BuscarEmpleadoYNomina(cn, tr, codigo, emp, numNomina)

                    If problema <> "" Then
                        res.Mensajes.Add(codigo & ", " & problema)
                        Continue For
                    End If

                    ' Una deducción por empleado, tipo y nómina; si ya existe sin pagar se corrige el monto.
                    Dim numDeduccion As Object = EscalarEn(cn, tr,
                        "SELECT TOP 1 NumDeduccion FROM Deduccion WHERE CodEmpleado = @Emp AND NUmNomina = @Nom AND CodTipoDeduccion = @Tipo",
                        New SqlParameter("@Emp", SqlDbType.Decimal) With {.Value = emp},
                        New SqlParameter("@Nom", numNomina),
                        New SqlParameter("@Tipo", codDeduccion))

                    Dim num As Integer

                    If numDeduccion Is Nothing OrElse IsDBNull(numDeduccion) Then

                        ' Como CsMovimientosDAL: máximo de la cabecera + 1 (Consecutivos.Deducciones puede ir atrasado).
                        num = Convert.ToInt32(EscalarEn(cn, tr,
                            "SELECT ISNULL(MAX(NumDeduccion), 0) + 1 FROM Deduccion WITH (UPDLOCK, HOLDLOCK)"))

                        EjecutarEn(cn, tr,
                            "INSERT INTO Deduccion (NumDeduccion, CodEmpleado, CodTipoDeduccion, NumVeces, Pagado, NUmNomina) " &
                            "VALUES (@Num, @Emp, @Tipo, '1', 0, @Nom)",
                            New SqlParameter("@Num", num),
                            New SqlParameter("@Emp", SqlDbType.Decimal) With {.Value = emp},
                            New SqlParameter("@Tipo", codDeduccion),
                            New SqlParameter("@Nom", numNomina))

                    Else
                        num = Convert.ToInt32(numDeduccion)
                    End If

                    Dim cambiadas As Integer = EjecutarEn(cn, tr,
                        "UPDATE DetalleDeduccion SET Valor = @Valor WHERE NumDeduccion = @Num AND NumNomina = @Nom AND Pagado = 0",
                        New SqlParameter("@Valor", monto),
                        New SqlParameter("@Num", num),
                        New SqlParameter("@Nom", numNomina))

                    If cambiadas = 0 Then
                        EjecutarEn(cn, tr,
                            "INSERT INTO DetalleDeduccion (NumDeduccion, Valor, NumVez, Pagado, NumNomina) VALUES (@Num, @Valor, '1', 0, @Nom)",
                            New SqlParameter("@Num", num),
                            New SqlParameter("@Valor", monto),
                            New SqlParameter("@Nom", numNomina))
                    End If

                    res.Importadas += 1

                Next

            End Sub)

        Return res

    End Function

#End Region

#Region "INCENTIVOS (INGRESOS)"

    ''' <summary>
    ''' Columnas por posición: 0 = INSS, 1 = código del empleado, 2 = nombre, 3 = monto, 4 = código del tipo de incentivo.
    ''' Un incentivo por empleado y tipo (se reutiliza el existente); en la nómina activa del empleado se corrige el
    ''' monto de la vez sin pagar o se agrega una vez nueva.
    ''' </summary>
    Public Function ImportarIncentivos(filas As DataTable) As ResultadoImportacion

        Dim res As New ResultadoImportacion

        EnTransaccion(
            Sub(cn, tr)

                For Each fila As DataRow In filas.Rows

                    Dim codigo As String = Texto(fila, 1)

                    If codigo = "" Then Continue For

                    If codigo.All(AddressOf Char.IsDigit) Then codigo = codigo.PadLeft(4, "0"c)

                    Dim monto As Double
                    If Not Double.TryParse(Texto(fila, 3), monto) Then
                        res.Mensajes.Add(codigo & ", monto no válido")
                        Continue For
                    End If

                    Dim codIncentivo As String = Texto(fila, 4)

                    If Convert.ToInt32(EscalarEn(cn, tr, "SELECT COUNT(*) FROM TipoIncentivo WHERE CodTipoIncentivo = @C",
                                                 New SqlParameter("@C", codIncentivo))) = 0 Then
                        res.Mensajes.Add(codigo & ", tipo de incentivo '" & codIncentivo & "' no existe")
                        Continue For
                    End If

                    Dim emp As Decimal, numNomina As Integer
                    Dim problema As String = BuscarEmpleadoYNomina(cn, tr, codigo, emp, numNomina)

                    If problema <> "" Then
                        res.Mensajes.Add(codigo & ", " & problema)
                        Continue For
                    End If

                    Dim numIncentivo As Object = EscalarEn(cn, tr,
                        "SELECT TOP 1 NumIncentivo FROM Incentivo WHERE CodEmpleado = @Emp AND CodTipoIncentivo = @Tipo ORDER BY NumIncentivo",
                        New SqlParameter("@Emp", SqlDbType.Decimal) With {.Value = emp},
                        New SqlParameter("@Tipo", codIncentivo))

                    Dim num As Integer

                    If numIncentivo Is Nothing OrElse IsDBNull(numIncentivo) Then

                        num = Convert.ToInt32(EscalarEn(cn, tr, "SELECT ISNULL(MAX(NumIncentivo), 0) + 1 FROM Incentivo WITH (UPDLOCK, HOLDLOCK)"))

                        EjecutarEn(cn, tr,
                            "INSERT INTO Incentivo (NumIncentivo, CodEmpleado, CodTipoIncentivo, NumVeces, Pagado) VALUES (@Num, @Emp, @Tipo, '1', 0)",
                            New SqlParameter("@Num", num),
                            New SqlParameter("@Emp", SqlDbType.Decimal) With {.Value = emp},
                            New SqlParameter("@Tipo", codIncentivo))

                    Else
                        num = Convert.ToInt32(numIncentivo)
                    End If

                    Dim cambiadas As Integer = EjecutarEn(cn, tr,
                        "UPDATE DetalleIncentivo SET Valor = @Valor WHERE NumIncentivo = @Num AND NumNomina = @Nom AND Pagado = 0",
                        New SqlParameter("@Valor", monto),
                        New SqlParameter("@Num", num),
                        New SqlParameter("@Nom", numNomina))

                    If cambiadas = 0 Then
                        ' PK (Id, NumIncentivo): Id es el número de vez dentro del incentivo.
                        Dim vez As Integer = Convert.ToInt32(EscalarEn(cn, tr,
                            "SELECT ISNULL(MAX(Id), 0) + 1 FROM DetalleIncentivo WHERE NumIncentivo = @Num", New SqlParameter("@Num", num)))

                        EjecutarEn(cn, tr,
                            "INSERT INTO DetalleIncentivo (Id, NumIncentivo, Valor, NumVez, Pagado, NumNomina) VALUES (@Id, @Num, @Valor, @Vez, 0, @Nom)",
                            New SqlParameter("@Id", vez),
                            New SqlParameter("@Num", num),
                            New SqlParameter("@Valor", monto),
                            New SqlParameter("@Vez", vez.ToString()),
                            New SqlParameter("@Nom", numNomina))
                    End If

                    res.Importadas += 1

                Next

            End Sub)

        Return res

    End Function

#End Region

#Region "SOLICITUDES"

    ''' <summary>
    ''' Columnas por posición: 0 = código del empleado, 1 = nombre, 2 = cantidad (días o horas según el tipo). El empleado debe estar
    ''' activo y figurar en el detalle de una nómina activa; la solicitud se fecha al inicio de esa nómina. Vacaciones, Vacaciones
    ''' Pagadas/Programadas y Subsidio llevan la cantidad en días (las tres últimas con fecha final = inicio + días); el resto
    ''' (Ausente, Feriado, Permiso Programado, Suspension, AusenteBasico*) es 1 día, o cantidad / horas del tipo de nómina si
    ''' la cantidad viene en horas. No se evitan duplicados: importar dos veces el mismo archivo agrega las solicitudes dos veces.
    ''' </summary>
    Public Function ImportarSolicitudes(filas As DataTable, tipoSolicitud As String, clasificacion As String,
                                        fechaSolicitud As Date) As ResultadoImportacion

        Dim res As New ResultadoImportacion
        Dim enDias As Boolean = {"Vacaciones", "Vacaciones Pagadas", "Vacaciones Programadas", "Subsidio"}.Contains(tipoSolicitud)
        Dim conFechaFinal As Boolean = {"Vacaciones Pagadas", "Vacaciones Programadas", "Subsidio"}.Contains(tipoSolicitud)

        EnTransaccion(
            Sub(cn, tr)

                Dim ultimo As Object = EscalarEn(cn, tr,
                    "SELECT MAX(CASE WHEN NumeroSolicitud NOT LIKE '%[^0-9]%' AND LEN(NumeroSolicitud) <= 9 " &
                    "THEN CAST(NumeroSolicitud AS int) END) FROM SolicitudVacaciones WITH (TABLOCKX)")
                Dim siguiente As Integer = If(ultimo Is Nothing OrElse IsDBNull(ultimo), 0, Convert.ToInt32(ultimo)) + 1

                For Each fila As DataRow In filas.Rows

                    Dim codigo As String = Texto(fila, 0)

                    If codigo = "" Then Continue For

                    Dim cantidad As Double
                    Dim textoCantidad As String = Texto(fila, 2)

                    If textoCantidad <> "" AndAlso Not Double.TryParse(textoCantidad, cantidad) Then
                        res.Mensajes.Add(codigo & ", cantidad no válida")
                        Continue For
                    End If

                    If enDias AndAlso cantidad <= 0 Then
                        res.Mensajes.Add(codigo & ", cantidad de días no válida")
                        Continue For
                    End If

                    Dim emp As DataTable = ConsultaEn(cn, tr,
                        "SELECT e.CodEmpleado, t.Horas FROM Empleado e LEFT JOIN TipoNomina t ON t.CodTipoNomina = e.CodTipoNomina " &
                        "WHERE e.CodEmpleado1 = @Cod AND e.Activo = 1", New SqlParameter("@Cod", codigo))

                    If emp.Rows.Count = 0 Then
                        res.Mensajes.Add(codigo & ", Inactivo o no existe")
                        Continue For
                    End If

                    Dim inicio As Object = EscalarEn(cn, tr,
                        "SELECT TOP 1 n.FechaNominaINI FROM Nomina n INNER JOIN DetalleNomina d ON d.NumNomina = n.NumNomina " &
                        "WHERE n.Activa = 1 AND d.CodEmpleado = @Emp ORDER BY n.NumNomina DESC",
                        New SqlParameter("@Emp", SqlDbType.Decimal) With {.Value = Convert.ToDecimal(emp.Rows(0)("CodEmpleado"))})

                    If inicio Is Nothing OrElse IsDBNull(inicio) Then
                        res.Mensajes.Add(codigo & ", Empleado no tiene nomina calculada")
                        Continue For
                    End If

                    Dim fechaInicio As Date = Convert.ToDateTime(inicio)
                    Dim fechaFin As Date = If(conFechaFinal, fechaInicio.AddDays(cantidad), fechaInicio)
                    Dim dias As Double

                    If enDias Then
                        dias = cantidad
                    ElseIf cantidad <= 0 Then
                        dias = 1
                    Else
                        Dim horasDia As Double = If(IsDBNull(emp.Rows(0)("Horas")), 0, Convert.ToDouble(emp.Rows(0)("Horas")))

                        If horasDia <= 0 Then
                            res.Mensajes.Add(codigo & ", el tipo de nómina no tiene horas por día")
                            Continue For
                        End If

                        dias = cantidad / horasDia
                    End If

                    EjecutarEn(cn, tr,
                        "INSERT INTO SolicitudVacaciones (FechaSolicitud, NumeroSolicitud, TipoSolicitud, CodigoEmpleado, DiasVacaciones, " &
                        "DiasDisfrutados, FechaInicio, FechaFin, DiasDisfrutar, Clasificacion, Observaciones, Anulado, Pagado) " &
                        "VALUES (@FechaSol, @Num, @Tipo, @Cod, @Dias, @Dias, @Ini, @Fin, @Dias, @Clas, 'Importado', 0, 0)",
                        New SqlParameter("@FechaSol", fechaSolicitud.Date),
                        New SqlParameter("@Num", siguiente.ToString("00000")),
                        New SqlParameter("@Tipo", tipoSolicitud),
                        New SqlParameter("@Cod", codigo),
                        New SqlParameter("@Dias", dias),
                        New SqlParameter("@Ini", fechaInicio),
                        New SqlParameter("@Fin", fechaFin),
                        New SqlParameter("@Clas", clasificacion))

                    siguiente += 1
                    res.Importadas += 1
                    res.Mensajes.Add(codigo & ", Solicitud Agregada")

                Next

            End Sub)

        Return res

    End Function

    Private Shared Function ConsultaEn(cn As SqlConnection, tr As SqlTransaction, sql As String, ParamArray parametros() As SqlParameter) As DataTable

        Dim dt As New DataTable

        Using cmd As New SqlCommand(sql, cn, tr)
            cmd.Parameters.AddRange(parametros)
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        Return dt

    End Function

#End Region

#Region "MOVIMIENTO SALARIAL"

    ''' <summary>Columnas por posición: 0 = código, 1 = nombre, 2 = salario básico más nivelación (0 = no cambiarlo), 3 = aumento en córdobas (OtrosIngresos).</summary>
    Public Function ImportarMovimientoSalarial(filas As DataTable) As ResultadoImportacion

        Dim res As New ResultadoImportacion

        EnTransaccion(
            Sub(cn, tr)

                For Each fila As DataRow In filas.Rows

                    Dim codigo As String = Texto(fila, 0)

                    If codigo = "" Then Continue For

                    Dim salario As Double, aumento As Double

                    If Not Double.TryParse(Texto(fila, 2), salario) OrElse Not Double.TryParse(Texto(fila, 3), aumento) Then
                        res.Mensajes.Add(codigo & ", salario o aumento no válidos")
                        Continue For
                    End If

                    Dim n As Integer = EjecutarEn(cn, tr,
                        "UPDATE Empleado SET OtrosIngresos = @Aumento, SueldoPeriodo = CASE WHEN @Salario > 0 THEN @Salario ELSE SueldoPeriodo END " &
                        "WHERE CodEmpleado1 = @Cod AND Activo = 1",
                        New SqlParameter("@Aumento", aumento),
                        New SqlParameter("@Salario", salario),
                        New SqlParameter("@Cod", codigo))

                    If n = 0 Then
                        res.Mensajes.Add(codigo & " " & Texto(fila, 1) & ", inactivo o no existe")
                    Else
                        res.Importadas += 1
                    End If

                Next

            End Sub)

        Return res

    End Function

#End Region

#Region "EMPLEADOS"

    ''' <summary>Posición del encabezado en la hoja de la V.5.0 (se usa si el encabezado no se encuentra por nombre).</summary>
    Private Shared ReadOnly ColumnasEmpleado As String(,) = {
        {"Codigo", "0"}, {"Nombre1", "1"}, {"Nombre2", "2"}, {"Apellido1", "3"}, {"Apellido2", "4"}, {"Direccion", "5"},
        {"Nacionalidad", "6"}, {"Sexo", "7"}, {"FechaIngreso", "8"}, {"Cargo", "9"}, {"Departamento", "10"}, {"RUC", "11"},
        {"INSS", "12"}, {"Cedula", "13"}, {"Hijos", "14"}, {"TipoNomina", "15"}, {"GrupoNomina", "16"}, {"SueldoPeriodo", "17"},
        {"TarifaHoraria", "18"}, {"CuentaBanco", "20"}, {"Turnos", "21"}, {"FechaNacimiento", "22"}, {"Numerocelular", "23"},
        {"CelularEmergencia", "24"}, {"Profesion", "25"}, {"EstadoCivil", "26"}, {"JefeInmediato", "27"}, {"Incentivo", "28"},
        {"CorreoElectronico", "29"}, {"CuentaBancoLAFISE", "30"}, {"CuentaBancoBANPRO", "31"}, {"CuentaBancoBDF", "32"}}

    ''' <summary>
    ''' Crea o actualiza empleados (catálogo inicial). Las columnas se buscan por encabezado (Codigo, Nombre1, Nombre2, Apellido1,
    ''' Apellido2, Direccion, Nacionalidad, Sexo, FechaIngreso, Cargo, Departamento, RUC, INSS, Cedula, Hijos, TipoNomina,
    ''' GrupoNomina, SueldoPeriodo, TarifaHoraria, CuentaBanco, Turnos, FechaNacimiento, Numerocelular, CelularEmergencia,
    ''' Profesion, EstadoCivil, JefeInmediato, Incentivo, CorreoElectronico, CuentaBancoLAFISE/BANPRO/BDF) y, si la hoja no
    ''' trae los encabezados Codigo y Nombre1, por la posición de la hoja de la V.5.0. Cargo, departamento y grupo que no existen se crean; el tipo de nómina debe existir.
    ''' Los empleados nuevos reciben histórico (fechas de contrato y, con <paramref name="sueldoActualEnCordobas"/>, el sueldo
    ''' actual = última tasa de cambio x sueldo del período). No se crea el registro del reloj (Userinfo) ni el horario.
    ''' </summary>
    Public Function ImportarEmpleados(filas As DataTable, dolarizado As Boolean, sueldoActualEnCordobas As Boolean) As ResultadoImportacion

        Dim res As New ResultadoImportacion
        Dim indices As Dictionary(Of String, Integer) = IndicesEmpleado(filas)

        EnTransaccion(
            Sub(cn, tr)

                Dim tasa As Double = 1

                If sueldoActualEnCordobas Then
                    Dim t As Object = EscalarEn(cn, tr, "SELECT TOP 1 MontoDia FROM CambioMoneda ORDER BY FechaDia DESC")
                    If t IsNot Nothing AndAlso Not IsDBNull(t) Then tasa = Convert.ToDouble(t)
                End If

                For Each fila As DataRow In filas.Rows

                    Dim codigo As String = Celda(fila, indices, "Codigo")

                    If codigo = "" Then Continue For

                    If codigo.All(AddressOf Char.IsDigit) Then codigo = codigo.PadLeft(4, "0"c)

                    Dim tipoNomina As String = BuscarTipoNomina(cn, tr, Celda(fila, indices, "TipoNomina"))

                    If tipoNomina = "" Then
                        res.Mensajes.Add(codigo & ", tipo de nómina '" & Celda(fila, indices, "TipoNomina") & "' no existe")
                        Continue For
                    End If

                    Dim sueldo As Double = Numero(Celda(fila, indices, "SueldoPeriodo"))
                    Dim tarifa As Double = Numero(Celda(fila, indices, "TarifaHoraria"))
                    Dim hijos As Integer = Math.Max(0, Math.Min(255, CInt(Numero(Celda(fila, indices, "Hijos")))))
                    Dim incentivo As String = Celda(fila, indices, "Incentivo")

                    Dim p As New List(Of SqlParameter) From {
                        New SqlParameter("@Nombre1", Celda(fila, indices, "Nombre1")),
                        New SqlParameter("@Nombre2", Celda(fila, indices, "Nombre2")),
                        New SqlParameter("@Apellido1", Celda(fila, indices, "Apellido1")),
                        New SqlParameter("@Apellido2", Celda(fila, indices, "Apellido2")),
                        New SqlParameter("@Direccion", Celda(fila, indices, "Direccion")),
                        New SqlParameter("@Cedula", Celda(fila, indices, "Cedula")),
                        New SqlParameter("@Sexo", Celda(fila, indices, "Sexo")),
                        New SqlParameter("@Inss", Celda(fila, indices, "INSS")),
                        New SqlParameter("@Ruc", Celda(fila, indices, "RUC")),
                        New SqlParameter("@Nacionalidad", Celda(fila, indices, "Nacionalidad")),
                        New SqlParameter("@Depto", CodigoCatalogo(cn, tr, "Departamento", "CodDepartamento", "Departamento", Celda(fila, indices, "Departamento"))),
                        New SqlParameter("@Cargo", CodigoCatalogo(cn, tr, "Cargo", "CodCargo", "Cargo", Celda(fila, indices, "Cargo"))),
                        New SqlParameter("@Grupo", CodigoCatalogo(cn, tr, "Grupo", "CodGrupo", "Grupo", Celda(fila, indices, "GrupoNomina"))),
                        New SqlParameter("@TipoNomina", tipoNomina),
                        New SqlParameter("@Hijos", hijos),
                        New SqlParameter("@Cuenta", Celda(fila, indices, "CuentaBanco")),
                        New SqlParameter("@Lafise", Celda(fila, indices, "CuentaBancoLAFISE")),
                        New SqlParameter("@Banpro", Celda(fila, indices, "CuentaBancoBANPRO")),
                        New SqlParameter("@Bdf", Celda(fila, indices, "CuentaBancoBDF")),
                        New SqlParameter("@Sueldo", sueldo),
                        New SqlParameter("@Tarifa", tarifa),
                        New SqlParameter("@Dolarizado", dolarizado),
                        New SqlParameter("@SueldoActualBasico", sueldoActualEnCordobas)}

                    Dim existente As Object = EscalarEn(cn, tr, "SELECT TOP 1 CodEmpleado FROM Empleado WHERE CodEmpleado1 = @Cod",
                                                        New SqlParameter("@Cod", codigo))

                    If existente IsNot Nothing AndAlso Not IsDBNull(existente) Then

                        p.Add(New SqlParameter("@Id", SqlDbType.Decimal) With {.Value = Convert.ToDecimal(existente)})

                        EjecutarEn(cn, tr,
                            "UPDATE Empleado SET Nombre1 = @Nombre1, Nombre2 = @Nombre2, Apellido1 = @Apellido1, Apellido2 = @Apellido2, " &
                            "Direccion = @Direccion, NumCedula = @Cedula, Sexo = @Sexo, NumeroInss = @Inss, NumeroRuc = @Ruc, " &
                            "Nacionalidad = @Nacionalidad, CodDepartamento = ISNULL(@Depto, CodDepartamento), CodCargo = ISNULL(@Cargo, CodCargo), " &
                            "CodGrupo = @Grupo, Sindicalista = 'No', CodTipoNomina = @TipoNomina, NumHijos = @Hijos, CuentaBanco = @Cuenta, " &
                            "CuentaBancoLAFISE = @Lafise, CuentaBancoBANPRO = @Banpro, CuentaBancoBDF = @Bdf, SueldoPeriodo = @Sueldo, " &
                            "TarifaHoraria = @Tarifa, Dolarizado = @Dolarizado, SueldoActualBasico = @SueldoActualBasico " &
                            "WHERE CodEmpleado = @Id", p.ToArray())

                        res.Mensajes.Add(codigo & ", empleado actualizado")

                    Else

                        p.Add(New SqlParameter("@Codigo", codigo))
                        p.Add(New SqlParameter("@Turno", Celda(fila, indices, "Turnos")))
                        p.Add(New SqlParameter("@Celular", Celda(fila, indices, "Numerocelular")))
                        p.Add(New SqlParameter("@CelularEmerg", Celda(fila, indices, "CelularEmergencia")))
                        p.Add(New SqlParameter("@Profesion", Celda(fila, indices, "Profesion")))
                        p.Add(New SqlParameter("@EstadoCivil", Celda(fila, indices, "EstadoCivil")))
                        p.Add(New SqlParameter("@Jefe", Celda(fila, indices, "JefeInmediato")))
                        p.Add(New SqlParameter("@Incentivo", If(incentivo = "", "0", incentivo)))
                        p.Add(New SqlParameter("@Correo", Celda(fila, indices, "CorreoElectronico")))

                        Dim nuevoId As Decimal = Convert.ToDecimal(EscalarEn(cn, tr,
                            "INSERT INTO Empleado (CodEmpleado1, Nombre1, Nombre2, Apellido1, Apellido2, Direccion, NumCedula, Sexo, NumeroInss, " &
                            "NumeroRuc, Nacionalidad, CodDepartamento, CodCargo, CodGrupo, Sindicalista, CodTipoNomina, NumHijos, CuentaBanco, " &
                            "CuentaBancoLAFISE, CuentaBancoBANPRO, CuentaBancoBDF, Turno, Numerocelular, CelularEmergencia, Profesion, EstadoCivil, " &
                            "JefeInmediato, Incentivo, SueldoPeriodo, TarifaHoraria, PorcentajeComision, Dolarizado, PorcientoIncentivo, SalarioMinimo, " &
                            "ExentoInss, ExentoIr, PagoInssPatronal, Correo_Electronico, SueldoActualBasico, Activo, Ausente, SumarSubsidio) " &
                            "OUTPUT INSERTED.CodEmpleado " &
                            "VALUES (@Codigo, @Nombre1, @Nombre2, @Apellido1, @Apellido2, @Direccion, @Cedula, @Sexo, @Inss, @Ruc, @Nacionalidad, " &
                            "@Depto, @Cargo, @Grupo, 'No', @TipoNomina, @Hijos, @Cuenta, @Lafise, @Banpro, @Bdf, @Turno, @Celular, @CelularEmerg, " &
                            "@Profesion, @EstadoCivil, @Jefe, @Incentivo, @Sueldo, @Tarifa, 0, @Dolarizado, 20, 0, 0, 0, 1, @Correo, " &
                            "@SueldoActualBasico, 1, 0, 0)", p.ToArray()))

                        EjecutarEn(cn, tr,
                            "INSERT INTO Historico (Id, Codempleado, FechaNacimiento, FechaContrato, FechaContratoVac, SueldoActual) " &
                            "SELECT ISNULL(MAX(Id), 0) + 1, @Emp, @Nac, @Fecha, @Fecha, @SueldoActual FROM Historico WITH (TABLOCKX)",
                            New SqlParameter("@Emp", SqlDbType.Decimal) With {.Value = nuevoId},
                            New SqlParameter("@Nac", FechaONulo(Celda(fila, indices, "FechaNacimiento"))),
                            New SqlParameter("@Fecha", FechaODefecto(Celda(fila, indices, "FechaIngreso"), Date.Today)),
                            New SqlParameter("@SueldoActual", If(sueldoActualEnCordobas, Math.Round(sueldo * tasa, 2), 0.0)))

                        res.Mensajes.Add(codigo & ", empleado creado")

                    End If

                    res.Importadas += 1

                Next

            End Sub)

        Return res

    End Function

    Private Shared Function IndicesEmpleado(filas As DataTable) As Dictionary(Of String, Integer)

        Dim porNombre As New Dictionary(Of String, Integer)

        For i As Integer = 0 To filas.Columns.Count - 1
            Dim clave As String = Normalizar(filas.Columns(i).ColumnName)
            If Not porNombre.ContainsKey(clave) Then porNombre.Add(clave, i)
        Next

        ' Con encabezados reconocibles solo se usan los que están; sin ellos se asume el orden de la hoja de la V.5.0.
        Dim porPosicion As Boolean = Not (porNombre.ContainsKey("codigo") AndAlso porNombre.ContainsKey("nombre1"))

        Dim res As New Dictionary(Of String, Integer)

        For k As Integer = 0 To ColumnasEmpleado.GetLength(0) - 1

            Dim nombre As String = ColumnasEmpleado(k, 0)
            Dim posicion As Integer = Integer.Parse(ColumnasEmpleado(k, 1))
            Dim indice As Integer

            If porNombre.TryGetValue(Normalizar(nombre), indice) Then
                res(nombre) = indice
            ElseIf porPosicion AndAlso posicion < filas.Columns.Count Then
                res(nombre) = posicion
            End If

        Next

        Return res

    End Function

    Private Shared Function Normalizar(encabezado As String) As String
        Return New String(encabezado.Where(AddressOf Char.IsLetterOrDigit).ToArray()).ToLowerInvariant()
    End Function

    Private Shared Function Celda(fila As DataRow, indices As Dictionary(Of String, Integer), nombre As String) As String

        Dim i As Integer

        If Not indices.TryGetValue(nombre, i) Then Return ""

        Return Texto(fila, i)

    End Function

    Private Shared Function Numero(texto As String) As Double

        Dim v As Double

        If Double.TryParse(texto, Globalization.NumberStyles.Any, Globalization.CultureInfo.CurrentCulture, v) Then Return v
        If Double.TryParse(texto, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, v) Then Return v

        Return 0

    End Function

    ''' <summary>Fecha válida para smalldatetime o DBNull.</summary>
    Private Shared Function FechaONulo(texto As String) As Object

        Dim d As Date

        If Date.TryParse(texto, d) AndAlso d.Year >= 1900 AndAlso d.Year <= 2078 Then Return d

        Return DBNull.Value

    End Function

    Private Shared Function FechaODefecto(texto As String, defecto As Date) As Date

        Dim d As Object = FechaONulo(texto)

        Return If(IsDBNull(d), defecto, CDate(d))

    End Function

    ''' <summary>Código del tipo de nómina cuyo nombre (o código) coincide; "" si no existe.</summary>
    Private Shared Function BuscarTipoNomina(cn As SqlConnection, tr As SqlTransaction, nombre As String) As String

        If nombre = "" Then Return ""

        Dim r As Object = EscalarEn(cn, tr,
            "SELECT TOP 1 CodTipoNomina FROM TipoNomina WHERE Nomina = @N OR CodTipoNomina = @N", New SqlParameter("@N", nombre))

        Return If(r Is Nothing OrElse IsDBNull(r), "", Convert.ToString(r))

    End Function

    ''' <summary>
    ''' Código del registro con ese nombre; si no existe lo crea (máximo numérico + 1, con el ancho de los códigos existentes).
    ''' Sin nombre devuelve NULL. Tabla y columnas son constantes del código, no entrada del usuario.
    ''' </summary>
    Private Shared Function CodigoCatalogo(cn As SqlConnection, tr As SqlTransaction, tabla As String, columnaCodigo As String,
                                           columnaNombre As String, nombre As String) As Object

        If nombre = "" Then Return DBNull.Value

        Dim r As Object = EscalarEn(cn, tr, "SELECT TOP 1 " & columnaCodigo & " FROM " & tabla & " WHERE " & columnaNombre & " = @N",
                                    New SqlParameter("@N", nombre))

        If r IsNot Nothing AndAlso Not IsDBNull(r) Then Return Convert.ToString(r)

        Dim maximo As Object = EscalarEn(cn, tr,
            "SELECT MAX(CASE WHEN " & columnaCodigo & " NOT LIKE '%[^0-9]%' AND LEN(" & columnaCodigo & ") <= 9 " &
            "THEN CAST(" & columnaCodigo & " AS int) END) FROM " & tabla & " WITH (UPDLOCK, HOLDLOCK)")
        Dim ancho As Object = EscalarEn(cn, tr, "SELECT MAX(LEN(" & columnaCodigo & ")) FROM " & tabla)

        Dim nuevo As String = (If(maximo Is Nothing OrElse IsDBNull(maximo), 0, Convert.ToInt32(maximo)) + 1).ToString().PadLeft(
            If(ancho Is Nothing OrElse IsDBNull(ancho), 2, Convert.ToInt32(ancho)), "0"c)

        EjecutarEn(cn, tr, "INSERT INTO " & tabla & " (" & columnaCodigo & ", " & columnaNombre & ") VALUES (@C, @N)",
                   New SqlParameter("@C", nuevo), New SqlParameter("@N", nombre))

        Return nuevo

    End Function

#End Region

#Region "SOLICITUD TOTAL"

    Private Class FilaSolicitudTotal
        Public Cod As String
        Public Nombre As String
        Public Ini, Fin, Aplica As Date
        Public HorasExtra, Tarde, Vac, VacPag, Aus, Subsidio, SubRiesgo As Double
        Public Restar3 As Boolean
        Public CodAuto As Decimal
        Public NumNomina As Integer
        Public HorasDia As Double
    End Class

    Private Shared ReadOnly ColumnasSolicitudTotal() As String =
        {"CodEmpleado", "HorasExtra", "LlegadasTarde", "DiasVacaciones", "DiasVacaPagadas", "DiasAusencia", "DiasSubsidio",
         "FechaIniNomina", "FechaFinNomina", "FechaAplica"}

    ''' <summary>
    ''' Asistencia total del período (V.5.0 pestaña "Solicitud Total"). Columnas por encabezado: CodEmpleado, HorasExtra, LlegadasTarde (min),
    ''' DiasVacaciones, DiasVacaPagadas, DiasAusencia, DiasSubsidio, FechaIniNomina, FechaFinNomina, FechaAplica y, opcionales, Nombres,
    ''' DiasSubsidioRiesgo, Rtar3DiasSubsidio. Las filas del mismo empleado y fechas se acumulan. Si algo no valida no se importa nada
    ''' (los errores quedan en <see cref="ResultadoImportacion.Mensajes"/>). Cada cantidad > 0 genera su solicitud (o actualiza la que ya
    ''' exista con el mismo empleado, tipo y fecha de inicio) con sus días en DescuentoDiasVacaciones; las horas extra van a la nómina activa.
    ''' </summary>
    Public Function ImportarSolicitudTotal(filas As DataTable, acumularHorasExtra As Boolean) As ResultadoImportacion

        Dim res As New ResultadoImportacion
        Dim items As List(Of FilaSolicitudTotal) = LeerSolicitudTotal(filas, res.Mensajes)

        If res.Mensajes.Count > 0 Then Return res

        EnTransaccion(
            Sub(cn, tr)

                For Each f In items

                    If Not CargarEmpleadoST(cn, tr, f) Then
                        res.Mensajes.Add("ERROR: " & f.Cod & ": el empleado no existe o está inactivo")
                        Continue For
                    End If

                    ValidarSolicitudTotal(f, res.Mensajes)

                Next

                If res.Mensajes.Count > 0 Then Return

                Dim siguiente As Integer = SiguienteNumeroSolicitud(cn, tr)

                For Each f In items

                    If f.HorasExtra > 0 Then
                        GrabarHorasExtraST(cn, tr, f, acumularHorasExtra, res.Mensajes)
                        res.Importadas += 1
                    End If

                    res.Importadas += GrabarSolicitudST(cn, tr, f, "Vacaciones", "Vacaciones", f.Vac, 0, False, siguiente, res.Mensajes)
                    res.Importadas += GrabarSolicitudST(cn, tr, f, "Vacaciones Pagadas", "Vacaciones Pagadas", f.VacPag, 0, False, siguiente, res.Mensajes)
                    res.Importadas += GrabarSolicitudST(cn, tr, f, "Ausente", "Ausente", f.Aus, 0, False, siguiente, res.Mensajes)
                    res.Importadas += GrabarSolicitudST(cn, tr, f, "Subsidio", "Accidente Comun", f.Subsidio, 0, False, siguiente, res.Mensajes)
                    ' Con subsidio común y de riesgo, el segundo empieza al terminar el primero.
                    res.Importadas += GrabarSolicitudST(cn, tr, f, "Subsidio", "Riesgo Laboral", f.SubRiesgo, CInt(f.Subsidio), False, siguiente, res.Mensajes)
                    res.Importadas += GrabarSolicitudST(cn, tr, f, "Llegada Tarde", "Ausente", f.Tarde, 0, True, siguiente, res.Mensajes)

                Next

                EjecutarEn(cn, tr, "UPDATE Consecutivos SET Solicitud = @N WHERE ISNULL(Solicitud, 0) < @N", New SqlParameter("@N", siguiente - 1))

            End Sub)

        Return res

    End Function

    Private Shared Function LeerSolicitudTotal(filas As DataTable, errores As List(Of String)) As List(Of FilaSolicitudTotal)

        Dim items As New List(Of FilaSolicitudTotal)

        For Each c In ColumnasSolicitudTotal
            If Not filas.Columns.Contains(c) Then errores.Add("ERROR: El archivo no tiene la columna " & c)
        Next

        If errores.Count > 0 Then Return items

        Dim numFila As Integer = 1

        For Each fila As DataRow In filas.Rows

            numFila += 1

            Dim cod As String = Convert.ToString(fila("CodEmpleado")).Trim().Replace("'", "")

            If cod = "" Then Continue For

            Dim ini, fin, aplica As Date

            If Not (Date.TryParse(Convert.ToString(fila("FechaIniNomina")), ini) AndAlso
                    Date.TryParse(Convert.ToString(fila("FechaFinNomina")), fin) AndAlso
                    Date.TryParse(Convert.ToString(fila("FechaAplica")), aplica)) Then
                errores.Add("ERROR: Fila " & numFila & " (" & cod & "): fechas inválidas o vacías")
                Continue For
            End If

            Dim f As FilaSolicitudTotal = items.FirstOrDefault(Function(x) x.Cod = cod AndAlso x.Ini = ini.Date AndAlso x.Fin = fin.Date AndAlso x.Aplica = aplica.Date)

            If f Is Nothing Then
                f = New FilaSolicitudTotal With {.Cod = cod, .Nombre = "", .Ini = ini.Date, .Fin = fin.Date, .Aplica = aplica.Date}
                items.Add(f)
            End If

            If filas.Columns.Contains("Nombres") AndAlso Not IsDBNull(fila("Nombres")) Then f.Nombre = Convert.ToString(fila("Nombres")).Trim()

            f.HorasExtra += Numero(fila, "HorasExtra")
            f.Tarde += Numero(fila, "LlegadasTarde")
            f.Vac += Numero(fila, "DiasVacaciones")
            f.VacPag += Numero(fila, "DiasVacaPagadas")
            f.Aus += Numero(fila, "DiasAusencia")
            f.Subsidio += Numero(fila, "DiasSubsidio")

            If filas.Columns.Contains("DiasSubsidioRiesgo") Then f.SubRiesgo += Numero(fila, "DiasSubsidioRiesgo")
            If filas.Columns.Contains("Rtar3DiasSubsidio") AndAlso EsVerdadero(fila("Rtar3DiasSubsidio")) Then f.Restar3 = True

        Next

        If items.Count = 0 Then errores.Add("ERROR: El archivo no tiene registros")

        Return items

    End Function

    Private Shared Function Numero(fila As DataRow, columna As String) As Double

        Dim v As Double

        If IsDBNull(fila(columna)) OrElse Not Double.TryParse(Convert.ToString(fila(columna)), v) Then Return 0

        Return v

    End Function

    Private Shared Function EsVerdadero(valor As Object) As Boolean

        If IsDBNull(valor) Then Return False

        Dim t As String = Convert.ToString(valor).Trim().ToUpperInvariant()
        Dim n As Double

        If t = "TRUE" OrElse t = "VERDADERO" OrElse t = "SI" OrElse t = "SÍ" Then Return True

        Return Double.TryParse(t, n) AndAlso n <> 0

    End Function

    Private Shared Function CargarEmpleadoST(cn As SqlConnection, tr As SqlTransaction, f As FilaSolicitudTotal) As Boolean

        Dim emp As DataTable = ConsultaEn(cn, tr,
            "SELECT e.CodEmpleado, e.CodTipoNomina, t.Horas, " &
            "(SELECT TOP 1 NumNomina FROM Nomina n WHERE n.Activa = 1 AND n.CodTipoNomina = e.CodTipoNomina ORDER BY n.NumNomina DESC) AS NumNomina " &
            "FROM Empleado e LEFT JOIN TipoNomina t ON t.CodTipoNomina = e.CodTipoNomina " &
            "WHERE e.CodEmpleado1 = @Cod AND e.Activo = 1", New SqlParameter("@Cod", f.Cod))

        If emp.Rows.Count = 0 Then Return False

        f.CodAuto = Convert.ToDecimal(emp.Rows(0)("CodEmpleado"))
        f.HorasDia = If(IsDBNull(emp.Rows(0)("Horas")), 0, Convert.ToDouble(emp.Rows(0)("Horas")))
        f.NumNomina = If(IsDBNull(emp.Rows(0)("NumNomina")), 0, Convert.ToInt32(emp.Rows(0)("NumNomina")))

        Return True

    End Function

    ''' <summary>Días válidos: 0, una fracción de día (&lt; 1, se guarda por horas) o un entero.</summary>
    Private Shared Function DiasValidos(dias As Double) As Boolean
        Return dias >= 0 AndAlso (dias < 1 OrElse dias = Math.Floor(dias))
    End Function

    Private Shared Sub ValidarSolicitudTotal(f As FilaSolicitudTotal, errores As List(Of String))

        Dim c As String = "ERROR: " & f.Cod & ": "

        If f.Fin < f.Ini Then
            errores.Add(c & "la fecha fin de la nómina es menor que la fecha inicio")
        ElseIf (f.Fin - f.Ini).Days + 1 > 15 Then
            errores.Add(c & "el período de la nómina no puede superar 15 días")
        End If

        If f.Aplica < f.Ini OrElse f.Aplica > f.Fin Then
            errores.Add(c & "la fecha de aplicación " & f.Aplica.ToString("dd/MM/yyyy") & " debe estar dentro del período de la nómina")
        End If

        If f.HorasExtra < 0 OrElse f.Tarde < 0 OrElse f.Vac < 0 OrElse f.VacPag < 0 OrElse f.Aus < 0 OrElse f.Subsidio < 0 OrElse f.SubRiesgo < 0 Then
            errores.Add(c & "no se permiten valores negativos")
        End If

        If Not (DiasValidos(f.Vac) AndAlso DiasValidos(f.VacPag) AndAlso DiasValidos(f.Aus) AndAlso DiasValidos(f.Subsidio) AndAlso DiasValidos(f.SubRiesgo)) Then
            errores.Add(c & "los días deben ser enteros (o menores a 1 si es por horas)")
        End If

        If f.Tarde > 0 Then
            If f.HorasDia <= 0 Then
                errores.Add(c & "no se encontraron las horas laborales del empleado para convertir las llegadas tarde")
            ElseIf f.Tarde / 60 > f.HorasDia Then
                errores.Add(c & "las llegadas tarde (" & f.Tarde & " min) superan las horas laborales del día")
            End If
        End If

        Dim deducir As Double = f.Aus + f.Subsidio + f.SubRiesgo + If(f.HorasDia > 0, (f.Tarde / 60) / f.HorasDia, 0)

        If deducir > 15 Then errores.Add(c & "la suma de días a deducir (ausencia, llegadas tarde, subsidio) supera 15 días")

        If f.Restar3 AndAlso (f.Subsidio + f.SubRiesgo) > 0 AndAlso (f.Subsidio + f.SubRiesgo) < 3 Then
            errores.Add(c & "restar 3 días de subsidio, pero el empleado solo tiene " & (f.Subsidio + f.SubRiesgo) & " días")
        End If

        If f.HorasExtra > 0 AndAlso f.NumNomina = 0 Then errores.Add(c & "no hay nómina activa para registrar las horas extra")

    End Sub

    Private Shared Function SiguienteNumeroSolicitud(cn As SqlConnection, tr As SqlTransaction) As Integer

        Dim ultimo As Object = EscalarEn(cn, tr,
            "SELECT MAX(CASE WHEN NumeroSolicitud NOT LIKE '%[^0-9]%' AND LEN(NumeroSolicitud) <= 9 " &
            "THEN CAST(NumeroSolicitud AS int) END) FROM SolicitudVacaciones WITH (TABLOCKX)")

        Return If(ultimo Is Nothing OrElse IsDBNull(ultimo), 0, Convert.ToInt32(ultimo)) + 1

    End Function

    Private Shared Sub GrabarHorasExtraST(cn As SqlConnection, tr As SqlTransaction, f As FilaSolicitudTotal, acumular As Boolean, mensajes As List(Of String))

        Dim existe As Object = EscalarEn(cn, tr, "SELECT COUNT(*) FROM HorasExtras WHERE CodEmpleado = @Emp AND NumNomina = @Nom",
                                         New SqlParameter("@Emp", f.CodAuto), New SqlParameter("@Nom", f.NumNomina))

        If Convert.ToInt32(existe) > 0 Then

            EjecutarEn(cn, tr,
                If(acumular, "UPDATE HorasExtras SET CantHoras = CantHoras + @H WHERE CodEmpleado = @Emp AND NumNomina = @Nom",
                             "UPDATE HorasExtras SET CantHoras = @H, Pagada = 0 WHERE CodEmpleado = @Emp AND NumNomina = @Nom"),
                New SqlParameter("@H", f.HorasExtra), New SqlParameter("@Emp", f.CodAuto), New SqlParameter("@Nom", f.NumNomina))
            mensajes.Add(f.Cod & ", horas extra actualizadas (" & f.HorasExtra & ")")

        Else

            EjecutarEn(cn, tr,
                "INSERT INTO HorasExtras (Id, CodEmpleado, NumNomina, CantHoras, Pagada) " &
                "VALUES ((SELECT ISNULL(MAX(Id), 0) + 1 FROM HorasExtras WITH (TABLOCKX)), @Emp, @Nom, @H, 0)",
                New SqlParameter("@Emp", f.CodAuto), New SqlParameter("@Nom", f.NumNomina), New SqlParameter("@H", f.HorasExtra))
            mensajes.Add(f.Cod & ", horas extra agregadas (" & f.HorasExtra & ")")

        End If

    End Sub

    ''' <summary>Cantidad = días, o minutos si <paramref name="esMinutos"/>. Devuelve 1 si grabó o actualizó la solicitud, 0 si no había cantidad.</summary>
    Private Shared Function GrabarSolicitudST(cn As SqlConnection, tr As SqlTransaction, f As FilaSolicitudTotal, tipo As String, clasificacion As String,
                                              cantidad As Double, desplazar As Integer, esMinutos As Boolean, ByRef siguiente As Integer,
                                              mensajes As List(Of String)) As Integer

        If cantidad <= 0 Then Return 0

        Dim inicio As Date = f.Aplica.AddDays(desplazar)
        Dim diasSol, horasSol As Double
        Dim fin As Date

        If esMinutos Then
            diasSol = 1 : horasSol = cantidad / 60 : fin = inicio
        ElseIf cantidad < 1 Then
            diasSol = 1 : horasSol = cantidad * f.HorasDia : fin = inicio
        Else
            diasSol = cantidad : horasSol = 0 : fin = inicio.AddDays(cantidad - 1)
        End If

        If horasSol > f.HorasDia Then
            mensajes.Add(f.Cod & ", " & tipo & ": según la nómina el empleado labora al día únicamente " & f.HorasDia & " horas")
            Return 0
        End If

        Dim diasDisfrutar As Double = If(horasSol = 0 AndAlso diasSol >= 1, diasSol, If(diasSol = 1 AndAlso horasSol > 0 AndAlso f.HorasDia > 0, horasSol / f.HorasDia, diasSol))

        Dim existente As Object = EscalarEn(cn, tr,
            "SELECT TOP 1 NumeroSolicitud FROM SolicitudVacaciones WHERE CodigoEmpleado = @Cod AND TipoSolicitud = @Tipo " &
            "AND FechaInicio >= @D AND FechaInicio < DATEADD(day, 1, @D) ORDER BY NumeroSolicitud",
            New SqlParameter("@Cod", f.Cod), New SqlParameter("@Tipo", tipo), New SqlParameter("@D", inicio))

        Dim numero As String

        If existente Is Nothing OrElse IsDBNull(existente) Then

            numero = siguiente.ToString("00000")
            siguiente += 1

            EjecutarEn(cn, tr,
                "INSERT INTO SolicitudVacaciones (FechaSolicitud, NumeroSolicitud, TipoSolicitud, CodigoEmpleado, DiasVacaciones, DiasDisfrutados, " &
                "FechaInicio, FechaFin, DiasDisfrutar, Clasificacion, Observaciones, Anulado, Pagado) " &
                "VALUES (@FechaSol, @Num, @Tipo, @Cod, @Dias, @Dias, @Ini, @Fin, @Disf, @Clas, 'Importado', 0, 0)",
                New SqlParameter("@FechaSol", Date.Today), New SqlParameter("@Num", numero), New SqlParameter("@Tipo", tipo),
                New SqlParameter("@Cod", f.Cod), New SqlParameter("@Dias", diasSol), New SqlParameter("@Ini", inicio),
                New SqlParameter("@Fin", fin), New SqlParameter("@Disf", diasDisfrutar), New SqlParameter("@Clas", clasificacion))

        Else

            numero = Convert.ToString(existente)

            Dim procesada As Object = EscalarEn(cn, tr, "SELECT NumNomina FROM SolicitudVacaciones WHERE NumeroSolicitud = @Num", New SqlParameter("@Num", numero))

            If Not IsDBNull(procesada) AndAlso procesada IsNot Nothing AndAlso Convert.ToInt32(procesada) <> 0 Then
                mensajes.Add(f.Cod & ", " & tipo & ": la solicitud " & numero & " ya fue procesada en la nómina " & procesada & " y no se puede modificar")
                Return 0
            End If

            EjecutarEn(cn, tr,
                "UPDATE SolicitudVacaciones SET DiasVacaciones = @Dias, DiasDisfrutados = @Dias, FechaInicio = @Ini, FechaFin = @Fin, " &
                "DiasDisfrutar = @Disf, Clasificacion = @Clas, Observaciones = 'Importado' WHERE NumeroSolicitud = @Num",
                New SqlParameter("@Dias", diasSol), New SqlParameter("@Ini", inicio), New SqlParameter("@Fin", fin),
                New SqlParameter("@Disf", diasDisfrutar), New SqlParameter("@Clas", clasificacion), New SqlParameter("@Num", numero))

        End If

        CsSolicitudDAL.GrabarDescuentosEn(cn, tr, f.Cod, tipo, numero, inicio, fin, If(tipo = "Vacaciones Pagadas", If(horasSol = 0 AndAlso diasSol >= 1, (fin - inicio).Days + 1, diasDisfrutar), 0))

        mensajes.Add(f.Cod & ", " & tipo & " " & If(esMinutos, cantidad & " min", cantidad & " días") & If(existente Is Nothing OrElse IsDBNull(existente), ", solicitud agregada", ", solicitud actualizada"))

        Return 1

    End Function

#End Region

#Region "LIMPIEZA"

    ''' <summary>Borra las solicitudes del tipo de los empleados que figuran en el detalle de la nómina activa indicada, con sus días.</summary>
    Public Function EliminarSolicitudesPendientes(numNomina As Integer, tipoSolicitud As String) As Integer

        Dim borradas As Integer = 0

        EnTransaccion(
            Sub(cn, tr)

                Const filtro As String =
                    "FROM SolicitudVacaciones s WHERE s.TipoSolicitud = @Tipo AND s.CodigoEmpleado IN " &
                    "(SELECT e.CodEmpleado1 FROM DetalleNomina d INNER JOIN Empleado e ON e.CodEmpleado = d.CodEmpleado " &
                    "INNER JOIN Nomina n ON n.NumNomina = d.NumNomina WHERE n.Activa = 1 AND d.NumNomina = @Nom)"

                EjecutarEn(cn, tr, "DELETE FROM DescuentoDiasVacaciones WHERE NumeroSolicitud IN (SELECT s.NumeroSolicitud " & filtro & ")",
                           New SqlParameter("@Tipo", tipoSolicitud), New SqlParameter("@Nom", numNomina))

                borradas = EjecutarEn(cn, tr, "DELETE s " & filtro,
                                      New SqlParameter("@Tipo", tipoSolicitud), New SqlParameter("@Nom", numNomina))

            End Sub)

        Return borradas

    End Function

    ''' <summary>
    ''' Borra TODOS los empleados con sus horas extra, deducciones, incentivos y solicitudes (V.5.0 "Eliminar Empleados"), en una transacción.
    ''' Si algún otro dato depende de un empleado (llave foránea), no se borra nada y se lanza el error. Devuelve los empleados borrados.
    ''' </summary>
    Public Function EliminarTodosLosEmpleados() As Integer

        Dim borrados As Integer = 0

        EnTransaccion(
            Sub(cn, tr)

                EjecutarEn(cn, tr, "DELETE FROM HorasExtras")
                EjecutarEn(cn, tr, "DELETE FROM DetalleDeduccion")
                EjecutarEn(cn, tr, "DELETE FROM Deduccion")
                EjecutarEn(cn, tr, "DELETE FROM DetalleIncentivo")
                EjecutarEn(cn, tr, "DELETE FROM Incentivo")
                EjecutarEn(cn, tr, "DELETE FROM DescuentoDiasVacaciones")
                EjecutarEn(cn, tr, "DELETE FROM SolicitudVacaciones")
                borrados = EjecutarEn(cn, tr, "DELETE FROM Empleado")

            End Sub)

        Return borrados

    End Function

    ''' <summary>Borra las horas extra sin pagar de la nómina. Devuelve las filas borradas.</summary>
    Public Function EliminarHorasExtraPendientes(numNomina As Integer) As Integer
        Return Ejecutar("DELETE FROM HorasExtras WHERE Pagada = 0 AND NumNomina = @Nom",
                        New SqlParameter("@Nom", numNomina))
    End Function

    ''' <summary>Borra las deducciones sin pagar del tipo en la nómina, con su detalle.</summary>
    Public Function EliminarDeduccionesPendientes(numNomina As Integer, codTipoDeduccion As String) As Integer

        Dim borradas As Integer = 0

        EnTransaccion(
            Sub(cn, tr)

                Dim p() As SqlParameter = {New SqlParameter("@Nom", numNomina), New SqlParameter("@Tipo", codTipoDeduccion)}

                EjecutarEn(cn, tr,
                    "DELETE dd FROM DetalleDeduccion dd INNER JOIN Deduccion d ON d.NumDeduccion = dd.NumDeduccion " &
                    "WHERE d.Pagado = 0 AND d.NUmNomina = @Nom AND d.CodTipoDeduccion = @Tipo AND dd.Pagado = 0",
                    New SqlParameter("@Nom", numNomina), New SqlParameter("@Tipo", codTipoDeduccion))

                borradas = EjecutarEn(cn, tr,
                    "DELETE FROM Deduccion WHERE Pagado = 0 AND NUmNomina = @Nom AND CodTipoDeduccion = @Tipo", p)

            End Sub)

        Return borradas

    End Function

    ''' <summary>Borra los incentivos sin pagar del tipo, con su detalle sin pagar.</summary>
    Public Function EliminarIncentivosPendientes(codTipoIncentivo As String) As Integer

        Dim borrados As Integer = 0

        EnTransaccion(
            Sub(cn, tr)

                EjecutarEn(cn, tr,
                    "DELETE di FROM DetalleIncentivo di INNER JOIN Incentivo i ON i.NumIncentivo = di.NumIncentivo " &
                    "WHERE i.Pagado = 0 AND i.CodTipoIncentivo = @Tipo AND di.Pagado = 0",
                    New SqlParameter("@Tipo", codTipoIncentivo))

                borrados = EjecutarEn(cn, tr,
                    "DELETE FROM Incentivo WHERE Pagado = 0 AND CodTipoIncentivo = @Tipo2",
                    New SqlParameter("@Tipo2", codTipoIncentivo))

            End Sub)

        Return borrados

    End Function

#End Region

    ''' <summary>Devuelve "" si el empleado está activo y su tipo de nómina tiene nómina activa; si no, el motivo.</summary>
    Private Shared Function BuscarEmpleadoYNomina(cn As SqlConnection, tr As SqlTransaction, codigo As String,
                                                  ByRef emp As Decimal, ByRef numNomina As Integer) As String

        Dim dt As New DataTable

        Using cmd As New SqlCommand(
            "SELECT TOP 1 e.CodEmpleado, (SELECT TOP 1 NumNomina FROM Nomina n WHERE n.CodTipoNomina = e.CodTipoNomina AND n.Activa = 1 ORDER BY NumNomina DESC) AS NumNomina " &
            "FROM Empleado e WHERE e.CodEmpleado1 = @Cod AND e.Activo = 1", cn, tr)
            cmd.Parameters.AddWithValue("@Cod", codigo)
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        If dt.Rows.Count = 0 Then Return "inactivo o no existe"

        If IsDBNull(dt.Rows(0)("NumNomina")) Then Return "no hay nómina activa para su tipo de nómina"

        emp = Convert.ToDecimal(dt.Rows(0)("CodEmpleado"))
        numNomina = Convert.ToInt32(dt.Rows(0)("NumNomina"))

        Return ""

    End Function

    Private Shared Function Texto(fila As DataRow, posicion As Integer) As String

        If posicion >= fila.Table.Columns.Count OrElse IsDBNull(fila(posicion)) Then Return ""

        Return Convert.ToString(fila(posicion)).Trim()

    End Function

End Class
