Imports DevExpress.XtraGrid.Views.Grid

''' <summary>Exporta a Excel lo que muestra un GridView (con sus filtros y orden). Reemplaza los bucles con Excel.Application de la V.5.0.</summary>
Public Class ExportadorGrid

    ''' <summary>Pide el archivo destino y exporta la vista a .xlsx. Devuelve la ruta, o "" si el usuario canceló.</summary>
    Public Shared Function ExportarAExcel(vista As GridView, nombreSugerido As String) As String

        Using dlg As New SaveFileDialog With {
            .Filter = "Libro de Excel (*.xlsx)|*.xlsx",
            .FileName = nombreSugerido,
            .Title = "Exportar a Excel"
        }

            If dlg.ShowDialog() <> DialogResult.OK Then Return ""

            vista.ExportToXlsx(dlg.FileName)

            Return dlg.FileName

        End Using

    End Function

    ''' <summary>
    ''' Pide el archivo destino y guarda una tabla como .xlsx sin fila de encabezados (archivos de banco). Devuelve la ruta, o ""
    ''' si el usuario canceló.
    ''' </summary>
    Public Shared Function ExportarTabla(tabla As DataTable, nombreSugerido As String) As String

        Using dlg As New SaveFileDialog With {
            .Filter = "Libro de Excel (*.xlsx)|*.xlsx",
            .FileName = nombreSugerido,
            .Title = "Exportar a Excel"
        }

            If dlg.ShowDialog() <> DialogResult.OK Then Return ""

            Using grid As New DevExpress.XtraGrid.GridControl
                Dim vista As New GridView(grid)
                grid.MainView = vista
                grid.ViewCollection.Add(vista)
                grid.DataSource = tabla
                vista.PopulateColumns()
                vista.OptionsPrint.PrintHeader = False
                vista.OptionsPrint.AutoWidth = False
                For Each c As DevExpress.XtraGrid.Columns.GridColumn In vista.Columns
                    c.BestFit()
                Next
                vista.ExportToXlsx(dlg.FileName)
            End Using

            Return dlg.FileName

        End Using

    End Function

End Class
