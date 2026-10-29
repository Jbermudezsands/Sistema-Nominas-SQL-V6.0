Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Historial de pagos de un empleado y su estado (activar, desactivar, copiar perfil, detectar duplicados).
''' </summary>
Public Class CsHistorialEmpleadoDAL
    Inherits CsDALBase

    Public Function ListarEmpleados() As DataTable
        Return ConsultarTabla(
            "SELECT CodEmpleado1, CodEmpleado, " &
            "LTRIM(RTRIM(ISNULL(Nombre1,'') + ' ' + ISNULL(Nombre2,'') + ' ' + ISNULL(Apellido1,'') + ' ' + ISNULL(Apellido2,''))) AS Nombres, " &
            "Activo FROM Empleado ORDER BY CodEmpleado1, CodEmpleado")
    End Function

    ''' <summary>Pagos del empleado acumulados por mes y año de nómina.</summary>
    Public Function Historial(codEmpleado As Decimal) As DataTable
        Return ConsultarTabla(
            "SELECT n.Ano AS Año, n.Mes, MIN(n.FechaNomina) AS Fecha, " &
            "SUM(d.SalarioBasico) AS SalarioBasico, SUM(d.Destajo) AS Destajo, SUM(d.Incentivos) AS Incentivos, " &
            "SUM(d.HorasExtras) AS HorasExtras, SUM(d.Comisiones) AS Comisiones, SUM(d.OtrosIngresos) AS OtrosIngresos, " &
            "SUM(d.SalarioBasico + d.Destajo + d.HorasExtras + d.Comisiones + d.OtrosIngresos) AS TotalIngresos, " &
            "SUM(d.Deducciones) AS Deducciones, SUM(d.Prestamo) AS Prestamo, SUM(d.MontoINSS) AS MontoINSS, SUM(d.MontoIR) AS MontoIR, " &
            "SUM(d.Deducciones + d.Prestamo + d.MontoINSS + d.MontoIR) AS TotalEgresos, " &
            "SUM(d.SalarioBasico + d.Destajo + d.HorasExtras + d.Comisiones + d.OtrosIngresos " &
            "- d.Deducciones - d.Prestamo - d.MontoINSS - d.MontoIR) AS NetoPagar, " &
            "SUM(d.INSSPatronal) AS INSSPatronal, SUM(d.IRPatronal) AS IRPatronal, SUM(d.INATEC) AS INATEC " &
            "FROM DetalleNomina d INNER JOIN Nomina n ON n.NumNomina = d.NumNomina " &
            "WHERE d.CodEmpleado = @Cod GROUP BY n.Ano, n.Mes ORDER BY n.Ano, n.Mes",
            New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = codEmpleado})
    End Function

    Public Sub Activar(codEmpleado As Decimal)
        Ejecutar("UPDATE Empleado SET Activo = 1 WHERE CodEmpleado = @Cod",
                 New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = codEmpleado})
    End Sub

    ''' <summary>Desactiva al empleado y lo quita del detalle de la nómina activa (no de las cerradas).</summary>
    Public Sub Desactivar(codEmpleado As Decimal)

        EnTransaccion(
            Sub(cn, tr)

                Dim p As New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = codEmpleado}

                EjecutarEn(cn, tr, "UPDATE Empleado SET Activo = 0 WHERE CodEmpleado = @Cod", p)

                EjecutarEn(cn, tr,
                    "DELETE FROM DetalleNomina WHERE CodEmpleado = @Cod2 " &
                    "AND NumNomina IN (SELECT NumNomina FROM Nomina WHERE Activa = 1)",
                    New SqlParameter("@Cod2", SqlDbType.Decimal) With {.Value = codEmpleado})

            End Sub)

    End Sub

    ''' <summary>Códigos visibles que tienen más de un empleado activo.</summary>
    Public Function ListarDuplicados() As DataTable
        Return ConsultarTabla(
            "SELECT COUNT(CodEmpleado) AS Registros, CodEmpleado1, " &
            "MAX(ISNULL(Nombre1,'') + ' ' + ISNULL(Nombre2,'') + ' ' + ISNULL(Apellido1,'') + ' ' + ISNULL(Apellido2,'')) AS Nombres " &
            "FROM Empleado WHERE Activo = 1 GROUP BY CodEmpleado1 HAVING COUNT(CodEmpleado) > 1 ORDER BY CodEmpleado1")
    End Function

    ''' <summary>
    ''' Crea un empleado nuevo (activo) copiando la ficha, el histórico y el horario de <paramref name="codEmpleado"/>.
    ''' Con <paramref name="reutilizarCodigo"/> el nuevo empleado conserva el código visible; si no, recibe uno
    ''' nuevo con el formato S1 + año + mes + correlativo. Devuelve el código visible del empleado nuevo.
    ''' </summary>
    Public Function CopiarPerfil(codEmpleado As Decimal, reutilizarCodigo As Boolean) As String

        Dim nuevoCodigo As String = ""

        EnTransaccion(
            Sub(cn, tr)

                Dim pOrigen As New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = codEmpleado}

                Dim codigoOrigen As Object = EscalarEn(cn, tr, "SELECT CodEmpleado1 FROM Empleado WHERE CodEmpleado = @Cod", pOrigen)

                If codigoOrigen Is Nothing OrElse IsDBNull(codigoOrigen) Then
                    Throw New InvalidOperationException("El empleado no existe.")
                End If

                If reutilizarCodigo Then
                    nuevoCodigo = Convert.ToString(codigoOrigen)
                Else
                    nuevoCodigo = SiguienteCodigoNuevo(cn, tr)
                End If

                Dim nuevoId As Decimal = Convert.ToDecimal(EscalarEn(cn, tr,
                    "INSERT INTO Empleado (CodEmpleado1, Nombre1, Nombre2, Apellido1, Apellido2, Direccion, Nacionalidad, CodigoPostal, " &
                    "NumCedula, Sexo, NumeroInss, NumeroRuc, CodDepartamento, CodCargo, CodGrupo, Sindicalista, CodTipoNomina, NumHijos, " &
                    "PorcientoIncentivo, SueldoPeriodo, TarifaHoraria, PorcentajeComision, OtrosIngresos, SalarioMinimo, ExentoInss, " &
                    "ExentoIr, PagoInssPatronal, ViaticoxDia, CuentaBanco, Activo, Ausente, SumarSubsidio) " &
                    "OUTPUT INSERTED.CodEmpleado " &
                    "SELECT @Nuevo, Nombre1, Nombre2, Apellido1, Apellido2, Direccion, Nacionalidad, CodigoPostal, " &
                    "NumCedula, Sexo, NumeroInss, NumeroRuc, CodDepartamento, CodCargo, CodGrupo, Sindicalista, CodTipoNomina, NumHijos, " &
                    "0, SueldoPeriodo, TarifaHoraria, PorcentajeComision, OtrosIngresos, SalarioMinimo, ExentoInss, " &
                    "ExentoIr, PagoInssPatronal, ViaticoxDia, CuentaBanco, 1, 0, SumarSubsidio " &
                    "FROM Empleado WHERE CodEmpleado = @Cod2",
                    New SqlParameter("@Nuevo", nuevoCodigo),
                    New SqlParameter("@Cod2", SqlDbType.Decimal) With {.Value = codEmpleado}))

                ' Histórico: el empleado nuevo hereda la fecha de nacimiento; las fechas de contrato son las de hoy.
                EjecutarEn(cn, tr,
                    "INSERT INTO Historico (Id, Codempleado, FechaNacimiento, FechaContrato, FechaContratoVac) " &
                    "SELECT ISNULL(MAX(Id), 0) + 1, @Nuevo, " &
                    "(SELECT TOP 1 FechaNacimiento FROM Historico WHERE Codempleado = @Origen ORDER BY Id), " &
                    "CAST(GETDATE() AS date), CAST(GETDATE() AS date) FROM Historico WITH (TABLOCKX)",
                    New SqlParameter("@Nuevo", SqlDbType.Decimal) With {.Value = nuevoId},
                    New SqlParameter("@Origen", SqlDbType.Decimal) With {.Value = codEmpleado})

                ' Horario: se copia el del empleado de origen si el código visible nuevo todavía no tiene uno.
                EjecutarEn(cn, tr,
                    "INSERT INTO HorarioEmpleado (CodEmpleado, LEntrada, LSalida, MEntrada, MSalida, MCEntrada, MCSalida, JEntrada, JSalida, " &
                    "VEntrada, VSalida, TComida, TurnoLunes, TurnoMartes, TurnoMiercoles, TurnoJueves, TurnoViernes, TurnoSabado, TurnoDomingo, " &
                    "SEntrada, SSalida, DEntrada, DSalida) " &
                    "SELECT @NuevoCod, LEntrada, LSalida, MEntrada, MSalida, MCEntrada, MCSalida, JEntrada, JSalida, " &
                    "VEntrada, VSalida, TComida, TurnoLunes, TurnoMartes, TurnoMiercoles, TurnoJueves, TurnoViernes, TurnoSabado, TurnoDomingo, " &
                    "SEntrada, SSalida, DEntrada, DSalida FROM HorarioEmpleado WHERE CodEmpleado = @CodOrigen " &
                    "AND NOT EXISTS (SELECT 1 FROM HorarioEmpleado WHERE CodEmpleado = @NuevoCod2)",
                    New SqlParameter("@NuevoCod", nuevoCodigo),
                    New SqlParameter("@CodOrigen", Convert.ToString(codigoOrigen)),
                    New SqlParameter("@NuevoCod2", nuevoCodigo))

            End Sub)

        Return nuevoCodigo

    End Function

    ''' <summary>S1 + año(2) + mes(2) + correlativo(4) más alto del mes + 1.</summary>
    Private Shared Function SiguienteCodigoNuevo(cn As SqlConnection, tr As SqlTransaction) As String

        Dim prefijo As String = "S1" & DateTime.Now.ToString("yyMM")

        Dim ultimo As Object = EscalarEn(cn, tr,
            "SELECT MAX(CAST(SUBSTRING(CodEmpleado1, 7, 4) AS int)) FROM Empleado " &
            "WHERE CodEmpleado1 LIKE @P + '[0-9][0-9][0-9][0-9]'",
            New SqlParameter("@P", prefijo))

        Dim n As Integer = If(ultimo Is Nothing OrElse IsDBNull(ultimo), 0, Convert.ToInt32(ultimo))

        Return prefijo & (n + 1).ToString("0000")

    End Function

End Class
