using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.UI.Controls;
using Syncfusion.UI.Xaml.DataGrid;

namespace BlOrders2023.Helpers;
public static class DatagridCellCopy
{
    public static void CopyGridCellContent(object sender, Syncfusion.UI.Xaml.Grids.GridCopyPasteCellEventArgs e)
    {
        SfDataGrid grid = e.OriginalSender is DetailsViewDataGrid ? (SfDataGrid)e.OriginalSender : (SfDataGrid)sender;
        if (grid != null && grid.SelectionController != null
            && grid.SelectionController.CurrentCellManager != null
            && grid.SelectionController.CurrentCellManager.CurrentCell != null
            && e.Column.MappingName != grid.SelectionController.CurrentCellManager.CurrentCell.GridColumn.MappingName)
        {
            e.Handled = true;
        }
    }

    public static void CopyGridCellContent(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridRowClipboardEventArgs e)
    {
        DataGrid? grid = sender as DataGrid;
        if(grid != null){
            var columnsToKeep = e.ClipboardRowContent.Where(i => i.Column == grid.CurrentColumn).ToList();
            e.ClipboardRowContent.Clear();
            foreach (var column in columnsToKeep)
            {
                e.ClipboardRowContent.Add(column);
            }
        }
    }
}
