Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls

''' <summary>
''' Catálogo de departamentos. Todo el acceso a datos pasa por CsDepartamentoDAL (consultas parametrizadas).
''' </summary>
Public Class FrmDepartamento

    Private ReadOnly _dal As New CsDepartamentoDAL

    Private _lista As DataTable
    Private _cargando As Boolean = False
    Private _modoNuevo As Boolean = False
    ' Código del departamento guardado que está en pantalla; "" = ninguno.
    Private _codigoActual As String = ""

#Region "CARGA INICIAL"

    Private Sub FrmDepartamento_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            TxtCodigo.MaxLength = 10
            TxtDepartamento.MaxLength = 100
            RefrescarLista()
            Limpiar()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub RefrescarLista()

        _lista = _dal.Listar()

        With cmbCodigo.Properties
            .DataSource = _lista
            .ValueMember = "CodDepartamento"
            .DisplayMember = "CodDepartamento"
            .TextEditStyle = TextEditStyles.Standard
            .NullText = ""
        End With

    End Sub

#End Region

#Region "PANTALLA"

    Private Sub Limpiar()

        _cargando = True

        Try
            SalirModoNuevo()

            _codigoActual = ""
            cmbCodigo.EditValue = Nothing
            TxtCodigo.Text = ""
            TxtDepartamento.Text = ""
            Text = "Departamentos"

            ActualizarBotones()
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

    Private Sub ActualizarBotones()
        BtnEliminar.Enabled = (_codigoActual <> "")
    End Sub

    Private Sub Cargar(codigo As String)

        Dim c As Departamento = _dal.Buscar(codigo)

        If c Is Nothing Then Exit Sub

        _cargando = True

        Try
            SalirModoNuevo()

            _codigoActual = c.Cod_Departamento
            cmbCodigo.EditValue = c.Cod_Departamento
            TxtDepartamento.Text = c.Descripcion_Departamento
            Text = "Departamentos - " & c.Descripcion_Departamento

            ActualizarBotones()
        Finally
            _cargando = False
        End Try

    End Sub

    Private Function CodigoDigitado() As String
        Return If(_modoNuevo, TxtCodigo.Text, Convert.ToString(cmbCodigo.EditValue)).Trim
    End Function

    Private Function ExisteEnLista(codigo As String) As Boolean
        Return _lista.AsEnumerable().Any(Function(r) String.Equals(Convert.ToString(r("CodDepartamento")), codigo, StringComparison.OrdinalIgnoreCase))
    End Function

#End Region

#Region "EVENTOS"

    Private Sub cmbCodigo_EditValueChanged(sender As Object, e As EventArgs) Handles cmbCodigo.EditValueChanged

        If _cargando OrElse _modoNuevo Then Exit Sub

        Dim codigo As String = Convert.ToString(cmbCodigo.EditValue).Trim

        If codigo = "" OrElse codigo = _codigoActual Then Exit Sub

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
            cmbCodigo.Visible = False
            TxtCodigo.Visible = True
            TxtCodigo.Text = _dal.ProximoCodigo()
            TxtCodigo.Focus()

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
            If Convert.ToString(_lista.Rows(i)("CodDepartamento")) = _codigoActual Then
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
            Cargar(Convert.ToString(_lista.Rows(nuevo)("CodDepartamento")))
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

            Dim c As New Departamento With {
                .Cod_Departamento = CodigoDigitado(),
                .Descripcion_Departamento = TxtDepartamento.Text.Trim
            }

            _dal.Guardar(c, _modoNuevo)

            MessageBox.Show("Departamento guardado correctamente.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

            _cargando = True
            Try
                RefrescarLista()
            Finally
                _cargando = False
            End Try

            Cargar(c.Cod_Departamento)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Function Validar() As Boolean

        If CodigoDigitado() = "" Then
            Return Avisar("Debe ingresar el código del departamento.", If(_modoNuevo, CType(TxtCodigo, Control), cmbCodigo))
        End If

        If TxtDepartamento.Text.Trim = "" Then
            Return Avisar("Debe ingresar la descripción del departamento.", TxtDepartamento)
        End If


        ' Un código que no existe y no se pidió "Nuevo" no se puede grabar.
        If Not _modoNuevo AndAlso _codigoActual = "" Then
            Return Avisar("Seleccione un departamento existente o presione Nuevo.", cmbCodigo)
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

        If _codigoActual = "" Then
            MessageBox.Show("Seleccione un departamento guardado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("¿Está seguro de eliminar el departamento " & TxtDepartamento.Text.Trim & "?",
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

            MessageBox.Show("Departamento eliminado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

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
