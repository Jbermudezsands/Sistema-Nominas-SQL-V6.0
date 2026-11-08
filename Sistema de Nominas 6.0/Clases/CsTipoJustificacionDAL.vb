Imports System.Data
Imports System.Data.SqlClient

''' <summary>Tipos de justificación (tabla LeaveClass; V.5.0 FrmRegistroJustifica). Los usa el combo de FrmJustificaciones.</summary>
Public Class CsTipoJustificacionDAL
    Inherits CsDALBase

    Public Const LongNombre As Integer = 50

    Public Function Listar() As DataTable
        Return ConsultarTabla("SELECT Classid, Classname FROM LeaveClass ORDER BY Classid")
    End Function

    Public Function Buscar(codigo As Integer) As TipoJustificacion

        Dim dt As DataTable = ConsultarTabla("SELECT Classid, Classname FROM LeaveClass WHERE Classid = @Id",
                                             New SqlParameter("@Id", codigo))
        If dt.Rows.Count = 0 Then Return Nothing

        Return New TipoJustificacion With {
            .Cod_Tipo = Convert.ToInt32(dt.Rows(0)("Classid")),
            .Nombre = Convert.ToString(dt.Rows(0)("Classname"))
        }

    End Function

    ''' <summary>Inserta (Cod_Tipo = 0) o actualiza y devuelve el código.</summary>
    Public Function Guardar(t As TipoJustificacion) As Integer

        Dim nombre As String = If(t.Nombre, "").Trim

        If nombre = "" Then Throw New InvalidOperationException("Escriba la descripción del tipo de justificación.")
        If nombre.Length > LongNombre Then
            Throw New InvalidOperationException("La descripción admite como máximo " & LongNombre & " caracteres.")
        End If

        If Convert.ToInt32(Escalar("SELECT COUNT(*) FROM LeaveClass WHERE Classname = @N AND Classid <> @Id",
                                   New SqlParameter("@N", nombre), New SqlParameter("@Id", t.Cod_Tipo))) > 0 Then
            Throw New InvalidOperationException("Ya existe un tipo de justificación con esa descripción.")
        End If

        If t.Cod_Tipo = 0 Then
            Return Convert.ToInt32(Escalar(
                "INSERT INTO LeaveClass (Classname, ViewColor) OUTPUT INSERTED.Classid VALUES (@N, 16777215)",
                New SqlParameter("@N", nombre)))
        End If

        Ejecutar("UPDATE LeaveClass SET Classname = @N WHERE Classid = @Id",
                 New SqlParameter("@N", nombre), New SqlParameter("@Id", t.Cod_Tipo))
        Return t.Cod_Tipo

    End Function

    Public Sub Eliminar(codigo As Integer)
        Try
            Ejecutar("DELETE FROM LeaveClass WHERE Classid = @Id", New SqlParameter("@Id", codigo))
        Catch ex As SqlException When EsEnUso(ex)
            Throw New Exception("No se puede eliminar el tipo porque tiene justificaciones registradas.")
        End Try
    End Sub

End Class
