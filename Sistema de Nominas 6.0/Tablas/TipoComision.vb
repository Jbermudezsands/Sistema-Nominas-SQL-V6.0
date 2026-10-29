''' <summary>Tipo de comisión (tabla TipoComision).</summary>
Public Class TipoComision

    Public Property Cod_Tipo_Comision As String = ""
    Public Property Descripcion_Comision As String = ""

    ''' <summary>Porcentaje como fracción (0.15 = 15%), tal como se guarda en la tabla.</summary>
    Public Property Porcentaje As Decimal
    Public Property Cuenta_Contable As String = ""

End Class
