''' <summary>Datos del empleado que muestra la pantalla de solicitudes.</summary>
Public Class EmpleadoSolicitud

    Public Property Codigo_Empleado1 As String = ""
    Public Property Nombre_Completo As String = ""
    Public Property Departamento As String = ""
    Public Property Cargo As String = ""
    ''' <summary>Historico.FechaContratoVac (inicio del cómputo de vacaciones).</summary>
    Public Property Fecha_Contrato_Vac As Date?
    ''' <summary>TipoNomina.Horas del tipo de nómina del empleado (0 si no tiene).</summary>
    Public Property Horas_Dia As Double

End Class
