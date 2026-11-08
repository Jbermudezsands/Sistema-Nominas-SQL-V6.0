Imports System.Data
Imports System.Data.SqlClient

''' <summary>Tabla de porcentajes de deducción por rango (DeduccionPorciento; V.5.0 FrmDeduccionPorciento).</summary>
Public Class CsDeduccionPorcientoDAL
    Inherits CsDALBase

    Public Function Listar() As DataTable
        Dim dt As DataTable = ConsultarTabla(
            "SELECT IdDeduccionPorciento, CAST(ISNULL(Rango1,0) AS float) AS Rango1, CAST(ISNULL(Rango2,0) AS float) AS Rango2, " &
            "CAST(ISNULL(Porciento,0) AS float) AS Porciento FROM DeduccionPorciento ORDER BY Rango1, Rango2")
        dt.Columns("IdDeduccionPorciento").ReadOnly = False
        Return dt
    End Function

    ''' <summary>Valida las filas (no eliminadas): rangos no negativos, Hasta >= Desde y Porciento entre 0 y 100.</summary>
    Public Shared Function Validar(tabla As DataTable) As String

        For Each r As DataRow In tabla.Rows
            If r.RowState = DataRowState.Deleted Then Continue For
            Dim d As Double = ValorODefecto(r("Rango1"))
            Dim h As Double = ValorODefecto(r("Rango2"))
            Dim p As Double = ValorODefecto(r("Porciento"))
            If d < 0 OrElse h < 0 Then Return "Los rangos no pueden ser negativos."
            If h < d Then Return "El rango 'Hasta' (" & h.ToString("N2") & ") no puede ser menor que 'Desde' (" & d.ToString("N2") & ")."
            If p < 0 OrElse p > 100 Then Return "El porciento debe estar entre 0 y 100."
        Next

        Return ""

    End Function

    Private Shared Function ValorODefecto(o As Object) As Double
        Return If(IsDBNull(o), 0.0, Convert.ToDouble(o))
    End Function

    ''' <summary>Aplica en una transacción las filas nuevas, cambiadas y eliminadas de <paramref name="tabla"/>.</summary>
    Public Sub Guardar(tabla As DataTable)

        Dim cambios As DataTable = tabla.GetChanges()
        If cambios Is Nothing Then Exit Sub

        EnTransaccion(
            Sub(cn, tr)
                For Each r As DataRow In cambios.Rows
                    Select Case r.RowState
                        Case DataRowState.Added
                            EjecutarEn(cn, tr, "INSERT INTO DeduccionPorciento (Rango1, Rango2, Porciento) VALUES (@R1, @R2, @P)",
                                       New SqlParameter("@R1", ValorODefecto(r("Rango1"))),
                                       New SqlParameter("@R2", ValorODefecto(r("Rango2"))),
                                       New SqlParameter("@P", ValorODefecto(r("Porciento"))))
                        Case DataRowState.Modified
                            EjecutarEn(cn, tr, "UPDATE DeduccionPorciento SET Rango1 = @R1, Rango2 = @R2, Porciento = @P WHERE IdDeduccionPorciento = @Id",
                                       New SqlParameter("@R1", ValorODefecto(r("Rango1"))),
                                       New SqlParameter("@R2", ValorODefecto(r("Rango2"))),
                                       New SqlParameter("@P", ValorODefecto(r("Porciento"))),
                                       New SqlParameter("@Id", r("IdDeduccionPorciento")))
                        Case DataRowState.Deleted
                            EjecutarEn(cn, tr, "DELETE FROM DeduccionPorciento WHERE IdDeduccionPorciento = @Id",
                                       New SqlParameter("@Id", r("IdDeduccionPorciento", DataRowVersion.Original)))
                    End Select
                Next
            End Sub)

    End Sub

End Class
