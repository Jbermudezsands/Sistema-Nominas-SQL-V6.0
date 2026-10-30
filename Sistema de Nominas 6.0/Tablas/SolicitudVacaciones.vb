''' <summary>
''' Solicitud de vacaciones, ausencia, subsidio, feriado, etc. (tabla SolicitudVacaciones; V.5.0 TSolicitud de Solicitudes.bas).
''' </summary>
Public Class SolicitudVacaciones

    ''' <summary>Vacío = asignar el siguiente número al grabar.</summary>
    Public Property Numero_Solicitud As String = ""
    ''' <summary>Código visible del empleado (Empleado.CodEmpleado1).</summary>
    Public Property Codigo_Empleado As String = ""
    Public Property Tipo_Solicitud As String = ""
    ''' <summary>Vacío = no se modifica al grabar.</summary>
    Public Property Clasificacion As String = ""
    Public Property Fecha_Solicitud As Date = Date.Today
    Public Property Fecha_Inicio As Date = Date.Today
    Public Property Fecha_Fin As Date = Date.Today
    ''' <summary>Días del rango (1 cuando la solicitud es por horas).</summary>
    Public Property Dias_Solicitud As Double = 1
    ''' <summary>0 = la solicitud es por días.</summary>
    Public Property Horas_Solicitud As Double
    ''' <summary>Informativo: días acumulados al grabar.</summary>
    Public Property Dias_Vacaciones As Double
    ''' <summary>Informativo: días disfrutados al grabar.</summary>
    Public Property Dias_Disfrutados As Double
    ''' <summary>Valor guardado en DiasDisfrutar (días, o fracción de día si es por horas).</summary>
    Public Property Dias_Disfrutar As Double
    Public Property Observaciones As String = ""
    ''' <summary>Nómina que procesó la solicitud (0 = pendiente; no se puede modificar ni eliminar si es distinta de 0).</summary>
    Public Property Num_Nomina As Double

End Class
