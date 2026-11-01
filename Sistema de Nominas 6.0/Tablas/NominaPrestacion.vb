''' <summary>
''' Cabecera de una nómina de 13vo mes (tabla Nom13Mes) o de vacaciones (NomVaca): las dos tienen las mismas columnas, salvo
''' <see cref="Transferir"/>, que solo existe en vacaciones.
''' </summary>
Public Class NominaPrestacion

    Public Property Numero As Integer
    Public Property Fecha_Aplica As Date = Date.Today
    Public Property Monto_Pagado As Double
    Public Property Fecha_Ini As Date
    Public Property Fecha_Fin As Date
    ''' <summary>Abierta (se puede recalcular); al cerrarla ya no se modifica.</summary>
    Public Property Activa As Boolean = True
    Public Property Cod_Tipo_Nomina As String = ""
    ''' <summary>Vacaciones: "cerrar sin transferir" a la nómina normal.</summary>
    Public Property Transferir As Boolean
    ''' <summary>Falso si es una propuesta (todavía no existe en la base de datos).</summary>
    Public Property Existe As Boolean

End Class
