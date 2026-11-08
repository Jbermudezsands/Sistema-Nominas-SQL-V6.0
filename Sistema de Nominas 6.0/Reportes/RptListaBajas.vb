''' <summary>
''' Listado de liquidaciones finales (V.5.0 ArepListaBajas / ListadoBajas, ActiveReports): código, nombre, fechas de ingreso y egreso, área,
''' tipo de baja, montos de vacaciones, aguinaldo e indemnización, deducciones, INSS, IR, neto a pagar y columnas para número de cheque
''' y firma. Es un <see cref="RptTabular"/> con las columnas de la V.5.0 y total general; la tabla viene de <see cref="CsListaBajasDAL.Listar"/>.
''' </summary>
Public Class RptListaBajas
    Inherits RptTabular

    Public Sub New(empresa As DatosEmpresa, bajas As DataTable, Optional periodo As String = "")
        MyBase.New(empresa, "LIQUIDACION FINAL", periodo, Preparar(bajas), Columnas())
    End Sub

    ''' <summary>Agrega las columnas vacías de número de cheque y firma, que se llenan a mano en el papel.</summary>
    Private Shared Function Preparar(bajas As DataTable) As DataTable
        For Each nombre As String In {"ChkNo", "Firma"}
            If Not bajas.Columns.Contains(nombre) Then bajas.Columns.Add(nombre, GetType(String))
        Next
        Return bajas
    End Function

    Private Shared Function Columnas() As IList(Of ColumnaReporte)
        Return New List(Of ColumnaReporte) From {
            New ColumnaReporte("Codigo Empleado", "CodEmpleado1", 45),
            New ColumnaReporte("Nombre Empleado", "Nombres", 140),
            New ColumnaReporte("Fecha Ingreso", "FechaIngreso", 58, "dd/MM/yyyy"),
            New ColumnaReporte("Fecha Egreso", "FechaBaja", 58, "dd/MM/yyyy"),
            New ColumnaReporte("Area", "Departamento", 80),
            New ColumnaReporte("Tipo de Baja", "TipoBaja", 50),
            New ColumnaReporte("Monto Vacaciones", "MontoVacaciones", 60, "#,##0.00", True),
            New ColumnaReporte("Monto Aguinaldo", "MontoAguinaldo", 60, "#,##0.00", True),
            New ColumnaReporte("Monto Indemnizacion", "MontoIndemnizacion", 60, "#,##0.00", True),
            New ColumnaReporte("Deducciones", "Deducciones", 56, "#,##0.00", True),
            New ColumnaReporte("Monto INSS", "MontoINSS", 52, "#,##0.00", True),
            New ColumnaReporte("Monto IR", "MontoIR", 52, "#,##0.00", True),
            New ColumnaReporte("Neto a Pagar", "NetoPagar", 66, "#,##0.00", True),
            New ColumnaReporte("Numero INSS", "NumeroInss", 62),
            New ColumnaReporte("Chk No", "ChkNo", 40),
            New ColumnaReporte("Firma", "Firma", 80)}
    End Function

End Class
