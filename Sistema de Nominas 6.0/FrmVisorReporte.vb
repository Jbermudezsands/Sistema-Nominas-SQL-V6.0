Imports DevExpress.XtraEditors
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraPrinting.Preview
Imports DevExpress.XtraReports.UI

''' <summary>
''' Visor común de reportes DevExpress (reemplaza el visor de ActiveReports de la V.5.0): vista previa con
''' impresión, zoom y exportación a PDF o Excel. Se abre con <see cref="Mostrar"/>; la interfaz se arma por código.
''' </summary>
Public Class FrmVisorReporte
    Inherits XtraForm

    Private ReadOnly _reporte As XtraReport
    Private ReadOnly _nombreArchivo As String
    Private ReadOnly _visor As DocumentViewer

    ''' <summary>Muestra el reporte en una ventana modal. <paramref name="nombreArchivo"/> es el nombre sugerido al exportar (sin extensión).</summary>
    Public Shared Sub Mostrar(reporte As XtraReport, Optional nombreArchivo As String = "", Optional propietario As IWin32Window = Nothing)
        Using frm As New FrmVisorReporte(reporte, nombreArchivo)
            frm.ShowDialog(propietario)
        End Using
    End Sub

    Public Sub New(reporte As XtraReport, nombreArchivo As String)

        _reporte = reporte
        _nombreArchivo = If(String.IsNullOrWhiteSpace(nombreArchivo), reporte.DisplayName, nombreArchivo)

        Text = "Vista previa - " & reporte.DisplayName
        ClientSize = New Size(1000, 760)
        StartPosition = FormStartPosition.CenterScreen
        MinimizeBox = False
        ShowInTaskbar = False
        KeyPreview = True

        _visor = New DocumentViewer With {.Dock = DockStyle.Fill, .IsMetric = False}
        Controls.Add(_visor)
        Controls.Add(CrearBarra())

        AddHandler Load, Sub(s, e)
                             _reporte.CreateDocument()
                             _visor.DocumentSource = _reporte
                         End Sub
        AddHandler KeyDown, Sub(s, e) If e.KeyCode = Keys.Escape Then Close()

    End Sub

    Private Function CrearBarra() As PanelControl

        Dim barra As New PanelControl With {.Dock = DockStyle.Top, .Height = 46}
        Dim x As Integer = 8

        Dim boton As Func(Of String, String, EventHandler, SimpleButton) =
            Function(texto, icono, accion)
                Dim b As New SimpleButton With {.Text = texto, .Location = New Point(x, 8), .Size = New Size(If(texto.Length > 12, 140, 110), 30)}
                b.ImageOptions.ImageUri.Uri = icono
                AddHandler b.Click, accion
                x += b.Width + 6
                barra.Controls.Add(b)
                Return b
            End Function

        boton("Imprimir", "Print", AddressOf Imprimir)
        boton("Exportar a PDF", "ExportToPDF", Sub(s, e) Exportar(True))
        boton("Exportar a Excel", "ExportToXLSX", Sub(s, e) Exportar(False))
        boton("Acercar", "ZoomIn", Sub(s, e) _visor.Zoom = Math.Min(5.0F, _visor.Zoom + 0.25F))
        boton("Alejar", "ZoomOut", Sub(s, e) _visor.Zoom = Math.Max(0.25F, _visor.Zoom - 0.25F))

        Dim cerrar As SimpleButton = boton("Cerrar", "Close", Sub(s, e) Close())
        cerrar.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        cerrar.Left = barra.Width - cerrar.Width - 8
        AddHandler barra.Resize, Sub(s, e) cerrar.Left = barra.Width - cerrar.Width - 8

        Return barra

    End Function

    Private Sub Imprimir(sender As Object, e As EventArgs)
        Using herramienta As New ReportPrintTool(_reporte)
            herramienta.PrintDialog()
        End Using
    End Sub

    ''' <summary>Pide la ruta y exporta el reporte a PDF o a Excel (.xlsx); ofrece abrir el archivo.</summary>
    Public Sub Exportar(aPdf As Boolean)

        Using dlg As New SaveFileDialog With {
            .Filter = If(aPdf, "Documento PDF (*.pdf)|*.pdf", "Libro de Excel (*.xlsx)|*.xlsx"),
            .FileName = ArchivoValido(_nombreArchivo),
            .Title = If(aPdf, "Exportar a PDF", "Exportar a Excel")}

            If dlg.ShowDialog(Me) <> DialogResult.OK Then Return

            Try
                Cursor = Cursors.WaitCursor
                If aPdf Then
                    _reporte.ExportToPdf(dlg.FileName)
                Else
                    _reporte.ExportToXlsx(dlg.FileName, New XlsxExportOptions With {
                        .ExportMode = XlsxExportMode.SingleFile, .SheetName = "Reporte", .ShowGridLines = False})
                End If
            Catch ex As Exception
                XtraMessageBox.Show("No se pudo exportar: " & ex.Message, "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Finally
                Cursor = Cursors.Default
            End Try

            If XtraMessageBox.Show("Archivo exportado:" & vbCrLf & dlg.FileName & vbCrLf & vbCrLf & "¿Desea abrirlo?", "Exportar",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                Try
                    Process.Start(New ProcessStartInfo(dlg.FileName) With {.UseShellExecute = True})
                Catch ex As Exception
                    XtraMessageBox.Show("No se pudo abrir el archivo: " & ex.Message, "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End Try
            End If

        End Using

    End Sub

    Private Shared Function ArchivoValido(nombre As String) As String
        For Each c As Char In IO.Path.GetInvalidFileNameChars()
            nombre = nombre.Replace(c, "_"c)
        Next
        Return nombre
    End Function

End Class
