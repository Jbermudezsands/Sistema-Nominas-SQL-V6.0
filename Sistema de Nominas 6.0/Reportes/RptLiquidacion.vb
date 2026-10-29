Imports DevExpress.Drawing
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

''' <summary>
''' Liquidación final de un empleado (V.5.0 ARBaja de ActiveReports), armada por código sin diseñador:
''' datos generales, historial de salarios, ingresos, egresos, total a pagar, constancia y firmas.
''' Se muestra con <see cref="FrmVisorReporte"/>, que permite imprimir y exportar a PDF o Excel.
''' </summary>
Public Class RptLiquidacion
    Inherits XtraReport

    Private Const Ancho As Single = 750
    Private Const Fuente As String = "Arial"
    Private Shared ReadOnly ColorSeccion As Color = Color.FromArgb(219, 226, 242)

    Private ReadOnly _detalle As New DetailBand
    Private _y As Single

    Public Sub New(empresa As CsBajasDAL.DatosEmpresaBaja, emp As EmpleadoBaja, liq As Liquidacion, salarios As IList(Of SalarioMesBaja),
                   mesesTexto As String, diasPlanillaTexto As String, rutaFoto As String)

        DisplayName = "Liquidación " & emp.Cod_Empleado1
        PaperKind = DevExpress.Drawing.Printing.DXPaperKind.Letter
        Margins = New DevExpress.Drawing.DXMargins(50, 50, 40, 40)
        Font = New DXFont(Fuente, 8)

        Dim encabezado As New ReportHeaderBand
        Dim pie As New PageFooterBand With {.HeightF = 25}
        Bands.AddRange(New Band() {encabezado, _detalle, pie})

        CrearEncabezado(encabezado, empresa, rutaFoto)
        CrearDatosGenerales(emp, liq, mesesTexto)
        CrearHistorial(salarios)
        CrearIngresos(liq, diasPlanillaTexto)
        CrearEgresos(liq)
        CrearFirmas(empresa, emp)

        _detalle.HeightF = _y + 5
        pie.Controls.Add(New XRPageInfo With {
            .PageInfo = PageInfo.NumberOfTotal, .TextFormatString = "Página {0} de {1}",
            .BoundsF = New RectangleF(Ancho - 150, 5, 150, 15), .TextAlignment = TextAlignment.MiddleRight})
        pie.Controls.Add(New XRPageInfo With {
            .PageInfo = PageInfo.DateTime, .TextFormatString = "{0:dd/MM/yyyy HH:mm}",
            .BoundsF = New RectangleF(0, 5, 200, 15)})

    End Sub

#Region "SECCIONES"

    Private Sub CrearEncabezado(banda As ReportHeaderBand, empresa As CsBajasDAL.DatosEmpresaBaja, rutaFoto As String)

        Dim logo As XRPictureBox = Imagen(empresa.Ruta_Logo, New RectangleF(0, 0, 110, 70))
        If logo IsNot Nothing Then banda.Controls.Add(logo)
        Dim foto As XRPictureBox = Imagen(rutaFoto, New RectangleF(Ancho - 80, 0, 80, 90))
        If foto IsNot Nothing Then banda.Controls.Add(foto)

        banda.Controls.Add(Etiqueta(empresa.Nombre_Empresa, 120, 0, Ancho - 240, 22, True, 13, TextAlignment.MiddleCenter))
        banda.Controls.Add(Etiqueta(empresa.Direccion, 120, 22, Ancho - 240, 16, False, 8, TextAlignment.MiddleCenter))
        banda.Controls.Add(Etiqueta("LIQUIDACIÓN FINAL", 120, 46, Ancho - 240, 24, True, 14, TextAlignment.MiddleCenter))
        banda.HeightF = 95

    End Sub

    Private Sub CrearDatosGenerales(emp As EmpleadoBaja, liq As Liquidacion, mesesTexto As String)

        Seccion("DATOS GENERALES")

        Dim filas As String()() = {
            New String() {"Código del empleado:", emp.Cod_Empleado1, "Nombre del empleado:", emp.Nombre_Completo},
            New String() {"Cargo:", emp.Cargo, "Departamento:", emp.Departamento},
            New String() {"Fecha de inicio:", FechaTexto(emp.Fecha_Contrato), "Fecha de baja:", liq.Fecha_Baja.ToString("dd/MM/yyyy")},
            New String() {"Años trabajados:", liq.Annos_Trabajados.ToString("n2"), "Meses trabajados:", mesesTexto},
            New String() {"Salario promedio:", liq.Salario_Mensual.ToString("n2"), "Salario más alto:", liq.Salario_Alto.ToString("n2")},
            New String() {"Salario mínimo:", liq.Salario_Basico.ToString("n2"), "Salario por día:", liq.Salario_Por_Dia.ToString("n2")},
            New String() {"Número de cédula:", emp.Num_Cedula, "Número INSS:", emp.Numero_Inss},
            New String() {"Tipo de baja:", liq.Tipo_Baja, "Motivo:", liq.Motivo_Baja}}

        For Each f As String() In filas
            _detalle.Controls.Add(Etiqueta(f(0), 0, _y, 125, 15, True))
            _detalle.Controls.Add(Etiqueta(f(1), 125, _y, 245, 15))
            _detalle.Controls.Add(Etiqueta(f(2), 375, _y, 135, 15, True))
            _detalle.Controls.Add(Etiqueta(f(3), 510, _y, 240, 15))
            _y += 15
        Next

        If liq.Referencia <> "" Then
            _detalle.Controls.Add(Etiqueta("Base IR: " & liq.Referencia, 0, _y, Ancho, 14, False, 7))
            _y += 14
        End If

        _y += 6

    End Sub

    Private Sub CrearHistorial(salarios As IList(Of SalarioMesBaja))

        Seccion("HISTORIAL")

        Dim anchos As Single() = {80, 100, 80, 95, 85, 95, 95, 120}
        Dim tabla As New XRTable With {.BoundsF = New RectangleF(0, _y, Ancho, 0), .Borders = BorderSide.All}
        tabla.BeginInit()

        tabla.Rows.Add(Fila(anchos, True, ColorSeccion, "Mes", "Salario Básico", "Séptimo", "Producción", "Reembolso", "Incentivos", "Otros", "Total Ingresos"))

        For Each s As SalarioMesBaja In salarios
            tabla.Rows.Add(Fila(anchos, False, Color.Empty, s.Periodo, N(s.Salario_Basico), N(s.Septimo), N(s.Destajo + s.Bono_Produccion),
                                N(s.Reembolso), N(s.Incentivos), N(s.Otros), N(s.Total_Ingresos)))
        Next

        tabla.Rows.Add(Fila(anchos, True, Color.Empty, "Total", N(salarios.Sum(Function(s) s.Salario_Basico)), N(salarios.Sum(Function(s) s.Septimo)),
                            N(salarios.Sum(Function(s) s.Destajo + s.Bono_Produccion)), N(salarios.Sum(Function(s) s.Reembolso)),
                            N(salarios.Sum(Function(s) s.Incentivos)), N(salarios.Sum(Function(s) s.Otros)), N(salarios.Sum(Function(s) s.Total_Ingresos))))

        tabla.HeightF = tabla.Rows.Count * 16
        tabla.EndInit()
        _detalle.Controls.Add(tabla)
        _y += tabla.HeightF + 8

    End Sub

    Private Sub CrearIngresos(liq As Liquidacion, diasPlanillaTexto As String)

        Seccion("INGRESOS")

        Dim filas As New List(Of String()) From {
            New String() {"Día(s) laborado(s)", diasPlanillaTexto, N(liq.Monto_Nom_Propor)},
            New String() {"Vacaciones", liq.Dias_Vacaciones_Acumulados.ToString("n2") & " días de vacaciones, menos " & liq.Dias_Menos_Vacaciones.ToString("n2") &
                          " días, para un total de: " & liq.Dias_Vacaciones.ToString("n2"), N(liq.Monto_Vacaciones)},
            New String() {"Aguinaldo", liq.Dias_Aguinaldo.ToString("n2") & " días de aguinaldo acumulados", N(liq.Monto_Aguinaldo)},
            New String() {"Indemnización por años de servicio (Arto. 43 y 45)", liq.Dias_Antiguedad.ToString("n2") & " días", N(liq.Monto_Antiguedad)},
            New String() {"Horas extra", liq.Horas_Extra.ToString("n2") & " horas", N(liq.Monto_Horas_Extra)},
            New String() {"Otros ingresos", liq.Otro, N(liq.Monto_Otro)}}

        Conceptos(filas, "TOTAL INGRESOS:", liq.Total_Ingresos)

    End Sub

    Private Sub CrearEgresos(liq As Liquidacion)

        Seccion("EGRESOS")

        Dim filas As New List(Of String()) From {
            New String() {"INSS", "", N(liq.Monto_Inss)},
            New String() {"IR", "", N(liq.Monto_Ir)},
            New String() {"Préstamos y/o adelantos", "", N(liq.Prestamo)},
            New String() {"Deducciones", "", N(liq.Deducciones)},
            New String() {"Otras deducciones", liq.Concepto_Otras_Deducciones, N(liq.Monto_Otras_Deducciones)}}

        Conceptos(filas, "TOTAL EGRESOS:", liq.Total_Egresos)

        Dim total As XRLabel = Etiqueta("TOTAL A PAGAR:   C$ " & N(liq.Neto_Pagar), Ancho - 320, _y, 320, 24, True, 12, TextAlignment.MiddleRight)
        total.Borders = BorderSide.All
        total.BackColor = ColorSeccion
        total.Padding = New PaddingInfo(4, 6, 0, 0)
        _detalle.Controls.Add(total)
        _y += 34

    End Sub

    Private Sub CrearFirmas(empresa As CsBajasDAL.DatosEmpresaBaja, emp As EmpleadoBaja)

        Dim nota As String = "Yo: " & emp.Nombre_Completo & " hago constar que recibo a mi entera satisfacción de parte de " & empresa.Nombre_Empresa &
            " mi liquidación de prestaciones laborales, no teniendo ningún reclamo anterior ni posterior que hacer por concepto de salario, " &
            "horas extras, ni ningún otro beneficio que tenga por causa la relación laboral."
        Dim lblNota As XRLabel = Etiqueta(nota, 0, _y, Ancho, 42, False, 8, TextAlignment.TopJustify)
        lblNota.Multiline = True
        lblNota.WordWrap = True
        _detalle.Controls.Add(lblNota)
        _y += 75

        Firmas({"Elaborado", "Revisado", "Aprobado", "Recibí conforme"}, {"RRHH", "RRHH", "Gerente General", "Cédula: " & emp.Num_Cedula})
        _y += 25
        Firmas({"Autorizado por", "Entregué conforme", "", ""}, {"Director Ejecutivo", "Contabilidad", "", ""})
        _detalle.Controls.Add(Etiqueta("Fecha y hora recibido:", 380, _y - 30, 370, 14, True))
        _detalle.Controls.Add(Etiqueta("Día ______   Mes ______   Año ________   Hora ________", 380, _y - 14, 370, 14))

    End Sub

#End Region

#Region "AYUDAS"

    Private Sub Seccion(titulo As String)
        Dim lbl As XRLabel = Etiqueta(titulo, 0, _y, Ancho, 17, True, 9)
        lbl.BackColor = ColorSeccion
        lbl.Padding = New PaddingInfo(4, 4, 0, 0)
        _detalle.Controls.Add(lbl)
        _y += 20
    End Sub

    ''' <summary>Tabla Concepto | Detalle | Monto con su total.</summary>
    Private Sub Conceptos(filas As List(Of String()), textoTotal As String, total As Double)

        Dim anchos As Single() = {260, 370, 120}
        Dim tabla As New XRTable With {.BoundsF = New RectangleF(0, _y, Ancho, 0), .Borders = BorderSide.Bottom}
        tabla.BeginInit()
        tabla.Rows.Add(Fila(anchos, True, Color.Empty, "Concepto", "Detalle", "Monto C$"))
        For Each f As String() In filas
            tabla.Rows.Add(Fila(anchos, False, Color.Empty, f))
        Next
        tabla.Rows.Add(Fila(anchos, True, Color.Empty, "", textoTotal, N(total)))
        tabla.HeightF = tabla.Rows.Count * 16
        tabla.EndInit()

        ' Los encabezados y montos a la derecha; la columna de detalle a la izquierda.
        For Each fila As XRTableRow In tabla.Rows
            fila.Cells(1).TextAlignment = TextAlignment.MiddleLeft
            fila.Cells(2).TextAlignment = TextAlignment.MiddleRight
        Next
        tabla.Rows(tabla.Rows.Count - 1).Cells(1).TextAlignment = TextAlignment.MiddleRight

        _detalle.Controls.Add(tabla)
        _y += tabla.HeightF + 8

    End Sub

    Private Sub Firmas(titulos As String(), cargos As String())
        Dim col As Single = Ancho / 4
        For i As Integer = 0 To 3
            If titulos(i) = "" Then Continue For
            Dim x As Single = i * col
            _detalle.Controls.Add(New XRLine With {.BoundsF = New RectangleF(x + 10, _y + 20, col - 20, 2)})
            _detalle.Controls.Add(Etiqueta(titulos(i), x, _y + 23, col, 14, True, 8, TextAlignment.MiddleCenter))
            _detalle.Controls.Add(Etiqueta(cargos(i), x, _y + 37, col, 14, False, 8, TextAlignment.MiddleCenter))
        Next
        _y += 55
    End Sub

    Private Shared Function Fila(anchos As Single(), negrita As Boolean, fondo As Color, ParamArray textos As String()) As XRTableRow
        Dim r As New XRTableRow With {.HeightF = 16}
        For i As Integer = 0 To textos.Length - 1
            Dim c As New XRTableCell With {
                .Text = textos(i), .WidthF = anchos(i), .Padding = New PaddingInfo(3, 3, 0, 0),
                .Font = New DXFont(Fuente, 8, If(negrita, DXFontStyle.Bold, DXFontStyle.Regular)),
                .TextAlignment = If(i = 0, TextAlignment.MiddleLeft, TextAlignment.MiddleRight)}
            If fondo <> Color.Empty Then c.BackColor = fondo
            r.Cells.Add(c)
        Next
        Return r
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

    Private Shared Function FechaTexto(f As Date?) As String
        Return If(f.HasValue, f.Value.ToString("dd/MM/yyyy"), "")
    End Function

#End Region

End Class
