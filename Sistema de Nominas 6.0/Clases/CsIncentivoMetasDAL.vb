Imports System.Data
Imports System.Data.SqlClient

''' <summary>
''' Datos de FrmIncentivoMetas (V.5.0 FrmMetaIncentivo): configuración de puntualidad/séptimo/viáticos
''' (ConfiguracionIncentivo, una sola fila), metas (MetaIncentivo) e incentivo por producción (IncentivoProduccion).
''' </summary>
Public Class CsIncentivoMetasDAL
    Inherits CsDALBase

#Region "CONFIGURACION"

    Public Function ObtenerConfiguracion() As ConfiguracionIncentivo

        Dim dt As DataTable = ConsultarTabla(
            "SELECT TOP 1 HorasPuntualidad, HorasSeptimo, HorasBasico, MontoPuntualidad, MontoViaticos " &
            "FROM ConfiguracionIncentivo ORDER BY Codigo")

        Dim c As New ConfiguracionIncentivo
        If dt.Rows.Count = 0 Then Return c

        Dim r As DataRow = dt.Rows(0)
        c.Horas_Puntualidad = Num(r("HorasPuntualidad"))
        c.Horas_Septimo = Convert.ToDouble(Num(r("HorasSeptimo")))
        c.Horas_Basico = Num(r("HorasBasico"))
        c.Monto_Puntualidad = Num(r("MontoPuntualidad"))
        c.Monto_Viaticos = Convert.ToDouble(Num(r("MontoViaticos")))
        Return c

    End Function

    Private Shared Function Num(o As Object) As Decimal
        Return If(IsDBNull(o), 0D, Convert.ToDecimal(o))
    End Function

    ''' <summary>Actualiza la fila de configuración; si no existe la inserta.</summary>
    Public Sub GuardarConfiguracion(c As ConfiguracionIncentivo)

        Dim ps() As SqlParameter = {
            New SqlParameter("@HP", c.Horas_Puntualidad),
            New SqlParameter("@HS", c.Horas_Septimo),
            New SqlParameter("@HB", c.Horas_Basico),
            New SqlParameter("@MP", SqlDbType.Money) With {.Value = c.Monto_Puntualidad},
            New SqlParameter("@MV", c.Monto_Viaticos)
        }

        EnTransaccion(
            Sub(cn, tr)
                Dim n As Integer = Convert.ToInt32(EscalarEn(cn, tr, "SELECT COUNT(*) FROM ConfiguracionIncentivo"))
                If n = 0 Then
                    EjecutarEn(cn, tr, "INSERT INTO ConfiguracionIncentivo (HorasPuntualidad, HorasSeptimo, HorasBasico, MontoPuntualidad, MontoViaticos) " &
                                       "VALUES (@HP, @HS, @HB, @MP, @MV)", ps)
                Else
                    EjecutarEn(cn, tr, "UPDATE ConfiguracionIncentivo SET HorasPuntualidad = @HP, HorasSeptimo = @HS, HorasBasico = @HB, " &
                                       "MontoPuntualidad = @MP, MontoViaticos = @MV " &
                                       "WHERE Codigo = (SELECT MIN(Codigo) FROM ConfiguracionIncentivo)", ps)
                End If
            End Sub)

    End Sub

#End Region

#Region "METAS"

    Public Function ListarMetas() As DataTable
        Return ConsultarTabla(
            "SELECT CodProceso, CodReferencia, Meta, Rango1, Rango2, Rango3, Rango4, Monto1, Monto2, Monto3, Monto4 " &
            "FROM MetaIncentivo ORDER BY CodProceso, CodReferencia, Meta")
    End Function

    Private Shared Function ParametrosMeta(r As DataRow) As SqlParameter()
        Return {
            New SqlParameter("@Proc", Convert.ToString(r("CodProceso")).Trim),
            New SqlParameter("@Ref", Convert.ToDecimal(r("CodReferencia"))),
            New SqlParameter("@Meta", Convert.ToDouble(r("Meta"))),
            New SqlParameter("@R1", Convert.ToDouble(r("Rango1"))),
            New SqlParameter("@R2", Dato(r("Rango2"))),
            New SqlParameter("@R3", Dato(r("Rango3"))),
            New SqlParameter("@R4", Dato(r("Rango4"))),
            New SqlParameter("@M1", SqlDbType.Money) With {.Value = Dato(r("Monto1"))},
            New SqlParameter("@M2", SqlDbType.Money) With {.Value = Dato(r("Monto2"))},
            New SqlParameter("@M3", SqlDbType.Money) With {.Value = Dato(r("Monto3"))},
            New SqlParameter("@M4", SqlDbType.Money) With {.Value = Dato(r("Monto4"))}
        }
    End Function

    Private Shared Function Dato(o As Object) As Object
        Return If(IsDBNull(o) OrElse o Is Nothing, DBNull.Value, o)
    End Function

    ''' <summary>Revisa las filas (no eliminadas): proceso, referencia, meta y primer rango son obligatorios y la llave no se repite.</summary>
    Public Shared Function ValidarMetas(tabla As DataTable) As String

        Dim vistas As New HashSet(Of String)

        For Each r As DataRow In tabla.Rows
            If r.RowState = DataRowState.Deleted Then Continue For

            If IsDBNull(r("CodProceso")) OrElse Convert.ToString(r("CodProceso")).Trim = "" Then Return "Cada fila necesita el código de proceso."
            If IsDBNull(r("CodReferencia")) Then Return "Cada fila necesita la referencia."
            If IsDBNull(r("Meta")) Then Return "Cada fila necesita la meta."
            If IsDBNull(r("Rango1")) Then Return "Cada fila necesita al menos el Rango1."

            Dim llave As String = Convert.ToString(r("CodProceso")).Trim & "|" & Convert.ToString(r("CodReferencia")) & "|" & Convert.ToString(r("Meta"))
            If Not vistas.Add(llave) Then Return "Hay dos filas con el mismo proceso, referencia y meta (" & llave.Replace("|", " / ") & ")."
        Next

        Return ""

    End Function

    ''' <summary>Aplica en una transacción las altas, cambios y bajas de la tabla de metas (llave: proceso + referencia + meta).</summary>
    Public Sub GuardarMetas(tabla As DataTable)

        Dim cambios As DataTable = tabla.GetChanges()
        If cambios Is Nothing Then Exit Sub

        Try
            GuardarMetasEnTransaccion(cambios)
        Catch ex As SqlException When EsEnUso(ex)
            Throw New Exception("El proceso y la referencia de cada meta deben existir en el catálogo de Procesos (módulo de producción).")
        End Try

    End Sub

    Private Sub GuardarMetasEnTransaccion(cambios As DataTable)

        EnTransaccion(
            Sub(cn, tr)
                For Each r As DataRow In cambios.Rows
                    Select Case r.RowState
                        Case DataRowState.Added
                            EjecutarEn(cn, tr, "INSERT INTO MetaIncentivo (CodProceso, CodReferencia, Meta, Rango1, Rango2, Rango3, Rango4, Monto1, Monto2, Monto3, Monto4) " &
                                               "VALUES (@Proc, @Ref, @Meta, @R1, @R2, @R3, @R4, @M1, @M2, @M3, @M4)", ParametrosMeta(r))
                        Case DataRowState.Modified
                            Dim ps As New List(Of SqlParameter)(ParametrosMeta(r))
                            ps.Add(New SqlParameter("@OProc", Convert.ToString(r("CodProceso", DataRowVersion.Original)).Trim))
                            ps.Add(New SqlParameter("@ORef", Convert.ToDecimal(r("CodReferencia", DataRowVersion.Original))))
                            ps.Add(New SqlParameter("@OMeta", Convert.ToDouble(r("Meta", DataRowVersion.Original))))
                            EjecutarEn(cn, tr, "UPDATE MetaIncentivo SET CodProceso = @Proc, CodReferencia = @Ref, Meta = @Meta, Rango1 = @R1, Rango2 = @R2, " &
                                               "Rango3 = @R3, Rango4 = @R4, Monto1 = @M1, Monto2 = @M2, Monto3 = @M3, Monto4 = @M4 " &
                                               "WHERE CodProceso = @OProc AND CodReferencia = @ORef AND Meta = @OMeta", ps.ToArray())
                        Case DataRowState.Deleted
                            EjecutarEn(cn, tr, "DELETE FROM MetaIncentivo WHERE CodProceso = @OProc AND CodReferencia = @ORef AND Meta = @OMeta",
                                       New SqlParameter("@OProc", Convert.ToString(r("CodProceso", DataRowVersion.Original)).Trim),
                                       New SqlParameter("@ORef", Convert.ToDecimal(r("CodReferencia", DataRowVersion.Original))),
                                       New SqlParameter("@OMeta", Convert.ToDouble(r("Meta", DataRowVersion.Original))))
                    End Select
                Next
            End Sub)

    End Sub

#End Region

#Region "PRODUCCION"

    Public Function ListarDepartamentos() As DataTable
        Return ConsultarTabla("SELECT CodDepartamento, Departamento FROM Departamento ORDER BY CodDepartamento")
    End Function

    Public Function ListarProduccion() As DataTable
        Return ConsultarTabla(
            "SELECT ip.NumeroIncentivo, ip.CodDepartamento, d.Departamento, ip.CantidadHoras, ip.Valor " &
            "FROM IncentivoProduccion ip LEFT JOIN Departamento d ON d.CodDepartamento = ip.CodDepartamento " &
            "ORDER BY ip.CodDepartamento, ip.NumeroIncentivo")
    End Function

    Public Sub AgregarProduccion(codDepartamento As String, cantidadHoras As String, valor As Double)

        If String.IsNullOrWhiteSpace(codDepartamento) Then Throw New InvalidOperationException("Seleccione el departamento.")
        If String.IsNullOrWhiteSpace(cantidadHoras) Then Throw New InvalidOperationException("Escriba la cantidad de horas.")
        If cantidadHoras.Trim.Length > 10 Then Throw New InvalidOperationException("La cantidad de horas admite como máximo 10 caracteres.")

        If Convert.ToInt32(Escalar("SELECT COUNT(*) FROM IncentivoProduccion WHERE CodDepartamento = @D AND CantidadHoras = @H",
                                   New SqlParameter("@D", codDepartamento), New SqlParameter("@H", cantidadHoras.Trim))) > 0 Then
            Throw New InvalidOperationException("Ese departamento ya tiene un incentivo para esa cantidad de horas.")
        End If

        Ejecutar("INSERT INTO IncentivoProduccion (CodDepartamento, CantidadHoras, Valor) VALUES (@D, @H, @V)",
                 New SqlParameter("@D", codDepartamento), New SqlParameter("@H", cantidadHoras.Trim), New SqlParameter("@V", valor))

    End Sub

    Public Sub EliminarProduccion(numeroIncentivo As Integer)
        Ejecutar("DELETE FROM IncentivoProduccion WHERE NumeroIncentivo = @N", New SqlParameter("@N", numeroIncentivo))
    End Sub

#End Region

End Class
