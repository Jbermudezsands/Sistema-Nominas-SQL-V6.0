Imports System.Data
Imports System.Data.SqlClient

Public Class CsTipoIncentivoDAL
    Inherits CsDALBase

    ''' <summary>Tipos 01 a 07 los usa el cálculo de nómina (días perdidos, subsidio, viáticos, vacaciones pagadas...): no se pueden eliminar.</summary>
    Public Shared Function EsProtegida(codigo As String) As Boolean
        Return {"01", "02", "03", "04", "05", "06", "07"}.Contains(codigo.Trim)
    End Function

    Public Function Listar() As DataTable
        Return ConsultarTabla("SELECT CodTipoIncentivo, Incentivo, Tipo, CuentaContable FROM TipoIncentivo ORDER BY CodTipoIncentivo")
    End Function

    Public Function ProximoCodigo() As String
        Return SiguienteCodigo("TipoIncentivo", "CodTipoIncentivo", 2)
    End Function

    Public Function Existe(codigo As String) As Boolean
        Return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM TipoIncentivo WHERE CodTipoIncentivo = @Cod",
                                       New SqlParameter("@Cod", codigo))) > 0
    End Function

    Public Function Buscar(codigo As String) As TipoIncentivo

        Dim dt As DataTable = ConsultarTabla(
            "SELECT CodTipoIncentivo, Incentivo, Tipo, CuentaContable FROM TipoIncentivo WHERE CodTipoIncentivo = @Cod",
            New SqlParameter("@Cod", codigo))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New TipoIncentivo With {
            .Cod_Tipo_Incentivo = Convert.ToString(r("CodTipoIncentivo")),
            .Descripcion_Incentivo = If(IsDBNull(r("Incentivo")), "", Convert.ToString(r("Incentivo"))),
            .Tipo_Incentivo = If(IsDBNull(r("Tipo")), "F", Convert.ToString(r("Tipo")).Trim),
            .Cuenta_Contable = If(IsDBNull(r("CuentaContable")), "", Convert.ToString(r("CuentaContable")))
        }

    End Function

    Public Sub Guardar(d As TipoIncentivo, esNuevo As Boolean)

        Dim cod As String = d.Cod_Tipo_Incentivo.Trim

        Dim pCod As New SqlParameter("@Cod", cod)
        Dim pDed As New SqlParameter("@Ded", d.Descripcion_Incentivo.Trim)
        Dim pTipo As New SqlParameter("@Tipo", If(d.Tipo_Incentivo = "V", "V", "F"))
        Dim pCta As New SqlParameter("@Cta", ValorODbNull(d.Cuenta_Contable))

        If esNuevo Then

            If Existe(cod) Then
                Throw New Exception("Ya existe un tipo de incentivo con el código " & cod & ".")
            End If

            Ejecutar("INSERT INTO TipoIncentivo (CodTipoIncentivo, Incentivo, Tipo, CuentaContable) " &
                     "VALUES (@Cod, @Ded, @Tipo, @Cta)", pCod, pDed, pTipo, pCta)

        Else
            Ejecutar("UPDATE TipoIncentivo SET Incentivo = @Ded, Tipo = @Tipo, CuentaContable = @Cta " &
                     "WHERE CodTipoIncentivo = @Cod", pCod, pDed, pTipo, pCta)
        End If

    End Sub

    Public Sub Eliminar(codigo As String)

        If EsProtegida(codigo) Then
            Throw New Exception("Este tipo de incentivo lo usa el cálculo de nómina y no puede ser eliminado.")
        End If

        Try
            Ejecutar("DELETE FROM TipoIncentivo WHERE CodTipoIncentivo = @Cod", New SqlParameter("@Cod", codigo))
        Catch ex As SqlException When EsEnUso(ex)
            Throw New Exception("No se puede eliminar el incentivo porque ya tiene movimientos registrados.")
        End Try

    End Sub

End Class
