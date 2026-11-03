''' <summary>
''' Cálculo de un empleado en la nómina, port del cuerpo de CmdCalcular_Click (FrmCalcularNomina.frm, V.5.0) sin
''' acceso a datos ni variables globales: quien llama arma <see cref="EntradaNomina"/> (empleado, configuración,
''' movimientos y acumulados) y recibe <see cref="ResultadoNomina"/> con los valores de DetalleNomina.
''' Usa <see cref="CalculadoraInss"/> y <see cref="CalculadoraIR"/> para las retenciones.
''' </summary>
Public Class CalculadoraNomina

    Public Class EntradaNomina
        ' --- Tipo de nómina y configuración ---
        Public Property Periodo As String = "Quincenal"
        Public Property FormatoNomina As String = ""
        Public Property CalcularHoraTrabajada As Boolean
        Public Property EmpleadoConstruccion As Boolean
        Public Property HorasDia As Double = 8
        Public Property DiasMes As Double = 30
        Public Property DiasHorasExtra As Double = 2
        Public Property PorcientoSubsidio As Double
        Public Property CalcularRedondeado As Boolean
        Public Property Factor As Double = 1
        Public Property InssParametros As CalculadoraInss.ParametrosInss = New CalculadoraInss.ParametrosInss()
        Public Property IrParametros As CalculadoraIR.ParametrosIR = New CalculadoraIR.ParametrosIR()

        ' --- Empleado ---
        ''' <summary>Empleado.SueldoPeriodo ya multiplicado por la tasa de cambio si es dolarizado.</summary>
        Public Property SueldoPeriodo As Double
        Public Property AumentoBasico As Double
        Public Property PorcentajeComision As Double
        Public Property OtrosIngresosEmpleado As Double
        Public Property DescripOtrIngre As String = ""
        Public Property PorcientoIncentivo As Double
        Public Property MontoViaticoEmpleado As Double
        Public Property ExentoInss As Boolean
        Public Property ExentoIr As Boolean
        Public Property Reembolso As Double
        Public Property PorcentajePension As Double
        Public Property MontoPension As Double
        Public Property PorcentajeEmbargo As Double
        Public Property MontoEmbargo As Double

        ' --- Conceptos que resuelve quien llama (horas, ausencias, subsidio, movimientos) ---
        Public Property TotalSalarioxHora As Double
        Public Property TotalHoras As Double
        Public Property TarifaHoraria As Double
        ''' <summary>Descuento de días de salario básico (MontoBasico = DiasBasico * MontoDia).</summary>
        Public Property MontoBasico As Double
        Public Property DiasDescuento As Double
        Public Property DiasSubsidio As Double
        Public Property MontoSubsidio As Double
        Public Property Antiguedad As Double
        Public Property MontoVacaciones As Double
        Public Property MontoTipoVacaciones As Double
        Public Property MontoDestajos As Double
        Public Property Septimo As Double
        Public Property IncentivoProduccion As Double
        Public Property BonoProduccion As Double
        Public Property MontoIncentivos As Double
        Public Property MontoIncentivoExcento As Double
        Public Property MontoHorasTurno As Double
        Public Property MontoAusenteExcento As Double
        Public Property MontoOtrosIngresos As Double
        Public Property TotalPuntualidad As Double
        Public Property MontoViaticos As Double
        Public Property MontoDeduccion As Double
        Public Property MontoPrestamo As Double
        Public Property CantidadHorasExtra As Double
        ''' <summary>BuscaMontoHora ya calculado (<see cref="MontoHoraPeriodo"/>).</summary>
        Public Property MontoHora As Double
        Public Property EsSalarioFijo As Boolean = True

        ' --- Acumulados del mes anterior (cierre de mes quincenal/catorcenal) ---
        Public Property TotalDevengadoAnterior As Double
    End Class

    Public Class ResultadoNomina
        Public Property TotalDevengado As Double
        Public Property Salario As Double
        Public Property SalarioMensual As Double
        Public Property HorasExtras As Double
        Public Property HE As Double
        Public Property Septimo As Double
        Public Property DeduccionPorFalta As Double
        Public Property Inss As Double
        Public Property InssPatronal As Double
        Public Property InssMensual As Double
        Public Property InssPatronalMensual As Double
        Public Property Inatec As Double
        Public Property Ir As Double
        Public Property IrPatronal As Double
        Public Property AjusteIr As Double
        Public Property AjusteInss As Double
        Public Property ProvisionVacaciones As Double
        Public Property Mes13 As Double
        Public Property MontoPension As Double
        Public Property MontoEmbargo As Double
        Public Property DescripOtrIngre As String = ""
        Public Property BaseInss As Double
        Public Property BaseIr As Double
    End Class

    Public Shared Function Calcular(e As EntradaNomina) As ResultadoNomina

        Dim r As New ResultadoNomina()
        r.DescripOtrIngre = e.DescripOtrIngre
        r.Septimo = e.Septimo

        Dim otrosIngresos As Double = e.MontoOtrosIngresos + e.OtrosIngresosEmpleado

        ' ---- Total devengado ----
        Dim devengado As Double
        Dim salario As Double
        If e.CalcularHoraTrabajada Then
            devengado = e.PorcentajeComision + otrosIngresos + e.TotalSalarioxHora - e.MontoBasico
            salario = e.TotalSalarioxHora - e.MontoBasico
        Else
            devengado = e.SueldoPeriodo + e.PorcentajeComision + otrosIngresos + e.TotalSalarioxHora - e.MontoBasico
            salario = e.SueldoPeriodo + e.TotalSalarioxHora - e.MontoBasico
        End If
        ' En la nómina de bono de producción la V.5.0 solo reemplaza el devengado; el salario (DetalleNomina.SalarioBasico, 13vo) sigue siendo el del sueldo.
        If e.FormatoNomina = "Nomina Bono Produccion" Then devengado = e.BonoProduccion + otrosIngresos

        devengado += e.Antiguedad
        If e.PorcientoSubsidio > 0 Then devengado += e.MontoSubsidio

        If e.PorcientoIncentivo <> 0 Then r.DescripOtrIngre = e.PorcientoIncentivo.ToString("0.##") & "% Incentivo"

        r.TotalDevengado = devengado
        r.Salario = salario

        ' ---- Salario mensual (base para buscar el tramo de la tabla INSS) ----
        Dim cierreMes As Boolean = e.InssParametros.DiaFin >= 28
        Select Case e.Periodo
            Case "Semanal Viernes", "Semanal Sabado"
                r.SalarioMensual = (devengado + e.MontoVacaciones) * e.InssParametros.CantSabados
            Case "Catorcenal los Viernes", "Catorcenal los Sabados"
                If Not cierreMes Then
                    r.SalarioMensual = (((devengado * 15) / 14) + e.MontoVacaciones) * 2
                Else
                    r.SalarioMensual = devengado + e.TotalDevengadoAnterior + e.MontoVacaciones
                End If
            Case "Quincenal"
                If Not cierreMes Then
                    r.SalarioMensual = (devengado + e.MontoVacaciones) * 2
                Else
                    r.SalarioMensual = devengado + e.TotalDevengadoAnterior + e.MontoVacaciones
                End If
            Case "Mensual"
                r.SalarioMensual = devengado + e.MontoVacaciones
        End Select

        ' ---- Horas extra ----
        Dim horasExtras As Double = e.CantidadHorasExtra * e.MontoHora * e.DiasHorasExtra
        r.HorasExtras = horasExtras
        r.HE = e.CantidadHorasExtra
        If e.EmpleadoConstruccion Then r.Septimo = (e.TarifaHoraria * 8) + horasExtras

        ' ---- Descuento por faltas ----
        If e.DiasDescuento > 0 Then
            r.DeduccionPorFalta = DeduccionPorFaltas(e.Periodo, devengado - e.TotalPuntualidad, e.MontoVacaciones, e.DiasDescuento, e.DiasMes, e.SueldoPeriodo)
        End If

        ' ---- Bases de retención ----
        Dim resto As Double = e.AumentoBasico + e.MontoDestajos + r.Septimo + e.IncentivoProduccion + horasExtras + e.MontoIncentivos + e.MontoHorasTurno + e.MontoTipoVacaciones + e.Reembolso

        ' El INSS y el IR se calculan sobre lo devengado menos el descuento por faltas: así lo muestran los
        ' datos grabados por la V.5.0 (DetalleNomina.DiasDescuento), aunque el código fuente no lo resta.
        Dim baseInss As Double = devengado + resto + e.MontoVacaciones - e.MontoSubsidio - e.MontoAusenteExcento - r.DeduccionPorFalta
        r.BaseInss = baseInss

        Dim pi As CalculadoraInss.ParametrosInss = e.InssParametros
        pi.TotalDevengado = baseInss
        pi.SalarioMensual = r.SalarioMensual
        pi.Excento = e.ExentoInss
        pi.Periodo = e.Periodo
        pi.DiasSubsidio = e.DiasSubsidio
        pi.MontoSubsidio = e.MontoSubsidio
        Dim inss As CalculadoraInss.ResultadoInss = CalculadoraInss.Calcular(pi)
        r.Inss = inss.Inss
        r.InssPatronal = inss.Patronal
        r.InssMensual = inss.Mensual
        r.InssPatronalMensual = inss.PatronalMensual

        ' ---- INATEC (2 %) ----
        If e.DiasSubsidio >= 15 AndAlso e.MontoSubsidio = 0 Then
            r.Inatec = 0
        Else
            r.Inatec = baseInss * 0.02
        End If

        ' ---- IR ----
        If Not e.ExentoIr Then
            Dim baseIr As Double = devengado + resto + e.MontoSubsidio - e.MontoAusenteExcento - r.DeduccionPorFalta
            r.BaseIr = baseIr
            Dim pr As CalculadoraIR.ParametrosIR = e.IrParametros
            pr.Periodo = e.Periodo
            pr.TotalDevengado = baseIr
            pr.InssSalario = r.Inss
            pr.SalarioMensual = r.SalarioMensual
            pr.InssMensual = r.InssMensual
            pr.VacacionesPagadas = e.MontoVacaciones
            pr.MontoSubsidio = e.MontoSubsidio
            pr.DiasSubsidio = e.DiasSubsidio
            pr.Excento = False
            Dim ir As CalculadoraIR.ResultadoIR = CalculadoraIR.Calcular(pr)
            r.Ir = ir.Ir
            ' La V.5.0 deja MontoIRPatronal en 0 (la función de IR escribe en una variable local); todas las nóminas grabadas tienen IRPatronal = 0.
            r.IrPatronal = 0
            If r.Ir < 0 Then
                r.AjusteIr = Math.Abs(r.Ir)
                r.Ir = 0
                r.IrPatronal = 0
            End If
        End If

        ' ---- Provisión de vacaciones / 13vo ----
        Dim extrasProv As Double = e.AumentoBasico + e.MontoDestajos + r.Septimo + e.IncentivoProduccion + horasExtras + e.MontoIncentivos + e.MontoHorasTurno + e.MontoTipoVacaciones + e.Reembolso
        Dim prov As Double() = {0, 0}
        If salario <> 0 Then
            prov = ProvisionMes13Vacaciones(e.Periodo, salario + extrasProv, e.SueldoPeriodo, e.DiasSubsidio, e.MontoSubsidio, e.InssParametros.CantSabados, 1)
        ElseIf e.DiasSubsidio >= 15 AndAlso e.MontoSubsidio = 0 Then
            ' Subsidio de todo el período sin monto: se provisiona sobre el sueldo del período.
            prov = ProvisionMes13Vacaciones(e.Periodo, e.SueldoPeriodo + extrasProv, e.SueldoPeriodo, e.DiasSubsidio, e.MontoSubsidio, e.InssParametros.CantSabados, 1)
        End If
        r.ProvisionVacaciones = prov(0)
        r.Mes13 = prov(1)

        ' ---- Pensión alimenticia y embargo ----
        Dim neto As Double = devengado + e.TotalSalarioxHora + e.AumentoBasico + e.MontoSubsidio + e.MontoVacaciones + e.MontoDestajos + r.Septimo + e.IncentivoProduccion + horasExtras + e.MontoViaticos + e.MontoIncentivos + e.MontoHorasTurno + e.MontoTipoVacaciones - r.Inss - r.Ir
        r.MontoPension = e.MontoPension
        r.MontoEmbargo = e.MontoEmbargo
        If e.PorcentajePension > 0 AndAlso neto > 0 Then r.MontoPension += neto * (e.PorcentajePension / 100)
        If e.PorcentajeEmbargo > 0 AndAlso neto > 0 Then r.MontoEmbargo += neto * (e.PorcentajeEmbargo / 100)

        Return r

    End Function

#Region "Fórmulas auxiliares"

    ''' <summary>BuscaDeduccionPorFalta de la V.5.0.</summary>
    Public Shared Function DeduccionPorFaltas(periodo As String, totalDevengado As Double, montoVacaciones As Double, diasDescuento As Double, diasMes As Double, salarioBasico As Double) As Double
        Select Case periodo
            Case "Semanal Viernes", "Semanal Sabado"
                Return totalDevengado - ((totalDevengado - montoVacaciones) * (7 - diasDescuento) / 7)
            Case "Catorcenal los Viernes", "Catorcenal los Sabados"
                Return totalDevengado - ((totalDevengado - montoVacaciones) * (14 - diasDescuento) / 14)
            Case "Quincenal"
                Return (salarioBasico / 15) * diasDescuento
            Case "Mensual"
                Return totalDevengado - ((totalDevengado - montoVacaciones) * (diasMes - diasDescuento) / diasMes)
            Case "Trimestral"
                Return totalDevengado - ((totalDevengado - montoVacaciones) * ((diasMes * 3) - diasDescuento) / (diasMes * 3))
            Case "Semestral"
                Return totalDevengado - ((totalDevengado - montoVacaciones) * ((diasMes * 6) - diasDescuento) / (diasMes * 6))
        End Select
        Return 0
    End Function

    ''' <summary>Tarifa horaria que la V.5.0 deriva del sueldo del período (variable TarifaHoraria, redondeada a dos decimales); base del séptimo día.</summary>
    Public Shared Function TarifaHorariaSueldo(periodo As String, sueldoPeriodo As Double, diasMes As Double) As Double
        Dim divisor As Double
        Select Case periodo
            Case "Catorcenal los Viernes", "Catorcenal los Sabados" : divisor = 112
            Case "Quincenal" : divisor = (diasMes * 8) / 2
            Case "Mensual" : divisor = diasMes * 8
            Case "Trimestral" : divisor = diasMes * 8 * 3
            Case "Semestral" : divisor = diasMes * 8 * 6
            Case Else : Return 0
        End Select
        If divisor = 0 Then Return 0
        Return Math.Round(sueldoPeriodo / divisor, 2, MidpointRounding.AwayFromZero)
    End Function

    ''' <summary>BuscaMontoHora de la V.5.0 (valor de la hora ordinaria, usado para las horas extra).</summary>
    Public Shared Function MontoHoraPeriodo(periodo As String, tarifaHoraria As Double, sueldoPeriodo As Double, salario As Double, diasMes As Double, horas As Double) As Double
        If horas = 0 Then Return 0
        Select Case periodo
            Case "Semanal Viernes"
                Return Math.Round(((salario * 4.33) / diasMes) / horas, 6, MidpointRounding.AwayFromZero)
            Case "Semanal Sabado"
                Return Math.Round(tarifaHoraria, 6, MidpointRounding.AwayFromZero)
            Case "Catorcenal los Viernes"
                Return If(sueldoPeriodo <> 0, Math.Round(sueldoPeriodo / (diasMes * horas), 4, MidpointRounding.AwayFromZero), Math.Round(tarifaHoraria, 4, MidpointRounding.AwayFromZero))
            Case "Catorcenal los Sabados"
                If sueldoPeriodo <> 0 Then Return Math.Round(sueldoPeriodo / 14 / horas, 4, MidpointRounding.AwayFromZero)
                If salario = 0 Then Return Math.Round(tarifaHoraria, 4, MidpointRounding.AwayFromZero)
                Return Math.Round(salario / 14 / horas, 4, MidpointRounding.AwayFromZero)
            Case "Quincenal"
                Return Math.Round((sueldoPeriodo / 15) / horas, 6, MidpointRounding.AwayFromZero)
            Case "Mensual"
                Return Math.Round(sueldoPeriodo / (diasMes * horas), 2, MidpointRounding.AwayFromZero)
            Case "Trimestral"
                Return Math.Round(sueldoPeriodo / (diasMes * horas * 3), 2, MidpointRounding.AwayFromZero)
            Case "Semestral"
                Return Math.Round(sueldoPeriodo / (diasMes * horas * 6), 2, MidpointRounding.AwayFromZero)
        End Select
        Return 0
    End Function

    ''' <summary>BuscaMontoDia de la V.5.0 (valor del día, para descuentos de salario y vacaciones pagadas).</summary>
    Public Shared Function MontoDiaPeriodo(periodo As String, tarifaHoraria As Double, sueldoPeriodo As Double, salario As Double, diasMes As Double, horas As Double) As Double
        Select Case periodo
            Case "Semanal Viernes"
                If horas = 0 Then Return 0
                Return Math.Round(((salario * 4.33) / diasMes) / horas, 6, MidpointRounding.AwayFromZero)
            Case "Semanal Sabado"
                Return Math.Round(tarifaHoraria, 6, MidpointRounding.AwayFromZero)
            Case "Catorcenal los Viernes", "Catorcenal los Sabados"
                Return If(sueldoPeriodo <> 0, Math.Round(sueldoPeriodo / 14, 2, MidpointRounding.AwayFromZero), Math.Round(tarifaHoraria * horas, 4, MidpointRounding.AwayFromZero))
            Case "Quincenal"
                Return Math.Round((sueldoPeriodo * 2) / diasMes, 6, MidpointRounding.AwayFromZero)
            Case "Mensual", "Trimestral", "Semestral"
                Return Math.Round(sueldoPeriodo / diasMes, 2, MidpointRounding.AwayFromZero)
        End Select
        Return 0
    End Function

    ''' <summary>CalcularProvMes13Vaca de la V.5.0: devuelve (provisión de vacaciones, 13vo mes).</summary>
    ''' <param name="salario">Base del período; con 0 y subsidio completo sin monto se provisiona sobre SueldoPeriodo.</param>
    Public Shared Function ProvisionMes13Vacaciones(periodo As String, salario As Double, sueldoPeriodo As Double, diasSubsidio As Double, montoSubsidio As Double, cantSabados As Integer, tasaCambio As Double) As Double()

        Dim factor As Double = 0
        Dim diasSubsidioCompleto As Double = 0
        Dim factorSubsidio As Double = 0
        Dim divisor As Double = 1

        Select Case periodo
            Case "Semanal Viernes", "Semanal Sabado"
                diasSubsidioCompleto = 7
                If cantSabados = 5 Then
                    factor = 0.5 : factorSubsidio = 0.5 : divisor = 7
                Else
                    factor = 0.625 : factorSubsidio = 0.5 : divisor = 7
                End If
            Case "Catorcenal los Viernes", "Catorcenal los Sabados"
                Dim v As Double = ((salario * 26) / 4320) * 14
                Dim vs As Double = (((sueldoPeriodo * tasaCambio) * 26) / 4320) * 14
                If salario = 0 AndAlso diasSubsidio >= 14 AndAlso montoSubsidio = 0 Then v = vs
                Return {v, v}
            Case "Quincenal"
                factor = 1.25 : factorSubsidio = 1.25 : divisor = 15 : diasSubsidioCompleto = 15
            Case "Mensual"
                factor = 2.5 : factorSubsidio = 1.25 : divisor = 30 : diasSubsidioCompleto = 30
            Case "Trimestral"
                factor = 7.5 : factorSubsidio = 1.25 : divisor = 90 : diasSubsidioCompleto = 90
            Case "Semestral"
                Dim s As Double = salario * 15 / 180
                Return {s, s}
            Case Else
                Return {0, 0}
        End Select

        Dim valor As Double = salario * factor / divisor
        If salario = 0 AndAlso diasSubsidio >= diasSubsidioCompleto AndAlso montoSubsidio = 0 Then
            valor = (sueldoPeriodo * tasaCambio) * factorSubsidio / divisor
        End If
        Return {valor, valor}

    End Function

    ''' <summary>Redondear de la V.5.0 (Controles.CalcularRedondeado): entero más cercano si está activo, si no el mismo valor.</summary>
    Public Shared Function Redondear(valor As Double, activo As Boolean) As Double
        If Not activo Then Return valor
        ' La V.5.0 devolvía 0 cuando el valor ya era entero y sumaba 1 en el resto de los casos; aquí se redondea al entero más cercano.
        Return Math.Round(valor, 0, MidpointRounding.AwayFromZero)
    End Function

#End Region

End Class
