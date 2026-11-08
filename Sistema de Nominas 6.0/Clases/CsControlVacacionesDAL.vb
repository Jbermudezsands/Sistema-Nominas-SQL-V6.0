Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Datos del control de vacaciones de un empleado (V.5.0 FrmControlVacaciones): ficha del empleado (<see cref="CsSolicitudDAL"/>),
''' saldo a una fecha (<see cref="CsVacacionesDAL"/>, el mismo de las solicitudes) y los días marcados del año en DescuentoDiasVacaciones.
''' </summary>
Public Class CsControlVacacionesDAL
    Inherits CsDALBase

    Private ReadOnly _solicitudes As New CsSolicitudDAL
    Private ReadOnly _vacaciones As New CsVacacionesDAL

    Public Function ListarEmpleados() As DataTable
        Return _solicitudes.ListarEmpleados()
    End Function

    ''' <summary>
    ''' Control del empleado activo con ese código visible para el <paramref name="anio"/> del calendario y el saldo a <paramref name="corte"/>.
    ''' Lanza InvalidOperationException si el empleado no existe o está inactivo.
    ''' </summary>
    Public Function Cargar(codigoVisible As String, anio As Integer, corte As Date) As ControlVacaciones

        Dim emp As EmpleadoSolicitud = _solicitudes.BuscarEmpleado(codigoVisible)

        If emp Is Nothing Then Throw New InvalidOperationException("El empleado no existe o está inactivo.")

        Dim control As New ControlVacaciones With {
            .Codigo = emp.Codigo_Empleado1,
            .Nombre_Completo = emp.Nombre_Completo,
            .Departamento = emp.Departamento,
            .Cargo = emp.Cargo,
            .Fecha_Contrato_Vac = emp.Fecha_Contrato_Vac,
            .Anio = anio,
            .Fecha_Corte = corte.Date}

        ' Sin fecha de contrato de vacaciones no hay acumulado que calcular: el saldo queda en 0 y la pantalla lo avisa.
        If emp.Fecha_Contrato_Vac.HasValue Then
            control.Resumen = If(_vacaciones.CalcularResumen(emp.Codigo_Empleado1, corte), New ResumenVacaciones)
        End If

        Dim dt As DataTable = ConsultarTabla(
            "SELECT FechaDescuento, TipoDescuento, CantDias, NumeroSolicitud FROM DescuentoDiasVacaciones " &
            "WHERE CodigoEmpleado = @Cod AND FechaDescuento >= @Ini AND FechaDescuento < @FinMas1 " &
            "ORDER BY FechaDescuento, TipoDescuento",
            New SqlParameter("@Cod", SqlDbType.NVarChar, 50) With {.Value = emp.Codigo_Empleado1},
            New SqlParameter("@Ini", SqlDbType.SmallDateTime) With {.Value = New Date(anio, 1, 1)},
            New SqlParameter("@FinMas1", SqlDbType.DateTime) With {.Value = New Date(anio + 1, 1, 1)})

        For Each f As DataRow In dt.Rows
            control.Dias.Add(New DiaVacacion With {
                .Fecha = Convert.ToDateTime(f("FechaDescuento")).Date,
                .Tipo = Convert.ToString(f("TipoDescuento")).Trim(),
                .Cant_Dias = If(IsDBNull(f("CantDias")), 1.0, Convert.ToDouble(f("CantDias"))),
                .Numero_Solicitud = If(IsDBNull(f("NumeroSolicitud")), "", Convert.ToString(f("NumeroSolicitud")))})
        Next

        Return control

    End Function

End Class
