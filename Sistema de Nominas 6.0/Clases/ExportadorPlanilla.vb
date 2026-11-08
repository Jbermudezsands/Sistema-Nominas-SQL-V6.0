Imports System.Data
Imports System.IO
Imports DevExpress.Export.Xl

''' <summary>
''' Planilla de nómina a Excel con el formato de la V.5.0 (FrmCalcularNomina.PushButton2): título y rango de fechas, una columna por cada
''' tipo de incentivo y de deducción de la nómina (con su código sobre el encabezado), las deducciones en negativo, y al final las
''' obligaciones del patrono. Sin Excel instalado: escribe el .xlsx directamente.
''' </summary>
Public Class ExportadorPlanilla

    Private Const FormatoMonto As String = "#,##0.00"

    Private ReadOnly _dal As New CsPlanillaNominaDAL

    ''' <summary>Genera el archivo <paramref name="ruta"/> con la planilla de la nómina.</summary>
    Public Sub Exportar(ruta As String, numNomina As Integer, titulo As String, desde As Date, hasta As Date, mostrarCeros As Boolean)

        Dim empleados As DataTable = _dal.ListarParaExcel(numNomina, mostrarCeros)
        Dim tiposIncentivo As DataTable = _dal.ListarTiposIncentivoNomina(numNomina)
        Dim tiposDeduccion As DataTable = _dal.ListarTiposDeduccionNomina(numNomina)

        Dim incentivos As Dictionary(Of String, Double) = Indexar(_dal.ListarMontosIncentivoNomina(numNomina))
        Dim deducciones As Dictionary(Of String, Double) = Indexar(_dal.ListarMontosDeduccionNomina(numNomina))

        Dim exportador As IXlExporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx)

        Using flujo As New FileStream(ruta, FileMode.Create, FileAccess.Write)
            Using documento As IXlDocument = exportador.CreateDocument(flujo)
                Using hoja As IXlSheet = documento.CreateSheet()

                    hoja.Name = "Nomina"
                    CrearColumnas(hoja, 11 + tiposIncentivo.Rows.Count + 7 + tiposDeduccion.Rows.Count + 8)

                    Dim negrita As XlCellFormatting = Formato(True, 11)
                    Dim monto As XlCellFormatting = New XlCellFormatting With {.NumberFormat = FormatoMonto}
                    Dim montoNegrita As XlCellFormatting = Formato(True, 11)
                    montoNegrita.NumberFormat = FormatoMonto

                    ' Fila 1: título; fila 2: rango de fechas; fila 3: códigos de incentivo/deducción; fila 4: encabezados.
                    ' Como la V.5.0: título combinado A1:J1 y centrado; fechas en formato largo ("martes, 16 de diciembre de 2025").
                    Dim formatoTitulo As XlCellFormatting = Formato(True, 20)
                    formatoTitulo.Alignment = XlCellAlignment.FromHV(XlHorizontalAlignment.Center, XlVerticalAlignment.Center)
                    Escribir(hoja, {titulo}, formatoTitulo)
                    hoja.MergedCells.Add(XlCellRange.FromLTRB(0, 0, 9, 0))
                    Dim es As New Globalization.CultureInfo("es-ES")
                    Dim largo As String = "dddd, d 'de' MMMM 'de' yyyy"
                    Escribir(hoja, {"EXPORTACION DESDE " & desde.ToString(largo, es) & " HASTA " & hasta.ToString(largo, es)}, Formato(True, 14))

                    Dim codigos As New List(Of Object)
                    Dim encabezados As New List(Of Object) From {"Area/Dpto", "CodEmpleado", "Nombre y Apellido", "NumerCedula", "NumerInss", "Salario Basico",
                                                                  "Subsidio", "Horas Extra", "Sub.Alimentacion", "Comisiones", "Vacaciones Pag"}
                    For i As Integer = 1 To 11
                        codigos.Add(Nothing)
                    Next

                    For Each t As DataRow In tiposIncentivo.Rows
                        codigos.Add(CStr(t("Codigo")))
                        encabezados.Add(CStr(t("Nombre")))
                    Next

                    For Each nombre As String In {"Otros Ingresos", "Total Devengado", "Monto Inss", "Monto Ir", "Prestamos", "Pension Alimenticia", "Embargo judicial"}
                        codigos.Add(Nothing)
                        encabezados.Add(nombre)
                    Next

                    For Each t As DataRow In tiposDeduccion.Rows
                        codigos.Add(CStr(t("Codigo")))
                        encabezados.Add(CStr(t("Nombre")))
                    Next

                    For Each nombre As String In {"Total Deducciones", "Neto Pagar", "INATEC", "INSS PATRONAL", "PROV 13VO", "PROV VACACIONES", "INDEMNIZACION", "TOTAL PASIVO"}
                        codigos.Add(Nothing)
                        encabezados.Add(nombre)
                    Next

                    Escribir(hoja, codigos.ToArray(), negrita)
                    Escribir(hoja, encabezados.ToArray(), negrita)

                    For Each e As DataRow In empleados.Rows

                        Dim fila As New List(Of Object) From {
                            e("Departamento"), e("CodEmpleado1"), e("Nombres"), e("NumCedula"), e("NumeroInss"),
                            Numero(e("SalarioBasico")), Numero(e("TotalSubsidio")), Numero(e("HorasExtras")), Numero(e("Viaticos")),
                            Numero(e("Comisiones")), Numero(e("VacacionesPagadas"))}

                        Dim cod As String = Convert.ToString(e("CodEmpleado"), Globalization.CultureInfo.InvariantCulture)

                        For Each t As DataRow In tiposIncentivo.Rows
                            Dim v As Double
                            fila.Add(If(incentivos.TryGetValue(cod & "|" & CStr(t("Codigo")), v), CObj(v), Nothing))
                        Next

                        fila.Add(Numero(e("OtrosIngresos")))
                        fila.Add(Numero(e("TotalDevengado")))
                        fila.Add(-Numero(e("MontoINSS")))
                        fila.Add(-Numero(e("MontoIR")))
                        fila.Add(-Numero(e("Prestamo")))
                        fila.Add(-Numero(e("MontoPension")))
                        fila.Add(-Numero(e("EmbargoJudicial")))

                        ' Las deducciones van en negativo, como en la V.5.0.
                        For Each t As DataRow In tiposDeduccion.Rows
                            Dim v As Double
                            fila.Add(If(deducciones.TryGetValue(cod & "|" & CStr(t("Codigo")), v), CObj(-v), Nothing))
                        Next

                        Dim inatec As Double = Numero(e("INATEC"))
                        Dim mes13 As Double = Numero(e("Mes13"))
                        Dim patronal As Double = Numero(e("INSSPatronal"))
                        Dim indemnizacion As Double = Numero(e("ProvIndemnizacion"))

                        fila.Add(-Numero(e("TotalDeducir")))
                        fila.Add(Numero(e("NetoPagar")))
                        fila.Add(inatec)
                        fila.Add(patronal)
                        fila.Add(mes13)
                        fila.Add(mes13) ' PROV VACACIONES: la V.5.0 exportaba el mismo monto del 13vo.
                        fila.Add(indemnizacion)
                        fila.Add(Math.Round(inatec + mes13 + mes13 + patronal + indemnizacion, 2))

                        Escribir(hoja, fila.ToArray(), monto, False)

                    Next

                End Using
            End Using
        End Using

    End Sub

#Region "AYUDAS"

    Private Shared Sub CrearColumnas(hoja As IXlSheet, cantidad As Integer)
        For i As Integer = 0 To cantidad - 1
            Using columna As IXlColumn = hoja.CreateColumn()
                columna.WidthInCharacters = If(i = 2, 45, If(i = 3, 17, If(i = 0, 22, 15)))
            End Using
        Next
    End Sub

    Private Shared Function Formato(negrita As Boolean, tamano As Double) As XlCellFormatting
        Dim f As New XlCellFormatting
        f.Font = New XlFont With {.Bold = negrita, .Size = tamano}
        Return f
    End Function

    ''' <summary>Una fila: los números llevan el formato de monto; el texto y los vacíos no.</summary>
    Private Shared Sub Escribir(hoja As IXlSheet, valores As Object(), formato As XlCellFormatting, Optional aplicarATexto As Boolean = True)

        Using fila As IXlRow = hoja.CreateRow()
            For Each v As Object In valores
                Using celda As IXlCell = fila.CreateCell()
                    If v Is Nothing OrElse IsDBNull(v) Then
                        ' celda vacía
                    ElseIf TypeOf v Is Double Then
                        celda.Value = XlVariantValue.FromObject(v)
                        celda.ApplyFormatting(formato)
                    Else
                        celda.Value = XlVariantValue.FromObject(Convert.ToString(v))
                        If aplicarATexto Then celda.ApplyFormatting(formato)
                    End If
                End Using
            Next
        End Using

    End Sub

    Private Shared Function Numero(valor As Object) As Double
        Return If(IsDBNull(valor), 0.0, Math.Round(Convert.ToDouble(valor), 2))
    End Function

    ''' <summary>Montos por "empleado|código".</summary>
    Private Shared Function Indexar(tabla As DataTable) As Dictionary(Of String, Double)

        Dim d As New Dictionary(Of String, Double)

        For Each r As DataRow In tabla.Rows
            d(Convert.ToString(r("CodEmpleado"), Globalization.CultureInfo.InvariantCulture) & "|" & CStr(r("Codigo"))) = If(IsDBNull(r("Valor")), 0.0, Convert.ToDouble(r("Valor")))
        Next

        Return d

    End Function

#End Region

End Class
