Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Movimientos de la nómina activa de un empleado (V.5.0 FrmMovimientos): horas extra, comisiones (DetalleComisiones)
''' y otros destajos (DestalleDestajos). Todo se registra contra la nómina activa del tipo de nómina del empleado.
''' </summary>
Public Class CsMovimientoNominaDAL
    Inherits CsDALBase

    Public Class DatosEmpleado
        Public Property CodEmpleado As Decimal
        Public Property CodigoVisible As String
        Public Property Nombres As String
        Public Property Apellidos As String
        Public Property CodTipoNomina As String
        Public Property Nomina As String
        Public Property TipoPago As String
        Public Property SalarioFijo As Boolean
        ''' <summary>Nómina activa de su tipo de nómina; 0 si no hay.</summary>
        Public Property NumNomina As Integer
    End Class

#Region "EMPLEADO"

    Public Function ListarEmpleados() As DataTable
        Return ConsultarTabla(
            "SELECT CodEmpleado1, LTRIM(RTRIM(ISNULL(Nombre1,'') + ' ' + ISNULL(Nombre2,'') + ' ' + ISNULL(Apellido1,'') + ' ' + ISNULL(Apellido2,''))) AS Nombres " &
            "FROM Empleado WHERE Activo = 1 ORDER BY CodEmpleado1")
    End Function

    ''' <summary>Empleado activo por código visible, con su tipo de pago y nómina activa; Nothing si no existe.</summary>
    Public Function BuscarEmpleado(codigoVisible As String) As DatosEmpleado

        Dim dt As DataTable = ConsultarTabla(
            "SELECT e.CodEmpleado, e.CodEmpleado1, e.Nombre1, e.Nombre2, e.Apellido1, e.Apellido2, e.CodTipoNomina, e.SalarioFijo, t.Nomina, t.TipoPago, " &
            "(SELECT TOP 1 n.NumNomina FROM Nomina n WHERE n.Activa = 1 AND n.CodTipoNomina = e.CodTipoNomina ORDER BY n.NumNomina DESC) AS NumNomina " &
            "FROM Empleado e LEFT JOIN TipoNomina t ON t.CodTipoNomina = e.CodTipoNomina WHERE e.CodEmpleado1 = @Cod AND e.Activo = 1",
            New SqlParameter("@Cod", codigoVisible))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)
        Dim fijo As String = Texto(r("SalarioFijo")).ToUpperInvariant()

        Return New DatosEmpleado With {
            .CodEmpleado = Convert.ToDecimal(r("CodEmpleado")),
            .CodigoVisible = Texto(r("CodEmpleado1")),
            .Nombres = (Texto(r("Nombre1")) & " " & Texto(r("Nombre2"))).Trim(),
            .Apellidos = (Texto(r("Apellido1")) & " " & Texto(r("Apellido2"))).Trim(),
            .CodTipoNomina = Texto(r("CodTipoNomina")),
            .Nomina = Texto(r("Nomina")),
            .TipoPago = Texto(r("TipoPago")),
            .SalarioFijo = fijo = "S" OrElse fijo = "TRUE" OrElse fijo = "1",
            .NumNomina = If(IsDBNull(r("NumNomina")), 0, Convert.ToInt32(r("NumNomina")))
        }

    End Function

    Private Shared Function Texto(valor As Object) As String
        Return If(IsDBNull(valor), "", Convert.ToString(valor))
    End Function

#End Region

#Region "HORAS EXTRA"

    Public Function ObtenerHorasExtra(codEmpleado As Decimal, numNomina As Integer) As Double

        Dim r As Object = Escalar("SELECT TOP 1 CantHoras FROM HorasExtras WHERE CodEmpleado = @Emp AND NumNomina = @Nom",
                                  New SqlParameter("@Emp", codEmpleado), New SqlParameter("@Nom", numNomina))

        Return If(r Is Nothing OrElse IsDBNull(r), 0, Convert.ToDouble(r))

    End Function

    ''' <summary>Graba las horas del empleado en la nómina; si ya tenía las reemplaza (y las deja sin pagar). Devuelve True si reemplazó.</summary>
    Public Function GuardarHorasExtra(codEmpleado As Decimal, numNomina As Integer, horas As Double) As Boolean

        Dim reemplazo As Boolean = False

        EnTransaccion(
            Sub(cn, tr)

                Dim p() As SqlParameter = {New SqlParameter("@Emp", codEmpleado), New SqlParameter("@Nom", numNomina), New SqlParameter("@H", horas)}

                Dim existe As Integer = Convert.ToInt32(EscalarEn(cn, tr,
                    "SELECT COUNT(*) FROM HorasExtras WITH (UPDLOCK, HOLDLOCK) WHERE CodEmpleado = @Emp AND NumNomina = @Nom",
                    New SqlParameter("@Emp", codEmpleado), New SqlParameter("@Nom", numNomina)))

                If existe > 0 Then

                    EjecutarEn(cn, tr, "UPDATE HorasExtras SET CantHoras = @H, Pagada = 0 WHERE CodEmpleado = @Emp AND NumNomina = @Nom", p)
                    reemplazo = True

                Else

                    EjecutarEn(cn, tr,
                        "INSERT INTO HorasExtras (Id, CodEmpleado, NumNomina, CantHoras, Pagada) " &
                        "VALUES ((SELECT ISNULL(MAX(Id), 0) + 1 FROM HorasExtras WITH (TABLOCKX)), @Emp, @Nom, @H, 0)", p)

                End If

            End Sub)

        Return reemplazo

    End Function

#End Region

#Region "COMISIONES Y DESTAJOS"

    Private Shared Function Tabla(comision As Boolean) As String
        Return If(comision, "DetalleComisiones", "DestalleDestajos")
    End Function

    Private Shared Function ColTipo(comision As Boolean) As String
        Return If(comision, "CodTipoComision", "CodTipoDestajo")
    End Function

    ''' <summary>Tipos disponibles: Cod, Nombre.</summary>
    Public Function ListarTipos(comision As Boolean) As DataTable
        Return ConsultarTabla(If(comision,
            "SELECT CodTipoComision AS Cod, Comision AS Nombre FROM TipoComision ORDER BY CodTipoComision",
            "SELECT CodTipoDestajo AS Cod, Destajo AS Nombre FROM TipoDestajo ORDER BY CodTipoDestajo"))
    End Function

    ''' <summary>Movimientos del empleado en la nómina: Id, Tipo, Valor (porcentaje o monto), Cantidad, Total = Cantidad x Valor.</summary>
    Public Function ListarDetalle(comision As Boolean, codEmpleado As Decimal, numNomina As Integer) As DataTable

        Dim valor As String = If(comision, "t.Porcentaje", "t.Monto")
        Dim nombre As String = If(comision, "t.Comision", "t.Destajo")

        Return ConsultarTabla(
            "SELECT d.Id, " & nombre & " AS Tipo, " & valor & " AS Valor, d.Cantidad, d.Cantidad * " & valor & " AS Total " &
            "FROM " & Tabla(comision) & " d INNER JOIN " & If(comision, "TipoComision", "TipoDestajo") & " t ON t." & ColTipo(comision) & " = d." & ColTipo(comision) & " " &
            "WHERE d.CodEmpleado = @Emp AND d.NUmNomina = @Nom ORDER BY d.Id",
            New SqlParameter("@Emp", codEmpleado), New SqlParameter("@Nom", numNomina))

    End Function

    ''' <summary>Agrega el movimiento; si el empleado ya tiene ese tipo en la nómina reemplaza la cantidad. Devuelve True si reemplazó.</summary>
    Public Function GuardarDetalle(comision As Boolean, codEmpleado As Decimal, numNomina As Integer, codTipo As String, cantidad As Double) As Boolean

        Dim reemplazo As Boolean = False

        EnTransaccion(
            Sub(cn, tr)

                Dim existe As Object = EscalarEn(cn, tr,
                    "SELECT TOP 1 Id FROM " & Tabla(comision) & " WITH (UPDLOCK, HOLDLOCK) WHERE CodEmpleado = @Emp AND NUmNomina = @Nom AND " & ColTipo(comision) & " = @Tipo",
                    New SqlParameter("@Emp", codEmpleado), New SqlParameter("@Nom", numNomina), New SqlParameter("@Tipo", codTipo))

                If existe IsNot Nothing AndAlso Not IsDBNull(existe) Then

                    EjecutarEn(cn, tr, "UPDATE " & Tabla(comision) & " SET Cantidad = @C WHERE Id = @Id",
                               New SqlParameter("@C", cantidad), New SqlParameter("@Id", Convert.ToInt32(existe)))
                    reemplazo = True

                Else

                    EjecutarEn(cn, tr,
                        "INSERT INTO " & Tabla(comision) & " (Id, CodEmpleado, " & ColTipo(comision) & ", Cantidad, NUmNomina) " &
                        "VALUES ((SELECT ISNULL(MAX(Id), 0) + 1 FROM " & Tabla(comision) & " WITH (TABLOCKX)), @Emp, @Tipo, @C, @Nom)",
                        New SqlParameter("@Emp", codEmpleado), New SqlParameter("@Tipo", codTipo),
                        New SqlParameter("@C", cantidad), New SqlParameter("@Nom", numNomina))

                End If

            End Sub)

        Return reemplazo

    End Function

    Public Sub EliminarDetalle(comision As Boolean, id As Integer)
        Ejecutar("DELETE FROM " & Tabla(comision) & " WHERE Id = @Id", New SqlParameter("@Id", id))
    End Sub

    ''' <summary>Deja el total en la ficha del empleado: PorcentajeComision (comisiones) o TarifaHoraria (destajos).</summary>
    Public Sub AplicarAlEmpleado(comision As Boolean, codEmpleado As Decimal, total As Double)
        Ejecutar("UPDATE Empleado SET " & If(comision, "PorcentajeComision", "TarifaHoraria") & " = @T WHERE CodEmpleado = @Emp",
                 New SqlParameter("@T", total), New SqlParameter("@Emp", codEmpleado))
    End Sub

#End Region

End Class
