Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Pestaña Incentivos / Deducciones / Subsidios de la ficha del empleado (V.5.0). Un solo control para
''' las tres: solo cambian la tabla (CsMovimientosDAL), el campo Descripción (subsidios) y Anular (incentivos).
''' Solo UI: los datos y las reglas están en CsMovimientosDAL.
''' </summary>
Public Class UcMovimientosEmpleado
    Inherits UserControl

    Private ReadOnly _tipo As TipoMovimiento
    Private ReadOnly _dal As CsMovimientosDAL
    Private ReadOnly _singular As String

    Private _codEmpleado As Double = 0

    Private cmbTipo As LookUpEdit
    Private txtMonto As SpinEdit
    Private txtVeces As TextEdit
    Private txtDescripcion As TextEdit
    Private WithEvents btnAgregar As SimpleButton
    Private WithEvents btnEliminar As SimpleButton
    Private WithEvents btnAnular As SimpleButton
    Private grid As GridControl
    Private WithEvents vista As GridView

    Public Sub New(tipo As TipoMovimiento)

        _tipo = tipo
        _dal = New CsMovimientosDAL(tipo)
        _singular = tipo.ToString().ToLowerInvariant()

        ConstruirControles()
        Habilitar(False)

    End Sub

#Region "CONSTRUCCION DE LA PANTALLA"

    Private Sub ConstruirControles()

        Dock = DockStyle.Fill

        Dim titulo As String = NombreTipo()

        Dim grupo As New GroupControl With {
            .Text = "Nuevo " & titulo,
            .Location = New Point(10, 8),
            .Size = New Size(290, 250)
        }

        Dim y As Integer = 32

        grupo.Controls.Add(Etiqueta(titulo, 10, y))
        cmbTipo = New LookUpEdit With {.Location = New Point(10, y + 18), .Size = New Size(265, 20)}
        grupo.Controls.Add(cmbTipo)

        y += 50
        grupo.Controls.Add(Etiqueta("Monto", 10, y + 3))
        txtMonto = New SpinEdit With {.Location = New Point(110, y), .Size = New Size(165, 20)}
        txtMonto.Properties.Mask.EditMask = "n2"
        txtMonto.Properties.Mask.UseMaskAsDisplayFormat = True
        txtMonto.Properties.MinValue = 0
        txtMonto.Properties.MaxValue = 100000000
        txtMonto.Properties.Increment = 1
        grupo.Controls.Add(txtMonto)

        y += 30
        grupo.Controls.Add(Etiqueta("Número de veces", 10, y + 3))
        txtVeces = New TextEdit With {.Location = New Point(110, y), .Size = New Size(165, 20)}
        txtVeces.Properties.MaxLength = 4
        txtVeces.ToolTip = "Digite ""n"" para indicar que es de carácter indefinido."
        grupo.Controls.Add(txtVeces)

        If _tipo = TipoMovimiento.Subsidio Then
            y += 30
            grupo.Controls.Add(Etiqueta("Descripción", 10, y + 3))
            txtDescripcion = New TextEdit With {.Location = New Point(110, y), .Size = New Size(165, 20)}
            txtDescripcion.Properties.MaxLength = 25
            grupo.Controls.Add(txtDescripcion)
        End If

        btnAgregar = New SimpleButton With {.Text = "Agregar", .Location = New Point(180, 205), .Size = New Size(95, 30)}
        grupo.Controls.Add(btnAgregar)

        Controls.Add(grupo)

        grid = New GridControl With {.Location = New Point(310, 8), .Size = New Size(470, 300)}
        vista = New GridView(grid)
        grid.MainView = vista
        grid.ViewCollection.Add(vista)

        With vista
            .OptionsBehavior.Editable = False
            .OptionsView.ShowGroupPanel = False
            .OptionsView.EnableAppearanceEvenRow = True
            .OptionsView.ColumnAutoWidth = True
            .OptionsSelection.EnableAppearanceFocusedCell = False
            .FocusRectStyle = DrawFocusRectStyle.RowFocus
        End With

        Controls.Add(grid)

        btnEliminar = New SimpleButton With {.Text = "Eliminar", .Location = New Point(685, 316), .Size = New Size(95, 30), .Enabled = False}
        Controls.Add(btnEliminar)

        If _tipo = TipoMovimiento.Incentivo Then
            btnAnular = New SimpleButton With {.Text = "Anular", .Location = New Point(584, 316), .Size = New Size(95, 30), .Enabled = False}
            Controls.Add(btnAnular)
        End If

    End Sub

    Private Function NombreTipo() As String
        Select Case _tipo
            Case TipoMovimiento.Incentivo : Return "Incentivo"
            Case TipoMovimiento.Deduccion : Return "Deducción"
            Case Else : Return "Subsidio"
        End Select
    End Function

    Private Shared Function Etiqueta(texto As String, x As Integer, y As Integer) As LabelControl
        Return New LabelControl With {.Text = texto, .Location = New Point(x, y), .AutoSizeMode = LabelAutoSizeMode.Default}
    End Function

#End Region

#Region "CARGA"

    ''' <summary>Muestra los movimientos pendientes del empleado; 0 = sin empleado guardado (pestaña inactiva).</summary>
    Public Sub Cargar(codEmpleado As Double)

        _codEmpleado = codEmpleado

        If codEmpleado <= 0 Then
            grid.DataSource = Nothing
            Habilitar(False)
            Exit Sub
        End If

        ConfigurarTipos()
        Habilitar(True)
        RefrescarGrid()

    End Sub

    Private Sub ConfigurarTipos()

        With cmbTipo.Properties
            .Columns.Clear()
            .Columns.Add(New LookUpColumnInfo("Descripcion", ""))
            .ShowHeader = False
            .DataSource = _dal.ListarTipos()
            .ValueMember = "Codigo"
            .DisplayMember = "Descripcion"
            .NullText = ""
        End With

    End Sub

    Private Sub RefrescarGrid()

        grid.DataSource = _dal.ListarPendientes(_codEmpleado)

        vista.PopulateColumns()

        Ocultar("Id")
        Ocultar("Numero")

        Caption("Tipo", If(_tipo = TipoMovimiento.Deduccion, "Deducción", NombreTipo()))
        Caption("Valor", "Monto")
        Caption("NumVez", "Vez")
        Caption("Descripcion", "Descripción")

        If Not IsNothing(vista.Columns("Valor")) Then
            vista.Columns("Valor").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
            vista.Columns("Valor").DisplayFormat.FormatString = "N2"
        End If

        ActualizarBotonesGrid()

    End Sub

    Private Sub Ocultar(columna As String)
        If Not IsNothing(vista.Columns(columna)) Then vista.Columns(columna).Visible = False
    End Sub

    Private Sub Caption(columna As String, texto As String)
        If Not IsNothing(vista.Columns(columna)) Then vista.Columns(columna).Caption = texto
    End Sub

    Private Sub Habilitar(activo As Boolean)

        cmbTipo.Enabled = activo
        txtMonto.Enabled = activo
        txtVeces.Enabled = activo
        If Not IsNothing(txtDescripcion) Then txtDescripcion.Enabled = activo
        btnAgregar.Enabled = activo
        btnEliminar.Enabled = False
        If Not IsNothing(btnAnular) Then btnAnular.Enabled = False

    End Sub

    Private Sub ActualizarBotonesGrid()

        Dim haySeleccion As Boolean = (_codEmpleado > 0 AndAlso vista.FocusedRowHandle >= 0)

        btnEliminar.Enabled = haySeleccion
        If Not IsNothing(btnAnular) Then btnAnular.Enabled = haySeleccion

    End Sub

    Private Sub vista_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) _
        Handles vista.FocusedRowChanged
        ActualizarBotonesGrid()
    End Sub

    Private Sub LimpiarEntrada()

        cmbTipo.EditValue = Nothing
        txtMonto.Value = 0
        txtVeces.Text = ""
        If Not IsNothing(txtDescripcion) Then txtDescripcion.Text = ""

    End Sub

#End Region

#Region "ACCIONES"

    Private Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click

        Try

            If _codEmpleado <= 0 Then Exit Sub

            If IsNothing(cmbTipo.EditValue) OrElse Convert.ToString(cmbTipo.EditValue) = "" Then
                Aviso("No ha seleccionado el tipo de " & NombreTipo().ToLowerInvariant() & ".")
                cmbTipo.Focus()
                Exit Sub
            End If

            Dim veces As String

            Try
                veces = CsMovimientosDAL.NormalizarVeces(txtVeces.Text)
            Catch ex As Exception
                Aviso(ex.Message)
                txtVeces.Focus()
                Exit Sub
            End Try

            If txtMonto.Value <= 0 Then
                Aviso("El monto digitado es erróneo.")
                txtMonto.Focus()
                Exit Sub
            End If

            Dim nomina As Integer = 0

            If _dal.RequiereNominaActiva Then
                nomina = _dal.NominaActiva(_codEmpleado)

                If nomina = 0 Then
                    Aviso("No existe una nómina activa para este empleado.")
                    Exit Sub
                End If
            End If

            _dal.Agregar(_codEmpleado, Convert.ToString(cmbTipo.EditValue), veces, Convert.ToDouble(txtMonto.Value),
                         If(IsNothing(txtDescripcion), "", txtDescripcion.Text), nomina)

            LimpiarEntrada()
            RefrescarGrid()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click

        Try

            If vista.FocusedRowHandle < 0 Then Exit Sub

            Dim numero As Integer = Convert.ToInt32(vista.GetFocusedRowCellValue("Numero"))

            If MessageBox.Show("¿Desea eliminar este " & _singular & " completo (todas sus veces)?", "Zeus Nominas",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Exit Sub
            End If

            _dal.Eliminar(numero)
            RefrescarGrid()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub btnAnular_Click(sender As Object, e As EventArgs) Handles btnAnular.Click

        Try

            If vista.FocusedRowHandle < 0 Then Exit Sub

            If MessageBox.Show("¿Desea anular esta vez del incentivo? Su monto quedará en 0.", "Zeus Nominas",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Exit Sub
            End If

            _dal.Anular(Convert.ToInt32(vista.GetFocusedRowCellValue("Id")),
                        Convert.ToInt32(vista.GetFocusedRowCellValue("Numero")))
            RefrescarGrid()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Shared Sub Aviso(mensaje As String)
        MessageBox.Show(mensaje, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub

#End Region

End Class
