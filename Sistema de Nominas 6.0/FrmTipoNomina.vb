Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls

''' <summary>
''' Catálogo de tipos de nómina. Todo el acceso a datos pasa por CsTipoNominaDAL (consultas parametrizadas).
''' </summary>
Public Class FrmTipoNomina

    Private ReadOnly _dal As New CsTipoNominaDAL

    Private _lista As DataTable
    Private _cargando As Boolean = False
    Private _modoNuevo As Boolean = False
    ' Código del tipo de nómina guardado que está en pantalla; "" = ninguno.
    Private _codigoActual As String = ""

#Region "CARGA INICIAL"

    Private Sub FrmTipoNomina_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            TxtCodigo.MaxLength = 2
            TxtDescripcion.MaxLength = 50
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
            .ValueMember = "CodTipoNomina"
            .DisplayMember = "CodTipoNomina"
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
            MostrarEntidad(New TipoNomina)
            Text = "Tipos de Nómina"

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

    Private Sub MostrarEntidad(t As TipoNomina)

        TxtDescripcion.Text = t.Descripcion_Nomina
        CmbPeriodo.SelectedIndex = CmbPeriodo.Properties.Items.IndexOf(t.Periodo)
        TxtTarifa.EditValue = Convert.ToDecimal(t.Tarifa_Horaria)
        TxtHoras.EditValue = Convert.ToDecimal(t.Horas)
        RgTipoPago.EditValue = t.Tipo_Pago
        RgMoneda.EditValue = t.Moneda
        ChkMantValor.Checked = t.Mantenimiento_Valor
        RgInss.EditValue = t.Inss_Por_Porcentaje
        TxtInssLaboral.EditValue = t.Tasa_Inss
        TxtInssPatronal.EditValue = t.Tasa_Inss_Patronal
        RgIr.EditValue = t.Ir_Por_Porcentaje
        TxtIr.EditValue = t.Tasa_Ir
        ChkEmpleadoCero.Checked = t.Mostrar_Empleado_Cero
        ChkConstruccion.Checked = t.Empleado_Construccion
        ChkIrUltima.Checked = t.Ir_Ultima_Semana
        ChkHorasTrabajadas.Checked = t.Calcular_Hora_Trabajada

        AjustarVisibilidad()

    End Sub

    ''' <summary>Los porcentajes solo aplican si el cálculo es por porcentaje; el mantenimiento de valor solo en córdobas.</summary>
    Private Sub AjustarVisibilidad()

        Dim inss As Boolean = Convert.ToBoolean(RgInss.EditValue)
        Dim ir As Boolean = Convert.ToBoolean(RgIr.EditValue)

        lblInssLab.Visible = inss
        TxtInssLaboral.Visible = inss
        lblInssPat.Visible = inss
        TxtInssPatronal.Visible = inss
        lblIrPct.Visible = ir
        TxtIr.Visible = ir

        Dim cordobas As Boolean = (Convert.ToString(RgMoneda.EditValue) = "CS")

        ChkMantValor.Enabled = cordobas

        If Not cordobas Then ChkMantValor.Checked = False

    End Sub

    Private Sub Cargar(codigo As String)

        Dim t As TipoNomina = _dal.Buscar(codigo)

        If t Is Nothing Then Exit Sub

        _cargando = True

        Try
            SalirModoNuevo()

            _codigoActual = t.Cod_Tipo_Nomina
            cmbCodigo.EditValue = t.Cod_Tipo_Nomina
            MostrarEntidad(t)
            Text = "Tipos de Nómina - " & t.Descripcion_Nomina

            ActualizarBotones()
        Finally
            _cargando = False
        End Try

    End Sub

    Private Function CodigoDigitado() As String
        Return If(_modoNuevo, TxtCodigo.Text, Convert.ToString(cmbCodigo.EditValue)).Trim
    End Function

    Private Function ExisteEnLista(codigo As String) As Boolean
        Return _lista.AsEnumerable().Any(Function(r) String.Equals(Convert.ToString(r("CodTipoNomina")), codigo, StringComparison.OrdinalIgnoreCase))
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

    Private Sub Opciones_EditValueChanged(sender As Object, e As EventArgs) Handles RgInss.EditValueChanged, RgIr.EditValueChanged, RgMoneda.EditValueChanged
        AjustarVisibilidad()
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
            If Convert.ToString(_lista.Rows(i)("CodTipoNomina")) = _codigoActual Then
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
            Cargar(Convert.ToString(_lista.Rows(nuevo)("CodTipoNomina")))
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

            Dim t As New TipoNomina With {
                .Cod_Tipo_Nomina = CodigoDigitado(),
                .Descripcion_Nomina = TxtDescripcion.Text.Trim,
                .Periodo = Convert.ToString(CmbPeriodo.SelectedItem),
                .Tipo_Pago = Convert.ToString(RgTipoPago.EditValue),
                .Moneda = Convert.ToString(RgMoneda.EditValue),
                .Mantenimiento_Valor = ChkMantValor.Checked,
                .Inss_Por_Porcentaje = Convert.ToBoolean(RgInss.EditValue),
                .Tasa_Inss = Convert.ToDecimal(TxtInssLaboral.EditValue),
                .Tasa_Inss_Patronal = Convert.ToDecimal(TxtInssPatronal.EditValue),
                .Ir_Por_Porcentaje = Convert.ToBoolean(RgIr.EditValue),
                .Tasa_Ir = Convert.ToDecimal(TxtIr.EditValue),
                .Tarifa_Horaria = Convert.ToDouble(TxtTarifa.EditValue),
                .Horas = Convert.ToDouble(TxtHoras.EditValue),
                .Calcular_Hora_Trabajada = ChkHorasTrabajadas.Checked,
                .Ir_Ultima_Semana = ChkIrUltima.Checked,
                .Empleado_Construccion = ChkConstruccion.Checked,
                .Mostrar_Empleado_Cero = ChkEmpleadoCero.Checked
            }

            _dal.Guardar(t, _modoNuevo)

            MessageBox.Show("Tipo de nómina guardado correctamente.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

            _cargando = True
            Try
                RefrescarLista()
            Finally
                _cargando = False
            End Try

            Cargar(t.Cod_Tipo_Nomina)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Function Validar() As Boolean

        If CodigoDigitado() = "" Then
            Return Avisar("Debe ingresar el código del tipo de nómina.", If(_modoNuevo, CType(TxtCodigo, Control), cmbCodigo))
        End If

        If TxtDescripcion.Text.Trim = "" Then
            Return Avisar("Debe ingresar la descripción de la nómina.", TxtDescripcion)
        End If

        If CmbPeriodo.SelectedIndex < 0 Then
            Return Avisar("Debe seleccionar el período de la nómina.", CmbPeriodo)
        End If

        If Convert.ToDecimal(TxtHoras.EditValue) <= 0 Then
            Return Avisar("Las horas laboradas deben ser mayores que cero.", TxtHoras)
        End If

        If Convert.ToDecimal(TxtTarifa.EditValue) < 0 Then
            Return Avisar("La tarifa básica no puede ser negativa.", TxtTarifa)
        End If

        If Convert.ToBoolean(RgInss.EditValue) AndAlso
           (Convert.ToDecimal(TxtInssLaboral.EditValue) < 0 OrElse Convert.ToDecimal(TxtInssPatronal.EditValue) < 0) Then
            Return Avisar("Los porcentajes del INSS no pueden ser negativos.", TxtInssLaboral)
        End If

        If Convert.ToBoolean(RgIr.EditValue) AndAlso Convert.ToDecimal(TxtIr.EditValue) < 0 Then
            Return Avisar("El porcentaje del IR no puede ser negativo.", TxtIr)
        End If

        ' Un código que no existe y no se pidió "Nuevo" no se puede grabar.
        If Not _modoNuevo AndAlso _codigoActual = "" Then
            Return Avisar("Seleccione un tipo de nómina existente o presione Nuevo.", cmbCodigo)
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
            MessageBox.Show("Seleccione un tipo de nómina guardado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("¿Está seguro de eliminar el tipo de nómina " & TxtDescripcion.Text.Trim & "?",
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

            MessageBox.Show("Tipo de nómina eliminado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

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
