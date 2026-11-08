Public Class TipoIncentivo

    Private CodTipoIncentivo As String
    Private Incentivo As String
    Private Tipo As String
    Private CuentaContable As String

    Public Property Cod_Tipo_Incentivo As String
        Get
            Return CodTipoIncentivo
        End Get
        Set(value As String)
            CodTipoIncentivo = value
        End Set
    End Property

    Public Property Descripcion_Incentivo As String
        Get
            Return Incentivo
        End Get
        Set(value As String)
            Incentivo = value
        End Set
    End Property

    ''' <summary>"F" = fija, "V" = variable.</summary>
    Public Property Tipo_Incentivo As String
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
