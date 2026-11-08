Imports System.Globalization
Imports DevExpress.Drawing
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

''' <summary>
''' Control de vacaciones del empleado (calendario anual de <see cref="FrmControlVacaciones"/>) en DevExpress: encabezado de empresa,
''' datos del empleado, saldo a la fecha de corte, días del año por tipo, los 12 meses con cada día pintado según su tipo,
''' leyenda y el detalle de solicitudes (un renglón por tramo de fechas).
''' </summary>
Public Class RptControlVacaciones
    Inherits XtraReport

    Private Const Ancho As Single = 1020
    Private Const AltoFilaMes As Single = 13

    Private Shared ReadOnly Cultura As New CultureInfo("es-NI")
    Private Shared ReadOnly ColorSeccion As Color = Color.FromArgb(219, 226, 242)

    Public Sub New(empresa As DatosEmpresa, control As ControlVacaciones)

        DisplayName = "Control de vacaciones " & control.Codigo
        PaperKind = DevExpress.Drawing.Printing.DXPaperKind.Letter
        Landscape = True
        Margins = New DevExpress.Drawing.DXMargins(40, 40, 40, 40)
        Font = New DXFont(ConstructorReporte.Fuente, 8)

        ' Detalle de solicitudes: una fila por tramo.
        Dim tramos As DataTable = TablaTramos(control)
        DataSource = tramos

        Dim encabezado As New ReportHeaderBand
        Dim y As Single = ConstructorReporte.Encabezado(encabezado, empresa, "CONTROL DE VACACIONES", "Año " & control.Anio, Ancho)

        y = DatosEmpleado(encabezado, control, y)
        y = Resumen(encabezado, control, y)
        y = Calendario(encabezado, control, y)
        y = Leyenda(encabezado, control, y)
        y = TitulosDetalle(encabezado, y, tramos.Rows.Count > 0)
        encabezado.HeightF = y

        Dim detalle As New DetailBand With {.HeightF = 15}
        detalle.Controls.Add(ConstructorReporte.Campo("Solicitud", 0, 0, 90, 15))
        detalle.Controls.Add(ConstructorReporte.Campo("Tipo", 90, 0, 170, 15))
        detalle.Controls.Add(ConstructorReporte.Campo("Inicio", 260, 0, 90, 15, "{0:dd/MM/yyyy}"))
        detalle.Controls.Add(ConstructorReporte.Campo("Fin", 350, 0, 90, 15, "{0:dd/MM/yyyy}"))
        detalle.Controls.Add(ConstructorReporte.Campo("Dias", 440, 0, 70, 15, "{0:#,##0.##}", False, 8, TextAlignment.MiddleRight))

        Dim pie As New PageFooterBand With {.HeightF = 22}
        pie.Controls.Add(New XRPageInfo With {
            .PageInfo = PageInfo.NumberOfTotal, .TextFormatString = "Página {0} de {1}",
            .BoundsF = New RectangleF(Ancho - 150, 4, 150, 14), .TextAlignment = TextAlignment.MiddleRight,
            .Font = New DXFont(ConstructorReporte.Fuente, 8)})

        Bands.AddRange(New Band() {encabezado, detalle, pie})

    End Sub

#Region "SECCIONES"

    Private Shared Function DatosEmpleado(banda As Band, c As ControlVacaciones, y As Single) As Single

        banda.Controls.Add(ConstructorReporte.Etiqueta("Empleado:", 0, y, 85, 18, True, 10))
        banda.Controls.Add(ConstructorReporte.Etiqueta(c.Codigo & "  -  " & c.Nombre_Completo, 85, y, Ancho - 85, 18, True, 10))
        y += 20

        Dim filas As String()() = {
            New String() {"Departamento:", c.Departamento, "Cargo:", c.Cargo},
            New String() {"Fecha contrato vacaciones:", If(c.Fecha_Contrato_Vac.HasValue, c.Fecha_Contrato_Vac.Value.ToString("dd/MM/yyyy"), "(sin fecha)"),
                          "Saldo calculado al:", c.Fecha_Corte.ToString("dd/MM/yyyy")}}

        For Each f As String() In filas
            banda.Controls.Add(ConstructorReporte.Etiqueta(f(0), 0, y, 170, 14, True))
            banda.Controls.Add(ConstructorReporte.Etiqueta(f(1), 170, y, 330, 14))
            banda.Controls.Add(ConstructorReporte.Etiqueta(f(2), 510, y, 130, 14, True))
            banda.Controls.Add(ConstructorReporte.Etiqueta(f(3), 640, y, 380, 14))
            y += 14
        Next

        Return y + 6

    End Function

    ''' <summary>Cuadro con el saldo oficial y los días del año por categoría.</summary>
    Private Shared Function Resumen(banda As Band, c As ControlVacaciones, y As Single) As Single

        Dim r As ResumenVacaciones = c.Resumen

        Dim cajas As String()() = {
            New String() {"Días acumulados", r.Acumulado.ToString("n2")},
            New String() {"Días solicitados", r.Solicitado.ToString("n2")},
            New String() {"Saldo disponible", r.Saldo.ToString("n2")},
            New String() {"Programados " & c.Anio, c.Programados_Anio.ToString("n2")},
            New String() {"Disponible sin programar", c.Disponible_Sin_Programar.ToString("n2")},
            New String() {"Tomados " & c.Anio, c.Disfrutados_Anio.ToString("n2")}}

        Dim anchoCaja As Single = Ancho / cajas.Length

        For i As Integer = 0 To cajas.Length - 1
            Dim titulo As XRLabel = ConstructorReporte.Etiqueta(cajas(i)(0), i * anchoCaja, y, anchoCaja, 14, True, 8, TextAlignment.MiddleCenter)
            titulo.BackColor = ColorSeccion
            titulo.Borders = BorderSide.All
            Dim valor As XRLabel = ConstructorReporte.Etiqueta(cajas(i)(1), i * anchoCaja, y + 14, anchoCaja, 20, True, 12, TextAlignment.MiddleCenter)
            valor.Borders = BorderSide.All
            banda.Controls.Add(titulo)
            banda.Controls.Add(valor)
        Next

        Return y + 34 + 8

    End Function

    ''' <summary>12 meses (4 x 3) con los días pintados.</summary>
    Private Shared Function Calendario(banda As Band, c As ControlVacaciones, y As Single) As Single

        Dim dias As ILookup(Of Date, DiaVacacion) = c.Dias.ToLookup(Function(d) d.Fecha.Date)
        Dim anchoMes As Single = Ancho / 4
        Dim altoMes As Single = 14 + 7 * AltoFilaMes + 6

        For mes As Integer = 1 To 12

            Dim x As Single = ((mes - 1) Mod 4) * anchoMes
            Dim yMes As Single = y + ((mes - 1) \ 4) * altoMes
            Dim anchoTabla As Single = anchoMes - 10

            banda.Controls.Add(ConstructorReporte.Etiqueta(Cultura.TextInfo.ToTitleCase(Cultura.DateTimeFormat.GetMonthName(mes)) & " " & c.Anio,
                                                           x, yMes, anchoTabla, 14, True, 9, TextAlignment.MiddleCenter))

            Dim tabla As New XRTable With {.BoundsF = New RectangleF(x, yMes + 14, anchoTabla, 7 * AltoFilaMes), .Borders = BorderSide.All, .BorderColor = Color.Silver}
            tabla.BeginInit()

            Dim letras As String() = {"L", "M", "M", "J", "V", "S", "D"}
            Dim cab As XRTableRow = NuevaFila(anchoTabla)
            For i As Integer = 0 To 6
                cab.Cells(i).Text = letras(i)
                cab.Cells(i).BackColor = ColorSeccion
                cab.Cells(i).Font = New DXFont(ConstructorReporte.Fuente, 7, DXFontStyle.Bold)
            Next
            tabla.Rows.Add(cab)

            Dim primero As New Date(c.Anio, mes, 1)
            Dim desfase As Integer = (CInt(primero.DayOfWeek) + 6) Mod 7

            For semana As Integer = 0 To 5
                Dim fila As XRTableRow = NuevaFila(anchoTabla)
                For col As Integer = 0 To 6
                    Dim numero As Integer = semana * 7 + col - desfase + 1
                    If numero < 1 OrElse numero > Date.DaysInMonth(c.Anio, mes) Then Continue For

                    Dim celda As XRTableCell = fila.Cells(col)
                    celda.Text = numero.ToString()

                    Dim marcas As IEnumerable(Of DiaVacacion) = dias(New Date(c.Anio, mes, numero))
                    If marcas.Any() Then
                        Dim principal As DiaVacacion = marcas.OrderBy(Function(m) PaletaVacaciones.Orden(m.Tipo)).First()
                        celda.BackColor = PaletaVacaciones.ColorDe(principal.Tipo)
                        celda.ForeColor = PaletaVacaciones.ColorTexto(celda.BackColor)
                    ElseIf col = 6 Then
                        celda.ForeColor = Color.Red
                    ElseIf col = 5 Then
                        celda.ForeColor = Color.RoyalBlue
                    End If
                Next
                tabla.Rows.Add(fila)
            Next

            tabla.EndInit()
            banda.Controls.Add(tabla)

        Next

        Return y + 3 * altoMes + 4

    End Function

    Private Shared Function NuevaFila(anchoTabla As Single) As XRTableRow
        Dim fila As New XRTableRow With {.HeightF = AltoFilaMes}
        For i As Integer = 0 To 6
            fila.Cells.Add(New XRTableCell With {
                .WidthF = anchoTabla / 7, .TextAlignment = TextAlignment.MiddleCenter, .Padding = New PaddingInfo(0, 0, 0, 0),
                .Font = New DXFont(ConstructorReporte.Fuente, 7), .Text = ""})
        Next
        Return fila
    End Function

    ''' <summary>Cuadro de color por tipo con los días del año; solo los tipos que aparecen.</summary>
    Private Shared Function Leyenda(banda As Band, c As ControlVacaciones, y As Single) As Single

        Dim tipos As IList(Of KeyValuePair(Of String, Double)) = c.DiasPorTipo()
        Dim x As Single = 0

        For Each t As KeyValuePair(Of String, Double) In tipos
            Dim muestra As New XRLabel With {.BoundsF = New RectangleF(x, y + 2, 14, 11), .BackColor = PaletaVacaciones.ColorDe(t.Key), .Borders = BorderSide.All, .BorderColor = Color.Gray}
            banda.Controls.Add(muestra)
            Dim texto As String = t.Key & ": " & t.Value.ToString("0.##")
            Dim ancho As Single = 22 + texto.Length * 5.2F
            banda.Controls.Add(ConstructorReporte.Etiqueta(texto, x + 16, y, ancho, 15))
            x += 16 + ancho + 8
        Next

        If tipos.Count = 0 Then banda.Controls.Add(ConstructorReporte.Etiqueta("Sin días marcados en " & c.Anio & ".", 0, y, 400, 15))

        Return y + 22

    End Function

    Private Shared Function TitulosDetalle(banda As Band, y As Single, hayDetalle As Boolean) As Single

        If Not hayDetalle Then Return y

        banda.Controls.Add(ConstructorReporte.Etiqueta("DETALLE DE SOLICITUDES", 0, y, 300, 16, True, 9))
        y += 18

        Dim titulos As String() = {"Solicitud", "Tipo", "Desde", "Hasta", "Días"}
        Dim posiciones As Single() = {0, 90, 260, 350, 440}
        Dim anchos As Single() = {90, 170, 90, 90, 70}

        For i As Integer = 0 To titulos.Length - 1
            Dim lbl As XRLabel = ConstructorReporte.Etiqueta(titulos(i), posiciones(i), y, anchos(i), 15, True, 8,
                                                             If(i = 4, TextAlignment.MiddleRight, TextAlignment.MiddleLeft))
            lbl.BackColor = ColorSeccion
            lbl.Borders = BorderSide.Bottom
            banda.Controls.Add(lbl)
        Next

        Return y + 17

    End Function

    Private Shared Function TablaTramos(c As ControlVacaciones) As DataTable

        Dim t As New DataTable
        t.Columns.Add("Solicitud", GetType(String))
        t.Columns.Add("Tipo", GetType(String))
        t.Columns.Add("Inicio", GetType(Date))
        t.Columns.Add("Fin", GetType(Date))
        t.Columns.Add("Dias", GetType(Double))

        For Each tr As TramoVacacion In c.Tramos()
            t.Rows.Add(tr.Numero_Solicitud, tr.Tipo, tr.Inicio, tr.Fin, tr.Dias)
        Next

        Return t

    End Function

#End Region

End Class
