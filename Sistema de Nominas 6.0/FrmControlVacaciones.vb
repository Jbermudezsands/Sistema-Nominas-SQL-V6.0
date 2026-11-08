Imports DevExpress.XtraEditors

''' <summary>
''' Control de vacaciones por empleado (V.5.0 FrmControlVacaciones): calendario del año con los días marcados por tipo
''' (vacaciones, programadas, permisos, ausencias, feriados, subsidios), el saldo oficial a una fecha de corte y el resumen del año.
''' Se imprime en DevExpress con <see cref="RptControlVacaciones"/>. Datos en <see cref="CsControlVacacionesDAL"/>; solo UI, armada por código.
''' <see cref="CargarEmpleado"/> es público para abrir la pantalla con un empleado desde otros formularios.
''' </summary>
''' <remarks>
''' Diferencias con la V.5.0: el saldo se calcula una sola vez para una fecha de corte (hoy por omisión) con el mismo cálculo de las solicitudes,
''' no al inicio del rango visible del calendario; los contadores se llenan al cargar el año, no dentro del pintado de cada día;
''' "tomados" son solo Vacaciones y Vacaciones Pagadas (Ausente, Feriado y Subsidio no descuentan vacaciones); y se ve el año completo.
''' </remarks>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmControlVacaciones
    Inherits XtraForm

    Private ReadOnly _dal As New CsControlVacacionesDAL

    Private _control As ControlVacaciones
    Private _cargando As Boolean

    Private CmbEmpleado As SearchLookUpEdit
    Private TxtNombre, TxtDepartamento, TxtCargo, TxtContrato As TextEdit
    Private TxtAnio As SpinEdit
    Private DtpCorte As DateEdit
    Private Calendario As CalendarioAnual
    Private BtnImprimir, BtnActualizar, BtnSalir As SimpleButton
    Private PnlLeyenda As XtraScrollableControl
    Private LblAcumulado, LblSolicitado, LblSaldo, LblProgramados, LblDisponible, LblTomados, LblAviso As LabelControl

    Public Sub New()

        Text = "Calendario de Vacaciones"
        ClientSize = New Size(1100, 560)
        MinimumSize = New Size(760, 480)
        StartPosition = FormStartPosition.CenterParent

        CrearInterfaz()
        AddHandler Load, AddressOf FrmControlVacaciones_Load

    End Sub

#Region "INTERFAZ"

    Private Sub CrearInterfaz()

        ' Columna derecha acoplada: botones, saldo y leyenda (con scroll si la ventana es baja; ver CLAUDE.md sobre el MDI).
        Dim derecha As New XtraScrollableControl With {.Dock = DockStyle.Right, .Width = 250}
        derecha.Appearance.BackColor = EstiloGrid.ColorBarra
        derecha.Appearance.Options.UseBackColor = True

        BtnImprimir = Boton("Imprimir", "Print")
        BtnActualizar = Boton("Actualizar", "Refresh")
        BtnSalir = Boton("Salir", "Close")
        BtnImprimir.Location = New Point(10, 10)
        BtnActualizar.Location = New Point(130, 10)
        BtnSalir.Location = New Point(10, 46)
        BtnImprimir.Size = New Size(110, 30)
        BtnActualizar.Size = New Size(110, 30)
        BtnSalir.Size = New Size(110, 30)
        derecha.Controls.AddRange(New Control() {BtnImprimir, BtnActualizar, BtnSalir})

        AddHandler BtnImprimir.Click, AddressOf BtnImprimir_Click
        AddHandler BtnActualizar.Click, Sub(s, e) Recargar()
        AddHandler BtnSalir.Click, Sub(s, e) Close()

        Dim gSaldo As New GroupControl With {.Text = "Saldo de vacaciones", .Location = New Point(10, 88), .Size = New Size(230, 150)}
        LblAcumulado = FilaValor(gSaldo, "Días acumulados:", 28)
        LblSolicitado = FilaValor(gSaldo, "Días solicitados:", 50)
        LblSaldo = FilaValor(gSaldo, "Saldo disponible:", 72, True)
        LblProgramados = FilaValor(gSaldo, "Programados del año:", 94)
        LblDisponible = FilaValor(gSaldo, "Disponible sin programar:", 116)
        derecha.Controls.Add(gSaldo)

        Dim gAnio As New GroupControl With {.Text = "Días del año por tipo", .Location = New Point(10, 246), .Size = New Size(230, 260)}
        PnlLeyenda = New XtraScrollableControl With {.Dock = DockStyle.Fill}
        gAnio.Controls.Add(PnlLeyenda)
        LblTomados = New LabelControl With {.Dock = DockStyle.Bottom, .Padding = New Padding(4)}
        gAnio.Controls.Add(LblTomados)
        derecha.Controls.Add(gAnio)

        ' Cabecera: empleado, año y fecha de corte
        Dim cabecera As New PanelControl With {.Dock = DockStyle.Top, .Height = 92, .BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder}

        cabecera.Controls.Add(Etiqueta("Código:", 10, 13))
        CmbEmpleado = New SearchLookUpEdit With {.Location = New Point(75, 10), .Size = New Size(130, 20)}
        CmbEmpleado.Properties.DisplayMember = "CodEmpleado1"
        CmbEmpleado.Properties.ValueMember = "CodEmpleado1"
        CmbEmpleado.Properties.NullText = ""
        CmbEmpleado.Properties.PopupFormSize = New Size(450, 350)
        AddHandler CmbEmpleado.EditValueChanged, AddressOf CmbEmpleado_EditValueChanged
        cabecera.Controls.Add(CmbEmpleado)

        TxtNombre = Lectura(cabecera, "Nombre:", 225, 13, 280, 330)
        TxtDepartamento = Lectura(cabecera, "Departamento:", 10, 39, 100, 195)
        TxtCargo = Lectura(cabecera, "Cargo:", 315, 39, 360, 200)
        TxtContrato = Lectura(cabecera, "Contrato vac.:", 525, 39, 605, 90)

        cabecera.Controls.Add(Etiqueta("Año:", 10, 67))
        TxtAnio = New SpinEdit With {.Location = New Point(75, 64), .Size = New Size(70, 20)}
        TxtAnio.Properties.MinValue = 2000
        TxtAnio.Properties.MaxValue = 2100
        TxtAnio.Properties.IsFloatValue = False
        TxtAnio.Properties.Mask.EditMask = "0000"
        TxtAnio.Properties.Mask.UseMaskAsDisplayFormat = True
        TxtAnio.Value = Date.Today.Year
        AddHandler TxtAnio.EditValueChanged, Sub(s, e) Recargar()
        cabecera.Controls.Add(TxtAnio)

        cabecera.Controls.Add(Etiqueta("Saldo al:", 170, 67))
        DtpCorte = New DateEdit With {.Location = New Point(225, 64), .Size = New Size(100, 20), .DateTime = Date.Today}
        AddHandler DtpCorte.EditValueChanged, Sub(s, e) Recargar()
        cabecera.Controls.Add(DtpCorte)

        LblAviso = New LabelControl With {.Location = New Point(345, 67), .Text = ""}
        LblAviso.Appearance.ForeColor = Color.Firebrick
        cabecera.Controls.Add(LblAviso)

        Calendario = New CalendarioAnual With {.Dock = DockStyle.Fill}

        Controls.Add(Calendario)
        Controls.Add(cabecera)
        Controls.Add(derecha)
        Calendario.BringToFront()

    End Sub

    Private Shared Function Boton(texto As String, icono As String) As SimpleButton
        Dim b As New SimpleButton With {.Text = texto}
        b.ImageOptions.ImageUri.Uri = icono
        Return b
    End Function

    Private Shared Function Etiqueta(texto As String, x As Integer, y As Integer) As LabelControl
        Return New LabelControl With {.Text = texto, .Location = New Point(x, y)}
    End Function

    Private Shared Function Lectura(padre As Control, titulo As String, xEt As Integer, y As Integer, x As Integer, ancho As Integer) As TextEdit
        padre.Controls.Add(Etiqueta(titulo, xEt, y + 3))
        Dim t As New TextEdit With {.Location = New Point(x, y), .Size = New Size(ancho, 20)}
        t.Properties.ReadOnly = True
        padre.Controls.Add(t)
        Return t
    End Function

    Private Shared Function FilaValor(g As GroupControl, titulo As String, y As Integer, Optional negrita As Boolean = False) As LabelControl
        g.Controls.Add(New LabelControl With {.Text = titulo, .Location = New Point(10, y)})
        Dim v As New LabelControl With {
            .Location = New Point(150, y), .AutoSizeMode = LabelAutoSizeMode.None, .Size = New Size(70, 15), .Text = "0.00"}
        v.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
        If negrita Then v.Appearance.Font = New Font(v.Appearance.Font, FontStyle.Bold)
        g.Controls.Add(v)
        Return v
    End Function

#End Region

#Region "CARGA"

    Private Sub FrmControlVacaciones_Load(sender As Object, e As EventArgs)

        If MdiParent IsNot Nothing Then WindowState = FormWindowState.Maximized

        Try
            FrmConsulta.AgregarBotonBuscar(CmbEmpleado, TipoBusqueda.Empleado, Me)
            CmbEmpleado.Properties.DataSource = _dal.ListarEmpleados()
            CmbEmpleado.Properties.View.Columns.Clear()
            CmbEmpleado.Properties.View.PopulateColumns(CmbEmpleado.Properties.DataSource)
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo cargar la lista de empleados: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Limpiar()

    End Sub

    ''' <summary>Muestra el control de vacaciones del empleado con ese código visible.</summary>
    Public Sub CargarEmpleado(codigoVisible As String)
        CmbEmpleado.EditValue = codigoVisible
    End Sub

    Private Sub CmbEmpleado_EditValueChanged(sender As Object, e As EventArgs)
        If _cargando Then Return
        Recargar()
    End Sub

    Private Sub Recargar()

        If _cargando Then Return

        Dim codigo As String = Convert.ToString(CmbEmpleado.EditValue)

        If String.IsNullOrWhiteSpace(codigo) Then
            Limpiar()
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor

            Dim anio As Integer = Convert.ToInt32(TxtAnio.Value)
            Dim corte As Date = If(DtpCorte.EditValue Is Nothing, Date.Today, DtpCorte.DateTime.Date)

            _control = _dal.Cargar(codigo, anio, corte)
            Mostrar()

        Catch ex As InvalidOperationException
            XtraMessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Limpiar()
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo cargar el control de vacaciones: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub Limpiar()

        _control = Nothing
        TxtNombre.Text = ""
        TxtDepartamento.Text = ""
        TxtCargo.Text = ""
        TxtContrato.Text = ""
        LblAviso.Text = ""
        Calendario.Mostrar(Convert.ToInt32(TxtAnio.Value), New List(Of DiaVacacion))
        MostrarResumen(Nothing)
        BtnImprimir.Enabled = False

    End Sub

    Private Sub Mostrar()

        TxtNombre.Text = _control.Nombre_Completo
        TxtDepartamento.Text = _control.Departamento
        TxtCargo.Text = _control.Cargo
        TxtContrato.Text = If(_control.Fecha_Contrato_Vac.HasValue, _control.Fecha_Contrato_Vac.Value.ToString("dd/MM/yyyy"), "")
        LblAviso.Text = If(_control.Fecha_Contrato_Vac.HasValue, "", "El empleado no tiene fecha de contrato: no se puede calcular el saldo.")

        Calendario.Mostrar(_control.Anio, _control.Dias)
        MostrarResumen(_control)
        BtnImprimir.Enabled = True

    End Sub

    Private Sub MostrarResumen(c As ControlVacaciones)

        LblAcumulado.Text = If(c Is Nothing, "0.00", c.Resumen.Acumulado.ToString("n2"))
        LblSolicitado.Text = If(c Is Nothing, "0.00", c.Resumen.Solicitado.ToString("n2"))
        LblSaldo.Text = If(c Is Nothing, "0.00", c.Resumen.Saldo.ToString("n2"))
        LblProgramados.Text = If(c Is Nothing, "0.00", c.Programados_Anio.ToString("n2"))
        LblDisponible.Text = If(c Is Nothing, "0.00", c.Disponible_Sin_Programar.ToString("n2"))
        LblTomados.Text = If(c Is Nothing, "", "Vacaciones tomadas y pagadas en " & c.Anio & ": " & c.Disfrutados_Anio.ToString("n2") & " días")

        PnlLeyenda.Controls.Clear()

        Dim tipos As New List(Of String)(PaletaVacaciones.TiposConocidos)
        If c IsNot Nothing Then tipos.AddRange(c.Dias.Select(Function(d) d.Tipo).Distinct().Where(Function(t) Not tipos.Contains(t)))

        Dim y As Integer = 6
        For Each tipo As String In tipos
            Dim dias As Double = If(c Is Nothing, 0, c.DiasDe(tipo))
            Dim muestra As New Panel With {.Location = New Point(6, y + 1), .Size = New Size(16, 14), .BorderStyle = BorderStyle.FixedSingle, .BackColor = PaletaVacaciones.ColorDe(tipo)}
            PnlLeyenda.Controls.Add(muestra)
            PnlLeyenda.Controls.Add(New LabelControl With {.Text = tipo, .Location = New Point(28, y)})
            Dim valor As New LabelControl With {.Text = dias.ToString("0.##"), .Location = New Point(160, y), .AutoSizeMode = LabelAutoSizeMode.None, .Size = New Size(40, 15)}
            valor.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far
            If dias > 0 Then valor.Appearance.Font = New Font(valor.Appearance.Font, FontStyle.Bold)
            PnlLeyenda.Controls.Add(valor)
            y += 20
        Next

    End Sub

#End Region

#Region "IMPRESION"

    ''' <summary>Imprime el calendario del año con el saldo y el detalle de solicitudes (reporte DevExpress).</summary>
    Private Sub BtnImprimir_Click(sender As Object, e As EventArgs)

        If _control Is Nothing Then Return

        Try
            Cursor = Cursors.WaitCursor
            Using rpt As New RptControlVacaciones(New CsControlesDAL().ObtenerDatosEmpresa(), _control)
                Cursor = Cursors.Default
                FrmVisorReporte.Mostrar(rpt, "Control vacaciones " & _control.Codigo & " " & _control.Anio, Me)
            End Using
        Catch ex As Exception
            XtraMessageBox.Show("No se pudo mostrar el reporte: " & ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

#End Region

End Class
