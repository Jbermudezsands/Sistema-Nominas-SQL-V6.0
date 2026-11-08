Imports System.Globalization

''' <summary>
''' Calendario de un año completo (12 meses en 4 columnas x 3 filas) que pinta cada día según su tipo (<see cref="PaletaVacaciones"/>).
''' Reemplaza el calendario de Codejock de la V.5.0. Semana de lunes a domingo, sábados en azul y domingos en rojo, con hoy remarcado y
''' una pista (tooltip) con el tipo y la solicitud de cada día marcado. Se asigna con <see cref="Mostrar"/>; hace scroll si la ventana es pequeña.
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class CalendarioAnual
    Inherits ScrollableControl

    Private Const Columnas As Integer = 4
    Private Const Filas As Integer = 3
    Private Const AnchoMinimoMes As Integer = 170
    Private Const AltoMinimoMes As Integer = 150
    Private Const AltoTitulo As Integer = 20
    Private Const AltoSemana As Integer = 16

    Private ReadOnly _cultura As New CultureInfo("es-NI")
    Private ReadOnly _pista As New ToolTip With {.InitialDelay = 300, .ReshowDelay = 100}
    Private _anio As Integer = Date.Today.Year
    Private _dias As ILookup(Of Date, DiaVacacion) = Enumerable.Empty(Of DiaVacacion)().ToLookup(Function(d) d.Fecha)
    Private _textoPista As String = ""

    Public Sub New()
        DoubleBuffered = True
        ResizeRedraw = True
        BackColor = Color.White
        AutoScroll = True
        AutoScrollMinSize = New Size(Columnas * AnchoMinimoMes, Filas * AltoMinimoMes)
    End Sub

    Public ReadOnly Property Anio As Integer
        Get
            Return _anio
        End Get
    End Property

    ''' <summary>Pinta el <paramref name="anio"/> con los días marcados.</summary>
    Public Sub Mostrar(anio As Integer, dias As IEnumerable(Of DiaVacacion))
        _anio = anio
        _dias = dias.ToLookup(Function(d) d.Fecha.Date)
        Invalidate()
    End Sub

#Region "GEOMETRIA"

    Private Function AnchoMes() As Single
        Return Math.Max(AnchoMinimoMes, ClientSize.Width) / Columnas
    End Function

    Private Function AltoMes() As Single
        Return Math.Max(AltoMinimoMes, ClientSize.Height) / Filas
    End Function

    ''' <summary>Rectángulo del mes (1-12) en coordenadas del contenido (sin scroll).</summary>
    Private Function RectMes(mes As Integer) As RectangleF
        Dim i As Integer = mes - 1
        Return New RectangleF((i Mod Columnas) * AnchoMes() + 6, (i \ Columnas) * AltoMes() + 6, AnchoMes() - 12, AltoMes() - 12)
    End Function

    ''' <summary>Posición de la celda del día dentro del mes: columna 0 = lunes, fila 0 = primera semana.</summary>
    Private Shared Sub PosicionDia(dia As Date, ByRef columna As Integer, ByRef fila As Integer)
        Dim primero As New Date(dia.Year, dia.Month, 1)
        Dim desfase As Integer = (CInt(primero.DayOfWeek) + 6) Mod 7
        columna = (CInt(dia.DayOfWeek) + 6) Mod 7
        fila = (desfase + dia.Day - 1) \ 7
    End Sub

    Private Function RectDia(mes As Integer, columna As Integer, fila As Integer) As RectangleF
        Dim r As RectangleF = RectMes(mes)
        Dim ancho As Single = r.Width / 7
        Dim alto As Single = (r.Height - AltoTitulo - AltoSemana) / 6
        Return New RectangleF(r.X + columna * ancho, r.Y + AltoTitulo + AltoSemana + fila * alto, ancho, alto)
    End Function

#End Region

#Region "PINTADO"

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)

        Dim g As Graphics = e.Graphics
        g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y)
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        Dim centro As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}

        Using fuenteTitulo As New Font(Font, FontStyle.Bold), fuenteDia As New Font(Font.FontFamily, Math.Max(7.0F, Font.Size - 1)),
              fuenteSemana As New Font(Font.FontFamily, Math.Max(7.0F, Font.Size - 1.5F), FontStyle.Bold),
              lapiz As New Pen(Color.FromArgb(200, 205, 215)), lapizHoy As New Pen(Color.Firebrick, 2)

            For mes As Integer = 1 To 12

                Dim r As RectangleF = RectMes(mes)
                g.DrawRectangle(lapiz, r.X, r.Y, r.Width, r.Height)

                Dim titulo As String = _cultura.TextInfo.ToTitleCase(_cultura.DateTimeFormat.GetMonthName(mes)) & " " & _anio
                Using relleno As New SolidBrush(Color.FromArgb(219, 226, 242))
                    g.FillRectangle(relleno, r.X + 1, r.Y + 1, r.Width - 1, AltoTitulo - 1)
                End Using
                g.DrawString(titulo, fuenteTitulo, Brushes.Black, New RectangleF(r.X, r.Y, r.Width, AltoTitulo), centro)

                Dim letras As String() = {"L", "M", "M", "J", "V", "S", "D"}
                For c As Integer = 0 To 6
                    Dim cab As New RectangleF(r.X + c * r.Width / 7, r.Y + AltoTitulo, r.Width / 7, AltoSemana)
                    Using b As New SolidBrush(ColorSemana(c))
                        g.DrawString(letras(c), fuenteSemana, b, cab, centro)
                    End Using
                Next

                For d As Integer = 1 To Date.DaysInMonth(_anio, mes)

                    Dim dia As New Date(_anio, mes, d)
                    Dim col, fila As Integer
                    PosicionDia(dia, col, fila)
                    Dim celda As RectangleF = RectDia(mes, col, fila)
                    Dim marcas As IEnumerable(Of DiaVacacion) = _dias(dia)
                    Dim colorTexto As Color = ColorSemana(col)

                    If marcas.Any() Then
                        Dim principal As DiaVacacion = marcas.OrderBy(Function(m) PaletaVacaciones.Orden(m.Tipo)).First()
                        Dim fondo As Color = PaletaVacaciones.ColorDe(principal.Tipo)
                        Using relleno As New SolidBrush(fondo)
                            g.FillRectangle(relleno, celda.X + 1, celda.Y + 1, celda.Width - 2, celda.Height - 2)
                        End Using
                        colorTexto = PaletaVacaciones.ColorTexto(fondo)
                    End If

                    Using b As New SolidBrush(colorTexto)
                        g.DrawString(d.ToString(), fuenteDia, b, celda, centro)
                    End Using

                    If dia = Date.Today Then g.DrawRectangle(lapizHoy, celda.X + 1, celda.Y + 1, celda.Width - 2, celda.Height - 2)

                Next
            Next
        End Using

        centro.Dispose()

    End Sub

    Private Shared Function ColorSemana(columna As Integer) As Color
        Select Case columna
            Case 5 : Return Color.RoyalBlue
            Case 6 : Return Color.Red
            Case Else : Return Color.Black
        End Select
    End Function

#End Region

#Region "PISTA"

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)

        MyBase.OnMouseMove(e)

        Dim texto As String = TextoEn(New Point(e.X - AutoScrollPosition.X, e.Y - AutoScrollPosition.Y))

        If texto <> _textoPista Then
            _textoPista = texto
            _pista.SetToolTip(Me, texto)
        End If

    End Sub

    ''' <summary>Texto de la pista del día bajo el punto (coordenadas del contenido); vacío si no hay un día marcado.</summary>
    Private Function TextoEn(p As Point) As String

        For mes As Integer = 1 To 12
            If Not RectMes(mes).Contains(p) Then Continue For

            For d As Integer = 1 To Date.DaysInMonth(_anio, mes)
                Dim dia As New Date(_anio, mes, d)
                Dim col, fila As Integer
                PosicionDia(dia, col, fila)
                If Not RectDia(mes, col, fila).Contains(p) Then Continue For

                Dim marcas As List(Of DiaVacacion) = _dias(dia).ToList()
                If marcas.Count = 0 Then Return ""

                Return dia.ToString("dddd dd/MM/yyyy", _cultura) & Environment.NewLine &
                    String.Join(Environment.NewLine, marcas.Select(Function(m) m.Tipo &
                        If(m.Tipo = "Vacaciones Pagadas", " (" & m.Cant_Dias.ToString("0.##") & " días)", "") &
                        If(m.Numero_Solicitud <> "", "  - Solicitud " & m.Numero_Solicitud, "")))
            Next
        Next

        Return ""

    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then _pista.Dispose()
        MyBase.Dispose(disposing)
    End Sub

#End Region

End Class
