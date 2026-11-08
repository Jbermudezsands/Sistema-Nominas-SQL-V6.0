Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Listado de bajas calculadas y aún sin procesar (V.5.0 FrmListadoBajas): consulta con filtro de fechas, procesar y eliminar.
''' El cálculo y el proceso individual de cada liquidación están en <see cref="CsBajasDAL"/>.
''' </summary>
Public Class CsListaBajasDAL
    Inherits CsDALBase

    ' Neto = ingresos - egresos de la liquidación (mismos conceptos que Liquidacion.Neto_Pagar).
    Private Const SqlNeto As String =
        "(ISNULL(B.MontoNomPropor, 0) + ISNULL(B.MontoVaca, 0) + ISNULL(B.Monto13Mes, 0) + ISNULL(B.MontoAntiguedad, 0) + ISNULL(B.MontoHorasExtra, 0) + ISNULL(B.MontoOtro, 0) - " &
        "ISNULL(B.MontoINSS, 0) - ISNULL(B.MontoIR, 0) - ISNULL(B.Prestamo, 0) - ISNULL(B.Deducciones, 0) - ISNULL(B.Monto_OtrasDeducciones, 0))"

    ''' <summary>
    ''' Bajas calculadas, no procesadas, de empleados activos, por nombre. Con fechas, solo las que caen entre <paramref name="desde"/>
    ''' y <paramref name="hasta"/> (inclusive).
    ''' </summary>
    Public Function Listar(Optional desde As Date? = Nothing, Optional hasta As Date? = Nothing) As DataTable

        Dim sql As String =
            "SELECT B.Id, E.CodEmpleado, E.CodEmpleado1, " & CsPlanillaNominaDAL.SqlNombres & " AS Nombres, ISNULL(Dp.Departamento, '') AS Departamento, " &
            "H.FechaContrato AS FechaIngreso, B.FechaBaja, ISNULL(B.TipoBaja, '') AS TipoBaja, ISNULL(B.AnnosTrabajados, 0) AS Annos, " &
            "ISNULL(B.MesesTrabajados, 0) AS Meses, ISNULL(B.DiasTrabajados, 0) AS Dias, ISNULL(B.SalarioMensual, 0) AS SalarioMensual, " &
            "ISNULL(B.MontoVaca, 0) AS MontoVacaciones, ISNULL(B.Monto13Mes, 0) AS MontoAguinaldo, ISNULL(B.MontoAntiguedad, 0) AS MontoIndemnizacion, " &
            "ISNULL(B.Prestamo, 0) AS Prestamo, ISNULL(B.Prestamo, 0) + ISNULL(B.Deducciones, 0) + ISNULL(B.Monto_OtrasDeducciones, 0) AS Deducciones, " &
            "ISNULL(B.MontoINSS, 0) AS MontoINSS, ISNULL(B.MontoIR, 0) AS MontoIR, " & SqlNeto & " AS NetoPagar, " &
            "ISNULL(E.NumeroInss, '') AS NumeroInss, ISNULL(H.CuentaBanco, '') AS CuentaBanco " &
            "FROM Bajas B INNER JOIN Empleado E ON E.CodEmpleado = B.CodEmpleado " &
            "LEFT JOIN Departamento Dp ON Dp.CodDepartamento = E.CodDepartamento " &
            "OUTER APPLY (SELECT TOP 1 FechaContrato, CuentaBanco FROM Historico WHERE CodEmpleado = E.CodEmpleado) H " &
            "WHERE ISNULL(B.Calculada, 0) = 1 AND ISNULL(B.Procesada, 0) = 0 AND E.Activo = 1 " &
            "AND (@Desde IS NULL OR B.FechaBaja >= @Desde) AND (@Hasta IS NULL OR B.FechaBaja < DATEADD(DAY, 1, @Hasta)) " &
            "ORDER BY Nombres"

        Return ConsultarTabla(sql,
            New SqlParameter("@Desde", SqlDbType.DateTime) With {.Value = If(desde.HasValue, CObj(desde.Value.Date), DBNull.Value)},
            New SqlParameter("@Hasta", SqlDbType.DateTime) With {.Value = If(hasta.HasValue, CObj(hasta.Value.Date), DBNull.Value)})

    End Function

    ''' <summary>Marca como procesadas las bajas indicadas (una transacción). Devuelve cuántas se marcaron.</summary>
    Public Function Procesar(ids As IEnumerable(Of Integer)) As Integer

        Dim total As Integer = 0

        EnTransaccion(
            Sub(cn, tr)
                For Each id As Integer In ids
                    total += EjecutarEn(cn, tr, "UPDATE Bajas SET Procesada = 1 WHERE Id = @Id AND ISNULL(Procesada, 0) = 0",
                                        New SqlParameter("@Id", SqlDbType.Int) With {.Value = id})
                Next
            End Sub)

        Return total

    End Function

    ''' <summary>Borra de verdad la baja calculada (no procesada) con ese Id.</summary>
    Public Function Eliminar(id As Integer) As Boolean
        Return Ejecutar("DELETE FROM Bajas WHERE Id = @Id AND ISNULL(Procesada, 0) = 0", New SqlParameter("@Id", SqlDbType.Int) With {.Value = id}) > 0
    End Function

End Class
