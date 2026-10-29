Imports System.Data
Imports System.Data.SqlClient

''' <summary>Datos del carnet de un empleado (empleado + cargo).</summary>
Public Class CsCarnetDAL
    Inherits CsDALBase

    ''' <summary>Nothing si el empleado no existe.</summary>
    Public Function Buscar(codEmpleado As Decimal) As DatosCarnet

        Dim dt As DataTable = ConsultarTabla(
            "SELECT e.CodEmpleado1, e.Nombre1, e.Nombre2, e.Apellido1, e.Apellido2, e.Foto, c.Cargo " &
            "FROM Empleado e LEFT JOIN Cargo c ON c.CodCargo = e.CodCargo WHERE e.CodEmpleado = @Cod",
            New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = codEmpleado})

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New DatosCarnet With {
            .Codigo = Texto(r("CodEmpleado1")),
            .Nombres = (Texto(r("Nombre1")) & " " & Texto(r("Nombre2"))).Trim(),
            .Apellidos = (Texto(r("Apellido1")) & " " & Texto(r("Apellido2"))).Trim(),
            .Cargo = Texto(r("Cargo")),
            .Foto = Texto(r("Foto"))
        }

    End Function

    Private Shared Function Texto(v As Object) As String
        Return If(IsDBNull(v), "", Convert.ToString(v))
    End Function

End Class
