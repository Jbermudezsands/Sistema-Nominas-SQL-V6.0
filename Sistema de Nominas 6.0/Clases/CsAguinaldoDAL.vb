Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Nómina de 13vo mes (aguinaldo): V.5.0 Frm13VacaMes, parte del 13vo (tablas Nom13Mes, DetalleNom13Mes y
''' HistorialSalarioMes con Tipo = 'Aguinaldo'). Aquí está todo el acceso a datos; las reglas de cálculo están en
''' <see cref="CalculadoraAguinaldo"/>. Todo parametrizado; calcular y cerrar trabajan en una transacción.
''' </summary>
Public Class CsAguinaldoDAL
    Inherits CsDALBase

    ''' <summary>
    ''' Empleados cuyo 13vo no suma comisiones ni séptimo día: están escritos en el código de la V.5.0 (cálculo optimizado de
    ''' <c>CargarDatosAguinaldo</c>, tomado de <c>LlenarHistorialAguinaldo</c>). Se conservan para no cambiar su pago; conviene
    ''' reemplazarlos por una opción del empleado.
    ''' </summary>
    Private Shared ReadOnly EmpleadosSinComisionNiSeptimo As Decimal() = {10709D, 10796D}

    Private Const TipoHistorial As String = "Aguinaldo"

    Public Class ResultadoCalculo
        Public Property Empleados As Integer
        Public Property Grabados As Integer
        ''' <summary>Suma del salario mensual neto de cada empleado (así lo guardaba la V.5.0 en Nom13Mes.MontoPagado).</summary>
        Public Property Monto_Pagado As Double
    End Class

#Region "CATALOGOS Y PERIODO"

    ''' <summary>Tipos de nómina: CodTipoNomina, Nomina (nombre), Periodo y TipoPago.</summary>
    Public Function ListarTiposNomina() As DataTable
        Return ConsultarTabla("SELECT CodTipoNomina, Nomina, Periodo, TipoPago FROM TipoNomina ORDER BY Nomina")
    End Function

    Public Function ObtenerDiasMes() As Double
        Dim r As Object = Escalar("SELECT TOP 1 DiasMes FROM Controles ORDER BY Id")
        Return If(r Is Nothing OrElse IsDBNull(r), 30.0, Convert.ToDouble(r))
    End Function

    ''' <summary>
    ''' Nómina con la que abre la pantalla para el tipo de nómina: la abierta (la última si hay varias); si no hay, la siguiente
    ''' del último 13vo (1 dic a 30 nov del año siguiente); si nunca se hizo uno, el período que termina el 30 de noviembre del año actual.
    ''' </summary>
    Public Function Proponer(codTipoNomina As String) As NominaPrestacion

        Dim abierta As DataTable = ConsultarTabla(
            "SELECT TOP 1 NumNom13Mes, FechaAplica, MontoPagado, FechaIni, FechaFin, Activa, CodTipoNomina FROM Nom13Mes " &
            "WHERE Activa = 1 AND CodTipoNomina = @Tipo ORDER BY NumNom13Mes DESC",
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = codTipoNomina})

        If abierta.Rows.Count > 0 Then Return Mapear(abierta.Rows(0), True)

        Dim ultima As DataTable = ConsultarTabla("SELECT TOP 1 FechaFin FROM Nom13Mes ORDER BY FechaFin DESC")
        Dim maximo As Object = Escalar("SELECT MAX(NumNom13Mes) FROM Nom13Mes")
        Dim numero As Integer = If(maximo Is Nothing OrElse IsDBNull(maximo), 1, Convert.ToInt32(maximo) + 1)

        Dim anioFin As Integer = If(ultima.Rows.Count > 0 AndAlso Not IsDBNull(ultima.Rows(0)("FechaFin")),
                                    CDate(ultima.Rows(0)("FechaFin")).Year, Date.Today.Year - 1)

        Return New NominaPrestacion With {
            .Numero = numero, .Cod_Tipo_Nomina = codTipoNomina, .Existe = False,
            .Fecha_Ini = New Date(anioFin, 12, 1), .Fecha_Fin = New Date(anioFin + 1, 11, 30)}

    End Function

    Public Function Buscar(numero As Integer) As NominaPrestacion

        Dim dt As DataTable = ConsultarTabla(
            "SELECT NumNom13Mes, FechaAplica, MontoPagado, FechaIni, FechaFin, Activa, CodTipoNomina FROM Nom13Mes WHERE NumNom13Mes = @Num",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})

        Return If(dt.Rows.Count = 0, Nothing, Mapear(dt.Rows(0), True))

    End Function

    Private Shared Function Mapear(f As DataRow, existe As Boolean) As NominaPrestacion
        Return New NominaPrestacion With {
            .Numero = Convert.ToInt32(f("NumNom13Mes")),
            .Fecha_Aplica = If(IsDBNull(f("FechaAplica")), Date.Today, CDate(f("FechaAplica"))),
            .Monto_Pagado = If(IsDBNull(f("MontoPagado")), 0, Convert.ToDouble(f("MontoPagado"))),
            .Fecha_Ini = CDate(f("FechaIni")),
            .Fecha_Fin = CDate(f("FechaFin")),
            .Activa = Not IsDBNull(f("Activa")) AndAlso CBool(f("Activa")),
            .Cod_Tipo_Nomina = If(IsDBNull(f("CodTipoNomina")), "", CStr(f("CodTipoNomina"))),
            .Existe = existe}
    End Function

#End Region

#Region "CALCULAR"

    ''' <summary>
    ''' Calcula (o recalcula) la nómina de 13vo mes <paramref name="numero"/>: borra el detalle anterior si la nómina ya existía y
    ''' graba un detalle por empleado activo del tipo de nómina, más su historial de salarios por mes. Todo o nada.
    ''' </summary>
    ''' <param name="progreso">Se llama por cada empleado con (actual, total, texto); puede ser Nothing.</param>
    Public Function Calcular(codTipoNomina As String, numero As Integer, fechaIni As Date, fechaFin As Date,
                             antiguedadMinima As Double, progreso As Action(Of Integer, Integer, String)) As ResultadoCalculo

        If fechaFin.Date < fechaIni.Date Then Throw New ArgumentException("La fecha final es menor que la inicial.")

        Dim tipo As DataTable = ConsultarTabla("SELECT Periodo, TipoPago FROM TipoNomina WHERE CodTipoNomina = @Tipo",
                                               New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = codTipoNomina})
        If tipo.Rows.Count = 0 Then Throw New InvalidOperationException("No se encontró el tipo de nómina seleccionado.")

        Dim diasMes As Double = ObtenerDiasMes()
        Dim resultado As New ResultadoCalculo

        If progreso IsNot Nothing Then progreso(0, 0, "Actualizando sueldos actuales...")
        Call New CsBajasDAL().ActualizarSueldoActualPorTipo(codTipoNomina, diasMes)

        Dim ctx As New CalculadoraAguinaldo.Contexto With {
            .Fecha_Ini = fechaIni.Date, .Fecha_Fin = fechaFin.Date,
            .Mes_Fin = fechaFin.Month, .Mes_Ini = If(fechaFin.Month > 6, fechaFin.Month - 5, 1),
            .Periodo = If(IsDBNull(tipo.Rows(0)("Periodo")), "", CStr(tipo.Rows(0)("Periodo"))),
            .Tipo_Pago = If(IsDBNull(tipo.Rows(0)("TipoPago")), "", CStr(tipo.Rows(0)("TipoPago"))),
            .Dias_Mes = diasMes, .Antiguedad_Minima = antiguedadMinima}

        EnTransaccion(
            Sub(cn, tr)

                If progreso IsNot Nothing Then progreso(0, 0, "Cargando datos del período...")

                Dim empleados As List(Of CalculadoraAguinaldo.DatosEmpleado) = CargarEmpleados(cn, tr, codTipoNomina, ctx, fechaIni, fechaFin)

                PrepararCabecera(cn, tr, numero, codTipoNomina, fechaIni, fechaFin)

                Dim id As Integer = Convert.ToInt32(EscalarEn(cn, tr, "SELECT ISNULL(MAX(Id), 0) + 1 FROM DetalleNom13Mes WITH (TABLOCKX, HOLDLOCK)"))
                Dim total As Double = 0

                For i As Integer = 0 To empleados.Count - 1

                    Dim e As CalculadoraAguinaldo.DatosEmpleado = empleados(i)
                    If progreso IsNot Nothing Then progreso(i + 1, empleados.Count, $"{e.Cod_Empleado1}")

                    Dim r As CalculadoraAguinaldo.Resultado = CalculadoraAguinaldo.Calcular(e, ctx)
                    If Not r.Procede Then Continue For

                    total += r.Neto_Mensual
                    GuardarHistorial(cn, tr, e.Cod_Empleado, fechaIni, fechaFin, numero, r.Salarios)

                    If r.Graba Then
                        InsertarDetalle(cn, tr, id, numero, e.Cod_Empleado, r)
                        id += 1
                        resultado.Grabados += 1
                    End If

                Next

                resultado.Empleados = empleados.Count
                resultado.Monto_Pagado = total

                EjecutarEn(cn, tr, "UPDATE Nom13Mes SET MontoPagado = @Monto WHERE NumNom13Mes = @Num AND Activa = 1",
                           New SqlParameter("@Monto", SqlDbType.Float) With {.Value = total},
                           New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})

            End Sub)

        Return resultado

    End Function

    ''' <summary>Crea la nómina o, si ya existe y está abierta, actualiza sus fechas y borra su detalle. Una nómina cerrada no se toca.</summary>
    Private Sub PrepararCabecera(cn As SqlConnection, tr As SqlTransaction, numero As Integer, codTipoNomina As String,
                                 fechaIni As Date, fechaFin As Date)

        Dim existente As Object = EscalarEn(cn, tr, "SELECT Activa FROM Nom13Mes WITH (UPDLOCK) WHERE NumNom13Mes = @Num",
                                            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})

        Dim parametros() As SqlParameter = {
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero},
            New SqlParameter("@Aplica", SqlDbType.SmallDateTime) With {.Value = Date.Today},
            New SqlParameter("@Ini", SqlDbType.SmallDateTime) With {.Value = fechaIni.Date},
            New SqlParameter("@Fin", SqlDbType.SmallDateTime) With {.Value = fechaFin.Date},
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = codTipoNomina}}

        If existente Is Nothing Then
            EjecutarEn(cn, tr,
                "INSERT INTO Nom13Mes (NumNom13Mes, FechaAplica, MontoPagado, FechaIni, FechaFin, Activa, CodTipoNomina) " &
                "VALUES (@Num, @Aplica, 0, @Ini, @Fin, 1, @Tipo)", parametros)
            Return
        End If

        If Not CBool(existente) Then Throw New InvalidOperationException($"La nómina de 13vo mes {numero} ya está cerrada.")

        EjecutarEn(cn, tr,
            "UPDATE Nom13Mes SET FechaAplica = @Aplica, MontoPagado = 0, FechaIni = @Ini, FechaFin = @Fin, CodTipoNomina = @Tipo " &
            "WHERE NumNom13Mes = @Num", parametros)

        EjecutarEn(cn, tr, "DELETE FROM DetalleNom13Mes WHERE NumNom13Mes = @Num",
                   New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})

    End Sub

    Private Sub InsertarDetalle(cn As SqlConnection, tr As SqlTransaction, id As Integer, numero As Integer,
                                codEmpleado As String, r As CalculadoraAguinaldo.Resultado)

        EjecutarEn(cn, tr,
            "INSERT INTO DetalleNom13Mes (Id, NumNom13Mes, CodEmpleado, SalarioMensual, SalarioAPagar, DiasAPagar, Adelanto13vo, " &
            "MontoPension, PorcentajePension, MontoSuspension, DiasSuspension, TotalDeducciones) " &
            "VALUES (@Id, @Num, @Cod, @Salario, @APagar, @Dias, @Adelanto, @Pension, @Pct, @MontoSusp, @DiasSusp, @TotalDed)",
            New SqlParameter("@Id", SqlDbType.Int) With {.Value = id},
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero},
            New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = Convert.ToDecimal(codEmpleado)},
            New SqlParameter("@Salario", SqlDbType.Float) With {.Value = r.Salario_Mensual},
            New SqlParameter("@APagar", SqlDbType.Float) With {.Value = r.Salario_A_Pagar},
            New SqlParameter("@Dias", SqlDbType.Float) With {.Value = r.Dias},
            New SqlParameter("@Adelanto", SqlDbType.Money) With {.Value = CDec(r.Adelanto)},
            New SqlParameter("@Pension", SqlDbType.Float) With {.Value = Math.Round(r.Monto_Pension, 2)},
            New SqlParameter("@Pct", SqlDbType.Float) With {.Value = Math.Round(r.Monto_Porcentaje, 2)},
            New SqlParameter("@MontoSusp", SqlDbType.Float) With {.Value = r.Monto_Suspension},
            New SqlParameter("@DiasSusp", SqlDbType.Float) With {.Value = r.Dias_Suspension},
            New SqlParameter("@TotalDed", SqlDbType.Float) With {.Value = r.Monto_Pension + r.Monto_Porcentaje})

    End Sub

    Private Sub GuardarHistorial(cn As SqlConnection, tr As SqlTransaction, codEmpleado As String, fechaIni As Date,
                                 fechaFin As Date, numero As Integer, salarios As Double())
        CsHistorialSalarialDAL.GuardarEn(cn, tr, TipoHistorial, codEmpleado, fechaIni, fechaFin, numero, salarios)
    End Sub

#End Region

#Region "CARGA DE DATOS DEL PERIODO"

    ''' <summary>
    ''' Lee, de una vez para todos los empleados activos del tipo de nómina, lo que necesita el cálculo: ficha, nómina de cada mes,
    ''' subsidios, adelantos y suspensiones (el equivalente a <c>CargarDatosAguinaldo</c> de la V.5.0).
    ''' </summary>
    Private Function CargarEmpleados(cn As SqlConnection, tr As SqlTransaction, codTipoNomina As String,
                                     ctx As CalculadoraAguinaldo.Contexto, fechaIni As Date, fechaFin As Date) As List(Of CalculadoraAguinaldo.DatosEmpleado)

        Dim tipo As New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = codTipoNomina}
        Dim lista As New List(Of CalculadoraAguinaldo.DatosEmpleado)
        Dim porCodigo As New Dictionary(Of String, CalculadoraAguinaldo.DatosEmpleado)

        For Each f As DataRow In TablaEn(cn, tr,
            "SELECT E.CodEmpleado, E.CodEmpleado1, E.SalarioFijo, E.SueldoPeriodo, E.TarifaHoraria, E.MontoPension, E.PorcentajePension, " &
            "E.SueldoActualBasico, E.SalPorcentaje, H.FechaContrato, H.SueldoActual " &
            "FROM Empleado E OUTER APPLY (SELECT TOP 1 FechaContrato, SueldoActual FROM Historico WHERE CodEmpleado = E.CodEmpleado) H " &
            "WHERE E.CodTipoNomina = @Tipo AND E.Activo = 1 ORDER BY E.CodEmpleado1", Copia(tipo)).Rows

            Dim e As New CalculadoraAguinaldo.DatosEmpleado With {
                .Cod_Empleado = Convert.ToDecimal(f("CodEmpleado")).ToString("0"),
                .Cod_Empleado1 = CStr(f("CodEmpleado1")),
                .Salario_Variable = Not IsDBNull(f("SalarioFijo")) AndAlso CStr(f("SalarioFijo")) = "N",
                .Sueldo_Periodo = Numero(f("SueldoPeriodo")),
                .Tarifa_Horaria = Numero(f("TarifaHoraria")),
                .Monto_Pension = Numero(f("MontoPension")),
                .Porcentaje_Pension = Numero(f("PorcentajePension")),
                .Sueldo_Actual_Basico = Not IsDBNull(f("SueldoActualBasico")) AndAlso CBool(f("SueldoActualBasico")),
                .Sal_Porcentaje = Numero(f("SalPorcentaje")),
                .Fecha_Contrato = If(IsDBNull(f("FechaContrato")), CType(Nothing, Date?), CDate(f("FechaContrato"))),
                .Sueldo_Actual = If(IsDBNull(f("SueldoActual")), CType(Nothing, Double?), Convert.ToDouble(f("SueldoActual")))}

            lista.Add(e)
            porCodigo(e.Cod_Empleado) = e
        Next

        ' Nómina de cada mes
        Dim sinComision As String = String.Join(", ", EmpleadosSinComisionNiSeptimo.Select(Function(x) x.ToString("0")))

        For Each f As DataRow In TablaEn(cn, tr,
            "SELECT D.CodEmpleado, N.Mes, " &
            "SUM(ISNULL(D.SalarioBasico,0) + ISNULL(D.Destajo,0) + ISNULL(D.Antiguedad,0) + ISNULL(D.Incentivos,0) + ISNULL(D.OtrosIngresos,0) + " &
            "    CASE WHEN D.CodEmpleado IN (" & sinComision & ") THEN 0 ELSE ISNULL(D.Comisiones,0) + ISNULL(D.SeptimoDia,0) END) AS Total, " &
            "SUM(ISNULL(D.HorasExtras,0)) AS HE, SUM(ISNULL(D.Destajo,0) + ISNULL(D.OtrosIngresos,0) + ISNULL(D.Incentivos,0)) AS Ingresos " &
            "FROM Nomina N INNER JOIN DetalleNomina D ON N.NumNomina = D.NumNomina " &
            "INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "WHERE E.CodTipoNomina = @Tipo AND E.Activo = 1 AND N.Ano = @Anno AND N.Mes BETWEEN @M1 AND @M2 " &
            "GROUP BY D.CodEmpleado, N.Mes", Copia(tipo),
            New SqlParameter("@Anno", SqlDbType.Int) With {.Value = fechaFin.Year},
            New SqlParameter("@M1", SqlDbType.Int) With {.Value = ctx.Mes_Ini},
            New SqlParameter("@M2", SqlDbType.Int) With {.Value = ctx.Mes_Fin}).Rows

            Dim e As CalculadoraAguinaldo.DatosEmpleado = Nothing
            Dim mes As Integer = Convert.ToInt32(f("Mes"))
            If mes < 1 OrElse mes > 12 OrElse Not porCodigo.TryGetValue(Convert.ToDecimal(f("CodEmpleado")).ToString("0"), e) Then Continue For
            e.Devengado(mes) = Numero(f("Total"))
            e.Horas_Extra(mes) = Numero(f("HE"))
            e.Ingresos(mes) = Numero(f("Ingresos"))
        Next

        ' Períodos de planilla por mes (solo cuenta en nómina catorcenal)
        For Each f As DataRow In TablaEn(cn, tr,
            "SELECT CONVERT(int, mes) AS Mes, COUNT(*) AS Cont FROM Fecha_Planilla WHERE [año] = @Anno AND CodTipoNomina = @Tipo " &
            "AND ISNUMERIC(mes) = 1 GROUP BY mes", Copia(tipo),
            New SqlParameter("@Anno", SqlDbType.Int) With {.Value = fechaFin.Year}).Rows
            Dim mes As Integer = Convert.ToInt32(f("Mes"))
            If mes >= 1 AndAlso mes <= 12 Then ctx.Cant_Planilla(mes) = Convert.ToInt32(f("Cont"))
        Next

        ' Subsidios (solo de quienes los suman al salario)
        For Each f As DataRow In TablaEn(cn, tr,
            "SELECT S.CodEmpleado, MONTH(NS.FechaPago) AS Mes, SUM(S.Subsidio) AS Monto " &
            "FROM NomSubsidio NS INNER JOIN DetalleNomSubsidio S ON NS.NumNomina = S.NumNominaSubsidio " &
            "INNER JOIN Empleado E ON E.CodEmpleado = S.CodEmpleado " &
            "WHERE E.CodTipoNomina = @Tipo AND E.Activo = 1 AND E.SumarSubsidio = 1 AND YEAR(NS.FechaPago) = @Anno " &
            "GROUP BY S.CodEmpleado, MONTH(NS.FechaPago)", Copia(tipo),
            New SqlParameter("@Anno", SqlDbType.Int) With {.Value = fechaFin.Year}).Rows

            Dim e As CalculadoraAguinaldo.DatosEmpleado = Nothing
            Dim mes As Integer = Convert.ToInt32(f("Mes"))
            If mes < 1 OrElse mes > 12 OrElse Not porCodigo.TryGetValue(Convert.ToDecimal(f("CodEmpleado")).ToString("0"), e) Then Continue For
            e.Subsidio(mes) = Numero(f("Monto"))
        Next

        ' Adelantos de 13vo del período
        For Each f As DataRow In TablaEn(cn, tr,
            "SELECT A.CodEmpleado, SUM(A.MontoAdelanto) AS Monto FROM Adelanto13vo A " &
            "INNER JOIN Empleado E ON E.CodEmpleado = A.CodEmpleado " &
            "WHERE E.CodTipoNomina = @Tipo AND E.Activo = 1 AND A.TipoAdelanto = '13vo Mes' " &
            "AND A.FechaAdelanto >= @Ini AND A.FechaAdelanto < @FinMas1 GROUP BY A.CodEmpleado", Copia(tipo),
            New SqlParameter("@Ini", SqlDbType.DateTime) With {.Value = fechaIni.Date},
            New SqlParameter("@FinMas1", SqlDbType.DateTime) With {.Value = fechaFin.Date.AddDays(1)}).Rows

            Dim e As CalculadoraAguinaldo.DatosEmpleado = Nothing
            If porCodigo.TryGetValue(Convert.ToDecimal(f("CodEmpleado")).ToString("0"), e) Then e.Adelanto = Numero(f("Monto"))
        Next

        CargarSuspensiones(cn, tr, lista, fechaIni, fechaFin)

        Return lista

    End Function

    ''' <summary>
    ''' Suspensiones: las propias y las de "Todos" restan días de aguinaldo (<see cref="CalculadoraAguinaldo.DatosEmpleado.Suspensiones"/>);
    ''' solo las del propio empleado con inicio y fin dentro del período se descuentan en dinero.
    ''' </summary>
    Private Sub CargarSuspensiones(cn As SqlConnection, tr As SqlTransaction, empleados As List(Of CalculadoraAguinaldo.DatosEmpleado),
                                   fechaIni As Date, fechaFin As Date)

        Dim porEmpleado As New Dictionary(Of String, List(Of KeyValuePair(Of Date, Double)))(StringComparer.OrdinalIgnoreCase)
        Dim dentroDelPeriodo As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        Dim limite As Date = fechaFin.Date.AddDays(1)

        For Each f As DataRow In TablaEn(cn, tr,
            "SELECT CodigoEmpleado, FechaInicio, FechaFin, DiasDisfrutar FROM SolicitudVacaciones " &
            "WHERE TipoSolicitud = 'Suspension' AND FechaInicio IS NOT NULL").Rows

            Dim codigo As String = CStr(f("CodigoEmpleado"))
            Dim dias As Double = Numero(f("DiasDisfrutar"))
            Dim inicio As Date = CDate(f("FechaInicio"))

            If Not porEmpleado.ContainsKey(codigo) Then porEmpleado(codigo) = New List(Of KeyValuePair(Of Date, Double))
            porEmpleado(codigo).Add(New KeyValuePair(Of Date, Double)(inicio, dias))

            If inicio.Date >= fechaIni.Date AndAlso Not IsDBNull(f("FechaFin")) AndAlso CDate(f("FechaFin")) < limite Then
                dentroDelPeriodo(codigo) = If(dentroDelPeriodo.ContainsKey(codigo), dentroDelPeriodo(codigo), 0) + dias
            End If
        Next

        Dim deTodos As List(Of KeyValuePair(Of Date, Double)) = Nothing
        porEmpleado.TryGetValue("Todos", deTodos)

        For Each e As CalculadoraAguinaldo.DatosEmpleado In empleados

            Dim propias As List(Of KeyValuePair(Of Date, Double)) = Nothing
            porEmpleado.TryGetValue(e.Cod_Empleado1, propias)

            Dim lista As New List(Of KeyValuePair(Of Date, Double))
            If propias IsNot Nothing Then lista.AddRange(propias)
            If deTodos IsNot Nothing Then lista.AddRange(deTodos)

            If lista.Count > 0 Then
                e.Suspensiones = Function(d, h) lista.Where(Function(s) s.Key.Date >= d.Date AndAlso s.Key.Date <= h.Date).Sum(Function(s) s.Value)
            End If

            If dentroDelPeriodo.ContainsKey(e.Cod_Empleado1) Then e.Dias_Suspension = dentroDelPeriodo(e.Cod_Empleado1)

        Next

    End Sub

    Private Shared Function Numero(valor As Object) As Double
        Return If(IsDBNull(valor) OrElse valor Is Nothing, 0, Convert.ToDouble(valor))
    End Function

    ''' <summary>Un parámetro nuevo con el mismo valor (un SqlParameter no se puede usar en dos comandos).</summary>
    Private Shared Function Copia(p As SqlParameter) As SqlParameter
        Return New SqlParameter(p.ParameterName, p.SqlDbType, p.Size) With {.Value = p.Value}
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

    ''' <summary>
    ''' Detalle para la cuadrícula: Codigo, Nombres, SalarioMensual, DiasAPagar, SalarioAPagar, Adelanto, MontoPagar
    ''' (= SalarioAPagar - Adelanto, lo que se paga).
    ''' </summary>
    Public Function ListarDetalle(numero As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & NombreCompleto & " AS Nombres, " &
            "D.SalarioMensual, D.DiasAPagar, D.SalarioAPagar, ISNULL(D.Adelanto13vo, 0) AS Adelanto, " &
            "ISNULL(D.SalarioAPagar, 0) - ISNULL(D.Adelanto13vo, 0) AS MontoPagar " &
            "FROM DetalleNom13Mes D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "WHERE D.NumNom13Mes = @Num ORDER BY E.CodEmpleado1",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})
    End Function

    ''' <summary>
    ''' Datos de la colilla y de la nómina impresa. Columnas: Codigo, Nombres, Cargo, CodDepartamento, Departamento, FechaIngreso,
    ''' SalarioMensual, DiasAPagar, TotalDevengado, Pension, Suspension, DiasSuspension, Adelanto, TotalDeducir, MontoPagar.
    ''' Con <paramref name="codDepartamento"/> vacío trae todos los departamentos.
    ''' </summary>
    Public Function ListarParaReporte(numero As Integer, codDepartamento As String) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & NombreCompleto & " AS Nombres, C.Cargo, " &
            "ISNULL(E.CodDepartamento, '') AS CodDepartamento, ISNULL(DP.Departamento, 'Sin departamento') AS Departamento, H.FechaContrato AS FechaIngreso, " &
            "ISNULL(D.SalarioMensual, 0) AS SalarioMensual, ISNULL(D.DiasAPagar, 0) AS DiasAPagar, " &
            "ISNULL(D.SalarioAPagar, 0) AS TotalDevengado, ISNULL(D.TotalDeducciones, 0) AS Pension, " &
            "ISNULL(D.MontoSuspension, 0) AS Suspension, ISNULL(D.DiasSuspension, 0) AS DiasSuspension, " &
            "ISNULL(D.Adelanto13vo, 0) AS Adelanto, ISNULL(D.Adelanto13vo, 0) AS TotalDeducir, " &
            "ISNULL(D.SalarioAPagar, 0) - ISNULL(D.Adelanto13vo, 0) AS MontoPagar " &
            "FROM DetalleNom13Mes D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "LEFT JOIN Cargo C ON C.CodCargo = E.CodCargo " &
            "LEFT JOIN Departamento DP ON DP.CodDepartamento = E.CodDepartamento " &
            "OUTER APPLY (SELECT TOP 1 FechaContrato FROM Historico WHERE CodEmpleado = E.CodEmpleado) H " &
            "WHERE D.NumNom13Mes = @Num AND (@Dpto = '' OR E.CodDepartamento = @Dpto) " &
            "ORDER BY Departamento, Nombres",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero},
            New SqlParameter("@Dpto", SqlDbType.NVarChar, 10) With {.Value = If(codDepartamento, "")})
    End Function

    ''' <summary>Departamentos con empleados en la nómina (CodDepartamento, Departamento), para imprimir una colilla por departamento.</summary>
    Public Function ListarDepartamentos(numero As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT DISTINCT ISNULL(E.CodDepartamento, '') AS CodDepartamento, ISNULL(DP.Departamento, 'Sin departamento') AS Departamento " &
            "FROM DetalleNom13Mes D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "LEFT JOIN Departamento DP ON DP.CodDepartamento = E.CodDepartamento " &
            "WHERE D.NumNom13Mes = @Num ORDER BY CodDepartamento",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})
    End Function

    ''' <summary>
    ''' Pago por empleado para la planilla del banco: Codigo, Nombres, Monto (= lo que se paga) y las cuentas
    ''' (CuentaBanco = BAC; CuentaBancoBANPRO, CuentaBancoBDF y CuentaBancoLAFISE).
    ''' </summary>
    Public Function ListarParaBanco(numero As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & NombreCompleto & " AS Nombres, " &
            "ISNULL(D.SalarioAPagar, 0) - ISNULL(D.Adelanto13vo, 0) AS Monto, " &
            "E.NumCedula, H.CuentaBanco, E.CuentaBancoBANPRO, E.CuentaBancoBDF, E.CuentaBancoLAFISE " &
            "FROM DetalleNom13Mes D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "OUTER APPLY (SELECT TOP 1 CuentaBanco FROM Historico WHERE CodEmpleado = E.CodEmpleado) H " &
            "WHERE D.NumNom13Mes = @Num ORDER BY E.CodEmpleado1",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})
    End Function

    ''' <summary>Montos a pagar por empleado (Codigo, Nombres, Monto) para el desglose de billetes y monedas.</summary>
    Public Function ListarMontos(numero As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & NombreCompleto & " AS Nombres, ISNULL(D.SalarioAPagar, 0) - ISNULL(D.Adelanto13vo, 0) AS Monto " &
            "FROM DetalleNom13Mes D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "WHERE D.NumNom13Mes = @Num ORDER BY E.CodEmpleado1",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})
    End Function

    Private Const NombreCompleto As String =
        "RTRIM(ISNULL(E.Nombre1 + ' ', '') + ISNULL(E.Nombre2 + ' ', '') + ISNULL(E.Apellido1 + ' ', '') + ISNULL(E.Apellido2, ''))"

#End Region

#Region "LISTADO"

    ''' <summary>
    ''' Todas las nóminas de 13vo mes, de la más reciente a la más antigua, para el listado histórico. Columnas: Numero, CodTipoNomina,
    ''' Tipo, FechaIni, FechaFin, FechaAplica, MontoPagado y Estado ("Abierta" o "Cerrada").
    ''' </summary>
    Public Function Listar() As DataTable
        Return ConsultarTabla(
            "SELECT N.NumNom13Mes AS Numero, N.CodTipoNomina, T.Nomina AS Tipo, N.FechaIni, N.FechaFin, N.FechaAplica, " &
            "ISNULL(N.MontoPagado, 0) AS MontoPagado, CASE WHEN ISNULL(N.Activa, 0) = 1 THEN 'Abierta' ELSE 'Cerrada' END AS Estado " &
            "FROM Nom13Mes N LEFT JOIN TipoNomina T ON T.CodTipoNomina = N.CodTipoNomina ORDER BY N.NumNom13Mes DESC")
    End Function

#End Region

#Region "CERRAR"

    ''' <summary>
    ''' Cierra la nómina (ya no se puede recalcular) y avanza <c>Consecutivos.nom13</c>, como hacía <c>CmdCerrar13_Click</c>.
    ''' Devuelve falso si la nómina no existe o ya estaba cerrada.
    ''' </summary>
    Public Function Cerrar(numero As Integer) As Boolean

        Dim cerrada As Boolean = False

        EnTransaccion(
            Sub(cn, tr)
                Dim n As Integer = EjecutarEn(cn, tr, "UPDATE Nom13Mes SET Activa = 0 WHERE NumNom13Mes = @Num AND Activa = 1",
                                              New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero})
                If n = 0 Then Return
                EjecutarEn(cn, tr, "UPDATE Consecutivos SET nom13 = nom13 + 1")
                cerrada = True
            End Sub)

        Return cerrada

    End Function

#End Region

End Class
