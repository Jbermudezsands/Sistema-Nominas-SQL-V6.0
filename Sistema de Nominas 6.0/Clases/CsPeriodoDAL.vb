Imports System.Data
Imports System.Data.SqlClient

''' <summary>Qué calendario se edita: el de planilla (Fecha_Planilla) o el fiscal (PeriodoFiscal).</summary>
Public Enum TipoCalendario
    Planilla
    Fiscal
End Enum

''' <summary>Resultado de grabar un calendario.</summary>
Public Class ResumenPeriodos
    Public Property Insertados As Integer
    Public Property Actualizados As Integer
    ''' <summary>Períodos con nómina calculada: se dejan como están.</summary>
    Public Property Omitidos As Integer
End Class

''' <summary>
''' Acceso a los calendarios de períodos. Planilla = Fecha_Planilla + Año_Actual;
''' Fiscal = PeriodoFiscal + Año_Fiscal (misma estructura de columnas).
''' </summary>
Public Class CsPeriodoDAL
    Inherits CsDALBase

    ' Constantes del código (no vienen del usuario), por eso se pueden concatenar.
    Private ReadOnly _tPeriodos As String
    Private ReadOnly _tAnio As String

    Public Sub New(calendario As TipoCalendario)
        If calendario = TipoCalendario.Planilla Then
            _tPeriodos = "[Fecha_Planilla]"
            _tAnio = "[Año_Actual]"
        Else
            _tPeriodos = "[PeriodoFiscal]"
            _tAnio = "[Año_Fiscal]"
        End If
    End Sub

    Public Function ListarTiposNomina() As DataTable
        Return ConsultarTabla("SELECT CodTipoNomina, Nomina, Periodo FROM TipoNomina ORDER BY CodTipoNomina")
    End Function

    ''' <summary>Año marcado como actual para la nómina; Nothing si no hay ninguno.</summary>
    Public Function ObtenerAnioActual(codTipoNomina As String) As AnioPeriodo

        Dim dt As DataTable = ConsultarTabla(
            "SELECT TOP 1 [Año], CodTipoNomina, Actual, FecIni, FecFin FROM " & _tAnio &
            " WHERE CodTipoNomina = @Cod AND Actual = 1 ORDER BY [Año] DESC",
            New SqlParameter("@Cod", codTipoNomina))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New AnioPeriodo With {
            .Anio = Convert.ToInt32(r("Año")),
            .Cod_Tipo_Nomina = Convert.ToString(r("CodTipoNomina")),
            .Actual = True,
            .Fecha_Inicio = Convert.ToDateTime(r("FecIni")),
            .Fecha_Fin = Convert.ToDateTime(r("FecFin"))
        }

    End Function

    ''' <summary>Períodos ya grabados de una nómina y año, por número de período.</summary>
    Public Function ListarExistentes(codTipoNomina As String, anio As Integer) As Dictionary(Of Integer, PeriodoNomina)

        Dim dt As DataTable = ConsultarTabla(
            "SELECT Periodo, [año], mes, Inicio, Final, Actual, Calculada, NumNomina FROM " & _tPeriodos &
            " WHERE CodTipoNomina = @Cod AND [año] = @Anio ORDER BY Periodo",
            New SqlParameter("@Cod", codTipoNomina),
            New SqlParameter("@Anio", anio))

        Dim d As New Dictionary(Of Integer, PeriodoNomina)

        For Each r As DataRow In dt.Rows

            Dim p As New PeriodoNomina With {
                .Periodo = Convert.ToInt32(r("Periodo")),
                .Anio = anio,
                .Mes = Convert.ToString(r("mes")),
                .Inicio = Convert.ToDateTime(r("Inicio")),
                .Final = Convert.ToDateTime(r("Final")),
                .Actual = Not IsDBNull(r("Actual")) AndAlso Convert.ToBoolean(r("Actual")),
                .Calculada = Not IsDBNull(r("Calculada")) AndAlso Convert.ToBoolean(r("Calculada")),
                .Num_Nomina = If(IsDBNull(r("NumNomina")), 0, Convert.ToInt32(r("NumNomina")))
            }

            d(p.Periodo) = p

        Next

        Return d

    End Function

    ''' <summary>
    ''' Activa el año para la nómina y graba su calendario (todo en una transacción).
    ''' Como en la V.5.0, el período 1 de los períodos nuevos queda como actual.
    ''' Los períodos que ya existen solo cambian de fechas si no tienen nómina calculada.
    ''' </summary>
    Public Function Guardar(codTipoNomina As String, anio As AnioPeriodo, periodos As IEnumerable(Of PeriodoNomina)) As ResumenPeriodos

        Dim resumen As New ResumenPeriodos

        Using cn As New SqlConnection(CadenaConexion)

            cn.Open()

            Using tr As SqlTransaction = cn.BeginTransaction()

                Try

                    EjecutarTx(cn, tr, "UPDATE " & _tAnio & " SET Actual = 0 WHERE CodTipoNomina = @Cod AND Actual = 1",
                             New SqlParameter("@Cod", codTipoNomina))

                    Dim pAnio() As SqlParameter = {
                        New SqlParameter("@Cod", codTipoNomina),
                        New SqlParameter("@Anio", anio.Anio),
                        New SqlParameter("@Ini", SqlDbType.SmallDateTime) With {.Value = anio.Fecha_Inicio},
                        New SqlParameter("@Fin", SqlDbType.SmallDateTime) With {.Value = anio.Fecha_Fin}
                    }

                    If EjecutarTx(cn, tr,
                                "UPDATE " & _tAnio & " SET Actual = 1, FecIni = @Ini, FecFin = @Fin " &
                                "WHERE [Año] = @Anio AND CodTipoNomina = @Cod", pAnio) = 0 Then

                        EjecutarTx(cn, tr,
                                 "INSERT INTO " & _tAnio & " ([Año], CodTipoNomina, Actual, FecIni, FecFin) " &
                                 "VALUES (@Anio, @Cod, 1, @Ini, @Fin)",
                                 CopiarParametros(pAnio))
                    End If

                    For Each per As PeriodoNomina In periodos

                        Dim fila As DataTable = Tabla(cn, tr,
                            "SELECT Calculada, NumNomina FROM " & _tPeriodos &
                            " WHERE CodTipoNomina = @Cod AND [año] = @Anio AND Periodo = @Per",
                            New SqlParameter("@Cod", codTipoNomina),
                            New SqlParameter("@Anio", anio.Anio),
                            New SqlParameter("@Per", per.Periodo))

                        Dim pPer() As SqlParameter = {
                            New SqlParameter("@Cod", codTipoNomina),
                            New SqlParameter("@Anio", anio.Anio),
                            New SqlParameter("@Per", per.Periodo),
                            New SqlParameter("@Mes", per.Mes),
                            New SqlParameter("@Ini", SqlDbType.SmallDateTime) With {.Value = per.Inicio},
                            New SqlParameter("@Fin", SqlDbType.SmallDateTime) With {.Value = per.Final},
                            New SqlParameter("@Actual", per.Actual)
                        }

                        If fila.Rows.Count = 0 Then

                            EjecutarTx(cn, tr,
                                     "INSERT INTO " & _tPeriodos & " (Periodo, [año], mes, CodTipoNomina, Inicio, Final, Actual, Calculada) " &
                                     "VALUES (@Per, @Anio, @Mes, @Cod, @Ini, @Fin, @Actual, 0)", pPer)
                            resumen.Insertados += 1

                        Else

                            Dim r As DataRow = fila.Rows(0)
                            Dim calculada As Boolean = Not IsDBNull(r("Calculada")) AndAlso Convert.ToBoolean(r("Calculada"))
                            Dim conNomina As Boolean = Not IsDBNull(r("NumNomina")) AndAlso Convert.ToInt32(r("NumNomina")) > 0

                            If calculada OrElse conNomina Then
                                resumen.Omitidos += 1
                            Else
                                EjecutarTx(cn, tr,
                                         "UPDATE " & _tPeriodos & " SET mes = @Mes, Inicio = @Ini, Final = @Fin " &
                                         "WHERE CodTipoNomina = @Cod AND [año] = @Anio AND Periodo = @Per", pPer)
                                resumen.Actualizados += 1
                            End If

                        End If

                    Next

                    tr.Commit()

                Catch
                    tr.Rollback()
                    Throw
                End Try

            End Using

        End Using

        Return resumen

    End Function

    Private Shared Function EjecutarTx(cn As SqlConnection, tr As SqlTransaction, sql As String, ParamArray p() As SqlParameter) As Integer

        Using cmd As New SqlCommand(sql, cn, tr)
            cmd.Parameters.AddRange(p)
            Return cmd.ExecuteNonQuery()
        End Using

    End Function

    Private Shared Function Tabla(cn As SqlConnection, tr As SqlTransaction, sql As String, ParamArray p() As SqlParameter) As DataTable

        Dim dt As New DataTable

        Using cmd As New SqlCommand(sql, cn, tr)
            cmd.Parameters.AddRange(p)
            Using da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End Using
        End Using

        Return dt

    End Function

    ''' <summary>Un SqlParameter no se puede reutilizar en otro comando.</summary>
    Private Shared Function CopiarParametros(p() As SqlParameter) As SqlParameter()
        Return p.Select(Function(x) New SqlParameter(x.ParameterName, x.SqlDbType) With {.Value = x.Value}).ToArray()
    End Function

End Class
