Imports System.Globalization

''' <summary>
''' Archivos de pago para los bancos (V.5.0 CmdExportaBanpro, PushButton1 "Bacentro" y FrmExportaBac). Arma tablas de texto con el
''' formato de cada banco; <see cref="ExportadorGrid.ExportarTabla"/> las guarda como .xlsx. La tabla de pagos es la de
''' <see cref="CsAguinaldoDAL.ListarParaBanco"/> (Nombres, Monto, NumCedula y las cuentas); sin acceso a datos.
''' </summary>
Public Class ExportadorBancos

    Private Shared ReadOnly Cultura As CultureInfo = CultureInfo.InvariantCulture

    ''' <summary>Qué columna de cuenta usa cada formato.</summary>
    Public Const CuentaBac As String = "CuentaBanco"
    Public Const CuentaBanpro As String = "CuentaBancoBANPRO"

    ''' <summary>
    ''' Formato de Banpro y de Bacentro: una fila por empleado (nombre, cuenta, concepto, monto, "C" crédito) y al final la de la
    ''' empresa con el total ("D" débito). Los empleados sin monto no se incluyen.
    ''' </summary>
    ''' <param name="columnaCuenta"><see cref="CuentaBanpro"/> o <see cref="CuentaBac"/> (Bacentro usa la cuenta del histórico, como la V.5.0).</param>
    Public Shared Function FormatoCreditoDebito(pagos As DataTable, columnaCuenta As String, concepto As String,
                                                nombreEmpresa As String, cuentaEmpresa As String) As DataTable

        Dim t As New DataTable
        For i As Integer = 1 To 5
            t.Columns.Add("C" & i, GetType(String))
        Next

        Dim total As Decimal = 0

        For Each f As DataRow In pagos.Rows
            Dim monto As Decimal = Math.Round(Convert.ToDecimal(f("Monto")), 2)
            If monto = 0 Then Continue For
            total += monto
            t.Rows.Add(Texto(f("Nombres")), Texto(f(columnaCuenta)), concepto, monto.ToString("#,##0.00", Cultura), "C")
        Next

        t.Rows.Add(nombreEmpresa, cuentaEmpresa, concepto, total.ToString("#,##0.00", Cultura), "D")

        Return t

    End Function

    ''' <summary>
    ''' Formato BAC: fila de encabezado ("B", código del cliente, consecutivo, fecha, total y cantidad) y una fila "T" por empleado con
    ''' su cédula, fecha, monto sin punto decimal (13 dígitos), concepto y nombre (25 caracteres). Con <paramref name="tasaCambio"/> mayor
    ''' que cero los montos se pasan a dólares.
    ''' </summary>
    Public Shared Function FormatoBac(pagos As DataTable, codigoBac As String, consecutivo As Integer, fecha As Date,
                                      concepto As String, tasaCambio As Double) As DataTable

        Dim t As New DataTable
        For i As Integer = 1 To 13
            t.Columns.Add("C" & i, GetType(String))
        Next

        Dim anio As String = fecha.Year.ToString("00000")
        Dim mes As String = fecha.Month.ToString("00")
        Dim dia As String = fecha.Day.ToString("00")
        Dim consec As String = consecutivo.ToString("00000")

        Dim encabezado As DataRow = t.Rows.Add("B", codigoBac, consec, "", "", anio, mes, dia, "", "", "", "", "")

        Dim n As Integer = 0
        Dim totalCentavos As Long = 0

        For Each f As DataRow In pagos.Rows
            Dim monto As Decimal = Convert.ToDecimal(f("Monto"))
            If tasaCambio > 0 Then monto = monto / CDec(tasaCambio)
            Dim centavos As Long = CLng(Math.Round(monto * 100, 0, MidpointRounding.AwayFromZero))
            If centavos = 0 Then Continue For

            n += 1
            totalCentavos += centavos
            Dim nombre As String = Texto(f("Nombres"))
            If nombre.Length > 25 Then nombre = nombre.Substring(0, 25)

            t.Rows.Add("T", codigoBac, consec, Texto(f("NumCedula")), n.ToString("00000"), anio, mes, dia,
                       centavos.ToString("0000000000000"), "", concepto, "", nombre)
        Next

        encabezado("C9") = totalCentavos.ToString("0000000000000")
        encabezado("C10") = n.ToString("00000")

        Return t

    End Function

    Private Shared Function Texto(valor As Object) As String
        Return If(valor Is Nothing OrElse IsDBNull(valor), "", CStr(valor).Trim())
    End Function

End Class
