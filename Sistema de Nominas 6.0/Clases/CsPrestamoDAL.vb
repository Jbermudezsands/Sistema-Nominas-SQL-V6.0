Imports System.Data
Imports System.Data.SqlClient

''' <summary>Préstamos del empleado (V.5.0 pestaña Préstamo): Prestamo + MovPrestamo (libreta).</summary>
Public Class CsPrestamoDAL
    Inherits CsDALBase

    ''' <summary>Préstamo vigente (no cancelado) del empleado, o Nothing. Incluye cuentas, saldo y libreta.</summary>
    Public Function BuscarVigente(codEmpleado As Double) As PrestamoEmpleado

        Dim dt As DataTable = ConsultarTabla(
            "SELECT TOP 1 NumPrestamo, CuentaDebito, CuentaCredito, Monto, CantCuotas, Interes, Saldo, Moneda, CuotasIguales " &
            "FROM Prestamo WHERE CodEmpleado = @Emp AND ISNULL(Cancelado, 0) = 0 ORDER BY NumPrestamo DESC",
            New SqlParameter("@Emp", codEmpleado))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New PrestamoEmpleado With {
            .Num_Prestamo = Convert.ToInt32(r("NumPrestamo")),
            .Cod_Empleado = codEmpleado,
            .Cuenta_Debito = Texto(r("CuentaDebito")),
            .Cuenta_Credito = Texto(r("CuentaCredito")),
            .Monto = Numero(r("Monto")),
            .Cant_Cuotas = Convert.ToInt32(Numero(r("CantCuotas"))),
            .Interes = Numero(r("Interes")),
            .Saldo = Numero(r("Saldo")),
            .Moneda = Texto(r("Moneda")),
            .Cuotas_Iguales = Not IsDBNull(r("CuotasIguales")) AndAlso Convert.ToBoolean(r("CuotasIguales"))
        }

    End Function

    ''' <summary>Cuotas no canceladas del préstamo vigente.</summary>
    Public Function ListarLibreta(codEmpleado As Double) As DataTable

        Return ConsultarTabla(
            "SELECT m.NumCuota, m.Monto, m.Interes, m.CuotaIgual, m.SaldoCuota " &
            "FROM MovPrestamo m INNER JOIN Prestamo p ON p.NumPrestamo = m.NumPrestamo " &
            "WHERE p.CodEmpleado = @Emp AND ISNULL(p.Cancelado, 0) = 0 AND ISNULL(m.Cancelado, 0) = 0 " &
            "ORDER BY m.NumPrestamo, m.NumCuota",
            New SqlParameter("@Emp", codEmpleado))

    End Function

    ''' <summary>Crea Prestamo y su libreta en una transacción. Un empleado no puede tener dos préstamos con saldo.</summary>
    Public Function Crear(p As PrestamoEmpleado) As Integer

        Dim vigente As PrestamoEmpleado = BuscarVigente(p.Cod_Empleado)

        If Not IsNothing(vigente) AndAlso vigente.Saldo <> 0 Then
            Throw New Exception("Este empleado ya tiene un préstamo y su saldo es de: " & vigente.Saldo.ToString("N2"))
        End If

        Dim numero As Integer = 0

        EnTransaccion(
            Sub(cn, tr)

                numero = Convert.ToInt32(EscalarEn(cn, tr,
                    "SELECT ISNULL(MAX(NumPrestamo), 0) + 1 FROM Prestamo WITH (UPDLOCK, HOLDLOCK)"))

                EjecutarEn(cn, tr,
                    "INSERT INTO Prestamo (NumPrestamo, CuentaDebito, CuentaCredito, Monto, CantCuotas, Interes, Saldo, " &
                    "FechaInicial, Cancelado, Moneda, CuotasIguales, CodEmpleado) " &
                    "VALUES (@Num, @Deb, @Cre, @Monto, @Cuotas, @Int, @Saldo, @Fecha, 0, @Mon, @Iguales, @Emp)",
                    New SqlParameter("@Num", numero),
                    New SqlParameter("@Deb", p.Cuenta_Debito),
                    New SqlParameter("@Cre", p.Cuenta_Credito),
                    New SqlParameter("@Monto", p.Monto),
                    New SqlParameter("@Cuotas", p.Cant_Cuotas),
                    New SqlParameter("@Int", p.Interes),
                    New SqlParameter("@Saldo", p.Saldo),
                    New SqlParameter("@Fecha", Now),
                    New SqlParameter("@Mon", p.Moneda),
                    New SqlParameter("@Iguales", p.Cuotas_Iguales),
                    New SqlParameter("@Emp", p.Cod_Empleado))

                For Each c As CuotaPrestamo In p.Cuotas
                    EjecutarEn(cn, tr,
                        "INSERT INTO MovPrestamo (ID, NumPrestamo, NumCuota, Monto, Interes, SaldoCuota, CuotaIgual, Cancelado) " &
                        "VALUES (@Id, @Num, @Cuota, @Monto, @Int, @SaldoC, @Igual, 0)",
                        New SqlParameter("@Id", c.Num_Cuota),
                        New SqlParameter("@Num", numero),
                        New SqlParameter("@Cuota", c.Num_Cuota),
                        New SqlParameter("@Monto", c.Monto),
                        New SqlParameter("@Int", c.Interes),
                        New SqlParameter("@SaldoC", c.Saldo_Cuota),
                        New SqlParameter("@Igual", p.Cuota_Igual))
                Next

            End Sub)

        Return numero

    End Function

    ''' <summary>Marca como cancelado el préstamo vigente y sus cuotas (no borra).</summary>
    Public Sub Cancelar(codEmpleado As Double)

        EnTransaccion(
            Sub(cn, tr)
                EjecutarEn(cn, tr,
                    "UPDATE m SET m.Cancelado = 1 FROM MovPrestamo m " &
                    "INNER JOIN Prestamo p ON p.NumPrestamo = m.NumPrestamo " &
                    "WHERE p.CodEmpleado = @Emp AND ISNULL(p.Cancelado, 0) = 0",
                    New SqlParameter("@Emp", codEmpleado))

                EjecutarEn(cn, tr,
                    "UPDATE Prestamo SET Cancelado = 1 WHERE CodEmpleado = @Emp AND ISNULL(Cancelado, 0) = 0",
                    New SqlParameter("@Emp", codEmpleado))
            End Sub)

    End Sub

    Private Shared Function Texto(v As Object) As String
        Return If(IsDBNull(v), "", Convert.ToString(v).Trim)
    End Function

    Private Shared Function Numero(v As Object) As Double
        Return If(IsDBNull(v), 0, Convert.ToDouble(v))
    End Function

End Class
