Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Catálogo y consultas de los reportes de FrmReportesNomina (V.5.0 FrmReportes). Cada reporte es una consulta parametrizada más la
''' definición de sus columnas; <see cref="RptTabular"/> los dibuja todos. Los reportes por rango suman las nóminas calculadas
''' (Procesada, no anuladas) cuyo fin cae entre las fechas indicadas, del tipo elegido o de todos.
''' </summary>
Public Class CsReportesNominaDAL
    Inherits CsDALBase

    Public Class DefinicionReporte
        Public Property Clave As String
        Public Property Nombre As String
        Public Property Descripcion As String
        ''' <summary>El reporte usa el tipo de nómina y el rango de fechas.</summary>
        Public Property UsaRango As Boolean = True
        ''' <summary>Sin rango: usa solo la fecha "Hasta" como corte (vacaciones).</summary>
        Public Property UsaFechaCorte As Boolean
        ''' <summary>Reporte de una sola nómina: pide el número de nómina.</summary>
        Public Property UsaNumeroNomina As Boolean
        ''' <summary>Habilita el filtro "Saldo mayor a".</summary>
        Public Property UsaSaldoMinimo As Boolean
        Public Property CampoGrupo As String = ""
        Public Property TituloGrupo As String = ""
        Public Property Columnas As New List(Of ColumnaReporte)
    End Class

    Private Const Hasta As String = " FROM DetalleNomina D INNER JOIN Nomina N ON N.NumNomina = D.NumNomina INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
        "LEFT JOIN Departamento Dp ON Dp.CodDepartamento = E.CodDepartamento LEFT JOIN Cargo C ON C.CodCargo = E.CodCargo " &
        "WHERE ISNULL(N.Anulada, 0) = 0 AND N.Procesada = 1 AND N.FechaNomina BETWEEN @Ini AND @Fin AND (@Tipo = '' OR N.CodTipoNomina = @Tipo)"

    Private Const Identidad As String = "Codigo, Nombres, Departamento, Cargo, NumeroInss, NumCedula"

    Private Shared ReadOnly Definiciones As IList(Of DefinicionReporte) = CrearCatalogo()

    Public Shared Function Catalogo() As IList(Of DefinicionReporte)
        Return Definiciones
    End Function

    Public Shared Function Buscar(clave As String) As DefinicionReporte
        Return Definiciones.First(Function(d) d.Clave = clave)
    End Function

    Private Shared Function Num(titulo As String, campo As String, Optional ancho As Single = 60, Optional formato As String = "#,##0.00") As ColumnaReporte
        Return New ColumnaReporte(titulo, campo, ancho, formato, True)
    End Function

    Private Shared Function Persona(extra As IEnumerable(Of ColumnaReporte), Optional conInss As Boolean = False) As List(Of ColumnaReporte)
        Dim c As New List(Of ColumnaReporte) From {New ColumnaReporte("Código", "Codigo", 45), New ColumnaReporte("Nombre", "Nombres", 190)}
        If conInss Then c.Add(New ColumnaReporte("No. INSS", "NumeroInss", 75))
        c.AddRange(extra)
        Return c
    End Function

    Private Shared Function CrearCatalogo() As IList(Of DefinicionReporte)

        Dim l As New List(Of DefinicionReporte)

        l.Add(New DefinicionReporte With {
            .Clave = "ResumenPago", .Nombre = "Resumen de pago mensual", .Descripcion = "Una línea por nómina del rango: empleados, devengado, deducciones y neto.",
            .Columnas = New List(Of ColumnaReporte) From {
                New ColumnaReporte("Nómina", "NumNomina", 50), New ColumnaReporte("Tipo", "Nomina", 150), New ColumnaReporte("Desde", "FechaIni", 70, "dd/MM/yyyy"),
                New ColumnaReporte("Hasta", "FechaFin", 70, "dd/MM/yyyy"), Num("Empleados", "Empleados", 60, "#,##0"), Num("Total devengado", "Devengado", 100),
                Num("Total deducciones", "Deducciones", 100), Num("Neto a pagar", "Neto", 100)}})

        l.Add(New DefinicionReporte With {
            .Clave = "TotalPago", .Nombre = "Total de pago mensual", .Descripcion = "Totales del rango por departamento.", .CampoGrupo = "",
            .Columnas = New List(Of ColumnaReporte) From {
                New ColumnaReporte("Departamento", "Departamento", 200), Num("Empleados", "Empleados", 70, "#,##0"), Num("Total devengado", "Devengado", 110),
                Num("Total deducciones", "Deducciones", 110), Num("Neto a pagar", "Neto", 110)}})

        l.Add(New DefinicionReporte With {
            .Clave = "Inss", .Nombre = "INSS", .Descripcion = "INSS laboral y patronal por empleado, por departamento.", .CampoGrupo = "Departamento", .TituloGrupo = "Departamento",
            .Columnas = Persona({Num("Devengado", "Devengado", 85), Num("INSS laboral", "InssLaboral", 80), Num("INSS patronal", "InssPatronal", 80), Num("INATEC", "Inatec", 70)}, True)})

        l.Add(New DefinicionReporte With {
            .Clave = "Ir", .Nombre = "IR", .Descripcion = "Impuesto sobre la renta retenido por empleado, por departamento.", .CampoGrupo = "Departamento", .TituloGrupo = "Departamento",
            .Columnas = New List(Of ColumnaReporte) From {
                New ColumnaReporte("Código", "Codigo", 45), New ColumnaReporte("Nombre", "Nombres", 190), New ColumnaReporte("Cédula", "NumCedula", 90),
                Num("Devengado", "Devengado", 90), Num("INSS laboral", "InssLaboral", 80), Num("IR retenido", "MontoIr", 90)}})

        l.Add(New DefinicionReporte With {
            .Clave = "InssIr", .Nombre = "INSS e IR mensual", .Descripcion = "INSS e IR juntos por empleado.", .CampoGrupo = "Departamento", .TituloGrupo = "Departamento",
            .Columnas = Persona({Num("Devengado", "Devengado", 85), Num("INSS laboral", "InssLaboral", 80), Num("IR retenido", "MontoIr", 80), Num("INSS patronal", "InssPatronal", 80)}, True)})

        l.Add(New DefinicionReporte With {
            .Clave = "HorasExtra", .Nombre = "Horas extra", .Descripcion = "Horas y monto de horas extras por empleado.", .CampoGrupo = "Departamento", .TituloGrupo = "Departamento",
            .Columnas = Persona({Num("Horas extra", "HorasExtra", 80, "#,##0.00"), Num("Monto horas extra", "MontoHorasExtra", 100)})})

        l.Add(New DefinicionReporte With {
            .Clave = "Ingresos", .Nombre = "Ingresos generales", .Descripcion = "Todos los ingresos por empleado.", .CampoGrupo = "Departamento", .TituloGrupo = "Departamento",
            .Columnas = Persona({Num("Salario", "Salario", 70), Num("Destajo", "Destajo", 60), Num("Séptimo", "Septimo", 55), Num("Horas extra", "MontoHorasExtra", 65),
                                 Num("Comisiones", "Comisiones", 65), Num("Incentivos", "Incentivos", 65), Num("Otros", "OtrosIngresos", 60), Num("Vacaciones", "Vacaciones", 65),
                                 Num("Total devengado", "Devengado", 80)})})

        l.Add(New DefinicionReporte With {
            .Clave = "Deducciones", .Nombre = "Detalle de deducciones", .Descripcion = "Deducciones, adelantos, préstamos y retenciones por empleado.", .CampoGrupo = "Departamento", .TituloGrupo = "Departamento",
            .Columnas = Persona({Num("INSS", "InssLaboral", 65), Num("IR", "MontoIr", 65), Num("Préstamos", "Prestamo", 65), Num("Otras deducciones", "Otras", 80),
                                 Num("Pensión", "Pension", 60), Num("Embargo", "Embargo", 60), Num("Total deducciones", "Deducciones", 85)})})

        l.Add(New DefinicionReporte With {
            .Clave = "SalarioProduccion", .Nombre = "Salario básico vs producción", .Descripcion = "Compara el salario básico con lo pagado por producción.", .CampoGrupo = "Departamento", .TituloGrupo = "Departamento",
            .Columnas = Persona({Num("Salario básico", "Salario", 85), Num("Destajo", "Destajo", 75), Num("Incentivo producción", "IncentivoProduccion", 95),
                                 Num("Bono producción", "BonoProduccion", 85), Num("Total devengado", "Devengado", 90)})})

        l.Add(New DefinicionReporte With {
            .Clave = "EmpleadosActivos", .Nombre = "Lista de empleados activos", .Descripcion = "Empleados activos hoy, por departamento (no usa tipo ni fechas).", .UsaRango = False,
            .CampoGrupo = "Departamento", .TituloGrupo = "Departamento",
            .Columnas = New List(Of ColumnaReporte) From {
                New ColumnaReporte("Código", "Codigo", 45), New ColumnaReporte("Nombre", "Nombres", 190), New ColumnaReporte("Cédula", "NumCedula", 90),
                New ColumnaReporte("No. INSS", "NumeroInss", 75), New ColumnaReporte("Cargo", "Cargo", 130), New ColumnaReporte("Ingreso", "FechaIngreso", 70, "dd/MM/yyyy"),
                Num("Salario", "Salario", 80)}})

        l.Add(New DefinicionReporte With {
            .Clave = "InssDetalle", .Nombre = "Detalle de INSS por nómina", .Descripcion = "Una línea por empleado y nómina del rango (con salario básico): INSS laboral, patronal, total e INATEC.",
            .Columnas = New List(Of ColumnaReporte) From {
                New ColumnaReporte("Código", "Codigo", 42), New ColumnaReporte("Nombre", "Nombres", 170), New ColumnaReporte("Cargo", "Cargo", 110),
                New ColumnaReporte("No. INSS", "CodInss", 65), New ColumnaReporte("Nómina", "NumNomina", 45), New ColumnaReporte("Fecha", "FechaNomina", 62, "dd/MM/yyyy"),
                Num("Devengado", "Devengado", 72), Num("INSS laboral", "InssLaboral", 66), Num("INSS patronal", "InssPatronal", 66), Num("Total INSS", "TotalInss", 66), Num("INATEC", "Inatec", 56)}})

        l.Add(New DefinicionReporte With {
            .Clave = "IrDetalle", .Nombre = "Detalle de IR por nómina", .Descripcion = "Una línea por empleado y nómina del rango con IR retenido (el devengado incluye antigüedad).",
            .Columnas = New List(Of ColumnaReporte) From {
                New ColumnaReporte("Código", "Codigo", 42), New ColumnaReporte("Nombre", "Nombres", 170), New ColumnaReporte("Cargo", "Cargo", 110),
                New ColumnaReporte("Código IR", "CodIr", 65), New ColumnaReporte("Nómina", "NumNomina", 45), New ColumnaReporte("Fecha", "FechaNomina", 62, "dd/MM/yyyy"),
                Num("Devengado", "Devengado", 80), Num("INSS laboral", "InssLaboral", 72), Num("IR retenido", "MontoIr", 72)}})

        l.Add(New DefinicionReporte With {
            .Clave = "ListadoMaestro", .Nombre = "Listado maestro de la nómina", .UsaRango = False, .UsaNumeroNomina = True, .CampoGrupo = "Grupo", .TituloGrupo = "Grupo",
            .Descripcion = "Todos los empleados de una nómina, por grupo, con sus ingresos, deducciones y neto. Indique el número de nómina.",
            .Columnas = New List(Of ColumnaReporte) From {
                New ColumnaReporte("Código", "Codigo", 40), New ColumnaReporte("Nombre", "Nombres", 140), New ColumnaReporte("Cargo", "Cargo", 90),
                New ColumnaReporte("Ingreso", "FechaIngreso", 62, "dd/MM/yyyy"), Num("Salario básico", "SalarioBasico", 62), Num("Horas extra", "HorasExtras", 62),
                Num("Comisiones", "Comisiones", 62), Num("Incentivos", "Incentivos", 62), Num("Otros ingresos", "OtrosIngresos", 62),
                Num("Total devengado", "TotalDevengado", 62), Num("INSS", "MontoINSS", 62), Num("IR", "MontoIR", 62), Num("Neto a pagar", "NetoPagar", 62)}})

        l.Add(New DefinicionReporte With {
            .Clave = "VacConsolidado", .Nombre = "Consolidado de vacaciones", .UsaRango = False, .UsaFechaCorte = True, .UsaSaldoMinimo = True,
            .Descripcion = "Días acumulados, solicitados y saldo de cada empleado activo a la fecha 'Hasta' (también cubre 'Días acumulados'). Con 'Saldo mayor a' solo los que lo superan.",
            .CampoGrupo = "Departamento", .TituloGrupo = "Departamento",
            .Columnas = New List(Of ColumnaReporte) From {
                New ColumnaReporte("Código", "Codigo", 45), New ColumnaReporte("Nombre", "Nombres", 200), New ColumnaReporte("Cargo", "Cargo", 130),
                New ColumnaReporte("Ingreso vacaciones", "FechaContratoVac", 85, "dd/MM/yyyy"), Num("Acumulados", "Acumulado", 70), Num("Solicitados", "Solicitado", 70),
                Num("Saldo", "Saldo", 70)}})

        Return l

    End Function

    ''' <summary>
    ''' Saldo de vacaciones al <paramref name="corte"/> de los empleados activos (del tipo, o todos), con el mismo cálculo que la
    ''' solicitud de vacaciones (<see cref="CalculadoraVacaciones"/>). Con <paramref name="saldoMinimo"/> solo los de saldo mayor.
    ''' </summary>
    Private Function ConsolidadoVacaciones(codTipoNomina As String, corte As Date, saldoMinimo As Double?) As DataTable

        Dim t As New DataTable
        t.Columns.Add("Codigo", GetType(String))
        t.Columns.Add("Nombres", GetType(String))
        t.Columns.Add("Departamento", GetType(String))
        t.Columns.Add("Cargo", GetType(String))
        t.Columns.Add("FechaContratoVac", GetType(Date))
        t.Columns.Add("Acumulado", GetType(Double))
        t.Columns.Add("Solicitado", GetType(Double))
        t.Columns.Add("Saldo", GetType(Double))

        EnTransaccion(
            Sub(cn, tr)

                Dim es30 As Boolean = CsVacacionesDAL.DiasMesEs30(cn, tr)
                Dim empleados As New DataTable

                Using cmd As New SqlCommand(
                    "SELECT E.CodEmpleado1 AS Codigo, " & CsPlanillaNominaDAL.SqlNombres & " AS Nombres, ISNULL(Dp.Departamento, '') AS Departamento, " &
                    "ISNULL(C.Cargo, '') AS Cargo, H.FechaContratoVac FROM Empleado E LEFT JOIN Departamento Dp ON Dp.CodDepartamento = E.CodDepartamento " &
                    "LEFT JOIN Cargo C ON C.CodCargo = E.CodCargo OUTER APPLY (SELECT TOP 1 FechaContratoVac FROM Historico WHERE CodEmpleado = E.CodEmpleado ORDER BY Id) H WHERE E.Activo = 1 AND (@Tipo = '' OR E.CodTipoNomina = @Tipo) " &
                    "ORDER BY ISNULL(Dp.Departamento, ''), E.CodEmpleado1", cn, tr)
                    cmd.Parameters.AddWithValue("@Tipo", If(codTipoNomina, ""))
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(empleados)
                    End Using
                End Using

                ' Las solicitudes se cargan una sola vez (el cálculo consulta cada mes de cada empleado); 'Todos' aplica a cualquiera.
                Dim porCodigo As New Dictionary(Of String, List(Of Tuple(Of Date, Double)))(StringComparer.OrdinalIgnoreCase)
                Dim solicitudes As New DataTable

                Using cmd As New SqlCommand(
                    "SELECT CodigoEmpleado, FechaInicio, DiasDisfrutar FROM SolicitudVacaciones WHERE TipoSolicitud NOT IN " & CsVacacionesDAL.TiposExcluidos &
                    " AND DiasDisfrutar IS NOT NULL AND FechaInicio IS NOT NULL", cn, tr)
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(solicitudes)
                    End Using
                End Using

                For Each s As DataRow In solicitudes.Rows
                    Dim cod As String = CStr(s("CodigoEmpleado"))
                    If Not porCodigo.ContainsKey(cod) Then porCodigo(cod) = New List(Of Tuple(Of Date, Double))
                    porCodigo(cod).Add(Tuple.Create(Convert.ToDateTime(s("FechaInicio")).Date, Convert.ToDouble(s("DiasDisfrutar"))))
                Next

                Dim paraTodos As List(Of Tuple(Of Date, Double)) = If(porCodigo.ContainsKey("Todos"), porCodigo("Todos"), New List(Of Tuple(Of Date, Double)))

                For Each f As DataRow In empleados.Rows

                    If IsDBNull(f("FechaContratoVac")) Then Continue For

                    Dim propias As List(Of Tuple(Of Date, Double)) = If(porCodigo.ContainsKey(CStr(f("Codigo"))), porCodigo(CStr(f("Codigo"))), New List(Of Tuple(Of Date, Double)))
                    Dim r As ResumenVacaciones = CalculadoraVacaciones.Calcular(Convert.ToDateTime(f("FechaContratoVac")), corte, es30,
                        Function(desde As Date, limite As Date)
                            Dim fin As Date = limite.Date.AddDays(1)
                            Return propias.Concat(paraTodos).Where(Function(x) x.Item1 >= desde.Date AndAlso x.Item1 < fin).Sum(Function(x) x.Item2)
                        End Function)

                    If saldoMinimo.HasValue AndAlso r.Saldo <= saldoMinimo.Value Then Continue For

                    t.Rows.Add(f("Codigo"), f("Nombres"), f("Departamento"), f("Cargo"), f("FechaContratoVac"), r.Acumulado, r.Solicitado, r.Saldo)

                Next

            End Sub)

        Return t

    End Function

    Public Function ListarTipos() As DataTable
        Return ConsultarTabla("SELECT CodTipoNomina, Nomina FROM TipoNomina ORDER BY CodTipoNomina")
    End Function

    ''' <summary>Rango de fechas de las nóminas calculadas del tipo (o de todos): para proponer las fechas del reporte.</summary>
    Public Function RangoDisponible(codTipoNomina As String) As Tuple(Of Date, Date)

        Dim t As DataTable = ConsultarTabla(
            "SELECT MIN(FechaNominaINI) AS Ini, MAX(FechaNomina) AS Fin FROM Nomina " &
            "WHERE ISNULL(Anulada, 0) = 0 AND Procesada = 1 AND (@Tipo = '' OR CodTipoNomina = @Tipo)", New SqlParameter("@Tipo", codTipoNomina))

        If t.Rows.Count = 0 OrElse IsDBNull(t.Rows(0)("Ini")) Then Return Nothing
        Return Tuple.Create(Convert.ToDateTime(t.Rows(0)("Ini")), Convert.ToDateTime(t.Rows(0)("Fin")))

    End Function

    ''' <summary>Número de la última nómina calculada del tipo (o de todos); 0 si no hay.</summary>
    Public Function UltimaNomina(codTipoNomina As String) As Integer
        Dim r As Object = Escalar("SELECT MAX(NumNomina) FROM Nomina WHERE ISNULL(Anulada, 0) = 0 AND Procesada = 1 AND (@Tipo = '' OR CodTipoNomina = @Tipo)",
                                  New SqlParameter("@Tipo", If(codTipoNomina, "")))
        Return If(r Is Nothing OrElse IsDBNull(r), 0, Convert.ToInt32(r))
    End Function

    Public Function Consultar(clave As String, codTipoNomina As String, inicio As Date, fin As Date, Optional saldoMinimo As Double? = Nothing, Optional numeroNomina As Integer = 0) As DataTable

        Dim p As SqlParameter() = {
            New SqlParameter("@Tipo", If(codTipoNomina, "")),
            New SqlParameter("@Ini", SqlDbType.DateTime) With {.Value = inicio.Date},
            New SqlParameter("@Fin", SqlDbType.DateTime) With {.Value = fin.Date}}

        Select Case clave
            Case "ResumenPago"
                Return ConsultarTabla(
                    "SELECT N.NumNomina, T.Nomina, N.FechaNominaINI AS FechaIni, N.FechaNomina AS FechaFin, COUNT(*) AS Empleados, " &
                    "SUM(" & CsPlanillaNominaDAL.SqlDevengado & ") AS Devengado, SUM(" & CsPlanillaNominaDAL.SqlDeducir & ") AS Deducciones, " &
                    "SUM(" & CsPlanillaNominaDAL.SqlDevengado & " - " & CsPlanillaNominaDAL.SqlDeducir & ") AS Neto " &
                    "FROM DetalleNomina D INNER JOIN Nomina N ON N.NumNomina = D.NumNomina LEFT JOIN TipoNomina T ON T.CodTipoNomina = N.CodTipoNomina " &
                    "WHERE ISNULL(N.Anulada, 0) = 0 AND N.Procesada = 1 AND N.FechaNomina BETWEEN @Ini AND @Fin AND (@Tipo = '' OR N.CodTipoNomina = @Tipo) " &
                    "GROUP BY N.NumNomina, T.Nomina, N.FechaNominaINI, N.FechaNomina ORDER BY N.FechaNomina, N.NumNomina", p)

            Case "TotalPago"
                Return ConsultarTabla(
                    "SELECT ISNULL(Dp.Departamento, '') AS Departamento, COUNT(DISTINCT E.CodEmpleado) AS Empleados, " &
                    "SUM(" & CsPlanillaNominaDAL.SqlDevengado & ") AS Devengado, SUM(" & CsPlanillaNominaDAL.SqlDeducir & ") AS Deducciones, " &
                    "SUM(" & CsPlanillaNominaDAL.SqlDevengado & " - " & CsPlanillaNominaDAL.SqlDeducir & ") AS Neto" & Hasta &
                    " GROUP BY ISNULL(Dp.Departamento, '') ORDER BY ISNULL(Dp.Departamento, '')", p)

            Case "InssDetalle", "IrDetalle"
                Dim esIr As Boolean = (clave = "IrDetalle")
                Return ConsultarTabla(
                    "SELECT E.CodEmpleado1 AS Codigo, " & CsPlanillaNominaDAL.SqlNombres & " AS Nombres, ISNULL(C.Cargo, '') AS Cargo, E.CodInss, E.CodIr, " &
                    "D.NumNomina, N.FechaNomina, D.SalarioBasico + D.Comisiones + D.Incentivos + D.HorasExtras + D.OtrosIngresos + D.Destajo + D.VacacionesPagadas + " &
                    "D.SeptimoDia + D.IncetivoProduccion" & If(esIr, " + D.Antiguedad", "") & " AS Devengado, D.MontoINSS AS InssLaboral, D.INSSPatronal AS InssPatronal, " &
                    "D.MontoINSS + D.INSSPatronal AS TotalInss, D.INATEC AS Inatec, D.MontoIR AS MontoIr " &
                    "FROM DetalleNomina D INNER JOIN Nomina N ON N.NumNomina = D.NumNomina INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
                    "LEFT JOIN Cargo C ON C.CodCargo = E.CodCargo " &
                    "WHERE ISNULL(N.Anulada, 0) = 0 AND N.Procesada = 1 AND N.FechaNomina BETWEEN @Ini AND @Fin AND (@Tipo = '' OR N.CodTipoNomina = @Tipo) AND " &
                    If(esIr, "D.MontoIR <> 0", "D.SalarioBasico <> 0") & " ORDER BY E.CodEmpleado1, D.NumNomina", p)

            Case "ListadoMaestro"
                Return New CsPlanillaNominaDAL().ListarParaReporte(numeroNomina, "Grupo", True)

            Case "VacConsolidado"
                Return ConsolidadoVacaciones(codTipoNomina, fin, saldoMinimo)

            Case "EmpleadosActivos"
                Return ConsultarTabla(
                    "SELECT E.CodEmpleado1 AS Codigo, " & CsPlanillaNominaDAL.SqlNombres & " AS Nombres, ISNULL(Dp.Departamento, '') AS Departamento, ISNULL(C.Cargo, '') AS Cargo, " &
                    "E.NumeroInss, E.NumCedula, H.FechaContrato AS FechaIngreso, ISNULL(E.SueldoPeriodo, 0) AS Salario " &
                    "FROM Empleado E LEFT JOIN Departamento Dp ON Dp.CodDepartamento = E.CodDepartamento LEFT JOIN Cargo C ON C.CodCargo = E.CodCargo " &
                    "OUTER APPLY (SELECT TOP 1 FechaContrato FROM Historico WHERE CodEmpleado = E.CodEmpleado) H " &
                    "WHERE E.Activo = 1 ORDER BY ISNULL(Dp.Departamento, ''), E.CodEmpleado1")

            Case Else
                Return ConsultarTabla(PorEmpleado(clave), p)
        End Select

    End Function

    ''' <summary>Una fila por empleado con las sumas del rango; el interior reutiliza las columnas de la planilla.</summary>
    Private Shared Function PorEmpleado(clave As String) As String

        Dim sumas As String
        Select Case clave
            Case "Inss"
                sumas = "SUM(TotalDevengado) AS Devengado, SUM(MontoINSS) AS InssLaboral, SUM(INSSPatronal) AS InssPatronal, SUM(INATEC) AS Inatec"
            Case "Ir"
                sumas = "SUM(TotalDevengado) AS Devengado, SUM(MontoINSS) AS InssLaboral, SUM(MontoIR) AS MontoIr"
            Case "InssIr"
                sumas = "SUM(TotalDevengado) AS Devengado, SUM(MontoINSS) AS InssLaboral, SUM(MontoIR) AS MontoIr, SUM(INSSPatronal) AS InssPatronal"
            Case "HorasExtra"
                sumas = "SUM(HE) AS HorasExtra, SUM(HorasExtras) AS MontoHorasExtra"
            Case "Ingresos"
                sumas = "SUM(SalarioBasico) AS Salario, SUM(Destajo) AS Destajo, SUM(SeptimoDia) AS Septimo, SUM(HorasExtras) AS MontoHorasExtra, SUM(Comisiones) AS Comisiones, " &
                        "SUM(Incentivos) AS Incentivos, SUM(OtrosIngresos) AS OtrosIngresos, SUM(VacacionesPagadas) AS Vacaciones, SUM(TotalDevengado) AS Devengado"
            Case "Deducciones"
                sumas = "SUM(MontoINSS) AS InssLaboral, SUM(MontoIR) AS MontoIr, SUM(Prestamo) AS Prestamo, SUM(Deducciones) AS Otras, SUM(MontoPension) AS Pension, " &
                        "SUM(EmbargoJudicial) AS Embargo, SUM(TotalDeducir) AS Deducciones"
            Case "SalarioProduccion"
                sumas = "SUM(SalarioBasico) AS Salario, SUM(Destajo) AS Destajo, SUM(IncetivoProduccion) AS IncentivoProduccion, SUM(BonoProduccion) AS BonoProduccion, SUM(TotalDevengado) AS Devengado"
            Case Else
                Throw New ArgumentException("Reporte desconocido: " & clave)
        End Select

        Return "SELECT " & Identidad & ", " & sumas & " FROM (SELECT " & CsPlanillaNominaDAL.SqlColumnasPlanilla & Hasta & ") X " &
               "GROUP BY " & Identidad & " ORDER BY Departamento, Codigo"

    End Function

End Class
