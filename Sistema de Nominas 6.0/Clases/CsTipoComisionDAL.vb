Imports System.Data
Imports System.Data.SqlClient

Public Class CsTipoComisionDAL
    Inherits CsDALBase

    Public Function Listar() As DataTable
        Return ConsultarTabla("SELECT CodTipoComision, Comision FROM TipoComision ORDER BY CodTipoComision")
    End Function

    Public Function ProximoCodigo() As String
        Return SiguienteCodigo("TipoComision", "CodTipoComision", 3)
    End Function

    Public Function Existe(codigo As String) As Boolean
        Return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM TipoComision WHERE CodTipoComision = @Cod",
                                       New SqlParameter("@Cod", codigo))) > 0
    End Function

    Public Function Buscar(codigo As String) As TipoComision

        Dim dt As DataTable = ConsultarTabla(
            "SELECT CodTipoComision, Comision, Porcentaje, CuentaContable FROM TipoComision WHERE CodTipoComision = @Cod",
            New SqlParameter("@Cod", codigo))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New TipoComision With {
            .Cod_Tipo_Comision = Convert.ToString(r("CodTipoComision")),
            .Descripcion_Comision = If(IsDBNull(r("Comision")), "", Convert.ToString(r("Comision"))),
            .Porcentaje = If(IsDBNull(r("Porcentaje")), 0D, Math.Round(Convert.ToDecimal(r("Porcentaje")), 6)),
            .Cuenta_Contable = If(IsDBNull(r("CuentaContable")), "", Convert.ToString(r("CuentaContable")))
        }

    End Function

    Public Sub Guardar(t As TipoComision, esNuevo As Boolean)

        Dim pCod As New SqlParameter("@Cod", t.Cod_Tipo_Comision.Trim)
        Dim pDesc As New SqlParameter("@Desc", t.Descripcion_Comision.Trim)
        Dim pPorc As New SqlParameter("@Porc", SqlDbType.Real) With {.Value = Convert.ToSingle(t.Porcentaje)}
        Dim pCuenta As New SqlParameter("@Cuenta", ValorODbNull(t.Cuenta_Contable))

        If esNuevo Then

            If Existe(t.Cod_Tipo_Comision.Trim) Then
                Throw New Exception("Ya existe un tipo de comisión con el código " & t.Cod_Tipo_Comision.Trim & ".")
            End If

            Ejecutar("INSERT INTO TipoComision (CodTipoComision, Comision, Porcentaje, CuentaContable) " &
                     "VALUES (@Cod, @Desc, @Porc, @Cuenta)", pCod, pDesc, pPorc, pCuenta)
        Else
            Ejecutar("UPDATE TipoComision SET Comision = @Desc, Porcentaje = @Porc, CuentaContable = @Cuenta " &
                     "WHERE CodTipoComision = @Cod", pCod, pDesc, pPorc, pCuenta)
        End If

    End Sub

    Public Sub Eliminar(codigo As String)

        Try
            Ejecutar("DELETE FROM TipoComision WHERE CodTipoComision = @Cod", New SqlParameter("@Cod", codigo))
        Catch ex As SqlException When EsEnUso(ex)
            Throw New Exception("No se puede eliminar el tipo de comisión porque tiene comisiones registradas.")
        End Try

    End Sub

End Class
