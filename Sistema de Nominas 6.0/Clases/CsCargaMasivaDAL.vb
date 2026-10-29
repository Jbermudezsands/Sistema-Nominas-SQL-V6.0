Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Carga masiva de una deducción o un incentivo (mismo monto) a todos los empleados activos de un tipo de
''' nómina, dentro de la nómina activa de ese tipo. Cada carga es una sola transacción: o entran todos o ninguno.
''' </summary>
Public Class CsCargaMasivaDAL
    Inherits CsDALBase

    ''' <summary>Avance de una carga: (procesados, total, texto del empleado actual).</summary>
    Public Delegate Sub AvanceCarga(procesados As Integer, total As Integer, empleado As String)

    ''' <summary>Tipos de nómina que tienen una nómina activa, con su número y fechas.</summary>
    Public Function ListarNominasActivas() As DataTable
        Return ConsultarTabla(
            "SELECT t.CodTipoNomina, t.Nomina, n.NumNomina, n.FechaNominaINI, n.FechaNomina " &
            "FROM TipoNomina t INNER JOIN Nomina n ON n.CodTipoNomina = t.CodTipoNomina " &
            "WHERE n.Activa = 1 ORDER BY t.CodTipoNomina")
    End Function

    Public Function ListarTiposDeduccion() As DataTable
        Return ConsultarTabla("SELECT CodTipoDeduccion, Deduccion FROM TipoDeduccion ORDER BY CodTipoDeduccion")
    End Function

    Public Function ListarTiposIncentivo() As DataTable
        Return ConsultarTabla("SELECT CodTipoIncentivo, Incentivo FROM TipoIncentivo ORDER BY CodTipoIncentivo")
    End Function

    ''' <summary>Número de nómina activa del tipo; 0 si no hay.</summary>
    Public Function NominaActivaDelTipo(codTipoNomina As String) As Integer

        Dim r As Object = Escalar("SELECT TOP 1 NumNomina FROM Nomina WHERE Activa = 1 AND CodTipoNomina = @T ORDER BY NumNomina DESC",
                                  New SqlParameter("@T", codTipoNomina))

        Return If(r Is Nothing OrElse IsDBNull(r), 0, Convert.ToInt32(r))

    End Function

    Public Function ContarEmpleados(codTipoNomina As String) As Integer
        Return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Empleado WHERE CodTipoNomina = @T AND Activo = 1",
                                       New SqlParameter("@T", codTipoNomina)))
    End Function

    ''' <summary>Agrega la deducción (una vez, sin pagar) a cada empleado activo del tipo de nómina. Devuelve cuántos recibieron.</summary>
    Public Function CargarDeduccion(codTipoNomina As String, codTipoDeduccion As String, monto As Double,
                                    Optional avance As AvanceCarga = Nothing) As Integer

        Validar(codTipoNomina, monto)

        Dim numNomina As Integer = NominaActivaDelTipo(codTipoNomina)
        Dim cargados As Integer = 0

        If numNomina = 0 Then
            Throw New InvalidOperationException("El tipo de nómina no tiene una nómina activa.")
        End If

        Dim empleados As DataTable = ListarEmpleadosDelTipo(codTipoNomina)

        EnTransaccion(
            Sub(cn, tr)

                ' Igual que CsMovimientosDAL: el número sale del máximo de la cabecera (Consecutivos.Deducciones puede ir atrasado).
                Dim num As Integer = Convert.ToInt32(EscalarEn(cn, tr,
                    "SELECT ISNULL(MAX(NumDeduccion), 0) + 1 FROM Deduccion WITH (TABLOCKX)")) - 1

                For i As Integer = 0 To empleados.Rows.Count - 1

                    Dim e As DataRow = empleados.Rows(i)

                    If avance IsNot Nothing Then avance(i, empleados.Rows.Count, Descripcion(e))

                    num += 1

                    EjecutarEn(cn, tr,
                        "INSERT INTO Deduccion (NumDeduccion, CodEmpleado, CodTipoDeduccion, NumVeces, Pagado, NUmNomina) " &
                        "VALUES (@Num, @Emp, @Tipo, '1', 0, @Nom)",
                        New SqlParameter("@Num", num),
                        New SqlParameter("@Emp", SqlDbType.Decimal) With {.Value = e("CodEmpleado")},
                        New SqlParameter("@Tipo", codTipoDeduccion),
                        New SqlParameter("@Nom", numNomina))

                    EjecutarEn(cn, tr,
                        "INSERT INTO DetalleDeduccion (NumDeduccion, Valor, NumVez, Pagado, NumNomina) VALUES (@Num, @Valor, '1', 0, @Nom)",
                        New SqlParameter("@Num", num),
                        New SqlParameter("@Valor", monto),
                        New SqlParameter("@Nom", numNomina))

                    cargados += 1

                Next

            End Sub)

        If avance IsNot Nothing Then avance(empleados.Rows.Count, empleados.Rows.Count, "")

        Return cargados

    End Function

    ''' <summary>Agrega el incentivo (una vez, sin pagar) a cada empleado activo del tipo de nómina. Devuelve cuántos recibieron.</summary>
    Public Function CargarIncentivo(codTipoNomina As String, codTipoIncentivo As String, monto As Double,
                                    Optional avance As AvanceCarga = Nothing) As Integer

        Validar(codTipoNomina, monto)

        Dim numNomina As Integer = NominaActivaDelTipo(codTipoNomina)
        Dim cargados As Integer = 0

        If numNomina = 0 Then
            Throw New InvalidOperationException("El tipo de nómina no tiene una nómina activa.")
        End If

        Dim empleados As DataTable = ListarEmpleadosDelTipo(codTipoNomina)

        EnTransaccion(
            Sub(cn, tr)

                ' El número no es identidad: se calcula una vez bajo bloqueo y se incrementa en memoria.
                Dim numIncentivo As Integer = Convert.ToInt32(EscalarEn(cn, tr,
                    "SELECT ISNULL(MAX(NumIncentivo), 0) + 1 FROM Incentivo WITH (TABLOCKX)"))

                For i As Integer = 0 To empleados.Rows.Count - 1

                    Dim e As DataRow = empleados.Rows(i)

                    If avance IsNot Nothing Then avance(i, empleados.Rows.Count, Descripcion(e))

                    EjecutarEn(cn, tr,
                        "INSERT INTO Incentivo (NumIncentivo, CodEmpleado, CodTipoIncentivo, NumVeces, Pagado) VALUES (@Num, @Emp, @Tipo, '1', 0)",
                        New SqlParameter("@Num", numIncentivo),
                        New SqlParameter("@Emp", SqlDbType.Decimal) With {.Value = e("CodEmpleado")},
                        New SqlParameter("@Tipo", codTipoIncentivo))

                    ' PK (Id, NumIncentivo): Id es el número de vez, aquí siempre la primera.
                    EjecutarEn(cn, tr,
                        "INSERT INTO DetalleIncentivo (Id, NumIncentivo, Valor, NumVez, Pagado, NumNomina) VALUES (1, @Num, @Valor, '1', 0, @Nom)",
                        New SqlParameter("@Num", numIncentivo),
                        New SqlParameter("@Valor", monto),
                        New SqlParameter("@Nom", numNomina))

                    numIncentivo += 1
                    cargados += 1

                Next

            End Sub)

        If avance IsNot Nothing Then avance(empleados.Rows.Count, empleados.Rows.Count, "")

        Return cargados

    End Function

    Private Function ListarEmpleadosDelTipo(codTipoNomina As String) As DataTable
        Return ConsultarTabla(
            "SELECT CodEmpleado, CodEmpleado1, Nombre1 FROM Empleado WHERE CodTipoNomina = @T AND Activo = 1 ORDER BY CodEmpleado1",
            New SqlParameter("@T", codTipoNomina))
    End Function

    Private Shared Function Descripcion(e As DataRow) As String
        Return Convert.ToString(e("CodEmpleado1")) & " " & Convert.ToString(e("Nombre1"))
    End Function

    Private Shared Sub Validar(codTipoNomina As String, monto As Double)

        If String.IsNullOrWhiteSpace(codTipoNomina) Then
            Throw New InvalidOperationException("Seleccione el tipo de nómina.")
        End If

        If monto <= 0 Then
            Throw New InvalidOperationException("Digite el monto a procesar (mayor que cero).")
        End If

    End Sub

End Class
