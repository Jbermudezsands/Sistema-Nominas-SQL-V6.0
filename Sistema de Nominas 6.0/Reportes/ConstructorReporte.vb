Imports DevExpress.Drawing
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

''' <summary>
''' Piezas comunes de los reportes DevExpress armados por código (etiquetas, logo y encabezado de empresa), para que los
''' reportes del 13vo y de vacaciones no repitan cada uno sus propios ayudantes.
''' </summary>
Friend NotInheritable Class ConstructorReporte

    Public Const Fuente As String = "Arial"

    Private Sub New()
    End Sub

    Public Shared Function Etiqueta(texto As String, x As Single, y As Single, ancho As Single, alto As Single,
                                    Optional negrita As Boolean = False, Optional tamano As Single = 8,
                                    Optional alineacion As TextAlignment = TextAlignment.MiddleLeft) As XRLabel
        Return New XRLabel With {
            .Text = If(texto, ""), .BoundsF = New RectangleF(x, y, ancho, alto), .TextAlignment = alineacion,
            .Font = New DXFont(Fuente, tamano, If(negrita, DXFontStyle.Bold, DXFontStyle.Regular)),
            .Padding = New PaddingInfo(2, 2, 0, 0), .CanGrow = False}
    End Function

    ''' <summary>Etiqueta que muestra el campo <paramref name="campo"/> de la fila actual.</summary>
    Public Shared Function Campo(nombre As String, x As Single, y As Single, ancho As Single, alto As Single,
                                 Optional formato As String = "", Optional negrita As Boolean = False, Optional tamano As Single = 8,
                                 Optional alineacion As TextAlignment = TextAlignment.MiddleLeft) As XRLabel
        Dim lbl As XRLabel = Etiqueta("", x, y, ancho, alto, negrita, tamano, alineacion)
        lbl.DataBindings.Add("Text", Nothing, nombre, formato)
        Return lbl
    End Function

    ''' <summary>Imagen desde archivo; Nothing si la ruta está vacía, no existe o no es una imagen.</summary>
    Public Shared Function Imagen(ruta As String, limites As RectangleF) As XRPictureBox
        Try
            If String.IsNullOrWhiteSpace(ruta) OrElse Not IO.File.Exists(ruta) Then Return Nothing
            ' Se copia a memoria para no dejar el archivo bloqueado.
            Dim img As Image
            Using ms As New IO.MemoryStream(IO.File.ReadAllBytes(ruta)), original As Image = Image.FromStream(ms)
                img = New Bitmap(original)
            End Using
            Return New XRPictureBox With {
                .ImageSource = New DevExpress.XtraPrinting.Drawing.ImageSource(img),
                .Sizing = DevExpress.XtraPrinting.ImageSizeMode.ZoomImage, .BoundsF = limites}
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Encabezado de la V.5.0 (logo, empresa, dirección, título del reporte, período e impresión) dentro de <paramref name="banda"/>.
    ''' Devuelve la altura que ocupa.
    ''' </summary>
    Public Shared Function Encabezado(banda As Band, empresa As DatosEmpresa, titulo As String, periodo As String, ancho As Single) As Single

        Dim logo As XRPictureBox = Imagen(empresa.Ruta_Logo, New RectangleF(0, 0, 100, 60))
        If logo IsNot Nothing Then banda.Controls.Add(logo)

        banda.Controls.Add(Etiqueta(empresa.Nombre_Empresa, 110, 0, ancho - 260, 20, True, 13, TextAlignment.MiddleCenter))
        banda.Controls.Add(Etiqueta(empresa.Direccion, 110, 20, ancho - 260, 14, False, 8, TextAlignment.MiddleCenter))
        banda.Controls.Add(Etiqueta(titulo, 110, 36, ancho - 260, 22, True, 14, TextAlignment.MiddleCenter))
        banda.Controls.Add(Etiqueta(periodo, 110, 58, ancho - 260, 16, False, 9, TextAlignment.MiddleCenter))
        banda.Controls.Add(Etiqueta("Impreso: " & Date.Now.ToString("dd/MM/yyyy"), ancho - 140, 0, 140, 14, False, 8, TextAlignment.MiddleRight))
        banda.Controls.Add(New XRLine With {.BoundsF = New RectangleF(0, 78, ancho, 2)})

        Return 84

    End Function

    Public Shared Function N(valor As Double) As String
        Return valor.ToString("#,##0.00")
    End Function

End Class
