Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Consultas de los reportes de la nómina activa (FrmNominaActiva de la V.5.0): pago por cheque, horas extra, producción, resumen
''' por cargo, percepciones y la nómina sin formato. Las nóminas por código, departamento y cargo salen de
''' <see cref="CsPlanillaNominaDAL.ListarParaReporte"/>.
''' </summary>
Public Class CsNominaActivaDAL
    Inherits CsDALBase

    Private Shared Function Num(numNomina As Integer) As SqlParameter
        Return New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina}
    End Function

    ''' <summary>Empleados que se pagan con cheque (Empleado.PagoCheque): el neto resta también pensión y embargo, que ya van en SqlDeducir.</summary>
    Public Function ListarCheques(numNomina As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & CsPlanillaNominaDAL.SqlNombres & " AS Nombres, ISNULL(E.NumCedula, '') AS NumCedula, " &
            "ISNULL(Dp.Departamento, '') AS Departamento, " & CsPlanillaNominaDAL.SqlDevengado & " - " & CsPlanillaNominaDAL.SqlDeducir & " AS NetoPagar " &
            "FROM DetalleNomina D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado " &
            "LEFT JOIN Departamento Dp ON Dp.CodDepartamento = E.CodDepartamento " &
            "WHERE D.NumNomina = @Num AND E.PagoCheque = 1 ORDER BY E.CodEmpleado1", Num(numNomina))
    End Function

    Public Function ListarHorasExtra(numNomina As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & CsPlanillaNominaDAL.SqlNombres & " AS Nombres, H.CantHoras, H.NumNomina " &
            "FROM Empleado E INNER JOIN HorasExtras H ON E.CodEmpleado = H.CodEmpleado " &
            "WHERE H.CantHoras <> 0 AND H.NumNomina = @Num ORDER BY E.CodEmpleado1", Num(numNomina))
    End Function

    Public Function ListarProduccion(numNomina As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & CsPlanillaNominaDAL.SqlNombres & " AS Nombres, P.CodReferencia, P.CodProceso, P.Ref, P.Lunes, P.Martes, P.Miercoles, " &
            "P.Jueves, P.Viernes, P.Sabado, P.Domingo, P.TotalUnidades, P.SalarioPieza, P.Precio, P.Unidad, P.Pagado " &
            "FROM DetalleProduccion P INNER JOIN Empleado E ON P.CodEmpleado = E.CodEmpleado WHERE P.NumNomina = @Num " &
            "ORDER BY E.CodEmpleado1, P.CodReferencia, P.CodProceso", Num(numNomina))
    End Function

    ''' <summary>Resumen por cargo: empleados y totales (V.5.0 ArepNominaDptoResumen); omite a quienes no tienen devengado.</summary>
    Public Function ListarResumenPorCargo(numNomina As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT ISNULL(C.Cargo, '') AS Cargo, COUNT(*) AS Empleados, SUM(D.SalarioBasico) AS SalarioBasico, SUM(D.Destajo) AS Destajo, SUM(D.SeptimoDia) AS SeptimoDia, " &
            "SUM(D.HorasExtras) AS HorasExtras, SUM(D.IncetivoProduccion) AS IncetivoProduccion, SUM(D.OtrosIngresos) AS OtrosIngresos, " &
            "SUM(" & CsPlanillaNominaDAL.SqlDevengado & ") AS TotalDevengado, SUM(D.MontoINSS) AS MontoINSS, SUM(D.MontoIR) AS MontoIR, " &
            "SUM(" & CsPlanillaNominaDAL.SqlDeducir & ") AS TotalDeducir, SUM(" & CsPlanillaNominaDAL.SqlDevengado & " - " & CsPlanillaNominaDAL.SqlDeducir & ") AS NetoPagar " &
            "FROM DetalleNomina D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado LEFT JOIN Cargo C ON C.CodCargo = E.CodCargo " &
            "WHERE D.NumNomina = @Num AND " & CsPlanillaNominaDAL.SqlDevengado & " <> 0 GROUP BY ISNULL(C.Cargo, '') ORDER BY ISNULL(C.Cargo, '')", Num(numNomina))
    End Function

    ''' <summary>
    ''' Percepciones y deducciones totales de la nómina, como lista Concepto / Monto (V.5.0 ArepPersepciones). Diferencia con la V.5.0:
    ''' los adelantos ya van dentro de "Otras deducciones" y los días de descuento se muestran como días, no se suman a las deducciones.
    ''' </summary>
    Public Function ListarPercepciones(numNomina As Integer) As DataTable

        Dim t As DataTable = ConsultarTabla(
            "SELECT SUM(D.SalarioBasico) AS Salario, SUM(D.Destajo) AS Produccion, SUM(D.HorasExtras) AS HorasExtra, SUM(D.Comisiones) AS Puntualidad, " &
            "SUM(D.VacacionesPagadas) AS Vacaciones, SUM(D.SeptimoDia) AS Septimo, SUM(D.IncetivoProduccion) AS IncProd, SUM(D.BonoProduccion) AS Bono, " &
            "SUM(D.Incentivos) AS Antiguedad, SUM(D.OtrosIngresos) AS Otros, SUM(" & CsPlanillaNominaDAL.SqlDevengado & ") AS Devengado, " &
            "SUM(D.Deducciones) AS Deducciones, SUM(D.Adelantos) AS Adelantos, SUM(D.Prestamo) AS Prestamo, SUM(D.MontoINSS) AS Inss, SUM(D.MontoIR) AS Ir, " &
            "SUM(D.MontoPension) AS Pension, SUM(D.EmbargoJudicial) AS Embargo, SUM(D.DiasDescuento) AS DiasDescuento, SUM(" & CsPlanillaNominaDAL.SqlDeducir & ") AS TotalDeducir, " &
            "SUM(D.INSSPatronal) AS InssPatronal, SUM(D.IRPatronal) AS IrPatronal, SUM(D.INATEC) AS Inatec, SUM(D.HE) AS HE, SUM(D.HTrabajada) AS HTrabajada " &
            "FROM DetalleNomina D WHERE D.NumNomina = @Num", Num(numNomina))

        Dim r As DataTable = New DataTable
        r.Columns.Add("Concepto", GetType(String))
        r.Columns.Add("Monto", GetType(Double))

        If t.Rows.Count = 0 Then Return r

        Dim f As DataRow = t.Rows(0)
        Dim v As Func(Of String, Double) = Function(c) If(IsDBNull(f(c)), 0.0, Convert.ToDouble(f(c)))
        Dim devengado As Double = v("Devengado"), deducir As Double = v("TotalDeducir")

        r.Rows.Add("INGRESOS", DBNull.Value)
        For Each p As String() In {
            New String() {"Salario básico", "Salario"}, New String() {"Producción (destajo)", "Produccion"}, New String() {"Horas extra", "HorasExtra"},
            New String() {"Puntualidad", "Puntualidad"}, New String() {"Vacaciones", "Vacaciones"}, New String() {"Séptimo día", "Septimo"},
            New String() {"Incentivos de producción", "IncProd"}, New String() {"Bono de producción", "Bono"}, New String() {"Antigüedad / incentivos", "Antiguedad"},
            New String() {"Otros ingresos", "Otros"}}
            r.Rows.Add("   " & p(0), v(p(1)))
        Next
        r.Rows.Add("TOTAL DEVENGADO", devengado)

        r.Rows.Add("DEDUCCIONES", DBNull.Value)
        For Each p As String() In {
            New String() {"INSS laboral", "Inss"}, New String() {"IR", "Ir"}, New String() {"Préstamos", "Prestamo"},
            New String() {"Otras deducciones (incluye adelantos)", "Deducciones"}, New String() {"Pensión alimenticia", "Pension"}, New String() {"Embargo judicial", "Embargo"}}
            r.Rows.Add("   " & p(0), v(p(1)))
        Next
        r.Rows.Add("TOTAL DEDUCCIONES", deducir)
        r.Rows.Add("NETO A PAGAR", devengado - deducir)

        r.Rows.Add("OBLIGACIONES DEL PATRONO", DBNull.Value)
        r.Rows.Add("   INSS patronal", v("InssPatronal"))
        r.Rows.Add("   INATEC", v("Inatec"))
        r.Rows.Add("TOTAL OBLIGACIONES", v("InssPatronal") + v("Inatec"))

        r.Rows.Add("INFORMATIVO", DBNull.Value)
        r.Rows.Add("   Adelantos (ya incluidos en deducciones)", v("Adelantos"))
        r.Rows.Add("   Días de descuento", v("DiasDescuento"))
        r.Rows.Add("   Horas extra (cantidad)", v("HE"))
        r.Rows.Add("   Horas trabajadas", v("HTrabajada"))

        Return r

    End Function

    ''' <summary>Nómina sin formato para Excel (V.5.0 "Exportar Nomina sin Formato"): empleados con neto distinto de cero, por grupo y código.</summary>
    Public Function ListarSinFormato(numNomina As Integer) As DataTable
        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " & CsPlanillaNominaDAL.SqlNombres & " AS Nombres, ISNULL(C.Cargo, '') AS Cargo, ISNULL(D.TarifaHoraria, 0) AS TarifaHoraria, " &
            "ISNULL(D.HTrabajada, 0) AS HTrabajada, D.SalarioBasico, D.Destajo, D.SeptimoDia, D.HE, D.HorasExtras, D.IncetivoProduccion, D.OtrosIngresos, " &
            CsPlanillaNominaDAL.SqlDevengado & " AS TotalDevengado, D.MontoINSS, D.MontoIR, " & CsPlanillaNominaDAL.SqlDeducir & " AS TotalDeducir, " &
            CsPlanillaNominaDAL.SqlDevengado & " - " & CsPlanillaNominaDAL.SqlDeducir & " AS NetoPagar " &
            "FROM DetalleNomina D INNER JOIN Empleado E ON E.CodEmpleado = D.CodEmpleado LEFT JOIN Cargo C ON C.CodCargo = E.CodCargo " &
            "WHERE D.NumNomina = @Num AND (" & CsPlanillaNominaDAL.SqlDevengado & " - " & CsPlanillaNominaDAL.SqlDeducir & ") <> 0 ORDER BY E.CodGrupo, E.CodEmpleado1", Num(numNomina))
    End Function

End Class
