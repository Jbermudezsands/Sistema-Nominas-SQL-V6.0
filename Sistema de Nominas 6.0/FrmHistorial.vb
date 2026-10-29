Imports DevExpress.XtraEditors.Controls

''' <summary>
''' Historial de pagos de un empleado y cambio de su estado (activar, desactivar, copiar perfil, duplicados).
''' Acceso a datos por CsHistorialEmpleadoDAL; <see cref="CargarEmpleado"/> queda público para abrir el
''' formulario ya posicionado en un empleado.
''' </summary>
Public Class FrmHistorial

    Private ReadOnly _dal As New CsHistorialEmpleadoDAL

    Private _empleados As DataTable

#Region "CARGA"

    Private Sub FrmHistorial_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try

            _empleados = _dal.ListarEmpleados()

            gridViewHistorial.OptionsView.EnableAppearanceEvenRow = True

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
            If cmbEmpleadoView.Columns("CodEmpleado") IsNot Nothing Then cmbEmpleadoView.Columns("CodEmpleado").Visible = False
            If cmbEmpleadoView.Columns("Nombres") IsNot Nothing Then cmbEmpleadoView.Columns("Nombres").Width = 300

            ActualizarBotones()

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    ''' <summary>Deja seleccionado al empleado de código visible <paramref name="codigoEmpleado1"/> y muestra su historial.</summary>
    Public Sub CargarEmpleado(codigoEmpleado1 As String)
        cmbEmpleado.EditValue = codigoEmpleado1
    End Sub

    Private Function FilaSeleccionada() As DataRow

        Dim cod As String = Convert.ToString(cmbEmpleado.EditValue).Trim()

        If cod = "" OrElse _empleados Is Nothing Then Return Nothing

        Dim filas() As DataRow = _empleados.Select("CodEmpleado1 = '" & cod.Replace("'", "''") & "'")

        Return If(filas.Length > 0, filas(0), Nothing)

    End Function

    Private Sub cmbEmpleado_EditValueChanged(sender As Object, e As EventArgs) Handles cmbEmpleado.EditValueChanged
        MostrarEmpleado()
    End Sub

    Private Sub MostrarEmpleado()

        Dim fila As DataRow = FilaSeleccionada()

        If fila Is Nothing Then
            LblNombre.Text = ""
            LblEstado.Text = ""
            gridHistorial.DataSource = Nothing
            ActualizarBotones()
            Exit Sub
        End If

        LblNombre.Text = Convert.ToString(fila("Nombres"))
        LblEstado.Text = If(EstaActivo(fila), "Activo", "Inactivo")

        Try

            gridHistorial.DataSource = _dal.Historial(Convert.ToDecimal(fila("CodEmpleado")))
            gridViewHistorial.PopulateColumns()
            gridViewHistorial.BestFitColumns()

            For Each col As DevExpress.XtraGrid.Columns.GridColumn In gridViewHistorial.Columns
                If col.FieldName <> "Año" AndAlso col.FieldName <> "Mes" AndAlso col.FieldName <> "Fecha" Then
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                    col.DisplayFormat.FormatString = "n2"
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n2}")
                ElseIf col.FieldName = "Fecha" Then
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
                    col.DisplayFormat.FormatString = "dd/MM/yyyy"
                End If
            Next

        Catch ex As Exception
            MostrarError(ex)
        End Try

        ActualizarBotones()

    End Sub

    Private Shared Function EstaActivo(fila As DataRow) As Boolean
        Return Not IsDBNull(fila("Activo")) AndAlso Convert.ToBoolean(fila("Activo"))
    End Function

    Private Sub ActualizarBotones()

        Dim fila As DataRow = FilaSeleccionada()
        Dim hay As Boolean = fila IsNot Nothing

        BtnActivar.Enabled = hay AndAlso Not EstaActivo(fila)
        BtnDesactivar.Enabled = hay AndAlso EstaActivo(fila)
        BtnCopiar.Enabled = hay

    End Sub

    ''' <summary>Vuelve a leer la lista de empleados y el historial del seleccionado.</summary>
    Public Sub Recargar()

        Dim cod As Object = cmbEmpleado.EditValue

        _empleados = _dal.ListarEmpleados()
        cmbEmpleado.Properties.DataSource = _empleados
        cmbEmpleado.EditValue = cod

        MostrarEmpleado()

    End Sub

#End Region

#Region "ACCIONES"

    Private Sub BtnActivar_Click(sender As Object, e As EventArgs) Handles BtnActivar.Click

        Dim fila As DataRow = FilaSeleccionada()
        If fila Is Nothing Then Exit Sub

        Try
            _dal.Activar(Convert.ToDecimal(fila("CodEmpleado")))
            Recargar()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnDesactivar_Click(sender As Object, e As EventArgs) Handles BtnDesactivar.Click

        Dim fila As DataRow = FilaSeleccionada()
        If fila Is Nothing Then Exit Sub

        If MessageBox.Show("Se desactivará al empleado y se quitará de la nómina activa. ¿Desea continuar?", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try
            _dal.Desactivar(Convert.ToDecimal(fila("CodEmpleado")))
            Recargar()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnCopiar_Click(sender As Object, e As EventArgs) Handles BtnCopiar.Click

        Dim fila As DataRow = FilaSeleccionada()
        If fila Is Nothing Then Exit Sub

        Dim reutilizar As Boolean =
            MessageBox.Show("¿Desea que el empleado nuevo conserve el código " & Convert.ToString(fila("CodEmpleado1")) & "?" & vbCrLf &
                            "Sí = mismo código; No = código nuevo.", "Zeus Nominas",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes

        Try

            Dim nuevo As String = _dal.CopiarPerfil(Convert.ToDecimal(fila("CodEmpleado")), reutilizar)

            Recargar()
            MessageBox.Show("Perfil copiado. Código del empleado nuevo: " & nuevo, "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnDuplicados_Click(sender As Object, e As EventArgs) Handles BtnDuplicados.Click

        Try

            Dim dt As DataTable = _dal.ListarDuplicados()

            If dt.Rows.Count = 0 Then
                MessageBox.Show("No hay códigos duplicados entre los empleados activos.", "Zeus Nominas",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            gridHistorial.DataSource = dt
            gridViewHistorial.PopulateColumns()
            gridViewHistorial.BestFitColumns()
            LblNombre.Text = "Códigos duplicados: " & dt.Rows.Count
            LblEstado.Text = ""

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

#End Region

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs) Handles BtnExcel.Click

        If gridViewHistorial.DataRowCount = 0 Then
            MessageBox.Show("No hay datos para exportar.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Try
            Dim ruta As String = ExportadorGrid.ExportarAExcel(gridViewHistorial, "Historial")

            If ruta <> "" Then
                MessageBox.Show("Datos exportados a " & ruta, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
