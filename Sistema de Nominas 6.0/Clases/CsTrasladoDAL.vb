Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Traslado de historial de pagos entre empleados (V.5.0 FrmTraslados/TrasladoHistoricos): reasigna a otro empleado
''' las filas de DetalleNomina de las nóminas cuyo inicio cae en un rango de fechas.
''' Solo se trasladan nóminas cerradas (una nómina abierta se recalcula) y no se permite si el destino ya
''' tiene pago en alguna de esas nóminas.
''' </summary>
Public Class CsTrasladoDAL
    Inherits CsDALBase

    Public Function ListarEmpleados() As DataTable
        Return New CsHistorialEmpleadoDAL().ListarEmpleados()
    End Function

    ''' <summary>Pagos del empleado (una fila por nómina) cuyo inicio de nómina cae en el rango.</summary>
    Public Function Historial(codEmpleado As Decimal, desde As Date, hasta As Date) As DataTable
        Return ConsultarTabla(
            "SELECT n.NumNomina, n.FechaNominaINI AS Inicio, n.FechaNomina AS Fin, " &
            "CASE WHEN n.Anulada = 1 THEN 'Anulada' WHEN n.Cerrada = 1 THEN 'Cerrada' ELSE 'Abierta' END AS Estado, " &
            "ISNULL(d.SalarioBasico,0) AS SalarioBasico, ISNULL(d.Destajo,0) AS Destajo, ISNULL(d.Incentivos,0) AS Incentivos, " &
            "ISNULL(d.HorasExtras,0) AS HorasExtras, ISNULL(d.Comisiones,0) AS Comisiones, ISNULL(d.OtrosIngresos,0) AS OtrosIngresos, " &
            "ISNULL(d.Deducciones,0) AS Deducciones, ISNULL(d.Prestamo,0) AS Prestamo, ISNULL(d.MontoINSS,0) AS MontoINSS, ISNULL(d.MontoIR,0) AS MontoIR " &
            "FROM DetalleNomina d INNER JOIN Nomina n ON n.NumNomina = d.NumNomina " &
            "WHERE d.CodEmpleado = @Emp AND n.FechaNominaINI >= @Desde AND n.FechaNominaINI < DATEADD(day, 1, @Hasta) " &
            "ORDER BY n.FechaNominaINI, n.NumNomina",
            New SqlParameter("@Emp", SqlDbType.Decimal) With {.Value = codEmpleado},
            New SqlParameter("@Desde", SqlDbType.DateTime) With {.Value = desde.Date},
            New SqlParameter("@Hasta", SqlDbType.DateTime) With {.Value = hasta.Date})
    End Function

    ''' <summary>Traslada el historial y devuelve cuántas filas se movieron. Una sola transacción, con rastro en la Bitacora.</summary>
    Public Function Trasladar(origen As Decimal, destino As Decimal, desde As Date, hasta As Date, usuario As String) As Integer

        If origen = destino Then Throw New InvalidOperationException("El empleado de origen y el de destino deben ser distintos.")
        If hasta.Date < desde.Date Then Throw New InvalidOperationException("La fecha final no puede ser menor que la inicial.")

        Dim movidas As Integer = 0

        EnTransaccion(
            Sub(cn, tr)

                Dim pO As New SqlParameter("@Orig", SqlDbType.Decimal) With {.Value = origen}
                Dim pD As New SqlParameter("@Dest", SqlDbType.Decimal) With {.Value = destino}
                Dim pDesde As New SqlParameter("@Desde", SqlDbType.DateTime) With {.Value = desde.Date}
                Dim pHasta As New SqlParameter("@Hasta", SqlDbType.DateTime) With {.Value = hasta.Date}

                Const rango As String =
                    "FROM DetalleNomina d INNER JOIN Nomina n ON n.NumNomina = d.NumNomina " &
                    "WHERE n.Cerrada = 1 AND ISNULL(n.Anulada,0) = 0 AND n.FechaNominaINI >= @Desde AND n.FechaNominaINI < DATEADD(day, 1, @Hasta) "

                Dim abiertas As Integer = Convert.ToInt32(EscalarEn(cn, tr,
                    "SELECT COUNT(*) FROM DetalleNomina d INNER JOIN Nomina n ON n.NumNomina = d.NumNomina " &
                    "WHERE n.Cerrada = 0 AND d.CodEmpleado = @Orig AND n.FechaNominaINI >= @Desde AND n.FechaNominaINI < DATEADD(day, 1, @Hasta)",
                    Clonar(pO), Clonar(pDesde), Clonar(pHasta)))

                Dim choques As Integer = Convert.ToInt32(EscalarEn(cn, tr,
                    "SELECT COUNT(*) " & rango & "AND d.CodEmpleado = @Dest AND d.NumNomina IN " &
                    "(SELECT d2.NumNomina FROM DetalleNomina d2 WHERE d2.CodEmpleado = @Orig)",
                    Clonar(pO), Clonar(pD), Clonar(pDesde), Clonar(pHasta)))

                If choques > 0 Then
                    Throw New InvalidOperationException("El empleado destino ya tiene pagos en " & choques &
                                                        " de las nóminas del rango. Quite esos pagos antes de trasladar.")
                End If

                movidas = EjecutarEn(cn, tr,
                    "UPDATE d SET d.CodEmpleado = @Dest " & rango & "AND d.CodEmpleado = @Orig",
                    Clonar(pD), Clonar(pO), Clonar(pDesde), Clonar(pHasta))

                If movidas = 0 Then
                    Throw New InvalidOperationException(If(abiertas > 0,
                        "Las nóminas del rango aún están abiertas: ciérrelas antes de trasladar.",
                        "El empleado de origen no tiene pagos en nóminas cerradas dentro del rango."))
                End If

                EjecutarEn(cn, tr, "INSERT INTO Bitacora (FechaHora, Usuario, Modulo, Accion) VALUES (@F, @U, 'Traslados', @A)",
                           New SqlParameter("@F", SqlDbType.DateTime) With {.Value = DateTime.Now},
                           New SqlParameter("@U", If(usuario, "")),
                           New SqlParameter("@A", $"Traslado de historial del empleado {origen} al {destino} ({desde:dd/MM/yyyy} a {hasta:dd/MM/yyyy}): {movidas} pagos"))

            End Sub)

        Return movidas

    End Function

    ''' <summary>Un SqlParameter no puede pertenecer a dos comandos: se copia cada vez.</summary>
    Private Shared Function Clonar(p As SqlParameter) As SqlParameter
        Return New SqlParameter(p.ParameterName, p.SqlDbType) With {.Value = p.Value}
    End Function

End Class
