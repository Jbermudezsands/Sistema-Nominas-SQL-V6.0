Imports DevExpress.XtraEditors

''' <summary>
''' Datos que pide el banco antes de generar el archivo de pago (V.5.0 FrmExportaBac y los datos fijos de CmdExportaBanpro):
''' en el formato BAC, código de cliente, consecutivo, fecha y moneda; en Banpro/Bacentro, la cuenta de la empresa que se debita.
''' Solo recoge los datos; el archivo lo arma <see cref="ExportadorBancos"/>.
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmDatosBanco
    Inherits XtraForm

    Public Enum Formato
        Bac
        CreditoDebito
    End Enum

    Private ReadOnly _formato As Formato
    Private TxtConcepto, TxtCodigo, TxtCuenta As TextEdit
    Private TxtConsecutivo As SpinEdit
    Private TxtFecha As DateEdit
    Private ChkDolares As CheckEdit

    Public ReadOnly Property Concepto As String
        Get
            Return TxtConcepto.Text.Trim()
        End Get
    End Property

    Public ReadOnly Property CodigoCliente As String
        Get
            Return TxtCodigo.Text.Trim()
        End Get
    End Property

    Public ReadOnly Property Consecutivo As Integer
        Get
            Return Convert.ToInt32(TxtConsecutivo.Value)
        End Get
    End Property

    Public ReadOnly Property Fecha As Date
        Get
            Return TxtFecha.DateTime.Date
        End Get
    End Property

    Public ReadOnly Property EnDolares As Boolean
        Get
            Return ChkDolares.Checked
        End Get
    End Property

    Public ReadOnly Property CuentaEmpresa As String
        Get
            Return TxtCuenta.Text.Trim()
        End Get
    End Property

    Public Sub New(formato As Formato, titulo As String, conceptoInicial As String)

        _formato = formato
        Text = titulo
        ClientSize = New Size(380, If(formato = FrmDatosBanco.Formato.Bac, 230, 130))
        StartPosition = FormStartPosition.CenterParent
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False

        Dim y As Integer = 14
        TxtConcepto = Fila("Concepto:", y, New TextEdit With {.Text = conceptoInicial})
        y += 32

        If formato = FrmDatosBanco.Formato.Bac Then
            TxtCodigo = Fila("Código de cliente BAC:", y, New TextEdit)
            y += 32
            TxtConsecutivo = New SpinEdit With {.Value = 1}
            TxtConsecutivo.Properties.IsFloatValue = False
            TxtConsecutivo.Properties.MinValue = 0
            TxtConsecutivo.Properties.MaxValue = 99999
            Fila("Consecutivo:", y, TxtConsecutivo)
            y += 32
            TxtFecha = New DateEdit With {.DateTime = Date.Today}
            Fila("Fecha de pago:", y, TxtFecha)
            y += 32
            ChkDolares = New CheckEdit With {.Text = "Exportar en dólares (tasa de la fecha)", .Location = New Point(150, y), .Width = 220}
            Controls.Add(ChkDolares)
            y += 30
        Else
            TxtCuenta = Fila("Cuenta de la empresa:", y, New TextEdit)
            y += 32
        End If

        Dim aceptar As New SimpleButton With {.Text = "Generar", .Location = New Point(190, y + 4), .Size = New Size(85, 28), .DialogResult = DialogResult.None}
        Dim cancelar As New SimpleButton With {.Text = "Cancelar", .Location = New Point(285, y + 4), .Size = New Size(85, 28), .DialogResult = DialogResult.Cancel}
        AddHandler aceptar.Click, AddressOf Aceptar_Click
        Controls.AddRange(New Control() {aceptar, cancelar})
        AcceptButton = aceptar
        CancelButton = cancelar

        ClientSize = New Size(380, y + 44)

    End Sub

    Private Function Fila(texto As String, y As Integer, editor As BaseEdit) As Control
        Controls.Add(New LabelControl With {.Text = texto, .Location = New Point(12, y + 4)})
        editor.Location = New Point(150, y)
        editor.Width = 220
        Controls.Add(editor)
        Return editor
    End Function

    Private Sub Aceptar_Click(sender As Object, e As EventArgs)

        If Concepto = "" Then
            XtraMessageBox.Show("Escriba el concepto.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If _formato = Formato.Bac AndAlso CodigoCliente = "" Then
            XtraMessageBox.Show("Escriba el código de cliente BAC.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If _formato = Formato.CreditoDebito AndAlso CuentaEmpresa = "" Then
            XtraMessageBox.Show("Escriba la cuenta de la empresa.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        DialogResult = DialogResult.OK

    End Sub

End Class
