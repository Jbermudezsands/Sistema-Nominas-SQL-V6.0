Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Data.SqlClient


Public Class Conexion

    'EntCompañias EntCompañias = New EntCompañias();
    Public RutaBD As String = My.Application.Info.DirectoryPath & "\CntNominas.dll"
    Private Shared CadenaConexion As String


    Public Property Cadena_Conexion As String
        Get
            Return CadenaConexion
        End Get
        Set(value As String)
            CadenaConexion = value
        End Set
    End Property



    Public Function conexionAccess() As String
        Dim Conexion As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + RutaBD
        Return Conexion
    End Function

    Public Function ConexionSQL() As String
        Dim Conexion As String = Cadena_Conexion
        Return Conexion
    End Function

    Public Function MiConexion_Access() As OleDbConnection
        Dim MiConexionAccess As OleDbConnection = New OleDbConnection(conexionAccess())
        Return MiConexionAccess
    End Function

    Public Function MiConexion_SQl() As SqlConnection
        Dim MiConexionSQL As SqlConnection = New SqlConnection(ConexionSQL())
        Return MiConexionSQL
    End Function



















End Class
