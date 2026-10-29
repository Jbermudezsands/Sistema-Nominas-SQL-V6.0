Imports System.Data
Imports System.Data.SqlClient

Public Class CsTipoDestajoDAL
    Inherits CsDALBase

    Public Function Listar() As DataTable
        Return ConsultarTabla("SELECT CodTipoDestajo, Destajo FROM TipoDestajo ORDER BY CodTipoDestajo")
    End Function

    Public Function ProximoCodigo() As String
        Return SiguienteCodigo("TipoDestajo", "CodTipoDestajo", 3)
    End Function

    Public Function Existe(codigo As String) As Boolean
        Return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM TipoDestajo WHERE CodTipoDestajo = @Cod",
                                       New SqlParameter("@Cod", codigo))) > 0
    End Function

    Public Function Buscar(codigo As String) As TipoDestajo

        Dim dt As DataTable = ConsultarTabla(
            "SELECT CodTipoDestajo, Destajo, Monto, CuentaContable FROM TipoDestajo WHERE CodTipoDestajo = @Cod",
            New SqlParameter("@Cod", codigo))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New TipoDestajo With {
            .Cod_Tipo_Destajo = Convert.ToString(r("CodTipoDestajo")),
            .Descripcion_Destajo = If(IsDBNull(r("Destajo")), "", Convert.ToString(r("Destajo"))),
            .Monto = If(IsDBNull(r("Monto")), 0D, Convert.ToDecimal(r("Monto"))),
            .Cuenta_Contable = If(IsDBNull(r("CuentaContable")), "", Convert.ToString(r("CuentaContable")))
        }

    End Function

    Public Sub Guardar(t As TipoDestajo, esNuevo As Boolean)

        Dim pCod As New SqlParameter("@Cod", t.Cod_Tipo_Destajo.Trim)
        Dim pDesc As New SqlParameter("@Desc", t.Descripcion_Destajo.Trim)
        Dim pMonto As New SqlParameter("@Monto", SqlDbType.Float) With {.Value = Convert.ToDouble(t.Monto)}
        Dim pCuenta As New SqlParameter("@Cuenta", ValorODbNull(t.Cuenta_Contable))

        If esNuevo Then

            If Existe(t.Cod_Tipo_Destajo.Trim) Then
                Throw New Exception("Ya existe un tipo de destajo con el código " & t.Cod_Tipo_Destajo.Trim & ".")
            End If

            Ejecutar("INSERT INTO TipoDestajo (CodTipoDestajo, Destajo, Monto, CuentaContable) " &
                     "VALUES (@Cod, @Desc, @Monto, @Cuenta)", pCod, pDesc, pMonto, pCuenta)
        Else
            Ejecutar("UPDATE TipoDestajo SET Destajo = @Desc, Monto = @Monto, CuentaContable = @Cuenta " &
                     "WHERE CodTipoDestajo = @Cod", pCod, pDesc, pMonto, pCuenta)
        End If

    End Sub

    ''' <summary>"Repetir" de la V.5.0: deja el mismo monto a todos los tipos de destajo. Devuelve cuántos cambió.</summary>
    Public Function AplicarMontoATodos(monto As Decimal) As Integer
        Return Ejecutar("UPDATE TipoDestajo SET Monto = @Monto",
                        New SqlParameter("@Monto", SqlDbType.Float) With {.Value = Convert.ToDouble(monto)})
    End Function

    Public Sub Eliminar(codigo As String)

        Try
            Ejecutar("DELETE FROM TipoDestajo WHERE CodTipoDestajo = @Cod", New SqlParameter("@Cod", codigo))
        Catch ex As SqlException When EsEnUso(ex)
            Throw New Exception("No se puede eliminar el tipo de destajo porque tiene destajos registrados.")
        End Try

    End Sub

End Class
