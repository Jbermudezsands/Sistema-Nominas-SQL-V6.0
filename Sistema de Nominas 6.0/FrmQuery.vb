''' <summary>
''' Consola de consultas SQL para el administrador. Acceso a datos por CsConsultaDAL; <see cref="EjecutarConsulta"/>
''' queda público para lanzar una consulta desde otro formulario.
''' </summary>
Public Class FrmQuery

    Private ReadOnly _dal As New CsConsultaDAL

    ' Colores y medidas de la pantalla Query de la V.5.0 (TDBGrid): filas pares azul claro, título del grid lila, barra inferior azul pálido.
    Private Shared ReadOnly ColorFilaPar As Color = Color.FromArgb(216, 228, 248)
    Private Shared ReadOnly ColorTituloGrid As Color = Color.FromArgb(157, 157, 206)
    Private Shared ReadOnly ColorBarra As Color = Color.FromArgb(211, 226, 241)
    Private Shared ReadOnly ColorBoton As Color = Color.FromArgb(0, 0, 128)
    Private Const AnchoMaximoColumna As Integer = 320

#Region "ASPECTO"

    Private Sub FrmQuery_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ClientSize = New Size(842, 508)
        panelConsulta.Height = 110

        panelBotones.Appearance.BackColor = ColorBarra
        panelBotones.Appearance.Options.UseBackColor = True

        For Each b As DevExpress.XtraEditors.SimpleButton In {BtnEjecutar, BtnExcel, BtnInstalarSP, BtnEstructura, BtnSalir}
            b.Appearance.ForeColor = ColorBoton
            b.Appearance.Options.UseForeColor = True
        Next

        With gridViewConsulta

            .OptionsView.ColumnAutoWidth = False
            .OptionsView.EnableAppearanceEvenRow = True
            .OptionsView.ShowViewCaption = True
            .ViewCaption = "Resultado de la Consulta"
            .OptionsView.BestFitMaxRowCount = 200
            .OptionsSelection.EnableAppearanceFocusedCell = False
            .OptionsSelection.EnableAppearanceFocusedRow = True
            .HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always

            .Appearance.EvenRow.BackColor = ColorFilaPar
            .Appearance.EvenRow.Options.UseBackColor = True
            .Appearance.ViewCaption.BackColor = ColorTituloGrid
            .Appearance.ViewCaption.ForeColor = ColorBoton
            .Appearance.ViewCaption.Font = New Font(.Appearance.ViewCaption.Font, FontStyle.Bold)
            .Appearance.ViewCaption.Options.UseBackColor = True
            .Appearance.ViewCaption.Options.UseForeColor = True
            .Appearance.ViewCaption.Options.UseFont = True
            .Appearance.HeaderPanel.Options.UseTextOptions = True
            .Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center

        End With

    End Sub

    ''' <summary>Ajusta cada columna a su contenido (como AutoSize del TDBGrid), sin pasar de un ancho razonable.</summary>
    Private Sub AjustarColumnas()

        gridViewConsulta.BestFitColumns()

        For Each col As DevExpress.XtraGrid.Columns.GridColumn In gridViewConsulta.Columns
            If col.Width > AnchoMaximoColumna Then col.Width = AnchoMaximoColumna
            If col.Width < 60 Then col.Width = 60
        Next

    End Sub

#End Region

#Region "CONSULTAS"

    ''' <summary>Pone <paramref name="sentencia"/> en el editor y la ejecuta.</summary>
    Public Sub EjecutarConsulta(sentencia As String)
        TxtConsulta.Text = sentencia
        Ejecutar()
    End Sub

    Private Sub Ejecutar()

        Dim sentencia As String = TxtConsulta.Text.Trim()

        If sentencia = "" Then
            MessageBox.Show("Escriba la consulta SQL.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' SELECT/WITH solo leen; cualquier otra cosa puede modificar la base y se confirma.
        If Not CsConsultaDAL.SoloLee(sentencia) Then
            If MessageBox.Show("Esta sentencia puede modificar la base de datos. ¿Desea ejecutarla?", "Zeus Nominas",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then
                Exit Sub
            End If
        End If

        Try

            Cursor = Cursors.WaitCursor
            LblEstado.Text = "Ejecutando..."
            Application.DoEvents()

            Dim res As ResultadoConsulta = _dal.EjecutarSentencia(sentencia)

            If res.EsConsulta Then

                gridConsulta.DataSource = res.Tabla
                gridViewConsulta.PopulateColumns()
                AjustarColumnas()

                LblEstado.Text = If(res.Tabla.Columns.Count = 0, "Ejecutado, sin resultados.", res.Tabla.Rows.Count & " registros.")

            Else

                gridConsulta.DataSource = Nothing
                LblEstado.Text = "Filas afectadas: " & res.FilasAfectadas

                MessageBox.Show("Consulta ejecutada correctamente." & vbCrLf & "Filas afectadas: " & res.FilasAfectadas,
                                "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

            End If

        Catch ex As Exception
            LblEstado.Text = "Error."
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnEjecutar_Click(sender As Object, e As EventArgs) Handles BtnEjecutar.Click
        Ejecutar()
    End Sub

    Private Sub TxtConsulta_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtConsulta.KeyDown

        If e.KeyCode = Keys.F5 OrElse (e.Control AndAlso e.KeyCode = Keys.Enter) Then
            e.SuppressKeyPress = True
            Ejecutar()
        End If

    End Sub

#End Region

#Region "INSTALACIÓN"

    Private Sub BtnInstalarSP_Click(sender As Object, e As EventArgs) Handles BtnInstalarSP.Click

        If MessageBox.Show("Se creará o reemplazará el procedimiento sp_AsistenciaUnificada. ¿Desea continuar?", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try
            Cursor = Cursors.WaitCursor
            _dal.InstalarAsistenciaUnificada()
            LblEstado.Text = "SP instalado correctamente."
            MessageBox.Show("Procedimiento [sp_AsistenciaUnificada] instalado/actualizado correctamente.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            LblEstado.Text = "Error al instalar."
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub BtnEstructura_Click(sender As Object, e As EventArgs) Handles BtnEstructura.Click

        If MessageBox.Show("Se agregarán a la base las columnas que usa el módulo de asistencia (OPinfo.OPPwd2, UserLeave.Opid y UserLeave.FechaRegistro) si faltan. ¿Desea continuar?",
                           "Zeus Nominas", MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try
            Cursor = Cursors.WaitCursor
            _dal.ActualizarEstructuraAsistencia()
            LblEstado.Text = "Estructura actualizada."
            MessageBox.Show("Todos los scripts se ejecutaron correctamente.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            LblEstado.Text = "Error al ejecutar scripts."
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

#End Region

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs) Handles BtnExcel.Click

        If gridViewConsulta.DataRowCount = 0 Then
            MessageBox.Show("No hay resultados para exportar.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Try
            Dim ruta As String = ExportadorGrid.ExportarAExcel(gridViewConsulta, "Consulta")

            If ruta <> "" Then
                MessageBox.Show("Resultado exportado a " & ruta, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
