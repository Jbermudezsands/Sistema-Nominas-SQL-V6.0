Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls

''' <summary>
''' Justificaciones de asistencia por empleado. Acceso a datos por CsJustificacionDAL;
''' <see cref="CargarEmpleado"/> queda público para abrir el formulario ya posicionado en un empleado.
''' </summary>
Public Class FrmJustificaciones

    Private ReadOnly _dal As New CsJustificacionDAL

    Private _empleados As DataTable
    Private _cargando As Boolean = False

#Region "CARGA"

    Private Sub FrmJustificaciones_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try

            _empleados = _dal.ListarEmpleados()

            With cmbEmpleado.Properties
                .DataSource = _empleados
                .ValueMember = "CodEmpleado1"
                .DisplayMember = "CodEmpleado1"
                .TextEditStyle = TextEditStyles.Standard
                .NullText = ""
            End With

            If cmbEmpleadoView.Columns("CodEmpleado1") IsNot Nothing Then
                cmbEmpleadoView.Columns("CodEmpleado1").Caption = "Código"
                cmbEmpleadoView.Columns("CodEmpleado1").Width = 80
            End If
            If cmbEmpleadoView.Columns("Nombre") IsNot Nothing Then cmbEmpleadoView.Columns("Nombre").Width = 300

            FrmConsulta.AgregarBotonBuscar(cmbEmpleado, TipoBusqueda.Empleado, Me)

            RecargarTipos()
            Limpiar()

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    ''' <summary>Vuelve a leer los tipos de justificación.</summary>
    Public Sub RecargarTipos()

        CmbTipo.Properties.DataSource = _dal.ListarTipos()

        If CmbTipo.EditValue Is Nothing AndAlso CType(CmbTipo.Properties.DataSource, DataTable).Rows.Count > 0 Then
            CmbTipo.EditValue = CType(CmbTipo.Properties.DataSource, DataTable).Rows(0)("Classid")
        End If

    End Sub

    Private Sub Limpiar()
        DtpInicio.DateTime = DateTime.Today
        DtpFin.DateTime = DateTime.Today
        TmeInicio.Time = New DateTime(2000, 1, 1, 8, 0, 0)
        TmeFin.Time = New DateTime(2000, 1, 1, 17, 0, 0)
        TxtMotivo.Text = ""
    End Sub

    ''' <summary>Deja seleccionado al empleado de código visible <paramref name="codigoEmpleado1"/> y muestra sus justificaciones.</summary>
    Public Sub CargarEmpleado(codigoEmpleado1 As String)
        cmbEmpleado.EditValue = codigoEmpleado1
    End Sub

    Private Function CodigoSeleccionado() As String
        Return Convert.ToString(cmbEmpleado.EditValue).Trim()
    End Function

    Private Sub cmbEmpleado_EditValueChanged(sender As Object, e As EventArgs) Handles cmbEmpleado.EditValueChanged

        Dim codigo As String = CodigoSeleccionado()

        TxtNombre.Text = ""

        If codigo = "" Then
            gridJustificaciones.DataSource = Nothing
            Exit Sub
        End If

        Dim filas() As DataRow = _empleados.Select("CodEmpleado1 = '" & codigo.Replace("'", "''") & "'")

        If filas.Length = 0 Then
            gridJustificaciones.DataSource = Nothing
            Exit Sub
        End If

        TxtNombre.Text = Convert.ToString(filas(0)("Nombre"))
        RecargarLista()

    End Sub

    Public Sub RecargarLista()

        Try

            gridJustificaciones.DataSource = _dal.ListarPorEmpleado(CodigoSeleccionado())

            With gridViewJustificaciones
                .OptionsView.EnableAppearanceEvenRow = True
                If .Columns("Lsh") IsNot Nothing Then .Columns("Lsh").Visible = False
                For Each nombre As String In {"Inicio", "Fin"}
                    If .Columns(nombre) IsNot Nothing Then
                        .Columns(nombre).DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
                        .Columns(nombre).DisplayFormat.FormatString = "dd/MM/yyyy HH:mm"
                        .Columns(nombre).Width = 130
                    End If
                Next
                If .Columns("Tipo") IsNot Nothing Then .Columns("Tipo").Width = 160
            End With

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

#End Region

#Region "ACCIONES"

    Private Sub BtnTipo_Click(sender As Object, e As EventArgs) Handles BtnTipo.Click

        Dim nombre As String = XtraInputBox.Show("Descripción del nuevo tipo de justificación:", "Zeus Nominas", "")

        If String.IsNullOrWhiteSpace(nombre) Then Exit Sub

        Try
            Dim id As Integer = _dal.AgregarTipo(nombre)
            RecargarTipos()
            CmbTipo.EditValue = id
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnAgregar_Click(sender As Object, e As EventArgs) Handles BtnAgregar.Click

        If CodigoSeleccionado() = "" OrElse TxtNombre.Text = "" Then
            MessageBox.Show("Seleccione el empleado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            cmbEmpleado.Focus()
            Exit Sub
        End If

        If CmbTipo.EditValue Is Nothing OrElse IsDBNull(CmbTipo.EditValue) Then
            MessageBox.Show("Seleccione el tipo de justificación (o cree uno con 'Nuevo tipo...').", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            CmbTipo.Focus()
            Exit Sub
        End If

        If DtpInicio.EditValue Is Nothing OrElse DtpFin.EditValue Is Nothing Then
            MessageBox.Show("Digite las fechas de inicio y fin.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Try

            Cursor = Cursors.WaitCursor

            _dal.Agregar(New Justificacion With {
                .Cod_Empleado1 = CodigoSeleccionado(),
                .Cod_Tipo = Convert.ToInt32(CmbTipo.EditValue),
                .Inicio = DtpInicio.DateTime.Date + TmeInicio.Time.TimeOfDay,
                .Fin = DtpFin.DateTime.Date + TmeFin.Time.TimeOfDay,
                .Motivo = TxtMotivo.Text.Trim()})

            RecargarLista()
            TxtMotivo.Text = ""

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnEliminar_Click(sender As Object, e As EventArgs) Handles BtnEliminar.Click

        Dim fila As DataRow = TryCast(gridViewJustificaciones.GetFocusedDataRow(), DataRow)

        If fila Is Nothing Then
            MessageBox.Show("Seleccione la justificación que desea eliminar.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("¿Está seguro de eliminar el registro?", "Zeus Nominas", MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try
            _dal.Eliminar(Convert.ToInt32(fila("Lsh")))
            RecargarLista()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs) Handles BtnExcel.Click

        Try
            Dim ruta As String = ExportadorGrid.ExportarAExcel(gridViewJustificaciones, "Justificaciones" & CodigoSeleccionado())

            If ruta <> "" Then
                MessageBox.Show("Listado exportado a " & ruta, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MostrarError(ex)
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
