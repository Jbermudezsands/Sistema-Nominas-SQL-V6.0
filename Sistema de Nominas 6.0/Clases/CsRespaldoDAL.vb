Imports System.Data.SqlClient
Imports System.IO

''' <summary>
''' Respaldo completo (BACKUP DATABASE) de la base de datos de la compañía conectada.
''' El nombre de la base se toma de la cadena de conexión elegida en runtime y la ruta viaja como
''' parámetro; el nombre de la base se cita con QUOTENAME-equivalente porque T-SQL no la admite como parámetro.
''' </summary>
Public Class CsRespaldoDAL
    Inherits CsDALBase

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>Base de datos (Initial Catalog) a la que apunta la conexión actual.</summary>
    Public Function NombreBaseDatos() As String
        Return New SqlConnectionStringBuilder(CadenaConexion).InitialCatalog
    End Function

    ''' <summary>Carpeta por defecto de los respaldos: \Respaldos junto al ejecutable.</summary>
    Public Shared Function CarpetaPorDefecto() As String
        Return Path.Combine(My.Application.Info.DirectoryPath, "Respaldos")
    End Function

    ''' <summary>Ruta sugerida: carpeta por defecto + BaseDatos_yyyyMMdd_HHmmss.bak.</summary>
    Public Function RutaSugerida() As String
        Return Path.Combine(CarpetaPorDefecto(), NombreBaseDatos() & "_" & DateTime.Now.ToString("yyyyMMdd_HHmmss") & ".bak")
    End Function

    ''' <summary>
    ''' Genera el respaldo completo en <paramref name="ruta"/>. SQL Server escribe el archivo en el equipo donde
    ''' corre el servidor, por lo que la ruta debe ser válida y con permisos desde allí.
    ''' </summary>
    Public Sub Respaldar(ruta As String)

        Dim bd As String = NombreBaseDatos()

        If String.IsNullOrWhiteSpace(bd) Then
            Throw New InvalidOperationException("No se pudo determinar la base de datos a respaldar.")
        End If

        If String.IsNullOrWhiteSpace(ruta) Then
            Throw New ArgumentException("Debe indicar la ruta donde se guardará el respaldo.")
        End If

        CrearCarpetaSiEsLocal(ruta)

        Dim sql As String = "BACKUP DATABASE [" & bd.Replace("]", "]]") & "] TO DISK = @ruta " &
                            "WITH INIT, NAME = @nombre, STATS = 10"

        Using cn As New SqlConnection(CadenaConexion)
            Using cmd As New SqlCommand(sql, cn)
                cmd.CommandTimeout = 0
                cmd.Parameters.AddWithValue("@ruta", ruta)
                cmd.Parameters.AddWithValue("@nombre", bd & "-Full Database Backup")
                cn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

    End Sub

    ''' <summary>Si el servidor es este equipo, crea la carpeta destino; en un servidor remoto no se puede hacer desde aquí.</summary>
    Private Sub CrearCarpetaSiEsLocal(ruta As String)

        Try
            Dim carpeta As String = Path.GetDirectoryName(ruta)
            If Not String.IsNullOrEmpty(carpeta) AndAlso Not Directory.Exists(carpeta) Then
                Directory.CreateDirectory(carpeta)
            End If
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException OrElse TypeOf ex Is ArgumentException
            ' Ruta no accesible desde el cliente: se deja que SQL Server la valide al ejecutar el BACKUP.
        End Try

    End Sub

End Class
