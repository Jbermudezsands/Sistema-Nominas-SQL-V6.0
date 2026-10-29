Imports DevExpress.XtraGrid.Columns
Imports DevExpress.XtraGrid.Views.Grid

''' <summary>
''' Tablas de tramos del INSS y del IR, editables en cuadrícula. Acceso a datos por CsInssIrDAL;
''' <see cref="Recargar"/> queda público para refrescar desde otros formularios.
''' </summary>
Public Class FrmInssIr

    Private ReadOnly _dal As New CsInssIrDAL

#Region "CARGA"

    Private Sub FrmInssIr_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Recargar()
    End Sub

    Public Sub Recargar()

        Try

            Cursor = Cursors.WaitCursor

            gridInss.DataSource = _dal.ListarInss()
            gridIr.DataSource = _dal.ListarIr()

            ConfigurarInss()
            ConfigurarIr()

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Sub ConfigurarInss()

        With gridViewInss

            .OptionsView.EnableAppearanceEvenRow = True

            Encabezado(gridViewInss, "CodInss", "Código", 70, False)
            Encabezado(gridViewInss, "Categoria", "Categoría", 110, False)
            Encabezado(gridViewInss, "Desde", "Desde", 90, True)
            Encabezado(gridViewInss, "Hasta", "Hasta", 90, True)
            Encabezado(gridViewInss, "Promedio Semanal", "Promedio semanal", 110, True)
            Encabezado(gridViewInss, "MontoLaboral1", "Laboral", 90, True)
            Encabezado(gridViewInss, "MontoPatronal1", "Patronal", 90, True)
            Encabezado(gridViewInss, "MontoLaboral4", "Laboral 4", 90, True)
            Encabezado(gridViewInss, "MontoPatronal4", "Patronal 4", 90, True)
            Encabezado(gridViewInss, "MontoLaboral5", "Laboral 5", 90, True)
            Encabezado(gridViewInss, "MontoPatronal5", "Patronal 5", 90, True)

        End With

    End Sub

    Private Sub ConfigurarIr()

        With gridViewIr

            .OptionsView.EnableAppearanceEvenRow = True

            Encabezado(gridViewIr, "CodIr", "Código", 70, False)
            Encabezado(gridViewIr, "Desde", "Desde", 110, True)
            Encabezado(gridViewIr, "Hasta", "Hasta", 110, True)
            Encabezado(gridViewIr, "ImpuestoBase", "Impuesto base", 110, True)
            Encabezado(gridViewIr, "PorcientoImpuesto", "% Impuesto", 90, True)
            Encabezado(gridViewIr, "SobreExceso", "Sobre exceso", 110, True)

        End With

    End Sub

    Private Shared Sub Encabezado(vista As GridView, columna As String, titulo As String, ancho As Integer, numerica As Boolean)

        Dim col As GridColumn = vista.Columns(columna)

        If col Is Nothing Then Exit Sub

        col.Caption = titulo
        col.Width = ancho

        If numerica Then
            col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
            col.DisplayFormat.FormatString = "n2"
        End If

    End Sub

#End Region

#Region "ACCIONES"

    ''' <summary>Aplica a la base las altas, cambios y bajas hechos en ambas cuadrículas.</summary>
    Public Function Guardar() As Boolean

        Try

            Cursor = Cursors.WaitCursor

            gridViewInss.CloseEditor()
            gridViewInss.UpdateCurrentRow()
            gridViewIr.CloseEditor()
            gridViewIr.UpdateCurrentRow()

            Dim tocadas As Integer =
                _dal.GuardarInss(CType(gridInss.DataSource, DataTable)) +
                _dal.GuardarIr(CType(gridIr.DataSource, DataTable))

            MessageBox.Show(If(tocadas = 0, "No hay cambios por guardar.", "Cambios guardados (" & tocadas & " filas)."),
                            "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Return True

        Catch ex As Exception
            MostrarError(ex)
            Return False
        Finally
            Cursor = Cursors.Default
        End Try

    End Function

    Private Sub BtnGuardar_Click(sender As Object, e As EventArgs) Handles BtnGuardar.Click
        Guardar()
    End Sub

    Private Sub BtnRecargar_Click(sender As Object, e As EventArgs) Handles BtnRecargar.Click

        If HayCambiosPendientes() AndAlso
           MessageBox.Show("Hay cambios sin guardar que se perderán. ¿Desea recargar?", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Recargar()

    End Sub

    Private Function HayCambiosPendientes() As Boolean

        gridViewInss.CloseEditor()
        gridViewIr.CloseEditor()

        Dim a As DataTable = TryCast(gridInss.DataSource, DataTable)
        Dim b As DataTable = TryCast(gridIr.DataSource, DataTable)

        Return (a IsNot Nothing AndAlso a.GetChanges() IsNot Nothing) OrElse (b IsNot Nothing AndAlso b.GetChanges() IsNot Nothing)

    End Function

    Private Sub BtnExcel_Click(sender As Object, e As EventArgs) Handles BtnExcel.Click

        Try

            Dim esInss As Boolean = (TabControl.SelectedTabPage Is PageInss)
            Dim ruta As String = ExportadorGrid.ExportarAExcel(If(esInss, gridViewInss, gridViewIr), If(esInss, "TablaINSS", "TablaIR"))

            If ruta <> "" Then
                MessageBox.Show("Tabla exportada a " & ruta, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

    Private Sub FrmInssIr_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If HayCambiosPendientes() Then
            Select Case MessageBox.Show("Hay cambios sin guardar. ¿Desea grabarlos antes de salir?", "Zeus Nominas",
                                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
                Case DialogResult.Yes
                    If Not Guardar() Then e.Cancel = True
                Case DialogResult.Cancel
                    e.Cancel = True
            End Select
        End If

    End Sub

#End Region

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
