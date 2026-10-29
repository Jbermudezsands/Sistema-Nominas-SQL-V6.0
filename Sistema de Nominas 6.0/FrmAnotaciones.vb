''' <summary>
''' Anotaciones (currículum) de un empleado. Acceso a datos por CsCurriculumDAL; la ficha del empleado la abre con
''' <see cref="CargarEmpleado"/>, que también queda público para otros formularios.
''' </summary>
Public Class FrmAnotaciones

    Private ReadOnly _dal As New CsCurriculumDAL

    Private _codEmpleado As Decimal = 0

#Region "CARGA"

    Private Sub FrmAnotaciones_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        CmbFaltas.Properties.MaxLength = CsCurriculumDAL.LongFaltas
        TxtJustificaFalta.Properties.MaxLength = CsCurriculumDAL.LongJustificacion
        TxtDatosRecord.Properties.MaxLength = CsCurriculumDAL.LongDatosRecord
        TxtIdiomas.Properties.MaxLength = CsCurriculumDAL.LongIdiomas
        TxtTelEmergencia.Properties.MaxLength = CsCurriculumDAL.LongTelefono
        TxtCursos.Properties.MaxLength = CsCurriculumDAL.LongCursos
        TxtRazones.Properties.MaxLength = CsCurriculumDAL.LongRazones
        TxtTrabAnteriores.Properties.MaxLength = CsCurriculumDAL.LongTrabajoAnterior
        TxtRecomendaciones.Properties.MaxLength = CsCurriculumDAL.LongRecomendaciones
        TxtSalida.Properties.MaxLength = CsCurriculumDAL.LongCausaSalida

    End Sub

    ''' <summary>Muestra las anotaciones del empleado (<paramref name="codEmpleado"/> es su código interno, Empleado.CodEmpleado).</summary>
    Public Sub CargarEmpleado(codEmpleado As Decimal, Optional codigoVisible As String = "")

        Try

            Dim nombre As String = _dal.NombreEmpleado(codEmpleado)

            If nombre Is Nothing Then
                MessageBox.Show("El empleado no existe.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            _codEmpleado = codEmpleado
            TxtCodigo.Text = If(codigoVisible <> "", codigoVisible, codEmpleado.ToString("0"))
            TxtNombre.Text = nombre

            Dim c As CurriculumEmpleado = _dal.Buscar(codEmpleado)

            If c Is Nothing Then
                c = New CurriculumEmpleado With {.Cod_Empleado = codEmpleado}
            End If

            CmbFaltas.Text = c.Faltas
            TxtJustificaFalta.Text = c.Justificacion_Faltas
            TxtDatosRecord.Text = c.Datos_Record
            TxtIdiomas.Text = c.Idiomas
            TxtTelEmergencia.Text = c.Telefono_Emergencia
            TxtCursos.Text = c.Cursos
            TxtRazones.Text = c.Razones_Contratacion
            TxtTrabAnteriores.Text = c.Trabajo_Anterior
            TxtRecomendaciones.Text = c.Recomendaciones
            TxtSalida.Text = c.Causa_Salida
            DtpContratacion.DateTime = c.Fecha_Contratacion
            DtpFinaliza.DateTime = c.Fecha_Finalizacion

        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

#End Region

#Region "ACCIONES"

    Public Function Grabar() As Boolean

        If _codEmpleado = 0 Then Return False

        Try

            Cursor = Cursors.WaitCursor

            _dal.Guardar(New CurriculumEmpleado With {
                .Cod_Empleado = _codEmpleado,
                .Faltas = CmbFaltas.Text,
                .Justificacion_Faltas = TxtJustificaFalta.Text,
                .Datos_Record = TxtDatosRecord.Text,
                .Idiomas = TxtIdiomas.Text,
                .Telefono_Emergencia = TxtTelEmergencia.Text,
                .Cursos = TxtCursos.Text,
                .Razones_Contratacion = TxtRazones.Text,
                .Trabajo_Anterior = TxtTrabAnteriores.Text,
                .Recomendaciones = TxtRecomendaciones.Text,
                .Causa_Salida = TxtSalida.Text,
                .Fecha_Contratacion = FechaDe(DtpContratacion),
                .Fecha_Finalizacion = FechaDe(DtpFinaliza)})

            Return True

        Catch ex As Exception
            MostrarError(ex)
            Return False
        Finally
            Cursor = Cursors.Default
        End Try

    End Function

    Private Sub BtnGrabar_Click(sender As Object, e As EventArgs) Handles BtnGrabar.Click

        If Grabar() Then
            MessageBox.Show("Anotaciones guardadas.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

    Private Sub BtnBorrar_Click(sender As Object, e As EventArgs) Handles BtnBorrar.Click

        If _codEmpleado = 0 Then Exit Sub

        If MessageBox.Show("¿Está seguro de borrar las anotaciones de " & TxtNombre.Text & "?", "Zeus Nominas",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) <> DialogResult.Yes Then
            Exit Sub
        End If

        Try
            If Not _dal.Eliminar(_codEmpleado) Then
                MessageBox.Show("El empleado no tiene anotaciones guardadas.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            CargarEmpleado(_codEmpleado, TxtCodigo.Text)
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

#End Region

    Private Shared Function FechaDe(editor As DevExpress.XtraEditors.DateEdit) As DateTime
        Return If(editor.EditValue Is Nothing, DateTime.Today, editor.DateTime.Date)
    End Function

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
