Imports DevExpress.XtraGrid.Views.Base

''' <summary>
''' Listado de nóminas con su detalle por empleado y la anulación de una nómina cerrada.
''' Acceso a datos por CsNominaDAL. <see cref="Recargar"/> queda público para que otros formularios
''' (p. ej. Activar Nómina) refresquen el listado.
''' </summary>
Public Class FrmListNomina

    Private ReadOnly _dal As New CsNominaDAL

    Private _cargando As Boolean = False

#Region "CARGA"

    Private Sub FrmListNomina_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EstiloGrid.AplicarBarra(panelBotones)
        Recargar()
    End Sub

    Public Sub Recargar()

        Try

            Cursor = Cursors.WaitCursor

            _cargando = True
            Try
                gridNominas.DataSource = _dal.Listar()
                ConfigurarColumnasNominas()
            Finally
                _cargando = False
            End Try

            MostrarNominaSeleccionada()

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub ConfigurarColumnasNominas()

        With gridViewNominas

            .OptionsBehavior.Editable = False
            .OptionsView.ShowGroupPanel = False
            EstiloGrid.Aplicar(gridViewNominas)

            Encabezado(gridViewNominas, "NumNomina", "Nómina", 70)
            Encabezado(gridViewNominas, "CodTipoNomina", "Tipo", 50)
            Encabezado(gridViewNominas, "FechaNominaINI", "Desde", 90, "dd/MM/yyyy")
            Encabezado(gridViewNominas, "FechaNomina", "Hasta", 90, "dd/MM/yyyy")
            Encabezado(gridViewNominas, "Estado", "Estado", 80)

            For Each c As String In {"TotalSalarioBasico", "TotalDestajo", "TotalHorasExtras", "TotalComisiones", "TotalOtrosIngresos",
                                     "TotalIncentivos", "TotalDeducciones", "TotalPrestamo", "TotalMontoINSS", "TotalMontoIR"}
                If .Columns(c) IsNot Nothing Then .Columns(c).Visible = False
            Next

        End With

    End Sub

    Private Sub ConfigurarColumnasDetalle()

        With gridViewDetalle

            .OptionsBehavior.Editable = False
            .OptionsView.ShowGroupPanel = False
            .OptionsView.ShowAutoFilterRow = True
            EstiloGrid.Aplicar(gridViewDetalle)

            Encabezado(gridViewDetalle, "CodEmpleado1", "Código", 70)
            Encabezado(gridViewDetalle, "Nombres", "Nombres", 250)
            Encabezado(gridViewDetalle, "SalarioBasico", "Salario Básico", 100, "n2")
            Encabezado(gridViewDetalle, "Destajo", "Destajo", 90, "n2")
            Encabezado(gridViewDetalle, "HorasExtras", "Horas Extras", 90, "n2")
            Encabezado(gridViewDetalle, "Comisiones", "Comisiones", 90, "n2")
            Encabezado(gridViewDetalle, "OtrosIngresos", "Otros Ingresos", 100, "n2")
            Encabezado(gridViewDetalle, "Incentivos", "Incentivos", 90, "n2")
            Encabezado(gridViewDetalle, "Deducciones", "Deducciones", 90, "n2")
            Encabezado(gridViewDetalle, "Prestamo", "Préstamo", 90, "n2")
            Encabezado(gridViewDetalle, "MontoINSS", "INSS", 80, "n2")
            Encabezado(gridViewDetalle, "MontoIR", "IR", 80, "n2")
            Encabezado(gridViewDetalle, "Vacaciones", "Vacaciones", 90, "n2")
            Encabezado(gridViewDetalle, "INSSPatronal", "INSS Patronal", 100, "n2")
            Encabezado(gridViewDetalle, "Mes13", "Mes 13", 90, "n2")

        End With

    End Sub

    Private Shared Sub Encabezado(vista As DevExpress.XtraGrid.Views.Grid.GridView, columna As String, titulo As String, ancho As Integer,
                                  Optional formato As String = "")

        Dim col As DevExpress.XtraGrid.Columns.GridColumn = vista.Columns(columna)

        If col Is Nothing Then Exit Sub

        col.Caption = titulo
        col.Width = ancho

        If formato <> "" Then
            col.DisplayFormat.FormatType = If(formato.Contains("/"), DevExpress.Utils.FormatType.DateTime, DevExpress.Utils.FormatType.Numeric)
            col.DisplayFormat.FormatString = formato
        End If

    End Sub

#End Region

#Region "SELECCION"

    ''' <summary>Número de la nómina seleccionada; 0 si no hay.</summary>
    Public Function NominaSeleccionada() As Integer

        Dim fila As DataRow = TryCast(gridViewNominas.GetFocusedDataRow(), DataRow)

        If fila Is Nothing Then Return 0

        Return Convert.ToInt32(fila("NumNomina"))

    End Function

    Private Function EstadoSeleccionado() As String

        Dim fila As DataRow = TryCast(gridViewNominas.GetFocusedDataRow(), DataRow)

        If fila Is Nothing Then Return ""

        Return Convert.ToString(fila("Estado"))

    End Function

    Private Sub gridViewNominas_FocusedRowChanged(sender As Object, e As FocusedRowChangedEventArgs) Handles gridViewNominas.FocusedRowChanged
        If Not _cargando Then MostrarNominaSeleccionada()
    End Sub

    Private Sub MostrarNominaSeleccionada()

        Dim fila As DataRow = TryCast(gridViewNominas.GetFocusedDataRow(), DataRow)

        If fila Is Nothing Then
            LimpiarTotales()
            gridDetalle.DataSource = Nothing
            BtnAnular.Enabled = False
            Exit Sub
        End If

        TxtSalBasico.EditValue = Valor(fila, "TotalSalarioBasico")
        TxtDestajo.EditValue = Valor(fila, "TotalDestajo")
        TxtHE.EditValue = Valor(fila, "TotalHorasExtras")
        TxtComisiones.EditValue = Valor(fila, "TotalComisiones")
        TxtOtIngre.EditValue = Valor(fila, "TotalOtrosIngresos")
        TxtIncentivos.EditValue = Valor(fila, "TotalIncentivos")
        TxtDeducciones.EditValue = Valor(fila, "TotalDeducciones")
        TxtPrestamo.EditValue = Valor(fila, "TotalPrestamo")
        TxtInss.EditValue = Valor(fila, "TotalMontoINSS")
        TxtIR.EditValue = Valor(fila, "TotalMontoIR")

        Try
            gridDetalle.DataSource = _dal.ListarDetalle(Convert.ToInt32(fila("NumNomina")))
            ConfigurarColumnasDetalle()
        Catch ex As Exception
            MostrarError(ex)
        End Try

        BtnAnular.Enabled = (Convert.ToString(fila("Estado")) = "Cerrada")

    End Sub

    Private Shared Function Valor(fila As DataRow, columna As String) As Decimal
        Return If(IsDBNull(fila(columna)), 0D, Convert.ToDecimal(fila(columna)))
    End Function

    Private Sub LimpiarTotales()
        For Each t As DevExpress.XtraEditors.TextEdit In {TxtSalBasico, TxtDestajo, TxtHE, TxtComisiones, TxtOtIngre,
                                                           TxtIncentivos, TxtDeducciones, TxtPrestamo, TxtInss, TxtIR}
            t.EditValue = 0D
        Next
    End Sub

#End Region

#Region "ACCIONES"

    Private Sub BtnAnular_Click(sender As Object, e As EventArgs) Handles BtnAnular.Click

        Dim num As Integer = NominaSeleccionada()

        If num = 0 Then
            MessageBox.Show("Seleccione una nómina.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If EstadoSeleccionado() <> "Cerrada" Then
            MessageBox.Show("Solo se pueden anular nóminas cerradas.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("¿Realmente desea anular la nómina " & num & "?" & vbCrLf & vbCrLf &
                           "Se borrará su detalle y se revertirán los movimientos pagados (deducciones, incentivos, comisiones, " &
                           "destajos, horas extra y cuotas de préstamo).",
                           "Zeus Nominas", MessageBoxButtons.YesNo,
                           MessageBoxIcon.Warning) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try

            Cursor = Cursors.WaitCursor

            _dal.Anular(num)

            Recargar()

            MessageBox.Show("La nómina ha sido anulada; los movimientos han sido revertidos.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs) Handles BtnExcel.Click

        Dim num As Integer = NominaSeleccionada()

        If num = 0 Then
            MessageBox.Show("Seleccione una nómina.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Try
            Dim ruta As String = ExportadorGrid.ExportarAExcel(gridViewDetalle, "DetalleNomina" & num)

            If ruta <> "" Then
                MessageBox.Show("Detalle exportado a " & ruta, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
