Imports DevExpress.XtraEditors
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraTab

''' <summary>
''' Movimientos de la nómina activa de un empleado: horas extra, comisiones y otros destajos (V.5.0 FrmMovimientos).
''' Acceso a datos por CsMovimientoNominaDAL. <see cref="CargarEmpleado"/> queda público para abrirlo desde el listado de horas extra;
''' <see cref="MovimientoGuardado"/> avisa para que el listado se refresque. La interfaz se arma por código (no hay Designer).
''' </summary>
Public Class FrmMovimientos
    Inherits XtraForm

    Private ReadOnly _dal As New CsMovimientoNominaDAL
    Private _emp As CsMovimientoNominaDAL.DatosEmpleado

    Private CmbEmpleado As SearchLookUpEdit
    Private TxtNombres As TextEdit
    Private TxtNomina As TextEdit
    Private TxtNumNomina As TextEdit
    Private Tabs As XtraTabControl
    Private PagHoras As XtraTabPage
    Private SpnHoras As SpinEdit
    Private WithEvents BtnAgregarHoras As SimpleButton
    Private _comisiones As PaginaDetalle
    Private _destajos As PaginaDetalle
    Private _cargando As Boolean

    Public Event MovimientoGuardado()

    ''' <summary>Controles de una pestaña de comisiones o destajos (las dos son iguales).</summary>
    Private Class PaginaDetalle
        Public Pagina As XtraTabPage
        Public EsComision As Boolean
        Public Tipo As LookUpEdit
        Public Cantidad As SpinEdit
        Public Grid As GridControl
        Public Vista As GridView
        Public Total As LabelControl
    End Class

    Public Sub New()

        Text = "Movimientos de la nómina"
        ClientSize = New Size(760, 470)
        StartPosition = FormStartPosition.CenterParent

        CrearEncabezado()
        CrearPestanas()

        AddHandler Load, AddressOf FrmMovimientos_Load

    End Sub

#Region "INTERFAZ"

    Private Sub CrearEncabezado()

        Dim panel As New PanelControl With {.Dock = DockStyle.Top, .Height = 80, .BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder}

        CmbEmpleado = New SearchLookUpEdit With {.Location = New Point(80, 12), .Size = New Size(150, 20)}
        CmbEmpleado.Properties.DisplayMember = "CodEmpleado1"
        CmbEmpleado.Properties.ValueMember = "CodEmpleado1"
        CmbEmpleado.Properties.NullText = ""
        AddHandler CmbEmpleado.EditValueChanged, AddressOf CmbEmpleado_EditValueChanged

        TxtNombres = New TextEdit With {.Location = New Point(240, 12), .Size = New Size(500, 20)}
        TxtNombres.Properties.ReadOnly = True
        TxtNomina = New TextEdit With {.Location = New Point(80, 44), .Size = New Size(260, 20)}
        TxtNomina.Properties.ReadOnly = True
        TxtNumNomina = New TextEdit With {.Location = New Point(440, 44), .Size = New Size(100, 20)}
        TxtNumNomina.Properties.ReadOnly = True

        panel.Controls.Add(New LabelControl With {.Text = "Empleado:", .Location = New Point(13, 15)})
        panel.Controls.Add(New LabelControl With {.Text = "Nómina:", .Location = New Point(13, 47)})
        panel.Controls.Add(New LabelControl With {.Text = "N.º nómina activa:", .Location = New Point(350, 47)})
        panel.Controls.AddRange(New Control() {CmbEmpleado, TxtNombres, TxtNomina, TxtNumNomina})

        Controls.Add(panel)

    End Sub

    Private Sub CrearPestanas()

        Tabs = New XtraTabControl With {.Dock = DockStyle.Fill, .Enabled = False}

        PagHoras = New XtraTabPage With {.Text = "Horas Extras"}
        SpnHoras = New SpinEdit With {.Location = New Point(110, 20), .Size = New Size(100, 20)}
        SpnHoras.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        SpnHoras.Properties.DisplayFormat.FormatString = "n2"
        SpnHoras.Properties.MinValue = 0
        SpnHoras.Properties.MaxValue = 100000
        BtnAgregarHoras = New SimpleButton With {.Text = "Agregar", .Location = New Point(225, 17), .Size = New Size(95, 27)}
        PagHoras.Controls.Add(New LabelControl With {.Text = "Horas extra:", .Location = New Point(13, 23)})
        PagHoras.Controls.AddRange(New Control() {SpnHoras, BtnAgregarHoras})

        _comisiones = CrearPaginaDetalle("Comisiones", True)
        _destajos = CrearPaginaDetalle("Otros Destajos", False)

        Tabs.TabPages.AddRange(New XtraTabPage() {PagHoras, _comisiones.Pagina, _destajos.Pagina})
        Controls.Add(Tabs)
        Controls.SetChildIndex(Tabs, 0)

    End Sub

    Private Function CrearPaginaDetalle(titulo As String, comision As Boolean) As PaginaDetalle

        Dim p As New PaginaDetalle With {.EsComision = comision, .Pagina = New XtraTabPage With {.Text = titulo}}
        Dim etiqueta As String = If(comision, "Comisión:", "Destajo:")

        Dim superior As New PanelControl With {.Dock = DockStyle.Top, .Height = 50, .BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder}
        Dim inferior As New PanelControl With {.Dock = DockStyle.Bottom, .Height = 50, .BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder}

        p.Tipo = New LookUpEdit With {.Location = New Point(80, 14), .Size = New Size(220, 20)}
        p.Tipo.Properties.DisplayMember = "Nombre"
        p.Tipo.Properties.ValueMember = "Cod"
        p.Tipo.Properties.Columns.Add(New DevExpress.XtraEditors.Controls.LookUpColumnInfo("Nombre", "Tipo"))
        p.Tipo.Properties.ShowHeader = False
        p.Tipo.Properties.NullText = ""

        p.Cantidad = New SpinEdit With {.Location = New Point(370, 14), .Size = New Size(90, 20)}
        p.Cantidad.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
        p.Cantidad.Properties.DisplayFormat.FormatString = "n2"
        p.Cantidad.Properties.MaxValue = 1000000000

        Dim btnAgregar As New SimpleButton With {.Text = "Agregar", .Location = New Point(475, 11), .Size = New Size(90, 27)}
        Dim btnBorrar As New SimpleButton With {.Text = "Borrar", .Location = New Point(575, 11), .Size = New Size(90, 27)}
        AddHandler btnAgregar.Click, Sub(s, e) AgregarDetalle(p)
        AddHandler btnBorrar.Click, Sub(s, e) BorrarDetalle(p)

        superior.Controls.Add(New LabelControl With {.Text = etiqueta, .Location = New Point(13, 17)})
        superior.Controls.Add(New LabelControl With {.Text = "Cantidad:", .Location = New Point(315, 17)})
        superior.Controls.AddRange(New Control() {p.Tipo, p.Cantidad, btnAgregar, btnBorrar})

        p.Total = New LabelControl With {.Text = "0.00", .Location = New Point(110, 18)}
        Dim btnAplicar As New SimpleButton With {.Text = "Aplicar al empleado", .Location = New Point(300, 11), .Size = New Size(140, 27)}
        AddHandler btnAplicar.Click, Sub(s, e) AplicarDetalle(p)
        inferior.Controls.Add(New LabelControl With {.Text = If(comision, "Total comisión:", "Total destajo:"), .Location = New Point(13, 18)})
        inferior.Controls.AddRange(New Control() {p.Total, btnAplicar})

        p.Grid = New GridControl With {.Dock = DockStyle.Fill}
        p.Vista = New GridView(p.Grid)
        p.Grid.MainView = p.Vista
        p.Grid.ViewCollection.Add(p.Vista)
        p.Vista.OptionsBehavior.Editable = False
        p.Vista.OptionsView.ShowGroupPanel = False
        p.Vista.OptionsView.EnableAppearanceEvenRow = True

        p.Pagina.Controls.Add(p.Grid)
        p.Pagina.Controls.Add(superior)
        p.Pagina.Controls.Add(inferior)

        Return p

    End Function

#End Region

#Region "CARGA"

    Private Sub FrmMovimientos_Load(sender As Object, e As EventArgs)

        Try

            _cargando = True
            CmbEmpleado.Properties.DataSource = _dal.ListarEmpleados()
            CmbEmpleado.Properties.View.PopulateColumns(CmbEmpleado.Properties.DataSource)
            CmbEmpleado.Properties.PopupFormSize = New Size(520, 300)
            _comisiones.Tipo.Properties.DataSource = _dal.ListarTipos(True)
            _destajos.Tipo.Properties.DataSource = _dal.ListarTipos(False)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            _cargando = False
        End Try

    End Sub

    ''' <summary>Elige el empleado (código visible) y, si viene, deja las horas extra indicadas en pantalla (como el botón Editar del listado).</summary>
    Public Sub CargarEmpleado(codigoVisible As String, Optional horas As Double? = Nothing)

        CmbEmpleado.EditValue = Nothing
        CmbEmpleado.EditValue = codigoVisible

        If horas.HasValue AndAlso _emp IsNot Nothing Then SpnHoras.Value = Convert.ToDecimal(horas.Value)

    End Sub

    Private Sub CmbEmpleado_EditValueChanged(sender As Object, e As EventArgs)

        If _cargando Then Exit Sub

        Try

            Dim codigo As String = Convert.ToString(CmbEmpleado.EditValue)
            _emp = If(codigo = "", Nothing, _dal.BuscarEmpleado(codigo))

            Tabs.Enabled = False
            TxtNombres.Text = ""
            TxtNomina.Text = ""
            TxtNumNomina.Text = ""

            If _emp Is Nothing Then Exit Sub

            TxtNombres.Text = (_emp.Nombres & " " & _emp.Apellidos).Trim()
            TxtNomina.Text = _emp.Nomina

            If _emp.NumNomina = 0 Then
                MessageBox.Show("Este empleado no tiene su nómina activa.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
                _emp = Nothing
                Exit Sub
            End If

            TxtNumNomina.Text = _emp.NumNomina.ToString()
            Tabs.Enabled = True

            SpnHoras.Value = Convert.ToDecimal(_dal.ObtenerHorasExtra(_emp.CodEmpleado, _emp.NumNomina))
            HabilitarPestanas()
            Recargar(_comisiones)
            Recargar(_destajos)

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    ''' <summary>Según el tipo de pago de la nómina se activan comisiones y/o destajos; con salario fijo del empleado ninguno.</summary>
    Private Sub HabilitarPestanas()

        Dim comision As Boolean = False
        Dim destajo As Boolean = False

        Select Case _emp.TipoPago
            Case "Salario Destajo" : destajo = True
            Case "Salario Fijo y Comision" : comision = True
            Case "Salario Destajo y Comision", "Salario Fijo,Destajo y Comision" : comision = True : destajo = True
        End Select

        If _emp.SalarioFijo Then
            comision = False
            destajo = False
        End If

        _comisiones.Pagina.PageEnabled = comision
        _destajos.Pagina.PageEnabled = destajo

    End Sub

    Private Sub Recargar(p As PaginaDetalle)

        p.Grid.DataSource = _dal.ListarDetalle(p.EsComision, _emp.CodEmpleado, _emp.NumNomina)

        With p.Vista

            .PopulateColumns()
            If .Columns("Id") IsNot Nothing Then .Columns("Id").Visible = False

            .Columns("Valor").Caption = If(p.EsComision, "Porcentaje", "Monto")
            .Columns("Valor").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
            .Columns("Valor").DisplayFormat.FormatString = If(p.EsComision, "p2", "n2")
            .Columns("Cantidad").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
            .Columns("Cantidad").DisplayFormat.FormatString = "n2"
            .Columns("Total").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
            .Columns("Total").DisplayFormat.FormatString = "n2"

        End With

        p.Total.Text = Total(p).ToString("N2")

    End Sub

    Private Shared Function Total(p As PaginaDetalle) As Double

        Dim suma As Double = 0

        For i As Integer = 0 To p.Vista.DataRowCount - 1
            suma += Convert.ToDouble(p.Vista.GetRowCellValue(i, "Total"))
        Next

        Return suma

    End Function

#End Region

#Region "ACCIONES"

    Private Sub BtnAgregarHoras_Click(sender As Object, e As EventArgs) Handles BtnAgregarHoras.Click

        If _emp Is Nothing Then Exit Sub

        Try

            If _dal.GuardarHorasExtra(_emp.CodEmpleado, _emp.NumNomina, Convert.ToDouble(SpnHoras.Value)) Then
                MessageBox.Show("Ya le fueron agregadas horas extra a este empleado; las horas anteriores fueron reemplazadas.", "Zeus Nominas",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            RaiseEvent MovimientoGuardado()

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub AgregarDetalle(p As PaginaDetalle)

        If _emp Is Nothing Then Exit Sub

        If Convert.ToString(p.Tipo.EditValue) = "" Then
            MessageBox.Show("Elija el tipo.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Try

            If _dal.GuardarDetalle(p.EsComision, _emp.CodEmpleado, _emp.NumNomina, Convert.ToString(p.Tipo.EditValue), Convert.ToDouble(p.Cantidad.Value)) Then
                MessageBox.Show("Ya le fue grabado este tipo al empleado; la cantidad anterior fue reemplazada.", "Zeus Nominas",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            Recargar(p)

            ' V.5.0: al cambiar las comisiones, el total pasa solo a la ficha del empleado.
            If p.EsComision Then _dal.AplicarAlEmpleado(True, _emp.CodEmpleado, Total(p))

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BorrarDetalle(p As PaginaDetalle)

        Dim fila As DataRow = TryCast(p.Vista.GetFocusedDataRow(), DataRow)

        If fila Is Nothing Then
            MessageBox.Show("Seleccione el registro que desea borrar.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Try
            _dal.EliminarDetalle(p.EsComision, Convert.ToInt32(fila("Id")))
            Recargar(p)
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub AplicarDetalle(p As PaginaDetalle)

        If _emp Is Nothing Then Exit Sub

        Try
            _dal.AplicarAlEmpleado(p.EsComision, _emp.CodEmpleado, Total(p))
            MessageBox.Show("Total aplicado a la ficha del empleado: " & Total(p).ToString("N2"), "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

#End Region

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
