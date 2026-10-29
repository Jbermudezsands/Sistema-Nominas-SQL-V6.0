Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Pestaña Préstamo de la ficha del empleado (V.5.0): genera la libreta, la muestra y cancela el préstamo.
''' Solo UI: el cálculo está en CalculadoraPrestamo y los datos en CsPrestamoDAL.
''' </summary>
Public Class UcPrestamoEmpleado
    Inherits UserControl

    Private ReadOnly _dal As New CsPrestamoDAL

    Private _codEmpleado As Double = 0

    Private WithEvents txtMonto As SpinEdit
    Private txtCuotas As SpinEdit
    Private txtInteres As SpinEdit
    Private txtSaldo As TextEdit
    Private optMoneda As RadioGroup
    Private chkCuotasIguales As CheckEdit
    Private txtCuentaDebito As TextEdit
    Private txtCuentaCredito As TextEdit
    Private WithEvents btnGenerar As SimpleButton
    Private WithEvents btnCancelarPrestamo As SimpleButton
    Private lblEstado As LabelControl
    Private grid As GridControl
    Private vista As GridView

    Public Sub New()

        ConstruirControles()
        Habilitar(False)

    End Sub

#Region "CONSTRUCCION DE LA PANTALLA"

    Private Sub ConstruirControles()

        Dock = DockStyle.Fill

        Dim grupo As New GroupControl With {
            .Text = "Nuevo préstamo",
            .Location = New Point(10, 8),
            .Size = New Size(340, 300)
        }

        Dim y As Integer = 32

        grupo.Controls.Add(Etiqueta("Monto", 10, y + 3))
        txtMonto = Numerico(120, y, 200, "n2", 100000000)
        grupo.Controls.Add(txtMonto)

        y += 28
        grupo.Controls.Add(Etiqueta("Número de cuotas", 10, y + 3))
        txtCuotas = Numerico(120, y, 90, "d0", 255)
        grupo.Controls.Add(txtCuotas)

        y += 28
        grupo.Controls.Add(Etiqueta("Interés % por cuota", 10, y + 3))
        txtInteres = Numerico(120, y, 90, "n2", 100)
        grupo.Controls.Add(txtInteres)

        y += 28
        grupo.Controls.Add(Etiqueta("Saldo", 10, y + 3))
        txtSaldo = New TextEdit With {.Location = New Point(120, y), .Size = New Size(200, 20), .Enabled = False}
        grupo.Controls.Add(txtSaldo)

        y += 28
        optMoneda = New RadioGroup With {.Location = New Point(10, y), .Size = New Size(140, 24)}
        optMoneda.Properties.Columns = 2
        optMoneda.Properties.Items.Add(New DevExpress.XtraEditors.Controls.RadioGroupItem("CS", "C$"))
        optMoneda.Properties.Items.Add(New DevExpress.XtraEditors.Controls.RadioGroupItem("US", "US$"))
        optMoneda.EditValue = "CS"
        grupo.Controls.Add(optMoneda)

        chkCuotasIguales = New CheckEdit With {.Text = "Cuotas iguales", .Location = New Point(170, y + 2), .Size = New Size(150, 20), .Checked = True}
        grupo.Controls.Add(chkCuotasIguales)

        y += 34
        grupo.Controls.Add(Etiqueta("Cuenta débito", 10, y + 3))
        txtCuentaDebito = New TextEdit With {.Location = New Point(120, y), .Size = New Size(200, 20)}
        txtCuentaDebito.Properties.MaxLength = 25
        grupo.Controls.Add(txtCuentaDebito)

        y += 28
        grupo.Controls.Add(Etiqueta("Cuenta crédito", 10, y + 3))
        txtCuentaCredito = New TextEdit With {.Location = New Point(120, y), .Size = New Size(200, 20)}
        txtCuentaCredito.Properties.MaxLength = 25
        grupo.Controls.Add(txtCuentaCredito)

        btnGenerar = New SimpleButton With {.Text = "Generar préstamo", .Location = New Point(205, 255), .Size = New Size(120, 30)}
        grupo.Controls.Add(btnGenerar)

        Controls.Add(grupo)

        lblEstado = New LabelControl With {.Location = New Point(365, 12), .Text = ""}
        Controls.Add(lblEstado)

        grid = New GridControl With {.Location = New Point(360, 34), .Size = New Size(420, 274)}
        vista = New GridView(grid)
        grid.MainView = vista
        grid.ViewCollection.Add(vista)

        With vista
            .OptionsBehavior.Editable = False
            .OptionsView.ShowGroupPanel = False
            .OptionsView.EnableAppearanceEvenRow = True
            .OptionsView.ColumnAutoWidth = True
            .FocusRectStyle = DrawFocusRectStyle.RowFocus
        End With

        Controls.Add(grid)

        btnCancelarPrestamo = New SimpleButton With {.Text = "Cancelar préstamo", .Location = New Point(660, 316), .Size = New Size(120, 30), .Enabled = False}
        Controls.Add(btnCancelarPrestamo)

    End Sub

    Private Shared Function Etiqueta(texto As String, x As Integer, y As Integer) As LabelControl
        Return New LabelControl With {.Text = texto, .Location = New Point(x, y)}
    End Function

    Private Shared Function Numerico(x As Integer, y As Integer, ancho As Integer, mascara As String, maximo As Decimal) As SpinEdit

        Dim s As New SpinEdit With {.Location = New Point(x, y), .Size = New Size(ancho, 20)}

        s.Properties.Mask.EditMask = mascara
        s.Properties.Mask.UseMaskAsDisplayFormat = True
        s.Properties.MinValue = 0
        s.Properties.MaxValue = maximo
        s.Properties.Increment = 1

        Return s

    End Function

#End Region

#Region "CARGA"

    ''' <summary>Muestra el préstamo vigente del empleado; 0 = sin empleado guardado (pestaña inactiva).</summary>
    Public Sub Cargar(codEmpleado As Double)

        _codEmpleado = codEmpleado

        If codEmpleado <= 0 Then
            grid.DataSource = Nothing
            lblEstado.Text = ""
            Habilitar(False)
            Exit Sub
        End If

        Habilitar(True)
        Refrescar()

    End Sub

    Private Sub Refrescar()

        Dim vigente As PrestamoEmpleado = _dal.BuscarVigente(_codEmpleado)

        grid.DataSource = _dal.ListarLibreta(_codEmpleado)

        vista.PopulateColumns()
        Caption("NumCuota", "Cuota")
        Caption("Monto", "Capital")
        Caption("Interes", "Interés")
        Caption("CuotaIgual", "Cuota igual")
        Caption("SaldoCuota", "Saldo")

        For Each nombre As String In {"Monto", "Interes", "CuotaIgual", "SaldoCuota"}
            If Not IsNothing(vista.Columns(nombre)) Then
                vista.Columns(nombre).DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                vista.Columns(nombre).DisplayFormat.FormatString = "N2"
            End If
        Next

        If IsNothing(vigente) Then
            lblEstado.Text = "Sin préstamo vigente."
            txtCuentaDebito.Text = ""
            txtCuentaCredito.Text = ""
        Else
            lblEstado.Text = "Préstamo " & vigente.Num_Prestamo & "  -  Saldo: " & vigente.Saldo.ToString("N2") &
                             If(vigente.Moneda = "US", " US$", " C$")
            txtCuentaDebito.Text = vigente.Cuenta_Debito
            txtCuentaCredito.Text = vigente.Cuenta_Credito
        End If

        btnCancelarPrestamo.Enabled = Not IsNothing(vigente)

    End Sub

    Private Sub Caption(columna As String, texto As String)
        If Not IsNothing(vista.Columns(columna)) Then vista.Columns(columna).Caption = texto
    End Sub

    Private Sub Habilitar(activo As Boolean)

        txtMonto.Enabled = activo
        txtCuotas.Enabled = activo
        txtInteres.Enabled = activo
        optMoneda.Enabled = activo
        chkCuotasIguales.Enabled = activo
        txtCuentaDebito.Enabled = activo
        txtCuentaCredito.Enabled = activo
        btnGenerar.Enabled = activo
        btnCancelarPrestamo.Enabled = False

    End Sub

    ''' <summary>El saldo inicial del préstamo es el monto (V.5.0 TxtMontoPrestamoUS_LostFocus).</summary>
    Private Sub txtMonto_EditValueChanged(sender As Object, e As EventArgs) Handles txtMonto.EditValueChanged
        txtSaldo.Text = txtMonto.Value.ToString("N2")
    End Sub

#End Region

#Region "ACCIONES"

    Private Sub btnGenerar_Click(sender As Object, e As EventArgs) Handles btnGenerar.Click

        Try

            If _codEmpleado <= 0 Then Exit Sub

            If txtMonto.Value <= 0 Then
                Aviso("Digite el monto del préstamo.")
                txtMonto.Focus()
                Exit Sub
            End If

            If txtCuotas.Value < 1 Then
                Aviso("Digite la cantidad de cuotas.")
                txtCuotas.Focus()
                Exit Sub
            End If

            If txtCuentaCredito.Text.Trim = "" Then
                Aviso("La cuenta de crédito no ha sido digitada.")
                txtCuentaCredito.Focus()
                Exit Sub
            End If

            If txtCuentaDebito.Text.Trim = "" Then
                Aviso("La cuenta de débito no ha sido digitada.")
                txtCuentaDebito.Focus()
                Exit Sub
            End If

            Dim p As PrestamoEmpleado = CalculadoraPrestamo.Calcular(
                Convert.ToDouble(txtMonto.Value), Convert.ToInt32(txtCuotas.Value), Convert.ToDouble(txtInteres.Value))

            p.Cod_Empleado = _codEmpleado
            p.Moneda = Convert.ToString(optMoneda.EditValue)
            p.Cuotas_Iguales = chkCuotasIguales.Checked
            p.Cuenta_Debito = txtCuentaDebito.Text.Trim
            p.Cuenta_Credito = txtCuentaCredito.Text.Trim

            _dal.Crear(p)

            txtMonto.Value = 0
            txtCuotas.Value = 0
            txtInteres.Value = 0
            Refrescar()

            MessageBox.Show("El préstamo fue realizado con éxito.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub btnCancelarPrestamo_Click(sender As Object, e As EventArgs) Handles btnCancelarPrestamo.Click

        Try

            If _codEmpleado <= 0 Then Exit Sub

            If MessageBox.Show("¿Desea realmente cancelar el préstamo?", "Zeus Nominas",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
                Exit Sub
            End If

            _dal.Cancelar(_codEmpleado)
            Refrescar()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Shared Sub Aviso(mensaje As String)
        MessageBox.Show(mensaje, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Warning)
    End Sub

#End Region

End Class
