Imports System.IO

''' <summary>
''' Controles personalizados: datos de la empresa (DatosEmpresa) y parámetros de cálculo
''' (Controles). Todo el acceso a datos pasa por CsControlesDAL.
''' Los procesos masivos de la V.5.0 (ajuste de salarios, asignación de horarios, etc.)
''' no forman parte de este formulario.
''' </summary>
Public Class FrmControles

    Private ReadOnly _dal As New CsControlesDAL

    Private Const FormatoConProduccion As String = "Nomina Bono Produccion"

#Region "CARGA"

    Private Sub FrmControles_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Try
            Cargar()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub Cargar()

        Dim c As Controles = _dal.ObtenerControles()
        Dim d As DatosEmpresa = _dal.ObtenerDatosEmpresa()

        ' Empresa
        TxtNombreEmpresa.Text = d.Nombre_Empresa
        TxtRucEmpresa.Text = d.Numero_Ruc
        TxtDireccionEmpresa.Text = d.Direccion
        TxtTelefono.Text = d.Telefono
        TxtFax.Text = d.Fax
        TxtEmail.Text = d.Email
        TxtRutaLogo.Text = d.Ruta_Logo
        TxtRutaFoto.Text = d.Ruta_Foto
        MostrarLogo()

        ' Configuración
        If Math.Abs(c.Dias_Mes - 25) < 0.01 Then
            RgDiasMes.EditValue = 25
        ElseIf c.Dias_Mes > 30.1 Then
            RgDiasMes.EditValue = 30.41667
        Else
            RgDiasMes.EditValue = 30
        End If

        RgDiasSemana.EditValue = If(Math.Abs(c.Dias_Semana - 6) < 0.01, 6, 7)
        RgVacaciones.EditValue = d.Metodo_Vacaciones
        RgIr.EditValue = d.Tipo_Calculo_Ir
        RgSalarioPromedio.EditValue = c.Salario_Promedio_Real
        ChkAntiguedadMenor.Checked = c.Antiguedad_Menor
        ChkTasa.Checked = c.Verificar_Tasa
        ChkRedondeo.Checked = c.Calcular_Redondeado
        ChkRestar3Dias.Checked = d.Restar_3_Dias_Subsidio
        Chk7mo.Checked = d.Calcular_7mo
        TxtPorcientoSubsidio.EditValue = Convert.ToDecimal(d.Porciento_Subsidio)

        Dim horas As Decimal = 2
        Decimal.TryParse(d.Horas_Extra, horas)
        TxtHorasExtra.EditValue = horas

        CmbColillas.Text = If(d.Formato_Colilla = "", "Predeterminado", d.Formato_Colilla)
        CmbNominas.Text = If(d.Formato_Nomina = "", "Predeterminado", d.Formato_Nomina)
        Chk7mo.Visible = (CmbNominas.Text = FormatoConProduccion)

        ' Contable
        TxtConexionString.Text = d.Conexion_Sistema_Contable

    End Sub

    Private Sub MostrarLogo()

        If ImgLogo.Image IsNot Nothing Then
            ImgLogo.Image.Dispose()
            ImgLogo.Image = Nothing
        End If

        Dim ruta As String = TxtRutaLogo.Text.Trim

        If ruta = "" OrElse Not File.Exists(ruta) Then Exit Sub

        Try
            ' Se copia a memoria para no dejar el archivo bloqueado.
            Using original As Image = Image.FromStream(New MemoryStream(File.ReadAllBytes(ruta)))
                ImgLogo.Image = New Bitmap(original)
            End Using
        Catch
            ' Un archivo que no es imagen simplemente no se muestra.
        End Try

    End Sub

#End Region

#Region "EVENTOS"

    Private Sub CmbNominas_EditValueChanged(sender As Object, e As EventArgs) Handles CmbNominas.EditValueChanged
        Chk7mo.Visible = (CmbNominas.Text = FormatoConProduccion)
    End Sub

    Private Sub BtnBuscarLogo_Click(sender As Object, e As EventArgs) Handles BtnBuscarLogo.Click

        Using dlg As New OpenFileDialog With {
            .Title = "Seleccione el logo de la empresa",
            .Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Todos los archivos|*.*"}

            If dlg.ShowDialog() <> DialogResult.OK Then Exit Sub

            TxtRutaLogo.Text = dlg.FileName
            MostrarLogo()
        End Using

    End Sub

    Private Sub BtnBuscarFoto_Click(sender As Object, e As EventArgs) Handles BtnBuscarFoto.Click

        Using dlg As New FolderBrowserDialog With {.Description = "Seleccione la carpeta de fotos de los empleados"}

            If Directory.Exists(TxtRutaFoto.Text) Then dlg.SelectedPath = TxtRutaFoto.Text

            If dlg.ShowDialog() <> DialogResult.OK Then Exit Sub

            TxtRutaFoto.Text = dlg.SelectedPath
        End Using

    End Sub

    Private Sub TxtRutaLogo_Leave(sender As Object, e As EventArgs) Handles TxtRutaLogo.Leave
        MostrarLogo()
    End Sub

    Private Sub BtnRecargar_Click(sender As Object, e As EventArgs) Handles BtnRecargar.Click

        Try
            Cargar()
        Catch ex As Exception
            MostrarError(ex)
        End Try

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

#End Region

#Region "GRABAR"

    Private Sub BtnGuardar_Click(sender As Object, e As EventArgs) Handles BtnGuardar.Click

        Try

            Cursor = Cursors.WaitCursor

            If Not Validar() Then Exit Sub

            _dal.GuardarControles(ObtenerControles())
            _dal.GuardarDatosEmpresa(ObtenerDatosEmpresa())

            MessageBox.Show("Los controles se guardaron correctamente.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MostrarError(ex)
        Finally
            Cursor = Cursors.Default
        End Try

    End Sub

    Private Function Validar() As Boolean

        Dim subsidio As Decimal = Convert.ToDecimal(TxtPorcientoSubsidio.EditValue)

        If subsidio < 0 OrElse subsidio > 100 Then
            Tabs.SelectedTabPage = TabConfig
            MessageBox.Show("El porciento de subsidio debe estar entre 0 y 100.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtPorcientoSubsidio.Focus()
            Return False
        End If

        If Convert.ToDecimal(TxtHorasExtra.EditValue) <= 0 Then
            Tabs.SelectedTabPage = TabConfig
            MessageBox.Show("El valor de horas extra debe ser mayor que cero.", "Zeus Nominas",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtHorasExtra.Focus()
            Return False
        End If

        Return True

    End Function

    Private Function ObtenerControles() As Controles

        Dim diasMes As Double = Convert.ToDouble(RgDiasMes.EditValue)

        Return New Controles With {
            .Dias_Mes = If(diasMes > 30.1, 365 / 12, diasMes),
            .Dias_Semana = Convert.ToDouble(RgDiasSemana.EditValue),
            .Verificar_Tasa = ChkTasa.Checked,
            .Salario_Promedio_Real = Convert.ToBoolean(RgSalarioPromedio.EditValue),
            .Antiguedad_Menor = ChkAntiguedadMenor.Checked,
            .Calcular_Redondeado = ChkRedondeo.Checked
        }

    End Function

    Private Function ObtenerDatosEmpresa() As DatosEmpresa

        Return New DatosEmpresa With {
            .Nombre_Empresa = TxtNombreEmpresa.Text.Trim,
            .Numero_Ruc = TxtRucEmpresa.Text.Trim,
            .Direccion = TxtDireccionEmpresa.Text.Trim,
            .Telefono = TxtTelefono.Text.Trim,
            .Fax = TxtFax.Text.Trim,
            .Email = TxtEmail.Text.Trim,
            .Ruta_Logo = TxtRutaLogo.Text.Trim,
            .Ruta_Foto = TxtRutaFoto.Text.Trim,
            .Formato_Colilla = CmbColillas.Text.Trim,
            .Formato_Nomina = CmbNominas.Text.Trim,
            .Metodo_Vacaciones = Convert.ToString(RgVacaciones.EditValue),
            .Tipo_Calculo_Ir = Convert.ToString(RgIr.EditValue),
            .Conexion_Sistema_Contable = TxtConexionString.Text.Trim,
            .Porciento_Subsidio = Convert.ToDouble(TxtPorcientoSubsidio.EditValue),
            .Horas_Extra = Convert.ToDecimal(TxtHorasExtra.EditValue).ToString("0.##", Globalization.CultureInfo.InvariantCulture),
            .Calcular_7mo = Chk7mo.Checked,
            .Restar_3_Dias_Subsidio = ChkRestar3Dias.Checked
        }

    End Function

#End Region

    Private Shared Sub MostrarError(ex As Exception)
        MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

End Class
