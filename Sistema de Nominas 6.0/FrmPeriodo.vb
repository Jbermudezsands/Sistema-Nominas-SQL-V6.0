Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls

''' <summary>
''' Calendario de períodos de una nómina. Una sola pantalla para dos calendarios:
''' Planilla (V.5.0 frmFecha) y Fiscal (V.5.0 FrmPeriodoFiscal). El cálculo está en GeneradorPeriodos
''' y el acceso a datos en CsPeriodoDAL.
''' </summary>
Public Class FrmPeriodo

    Private ReadOnly _calendario As TipoCalendario
    Private ReadOnly _dal As CsPeriodoDAL

    Private _tipos As DataTable
    Private _cargando As Boolean = False
    Private _periodos As New List(Of PeriodoNomina)

    Public Sub New(calendario As TipoCalendario)

        InitializeComponent()

        _calendario = calendario
        _dal = New CsPeriodoDAL(calendario)

        If calendario = TipoCalendario.Planilla Then
            Text = "Períodos de Nómina"
            groupDatos.Text = "Planilla"
        Else
            Text = "Períodos Fiscales"
            groupDatos.Text = "Período fiscal"
        End If

    End Sub

#Region "CARGA INICIAL"

    Private Sub FrmPeriodo_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try

            _cargando = True

            _tipos = _dal.ListarTiposNomina()

            With cmbTipoNomina.Properties
                .DataSource = _tipos
                .ValueMember = "CodTipoNomina"
                .DisplayMember = "Nomina"
                .Columns.Clear()
                .Columns.Add(New LookUpColumnInfo("CodTipoNomina", "Código", 50))
                .Columns.Add(New LookUpColumnInfo("Nomina", "Nómina", 200))
                .NullText = ""
            End With

            With gridView1
                .Columns.Clear()
                .Columns.AddVisible("Periodo", "Período")
                .Columns.AddVisible("Mes", "Mes")
                .Columns.AddVisible("Inicio", "Desde")
                .Columns.AddVisible("Final", "Hasta")
                .Columns.AddVisible("Estado", "Estado")
            End With

            _cargando = False

            BtnGuardar.Enabled = False

            If _tipos.Rows.Count > 0 Then cmbTipoNomina.EditValue = Convert.ToString(_tipos.Rows(0)("CodTipoNomina"))

        Catch ex As Exception
            _cargando = False
            MostrarError(ex)
        End Try

    End Sub

#End Region

#Region "PANTALLA"

    Private Function CodigoSeleccionado() As String
        Return Convert.ToString(cmbTipoNomina.EditValue).Trim
    End Function

    Private Function PeriodoSeleccionado() As String

        Dim cod As String = CodigoSeleccionado()

        For Each r As DataRow In _tipos.Rows
            If Convert.ToString(r("CodTipoNomina")) = cod Then Return Convert.ToString(r("Periodo")).Trim
        Next

        Return ""

    End Function

    ''' <summary>Planilla: 1 de enero al 31 de diciembre. Fiscal: 1 de julio al 30 de junio siguiente.</summary>
    Private Sub FechasPorDefecto()

        Dim hoy As Integer = Date.Today.Year

        If _calendario = TipoCalendario.Planilla Then
            dtpInicio.DateTime = New Date(hoy, 1, 1)
            dtpFinal.DateTime = New Date(hoy, 12, 31)
            TxtAnio.EditValue = hoy
        Else
            dtpInicio.DateTime = New Date(hoy, 7, 1)
            dtpFinal.DateTime = New Date(hoy + 1, 6, 30)
            TxtAnio.EditValue = hoy
        End If

    End Sub

    Private Sub CargarAnioActual()

        Dim a As AnioPeriodo = _dal.ObtenerAnioActual(CodigoSeleccionado())

        If a Is Nothing Then
            FechasPorDefecto()
        Else
            TxtAnio.EditValue = a.Anio
            dtpInicio.DateTime = a.Fecha_Inicio
            dtpFinal.DateTime = a.Fecha_Fin
        End If

    End Sub

    Private Sub GenerarVista()

        _periodos = New List(Of PeriodoNomina)
        BtnGuardar.Enabled = False
        gridPeriodos.DataSource = Nothing

        Dim periodo As String = PeriodoSeleccionado()

        LblPeriodoValor.Text = periodo

        If periodo = "" Then Exit Sub

        If Not GeneradorPeriodos.EsSoportado(periodo) Then
            MessageBox.Show("El período de nómina '" & periodo & "' no tiene generador de calendario.",
                            "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Dim anio As Integer = Convert.ToInt32(TxtAnio.EditValue)

        _periodos = GeneradorPeriodos.Generar(periodo, dtpInicio.DateTime.Date, dtpFinal.DateTime.Date, anio)

        Dim existentes As Dictionary(Of Integer, PeriodoNomina) = _dal.ListarExistentes(CodigoSeleccionado(), anio)

        Dim dt As New DataTable
        dt.Columns.Add("Periodo", GetType(Integer))
        dt.Columns.Add("Mes", GetType(String))
        dt.Columns.Add("Inicio", GetType(Date))
        dt.Columns.Add("Final", GetType(Date))
        dt.Columns.Add("Estado", GetType(String))

        For Each p As PeriodoNomina In _periodos

            Dim estado As String = "Nuevo"
            Dim ex As PeriodoNomina = Nothing

            If existentes.TryGetValue(p.Periodo, ex) Then estado = If(ex.TieneNomina, "Con nómina (no se modifica)", "Existe (se actualiza)")

            dt.Rows.Add(p.Periodo, p.Mes, p.Inicio, p.Final, estado)

        Next

        gridPeriodos.DataSource = dt
        gridView1.Columns("Inicio").DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
        gridView1.Columns("Inicio").DisplayFormat.FormatString = "dd/MM/yyyy"
        gridView1.Columns("Final").DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
        gridView1.Columns("Final").DisplayFormat.FormatString = "dd/MM/yyyy"
        gridView1.BestFitColumns()

        BtnGuardar.Enabled = (_periodos.Count > 0)

    End Sub

#End Region

#Region "EVENTOS"

    Private Sub cmbTipoNomina_EditValueChanged(sender As Object, e As EventArgs) Handles cmbTipoNomina.EditValueChanged

        If _cargando OrElse CodigoSeleccionado() = "" Then Exit Sub

        Try
            CargarAnioActual()
            GenerarVista()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnGenerar_Click(sender As Object, e As EventArgs) Handles BtnGenerar.Click

        Try
            If CodigoSeleccionado() = "" Then
                MessageBox.Show("Seleccione el tipo de nómina.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If dtpFinal.DateTime.Date < dtpInicio.DateTime.Date Then
                MessageBox.Show("La fecha final no puede ser menor que la fecha inicial.", "Zeus Nominas",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            GenerarVista()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnGuardar_Click(sender As Object, e As EventArgs) Handles BtnGuardar.Click

        If _periodos.Count = 0 Then Exit Sub

        Dim anio As Integer = Convert.ToInt32(TxtAnio.EditValue)

        If MessageBox.Show("Se activará el año " & anio & " para la nómina " & cmbTipoNomina.Text &
                           " y se grabarán " & _periodos.Count & " períodos. ¿Desea continuar?",
                           "Zeus Nominas", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try

            Cursor = Cursors.WaitCursor

            Dim a As New AnioPeriodo With {
                .Anio = anio,
                .Cod_Tipo_Nomina = CodigoSeleccionado(),
                .Actual = True,
                .Fecha_Inicio = dtpInicio.DateTime.Date,
                .Fecha_Fin = dtpFinal.DateTime.Date
            }

            Dim r As ResumenPeriodos = _dal.Guardar(CodigoSeleccionado(), a, _periodos)

            MessageBox.Show("Calendario guardado: " & r.Insertados & " nuevos, " & r.Actualizados & " actualizados, " &
                            r.Omitidos & " omitidos por tener nómina calculada.",
                            "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

            GenerarVista()

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

#End Region

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
