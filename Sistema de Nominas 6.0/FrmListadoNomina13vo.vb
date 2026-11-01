''' <summary>
''' Listado histórico de nóminas de 13vo mes (V.5.0 FrmListadoNominaVacaciones, pestaña "Nominas 13vo Mes"). Datos en
''' <see cref="CsAguinaldoDAL"/>; la mecánica del listado está en <see cref="FrmListadoNominaBase"/>.
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmListadoNomina13vo
    Inherits FrmListadoNominaBase

    Private ReadOnly _dal As New CsAguinaldoDAL

    Public Sub New()
        MyBase.New("Listado de Nóminas de 13vo Mes")
    End Sub

    Protected Overrides Function EtiquetaNomina() As String
        Return "13vo Mes"
    End Function

    Protected Overrides Function ListarNominas() As DataTable
        Return _dal.Listar()
    End Function

    Protected Overrides Function ListarDetalle(numero As Integer) As DataTable
        Return _dal.ListarDetalle(numero)
    End Function

    Protected Overrides Function ListarMontos(numero As Integer) As DataTable
        Return _dal.ListarMontos(numero)
    End Function

    Protected Overrides Sub ConfigurarDetalle()

        VistaDetalle.PopulateColumns()
        If VistaDetalle.Columns.Count = 0 Then Return

        VistaDetalle.Columns("Codigo").Caption = "Código"
        VistaDetalle.Columns("Codigo").Width = 90
        VistaDetalle.Columns("Nombres").Caption = "Nombre del empleado"
        VistaDetalle.Columns("Nombres").Width = 260
        Formatear("SalarioMensual", "Salario mensual", 110, sumar:=True)
        Formatear("DiasAPagar", "Días a pagar", 80, sumar:=True)
        Formatear("SalarioAPagar", "Salario a pagar", 110, sumar:=True)
        Formatear("Adelanto", "Adelanto 13vo", 100, sumar:=True)
        Formatear("MontoPagar", "Monto a pagar", 110, sumar:=True)

    End Sub

    Protected Overrides Sub Colillas(numero As Integer, periodo As String)

        Dim datos As DataTable = _dal.ListarParaReporte(numero, "")
        Dim detalle As New List(Of ConceptoColilla) From {
            New ConceptoColilla("Salario mensual", "SalarioMensual"),
            New ConceptoColilla("Días a pagar", "DiasAPagar", "{0:0.00}")}
        Dim deducciones As New List(Of ConceptoColilla) From {New ConceptoColilla("Adelanto de 13vo mes", "Adelanto")}

        Dim reporte As New RptColillaPago(New CsControlesDAL().ObtenerDatosEmpresa(), "Colilla de 13vo Mes", periodo, datos,
                                          detalle, deducciones, ChkPorDepartamento.Checked)
        FrmVisorReporte.Mostrar(reporte, $"Colillas 13vo Mes {numero}", Me)

    End Sub

    Protected Overrides Sub NominaImpresa(numero As Integer, periodo As String)

        Dim columnas As New List(Of ColumnaReporte) From {
            New ColumnaReporte("Código", "Codigo", 70),
            New ColumnaReporte("Nombres", "Nombres", 260),
            New ColumnaReporte("Fecha ingreso", "FechaIngreso", 80, "dd/MM/yyyy"),
            New ColumnaReporte("Salario mensual", "SalarioMensual", 100, "#,##0.00", True),
            New ColumnaReporte("Días a pagar", "DiasAPagar", 70, "0.00"),
            New ColumnaReporte("Total devengado", "TotalDevengado", 100, "#,##0.00", True),
            New ColumnaReporte("Deducciones", "TotalDeducir", 90, "#,##0.00", True),
            New ColumnaReporte("Monto a pagar", "MontoPagar", 100, "#,##0.00", True)}

        Dim reporte As New RptTabular(New CsControlesDAL().ObtenerDatosEmpresa(), "Nómina de 13vo Mes", periodo,
                                      _dal.ListarParaReporte(numero, ""), columnas, "Departamento", "Departamento")
        FrmVisorReporte.Mostrar(reporte, $"Nomina 13vo Mes {numero}", Me)

    End Sub

    Protected Overrides Sub AbrirHistorial(numero As Integer)
        Using frm As New FrmSalarioHistorial(CsHistorialSalarialDAL.TipoAguinaldo, numero, 0)
            frm.ShowDialog(Me)
        End Using
    End Sub

End Class
