Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Calcula la nómina activa de un tipo de nómina (CmdCalcular_Click de FrmCalcularNomina, V.5.0): lee empleado y
''' movimientos, llama a <see cref="CalculadoraNomina"/> y graba DetalleNomina y los totales de Nomina, todo en una
''' sola transacción. <see cref="Simular"/> hace el mismo proceso y lo deshace, para contrastar con lo calculado.
''' </summary>
''' <remarks>
''' Aún no cubre (se avisa con NotSupportedException o en <see cref="ResumenCalculo.Advertencias"/>): nómina por horas
''' trabajadas/producción, períodos semanales, IR distinto de "Calcular IR x 12", puntos/horas turno, indemnización,
''' días adicionales, empleados suspendidos y el viático automático por configuración.
''' </remarks>
Public Class CsCalculoNominaDAL
    Inherits CsDALBase

    Public Class ResumenCalculo
        Public Property NumNomina As Integer
        Public Property Empleados As Integer
        Public Property Advertencias As New List(Of String)
        ''' <summary>Filas de DetalleNomina resultantes (solo se llena en <see cref="Simular"/>).</summary>
        Public Property Detalle As DataTable
    End Class

    Private Class Contexto
        Public NumNomina As Integer
        Public CodTipoNomina As String
        Public Inicio As Date
        Public Fin As Date
        Public Mes As Double
        Public Ano As Double
        Public NumeroPeriodo As Double
        Public Periodo As String
        Public Moneda As String
        Public MantValor As Boolean
        Public PorcientoInss As Boolean
        Public TasaInss As Double
        Public TasaInssPatronal As Double
        Public PorcientoIr As Boolean
        Public TasaIr As Double
        Public IrUltimaSemana As Boolean
        Public Horas As Double
        Public TipoCalculoIr As String
        Public PorcientoSubsidio As Double
        Public Restar3Dias As Boolean
        Public DiasHorasExtra As Double
        Public Redondeado As Boolean
        Public DiasMes As Double
        Public Tasa As Double
        Public Factor As Double
        Public Formato As String
        Public Calcular7mo As Boolean
        Public HoraSeptimo As Double
        Public MontoViaticosConfig As Double
        Public TramosIr As New List(Of CalculadoraIRBaja.TramoIR)
        Public TramosInss As DataTable
        Public Antiguedad As New List(Of CalculadoraAusencias.TramoAntiguedad)
        Public NominaAnterior As Integer
    End Class

    Private Const ColAnioAntiguedad As String = "A" & ChrW(241) & "oAntiguedad"
    Private Const ColAniosIndemnizacion As String = "A" & ChrW(241) & "osIndemnizacion"

    Public Function Calcular(codTipoNomina As String, Optional progreso As Action(Of Integer, Integer, String) = Nothing) As ResumenCalculo

        Dim resumen As ResumenCalculo = Nothing

        EnTransaccion(Sub(cn, tr) resumen = Procesar(cn, tr, codTipoNomina, progreso, False))

        Return resumen

    End Function

    ''' <summary>Calcula y deshace la transacción; <see cref="ResumenCalculo.Detalle"/> trae lo que se habría grabado.</summary>
    Public Function Simular(codTipoNomina As String) As ResumenCalculo

        Using cn As New SqlConnection(CadenaConexion)
            cn.Open()
            Using tr As SqlTransaction = cn.BeginTransaction()
                Try
                    Return Procesar(cn, tr, codTipoNomina, Nothing, True)
                Finally
                    tr.Rollback()
                End Try
            End Using
        End Using

    End Function

#Region "Proceso"

    ''' <summary>Los cuatro nombres del empleado sin espacios sobrantes; el progreso lo entrega junto al código, separado por tabulador.</summary>
    Private Shared Function NombreCompleto(emp As DataRow) As String
        Return String.Join(" ", {"Nombre1", "Nombre2", "Apellido1", "Apellido2"}.Select(Function(c) Convert.ToString(emp(c)).Trim()).Where(Function(s) s <> ""))
    End Function

    Private Function Procesar(cn As SqlConnection, tr As SqlTransaction, codTipoNomina As String, progreso As Action(Of Integer, Integer, String), devolverDetalle As Boolean) As ResumenCalculo

        Dim resumen As New ResumenCalculo()
        Dim ctx As Contexto = CargarContexto(cn, tr, codTipoNomina)
        resumen.NumNomina = ctx.NumNomina

        ValidarSoportado(ctx)

        EjecutarEn(cn, tr, "DELETE FROM DetalleNomina WHERE NumNomina = @n", P("@n", ctx.NumNomina))
        EjecutarEn(cn, tr,
            "UPDATE Nomina SET TotalSalarioBasico = 0, TotalDestajo = 0, TotalHorasExtras = 0, TotalComisiones = 0, TotalIncentivos = 0, " &
            "TotalDeducciones = 0, TotalOtrosIngresos = 0, TotalPrestamo = 0, TotalMontoINSS = 0, TotalMontoIR = 0, TotalVacaciones = 0, " &
            "TotalINSSPatronal = 0, TotalIRPatronal = 0, TotalINATEC = 0, Totalmes13 = 0, Procesada = 1 WHERE NumNomina = @n", P("@n", ctx.NumNomina))

        Dim empleados As DataTable = Tabla(cn, tr, "SELECT * FROM Empleado WHERE CodTipoNomina = @t AND Activo = 1 ORDER BY CodEmpleado1", P("@t", codTipoNomina))
        If empleados.Rows.Count = 0 Then Throw New InvalidOperationException("No existe ningún empleado activo asignado a este tipo de nómina.")

        Dim tot(14) As Double
        Dim i As Integer = 0

        For Each emp As DataRow In empleados.Rows
            i += 1
            If progreso IsNot Nothing Then progreso(i, empleados.Rows.Count, Convert.ToString(emp("CodEmpleado1")) & vbTab & NombreCompleto(emp))
            CalcularEmpleado(cn, tr, ctx, emp, tot, resumen)
        Next

        EjecutarEn(cn, tr,
            "UPDATE Nomina SET TotalSalarioBasico = @a0, TotalDestajo = @a1, TotalHorasExtras = @a2, TotalComisiones = @a3, TotalIncentivos = @a4, " &
            "TotalOtrosIngresos = @a5, TotalDeducciones = @a6, TotalPrestamo = @a7, TotalMontoINSS = @a8, TotalMontoIR = @a9, TotalVacaciones = @a10, " &
            "TotalINSSPatronal = @a11, TotalIRPatronal = @a12, Totalmes13 = @a13, TotalINATEC = @a14 WHERE NumNomina = @n",
            P("@a0", tot(0)), P("@a1", tot(1)), P("@a2", tot(2)), P("@a3", tot(3)), P("@a4", tot(4)), P("@a5", tot(5)), P("@a6", tot(6)),
            P("@a7", tot(7)), P("@a8", tot(8)), P("@a9", tot(9)), P("@a10", tot(10)), P("@a11", tot(11)), P("@a12", tot(12)), P("@a13", tot(13)),
            P("@a14", tot(14)), P("@n", ctx.NumNomina))

        resumen.Empleados = empleados.Rows.Count

        If devolverDetalle Then
            resumen.Detalle = Tabla(cn, tr,
                "SELECT e.CodEmpleado1, d.* FROM DetalleNomina d INNER JOIN Empleado e ON e.CodEmpleado = d.CodEmpleado WHERE d.NumNomina = @n ORDER BY e.CodEmpleado1",
                P("@n", ctx.NumNomina))
        End If

        Return resumen

    End Function

    Private Sub ValidarSoportado(ctx As Contexto)

        If ctx.Periodo.StartsWith("Semanal") Then Throw New NotSupportedException("El cálculo de nóminas semanales todavía no está migrado.")
        If ctx.TipoCalculoIr <> "Calcular IR x 12" AndAlso Not ctx.PorcientoIr Then Throw New NotSupportedException("El cálculo de IR '" & ctx.TipoCalculoIr & "' todavía no está migrado (solo 'Calcular IR x 12').")

    End Sub

    Private Function CargarContexto(cn As SqlConnection, tr As SqlTransaction, codTipoNomina As String) As Contexto

        Dim c As New Contexto()
        c.CodTipoNomina = codTipoNomina

        Dim nom As DataTable = Tabla(cn, tr, "SELECT TOP 1 * FROM Nomina WHERE CodTipoNomina = @t AND Activa = 1 AND Cerrada = 0 AND Anulada = 0 ORDER BY NumNomina DESC", P("@t", codTipoNomina))
        If nom.Rows.Count = 0 Then Throw New InvalidOperationException("El tipo de nómina no tiene una nómina activa.")
        Dim rn As DataRow = nom.Rows(0)
        c.NumNomina = Convert.ToInt32(rn("NumNomina"))
        c.Inicio = Convert.ToDateTime(rn("FechaNominaINI"))
        c.Fin = Convert.ToDateTime(rn("FechaNomina"))
        c.Mes = Num(rn("Mes"))
        c.Ano = Num(rn("Ano"))
        c.NumeroPeriodo = Num(rn("Periodo"))

        Dim tipo As DataRow = Tabla(cn, tr, "SELECT * FROM TipoNomina WHERE CodTipoNomina = @t", P("@t", codTipoNomina)).Rows(0)
        c.Periodo = Convert.ToString(tipo("Periodo"))
        c.Moneda = Convert.ToString(tipo("Moneda"))
        c.MantValor = Bool(tipo("MantValor"))
        c.PorcientoInss = Bool(tipo("PorcientoInss"))
        c.TasaInss = Num(tipo("TasaInss"))
        c.TasaInssPatronal = Num(tipo("TasaInssPatronal"))
        c.PorcientoIr = Bool(tipo("PorcientoIr"))
        c.TasaIr = Num(tipo("TasaIr"))
        c.IrUltimaSemana = Bool(tipo("IrUltimaSemana"))
        c.Horas = Num(tipo("Horas"))
        If Bool(tipo("CalcularHoraTrabajada")) Then Throw New NotSupportedException("La nómina por horas trabajadas todavía no está migrada.")

        Dim emp As DataRow = Tabla(cn, tr, "SELECT TOP 1 * FROM DatosEmpresa").Rows(0)
        c.TipoCalculoIr = Convert.ToString(emp("TipoCalculoIR"))
        c.PorcientoSubsidio = Num(emp("PorcientoSubsidio"))
        c.Restar3Dias = Bool(emp("Restar3DiasSubsidio"))
        c.Formato = Convert.ToString(emp("FormatoNomina"))
        c.Calcular7mo = Bool(emp("Calcular7mo"))

        Dim cfg As DataTable = Tabla(cn, tr, "SELECT TOP 1 * FROM ConfiguracionIncentivo")
        If cfg.Rows.Count > 0 Then
            c.HoraSeptimo = Num(cfg.Rows(0)("HorasSeptimo"))
            c.MontoViaticosConfig = Num(cfg.Rows(0)("MontoViaticos"))
        End If
        Dim he As Double
        c.DiasHorasExtra = If(Double.TryParse(Convert.ToString(emp("HorasExtra")), he), he, 2)

        Dim ctl As DataRow = Tabla(cn, tr, "SELECT TOP 1 * FROM Controles").Rows(0)
        c.DiasMes = Num(ctl("DiasMes"))
        c.Redondeado = Bool(ctl("CalcularRedondeado"))

        Dim tasa As Object = EscalarEn(cn, tr, "SELECT MontoDia FROM CambioMoneda WHERE FechaDia = @f", P("@f", c.Fin.Date))
        c.Tasa = If(tasa Is Nothing OrElse IsDBNull(tasa), 1, Convert.ToDouble(tasa))
        c.Factor = If(c.MantValor, c.Tasa, 1)

        For Each r As DataRow In Tabla(cn, tr, "SELECT * FROM IR ORDER BY Desde").Rows
            c.TramosIr.Add(New CalculadoraIRBaja.TramoIR With {.Desde = Num(r("Desde")), .Hasta = Num(r("Hasta")), .Impuesto_Base = Num(r("ImpuestoBase")), .Porciento = Num(r("PorcientoImpuesto")), .Sobre_Exceso = Num(r("SobreExceso"))})
        Next

        c.TramosInss = Tabla(cn, tr, "SELECT * FROM INSS ORDER BY Desde")

        For Each r As DataRow In Tabla(cn, tr, "SELECT * FROM Antiguedad").Rows
            Dim pct As Double
            Double.TryParse(Convert.ToString(r("porcent")), pct)
            c.Antiguedad.Add(New CalculadoraAusencias.TramoAntiguedad With {.Anios = Convert.ToInt32(r("a" & ChrW(241) & "os_acum")), .Porcentaje = pct})
        Next

        ' Nómina anterior del mismo mes (solo importa cuando la nómina cierra el mes: día final >= 28).
        If c.Fin.Day >= 28 Then
            Dim ant As Object = EscalarEn(cn, tr, "SELECT TOP 1 NumNomina FROM Nomina WHERE CodTipoNomina = @t AND NumNomina < @n AND Mes = @m AND Ano = @a AND Anulada = 0 ORDER BY NumNomina DESC",
                                          P("@t", codTipoNomina), P("@n", c.NumNomina), P("@m", c.Mes), P("@a", c.Ano))
            If ant IsNot Nothing AndAlso Not IsDBNull(ant) Then c.NominaAnterior = Convert.ToInt32(ant)
        End If

        Return c

    End Function

    Private Sub CalcularEmpleado(cn As SqlConnection, tr As SqlTransaction, c As Contexto, emp As DataRow, tot() As Double, resumen As ResumenCalculo)

        Dim codigo As Object = emp("CodEmpleado")
        Dim codigo1 As String = Convert.ToString(emp("CodEmpleado1"))
        Dim dolarizado As Boolean = Bool(emp("Dolarizado"))
        Dim tasaEmp As Double = If(dolarizado, c.Tasa, 1)
        Dim sueldoBruto As Double = Num(emp("SueldoPeriodo"))
        Dim sueldo As Double = sueldoBruto * tasaEmp
        Dim aumentoBasico As Double = Num(emp("AumentoBasico"))
        Dim salPorcentaje As Double = Num(emp("SalPorcentaje"))

        ' ---- Solicitudes de ausencia que cruzan el período ----
        Dim finDia As Date = c.Fin.Date.AddHours(23).AddMinutes(59).AddSeconds(59)
        Dim solicitudes As New List(Of CalculadoraAusencias.SolicitudDias)
        For Each r As DataRow In Tabla(cn, tr,
            "SELECT TipoSolicitud, SUM(DiasDisfrutar) AS Dias FROM SolicitudVacaciones WHERE CodigoEmpleado = @e AND FechaInicio <= @fin AND FechaFin >= @ini AND ISNULL(Anulado, 0) = 0 " &
            "AND TipoSolicitud IN ('Ausente', 'Ausencia', 'AusenteBasico', 'AusenteBasicoDINSS', 'AusenteBasicoExcento', 'Llegada Tarde') GROUP BY TipoSolicitud",
            P("@e", codigo1), P("@fin", finDia), P("@ini", c.Inicio.Date)).Rows
            solicitudes.Add(New CalculadoraAusencias.SolicitudDias With {.TipoSolicitud = Convert.ToString(r("TipoSolicitud")), .Dias = Num(r("Dias"))})
        Next
        Dim aus As CalculadoraAusencias.ResultadoAusencias = CalculadoraAusencias.ClasificarSolicitudes(solicitudes)

        ' ---- Subsidio ----
        Dim montoDia As Double = CalculadoraNomina.MontoDiaPeriodo(c.Periodo, 0, sueldo, 0, c.DiasMes, c.Horas)
        Dim subsidios As New List(Of CalculadoraAusencias.SolicitudSubsidio)
        For Each r As DataRow In Tabla(cn, tr,
            "SELECT FechaInicio, FechaFin, Clasificacion FROM SolicitudVacaciones WHERE TipoSolicitud = 'Subsidio' AND CodigoEmpleado = @e AND FechaInicio <= @fin AND FechaFin >= @ini AND ISNULL(Anulado, 0) = 0 ORDER BY FechaSolicitud",
            P("@e", codigo1), P("@fin", finDia), P("@ini", c.Inicio.Date)).Rows
            subsidios.Add(New CalculadoraAusencias.SolicitudSubsidio With {.Inicio = Convert.ToDateTime(r("FechaInicio")), .Fin = Convert.ToDateTime(r("FechaFin")), .Clasificacion = Convert.ToString(r("Clasificacion"))})
        Next
        Dim sub1 As CalculadoraAusencias.ResultadoSubsidio = CalculadoraAusencias.CalcularSubsidio(subsidios, c.Inicio, c.Fin, aus.DiasDescuento, montoDia, c.PorcientoSubsidio, c.Restar3Dias)

        Dim diasBasico As Double = aus.DiasBasico + sub1.DiasBasico
        Dim montoBasico As Double = diasBasico * montoDia
        Dim montoAusenteExcento As Double = aus.DiasBasicoExcento * montoDia

        ' ---- Antigüedad ----
        Dim anios As Double = 0
        Dim montoAntiguedad As Double = 0
        Dim contrato As Object = EscalarEn(cn, tr, "SELECT TOP 1 FechaContrato FROM Historico WHERE Codempleado = @e", P("@e", codigo))
        If contrato IsNot Nothing AndAlso Not IsDBNull(contrato) Then
            anios = CalculadoraAusencias.AniosAntiguedad(Convert.ToDateTime(contrato), c.Fin)
            montoAntiguedad = CalculadoraAusencias.Antiguedad(anios, c.Antiguedad, sueldo + aumentoBasico + sub1.MontoSubsidio)
        End If

        ' ---- Incentivos ----
        Dim montoIncentivos As Double = 0, montoIncentivoExcento As Double = 0, montoTipoVacaciones As Double = 0
        Dim exentos As String() = {"14", "15", "16", "17", "18"}
        For Each r As DataRow In Tabla(cn, tr,
            "SELECT di.Id, di.Valor, i.CodTipoIncentivo, ti.Incentivo FROM DetalleIncentivo di INNER JOIN Incentivo i ON i.NumIncentivo = di.NumIncentivo " &
            "INNER JOIN TipoIncentivo ti ON ti.CodTipoIncentivo = i.CodTipoIncentivo WHERE i.CodEmpleado = @e AND di.Pagado = 0 AND " &
            "((di.NumNomina = @n AND di.NumVez <> 'n') OR di.NumVez = 'n')", P("@e", codigo), P("@n", c.NumNomina)).Rows
            Dim valor As Double = Num(r("Valor"))
            If Array.IndexOf(exentos, Convert.ToString(r("CodTipoIncentivo"))) >= 0 Then
                montoIncentivoExcento += valor
            ElseIf Convert.ToString(r("Incentivo")) = "Vacaciones" Then
                montoTipoVacaciones += valor
            Else
                montoIncentivos += valor
            End If
            EjecutarEn(cn, tr, "UPDATE DetalleIncentivo SET NumNomina = @n WHERE Id = @id", P("@n", c.NumNomina), P("@id", r("Id")))
        Next

        ' ---- Vacaciones (nómina de vacaciones aplicada en el período, o vacaciones pagadas por solicitud) ----
        Dim montoVacaciones As Double = 0
        Dim diasVacaciones As Double = 0
        Dim nv As DataTable = Tabla(cn, tr,
            "SELECT d.DiasAPagar, d.DiasDescuento, ISNULL(d.SalarioMensual * (d.DiasAPagar - d.DiasDescuento) / 30 - d.AdelantoVacaciones, 0) AS MontoAPagar " &
            "FROM NomVaca n INNER JOIN DetalleNomVaca d ON d.NumNomVaca = n.NumNomVaca WHERE n.CodTipoNomina = @t AND n.Transfereir = 1 AND d.CodEmpleado = @e " &
            "AND n.FechaAplica BETWEEN @ini AND @fin", P("@t", c.CodTipoNomina), P("@e", codigo), P("@ini", c.Inicio.Date), P("@fin", finDia))
        If nv.Rows.Count = 0 Then
            Dim dias As New List(Of Double)
            For Each r As DataRow In Tabla(cn, tr,
                "SELECT FechaInicio, FechaFin, DiasDisfrutar FROM SolicitudVacaciones WHERE TipoSolicitud = 'Vacaciones Pagadas' AND CodigoEmpleado = @e AND ISNULL(Pagado, 0) = 0 " &
                "AND ISNULL(Anulado, 0) = 0 AND FechaInicio <= @fin AND FechaFin >= @ini", P("@e", codigo1), P("@fin", finDia), P("@ini", c.Inicio.Date)).Rows
                dias.Add(Num(r("DiasDisfrutar")))
            Next
            Dim vp As CalculadoraAusencias.ResultadoVacaciones = CalculadoraAusencias.VacacionesPagadas(dias, montoDia)
            montoVacaciones = vp.Monto
            diasVacaciones = vp.Dias
        Else
            For Each r As DataRow In nv.Rows
                montoTipoVacaciones += Num(r("MontoAPagar"))
                diasVacaciones += Num(r("DiasAPagar")) - Num(r("DiasDescuento"))
            Next
        End If

        ' ---- Deducciones y adelantos (la falta, tipo 01, se agrega después) ----
        Dim montoDeduccion As Double = 0, adelantos As Double = 0
        For Each r As DataRow In Tabla(cn, tr,
            "SELECT dd.Id, dd.Valor, d.CodTipoDeduccion FROM Deduccion d INNER JOIN DetalleDeduccion dd ON dd.NumDeduccion = d.NumDeduccion " &
            "WHERE d.CodEmpleado = @e AND dd.NumNomina = @n AND dd.Pagado = 0 AND d.CodTipoDeduccion <> '01'", P("@e", codigo), P("@n", c.NumNomina)).Rows
            If Convert.ToString(r("CodTipoDeduccion")) = "02" Then
                adelantos += Num(r("Valor"))
            Else
                montoDeduccion += Num(r("Valor"))
            End If
        Next

        ' ---- Préstamo: primera cuota sin cancelar ----
        Dim montoPrestamo As Double = 0
        Dim cuota As DataTable = Tabla(cn, tr,
            "SELECT TOP 1 m.ID, m.CuotaIgual, p.Moneda FROM Prestamo p INNER JOIN MovPrestamo m ON m.NumPrestamo = p.NumPrestamo " &
            "WHERE p.CodEmpleado = @e AND p.Cancelado = 0 AND m.Cancelado = 0 AND ISNULL(m.NumNomina, 0) IN (0, @n) ORDER BY m.NumCuota", P("@e", codigo), P("@n", c.NumNomina))
        If cuota.Rows.Count > 0 Then
            Dim cuotaPrestamo As Double = Num(cuota.Rows(0)("CuotaIgual"))
            Dim prestamoEnCordobas As Boolean = Convert.ToString(cuota.Rows(0)("Moneda")) = "CS"
            Dim nominaEnCordobas As Boolean = c.Moneda = "CS"
            If prestamoEnCordobas = nominaEnCordobas Then
                montoPrestamo = cuotaPrestamo
            ElseIf prestamoEnCordobas Then
                montoPrestamo = cuotaPrestamo / c.Tasa
            Else
                montoPrestamo = cuotaPrestamo * c.Tasa
            End If
            EjecutarEn(cn, tr, "UPDATE MovPrestamo SET NumNomina = @n WHERE ID = @id", P("@n", c.NumNomina), P("@id", cuota.Rows(0)("ID")))
        End If

        ' ---- Destajos y salario por porcentaje ----
        Dim destajos As Double = 0
        Dim dst As Object = EscalarEn(cn, tr,
            "SELECT SUM(ds.Cantidad * td.Monto) FROM DestalleDestajos ds INNER JOIN TipoDestajo td ON td.COdTipoDestajo = ds.CodTipoDestajo WHERE ds.NUmNomina = @n AND ds.CodEmpleado = @e",
            P("@n", c.NumNomina), P("@e", codigo))
        If dst IsNot Nothing AndAlso Not IsDBNull(dst) Then destajos = Convert.ToDouble(dst)
        If salPorcentaje > 0 Then destajos += sueldo * salPorcentaje / 100

        ' ---- Horas extra ----
        Dim horasExtra As Double = 0
        Dim hx As Object = EscalarEn(cn, tr, "SELECT SUM(CantHoras) FROM HorasExtras WHERE CodEmpleado = @e AND Pagada = 0 AND NumNomina = @n", P("@e", codigo), P("@n", c.NumNomina))
        If hx IsNot Nothing AndAlso Not IsDBNull(hx) Then horasExtra = Convert.ToDouble(hx)

        ' ---- Entrada del motor ----
        Dim e As New CalculadoraNomina.EntradaNomina()
        e.Periodo = c.Periodo
        e.FormatoNomina = c.Formato
        e.HorasDia = c.Horas
        e.DiasMes = c.DiasMes
        e.DiasHorasExtra = c.DiasHorasExtra
        e.PorcientoSubsidio = c.PorcientoSubsidio
        e.CalcularRedondeado = c.Redondeado
        e.Factor = c.Factor
        e.EmpleadoConstruccion = Bool(emp("EmpleadoConstruccion"))
        e.SueldoPeriodo = sueldo
        e.AumentoBasico = aumentoBasico
        e.PorcentajeComision = Num(emp("PorcentajeComision"))
        e.OtrosIngresosEmpleado = Num(emp("OtrosIngresos"))
        e.DescripOtrIngre = Convert.ToString(emp("DescripOtrIngre"))
        e.PorcientoIncentivo = Num(emp("PorcientoIncentivo"))
        e.MontoViaticoEmpleado = Num(emp("MontoViatico"))
        e.ExentoInss = Bool(emp("ExentoInss"))
        e.ExentoIr = Bool(emp("ExentoIr"))
        e.Reembolso = Num(emp("Reembolso"))
        e.PorcentajePension = Num(emp("PorcentajePension"))
        e.MontoPension = Num(emp("MontoPension"))
        e.PorcentajeEmbargo = Num(emp("PorcentajeEmbargo"))
        e.MontoEmbargo = Num(emp("MontoEmbargo"))
        e.TarifaHoraria = CalculadoraNomina.TarifaHorariaSueldo(c.Periodo, sueldo, c.DiasMes)
        e.MontoBasico = montoBasico
        e.DiasDescuento = aus.DiasDescuento
        e.DiasSubsidio = sub1.DiasSubsidio
        e.MontoSubsidio = sub1.MontoSubsidio
        e.Antiguedad = montoAntiguedad
        e.MontoVacaciones = montoVacaciones
        e.MontoTipoVacaciones = montoTipoVacaciones
        e.MontoDestajos = destajos
        e.MontoIncentivos = montoIncentivos
        e.MontoIncentivoExcento = montoIncentivoExcento
        e.MontoAusenteExcento = montoAusenteExcento
        ' Viático de la configuración: no aplica a liquidados, IR por porcentaje ni con subsidio en el período; el del empleado siempre se suma.
        Dim viaticoConfig As Double = If(Bool(emp("Liquidado")) OrElse c.PorcientoIr OrElse sub1.DiasSubsidio > 0, 0, c.MontoViaticosConfig)
        e.MontoViaticos = viaticoConfig + e.MontoViaticoEmpleado
        e.MontoDeduccion = montoDeduccion
        e.MontoPrestamo = montoPrestamo
        e.CantidadHorasExtra = horasExtra
        e.MontoHora = CalculadoraNomina.MontoHoraPeriodo(c.Periodo, e.TarifaHoraria, sueldo, 0, c.DiasMes, c.Horas)
        e.EsSalarioFijo = True

        ' Nómina de bono de producción (sin horas de producción): el bono es lo que exceden los destajos sobre las horas extra, menos el séptimo.
        If c.Formato = "Nomina Bono Produccion" Then
            Dim montoHrsExtras As Double = horasExtra * e.MontoHora * c.DiasHorasExtra
            Dim pagaSeptimo As Boolean = c.HoraSeptimo <= 0
            Dim septimo As Double = If(pagaSeptimo, e.TarifaHoraria * 8, 0)
            If destajos > montoHrsExtras Then
                Dim septimoAnterior As Double = If(pagaSeptimo, destajos / 6, 0)
                e.BonoProduccion = If(c.Calcular7mo, (destajos + septimoAnterior) - montoHrsExtras, destajos - montoHrsExtras) - septimo
            End If
            e.Septimo = septimo
            destajos = 0
            e.MontoDestajos = 0
        End If

        Dim anterior As DataRow = Nothing
        If c.NominaAnterior > 0 Then
            Dim t As DataTable = Tabla(cn, tr, "SELECT * FROM DetalleNomina WHERE NumNomina = @n AND CodEmpleado = @e", P("@n", c.NominaAnterior), P("@e", codigo))
            If t.Rows.Count > 0 Then anterior = t.Rows(0)
        End If
        Dim sueldoAnterior As Double = 0, inssAnterior As Double = 0, irAnterior As Double = 0, inssPatAnterior As Double = 0, irPatAnterior As Double = 0
        If anterior IsNot Nothing Then
            sueldoAnterior = Num(anterior("SalarioBasico")) + Num(anterior("Comisiones")) + Num(anterior("OtrosIngresos")) + Num(anterior("Antiguedad")) + If(c.PorcientoSubsidio > 0, Num(anterior("TotalSubsidio")), 0)
            inssAnterior = Num(anterior("MontoINSS")) : irAnterior = Num(anterior("MontoIR"))
            inssPatAnterior = Num(anterior("INSSPatronal")) : irPatAnterior = Num(anterior("IRPatronal"))
        End If
        e.TotalDevengadoAnterior = sueldoAnterior

        Dim pi As CalculadoraInss.ParametrosInss = e.InssParametros
        pi.PorcientoInss = c.PorcientoInss
        pi.TasaInss = c.TasaInss
        pi.TasaInssPatronal = c.TasaInssPatronal
        pi.DiaFin = c.Fin.Day
        pi.InssAnterior = inssAnterior
        pi.InssPatronalAnterior = inssPatAnterior
        If Not c.PorcientoInss Then pi.Tramos = TramosInssDe(c.TramosInss, Convert.ToString(emp("CodInss")))

        Dim pr As CalculadoraIR.ParametrosIR = e.IrParametros
        pr.TipoCalculoIr = c.TipoCalculoIr
        pr.IrUltimaSemana = c.IrUltimaSemana
        pr.PorcientoIr = c.PorcientoIr
        pr.TasaIr = c.TasaIr
        pr.Moneda = If(c.Moneda = "US", "US", "CS")
        pr.TasaCambio = If(c.Moneda = "US", c.Tasa, 1)
        pr.Tramos = c.TramosIr
        pr.DiaFin = c.Fin.Day
        pr.NumeroPeriodo = c.NumeroPeriodo
        pr.IrAnterior = irAnterior
        pr.IrPatronalAnterior = irPatAnterior
        pr.SueldoAnteriorMes = sueldoAnterior
        pr.InssAnteriorMes = inssAnterior
        pr.IrAnteriorMes = irAnterior

        Dim r1 As CalculadoraNomina.ResultadoNomina = CalculadoraNomina.Calcular(e)

        ' ---- Falta (deducción tipo 01 de la nómina) ----
        If aus.DiasDescuento > 0 Then
            montoDeduccion += r1.DeduccionPorFalta
            GrabarDeduccionFalta(cn, tr, c.NumNomina, codigo, Math.Round(r1.DeduccionPorFalta, 2))
        End If

        ' ---- DetalleNomina (valores de la V.5.0 para una fila nueva) ----
        Dim f As Double = c.Factor
        Dim rd As Boolean = c.Redondeado
        Dim v As New Dictionary(Of String, Object)
        v("NumNomina") = c.NumNomina
        v("CodEmpleado") = codigo
        v("SeptimoDia") = CalculadoraNomina.Redondear(r1.Septimo, rd)
        v("BonoProduccion") = CalculadoraNomina.Redondear(e.BonoProduccion, rd)
        v("TarifaHoraria") = CalculadoraNomina.Redondear(e.MontoHora, rd)
        v("IncetivoProduccion") = 0.0
        v("HTrabajada") = 0.0
        v("produjo") = If((sueldoBruto * f) = 0, "S", "N")
        v("DiasVacaciones") = diasVacaciones
        v("SalarioBasico") = CalculadoraNomina.Redondear(r1.Salario + aumentoBasico, rd) * f
        v("Destajo") = CalculadoraNomina.Redondear(destajos, rd) * f
        v("HE") = r1.HE
        v("HorasExtras") = r1.HorasExtras * f
        v("Comisiones") = e.PorcentajeComision + 0 + e.MontoViaticoEmpleado * f
        v("Incentivos") = (montoIncentivos + montoIncentivoExcento) * f
        v("OtrosIngresos") = (e.MontoOtrosIngresos + e.OtrosIngresosEmpleado) * f
        v("DescripOtrIngre") = r1.DescripOtrIngre
        v("Deducciones") = (montoDeduccion + adelantos) * f
        v("Prestamo") = montoPrestamo * f
        v("MontoINSS") = CalculadoraNomina.Redondear(r1.Inss, rd) * f
        v("MontoIR") = r1.Ir * f
        v("Vacaciones") = montoVacaciones * f
        v("TotalSubsidio") = sub1.MontoSubsidio
        v("Mes13") = r1.Mes13 * f
        v("INSSPatronal") = r1.InssPatronal * f
        v("IRPatronal") = r1.IrPatronal * f
        v("INATEC") = r1.Inatec * f
        v("Viaticos") = e.MontoViaticos * f
        v("DD") = aus.DiasDescuento * f
        v("Adelantos") = adelantos
        v("DiasDescuento") = r1.DeduccionPorFalta
        v("AjusteINSS") = r1.AjusteInss * f
        v("HTurno") = 0.0
        v("HorasTurno") = 0.0
        v(ColAnioAntiguedad) = Math.Floor(anios)
        v("Antiguedad") = montoAntiguedad * f
        v("VacacionesPagadas") = montoTipoVacaciones * f
        v("ValorDiasAdicionales") = 0.0
        v("DiasAdicionales") = 0.0
        v("Reembolso") = e.Reembolso
        v("Ajuste") = r1.AjusteIr * f
        v("Dias_Subsidio") = sub1.DiasSubsidio
        v("Clasificacion_Subsidio") = sub1.Clasificacion
        v("ProvIndemnizacion") = 0.0
        v(ColAniosIndemnizacion) = 0.0
        v("MontoPension") = r1.MontoPension * f
        v("EmbargoJudicial") = r1.MontoEmbargo * f

        Dim cols As New List(Of String), pars As New List(Of String), lista As New List(Of SqlParameter)
        For Each kv As KeyValuePair(Of String, Object) In v
            cols.Add("[" & kv.Key & "]")
            pars.Add("@p" & lista.Count)
            lista.Add(New SqlParameter("@p" & lista.Count, If(kv.Value, DBNull.Value)))
        Next
        EjecutarEn(cn, tr, "INSERT INTO DetalleNomina (" & String.Join(", ", cols) & ") VALUES (" & String.Join(", ", pars) & ")", lista.ToArray())

        ' ---- Totales de la nómina ----
        tot(0) += sueldoBruto * f
        tot(1) += destajos * f
        tot(2) += r1.HorasExtras * f
        tot(3) += e.PorcentajeComision * f
        tot(4) += montoIncentivos * f
        tot(5) += (e.MontoOtrosIngresos + e.OtrosIngresosEmpleado) * f
        tot(6) += (montoDeduccion + adelantos) * f + r1.MontoPension * f
        tot(7) += montoPrestamo * f
        tot(8) += r1.Inss * f
        tot(9) += r1.Ir * f
        tot(10) += montoVacaciones * f
        tot(11) += r1.InssPatronal * f
        tot(12) += r1.IrPatronal * f
        tot(13) += r1.Mes13 * f
        tot(14) += r1.Inatec

    End Sub

    Private Sub GrabarDeduccionFalta(cn As SqlConnection, tr As SqlTransaction, numNomina As Integer, codEmpleado As Object, monto As Double)

        Dim existe As Object = EscalarEn(cn, tr,
            "SELECT TOP 1 dd.Id FROM Deduccion d INNER JOIN DetalleDeduccion dd ON dd.NumDeduccion = d.NumDeduccion WHERE d.CodTipoDeduccion = '01' AND dd.NumNomina = @n AND d.CodEmpleado = @e",
            P("@n", numNomina), P("@e", codEmpleado))

        If existe IsNot Nothing AndAlso Not IsDBNull(existe) Then
            EjecutarEn(cn, tr, "UPDATE DetalleDeduccion SET Valor = @v WHERE Id = @id", P("@v", monto), P("@id", existe))
            Return
        End If

        Dim sig As Object = EscalarEn(cn, tr, "SELECT ISNULL(MAX(NumDeduccion), 0) + 1 FROM Deduccion")
        Dim num As Integer = Convert.ToInt32(sig)
        EjecutarEn(cn, tr, "INSERT INTO Deduccion (NumDeduccion, CodEmpleado, CodTipoDeduccion, NumVeces, Pagado, NUmNomina) VALUES (@d, @e, '01', '1', 0, @n)",
                   P("@d", num), P("@e", codEmpleado), P("@n", numNomina))
        EjecutarEn(cn, tr, "INSERT INTO DetalleDeduccion (NumDeduccion, Valor, NumVez, Pagado, NumNomina) VALUES (@d, @v, '1', 0, @n)",
                   P("@d", num), P("@v", monto), P("@n", numNomina))

    End Sub

#End Region

#Region "Utilitarios"

    Private Shared Function TramosInssDe(tabla As DataTable, codInss As String) As IList(Of CalculadoraInss.TramoInss)
        Dim lista As New List(Of CalculadoraInss.TramoInss)
        Dim filas As DataRow() = tabla.Select("CodInss = '" & codInss.Replace("'", "''") & "'")
        If filas.Length = 0 Then filas = tabla.Select()
        For Each r As DataRow In filas
            lista.Add(New CalculadoraInss.TramoInss With {
                .Desde = Num(r("Desde")), .Hasta = Num(r("Hasta")),
                .Laboral1 = Num(r("MontoLaboral1")), .Patronal1 = Num(r("MontoPatronal1")),
                .Laboral4 = Num(r("MontoLaboral4")), .Patronal4 = Num(r("MontoPatronal4")),
                .Laboral5 = Num(r("MontoLaboral5")), .Patronal5 = Num(r("MontoPatronal5"))})
        Next
        Return lista
    End Function

    Private Shared Function Tabla(cn As SqlConnection, tr As SqlTransaction, sql As String, ParamArray parametros() As SqlParameter) As DataTable
        Dim dt As New DataTable
        Using cmd As New SqlCommand(sql, cn, tr)
            cmd.Parameters.AddRange(parametros)
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using
        Return dt
    End Function

    Private Shared Function P(nombre As String, valor As Object) As SqlParameter
        Return New SqlParameter(nombre, If(valor, DBNull.Value))
    End Function

    Private Shared Function Num(o As Object) As Double
        If o Is Nothing OrElse IsDBNull(o) Then Return 0
        Return Convert.ToDouble(o)
    End Function

    Private Shared Function Bool(o As Object) As Boolean
        If o Is Nothing OrElse IsDBNull(o) Then Return False
        Return Convert.ToBoolean(o)
    End Function

#End Region

End Class
