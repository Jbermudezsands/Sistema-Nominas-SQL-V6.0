Imports System.Drawing

''' <summary>Dibuja códigos de barras Code 39 (el que usaba el carnet con la fuente "3 of 9") sin depender de fuentes instaladas.</summary>
Public Class CodigoBarras39

    ' Cada carácter son 9 elementos (barra, espacio, barra...): 'w' ancho, 'n' angosto.
    Private Shared ReadOnly Patrones As New Dictionary(Of Char, String) From {
        {"0"c, "nnnwwnwnn"}, {"1"c, "wnnwnnnnw"}, {"2"c, "nnwwnnnnw"}, {"3"c, "wnwwnnnnn"}, {"4"c, "nnnwwnnnw"},
        {"5"c, "wnnwwnnnn"}, {"6"c, "nnwwwnnnn"}, {"7"c, "nnnwnnwnw"}, {"8"c, "wnnwnnwnn"}, {"9"c, "nnwwnnwnn"},
        {"A"c, "wnnnnwnnw"}, {"B"c, "nnwnnwnnw"}, {"C"c, "wnwnnwnnn"}, {"D"c, "nnnnwwnnw"}, {"E"c, "wnnnwwnnn"},
        {"F"c, "nnwnwwnnn"}, {"G"c, "nnnnnwwnw"}, {"H"c, "wnnnnwwnn"}, {"I"c, "nnwnnwwnn"}, {"J"c, "nnnnwwwnn"},
        {"K"c, "wnnnnnnww"}, {"L"c, "nnwnnnnww"}, {"M"c, "wnwnnnnwn"}, {"N"c, "nnnnwnnww"}, {"O"c, "wnnnwnnwn"},
        {"P"c, "nnwnwnnwn"}, {"Q"c, "nnnnnnwww"}, {"R"c, "wnnnnnwwn"}, {"S"c, "nnwnnnwwn"}, {"T"c, "nnnnwnwwn"},
        {"U"c, "wwnnnnnnw"}, {"V"c, "nwwnnnnnw"}, {"W"c, "wwwnnnnnn"}, {"X"c, "nwnnwnnnw"}, {"Y"c, "wwnnwnnnn"},
        {"Z"c, "nwwnwnnnn"}, {"-"c, "nwnnnnwnw"}, {"."c, "wwnnnnwnn"}, {" "c, "nwwnnnwnn"}, {"*"c, "nwnnwnwnn"},
        {"$"c, "nwnwnwnnn"}, {"/"c, "nwnwnnnwn"}, {"+"c, "nwnnnwnwn"}, {"%"c, "nnnwnwnwn"}}

    ''' <summary>Imagen del código de barras de <paramref name="texto"/> (se pasa a mayúsculas; los caracteres que Code 39 no admite se omiten).</summary>
    Public Shared Function Dibujar(texto As String, ancho As Integer, alto As Integer) As Bitmap

        Dim datos As New System.Text.StringBuilder("*")

        For Each ch As Char In If(texto, "").ToUpperInvariant()
            If ch <> "*"c AndAlso Patrones.ContainsKey(ch) Then datos.Append(ch)
        Next

        datos.Append("*"c)

        ' Ancho total en módulos: 3 angostos + 1 ancho(=3) ... por carácter 6 angostos + 3 anchos = 15 módulos + 1 de separación.
        Dim modulos As Integer = datos.Length * 16 - 1
        Dim anchoModulo As Single = ancho / CSng(modulos)

        Dim bmp As New Bitmap(ancho, alto)

        Using g As Graphics = Graphics.FromImage(bmp)

            g.Clear(Color.White)

            Dim x As Single = 0

            For Each ch As Char In datos.ToString()

                Dim patron As String = Patrones(ch)

                For i As Integer = 0 To 8

                    Dim w As Single = If(patron(i) = "w"c, 3, 1) * anchoModulo

                    If i Mod 2 = 0 Then g.FillRectangle(Brushes.Black, x, 0, w, alto)

                    x += w

                Next

                x += anchoModulo

            Next

        End Using

        Return bmp

    End Function

End Class
