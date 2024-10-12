Public Class Cargos
    Private CodCargo As String
    Private Cargo As String
    Private Monto As String

    Public Property Cod_Cargo As String
        Get
            Return CodCargo
        End Get
        Set(value As String)
            CodCargo = value
        End Set
    End Property

    Public Property Descripcion_Cargo As String
        Get
            Return Cargo
        End Get
        Set(value As String)
            Cargo = value
        End Set
    End Property

    Public Property Monto_Cargo As String
        Get
            Return Monto
        End Get
        Set(value As String)
            Monto = value
        End Set
    End Property
End Class
