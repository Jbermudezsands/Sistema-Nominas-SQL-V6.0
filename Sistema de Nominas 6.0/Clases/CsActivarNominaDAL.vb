Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Activación de una nómina (V.5.0 FrmActivarNomina): elige el período del calendario, crea la fila de
''' Nomina con totales en 0, marca el período como actual (planilla y fiscal) y deja activo el tipo de nómina.
''' </summary>
Public Class CsActivarNominaDAL
    Inherits CsDALBase

    Private Const SqlNominaCerradaEn As String =
        "SELECT COUNT(*) FROM Nomina WHERE Cerrada = 1 AND CodTipoNomina = @Cod AND FechaNomina BETWEEN @Ini AND @Fin"

#Region "CONSULTAS"

    ''' <summary>Tipos de nómina sin nómina activa, que son los que se pueden activar.</summary>
    Public Function ListarTiposInactivos() As DataTable
        Return ConsultarTabla(
            "SELECT CodTipoNomina, Nomina, Periodo FROM TipoNomina WHERE ISNULL(Activa, 0) = 0 ORDER BY CodTipoNomina")
    End Function

    ''' <summary>Períodos del calendario de planilla que aún no se calculan, para el combo.</summary>
    Public Function ListarPeriodos(codTipoNomina As String, anio As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT Periodo, mes, Inicio, Final FROM Fecha_Planilla " &
            "WHERE CodTipoNomina = @Cod AND [año] = @Anio AND ISNULL(Calculada, 0) = 0 ORDER BY Periodo",
            New SqlParameter("@Cod", codTipoNomina),
            New SqlParameter("@Anio", SqlDbType.SmallInt) With {.Value = anio})
    End Function

    ''' <summary>
    ''' Período que toca activar: el período actual del calendario si su nómina aún no se cierra; si ya se
    ''' cerró, el siguiente (o el primero del año siguiente). Lanza una excepción con el motivo si no hay.
    ''' </summary>
    Public Function SugerirPeriodo(codTipoNomina As String) As PeriodoNomina

        Dim actual As PeriodoNomina = BuscarPeriodo(
            "SELECT TOP 1 Periodo, [año], mes, Inicio, Final, ISNULL(Actual, 0) AS Actual, ISNULL(Calculada, 0) AS Calculada, " &
            "ISNULL(NumNomina, 0) AS NumNomina FROM Fecha_Planilla WHERE CodTipoNomina = @Cod AND Actual = 1 " &
            "ORDER BY [año] DESC, Periodo DESC",
            New SqlParameter("@Cod", codTipoNomina))

        If actual Is Nothing Then
            Throw New Exception("No está definido el período actual de la nómina.")
        End If

        If Not NominaCerrada(codTipoNomina, actual.Inicio, actual.Final) Then Return actual

        Dim siguiente As PeriodoNomina = BuscarPeriodo(
            "SELECT TOP 1 Periodo, [año], mes, Inicio, Final, ISNULL(Actual, 0) AS Actual, ISNULL(Calculada, 0) AS Calculada, " &
            "ISNULL(NumNomina, 0) AS NumNomina FROM Fecha_Planilla WHERE CodTipoNomina = @Cod " &
            "AND (([año] = @Anio AND Periodo > @Per) OR [año] > @Anio) ORDER BY [año], Periodo",
            New SqlParameter("@Cod", codTipoNomina),
            New SqlParameter("@Anio", SqlDbType.SmallInt) With {.Value = actual.Anio},
            New SqlParameter("@Per", SqlDbType.TinyInt) With {.Value = actual.Periodo})

        If siguiente Is Nothing Then
            Throw New Exception("No se puede asignar ningún período a esta nómina." & vbLf &
                                "Necesita crear un nuevo período; de lo contrario no podrá activar la nómina.")
        End If

        Return siguiente

    End Function

    Private Function BuscarPeriodo(sql As String, ParamArray parametros() As SqlParameter) As PeriodoNomina

        Dim dt As DataTable = ConsultarTabla(sql, parametros)

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New PeriodoNomina With {
            .Periodo = Convert.ToInt32(r("Periodo")),
            .Anio = Convert.ToInt32(r("año")),
            .Mes = Convert.ToString(r("mes")).Trim,
            .Inicio = Convert.ToDateTime(r("Inicio")),
            .Final = Convert.ToDateTime(r("Final")),
            .Actual = Convert.ToBoolean(r("Actual")),
            .Calculada = Convert.ToBoolean(r("Calculada")),
            .Num_Nomina = Convert.ToInt32(r("NumNomina"))
        }

    End Function

    Private Function NominaCerrada(codTipoNomina As String, inicio As Date, final As Date) As Boolean
        Return Convert.ToInt32(Escalar(SqlNominaCerradaEn, ParametrosRango(codTipoNomina, inicio, final))) > 0
    End Function

    Private Shared Function ParametrosRango(codTipoNomina As String, inicio As Date, final As Date) As SqlParameter()
        Return {
            New SqlParameter("@Cod", codTipoNomina),
            New SqlParameter("@Ini", SqlDbType.SmallDateTime) With {.Value = inicio.Date},
            New SqlParameter("@Fin", SqlDbType.SmallDateTime) With {.Value = final.Date}
        }
    End Function

#End Region

#Region "ACTIVAR"

    ''' <summary>
    ''' Activa la nómina del período indicado y devuelve su número. <paramref name="final"/> es la fecha de
    ''' pago (FechaNomina). Todo en una transacción: si algo falla no queda nada a medias.
    ''' </summary>
    Public Function Activar(codTipoNomina As String, anio As Integer, periodo As Integer,
                            inicio As Date, final As Date) As Integer

        If final.Date < inicio.Date Then
            Throw New Exception("La fecha final de la nómina no puede ser menor que la inicial.")
        End If

        Dim numNomina As Integer = 0

        EnTransaccion(
            Sub(cn, tr)

                Dim pCod As Func(Of SqlParameter) = Function() New SqlParameter("@Cod", codTipoNomina)
                Dim pAnio As Func(Of SqlParameter) = Function() New SqlParameter("@Anio", SqlDbType.SmallInt) With {.Value = anio}
                Dim pPer As Func(Of SqlParameter) = Function() New SqlParameter("@Per", SqlDbType.TinyInt) With {.Value = periodo}
                Dim pIni As Func(Of SqlParameter) = Function() New SqlParameter("@Ini", SqlDbType.SmallDateTime) With {.Value = inicio.Date}
                Dim pFin As Func(Of SqlParameter) = Function() New SqlParameter("@Fin", SqlDbType.SmallDateTime) With {.Value = final.Date}

                ' 1) El tipo de nómina debe existir y no estar activo.
                Dim activa As Object = EscalarEn(cn, tr, "SELECT ISNULL(Activa, 0) FROM TipoNomina WITH (UPDLOCK) WHERE CodTipoNomina = @Cod", pCod())

                If activa Is Nothing Then
                    Throw New Exception("No existe el tipo de nómina " & codTipoNomina & ".")
                End If

                If Convert.ToBoolean(activa) Then
                    Throw New Exception("El tipo de nómina " & codTipoNomina & " ya tiene una nómina activa.")
                End If

                ' 2) El período debe existir en el calendario.
                Dim mes As Object = EscalarEn(cn, tr,
                    "SELECT mes FROM Fecha_Planilla WHERE CodTipoNomina = @Cod AND [año] = @Anio AND Periodo = @Per",
                    pCod(), pAnio(), pPer())

                If mes Is Nothing Then
                    Throw New Exception("No se puede asignar ningún período a esta nómina." & vbLf &
                                        "Necesita crear un nuevo período; de lo contrario no podrá activar la nómina.")
                End If

                ' 3) Ese rango no puede tener ya una nómina cerrada.
                If Convert.ToInt32(EscalarEn(cn, tr, SqlNominaCerradaEn, pCod(), pIni(), pFin())) > 0 Then
                    Throw New Exception("La fecha de esta nómina ya fue activada. Consulte a su soporte.")
                End If

                ' 4) Número de nómina: consecutivo bloqueado hasta el commit.
                Dim consecutivo As DataTable = New DataTable
                Using cmd As New SqlCommand("SELECT TOP 1 Id, ISNULL(Nominas, 1) AS Nominas FROM Consecutivos WITH (UPDLOCK, HOLDLOCK) ORDER BY Id", cn, tr)
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(consecutivo)
                    End Using
                End Using

                If consecutivo.Rows.Count = 0 Then
                    Throw New Exception("No está definida la tabla de consecutivos.")
                End If

                numNomina = Convert.ToInt32(consecutivo.Rows(0)("Nominas"))

                EjecutarEn(cn, tr, "UPDATE Consecutivos SET Nominas = @Sig WHERE Id = @Id",
                           New SqlParameter("@Sig", SqlDbType.Int) With {.Value = numNomina + 1},
                           New SqlParameter("@Id", SqlDbType.Int) With {.Value = Convert.ToInt32(consecutivo.Rows(0)("Id"))})

                ' 5) Fila de la nómina con todos los totales en 0.
                EjecutarEn(cn, tr,
                    "INSERT INTO Nomina (NumNomina, CodTipoNomina, FechaNomina, FechaNominaINI, Activa, TotalSalarioBasico, TotalDestajo, " &
                    "TotalHorasExtras, TotalComisiones, TotalIncentivos, TotalDeducciones, TotalPrestamo, TotalMontoINSS, TotalMontoIR, " &
                    "TotalVacaciones, TotalINSSPatronal, TotalIRPatronal, Anulada, Cerrada, Procesada, Mes, Ano, Periodo) " &
                    "VALUES (@Num, @Cod, @Fin, @Ini, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, @Mes, @Anio, @Per)",
                    New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina},
                    pCod(), pFin(), pIni(),
                    New SqlParameter("@Mes", SqlDbType.Decimal) With {.Value = Convert.ToDecimal(Integer.Parse(Convert.ToString(mes).Trim))},
                    New SqlParameter("@Anio", SqlDbType.Decimal) With {.Value = Convert.ToDecimal(anio)},
                    New SqlParameter("@Per", SqlDbType.Decimal) With {.Value = Convert.ToDecimal(periodo)})

                ' 6) Calendario de planilla: este período queda como el actual y apunta a la nómina.
                EjecutarEn(cn, tr, "UPDATE Fecha_Planilla SET Actual = 0 WHERE CodTipoNomina = @Cod AND Actual = 1", pCod())
                EjecutarEn(cn, tr,
                    "UPDATE Fecha_Planilla SET Actual = 1, NumNomina = @Num WHERE CodTipoNomina = @Cod AND [año] = @Anio AND Periodo = @Per",
                    New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina}, pCod(), pAnio(), pPer())

                ' 7) Calendario fiscal: solo si tiene un período que empiece ese día.
                If Convert.ToInt32(EscalarEn(cn, tr,
                        "SELECT COUNT(*) FROM PeriodoFiscal WHERE CodTipoNomina = @Cod AND Inicio = @Ini", pCod(), pIni())) > 0 Then

                    EjecutarEn(cn, tr, "UPDATE PeriodoFiscal SET Actual = 0 WHERE CodTipoNomina = @Cod AND Actual = 1", pCod())
                    EjecutarEn(cn, tr,
                        "UPDATE PeriodoFiscal SET Actual = 1, NumNomina = @Num WHERE CodTipoNomina = @Cod AND Inicio = @Ini",
                        New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina}, pCod(), pIni())
                End If

                ' 8) El tipo de nómina queda activo.
                EjecutarEn(cn, tr, "UPDATE TipoNomina SET Activa = 1 WHERE CodTipoNomina = @Cod", pCod())

            End Sub)

        Return numNomina

    End Function

#End Region

End Class
