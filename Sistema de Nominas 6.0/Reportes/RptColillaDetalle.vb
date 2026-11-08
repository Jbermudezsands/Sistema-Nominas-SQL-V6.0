Imports DevExpress.Drawing
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

''' <summary>
''' Sub-reporte con una fila por deducción del empleado (V.5.0 SubColillaDeduccion): crece con la cantidad de filas. Se llena con la
''' vista ya filtrada al empleado (columnas Deduccion y Valor).
''' </summary>
Public Class RptSubDeducciones
    Inherits XtraReport

    Public Const Ancho As Single = 240

    Public Sub New(filas As DataView)

        PaperKind = DevExpress.Drawing.Printing.DXPaperKind.Custom
        PageWidth = CInt(Ancho)
        PageHeight = 1100
        Margins = New DevExpress.Drawing.DXMargins(0, 0, 0, 0)
        Font = New DXFont(ConstructorReporte.Fuente, 8)
        DataSource = filas

        Dim cuerpo As New DetailBand With {.HeightF = 13}
        Bands.Add(cuerpo)
        cuerpo.Controls.Add(ConstructorReporte.Campo("Deduccion", 0, 0, 150, 13))
        cuerpo.Controls.Add(ConstructorReporte.Campo("Valor", 150, 0, 90, 13, "{0:#,##0.00}", False, 8, TextAlignment.MiddleRight))

    End Sub

End Class

''' <summary>
''' Colilla de pago "Trides" (V.5.0 ArepColillasPago4), con la disposición de la impresión de la V.5.0: tres colillas por hoja carta,
''' logo de la empresa, ingresos y deducciones con sus cantidades y, a la derecha, el cuadro "Detalle Deducciones" con un
''' sub-reporte (<see cref="RptSubDeducciones"/>) que crece con cada deducción y empuja hacia abajo el total. La tabla es la de
''' <see cref="CsPlanillaNominaDAL.ListarParaReporte"/> y <c>detalleDeducciones</c> la de
''' <see cref="CsPlanillaNominaDAL.ListarDetalleDeducciones"/> (Codigo, Deduccion, Valor).
''' </summary>
Public Class RptColillaDetalle
    Inherits XtraReport

    Private Const Ancho As Single = 750
    Private Const AltoFila As Single = 13
    Private Const FilasDetalle As Integer = 10

    Private ReadOnly _detalle As DataTable
    Private ReadOnly _empresa As DatosEmpresa

    Public Sub New(empresa As DatosEmpresa, titulo As String, periodo As String, tabla As DataTable, detalleDeducciones As DataTable, porDepartamento As Boolean)

        _empresa = empresa
        _detalle = detalleDeducciones
        PrepararColumnas(tabla)

        DisplayName = titulo
        PaperKind = DevExpress.Drawing.Printing.DXPaperKind.Letter
        Margins = New DevExpress.Drawing.DXMargins(40, 40, 40, 40)
        Font = New DXFont(ConstructorReporte.Fuente, 8)
        DataSource = tabla

        Dim cuerpo As New DetailBand With {.HeightF = 330, .KeepTogether = True}

        If porDepartamento Then
            Dim grupo As New GroupHeaderBand With {.HeightF = 0, .PageBreak = PageBreak.BeforeBand}
            grupo.GroupFields.Add(New GroupField("Departamento"))
            Bands.Add(grupo)
        End If
        Bands.Add(cuerpo)

        ' ---- Encabezado: línea gruesa, empresa, título, logo y período ----
        cuerpo.Controls.Add(New XRLine With {.BoundsF = New RectangleF(0, 0, Ancho, 4), .LineStyle = DevExpress.Drawing.DXDashStyle.Dash, .LineWidth = 3})
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta(empresa.Nombre_Empresa, 0, 6, Ancho, 16, True, 12, TextAlignment.MiddleCenter))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("COLILLA DE PAGO", 0, 22, Ancho, 14, True, 10, TextAlignment.MiddleCenter))

        Dim foto As New XRPictureBox With {.BoundsF = New RectangleF(10, 36, 90, 38), .Sizing = ImageSizeMode.ZoomImage}
        AddHandler foto.BeforePrint, AddressOf Logo_BeforePrint
        cuerpo.Controls.Add(foto)
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta(TextoPeriodo(periodo), 100, 38, 650, 14, False, 8, TextAlignment.MiddleLeft))

        ' ---- Empleado ----
        Dim y As Single = 54
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("NUMERO", 0, y, 55, 14, True, 8))
        cuerpo.Controls.Add(ConstructorReporte.Campo("Codigo", 55, y, 60, 14, "", False, 9))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("N O M B R E", 125, y, 75, 14, True, 8))
        cuerpo.Controls.Add(ConstructorReporte.Campo("Nombres", 200, y, 330, 14, "", False, 8))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("CARGO:", 540, y, 55, 14, True, 8))
        cuerpo.Controls.Add(ConstructorReporte.Campo("Cargo", 595, y, 155, 14, "", False, 8))
        y += 14
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Nomina #:", 0, y, 55, 14, True, 8))
        cuerpo.Controls.Add(ConstructorReporte.Campo("NumNomina", 55, y, 50, 14, "", False, 8))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Dpto:", 125, y, 35, 14, True, 8))
        cuerpo.Controls.Add(ConstructorReporte.Campo("Departamento", 160, y, 200, 14, "", False, 9))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Nomina:", 540, y, 55, 14, True, 9))
        cuerpo.Controls.Add(ConstructorReporte.Campo("TipoNomina", 595, y, 155, 14, "", False, 9))
        y += 15
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Periodo:", 0, y, 55, 14, True, 9))
        cuerpo.Controls.Add(ConstructorReporte.Campo("PeriodoNomina", 55, y, 120, 14, "", False, 9))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("No INSS", 300, y, 50, 14, True, 8))
        cuerpo.Controls.Add(ConstructorReporte.Campo("NumeroInss", 350, y, 120, 14, "", False, 8))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Cedula:", 540, y, 55, 14, True, 9))
        cuerpo.Controls.Add(ConstructorReporte.Campo("NumCedula", 595, y, 155, 14, "", False, 8))
        y += 18

        ' ---- Títulos de columnas ----
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("INGRESOS", 0, y, 240, 18, True, 11, TextAlignment.MiddleCenter))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("DEDUCCIONES", 265, y, 225, 18, True, 11, TextAlignment.MiddleCenter))
        Dim titDetalle As XRLabel = ConstructorReporte.Etiqueta("Detalle Deducciones", 510, y, 240, 16, True, 9, TextAlignment.MiddleCenter)
        titDetalle.Borders = BorderSide.All
        cuerpo.Controls.Add(titDetalle)
        y += 20

        ' ---- Ingresos: título, cantidad y monto ----
        Dim yIng As Single = y
        yIng = Fila(cuerpo, 0, yIng, "Salario Basico:", "SalarioBasico", "HTrabajada", "{0:#,##0.00}")
        yIng = Fila(cuerpo, 0, yIng, "", "TotalSubsidio", "DiasSubsidio", "{0:#,##0.00}", "EtiquetaSubsidio")
        yIng = Fila(cuerpo, 0, yIng, "Antiguedad", "Antiguedad")
        yIng = Fila(cuerpo, 0, yIng, "Horas Extras:", "HorasExtras", "HE", "{0:#,##0.00}")
        yIng = Fila(cuerpo, 0, yIng, "Monto Destajo:", "Destajo")
        yIng = Fila(cuerpo, 0, yIng, "Sub.Alimentacion", "Viaticos")
        yIng = Fila(cuerpo, 0, yIng, "Incentivos", "Incentivos")
        yIng = Fila(cuerpo, 0, yIng, "Vacaciones", "VacacionesPagadas", "DiasVacaciones", "{0:#,##0.00}")
        yIng = Fila(cuerpo, 0, yIng, "Comision:", "Comisiones")
        yIng = Fila(cuerpo, 0, yIng, "Otros Ingresos:", "OtrosIngresos")

        ' ---- Deducciones ----
        Dim yDed As Single = y
        yDed = Fila(cuerpo, 265, yDed, "Monto INSS:", "MontoINSS")
        yDed = Fila(cuerpo, 265, yDed, "Monto IR:", "MontoIR")
        yDed = Fila(cuerpo, 265, yDed, "Prestamos:", "Prestamo")
        yDed = Fila(cuerpo, 265, yDed, "Dias Ausente", "DescuentoFalta", "DiasAusente", "{0:#,##0.00}")
        yDed = Fila(cuerpo, 265, yDed, "Pens Alimentos", "MontoPension")
        yDed = Fila(cuerpo, 265, yDed, "Embargo Judic", "EmbargoJudicial")
        yDed = Fila(cuerpo, 265, yDed, "Deducciones:", "Deducciones")
        yDed = Fila(cuerpo, 265, yDed, "Ajuste IR", "Ajuste", "", "", "")

        ' ---- Totales ----
        Dim yTot As Single = y + FilasDetalle * AltoFila + 4
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Total Devengado:", 5, yTot, 110, 16, True, 9))
        cuerpo.Controls.Add(ConstructorReporte.Campo("TotalDevengado", 115, yTot, 125, 16, "{0:#,##0.00}", True, 9, TextAlignment.MiddleRight))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Total Deducciones:", 265, yTot - 6, 120, 14, True, 8))
        cuerpo.Controls.Add(ConstructorReporte.Campo("TotalDeducir", 385, yTot - 6, 105, 14, "{0:#,##0.00}", True, 8, TextAlignment.MiddleRight))
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Total a Pagar", 255, yTot + 10, 120, 18, True, 10))
        cuerpo.Controls.Add(ConstructorReporte.Campo("MontoPagar", 375, yTot + 10, 115, 18, "{0:#,##0.00}", True, 10, TextAlignment.MiddleRight))

        ' ---- Detalle de deducciones: sub-reporte que crece y total debajo ----
        Dim ySub As Single = y
        Dim sub1 As New XRSubreport With {.BoundsF = New RectangleF(510, ySub, RptSubDeducciones.Ancho, FilasDetalle * AltoFila - 10), .CanGrow = True}
        AddHandler sub1.BeforePrint, AddressOf Subreporte_BeforePrint
        cuerpo.Controls.Add(sub1)

        Dim yTotDet As Single = ySub + FilasDetalle * AltoFila - 6
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Total Detalle", 520, yTotDet, 100, 14, True, 8))
        cuerpo.Controls.Add(ConstructorReporte.Campo("Deducciones", 650, yTotDet, 100, 14, "{0:#,##0.00}", True, 8, TextAlignment.MiddleRight))

        ' ---- Firma ----
        Dim yFirma As Single = yTot + 30
        cuerpo.Controls.Add(New XRLine With {.BoundsF = New RectangleF(510, yFirma, 240, 1)})
        cuerpo.Controls.Add(ConstructorReporte.Etiqueta("Recibi Conforme", 510, yFirma + 1, 240, 14, True, 9, TextAlignment.MiddleCenter))

        cuerpo.HeightF = yFirma + 24

    End Sub

    ''' <summary>
    ''' Una línea "título … cantidad … monto" (título y monto en la columna que empieza en <paramref name="x"/>); devuelve la siguiente Y.
    ''' Con <paramref name="campoTitulo"/> el título sale de un campo de la tabla.
    ''' </summary>
    Private Shared Function Fila(banda As DetailBand, x As Single, y As Single, titulo As String, campoMonto As String,
                                 Optional campoCantidad As String = "", Optional formatoCantidad As String = "", Optional campoTitulo As String = "") As Single

        If campoTitulo <> "" Then
            banda.Controls.Add(ConstructorReporte.Campo(campoTitulo, x, y, 95, AltoFila, "", False, 8))
        Else
            banda.Controls.Add(ConstructorReporte.Etiqueta(titulo, x, y, 95, AltoFila, False, 8))
        End If
        If campoCantidad <> "" Then
            banda.Controls.Add(ConstructorReporte.Campo(campoCantidad, x + 95, y, 55, AltoFila, formatoCantidad, False, 8, TextAlignment.MiddleLeft))
        End If
        banda.Controls.Add(ConstructorReporte.Campo(campoMonto, x + 150, y, 90, AltoFila, "{0:#,##0.00}", False, 8, TextAlignment.MiddleRight))
        Return y + AltoFila

    End Function

    ''' <summary>"Colilla de Pago # 33, Correspondiente del martes, 16 de diciembre de 2025 al miércoles, 31 de diciembre de 2025" (V.5.0).</summary>
    Public Shared Function TextoPeriodo(periodo As String) As String

        Dim m As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(
            If(periodo, ""), "No\.\s*(\d+).*?(\d{2}/\d{2}/\d{4}).*?(\d{2}/\d{2}/\d{4})")
        If Not m.Success Then Return periodo

        Dim inicio, fin As Date
        If Not Date.TryParseExact(m.Groups(2).Value, "dd/MM/yyyy", Nothing, Globalization.DateTimeStyles.None, inicio) OrElse
           Not Date.TryParseExact(m.Groups(3).Value, "dd/MM/yyyy", Nothing, Globalization.DateTimeStyles.None, fin) Then Return periodo

        Dim cultura As New Globalization.CultureInfo("es-NI")
        Return $"Colilla de Pago # {m.Groups(1).Value}, Correspondiente del {inicio.ToString("dddd, d 'de' MMMM 'de' yyyy", cultura)} al {fin.ToString("dddd, d 'de' MMMM 'de' yyyy", cultura)}"

    End Function

    ''' <summary>Agrega a la tabla los campos derivados que usa el diseño (etiqueta del subsidio).</summary>
    Private Shared Sub PrepararColumnas(tabla As DataTable)

        If Not tabla.Columns.Contains("EtiquetaSubsidio") Then tabla.Columns.Add("EtiquetaSubsidio", GetType(String))

        For Each f As DataRow In tabla.Rows
            f("EtiquetaSubsidio") = EtiquetaSubsidio(If(IsDBNull(f("Clasificacion_Subsidio")), "", Convert.ToString(f("Clasificacion_Subsidio"))))
        Next

    End Sub

    ''' <summary>Rótulo del subsidio según la clasificación (V.5.0 Detail_Format de ArepColillasPago4).</summary>
    Public Shared Function EtiquetaSubsidio(clasificacion As String) As String

        Select Case clasificacion
            Case "" : Return "Sub.Comun"
            Case "Embarazo" : Return "Embarazo"
            Case "Riesgo Laboral" : Return "Sub.Laboral"
            Case "Accidente Comun" : Return "Sub.Comun"
            Case Else : Return clasificacion
        End Select

    End Function

    ''' <summary>Logo de la empresa (como la V.5.0, que lo ponía sobre la foto); si no hay logo, la foto del empleado (RutaFoto\código.jpg|gif|bmp).</summary>
    Private Sub Logo_BeforePrint(sender As Object, e As System.ComponentModel.CancelEventArgs)

        Dim caja As XRPictureBox = DirectCast(sender, XRPictureBox)
        Dim origen As XRPictureBox = ConstructorReporte.Imagen(_empresa.Logo, caja.BoundsF)

        If origen Is Nothing AndAlso _empresa.Ruta_Foto <> "" Then
            Dim codigo As String = Convert.ToString(GetCurrentColumnValue("Codigo"))
            For Each ext As String In {".jpg", ".gif", ".bmp"}
                origen = ConstructorReporte.Imagen(IO.Path.Combine(_empresa.Ruta_Foto, codigo & ext), caja.BoundsF)
                If origen IsNot Nothing Then Exit For
            Next
        End If

        caja.ImageSource = If(origen Is Nothing, Nothing, origen.ImageSource)

    End Sub

    ''' <summary>Arma el sub-reporte con las deducciones del empleado que se está imprimiendo.</summary>
    Private Sub Subreporte_BeforePrint(sender As Object, e As System.ComponentModel.CancelEventArgs)

        Dim vista As New DataView(_detalle)
        vista.RowFilter = "Codigo = '" & Convert.ToString(GetCurrentColumnValue("Codigo")).Replace("'", "''") & "'"
        DirectCast(sender, XRSubreport).ReportSource = New RptSubDeducciones(vista)

    End Sub

End Class
