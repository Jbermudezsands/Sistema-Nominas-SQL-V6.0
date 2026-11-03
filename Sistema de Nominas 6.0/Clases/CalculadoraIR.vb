''' <summary>
''' IR de la nómina, port de CalcularMontoIr / BuscarIrAnual (FrmCalcularNomina.frm, V.5.0) sin acceso a datos ni
''' variables globales. Quien llama (CsCalculoNominaDAL) reúne los acumulados y arma <see cref="ParametrosIR"/>.
''' La tabla IR es anual; el primer tramo (exento) se omite como hacía la V.5.0 con MoveNext.
''' </summary>
Public Class CalculadoraIR

    Public Class ParametrosIR
        Public Property Periodo As String = ""
        ''' <summary>"Calcular IR x 12" u otro valor (DatosEmpresa.TipoCalculoIR).</summary>
        Public Property TipoCalculoIr As String = ""
        Public Property IrUltimaSemana As Boolean
        ''' <summary>TipoNomina.PorcientoIr: IR = % fijo sobre el bruto en lugar de la tabla.</summary>
        Public Property PorcientoIr As Boolean
        Public Property TasaIr As Double
        Public Property Excento As Boolean
        ''' <summary>"US" = nómina en dólares (se convierte con TasaCambio).</summary>
        Public Property Moneda As String = "C$"
        Public Property TasaCambio As Double = 1
        ''' <summary>Tabla IR ordenada por Desde, incluyendo el tramo exento.</summary>
        Public Property Tramos As IList(Of CalculadoraIRBaja.TramoIR) = New List(Of CalculadoraIRBaja.TramoIR)()

        Public Property TotalDevengado As Double
        ''' <summary>INSS del salario (sin vacaciones) que se resta del devengado.</summary>
        Public Property InssSalario As Double
        Public Property SalarioMensual As Double
        Public Property InssMensual As Double
        ''' <summary>Sueldo del período, para el caso de subsidio de 15 días o más sin monto (BuscaSalarioPeriodo).</summary>
        Public Property SueldoPeriodo As Double

        Public Property VacacionesPagadas As Double
        ''' <summary>INSS de las vacaciones pagadas (CalculadoraInss sobre MontoVacacionesPagadas).</summary>
        Public Property InssVacaciones As Double
        Public Property MontoSubsidio As Double
        Public Property DiasSubsidio As Double

        ''' <summary>Día del mes de la fecha final de la nómina.</summary>
        Public Property DiaFin As Integer
        Public Property CantSabados As Integer = 4
        ''' <summary>BuscaUltimaSemana: la nómina es la última semana del mes (semanales).</summary>
        Public Property EsUltimaSemanaDelMes As Boolean

        ' Acumulados de las demás nóminas del mismo mes (TotalSueldoAnterior / TotalInssAnterior / TotalIrAnterior)
        Public Property SueldoAnteriorMes As Double
        Public Property InssAnteriorMes As Double
        Public Property IrAnteriorMes As Double
        ' MontoIrAnterior / MontoIrPatronalAnterior (detalle de la nómina anterior)
        Public Property IrAnterior As Double
        Public Property IrPatronalAnterior As Double

        ' Acumulados del año fiscal (nóminas cerradas del año + DetalleNominaAcumulada)
        Public Property IrAcumulado As Double
        Public Property TotalDevengadoAcumulado As Double
        Public Property NQuincenas As Double = 1
        Public Property PeriodoIngreso As Double = 1
        ''' <summary>Períodos que faltan en el año fiscal (24 - (período - 1) quincenal, 12 - (...) mensual).</summary>
        Public Property NumeroPeriodo As Double

        ' Catorcenal los Sabados con IrUltimaSemana: lo devengado en las otras nóminas del mes
        Public Property EsUltimoPeriodoDelMes As Boolean = True
        Public Property DevengadoOtrasNominasMes As Double
        Public Property InssOtrasNominasMes As Double
        Public Property ViaticosOtrasNominasMes As Double
    End Class

    Public Class ResultadoIR
        Public Property Ir As Double
        Public Property IrPatronal As Double
        Public Property BrutoMensual As Double
    End Class

    Public Shared Function Calcular(p As ParametrosIR) As ResultadoIR

        Dim res As New ResultadoIR()
        If p.Excento Then Return res

        If p.PorcientoIr Then
            Dim bruto As Double = p.TotalDevengado - p.InssSalario
            res.Ir = bruto * (p.TasaIr / 100)
            res.IrPatronal = res.Ir
            res.BrutoMensual = bruto
            Return res
        End If

        Dim gravables As List(Of CalculadoraIRBaja.TramoIR) = If(p.Tramos Is Nothing, New List(Of CalculadoraIRBaja.TramoIR)(), p.Tramos.Skip(1).ToList())
        Dim minimo As Double = If(gravables.Count > 0, (gravables(0).Desde - 1) / 12, 0)

        Dim bruto2 As Double = 0, brutoMensual As Double = 0, brutoAnual As Double = 0
        CalcularBases(p, bruto2, brutoMensual, brutoAnual)

        Dim esDolar As Boolean = p.Moneda = "US"
        Dim montoDolares As Double = brutoMensual
        If esDolar Then brutoMensual *= p.TasaCambio

        Dim ir As Double = 0, irPatronal As Double = 0

        Select Case p.Periodo
            Case "Semanal Viernes", "Semanal Sabado"
                Dim t As CalculadoraIRBaja.TramoIR = TramoMensual(gravables, brutoMensual, minimo)
                If t IsNot Nothing Then
                    ir = R(t.Impuesto(brutoMensual * 12) / 12)
                    irPatronal = ir
                End If

            Case "Catorcenal los Viernes"
                Dim t As CalculadoraIRBaja.TramoIR = TramoMensual(gravables, brutoMensual, minimo)
                If brutoMensual >= minimo Then
                    If t IsNot Nothing Then
                        Dim impuesto As Double = t.Impuesto(brutoMensual * 12)
                        If p.DiaFin < 28 Then
                            ir = R(impuesto / 2 / 12)
                            irPatronal = ir
                        Else
                            Dim irMensual As Double = R(impuesto / 12)
                            ir = irMensual - p.IrAnterior
                            irPatronal = irMensual - p.IrPatronalAnterior
                        End If
                    End If
                Else
                    ir = -p.IrAnterior
                    irPatronal = -p.IrPatronalAnterior
                End If

            Case "Catorcenal los Sabados"
                If brutoMensual >= minimo Then
                    Dim t As CalculadoraIRBaja.TramoIR = TramoMensual(gravables, brutoMensual, minimo)
                    If t IsNot Nothing Then
                        Dim impuesto As Double = t.Impuesto(brutoMensual * 12)
                        ir = If(p.IrUltimaSemana, R(impuesto / 12), R(impuesto / 26))
                        irPatronal = ir
                    End If
                Else
                    ir = -p.IrAnterior
                    irPatronal = -p.IrPatronalAnterior
                End If

            Case "Quincenal"
                CalcularQuincenal(p, gravables, brutoAnual, ir, irPatronal)

            Case "Mensual"
                Dim t As CalculadoraIRBaja.TramoIR = gravables.FirstOrDefault(Function(x) x.Contiene(brutoAnual))
                If t IsNot Nothing Then
                    ir = (t.Impuesto(brutoAnual) - p.IrAcumulado) / 12
                    irPatronal = ir - p.IrPatronalAnterior
                End If

            Case "Trimestral"
                Dim t As CalculadoraIRBaja.TramoIR = TramoMensual(gravables, brutoMensual, minimo)
                If t IsNot Nothing Then
                    ir = R(t.Impuesto(brutoMensual * 12) / 4)
                    irPatronal = ir
                End If

            Case "Semestral"
                Dim t As CalculadoraIRBaja.TramoIR = TramoMensual(gravables, brutoMensual, minimo)
                If t IsNot Nothing Then
                    ir = R(t.Impuesto(brutoMensual * 12) / 2)
                    irPatronal = ir
                End If
        End Select

        If esDolar Then
            brutoMensual = montoDolares
            If p.TasaCambio <> 0 Then
                ir /= p.TasaCambio
                irPatronal /= p.TasaCambio
            End If
        End If

        res.Ir = ir
        res.IrPatronal = irPatronal
        res.BrutoMensual = brutoMensual
        Return res

    End Function

    ''' <summary>Impuesto anual sobre un monto anual (BuscarIrAnual de la V.5.0, rama quincenal): 0 si cae en el tramo exento.</summary>
    Public Shared Function ImpuestoAnual(tramos As IList(Of CalculadoraIRBaja.TramoIR), montoAnual As Double) As Double
        If tramos Is Nothing OrElse tramos.Count < 2 Then Return 0
        Dim t As CalculadoraIRBaja.TramoIR = tramos.Skip(1).FirstOrDefault(Function(x) x.Contiene(montoAnual))
        Return If(t Is Nothing, 0, t.Impuesto(montoAnual))
    End Function

#Region "Bases de cálculo por período"

    ''' <summary>Calcula MontoBruto, MontoBrutoMensual y MontoBrutoAnual según el período (primer Select Case de la V.5.0).</summary>
    Private Shared Sub CalcularBases(p As ParametrosIR, ByRef bruto As Double, ByRef brutoMensual As Double, ByRef brutoAnual As Double)

        Dim neto As Double = p.TotalDevengado - p.InssSalario
        bruto = neto

        Select Case p.Periodo
            Case "Semanal Viernes", "Semanal Sabado"
                If p.EsUltimaSemanaDelMes Then
                    brutoMensual = bruto + p.SueldoAnteriorMes - p.InssAnteriorMes
                ElseIf Not p.IrUltimaSemana Then
                    brutoMensual = bruto * p.CantSabados
                    If bruto = 0 AndAlso p.DiasSubsidio >= 15 AndAlso p.MontoSubsidio = 0 Then
                        bruto = p.SueldoPeriodo
                        brutoMensual = bruto * p.CantSabados
                    End If
                Else
                    brutoMensual = 0
                End If

            Case "Catorcenal los Viernes"
                If p.DiaFin < 28 Then
                    brutoMensual = ((bruto * 15) / 14) * 2
                Else
                    brutoMensual = p.SalarioMensual - p.InssMensual
                End If
                If bruto = 0 AndAlso p.DiasSubsidio >= 15 AndAlso p.MontoSubsidio = 0 Then
                    bruto = p.SueldoPeriodo
                    brutoMensual = ((bruto * 15) / 14) * 2
                End If

            Case "Catorcenal los Sabados"
                If Not p.IrUltimaSemana Then
                    brutoMensual = (bruto * 26) / 12
                    If bruto = 0 AndAlso p.DiasSubsidio >= 15 AndAlso p.MontoSubsidio = 0 Then
                        bruto = p.SueldoPeriodo
                        brutoMensual = (bruto * 26) / 12
                    End If
                ElseIf p.EsUltimoPeriodoDelMes Then
                    brutoMensual = (bruto * 26) / 12
                    If bruto = 0 AndAlso p.DiasSubsidio >= 15 AndAlso p.MontoSubsidio = 0 Then
                        bruto = p.SueldoPeriodo
                        brutoMensual = (bruto * 26) / 12
                    End If
                    brutoMensual = bruto + (p.DevengadoOtrasNominasMes - p.InssOtrasNominasMes - p.ViaticosOtrasNominasMes)
                Else
                    brutoMensual = 0
                End If

            Case "Quincenal"
                If p.TipoCalculoIr = "Calcular IR x 12" Then
                    If p.DiaFin < 28 AndAlso p.IrUltimaSemana Then
                        bruto = 0 : brutoMensual = 0 : brutoAnual = 0
                    ElseIf Not p.IrUltimaSemana Then
                        brutoMensual = bruto * 2
                        brutoAnual = brutoMensual * 12
                        If bruto = 0 AndAlso p.DiasSubsidio >= 15 AndAlso p.MontoSubsidio = 0 Then
                            bruto = p.SueldoPeriodo
                            brutoMensual = bruto * 2
                            brutoAnual = brutoMensual * 12
                        End If
                    Else
                        brutoMensual = bruto + p.SueldoAnteriorMes - p.InssAnteriorMes
                        brutoAnual = brutoMensual * 12
                        If bruto = 0 AndAlso p.DiasSubsidio >= 15 AndAlso p.MontoSubsidio = 0 Then
                            bruto = p.SueldoPeriodo
                            brutoMensual = bruto * 2
                            brutoAnual = brutoMensual * 12
                        End If
                    End If
                Else
                    brutoAnual = ((p.TotalDevengadoAcumulado + bruto) / p.NQuincenas) * 24
                    brutoMensual = bruto * 2
                    If bruto = 0 AndAlso p.DiasSubsidio >= 15 AndAlso p.MontoSubsidio = 0 Then
                        bruto = p.SueldoPeriodo
                        brutoAnual = ((p.TotalDevengadoAcumulado + bruto) / p.NQuincenas) * 24
                        brutoMensual = bruto * 2
                    End If
                End If

            Case "Mensual"
                brutoMensual = bruto
                brutoAnual = ((p.TotalDevengadoAcumulado + bruto) * (12 - (p.PeriodoIngreso - 1))) / p.NQuincenas
                If bruto = 0 AndAlso p.DiasSubsidio >= 15 AndAlso p.MontoSubsidio = 0 Then
                    bruto = p.SueldoPeriodo
                    brutoMensual = bruto
                    brutoAnual = ((p.TotalDevengadoAcumulado + bruto) * (12 - (p.PeriodoIngreso - 1))) / p.NQuincenas
                End If

            Case "Trimestral", "Semestral"
                Dim n As Double = If(p.Periodo = "Trimestral", 3, 6)
                bruto = p.SalarioMensual - p.InssMensual
                brutoMensual = bruto / n
                If bruto = 0 AndAlso p.DiasSubsidio >= 15 AndAlso p.MontoSubsidio = 0 Then
                    bruto = p.SueldoPeriodo
                    brutoMensual = bruto / n
                End If
        End Select

    End Sub

#End Region

#Region "Quincenal"

    Private Shared Sub CalcularQuincenal(p As ParametrosIR, gravables As List(Of CalculadoraIRBaja.TramoIR), brutoAnual As Double, ByRef ir As Double, ByRef irPatronal As Double)

        Dim todos As List(Of CalculadoraIRBaja.TramoIR) = p.Tramos.ToList()
        Dim x12 As Boolean = p.TipoCalculoIr = "Calcular IR x 12"
        Dim tramo As CalculadoraIRBaja.TramoIR = gravables.FirstOrDefault(Function(t) t.Contiene(brutoAnual))
        Dim impuesto As Double = If(tramo Is Nothing, 0, tramo.Impuesto(brutoAnual))

        ' Vacaciones: IR del incremento anual que producen (INSS de vacaciones ya calculado por quien llama).
        Dim irVaca As Double = 0
        If p.VacacionesPagadas <> 0 Then
            Dim montoVaca As Double = p.VacacionesPagadas - p.InssVacaciones
            irVaca = ImpuestoAnual(todos, brutoAnual + montoVaca) - If(tramo Is Nothing, 0, impuesto)
        End If

        If tramo IsNot Nothing Then
            If p.VacacionesPagadas <> 0 AndAlso p.MontoSubsidio = 0 AndAlso p.DiasSubsidio >= 15 Then impuesto = 0
            AplicarForma(p, x12, impuesto, irVaca, ir, irPatronal)
            Return
        End If

        ' Sin tramo gravable (IR anual = 0): solo cuentan las vacaciones.
        If p.VacacionesPagadas <> 0 Then
            Dim montoVaca As Double = p.VacacionesPagadas - p.InssVacaciones
            irVaca = ImpuestoAnual(todos, brutoAnual + montoVaca)
            If irVaca <> 0 Then AplicarForma(p, x12, 0, irVaca, ir, irPatronal)
        End If

    End Sub

    Private Shared Sub AplicarForma(p As ParametrosIR, x12 As Boolean, impuestoAnual As Double, irVaca As Double, ByRef ir As Double, ByRef irPatronal As Double)

        If x12 Then
            If p.DiaFin < 28 Then
                ir = R(impuestoAnual / 2 / 12)
                irPatronal = ir
                ir += irVaca
            ElseIf Not p.IrUltimaSemana Then
                ir = R(impuestoAnual / 24) + irVaca
                irPatronal = ir - p.IrPatronalAnterior
            Else
                ir = R((impuestoAnual / 12) - p.IrAnteriorMes) + irVaca
                irPatronal = ir - p.IrPatronalAnterior
            End If
        Else
            If p.NumeroPeriodo <> 0 Then
                ir = (impuestoAnual - p.IrAcumulado) / p.NumeroPeriodo
            Else
                ir = 0
            End If
            irPatronal = ir - p.IrPatronalAnterior
        End If

    End Sub

#End Region

    ''' <summary>Tramo del bruto mensual (bruto*12 contra la tabla) si alcanza el mínimo gravable.</summary>
    Private Shared Function TramoMensual(gravables As List(Of CalculadoraIRBaja.TramoIR), brutoMensual As Double, minimo As Double) As CalculadoraIRBaja.TramoIR
        If brutoMensual < minimo Then Return Nothing
        Dim anual As Double = brutoMensual * 12
        Return gravables.FirstOrDefault(Function(t) t.Contiene(anual))
    End Function

    Private Shared Function R(valor As Double) As Double
        Return Math.Round(valor, 2, MidpointRounding.AwayFromZero)
    End Function

End Class
