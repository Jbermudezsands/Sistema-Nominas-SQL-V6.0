Imports System.Data

''' <summary>
''' Datos de <see cref="FrmConsulta"/>: ejecuta la consulta de una <see cref="DefinicionBusqueda"/>.
''' El filtrado lo hace el grid en memoria (como el FilterBar del TDBGrid de la V.5.0), no hay SQL dinámico.
''' </summary>
Public Class CsBuscadorDAL
    Inherits CsDALBase

    Public Function Listar(definicion As DefinicionBusqueda) As DataTable
        Return ConsultarTabla(definicion.Sql)
    End Function

    Public Function Listar(tipo As TipoBusqueda) As DataTable
        Return Listar(CatalogoBusquedas.Obtener(tipo))
    End Function

End Class
