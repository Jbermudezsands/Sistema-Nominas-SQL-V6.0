Imports System.Data
Imports System.Data.SqlClient

''' <summary>Tasas de cambio diarias (tabla CambioMoneda, una por fecha).</summary>
Public Class CsCambioMonedaDAL
    Inherits CsDALBase

    Public Function Listar() As DataTable
        Return ConsultarTabla("SELECT FechaDia, MontoDia FROM CambioMoneda ORDER BY FechaDia DESC")
    End Function

    ''' <summary>Tasa de la fecha indicada; Nothing si ese día no se ha grabado.</summary>
    Public Function Buscar(fecha As DateTime) As TasaCambio

        Dim dt As DataTable = ConsultarTabla(
            "SELECT FechaDia, MontoDia FROM CambioMoneda WHERE FechaDia = @F",
            New SqlParameter("@F", SqlDbType.SmallDateTime) With {.Value = fecha.Date})

        If dt.Rows.Count = 0 Then Return Nothing

        Return Convertir(dt.Rows(0))

    End Function

    ''' <summary>Tasa de hoy; Nothing si todavía no se ha grabado (VB6 avisaba y no dejaba cerrar).</summary>
    Public Function TasaDeHoy() As TasaCambio
        Return Buscar(DateTime.Today)
    End Function

    ''' <summary>Última tasa grabada (la más reciente por fecha); Nothing si la tabla está vacía.</summary>
    Public Function UltimaTasa() As TasaCambio

        Dim dt As DataTable = ConsultarTabla("SELECT TOP 1 FechaDia, MontoDia FROM CambioMoneda ORDER BY FechaDia DESC")

        If dt.Rows.Count = 0 Then Return Nothing

        Return Convertir(dt.Rows(0))

    End Function

    ''' <summary>Graba la tasa de la fecha: crea la fila o, si ya existe, cambia el monto.</summary>
    Public Sub Guardar(t As TasaCambio)

        If t.Monto <= 0 Then
            Throw New InvalidOperationException("La tasa de cambio debe ser mayor que cero.")
        End If

        ' Un SqlParameter no puede pertenecer a dos comandos: cada sentencia recibe los suyos.
        Dim parametros As Func(Of SqlParameter()) =
            Function() {
                New SqlParameter("@F", SqlDbType.SmallDateTime) With {.Value = t.Fecha.Date},
                New SqlParameter("@M", SqlDbType.Money) With {.Value = t.Monto}}

        EnTransaccion(
            Sub(cn, tr)
                If EjecutarEn(cn, tr, "UPDATE CambioMoneda SET MontoDia = @M WHERE FechaDia = @F", parametros()) = 0 Then
                    EjecutarEn(cn, tr, "INSERT INTO CambioMoneda (FechaDia, MontoDia) VALUES (@F, @M)", parametros())
                End If
            End Sub)

    End Sub

    Public Function Eliminar(fecha As DateTime) As Boolean
        Return Ejecutar("DELETE FROM CambioMoneda WHERE FechaDia = @F",
                        New SqlParameter("@F", SqlDbType.SmallDateTime) With {.Value = fecha.Date}) > 0
    End Function

    Private Shared Function Convertir(r As DataRow) As TasaCambio
        Return New TasaCambio With {
            .Fecha = Convert.ToDateTime(r("FechaDia")),
            .Monto = If(IsDBNull(r("MontoDia")), 0D, Convert.ToDecimal(r("MontoDia")))
        }
    End Function

End Class
