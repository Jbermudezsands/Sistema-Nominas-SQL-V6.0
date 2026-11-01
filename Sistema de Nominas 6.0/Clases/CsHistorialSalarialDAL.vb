Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Lectura de <c>HistorialSalarioMes</c>: el salario de cada mes que tomó el cálculo del aguinaldo o de las vacaciones
''' (V.5.0 FrmSalarioHistorial). Solo consulta; el historial lo escriben <see cref="CsAguinaldoDAL"/> y
''' <see cref="CsNominaVacacionesDAL"/> al calcular.
''' </summary>
Public Class CsHistorialSalarialDAL
    Inherits CsDALBase

    Public Const TipoAguinaldo As String = "Aguinaldo"
    Public Const TipoVacaciones As String = "Vacaciones"

    Public Shared ReadOnly Meses As String() = {
        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"}

    ''' <summary>
    ''' Historial de la nómina <paramref name="numNomina"/> del <paramref name="tipo"/> ("Aguinaldo" o "Vacaciones"), una fila por
    ''' empleado con sus doce meses. Columnas: Codigo, Nombres, Cargo, Departamento, FechaIni, FechaFin, Enero..Diciembre, NumNomina.
    ''' </summary>
    Public Function Listar(tipo As String, numNomina As Integer) As DataTable

        Return ConsultarTabla(
            "SELECT E.CodEmpleado1 AS Codigo, " &
            "RTRIM(ISNULL(E.Nombre1 + ' ', '') + ISNULL(E.Nombre2 + ' ', '') + ISNULL(E.Apellido1 + ' ', '') + ISNULL(E.Apellido2, '')) AS Nombres, " &
            "C.Cargo, D.Departamento, H.FechaIni, H.FechaFin, " &
            "H.Enero, H.Febrero, H.Marzo, H.Abril, H.Mayo, H.Junio, H.Julio, H.Agosto, H.Septiembre, H.Octubre, H.Noviembre, H.Diciembre, " &
            "H.NumNomina " &
            "FROM HistorialSalarioMes H " &
            "INNER JOIN Empleado E ON E.CodEmpleado = H.CodEmpleado " &
            "LEFT JOIN Cargo C ON C.CodCargo = E.CodCargo " &
            "LEFT JOIN Departamento D ON D.CodDepartamento = E.CodDepartamento " &
            "WHERE H.NumNomina = @Num AND H.Tipo = @Tipo " &
            "ORDER BY Nombres",
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numNomina},
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 50) With {.Value = tipo})

    End Function

    ''' <summary>
    ''' Graba, dentro de la transacción de quien calcula, el salario de cada mes del empleado en este período: actualiza la fila
    ''' (empleado + fecha inicial + fecha final) o la crea; solo guarda los meses con valor (<paramref name="salarios"/> va del
    ''' índice 1 al 12).
    ''' </summary>
    Friend Shared Sub GuardarEn(cn As SqlConnection, tr As SqlTransaction, tipo As String, codEmpleado As String, fechaIni As Date,
                                fechaFin As Date, numero As Integer, salarios As Double())

        Dim sets As New List(Of String)
        Dim columnas As New List(Of String)
        Dim valores As New List(Of String)
        Dim parametros As New List(Of SqlParameter) From {
            New SqlParameter("@Cod", SqlDbType.Decimal) With {.Value = Convert.ToDecimal(codEmpleado)},
            New SqlParameter("@Ini", SqlDbType.DateTime) With {.Value = fechaIni.Date},
            New SqlParameter("@Fin", SqlDbType.DateTime) With {.Value = fechaFin.Date},
            New SqlParameter("@Num", SqlDbType.Int) With {.Value = numero},
            New SqlParameter("@Tipo", SqlDbType.NVarChar, 50) With {.Value = tipo}}

        ' Los nombres de columna salen de una lista fija del código, no del usuario.
        For m As Integer = 1 To 12
            If salarios(m) = 0 Then Continue For
            Dim columna As String = Meses(m - 1)
            sets.Add($"{columna} = @M{m}")
            columnas.Add(columna)
            valores.Add($"@M{m}")
            parametros.Add(New SqlParameter($"@M{m}", SqlDbType.Float) With {.Value = salarios(m)})
        Next

        If sets.Count = 0 Then Return

        Dim n As Integer = EjecutarEn(cn, tr,
            "UPDATE HistorialSalarioMes SET NumNomina = @Num, Tipo = @Tipo, " & String.Join(", ", sets) &
            " WHERE CodEmpleado = @Cod AND FechaIni = @Ini AND FechaFin = @Fin", parametros.ToArray())

        If n = 0 Then
            EjecutarEn(cn, tr,
                "INSERT INTO HistorialSalarioMes (CodEmpleado, FechaIni, FechaFin, NumNomina, Tipo, " & String.Join(", ", columnas) & ") " &
                "VALUES (@Cod, @Ini, @Fin, @Num, @Tipo, " & String.Join(", ", valores) & ")",
                parametros.Select(Function(p) New SqlParameter(p.ParameterName, p.SqlDbType) With {.Value = p.Value}).ToArray())
        End If

    End Sub

End Class
