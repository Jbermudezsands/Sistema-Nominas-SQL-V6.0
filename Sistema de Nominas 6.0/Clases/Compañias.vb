Imports System
Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient


Public Class Compañias
    Dim MiConexion As New Conexion

    Private Shared NombreUsuario As String
    Private Shared Contraseña As String
    Private Shared NombreCompañia As String
    Private Shared NivelAcceso As String

    Public Property Nombre_Usuario As String
        Get
            Return NombreUsuario
        End Get
        Set(value As String)
            NombreUsuario = value
        End Set
    End Property

    Public Property Contraseña_Usuario As String
        Get
            Return Contraseña
        End Get
        Set(value As String)
            Contraseña = value
        End Set
    End Property

    Public Property Nombre_Compañia As String
        Get
            Return NombreCompañia
        End Get
        Set(value As String)
            NombreCompañia = value
        End Set
    End Property

    Public Property Nivel_Acceso As String
        Get
            Return NivelAcceso
        End Get
        Set(value As String)
            NivelAcceso = value
        End Set
    End Property

    Public Function ConsultaCompañia() As Double
        'Creo la Conexion con el archivo que tiene el listado de Compañias.

        Dim StrSQLAccess As String, DataAdapterAccess As New OleDbDataAdapter
        Dim DataSetCompañias As New DataSet


        MiConexion.MiConexion_Access.Open()
        StrSQLAccess = "Select * From Servidor"
        DataAdapterAccess = New OleDbDataAdapter(StrSQLAccess, MiConexion.MiConexion_Access)

        DataSetCompañias = New DataSet()

        DataAdapterAccess.Fill(DataSetCompañias, "Compañias")
        MiConexion.MiConexion_Access.Close()
        Return DataSetCompañias.Tables("Compañias").Rows.Count
    End Function

    Public Function ListadoCompañia() As DataTable

        Dim StrSQLAccess As String
        Dim DataAdapterAccess As New OleDbDataAdapter
        Dim DataSetCompañias = New DataSet()


        MiConexion.MiConexion_Access.Open()
        StrSQLAccess = "Select * From Servidor"
        DataAdapterAccess = New OleDbDataAdapter(StrSQLAccess, MiConexion.MiConexion_Access)

        DataAdapterAccess.Fill(DataSetCompañias, "Compañias")
        MiConexion.MiConexion_Access.Close()
        Return DataSetCompañias.Tables("Compañias")

    End Function

    Public Function BuscaCompañia(NombreCompañia As String) As String

        Dim StrSQLAccess As String, Count As Double = 0, Cadena As String = ""
        Dim DataAdapterAccess As New OleDbDataAdapter, Comando As OleDbCommand, Compañia As DataTable
        Dim DataSetCompañias = New DataSet()


        Try
            MiConexion.MiConexion_Access.Open()
            StrSQLAccess = "Select * From Servidor Where (NombreBD= '" & NombreCompañia & "') "
            Comando = New OleDbCommand(StrSQLAccess, MiConexion.MiConexion_Access)

            DataAdapterAccess = New OleDbDataAdapter(Comando)
            Compañia = New DataTable()
            DataAdapterAccess.Fill(Compañia)
            Count = Compañia.Rows.Count
            If Count = 0 Then
                Cadena = ""
            Else
                Cadena = Compañia.Rows(0)("Servidor").ToString()
            End If

            Return Cadena


            MiConexion.MiConexion_Access.Close()


        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Throw
        End Try
    End Function


    Public Function CantUsuario() As Double

        Dim Contador As Double, SQLString As String, DataAdapter As SqlDataAdapter, DataSet As New DataSet

        MiConexion.MiConexion_SQl.Open()
        SQLString = "SELECT * FROM Usuarios"
        DataAdapter = New SqlDataAdapter(SQLString, MiConexion.MiConexion_SQl)
        DataSet = New DataSet()
        DataAdapter.Fill(DataSet, "Usuarios")
        Contador = DataSet.Tables("Usuarios").Rows.Count
        MiConexion.MiConexion_SQl.Close()
        Return Contador

    End Function

    Public Function ContraseñaCorrecta(NombreUsuario As String, Contraseña As String) As Boolean

        Dim Respuesta As Boolean, SQLString As String, DataAdapter As SqlDataAdapter, DataSet As New DataSet

        MiConexion.MiConexion_SQl.Open()
        SQLString = "SELECT * FROM Usuarios WHERE(NombreUsuario = '" & NombreUsuario & "') AND(Clave = '" & Contraseña & "')"
        DataAdapter = New SqlDataAdapter(SQLString, MiConexion.MiConexion_SQl)
        DataSet = New DataSet()
        DataAdapter.Fill(DataSet, "Usuarios")
        If (DataSet.Tables("Usuarios").Rows.Count = 0) Then
            Respuesta = False
        Else
            Nombre_Usuario = NombreUsuario
            Contraseña_Usuario = Contraseña
            Nivel_Acceso = DataSet.Tables("Usuarios").Rows(0)("NivelAcceso").ToString()
            Respuesta = True
        End If

        MiConexion.MiConexion_SQl.Close()

        Return Respuesta

    End Function

End Class
