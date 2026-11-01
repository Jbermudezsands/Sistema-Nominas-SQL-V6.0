''' <summary>
''' Historial salarial del aguinaldo o de las vacaciones (V.5.0 ArepHistorialSalarial13vo de ActiveReports 2.0): código,
''' nombre y el salario de cada mes que tomó el cálculo. Es un <see cref="RptTabular"/>; solo se imprimen los meses con datos.
''' </summary>
Public NotInheritable Class RptHistorialSalarial

    Private Sub New()
    End Sub

    ''' <param name="tipo">"Aguinaldo" o "Vacaciones".</param>
    ''' <param name="historial">Resultado de <see cref="CsHistorialSalarialDAL.Listar"/>.</param>
    Public Shared Function Crear(empresa As DatosEmpresa, tipo As String, numNomina As Integer, historial As DataTable) As RptTabular

        Dim columnas As New List(Of ColumnaReporte) From {
            New ColumnaReporte("Código", "Codigo", 70),
            New ColumnaReporte("Nombre", "Nombres", 230),
            New ColumnaReporte("Fecha corte", "FechaFin", 75, "dd/MM/yyyy")}

        For Each mes As String In CsHistorialSalarialDAL.Meses
            If TieneDatos(historial, mes) Then columnas.Add(New ColumnaReporte(mes, mes, 82, "#,##0.00", True))
        Next

        Dim titulo As String = "Historial Salarial - " & If(tipo = CsHistorialSalarialDAL.TipoAguinaldo, "Aguinaldo", "Vacaciones")
        Dim periodo As String = "Nómina No. " & numNomina

        Return New RptTabular(empresa, titulo, periodo, historial, columnas)

    End Function

    ''' <summary>True si algún empleado tiene un valor distinto de cero en la columna del mes.</summary>
    Public Shared Function TieneDatos(historial As DataTable, mes As String) As Boolean
        Return historial.AsEnumerable().Any(Function(f) Not IsDBNull(f(mes)) AndAlso Convert.ToDouble(f(mes)) <> 0)
    End Function

End Class
