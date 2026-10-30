Imports DevExpress.Drawing
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

''' <summary>
''' Solicitud de vacaciones en formato corto (V.5.0 ArepSolicitudVacaCorto de ActiveReports 2.0, el que imprimía
''' FrmSolicitud para 3 días o menos), armada por código sin diseñador: encabezado, datos del empleado, de la solicitud
''' y del período vacacional, y firmas. Se muestra con <see cref="FrmVisorReporte"/>.
''' </summary>
Public Class RptSolicitudVacaCorto
    Inherits XtraReport

    Private Const Ancho As Single = 750
    Private Const Fuente As String = "Arial"
    Private Shared ReadOnly ColorSeccion As Color = Color.FromArgb(219, 226, 242)

    Private ReadOnly _detalle As New DetailBand
    Private _y As Single

    ''' <param name="diasVacaciones">Días de vacaciones acumulados (pantalla: Días vacaciones).</param>
    ''' <param name="diasDisfrutados">Días disfrutados incluida esta solicitud.</param>
    ''' <param name="diasDisponibles">Días disponibles después de esta solicitud.</param>
    Public Sub New(empresa As DatosEmpresa, sol As SolicitudVacaciones, emp As EmpleadoSolicitud,
                   diasVacaciones As Double, diasDisfrutados As Double, diasDisponibles As Double,
                   Optional fechaImpresion As Date? = Nothing)

        DisplayName = "Solicitud de vacaciones " & sol.Numero_Solicitud
        PaperKind = DevExpress.Drawing.Printing.DXPaperKind.Letter
        Margins = New DevExpress.Drawing.DXMargins(50, 50, 40, 40)
        Font = New DXFont(Fuente, 10)

        Dim encabezado As New ReportHeaderBand
        Dim pie As New PageFooterBand With {.HeightF = 25}
        Bands.AddRange(New Band() {encabezado, _detalle, pie})

        CrearEncabezado(encabezado, empresa, sol, If(fechaImpresion, Date.Now))

        Seccion("DATOS DEL EMPLEADO")
        Campos({"Nombre:", emp.Nombre_Completo, "Código:", emp.Codigo_Empleado1},
               {"Cargo:", emp.Cargo, "Departamento:", emp.Departamento})

        Seccion("DATOS DE LA SOLICITUD")
        Campos({"Fecha inicio:", sol.Fecha_Inicio.ToString("dd/MM/yyyy"), "Fecha fin:", sol.Fecha_Fin.ToString("dd/MM/yyyy")},
               {"Días a disfrutar:", N(sol.Dias_Solicitud), "Horas a disfrutar:", N(sol.Horas_Solicitud)})
        Observaciones(sol.Observaciones)

        Seccion("DATOS DEL PERÍODO VACACIONAL")
        Campos({"Días vacaciones:", N(diasVacaciones), "Días disfrutados:", N(diasDisfrutados)},
               {"Días disponibles:", N(diasDisponibles), "", ""})

        CrearFirmas()

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
        banda.HeightF = 100

    End Sub

    ''' <summary>Filas de dos pares etiqueta / valor.</summary>
    Private Sub Campos(ParamArray filas As String()())
        For Each f As String() In filas
            _detalle.Controls.Add(Etiqueta(f(0), 10, _y, 120, 18, True, 10))
            _detalle.Controls.Add(Valor(f(1), 130, _y, 270))
            _detalle.Controls.Add(Etiqueta(f(2), 405, _y, 125, 18, True, 10))
            _detalle.Controls.Add(Valor(f(3), 530, _y, 220))
            ' Un valor largo baja a una segunda línea.
            _y += If(f(1).Length > 34 OrElse f(3).Length > 28, 38, 22)
        Next
        _y += 10
    End Sub

    Private Sub Observaciones(texto As String)
        _detalle.Controls.Add(Etiqueta("Observaciones:", 10, _y, 120, 18, True, 10))
        Dim lbl As XRLabel = Etiqueta(texto, 130, _y, Ancho - 130, 40, False, 10, TextAlignment.TopLeft)
        lbl.Multiline = True
        lbl.WordWrap = True
        lbl.CanGrow = True
        lbl.Borders = BorderSide.Bottom
        _detalle.Controls.Add(lbl)
        _y += 52
    End Sub

    Private Sub CrearFirmas()

        _y += 40
        Dim titulos As String() = {"Firma del Empleado", "Autorizado por"}
        Dim col As Single = Ancho / titulos.Length

        For i As Integer = 0 To titulos.Length - 1
            Dim x As Single = i * col
            _detalle.Controls.Add(New XRLine With {.BoundsF = New RectangleF(x + 40, _y + 30, col - 80, 2)})
            _detalle.Controls.Add(Etiqueta(titulos(i), x, _y + 34, col, 16, True, 9, TextAlignment.MiddleCenter))
        Next

        _y += 60

    End Sub

#End Region

#Region "AYUDAS"

    Private Sub Seccion(titulo As String)
        Dim lbl As XRLabel = Etiqueta(titulo, 0, _y, Ancho, 18, True, 9)
        lbl.BackColor = ColorSeccion
        lbl.Padding = New PaddingInfo(4, 4, 0, 0)
        _detalle.Controls.Add(lbl)
        _y += 26
    End Sub

    Private Shared Function Valor(texto As String, x As Single, y As Single, ancho As Single) As XRLabel
        Dim lbl As XRLabel = Etiqueta(texto, x, y, ancho, 18, False, 10, TextAlignment.TopLeft)
        lbl.Multiline = True
        lbl.WordWrap = True
        lbl.CanGrow = True
        Return lbl
    End Function

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

#End Region

End Class
