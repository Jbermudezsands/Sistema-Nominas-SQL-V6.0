Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls

''' <summary>
''' Registro de usuarios (V.5.0 frmClaves). Todo el acceso a datos pasa por CsUsuarioDAL.
''' Un usuario con nivel distinto de 0 no puede ver/editar usuarios de nivel mayor ni asignar un nivel mayor al suyo.
''' </summary>
Public Class FrmUsuarios

    Private ReadOnly _dal As New CsUsuarioDAL

    Private _lista As DataTable
    Private _cargando As Boolean = False
    Private _modoNuevo As Boolean = False
    ' Código del usuario guardado que está en pantalla; 0 = ninguno.
    Private _codigoActual As Integer = 0

#Region "CARGA INICIAL"

    Private Sub FrmUsuarios_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            TxtNombre.MaxLength = 255
            TxtClave.MaxLength = 50
            TxtClave2.MaxLength = 50
            RefrescarLista()
            Limpiar()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub RefrescarLista()

        _lista = _dal.Listar()

        With cmbUsuario.Properties
            .DataSource = _lista
            .ValueMember = "CodUsuario"
            .DisplayMember = "NombreUsuario"
            .TextEditStyle = TextEditStyles.Standard
            .NullText = ""
        End With

    End Sub

    ''' <summary>Nivel del usuario conectado; 0 = sin restricción (regla de la V.5.0).</summary>
    Private Shared Function NivelActual() As Integer
        Dim n As Integer = 0
        Integer.TryParse(New Compañias().Nivel_Acceso, n)
        Return n
    End Function

    Private Shared Function PuedeAdministrar(nivelUsuario As Integer) As Boolean
        Dim miNivel As Integer = NivelActual()
        Return miNivel = 0 OrElse nivelUsuario <= miNivel
    End Function

#End Region

#Region "PANTALLA"

    Private Sub Limpiar()

        _cargando = True

        Try
            SalirModoNuevo()

            _codigoActual = 0
            cmbUsuario.EditValue = Nothing
            TxtNombre.Text = ""
            TxtNivel.EditValue = 0
            TxtClave.Text = ""
            TxtClave2.Text = ""
            Text = "Usuarios"

            ActualizarBotones()
        Finally
            _cargando = False
        End Try

    End Sub

    Private Sub SalirModoNuevo()
        _modoNuevo = False
        btnCancelar.Visible = False
        BtnNuevo.Visible = True
        cmbUsuario.Enabled = True
    End Sub

    Private Sub ActualizarBotones()
        BtnEliminar.Enabled = (_codigoActual > 0)
    End Sub

    Private Sub Cargar(codigo As Integer)

        Dim u As Usuarios = _dal.Buscar(codigo)

        If u Is Nothing Then Exit Sub

        If Not PuedeAdministrar(u.Nivel_Acceso) Then
            MessageBox.Show("No tiene permiso para esta acción.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Limpiar()
            Exit Sub
        End If

        _cargando = True

        Try
            SalirModoNuevo()

            _codigoActual = u.Cod_Usuario
            cmbUsuario.EditValue = u.Cod_Usuario
            TxtNombre.Text = u.Nombre_Usuario
            TxtNivel.EditValue = u.Nivel_Acceso
            TxtClave.Text = u.Clave_Usuario
            TxtClave2.Text = u.Clave_Usuario
            Text = "Usuarios - " & u.Nombre_Usuario

            ActualizarBotones()
        Finally
            _cargando = False
        End Try

    End Sub

    Private Function ExisteEnLista(codigo As Integer) As Boolean
        Return _lista.AsEnumerable().Any(Function(r) Convert.ToInt32(r("CodUsuario")) = codigo)
    End Function

#End Region

#Region "EVENTOS"

    Private Sub cmbUsuario_EditValueChanged(sender As Object, e As EventArgs) Handles cmbUsuario.EditValueChanged

        If _cargando OrElse _modoNuevo Then Exit Sub

        Dim codigo As Integer = 0

        If Not Integer.TryParse(Convert.ToString(cmbUsuario.EditValue), codigo) OrElse codigo = _codigoActual Then Exit Sub

        If ExisteEnLista(codigo) Then
            Try
                Cargar(codigo)
            Catch ex As Exception
                MostrarError(ex)
            End Try
        End If

    End Sub

    Private Sub BtnNuevo_Click(sender As Object, e As EventArgs) Handles BtnNuevo.Click

        Try
            Limpiar()

            _modoNuevo = True
            btnCancelar.Visible = True
            BtnNuevo.Visible = False
            cmbUsuario.Enabled = False
            TxtNombre.Focus()

            ActualizarBotones()
        Catch ex As Exception
            MostrarError(ex)
        End Try

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
            If Convert.ToInt32(_lista.Rows(i)("CodUsuario")) = _codigoActual Then
                indice = i
                Exit For
            End If
        Next

        Dim nuevo As Integer = If(indice = -1, If(delta > 0, 0, _lista.Rows.Count - 1), indice + delta)

        If nuevo < 0 Then
            MessageBox.Show("Está en el primer registro.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If nuevo >= _lista.Rows.Count Then
            MessageBox.Show("Está en el último registro.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Try
            Cargar(Convert.ToInt32(_lista.Rows(nuevo)("CodUsuario")))
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

#End Region

#Region "GRABAR"

    Private Sub BtnGuardar_Click(sender As Object, e As EventArgs) Handles BtnGuardar.Click

        Try

            Cursor = Cursors.WaitCursor

            If Not Validar() Then Exit Sub

            Dim u As New Usuarios With {
                .Cod_Usuario = _codigoActual,
                .Nombre_Usuario = TxtNombre.Text.Trim,
                .Clave_Usuario = TxtClave.Text,
                .Nivel_Acceso = Convert.ToInt32(TxtNivel.EditValue)
            }

            _dal.Guardar(u, _modoNuevo)

            MessageBox.Show("Usuario guardado correctamente.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

            _cargando = True
            Try
                RefrescarLista()
            Finally
                _cargando = False
            End Try

            Cargar(u.Cod_Usuario)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Function Validar() As Boolean

        If Not _modoNuevo AndAlso _codigoActual = 0 Then
            Return Avisar("Seleccione un usuario existente o presione Nuevo.", cmbUsuario)
        End If

        If TxtNombre.Text.Trim = "" Then
            Return Avisar("Debe ingresar el nombre del usuario.", TxtNombre)
        End If

        Dim nivel As Integer = Convert.ToInt32(TxtNivel.EditValue)

        If nivel = 0 Then
            Return Avisar("El nivel del usuario no puede quedar en 0.", TxtNivel)
        End If

        If Not PuedeAdministrar(nivel) Then
            Return Avisar("No puede asignar un nivel mayor al suyo.", TxtNivel)
        End If

        If TxtClave.Text = "" Then
            Return Avisar("Verifique la clave de acceso.", TxtClave)
        End If

        If TxtClave.Text <> TxtClave2.Text Then
            TxtClave2.Text = ""
            Return Avisar("Las claves no coinciden. Repita la clave.", TxtClave2)
        End If

        Return True

    End Function

    Private Shared Function Avisar(mensaje As String, control As Control) As Boolean

        MessageBox.Show(mensaje, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        control.Focus()

        Return False

    End Function

#End Region

#Region "ELIMINAR"

    Private Sub BtnEliminar_Click(sender As Object, e As EventArgs) Handles BtnEliminar.Click

        If _codigoActual = 0 Then
            MessageBox.Show("Seleccione un usuario guardado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("¿Está seguro de eliminar el usuario " & TxtNombre.Text.Trim & "?",
                           "Zeus Nominas", MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

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

            MessageBox.Show("Usuario eliminado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

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
