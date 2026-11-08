Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Listado de bajas calculadas y pendientes de procesar (V.5.0 FrmListadoBajas): Abrir la liquidación en <see cref="FrmBajas"/>,
''' Procesar todas las que se ven, Eliminar la seleccionada, Imprimir (<see cref="RptListaBajas"/>) y filtrar por fecha de baja.
''' Datos en <see cref="CsListaBajasDAL"/>; solo UI. Interfaz armada por código. <see cref="Recargar"/> es público.
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmListaBajas
    Inherits XtraForm

    Private ReadOnly _dal As New CsListaBajasDAL

    Private _tabla As DataTable
    Private _filtrado As Boolean

    Private Grid As GridControl
    Private Vista As GridView
    Private DtpDesde, DtpHasta As DateEdit
    Private BtnAbrir, BtnProcesar, BtnEliminar, BtnImprimir, BtnFiltrar, BtnTodas, BtnSalir As SimpleButton

    Public Sub New()

        Text = "Listado de Bajas"
        ClientSize = New Size(1100, 520)
        MinimumSize = New Size(700, 400)
        StartPosition = FormStartPosition.CenterParent

        CrearInterfaz()
        AddHandler Load, AddressOf FrmListaBajas_Load

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        ' Columna de botones acoplada a la derecha, con scroll si la ventana es baja (ver CLAUDE.md: calzar dentro del MDI).
        Dim barra As New XtraScrollableControl With {.Dock = DockStyle.Right, .Width = 150}
        barra.Appearance.BackColor = EstiloGrid.ColorBarra
        barra.Appearance.Options.UseBackColor = True

        BtnAbrir = Boton("Abrir", "Open")
        BtnProcesar = Boton("Procesar", "Apply")
        BtnEliminar = Boton("Eliminar", "Delete")
        BtnImprimir = Boton("Imprimir", "Print")
        BtnSalir = Boton("Salir", "Close")
        BtnFiltrar = Boton("Filtrar", "Filter")
        BtnTodas = Boton("Ver todas", "Refresh")

        DtpDesde = New DateEdit With {.Size = New Size(130, 20)}
        DtpHasta = New DateEdit With {.Size = New Size(130, 20)}
        DtpDesde.EditValue = Date.Today.AddMonths(-1)
        DtpHasta.EditValue = Date.Today

        Dim y As Integer = 10
        For Each b As SimpleButton In {BtnAbrir, BtnProcesar, BtnEliminar, BtnImprimir}
            b.Location = New Point(10, y)
            barra.Controls.Add(b)
            y += 38
        Next

        y += 8
        barra.Controls.Add(New LabelControl With {.Text = "Fecha de baja desde:", .Location = New Point(10, y)})
        DtpDesde.Location = New Point(10, y + 18)
        barra.Controls.Add(DtpDesde)
        y += 46
        barra.Controls.Add(New LabelControl With {.Text = "hasta:", .Location = New Point(10, y)})
        DtpHasta.Location = New Point(10, y + 18)
        barra.Controls.Add(DtpHasta)
        y += 48

        BtnFiltrar.Location = New Point(10, y)
        barra.Controls.Add(BtnFiltrar)
        y += 38
        BtnTodas.Location = New Point(10, y)
        barra.Controls.Add(BtnTodas)
        y += 46
        BtnSalir.Location = New Point(10, y)
        barra.Controls.Add(BtnSalir)

        AddHandler BtnAbrir.Click, AddressOf BtnAbrir_Click
        AddHandler BtnProcesar.Click, AddressOf BtnProcesar_Click
        AddHandler BtnEliminar.Click, AddressOf BtnEliminar_Click
        AddHandler BtnImprimir.Click, AddressOf BtnImprimir_Click
        AddHandler BtnFiltrar.Click, AddressOf BtnFiltrar_Click
        AddHandler BtnTodas.Click, AddressOf BtnTodas_Click
        AddHandler BtnSalir.Click, Sub(s, e) Close()

        Grid = New GridControl With {.Dock = DockStyle.Fill}
        Vista = New GridView(Grid)
        Grid.MainView = Vista
        Grid.ViewCollection.Add(Vista)
        Vista.OptionsView.ShowGroupPanel = False
        Vista.OptionsView.ShowAutoFilterRow = True
        Vista.OptionsView.ShowFooter = True
        Vista.OptionsView.ColumnAutoWidth = False
        Vista.OptionsBehavior.Editable = False
        EstiloGrid.Aplicar(Vista)
        AddHandler Vista.DoubleClick, AddressOf BtnAbrir_Click

        Controls.Add(Grid)
        Controls.Add(barra)

    End Sub

    Private Shared Function Boton(texto As String, icono As String) As SimpleButton
        Dim b As New SimpleButton With {.Text = texto, .Size = New Size(130, 30)}
        b.ImageOptions.ImageUri.Uri = icono
        Return b
    End Function

#End Region

#Region "CARGA"

    Private Sub FrmListaBajas_Load(sender As Object, e As EventArgs)
        If MdiParent IsNot Nothing Then WindowState = FormWindowState.Maximized
        Recargar()
    End Sub

    ''' <summary>Vuelve a consultar el listado, respetando el filtro de fechas si está activo.</summary>
    Public Sub Recargar()

        Try

            Cursor = Cursors.WaitCursor

            If _filtrado Then
                _tabla = _dal.Listar(DtpDesde.DateTime.Date, DtpHasta.DateTime.Date)
            Else
                _tabla = _dal.Listar()
            End If

            Grid.DataSource = _tabla
            ConfigurarColumnas()
            Text = $"Listado de Bajas ({_tabla.Rows.Count})"

        Catch ex As Exception
            XtraMessageBox.Show("No se pudo leer el listado de bajas: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub ConfigurarColumnas()

        Vista.PopulateColumns()

        For Each c As String In {"Id", "CodEmpleado", "Annos", "Meses", "Dias", "Prestamo", "CuentaBanco"}
            If Vista.Columns(c) IsNot Nothing Then Vista.Columns(c).Visible = False
        Next

        Texto("CodEmpleado1", "Código", 70)
        Texto("Nombres", "Nombres", 230)
        Texto("Departamento", "Área", 130)
        Fecha("FechaIngreso", "Fecha Ingreso", 95)
        Fecha("FechaBaja", "Fecha Egreso", 95)
        Texto("TipoBaja", "Tipo de Baja", 90)
        Monto("SalarioMensual", "Salario Mensual", 100)
        Monto("MontoVacaciones", "Vacaciones", 95)
        Monto("MontoAguinaldo", "Aguinaldo", 95)
        Monto("MontoIndemnizacion", "Indemnización", 95)
        Monto("Deducciones", "Deducciones", 95)
        Monto("MontoINSS", "INSS", 85)
        Monto("MontoIR", "IR", 85)
        Monto("NetoPagar", "Neto a Pagar", 105)
        Texto("NumeroInss", "Número INSS", 100)

        If Vista.Columns("NetoPagar") IsNot Nothing Then
            Vista.Columns("NetoPagar").Summary.Add(DevExpress.Data.SummaryItemType.Sum, "NetoPagar", "{0:n2}")
        End If

    End Sub

    Private Sub Texto(campo As String, titulo As String, ancho As Integer)
        Dim c As GridColumn = Vista.Columns(campo)
        If c Is Nothing Then Return
        c.Caption = titulo
        c.Width = ancho
    End Sub

    Private Sub Fecha(campo As String, titulo As String, ancho As Integer)
        Dim c As GridColumn = Vista.Columns(campo)
        If c Is Nothing Then Return
        c.Caption = titulo
        c.Width = ancho
        c.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
        c.DisplayFormat.FormatString = "dd/MM/yyyy"
    End Sub

    Private Sub Monto(campo As String, titulo As String, ancho As Integer)
        Dim c As GridColumn = Vista.Columns(campo)
        If c Is Nothing Then Return
        c.Caption = titulo
        c.Width = ancho
        c.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        c.DisplayFormat.FormatString = "n2"
    End Sub

#End Region

#Region "ACCIONES"

    Private Function FilaSeleccionada() As DataRow
        Return TryCast(Vista.GetFocusedDataRow(), DataRow)
    End Function

    ''' <summary>Abre la liquidación del empleado seleccionado en <see cref="FrmBajas"/> (como hijo MDI si este lo es).</summary>
    Private Sub BtnAbrir_Click(sender As Object, e As EventArgs)

        Dim fila As DataRow = FilaSeleccionada()
        If fila Is Nothing Then Return

        Try
            Dim frm As FrmBajas = My.Forms.FrmBajas
            If MdiParent IsNot Nothing Then frm.MdiParent = MdiParent
            frm.Show()
            frm.Activate()
            frm.CargarEmpleado(CStr(fila("CodEmpleado1")))
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo abrir la liquidación: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    ''' <summary>Marca como procesadas todas las bajas que muestra la lista (con el filtro aplicado), como la V.5.0.</summary>
    Private Sub BtnProcesar_Click(sender As Object, e As EventArgs)

        If _tabla Is Nothing OrElse _tabla.Rows.Count = 0 Then
            XtraMessageBox.Show("No hay bajas para procesar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If XtraMessageBox.Show($"¿Está seguro de procesar los {_tabla.Rows.Count} registros del listado?", "Zeus Nominas",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Return

        Try
            Dim ids As List(Of Integer) = _tabla.AsEnumerable().Select(Function(r) Convert.ToInt32(r("Id"))).ToList()
            Dim n As Integer = _dal.Procesar(ids)
            Recargar()
            XtraMessageBox.Show($"Registros procesados: {n}", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo procesar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnEliminar_Click(sender As Object, e As EventArgs)

        Dim fila As DataRow = FilaSeleccionada()
        If fila Is Nothing Then Return

        If XtraMessageBox.Show($"¿Está seguro de borrar la liquidación de {fila("Nombres")}?", "Borrando la baja: " & CStr(fila("CodEmpleado1")),
                               MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then Return

        Try
            _dal.Eliminar(Convert.ToInt32(fila("Id")))
            Recargar()
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo eliminar: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnImprimir_Click(sender As Object, e As EventArgs)

        If _tabla Is Nothing OrElse _tabla.Rows.Count = 0 Then
            XtraMessageBox.Show("No hay bajas para imprimir.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Try
            Dim periodo As String = If(_filtrado, $"Del {DtpDesde.DateTime:dd/MM/yyyy} al {DtpHasta.DateTime:dd/MM/yyyy}", "")
            Dim empresa As DatosEmpresa = New CsControlesDAL().ObtenerDatosEmpresa()
            Using rpt As New RptListaBajas(empresa, _tabla.Copy(), periodo)
                FrmVisorReporte.Mostrar(rpt, "Listado de Bajas", Me)
            End Using
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo mostrar el reporte: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnFiltrar_Click(sender As Object, e As EventArgs)

        If DtpDesde.DateTime.Date > DtpHasta.DateTime.Date Then
            XtraMessageBox.Show("La fecha inicial no puede ser mayor que la final.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        _filtrado = True
        Recargar()

    End Sub

    Private Sub BtnTodas_Click(sender As Object, e As EventArgs)
        _filtrado = False
        Recargar()
    End Sub

#End Region

End Class
