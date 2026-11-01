''' <summary>
''' Listado histórico de nóminas de vacaciones (V.5.0 FrmListadoNominaVacaciones, pestaña "Nominas Vacaciones"). Datos en
''' <see cref="CsNominaVacacionesDAL"/>; la mecánica del listado está en <see cref="FrmListadoNominaBase"/>.
''' </summary>
<System.ComponentModel.DesignerCategory("Code")>
Public Class FrmListadoNominaVacaciones
    Inherits FrmListadoNominaBase

    Private ReadOnly _dal As New CsNominaVacacionesDAL

    Public Sub New()
        MyBase.New("Listado de Nóminas de Vacaciones")
    End Sub

    Protected Overrides ReadOnly Property MuestraRestarImpuestos As Boolean = True

    Protected Overrides Function EtiquetaNomina() As String
        Return "Vacaciones"
    End Function

    Protected Overrides Function ListarNominas() As DataTable
        Return _dal.Listar()
    End Function

    Protected Overrides Function ListarDetalle(numero As Integer) As DataTable
        Return _dal.ListarDetalle(numero, ChkRestarImpuestos.Checked)
    End Function

    Protected Overrides Function ListarMontos(numero As Integer) As DataTable
        Return _dal.ListarMontos(numero, ChkRestarImpuestos.Checked)
    End Function

    Protected Overrides Sub ConfigurarDetalle()

        VistaDetalle.PopulateColumns()
        If VistaDetalle.Columns.Count = 0 Then Return

        VistaDetalle.Columns("Codigo").Caption = "Código"
        VistaDetalle.Columns("Codigo").Width = 90
        VistaDetalle.Columns("Nombres").Caption = "Nombre del empleado"
        VistaDetalle.Columns("Nombres").Width = 260
        Formatear("SalarioMensual", "Salario promedio", 110, sumar:=True)
        Formatear("DiasAPagar", "Días a pagar", 80, sumar:=True)
        Formatear("DiasDescuento", "Días desc.", 80, sumar:=True)
        Formatear("Adelanto", "Adelanto vac.", 100, sumar:=True)
        Formatear("Inss", "INSS vac.", 90, sumar:=True)
        Formatear("Ir", "IR vac.", 90, sumar:=True)
        Formatear("TotalDevengado", "Total devengado", 110, sumar:=True)
        Formatear("MontoPagar", "Monto a pagar", 110, sumar:=True)

    End Sub

    Protected Overrides Sub Colillas(numero As Integer, periodo As String)

        Dim restar As Boolean = ChkRestarImpuestos.Checked
        Dim datos As DataTable = _dal.ListarParaReporte(numero, restar, "")

        Dim detalle As New List(Of ConceptoColilla) From {
            New ConceptoColilla("Salario promedio mensual", "SalarioMensual"),
            New ConceptoColilla("Días a pagar", "DiasAPagar", "{0:0.00}"),
            New ConceptoColilla("Días de descuento", "DiasDescuento", "{0:0.00}"),
            New ConceptoColilla("Días pagados", "TotalDias", "{0:0.00}")}
        Dim deducciones As New List(Of ConceptoColilla) From {New ConceptoColilla("Adelanto de vacaciones", "Adelanto")}
        If restar Then
            deducciones.Add(New ConceptoColilla("INSS de vacaciones", "Inss"))
            deducciones.Add(New ConceptoColilla("IR de vacaciones", "Ir"))
        End If

        Dim reporte As New RptColillaPago(New CsControlesDAL().ObtenerDatosEmpresa(), "Colilla de Vacaciones", periodo, datos,
                                          detalle, deducciones, ChkPorDepartamento.Checked)
        FrmVisorReporte.Mostrar(reporte, $"Colillas Vacaciones {numero}", Me)

    End Sub

    Protected Overrides Sub NominaImpresa(numero As Integer, periodo As String)

        Dim columnas As New List(Of ColumnaReporte) From {
            New ColumnaReporte("Código", "Codigo", 60),
            New ColumnaReporte("Nombres", "Nombres", 190),
            New ColumnaReporte("Cédula", "Cedula", 85),
            New ColumnaReporte("No. INSS", "NumeroInss", 70),
            New ColumnaReporte("Salario promedio", "SalarioMensual", 85, "#,##0.00", True),
            New ColumnaReporte("Días pagar", "DiasAPagar", 55, "0.00"),
            New ColumnaReporte("Días desc.", "DiasDescuento", 55, "0.00"),
            New ColumnaReporte("Adelanto", "Adelanto", 75, "#,##0.00", True),
            New ColumnaReporte("Total devengado", "TotalDevengado", 85, "#,##0.00", True),
            New ColumnaReporte("INSS", "Inss", 70, "#,##0.00", True),
            New ColumnaReporte("IR", "Ir", 70, "#,##0.00", True),
            New ColumnaReporte("Monto a pagar", "MontoPagar", 85, "#,##0.00", True)}

        Dim reporte As New RptTabular(New CsControlesDAL().ObtenerDatosEmpresa(), "Nómina de Vacaciones", periodo,
                                      _dal.ListarParaReporte(numero, ChkRestarImpuestos.Checked, ""), columnas, "Departamento", "Departamento")
        FrmVisorReporte.Mostrar(reporte, $"Nomina Vacaciones {numero}", Me)

    End Sub

    Protected Overrides Sub AbrirHistorial(numero As Integer)
        Using frm As New FrmSalarioHistorial(CsHistorialSalarialDAL.TipoVacaciones, 0, numero)
            frm.ShowDialog(Me)
        End Using
    End Sub

End Class
