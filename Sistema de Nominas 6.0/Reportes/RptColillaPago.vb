Imports DevExpress.Drawing
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

''' <summary>Una línea de la colilla: texto y campo de la tabla que trae su valor.</summary>
Public Class ConceptoColilla

    Public Property Titulo As String
    Public Property Campo As String
    Public Property Formato As String

    Public Sub New(titulo As String, campo As String, Optional formato As String = "{0:#,##0.00}")
        Me.Titulo = titulo
        Me.Campo = campo
        Me.Formato = formato
    End Sub

End Class

''' <summary>
''' Colilla de pago (una por empleado) del 13vo mes y de vacaciones: reemplaza ArepColilla13vo y ArepColillaVaca de la V.5.0, que
''' eran el mismo diseño con otros conceptos. La tabla debe traer Codigo, Nombres, Cargo, Departamento, FechaIngreso, los campos de
''' los conceptos y TotalDevengado, TotalDeducir y MontoPagar. Con <c>porDepartamento</c> cada departamento empieza en página
''' nueva (la tabla debe venir ordenada por Departamento). Se muestra con <see cref="FrmVisorReporte"/>.
''' </summary>
Public Class RptColillaPago
    Inherits XtraReport

    Private Const Ancho As Single = 750
    Private Const AltoColilla As Single = 245

    Public Sub New(empresa As DatosEmpresa, titulo As String, periodo As String, tabla As DataTable,
                   detalle As IList(Of ConceptoColilla), deducciones As IList(Of ConceptoColilla), porDepartamento As Boolean)

        DisplayName = titulo
        PaperKind = DevExpress.Drawing.Printing.DXPaperKind.Letter
        Margins = New DevExpress.Drawing.DXMargins(40, 40, 40, 40)
        Font = New DXFont(ConstructorReporte.Fuente, 9)
        DataSource = tabla

        Dim cuerpo As New DetailBand With {.HeightF = AltoColilla, .KeepTogether = True}

        If porDepartamento Then
            Dim grupo As New GroupHeaderBand With {.HeightF = 0, .PageBreak = PageBreak.BeforeBand}
            grupo.GroupFields.Add(New GroupField("Departamento"))
            Bands.Add(grupo)
        End If
        Bands.Add(cuerpo)

        Dim y As Single = 4

        ' Empresa y título
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta(empresa.Nombre_Empresa, 0, y, Ancho, 18, True, 12, TextAlignment.MiddleCenter))
        y += 18
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta(titulo & If(periodo <> "", "  -  " & periodo, ""), 0, y, Ancho, 16, True, 10, TextAlignment.MiddleCenter))
        y += 22

        ' Empleado
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Código:", 0, y, 50, 16, True))
        cuerpo.Controls.Add(ConstructorReporte.Campo("Codigo", 50, y, 80, 16))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Nombre:", 140, y, 50, 16, True))
        cuerpo.Controls.Add(ConstructorReporte.Campo("Nombres", 190, y, 300, 16, "", True))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Ingreso:", 520, y, 50, 16, True))
        cuerpo.Controls.Add(ConstructorReporte.Campo("FechaIngreso", 570, y, 100, 16, "{0:dd/MM/yyyy}"))
        y += 16
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Cargo:", 0, y, 50, 16, True))
        cuerpo.Controls.Add(ConstructorReporte.Campo("Cargo", 50, y, 300, 16))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Depto.:", 370, y, 50, 16, True))
        cuerpo.Controls.Add(ConstructorReporte.Campo("Departamento", 420, y, 300, 16))
        y += 20
        cuerpo.Controls.Add(New XRLine With {.BoundsF = New RectangleF(0, y, Ancho, 2)})
        y += 6

        ' Dos columnas: conceptos y deducciones
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Conceptos", 0, y, 340, 16, True, 9))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Deducciones", 390, y, 340, 16, True, 9))
        y += 18

        Dim yDet As Single = y
        For Each c As ConceptoColilla In detalle
            cuerpo.Controls.Add(ConstructorReporte.Etiqueta(c.Titulo, 0, yDet, 220, 15))
            cuerpo.Controls.Add(ConstructorReporte.Campo(c.Campo, 220, yDet, 120, 15, c.Formato, False, 9, TextAlignment.MiddleRight))
            yDet += 15
        Next

        Dim yDed As Single = y
        For Each c As ConceptoColilla In deducciones
            cuerpo.Controls.Add(ConstructorReporte.Etiqueta(c.Titulo, 390, yDed, 220, 15))
            cuerpo.Controls.Add(ConstructorReporte.Campo(c.Campo, 610, yDed, 120, 15, c.Formato, False, 9, TextAlignment.MiddleRight))
            yDed += 15
        Next

        y = Math.Max(yDet, yDed) + 4
        cuerpo.Controls.Add(New XRLine With {.BoundsF = New RectangleF(0, y, Ancho, 1)})
        y += 4

        ' Totales
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Total devengado:", 0, y, 220, 16, True))
        cuerpo.Controls.Add(ConstructorReporte.Campo("TotalDevengado", 220, y, 120, 16, "{0:#,##0.00}", True, 9, TextAlignment.MiddleRight))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Total deducciones:", 390, y, 220, 16, True))
        cuerpo.Controls.Add(ConstructorReporte.Campo("TotalDeducir", 610, y, 120, 16, "{0:#,##0.00}", True, 9, TextAlignment.MiddleRight))
        y += 18
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("TOTAL A PAGAR:", 390, y, 220, 18, True, 10))
        cuerpo.Controls.Add(ConstructorReporte.Campo("MontoPagar", 610, y, 120, 18, "{0:#,##0.00}", True, 10, TextAlignment.MiddleRight))
        y += 34

        ' Firma
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Recibí conforme:", 0, y, 100, 16, True))
        cuerpo.Controls.Add(New XRLine With {.BoundsF = New RectangleF(100, y + 14, 260, 1)})

        ' Separador entre colillas
        cuerpo.Controls.Add(New XRLine With {.BoundsF = New RectangleF(0, AltoColilla - 6, Ancho, 1), .LineStyle = DevExpress.Drawing.DXDashStyle.Dash})

    End Sub

End Class
