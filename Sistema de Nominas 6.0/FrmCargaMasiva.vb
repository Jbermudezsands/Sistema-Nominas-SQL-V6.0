''' <summary>
''' Carga masiva de una deducción o un incentivo (mismo monto) a todos los empleados activos de un tipo de nómina.
''' Acceso a datos por CsCargaMasivaDAL; <see cref="ProcesarDeduccion"/> y <see cref="ProcesarIncentivo"/> quedan
''' públicos para lanzar la carga desde otro formulario.
''' </summary>
Public Class FrmCargaMasiva

    Private ReadOnly _dal As New CsCargaMasivaDAL

    Private _nominas As DataTable

#Region "CARGA"

    Private Sub FrmCargaMasiva_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try

            _nominas = _dal.ListarNominasActivas()
            CmbNomina.Properties.DataSource = _nominas
            CmbDeduccion.Properties.DataSource = _dal.ListarTiposDeduccion()
            CmbIncentivo.Properties.DataSource = _dal.ListarTiposIncentivo()

            If _nominas.Rows.Count > 0 Then CmbNomina.EditValue = _nominas.Rows(0)("CodTipoNomina")

            SeleccionarPrimero(CmbDeduccion, "CodTipoDeduccion")
            SeleccionarPrimero(CmbIncentivo, "CodTipoIncentivo")

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Shared Sub SeleccionarPrimero(combo As DevExpress.XtraEditors.LookUpEdit, columna As String)

        Dim dt As DataTable = TryCast(combo.Properties.DataSource, DataTable)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then combo.EditValue = dt.Rows(0)(columna)

    End Sub

    Private Sub CmbNomina_EditValueChanged(sender As Object, e As EventArgs) Handles CmbNomina.EditValueChanged

        Dim cod As String = CodigoNomina()

        If cod = "" Then
            LblInfo.Text = "No hay nóminas activas."
            Exit Sub
        End If

        Try
            LblInfo.Text = "Nómina N° " & _dal.NominaActivaDelTipo(cod) & " - " & _dal.ContarEmpleados(cod) & " empleados activos."
        Catch ex As Exception
            LblInfo.Text = ""
        End Try

    End Sub

    Private Function CodigoNomina() As String
        Return Convert.ToString(CmbNomina.EditValue).Trim()
    End Function

#End Region

#Region "PROCESO"

    ''' <summary>Agrega la deducción a todos los empleados activos del tipo de nómina. Devuelve cuántos la recibieron (0 si se canceló o falló).</summary>
    Public Function ProcesarDeduccion(codTipoNomina As String, codTipoDeduccion As String, monto As Double) As Integer
        Return Procesar(Function(av) _dal.CargarDeduccion(codTipoNomina, codTipoDeduccion, monto, av), "la deducción")
    End Function

    ''' <summary>Agrega el incentivo a todos los empleados activos del tipo de nómina. Devuelve cuántos lo recibieron (0 si se canceló o falló).</summary>
    Public Function ProcesarIncentivo(codTipoNomina As String, codTipoIncentivo As String, monto As Double) As Integer
        Return Procesar(Function(av) _dal.CargarIncentivo(codTipoNomina, codTipoIncentivo, monto, av), "el incentivo")
    End Function

    Private Function Procesar(carga As Func(Of CsCargaMasivaDAL.AvanceCarga, Integer), que As String) As Integer

        If MessageBox.Show("Se agregará " & que & " a todos los empleados activos de la nómina seleccionada. ¿Desea continuar?",
                           "Zeus Nominas", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Return 0
        End If

        HabilitarBotones(False)

        Try

            Cursor = Cursors.WaitCursor

            Dim cargados As Integer = carga(AddressOf MostrarAvance)

            LblProcesando.Text = "Proceso terminado."
            MessageBox.Show("Proceso terminado. Empleados procesados: " & cargados, "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return cargados

        Catch ex As Exception
            LblProcesando.Text = "El proceso falló; no se guardó ningún cambio."
            MostrarError(ex)
            Return 0
        Finally
            Cursor = Cursors.Default
            HabilitarBotones(True)
        End Try

    End Function

    Private Sub MostrarAvance(procesados As Integer, total As Integer, empleado As String)

        Progreso.Properties.Maximum = Math.Max(total, 1)
        Progreso.Position = procesados
        LblProcesando.Text = If(empleado = "", "", "Empleado: " & empleado)

        If procesados Mod 20 = 0 Then Application.DoEvents()

    End Sub

    Private Sub HabilitarBotones(habilitar As Boolean)
        BtnProcesarDeduccion.Enabled = habilitar
        BtnProcesarIncentivo.Enabled = habilitar
        BtnSalir.Enabled = habilitar
    End Sub

    Private Sub BtnProcesarDeduccion_Click(sender As Object, e As EventArgs) Handles BtnProcesarDeduccion.Click

        If Not Completo(CmbDeduccion, SpnDeduccion, "la deducción") Then Exit Sub

        ProcesarDeduccion(CodigoNomina(), Convert.ToString(CmbDeduccion.EditValue), Convert.ToDouble(SpnDeduccion.Value))

    End Sub

    Private Sub BtnProcesarIncentivo_Click(sender As Object, e As EventArgs) Handles BtnProcesarIncentivo.Click

        If Not Completo(CmbIncentivo, SpnIncentivo, "el incentivo") Then Exit Sub

        ProcesarIncentivo(CodigoNomina(), Convert.ToString(CmbIncentivo.EditValue), Convert.ToDouble(SpnIncentivo.Value))

    End Sub

    Private Function Completo(tipo As DevExpress.XtraEditors.LookUpEdit, monto As DevExpress.XtraEditors.SpinEdit, que As String) As Boolean

        If CodigoNomina() = "" Then
            MessageBox.Show("No hay una nómina activa seleccionada.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return False
        End If

        If tipo.EditValue Is Nothing OrElse Convert.ToString(tipo.EditValue) = "" Then
            MessageBox.Show("Seleccione el tipo de " & que & ".", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            tipo.Focus()
            Return False
        End If

        If monto.Value <= 0 Then
            MessageBox.Show("Digite el monto a procesar.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            monto.Focus()
            Return False
        End If

        Return True

    End Function

#End Region

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
