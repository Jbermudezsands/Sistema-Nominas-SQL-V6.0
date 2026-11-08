Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Traslado de historial de pagos de un empleado a otro (V.5.0 FrmTraslados). Solo UI; la lógica está en CsTrasladoDAL.
''' </summary>
Public Class FrmTraslados

    Private ReadOnly _dal As New CsTrasladoDAL

    Private Sub FrmTraslados_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            Dim empleados As DataTable = _dal.ListarEmpleados()

            ConfigurarCombo(CmbOrigen, CmbOrigenView, empleados)
            ConfigurarCombo(CmbDestino, CmbDestinoView, empleados)

            Dim hoy As Date = Date.Today
            DtDesde.DateTime = New Date(hoy.Year, 1, 1)
            DtHasta.DateTime = hoy

            AddHandler CmbOrigen.EditValueChanged, AddressOf CambioOrigen

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Shared Sub ConfigurarCombo(combo As DevExpress.XtraEditors.SearchLookUpEdit, vista As GridView, empleados As DataTable)

        combo.Properties.DataSource = empleados
        combo.Properties.PopupFormSize = New Size(520, 300)

        vista.PopulateColumns(empleados)

        If vista.Columns("CodEmpleado1") IsNot Nothing Then
            vista.Columns("CodEmpleado1").Caption = "Código"
            vista.Columns("CodEmpleado1").Width = 80
        End If
        If vista.Columns("CodEmpleado") IsNot Nothing Then vista.Columns("CodEmpleado").Visible = False
        If vista.Columns("Nombres") IsNot Nothing Then vista.Columns("Nombres").Width = 320
        If vista.Columns("Activo") IsNot Nothing Then vista.Columns("Activo").Width = 50

    End Sub

    Private Sub CambioOrigen(sender As Object, e As EventArgs)
        Grid.DataSource = Nothing
    End Sub

    Private Function Origen() As Decimal?
        If CmbOrigen.EditValue Is Nothing OrElse IsDBNull(CmbOrigen.EditValue) Then Return Nothing
        Return Convert.ToDecimal(CmbOrigen.EditValue)
    End Function

    Private Function Destino() As Decimal?
        If CmbDestino.EditValue Is Nothing OrElse IsDBNull(CmbDestino.EditValue) Then Return Nothing
        Return Convert.ToDecimal(CmbDestino.EditValue)
    End Function

    Private Function RangoValido() As Boolean

        If DtDesde.EditValue Is Nothing OrElse DtHasta.EditValue Is Nothing Then
            MessageBox.Show("Indique el rango de fechas.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If DtHasta.DateTime.Date < DtDesde.DateTime.Date Then
            MessageBox.Show("La fecha final no puede ser menor que la inicial.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True

    End Function

    Private Sub BtnConsultar_Click(sender As Object, e As EventArgs) Handles BtnConsultar.Click

        Try
            If Origen() Is Nothing Then
                MessageBox.Show("Seleccione el empleado de origen.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                CmbOrigen.Focus()
                Exit Sub
            End If

            If Not RangoValido() Then Exit Sub

            Cursor = Cursors.WaitCursor
            Grid.DataSource = _dal.Historial(Origen().Value, DtDesde.DateTime, DtHasta.DateTime)

            For Each col As DevExpress.XtraGrid.Columns.GridColumn In Vista.Columns
                If col.FieldName = "NumNomina" Then
                    col.Caption = "Nómina"
                    col.Width = 70
                ElseIf col.FieldName = "Inicio" OrElse col.FieldName = "Fin" Then
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime
                    col.DisplayFormat.FormatString = "dd/MM/yyyy"
                    col.Width = 90
                ElseIf col.FieldName = "Estado" Then
                    col.Width = 70
                Else
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                    col.DisplayFormat.FormatString = "N2"
                    col.Width = 100
                End If
            Next

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnTrasladar_Click(sender As Object, e As EventArgs) Handles BtnTrasladar.Click

        Try
            If Origen() Is Nothing OrElse Destino() Is Nothing Then
                MessageBox.Show("Seleccione el empleado de origen y el de destino.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If Origen().Value = Destino().Value Then
                MessageBox.Show("El empleado de origen y el de destino deben ser distintos.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            If Not RangoValido() Then Exit Sub

            If MessageBox.Show("Se trasladarán los pagos de las nóminas cerradas entre " & DtDesde.DateTime.ToString("dd/MM/yyyy") &
                               " y " & DtHasta.DateTime.ToString("dd/MM/yyyy") & vbCrLf & vbCrLf &
                               "De: " & CmbOrigen.Text & vbCrLf & "A: " & CmbDestino.Text & vbCrLf & vbCrLf & "¿Desea continuar?",
                               "Zeus Nominas", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then Exit Sub

            Cursor = Cursors.WaitCursor

            Dim movidas As Integer = _dal.Trasladar(Origen().Value, Destino().Value, DtDesde.DateTime, DtHasta.DateTime, New Compañias().Nombre_Usuario)

            MessageBox.Show("Traslado realizado: " & movidas & " pago(s) trasladado(s).", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

            BtnConsultar_Click(Nothing, EventArgs.Empty)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
