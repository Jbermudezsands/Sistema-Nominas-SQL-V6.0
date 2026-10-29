''' <summary>
''' Colores de los listados de la V.5.0 (TDBGrid): filas pares azul claro, impares blancas, barra de botones lavanda.
''' </summary>
Public Class EstiloGrid

    Public Shared ReadOnly ColorFilaPar As Color = Color.FromArgb(216, 228, 248)
    Public Shared ReadOnly ColorBarra As Color = Color.FromArgb(222, 227, 247)

    Public Shared Sub Aplicar(vista As DevExpress.XtraGrid.Views.Grid.GridView)

        With vista
            .OptionsView.EnableAppearanceEvenRow = True
            .OptionsView.EnableAppearanceOddRow = True
            .Appearance.EvenRow.BackColor = ColorFilaPar
            .Appearance.EvenRow.Options.UseBackColor = True
            .Appearance.OddRow.BackColor = Color.White
            .Appearance.OddRow.Options.UseBackColor = True
        End With

    End Sub

    Public Shared Sub AplicarBarra(panel As DevExpress.XtraEditors.PanelControl)
        panel.Appearance.BackColor = ColorBarra
        panel.Appearance.Options.UseBackColor = True
    End Sub

End Class
