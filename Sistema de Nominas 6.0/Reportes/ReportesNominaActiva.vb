Imports DevExpress.XtraReports.UI

''' <summary>Un reporte de la lista de FrmNominaActiva.</summary>
Public Class ReporteNominaActiva
    Public Property Clave As String
    Public Property Nombre As String
    Public Property Descripcion As String
    ''' <summary>Se puede ver/imprimir (la nómina sin formato solo se exporta).</summary>
    Public Property Imprime As Boolean = True
End Class

''' <summary>
''' Reportes de la nómina activa de la V.5.0 (FrmNominaActiva) con DevExpress: reemplaza ArepListadoCheque, ArepHorasExtras, ArepNomina,
''' ArepNominaDepartamento, ArepNominaDpto, ArepNominaDptoResumen, ArepProduccion y ArepPersepciones. Cada reporte sale de una consulta de
''' <see cref="CsNominaActivaDAL"/> o <see cref="CsPlanillaNominaDAL"/> y se dibuja con <see cref="RptTabular"/> o
''' <see cref="RptColillaPago"/>; los mismos datos y columnas sirven para exportar a Excel y CSV.
''' </summary>
Public Class ReportesNominaActiva

    Private ReadOnly _numNomina As Integer
    Private ReadOnly _periodo As String
    Private ReadOnly _mostrarCeros As Boolean
    Private ReadOnly _empresa As DatosEmpresa
    Private ReadOnly _activa As New CsNominaActivaDAL
    Private ReadOnly _planilla As New CsPlanillaNominaDAL

    Public Sub New(numNomina As Integer, periodo As String, mostrarCeros As Boolean, empresa As DatosEmpresa)
        _numNomina = numNomina
        _periodo = periodo
        _mostrarCeros = mostrarCeros
        _empresa = empresa
    End Sub

    Public Shared Function Lista() As IList(Of ReporteNominaActiva)
        Return New List(Of ReporteNominaActiva) From {
            New ReporteNominaActiva With {.Clave = "Cheque", .Nombre = "Pago Cheque", .Descripcion = "Empleados que se pagan con cheque y su neto."},
            New ReporteNominaActiva With {.Clave = "HorasExtra", .Nombre = "Listado Horas Extra", .Descripcion = "Horas extra registradas para la nómina."},
            New ReporteNominaActiva With {.Clave = "NominaCodigo", .Nombre = "Nomina x Codigo Empleado", .Descripcion = "Nómina en el formato de la empresa, ordenada por código de empleado."},
            New ReporteNominaActiva With {.Clave = "NominaDepartamento", .Nombre = "Nomina x Departamento", .Descripcion = "Nómina agrupada por departamento, con subtotales."},
            New ReporteNominaActiva With {.Clave = "NominaCadaDepartamento", .Nombre = "Nomina para Cada Departamento", .Descripcion = "Una hoja por departamento."},
            New ReporteNominaActiva With {.Clave = "ColillaCadaDepartamento", .Nombre = "Colilla para Cada Departamento", .Descripcion = "Colillas de pago, cada departamento en hoja nueva."},
            New ReporteNominaActiva With {.Clave = "NominaCargo", .Nombre = "Nomina x Cargo", .Descripcion = "Nómina agrupada por cargo, con subtotales."},
            New ReporteNominaActiva With {.Clave = "NominaCargoResumen", .Nombre = "Nomina x Cargo Resumen", .Descripcion = "Una línea por cargo con cantidad de empleados y totales."},
            New ReporteNominaActiva With {.Clave = "Produccion", .Nombre = "Reporte x Produccion", .Descripcion = "Unidades producidas por empleado y proceso en la nómina."},
            New ReporteNominaActiva With {.Clave = "Percepciones", .Nombre = "Reporte Percepsiones", .Descripcion = "Totales de ingresos, deducciones, neto y obligaciones del patrono."},
            New ReporteNominaActiva With {.Clave = "SinFormato", .Nombre = "Exportar Nomina sin Formato", .Imprime = False,
                                          .Descripcion = "Solo exporta: nómina con las columnas de la planilla de Excel de la V.5.0."}}
    End Function

    Private Function FormatoNomina() As String
        Return CatalogoReportesNomina.ClaveDe(CatalogoReportesNomina.FormatosNomina(), _empresa.Formato_Nomina)
    End Function

    Private Function FormatoColilla() As String
        Return CatalogoReportesNomina.ClaveDe(CatalogoReportesNomina.FormatosColilla(), _empresa.Formato_Colilla)
    End Function

    ''' <summary>Formato de nómina que usa el reporte (cuando es uno de los de planilla); vacío para los demás.</summary>
    Private Function ClaveNomina(clave As String) As String
        Select Case clave
            Case "NominaCodigo" : Return FormatoNomina()
            Case "NominaDepartamento", "NominaCadaDepartamento" : Return "NominaDepartamento"
            Case "NominaCargo" : Return "NominaDpto"
            Case Else : Return ""
        End Select
    End Function

    Public Function Datos(clave As String) As DataTable

        Select Case clave
            Case "Cheque" : Return _activa.ListarCheques(_numNomina)
            Case "HorasExtra" : Return _activa.ListarHorasExtra(_numNomina)
            Case "Produccion" : Return _activa.ListarProduccion(_numNomina)
            Case "NominaCargoResumen" : Return _activa.ListarResumenPorCargo(_numNomina)
            Case "Percepciones" : Return _activa.ListarPercepciones(_numNomina)
            Case "SinFormato" : Return _activa.ListarSinFormato(_numNomina)
            Case "NominaCodigo" : Return _planilla.ListarParaReporte(_numNomina, "", True)
            Case "NominaDepartamento", "NominaCadaDepartamento" : Return _planilla.ListarParaReporte(_numNomina, "Departamento", _mostrarCeros)
            Case "ColillaCadaDepartamento" : Return _planilla.ListarParaReporte(_numNomina, "Departamento", _mostrarCeros)
            Case "NominaCargo" : Return _planilla.ListarParaReporte(_numNomina, "Cargo", False)
            Case Else : Throw New ArgumentException("Reporte desconocido: " & clave)
        End Select

    End Function

    ''' <summary>Columnas del reporte: se usan para dibujarlo y para titular las exportaciones.</summary>
    Public Function Columnas(clave As String) As IList(Of ColumnaReporte)

        Select Case clave
            Case "Cheque"
                Return New List(Of ColumnaReporte) From {
                    New ColumnaReporte("Código", "Codigo", 60), New ColumnaReporte("Nombre", "Nombres", 280), New ColumnaReporte("Cédula", "NumCedula", 120),
                    New ColumnaReporte("Departamento", "Departamento", 220), New ColumnaReporte("Neto a pagar", "NetoPagar", 120, "#,##0.00", True)}

            Case "HorasExtra"
                Return New List(Of ColumnaReporte) From {
                    New ColumnaReporte("Código", "Codigo", 70), New ColumnaReporte("Nombre", "Nombres", 380),
                    New ColumnaReporte("Cantidad de horas", "CantHoras", 120, "#,##0.00", True), New ColumnaReporte("Nómina", "NumNomina", 70, "0")}

            Case "Produccion"
                Return New List(Of ColumnaReporte) From {
                    New ColumnaReporte("Código", "Codigo", 45), New ColumnaReporte("Nombre", "Nombres", 150), New ColumnaReporte("Referencia", "CodReferencia", 70),
                    New ColumnaReporte("Proceso", "CodProceso", 60), New ColumnaReporte("Ref.", "Ref", 50), New ColumnaReporte("Lun", "Lunes", 40, "#,##0", True),
                    New ColumnaReporte("Mar", "Martes", 40, "#,##0", True), New ColumnaReporte("Mié", "Miercoles", 40, "#,##0", True), New ColumnaReporte("Jue", "Jueves", 40, "#,##0", True),
                    New ColumnaReporte("Vie", "Viernes", 40, "#,##0", True), New ColumnaReporte("Sáb", "Sabado", 40, "#,##0", True), New ColumnaReporte("Dom", "Domingo", 40, "#,##0", True),
                    New ColumnaReporte("Total unidades", "TotalUnidades", 70, "#,##0", True), New ColumnaReporte("Precio", "Precio", 60, "#,##0.0000"),
                    New ColumnaReporte("Salario pieza", "SalarioPieza", 80, "#,##0.00", True), New ColumnaReporte("Unidad", "Unidad", 50)}

            Case "NominaCargoResumen"
                Return New List(Of ColumnaReporte) From {
                    New ColumnaReporte("Cargo", "Cargo", 150), New ColumnaReporte("Empleados", "Empleados", 55, "#,##0", True),
                    New ColumnaReporte("Salario básico", "SalarioBasico", 75, "#,##0.00", True), New ColumnaReporte("Destajo", "Destajo", 65, "#,##0.00", True),
                    New ColumnaReporte("Séptimo día", "SeptimoDia", 65, "#,##0.00", True), New ColumnaReporte("Horas extras", "HorasExtras", 65, "#,##0.00", True),
                    New ColumnaReporte("Incent. prod.", "IncetivoProduccion", 65, "#,##0.00", True), New ColumnaReporte("Otros ingresos", "OtrosIngresos", 65, "#,##0.00", True),
                    New ColumnaReporte("Total devengado", "TotalDevengado", 80, "#,##0.00", True), New ColumnaReporte("INSS", "MontoINSS", 60, "#,##0.00", True),
                    New ColumnaReporte("IR", "MontoIR", 60, "#,##0.00", True), New ColumnaReporte("Total deduc.", "TotalDeducir", 70, "#,##0.00", True),
                    New ColumnaReporte("Neto a pagar", "NetoPagar", 80, "#,##0.00", True)}

            Case "Percepciones"
                Return New List(Of ColumnaReporte) From {New ColumnaReporte("Concepto", "Concepto", 380), New ColumnaReporte("Monto", "Monto", 130, "#,##0.00")}

            Case "SinFormato"
                Return New List(Of ColumnaReporte) From {
                    New ColumnaReporte("Código", "Codigo", 50), New ColumnaReporte("Nombre", "Nombres", 160), New ColumnaReporte("Cargo", "Cargo", 100),
                    New ColumnaReporte("Tarifa horaria", "TarifaHoraria", 60, "#,##0.00"), New ColumnaReporte("Horas trabajadas", "HTrabajada", 60, "#,##0.00"),
                    New ColumnaReporte("Salario básico", "SalarioBasico", 70, "#,##0.00"), New ColumnaReporte("Monto destajo", "Destajo", 70, "#,##0.00"),
                    New ColumnaReporte("Séptimo día", "SeptimoDia", 70, "#,##0.00"), New ColumnaReporte("Num horas", "HE", 50, "#,##0.00"),
                    New ColumnaReporte("Horas extra", "HorasExtras", 70, "#,##0.00"), New ColumnaReporte("Incentivo producción", "IncetivoProduccion", 70, "#,##0.00"),
                    New ColumnaReporte("Otros ingresos", "OtrosIngresos", 70, "#,##0.00"), New ColumnaReporte("Total devengado", "TotalDevengado", 80, "#,##0.00"),
                    New ColumnaReporte("INSS", "MontoINSS", 60, "#,##0.00"), New ColumnaReporte("IR", "MontoIR", 60, "#,##0.00"),
                    New ColumnaReporte("Total deducciones", "TotalDeducir", 80, "#,##0.00"), New ColumnaReporte("Neto a pagar", "NetoPagar", 80, "#,##0.00")}

            Case "ColillaCadaDepartamento"
                Return New List(Of ColumnaReporte)

            Case Else
                Return CatalogoReportesNomina.ColumnasNomina(ClaveNomina(clave))
        End Select

    End Function

    Public Function Construir(clave As String, datos As DataTable) As XtraReport

        Dim nombre As String = Lista().First(Function(r) r.Clave = clave).Nombre

        Select Case clave
            Case "ColillaCadaDepartamento"
                Return CatalogoReportesNomina.Colilla(FormatoColilla(), _empresa, "Colilla de Pago", _periodo, datos, True)

            Case "NominaCodigo"
                Return CatalogoReportesNomina.Nomina(FormatoNomina(), _empresa, "Nómina", _periodo, datos, False)

            Case "NominaDepartamento", "NominaCargo"
                Return CatalogoReportesNomina.Nomina(ClaveNomina(clave), _empresa, "Nómina", _periodo, datos, True)

            Case "NominaCadaDepartamento"
                Return CatalogoReportesNomina.Nomina("NominaDepartamento", _empresa, "Nómina", _periodo, datos, True, True)

            Case "Produccion"
                Return New RptTabular(_empresa, "Reporte de Producción", _periodo, datos, Columnas(clave), "Codigo", "Empleado")

            Case "NominaCargoResumen"
                Return New RptTabular(_empresa, "Nómina por Cargo - Resumen", _periodo, datos, Columnas(clave))

            Case Else
                Return New RptTabular(_empresa, nombre.Replace("Percepsiones", "Percepciones"), _periodo, datos, Columnas(clave))
        End Select

    End Function

    ''' <summary>Copia de los datos con los títulos de las columnas del reporte como encabezados, lista para Excel.</summary>
    Public Function TablaExportable(clave As String, datos As DataTable) As DataTable

        Dim cols As IList(Of ColumnaReporte) = Columnas(clave).Where(Function(c) datos.Columns.Contains(c.Campo)).ToList()
        Dim t As New DataTable

        For Each c As ColumnaReporte In cols
            t.Columns.Add(c.Titulo, datos.Columns(c.Campo).DataType)
        Next

        For Each f As DataRow In datos.Rows
            t.Rows.Add(cols.Select(Function(c) f(c.Campo)).ToArray())
        Next

        Return t

    End Function

    ''' <summary>Pares encabezado/campo para <see cref="ExportadorCsv"/>.</summary>
    Public Function ColumnasCsv(clave As String, datos As DataTable) As IList(Of KeyValuePair(Of String, String))
        Return Columnas(clave).Where(Function(c) datos.Columns.Contains(c.Campo)).
            Select(Function(c) New KeyValuePair(Of String, String)(c.Titulo, c.Campo)).ToList()
    End Function

End Class
