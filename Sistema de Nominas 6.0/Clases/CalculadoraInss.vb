''' <summary>
''' INSS laboral y patronal de la nómina, port de CalcularMontoINSS (Funciones.bas, V.5.0) sin acceso a datos ni
''' variables globales. Lo usan todos los formularios que calculan nómina (normal, vacaciones, liquidación...).
''' Dos modos, según TipoNomina.PorcientoInss: porcentaje (TasaInss/TasaInssPatronal) o tabla (tabla Inss).
''' </summary>
Public Class CalculadoraInss

    ''' <summary>Techo mensual de cotización (constante fija de la V.5.0).</summary>
    Public Const TechoMensual As Double = 88005.78

    ''' <summary>Fila de la tabla Inss. Columnas 1 = semanal, 4 = mes de 4 semanas, 5 = mes de 5 semanas.</summary>
    Public Class TramoInss
        Public Property Desde As Double
        Public Property Hasta As Double
        Public Property Laboral1 As Double
        Public Property Patronal1 As Double
        Public Property Laboral4 As Double
        Public Property Patronal4 As Double
        Public Property Laboral5 As Double
        Public Property Patronal5 As Double
    End Class

    Public Class ParametrosInss
        ''' <summary>TipoNomina.Periodo (Quincenal, Mensual, Semanal Viernes...).</summary>
        Public Property Periodo As String = ""
        Public Property PorcientoInss As Boolean
        Public Property TasaInss As Double
        Public Property TasaInssPatronal As Double
        ''' <summary>Tabla Inss ordenada por Desde (solo en modo tabla).</summary>
        Public Property Tramos As IList(Of TramoInss) = New List(Of TramoInss)()
        Public Property TotalDevengado As Double
        ''' <summary>Base para buscar el tramo de la tabla (en la V.5.0 el parámetro SalarioMensual).</summary>
        Public Property SalarioMensual As Double
        Public Property Excento As Boolean
        Public Property DiasSubsidio As Double
        Public Property MontoSubsidio As Double
        ''' <summary>Día del mes de la fecha final de la nómina (DiaFin >= 28 = cierre de mes).</summary>
        Public Property DiaFin As Integer
        ''' <summary>Sábados/viernes del mes: 4 usa la columna 4 de la tabla, otro valor la 5.</summary>
        Public Property CantSabados As Integer = 4
        ''' <summary>INSS ya retenido en las nóminas anteriores del mismo mes.</summary>
        Public Property InssAnterior As Double
        Public Property InssPatronalAnterior As Double
    End Class

    Public Class ResultadoInss
        Public Property Inss As Double
        Public Property Patronal As Double
        ''' <summary>INSS acumulado del mes (anterior + el de esta nómina).</summary>
        Public Property Mensual As Double
        Public Property PatronalMensual As Double
    End Class

    ''' <summary>Días de cada período (PeriodoDias de la V.5.0).</summary>
    Public Shared Function DiasPeriodo(periodo As String) As Double
        Select Case periodo
            Case "Semanal Viernes", "Semanal Sabado" : Return 7
            Case "Catorcenal los Viernes", "Catorcenal los Sabados" : Return 14
            Case "Quincenal" : Return 15
            Case "Mensual" : Return 30
            Case "Trimestral" : Return 90
            Case "Semestral" : Return 180
        End Select
        Return 0
    End Function

    Public Shared Function Calcular(p As ParametrosInss) As ResultadoInss

        Dim r As New ResultadoInss()

        ' Subsidio todo el período y sin monto: no cotiza.
        Dim excento As Boolean = p.Excento
        If p.DiasSubsidio >= DiasPeriodo(p.Periodo) AndAlso p.MontoSubsidio = 0 Then excento = True
        If excento Then Return r

        If p.PorcientoInss Then
            CalcularPorPorcentaje(p, r)
        Else
            CalcularPorTabla(p, r)
        End If

        Return r

    End Function

    Private Shared Sub CalcularPorPorcentaje(p As ParametrosInss, r As ResultadoInss)

        Dim tasa As Double = p.TasaInss / 100
        Dim tasaPat As Double = p.TasaInssPatronal / 100
        Dim cierreMes As Boolean = p.DiaFin >= 28

        Select Case p.Periodo
            Case "Quincenal"
                If Not cierreMes Then
                    If p.TotalDevengado > TechoMensual Then
                        r.Inss = (TechoMensual * tasa) / 2
                        r.Patronal = (TechoMensual * tasaPat) / 2
                    Else
                        r.Inss = p.TotalDevengado * tasa
                        r.Patronal = p.TotalDevengado * tasaPat
                    End If
                Else
                    Dim mensual As Double = (p.TotalDevengado * tasa) + p.InssAnterior
                    If mensual > TechoMensual * tasa Then
                        r.Inss = (TechoMensual * tasa) - p.InssAnterior
                        r.Patronal = (TechoMensual * tasaPat) - p.InssPatronalAnterior
                    Else
                        r.Inss = p.TotalDevengado * tasa
                        r.Patronal = p.TotalDevengado * tasaPat
                    End If
                End If
            Case "Mensual"
                ' La V.5.0 usaba 0.0625 y 0.019 fijos sobre el techo; aquí se usan las tasas del tipo de nómina.
                If p.TotalDevengado > TechoMensual Then
                    r.Inss = TechoMensual * tasa
                    r.Patronal = TechoMensual * tasaPat
                Else
                    r.Inss = p.TotalDevengado * tasa
                    r.Patronal = p.TotalDevengado * tasaPat
                End If
            Case Else
                r.Inss = p.TotalDevengado * tasa
                r.Patronal = p.TotalDevengado * tasaPat
        End Select

        r.Mensual = p.InssAnterior + r.Inss
        r.PatronalMensual = p.InssPatronalAnterior + r.Patronal

    End Sub

    Private Shared Sub CalcularPorTabla(p As ParametrosInss, r As ResultadoInss)

        Dim tramo As TramoInss = p.Tramos.FirstOrDefault(Function(t) t.Desde < p.SalarioMensual AndAlso t.Hasta > p.SalarioMensual)
        If tramo Is Nothing Then Return

        Dim cuatro As Boolean = p.CantSabados = 4
        Dim laboral As Double = If(cuatro, tramo.Laboral4, tramo.Laboral5)
        Dim patronal As Double = If(cuatro, tramo.Patronal4, tramo.Patronal5)
        Dim cierreMes As Boolean = p.DiaFin >= 28

        Select Case p.Periodo
            Case "Semanal Viernes", "Semanal Sabado"
                r.Inss = tramo.Laboral1
                r.Patronal = tramo.Patronal1
            Case "Catorcenal los Viernes", "Catorcenal los Sabados", "Quincenal"
                If Not cierreMes Then
                    r.Inss = laboral / 2
                    r.Patronal = patronal / 2
                Else
                    r.Mensual = laboral
                    r.PatronalMensual = patronal
                    r.Inss = laboral - p.InssAnterior
                    r.Patronal = patronal - p.InssPatronalAnterior
                    Return
                End If
            Case "Mensual"
                r.Inss = laboral
                r.Patronal = patronal
            Case "Trimestral"
                r.Inss = laboral * 3
                r.Patronal = patronal * 3
            Case "Semestral"
                r.Inss = laboral * 6
                r.Patronal = patronal * 6
        End Select

        r.Mensual = p.InssAnterior + r.Inss
        r.PatronalMensual = p.InssPatronalAnterior + r.Patronal

    End Sub

End Class
