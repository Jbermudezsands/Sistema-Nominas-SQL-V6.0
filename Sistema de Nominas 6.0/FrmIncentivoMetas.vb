Imports DevExpress.XtraGrid.Columns

''' <summary>
''' Incentivo por metas (V.5.0 FrmIncentivoMetas): configuración de puntualidad/séptimo/viáticos, tabla de metas
''' e incentivo por producción. Solo UI; los datos pasan por CsIncentivoMetasDAL.
''' </summary>
Public Class FrmIncentivoMetas

    Private ReadOnly _dal As New CsIncentivoMetasDAL
    Private _metas As DataTable

    Private Sub FrmIncentivoMetas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            CargarConfiguracion()
            CargarMetas()
            CmbDepartamento.Properties.DataSource = _dal.ListarDepartamentos()
            CargarProduccion()
        Catch ex As Exception
            MostrarError(ex)
        End Try
    End Sub

#Region "CONFIGURACION"

    Private Sub CargarConfiguracion()
        Dim c As ConfiguracionIncentivo = _dal.ObtenerConfiguracion()
        TxtHoras.Value = c.Horas_Puntualidad
        TxtMonto.Value = c.Monto_Puntualidad
        TxtHorasSeptimo.Value = Convert.ToDecimal(c.Horas_Septimo)
        TxtHorasBasico.Value = c.Horas_Basico
        TxtMontoViatico.Value = Convert.ToDecimal(c.Monto_Viaticos)
    End Sub

    Private Sub BtnGrabarConfig_Click(sender As Object, e As EventArgs) Handles BtnGrabarConfig.Click
        Try
            Cursor = Cursors.WaitCursor
            _dal.GuardarConfiguracion(New ConfiguracionIncentivo With {
                .Horas_Puntualidad = TxtHoras.Value,
                .Monto_Puntualidad = TxtMonto.Value,
                .Horas_Septimo = Convert.ToDouble(TxtHorasSeptimo.Value),
                .Horas_Basico = TxtHorasBasico.Value,
                .Monto_Viaticos = Convert.ToDouble(TxtMontoViatico.Value)
            })
            MessageBox.Show("La configuración ha sido guardada.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

#End Region

#Region "METAS"

    Private Sub CargarMetas()

        _metas = _dal.ListarMetas()
        GridMetas.DataSource = _metas

        Dim titulos As String() = {"CodProceso|Proceso", "CodReferencia|Referencia", "Meta|Meta",
                                   "Rango1|Rango 1", "Monto1|Monto 1", "Rango2|Rango 2", "Monto2|Monto 2",
                                   "Rango3|Rango 3", "Monto3|Monto 3", "Rango4|Rango 4", "Monto4|Monto 4"}
        Dim posicion As Integer = 0

        For Each t As String In titulos
            Dim partes() As String = t.Split("|"c)
            Dim col As GridColumn = VistaMetas.Columns(partes(0))
            If col Is Nothing Then Continue For
            col.Caption = partes(1)
            col.VisibleIndex = posicion
            col.Width = 90
            If partes(0) <> "CodProceso" Then
                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                col.DisplayFormat.FormatString = If(partes(0).StartsWith("Monto"), "N2", "N2")
            End If
            posicion += 1
        Next

    End Sub

    Private Sub BtnGrabarMetas_Click(sender As Object, e As EventArgs) Handles BtnGrabarMetas.Click

        Try
            VistaMetas.CloseEditor()
            VistaMetas.UpdateCurrentRow()

            Dim problema As String = CsIncentivoMetasDAL.ValidarMetas(_metas)
            If problema <> "" Then
                MessageBox.Show(problema, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            Cursor = Cursors.WaitCursor
            _dal.GuardarMetas(_metas)
            CargarMetas()
            MessageBox.Show("Las metas han sido guardadas.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnEliminarMeta_Click(sender As Object, e As EventArgs) Handles BtnEliminarMeta.Click

        If VistaMetas.FocusedRowHandle < 0 Then
            MessageBox.Show("Seleccione la fila que desea eliminar.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("¿Está seguro de eliminar la fila seleccionada? Se aplicará al presionar Grabar.", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            VistaMetas.DeleteRow(VistaMetas.FocusedRowHandle)
        End If

    End Sub

#End Region

#Region "PRODUCCION"

    Private Sub CargarProduccion()

        GridProduccion.DataSource = _dal.ListarProduccion()

        If VistaProduccion.Columns("NumeroIncentivo") IsNot Nothing Then VistaProduccion.Columns("NumeroIncentivo").Visible = False
        If VistaProduccion.Columns("CodDepartamento") IsNot Nothing Then VistaProduccion.Columns("CodDepartamento").Caption = "Código"
        If VistaProduccion.Columns("Departamento") IsNot Nothing Then VistaProduccion.Columns("Departamento").Caption = "Departamento"
        If VistaProduccion.Columns("CantidadHoras") IsNot Nothing Then VistaProduccion.Columns("CantidadHoras").Caption = "Cantidad de horas"
        If VistaProduccion.Columns("Valor") IsNot Nothing Then
            VistaProduccion.Columns("Valor").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
            VistaProduccion.Columns("Valor").DisplayFormat.FormatString = "N2"
        End If

    End Sub

    Private Sub BtnAgregar_Click(sender As Object, e As EventArgs) Handles BtnAgregar.Click

        Try
            If CmbDepartamento.EditValue Is Nothing Then
                MessageBox.Show("Seleccione el departamento.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                CmbDepartamento.Focus()
                Exit Sub
            End If

            Dim horas As Decimal
            If Not Decimal.TryParse(TxtCantidadHoras.Text.Trim, horas) OrElse horas < 0 Then
                MessageBox.Show("La cantidad de horas debe ser un número válido.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                TxtCantidadHoras.Focus()
                Exit Sub
            End If

            _dal.AgregarProduccion(Convert.ToString(CmbDepartamento.EditValue), TxtCantidadHoras.Text.Trim, Convert.ToDouble(TxtValor.Value))

            TxtCantidadHoras.Text = ""
            TxtValor.Value = 0
            CargarProduccion()

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnEliminarProd_Click(sender As Object, e As EventArgs) Handles BtnEliminarProd.Click

        Dim fila As DataRow = TryCast(VistaProduccion.GetFocusedDataRow(), DataRow)

        If fila Is Nothing Then
            MessageBox.Show("Seleccione la fila que desea eliminar.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("¿Está seguro de eliminar el registro seleccionado?", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Exit Sub

        Try
            _dal.EliminarProduccion(Convert.ToInt32(fila("NumeroIncentivo")))
            CargarProduccion()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

#End Region

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
