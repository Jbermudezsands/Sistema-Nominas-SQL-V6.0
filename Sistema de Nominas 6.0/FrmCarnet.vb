Imports System.Drawing.Printing

''' <summary>
''' Carnet de un empleado (nombre, cargo, código, foto y código de barras). Datos por CsCarnetDAL;
''' <see cref="CargarEmpleado"/> queda público para abrir el carnet desde otros formularios.
''' </summary>
Public Class FrmCarnet

    Private ReadOnly _dal As New CsCarnetDAL

    ''' <summary>Muestra el carnet del empleado (<paramref name="codEmpleado"/> es su código interno, Empleado.CodEmpleado).</summary>
    Public Sub CargarEmpleado(codEmpleado As Decimal)

        Try

            Dim datos As DatosCarnet = _dal.Buscar(codEmpleado)

            If datos Is Nothing Then
                MessageBox.Show("El empleado no existe.", "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            LblNombres.Text = datos.Nombres
            LblApellidos.Text = datos.Apellidos
            LblCargo.Text = datos.Cargo
            LblCodigo.Text = datos.Codigo
            LblCodigoBarras.Text = "*" & datos.Codigo & "*"

            PicBarras.Image = CodigoBarras39.Dibujar(datos.Codigo, 672, 112)
            PicFoto.Image = ImagenDeFoto(datos.Foto)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Shared Function ImagenDeFoto(foto As String) As Image

        Dim img As Image = Nothing

        If Not String.IsNullOrWhiteSpace(foto) Then
            img = BytesToImagen(StringtoByte(foto))
        End If

        Return If(img, My.Resources.Silueta)

    End Function

    Private Sub BtnImprimir_Click(sender As Object, e As EventArgs) Handles BtnImprimir.Click

        Try

            Dim bmp As New Bitmap(PanelCarnet.Width, PanelCarnet.Height)
            PanelCarnet.DrawToBitmap(bmp, New Rectangle(0, 0, PanelCarnet.Width, PanelCarnet.Height))

            Using doc As New PrintDocument
                AddHandler doc.PrintPage,
                    Sub(s, ev)
                        ev.Graphics.DrawImage(bmp, ev.MarginBounds.Left, ev.MarginBounds.Top)
                        ev.HasMorePages = False
                    End Sub

                Using dlg As New PrintDialog With {.Document = doc}
                    If dlg.ShowDialog(Me) = DialogResult.OK Then doc.Print()
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Zeus Nominas", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub

    Private Sub BtnSalir_Click(sender As Object, e As EventArgs) Handles BtnSalir.Click
        Close()
    End Sub

End Class
