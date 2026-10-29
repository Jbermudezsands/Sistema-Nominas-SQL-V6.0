Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Consola SQL del administrador (FrmQuery) e instalación de los procedimientos y columnas que usa el
''' módulo de asistencia. Ejecuta lo que el administrador escribe: no se parametriza por diseño, así que
''' el formulario que la use debe quedar restringido a quien administra la base.
''' </summary>
Public Class CsConsultaDAL
    Inherits CsDALBase

    Private Const TiempoEsperaSegundos As Integer = 180

    ''' <summary>SELECT/WITH/EXEC devuelven filas; cualquier otra sentencia devuelve las filas afectadas.</summary>
    Public Function EjecutarSentencia(sentencia As String) As ResultadoConsulta

        If String.IsNullOrWhiteSpace(sentencia) Then
            Throw New InvalidOperationException("Escriba la consulta SQL.")
        End If

        Dim esConsulta As Boolean = DevuelveFilas(sentencia)

        Using cn As New SqlConnection(CadenaConexion)
            Using cmd As New SqlCommand(sentencia, cn)

                cmd.CommandTimeout = TiempoEsperaSegundos

                If esConsulta Then

                    Dim dt As New DataTable
                    Using da As New SqlDataAdapter(cmd)
                        da.Fill(dt)
                    End Using

                    Return New ResultadoConsulta With {.EsConsulta = True, .Tabla = dt}

                End If

                cn.Open()

                Return New ResultadoConsulta With {.EsConsulta = False, .FilasAfectadas = cmd.ExecuteNonQuery()}

            End Using
        End Using

    End Function

    ''' <summary>True si la sentencia empieza por SELECT, WITH, EXEC o EXECUTE (se muestra como tabla); si no, es una acción.</summary>
    Public Shared Function DevuelveFilas(sentencia As String) As Boolean
        Return PrimeraPalabra(sentencia) = "SELECT" OrElse PrimeraPalabra(sentencia) = "WITH" OrElse
               PrimeraPalabra(sentencia) = "EXEC" OrElse PrimeraPalabra(sentencia) = "EXECUTE"
    End Function

    ''' <summary>True si la sentencia solo lee (SELECT o WITH): no pide confirmación en pantalla.</summary>
    Public Shared Function SoloLee(sentencia As String) As Boolean
        Return PrimeraPalabra(sentencia) = "SELECT" OrElse PrimeraPalabra(sentencia) = "WITH"
    End Function

    Private Shared Function PrimeraPalabra(sentencia As String) As String

        If String.IsNullOrWhiteSpace(sentencia) Then Return ""

        Return sentencia.Trim().Split({" "c, ControlChars.Tab, ControlChars.Cr, ControlChars.Lf}, 2,
                                      StringSplitOptions.RemoveEmptyEntries)(0).ToUpperInvariant()

    End Function
    ''' <summary>Crea o reemplaza sp_AsistenciaUnificada.</summary>
    Public Sub InstalarAsistenciaUnificada()
        EjecutarScripts(ScriptsAsistenciaUnificada())
    End Sub

    ''' <summary>Agrega las columnas que el módulo de asistencia espera (OPinfo.OPPwd2, UserLeave.Opid y FechaRegistro) si faltan.</summary>
    Public Sub ActualizarEstructuraAsistencia()
        EjecutarScripts(ScriptsEstructuraAsistencia())
    End Sub

    Private Sub EjecutarScripts(scripts As IEnumerable(Of String))

        EnTransaccion(
            Sub(cn, tr)
                For Each s As String In scripts
                    Using cmd As New SqlCommand(s, cn, tr)
                        cmd.CommandTimeout = TiempoEsperaSegundos
                        cmd.ExecuteNonQuery()
                    End Using
                Next
            End Sub)

    End Sub

    ''' <summary>Sentencias de estructura, en el orden en que se ejecutan (cada una es un lote independiente).</summary>
    Public Shared Function ScriptsEstructuraAsistencia() As String()
        Return {
            "IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.OPinfo') AND name = 'OPPwd2') " &
            "ALTER TABLE dbo.OPinfo ADD OPPwd2 varchar(100) NULL",
            "IF NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_OPinfo_OPPwd2') " &
            "EXEC('ALTER TABLE dbo.OPinfo ADD CONSTRAINT DF_OPinfo_OPPwd2 DEFAULT 888888 FOR OPPwd2')",
            "EXEC('UPDATE dbo.OPinfo SET OPPwd2 = ''888888'' WHERE OPPwd2 IS NULL')",
            "IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.UserLeave') AND name = 'Opid') " &
            "ALTER TABLE dbo.UserLeave ADD Opid int NULL",
            "IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.UserLeave') AND name = 'FechaRegistro') " &
            "ALTER TABLE dbo.UserLeave ADD FechaRegistro datetime NULL",
            "IF NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_UserLeave_FechaRegistro') " &
            "EXEC('ALTER TABLE dbo.UserLeave ADD CONSTRAINT DF_UserLeave_FechaRegistro DEFAULT (getdate()) FOR FechaRegistro')",
            "EXEC('UPDATE dbo.UserLeave SET FechaRegistro = BeginTime WHERE FechaRegistro IS NULL')"}
    End Function

    ''' <summary>Quita el procedimiento si existe y lo crea de nuevo (CREATE OR ALTER no existe antes de SQL Server 2016).</summary>
    Public Shared Function ScriptsAsistenciaUnificada() As String()
        Return {
            "IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'sp_AsistenciaUnificada' AND type = 'P') DROP PROCEDURE dbo.sp_AsistenciaUnificada",
            ProcedimientoAsistenciaUnificada}
    End Function

    Private Const ProcedimientoAsistenciaUnificada As String =
"CREATE PROCEDURE [dbo].[sp_AsistenciaUnificada]
 @FechaInicio DATETIME,
 @FechaFin DATETIME,
 @MinutosAlmuerzo INT = 60,
 @DeptDesde VARCHAR(100) = NULL,
 @DeptHasta VARCHAR(100) = NULL,
 @UserDesde VARCHAR(20) = NULL,
 @UserHasta VARCHAR(20) = NULL,
 @TipoReporte VARCHAR(30) = 'ASISTENCIA_MENSUAL'
AS
BEGIN
 SET NOCOUNT ON;
 SET DATEFIRST 1;
 SET ANSI_WARNINGS OFF;
 ;WITH Fechas AS (
  SELECT CAST(@FechaInicio AS DATE) AS Dia
  UNION ALL
  SELECT DATEADD(DAY,1,Dia)
  FROM Fechas
  WHERE Dia < CAST(@FechaFin AS DATE)
 ),
 Emps AS (
  SELECT U.Userid, U.Name, U.Duty, D.DeptName, D.Deptid
  FROM Userinfo U
  LEFT JOIN Dept D ON D.Deptid = U.Deptid
  WHERE 1=1
   AND ((@DeptDesde IS NULL AND @DeptHasta IS NULL)
    OR (D.DeptName BETWEEN ISNULL(@DeptDesde,D.DeptName) AND ISNULL(@DeptHasta,D.DeptName)))
   AND ((@UserDesde IS NULL AND @UserHasta IS NULL)
    OR (U.Userid BETWEEN ISNULL(@UserDesde,U.Userid) AND ISNULL(@UserHasta,U.Userid)))
 ),
 Base AS (
  SELECT E.Userid, E.Name, E.Duty, E.DeptName, F.Dia AS Fecha,
  CASE WHEN DATEPART(WEEKDAY,F.Dia)=7 THEN 0 ELSE DATEPART(WEEKDAY,F.Dia) END AS DiaSemana
  FROM Emps E CROSS JOIN Fechas F
 )
 SELECT * INTO #Base FROM Base OPTION (MAXRECURSION 32767);
 SELECT B.Userid, B.Fecha,
  MIN(C.CheckTime) AS EntradaNoHor,
  MAX(C.CheckTime) AS SalidaNoHor
 INTO #SinHorario
 FROM #Base B
 LEFT JOIN Checkinout C ON C.Userid=B.Userid
 AND C.CheckTime >= CAST(B.Fecha AS DATETIME)
 AND C.CheckTime < DATEADD(DAY,1,CAST(B.Fecha AS DATETIME))
 GROUP BY B.Userid,B.Fecha;
 SELECT B.Userid, B.Fecha,
 ISNULL(UL.Whys,'') AS Why,
 LC.Classname
 INTO #Just
 FROM #Base B
 LEFT JOIN UserLeave UL ON UL.Userid=B.Userid
 AND UL.BeginTime < DATEADD(DAY,1,CAST(B.Fecha AS DATETIME))
 AND UL.EndTime >= CAST(B.Fecha AS DATETIME)
 LEFT JOIN LeaveClass LC ON LC.Classid = UL.LeaveClassid;
 SELECT B.Userid, B.Name, B.Duty, B.DeptName, B.Fecha,
 J.Why, J.Classname
 INTO #DiaFinal
 FROM #Base B
 LEFT JOIN #Just J ON J.Userid=B.Userid AND J.Fecha=B.Fecha;
 IF UPPER(@TipoReporte)='ASISTENCIA_MENSUAL'
 BEGIN
 SELECT D.Userid,
 MAX(D.Name) AS Nombre,
 MAX(D.Duty) AS Cargo,
 MAX(D.DeptName) AS Departamento,
 MAX(D.Why) AS Whys,
 MAX(D.Classname) AS Classname,
 COUNT(*) AS Dias
 FROM #DiaFinal D
 GROUP BY D.Userid
 ORDER BY D.Userid;
 RETURN;
 END
 IF UPPER(@TipoReporte)='ASISTENCIA_DIARIA'
 BEGIN
 SELECT D.* FROM #DiaFinal D
 ORDER BY D.Userid, D.Fecha;
 RETURN;
 END
END"

End Class
