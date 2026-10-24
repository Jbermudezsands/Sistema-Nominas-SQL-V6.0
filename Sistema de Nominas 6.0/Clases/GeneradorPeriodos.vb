''' <summary>
''' Calcula el calendario de períodos de una nómina (migrado de frmFecha / FrmPeriodoFiscal de la V.5.0).
''' No toca la base de datos: devuelve la lista de períodos para un rango de fechas.
''' </summary>
Public Class GeneradorPeriodos

    Public Shared ReadOnly Soportados As String() = {
        "Semanal Sabado", "Semanal Viernes",
        "Catorcenal los Sabados", "Catorcenal los Viernes",
        "Quincenal", "Mensual"}

    Public Shared Function EsSoportado(periodo As String) As Boolean
        Return Soportados.Any(Function(p) String.Equals(p, periodo, StringComparison.OrdinalIgnoreCase))
    End Function

    ''' <param name="periodo">Valor de TipoNomina.Periodo.</param>
    ''' <param name="inicio">Primer día del calendario.</param>
    ''' <param name="fin">Último día del calendario.</param>
    ''' <param name="anio">Año que se graba en cada período.</param>
    Public Shared Function Generar(periodo As String, inicio As Date, fin As Date, anio As Integer) As List(Of PeriodoNomina)

        If fin < inicio Then
            Throw New ArgumentException("La fecha final no puede ser menor que la fecha inicial.")
        End If

        Select Case periodo.Trim.ToLowerInvariant()
            Case "quincenal"
                Return Quincenal(inicio, fin, anio)
            Case "mensual"
                Return Mensual(inicio, fin, anio)
            Case "semanal sabado"
                Return PorDiaDeSemana(inicio, fin, anio, DayOfWeek.Saturday, 7, 52, 0)
            Case "semanal viernes"
                Return PorDiaDeSemana(inicio, fin, anio, DayOfWeek.Friday, 7, 52, 0)
            Case "catorcenal los sabados"
                Return PorDiaDeSemana(inicio, fin, anio, DayOfWeek.Saturday, 14, 26, 2)
            Case "catorcenal los viernes"
                Return PorDiaDeSemana(inicio, fin, anio, DayOfWeek.Friday, 14, 26, 2)
            Case Else
                Throw New NotSupportedException(
                    "El período de nómina '" & periodo & "' no tiene generador de calendario.")
        End Select

    End Function

    ''' <summary>Dos períodos por mes: del 1 al 15 y del 16 al último día. Máximo 24.</summary>
    Private Shared Function Quincenal(inicio As Date, fin As Date, anio As Integer) As List(Of PeriodoNomina)

        Dim lista As New List(Of PeriodoNomina)
        Dim mes As New Date(inicio.Year, inicio.Month, 1)

        Do While mes <= fin AndAlso lista.Count < 24

            Dim ultimo As Date = mes.AddMonths(1).AddDays(-1)

            Dim q1Ini As Date = If(inicio > mes, inicio, mes)
            Dim q1Fin As Date = Minimo(mes.AddDays(14), fin)

            If q1Ini <= q1Fin Then Agregar(lista, anio, q1Ini, q1Fin, mes.Month)

            Dim q2Ini As Date = If(inicio > mes.AddDays(15), inicio, mes.AddDays(15))
            Dim q2Fin As Date = Minimo(ultimo, fin)

            If q2Ini <= q2Fin AndAlso lista.Count < 24 Then Agregar(lista, anio, q2Ini, q2Fin, mes.Month)

            mes = mes.AddMonths(1)

        Loop

        Return lista

    End Function

    ''' <summary>Un período por mes calendario. Máximo 12.</summary>
    Private Shared Function Mensual(inicio As Date, fin As Date, anio As Integer) As List(Of PeriodoNomina)

        Dim lista As New List(Of PeriodoNomina)
        Dim mes As New Date(inicio.Year, inicio.Month, 1)

        Do While mes <= fin AndAlso lista.Count < 12

            Dim ultimo As Date = mes.AddMonths(1).AddDays(-1)

            Agregar(lista, anio, If(inicio > mes, inicio, mes), Minimo(ultimo, fin), mes.Month)

            mes = mes.AddMonths(1)

        Loop

        Return lista

    End Function

    ''' <summary>
    ''' Períodos de 7 o 14 días consecutivos desde la fecha inicial. Cada mes recibe tantos períodos
    ''' como veces aparece <paramref name="dia"/> en él (semanales) o 2 (catorcenales, más uno en
    ''' los meses de ajuste: junio y diciembre en año bisiesto; julio y diciembre en los demás).
    ''' </summary>
    Private Shared Function PorDiaDeSemana(inicio As Date, fin As Date, anio As Integer, dia As DayOfWeek,
                                           dias As Integer, maximo As Integer, basePorMes As Integer) As List(Of PeriodoNomina)

        Dim lista As New List(Of PeriodoNomina)
        Dim ini As Date = inicio
        Dim mesRef As New Date(inicio.Year, inicio.Month, 1)
        Dim mesesAjuste As Integer() = If(anio Mod 4 = 0, New Integer() {6, 12}, New Integer() {7, 12})

        For m As Integer = 0 To 11

            Dim mes As Date = mesRef.AddMonths(m)

            Dim cantidad As Integer =
                If(basePorMes = 0, ContarDia(mes, dia), basePorMes + If(mesesAjuste.Contains(mes.Month), 1, 0))

            For k As Integer = 1 To cantidad

                If lista.Count >= maximo OrElse ini > fin Then Return lista

                Dim f As Date = ini.AddDays(dias - 1)

                Agregar(lista, anio, ini, f, mes.Month)

                ini = f.AddDays(1)

            Next

        Next

        Return lista

    End Function

    Private Shared Sub Agregar(lista As List(Of PeriodoNomina), anio As Integer, ini As Date, fin As Date, mes As Integer)

        lista.Add(New PeriodoNomina With {
            .Periodo = lista.Count + 1,
            .Anio = anio,
            .Mes = mes.ToString("00"),
            .Inicio = ini,
            .Final = fin,
            .Actual = (lista.Count = 0)
        })

    End Sub

    Private Shared Function ContarDia(mes As Date, dia As DayOfWeek) As Integer

        Dim n As Integer = 0

        For d As Integer = 1 To Date.DaysInMonth(mes.Year, mes.Month)
            If New Date(mes.Year, mes.Month, d).DayOfWeek = dia Then n += 1
        Next

        Return n

    End Function

    Private Shared Function Minimo(a As Date, b As Date) As Date
        Return If(a < b, a, b)
    End Function

End Class
