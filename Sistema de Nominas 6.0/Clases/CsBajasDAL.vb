Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Acceso a datos de la liquidación de empleados (tabla Bajas y lo que lee FrmBajas de la V.5.0).
''' Todo parametrizado; Guardar y Procesar trabajan en una transacción.
''' </summary>
Public Class CsBajasDAL
    Inherits CsDALBase

    ''' <summary>Historial de salarios: qué columnas suma cada variante (las de la V.5.0).</summary>
    Public Enum VarianteSalarios
        ''' <summary>DetalleNomina normal (ChkSueldoActual sin marcar).</summary>
        Normal
        ''' <summary>Salario básico = Historico.SueldoActual (ChkSueldoActual marcado).</summary>
        SueldoActual
        ''' <summary>Formato de nómina "Nomina Bono Produccion".</summary>
        BonoProduccion
    End Enum

    Public Class DatosEmpresaBaja
        Public Property Nombre_Empresa As String = ""
        Public Property Direccion As String = ""
        Public Property Ruta_Logo As String = ""
        Public Property Ruta_Foto As String = ""
        Public Property Salario_Minimo As Double
        Public Property Formato_Nomina As String = ""
    End Class

    Public Class NominaActivaBaja
        Public Property Num_Nomina As Integer
        Public Property Inicio As Date
        Public Property Fin As Date
    End Class

    ''' <summary>Lo devengado por el empleado en las otras nóminas del mes de la baja (IR con "IR última semana").</summary>
    Public Class DevengadoMes
        Public Property Devengado As Double
        Public Property Inss As Double
        Public Property Viaticos As Double
    End Class

#Region "EMPLEADO"

    ''' <summary>Empleados que se pueden dar de baja (activos y no liquidados), para el buscador.</summary>
    Public Function ListarEmpleadosActivos() As DataTable
        Return ConsultarTabla(
            "SELECT CodEmpleado1, LTRIM(RTRIM(ISNULL(Nombre1,'') + ' ' + ISNULL(Nombre2,'') + ' ' + ISNULL(Apellido1,'') + ' ' + ISNULL(Apellido2,''))) AS Nombres " &
            "FROM Empleado WHERE Activo = 1 AND ISNULL(Liquidado, 0) = 0 ORDER BY CodEmpleado1")
    End Function

    ''' <summary>Empleado por código visible (sin filtrar activos: quien llama valida). Nothing si no existe.</summary>
    Public Function BuscarEmpleado(codigo1 As String) As EmpleadoBaja

        Dim dt As DataTable = ConsultarTabla(
            "SELECT TOP 1 E.CodEmpleado, E.CodEmpleado1, E.Nombre1, E.Nombre2, E.Apellido1, E.Apellido2, E.Direccion, E.Sexo, " &
            "E.NumCedula, E.NumeroInss, E.Activo, E.Liquidado, E.SueldoPeriodo, E.Dolarizado, E.SalarioFijo, E.SueldoActualBasico, " &
            "E.CodTipoNomina, C.Cargo, D.Departamento, H.SueldoActual, H.FechaContrato, T.Periodo, T.TasaInss, T.IrUltimaSemana " &
            "FROM Empleado E " &
            "LEFT JOIN Cargo C ON C.CodCargo = E.CodCargo " &
            "LEFT JOIN Departamento D ON D.CodDepartamento = E.CodDepartamento " &
            "LEFT JOIN Historico H ON H.Codempleado = E.CodEmpleado " &
            "LEFT JOIN TipoNomina T ON T.CodTipoNomina = E.CodTipoNomina " &
            "WHERE E.CodEmpleado1 = @Cod",
            New SqlParameter("@Cod", SqlDbType.NVarChar, 50) With {.Value = codigo1.Trim()})

        If dt.Rows.Count = 0 Then Return Nothing

        Return MapearEmpleado(dt.Rows(0))

    End Function

    ''' <summary>
    ''' Recalcula Historico.SueldoActual desde Empleado.SueldoPeriodo (V.5.0 ActualizaSueldoActualHistorico, que
    ''' FrmBajas ejecutaba al cargar el empleado). Sin tasa de hoy usa la última grabada; sin ninguna, 36.6243.
    ''' </summary>
    Public Sub ActualizarSueldoActual(codEmpleado As Decimal, diasMes As Double)

        Dim tasa As Double = TasaCambio(Date.Today)
        If tasa = 0 OrElse tasa = 1 Then
            Dim ultima As Object = Escalar("SELECT TOP 1 MontoDia FROM CambioMoneda ORDER BY FechaDia DESC")
            tasa = If(ultima Is Nothing OrElse IsDBNull(ultima), 0, Convert.ToDouble(ultima))
            If tasa = 0 OrElse tasa = 1 Then tasa = 36.6243
        End If

        Ejecutar(
            "UPDATE H SET H.SueldoActual = " &
            "(CASE WHEN TN.Periodo = 'Semanal' THEN ISNULL(E.SueldoPeriodo,0) * 4 " &
            "      WHEN TN.Periodo = 'Quincenal' THEN ISNULL(E.SueldoPeriodo,0) * 2 " &
            "      WHEN TN.Periodo = 'Mensual' THEN ISNULL(E.SueldoPeriodo,0) " &
            "      WHEN TN.Periodo = 'Catorcenal los Sabados' THEN (ISNULL(E.SueldoPeriodo,0) / 14) * @DiasMes " &
            "      ELSE ISNULL(E.SueldoPeriodo,0) END) * " &
            "(CASE WHEN ISNULL(E.Dolarizado,0) = 1 THEN @Tasa ELSE 1 END) " &
            "FROM Empleado E INNER JOIN Historico H ON E.CodEmpleado = H.Codempleado " &
            "INNER JOIN TipoNomina TN ON E.CodTipoNomina = TN.CodTipoNomina " &
            "WHERE E.CodEmpleado = @Cod",
            New SqlParameter("@DiasMes", SqlDbType.Float) With {.Value = diasMes},
            New SqlParameter("@Tasa", SqlDbType.Float) With {.Value = tasa},
            ParCod(codEmpleado))

    End Sub

    ''' <summary>Tasa de cambio del día; 1 si no está grabada (V.5.0 BuscaTasaCambio).</summary>
    Public Function TasaCambio(fecha As Date) As Double
        Dim r As Object = Escalar("SELECT MontoDia FROM CambioMoneda WHERE FechaDia = @F",
                                  New SqlParameter("@F", SqlDbType.SmallDateTime) With {.Value = fecha.Date})
        Return If(r Is Nothing OrElse IsDBNull(r), 1, Convert.ToDouble(r))
    End Function

    Public Function UltimaFechaNomina(codEmpleado As Decimal) As Date?
        Dim r As Object = Escalar(
            "SELECT MAX(N.FechaNomina) FROM Nomina N INNER JOIN DetalleNomina D ON D.NumNomina = N.NumNomina WHERE D.CodEmpleado = @Cod",
            ParCod(codEmpleado))
        Return If(r Is Nothing OrElse IsDBNull(r), CType(Nothing, Date?), CDate(r))
    End Function

    ''' <summary>Nómina activa del tipo de nómina del empleado; Nothing si no hay.</summary>
    Public Function NominaActiva(codEmpleado As Decimal) As NominaActivaBaja
        Dim dt As DataTable = ConsultarTabla(
            "SELECT TOP 1 N.NumNomina, N.FechaNominaINI, N.FechaNomina FROM Empleado E " &
            "INNER JOIN Nomina N ON N.CodTipoNomina = E.CodTipoNomina " &
            "WHERE N.Activa = 1 AND E.CodEmpleado = @Cod ORDER BY N.NumNomina DESC",
            ParCod(codEmpleado))
        If dt.Rows.Count = 0 Then Return Nothing
        Dim f As DataRow = dt.Rows(0)
        Return New NominaActivaBaja With {.Num_Nomina = Convert.ToInt32(f("NumNomina")), .Inicio = CDate(f("FechaNominaINI")), .Fin = CDate(f("FechaNomina"))}
    End Function

    ''' <summary>Inicio del período 1 del año en Fecha_Planilla para el tipo de nómina.</summary>
    Public Function InicioAnioPlanilla(codTipoNomina As String, anio As Integer) As Date?
        Dim r As Object = Escalar(
            "SELECT TOP 1 Inicio FROM Fecha_Planilla WHERE CodTipoNomina = @Tipo AND año = @Anio AND Periodo = 1",
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = codTipoNomina},
            New SqlParameter("@Anio", SqlDbType.SmallInt) With {.Value = anio})
        Return If(r Is Nothing OrElse IsDBNull(r), CType(Nothing, Date?), CDate(r))
    End Function

    Public Function ObtenerDatosEmpresa() As DatosEmpresaBaja
        Dim dt As DataTable = ConsultarTabla("SELECT TOP 1 NombreEmpresa, Direccion, RutaLogo, RutaFoto, SalarioMinimo, FormatoNomina FROM DatosEmpresa")
        Dim d As New DatosEmpresaBaja
        If dt.Rows.Count = 0 Then Return d
        Dim f As DataRow = dt.Rows(0)
        d.Nombre_Empresa = Texto(f("NombreEmpresa"))
        d.Direccion = Texto(f("Direccion"))
        d.Ruta_Logo = Texto(f("RutaLogo"))
        d.Ruta_Foto = Texto(f("RutaFoto"))
        d.Salario_Minimo = Numero(f("SalarioMinimo"))
        d.Formato_Nomina = Texto(f("FormatoNomina"))
        Return d
    End Function

#End Region

#Region "SALARIOS"

    ''' <summary>
    ''' Meses con nómina del empleado (básico + destajo [+ comisiones] distinto de 0), en orden, con su primera fecha
    ''' de inicio y su última fecha de nómina. Con <paramref name="tope"/> solo los meses que terminan hasta esa fecha.
    ''' </summary>
    Public Function MesesConNomina(codEmpleado As Decimal, tope As Date?) As List(Of SalarioMesBaja)

        Dim sql As String =
            "SELECT N.Mes, N.Ano, MIN(N.FechaNominaINI) AS FechaInicio, MAX(N.FechaNomina) AS FechaFin " &
            "FROM DetalleNomina D INNER JOIN Nomina N ON D.NumNomina = N.NumNomina " &
            "WHERE D.CodEmpleado = @Cod GROUP BY N.Mes, N.Ano " &
            If(tope.HasValue,
               "HAVING SUM(D.SalarioBasico + D.Destajo) <> 0 AND MAX(N.FechaNomina) <= @Tope ",
               "HAVING SUM(D.SalarioBasico + D.Destajo + D.Comisiones) <> 0 ") &
            "ORDER BY N.Ano, N.Mes"

        Dim pars As New List(Of SqlParameter) From {ParCod(codEmpleado)}
        If tope.HasValue Then pars.Add(New SqlParameter("@Tope", SqlDbType.SmallDateTime) With {.Value = tope.Value.Date})

        Return ConsultarTabla(sql, pars.ToArray()).AsEnumerable().Select(
            Function(f) New SalarioMesBaja With {
                .Mes = Convert.ToInt32(f("Mes")), .Anio = Convert.ToInt32(f("Ano")),
                .Fecha_Inicio = CDate(f("FechaInicio")), .Fecha_Fin = CDate(f("FechaFin"))}).ToList()

    End Function

    ''' <summary>
    ''' Ingresos por mes de las nóminas cuya primera fecha de nómina cae entre <paramref name="desde"/> y <paramref name="hasta"/>.
    ''' Las columnas sumadas son las de la V.5.0 para cada variante; los fragmentos SQL son constantes del código.
    ''' </summary>
    Public Function ListarSalarios(codEmpleado As Decimal, desde As Date, hasta As Date, variante As VarianteSalarios,
                                   conIncentivos As Boolean, conHorasExtra As Boolean) As List(Of SalarioMesBaja)

        Dim basico, incentivos, total, joins, extras As String
        joins = ""
        extras = "SUM(D.Reembolso) AS Reembolso, SUM(0) AS BonoProduccion, SUM(0) AS HorasExtras"

        Select Case variante
            Case VarianteSalarios.BonoProduccion
                basico = "SUM(D.SalarioBasico + D.Antiguedad)"
                incentivos = "SUM(D.Incentivos + D.IncetivoProduccion)"
                total = "SUM(D.SalarioBasico + D.Destajo + D.BonoProduccion + D.SeptimoDia + D.OtrosIngresos + D.IncetivoProduccion + D.Antiguedad" &
                        If(conHorasExtra, " + D.HorasExtras", "") & ")"
                extras = "SUM(0) AS Reembolso, SUM(D.BonoProduccion) AS BonoProduccion, " & If(conHorasExtra, "SUM(D.HorasExtras)", "SUM(0)") & " AS HorasExtras"
            Case VarianteSalarios.SueldoActual
                basico = "AVG(H.SueldoActual)"
                incentivos = If(conIncentivos, "SUM(D.Incentivos)", "SUM(0)")
                total = "SUM(D.Destajo + D.OtrosIngresos + D.Reembolso" & If(conIncentivos, " + D.Incentivos", "") & ") + AVG(H.SueldoActual)"
                joins = " INNER JOIN Historico H ON H.Codempleado = D.CodEmpleado"
            Case Else
                basico = "SUM(D.SalarioBasico + D.Antiguedad)"
                incentivos = If(conIncentivos, "SUM(D.Incentivos)", "SUM(0)")
                total = "SUM(D.SalarioBasico + D.Destajo + D.SeptimoDia + D.OtrosIngresos + D.Reembolso + D.Antiguedad" & If(conIncentivos, " + D.Incentivos", "") & ")"
        End Select

        Dim septimo As String = If(variante = VarianteSalarios.SueldoActual, "SUM(0)", "SUM(D.SeptimoDia)")

        Dim sql As String =
            "SELECT N.Mes, N.Ano, MIN(N.FechaNominaINI) AS FechaInicio, MAX(N.FechaNomina) AS FechaFin, " &
            basico & " AS SalarioBasico, SUM(D.Destajo) AS Destajo, " & septimo & " AS Septimo, SUM(D.OtrosIngresos) AS Otros, " &
            incentivos & " AS Incentivos, " & total & " AS TotalIngresos, " & extras & " " &
            "FROM DetalleNomina D INNER JOIN Nomina N ON D.NumNomina = N.NumNomina" & joins & " " &
            "WHERE D.CodEmpleado = @Cod GROUP BY N.Mes, N.Ano " &
            "HAVING SUM(D.SalarioBasico + D.Destajo) <> 0 AND MIN(N.FechaNomina) BETWEEN @Desde AND @Hasta " &
            "ORDER BY N.Ano, N.Mes"

        Return ConsultarTabla(sql, ParCod(codEmpleado),
                              New SqlParameter("@Desde", SqlDbType.SmallDateTime) With {.Value = desde.Date},
                              New SqlParameter("@Hasta", SqlDbType.SmallDateTime) With {.Value = hasta.Date}).AsEnumerable().Select(
            Function(f) New SalarioMesBaja With {
                .Mes = Convert.ToInt32(f("Mes")), .Anio = Convert.ToInt32(f("Ano")),
                .Fecha_Inicio = CDate(f("FechaInicio")), .Fecha_Fin = CDate(f("FechaFin")),
                .Salario_Basico = Numero(f("SalarioBasico")), .Destajo = Numero(f("Destajo")), .Septimo = Numero(f("Septimo")),
                .Otros = Numero(f("Otros")), .Incentivos = Numero(f("Incentivos")), .Reembolso = Numero(f("Reembolso")),
                .Bono_Produccion = Numero(f("BonoProduccion")), .Horas_Extras = Numero(f("HorasExtras")),
                .Total_Ingresos = Numero(f("TotalIngresos"))}).ToList()

    End Function

    ''' <summary>Porcentaje de la tabla Antiguedad para los años acumulados (fracción); Nothing si no hay fila.</summary>
    Public Function PorcentajeAntiguedad(annos As Integer) As Double?
        Dim r As Object = Escalar("SELECT TOP 1 porcent FROM Antiguedad WHERE años_acum = @A",
                                  New SqlParameter("@A", SqlDbType.SmallInt) With {.Value = annos})
        Dim valor As Double
        If r Is Nothing OrElse IsDBNull(r) OrElse Not Double.TryParse(r.ToString().Trim(), Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, valor) Then
            Return Nothing
        End If
        Return valor
    End Function

#End Region

#Region "MOVIMIENTOS PENDIENTES"

    Public Function AdelantoVacaciones(codEmpleado As Decimal, desde As Date, hasta As Date) As Double
        Return Suma("SELECT SUM(MontoAdelanto) FROM Adelanto13vo WHERE CodEmpleado = @Cod AND FechaAdelanto BETWEEN @Desde AND @Hasta AND TipoAdelanto = 'Vacaciones'",
                    ParCod(codEmpleado),
                    New SqlParameter("@Desde", SqlDbType.SmallDateTime) With {.Value = desde.Date},
                    New SqlParameter("@Hasta", SqlDbType.SmallDateTime) With {.Value = hasta.Date})
    End Function

    Public Function HorasExtraPendientes(codEmpleado As Decimal) As Double
        Return Suma("SELECT SUM(CantHoras) FROM HorasExtras WHERE Pagada = 0 AND CodEmpleado = @Cod", ParCod(codEmpleado))
    End Function

    ''' <summary>Otros ingresos de todas las nóminas del empleado (así lo sumaba la V.5.0).</summary>
    Public Function OtrosIngresosPlanilla(codEmpleado As Decimal) As Double
        Return Suma("SELECT SUM(OtrosIngresos) FROM DetalleNomina WHERE CodEmpleado = @Cod", ParCod(codEmpleado))
    End Function

    ''' <summary>Cuotas de préstamo sin cancelar; las de préstamos en dólares se convierten con la tasa.</summary>
    Public Function CuotasPrestamoPendientes(codEmpleado As Decimal, tasa As Double) As Double
        Return Suma("SELECT SUM(CASE WHEN P.Moneda = 'US' THEN M.CuotaIgual * @Tasa ELSE M.CuotaIgual END) " &
                    "FROM Prestamo P INNER JOIN MovPrestamo M ON P.NumPrestamo = M.NumPrestamo " &
                    "WHERE M.Cancelado = 0 AND P.CodEmpleado = @Cod",
                    ParCod(codEmpleado), New SqlParameter("@Tasa", SqlDbType.Float) With {.Value = tasa})
    End Function

    Public Function DeduccionesPendientes(codEmpleado As Decimal) As Double
        Return Suma("SELECT SUM(DD.Valor) FROM Deduccion D INNER JOIN DetalleDeduccion DD ON D.NumDeduccion = DD.NumDeduccion " &
                    "WHERE DD.Pagado = 0 AND D.CodEmpleado = @Cod", ParCod(codEmpleado))
    End Function

    ''' <summary>Suspensiones (SolicitudVacaciones tipo "Suspension") del empleado o de "Todos": fecha de inicio y días.</summary>
    Public Function Suspensiones(codigo1 As String) As List(Of KeyValuePair(Of Date, Double))
        Return ConsultarTabla(
            "SELECT FechaInicio, DiasDisfrutar FROM SolicitudVacaciones " &
            "WHERE TipoSolicitud = 'Suspension' AND FechaInicio IS NOT NULL AND (CodigoEmpleado = @Cod OR CodigoEmpleado = 'Todos')",
            New SqlParameter("@Cod", SqlDbType.NVarChar, 50) With {.Value = codigo1}).AsEnumerable().Select(
            Function(f) New KeyValuePair(Of Date, Double)(CDate(f("FechaInicio")), Numero(f("DiasDisfrutar")))).ToList()
    End Function

#End Region

#Region "IR"

    Public Function TablaIR() As List(Of CalculadoraIRBaja.TramoIR)
        Return ConsultarTabla("SELECT Desde, Hasta, ImpuestoBase, PorcientoImpuesto, SobreExceso FROM IR ORDER BY Desde").AsEnumerable().Select(
            Function(f) New CalculadoraIRBaja.TramoIR With {
                .Desde = Numero(f("Desde")), .Hasta = Numero(f("Hasta")), .Impuesto_Base = Numero(f("ImpuestoBase")),
                .Porciento = Numero(f("PorcientoImpuesto")), .Sobre_Exceso = Numero(f("SobreExceso"))}).ToList()
    End Function

    ''' <summary>
    ''' Devengado, INSS y viáticos (incentivo tipo 14) del empleado en las nóminas del mes de la baja que no son el
    ''' período actual. Nothing si la fecha no cae en el calendario o el mes no tiene período actual.
    ''' </summary>
    Public Function DevengadoDelMes(codEmpleado As Decimal, codTipoNomina As String, fechaBaja As Date) As DevengadoMes

        Dim periodo As DataTable = ConsultarTabla(
            "SELECT TOP 1 mes, año FROM Fecha_Planilla WHERE Inicio <= @F AND Final >= @F AND CodTipoNomina = @Tipo",
            New SqlParameter("@F", SqlDbType.SmallDateTime) With {.Value = fechaBaja.Date},
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = codTipoNomina})
        If periodo.Rows.Count = 0 Then Return Nothing

        Dim mes As Integer = CInt(Val(periodo.Rows(0)("mes").ToString()))
        Dim anio As Integer = Convert.ToInt32(periodo.Rows(0)("año"))

        Dim actual As Object = Escalar(
            "SELECT TOP 1 Periodo FROM Fecha_Planilla WHERE año = @Anio AND mes = @Mes AND CodTipoNomina = @Tipo AND Actual = 1",
            New SqlParameter("@Anio", SqlDbType.SmallInt) With {.Value = anio},
            New SqlParameter("@Mes", SqlDbType.NVarChar, 10) With {.Value = mes.ToString("00")},
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = codTipoNomina})
        If actual Is Nothing OrElse IsDBNull(actual) Then Return Nothing

        Dim dt As DataTable = ConsultarTabla(
            "SELECT SUM(D.SalarioBasico + D.Incentivos + D.HorasExtras + D.OtrosIngresos + D.Destajo + D.VacacionesPagadas + D.SeptimoDia + " &
            "D.IncetivoProduccion + D.BonoProduccion + D.HorasTurno + D.Antiguedad + D.Reembolso) AS TotalDevengado, " &
            "SUM(D.MontoINSS) AS MontoINSS, MAX(N.NumNomina) AS NumNomina " &
            "FROM DetalleNomina D INNER JOIN Nomina N ON D.NumNomina = N.NumNomina " &
            "WHERE N.Mes = @Mes AND N.Ano = @Anio AND N.CodTipoNomina = @Tipo AND D.CodEmpleado = @Cod AND N.Periodo <> @Periodo",
            New SqlParameter("@Mes", SqlDbType.Int) With {.Value = mes},
            New SqlParameter("@Anio", SqlDbType.Int) With {.Value = anio},
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = codTipoNomina},
            ParCod(codEmpleado),
            New SqlParameter("@Periodo", SqlDbType.Int) With {.Value = Convert.ToInt32(actual)})

        Dim res As New DevengadoMes
        If dt.Rows.Count = 0 Then Return res

        res.Devengado = Numero(dt.Rows(0)("TotalDevengado"))
        res.Inss = Numero(dt.Rows(0)("MontoINSS"))
        Dim numNomina As Object = dt.Rows(0)("NumNomina")
        If Not IsDBNull(numNomina) Then
            res.Viaticos = Suma(
                "SELECT SUM(DI.Valor) FROM DetalleIncentivo DI INNER JOIN Incentivo I ON DI.NumIncentivo = I.NumIncentivo " &
                "WHERE I.CodTipoIncentivo = '14' AND I.CodEmpleado = @Cod AND DI.NumNomina = @Num",
                ParCod(codEmpleado), New SqlParameter("@Num", SqlDbType.Int) With {.Value = Convert.ToInt32(numNomina)})
        End If

        Return res

    End Function

#End Region

#Region "GUARDAR / PROCESAR"

    Public Function BuscarLiquidacion(codEmpleado As Decimal) As Liquidacion
        Dim dt As DataTable = ConsultarTabla("SELECT TOP 1 * FROM Bajas WHERE CodEmpleado = @Cod ORDER BY Id DESC", ParCod(codEmpleado))
        If dt.Rows.Count = 0 Then Return Nothing
        Return MapearLiquidacion(dt.Rows(0))
    End Function

    ''' <summary>Graba el cálculo: actualiza la fila de Bajas del empleado o inserta una nueva (Id = máximo + 1). Devuelve el Id.</summary>
    Public Function Guardar(liq As Liquidacion, fechaHistorial As Date) As Integer

        Dim id As Integer

        EnTransaccion(
            Sub(cn, tr)
                Dim existente As Object = EscalarEn(cn, tr, "SELECT TOP 1 Id FROM Bajas WITH (UPDLOCK, HOLDLOCK) WHERE CodEmpleado = @Cod ORDER BY Id DESC", ParCod(liq.Cod_Empleado))

                If existente Is Nothing OrElse IsDBNull(existente) Then
                    Dim max As Object = EscalarEn(cn, tr, "SELECT MAX(Id) FROM Bajas WITH (UPDLOCK, HOLDLOCK)")
                    id = If(max Is Nothing OrElse IsDBNull(max), 0, Convert.ToInt32(max) + 1)
                    liq.Id = id
                    EjecutarEn(cn, tr,
                        "INSERT INTO Bajas (Id, CodEmpleado, FechaBaja, AnnosTrabajados, MesesTrabajados, DiasTrabajados, MontoNomPropor, MontoVaca, " &
                        "Monto13Mes, MontoAntiguedad, TipoBaja, MotivoBaja, Otro, MontoOtro, Prestamo, Deducciones, SalarioMensual, MontoINSS, MontoIR, " &
                        "FechaIniAgui, FechaFinAgui, DiasAguinaldo, DiasVacaciones, DiasMenosVaca, FechaIniVaca, FechaFinVaca, HorasExtra, MontoHorasExtra, " &
                        "FechaEgreso, FechaHistorial, SueldoActualBasicoLiquida, PrestamoOpt, DeduccionesOpt, AguinaldoOpt, AntiguedadOpt, ViaticosOpt, " &
                        "VacacionesOpt, HorasExtraOpt, OtrosIngresosOpt, OtrosIngresosPlanillaOpt, PagarDiasPlanillaOpt, DiasLaborados, " &
                        "Concepto_OtrasDeducciones, Monto_OtrasDeducciones, Dias_Antiguedad, Calculada, Procesada) VALUES " &
                        "(@Id, @Cod, @Fecha, @Annos, @Meses, @DiasAnt, @Propor, @Vaca, @Agui, @Antig, @Tipo, @Motivo, @Otro, @MontoOtro, @Prestamo, " &
                        "@Deducciones, @Salario, @Inss, @Ir, @IniAgui, @Fecha, @DiasAgui, @DiasVaca, @MenosVaca, @IniVaca, @Fecha, @HE, @MontoHE, " &
                        "@Fecha, @Historial, @OptSueldo, @OptPrestamo, @OptDeducciones, @OptAgui, @OptAntig, @OptViaticos, @OptVaca, @OptHE, " &
                        "@OptOtros, @OptOtrosPlanilla, @OptPagarDias, @DiasLab, @ConceptoOD, @MontoOD, @DiasAnt, 1, 0)",
                        Parametros(liq, fechaHistorial))
                Else
                    id = Convert.ToInt32(existente)
                    liq.Id = id
                    EjecutarEn(cn, tr,
                        "UPDATE Bajas SET FechaBaja = @Fecha, AnnosTrabajados = @Annos, MesesTrabajados = @Meses, DiasTrabajados = @DiasAnt, " &
                        "MontoNomPropor = @Propor, MontoVaca = @Vaca, Monto13Mes = @Agui, MontoAntiguedad = @Antig, TipoBaja = @Tipo, MotivoBaja = @Motivo, " &
                        "Otro = @Otro, MontoOtro = @MontoOtro, Prestamo = @Prestamo, Deducciones = @Deducciones, SalarioMensual = @Salario, " &
                        "MontoINSS = @Inss, MontoIR = @Ir, FechaIniAgui = @IniAgui, FechaFinAgui = @Fecha, DiasAguinaldo = @DiasAgui, " &
                        "DiasVacaciones = @DiasVaca, DiasMenosVaca = @MenosVaca, FechaIniVaca = @IniVaca, FechaFinVaca = @Fecha, HorasExtra = @HE, " &
                        "MontoHorasExtra = @MontoHE, FechaEgreso = @Fecha, FechaHistorial = @Historial, SueldoActualBasicoLiquida = @OptSueldo, " &
                        "PrestamoOpt = @OptPrestamo, DeduccionesOpt = @OptDeducciones, AguinaldoOpt = @OptAgui, AntiguedadOpt = @OptAntig, " &
                        "ViaticosOpt = @OptViaticos, VacacionesOpt = @OptVaca, HorasExtraOpt = @OptHE, OtrosIngresosOpt = @OptOtros, " &
                        "OtrosIngresosPlanillaOpt = @OptOtrosPlanilla, PagarDiasPlanillaOpt = @OptPagarDias, DiasLaborados = @DiasLab, " &
                        "Concepto_OtrasDeducciones = @ConceptoOD, Monto_OtrasDeducciones = @MontoOD, Dias_Antiguedad = @DiasAnt, " &
                        "Calculada = 1, Procesada = 0 WHERE Id = @Id",
                        Parametros(liq, fechaHistorial))
                End If
            End Sub)

        Return id

    End Function

    ''' <summary>
    ''' Procesa la baja (V.5.0 CmdEfectuar_Click) en una transacción: marca al empleado como liquidado (y lo inactiva si
    ''' no se pagan los días en planilla), cancela préstamos y deducciones si se liquidaron, lo quita de la nómina activa
    ''' o registra la solicitud "AusenteBasico" por los días no trabajados, lo quita de las nóminas activas de
    ''' subsidio, vacaciones y 13vo, y deja la fila de Bajas como procesada.
    ''' </summary>
    Public Sub Procesar(emp As EmpleadoBaja, liq As Liquidacion, usuario As String)

        EnTransaccion(
            Sub(cn, tr)

                EjecutarEn(cn, tr, "UPDATE Empleado SET Activo = @Activo, Liquidado = 1 WHERE CodEmpleado = @Cod",
                           New SqlParameter("@Activo", SqlDbType.Bit) With {.Value = liq.Pagar_Dias_Planilla_Opt}, ParCod(emp.Cod_Empleado))

                If liq.Prestamo_Opt Then
                    EjecutarEn(cn, tr, "UPDATE Prestamo SET Cancelado = 1 WHERE CodEmpleado = @Cod", ParCod(emp.Cod_Empleado))
                End If

                If liq.Deducciones_Opt Then
                    EjecutarEn(cn, tr,
                        "UPDATE DD SET Pagado = 1 FROM DetalleDeduccion DD INNER JOIN Deduccion D ON D.NumDeduccion = DD.NumDeduccion " &
                        "WHERE DD.Pagado = 0 AND D.CodEmpleado = @Cod", ParCod(emp.Cod_Empleado))
                End If

                If Not liq.Pagar_Dias_Planilla_Opt Then
                    EjecutarEn(cn, tr,
                        "DELETE D FROM DetalleNomina D INNER JOIN Nomina N ON N.NumNomina = D.NumNomina WHERE N.Activa = 1 AND D.CodEmpleado = @Cod",
                        ParCod(emp.Cod_Empleado))
                Else
                    AgregarSolicitudAusencia(cn, tr, emp, liq.Dias_Laborados, usuario)
                End If

                EjecutarEn(cn, tr,
                    "DELETE D FROM DetalleNomSubsidio D INNER JOIN NomSubsidio N ON N.NumNomina = D.NumNominaSubsidio WHERE N.Activa = 1 AND D.CodEmpleado = @Cod",
                    ParCod(emp.Cod_Empleado))
                EjecutarEn(cn, tr,
                    "DELETE D FROM DetalleNomVaca D INNER JOIN NomVaca N ON N.NumNomVaca = D.NumNomVaca WHERE N.Activa = 1 AND D.CodEmpleado = @Cod",
                    ParCod(emp.Cod_Empleado))
                EjecutarEn(cn, tr,
                    "DELETE D FROM DetalleNom13Mes D INNER JOIN Nom13Mes N ON N.NumNom13Mes = D.NumNom13Mes WHERE N.Activa = 1 AND D.CodEmpleado = @Cod",
                    ParCod(emp.Cod_Empleado))

                EjecutarEn(cn, tr, "UPDATE Bajas SET Procesada = 1 WHERE Id = @Id",
                           New SqlParameter("@Id", SqlDbType.Int) With {.Value = liq.Id})

                BitacoraEn(cn, tr, usuario, "Se Proceso Liquidacion: " & emp.Cod_Empleado1 & " " & emp.Nombre1)
            End Sub)

    End Sub

    ''' <summary>
    ''' Con "pagar días en planilla" el empleado sigue en la nómina activa: se registra una solicitud "AusenteBasico"
    ''' por los días de la nómina que no trabajará (V.5.0 Agregar_Solicitud desde CmdEfectuar).
    ''' </summary>
    Private Shared Sub AgregarSolicitudAusencia(cn As SqlConnection, tr As SqlTransaction, emp As EmpleadoBaja, diasTrabajados As Double, usuario As String)

        Dim dt As New DataTable
        Using cmd As New SqlCommand(
            "SELECT TOP 1 N.FechaNominaINI, N.FechaNomina FROM Empleado E INNER JOIN Nomina N ON N.CodTipoNomina = E.CodTipoNomina " &
            "WHERE N.Activa = 1 AND E.CodEmpleado = @Cod ORDER BY N.NumNomina DESC", cn, tr)
            cmd.Parameters.Add(ParCod(emp.Cod_Empleado))
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using
        If dt.Rows.Count = 0 Then Return

        Dim inicio As Date = CDate(dt.Rows(0)("FechaNominaINI")).Date
        Dim fin As Date = CDate(dt.Rows(0)("FechaNomina")).Date
        Dim diasNomina As Double = (fin - inicio).TotalDays + 1
        Dim trabajados As Double = Math.Max(0, diasTrabajados)
        Dim diasSolicitud As Double = If(trabajados > 0 AndAlso diasNomina > trabajados, diasNomina - trabajados, 0)

        ' La V.5.0 grababa también la solicitud con 0 días; aquí solo si hay días que descontar.
        If diasSolicitud <= 0 Then Return

        Dim ultimo As Object = EscalarEn(cn, tr,
            "SELECT MAX(CASE WHEN NumeroSolicitud NOT LIKE '%[^0-9]%' AND LEN(NumeroSolicitud) <= 9 " &
            "THEN CAST(NumeroSolicitud AS int) END) FROM SolicitudVacaciones WITH (TABLOCKX)")
        Dim numero As Integer = If(ultimo Is Nothing OrElse IsDBNull(ultimo), 0, Convert.ToInt32(ultimo)) + 1

        EjecutarEn(cn, tr,
            "INSERT INTO SolicitudVacaciones (FechaSolicitud, NumeroSolicitud, TipoSolicitud, CodigoEmpleado, DiasVacaciones, DiasDisfrutados, " &
            "FechaInicio, FechaFin, DiasDisfrutar, Clasificacion, Observaciones, Anulado, Pagado) " &
            "VALUES (@Ini, @Num, 'AusenteBasico', @Cod1, @Dias, @Dias, @Ini, @Fin, @Dias, 'Accidente Comun', 'Liquidacion', 0, 0)",
            New SqlParameter("@Ini", SqlDbType.SmallDateTime) With {.Value = inicio},
            New SqlParameter("@Num", SqlDbType.NVarChar, 50) With {.Value = numero.ToString("00000")},
            New SqlParameter("@Cod1", SqlDbType.NVarChar, 50) With {.Value = emp.Cod_Empleado1},
            New SqlParameter("@Dias", SqlDbType.Float) With {.Value = diasSolicitud},
            New SqlParameter("@Fin", SqlDbType.SmallDateTime) With {.Value = inicio.AddDays(trabajados)})

    End Sub

    Public Sub RegistrarBitacora(usuario As String, accion As String)
        EnTransaccion(Sub(cn, tr) BitacoraEn(cn, tr, usuario, accion))
    End Sub

    Private Shared Sub BitacoraEn(cn As SqlConnection, tr As SqlTransaction, usuario As String, accion As String)
        EjecutarEn(cn, tr, "INSERT INTO Bitacora (FechaHora, Usuario, Modulo, Accion) VALUES (@F, @U, 'Liquidacion', @A)",
                   New SqlParameter("@F", SqlDbType.SmallDateTime) With {.Value = Date.Now},
                   New SqlParameter("@U", SqlDbType.NVarChar, 50) With {.Value = If(String.IsNullOrWhiteSpace(usuario), "Desconocido", usuario)},
                   New SqlParameter("@A", SqlDbType.NVarChar, 255) With {.Value = Left(accion, 255)})
    End Sub

#End Region

#Region "PARAMETROS / MAPEAR"

    Private Shared Function Parametros(l As Liquidacion, fechaHistorial As Date) As SqlParameter()
        Return {
            New SqlParameter("@Id", SqlDbType.Int) With {.Value = l.Id},
            ParCod(l.Cod_Empleado),
            New SqlParameter("@Fecha", SqlDbType.SmallDateTime) With {.Value = l.Fecha_Baja.Date},
            ParF("@Annos", l.Annos_Trabajados), ParF("@Meses", l.Meses_Trabajados), ParF("@DiasAnt", l.Dias_Antiguedad),
            ParF("@Propor", l.Monto_Nom_Propor), ParF("@Vaca", l.Monto_Vacaciones), ParF("@Agui", l.Monto_Aguinaldo), ParF("@Antig", l.Monto_Antiguedad),
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 10) With {.Value = l.Tipo_Baja},
            New SqlParameter("@Motivo", SqlDbType.NVarChar, 255) With {.Value = l.Motivo_Baja},
            New SqlParameter("@Otro", SqlDbType.NVarChar, 25) With {.Value = l.Otro},
            ParF("@MontoOtro", l.Monto_Otro), ParF("@Prestamo", l.Prestamo), ParF("@Deducciones", l.Deducciones),
            ParM("@Salario", l.Salario_Mensual), ParM("@Inss", l.Monto_Inss), ParM("@Ir", l.Monto_Ir),
            New SqlParameter("@IniAgui", SqlDbType.SmallDateTime) With {.Value = l.Fecha_Ini_Aguinaldo},
            ParF("@DiasAgui", l.Dias_Aguinaldo), ParF("@DiasVaca", l.Dias_Vacaciones), ParF("@MenosVaca", l.Dias_Menos_Vacaciones),
            New SqlParameter("@IniVaca", SqlDbType.SmallDateTime) With {.Value = l.Fecha_Ini_Vacaciones},
            ParM("@HE", l.Horas_Extra), ParM("@MontoHE", l.Monto_Horas_Extra),
            New SqlParameter("@Historial", SqlDbType.SmallDateTime) With {.Value = fechaHistorial.Date},
            ParB("@OptSueldo", l.Sueldo_Actual_Opt), ParB("@OptPrestamo", l.Prestamo_Opt), ParB("@OptDeducciones", l.Deducciones_Opt),
            ParB("@OptAgui", l.Aguinaldo_Opt), ParB("@OptAntig", l.Antiguedad_Opt), ParB("@OptViaticos", l.Viaticos_Opt), ParB("@OptVaca", l.Vacaciones_Opt),
            ParB("@OptHE", l.Horas_Extra_Opt), ParB("@OptOtros", l.Otros_Ingresos_Opt), ParB("@OptOtrosPlanilla", l.Otros_Ingresos_Planilla_Opt),
            ParB("@OptPagarDias", l.Pagar_Dias_Planilla_Opt), ParF("@DiasLab", l.Dias_Laborados),
            New SqlParameter("@ConceptoOD", SqlDbType.NVarChar, 50) With {.Value = l.Concepto_Otras_Deducciones},
            ParF("@MontoOD", l.Monto_Otras_Deducciones)}
    End Function

    Private Shared Function MapearEmpleado(f As DataRow) As EmpleadoBaja
        Return New EmpleadoBaja With {
            .Cod_Empleado = Convert.ToDecimal(f("CodEmpleado")),
            .Cod_Empleado1 = Texto(f("CodEmpleado1")),
            .Nombre1 = Texto(f("Nombre1")), .Nombre2 = Texto(f("Nombre2")),
            .Apellido1 = Texto(f("Apellido1")), .Apellido2 = Texto(f("Apellido2")),
            .Direccion = Texto(f("Direccion")), .Sexo = Texto(f("Sexo")),
            .Cargo = Texto(f("Cargo")), .Departamento = Texto(f("Departamento")),
            .Num_Cedula = Texto(f("NumCedula")), .Numero_Inss = Texto(f("NumeroInss")),
            .Activo = Bool(f("Activo")), .Liquidado = Bool(f("Liquidado")),
            .Sueldo_Periodo = Numero(f("SueldoPeriodo")), .Dolarizado = Bool(f("Dolarizado")),
            .Salario_Fijo = If(IsDBNull(f("SalarioFijo")), "S", Texto(f("SalarioFijo"))),
            .Sueldo_Actual_Basico = If(IsDBNull(f("SueldoActualBasico")), CType(Nothing, Boolean?), CBool(f("SueldoActualBasico"))),
            .Sueldo_Actual = Numero(f("SueldoActual")),
            .Fecha_Contrato = If(IsDBNull(f("FechaContrato")), CType(Nothing, Date?), CDate(f("FechaContrato"))),
            .Cod_Tipo_Nomina = Texto(f("CodTipoNomina")), .Periodo = Texto(f("Periodo")),
            .Tasa_Inss = Numero(f("TasaInss")) / 100,
            .Ir_Ultima_Semana = Bool(f("IrUltimaSemana"))}
    End Function

    Private Shared Function MapearLiquidacion(f As DataRow) As Liquidacion
        Return New Liquidacion With {
            .Id = Convert.ToInt32(f("Id")), .Cod_Empleado = Convert.ToDecimal(f("CodEmpleado")),
            .Fecha_Baja = Fecha(f("FechaBaja")), .Annos_Trabajados = Numero(f("AnnosTrabajados")), .Meses_Trabajados = Numero(f("MesesTrabajados")),
            .Dias_Antiguedad = Numero(f("DiasTrabajados")), .Monto_Nom_Propor = Numero(f("MontoNomPropor")),
            .Monto_Vacaciones = Numero(f("MontoVaca")), .Monto_Aguinaldo = Numero(f("Monto13Mes")), .Monto_Antiguedad = Numero(f("MontoAntiguedad")),
            .Horas_Extra = Numero(f("HorasExtra")), .Monto_Horas_Extra = Numero(f("MontoHorasExtra")),
            .Otro = Texto(f("Otro")), .Monto_Otro = Numero(f("MontoOtro")),
            .Fecha_Ini_Vacaciones = Fecha(f("FechaIniVaca")), .Fecha_Ini_Aguinaldo = Fecha(f("FechaIniAgui")),
            .Dias_Aguinaldo = Numero(f("DiasAguinaldo")), .Dias_Vacaciones = Numero(f("DiasVacaciones")), .Dias_Menos_Vacaciones = Numero(f("DiasMenosVaca")),
            .Salario_Mensual = Numero(f("SalarioMensual")), .Fecha_Historial = Fecha(f("FechaHistorial")),
            .Tipo_Baja = Texto(f("TipoBaja")), .Motivo_Baja = Texto(f("MotivoBaja")),
            .Prestamo = Numero(f("Prestamo")), .Deducciones = Numero(f("Deducciones")),
            .Monto_Inss = Numero(f("MontoINSS")), .Monto_Ir = Numero(f("MontoIR")),
            .Concepto_Otras_Deducciones = Texto(f("Concepto_OtrasDeducciones")), .Monto_Otras_Deducciones = Numero(f("Monto_OtrasDeducciones")),
            .Dias_Laborados = Numero(f("DiasLaborados")),
            .Sueldo_Actual_Opt = Bool(f("SueldoActualBasicoLiquida")), .Prestamo_Opt = Bool(f("PrestamoOpt")), .Deducciones_Opt = Bool(f("DeduccionesOpt")),
            .Aguinaldo_Opt = Bool(f("AguinaldoOpt")), .Antiguedad_Opt = Bool(f("AntiguedadOpt")), .Viaticos_Opt = Bool(f("ViaticosOpt")),
            .Vacaciones_Opt = Bool(f("VacacionesOpt")), .Horas_Extra_Opt = Bool(f("HorasExtraOpt")),
            .Otros_Ingresos_Opt = Bool(f("OtrosIngresosOpt")), .Otros_Ingresos_Planilla_Opt = Bool(f("OtrosIngresosPlanillaOpt")),
            .Pagar_Dias_Planilla_Opt = Bool(f("PagarDiasPlanillaOpt"))}
    End Function

    Private Shared Function ParCod(cod As Decimal) As SqlParameter
        Return New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = cod, .Precision = 18, .Scale = 0}
    End Function

    Private Shared Function ParF(nombre As String, valor As Double) As SqlParameter
        Return New SqlParameter(nombre, SqlDbType.Float) With {.Value = valor}
    End Function

    Private Shared Function ParM(nombre As String, valor As Double) As SqlParameter
        Return New SqlParameter(nombre, SqlDbType.Money) With {.Value = CDec(Math.Round(valor, 4))}
    End Function

    Private Shared Function ParB(nombre As String, valor As Boolean) As SqlParameter
        Return New SqlParameter(nombre, SqlDbType.Bit) With {.Value = valor}
    End Function

    Private Function Suma(sql As String, ParamArray parametros() As SqlParameter) As Double
        Dim r As Object = Escalar(sql, parametros)
        Return If(r Is Nothing OrElse IsDBNull(r), 0, Convert.ToDouble(r))
    End Function

    Private Shared Function Texto(v As Object) As String
        Return If(IsDBNull(v) OrElse v Is Nothing, "", v.ToString().Trim())
    End Function

    Private Shared Function Numero(v As Object) As Double
        Return If(IsDBNull(v) OrElse v Is Nothing, 0, Convert.ToDouble(v))
    End Function

    Private Shared Function Bool(v As Object) As Boolean
        Return Not (IsDBNull(v) OrElse v Is Nothing) AndAlso Convert.ToBoolean(v)
    End Function

    Private Shared Function Fecha(v As Object) As Date
        Return If(IsDBNull(v) OrElse v Is Nothing, Date.MinValue, CDate(v))
    End Function

#End Region

End Class
