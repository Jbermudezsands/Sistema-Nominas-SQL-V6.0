Imports System.Data
Imports System.Data.SqlClient

''' <summary>Catálogo de grupos de empleados (tabla Grupo; V.5.0 FrmGrupo/FrmTipoDivision).</summary>
Public Class CsGrupoDAL
    Inherits CsDALBase

    Public Function Listar() As DataTable
        Return ConsultarTabla("SELECT CodGrupo, Grupo FROM Grupo ORDER BY CodGrupo")
    End Function

    Public Function ProximoCodigo() As String
        Return SiguienteCodigo("Grupo", "CodGrupo", 3)
    End Function

    Public Function Existe(codigo As String) As Boolean
        Return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Grupo WHERE CodGrupo = @Cod",
                                       New SqlParameter("@Cod", codigo))) > 0
    End Function

    Public Function Buscar(codigo As String) As Grupos

        Dim dt As DataTable = ConsultarTabla("SELECT CodGrupo, Grupo FROM Grupo WHERE CodGrupo = @Cod",
                                             New SqlParameter("@Cod", codigo))
        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New Grupos With {
            .Cod_Grupo = Convert.ToString(r("CodGrupo")),
            .Descripcion_Grupo = If(IsDBNull(r("Grupo")), "", Convert.ToString(r("Grupo")))
        }

    End Function

    Public Sub Guardar(g As Grupos, esNuevo As Boolean)

        Dim pCod As New SqlParameter("@Cod", g.Cod_Grupo.Trim)
        Dim pNom As New SqlParameter("@Nom", g.Descripcion_Grupo.Trim)

        If esNuevo Then
            If Existe(g.Cod_Grupo.Trim) Then
                Throw New Exception("Ya existe un grupo con el código " & g.Cod_Grupo.Trim & ".")
            End If
            Ejecutar("INSERT INTO Grupo (CodGrupo, Grupo) VALUES (@Cod, @Nom)", pCod, pNom)
        Else
            Ejecutar("UPDATE Grupo SET Grupo = @Nom WHERE CodGrupo = @Cod", pCod, pNom)
        End If

    End Sub

    Public Sub Eliminar(codigo As String)

        ' Grupo no tiene llave foránea: se revisa a mano que ningún empleado lo use.
        Dim usados As Integer = Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Empleado WHERE CodGrupo = @Cod",
                                                        New SqlParameter("@Cod", codigo)))
        If usados > 0 Then
            Throw New Exception("No se puede eliminar el grupo porque está asignado a " & usados & " empleado(s).")
        End If

        Ejecutar("DELETE FROM Grupo WHERE CodGrupo = @Cod", New SqlParameter("@Cod", codigo))

    End Sub

End Class
