Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls

''' <summary>
''' Incapacidades de los empleados. El acceso a datos pasa por CsIncapacidadDAL / CsTipoIncapacidadDAL.
''' Otros formularios pueden abrirlo y llamar a <see cref="CargarEmpleado"/> con el código visible del empleado.
''' </summary>
Public Class FrmIncapacidades

    Private ReadOnly _dal As New CsIncapacidadDAL
    Private ReadOnly _dalTipos As New CsTipoIncapacidadDAL

    Private _empleados As DataTable
    Private _cargando As Boolean = False
    ' Llave interna (Empleado.CodEmpleado) y código visible del empleado en pantalla; 0 / "" = ninguno.
    Private _codEmpleado As Decimal = 0D
    Private _codigoActual As String = ""

#Region "CARGA INICIAL"

    Private Sub FrmIncapacidades_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            _cargando = True
            Try
                RefrescarEmpleados()
                RecargarTipos()
                dtFecha.DateTime = Date.Today
            Finally
                _cargando = False
            End Try

            Limpiar()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub RefrescarEmpleados()

        _empleados = _dal.ListarEmpleados()

        With cmbEmpleado.Properties
            .DataSource = _empleados
            .ValueMember = "CodEmpleado1"
            .DisplayMember = "CodEmpleado1"
            .TextEditStyle = TextEditStyles.Standard
            .NullText = ""
        End With

        If cmbEmpleadoView.Columns("CodEmpleado") IsNot Nothing Then cmbEmpleadoView.Columns("CodEmpleado").Visible = False
        If cmbEmpleadoView.Columns("CodEmpleado1") IsNot Nothing Then
            cmbEmpleadoView.Columns("CodEmpleado1").Caption = "Código"
            cmbEmpleadoView.Columns("CodEmpleado1").Width = 80
        End If
        If cmbEmpleadoView.Columns("Nombres") IsNot Nothing Then cmbEmpleadoView.Columns("Nombres").Width = 300

    End Sub

    ''' <summary>Vuelve a leer los tipos de incapacidad (p. ej. después de editarlos en su catálogo).</summary>
    Public Sub RecargarTipos()

        With cmbTipo.Properties
            .Columns.Clear()
            .Columns.Add(New LookUpColumnInfo("Incapacidad", "Incapacidad", 250))
            .DataSource = _dalTipos.Listar()
            .ValueMember = "CodIncapacidad"
            .DisplayMember = "Incapacidad"
            .NullText = ""
            .ShowHeader = False
        End With

    End Sub

#End Region

#Region "PANTALLA"

    Private Sub Limpiar()

        _cargando = True

        Try
            _codEmpleado = 0D
            _codigoActual = ""
            cmbEmpleado.EditValue = Nothing
            TxtNombre.Text = ""
            cmbTipo.EditValue = Nothing
            dtFecha.DateTime = Date.Today
            gridIncapacidades.DataSource = Nothing
            Text = "Incapacidades"

            ActualizarBotones()
        Finally
            _cargando = False
        End Try

    End Sub

    Private Sub ActualizarBotones()
        Dim hay As Boolean = (_codEmpleado <> 0D)
        BtnAgregar.Enabled = hay
        BtnEliminar.Enabled = hay
    End Sub

    ''' <summary>Carga un empleado por su código visible (CodEmpleado1) y muestra sus incapacidades.</summary>
    Public Sub CargarEmpleado(codigo As String)

        Try

            Dim fila As DataRow = _empleados.AsEnumerable().FirstOrDefault(
                Function(r) String.Equals(Convert.ToString(r("CodEmpleado1")).Trim, codigo.Trim, StringComparison.OrdinalIgnoreCase))

            If fila Is Nothing Then
                MessageBox.Show("No existe el empleado " & codigo & ".", "Zeus Nominas",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            _cargando = True
            Try
                _codEmpleado = Convert.ToDecimal(fila("CodEmpleado"))
                _codigoActual = Convert.ToString(fila("CodEmpleado1"))
                cmbEmpleado.EditValue = _codigoActual
                TxtNombre.Text = Convert.ToString(fila("Nombres"))
                Text = "Incapacidades - " & TxtNombre.Text
            Finally
                _cargando = False
            End Try

            RefrescarIncapacidades()
            ActualizarBotones()

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub RefrescarIncapacidades()

        gridIncapacidades.DataSource = _dal.ListarPorEmpleado(_codEmpleado)

        With gridViewIncapacidades
            .OptionsView.EnableAppearanceEvenRow = True
            If .Columns("CodIncapacidad") IsNot Nothing Then
                .Columns("CodIncapacidad").Caption = "Código"
                .Columns("CodIncapacidad").Width = 70
            End If
            If .Columns("Incapacidad") IsNot Nothing Then
                .Columns("Incapacidad").Caption = "Descripción Incapacidad"
                .Columns("Incapacidad").Width = 380
            End If
            If .Columns("FechaIncapacidad") IsNot Nothing Then
                .Columns("FechaIncapacidad").Caption = "Fecha Incapacidad"
                .Columns("FechaIncapacidad").DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
                .Columns("FechaIncapacidad").DisplayFormat.FormatString = "dd/MM/yyyy"
            End If
        End With

    End Sub

#End Region

#Region "EVENTOS"

    Private Sub cmbEmpleado_EditValueChanged(sender As Object, e As EventArgs) Handles cmbEmpleado.EditValueChanged

        If _cargando Then Exit Sub

        Dim codigo As String = Convert.ToString(cmbEmpleado.EditValue).Trim

        If codigo = "" OrElse codigo = _codigoActual Then Exit Sub

        If _empleados.AsEnumerable().Any(Function(r) String.Equals(Convert.ToString(r("CodEmpleado1")).Trim, codigo, StringComparison.OrdinalIgnoreCase)) Then
            CargarEmpleado(codigo)
        End If

    End Sub

    Private Sub BtnAnterior_Click(sender As Object, e As EventArgs) Handles BtnAnterior.Click
        Mover(-1)
    End Sub

    Private Sub BtnSiguiente_Click(sender As Object, e As EventArgs) Handles BtnSiguiente.Click
        Mover(1)
    End Sub

    Private Sub Mover(delta As Integer)

        If _empleados Is Nothing OrElse _empleados.Rows.Count = 0 Then Exit Sub

        Dim indice As Integer = -1

        For i As Integer = 0 To _empleados.Rows.Count - 1
            If Convert.ToString(_empleados.Rows(i)("CodEmpleado1")) = _codigoActual Then
                indice = i
                Exit For
            End If
        Next

        Dim nuevo As Integer = If(indice = -1, If(delta > 0, 0, _empleados.Rows.Count - 1), indice + delta)

        If nuevo < 0 Then
            MessageBox.Show("Está en el primer registro.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If nuevo >= _empleados.Rows.Count Then
            MessageBox.Show("Está en el último registro.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        CargarEmpleado(Convert.ToString(_empleados.Rows(nuevo)("CodEmpleado1")))

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

#End Region

#Region "AGREGAR / ELIMINAR"

    Private Sub BtnAgregar_Click(sender As Object, e As EventArgs) Handles BtnAgregar.Click

        Try

            If _codEmpleado = 0D Then
                MessageBox.Show("Seleccione un empleado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbEmpleado.Focus()
                Exit Sub
            End If

            If cmbTipo.EditValue Is Nothing OrElse Convert.ToString(cmbTipo.EditValue).Trim = "" Then
                MessageBox.Show("Seleccione el tipo de incapacidad.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbTipo.Focus()
                Exit Sub
            End If

            ' La columna es smalldatetime: 01/01/1900 a 06/06/2079.
            If dtFecha.DateTime.Year < 1900 OrElse dtFecha.DateTime.Year > 2078 Then
                MessageBox.Show("La fecha de la incapacidad no es válida.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                dtFecha.Focus()
                Exit Sub
            End If

            Cursor = Cursors.WaitCursor

            _dal.Agregar(New IncapacidadEmpleado With {
                .Cod_Empleado = _codEmpleado,
                .Cod_Incapacidad = Convert.ToString(cmbTipo.EditValue),
                .Fecha = dtFecha.DateTime.Date
            })

            RefrescarIncapacidades()

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnEliminar_Click(sender As Object, e As EventArgs) Handles BtnEliminar.Click

        Dim fila As DataRow = TryCast(gridViewIncapacidades.GetFocusedDataRow(), DataRow)

        If fila Is Nothing Then
            MessageBox.Show("Seleccione la incapacidad que desea eliminar.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("¿Está seguro de eliminar la incapacidad de " & TxtNombre.Text & "?",
                           "Zeus Nominas", MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try

            Cursor = Cursors.WaitCursor

            _dal.Eliminar(New IncapacidadEmpleado With {
                .Cod_Empleado = _codEmpleado,
                .Cod_Incapacidad = Convert.ToString(fila("CodIncapacidad")),
                .Fecha = Convert.ToDateTime(fila("FechaIncapacidad"))
            })

            RefrescarIncapacidades()

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

#End Region

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
