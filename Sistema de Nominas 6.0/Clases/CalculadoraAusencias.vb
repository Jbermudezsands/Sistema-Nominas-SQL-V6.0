''' <summary>
''' Ausencias, subsidio, vacaciones pagadas y antigüedad de la nómina. Port de la clasificación de solicitudes de
''' CmdCalcular_Click, CalcularSubsidio, CalcularVacacionesPagadasSolicitud y del bloque de antigüedad
''' (FrmCalcularNomina.frm, V.5.0), sin acceso a datos ni variables globales.
''' </summary>
Public Class CalculadoraAusencias

    ''' <summary>Solicitud de ausencia ya agrupada por tipo (SolicitudVacaciones.DiasDisfrutar).</summary>
    Public Class SolicitudDias
        Public Property TipoSolicitud As String = ""
        Public Property Dias As Double
    End Class

    ''' <summary>Solicitud de subsidio que se cruza con el período de la nómina.</summary>
    Public Class SolicitudSubsidio
        Public Property Inicio As Date
        Public Property Fin As Date
        ''' <summary>Embarazo, Riesgo Laboral u otra (Clasificacion de la solicitud).</summary>
        Public Property Clasificacion As String = ""
    End Class

    Public Class ResultadoAusencias
        ''' <summary>ConDiasDescuento: días que se descuentan como falta (deducción 01).</summary>
        Public Property DiasDescuento As Double
        ''' <summary>DiasBasico: días de salario básico que se restan del devengado.</summary>
        Public Property DiasBasico As Double
        Public Property DiasBasicoExcento As Double
    End Class

    Public Class ResultadoSubsidio
        ''' <summary>Días de subsidio del período (pDiasSubsidio).</summary>
        Public Property DiasSubsidio As Double
        ''' <summary>Días que se suman a DiasBasico (TotalDiasBasico).</summary>
        Public Property DiasBasico As Double
        Public Property MontoSubsidio As Double
        ''' <summary>Clasificación de la última solicitud (se graba en DetalleNomina.Clasificacion_Subsidio).</summary>
        Public Property Clasificacion As String = ""
    End Class

    ''' <summary>Convierte las solicitudes del período en días de descuento y de salario básico.</summary>
    Public Shared Function ClasificarSolicitudes(solicitudes As IEnumerable(Of SolicitudDias)) As ResultadoAusencias
        Dim r As New ResultadoAusencias()
        For Each s As SolicitudDias In solicitudes
            Select Case s.TipoSolicitud
                Case "Ausente", "Ausencia", "AusenteBasicoDINSS", "Llegada Tarde"
                    r.DiasDescuento = s.Dias
                Case "AusenteBasico"
                    r.DiasBasico = s.Dias
                Case "AusenteBasicoExcento"
                    r.DiasBasicoExcento = s.Dias
                    r.DiasDescuento = s.Dias
            End Select
        Next
        Return r
    End Function

    ''' <summary>Días calendario del traslape entre una solicitud y el período (las cuatro comparaciones de la V.5.0, en el mismo orden).</summary>
    Public Shared Function DiasTraslape(inicioSolicitud As Date, finSolicitud As Date, inicioNomina As Date, finNomina As Date) As Double
        Dim solIni As Date = inicioSolicitud.Date, solFin As Date = finSolicitud.Date, nomIni As Date = inicioNomina.Date, nomFin As Date = finNomina.Date
        Dim dias As Double = 0
        If solIni <= nomIni AndAlso solFin >= nomFin Then dias = (nomFin - nomIni).Days + 1
        If solIni >= nomIni AndAlso solFin >= nomFin Then dias = (nomFin - solIni).Days + 1
        If solIni >= nomIni AndAlso solFin <= nomFin Then dias = (solFin - solIni).Days + 1
        If solIni <= nomIni AndAlso solFin <= nomFin Then dias = (solFin - nomIni).Days + 1
        If dias > 15 Then dias = 15
        Return dias
    End Function

    ''' <summary>CalcularSubsidio de la V.5.0.</summary>
    ''' <param name="montoDia">BuscaMontoDia del tipo de nómina (<see cref="CalculadoraNomina.MontoDiaPeriodo"/>).</param>
    ''' <param name="diasDescuento">ConDiasDescuento ya clasificado.</param>
    ''' <param name="porcientoSubsidio">DatosEmpresa.PorcientoSubsidio (0 = el subsidio paga el día completo).</param>
    ''' <param name="restar3Dias">DatosEmpresa.Restar3DiasSubsidio.</param>
    Public Shared Function CalcularSubsidio(solicitudes As IEnumerable(Of SolicitudSubsidio), inicioNomina As Date, finNomina As Date, diasDescuento As Double, montoDia As Double, porcientoSubsidio As Double, restar3Dias As Boolean) As ResultadoSubsidio

        Dim r As New ResultadoSubsidio()
        Dim diasNomina As Double = (finNomina.Date - inicioNomina.Date).Days + 1
        Dim totalDiasSubsidio As Double = 0
        Dim totalDiasBasico As Double = 0
        Dim totalMonto As Double = 0

        For Each s As SolicitudSubsidio In solicitudes

            Dim diasAPagar As Double = DiasTraslape(s.Inicio, s.Fin, inicioNomina, finNomina)
            Dim conPorcentaje As Boolean = s.Clasificacion = "Embarazo" OrElse s.Clasificacion = "Riesgo Laboral"
            Dim pDias As Double
            Dim monto As Double = 0
            Dim diasBasico As Double

            If porcientoSubsidio > 0 Then
                If Not restar3Dias Then
                    pDias = diasAPagar
                    If conPorcentaje Then
                        monto = (pDias * montoDia) * (porcientoSubsidio / 100)
                        If (totalDiasSubsidio + pDias + diasDescuento) > diasNomina Then monto = (diasNomina * montoDia) * (porcientoSubsidio / 100)
                    End If
                    diasBasico = diasDescuento + diasAPagar
                ElseIf diasAPagar > 3 Then
                    pDias = diasAPagar - 3
                    If conPorcentaje Then
                        monto = (pDias * montoDia) * (porcientoSubsidio / 100)
                        If (totalDiasSubsidio + pDias + diasDescuento) > diasNomina Then monto = (diasNomina * montoDia) * (porcientoSubsidio / 100)
                    Else
                        monto = pDias * montoDia
                        If (totalDiasSubsidio + pDias + diasDescuento) > diasNomina Then monto = diasNomina * montoDia
                    End If
                    diasBasico = diasDescuento + diasAPagar
                    pDias = diasAPagar
                Else
                    pDias = diasAPagar
                    diasBasico = diasDescuento + diasAPagar
                    monto = 0
                End If
            Else
                pDias = diasAPagar
                monto = pDias * montoDia
                diasBasico = diasDescuento + diasAPagar
            End If

            If (totalDiasSubsidio + pDias + diasDescuento) > diasNomina Then
                totalDiasSubsidio = diasNomina
            Else
                totalDiasSubsidio += pDias
            End If

            If (totalDiasBasico + diasBasico) > diasNomina Then
                totalDiasBasico = diasNomina
            Else
                totalDiasBasico += diasBasico
            End If

            totalMonto += monto
            r.Clasificacion = s.Clasificacion
        Next

        r.DiasSubsidio = totalDiasSubsidio
        r.DiasBasico = totalDiasBasico
        r.MontoSubsidio = totalMonto
        Return r

    End Function

    ''' <summary>
    ''' Vacaciones pagadas por solicitud (CalcularVacacionesPagadasSolicitud). La V.5.0 acumulaba los días entre
    ''' solicitudes y volvía a sumar el monto anterior, con lo que pagaba de más si había dos o más; aquí es días × valor del día.
    ''' </summary>
    Public Shared Function VacacionesPagadas(diasPorSolicitud As IEnumerable(Of Double), montoDia As Double) As ResultadoVacaciones
        Dim dias As Double = diasPorSolicitud.Sum()
        Return New ResultadoVacaciones With {.Dias = dias, .Monto = dias * montoDia}
    End Function

    Public Class ResultadoVacaciones
        Public Property Dias As Double
        Public Property Monto As Double
    End Class

    ''' <summary>Fila de la tabla Antiguedad (años acumulados → % sobre el sueldo).</summary>
    Public Class TramoAntiguedad
        Public Property Anios As Integer
        Public Property Porcentaje As Double
    End Class

    ''' <summary>Años de antigüedad a la fecha final de la nómina, redondeados a dos decimales como la V.5.0 (días / 365).</summary>
    Public Shared Function AniosAntiguedad(fechaContrato As Date, fechaNomina As Date) As Double
        Return Math.Round((fechaNomina.Date - fechaContrato.Date).Days / 365, 2, MidpointRounding.AwayFromZero)
    End Function

    ''' <summary>Recargo por antigüedad del período; 0 si no hay fila para los años cumplidos.</summary>
    ''' <param name="sueldoBase">SueldoPeriodo + AumentoBasico (la V.5.0 sumaba además el MontoSubsidio del empleado anterior, un residuo del ciclo; aquí no).</param>
    ''' <remarks>La V.5.0 tenía una rama por horas (HoraBasico × tarifa) para los demás períodos, pero preguntaba por una variable mal escrita (siempre 0), así que nunca se ejecutaba; se conserva el resultado real: porcentaje sobre el sueldo.</remarks>
    Public Shared Function Antiguedad(anios As Double, tabla As IEnumerable(Of TramoAntiguedad), sueldoBase As Double) As Double
        Dim fila As TramoAntiguedad = tabla.FirstOrDefault(Function(t) t.Anios = CInt(Math.Floor(anios)))
        If fila Is Nothing Then Return 0
        Return sueldoBase * fila.Porcentaje / 100
    End Function

End Class
