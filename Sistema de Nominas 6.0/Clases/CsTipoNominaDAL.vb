Imports System.Data
Imports System.Data.SqlClient

Public Class CsTipoNominaDAL
    Inherits CsDALBase

    Private Const Columnas As String =
        "CodTipoNomina, Nomina, Periodo, TipoPago, Moneda, MantValor, PorcientoInss, TasaInss, TasaInssPatronal, " &
        "PorcientoIr, TasaIr, TarifaHoraria, Horas, CalcularHoraTrabajada, IrUltimaSemana, EmpleadoConstruccion, MostraEmpleadoCero"

    Public Function Listar() As DataTable
        Return ConsultarTabla("SELECT CodTipoNomina, Nomina, Periodo FROM TipoNomina ORDER BY CodTipoNomina")
    End Function

    Public Function ProximoCodigo() As String
        Return SiguienteCodigo("TipoNomina", "CodTipoNomina", 2)
    End Function

    Public Function Existe(codigo As String) As Boolean
        Return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM TipoNomina WHERE CodTipoNomina = @Cod",
                                       New SqlParameter("@Cod", codigo))) > 0
    End Function

    Public Function Buscar(codigo As String) As TipoNomina

        Dim dt As DataTable = ConsultarTabla(
            "SELECT " & Columnas & " FROM TipoNomina WHERE CodTipoNomina = @Cod",
            New SqlParameter("@Cod", codigo))

        If dt.Rows.Count = 0 Then Return Nothing

        Dim r As DataRow = dt.Rows(0)

        Return New TipoNomina With {
            .Cod_Tipo_Nomina = Convert.ToString(r("CodTipoNomina")),
            .Descripcion_Nomina = Texto(r("Nomina")),
            .Periodo = Texto(r("Periodo")),
            .Tipo_Pago = Texto(r("TipoPago")),
            .Moneda = Texto(r("Moneda")),
            .Mantenimiento_Valor = Bit(r("MantValor")),
            .Inss_Por_Porcentaje = Bit(r("PorcientoInss")),
            .Tasa_Inss = Numero(r("TasaInss")),
            .Tasa_Inss_Patronal = Numero(r("TasaInssPatronal")),
            .Ir_Por_Porcentaje = Bit(r("PorcientoIr")),
            .Tasa_Ir = Numero(r("TasaIr")),
            .Tarifa_Horaria = Convert.ToDouble(Numero(r("TarifaHoraria"))),
            .Horas = Convert.ToDouble(Numero(r("Horas"))),
            .Calcular_Hora_Trabajada = Bit(r("CalcularHoraTrabajada")),
            .Ir_Ultima_Semana = Bit(r("IrUltimaSemana")),
            .Empleado_Construccion = Bit(r("EmpleadoConstruccion")),
            .Mostrar_Empleado_Cero = Bit(r("MostraEmpleadoCero"))
        }

    End Function

    Public Sub Guardar(t As TipoNomina, esNuevo As Boolean)

        Dim cod As String = t.Cod_Tipo_Nomina.Trim

        Dim p() As SqlParameter = {
            New SqlParameter("@Cod", cod),
            New SqlParameter("@Nomina", t.Descripcion_Nomina.Trim),
            New SqlParameter("@Periodo", t.Periodo),
            New SqlParameter("@TipoPago", t.Tipo_Pago),
            New SqlParameter("@Moneda", t.Moneda),
            New SqlParameter("@Mant", t.Mantenimiento_Valor),
            New SqlParameter("@PInss", t.Inss_Por_Porcentaje),
            New SqlParameter("@TInss", SqlDbType.Decimal) With {.Precision = 9, .Scale = 4, .Value = If(t.Inss_Por_Porcentaje, t.Tasa_Inss, 0D)},
            New SqlParameter("@TInssPat", SqlDbType.Decimal) With {.Precision = 9, .Scale = 4, .Value = t.Tasa_Inss_Patronal},
            New SqlParameter("@PIr", t.Ir_Por_Porcentaje),
            New SqlParameter("@TIr", SqlDbType.Decimal) With {.Precision = 9, .Scale = 4, .Value = If(t.Ir_Por_Porcentaje, t.Tasa_Ir, 0D)},
            New SqlParameter("@Tarifa", t.Tarifa_Horaria),
            New SqlParameter("@Horas", t.Horas),
            New SqlParameter("@HoraTrab", t.Calcular_Hora_Trabajada),
            New SqlParameter("@IrUlt", t.Ir_Ultima_Semana),
            New SqlParameter("@Constr", t.Empleado_Construccion),
            New SqlParameter("@Cero", t.Mostrar_Empleado_Cero)
        }

        If esNuevo Then

            If Existe(cod) Then
                Throw New Exception("Ya existe un tipo de nómina con el código " & cod & ".")
            End If

            ' Activa = 0 y horario de entrada/salida = ahora, igual que en la V.5.0.
            Ejecutar(
                "INSERT INTO TipoNomina (CodTipoNomina, Nomina, Periodo, TipoPago, Moneda, MantValor, PorcientoInss, " &
                "TasaInss, TasaInssPatronal, PorcientoIr, TasaIr, TarifaHoraria, Horas, CalcularHoraTrabajada, " &
                "IrUltimaSemana, EmpleadoConstruccion, MostraEmpleadoCero, Activa, HEntrada, HSalida) " &
                "VALUES (@Cod, @Nomina, @Periodo, @TipoPago, @Moneda, @Mant, @PInss, @TInss, @TInssPat, @PIr, @TIr, " &
                "@Tarifa, @Horas, @HoraTrab, @IrUlt, @Constr, @Cero, 0, GETDATE(), GETDATE())", p)

        Else

            ' Con el INSS por tabla no se toca el % patronal (como en la V.5.0).
            Dim patronal As String = If(t.Inss_Por_Porcentaje, ", TasaInssPatronal = @TInssPat", "")

            Ejecutar(
                "UPDATE TipoNomina SET Nomina = @Nomina, Periodo = @Periodo, TipoPago = @TipoPago, Moneda = @Moneda, " &
                "MantValor = @Mant, PorcientoInss = @PInss, TasaInss = @TInss" & patronal & ", PorcientoIr = @PIr, " &
                "TasaIr = @TIr, TarifaHoraria = @Tarifa, Horas = @Horas, CalcularHoraTrabajada = @HoraTrab, " &
                "IrUltimaSemana = @IrUlt, EmpleadoConstruccion = @Constr, MostraEmpleadoCero = @Cero " &
                "WHERE CodTipoNomina = @Cod", p)

        End If

    End Sub

    Public Sub Eliminar(codigo As String)

        Try
            Ejecutar("DELETE FROM TipoNomina WHERE CodTipoNomina = @Cod", New SqlParameter("@Cod", codigo))
        Catch ex As SqlException When EsEnUso(ex)
            Throw New Exception("No se puede eliminar el tipo de nómina porque tiene empleados, nóminas u otros registros asociados.")
        End Try

    End Sub

    Private Shared Function Bit(valor As Object) As Boolean
        Return Not IsDBNull(valor) AndAlso Convert.ToBoolean(valor)
    End Function

    Private Shared Function Texto(valor As Object) As String
        Return If(IsDBNull(valor), "", Convert.ToString(valor))
    End Function

    Private Shared Function Numero(valor As Object) As Decimal
        Return If(IsDBNull(valor), 0D, Convert.ToDecimal(valor))
    End Function

End Class
