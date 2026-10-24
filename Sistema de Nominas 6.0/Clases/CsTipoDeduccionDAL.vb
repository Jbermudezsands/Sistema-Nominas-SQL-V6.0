Imports System.Data
Imports System.Data.SqlClient

Public Class CsTipoDeduccionDAL
    Inherits CsDALBase

    ''' <summary>Tipos 01 a 05 los usa el cálculo de nómina: no se borran y solo admiten cambiar la cuenta contable.</summary>
    Public Shared Function EsProtegida(codigo As String) As Boolean
        Return {"01", "02", "03", "04", "05"}.Contains(codigo.Trim)
    End Function

    Public Function Listar() As DataTable
        Return ConsultarTabla("SELECT CodTipoDeduccion, Deduccion, Tipo, CuentaContable FROM TipoDeduccion ORDER BY CodTipoDeduccion")
    End Function

    Public Function ProximoCodigo() As String
        Return SiguienteCodigo("TipoDeduccion", "CodTipoDeduccion", 2)
    End Function

    Public Function Existe(codigo As String) As Boolean
        Return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM TipoDeduccion WHERE CodTipoDeduccion = @Cod",
                                       New SqlParameter("@Cod", codigo))) > 0
    End Function

    Public Function Buscar(codigo As String) As TipoDeduccion

        Dim dt As DataTable = ConsultarTabla(
            "SELECT CodTipoDeduccion, Deduccion, Tipo, CuentaContable FROM TipoDeduccion WHERE CodTipoDeduccion = @Cod",
            New SqlParameter("@Cod", codigo))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New TipoDeduccion With {
            .Cod_Tipo_Deduccion = Convert.ToString(r("CodTipoDeduccion")),
            .Descripcion_Deduccion = If(IsDBNull(r("Deduccion")), "", Convert.ToString(r("Deduccion"))),
            .Tipo_Deduccion = If(IsDBNull(r("Tipo")), "F", Convert.ToString(r("Tipo")).Trim),
            .Cuenta_Contable = If(IsDBNull(r("CuentaContable")), "", Convert.ToString(r("CuentaContable")))
        }

    End Function

    Public Sub Guardar(d As TipoDeduccion, esNuevo As Boolean)

        Dim cod As String = d.Cod_Tipo_Deduccion.Trim

        Dim pCod As New SqlParameter("@Cod", cod)
        Dim pDed As New SqlParameter("@Ded", d.Descripcion_Deduccion.Trim)
        Dim pTipo As New SqlParameter("@Tipo", If(d.Tipo_Deduccion = "V", "V", "F"))
        Dim pCta As New SqlParameter("@Cta", ValorODbNull(d.Cuenta_Contable))

        If esNuevo Then

            If Existe(cod) Then
                Throw New Exception("Ya existe un tipo de deducción con el código " & cod & ".")
            End If

            Ejecutar("INSERT INTO TipoDeduccion (CodTipoDeduccion, Deduccion, Tipo, CuentaContable) " &
                     "VALUES (@Cod, @Ded, @Tipo, @Cta)", pCod, pDed, pTipo, pCta)

        ElseIf EsProtegida(cod) Then

            ' Solo se permite la cuenta contable (regla heredada de la V.5.0).
            Ejecutar("UPDATE TipoDeduccion SET CuentaContable = @Cta WHERE CodTipoDeduccion = @Cod", pCod, pCta)

        Else
            Ejecutar("UPDATE TipoDeduccion SET Deduccion = @Ded, Tipo = @Tipo, CuentaContable = @Cta " &
                     "WHERE CodTipoDeduccion = @Cod", pCod, pDed, pTipo, pCta)
        End If

    End Sub

    Public Sub Eliminar(codigo As String)

        If EsProtegida(codigo) Then
            Throw New Exception("Este tipo de deducción no puede ser eliminado.")
        End If

        Try
            Ejecutar("DELETE FROM TipoDeduccion WHERE CodTipoDeduccion = @Cod", New SqlParameter("@Cod", codigo))
        Catch ex As SqlException When EsEnUso(ex)
            Throw New Exception("No se puede eliminar la deducción porque ya tiene movimientos registrados.")
        End Try

    End Sub

End Class
