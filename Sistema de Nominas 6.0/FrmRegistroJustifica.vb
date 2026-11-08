Imports DevExpress.XtraEditors

''' <summary>
''' Catálogo de tipos de justificación (LeaveClass). El combo CmbTipo de FrmJustificaciones se llena con
''' estos registros; al grabar o eliminar se avisa con TiposCambiados.
''' </summary>
Public Class FrmRegistroJustifica

    ''' <summary>Se dispara cuando cambia el catálogo (alta, cambio o baja) para que otros formularios recarguen sus combos.</summary>
    Public Shared Event TiposCambiados()

    Private ReadOnly _dal As New CsTipoJustificacionDAL
    Private _lista As DataTable
    Private _cargando As Boolean = False
    Private _modoNuevo As Boolean = False
    Private _codigoActual As Integer = 0

    Private Sub FrmRegistroJustifica_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            TxtNombre.MaxLength = CsTipoJustificacionDAL.LongNombre
            RefrescarLista()
            Limpiar()
        Catch ex As Exception
            MostrarError(ex)
        End Try
    End Sub

    Private Sub RefrescarLista()
        _lista = _dal.Listar()
        cmbCodigo.Properties.DataSource = _lista
    End Sub

    Private Sub Limpiar()
        _cargando = True
        Try
            SalirModoNuevo()
            _codigoActual = 0
            cmbCodigo.EditValue = Nothing
            TxtNombre.Text = ""
            Text = "Tipos de Justificación"
            BtnEliminar.Enabled = False
        Finally
            _cargando = False
        End Try
    End Sub

    Private Sub SalirModoNuevo()
        _modoNuevo = False
        btnCancelar.Visible = False
        BtnNuevo.Visible = True
        TxtCodigo.Visible = False
        cmbCodigo.Visible = True
    End Sub

    Private Sub Cargar(codigo As Integer)
        Dim t As TipoJustificacion = _dal.Buscar(codigo)
        If t Is Nothing Then Exit Sub
        _cargando = True
        Try
            SalirModoNuevo()
            _codigoActual = t.Cod_Tipo
            cmbCodigo.EditValue = t.Cod_Tipo
            TxtNombre.Text = t.Nombre
            Text = "Tipos de Justificación - " & t.Nombre
            BtnEliminar.Enabled = True
        Finally
            _cargando = False
        End Try
    End Sub

    Private Sub cmbCodigo_EditValueChanged(sender As Object, e As EventArgs) Handles cmbCodigo.EditValueChanged
        If _cargando OrElse _modoNuevo OrElse cmbCodigo.EditValue Is Nothing Then Exit Sub
        Dim id As Integer = Convert.ToInt32(cmbCodigo.EditValue)
        If id = _codigoActual Then Exit Sub
        Try
            Cargar(id)
        Catch ex As Exception
            MostrarError(ex)
        End Try
    End Sub

    Private Sub BtnNuevo_Click(sender As Object, e As EventArgs) Handles BtnNuevo.Click
        Limpiar()
        _modoNuevo = True
        btnCancelar.Visible = True
        BtnNuevo.Visible = False
        cmbCodigo.Visible = False
        TxtCodigo.Visible = True
        TxtNombre.Focus()
    End Sub

    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        Limpiar()
    End Sub

    Private Sub BtnAnterior_Click(sender As Object, e As EventArgs) Handles BtnAnterior.Click
        Mover(-1)
    End Sub

    Private Sub BtnSiguiente_Click(sender As Object, e As EventArgs) Handles BtnSiguiente.Click
        Mover(1)
    End Sub

    Private Sub Mover(delta As Integer)

        If _lista Is Nothing OrElse _lista.Rows.Count = 0 Then Exit Sub

        Dim indice As Integer = -1
        For i As Integer = 0 To _lista.Rows.Count - 1
            If Convert.ToInt32(_lista.Rows(i)("Classid")) = _codigoActual Then
                indice = i
                Exit For
            End If
        Next

        Dim nuevo As Integer = If(indice = -1, If(delta > 0, 0, _lista.Rows.Count - 1), indice + delta)

        If nuevo < 0 OrElse nuevo >= _lista.Rows.Count Then
            MessageBox.Show(If(nuevo < 0, "Está en el primer registro.", "Está en el último registro."),
                            "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Try
            Cargar(Convert.ToInt32(_lista.Rows(nuevo)("Classid")))
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

    Private Sub BtnGuardar_Click(sender As Object, e As EventArgs) Handles BtnGuardar.Click

        Try
            Cursor = Cursors.WaitCursor

            If Not _modoNuevo AndAlso _codigoActual = 0 Then
                MessageBox.Show("Seleccione un tipo existente o presione Nuevo.", "Zeus Nominas",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cmbCodigo.Focus()
                Exit Sub
            End If

            If TxtNombre.Text.Trim = "" Then
                MessageBox.Show("Debe ingresar la descripción del tipo de justificación.", "Zeus Nominas",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TxtNombre.Focus()
                Exit Sub
            End If

            Dim id As Integer = _dal.Guardar(New TipoJustificacion With {
                .Cod_Tipo = If(_modoNuevo, 0, _codigoActual),
                .Nombre = TxtNombre.Text.Trim
            })

            MessageBox.Show("Tipo de justificación guardado correctamente.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

            _cargando = True
            Try
                RefrescarLista()
            Finally
                _cargando = False
            End Try

            Cargar(id)
            RaiseEvent TiposCambiados()

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnEliminar_Click(sender As Object, e As EventArgs) Handles BtnEliminar.Click

        If _codigoActual = 0 Then Exit Sub

        If MessageBox.Show("¿Está seguro de eliminar el tipo " & TxtNombre.Text.Trim & "?", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Exit Sub

        Try
            Cursor = Cursors.WaitCursor
            _dal.Eliminar(_codigoActual)

            _cargando = True
            Try
                RefrescarLista()
            Finally
                _cargando = False
            End Try

            Limpiar()
            RaiseEvent TiposCambiados()
            MessageBox.Show("Tipo de justificación eliminado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
