Imports DevExpress.XtraGrid.Views.Grid.ViewInfo

''' <summary>
''' Listado de empleados activos. Los datos vienen de CsEmpleadosDAL.ListarDetallado; la ficha se abre
''' con <see cref="FrmEmpleados"/> (CargarEmpleado / NuevoEmpleado).
''' </summary>
Public Class FrmListadoEmpleados

    Private ReadOnly _dal As New CsEmpleadosDAL

#Region "CARGA"

    Private Sub FrmListadoEmpleados_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EstiloGrid.AplicarBarra(panelBotones)
        Recargar()
    End Sub

    ''' <summary>Vuelve a leer el listado (p. ej. después de guardar un empleado en la ficha).</summary>
    Public Sub Recargar()

        Try

            Cursor = Cursors.WaitCursor

            gridEmpleados.DataSource = _dal.ListarDetallado()
            ConfigurarColumnas()
            ActualizarRegistros()

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub ConfigurarColumnas()

        With gridViewEmpleados

            .OptionsBehavior.Editable = False
            .OptionsView.ShowGroupPanel = False
            .OptionsView.ShowAutoFilterRow = True
            EstiloGrid.Aplicar(gridViewEmpleados)

            Formato("CodEmpleado1", "Código", 80)
            Formato("Nombres", "Nombres", 260)
            Formato("NumCedula", "No. Cédula", 130)
            Formato("NumeroInss", "No. INSS", 90)
            Formato("Departamento", "Departamento", 180)
            Formato("Cargo", "Cargo", 180)
            Formato("SueldoPeriodo", "Sueldo", 100, "n2")
            Formato("Sexo", "Sexo", 50)
            Formato("NumHijos", "Hijos", 50)
            Formato("TarifaHoraria", "Tarifa Horaria", 90, "n2")
            Formato("Dolarizado", "Dolarizado", 80)
            Formato("FechaNacimiento", "Fecha Nacimiento", 110, "dd/MM/yyyy")
            Formato("FechaContrato", "Fecha Contrato", 110, "dd/MM/yyyy")

        End With

    End Sub

    Private Sub Formato(columna As String, titulo As String, ancho As Integer, Optional formatoNumFecha As String = "")

        Dim col As DevExpress.XtraGrid.Columns.GridColumn = gridViewEmpleados.Columns(columna)

        If col Is Nothing Then Exit Sub

        col.Caption = titulo
        col.Width = ancho

        If formatoNumFecha <> "" Then
            col.DisplayFormat.FormatType = If(formatoNumFecha.Contains("/"), DevExpress.Utils.FormatType.DateTime, DevExpress.Utils.FormatType.Numeric)
            col.DisplayFormat.FormatString = formatoNumFecha
        End If

    End Sub

    Private Sub ActualizarRegistros()
        LblRegistros.Text = gridViewEmpleados.RowCount.ToString()
    End Sub

    Private Sub gridViewEmpleados_ColumnFilterChanged(sender As Object, e As EventArgs) Handles gridViewEmpleados.ColumnFilterChanged
        ActualizarRegistros()
    End Sub

#End Region

#Region "ACCIONES"

    ''' <summary>Código visible del empleado seleccionado en la rejilla; "" si no hay.</summary>
    Public Function CodigoSeleccionado() As String

        Dim fila As DataRow = TryCast(gridViewEmpleados.GetFocusedDataRow(), DataRow)

        If fila Is Nothing Then Return ""

        Return Convert.ToString(fila("CodEmpleado1"))

    End Function

    Private Sub AbrirFicha(codigo As String, esNuevo As Boolean)

        Dim frm As FrmEmpleados = My.Forms.FrmEmpleados

        If Not frm.Visible AndAlso Me.MdiParent IsNot Nothing Then frm.MdiParent = Me.MdiParent

        frm.Show()
        frm.Activate()

        If esNuevo Then
            frm.NuevoEmpleado()
        ElseIf codigo <> "" Then
            frm.CargarEmpleado(codigo)
        End If

    End Sub

    Private Sub BtnEditar_Click(sender As Object, e As EventArgs) Handles BtnEditar.Click

        Dim codigo As String = CodigoSeleccionado()

        If codigo = "" Then
            MessageBox.Show("Seleccione un empleado.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        AbrirFicha(codigo, False)

    End Sub

    Private Sub BtnNuevo_Click(sender As Object, e As EventArgs) Handles BtnNuevo.Click
        AbrirFicha("", True)
    End Sub

    Private Sub gridViewEmpleados_DoubleClick(sender As Object, e As EventArgs) Handles gridViewEmpleados.DoubleClick

        Dim hit As GridHitInfo = gridViewEmpleados.CalcHitInfo(gridEmpleados.PointToClient(Control.MousePosition))

        If hit.InRow Then
            Dim codigo As String = CodigoSeleccionado()
            If codigo <> "" Then AbrirFicha(codigo, False)
        End If

    End Sub

    Private Sub gridViewEmpleados_KeyDown(sender As Object, e As KeyEventArgs) Handles gridViewEmpleados.KeyDown

        If e.KeyCode = Keys.Enter AndAlso Not gridViewEmpleados.IsFilterRow(gridViewEmpleados.FocusedRowHandle) Then
            Dim codigo As String = CodigoSeleccionado()
            If codigo <> "" Then AbrirFicha(codigo, False)
            e.Handled = True
        End If

    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs) Handles BtnExcel.Click

        Try
            Dim ruta As String = ExportadorGrid.ExportarAExcel(gridViewEmpleados, "ListadoEmpleados")

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
