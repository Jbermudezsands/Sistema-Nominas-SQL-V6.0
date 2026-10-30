''' <summary>
''' Define cada consulta de <see cref="FrmConsulta"/> (V.5.0 Form_Load de FrmConsulta, un caso por QueProducto).
''' Los SQL son constantes: el usuario filtra en el grid, nunca se concatena su texto.
''' Solo difiere de la V.5.0 en que los empleados se unen con LEFT JOIN (el INNER JOIN ocultaba a los que no tenían cargo o departamento).
''' </summary>
Public Class CatalogoBusquedas

    Private Const NombreCompleto As String =
        "LTRIM(RTRIM(ISNULL(e.Nombre1,'') + ' ' + ISNULL(e.Nombre2,'') + ' ' + ISNULL(e.Apellido1,'') + ' ' + ISNULL(e.Apellido2,''))) AS Nombres"

    Private Const SelectEmpleados As String =
        "SELECT e.CodEmpleado1, " & NombreCompleto & ", e.CodEmpleado, e.NumCedula, e.NumeroInss, " &
        "d.Departamento, c.Cargo, e.Sexo FROM Empleado e " &
        "LEFT JOIN Departamento d ON e.CodDepartamento = d.CodDepartamento " &
        "LEFT JOIN Cargo c ON e.CodCargo = c.CodCargo "

    Public Shared Function Obtener(tipo As TipoBusqueda) As DefinicionBusqueda

        Select Case tipo

            Case TipoBusqueda.Empleado
                Return DefinicionEmpleados("Empleados", SelectEmpleados & "WHERE e.Activo = 1 ORDER BY e.CodEmpleado1")

            Case TipoBusqueda.EmpleadoTodos
                Return DefinicionEmpleados("Empleados (activos e inactivos)", SelectEmpleados & "ORDER BY e.CodEmpleado1")

            Case TipoBusqueda.Solicitud
                Return New DefinicionBusqueda("Solicitudes",
                    "SELECT s.NumeroSolicitud, s.FechaSolicitud, s.TipoSolicitud, s.CodigoEmpleado AS CodEmpleado, " &
                    NombreCompleto & " FROM SolicitudVacaciones s " &
                    "LEFT JOIN Empleado e ON s.CodigoEmpleado = e.CodEmpleado1 ORDER BY s.NumeroSolicitud DESC",
                    "NumeroSolicitud",
                    New ColumnaBusqueda("NumeroSolicitud", "Número", 90),
                    New ColumnaBusqueda("FechaSolicitud", "Fecha", 100, True, "dd/MM/yyyy"),
                    New ColumnaBusqueda("TipoSolicitud", "Tipo", 150),
                    New ColumnaBusqueda("CodEmpleado", "Código", 90),
                    New ColumnaBusqueda("Nombres", "Nombres", 320))

            Case TipoBusqueda.Justificacion
                Return New DefinicionBusqueda("Tipos de justificación",
                    "SELECT Classid, Classname FROM LeaveClass ORDER BY Classid",
                    "Classid",
                    New ColumnaBusqueda("Classid", "Código", 90),
                    New ColumnaBusqueda("Classname", "Justificación", 300))

            Case TipoBusqueda.Proceso
                Return New DefinicionBusqueda("Procesos de producción",
                    "SELECT Descrip, CodProceso, CodReferencia, Ref, Precio, Unid FROM Procesos " &
                    "ORDER BY Descrip, CodProceso, CodReferencia",
                    "CodProceso",
                    New ColumnaBusqueda("Descrip", "Descripción", 360),
                    New ColumnaBusqueda("CodProceso", "Proceso", 90),
                    New ColumnaBusqueda("CodReferencia", "Cód. ref.", 80),
                    New ColumnaBusqueda("Ref", "Referencia", 140),
                    New ColumnaBusqueda("Precio", "Precio", 90, True, "n2"),
                    New ColumnaBusqueda("Unid", "Unid.", 60))

            Case Else
                Throw New ArgumentOutOfRangeException(NameOf(tipo))

        End Select

    End Function

    Private Shared Function DefinicionEmpleados(titulo As String, sql As String) As DefinicionBusqueda
        Return New DefinicionBusqueda(titulo, sql, "CodEmpleado1",
            New ColumnaBusqueda("CodEmpleado1", "Código", 90),
            New ColumnaBusqueda("Nombres", "Nombres", 300),
            New ColumnaBusqueda("CodEmpleado", "Id", 60, False),
            New ColumnaBusqueda("NumCedula", "Cédula", 130),
            New ColumnaBusqueda("NumeroInss", "INSS", 90),
            New ColumnaBusqueda("Departamento", "Departamento", 160),
            New ColumnaBusqueda("Cargo", "Cargo", 160),
            New ColumnaBusqueda("Sexo", "Sexo", 50))
    End Function

End Class
