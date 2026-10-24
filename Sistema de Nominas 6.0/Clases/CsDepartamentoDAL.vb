Imports System.Data
Imports System.Data.SqlClient

Public Class CsDepartamentoDAL
    Inherits CsDALBase

    Public Function Listar() As DataTable
        Return ConsultarTabla("SELECT CodDepartamento, Departamento FROM Departamento ORDER BY CodDepartamento")
    End Function

    Public Function ProximoCodigo() As String
        Return SiguienteCodigo("Departamento", "CodDepartamento", 3)
    End Function

    Public Function Existe(codigo As String) As Boolean
        Return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Departamento WHERE CodDepartamento = @Cod",
                                       New SqlParameter("@Cod", codigo))) > 0
    End Function

    Public Function Buscar(codigo As String) As Departamento

        Dim dt As DataTable = ConsultarTabla(
            "SELECT CodDepartamento, Departamento FROM Departamento WHERE CodDepartamento = @Cod",
            New SqlParameter("@Cod", codigo))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New Departamento With {
            .Cod_Departamento = Convert.ToString(r("CodDepartamento")),
            .Descripcion_Departamento = If(IsDBNull(r("Departamento")), "", Convert.ToString(r("Departamento")))
        }

    End Function

    Public Sub Guardar(d As Departamento, esNuevo As Boolean)

        Dim pCod As New SqlParameter("@Cod", d.Cod_Departamento.Trim)
        Dim pDep As New SqlParameter("@Dep", d.Descripcion_Departamento.Trim)

        If esNuevo Then

            If Existe(d.Cod_Departamento.Trim) Then
                Throw New Exception("Ya existe un departamento con el código " & d.Cod_Departamento.Trim & ".")
            End If

            Ejecutar("INSERT INTO Departamento (CodDepartamento, Departamento) VALUES (@Cod, @Dep)", pCod, pDep)
        Else
            Ejecutar("UPDATE Departamento SET Departamento = @Dep WHERE CodDepartamento = @Cod", pCod, pDep)
        End If

    End Sub

    Public Sub Eliminar(codigo As String)

        Try
            Ejecutar("DELETE FROM Departamento WHERE CodDepartamento = @Cod", New SqlParameter("@Cod", codigo))
        Catch ex As SqlException When EsEnUso(ex)
            Throw New Exception("No se puede eliminar el departamento porque tiene empleados o registros asociados.")
        End Try

    End Sub

End Class
