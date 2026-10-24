Imports System.Data
Imports System.Data.SqlClient

Public Class CsCargoDAL
    Inherits CsDALBase

    Public Function Listar() As DataTable
        Return ConsultarTabla("SELECT CodCargo, Cargo, Monto FROM Cargo ORDER BY CodCargo")
    End Function

    Public Function ProximoCodigo() As String
        Return SiguienteCodigo("Cargo", "CodCargo", 3)
    End Function

    Public Function Existe(codigo As String) As Boolean
        Return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Cargo WHERE CodCargo = @Cod",
                                       New SqlParameter("@Cod", codigo))) > 0
    End Function

    Public Function Buscar(codigo As String) As Cargos

        Dim dt As DataTable = ConsultarTabla(
            "SELECT CodCargo, Cargo, Monto FROM Cargo WHERE CodCargo = @Cod",
            New SqlParameter("@Cod", codigo))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New Cargos With {
            .Cod_Cargo = Convert.ToString(r("CodCargo")),
            .Descripcion_Cargo = If(IsDBNull(r("Cargo")), "", Convert.ToString(r("Cargo"))),
            .Monto_Cargo = If(IsDBNull(r("Monto")), "0", Convert.ToDecimal(r("Monto")).ToString("0.00"))
        }

    End Function

    Public Sub Guardar(c As Cargos, esNuevo As Boolean)

        Dim monto As Decimal = 0
        Decimal.TryParse(c.Monto_Cargo, monto)

        Dim pCod As New SqlParameter("@Cod", c.Cod_Cargo.Trim)
        Dim pCargo As New SqlParameter("@Cargo", c.Descripcion_Cargo.Trim)
        Dim pMonto As New SqlParameter("@Monto", SqlDbType.Money) With {.Value = monto}

        If esNuevo Then

            If Existe(c.Cod_Cargo.Trim) Then
                Throw New Exception("Ya existe un cargo con el código " & c.Cod_Cargo.Trim & ".")
            End If

            Ejecutar("INSERT INTO Cargo (CodCargo, Cargo, Monto) VALUES (@Cod, @Cargo, @Monto)",
                     pCod, pCargo, pMonto)
        Else
            Ejecutar("UPDATE Cargo SET Cargo = @Cargo, Monto = @Monto WHERE CodCargo = @Cod",
                     pCod, pCargo, pMonto)
        End If

    End Sub

    Public Sub Eliminar(codigo As String)

        Try
            Ejecutar("DELETE FROM Cargo WHERE CodCargo = @Cod", New SqlParameter("@Cod", codigo))
        Catch ex As SqlException When EsEnUso(ex)
            Throw New Exception("No se puede eliminar el cargo porque está asignado a empleados.")
        End Try

    End Sub

End Class
