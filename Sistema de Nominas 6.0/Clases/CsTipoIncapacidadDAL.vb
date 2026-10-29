Imports System.Data
Imports System.Data.SqlClient

Public Class CsTipoIncapacidadDAL
    Inherits CsDALBase

    Public Function Listar() As DataTable
        Return ConsultarTabla("SELECT CodIncapacidad, Incapacidad FROM TipoIncapacidad ORDER BY CodIncapacidad")
    End Function

    Public Function ProximoCodigo() As String
        Return SiguienteCodigo("TipoIncapacidad", "CodIncapacidad", 2)
    End Function

    Public Function Existe(codigo As String) As Boolean
        Return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM TipoIncapacidad WHERE CodIncapacidad = @Cod",
                                       New SqlParameter("@Cod", codigo))) > 0
    End Function

    Public Function Buscar(codigo As String) As TipoIncapacidad

        Dim dt As DataTable = ConsultarTabla(
            "SELECT CodIncapacidad, Incapacidad FROM TipoIncapacidad WHERE CodIncapacidad = @Cod",
            New SqlParameter("@Cod", codigo))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New TipoIncapacidad With {
            .Cod_Incapacidad = Convert.ToString(r("CodIncapacidad")),
            .Descripcion_Incapacidad = If(IsDBNull(r("Incapacidad")), "", Convert.ToString(r("Incapacidad")))
        }

    End Function

    Public Sub Guardar(t As TipoIncapacidad, esNuevo As Boolean)

        Dim pCod As New SqlParameter("@Cod", t.Cod_Incapacidad.Trim)
        Dim pDesc As New SqlParameter("@Desc", t.Descripcion_Incapacidad.Trim)

        If esNuevo Then

            If Existe(t.Cod_Incapacidad.Trim) Then
                Throw New Exception("Ya existe un tipo de incapacidad con el código " & t.Cod_Incapacidad.Trim & ".")
            End If

            Ejecutar("INSERT INTO TipoIncapacidad (CodIncapacidad, Incapacidad) VALUES (@Cod, @Desc)", pCod, pDesc)
        Else
            Ejecutar("UPDATE TipoIncapacidad SET Incapacidad = @Desc WHERE CodIncapacidad = @Cod", pCod, pDesc)
        End If

    End Sub

    Public Sub Eliminar(codigo As String)

        Try
            Ejecutar("DELETE FROM TipoIncapacidad WHERE CodIncapacidad = @Cod", New SqlParameter("@Cod", codigo))
        Catch ex As SqlException When EsEnUso(ex)
            Throw New Exception("No se puede eliminar el tipo de incapacidad porque tiene incapacidades registradas.")
        End Try

    End Sub

End Class
