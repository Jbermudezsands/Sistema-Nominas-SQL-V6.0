Imports System.Data
Imports System.Data.SqlClient

''' <summary>Parámetros del sistema: tablas Controles (Id = 1) y DatosEmpresa (Numero = 1).</summary>
Public Class CsControlesDAL
    Inherits CsDALBase

#Region "CONTROLES"

    Public Function ObtenerControles() As Controles

        Dim dt As DataTable = ConsultarTabla(
            "SELECT TOP 1 DiasMes, DiasSemana, VerificarTasa, SalarioPromedioReal, " &
            "AntiguedadMenor, CalcularRedondeado FROM Controles ORDER BY Id")

        Dim c As New Controles

        If dt.Rows.Count = 0 Then Return c

        Dim r As DataRow = dt.Rows(0)

        c.Dias_Mes = Convert.ToDouble(If(IsDBNull(r("DiasMes")), 30.0, r("DiasMes")))
        c.Dias_Semana = Convert.ToDouble(If(IsDBNull(r("DiasSemana")), 7.0, r("DiasSemana")))
        c.Verificar_Tasa = Bit(r("VerificarTasa"))
        c.Salario_Promedio_Real = Bit(r("SalarioPromedioReal"))
        c.Antiguedad_Menor = Bit(r("AntiguedadMenor"))
        c.Calcular_Redondeado = Bit(r("CalcularRedondeado"))

        Return c

    End Function

    Public Sub GuardarControles(c As Controles)

        Dim p() As SqlParameter = {
            New SqlParameter("@DiasMes", SqlDbType.Real) With {.Value = CSng(c.Dias_Mes)},
            New SqlParameter("@DiasSemana", SqlDbType.Real) With {.Value = CSng(c.Dias_Semana)},
            New SqlParameter("@Tasa", c.Verificar_Tasa),
            New SqlParameter("@Prom", c.Salario_Promedio_Real),
            New SqlParameter("@Antig", c.Antiguedad_Menor),
            New SqlParameter("@Redond", c.Calcular_Redondeado)
        }

        Dim n As Integer = Ejecutar(
            "UPDATE Controles SET DiasMes = @DiasMes, DiasSemana = @DiasSemana, VerificarTasa = @Tasa, " &
            "SalarioPromedioReal = @Prom, AntiguedadMenor = @Antig, CalcularRedondeado = @Redond", p)

        If n = 0 Then
            Ejecutar(
                "INSERT INTO Controles (Id, DiasMes, DiasSemana, VerificarTasa, SalarioPromedioReal, " &
                "AntiguedadMenor, CalcularRedondeado) VALUES (1, @DiasMes, @DiasSemana, @Tasa, @Prom, @Antig, @Redond)",
                p.Select(Function(x) New SqlParameter(x.ParameterName, x.Value)).ToArray())
        End If

    End Sub

#End Region

#Region "DATOS EMPRESA"

    Public Function ObtenerDatosEmpresa() As DatosEmpresa

        Dim dt As DataTable = ConsultarTabla("SELECT TOP 1 * FROM DatosEmpresa ORDER BY Numero")

        Dim d As New DatosEmpresa

        If dt.Rows.Count = 0 Then Return d

        Dim r As DataRow = dt.Rows(0)

        d.Nombre_Empresa = Texto(r("NombreEmpresa"))
        d.Numero_Ruc = Texto(r("NumeroRUC"))
        d.Direccion = Texto(r("Direccion"))
        d.Telefono = Texto(r("Telefono"))
        d.Fax = Texto(r("Fax"))
        d.Email = Texto(r("Email"))
        d.Ruta_Logo = Texto(r("RutaLogo"))
        d.Ruta_Foto = Texto(r("RutaFoto"))
        d.Formato_Colilla = Texto(r("FormatoColilla"))
        d.Formato_Nomina = Texto(r("FormatoNomina"))
        d.Conexion_Sistema_Contable = Texto(r("ConexionSistemaContable"))
        d.Porciento_Subsidio = If(IsDBNull(r("PorcientoSubsidio")), 0.0, Convert.ToDouble(r("PorcientoSubsidio")))
        d.Calcular_7mo = Bit(r("Calcular7mo"))
        d.Restar_3_Dias_Subsidio = Bit(r("Restar3DiasSubsidio"))

        Dim horas As String = Texto(r("HorasExtra"))
        d.Horas_Extra = If(horas = "", "2", horas)

        Dim met As String = Texto(r("MetodoVacaciones"))
        d.Metodo_Vacaciones = If(met = "", "Vacaciones Mensuales", met)

        Dim ir As String = Texto(r("TipoCalculoIR"))
        d.Tipo_Calculo_Ir = If(ir = "", "Calcular IR x 12", ir)

        Return d

    End Function

    Public Sub GuardarDatosEmpresa(d As DatosEmpresa)

        Dim p() As SqlParameter = {
            New SqlParameter("@Nombre", ValorODbNull(d.Nombre_Empresa)),
            New SqlParameter("@Ruc", ValorODbNull(d.Numero_Ruc)),
            New SqlParameter("@Dir", ValorODbNull(d.Direccion)),
            New SqlParameter("@Tel", ValorODbNull(d.Telefono)),
            New SqlParameter("@Fax", ValorODbNull(d.Fax)),
            New SqlParameter("@Email", ValorODbNull(d.Email)),
            New SqlParameter("@Logo", ValorODbNull(d.Ruta_Logo)),
            New SqlParameter("@Foto", ValorODbNull(d.Ruta_Foto)),
            New SqlParameter("@Colilla", ValorODbNull(d.Formato_Colilla)),
            New SqlParameter("@FNomina", ValorODbNull(d.Formato_Nomina)),
            New SqlParameter("@Vaca", ValorODbNull(d.Metodo_Vacaciones)),
            New SqlParameter("@IR", ValorODbNull(d.Tipo_Calculo_Ir)),
            New SqlParameter("@Conta", ValorODbNull(d.Conexion_Sistema_Contable)),
            New SqlParameter("@Subsidio", d.Porciento_Subsidio),
            New SqlParameter("@Horas", ValorODbNull(d.Horas_Extra)),
            New SqlParameter("@Sept", d.Calcular_7mo),
            New SqlParameter("@Restar3", d.Restar_3_Dias_Subsidio)
        }

        Dim n As Integer = Ejecutar(
            "UPDATE DatosEmpresa SET NombreEmpresa = @Nombre, NumeroRUC = @Ruc, Direccion = @Dir, " &
            "Telefono = @Tel, Fax = @Fax, Email = @Email, RutaLogo = @Logo, RutaFoto = @Foto, " &
            "FormatoColilla = @Colilla, FormatoNomina = @FNomina, MetodoVacaciones = @Vaca, " &
            "TipoCalculoIR = @IR, ConexionSistemaContable = @Conta, PorcientoSubsidio = @Subsidio, " &
            "HorasExtra = @Horas, Calcular7mo = @Sept, Restar3DiasSubsidio = @Restar3", p)

        If n = 0 Then
            Ejecutar(
                "INSERT INTO DatosEmpresa (Numero, NombreEmpresa, NumeroRUC, Direccion, Telefono, Fax, Email, " &
                "RutaLogo, RutaFoto, FormatoColilla, FormatoNomina, MetodoVacaciones, TipoCalculoIR, " &
                "ConexionSistemaContable, PorcientoSubsidio, HorasExtra, Calcular7mo, Restar3DiasSubsidio) " &
                "VALUES (1, @Nombre, @Ruc, @Dir, @Tel, @Fax, @Email, @Logo, @Foto, @Colilla, @FNomina, @Vaca, @IR, " &
                "@Conta, @Subsidio, @Horas, @Sept, @Restar3)",
                p.Select(Function(x) New SqlParameter(x.ParameterName, x.Value)).ToArray())
        End If

    End Sub

#End Region

    Private Shared Function Bit(valor As Object) As Boolean
        Return Not IsDBNull(valor) AndAlso Convert.ToBoolean(valor)
    End Function

    Private Shared Function Texto(valor As Object) As String
        Return If(IsDBNull(valor), "", Convert.ToString(valor))
    End Function

End Class
