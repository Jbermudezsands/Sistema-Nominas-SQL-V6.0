Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Nómina de vacaciones: V.5.0 Frm13VacaMes, parte de vacaciones (tablas NomVaca, DetalleNomVaca y HistorialSalarioMes con
''' Tipo = 'Vacaciones'). Aquí está todo el acceso a datos; las reglas de cálculo están en
''' <see cref="CalculadoraNominaVacaciones"/>. Todo parametrizado; calcular y cerrar trabajan en una transacción.
''' </summary>
Public Class CsNominaVacacionesDAL
    Inherits CsDALBase

    Private Const TipoHistorial As String = "Vacaciones"

    Public Class ResultadoCalculo
        Public Property Empleados As Integer
        Public Property Grabados As Integer
        Public Property Monto_Pagado As Double
    End Class

    ''' <summary>Parámetros de un cálculo; agrupa lo que la pantalla le pide al usuario.</summary>
    Public Class ParametrosCalculo
        Public Property Cod_Tipo_Nomina As String = ""
        Public Property Numero As Integer
        ''' <summary>Período de la nómina de vacaciones (los días de vacaciones se calculan al final).</summary>
        Public Property Fecha_Ini As Date
        Public Property Fecha_Fin As Date
        ''' <summary>Rango de planillas de donde sale el salario promedio (y los adelantos).</summary>
        Public Property Planilla_Ini As Date
        Public Property Planilla_Fin As Date
        Public Property Dias_Descuento As Double
        Public Property Antiguedad_Minima As Double
        Public Property Transferir As Boolean
    End Class

#Region "CATALOGOS Y PERIODO"

    Public Function ListarTiposNomina() As DataTable
        Return ConsultarTabla("SELECT CodTipoNomina, Nomina, Periodo, TipoPago FROM TipoNomina ORDER BY Nomina")
    End Function

    ''' <summary>
    ''' Nómina con la que abre la pantalla: la abierta del tipo de nómina; si no hay, la siguiente del último semestre
    ''' (enero-junio o julio-diciembre); si nunca se hizo, el semestre del año actual.
    ''' </summary>
    Public Function Proponer(codTipoNomina As String) As NominaPrestacion

        Dim abierta As DataTable = ConsultarTabla(
            "SELECT TOP 1 NumNomVaca, FechaAplica, MontoPagado, FechaIni, FechaFin, Activa, CodTipoNomina, Transfereir FROM NomVaca " &
            "WHERE Activa = 1 AND CodTipoNomina = @Tipo ORDER BY FechaFin DESC",
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = codTipoNomina})

        If abierta.Rows.Count > 0 Then Return Mapear(abierta.Rows(0))

        Dim maximo As Object = Escalar("SELECT MAX(NumNomVaca) FROM NomVaca")
        Dim numero As Integer = If(maximo Is Nothing OrElse IsDBNull(maximo), 1, Convert.ToInt32(maximo) + 1)

        Dim ultima As Object = Escalar("SELECT MAX(FechaFin) FROM NomVaca")
        Dim n As New NominaPrestacion With {.Numero = numero, .Cod_Tipo_Nomina = codTipoNomina, .Existe = False}

        If ultima Is Nothing OrElse IsDBNull(ultima) Then
            Dim hoy As Date = Date.Today
            If hoy.Month <= 6 Then
                n.Fecha_Ini = New Date(hoy.Year, 1, 1) : n.Fecha_Fin = New Date(hoy.Year, 6, 30)
            Else
                n.Fecha_Ini = New Date(hoy.Year, 7, 1) : n.Fecha_Fin = New Date(hoy.Year, 12, 31)
            End If
        Else
            Dim fin As Date = CDate(ultima)
            If fin.Month <= 6 Then
                n.Fecha_Ini = New Date(fin.Year, 7, 1) : n.Fecha_Fin = New Date(fin.Year, 12, 31)
            Else
                n.Fecha_Ini = New Date(fin.Year + 1, 1, 1) : n.Fecha_Fin = New Date(fin.Year + 1, 6, 30)
            End If
        End If

        Return n

    End Function

    Public Function Buscar(numero As Integer) As NominaPrestacion

        Dim dt As DataTable = ConsultarTabla(
            "SELECT NumNomVaca, FechaAplica, MontoPagado, FechaIni, FechaFin, Activa, CodTipoNomina, Transfereir FROM NomVaca WHERE NumNomVaca = @Num",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})

        Return If(dt.Rows.Count = 0, Nothing, Mapear(dt.Rows(0)))

    End Function

    Private Shared Function Mapear(f As DataRow) As NominaPrestacion
        Return New NominaPrestacion With {
            .Numero = Convert.ToInt32(f("NumNomVaca")),
            .Fecha_Aplica = If(IsDBNull(f("FechaAplica")), Date.Today, CDate(f("FechaAplica"))),
            .Monto_Pagado = If(IsDBNull(f("MontoPagado")), 0, Convert.ToDouble(f("MontoPagado"))),
            .Fecha_Ini = CDate(f("FechaIni")),
            .Fecha_Fin = CDate(f("FechaFin")),
            .Activa = Not IsDBNull(f("Activa")) AndAlso CBool(f("Activa")),
            .Cod_Tipo_Nomina = If(IsDBNull(f("CodTipoNomina")), "", CStr(f("CodTipoNomina"))),
            .Transferir = Not IsDBNull(f("Transfereir")) AndAlso CBool(f("Transfereir")),
            .Existe = True}
    End Function

#End Region

#Region "CALCULAR"

    ''' <summary>
    ''' Calcula (o recalcula) la nómina de vacaciones: borra el detalle y el historial anteriores de la nómina si ya existía y graba
    ''' un detalle por empleado activo del tipo de nómina. Todo o nada.
    ''' </summary>
    ''' <param name="progreso">Se llama por cada empleado con (actual, total, texto); puede ser Nothing.</param>
    Public Function Calcular(p As ParametrosCalculo, progreso As Action(Of Integer, Integer, String)) As ResultadoCalculo

        If p.Fecha_Fin.Date < p.Fecha_Ini.Date Then Throw New ArgumentException("La fecha final de las vacaciones es menor que la inicial.")
        If p.Planilla_Fin.Date < p.Planilla_Ini.Date Then Throw New ArgumentException("El rango de planillas es incorrecto.")
        If p.Dias_Descuento < 0 OrElse p.Dias_Descuento > 15 Then Throw New ArgumentException("Los días de descuento deben estar entre 0 y 15.")
        If p.Fecha_Ini.Date > Date.Today Then Throw New ArgumentException("La fecha actual no coincide con la nómina: el período de vacaciones aún no empieza.")

        Dim tipo As DataTable = ConsultarTabla(
            "SELECT Periodo, PorcientoInss, TasaInss FROM TipoNomina WHERE CodTipoNomina = @Tipo",
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = p.Cod_Tipo_Nomina})
        If tipo.Rows.Count = 0 Then Throw New InvalidOperationException("No se encontró el tipo de nómina seleccionado.")

        Dim periodo As String = If(IsDBNull(tipo.Rows(0)("Periodo")), "", CStr(tipo.Rows(0)("Periodo")))
        Dim porcentaje As Boolean = Not IsDBNull(tipo.Rows(0)("PorcientoInss")) AndAlso CBool(tipo.Rows(0)("PorcientoInss"))

        Dim bajas As New CsBajasDAL
        Dim ctx As New CalculadoraNominaVacaciones.Contexto With {
            .Dias_Mes = DiasDelMes(), .Periodo = periodo,
            .Tasa_Cambio = bajas.TasaCambio(p.Fecha_Fin),
            .Tasa_Inss = If(porcentaje AndAlso Not IsDBNull(tipo.Rows(0)("TasaInss")), Convert.ToDouble(tipo.Rows(0)("TasaInss")), 0),
            .Antiguedad_Minima = p.Antiguedad_Minima, .Dias_Descuento = p.Dias_Descuento}

        Dim tramos As List(Of CalculadoraIRBaja.TramoIR) = bajas.TablaIR()
        Dim vacaciones As New CsVacacionesDAL
        Dim resultado As New ResultadoCalculo

        If progreso IsNot Nothing Then progreso(0, 0, "Cargando datos del período...")

        EnTransaccion(
            Sub(cn, tr)

                Dim empleados As List(Of CalculadoraNominaVacaciones.DatosEmpleado) = CargarEmpleados(cn, tr, p)

                PrepararCabecera(cn, tr, p)

                Dim id As Integer = Convert.ToInt32(EscalarEn(cn, tr, "SELECT ISNULL(MAX(Id), 0) + 1 FROM DetalleNomVaca WITH (TABLOCKX, HOLDLOCK)"))
                Dim total As Double = 0

                For i As Integer = 0 To empleados.Count - 1

                    Dim e As CalculadoraNominaVacaciones.DatosEmpleado = empleados(i)
                    If progreso IsNot Nothing Then progreso(i + 1, empleados.Count, e.Cod_Empleado1)

                    Dim resumen As ResumenVacaciones = vacaciones.CalcularResumen(e.Cod_Empleado1, p.Fecha_Fin)
                    If resumen IsNot Nothing Then
                        e.Dias_Saldo = resumen.Saldo
                        e.Dias_Acumulados = resumen.Acumulado
                    End If

                    e.Calcular_Ir = Function(salarioMensual) CalculadoraIRBaja.Calcular(periodo, tramos, salarioMensual, 0, 4)

                    Dim r As CalculadoraNominaVacaciones.Resultado = CalculadoraNominaVacaciones.Calcular(e, ctx)
                    If Not r.Procede OrElse Not r.Graba Then Continue For

                    total += r.Salario_Mensual
                    CsHistorialSalarialDAL.GuardarEn(cn, tr, TipoHistorial, e.Cod_Empleado, p.Fecha_Ini, p.Fecha_Fin, p.Numero, r.Salarios)
                    InsertarDetalle(cn, tr, id, p.Numero, e.Cod_Empleado, r)
                    id += 1
                    resultado.Grabados += 1

                Next

                resultado.Empleados = empleados.Count
                resultado.Monto_Pagado = total

                EjecutarEn(cn, tr, "UPDATE NomVaca SET MontoPagado = @Monto WHERE NumNomVaca = @Num AND Activa = 1",
                           New SqlParameter("@Monto", SqlDbType.Float) With {.Value = total},
                           New SqlParameter("@Num", SqlDbType.Int) With {.Value = p.Numero})

            End Sub)

        Return resultado

    End Function

    ''' <summary>Crea la nómina o, si ya existe y está abierta, actualiza sus datos y borra su detalle e historial. Una cerrada no se toca.</summary>
    Private Sub PrepararCabecera(cn As SqlConnection, tr As SqlTransaction, p As ParametrosCalculo)

        Dim existente As Object = EscalarEn(cn, tr, "SELECT Activa FROM NomVaca WITH (UPDLOCK) WHERE NumNomVaca = @Num",
                                            New SqlParameter("@Num", SqlDbType.Int) With {.Value = p.Numero})

        Dim parametros() As SqlParameter = {
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = p.Numero},
            New SqlParameter("@Aplica", SqlDbType.SmallDateTime) With {.Value = Date.Today},
            New SqlParameter("@Ini", SqlDbType.SmallDateTime) With {.Value = p.Fecha_Ini.Date},
            New SqlParameter("@Fin", SqlDbType.SmallDateTime) With {.Value = p.Fecha_Fin.Date},
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = p.Cod_Tipo_Nomina},
            New SqlParameter("@Transf", SqlDbType.Bit) With {.Value = p.Transferir}}

        If existente Is Nothing Then
            EjecutarEn(cn, tr,
                "INSERT INTO NomVaca (NumNomVaca, FechaAplica, MontoPagado, FechaIni, FechaFin, Activa, CodTipoNomina, Transfereir) " &
                "VALUES (@Num, @Aplica, 0, @Ini, @Fin, 1, @Tipo, @Transf)", parametros)
            Return
        End If

        If Not CBool(existente) Then Throw New InvalidOperationException($"La nómina de vacaciones {p.Numero} ya está cerrada.")

        EjecutarEn(cn, tr,
            "UPDATE NomVaca SET FechaAplica = @Aplica, MontoPagado = 0, FechaIni = @Ini, FechaFin = @Fin, CodTipoNomina = @Tipo, Transfereir = @Transf " &
            "WHERE NumNomVaca = @Num", parametros)

        EjecutarEn(cn, tr, "DELETE FROM DetalleNomVaca WHERE NumNomVaca = @Num", New SqlParameter("@Num", SqlDbType.Int) With {.Value = p.Numero})
        ' Solo el historial de vacaciones: el del aguinaldo puede tener el mismo número de nómina.
        EjecutarEn(cn, tr, "DELETE FROM HistorialSalarioMes WHERE NumNomina = @Num AND Tipo = @TipoH",
                   New SqlParameter("@Num", SqlDbType.Int) With {.Value = p.Numero},
                   New SqlParameter("@TipoH", SqlDbType.NVarChar, 50) With {.Value = TipoHistorial})

    End Sub

    Private Sub InsertarDetalle(cn As SqlConnection, tr As SqlTransaction, id As Integer, numero As Integer, codEmpleado As String,
                                r As CalculadoraNominaVacaciones.Resultado)

        EjecutarEn(cn, tr,
            "INSERT INTO DetalleNomVaca (Id, NumNomVaca, CodEmpleado, SalarioMensual, DiasAPagar, DiasDescuento, AdelantoVacaciones, Inss, TotalDevengado, Ir) " &
            "VALUES (@Id, @Num, @Cod, @Salario, @Dias, @Desc, @Adelanto, @Inss, @Total, @Ir)",
            New SqlParameter("@Id", SqlDbType.Int) With {.Value = id},
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero},
            New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = Convert.ToDecimal(codEmpleado)},
            New SqlParameter("@Salario", SqlDbType.Float) With {.Value = r.Salario_Mensual},
            New SqlParameter("@Dias", SqlDbType.Float) With {.Value = r.Dias_A_Pagar},
            New SqlParameter("@Desc", SqlDbType.Float) With {.Value = r.Dias_Descuento},
            New SqlParameter("@Adelanto", SqlDbType.Money) With {.Value = CDec(r.Adelanto)},
            New SqlParameter("@Inss", SqlDbType.Float) With {.Value = r.Inss},
            New SqlParameter("@Total", SqlDbType.Float) With {.Value = r.Total_Devengado},
            New SqlParameter("@Ir", SqlDbType.Float) With {.Value = r.Ir})

    End Sub

#End Region

#Region "CARGA DE DATOS DEL PERIODO"

    ''' <summary>Lee, de una vez para todos los empleados del tipo de nómina, lo que necesita el cálculo (sin los días de vacaciones, que se calculan por empleado).</summary>
    Private Function CargarEmpleados(cn As SqlConnection, tr As SqlTransaction, p As ParametrosCalculo) As List(Of CalculadoraNominaVacaciones.DatosEmpleado)

        Dim lista As New List(Of CalculadoraNominaVacaciones.DatosEmpleado)
        Dim porCodigo As New Dictionary(Of String, CalculadoraNominaVacaciones.DatosEmpleado)

        For Each f As DataRow In TablaEn(cn, tr,
            "SELECT E.CodEmpleado, E.CodEmpleado1, E.SalarioFijo, E.SueldoPeriodo, E.TarifaHoraria, E.SueldoActualBasico, H.SueldoActual " &
            "FROM Empleado E OUTER APPLY (SELECT TOP 1 FechaContrato, SueldoActual FROM Historico WHERE CodEmpleado = E.CodEmpleado) H " &
            "WHERE E.CodTipoNomina = @Tipo AND E.Activo = 1 AND ISNULL(E.Ausente, 0) = 0 AND H.FechaContrato <= @FinPlanilla " &
            "ORDER BY E.CodEmpleado1", Tipo(p), New SqlParameter("@FinPlanilla", SqlDbType.DateTime) With {.Value = p.Planilla_Fin.Date}).Rows

            Dim e As New CalculadoraNominaVacaciones.DatosEmpleado With {
                .Cod_Empleado = Convert.ToDecimal(f("CodEmpleado")).ToString("0"),
                .Cod_Empleado1 = CStr(f("CodEmpleado1")),
                .Salario_Variable = Not IsDBNull(f("SalarioFijo")) AndAlso CStr(f("SalarioFijo")) = "N",
                .Sueldo_Periodo = Numero(f("SueldoPeriodo")),
                .Tarifa_Horaria = Numero(f("TarifaHoraria")),
                .Sueldo_Actual_Basico = Not IsDBNull(f("SueldoActualBasico")) AndAlso CBool(f("SueldoActualBasico")),
                .Sueldo_Actual = If(IsDBNull(f("SueldoActual")), CType(Nothing, Double?), Convert.ToDouble(f("SueldoActual")))}

            lista.Add(e)
            porCodigo(e.Cod_Empleado) = e
        Next

        ' Meses de planilla del rango, con su cantidad de períodos.
        Dim meses As New List(Of Tuple(Of Integer, Integer, Integer))
        For Each f As DataRow In TablaEn(cn, tr,
            "SELECT [año] AS Anio, CONVERT(int, mes) AS Mes, COUNT(*) AS Cont FROM Fecha_Planilla " &
            "WHERE CodTipoNomina = @Tipo AND Inicio >= @Ini AND Final <= @Fin AND ISNUMERIC(mes) = 1 " &
            "GROUP BY [año], mes ORDER BY [año], CONVERT(int, mes)", Tipo(p),
            New SqlParameter("@Ini", SqlDbType.DateTime) With {.Value = p.Planilla_Ini.Date},
            New SqlParameter("@Fin", SqlDbType.DateTime) With {.Value = p.Planilla_Fin.Date}).Rows
            meses.Add(Tuple.Create(Convert.ToInt32(f("Anio")), Convert.ToInt32(f("Mes")), Convert.ToInt32(f("Cont"))))
        Next

        If meses.Count > 0 Then

            ' Los años y meses son enteros de la propia base de datos; no hay texto del usuario en esta lista.
            Dim claves As String = String.Join(", ", meses.Select(Function(m) (m.Item1 * 100 + m.Item2).ToString()))

            Dim totales As New Dictionary(Of String, Double)
            For Each f As DataRow In TablaEn(cn, tr,
                "SELECT D.CodEmpleado, N.Ano, N.Mes, " &
                "SUM(ISNULL(D.SalarioBasico,0) + ISNULL(D.Destajo,0) + ISNULL(D.SeptimoDia,0) + ISNULL(D.OtrosIngresos,0) + ISNULL(D.Incentivos,0) + " &
                "    ISNULL(D.IncetivoProduccion,0) + ISNULL(D.HorasExtras,0)) AS Total " &
                "FROM Nomina N INNER JOIN DetalleNomina D ON N.NumNomina = D.NumNomina " &
                "INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
                "WHERE E.CodTipoNomina = @Tipo AND E.Activo = 1 AND N.Ano * 100 + N.Mes IN (" & claves & ") " &
                "GROUP BY D.CodEmpleado, N.Ano, N.Mes HAVING SUM(ISNULL(D.SalarioBasico,0) + ISNULL(D.Destajo,0)) <> 0", Tipo(p)).Rows
                totales(Convert.ToDecimal(f("CodEmpleado")).ToString("0") & "|" & Convert.ToInt32(f("Ano")) * 100 + Convert.ToInt32(f("Mes"))) = Numero(f("Total"))
            Next

            For Each e As CalculadoraNominaVacaciones.DatosEmpleado In lista
                For Each m In meses
                    Dim valor As Double = 0
                    totales.TryGetValue(e.Cod_Empleado & "|" & (m.Item1 * 100 + m.Item2), valor)
                    e.Meses.Add(New CalculadoraNominaVacaciones.MesIngreso With {.Ano = m.Item1, .Mes = m.Item2, .Total = valor, .Cant_Planilla = m.Item3})
                Next
            Next

        End If

        ' Ingreso, INSS e IR del mes más reciente con nómina dentro del rango (base del IR de las vacaciones).
        Dim ultimo As New Dictionary(Of String, Integer)
        For Each f As DataRow In TablaEn(cn, tr,
            "SELECT D.CodEmpleado, N.Ano, N.Mes, " &
            "SUM(ISNULL(D.SalarioBasico,0) + ISNULL(D.Destajo,0) + ISNULL(D.Comisiones,0) + ISNULL(D.HorasExtras,0) + ISNULL(D.BonoProduccion,0) + " &
            "    ISNULL(D.SeptimoDia,0) + ISNULL(D.OtrosIngresos,0) + ISNULL(D.IncetivoProduccion,0)) AS Ingreso, " &
            "SUM(ISNULL(D.MontoINSS,0)) AS Inss, SUM(ISNULL(D.MontoIR,0)) AS Ir " &
            "FROM Nomina N INNER JOIN DetalleNomina D ON N.NumNomina = D.NumNomina " &
            "INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "WHERE E.CodTipoNomina = @Tipo AND E.Activo = 1 " &
            "GROUP BY D.CodEmpleado, N.Ano, N.Mes " &
            "HAVING SUM(ISNULL(D.SalarioBasico,0) + ISNULL(D.Destajo,0)) <> 0 AND MAX(N.FechaNomina) <= @Fin AND MIN(N.FechaNominaINI) >= @Ini", Tipo(p),
            New SqlParameter("@Ini", SqlDbType.DateTime) With {.Value = p.Planilla_Ini.Date},
            New SqlParameter("@Fin", SqlDbType.DateTime) With {.Value = p.Planilla_Fin.Date}).Rows

            Dim e As CalculadoraNominaVacaciones.DatosEmpleado = Nothing
            Dim codigo As String = Convert.ToDecimal(f("CodEmpleado")).ToString("0")
            If Not porCodigo.TryGetValue(codigo, e) Then Continue For

            Dim clave As Integer = Convert.ToInt32(f("Ano")) * 100 + Convert.ToInt32(f("Mes"))
            If ultimo.ContainsKey(codigo) AndAlso ultimo(codigo) > clave Then Continue For
            ultimo(codigo) = clave

            e.Ingreso_Ultimo_Mes = Numero(f("Ingreso"))
            e.Inss_Ultimo_Mes = Numero(f("Inss"))
            e.Ir_Ultimo_Mes = Numero(f("Ir"))
        Next

        ' Adelantos de vacaciones del rango de planillas
        For Each f As DataRow In TablaEn(cn, tr,
            "SELECT A.CodEmpleado, SUM(A.MontoAdelanto) AS Monto FROM Adelanto13vo A INNER JOIN Empleado E ON E.CodEmpleado = A.CodEmpleado " &
            "WHERE E.CodTipoNomina = @Tipo AND E.Activo = 1 AND A.TipoAdelanto = 'Vacaciones' " &
            "AND A.FechaAdelanto >= @Ini AND A.FechaAdelanto < @FinMas1 GROUP BY A.CodEmpleado", Tipo(p),
            New SqlParameter("@Ini", SqlDbType.DateTime) With {.Value = p.Planilla_Ini.Date},
            New SqlParameter("@FinMas1", SqlDbType.DateTime) With {.Value = p.Planilla_Fin.Date.AddDays(1)}).Rows
            Dim e As CalculadoraNominaVacaciones.DatosEmpleado = Nothing
            If porCodigo.TryGetValue(Convert.ToDecimal(f("CodEmpleado")).ToString("0"), e) Then e.Adelanto = Numero(f("Monto"))
        Next

        ' Vacaciones ya aprobadas dentro del período de la nómina
        Dim porVisible As Dictionary(Of String, CalculadoraNominaVacaciones.DatosEmpleado) =
            lista.ToDictionary(Function(x) x.Cod_Empleado1, StringComparer.OrdinalIgnoreCase)
        For Each f As DataRow In TablaEn(cn, tr,
            "SELECT CodigoEmpleado, SUM(DiasDisfrutar) AS Dias FROM SolicitudVacaciones " &
            "WHERE TipoSolicitud = 'Vacaciones' AND ISNULL(Anulado, 0) = 0 AND FechaInicio >= @Ini AND FechaFin <= @Fin GROUP BY CodigoEmpleado",
            New SqlParameter("@Ini", SqlDbType.DateTime) With {.Value = p.Fecha_Ini.Date},
            New SqlParameter("@Fin", SqlDbType.DateTime) With {.Value = p.Fecha_Fin.Date}).Rows
            Dim e As CalculadoraNominaVacaciones.DatosEmpleado = Nothing
            If porVisible.TryGetValue(CStr(f("CodigoEmpleado")), e) Then e.Dias_Solicitados = Numero(f("Dias"))
        Next

        Return lista

    End Function

    Private Shared Function DiasDelMes() As Double
        Dim c As Controles = New CsControlesDAL().ObtenerControles()
        Return If(c Is Nothing OrElse c.Dias_Mes <= 0, 30.0, c.Dias_Mes)
    End Function

    Private Shared Function Tipo(p As ParametrosCalculo) As SqlParameter
        Return New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = p.Cod_Tipo_Nomina}
    End Function

    Private Shared Function Numero(valor As Object) As Double
        Return If(IsDBNull(valor) OrElse valor Is Nothing, 0, Convert.ToDouble(valor))
    End Function

    Private Shared Function TablaEn(cn As SqlConnection, tr As SqlTransaction, sql As String, ParamArray parametros() As SqlParameter) As DataTable
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

#Region "CONSULTAS DE LA NOMINA"

    Private Const NombreCompleto As String =
        "RTRIM(ISNULL(E.Nombre1 + ' ', '') + ISNULL(E.Nombre2 + ' ', '') + ISNULL(E.Apellido1 + ' ', '') + ISNULL(E.Apellido2, ''))"

    ''' <summary>Monto a pagar de un detalle: salario por los días a pagar menos el descuento, entre los días del mes, menos el adelanto.</summary>
    Private Const SqlTotal As String = "ISNULL(D.SalarioMensual, 0) * (ISNULL(D.DiasAPagar, 0) - ISNULL(D.DiasDescuento, 0)) / @DiasMes"

    ''' <summary>
    ''' Detalle para la cuadrícula: Codigo, Nombres, SalarioMensual, DiasAPagar, DiasDescuento, Adelanto, Inss, Ir, TotalDevengado y
    ''' MontoPagar (= TotalDevengado - Adelanto - Inss - Ir cuando <paramref name="restarImpuestos"/>; si no, TotalDevengado - Adelanto).
    ''' </summary>
    Public Function ListarDetalle(numero As Integer, restarImpuestos As Boolean) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & NombreCompleto & " AS Nombres, " &
            "D.SalarioMensual, D.DiasAPagar, D.DiasDescuento, ISNULL(D.AdelantoVacaciones, 0) AS Adelanto, ISNULL(D.Inss, 0) AS Inss, ISNULL(D.Ir, 0) AS Ir, " &
            SqlTotal & " AS TotalDevengado, " & SqlMontoPagar(restarImpuestos) & " AS MontoPagar " &
            "FROM DetalleNomVaca D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado WHERE D.NumNomVaca = @Num ORDER BY E.CodEmpleado1",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero}, DiasMesParametro())
    End Function

    ''' <summary>
    ''' Datos de la colilla y de la nómina impresa. Columnas: Codigo, Nombres, Cargo, CodDepartamento, Departamento, FechaIngreso, NumeroInss,
    ''' Cedula, SalarioMensual, DiasAPagar, DiasDescuento, TotalDias, Adelanto, Inss, Ir, TotalDevengado, TotalDeducir y MontoPagar.
    ''' Solo los empleados con días por pagar. Con <paramref name="codDepartamento"/> vacío trae todos los departamentos.
    ''' </summary>
    Public Function ListarParaReporte(numero As Integer, restarImpuestos As Boolean, codDepartamento As String) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & NombreCompleto & " AS Nombres, C.Cargo, " &
            "ISNULL(E.CodDepartamento, '') AS CodDepartamento, ISNULL(DP.Departamento, 'Sin departamento') AS Departamento, H.FechaContrato AS FechaIngreso, " &
            "E.NumeroInss, E.NumCedula AS Cedula, ISNULL(D.SalarioMensual, 0) AS SalarioMensual, ISNULL(D.DiasAPagar, 0) AS DiasAPagar, " &
            "ISNULL(D.DiasDescuento, 0) AS DiasDescuento, ISNULL(D.DiasAPagar, 0) - ISNULL(D.DiasDescuento, 0) AS TotalDias, " &
            "ISNULL(D.AdelantoVacaciones, 0) AS Adelanto, ISNULL(D.Inss, 0) AS Inss, ISNULL(D.Ir, 0) AS Ir, " &
            SqlTotal & " AS TotalDevengado, " &
            "ISNULL(D.AdelantoVacaciones, 0)" & If(restarImpuestos, " + ISNULL(D.Inss, 0) + ISNULL(D.Ir, 0)", "") & " AS TotalDeducir, " &
            SqlMontoPagar(restarImpuestos) & " AS MontoPagar " &
            "FROM DetalleNomVaca D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "LEFT JOIN Cargo C ON C.CodCargo = E.CodCargo " &
            "LEFT JOIN Departamento DP ON DP.CodDepartamento = E.CodDepartamento " &
            "OUTER APPLY (SELECT TOP 1 FechaContrato FROM Historico WHERE CodEmpleado = E.CodEmpleado) H " &
            "WHERE D.NumNomVaca = @Num AND ISNULL(D.DiasAPagar, 0) - ISNULL(D.DiasDescuento, 0) > 0 AND (@Dpto = '' OR E.CodDepartamento = @Dpto) " &
            "ORDER BY Departamento, Nombres",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero},
            New SqlParameter("@Dpto", SqlDbType.NVarChar, 10) With {.Value = If(codDepartamento, "")}, DiasMesParametro())
    End Function

    Public Function ListarDepartamentos(numero As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT DISTINCT ISNULL(E.CodDepartamento, '') AS CodDepartamento, ISNULL(DP.Departamento, 'Sin departamento') AS Departamento " &
            "FROM DetalleNomVaca D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "LEFT JOIN Departamento DP ON DP.CodDepartamento = E.CodDepartamento WHERE D.NumNomVaca = @Num ORDER BY CodDepartamento",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})
    End Function

    ''' <summary>Pago por empleado para la planilla del banco: Codigo, Nombres, Monto, NumCedula y las cuentas. Solo con monto distinto de cero.</summary>
    Public Function ListarParaBanco(numero As Integer, restarImpuestos As Boolean) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & NombreCompleto & " AS Nombres, " & SqlMontoPagar(restarImpuestos) & " AS Monto, " &
            "E.NumCedula, H.CuentaBanco, E.CuentaBancoBANPRO, E.CuentaBancoBDF, E.CuentaBancoLAFISE " &
            "FROM DetalleNomVaca D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "OUTER APPLY (SELECT TOP 1 CuentaBanco FROM Historico WHERE CodEmpleado = E.CodEmpleado) H " &
            "WHERE D.NumNomVaca = @Num AND " & SqlTotal & " <> 0 ORDER BY E.CodEmpleado1",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero}, DiasMesParametro())
    End Function

    Public Function ListarMontos(numero As Integer, restarImpuestos As Boolean) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & NombreCompleto & " AS Nombres, " & SqlMontoPagar(restarImpuestos) & " AS Monto " &
            "FROM DetalleNomVaca D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "WHERE D.NumNomVaca = @Num AND " & SqlTotal & " <> 0 ORDER BY E.CodEmpleado1",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero}, DiasMesParametro())
    End Function

    Private Shared Function SqlMontoPagar(restarImpuestos As Boolean) As String
        Return "(" & SqlTotal & " - ISNULL(D.AdelantoVacaciones, 0)" & If(restarImpuestos, " - ISNULL(D.Inss, 0) - ISNULL(D.Ir, 0)", "") & ")"
    End Function

    Private Function DiasMesParametro() As SqlParameter
        Return New SqlParameter("@DiasMes", SqlDbType.Float) With {.Value = DiasDelMes()}
    End Function

#End Region

#Region "LISTADO"

    ''' <summary>
    ''' Todas las nóminas de vacaciones, de la más reciente a la más antigua, para el listado histórico. Columnas: Numero, CodTipoNomina,
    ''' Tipo, FechaIni, FechaFin, FechaAplica, MontoPagado y Estado ("Abierta" o "Cerrada").
    ''' </summary>
    Public Function Listar() As DataTable
        Return ConsultarTabla(
            "SELECT N.NumNomVaca AS Numero, N.CodTipoNomina, T.Nomina AS Tipo, N.FechaIni, N.FechaFin, N.FechaAplica, " &
            "ISNULL(N.MontoPagado, 0) AS MontoPagado, CASE WHEN ISNULL(N.Activa, 0) = 1 THEN 'Abierta' ELSE 'Cerrada' END AS Estado " &
            "FROM NomVaca N LEFT JOIN TipoNomina T ON T.CodTipoNomina = N.CodTipoNomina ORDER BY N.NumNomVaca DESC")
    End Function

#End Region

#Region "CERRAR"

    ''' <summary>
    ''' Cierra la nómina de vacaciones (ya no se puede recalcular). Si no es "sin transferir", pasa a la nómina activa y procesada del
    ''' tipo de nómina los días, el monto y los adelantos de vacaciones de cada empleado (DetalleNomina.DiasVacaciones, VacacionesPagadas,
    ''' AdelantosVacaciones). Devuelve falso si la nómina no existe o ya estaba cerrada.
    ''' </summary>
    Public Function Cerrar(numero As Integer, transferir As Boolean) As Boolean

        Dim cerrada As Boolean = False
        Dim diasMes As Double = DiasDelMes()

        EnTransaccion(
            Sub(cn, tr)

                Dim cab As DataTable = TablaEn(cn, tr, "SELECT CodTipoNomina, Activa FROM NomVaca WITH (UPDLOCK) WHERE NumNomVaca = @Num",
                                               New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})
                If cab.Rows.Count = 0 OrElse IsDBNull(cab.Rows(0)("Activa")) OrElse Not CBool(cab.Rows(0)("Activa")) Then Return

                If transferir Then
                    Dim nomina As Object = EscalarEn(cn, tr,
                        "SELECT TOP 1 NumNomina FROM Nomina WHERE CodTipoNomina = @Tipo AND Activa = 1 AND Procesada = 1 ORDER BY NumNomina DESC",
                        New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = CStr(cab.Rows(0)("CodTipoNomina"))})

                    If nomina Is Nothing OrElse IsDBNull(nomina) Then
                        Throw New InvalidOperationException(
                            "No existe ninguna nómina activa y calculada de este tipo. Active la nómina y calcúlela, o cierre sin transferir.")
                    End If

                    EjecutarEn(cn, tr,
                        "UPDATE N SET N.DiasVacaciones = ISNULL(V.DiasAPagar, 0) - ISNULL(V.DiasDescuento, 0), " &
                        "N.VacacionesPagadas = ISNULL(V.SalarioMensual, 0) * (ISNULL(V.DiasAPagar, 0) - ISNULL(V.DiasDescuento, 0)) / @DiasMes - ISNULL(V.AdelantoVacaciones, 0), " &
                        "N.AdelantosVacaciones = ISNULL(V.AdelantoVacaciones, 0) " &
                        "FROM DetalleNomina N INNER JOIN DetalleNomVaca V ON V.CodEmpleado = N.CodEmpleado " &
                        "INNER JOIN Empleado E ON E.CodEmpleado = N.CodEmpleado " &
                        "WHERE N.NumNomina = @NumNomina AND V.NumNomVaca = @Num AND E.Activo = 1",
                        New SqlParameter("@NumNomina", SqlDbType.Int) With {.Value = Convert.ToInt32(nomina)},
                        New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero},
                        New SqlParameter("@DiasMes", SqlDbType.Float) With {.Value = diasMes})
                End If

                EjecutarEn(cn, tr, "UPDATE NomVaca SET Activa = 0 WHERE NumNomVaca = @Num",
                           New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})
                cerrada = True

            End Sub)

        Return cerrada

    End Function

#End Region

End Class
