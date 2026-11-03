Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Consultas de la planilla de una nómina (FrmCalcularNomina): estado de la nómina activa, detalle por empleado y las
''' listas que alimentan los archivos de banco y las denominaciones.
''' </summary>
Public Class CsPlanillaNominaDAL
    Inherits CsDALBase

    Public Class EstadoNomina
        Public Property Numero As Integer
        Public Property Inicio As Date
        Public Property Fin As Date
        Public Property Procesada As Boolean
        Public Property Cerrada As Boolean
        Public Property Activa As Boolean
    End Class

    ' Fórmulas de los reportes de la V.5.0 (CmdExportar*, CmdMonedasDpto, PushButton2).
    Friend Const SqlDevengado As String =
        "(ISNULL(D.SalarioBasico, 0) + ISNULL(D.Comisiones, 0) + ISNULL(D.Incentivos, 0) + ISNULL(D.HorasExtras, 0) + ISNULL(D.OtrosIngresos, 0) + " &
        "ISNULL(D.Destajo, 0) + ISNULL(D.VacacionesPagadas, 0) + ISNULL(D.SeptimoDia, 0) + ISNULL(D.IncetivoProduccion, 0) + ISNULL(D.BonoProduccion, 0))"

    Friend Const SqlDeducir As String =
        "(ISNULL(D.Prestamo, 0) + ISNULL(D.MontoINSS, 0) + ISNULL(D.MontoIR, 0) + ISNULL(D.Deducciones, 0) + ISNULL(D.MontoPension, 0) + ISNULL(D.EmbargoJudicial, 0))"

    Friend Const SqlNombres As String =
        "LTRIM(RTRIM(ISNULL(E.Nombre1, '') + ' ' + ISNULL(E.Nombre2, '') + ' ' + ISNULL(E.Apellido1, '') + ' ' + ISNULL(E.Apellido2, '')))"

    Public Function ListarTipos() As DataTable
        Return ConsultarTabla("SELECT CodTipoNomina, Nomina, Periodo, ISNULL(MostraEmpleadoCero, 0) AS MostraEmpleadoCero FROM TipoNomina ORDER BY CodTipoNomina")
    End Function

    ''' <summary>La nómina abierta del tipo; si no hay, la última (cerrada) para poder consultarla. Nothing si el tipo no tiene ninguna.</summary>
    Public Function BuscarNomina(codTipoNomina As String) As EstadoNomina

        Dim t As DataTable = ConsultarTabla(
            "SELECT TOP 1 NumNomina, FechaNominaINI, FechaNomina, Procesada, Cerrada, Activa FROM Nomina " &
            "WHERE CodTipoNomina = @Cod AND ISNULL(Anulada, 0) = 0 ORDER BY Activa DESC, NumNomina DESC",
            New SqlParameter("@Cod", codTipoNomina))

        If t.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = t.Rows(0)
        Return New EstadoNomina With {
            .Numero = Convert.ToInt32(r("NumNomina")), .Inicio = Convert.ToDateTime(r("FechaNominaINI")), .Fin = Convert.ToDateTime(r("FechaNomina")),
            .Procesada = Convert.ToBoolean(r("Procesada")), .Cerrada = Convert.ToBoolean(r("Cerrada")), .Activa = Convert.ToBoolean(r("Activa"))}

    End Function

    Friend Const SqlColumnasPlanilla As String =
        "E.CodEmpleado1 AS Codigo, " & SqlNombres & " AS Nombres, ISNULL(Dp.Departamento, '') AS Departamento, ISNULL(C.Cargo, '') AS Cargo, " &
        "E.NumeroInss, E.NumCedula, D.SalarioBasico, D.SeptimoDia, D.IncetivoProduccion, D.BonoProduccion, D.Destajo, D.HE, D.HorasExtras, D.Comisiones, " &
        "D.Incentivos, D.OtrosIngresos, D.VacacionesPagadas, D.Antiguedad, D.TotalSubsidio, D.Viaticos, " & SqlDevengado & " AS TotalDevengado, " &
        "D.MontoINSS, D.MontoIR, D.Prestamo, D.Deducciones, D.Adelantos, D.MontoPension, D.EmbargoJudicial, " & SqlDeducir & " AS TotalDeducir, " &
        SqlDevengado & " - " & SqlDeducir & " AS NetoPagar, D.INSSPatronal, D.INATEC, D.Mes13, D.Vacaciones"

    Public Function ListarPlanilla(numNomina As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT " & SqlColumnasPlanilla & " " &
            "FROM DetalleNomina D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "LEFT JOIN Departamento Dp ON Dp.CodDepartamento = E.CodDepartamento LEFT JOIN Cargo C ON C.CodCargo = E.CodCargo " &
            "WHERE D.NumNomina = @Num ORDER BY E.CodEmpleado1",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina})
    End Function

    ''' <summary>
    ''' Planilla para colillas y reportes: <see cref="ListarPlanilla"/> más Grupo, FechaIngreso y SalarioMensual, ordenada por el
    ''' campo de agrupación del reporte (Grupo, Departamento o Cargo; otro valor = por código). Con <paramref name="mostrarCeros"/>
    ''' en falso omite a los empleados sin devengado (MostraEmpleadoCero del tipo de nómina).
    ''' </summary>
    Public Function ListarParaReporte(numNomina As Integer, ordenarPor As String, mostrarCeros As Boolean) As DataTable

        Dim orden As String
        Select Case ordenarPor
            Case "Grupo" : orden = "Grupo, E.CodEmpleado1"
            Case "Departamento" : orden = "Departamento, E.CodEmpleado1"
            Case "Cargo" : orden = "Cargo, E.CodEmpleado1"
            Case Else : orden = "E.CodEmpleado1"
        End Select

        Return ConsultarTabla(
            "SELECT " & SqlColumnasPlanilla & ", ISNULL(G.Grupo, '') AS Grupo, H.FechaContrato AS FechaIngreso, ISNULL(E.SueldoPeriodo, 0) AS SalarioMensual, " & SqlDevengado & " - " & SqlDeducir & " AS MontoPagar, " &
            "D.NumNomina, ISNULL(D.DiasVacaciones, 0) AS DiasVacaciones, ISNULL(D.[A" & ChrW(241) & "oAntiguedad], 0) AS AnioAntiguedad, ISNULL(D.DescripOtrIngre, '') AS DescripOtrIngre, " &
            "ISNULL(D.HTrabajada, 0) AS HTrabajada, ISNULL(D.Ajuste, 0) AS Ajuste, ISNULL(D.Dias_Subsidio, 0) AS DiasSubsidio, ISNULL(D.Clasificacion_Subsidio, '') AS Clasificacion_Subsidio, " &
            "ISNULL(D.DiasDescuento, 0) AS DescuentoFalta, ISNULL(D.DD, 0) AS DiasAusente, ISNULL(TN.Nomina, '') AS TipoNomina, ISNULL(TN.Periodo, '') AS PeriodoNomina " &
            "FROM DetalleNomina D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "LEFT JOIN Departamento Dp ON Dp.CodDepartamento = E.CodDepartamento LEFT JOIN Cargo C ON C.CodCargo = E.CodCargo " &
            "LEFT JOIN Grupo G ON G.CodGrupo = E.CodGrupo LEFT JOIN TipoNomina TN ON TN.CodTipoNomina = E.CodTipoNomina " &
            "OUTER APPLY (SELECT TOP 1 FechaContrato FROM Historico WHERE CodEmpleado = E.CodEmpleado) H " &
            "WHERE D.NumNomina = @Num AND (@Cero = 1 OR " & SqlDevengado & " <> 0) ORDER BY " & orden,
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina},
            New SqlParameter("@Cero", SqlDbType.Bit) With {.Value = mostrarCeros})

    End Function

    ''' <summary>
    ''' Una fila por deducción de cada empleado en la nómina (V.5.0 SubColillaDeduccion): Codigo, Deduccion, Valor. Excluye solo la falta (01), que la
    ''' colilla muestra como "Dias Ausente"; así la suma coincide con DetalleNomina.Deducciones (otras deducciones + adelantos).
    ''' </summary>
    Public Function ListarDetalleDeducciones(numNomina As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, T.Deduccion, DD.Valor FROM DetalleDeduccion DD " &
            "INNER JOIN Deduccion De ON De.NumDeduccion = DD.NumDeduccion INNER JOIN Empleado E ON E.CodEmpleado = De.CodEmpleado " &
            "INNER JOIN TipoDeduccion T ON T.CodTipoDeduccion = De.CodTipoDeduccion " &
            "WHERE DD.NumNomina = @Num AND T.CodTipoDeduccion <> '01' ORDER BY E.CodEmpleado1, DD.Id",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina})
    End Function

    ''' <summary>Pagos a banco: una fila por empleado con neto distinto de cero.</summary>
    Public Function ListarParaBanco(numNomina As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & SqlNombres & " AS Nombres, " & SqlDevengado & " - " & SqlDeducir & " AS Monto, " &
            "E.NumCedula, H.CuentaBanco, E.CuentaBancoBANPRO, E.CuentaBancoBDF, E.CuentaBancoLAFISE " &
            "FROM DetalleNomina D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "OUTER APPLY (SELECT TOP 1 CuentaBanco FROM Historico WHERE CodEmpleado = E.CodEmpleado) H " &
            "WHERE D.NumNomina = @Num AND (" & SqlDevengado & " - " & SqlDeducir & ") <> 0 ORDER BY E.CodEmpleado1",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina})
    End Function

    Public Function ListarMontos(numNomina As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & SqlNombres & " AS Nombres, " & SqlDevengado & " - " & SqlDeducir & " AS Monto " &
            "FROM DetalleNomina D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "WHERE D.NumNomina = @Num AND (" & SqlDevengado & " - " & SqlDeducir & ") <> 0 ORDER BY E.CodEmpleado1",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina})
    End Function

End Class
