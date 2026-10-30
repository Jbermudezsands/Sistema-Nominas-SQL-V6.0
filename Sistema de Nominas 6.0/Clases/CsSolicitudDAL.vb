Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Solicitudes de vacaciones, ausencias, subsidios, feriados, etc. (tabla SolicitudVacaciones), port de Solicitudes.bas (V.5.0).
''' Grabar regenera los días de la solicitud en DescuentoDiasVacaciones. Las solicitudes procesadas en una nómina
''' (NumNomina distinto de 0) no se modifican ni se eliminan. Los errores de validación salen como InvalidOperationException.
''' </summary>
Public Class CsSolicitudDAL
    Inherits CsDALBase

#Region "CATALOGOS"

    Public Function ListarEmpleados() As DataTable
        Return ConsultarTabla(
            "SELECT CodEmpleado1, " &
            "LTRIM(RTRIM(ISNULL(Nombre1,'') + ' ' + ISNULL(Nombre2,'') + ' ' + ISNULL(Apellido1,'') + ' ' + ISNULL(Apellido2,''))) AS Nombres " &
            "FROM Empleado WHERE Activo = 1 ORDER BY CodEmpleado1")
    End Function

    ''' <summary>Nombres de nómina (TipoNomina.Nomina) para crear una solicitud a todos sus empleados activos.</summary>
    Public Function ListarNominas() As List(Of String)
        Return ConsultarTabla("SELECT DISTINCT Nomina FROM TipoNomina WHERE ISNULL(Nomina, '') <> '' ORDER BY Nomina").
            AsEnumerable().Select(Function(f) Convert.ToString(f("Nomina"))).ToList()
    End Function

    ''' <summary>Empleado activo con ese código visible; Nothing si no existe o está inactivo.</summary>
    Public Function BuscarEmpleado(codigo1 As String) As EmpleadoSolicitud

        Dim dt As DataTable = ConsultarTabla(
            "SELECT TOP 1 e.CodEmpleado1, " &
            "LTRIM(RTRIM(ISNULL(e.Nombre1,'') + ' ' + ISNULL(e.Nombre2,'') + ' ' + ISNULL(e.Apellido1,'') + ' ' + ISNULL(e.Apellido2,''))) AS Nombres, " &
            "d.Departamento, c.Cargo, h.FechaContratoVac, t.Horas " &
            "FROM Empleado e LEFT JOIN Departamento d ON e.CodDepartamento = d.CodDepartamento " &
            "LEFT JOIN Cargo c ON e.CodCargo = c.CodCargo " &
            "LEFT JOIN Historico h ON e.CodEmpleado = h.Codempleado " &
            "LEFT JOIN TipoNomina t ON e.CodTipoNomina = t.CodTipoNomina " &
            "WHERE e.CodEmpleado1 = @Cod AND e.Activo = 1 ORDER BY h.Id",
            New SqlParameter("@Cod", codigo1))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim f As DataRow = dt.Rows(0)

        Return New EmpleadoSolicitud With {
            .Codigo_Empleado1 = Convert.ToString(f("CodEmpleado1")),
            .Nombre_Completo = Convert.ToString(f("Nombres")),
            .Departamento = If(IsDBNull(f("Departamento")), "", Convert.ToString(f("Departamento"))),
            .Cargo = If(IsDBNull(f("Cargo")), "", Convert.ToString(f("Cargo"))),
            .Fecha_Contrato_Vac = If(IsDBNull(f("FechaContratoVac")), CType(Nothing, Date?), Convert.ToDateTime(f("FechaContratoVac"))),
            .Horas_Dia = If(IsDBNull(f("Horas")), 0, Convert.ToDouble(f("Horas")))}

    End Function

#End Region

#Region "LISTAR / BUSCAR"

    ''' <summary>Solicitudes del empleado, de la más reciente a la más antigua (grid de la pantalla).</summary>
    Public Function ListarPorEmpleado(codigo1 As String) As DataTable
        Return ConsultarTabla(
            "SELECT NumeroSolicitud, FechaSolicitud, TipoSolicitud, Clasificacion, FechaInicio, FechaFin, DiasDisfrutar, " &
            "ISNULL(Pagado, 0) AS Pagado, ISNULL(NumNomina, 0) AS NumNomina " &
            "FROM SolicitudVacaciones WHERE CodigoEmpleado = @Cod ORDER BY NumeroSolicitud DESC",
            New SqlParameter("@Cod", codigo1))
    End Function

    ''' <summary>La solicitud con ese número; Nothing si no existe.</summary>
    Public Function Buscar(numero As String) As SolicitudVacaciones

        Dim dt As DataTable = ConsultarTabla("SELECT * FROM SolicitudVacaciones WHERE NumeroSolicitud = @Num",
                                             New SqlParameter("@Num", numero.Trim))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim s As SolicitudVacaciones = Mapear(dt.Rows(0))

        ' Por horas: DiasDisfrutar guarda la fracción del día (V.5.0 SolicitudObtener).
        If s.Dias_Disfrutar < 1 Then
            s.Dias_Solicitud = 1
            s.Horas_Solicitud = s.Dias_Disfrutar * HorasDia(s.Codigo_Empleado)
        Else
            s.Dias_Solicitud = s.Dias_Disfrutar
            s.Horas_Solicitud = 0
        End If

        Return s

    End Function

    ''' <summary>Número que tendría la próxima solicitud (5 dígitos, máximo numérico + 1).</summary>
    Public Function ProximoNumero() As String
        Return SiguienteCodigo("SolicitudVacaciones", "NumeroSolicitud", 5)
    End Function

    ''' <summary>Horas laborales por día según el tipo de nómina del empleado (0 si no se encuentra).</summary>
    Public Function HorasDia(codigo1 As String) As Double
        Using cn As New SqlConnection(CadenaConexion)
            cn.Open()
            Return HorasDiaEn(cn, Nothing, codigo1)
        End Using
    End Function

#End Region

#Region "GRABAR / ELIMINAR"

    ''' <summary>
    ''' Crea la solicitud (número vacío o inexistente) o la modifica, y regenera sus días en DescuentoDiasVacaciones.
    ''' Devuelve el número grabado.
    ''' </summary>
    Public Function Guardar(sol As SolicitudVacaciones) As String

        Dim numero As String = ""

        EnTransaccion(Sub(cn, tr) numero = GuardarEn(cn, tr, sol))

        Return numero

    End Function

    ''' <summary>
    ''' Crea una solicitud por cada empleado activo de la nómina (TipoNomina.Nomina), en una sola transacción.
    ''' Devuelve cuántas se grabaron. Si falla una no se graba ninguna (la V.5.0 dejaba grabadas las anteriores).
    ''' </summary>
    Public Function GuardarParaNomina(sol As SolicitudVacaciones, nomina As String) As Integer

        Dim total As Integer

        EnTransaccion(
            Sub(cn, tr)

                Dim codigos As New List(Of String)

                Using cmd As New SqlCommand(
                    "SELECT e.CodEmpleado1 FROM Empleado e INNER JOIN TipoNomina t ON e.CodTipoNomina = t.CodTipoNomina " &
                    "WHERE e.Activo = 1 AND t.Nomina = @Nomina ORDER BY e.CodEmpleado1", cn, tr)
                    cmd.Parameters.AddWithValue("@Nomina", nomina)
                    Using rd As SqlDataReader = cmd.ExecuteReader()
                        While rd.Read()
                            codigos.Add(rd.GetString(0))
                        End While
                    End Using
                End Using

                If codigos.Count = 0 Then Throw New InvalidOperationException("La nómina " & nomina & " no tiene empleados activos.")

                If sol.Clasificacion = "" Then sol.Clasificacion = sol.Tipo_Solicitud

                For Each codigo As String In codigos
                    Dim copia As SolicitudVacaciones = Copiar(sol)
                    copia.Codigo_Empleado = codigo
                    copia.Numero_Solicitud = ""
                    GuardarEn(cn, tr, copia)
                    total += 1
                Next

            End Sub)

        Return total

    End Function

    ''' <summary>Elimina la solicitud y sus días. No elimina solicitudes procesadas en una nómina.</summary>
    Public Sub Eliminar(numero As String)

        EnTransaccion(
            Sub(cn, tr)

                Dim procesada As Object = EscalarEn(cn, tr, "SELECT ISNULL(NumNomina, 0) FROM SolicitudVacaciones WHERE NumeroSolicitud = @Num",
                                                    New SqlParameter("@Num", numero))

                If procesada Is Nothing Then Throw New InvalidOperationException("La solicitud " & numero & " no existe.")

                If Convert.ToDouble(procesada) <> 0 Then
                    Throw New InvalidOperationException("La solicitud " & numero & " ya fue procesada en la nómina " & Convert.ToDouble(procesada) & " y no se puede eliminar.")
                End If

                EjecutarEn(cn, tr, "DELETE FROM SolicitudVacaciones WHERE NumeroSolicitud = @Num", New SqlParameter("@Num", numero))
                EjecutarEn(cn, tr, "DELETE FROM DescuentoDiasVacaciones WHERE NumeroSolicitud = @Num", New SqlParameter("@Num", numero))

            End Sub)

    End Sub

    ''' <summary>Valor que se guarda en DiasDisfrutar: los días, o la fracción de día si la solicitud es de 1 día por horas.</summary>
    Public Shared Function DiasDisfrutar(sol As SolicitudVacaciones, horasDia As Double) As Double
        If sol.Horas_Solicitud = 0 AndAlso sol.Dias_Solicitud >= 1 Then Return sol.Dias_Solicitud
        If sol.Dias_Solicitud = 1 AndAlso sol.Horas_Solicitud > 0 AndAlso horasDia > 0 Then Return sol.Horas_Solicitud / horasDia
        Return sol.Dias_Solicitud
    End Function

    Private Shared Function GuardarEn(cn As SqlConnection, tr As SqlTransaction, sol As SolicitudVacaciones) As String

        Dim horasDia As Double = Validar(cn, tr, sol)

        If sol.Numero_Solicitud.Trim = "" Then sol.Numero_Solicitud = SiguienteNumeroEn(cn, tr)

        Dim procesada As Object = EscalarEn(cn, tr, "SELECT ISNULL(NumNomina, 0) FROM SolicitudVacaciones WITH (UPDLOCK) WHERE NumeroSolicitud = @Num",
                                            New SqlParameter("@Num", sol.Numero_Solicitud))
        Dim esNueva As Boolean = procesada Is Nothing

        If Not esNueva AndAlso Convert.ToDouble(procesada) <> 0 Then
            Throw New InvalidOperationException("La solicitud " & sol.Numero_Solicitud & " ya fue procesada en la nómina " & Convert.ToDouble(procesada) & " y no se puede modificar.")
        End If

        sol.Dias_Disfrutar = DiasDisfrutar(sol, horasDia)

        If esNueva Then

            EjecutarEn(cn, tr,
                "INSERT INTO SolicitudVacaciones (FechaSolicitud, NumeroSolicitud, CodigoEmpleado, TipoSolicitud, DiasVacaciones, DiasDisfrutados, " &
                "FechaInicio, FechaFin, Clasificacion, DiasDisfrutar, Observaciones) " &
                "VALUES (@FechaSol, @Num, @Cod, @Tipo, @DiasVac, @DiasDisf, @Ini, @Fin, @Clas, @Disfrutar, @Obs)", Parametros(sol))

            ' Consecutivos.Solicitud queda al día con el número usado (la V.5.0 lo sobreescribía siempre).
            Dim n As Integer
            If Integer.TryParse(sol.Numero_Solicitud, n) Then
                EjecutarEn(cn, tr, "UPDATE Consecutivos SET Solicitud = @N WHERE ISNULL(Solicitud, 0) < @N", New SqlParameter("@N", n))
            End If

        Else

            ' Como la V.5.0: no cambia fecha de solicitud ni empleado; la clasificación vacía no se toca.
            EjecutarEn(cn, tr,
                "UPDATE SolicitudVacaciones SET TipoSolicitud = @Tipo, DiasVacaciones = @DiasVac, DiasDisfrutados = @DiasDisf, " &
                "FechaInicio = @Ini, FechaFin = @Fin, Clasificacion = CASE WHEN @Clas IS NULL THEN Clasificacion ELSE @Clas END, " &
                "DiasDisfrutar = @Disfrutar, Observaciones = @Obs WHERE NumeroSolicitud = @Num", Parametros(sol))

        End If

        GrabarDescuentosEn(cn, tr, sol.Codigo_Empleado, sol.Tipo_Solicitud, sol.Numero_Solicitud, sol.Fecha_Inicio.Date, sol.Fecha_Fin.Date,
                           If(sol.Horas_Solicitud = 0 AndAlso sol.Dias_Solicitud >= 1, (sol.Fecha_Fin.Date - sol.Fecha_Inicio.Date).Days + 1, sol.Dias_Disfrutar))

        Return sol.Numero_Solicitud

    End Function

    ''' <summary>V.5.0 SolicitudValidar. Devuelve las horas por día del empleado.</summary>
    Private Shared Function Validar(cn As SqlConnection, tr As SqlTransaction, sol As SolicitudVacaciones) As Double

        If sol.Codigo_Empleado.Trim = "" Then Throw New InvalidOperationException("Debe indicar un empleado.")
        If sol.Tipo_Solicitud.Trim = "" Then Throw New InvalidOperationException("Debe indicar el tipo de solicitud.")

        Dim existe As Object = EscalarEn(cn, tr, "SELECT COUNT(*) FROM Empleado WHERE CodEmpleado1 = @Cod AND Activo = 1",
                                         New SqlParameter("@Cod", sol.Codigo_Empleado))
        If Convert.ToInt32(existe) = 0 Then Throw New InvalidOperationException("El empleado " & sol.Codigo_Empleado & " no existe o está inactivo.")

        If sol.Fecha_Fin.Date < sol.Fecha_Inicio.Date Then Throw New InvalidOperationException("La fecha fin no puede ser menor que la fecha inicio.")
        If sol.Dias_Solicitud <= 0 Then Throw New InvalidOperationException("Los días a disfrutar deben ser mayores que cero.")
        If sol.Horas_Solicitud < 0 Then Throw New InvalidOperationException("Las horas a disfrutar no pueden ser negativas.")

        Dim horas As Double = HorasDiaEn(cn, tr, sol.Codigo_Empleado)

        If sol.Horas_Solicitud > horas Then
            Throw New InvalidOperationException("Según la nómina el empleado " & sol.Codigo_Empleado & " labora al día únicamente " & horas & " horas.")
        End If

        Return horas

    End Function

    Private Shared Function HorasDiaEn(cn As SqlConnection, tr As SqlTransaction, codigo1 As String) As Double
        Dim r As Object = EscalarEn(cn, tr,
            "SELECT TOP 1 t.Horas FROM TipoNomina t INNER JOIN Empleado e ON t.CodTipoNomina = e.CodTipoNomina WHERE e.CodEmpleado1 = @Cod",
            New SqlParameter("@Cod", codigo1))
        Return If(r Is Nothing OrElse IsDBNull(r), 0, Convert.ToDouble(r))
    End Function

    Private Shared Function SiguienteNumeroEn(cn As SqlConnection, tr As SqlTransaction) As String
        Dim ultimo As Object = EscalarEn(cn, tr,
            "SELECT MAX(CASE WHEN NumeroSolicitud NOT LIKE '%[^0-9]%' AND LEN(NumeroSolicitud) <= 9 " &
            "THEN CAST(NumeroSolicitud AS int) END) FROM SolicitudVacaciones WITH (TABLOCKX)")
        Return (If(ultimo Is Nothing OrElse IsDBNull(ultimo), 0, Convert.ToInt32(ultimo)) + 1).ToString("00000")
    End Function

#End Region

#Region "DESCUENTOS"

    ''' <summary>
    ''' Regenera los días de la solicitud en DescuentoDiasVacaciones: uno por día del rango, o uno solo con
    ''' <paramref name="cantidadPagadas"/> en "Vacaciones Pagadas" (V.5.0 SolicitudGrabarDescuentos/GrabaDescuentoDias).
    ''' También lo usa la importación de solicitudes.
    ''' </summary>
    Friend Shared Sub GrabarDescuentosEn(cn As SqlConnection, tr As SqlTransaction, codigo As String, tipo As String, numero As String,
                                         inicio As Date, fin As Date, cantidadPagadas As Double)

        EjecutarEn(cn, tr, "DELETE FROM DescuentoDiasVacaciones WHERE NumeroSolicitud = @Num", New SqlParameter("@Num", numero))

        If tipo = "Vacaciones Pagadas" Then
            GrabarDescuentoDia(cn, tr, inicio, codigo, tipo, numero, cantidadPagadas)
            Return
        End If

        Dim dia As Date = inicio

        Do While dia <= fin
            GrabarDescuentoDia(cn, tr, dia, codigo, tipo, numero, 1)
            dia = dia.AddDays(1)
        Loop

    End Sub

    Private Shared Sub GrabarDescuentoDia(cn As SqlConnection, tr As SqlTransaction, fecha As Date, codigo As String, tipo As String, numero As String, cantDias As Double)

        Dim color As Object = ColorDescuento(tipo)

        EjecutarEn(cn, tr,
            "IF EXISTS (SELECT 1 FROM DescuentoDiasVacaciones WHERE FechaDescuento = @F AND TipoDescuento = @T AND CodigoEmpleado = @C) " &
            "UPDATE DescuentoDiasVacaciones SET CantDias = @D WHERE FechaDescuento = @F AND TipoDescuento = @T AND CodigoEmpleado = @C " &
            "ELSE INSERT INTO DescuentoDiasVacaciones (FechaDescuento, CodigoEmpleado, TipoDescuento, NumeroSolicitud, CantDias, Color) " &
            "VALUES (@F, @C, @T, @N, @D, @Col)",
            New SqlParameter("@F", fecha.Date), New SqlParameter("@C", codigo), New SqlParameter("@T", tipo),
            New SqlParameter("@N", numero), New SqlParameter("@D", cantDias), New SqlParameter("@Col", color))

    End Sub

    Private Shared Function ColorDescuento(tipo As String) As Object

        Select Case tipo
            Case "Vacaciones Pagadas" : Return "&H0000C000&"
            Case "Vacaciones" : Return "&H00FFC0C0&"
            Case "Subsidio" : Return "&H0080C0FF&"
            Case "Ausente" : Return "&H00FF80FF&"
            Case "Feriado" : Return "&H008888FB&"
            Case "Vacaciones Programadas" : Return "&H0080FFFF&"
            Case "Permiso Programado" : Return "&H00FFFF00&"
            Case Else : Return DBNull.Value
        End Select

    End Function

#End Region

#Region "PARAMETROS / MAPEAR"

    Private Shared Function Parametros(s As SolicitudVacaciones) As SqlParameter()
        Return {
            New SqlParameter("@FechaSol", SqlDbType.SmallDateTime) With {.Value = s.Fecha_Solicitud.Date},
            New SqlParameter("@Num", SqlDbType.NVarChar, 50) With {.Value = s.Numero_Solicitud},
            New SqlParameter("@Cod", SqlDbType.NVarChar, 50) With {.Value = s.Codigo_Empleado},
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 50) With {.Value = s.Tipo_Solicitud},
            New SqlParameter("@DiasVac", SqlDbType.Float) With {.Value = s.Dias_Vacaciones},
            New SqlParameter("@DiasDisf", SqlDbType.Float) With {.Value = s.Dias_Disfrutados},
            New SqlParameter("@Ini", SqlDbType.SmallDateTime) With {.Value = s.Fecha_Inicio.Date},
            New SqlParameter("@Fin", SqlDbType.SmallDateTime) With {.Value = s.Fecha_Fin.Date},
            New SqlParameter("@Clas", SqlDbType.NVarChar, 50) With {.Value = ValorODbNull(s.Clasificacion)},
            New SqlParameter("@Disfrutar", SqlDbType.Float) With {.Value = s.Dias_Disfrutar},
            New SqlParameter("@Obs", SqlDbType.NVarChar, -1) With {.Value = If(s.Observaciones, "")}
        }
    End Function

    Private Shared Function Mapear(f As DataRow) As SolicitudVacaciones
        Return New SolicitudVacaciones With {
            .Numero_Solicitud = Convert.ToString(f("NumeroSolicitud")),
            .Codigo_Empleado = Convert.ToString(f("CodigoEmpleado")),
            .Tipo_Solicitud = Convert.ToString(f("TipoSolicitud")),
            .Clasificacion = If(IsDBNull(f("Clasificacion")), "", Convert.ToString(f("Clasificacion"))),
            .Fecha_Solicitud = Convert.ToDateTime(f("FechaSolicitud")),
            .Fecha_Inicio = If(IsDBNull(f("FechaInicio")), Date.Today, Convert.ToDateTime(f("FechaInicio"))),
            .Fecha_Fin = If(IsDBNull(f("FechaFin")), Date.Today, Convert.ToDateTime(f("FechaFin"))),
            .Dias_Vacaciones = Numero(f("DiasVacaciones")),
            .Dias_Disfrutados = Numero(f("DiasDisfrutados")),
            .Dias_Disfrutar = Numero(f("DiasDisfrutar")),
            .Observaciones = If(IsDBNull(f("Observaciones")), "", Convert.ToString(f("Observaciones"))),
            .Num_Nomina = Numero(f("NumNomina"))}
    End Function

    Private Shared Function Numero(valor As Object) As Double
        Return If(valor Is Nothing OrElse IsDBNull(valor), 0, Convert.ToDouble(valor))
    End Function

    Private Shared Function Copiar(s As SolicitudVacaciones) As SolicitudVacaciones
        Return New SolicitudVacaciones With {
            .Numero_Solicitud = s.Numero_Solicitud, .Codigo_Empleado = s.Codigo_Empleado, .Tipo_Solicitud = s.Tipo_Solicitud,
            .Clasificacion = s.Clasificacion, .Fecha_Solicitud = s.Fecha_Solicitud, .Fecha_Inicio = s.Fecha_Inicio, .Fecha_Fin = s.Fecha_Fin,
            .Dias_Solicitud = s.Dias_Solicitud, .Horas_Solicitud = s.Horas_Solicitud, .Dias_Vacaciones = s.Dias_Vacaciones,
            .Dias_Disfrutados = s.Dias_Disfrutados, .Observaciones = s.Observaciones}
    End Function

#End Region

End Class
