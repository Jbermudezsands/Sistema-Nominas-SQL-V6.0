Public Class Departamento

    Private CodDepartamento As String
    Private Departamento As String

    Public Property Cod_Departamento As String
        Get
            Return CodDepartamento
        End Get
        Set(value As String)
            CodDepartamento = value
        End Set
    End Property

    Public Property Descripcion_Departamento As String
        Get
            Return Departamento
        End Get
        Set(value As String)
            Departamento = value
        End Set
    End Property
End Class
