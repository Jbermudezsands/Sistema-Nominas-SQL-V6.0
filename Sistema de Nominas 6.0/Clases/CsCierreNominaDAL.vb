Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Cierre de la nómina (CmdCerrarNomina_Click, V.5.0): paga los movimientos, descuenta los préstamos, prepara las cuotas siguientes,
''' limpia los conceptos variables del empleado y cierra la nómina. Todo en una transacción, con rastro en la Bitacora.
''' </summary>
Public Class CsCierreNominaDAL
    Inherits CsDALBase

    Public Class ResultadoCierre
        Public Property NumNomina As Integer
        Public Property Empleados As Integer
        Public Property DeduccionesPagadas As Integer
        Public Property IncentivosPagados As Integer
        Public Property CuotasPrestamo As Integer
        Public Property SolicitudesPagadas As Integer
        Public Property Liquidados As Integer
    End Class

    Public Function Cerrar(codTipoNomina As String, usuario As String) As ResultadoCierre

        Dim res As New ResultadoCierre()

        EnTransaccion(
            Sub(cn, tr)

                Dim pTipo As Func(Of SqlParameter) = Function() New SqlParameter("@Tipo", codTipoNomina)

                Dim nom As DataTable = Leer(cn, tr, "SELECT NumNomina, FechaNominaINI, FechaNomina, Procesada, Cerrada FROM Nomina WITH (UPDLOCK, HOLDLOCK) " &
                                                    "WHERE CodTipoNomina = @Tipo AND Activa = 1 AND ISNULL(Anulada, 0) = 0", pTipo())
                If nom.Rows.Count = 0 Then Throw New InvalidOperationException("El tipo de nómina no tiene una nómina activa.")

                Dim n As DataRow = nom.Rows(0)
                If Not Convert.ToBoolean(n("Procesada")) Then Throw New InvalidOperationException("Esta nómina no ha sido procesada, no puede ser cerrada.")
                If Convert.ToBoolean(n("Cerrada")) Then Throw New InvalidOperationException("La nómina ya está cerrada.")

                Dim num As Integer = Convert.ToInt32(n("NumNomina"))
                Dim fin As Date = Convert.ToDateTime(n("FechaNomina"))
                Dim ini As Date = Convert.ToDateTime(n("FechaNominaINI"))
                res.NumNomina = num

                Dim pNum As Func(Of SqlParameter) = Function() New SqlParameter("@Num", SqlDbType.Int) With {.Value = num}

                ' La nómina de subsidio ligada tiene que cerrarse antes (si existe la tabla en la base).
                If ExisteTabla(cn, tr, "NomSubsidio") Then
                    Dim abiertas As Object = EscalarEn(cn, tr, "SELECT COUNT(*) FROM NomSubsidio WHERE NumNomina = @Num AND Activa = 1", pNum())
                    If Convert.ToInt32(abiertas) > 0 Then
                        Throw New InvalidOperationException("La nómina de subsidio correspondiente está activa; hay que cerrarla para poder cerrar esta nómina.")
                    End If
                End If

                res.Empleados = Convert.ToInt32(EscalarEn(cn, tr, "SELECT COUNT(*) FROM DetalleNomina WHERE NumNomina = @Num", pNum()))
                If res.Empleados = 0 Then Throw New InvalidOperationException("La nómina no tiene empleados calculados.")

                ' Conceptos variables del empleado: se vuelven a cero para la siguiente nómina.
                EjecutarEn(cn, tr,
                    "UPDATE Empleado SET DiasDescuento = 0, PorcentajeComision = 0, OtrosIngresos = 0, DescripOtrIngre = 'Sin Descrip', AumentoBasico = 0 " &
                    "WHERE CodTipoNomina = @Tipo AND Activo = 1", pTipo())

                ' Producción pagada en esta nómina.
                EjecutarEn(cn, tr, "UPDATE DetalleHorasProduccion SET Pagado = 1 WHERE NumNomina = @Num", pNum())
                EjecutarEn(cn, tr, "UPDATE DetalleProduccion SET Pagado = 1 WHERE NumNomina = @Num", pNum())

                ' Pagar movimientos de la nómina (las deducciones e incentivos de "n veces" siguen vigentes).
                res.DeduccionesPagadas = EjecutarEn(cn, tr, "UPDATE DetalleDeduccion SET Pagado = 1 WHERE NumNomina = @Num AND NumVez <> 'n'", pNum())
                res.IncentivosPagados = EjecutarEn(cn, tr, "UPDATE DetalleIncentivo SET Pagado = 1 WHERE NumNomina = @Num AND NumVez <> 'n'", pNum())

                ' Préstamos: saldo menos la cuota y cuota cancelada.
                EjecutarEn(cn, tr,
                    "UPDATE p SET p.Saldo = p.Saldo - m.Cuotas FROM Prestamo p INNER JOIN " &
                    "(SELECT NumPrestamo, SUM(ISNULL(CuotaIgual, 0)) AS Cuotas FROM MovPrestamo WHERE NumNomina = @Num AND Cancelado = 0 GROUP BY NumPrestamo) m " &
                    "ON m.NumPrestamo = p.NumPrestamo", pNum())
                res.CuotasPrestamo = EjecutarEn(cn, tr, "UPDATE MovPrestamo SET Cancelado = 1 WHERE NumNomina = @Num", pNum())

                EjecutarEn(cn, tr, "UPDATE Comisiones SET Pagado = 1 WHERE NumNomina = @Num", pNum())
                EjecutarEn(cn, tr, "UPDATE Destajo SET Pagado = 1 WHERE NumNomina = @Num", pNum())
                EjecutarEn(cn, tr, "UPDATE HorasExtras SET Pagada = 1 WHERE NumNomina = @Num", pNum())

                ' Solicitudes (ausencias, subsidios, vacaciones pagadas) que ya caen en la nómina. La V.5.0 marcaba todas las pendientes, incluso las futuras.
                res.SolicitudesPagadas = EjecutarEn(cn, tr,
                    "UPDATE SolicitudVacaciones SET Pagado = 1, NumNomina = @Num WHERE ISNULL(Pagado, 0) = 0 AND FechaInicio <= @Fin",
                    pNum(), New SqlParameter("@Fin", SqlDbType.DateTime) With {.Value = fin.Date.AddHours(23).AddMinutes(59).AddSeconds(59)})

                ' Siguiente cuota de cada deducción / incentivo de varias veces: queda en espera (-1) para la próxima nómina del tipo.
                EjecutarEn(cn, tr,
                    "UPDATE dd SET dd.NumNomina = -1 FROM DetalleDeduccion dd INNER JOIN " &
                    "(SELECT x.Id FROM (SELECT d2.Id, ROW_NUMBER() OVER (PARTITION BY d2.NumDeduccion ORDER BY TRY_CAST(d2.NumVez AS int), d2.Id) AS Orden " &
                    "FROM DetalleDeduccion d2 INNER JOIN Deduccion d ON d.NumDeduccion = d2.NumDeduccion INNER JOIN Empleado e ON e.CodEmpleado = d.CodEmpleado " &
                    "WHERE d2.Pagado = 0 AND d2.NumVez <> 'n' AND e.CodTipoNomina = @Tipo) x WHERE x.Orden = 1) s ON s.Id = dd.Id", pTipo())

                EjecutarEn(cn, tr,
                    "UPDATE di SET di.NumNomina = -1 FROM DetalleIncentivo di INNER JOIN " &
                    "(SELECT x.Id, x.NumIncentivo FROM (SELECT d2.Id, d2.NumIncentivo, ROW_NUMBER() OVER (PARTITION BY d2.NumIncentivo ORDER BY TRY_CAST(d2.NumVez AS int), d2.Id) AS Orden " &
                    "FROM DetalleIncentivo d2 INNER JOIN Incentivo i ON i.NumIncentivo = d2.NumIncentivo INNER JOIN Empleado e ON e.CodEmpleado = i.CodEmpleado " &
                    "WHERE d2.Pagado = 0 AND d2.NumNomina = 0 AND d2.NumVez <> 'n' AND e.CodTipoNomina = @Tipo) x WHERE x.Orden = 1) s " &
                    "ON s.Id = di.Id AND s.NumIncentivo = di.NumIncentivo", pTipo())

                ' Período fiscal y nómina.
                EjecutarEn(cn, tr, "UPDATE PeriodoFiscal SET Actual = 0 WHERE CodTipoNomina = @Tipo AND Inicio = @Ini",
                           pTipo(), New SqlParameter("@Ini", SqlDbType.SmallDateTime) With {.Value = ini.Date})
                EjecutarEn(cn, tr, "UPDATE Nomina SET Cerrada = 1, Activa = 0 WHERE NumNomina = @Num", pNum())
                EjecutarEn(cn, tr, "UPDATE TipoNomina SET Activa = 0 WHERE CodTipoNomina = @Tipo", pTipo())

                If ExisteTabla(cn, tr, "NomSubsidio") Then
                    EjecutarEn(cn, tr, "UPDATE NomSubsidio SET Activa = 0, Procesada = 1, Cerrada = 1 WHERE NumNomina = @Num", pNum())
                End If

                ' Los liquidados dejan de estar activos.
                res.Liquidados = EjecutarEn(cn, tr, "UPDATE Empleado SET Activo = 0 WHERE Liquidado = 1 AND Activo = 1 AND CodTipoNomina = @Tipo", pTipo())

                EjecutarEn(cn, tr, "INSERT INTO Bitacora (FechaHora, Usuario, Modulo, Accion) VALUES (@F, @U, 'Calcular Nomina', @A)",
                           New SqlParameter("@F", SqlDbType.DateTime) With {.Value = DateTime.Now},
                           New SqlParameter("@U", If(usuario, "")),
                           New SqlParameter("@A", $"Se Cerro la Nomina: {num} ({res.Empleados} empleados, {res.DeduccionesPagadas} deducciones, {res.IncentivosPagados} incentivos, {res.CuotasPrestamo} cuotas de préstamo)"))

            End Sub)

        Return res

    End Function

    ''' <summary>Rastro de una acción de la pantalla (cálculo, exportaciones) en la Bitacora.</summary>
    Public Sub Registrar(usuario As String, accion As String)
        Ejecutar("INSERT INTO Bitacora (FechaHora, Usuario, Modulo, Accion) VALUES (@F, @U, 'Calcular Nomina', @A)",
                 New SqlParameter("@F", SqlDbType.DateTime) With {.Value = DateTime.Now},
                 New SqlParameter("@U", If(usuario, "")),
                 New SqlParameter("@A", accion))
    End Sub

    Private Shared Function Leer(cn As SqlConnection, tr As SqlTransaction, sql As String, ParamArray parametros() As SqlParameter) As DataTable
        Dim dt As New DataTable
        Using cmd As New SqlCommand(sql, cn, tr)
            cmd.Parameters.AddRange(parametros)
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using
        Return dt
    End Function

    Private Shared Function ExisteTabla(cn As SqlConnection, tr As SqlTransaction, nombre As String) As Boolean
        Return EscalarEn(cn, tr, "SELECT COUNT(*) FROM sys.tables WHERE name = @N", New SqlParameter("@N", nombre)) IsNot Nothing AndAlso
               Convert.ToInt32(EscalarEn(cn, tr, "SELECT COUNT(*) FROM sys.tables WHERE name = @N", New SqlParameter("@N", nombre))) > 0
    End Function

End Class
