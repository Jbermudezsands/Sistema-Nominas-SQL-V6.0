Imports System.Data
Imports System.Data.SqlClient

''' <summary>Consulta de nóminas, su detalle por empleado y la anulación de una nómina cerrada.</summary>
Public Class CsNominaDAL
    Inherits CsDALBase

#Region "CONSULTAS"

    ''' <summary>Todas las nóminas, la más reciente primero. <c>Estado</c> = Anulada / Cerrada / Activa / Procesada.</summary>
    Public Function Listar() As DataTable
        Return ConsultarTabla(
            "SELECT NumNomina, CodTipoNomina, FechaNominaINI, FechaNomina, " &
            "CASE WHEN Anulada = 1 THEN 'Anulada' WHEN Cerrada = 1 THEN 'Cerrada' WHEN Activa = 1 THEN 'Activa' ELSE 'Procesada' END AS Estado, " &
            "TotalSalarioBasico, TotalDestajo, TotalHorasExtras, TotalComisiones, TotalOtrosIngresos, TotalIncentivos, " &
            "TotalDeducciones, TotalPrestamo, TotalMontoINSS, TotalMontoIR " &
            "FROM Nomina ORDER BY NumNomina DESC")
    End Function

    ''' <summary>Detalle de una nómina: una fila por empleado.</summary>
    Public Function ListarDetalle(numNomina As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT e.CodEmpleado1, " &
            "LTRIM(RTRIM(ISNULL(e.Nombre1,'') + ' ' + ISNULL(e.Nombre2,'') + ' ' + ISNULL(e.Apellido1,'') + ' ' + ISNULL(e.Apellido2,''))) AS Nombres, " &
            "d.SalarioBasico, d.Destajo, d.HorasExtras, d.Comisiones, d.OtrosIngresos, d.Incentivos, d.Deducciones, d.Prestamo, " &
            "d.MontoINSS, d.MontoIR, d.Vacaciones, d.INSSPatronal, d.Mes13 " &
            "FROM DetalleNomina d INNER JOIN Empleado e ON e.CodEmpleado = d.CodEmpleado " &
            "WHERE d.NumNomina = @Num ORDER BY e.CodEmpleado1",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina})
    End Function

#End Region

#Region "ANULAR"

    ''' <summary>
    ''' Anula una nómina cerrada: borra su detalle, deja los totales en 0 y revierte los movimientos que pagó
    ''' (deducciones, incentivos, comisiones, destajos, horas extra, cuotas de préstamo y saldo del préstamo).
    ''' Todo en una sola transacción. Lanza una excepción con el motivo si no se puede anular.
    ''' </summary>
    Public Sub Anular(numNomina As Integer)

        EnTransaccion(
            Sub(cn, tr)

                Dim pNum As New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina}

                Dim estado As DataTable = New DataTable
                Using cmd As New SqlCommand("SELECT Activa, Cerrada, Anulada, ISNULL(Contabilizado, 0) AS Contabilizado " &
                                            "FROM Nomina WITH (UPDLOCK, HOLDLOCK) WHERE NumNomina = @Num", cn, tr)
                    cmd.Parameters.Add(New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina})
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(estado)
                    End Using
                End Using

                If estado.Rows.Count = 0 Then
                    Throw New Exception("La nómina " & numNomina & " no existe.")
                End If

                Dim r As DataRow = estado.Rows(0)

                If Convert.ToBoolean(r("Anulada")) Then
                    Throw New Exception("La nómina " & numNomina & " ya está anulada.")
                End If

                If Convert.ToBoolean(r("Contabilizado")) Then
                    Throw New Exception("La nómina " & numNomina & " ya fue contabilizada y no se puede anular.")
                End If

                If Convert.ToBoolean(r("Activa")) AndAlso Not Convert.ToBoolean(r("Cerrada")) Then
                    Throw New Exception("La nómina " & numNomina & " está activa (no cerrada); solo se anulan nóminas cerradas.")
                End If

                ' Devuelve a cada préstamo lo que se le descontó en esta nómina.
                EjecutarEn(cn, tr,
                    "UPDATE p SET p.Saldo = p.Saldo + m.Cuotas " &
                    "FROM Prestamo p INNER JOIN " &
                    "(SELECT NumPrestamo, SUM(ISNULL(CuotaIgual, 0)) AS Cuotas FROM MovPrestamo WHERE NumNomina = @Num GROUP BY NumPrestamo) m " &
                    "ON m.NumPrestamo = p.NumPrestamo",
                    New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina})

                EjecutarEn(cn, tr, "UPDATE MovPrestamo SET Cancelado = 0 WHERE NumNomina = @Num", CopiaParametro(pNum))
                EjecutarEn(cn, tr, "UPDATE DetalleDeduccion SET Pagado = 0 WHERE NumNomina = @Num", CopiaParametro(pNum))
                EjecutarEn(cn, tr, "UPDATE DetalleIncentivo SET Pagado = 0 WHERE NumNomina = @Num", CopiaParametro(pNum))
                EjecutarEn(cn, tr, "UPDATE Comisiones SET Pagado = 0 WHERE NumNomina = @Num", CopiaParametro(pNum))
                EjecutarEn(cn, tr, "UPDATE Destajo SET Pagado = 0 WHERE NumNomina = @Num", CopiaParametro(pNum))
                EjecutarEn(cn, tr, "UPDATE HorasExtras SET Pagada = 0 WHERE NumNomina = @Num", CopiaParametro(pNum))

                EjecutarEn(cn, tr, "DELETE FROM DetalleNomina WHERE NumNomina = @Num", CopiaParametro(pNum))

                EjecutarEn(cn, tr,
                    "UPDATE Nomina SET TotalSalarioBasico = 0, TotalDestajo = 0, TotalHorasExtras = 0, TotalComisiones = 0, " &
                    "TotalIncentivos = 0, TotalDeducciones = 0, TotalOtrosIngresos = 0, TotalPrestamo = 0, TotalMontoINSS = 0, " &
                    "TotalMontoIR = 0, TotalVacaciones = 0, TotalINSSPatronal = 0, TotalIRPatronal = 0, TotalINATEC = 0, " &
                    "Totalmes13 = 0, Procesada = 1, Anulada = 1 WHERE NumNomina = @Num",
                    CopiaParametro(pNum))

            End Sub)

    End Sub

    ''' <summary>Un SqlParameter no se puede asignar a dos comandos: cada uno recibe su copia.</summary>
    Private Shared Function CopiaParametro(p As SqlParameter) As SqlParameter
        Return New SqlParameter(p.ParameterName, p.SqlDbType) With {.Value = p.Value}
    End Function

#End Region

End Class
