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

    ''' <summary>
    ''' Graba varias tasas en una sola transacción (crea las que faltan y cambia el monto de las que ya existen). Devuelve cuántas se
    ''' agregaron y cuántas se actualizaron.
    ''' </summary>
    Public Function GuardarVarias(tasas As IEnumerable(Of TasaCambio)) As (Agregadas As Integer, Actualizadas As Integer)

        Dim agregadas As Integer = 0
        Dim actualizadas As Integer = 0

        For Each t As TasaCambio In tasas
            If t.Monto <= 0 Then Throw New InvalidOperationException($"La tasa del {t.Fecha:dd/MM/yyyy} debe ser mayor que cero.")
        Next

        EnTransaccion(
            Sub(cn, tr)
                For Each t As TasaCambio In tasas
                    Dim f As Date = t.Fecha.Date
                    Dim m As Decimal = t.Monto
                    If EjecutarEn(cn, tr, "UPDATE CambioMoneda SET MontoDia = @M WHERE FechaDia = @F",
                                  New SqlParameter("@F", SqlDbType.SmallDateTime) With {.Value = f},
                                  New SqlParameter("@M", SqlDbType.Money) With {.Value = m}) > 0 Then
                        actualizadas += 1
                    Else
                        EjecutarEn(cn, tr, "INSERT INTO CambioMoneda (FechaDia, MontoDia) VALUES (@F, @M)",
                                   New SqlParameter("@F", SqlDbType.SmallDateTime) With {.Value = f},
                                   New SqlParameter("@M", SqlDbType.Money) With {.Value = m})
                        agregadas += 1
                    End If
                Next
            End Sub)

        Return (agregadas, actualizadas)

    End Function

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
