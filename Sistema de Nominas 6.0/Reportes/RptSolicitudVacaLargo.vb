Imports System.Globalization
Imports DevExpress.Drawing
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

''' <summary>
''' Constancia de solicitud de vacaciones (V.5.0 ArepSolicitudVacaLargo de ActiveReports 2.0, el que imprimía FrmSolicitud
''' para más de 3 días), armada por código sin diseñador: encabezado, carta del empleado con las fechas de descanso y
''' regreso, firmas y resumen de días. Se muestra con <see cref="FrmVisorReporte"/>.
''' </summary>
Public Class RptSolicitudVacaLargo
    Inherits XtraReport

    Private Const Ancho As Single = 750
    Private Const Fuente As String = "Arial"
    Private Shared ReadOnly ColorSeccion As Color = Color.FromArgb(219, 226, 242)
    Private Shared ReadOnly Cultura As New CultureInfo("es-NI")

    Private ReadOnly _detalle As New DetailBand
    Private _y As Single

    ''' <param name="diasAcumulados">Días de vacaciones acumulados (pantalla: Días vacaciones).</param>
    ''' <param name="diasSolicitados">Días disfrutados incluida esta solicitud (pantalla: Días disfrutados).</param>
    ''' <param name="saldo">Días disponibles después de esta solicitud.</param>
    ''' <param name="saldoPeriodosAnteriores">Línea 1 del resumen; la V.5.0 la dejaba vacía (Nothing = vacía).</param>
    Public Sub New(empresa As DatosEmpresa, sol As SolicitudVacaciones, emp As EmpleadoSolicitud,
                   diasAcumulados As Double, diasSolicitados As Double, saldo As Double,
                   Optional saldoPeriodosAnteriores As Double? = Nothing, Optional fechaImpresion As Date? = Nothing)

        Dim hoy As Date = If(fechaImpresion, Date.Now)

        DisplayName = "Solicitud de vacaciones " & sol.Numero_Solicitud
        PaperKind = DevExpress.Drawing.Printing.DXPaperKind.Letter
        Margins = New DevExpress.Drawing.DXMargins(50, 50, 40, 40)
        Font = New DXFont(Fuente, 10)

        Dim encabezado As New ReportHeaderBand
        Dim pie As New PageFooterBand With {.HeightF = 25}
        Bands.AddRange(New Band() {encabezado, _detalle, pie})

        CrearEncabezado(encabezado, empresa, sol, hoy)
        CrearCarta(empresa, sol, emp, hoy)
        CrearFirmas()
        CrearResumen(diasAcumulados, diasSolicitados, saldo, saldoPeriodosAnteriores)
        CrearObservaciones(sol.Observaciones)

        _detalle.HeightF = _y + 5
        pie.Controls.Add(New XRPageInfo With {
            .PageInfo = PageInfo.NumberOfTotal, .TextFormatString = "Página {0} de {1}",
            .BoundsF = New RectangleF(Ancho - 150, 5, 150, 15), .TextAlignment = TextAlignment.MiddleRight,
            .Font = New DXFont(Fuente, 8)})

    End Sub

#Region "SECCIONES"

    Private Sub CrearEncabezado(banda As ReportHeaderBand, empresa As DatosEmpresa, sol As SolicitudVacaciones, hoy As Date)

        Dim logo As XRPictureBox = Imagen(empresa.Ruta_Logo, New RectangleF(0, 0, 120, 75))
        If logo IsNot Nothing Then banda.Controls.Add(logo)

        banda.Controls.Add(Etiqueta(empresa.Nombre_Empresa, 130, 0, Ancho - 260, 22, True, 13, TextAlignment.MiddleCenter))
        banda.Controls.Add(Etiqueta(empresa.Direccion, 130, 22, Ancho - 260, 16, False, 8, TextAlignment.MiddleCenter))
        banda.Controls.Add(Etiqueta("SOLICITUD DE VACACIONES", 130, 44, Ancho - 260, 26, True, 15, TextAlignment.MiddleCenter))

        banda.Controls.Add(Etiqueta("Impreso: " & hoy.ToString("dd/MM/yyyy"), Ancho - 130, 0, 130, 16, False, 8, TextAlignment.MiddleRight))
        banda.Controls.Add(Etiqueta("Solicitud No: " & sol.Numero_Solicitud, Ancho - 180, 44, 180, 18, True, 10, TextAlignment.MiddleRight))
        banda.Controls.Add(Etiqueta("Fecha solicitud: " & sol.Fecha_Solicitud.ToString("dd/MM/yyyy"), Ancho - 180, 62, 180, 16, False, 9, TextAlignment.MiddleRight))

        banda.Controls.Add(New XRLine With {.BoundsF = New RectangleF(0, 85, Ancho, 2)})
        banda.HeightF = 95

    End Sub

    ''' <summary>Texto de la constancia (Label19-Label30 de la V.5.0), con los datos en negrita.</summary>
    Private Sub CrearCarta(empresa As DatosEmpresa, sol As SolicitudVacaciones, emp As EmpleadoSolicitud, hoy As Date)

        _y += 20

        Parrafo("Yo, " & B(emp.Nombre_Completo) & ", empleado(a) de " & B(empresa.Nombre_Empresa) &
                ", laborando con el cargo de " & B(emp.Cargo) & ".")

        Parrafo("Por este medio hago constar que la empresa me otorgará " & B(Dias(sol.Dias_Solicitud)) &
                " días de vacaciones, a como lo establecen las leyes laborales de Nicaragua vigentes.")

        Parrafo("Por tanto, estaré descansando del " & B(sol.Fecha_Inicio.ToString("dd/MM/yyyy")) &
                " al " & B(sol.Fecha_Fin.ToString("dd/MM/yyyy")) &
                ", regresando el " & B(sol.Fecha_Fin.Date.AddDays(1).ToString("dd/MM/yyyy")) & ".")

        Parrafo("En fe de lo antes expuesto, firmo el presente documento a los " & B(hoy.Day.ToString()) &
                " días del mes de " & B(Cultura.DateTimeFormat.GetMonthName(hoy.Month)) & " de " & B(hoy.Year.ToString()) & ".")

        _y += 30

    End Sub

    Private Sub CrearFirmas()

        Dim titulos As String() = {"FIRMA DEL EMPLEADO", "VO.BO. R.R.H.H.", "VO.BO. JEFE DE DEPARTAMENTO"}
        Dim col As Single = Ancho / titulos.Length

        For i As Integer = 0 To titulos.Length - 1
            Dim x As Single = i * col
            _detalle.Controls.Add(New XRLine With {.BoundsF = New RectangleF(x + 15, _y + 30, col - 30, 2)})
            _detalle.Controls.Add(Etiqueta(titulos(i), x, _y + 34, col, 16, True, 9, TextAlignment.MiddleCenter))
        Next

        _y += 80

    End Sub

    Private Sub CrearResumen(acumulados As Double, solicitados As Double, saldo As Double, anteriores As Double?)

        Seccion("RESUMEN DE DÍAS DE VACACIONES")

        Dim filas As String()() = {
            New String() {"1. Vacaciones períodos anteriores:", If(anteriores.HasValue, N(anteriores.Value), "")},
            New String() {"2. Días de vacaciones acumulados:", N(acumulados)},
            New String() {"3. Días solicitados de vacaciones:", N(solicitados)},
            New String() {"4. Saldo actual:", N(saldo) & " días"}}

        For Each f As String() In filas
            _detalle.Controls.Add(Etiqueta(f(0), 20, _y, 300, 18, False, 10))
            Dim valor As XRLabel = Etiqueta(f(1), 320, _y, 140, 18, True, 10, TextAlignment.MiddleRight)
            valor.Borders = BorderSide.Bottom
            _detalle.Controls.Add(valor)
            _y += 22
        Next

        _y += 10

    End Sub

    Private Sub CrearObservaciones(observaciones As String)

        Seccion("OBSERVACIONES")

        Dim lbl As XRLabel = Etiqueta(observaciones, 0, _y, Ancho, 50, False, 10, TextAlignment.TopLeft)
        lbl.Multiline = True
        lbl.WordWrap = True
        lbl.CanGrow = True
        lbl.Borders = BorderSide.All
        lbl.Padding = New PaddingInfo(5, 5, 4, 4)
        _detalle.Controls.Add(lbl)
        _y += 55

    End Sub

#End Region

#Region "AYUDAS"

    Private Sub Parrafo(markup As String)
        Dim lbl As New XRLabel With {
            .AllowMarkupText = True, .Text = markup, .BoundsF = New RectangleF(0, _y, Ancho, 40),
            .Font = New DXFont(Fuente, 11), .TextAlignment = TextAlignment.TopJustify,
            .Multiline = True, .WordWrap = True, .CanGrow = True, .Padding = New PaddingInfo(2, 2, 0, 0)}
        _detalle.Controls.Add(lbl)
        _y += 50
    End Sub

    ''' <summary>Texto en negrita para un XRLabel con markup (escapa los caracteres reservados).</summary>
    Private Shared Function B(texto As String) As String
        Dim t As String = If(texto, "").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
        Return "<b>" & t & "</b>"
    End Function

    Private Sub Seccion(titulo As String)
        Dim lbl As XRLabel = Etiqueta(titulo, 0, _y, Ancho, 18, True, 9)
        lbl.BackColor = ColorSeccion
        lbl.Padding = New PaddingInfo(4, 4, 0, 0)
        _detalle.Controls.Add(lbl)
        _y += 24
    End Sub

    Private Shared Function Etiqueta(texto As String, x As Single, y As Single, ancho As Single, alto As Single,
                                     Optional negrita As Boolean = False, Optional tamano As Single = 8,
                                     Optional alineacion As TextAlignment = TextAlignment.MiddleLeft) As XRLabel
        Return New XRLabel With {
            .Text = If(texto, ""), .BoundsF = New RectangleF(x, y, ancho, alto), .TextAlignment = alineacion,
            .Font = New DXFont(Fuente, tamano, If(negrita, DXFontStyle.Bold, DXFontStyle.Regular)),
            .Padding = New PaddingInfo(2, 2, 0, 0), .CanGrow = False}
    End Function

    ''' <summary>Imagen desde archivo; Nothing si la ruta está vacía, no existe o no es una imagen.</summary>
    Private Shared Function Imagen(ruta As String, limites As RectangleF) As XRPictureBox
        Try
            If String.IsNullOrWhiteSpace(ruta) OrElse Not IO.File.Exists(ruta) Then Return Nothing
            ' Se copia a memoria para no dejar el archivo bloqueado.
            Dim img As Image
            Using ms As New IO.MemoryStream(IO.File.ReadAllBytes(ruta)), original As Image = Image.FromStream(ms)
                img = New Bitmap(original)
            End Using
            Return New XRPictureBox With {
                .ImageSource = New DevExpress.XtraPrinting.Drawing.ImageSource(img),
                .Sizing = ImageSizeMode.ZoomImage, .BoundsF = limites}
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Shared Function N(valor As Double) As String
        Return valor.ToString("#,##0.00")
    End Function

    ''' <summary>Días sin decimales si son enteros (5) y con dos si no (0.50).</summary>
    Private Shared Function Dias(valor As Double) As String
        Return If(valor = Math.Floor(valor), valor.ToString("0"), valor.ToString("0.00"))
    End Function

#End Region

End Class
