Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Justificaciones de asistencia (UserLeave) y sus tipos (LeaveClass). El empleado se enlaza con
''' el reloj biométrico por Userinfo.IDCard = Empleado.CodEmpleado1.
''' </summary>
Public Class CsJustificacionDAL
    Inherits CsDALBase

    Public Const LongTipo As Integer = 50
    Public Const LongMotivo As Integer = 250

    Public Function ListarEmpleados() As DataTable
        Return ConsultarTabla(
            "SELECT CodEmpleado1, " &
            "LTRIM(RTRIM(ISNULL(Nombre1,'') + ' ' + ISNULL(Nombre2,'') + ' ' + ISNULL(Apellido1,'') + ' ' + ISNULL(Apellido2,''))) AS Nombre " &
            "FROM Empleado WHERE Activo = 1 ORDER BY CodEmpleado1")
    End Function

    Public Function ListarTipos() As DataTable
        Return ConsultarTabla("SELECT Classid, Classname FROM LeaveClass ORDER BY Classname")
    End Function

    ''' <summary>Crea un tipo de justificación y devuelve su código.</summary>
    Public Function AgregarTipo(nombre As String) As Integer

        If String.IsNullOrWhiteSpace(nombre) Then
            Throw New InvalidOperationException("Escriba la descripción del tipo de justificación.")
        End If

        If nombre.Trim().Length > LongTipo Then
            Throw New InvalidOperationException("La descripción del tipo admite como máximo " & LongTipo & " caracteres.")
        End If

        Dim id As Object = Escalar(
            "INSERT INTO LeaveClass (Classname, ViewColor) OUTPUT INSERTED.Classid VALUES (@N, 16777215)",
            New SqlParameter("@N", nombre.Trim()))

        Return Convert.ToInt32(id)

    End Function

    Public Function ListarPorEmpleado(codEmpleado1 As String) As DataTable
        Return ConsultarTabla(
            "SELECT ul.Lsh, ul.BeginTime AS Inicio, ul.EndTime AS Fin, lc.Classname AS Tipo, ul.Whys AS Motivo " &
            "FROM UserLeave ul INNER JOIN Userinfo u ON u.Userid = ul.Userid " &
            "INNER JOIN LeaveClass lc ON lc.Classid = ul.LeaveClassid " &
            "WHERE u.IDCard = @Cod ORDER BY ul.BeginTime",
            New SqlParameter("@Cod", codEmpleado1))
    End Function

    ''' <summary>Registra la justificación. Lanza si el empleado no está en el reloj (Userinfo) o las fechas no son válidas.</summary>
    Public Function Agregar(j As Justificacion) As Integer

        If j.Fin < j.Inicio Then
            Throw New InvalidOperationException("La fecha final no puede ser anterior a la inicial.")
        End If

        If j.Motivo IsNot Nothing AndAlso j.Motivo.Length > LongMotivo Then
            Throw New InvalidOperationException("El motivo admite como máximo " & LongMotivo & " caracteres.")
        End If

        Dim userId As Object = Escalar("SELECT TOP 1 Userid FROM Userinfo WHERE IDCard = @Cod",
                                       New SqlParameter("@Cod", j.Cod_Empleado1))

        If userId Is Nothing OrElse IsDBNull(userId) Then
            Throw New InvalidOperationException("No se encontró el registro del empleado en el reloj de asistencia (Userinfo).")
        End If

        Dim id As Object = Escalar(
            "INSERT INTO UserLeave (Userid, BeginTime, EndTime, LeaveClassid, Whys, CodEmpleado) " &
            "OUTPUT INSERTED.Lsh VALUES (@U, @B, @E, @T, @W, @C)",
            New SqlParameter("@U", Convert.ToString(userId)),
            New SqlParameter("@B", SqlDbType.DateTime) With {.Value = j.Inicio},
            New SqlParameter("@E", SqlDbType.DateTime) With {.Value = j.Fin},
            New SqlParameter("@T", SqlDbType.Int) With {.Value = j.Cod_Tipo},
            New SqlParameter("@W", j.Motivo),
            New SqlParameter("@C", j.Cod_Empleado1))

        Return Convert.ToInt32(id)

    End Function

    Public Function Eliminar(id As Integer) As Boolean
        Return Ejecutar("DELETE FROM UserLeave WHERE Lsh = @Id",
                        New SqlParameter("@Id", SqlDbType.Int) With {.Value = id}) > 0
    End Function

End Class
