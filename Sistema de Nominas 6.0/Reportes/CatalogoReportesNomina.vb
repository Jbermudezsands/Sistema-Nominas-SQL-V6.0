''' <summary>
''' Formatos de colilla y de nómina que la V.5.0 resolvía con ActiveReports (ArepColillas*, ArepNomina*): cada formato es el mismo
''' diseño con otros conceptos o con otra agrupación, así que se definen aquí como datos sobre <see cref="RptColillaPago"/> y
''' <see cref="RptTabular"/>. La tabla debe venir de <see cref="CsPlanillaNominaDAL.ListarParaReporte"/>.
''' </summary>
Public Class CatalogoReportesNomina

    Public Class Formato
        Public Property Clave As String
        Public Property Nombre As String
        ''' <summary>Valor de DatosEmpresa.Formato_Colilla / Formato_Nomina de la V.5.0 que corresponde a este formato.</summary>
        Public Property ValorV5 As String
    End Class

    Private Const AnchoNomina As Single = 1020

    Public Shared Function FormatosColilla() As IList(Of Formato)
        Return New List(Of Formato) From {
            New Formato With {.Clave = "Colilla", .Nombre = "Producción (ArepColillas)", .ValorV5 = "Colilla Produccion"},
            New Formato With {.Clave = "ColillasBono", .Nombre = "Bono de producción (ArepColillasBono)", .ValorV5 = "Colilla Bono Produccion"},
            New Formato With {.Clave = "ColillasPago", .Nombre = "Comercial (ArepColillasPago)", .ValorV5 = "Colilla Comercial"},
            New Formato With {.Clave = "ColillasPago2", .Nombre = "Comercial 2 (ArepColillasPago2)", .ValorV5 = "Colilla Comercial2"},
            New Formato With {.Clave = "ColillasPago4", .Nombre = "Trides (ArepColillasPago4)", .ValorV5 = "Colilla Trides"}}
    End Function

    Public Shared Function FormatosNomina() As IList(Of Formato)
        Return New List(Of Formato) From {
            New Formato With {.Clave = "Nomina", .Nombre = "Producción, por grupo (ArepNomina)", .ValorV5 = "Nomina Produccion"},
            New Formato With {.Clave = "NominaComercial", .Nombre = "Comercial (ArepNominaComercial)", .ValorV5 = "Nomina Comercial"},
            New Formato With {.Clave = "NominaComercial2", .Nombre = "Comercial 2 (ArepNominaComercial2)", .ValorV5 = "Nomina Comercial2"},
            New Formato With {.Clave = "NominaDepartamento", .Nombre = "Por departamento (ArepNominaDepartamento)", .ValorV5 = ""},
            New Formato With {.Clave = "NominaDpto", .Nombre = "Por cargo (ArepNominaDpto)", .ValorV5 = ""}}
    End Function

    ''' <summary>Clave del formato que corresponde al valor guardado en la V.5.0; si no hay equivalente, la primera de la lista.</summary>
    Public Shared Function ClaveDe(formatos As IList(Of Formato), valorV5 As String) As String
        Dim f As Formato = formatos.FirstOrDefault(Function(x) x.ValorV5 <> "" AndAlso String.Equals(x.ValorV5, valorV5, StringComparison.OrdinalIgnoreCase))
        Return If(f Is Nothing, formatos(0).Clave, f.Clave)
    End Function

    ''' <summary>Campo por el que debe venir ordenada la tabla para el formato de nómina (Grupo, Departamento o Cargo).</summary>
    Public Shared Function OrdenNomina(clave As String) As String
        Select Case clave
            Case "NominaDepartamento" : Return "Departamento"
            Case "NominaDpto" : Return "Cargo"
            Case Else : Return "Grupo"
        End Select
    End Function

    Public Shared Function Colilla(clave As String, empresa As DatosEmpresa, titulo As String, periodo As String, tabla As DataTable,
                                   porDepartamento As Boolean) As DevExpress.XtraReports.UI.XtraReport

        ' Trides (ArepColillasPago4) tiene su propio diseño con el sub-reporte de deducciones.
        If clave = "ColillasPago4" Then
            Dim numNomina As Integer = If(tabla.Rows.Count > 0, Convert.ToInt32(tabla.Rows(0)("NumNomina")), 0)
            Return New RptColillaDetalle(empresa, titulo, periodo, tabla, New CsPlanillaNominaDAL().ListarDetalleDeducciones(numNomina), porDepartamento)
        End If

        Dim ingresos As New List(Of ConceptoColilla)
        Dim salario As New ConceptoColilla("Salario básico", "SalarioBasico")

        Select Case clave
            Case "ColillasBono"
                ingresos.AddRange({salario, New ConceptoColilla("Bono producción", "BonoProduccion"), New ConceptoColilla("Horas extras", "HorasExtras"),
                                   New ConceptoColilla("Puntualidad", "Comisiones"), New ConceptoColilla("Antigüedad", "Antiguedad"),
                                   New ConceptoColilla("Otros ingresos", "OtrosIngresos")})
            Case "ColillasPago"
                ingresos.AddRange({salario, New ConceptoColilla("Destajo", "Destajo"), New ConceptoColilla("Horas extras", "HorasExtras"),
                                   New ConceptoColilla("Viáticos", "Viaticos"), New ConceptoColilla("Incentivos", "Incentivos"),
                                   New ConceptoColilla("Comisiones", "Comisiones"), New ConceptoColilla("Otros ingresos", "OtrosIngresos"),
                                   New ConceptoColilla("Vacaciones", "VacacionesPagadas")})
            Case "ColillasPago2"
                ingresos.AddRange({salario, New ConceptoColilla("Destajo", "Destajo"), New ConceptoColilla("Horas extras", "HorasExtras"),
                                   New ConceptoColilla("Comisiones", "Comisiones"), New ConceptoColilla("Incentivos", "Incentivos"),
                                   New ConceptoColilla("Otros ingresos", "OtrosIngresos"), New ConceptoColilla("Vacaciones", "VacacionesPagadas")})
            Case Else
                ingresos.AddRange({salario, New ConceptoColilla("Destajo", "Destajo"), New ConceptoColilla("Horas extras", "HorasExtras"),
                                   New ConceptoColilla("Puntualidad", "Comisiones"), New ConceptoColilla("Incentivos", "Incentivos"),
                                   New ConceptoColilla("Séptimo día", "SeptimoDia"), New ConceptoColilla("Incentivo producción", "IncetivoProduccion"),
                                   New ConceptoColilla("Otros ingresos", "OtrosIngresos")})
        End Select

        Dim deducciones As New List(Of ConceptoColilla) From {
            New ConceptoColilla("INSS", "MontoINSS"), New ConceptoColilla("IR", "MontoIR"), New ConceptoColilla("Préstamos", "Prestamo"),
            New ConceptoColilla("Otras deducciones", "Deducciones"), New ConceptoColilla("Pensión alimenticia", "MontoPension"),
            New ConceptoColilla("Embargo judicial", "EmbargoJudicial")}

        Return New RptColillaPago(empresa, titulo, periodo, tabla, ingresos, deducciones, porDepartamento)

    End Function

    ''' <summary>Columnas del formato de nómina con sus anchos relativos (sin ajustar a la hoja).</summary>
    Public Shared Function ColumnasNomina(clave As String) As List(Of ColumnaReporte)

        Dim c As New List(Of ColumnaReporte) From {
            New ColumnaReporte("Código", "Codigo", 42), New ColumnaReporte("Nombre", "Nombres", 170), Num("Salario básico", "SalarioBasico", 64)}

        Select Case clave
            Case "NominaComercial"
                c.AddRange({Num("Vacaciones", "VacacionesPagadas"), Num("Incentivos", "Incentivos"), Num("Horas extras", "HorasExtras"),
                            Num("Comisiones", "Comisiones"), Num("Subsidio", "TotalSubsidio"), Num("Otros ingresos", "OtrosIngresos")})
            Case "NominaComercial2"
                c.AddRange({Num("Comisiones", "Comisiones"), Num("Incentivos", "Incentivos"), Num("Antig.", "Antiguedad"),
                            Num("Horas extras", "HorasExtras"), Num("Subsidio", "TotalSubsidio"), Num("Otros ingresos", "OtrosIngresos")})
            Case "NominaDepartamento", "NominaDpto"
                c.AddRange({Num("Puntual.", "Comisiones"), Num("Antig.", "Antiguedad"), Num("Horas extras", "HorasExtras"),
                            Num("Séptimo día", "SeptimoDia"), Num("Destajo", "Destajo"), Num("Incent. prod.", "IncetivoProduccion"),
                            Num("Otros ingresos", "OtrosIngresos")})
            Case Else
                c.AddRange({Num("Destajo", "Destajo"), Num("Séptimo día", "SeptimoDia"), Num("Incent. prod.", "IncetivoProduccion"),
                            Num("Horas extras", "HorasExtras"), Num("Otros ingresos", "OtrosIngresos"), Num("Vacaciones", "VacacionesPagadas")})
        End Select

        c.AddRange({Num("Total devengado", "TotalDevengado", 68), Num("INSS", "MontoINSS"), Num("Prést.", "Prestamo"),
                    Num("Deduc.", "Deducciones"), Num("Total deduc.", "TotalDeducir", 64), Num("Neto a pagar", "NetoPagar", 68)})

        Return c

    End Function

    Public Shared Function Nomina(clave As String, empresa As DatosEmpresa, titulo As String, periodo As String, tabla As DataTable,
                                   Optional agrupar As Boolean = True, Optional paginaPorGrupo As Boolean = False) As RptTabular

        Dim c As List(Of ColumnaReporte) = ColumnasNomina(clave)

        ' Cada formato suma un ancho distinto: se reparte para ocupar siempre la hoja horizontal.
        Dim suma As Single = c.Sum(Function(x) x.Ancho)
        For Each col As ColumnaReporte In c
            col.Ancho = col.Ancho * AnchoNomina / suma
        Next

        Dim campoGrupo As String = If(agrupar, OrdenNomina(clave), "")

        Return New RptTabular(empresa, titulo, periodo, tabla, c, campoGrupo, campoGrupo, paginaPorGrupo)

    End Function

    Private Shared Function Num(titulo As String, campo As String, Optional ancho As Single = 56) As ColumnaReporte
        Return New ColumnaReporte(titulo, campo, ancho, "#,##0.00", True)
    End Function

End Class
