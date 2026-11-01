Imports DevExpress.Drawing
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

''' <summary>
''' Denominaciones de billetes y monedas que hay que retirar para pagar una nómina (V.5.0 ARDenominaciones de
''' ActiveReports 2.0), armado por código. Se muestra con <see cref="FrmVisorReporte"/>.
''' </summary>
Public Class RptDenominaciones
    Inherits XtraReport

    Private Const Ancho As Single = 520

    ''' <param name="subtitulo">Nómina a la que pertenece (por ejemplo "Aguinaldo No. 3").</param>
    ''' <param name="cantidades">Cantidad por denominación, en el orden de <see cref="CalculadoraDenominaciones.Valores"/>.</param>
    Public Sub New(empresa As DatosEmpresa, subtitulo As String, cantidades As Integer())

        DisplayName = "Denominaciones - " & subtitulo
        PaperKind = DevExpress.Drawing.Printing.DXPaperKind.Letter
        Margins = New DevExpress.Drawing.DXMargins(60, 60, 40, 40)
        Font = New DXFont(ConstructorReporte.Fuente, 10)

        Dim encabezado As New ReportHeaderBand
        Dim detalle As New DetailBand
        Bands.AddRange(New Band() {encabezado, detalle})

        encabezado.HeightF = ConstructorReporte.Encabezado(encabezado, empresa, "Denominaciones de Billetes", subtitulo, Ancho)

        Dim y As Single = 8
        detalle.Controls.Add(ConstructorReporte.Etiqueta("Denominación", 0, y, 170, 20, True, 10))
        detalle.Controls.Add(ConstructorReporte.Etiqueta("Cantidad", 170, y, 150, 20, True, 10, TextAlignment.MiddleRight))
        detalle.Controls.Add(ConstructorReporte.Etiqueta("Total", 320, y, 200, 20, True, 10, TextAlignment.MiddleRight))
        y += 22
        detalle.Controls.Add(New XRLine With {.BoundsF = New RectangleF(0, y, Ancho, 2)})
        y += 6

        Dim granTotal As Double = 0
        For i As Integer = 0 To CalculadoraDenominaciones.Valores.Length - 1
            Dim importe As Double = CalculadoraDenominaciones.Importe(i, cantidades(i))
            granTotal += importe
            detalle.Controls.Add(ConstructorReporte.Etiqueta(CalculadoraDenominaciones.Etiqueta(i), 0, y, 170, 18, False, 10))
            detalle.Controls.Add(ConstructorReporte.Etiqueta(cantidades(i).ToString("#,##0"), 170, y, 150, 18, False, 10, TextAlignment.MiddleRight))
            detalle.Controls.Add(ConstructorReporte.Etiqueta(ConstructorReporte.N(importe), 320, y, 200, 18, False, 10, TextAlignment.MiddleRight))
            y += 20
            ' Una línea separa los billetes de las monedas de centavo.
            If CalculadoraDenominaciones.Valores(i) = 1D Then
                detalle.Controls.Add(New XRLine With {.BoundsF = New RectangleF(0, y + 1, Ancho, 1)})
                y += 6
            End If
        Next

        y += 4
        detalle.Controls.Add(New XRLine With {.BoundsF = New RectangleF(0, y, Ancho, 2)})
        y += 6
        detalle.Controls.Add(ConstructorReporte.Etiqueta("Total:", 170, y, 150, 22, True, 11, TextAlignment.MiddleRight))
        detalle.Controls.Add(ConstructorReporte.Etiqueta(ConstructorReporte.N(granTotal), 320, y, 200, 22, True, 11, TextAlignment.MiddleRight))

        y += 70
        detalle.Controls.Add(New XRLine With {.BoundsF = New RectangleF(110, y, 300, 2)})
        detalle.Controls.Add(ConstructorReporte.Etiqueta("Firma Responsable de Nóminas", 110, y + 4, 300, 16, True, 9, TextAlignment.MiddleCenter))

        detalle.HeightF = y + 30

    End Sub

End Class
