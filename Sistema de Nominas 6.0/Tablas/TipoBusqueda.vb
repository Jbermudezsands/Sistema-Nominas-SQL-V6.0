''' <summary>
''' Consultas que ofrece <see cref="FrmConsulta"/> (V.5.0 QueProducto de FrmConsulta).
''' </summary>
Public Enum TipoBusqueda

    ''' <summary>Empleados activos (V.5.0 "CodigoEmpleado", "EmpleadosSoli", "CodigoEmpleadoReportesIni/Fin").</summary>
    Empleado = 0

    ''' <summary>Todos los empleados, activos o no (V.5.0 "CodigoProductoHistorico").</summary>
    EmpleadoTodos = 1

    ''' <summary>Solicitudes de vacaciones y ausencias (V.5.0 "Solicitud").</summary>
    Solicitud = 2

    ''' <summary>Tipos de justificación (V.5.0 "Justifica").</summary>
    Justificacion = 3

    ''' <summary>Procesos de producción con su precio (V.5.0 "Produccion").</summary>
    Proceso = 4

End Enum
