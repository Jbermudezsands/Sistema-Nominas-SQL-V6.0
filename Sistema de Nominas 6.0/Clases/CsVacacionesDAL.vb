Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Saldo de vacaciones: cálculo contra SolicitudVacaciones y ajuste automático desde un saldo de Excel
''' (V.5.0 FrmImportarEmpleados, pestaña "Saldo Vacaciones"). Las filas con problema no detienen la importación:
''' se devuelven en el resultado; un error inesperado de base de datos deshace todo.
''' </summary>
Public Class CsVacacionesDAL
    Inherits CsDALBase

    ''' <summary>Tipos de solicitud que no descuentan vacaciones.</summary>
    Private Const TiposExcluidos As String = "('Ausente', 'Subsidio', 'Suspension', 'Feriado')"

    ''' <summary>Saldo de días al <paramref name="hasta"/> del empleado activo con ese código visible; Nothing si no existe o está inactivo.</summary>
    Public Function CalcularSaldo(codigoVisible As String, hasta As Date) As Double?

        Dim saldo As Double? = Nothing

        EnTransaccion(Sub(cn, tr) saldo = CalcularSaldoEn(cn, tr, codigoVisible, hasta, DiasMesEs30(cn, tr)))

        Return saldo

    End Function

    ''' <summary>
    ''' Columnas por posición: 0 = código visible, 1 = nombre, 2 = saldo según Excel (negativo = días a favor que se suman).
    ''' Por cada empleado con diferencia entre el saldo del sistema y el de Excel registra una solicitud "Vacaciones" con
    ''' "AJUSTE AUTOMATICO" en <paramref name="fecha"/>. Con <paramref name="segunNomina"/> el saldo del sistema se limita a 15
    ''' días y se le restan las vacaciones disfrutadas dentro del período de la nómina de vacaciones activa del empleado.
    ''' </summary>
    Public Function ImportarSaldos(filas As DataTable, fecha As Date, segunNomina As Boolean) As CsImportacionDAL.ResultadoImportacion

        Dim res As New CsImportacionDAL.ResultadoImportacion

        EnTransaccion(
            Sub(cn, tr)

                Dim es30 As Boolean = DiasMesEs30(cn, tr)

                Dim ultimo As Object = EscalarEn(cn, tr,
                    "SELECT MAX(CASE WHEN NumeroSolicitud NOT LIKE '%[^0-9]%' AND LEN(NumeroSolicitud) <= 9 " &
                    "THEN CAST(NumeroSolicitud AS int) END) FROM SolicitudVacaciones WITH (TABLOCKX)")
                Dim siguiente As Integer = If(ultimo Is Nothing OrElse IsDBNull(ultimo), 0, Convert.ToInt32(ultimo)) + 1

                For Each fila As DataRow In filas.Rows

                    Dim codigo As String = Texto(fila, 0)

                    If codigo = "" Then Continue For

                    Dim excel As Double

                    If Not Double.TryParse(Texto(fila, 2), excel) Then
                        res.Mensajes.Add(codigo & ", saldo no válido")
                        Continue For
                    End If

                    Dim sistema As Double? = CalcularSaldoEn(cn, tr, codigo, fecha, es30)

                    If Not sistema.HasValue Then
                        res.Mensajes.Add("El empleado " & codigo & " " & Texto(fila, 1) & " no existe o está inactivo")
                        Continue For
                    End If

                    Dim totalSistema As Double = sistema.Value

                    If segunNomina Then

                        Dim periodo As DataTable = Tabla(cn, tr,
                            "SELECT TOP 1 nv.FechaIni, nv.FechaFin FROM DetalleNomVaca d " &
                            "INNER JOIN NomVaca nv ON d.NumNomVaca = nv.NumNomVaca " &
                            "INNER JOIN Empleado e ON d.CodEmpleado = e.CodEmpleado " &
                            "WHERE nv.Activa = 1 AND e.CodEmpleado1 = @Cod", New SqlParameter("@Cod", codigo))

                        If periodo.Rows.Count = 0 OrElse IsDBNull(periodo.Rows(0)("FechaIni")) OrElse IsDBNull(periodo.Rows(0)("FechaFin")) Then
                            res.Mensajes.Add(codigo & ", no está en la nómina de vacaciones activa")
                            Continue For
                        End If

                        If totalSistema > 15 Then

                            ' Anulado NULL cuenta como no anulado (las solicitudes importadas antes no lo traen).
                            Dim disfrutadas As Object = EscalarEn(cn, tr,
                                "SELECT SUM(DiasDisfrutar) FROM SolicitudVacaciones WHERE TipoSolicitud = 'Vacaciones' AND ISNULL(Anulado, 0) = 0 " &
                                "AND CodigoEmpleado = @Cod AND FechaInicio >= @Ini AND FechaFin <= @Fin",
                                New SqlParameter("@Cod", codigo),
                                New SqlParameter("@Ini", periodo.Rows(0)("FechaIni")),
                                New SqlParameter("@Fin", periodo.Rows(0)("FechaFin")))

                            totalSistema = 15 - If(disfrutadas Is Nothing OrElse IsDBNull(disfrutadas), 0, Convert.ToDouble(disfrutadas))

                        End If

                    End If

                    Dim ajuste As Double = If(excel < 0, totalSistema + Math.Abs(excel),
                                              Math.Round(totalSistema - excel, 2, MidpointRounding.AwayFromZero))

                    If ajuste = 0 Then
                        res.Mensajes.Add(codigo & ", sin diferencia")
                        Continue For
                    End If

                    EjecutarEn(cn, tr,
                        "INSERT INTO SolicitudVacaciones (FechaSolicitud, NumeroSolicitud, TipoSolicitud, CodigoEmpleado, DiasVacaciones, " &
                        "DiasDisfrutados, FechaInicio, FechaFin, DiasDisfrutar, Observaciones, Anulado, Pagado) " &
                        "VALUES (@Fecha, @Num, 'Vacaciones', @Cod, 0, 0, @Fecha, @Fecha, @Dias, 'AJUSTE AUTOMATICO', 0, 0)",
                        New SqlParameter("@Fecha", fecha.Date),
                        New SqlParameter("@Num", siguiente.ToString("00000")),
                        New SqlParameter("@Cod", codigo),
                        New SqlParameter("@Dias", ajuste))

                    siguiente += 1
                    res.Importadas += 1
                    res.Mensajes.Add(codigo & ", ajuste de " & ajuste.ToString("0.00") & " días (sistema " & totalSistema.ToString("0.00") & ", Excel " & excel.ToString("0.00") & ")")

                Next

            End Sub)

        Return res

    End Function

    Private Shared Function CalcularSaldoEn(cn As SqlConnection, tr As SqlTransaction, codigoVisible As String, hasta As Date,
                                            es30 As Boolean) As Double?

        Dim contrato As Object = EscalarEn(cn, tr,
            "SELECT TOP 1 h.FechaContratoVac FROM Empleado e INNER JOIN Historico h ON e.CodEmpleado = h.Codempleado " &
            "WHERE e.CodEmpleado1 = @Cod AND e.Activo = 1 ORDER BY h.Id", New SqlParameter("@Cod", codigoVisible))

        If contrato Is Nothing OrElse IsDBNull(contrato) Then Return Nothing

        Return CalculadoraVacaciones.Saldo(Convert.ToDateTime(contrato), hasta, es30,
            Function(desde As Date, limite As Date)

                Dim r As Object = EscalarEn(cn, tr,
                    "SELECT SUM(DiasDisfrutar) FROM SolicitudVacaciones WHERE TipoSolicitud NOT IN " & TiposExcluidos & " " &
                    "AND (CodigoEmpleado = @Cod OR CodigoEmpleado = 'Todos') AND FechaInicio >= @Desde AND FechaInicio < @Limite",
                    New SqlParameter("@Cod", codigoVisible),
                    New SqlParameter("@Desde", desde.Date),
                    New SqlParameter("@Limite", limite.Date.AddDays(1)))

                Return If(r Is Nothing OrElse IsDBNull(r), 0.0, Convert.ToDouble(r))

            End Function)

    End Function

    Private Shared Function DiasMesEs30(cn As SqlConnection, tr As SqlTransaction) As Boolean

        Dim r As Object = EscalarEn(cn, tr, "SELECT TOP 1 DiasMes FROM Controles")

        Return Not (r Is Nothing OrElse IsDBNull(r)) AndAlso Convert.ToDouble(r) = 30

    End Function

    Private Shared Function Tabla(cn As SqlConnection, tr As SqlTransaction, sql As String, ParamArray parametros() As SqlParameter) As DataTable

        Dim dt As New DataTable

        Using cmd As New SqlCommand(sql, cn, tr)
            cmd.Parameters.AddRange(parametros)
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        Return dt

    End Function

    Private Shared Function Texto(fila As DataRow, posicion As Integer) As String

        If posicion >= fila.Table.Columns.Count OrElse IsDBNull(fila(posicion)) Then Return ""

        Return Convert.ToString(fila(posicion)).Trim()

    End Function

End Class
