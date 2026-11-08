Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Adelantos de 13vo mes y vacaciones (tabla Adelanto13vo, V.5.0 FrmAdelantos13vo). Se descuentan en
''' <see cref="CsAguinaldoDAL"/> (TipoAdelanto = '13vo Mes') y <see cref="CsNominaVacacionesDAL"/> ('Vacaciones').
''' La tabla no tiene llave propia: Guardar reemplaza todos los adelantos del empleado en una transacción.
''' </summary>
Public Class CsAdelantosDAL
    Inherits CsDALBase

    Public Const TipoVacaciones As String = "Vacaciones"
    Public Const Tipo13vo As String = "13vo Mes"

    ''' <summary>Datos del empleado que se muestra en la cabecera (la foto viaja como texto "b1,b2,..." igual que en FrmEmpleados).</summary>
    Public Class FichaAdelanto
        Public Property Cod_Empleado As Decimal
        Public Property Codigo As String = ""
        Public Property Nombres As String = ""
        Public Property Cargo As String = ""
        Public Property Foto As String = ""
    End Class

    Public Function ListarEmpleados() As DataTable
        Return ConsultarTabla(
            "SELECT CodEmpleado1, " &
            "LTRIM(RTRIM(ISNULL(Nombre1,'') + ' ' + ISNULL(Nombre2,'') + ' ' + ISNULL(Apellido1,'') + ' ' + ISNULL(Apellido2,''))) AS Nombres " &
            "FROM Empleado WHERE Activo = 1 ORDER BY CodEmpleado1")
    End Function

    ''' <summary>Empleado activo con ese código visible; Nothing si no existe.</summary>
    Public Function BuscarEmpleado(codigo1 As String) As FichaAdelanto

        Dim dt As DataTable = ConsultarTabla(
            "SELECT TOP 1 e.CodEmpleado, e.CodEmpleado1, e.Foto, c.Cargo, " &
            "LTRIM(RTRIM(ISNULL(e.Nombre1,'') + ' ' + ISNULL(e.Nombre2,'') + ' ' + ISNULL(e.Apellido1,'') + ' ' + ISNULL(e.Apellido2,''))) AS Nombres " &
            "FROM Empleado e LEFT JOIN Cargo c ON e.CodCargo = c.CodCargo WHERE e.CodEmpleado1 = @Cod AND e.Activo = 1",
            New SqlParameter("@Cod", codigo1))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim f As DataRow = dt.Rows(0)

        Return New FichaAdelanto With {
            .Cod_Empleado = Convert.ToDecimal(f("CodEmpleado")),
            .Codigo = Convert.ToString(f("CodEmpleado1")),
            .Nombres = Convert.ToString(f("Nombres")),
            .Cargo = If(IsDBNull(f("Cargo")), "", Convert.ToString(f("Cargo"))),
            .Foto = If(IsDBNull(f("Foto")), "", Convert.ToString(f("Foto")))}

    End Function

    Public Function Listar(codEmpleado As Decimal) As DataTable
        Return ConsultarTabla(
            "SELECT FechaAdelanto AS Fecha, CAST(MontoAdelanto AS float) AS Monto, [Ref/Cheque] AS Referencia, TipoAdelanto AS Tipo " &
            "FROM Adelanto13vo WHERE CodEmpleado = @Cod ORDER BY FechaAdelanto",
            New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = codEmpleado, .Precision = 18, .Scale = 0})
    End Function

    ''' <summary>Reemplaza los adelantos del empleado por las filas de <paramref name="filas"/> (columnas Fecha, Monto, Referencia, Tipo).</summary>
    Public Sub Guardar(codEmpleado As Decimal, filas As DataTable)

        For Each f As DataRow In filas.Rows
            If f.RowState = DataRowState.Deleted Then Continue For
            If IsDBNull(f("Fecha")) Then Throw New InvalidOperationException("Hay un adelanto sin fecha.")
            If IsDBNull(f("Monto")) OrElse Convert.ToDouble(f("Monto")) <= 0 Then Throw New InvalidOperationException("Todos los adelantos deben tener un monto mayor que cero.")
            If Convert.ToString(f("Tipo")) <> TipoVacaciones AndAlso Convert.ToString(f("Tipo")) <> Tipo13vo Then
                Throw New InvalidOperationException("El tipo de adelanto debe ser '" & TipoVacaciones & "' o '" & Tipo13vo & "'.")
            End If
        Next

        ' La llave de la tabla es (CodEmpleado, FechaAdelanto): un solo adelanto por empleado y por día.
        Dim repetida = filas.Rows.Cast(Of DataRow)().Where(Function(f) f.RowState <> DataRowState.Deleted).
            GroupBy(Function(f) Convert.ToDateTime(f("Fecha")).Date).FirstOrDefault(Function(g) g.Count() > 1)

        If repetida IsNot Nothing Then
            Throw New InvalidOperationException("Solo se permite un adelanto por empleado por fecha. Hay más de uno el " & repetida.Key.ToString("dd/MM/yyyy") &
                                                "; sume los montos o use otra fecha.")
        End If

        EnTransaccion(
            Sub(cn, tr)
                EjecutarEn(cn, tr, "DELETE FROM Adelanto13vo WHERE CodEmpleado = @Cod", Codigo(codEmpleado))

                For Each f As DataRow In filas.Rows
                    If f.RowState = DataRowState.Deleted Then Continue For
                    EjecutarEn(cn, tr,
                        "INSERT INTO Adelanto13vo (CodEmpleado, FechaAdelanto, MontoAdelanto, [Ref/Cheque], TipoAdelanto) " &
                        "VALUES (@Cod, @Fecha, @Monto, @Ref, @Tipo)",
                        Codigo(codEmpleado),
                        New SqlParameter("@Fecha", SqlDbType.DateTime) With {.Value = Convert.ToDateTime(f("Fecha")).Date},
                        New SqlParameter("@Monto", SqlDbType.Money) With {.Value = Convert.ToDecimal(f("Monto"))},
                        New SqlParameter("@Ref", If(IsDBNull(f("Referencia")) OrElse Convert.ToString(f("Referencia")).Trim = "", DBNull.Value, CObj(Convert.ToString(f("Referencia")).Trim))),
                        New SqlParameter("@Tipo", Convert.ToString(f("Tipo"))))
                Next
            End Sub)

    End Sub

    Private Shared Function Codigo(cod As Decimal) As SqlParameter
        Return New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = cod, .Precision = 18, .Scale = 0}
    End Function

End Class
