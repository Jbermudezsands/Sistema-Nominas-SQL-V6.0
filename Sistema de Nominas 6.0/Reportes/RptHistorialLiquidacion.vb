Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

''' <summary>
''' Encabezado común de los reportes de salarios de una baja: bloque con el código y el nombre completo del empleado (siempre visibles),
''' departamento, cargo, tiempo trabajado y los salarios de referencia, debajo del encabezado de empresa de <see cref="RptTabular"/>.
''' </summary>
Public MustInherit Class RptSalariosBaja
    Inherits RptTabular

    Protected Sub New(empresa As DatosEmpresa, titulo As String, datos As DatosHistorialBaja, tabla As DataTable, columnas As IList(Of ColumnaReporte))

        MyBase.New(empresa, titulo, "", tabla, columnas)

        DisplayName = titulo & " " & datos.Codigo

        Dim encabezado As ReportHeaderBand = Bands.OfType(Of ReportHeaderBand)().First()
        Dim ancho As Single = columnas.Sum(Function(c) c.Ancho)
        Dim mitad As Single = ancho / 2
        Dim y As Single = encabezado.HeightF

        ' Código y nombre en grande: el nombre completo del empleado nunca se omite.
        encabezado.Controls.Add(ConstructorReporte.Etiqueta("Empleado:", 0, y, 85, 18, True, 10))
        encabezado.Controls.Add(ConstructorReporte.Etiqueta(datos.Codigo & "  -  " & datos.Nombre_Completo, 85, y, ancho - 85, 18, True, 10))
        y += 20

        Dim filas As String()() = {
            New String() {"Departamento:", datos.Departamento, "Cargo:", datos.Cargo},
            New String() {"Años trabajados:", datos.Annos, "Meses / Días:", datos.Meses & "  /  " & datos.Dias},
            New String() {"Salario más alto:", N(datos.Salario_Alto), "Salario promedio:", N(datos.Salario_Promedio)},
            New String() {"Salario básico:", N(datos.Salario_Basico), "Tarifa horaria:", N(datos.Tarifa)},
            New String() {"Inicio vacaciones:", datos.Fecha_Ini_Vacaciones.ToString("dd/MM/yyyy"), "Inicio aguinaldo:", datos.Fecha_Ini_Aguinaldo.ToString("dd/MM/yyyy")}}

        For Each f As String() In filas
            encabezado.Controls.Add(ConstructorReporte.Etiqueta(f(0), 0, y, 110, 14, True))
            encabezado.Controls.Add(ConstructorReporte.Etiqueta(f(1), 110, y, mitad - 115, 14))
            encabezado.Controls.Add(ConstructorReporte.Etiqueta(f(2), mitad, y, 110, 14, True))
            encabezado.Controls.Add(ConstructorReporte.Etiqueta(f(3), mitad + 110, y, mitad - 110, 14))
            y += 14
        Next

        encabezado.HeightF = y + 6

    End Sub

    Private Shared Function N(valor As Double) As String
        Return valor.ToString("#,##0.00")
    End Function

End Class

''' <summary>
''' Historial de salarios de la liquidación (V.5.0 ArepHistorialLiquida / ArepHistorialLiquidaBono / ArepHistorialLiquidaBono2):
''' ingresos por mes de los últimos 6 meses con nómina, con total general.
''' </summary>
Public Class RptHistorialLiquidacion
    Inherits RptSalariosBaja

    Public Sub New(empresa As DatosEmpresa, datos As DatosHistorialBaja, salarios As IList(Of SalarioMesBaja), formatoBono As Boolean)
        MyBase.New(empresa, "HISTORIAL DE SALARIOS", datos, Preparar(salarios), Columnas(formatoBono))
    End Sub

    Private Shared Function Preparar(salarios As IList(Of SalarioMesBaja)) As DataTable

        Dim t As New DataTable
        t.Columns.Add("Periodo", GetType(String))
        t.Columns.Add("FechaInicio", GetType(Date))
        t.Columns.Add("FechaFin", GetType(Date))
        For Each c As String In {"SalarioBasico", "Produccion", "Septimo", "Otros", "Incentivos", "Reembolso", "HorasExtras", "TotalIngresos"}
            t.Columns.Add(c, GetType(Double))
        Next

        For Each s As SalarioMesBaja In salarios
            t.Rows.Add(s.Periodo, s.Fecha_Inicio, s.Fecha_Fin, s.Salario_Basico, s.Destajo + s.Bono_Produccion, s.Septimo, s.Otros,
                       s.Incentivos, s.Reembolso, s.Horas_Extras, s.Total_Ingresos)
        Next

        Return t

    End Function

    Private Shared Function Columnas(formatoBono As Boolean) As IList(Of ColumnaReporte)

        Dim lista As New List(Of ColumnaReporte) From {
            New ColumnaReporte("Mes", "Periodo", 70),
            New ColumnaReporte("Fecha Inicio", "FechaInicio", 80, "dd/MM/yyyy"),
            New ColumnaReporte("Fecha Fin", "FechaFin", 80, "dd/MM/yyyy"),
            New ColumnaReporte("Salario Básico", "SalarioBasico", 95, "#,##0.00", True),
            New ColumnaReporte("Séptimo", "Septimo", 85, "#,##0.00", True),
            New ColumnaReporte(If(formatoBono, "Producción", "Destajo"), "Produccion", 90, "#,##0.00", True)}

        If formatoBono Then lista.Add(New ColumnaReporte("Horas Extra", "HorasExtras", 85, "#,##0.00", True))

        lista.AddRange({
            New ColumnaReporte("Reembolso", "Reembolso", 85, "#,##0.00", True),
            New ColumnaReporte("Incentivos", "Incentivos", 90, "#,##0.00", True),
            New ColumnaReporte("Otros", "Otros", 85, "#,##0.00", True),
            New ColumnaReporte("Total Ingresos", "TotalIngresos", 105, "#,##0.00", True)})

        Return lista

    End Function

End Class

''' <summary>
''' Detalle de salarios de la liquidación (V.5.0 ArepDetalleLiquida / ArepDetalleLiquidaBono / ArepDetalleLiquidaBono2):
''' una fila por nómina del mismo período del historial, con INSS e IR retenidos y total general.
''' </summary>
Public Class RptDetalleLiquidacion
    Inherits RptSalariosBaja

    ''' <param name="detalle">Tabla de <see cref="CsBajasDAL.ListarDetalleNominas"/>.</param>
    Public Sub New(empresa As DatosEmpresa, datos As DatosHistorialBaja, detalle As DataTable, formatoBono As Boolean)
        MyBase.New(empresa, "DETALLE DE SALARIOS", datos, detalle, Columnas(formatoBono))
    End Sub

    Private Shared Function Columnas(formatoBono As Boolean) As IList(Of ColumnaReporte)

        Dim lista As New List(Of ColumnaReporte) From {
            New ColumnaReporte("Fecha Inicio", "FechaInicio", 75, "dd/MM/yyyy"),
            New ColumnaReporte("Fecha Fin", "FechaFin", 75, "dd/MM/yyyy"),
            New ColumnaReporte("Salario Básico", "SalarioBasico", 90, "#,##0.00", True),
            New ColumnaReporte("Séptimo", "Septimo", 80, "#,##0.00", True),
            New ColumnaReporte(If(formatoBono, "Bono Producción", "Destajo"), If(formatoBono, "BonoProduccion", "Destajo"), 90, "#,##0.00", True)}

        If formatoBono Then lista.Add(New ColumnaReporte("Horas Extra", "HorasExtras", 80, "#,##0.00", True))

        lista.AddRange({
            New ColumnaReporte("Incentivos", "Incentivos", 85, "#,##0.00", True),
            New ColumnaReporte("Otros", "Otros", 80, "#,##0.00", True),
            New ColumnaReporte("INSS", "MontoINSS", 75, "#,##0.00", True),
            New ColumnaReporte("IR", "MontoIR", 75, "#,##0.00", True),
            New ColumnaReporte("Total Ingresos", "TotalIngresos", 100, "#,##0.00", True)})

        Return lista

    End Function

End Class
