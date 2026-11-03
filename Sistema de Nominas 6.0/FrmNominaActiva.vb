Imports DevExpress.XtraEditors

''' <summary>
''' Reportes de la nómina activa (FrmNominaActiva de la V.5.0, que se abría con el botón "Reportes" de FrmCalcularNomina): lista de
''' reportes de la nómina, vista previa/impresión con DevExpress y exportación a Excel y CSV. Solo UI: los datos salen de
''' <see cref="ReportesNominaActiva"/> (consultas en <see cref="CsNominaActivaDAL"/>). La interfaz se arma por código (no hay Designer).
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmNominaActiva
    Inherits XtraForm

    Private ReadOnly _numNomina As Integer
    Private ReadOnly _periodo As String
    Private ReadOnly _reportes As ReportesNominaActiva
    Private ReadOnly _lista As IList(Of ReporteNominaActiva) = ReportesNominaActiva.Lista()
    Private _ocupado As Boolean

    Private LstReportes As ListBoxControl
    Private LblDescripcion, LblEstado As LabelControl
    Private BtnImprimir, BtnExcel, BtnCsv, BtnSalir As SimpleButton
    Private BarraProgreso As MarqueeProgressBarControl

    ''' <param name="numNomina">Número de la nómina activa que se reporta.</param>
    ''' <param name="periodo">Texto del encabezado, por ejemplo "Nómina No. 33 - Del 16/12/2025 al 31/12/2025".</param>
    ''' <param name="mostrarCeros">MostraEmpleadoCero del tipo de nómina.</param>
    Public Sub New(numNomina As Integer, periodo As String, mostrarCeros As Boolean)

        _numNomina = numNomina
        _periodo = periodo
        _reportes = New ReportesNominaActiva(numNomina, periodo, mostrarCeros, New CsControlesDAL().ObtenerDatosEmpresa())

        Text = $"Reporte de la Nómina Activa - No. {numNomina}"
        ClientSize = New Size(640, 430)
        MinimumSize = New Size(560, 380)
        StartPosition = FormStartPosition.CenterParent

        CrearInterfaz()
        AddHandler Load, AddressOf FrmNominaActiva_Load

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        Dim superior As New PanelControl With {.Dock = DockStyle.Top, .Height = 36}
        EstiloGrid.AplicarBarra(superior)
        Dim lblPeriodo As New LabelControl With {.Text = _periodo, .Location = New Point(12, 11)}
        lblPeriodo.Appearance.Font = New Font(lblPeriodo.Appearance.Font, FontStyle.Bold)
        superior.Controls.Add(lblPeriodo)

        LstReportes = New ListBoxControl With {.Dock = DockStyle.Fill, .ItemHeight = 24}
        AddHandler LstReportes.SelectedIndexChanged, AddressOf LstReportes_SelectedIndexChanged
        AddHandler LstReportes.DoubleClick, AddressOf BtnImprimir_Click

        LblDescripcion = New LabelControl With {.Dock = DockStyle.Bottom, .AutoSizeMode = LabelAutoSizeMode.None, .Height = 40, .Padding = New Padding(8, 6, 8, 0)}
        LblDescripcion.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap

        BarraProgreso = New MarqueeProgressBarControl With {.Dock = DockStyle.Bottom, .Height = 22, .Visible = False}

        LblEstado = New LabelControl With {.Dock = DockStyle.Bottom, .AutoSizeMode = LabelAutoSizeMode.None, .Height = 20, .Padding = New Padding(8, 3, 8, 0)}

        Dim barra As New FlowLayoutPanel With {.Dock = DockStyle.Bottom, .Height = 46, .Padding = New Padding(8, 6, 8, 4)}
        BtnImprimir = Boton("Imprimir", "Print", 110)
        BtnExcel = Boton("Exportar", "ExportToXLSX", 110)
        BtnCsv = Boton("Exp. CSV", "ExportToCSV", 110)
        BtnSalir = Boton("Salir", "Close", 80)
        AddHandler BtnImprimir.Click, AddressOf BtnImprimir_Click
        AddHandler BtnExcel.Click, AddressOf BtnExcel_Click
        AddHandler BtnCsv.Click, AddressOf BtnCsv_Click
        AddHandler BtnSalir.Click, Sub(s, e) Close()
        barra.Controls.AddRange(New Control() {BtnImprimir, BtnExcel, BtnCsv, BtnSalir})

        Controls.Add(LstReportes)
        Controls.Add(LblDescripcion)
        Controls.Add(BarraProgreso)
        Controls.Add(LblEstado)
        Controls.Add(barra)
        Controls.Add(superior)

    End Sub

    Private Shared Function Boton(texto As String, icono As String, ancho As Integer) As SimpleButton
        Dim b As New SimpleButton With {.Text = texto, .Size = New Size(ancho, 30)}
        b.ImageOptions.ImageUri.Uri = icono
        Return b
    End Function

#End Region

#Region "SELECCIÓN"

    Private Sub FrmNominaActiva_Load(sender As Object, e As EventArgs)

        For Each r As ReporteNominaActiva In _lista
            LstReportes.Items.Add(r.Nombre)
        Next
        LstReportes.SelectedIndex = 0

    End Sub

    Private Function Seleccionado() As ReporteNominaActiva
        Return _lista(Math.Max(0, LstReportes.SelectedIndex))
    End Function

    Private Sub LstReportes_SelectedIndexChanged(sender As Object, e As EventArgs)

        If LstReportes.SelectedIndex < 0 Then Return

        Dim r As ReporteNominaActiva = Seleccionado()
        LblDescripcion.Text = r.Descripcion
        ActualizarBotones()

    End Sub

    ''' <summary>Las colillas no se exportan; la nómina sin formato no se imprime.</summary>
    Private Sub ActualizarBotones()

        Dim r As ReporteNominaActiva = Seleccionado()
        Dim exporta As Boolean = r.Clave <> "ColillaCadaDepartamento"

        BtnImprimir.Enabled = r.Imprime AndAlso Not _ocupado
        BtnExcel.Enabled = exporta AndAlso Not _ocupado
        BtnCsv.Enabled = exporta AndAlso Not _ocupado
        LstReportes.Enabled = Not _ocupado

    End Sub

    Private Sub Ocupar(ocupado As Boolean, mensaje As String)
        _ocupado = ocupado
        BarraProgreso.Visible = ocupado
        LblEstado.Text = mensaje
        Cursor = If(ocupado, Cursors.WaitCursor, Cursors.Default)
        ActualizarBotones()
    End Sub

#End Region

#Region "IMPRIMIR Y EXPORTAR"

    Private Async Sub BtnImprimir_Click(sender As Object, e As EventArgs)

        If _ocupado Then Return

        Dim r As ReporteNominaActiva = Seleccionado()
        If Not r.Imprime Then Return

        Try
            Ocupar(True, $"Generando {r.Nombre}...")
            Dim datos As DataTable = Await Task.Run(Function() _reportes.Datos(r.Clave))

            If datos.Rows.Count = 0 Then
                Ocupar(False, "")
                XtraMessageBox.Show("El reporte no tiene datos para esta nómina.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim reporte As DevExpress.XtraReports.UI.XtraReport = _reportes.Construir(r.Clave, datos)
            Registrar($"Se imprimio {r.Nombre} de la nomina: {_numNomina}")
            Ocupar(False, $"{datos.Rows.Count} filas.")
            FrmVisorReporte.Mostrar(reporte, $"{r.Nombre} - Nómina {_numNomina}", Me)
        Catch ex As Exception
            Ocupar(False, "")
            XtraMessageBox.Show("No se pudo generar el reporte: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Async Sub BtnExcel_Click(sender As Object, e As EventArgs)

        If _ocupado Then Return

        Dim r As ReporteNominaActiva = Seleccionado()

        Try
            Ocupar(True, $"Preparando {r.Nombre}...")
            Dim tabla As DataTable = Await Task.Run(Function() _reportes.TablaExportable(r.Clave, _reportes.Datos(r.Clave)))
            Ocupar(False, $"{tabla.Rows.Count} filas.")

            If tabla.Rows.Count = 0 Then
                XtraMessageBox.Show("El reporte no tiene datos para esta nómina.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim ruta As String = ExportadorGrid.ExportarTabla(tabla, $"{r.Nombre} {_numNomina}")
            If ruta <> "" Then
                Registrar($"Se exporto a Excel {r.Nombre} de la nomina: {_numNomina}")
                XtraMessageBox.Show("Archivo exportado:" & vbCrLf & ruta, Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            Ocupar(False, "")
            XtraMessageBox.Show("No se pudo exportar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Async Sub BtnCsv_Click(sender As Object, e As EventArgs)

        If _ocupado Then Return

        Dim r As ReporteNominaActiva = Seleccionado()

        Try
            Using dlg As New SaveFileDialog With {.Filter = "Archivo CSV (*.csv)|*.csv", .FileName = $"{r.Nombre} {_numNomina}", .Title = "Exportar a CSV"}
                If dlg.ShowDialog(Me) <> DialogResult.OK Then Return

                Dim ruta As String = dlg.FileName
                Ocupar(True, $"Exportando {r.Nombre}...")

                Dim filas As Integer = Await Task.Run(
                    Function()
                        Dim datos As DataTable = _reportes.Datos(r.Clave)
                        ExportadorCsv.Escribir(datos, _reportes.ColumnasCsv(r.Clave, datos), ruta)
                        Return datos.Rows.Count
                    End Function)

                Ocupar(False, $"{filas} filas exportadas.")
                Registrar($"Se exporto a CSV {r.Nombre} de la nomina: {_numNomina}")
                XtraMessageBox.Show("Archivo exportado:" & vbCrLf & ruta, Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Using
        Catch ex As Exception
            Ocupar(False, "")
            XtraMessageBox.Show("No se pudo exportar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub Registrar(accion As String)
        Try
            Dim cierre As New CsCierreNominaDAL
            cierre.Registrar(New Compañias().Nombre_Usuario, accion)
        Catch
            ' El rastro de un reporte no debe estorbar al usuario.
        End Try
    End Sub

#End Region

End Class
