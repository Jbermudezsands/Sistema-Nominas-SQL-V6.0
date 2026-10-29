Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls

''' <summary>
''' Catálogo de tipos de destajo. Todo el acceso a datos pasa por CsTipoDestajoDAL (consultas parametrizadas).
''' </summary>
Public Class FrmTipoDestajo

    Private ReadOnly _dal As New CsTipoDestajoDAL

    Private _lista As DataTable
    Private _cargando As Boolean = False
    Private _modoNuevo As Boolean = False
    ' Código guardado que está en pantalla; "" = ninguno.
    Private _codigoActual As String = ""

#Region "CARGA INICIAL"

    Private Sub FrmTipoDestajo_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            TxtCodigo.MaxLength = 10
            TxtDestajo.MaxLength = 25
            TxtCuenta.MaxLength = 50
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
            .ValueMember = "CodTipoDestajo"
            .DisplayMember = "CodTipoDestajo"
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
            TxtDestajo.Text = ""
            TxtMonto.EditValue = 0D
            TxtCuenta.Text = ""
            Text = "Tipos de Destajo"

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

        Dim t As TipoDestajo = _dal.Buscar(codigo)

        If t Is Nothing Then Exit Sub

        _cargando = True

        Try
            SalirModoNuevo()

            _codigoActual = t.Cod_Tipo_Destajo
            cmbCodigo.EditValue = t.Cod_Tipo_Destajo
            TxtDestajo.Text = t.Descripcion_Destajo
            TxtMonto.EditValue = t.Monto
            TxtCuenta.Text = t.Cuenta_Contable
            Text = "Tipos de Destajo - " & t.Descripcion_Destajo

            ActualizarBotones()
        Finally
            _cargando = False
        End Try

    End Sub

    Private Function CodigoDigitado() As String
        Return If(_modoNuevo, TxtCodigo.Text, Convert.ToString(cmbCodigo.EditValue)).Trim
    End Function

    Private Function ExisteEnLista(codigo As String) As Boolean
        Return _lista.AsEnumerable().Any(Function(r) String.Equals(Convert.ToString(r("CodTipoDestajo")), codigo, StringComparison.OrdinalIgnoreCase))
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
            If Convert.ToString(_lista.Rows(i)("CodTipoDestajo")) = _codigoActual Then
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
            Cargar(Convert.ToString(_lista.Rows(nuevo)("CodTipoDestajo")))
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

            Dim t As New TipoDestajo With {
                .Cod_Tipo_Destajo = CodigoDigitado(),
                .Descripcion_Destajo = TxtDestajo.Text.Trim,
                .Monto = Convert.ToDecimal(TxtMonto.EditValue),
                .Cuenta_Contable = TxtCuenta.Text.Trim
            }

            _dal.Guardar(t, _modoNuevo)

            MessageBox.Show("Tipo de destajo guardado correctamente.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

            _cargando = True
            Try
                RefrescarLista()
            Finally
                _cargando = False
            End Try

            Cargar(t.Cod_Tipo_Destajo)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Function Validar() As Boolean

        If CodigoDigitado() = "" Then
            Return Avisar("Debe ingresar el código del tipo de destajo.", If(_modoNuevo, CType(TxtCodigo, Control), cmbCodigo))
        End If

        If TxtDestajo.Text.Trim = "" Then
            Return Avisar("Debe ingresar la descripción del tipo de destajo.", TxtDestajo)
        End If

        If Convert.ToDecimal(TxtMonto.EditValue) < 0 Then
            Return Avisar("El monto no puede ser negativo.", TxtMonto)
        End If

        ' Un código que no existe y no se pidió "Nuevo" no se puede grabar.
        If Not _modoNuevo AndAlso _codigoActual = "" Then
            Return Avisar("Seleccione un tipo de destajo existente o presione Nuevo.", cmbCodigo)
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
            MessageBox.Show("Seleccione un tipo de destajo guardado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("¿Está seguro de eliminar el tipo de destajo " & TxtDestajo.Text.Trim & "?",
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

            MessageBox.Show("Tipo de destajo eliminado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

#End Region

#Region "MONTO A TODOS"

    ''' <summary>Equivale al botón Repetir de la V.5.0: deja el monto en pantalla a todos los tipos de destajo.</summary>
    Private Sub BtnAplicarTodos_Click(sender As Object, e As EventArgs) Handles BtnAplicarTodos.Click

        Dim monto As Decimal = Convert.ToDecimal(TxtMonto.EditValue)

        If monto < 0 Then
            Avisar("El monto no puede ser negativo.", TxtMonto)
            Exit Sub
        End If

        If MessageBox.Show("Se cambiará el monto de TODOS los tipos de destajo a " & monto.ToString("N2") & ". ¿Continuar?",
                           "Zeus Nominas", MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try

            Cursor = Cursors.WaitCursor

            Dim n As Integer = _dal.AplicarMontoATodos(monto)

            MessageBox.Show("Proceso terminado. Tipos actualizados: " & n & ".", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

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
