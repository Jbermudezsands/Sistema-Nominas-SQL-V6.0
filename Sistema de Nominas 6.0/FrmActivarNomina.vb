Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraGrid.Views.Base

''' <summary>
''' Activación de nóminas. Elige un tipo de nómina sin nómina activa, propone el período que toca
''' (CsActivarNominaDAL.SugerirPeriodo) y lo activa. Otros formularios pueden abrirlo y llamar a
''' <see cref="SeleccionarTipoNomina"/>, y escuchar <see cref="NominaActivada"/> para refrescarse.
''' </summary>
Public Class FrmActivarNomina

    Private ReadOnly _dal As New CsActivarNominaDAL

    Private _periodos As DataTable
    Private _cargando As Boolean = False
    Private _codTipoActual As String = ""
    Private _anio As Integer = 0

    ''' <summary>Se dispara cuando se activó una nómina; trae su número.</summary>
    Public Event NominaActivada(numNomina As Integer)

#Region "CARGA"

    Private Sub FrmActivarNomina_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            cmbPeriodo.Properties.Columns.Clear()
            cmbPeriodo.Properties.Columns.Add(New LookUpColumnInfo("Periodo", "Período", 60))
            cmbPeriodo.Properties.ShowHeader = False
            cmbPeriodo.Properties.NullText = ""
            cmbPeriodo.Properties.ValueMember = "Periodo"
            cmbPeriodo.Properties.DisplayMember = "Periodo"

            RecargarTipos()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    ''' <summary>Vuelve a leer los tipos de nómina que se pueden activar.</summary>
    Public Sub RecargarTipos()

        _cargando = True
        Try
            gridTipos.DataSource = _dal.ListarTiposInactivos()

            With gridViewTipos
                .OptionsBehavior.Editable = False
                .OptionsView.ShowGroupPanel = False
                .OptionsView.EnableAppearanceEvenRow = True
                If .Columns("CodTipoNomina") IsNot Nothing Then
                    .Columns("CodTipoNomina").Caption = "Código"
                    .Columns("CodTipoNomina").Width = 70
                End If
                If .Columns("Nomina") IsNot Nothing Then
                    .Columns("Nomina").Caption = "Nómina"
                    .Columns("Nomina").Width = 250
                End If
                If .Columns("Periodo") IsNot Nothing Then
                    .Columns("Periodo").Caption = "Período"
                    .Columns("Periodo").Width = 150
                End If
            End With
        Finally
            _cargando = False
        End Try

        CargarSugerencia()

    End Sub

    ''' <summary>Deja seleccionado un tipo de nómina por su código (si está en la lista de inactivos).</summary>
    Public Sub SeleccionarTipoNomina(codTipoNomina As String)

        For i As Integer = 0 To gridViewTipos.DataRowCount - 1
            If String.Equals(Convert.ToString(gridViewTipos.GetRowCellValue(i, "CodTipoNomina")).Trim, codTipoNomina.Trim,
                             StringComparison.OrdinalIgnoreCase) Then
                gridViewTipos.FocusedRowHandle = i
                Exit Sub
            End If
        Next

    End Sub

    Private Function TipoSeleccionado() As DataRow
        Return TryCast(gridViewTipos.GetFocusedDataRow(), DataRow)
    End Function

#End Region

#Region "PERIODO"

    Private Sub gridViewTipos_FocusedRowChanged(sender As Object, e As FocusedRowChangedEventArgs) Handles gridViewTipos.FocusedRowChanged
        If Not _cargando Then CargarSugerencia()
    End Sub

    Private Sub CargarSugerencia()

        _cargando = True

        Try

            _codTipoActual = ""
            _periodos = Nothing
            cmbPeriodo.Properties.DataSource = Nothing
            cmbPeriodo.EditValue = Nothing
            TxtAno.Text = ""
            dtInicio.EditValue = Nothing
            dtFinal.EditValue = Nothing
            LblFechaLarga.Text = ""
            LblAviso.Text = ""
            BtnActivar.Enabled = False

            Dim tipo As DataRow = TipoSeleccionado()

            If tipo Is Nothing Then
                LblAviso.Text = "No hay tipos de nómina por activar."
                Exit Sub
            End If

            _codTipoActual = Convert.ToString(tipo("CodTipoNomina"))

            Dim sugerido As PeriodoNomina

            Try
                sugerido = _dal.SugerirPeriodo(_codTipoActual)
            Catch ex As Exception
                LblAviso.Text = ex.Message.Replace(vbLf, " ")
                Exit Sub
            End Try

            _anio = sugerido.Anio
            _periodos = _dal.ListarPeriodos(_codTipoActual, _anio)

            TxtAno.Text = _anio.ToString()
            cmbPeriodo.Properties.DataSource = _periodos
            cmbPeriodo.EditValue = sugerido.Periodo

            MostrarFechas(sugerido.Periodo)

            ' Solo en Quincenal y Mensual se puede mover la fecha de pago (igual que la V.5.0).
            Dim periodoNomina As String = Convert.ToString(tipo("Periodo")).Trim
            dtFinal.Properties.ReadOnly = Not (periodoNomina = "Quincenal" OrElse periodoNomina = "Mensual")

            BtnActivar.Enabled = True

        Finally
            _cargando = False
        End Try

    End Sub

    Private Sub cmbPeriodo_EditValueChanged(sender As Object, e As EventArgs) Handles cmbPeriodo.EditValueChanged

        If _cargando OrElse cmbPeriodo.EditValue Is Nothing Then Exit Sub

        MostrarFechas(Convert.ToInt32(cmbPeriodo.EditValue))

    End Sub

    Private Sub MostrarFechas(periodo As Integer)

        If _periodos Is Nothing Then Exit Sub

        Dim fila As DataRow = _periodos.AsEnumerable().FirstOrDefault(Function(r) Convert.ToInt32(r("Periodo")) = periodo)

        If fila Is Nothing Then Exit Sub

        dtInicio.DateTime = Convert.ToDateTime(fila("Inicio"))
        dtFinal.DateTime = Convert.ToDateTime(fila("Final"))
        ActualizarFechaLarga()

    End Sub

    Private Sub dtFinal_EditValueChanged(sender As Object, e As EventArgs) Handles dtFinal.EditValueChanged
        ActualizarFechaLarga()
    End Sub

    Private Sub ActualizarFechaLarga()

        If dtFinal.EditValue Is Nothing Then
            LblFechaLarga.Text = ""
        Else
            LblFechaLarga.Text = "Pago: " & dtFinal.DateTime.ToString("D")
        End If

    End Sub

#End Region

#Region "ACTIVAR"

    Private Sub BtnActivar_Click(sender As Object, e As EventArgs) Handles BtnActivar.Click

        If _codTipoActual = "" OrElse cmbPeriodo.EditValue Is Nothing Then
            MessageBox.Show("Seleccione el tipo de nómina y el período.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If dtInicio.EditValue Is Nothing OrElse dtFinal.EditValue Is Nothing Then
            MessageBox.Show("Las fechas de la nómina no son correctas.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        If dtFinal.DateTime.Date < dtInicio.DateTime.Date Then
            MessageBox.Show("Error en las fechas de la nómina: la final es menor que la inicial.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim periodo As Integer = Convert.ToInt32(cmbPeriodo.EditValue)

        If MessageBox.Show("¿Activar la nómina del tipo " & _codTipoActual & ", período " & periodo & " (" &
                           dtInicio.DateTime.ToString("dd/MM/yyyy") & " al " & dtFinal.DateTime.ToString("dd/MM/yyyy") & ")?",
                           "Zeus Nominas", MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try

            Cursor = Cursors.WaitCursor

            Dim num As Integer = _dal.Activar(_codTipoActual, _anio, periodo, dtInicio.DateTime, dtFinal.DateTime)

            MessageBox.Show("Nómina " & num & " activada.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

            RaiseEvent NominaActivada(num)

            Close()

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
