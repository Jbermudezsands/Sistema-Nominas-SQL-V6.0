Imports DevExpress.Data.Linq.Helpers

Public Class Bitacora
    Private FechaHora As Date
    Private Usuario As String
    Private Modulo As String
    Private Accion As String

    Public Property Fecha_Hora As Date
        Get
            Return FechaHora
        End Get
        Set(value As Date)
            FechaHora = value
        End Set
    End Property

    Public Property Bitacora_Usuario As String
        Get
            Return Usuario
        End Get
        Set(value As String)
            Usuario = value
        End Set
    End Property

    Public Property Bitacora_Modulo As String
        Get
            Return Modulo
        End Get
        Set(value As String)
            Modulo = value
        End Set
    End Property

    Public Property Bitacora_Accion As String
        Get
            Return Accion
        End Get
        Set(value As String)
            Accion = value
        End Set
    End Property
End Class
