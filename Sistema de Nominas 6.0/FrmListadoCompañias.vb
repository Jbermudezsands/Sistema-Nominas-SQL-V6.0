Public Class FrmListadoCompañias
    Public Compañia As New Compañias(), MiConexion As New Conexion()


    Private Sub FrmListadoCompañias_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        listBoxCompañia.DisplayMember = "NombreBD"
        listBoxCompañia.DataSource = Compañia.ListadoCompañia()
    End Sub

    Private Sub BtnCancelar_Click(sender As Object, e As EventArgs) Handles BtnCancelar.Click
        Me.Close()
    End Sub

    Private Sub BtnSeleccionar_Click(sender As Object, e As EventArgs) Handles BtnSeleccionar.Click
        Dim CadenaConexionSQL As String = ""
        Compañia.Nombre_Compañia = listBoxCompañia.Text

        '***************************************************************************************************************
        'BUSCO SI LA COMPAÑIA EXISTE ************************************************
        '***************************************************************************************************************
        CadenaConexionSQL = Compañia.BuscaCompañia(Compañia.Nombre_Compañia)
        If (CadenaConexionSQL = "") Then
            MsgBox("Error al intentar Conectarse", "Zeus Nominas", MessageBoxButtons.OK)
        Else
            MiConexion.Cadena_Conexion = CadenaConexionSQL
            If (Compañia.CantUsuario() = 0) Then
                Main.ShowDialog()
            Else
                Me.Hide()
                FrmEntrada.ShowDialog()
                Me.Close()
            End If

        End If



    End Sub
End Class