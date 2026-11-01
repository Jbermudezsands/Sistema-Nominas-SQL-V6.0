Imports DevExpress.Drawing
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

''' <summary>Una columna de <see cref="RptTabular"/>: título, campo de la tabla, ancho (centésimas de pulgada) y formato.</summary>
Public Class ColumnaReporte

    Public Property Titulo As String
    Public Property Campo As String
    Public Property Ancho As Single
    ''' <summary>Formato numérico o de fecha sin llaves (por ejemplo <c>#,##0.00</c>); vacío = texto.</summary>
    Public Property Formato As String = ""
    Public Property Derecha As Boolean
    ''' <summary>Suma la columna en el pie de cada grupo y en el total general.</summary>
    Public Property Sumar As Boolean

    Public Sub New(titulo As String, campo As String, ancho As Single, Optional formato As String = "", Optional sumar As Boolean = False)
        Me.Titulo = titulo
        Me.Campo = campo
        Me.Ancho = ancho
        Me.Formato = formato
        Me.Sumar = sumar
        Derecha = (formato <> "" AndAlso formato.Contains("0"))
    End Sub

End Class

''' <summary>
''' Listado horizontal sobre un <see cref="DataTable"/>: encabezado de empresa, títulos de columna en cada página, grupos
''' opcionales con subtotales y total general. Reemplaza las nóminas de ActiveReports de la V.5.0 (Nom13vo,
''' ArepNomVacaciones, ArepHistorialSalarial13vo), que eran el mismo diseño con otras columnas. La tabla debe venir
''' ordenada por el campo de grupo. Se muestra con <see cref="FrmVisorReporte"/>.
''' </summary>
Public Class RptTabular
    Inherits XtraReport

    Private Shared ReadOnly ColorEncabezado As Color = Color.FromArgb(219, 226, 242)
    Private Shared ReadOnly ColorFilaPar As Color = Color.FromArgb(236, 241, 250)

    ''' <param name="campoGrupo">Campo por el que se agrupa (por ejemplo "Departamento"); vacío = sin grupos.</param>
    Public Sub New(empresa As DatosEmpresa, titulo As String, periodo As String, tabla As DataTable,
                   columnas As IList(Of ColumnaReporte), Optional campoGrupo As String = "", Optional tituloGrupo As String = "")

        DisplayName = titulo
        PaperKind = DevExpress.Drawing.Printing.DXPaperKind.Letter
        Landscape = True
        Margins = New DevExpress.Drawing.DXMargins(40, 40, 40, 40)
        Font = New DXFont(ConstructorReporte.Fuente, 8)
        DataSource = tabla

        Dim ancho As Single = columnas.Sum(Function(c) c.Ancho)

        Dim encabezado As New ReportHeaderBand
        encabezado.HeightF = ConstructorReporte.Encabezado(encabezado, empresa, titulo, periodo, ancho)

        Dim titulos As New PageHeaderBand With {.HeightF = 24}
        Dim x As Single = 0
        For Each c As ColumnaReporte In columnas
            Dim lbl As XRLabel = ConstructorReporte.Etiqueta(c.Titulo, x, 2, c.Ancho, 20, True, 8, If(c.Derecha, TextAlignment.MiddleRight, TextAlignment.MiddleLeft))
            lbl.BackColor = ColorEncabezado
            lbl.Borders = BorderSide.Bottom
            titulos.Controls.Add(lbl)
            x += c.Ancho
        Next

        StyleSheet.Add(New XRControlStyle With {.Name = "FilaPar", .BackColor = ColorFilaPar})
        Dim detalle As New DetailBand With {.HeightF = 16, .EvenStyleName = "FilaPar"}
        x = 0
        For Each c As ColumnaReporte In columnas
            detalle.Controls.Add(ConstructorReporte.Campo(c.Campo, x, 0, c.Ancho, 16, FormatoEnlace(c),
                                                          False, 8, If(c.Derecha, TextAlignment.MiddleRight, TextAlignment.MiddleLeft)))
            x += c.Ancho
        Next

        Dim pie As New PageFooterBand With {.HeightF = 22}
        pie.Controls.Add(New XRPageInfo With {
            .PageInfo = PageInfo.NumberOfTotal, .TextFormatString = "Página {0} de {1}",
            .BoundsF = New RectangleF(ancho - 150, 4, 150, 14), .TextAlignment = TextAlignment.MiddleRight,
            .Font = New DXFont(ConstructorReporte.Fuente, 8)})

        Dim total As New ReportFooterBand With {.HeightF = 34}
        total.Controls.Add(New XRLine With {.BoundsF = New RectangleF(0, 2, ancho, 1)})
        AgregarSumas(total, columnas, SummaryRunning.Report, "TOTAL GENERAL", 6)

        Bands.Add(encabezado)
        Bands.Add(titulos)

        If Not String.IsNullOrEmpty(campoGrupo) Then
            Dim cabeza As New GroupHeaderBand With {.HeightF = 22, .RepeatEveryPage = False}
            cabeza.GroupFields.Add(New GroupField(campoGrupo))
            Dim lblGrupo As XRLabel = ConstructorReporte.Campo(campoGrupo, 70, 3, ancho - 70, 16, "", True, 9)
            cabeza.Controls.Add(ConstructorReporte.Etiqueta(tituloGrupo & ":", 0, 3, 70, 16, True, 9))
            cabeza.Controls.Add(lblGrupo)
            Dim piePie As New GroupFooterBand With {.HeightF = 30}
            piePie.Controls.Add(New XRLine With {.BoundsF = New RectangleF(0, 2, ancho, 1)})
            AgregarSumas(piePie, columnas, SummaryRunning.Group, "Subtotal", 4)
            Bands.Add(cabeza)
            Bands.Add(detalle)
            Bands.Add(piePie)
        Else
            Bands.Add(detalle)
        End If

        Bands.Add(total)
        Bands.Add(pie)

    End Sub

    ''' <summary>Los enlaces de datos llevan el formato entre llaves; sin formato, el texto tal cual.</summary>
    Private Shared Function FormatoEnlace(c As ColumnaReporte) As String
        Return If(String.IsNullOrEmpty(c.Formato), "", "{0:" & c.Formato & "}")
    End Function

    ''' <summary>Fila de sumas bajo las columnas marcadas con <see cref="ColumnaReporte.Sumar"/>; la etiqueta va en la primera columna.</summary>
    Private Shared Sub AgregarSumas(banda As Band, columnas As IList(Of ColumnaReporte), alcance As SummaryRunning,
                                    etiqueta As String, y As Single)

        Dim x As Single = 0

        For Each c As ColumnaReporte In columnas
            If c Is columnas(0) Then
                banda.Controls.Add(ConstructorReporte.Etiqueta(etiqueta, x, y, c.Ancho + 60, 16, True, 8))
            ElseIf c.Sumar Then
                Dim lbl As XRLabel = ConstructorReporte.Campo(c.Campo, x, y, c.Ancho, 16, "", True, 8, TextAlignment.MiddleRight)
                lbl.Summary = New XRSummary With {.Running = alcance, .FormatString = FormatoEnlace(c)}
                banda.Controls.Add(lbl)
            End If
            x += c.Ancho
        Next

    End Sub

End Class
