Imports System.Data
Imports System.Data.SqlClient

Public Enum TipoMovimiento
    Incentivo
    Deduccion
    Subsidio
End Enum

''' <summary>
''' Incentivos, deducciones y subsidios de un empleado (V.5.0 pestañas Incentivos / Deducciones / Subsidios).
''' Las tres comparten la forma cabecera + detalle por vez; el nombre de tablas y columnas sale de
''' constantes de esta clase, nunca de la entrada del usuario.
''' </summary>
Public Class CsMovimientosDAL
    Inherits CsDALBase

    Private ReadOnly _tipo As TipoMovimiento

    Public Sub New(tipo As TipoMovimiento)
        _tipo = tipo
    End Sub

    Public ReadOnly Property RequiereNominaActiva As Boolean
        Get
            Return _tipo <> TipoMovimiento.Subsidio
        End Get
    End Property

#Region "NOMBRES DE TABLAS"

    Private ReadOnly Property Cabecera As String
        Get
            Select Case _tipo
                Case TipoMovimiento.Incentivo : Return "Incentivo"
                Case TipoMovimiento.Deduccion : Return "Deduccion"
                Case Else : Return "Subsidio"
            End Select
        End Get
    End Property

    Private ReadOnly Property Detalle As String
        Get
            Return "Detalle" & Cabecera
        End Get
    End Property

    Private ReadOnly Property Catalogo As String
        Get
            Return "Tipo" & Cabecera
        End Get
    End Property

    ''' <summary>Llave de la cabecera: NumIncentivo / NumDeduccion / NumSubsidio.</summary>
    Private ReadOnly Property ColNum As String
        Get
            Return "Num" & Cabecera
        End Get
    End Property

    Private ReadOnly Property ColCodTipo As String
        Get
            Return "CodTipo" & Cabecera
        End Get
    End Property

#End Region

#Region "CONSULTAS"

    Public Function ListarTipos() As DataTable
        Return ConsultarTabla("SELECT " & ColCodTipo & " AS Codigo, " & Cabecera & " AS Descripcion FROM " &
                              Catalogo & " ORDER BY " & Cabecera)
    End Function

    ''' <summary>Detalles pendientes de pago del empleado (los pagados ya no se muestran).</summary>
    Public Function ListarPendientes(codEmpleado As Double) As DataTable

        Dim extra As String = If(_tipo = TipoMovimiento.Subsidio, ", d.Descripcion", "")

        Return ConsultarTabla(
            "SELECT d.Id, c." & ColNum & " AS Numero, t." & Cabecera & " AS Tipo" & extra &
            ", d.Valor, d.NumVez " &
            "FROM " & Cabecera & " c " &
            "INNER JOIN " & Detalle & " d ON d." & ColNum & " = c." & ColNum & " " &
            "INNER JOIN " & Catalogo & " t ON t." & ColCodTipo & " = c." & ColCodTipo & " " &
            "WHERE c.CodEmpleado = @Emp AND d.Pagado = 0 ORDER BY c." & ColNum & ", d.Id",
            New SqlParameter("@Emp", codEmpleado))

    End Function

    ''' <summary>NumNomina de la nómina activa (abierta y no anulada) del tipo de nómina del empleado; 0 si no hay.</summary>
    Public Function NominaActiva(codEmpleado As Double) As Integer

        Dim r As Object = Escalar(
            "SELECT TOP 1 n.NumNomina FROM Nomina n INNER JOIN Empleado e ON e.CodTipoNomina = n.CodTipoNomina " &
            "WHERE e.CodEmpleado = @Emp AND n.Activa = 1 AND n.Cerrada = 0 AND n.Anulada = 0 " &
            "ORDER BY n.NumNomina DESC",
            New SqlParameter("@Emp", codEmpleado))

        Return If(IsDBNull(r) OrElse r Is Nothing, 0, Convert.ToInt32(r))

    End Function

#End Region

#Region "AGREGAR"

    ''' <summary>"n" = indefinido; si no, un entero entre 1 y 9999 (NumVeces es nvarchar(4)). Devuelve el texto normalizado.</summary>
    Public Shared Function NormalizarVeces(texto As String) As String

        Dim t As String = If(texto, "").Trim

        If t.Equals("n", StringComparison.OrdinalIgnoreCase) Then Return "n"

        Dim v As Integer

        If Integer.TryParse(t, v) AndAlso v >= 1 AndAlso v <= 9999 Then Return v.ToString()

        Throw New Exception("El número de veces digitado es erróneo (use un número o ""n"" para indefinido).")

    End Function

    ''' <param name="numVeces">Texto ya normalizado con <see cref="NormalizarVeces"/>.</param>
    ''' <param name="numNomina">Nómina activa (incentivos y deducciones); ignorado en subsidios.</param>
    ''' <param name="descripcion">Solo subsidios (máx. 25).</param>
    Public Sub Agregar(codEmpleado As Double, codTipo As String, numVeces As String,
                       monto As Double, descripcion As String, numNomina As Integer)

        Dim indefinido As Boolean = (numVeces = "n")
        Dim veces As Integer = If(indefinido, 1, Integer.Parse(numVeces))

        EnTransaccion(
            Sub(cn, tr)

                Dim num As Integer = Convert.ToInt32(EscalarEn(cn, tr,
                    "SELECT ISNULL(MAX(" & ColNum & "), 0) + 1 FROM " & Cabecera & " WITH (UPDLOCK, HOLDLOCK)"))

                ' Cabecera
                If _tipo = TipoMovimiento.Deduccion Then
                    EjecutarEn(cn, tr,
                        "INSERT INTO Deduccion (NumDeduccion, CodEmpleado, CodTipoDeduccion, NumVeces, Pagado, NUmNomina) " &
                        "VALUES (@Num, @Emp, @Tipo, @Veces, 0, @Nom)",
                        New SqlParameter("@Num", num), New SqlParameter("@Emp", codEmpleado),
                        New SqlParameter("@Tipo", codTipo), New SqlParameter("@Veces", numVeces),
                        New SqlParameter("@Nom", numNomina))
                Else
                    EjecutarEn(cn, tr,
                        "INSERT INTO " & Cabecera & " (" & ColNum & ", CodEmpleado, " & ColCodTipo & ", NumVeces, Pagado) " &
                        "VALUES (@Num, @Emp, @Tipo, @Veces, 0)",
                        New SqlParameter("@Num", num), New SqlParameter("@Emp", codEmpleado),
                        New SqlParameter("@Tipo", codTipo), New SqlParameter("@Veces", numVeces))
                End If

                ' Detalle
                Select Case _tipo

                    Case TipoMovimiento.Incentivo
                        ' PK (Id, NumIncentivo): Id = número de vez.
                        For i As Integer = 1 To veces
                            EjecutarEn(cn, tr,
                                "INSERT INTO DetalleIncentivo (Id, NumIncentivo, Valor, NumVez, Pagado, NumNomina) " &
                                "VALUES (@Id, @Num, @Valor, @Vez, 0, @Nom)",
                                New SqlParameter("@Id", i), New SqlParameter("@Num", num),
                                New SqlParameter("@Valor", monto),
                                New SqlParameter("@Vez", If(indefinido, "n", i.ToString())),
                                New SqlParameter("@Nom", If(i = 1, numNomina, 0)))
                        Next

                    Case TipoMovimiento.Deduccion
                        ' Indefinida = una sola fila con NumVez 9999. Id es identity.
                        For i As Integer = 1 To veces
                            EjecutarEn(cn, tr,
                                "INSERT INTO DetalleDeduccion (NumDeduccion, Valor, NumVez, Pagado, NumNomina) " &
                                "VALUES (@Num, @Valor, @Vez, 0, @Nom)",
                                New SqlParameter("@Num", num), New SqlParameter("@Valor", monto),
                                New SqlParameter("@Vez", If(indefinido, "9999", i.ToString())),
                                New SqlParameter("@Nom", If(i = 1, numNomina, 0)))
                        Next

                    Case Else
                        ' Subsidio: una sola fila; NumVez conserva lo digitado ("n" o el número).
                        Dim id As Integer = Convert.ToInt32(EscalarEn(cn, tr,
                            "SELECT ISNULL(MAX(Id), 0) + 1 FROM DetalleSubsidio WITH (UPDLOCK, HOLDLOCK)"))

                        EjecutarEn(cn, tr,
                            "INSERT INTO DetalleSubsidio (Id, NumSubsidio, Valor, NumVez, Pagado, Descripcion) " &
                            "VALUES (@Id, @Num, @Valor, @Vez, 0, @Desc)",
                            New SqlParameter("@Id", id), New SqlParameter("@Num", num),
                            New SqlParameter("@Valor", monto), New SqlParameter("@Vez", numVeces),
                            New SqlParameter("@Desc", If(String.IsNullOrWhiteSpace(descripcion), " ", descripcion.Trim)))

                End Select

            End Sub)

    End Sub

#End Region

#Region "ELIMINAR / ANULAR"

    ''' <summary>Borra el movimiento completo (cabecera y todas sus veces; el detalle cae en cascada).</summary>
    Public Sub Eliminar(numero As Integer)
        Ejecutar("DELETE FROM " & Cabecera & " WHERE " & ColNum & " = @Num", New SqlParameter("@Num", numero))
    End Sub

    ''' <summary>Solo incentivos (V.5.0 CmdAnular): deja en 0 el valor de esa vez sin borrarla.</summary>
    Public Sub Anular(idDetalle As Integer, numero As Integer)

        If _tipo <> TipoMovimiento.Incentivo Then
            Throw New InvalidOperationException("Solo los incentivos se pueden anular.")
        End If

        Ejecutar("UPDATE DetalleIncentivo SET Valor = 0 WHERE Id = @Id AND NumIncentivo = @Num",
                 New SqlParameter("@Id", idDetalle), New SqlParameter("@Num", numero))

    End Sub

#End Region


End Class
