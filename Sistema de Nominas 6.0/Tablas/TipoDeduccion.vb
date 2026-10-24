Public Class TipoDeduccion

    Private CodTipoDeduccion As String
    Private Deduccion As String
    Private Tipo As String
    Private CuentaContable As String

    Public Property Cod_Tipo_Deduccion As String
        Get
            Return CodTipoDeduccion
        End Get
        Set(value As String)
            CodTipoDeduccion = value
        End Set
    End Property

    Public Property Descripcion_Deduccion As String
        Get
            Return Deduccion
        End Get
        Set(value As String)
            Deduccion = value
        End Set
    End Property

    ''' <summary>"F" = fija, "V" = variable.</summary>
    Public Property Tipo_Deduccion As String
        Get
            Return Tipo
        End Get
        Set(value As String)
            Tipo = value
        End Set
    End Property

    Public Property Cuenta_Contable As String
        Get
            Return CuentaContable
        End Get
        Set(value As String)
            CuentaContable = value
        End Set
    End Property
End Class
