Imports System.Data
Imports System.Data.SqlClient

''' <summary>Incapacidades de los empleados (tabla Incapacidad, sin llave propia: se identifican por empleado + tipo + fecha).</summary>
Public Class CsIncapacidadDAL
    Inherits CsDALBase

    Public Function ListarEmpleados() As DataTable
        Return ConsultarTabla(
            "SELECT CodEmpleado, CodEmpleado1, " &
            "LTRIM(RTRIM(ISNULL(Nombre1,'') + ' ' + ISNULL(Nombre2,'') + ' ' + ISNULL(Apellido1,'') + ' ' + ISNULL(Apellido2,''))) AS Nombres " &
            "FROM Empleado WHERE Activo = 1 ORDER BY CodEmpleado1")
    End Function

    Public Function ListarPorEmpleado(codEmpleado As Decimal) As DataTable
        Return ConsultarTabla(
            "SELECT i.CodIncapacidad, t.Incapacidad, i.FechaIncapacidad " &
            "FROM Incapacidad i INNER JOIN TipoIncapacidad t ON t.CodIncapacidad = i.CodIncapacidad " &
            "WHERE i.CodEmpleado = @Emp ORDER BY i.FechaIncapacidad DESC, i.CodIncapacidad",
            New SqlParameter("@Emp", SqlDbType.Decimal) With {.Value = codEmpleado})
    End Function

    Public Function Existe(i As IncapacidadEmpleado) As Boolean
        Return Convert.ToInt32(Escalar(
            "SELECT COUNT(*) FROM Incapacidad WHERE CodEmpleado = @Emp AND CodIncapacidad = @Cod AND FechaIncapacidad = @Fecha",
            Parametros(i))) > 0
    End Function

    Public Sub Agregar(i As IncapacidadEmpleado)

        If Existe(i) Then
            Throw New Exception("El empleado ya tiene esa incapacidad registrada en esa fecha.")
        End If

        Ejecutar("INSERT INTO Incapacidad (CodEmpleado, CodIncapacidad, FechaIncapacidad) VALUES (@Emp, @Cod, @Fecha)",
                 Parametros(i))

    End Sub

    Public Sub Eliminar(i As IncapacidadEmpleado)
        Ejecutar("DELETE FROM Incapacidad WHERE CodEmpleado = @Emp AND CodIncapacidad = @Cod AND FechaIncapacidad = @Fecha",
                 Parametros(i))
    End Sub

    Private Shared Function Parametros(i As IncapacidadEmpleado) As SqlParameter()
        Return {
            New SqlParameter("@Emp", SqlDbType.Decimal) With {.Value = i.Cod_Empleado},
            New SqlParameter("@Cod", i.Cod_Incapacidad.Trim),
            New SqlParameter("@Fecha", SqlDbType.SmallDateTime) With {.Value = i.Fecha.Date}
        }
    End Function

End Class
