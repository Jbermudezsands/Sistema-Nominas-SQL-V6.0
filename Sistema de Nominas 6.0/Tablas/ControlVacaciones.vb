''' <summary>Un día marcado en el calendario de vacaciones (fila de DescuentoDiasVacaciones).</summary>
Public Class DiaVacacion

    Public Property Fecha As Date
    ''' <summary>Vacaciones, Vacaciones Pagadas, Vacaciones Programadas, Permiso Programado, Ausente, Feriado, Subsidio...</summary>
    Public Property Tipo As String = ""
    ''' <summary>1 por día; en "Vacaciones Pagadas" es la cantidad pagada (puede ser fracción).</summary>
    Public Property Cant_Dias As Double = 1
    Public Property Numero_Solicitud As String = ""

End Class

''' <summary>Un tramo continuo de una misma solicitud (por ejemplo del 05/03 al 09/03, 5 días de Vacaciones).</summary>
Public Class TramoVacacion

    Public Property Numero_Solicitud As String = ""
    Public Property Tipo As String = ""
    Public Property Inicio As Date
    Public Property Fin As Date
    Public Property Dias As Double

End Class

''' <summary>
''' Control de vacaciones de un empleado (V.5.0 FrmControlVacaciones): saldo oficial a una fecha de corte y los días marcados en un año
''' (vacaciones, programadas, permisos, ausencias, feriados, subsidios). Sin acceso a datos: lo arma <see cref="CsControlVacacionesDAL"/>.
''' </summary>
Public Class ControlVacaciones

    ''' <summary>Tipos que son vacaciones ya tomadas o pagadas (restan del saldo).</summary>
    Public Shared ReadOnly TiposDisfrutados As String() = {"Vacaciones", "Vacaciones Pagadas"}
    ''' <summary>Tipos solicitados que todavía no se toman.</summary>
    Public Shared ReadOnly TiposProgramados As String() = {"Vacaciones Programadas", "Permiso Programado"}

    Public Property Codigo As String = ""
    Public Property Nombre_Completo As String = ""
    Public Property Departamento As String = ""
    Public Property Cargo As String = ""
    Public Property Fecha_Contrato_Vac As Date?
    Public Property Anio As Integer
    Public Property Fecha_Corte As Date
    ''' <summary>Acumulado, solicitado y saldo a <see cref="Fecha_Corte"/>, con el mismo cálculo que las solicitudes de vacaciones.</summary>
    Public Property Resumen As New ResumenVacaciones
    Public ReadOnly Property Dias As New List(Of DiaVacacion)

    ''' <summary>Días del año por tipo, en el orden de la paleta (tipos conocidos primero).</summary>
    Public Function DiasPorTipo() As IList(Of KeyValuePair(Of String, Double))
        Return Dias.GroupBy(Function(d) d.Tipo).
            Select(Function(g) New KeyValuePair(Of String, Double)(g.Key, g.Sum(Function(d) d.Cant_Dias))).
            OrderBy(Function(p) PaletaVacaciones.Orden(p.Key)).ThenBy(Function(p) p.Key).ToList()
    End Function

    Public Function DiasDe(tipo As String) As Double
        Return Dias.Where(Function(d) d.Tipo = tipo).Sum(Function(d) d.Cant_Dias)
    End Function

    ''' <summary>Vacaciones (tomadas y pagadas) del año.</summary>
    Public ReadOnly Property Disfrutados_Anio As Double
        Get
            Return TiposDisfrutados.Sum(Function(t) DiasDe(t))
        End Get
    End Property

    ''' <summary>Vacaciones y permisos programados del año.</summary>
    Public ReadOnly Property Programados_Anio As Double
        Get
            Return TiposProgramados.Sum(Function(t) DiasDe(t))
        End Get
    End Property

    ''' <summary>Disponible después de descontar lo ya programado (saldo oficial menos programados del año).</summary>
    Public ReadOnly Property Disponible_Sin_Programar As Double
        Get
            Return Resumen.Saldo - Programados_Anio
        End Get
    End Property

    ''' <summary>Días del año agrupados por solicitud y tipo, uno por cada tramo de fechas consecutivas.</summary>
    Public Function Tramos() As IList(Of TramoVacacion)

        Dim lista As New List(Of TramoVacacion)

        For Each g In Dias.GroupBy(Function(d) New With {Key .Numero = d.Numero_Solicitud, Key .Tipo = d.Tipo}).OrderBy(Function(x) x.Min(Function(d) d.Fecha))

            Dim actual As TramoVacacion = Nothing

            For Each d As DiaVacacion In g.OrderBy(Function(x) x.Fecha)
                If actual IsNot Nothing AndAlso d.Fecha.Date <= actual.Fin.Date.AddDays(1) Then
                    actual.Fin = d.Fecha.Date
                    actual.Dias += d.Cant_Dias
                Else
                    actual = New TramoVacacion With {.Numero_Solicitud = g.Key.Numero, .Tipo = g.Key.Tipo, .Inicio = d.Fecha.Date, .Fin = d.Fecha.Date, .Dias = d.Cant_Dias}
                    lista.Add(actual)
                End If
            Next

        Next

        Return lista.OrderBy(Function(t) t.Inicio).ThenBy(Function(t) t.Tipo).ToList()

    End Function

End Class
