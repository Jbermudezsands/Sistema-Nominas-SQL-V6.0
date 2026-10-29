Imports System.Data
Imports System.Data.SqlClient

''' <summary>Horas extra pendientes de pago (tabla HorasExtras).</summary>
Public Class CsHorasExtraDAL
    Inherits CsDALBase

    ''' <summary>Horas extra con horas y sin pagar, por empleado y número de nómina.</summary>
    Public Function ListarPendientes() As DataTable
        Return ConsultarTabla(
            "SELECT h.Id, e.CodEmpleado1, " &
            "LTRIM(RTRIM(ISNULL(e.Nombre1,'') + ' ' + ISNULL(e.Nombre2,'') + ' ' + ISNULL(e.Apellido1,'') + ' ' + ISNULL(e.Apellido2,''))) AS Nombres, " &
            "h.CantHoras, h.NumNomina " &
            "FROM HorasExtras h INNER JOIN Empleado e ON e.CodEmpleado = h.CodEmpleado " &
            "WHERE ISNULL(h.CantHoras, 0) <> 0 AND ISNULL(h.Pagada, 0) = 0 " &
            "ORDER BY e.CodEmpleado1, h.NumNomina")
    End Function

    ''' <summary>Borra un registro de horas extra que aún no se ha pagado. Devuelve False si ya no existe o ya está pagado.</summary>
    Public Function EliminarPendiente(id As Integer) As Boolean
        Return Ejecutar("DELETE FROM HorasExtras WHERE Id = @Id AND ISNULL(Pagada, 0) = 0",
                        New SqlParameter("@Id", SqlDbType.Int) With {.Value = id}) > 0
    End Function

End Class
