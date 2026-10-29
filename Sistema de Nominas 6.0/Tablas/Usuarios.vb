Public Class Usuarios
    Private CodUsuario As Integer
    Private NombreUsuario As String
    Private Clave As String
    Private NivelAcceso As Integer

    Public Property Cod_Usuario As Integer
        Get
            Return CodUsuario
        End Get
        Set(value As Integer)
            CodUsuario = value
        End Set
    End Property

    Public Property Nombre_Usuario As String
        Get
            Return NombreUsuario
        End Get
        Set(value As String)
            NombreUsuario = value
        End Set
    End Property

    Public Property Clave_Usuario As String
        Get
            Return Clave
        End Get
        Set(value As String)
            Clave = value
        End Set
    End Property

    Public Property Nivel_Acceso As Integer
        Get
            Return NivelAcceso
        End Get
        Set(value As Integer)
            NivelAcceso = value
        End Set
    End Property
End Class
