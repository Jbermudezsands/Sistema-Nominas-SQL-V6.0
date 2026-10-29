''' <summary>
''' Tasas de cambio diarias. Acceso a datos por CsCambioMonedaDAL; <see cref="TasaSeleccionada"/> deja a otros
''' formularios tomar la tasa elegida en la lista, y <see cref="Recargar"/> refresca la cuadrícula.
''' </summary>
Public Class FrmTasaCambio

    Private ReadOnly _dal As New CsCambioMonedaDAL

    Private _cargando As Boolean = False

#Region "CARGA"

    Private Sub FrmTasaCambio_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EstiloGrid.AplicarBarra(panelBotones)
        Recargar()
        NuevaTasa()
    End Sub

    Public Sub Recargar()

        Try

            Cursor = Cursors.WaitCursor
            _cargando = True

            gridTasas.DataSource = _dal.Listar()
            ConfigurarColumnas()
            MostrarTasaDeHoy()

        Catch ex As Exception
            MostrarError(ex)
        Finally
            _cargando = False
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub ConfigurarColumnas()

        With gridViewTasas

            EstiloGrid.Aplicar(gridViewTasas)

            If .Columns("FechaDia") IsNot Nothing Then
                .Columns("FechaDia").Caption = "Fecha"
                .Columns("FechaDia").DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
                .Columns("FechaDia").DisplayFormat.FormatString = "dd/MM/yyyy"
            End If

            If .Columns("MontoDia") IsNot Nothing Then
                .Columns("MontoDia").Caption = "Tasa C$"
                .Columns("MontoDia").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                .Columns("MontoDia").DisplayFormat.FormatString = "n4"
            End If

        End With

    End Sub

    Private Sub MostrarTasaDeHoy()

        Dim hoy As TasaCambio = _dal.TasaDeHoy()

        If hoy Is Nothing Then
            LblHoy.Appearance.ForeColor = Color.Firebrick
            LblHoy.Text = "La tasa de hoy (" & DateTime.Today.ToString("dd/MM/yyyy") & ") todavía no ha sido grabada."
        Else
            LblHoy.Appearance.ForeColor = Color.DarkGreen
            LblHoy.Text = "Tasa de hoy: " & hoy.Monto.ToString("N4")
        End If

    End Sub

#End Region

#Region "SELECCIÓN"

    ''' <summary>Tasa de la fila elegida en la lista; Nothing si no hay ninguna.</summary>
    Public Function TasaSeleccionada() As TasaCambio

        Dim fila As DataRow = TryCast(gridViewTasas.GetFocusedDataRow(), DataRow)

        If fila Is Nothing Then Return Nothing

        Return New TasaCambio With {
            .Fecha = Convert.ToDateTime(fila("FechaDia")),
            .Monto = If(IsDBNull(fila("MontoDia")), 0D, Convert.ToDecimal(fila("MontoDia")))
        }

    End Function

    Private Sub gridViewTasas_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles gridViewTasas.FocusedRowChanged

        If _cargando Then Exit Sub

        Dim t As TasaCambio = TasaSeleccionada()

        If t Is Nothing Then Exit Sub

        DtpFecha.DateTime = t.Fecha
        SpnMonto.Value = t.Monto

    End Sub

    Private Sub UbicarFecha(fecha As DateTime)

        For i As Integer = 0 To gridViewTasas.DataRowCount - 1
            If Convert.ToDateTime(gridViewTasas.GetRowCellValue(i, "FechaDia")).Date = fecha.Date Then
                gridViewTasas.FocusedRowHandle = i
                Exit For
            End If
        Next

    End Sub

#End Region

#Region "ACCIONES"

    Private Sub NuevaTasa()
        DtpFecha.DateTime = DateTime.Today
        SpnMonto.Value = 0
        SpnMonto.Focus()
    End Sub

    Private Sub BtnNuevo_Click(sender As Object, e As EventArgs) Handles BtnNuevo.Click
        NuevaTasa()
    End Sub

    Private Sub BtnGrabar_Click(sender As Object, e As EventArgs) Handles BtnGrabar.Click

        If DtpFecha.EditValue Is Nothing Then
            MessageBox.Show("Seleccione la fecha.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Try

            Dim t As New TasaCambio With {.Fecha = DtpFecha.DateTime.Date, .Monto = SpnMonto.Value}

            _dal.Guardar(t)
            Recargar()
            UbicarFecha(t.Fecha)

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnEliminar_Click(sender As Object, e As EventArgs) Handles BtnEliminar.Click

        Dim t As TasaCambio = TasaSeleccionada()

        If t Is Nothing Then
            MessageBox.Show("Seleccione la tasa que desea eliminar.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        If MessageBox.Show("¿Está seguro de eliminar la tasa del " & t.Fecha.ToString("dd/MM/yyyy") & "?", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try
            _dal.Eliminar(t.Fecha)
            Recargar()
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
