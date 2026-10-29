Imports System.Data
Imports System.Data.SqlClient

''' <summary>Anotaciones / currículum del empleado (tabla Curriculum, una fila por empleado).</summary>
Public Class CsCurriculumDAL
    Inherits CsDALBase

    ' Largos de las columnas de Curriculum (nvarchar), para validar antes de enviar y para limitar los controles del formulario.
    Public Const LongFaltas As Integer = 2
    Public Const LongCausaSalida As Integer = 150
    Public Const LongJustificacion As Integer = 150
    Public Const LongDatosRecord As Integer = 150
    Public Const LongIdiomas As Integer = 150
    Public Const LongTelefono As Integer = 100
    Public Const LongCursos As Integer = 200
    Public Const LongRazones As Integer = 200
    Public Const LongTrabajoAnterior As Integer = 100
    Public Const LongRecomendaciones As Integer = 200
    ''' <summary>Código visible y nombre completo del empleado; Nothing si no existe.</summary>
    Public Function NombreEmpleado(codEmpleado As Decimal) As String

        Dim r As Object = Escalar(
            "SELECT LTRIM(RTRIM(ISNULL(Nombre1,'') + ' ' + ISNULL(Nombre2,'') + ' ' + ISNULL(Apellido1,'') + ' ' + ISNULL(Apellido2,''))) " &
            "FROM Empleado WHERE CodEmpleado = @Cod",
            New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = codEmpleado})

        If r Is Nothing OrElse IsDBNull(r) Then Return Nothing

        Return Convert.ToString(r)

    End Function

    Public Function Buscar(codEmpleado As Decimal) As CurriculumEmpleado

        Dim dt As DataTable = ConsultarTabla(
            "SELECT TOP 1 * FROM Curriculum WHERE CodEmpleado = @Cod ORDER BY Id",
            New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = codEmpleado})

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New CurriculumEmpleado With {
            .Cod_Empleado = codEmpleado,
            .Fecha_Contratacion = Fecha(r("FechaContratacion")),
            .Fecha_Finalizacion = Fecha(r("FechaFinalizacion")),
            .Causa_Salida = Texto(r("CausaSalida")),
            .Faltas = Texto(r("Faltas")),
            .Justificacion_Faltas = Texto(r("JustificacionFaltas")),
            .Datos_Record = Texto(r("DatosRecord")),
            .Idiomas = Texto(r("Idiomas")),
            .Telefono_Emergencia = Texto(r("TelefonoCasoEmergencia")),
            .Cursos = Texto(r("Cursos")),
            .Razones_Contratacion = Texto(r("RazonesContratacion")),
            .Trabajo_Anterior = Texto(r("TrabajoAnterior")),
            .Recomendaciones = Texto(r("Recomendaciones"))
        }

    End Function

    ''' <summary>Crea la anotación del empleado o, si ya existe, la actualiza.</summary>
    Public Sub Guardar(c As CurriculumEmpleado)

        If c.Fecha_Finalizacion < c.Fecha_Contratacion Then
            Throw New InvalidOperationException("La fecha de finalización no puede ser anterior a la de contratación.")
        End If

        Validar("Faltas", c.Faltas, LongFaltas)
        Validar("Causa de salida", c.Causa_Salida, LongCausaSalida)
        Validar("Justificación de faltas", c.Justificacion_Faltas, LongJustificacion)
        Validar("Datos del récord", c.Datos_Record, LongDatosRecord)
        Validar("Idiomas", c.Idiomas, LongIdiomas)
        Validar("Teléfono en caso de emergencia", c.Telefono_Emergencia, LongTelefono)
        Validar("Cursos", c.Cursos, LongCursos)
        Validar("Razones de contratación", c.Razones_Contratacion, LongRazones)
        Validar("Trabajos anteriores", c.Trabajo_Anterior, LongTrabajoAnterior)
        Validar("Recomendaciones", c.Recomendaciones, LongRecomendaciones)

        EnTransaccion(
            Sub(cn, tr)

                Dim parametros() As SqlParameter = {
                    New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = c.Cod_Empleado},
                    New SqlParameter("@FIni", SqlDbType.SmallDateTime) With {.Value = c.Fecha_Contratacion},
                    New SqlParameter("@FFin", SqlDbType.SmallDateTime) With {.Value = c.Fecha_Finalizacion},
                    New SqlParameter("@Causa", c.Causa_Salida),
                    New SqlParameter("@Faltas", c.Faltas),
                    New SqlParameter("@JustFaltas", c.Justificacion_Faltas),
                    New SqlParameter("@Record", c.Datos_Record),
                    New SqlParameter("@Idiomas", c.Idiomas),
                    New SqlParameter("@Tel", c.Telefono_Emergencia),
                    New SqlParameter("@Cursos", c.Cursos),
                    New SqlParameter("@Razones", c.Razones_Contratacion),
                    New SqlParameter("@Trab", c.Trabajo_Anterior),
                    New SqlParameter("@Recom", c.Recomendaciones)}

                Dim existe As Boolean = Convert.ToInt32(EscalarEn(cn, tr,
                    "SELECT COUNT(*) FROM Curriculum WITH (UPDLOCK, HOLDLOCK) WHERE CodEmpleado = @C",
                    New SqlParameter("@C", SqlDbType.Decimal) With {.Value = c.Cod_Empleado})) > 0

                If existe Then
                    EjecutarEn(cn, tr,
                        "UPDATE Curriculum SET FechaContratacion=@FIni, FechaFinalizacion=@FFin, CausaSalida=@Causa, Faltas=@Faltas, " &
                        "JustificacionFaltas=@JustFaltas, DatosRecord=@Record, Idiomas=@Idiomas, TelefonoCasoEmergencia=@Tel, " &
                        "Cursos=@Cursos, RazonesContratacion=@Razones, TrabajoAnterior=@Trab, Recomendaciones=@Recom " &
                        "WHERE CodEmpleado=@Cod", parametros)
                Else
                    EjecutarEn(cn, tr,
                        "INSERT INTO Curriculum (Id, CodEmpleado, FechaContratacion, FechaFinalizacion, CausaSalida, Faltas, " &
                        "JustificacionFaltas, DatosRecord, Idiomas, TelefonoCasoEmergencia, Cursos, RazonesContratacion, " &
                        "TrabajoAnterior, Recomendaciones) " &
                        "SELECT ISNULL(MAX(Id), 0) + 1, @Cod, @FIni, @FFin, @Causa, @Faltas, @JustFaltas, @Record, @Idiomas, " &
                        "@Tel, @Cursos, @Razones, @Trab, @Recom FROM Curriculum WITH (TABLOCKX)", parametros)
                End If

            End Sub)

    End Sub

    Public Function Eliminar(codEmpleado As Decimal) As Boolean
        Return Ejecutar("DELETE FROM Curriculum WHERE CodEmpleado = @Cod",
                        New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = codEmpleado}) > 0
    End Function

    Private Shared Sub Validar(campo As String, valor As String, largo As Integer)
        If valor IsNot Nothing AndAlso valor.Length > largo Then
            Throw New InvalidOperationException("El campo '" & campo & "' admite como máximo " & largo & " caracteres.")
        End If
    End Sub

    Private Shared Function Texto(v As Object) As String
        Return If(IsDBNull(v), "", Convert.ToString(v))
    End Function

    Private Shared Function Fecha(v As Object) As DateTime
        Return If(IsDBNull(v), DateTime.Today, Convert.ToDateTime(v))
    End Function

End Class
