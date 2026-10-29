Imports System.Data
Imports System.Data.SqlClient

''' <summary>Tablas de tramos del INSS laboral/patronal y del IR (tablas INSS e IR), editables como cuadrícula.</summary>
Public Class CsInssIrDAL
    Inherits CsDALBase

    Private Shared ReadOnly ColumnasInss As String() = {
        "Categoria", "Desde", "Hasta", "Promedio Semanal", "MontoLaboral1", "MontoPatronal1",
        "MontoLaboral4", "MontoPatronal4", "MontoLaboral5", "MontoPatronal5"}

    Private Shared ReadOnly ColumnasIr As String() = {
        "Desde", "Hasta", "ImpuestoBase", "PorcientoImpuesto", "SobreExceso"}

    Public Function ListarInss() As DataTable
        Return ConsultarTabla("SELECT * FROM INSS ORDER BY CodInss")
    End Function

    Public Function ListarIr() As DataTable
        Return ConsultarTabla("SELECT * FROM IR ORDER BY CodIr")
    End Function

    ''' <summary>Aplica a la tabla INSS las filas nuevas, modificadas y borradas de <paramref name="datos"/>. Devuelve las filas tocadas.</summary>
    Public Function GuardarInss(datos As DataTable) As Integer
        Return Guardar(datos, "INSS", "CodInss", ColumnasInss)
    End Function

    Public Function GuardarIr(datos As DataTable) As Integer
        Return Guardar(datos, "IR", "CodIr", ColumnasIr)
    End Function

    ''' <summary>Los nombres de tabla/columna son constantes de este módulo, nunca entrada del usuario.</summary>
    Private Function Guardar(datos As DataTable, tabla As String, llave As String, columnas As String()) As Integer

        Dim cambios As DataTable = datos.GetChanges()

        If cambios Is Nothing Then Return 0

        Dim tocadas As Integer = 0

        EnTransaccion(
            Sub(cn, tr)

                For Each fila As DataRow In cambios.Rows

                    If fila.RowState = DataRowState.Deleted Then

                        tocadas += EjecutarEn(cn, tr, "DELETE FROM " & tabla & " WHERE " & llave & " = @K",
                                              New SqlParameter("@K", Convert.ToString(fila(llave, DataRowVersion.Original))))
                        Continue For

                    End If

                    Dim codigo As String = Convert.ToString(fila(llave)).Trim()

                    If codigo = "" Then
                        Throw New InvalidOperationException("Todas las filas deben tener código.")
                    End If

                    Dim p As New List(Of SqlParameter) From {New SqlParameter("@K", codigo)}

                    For i As Integer = 0 To columnas.Length - 1
                        p.Add(New SqlParameter("@P" & i, If(IsDBNull(fila(columnas(i))), DBNull.Value, fila(columnas(i)))))
                    Next

                    If fila.RowState = DataRowState.Added Then

                        Dim nombres As String = String.Join(", ", columnas.Select(Function(c) "[" & c & "]"))
                        Dim valores As String = String.Join(", ", columnas.Select(Function(c, i) "@P" & i))

                        tocadas += EjecutarEn(cn, tr,
                            "INSERT INTO " & tabla & " (" & llave & ", " & nombres & ") VALUES (@K, " & valores & ")", p.ToArray())

                    Else

                        Dim asignaciones As String = String.Join(", ", columnas.Select(Function(c, i) "[" & c & "] = @P" & i))

                        ' El código original puede haber cambiado: se ubica la fila por su valor original.
                        p.Add(New SqlParameter("@KO", Convert.ToString(fila(llave, DataRowVersion.Original))))

                        tocadas += EjecutarEn(cn, tr,
                            "UPDATE " & tabla & " SET " & llave & " = @K, " & asignaciones & " WHERE " & llave & " = @KO", p.ToArray())

                    End If

                Next

            End Sub)

        datos.AcceptChanges()

        Return tocadas

    End Function

End Class
