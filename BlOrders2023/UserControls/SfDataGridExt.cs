using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Syncfusion.UI.Xaml.DataGrid;
using Syncfusion.UI.Xaml.DataGrid.Renderers;
using Syncfusion.UI.Xaml.Editors;
using Syncfusion.UI.Xaml.Grids;
using Windows.System;

namespace BlOrders2023.UserControls;

public class GridCellNumericRendererExt : GridCellTextBoxRenderer
{
    //ShouldGridTryToHandleKeyDown () is responsible for all key navigation associated with GridNumericColumn.
    protected override bool ShouldGridTryToHandleKeyDown(KeyRoutedEventArgs e)
    {
        //Check whether the GridNumericColumn's cell is not already in Edit mode 
        if (!IsInEditing)
        {
            //Edit mode will be based on the ProcessPreviewTextInput() method 
            ProcessPreviewTextInput(e);
            //if (!(CurrentCellRendererElement is TextBox))
                return true;
        }
        return base.ShouldGridTryToHandleKeyDown(e);
    }

    private void ProcessPreviewTextInput(KeyRoutedEventArgs e)
    {
        //Here you can customize the edit mode behavior of GridNumericColumn while pressing the key from the Keyboard.
        if ((!char.IsLetterOrDigit(e.Key.ToString(), 0) || !DataGrid.AllowEditing || DataGrid.NavigationMode != NavigationMode.Cell) || (e.Key == VirtualKey.F2))
            return;
        if (DataGrid.SelectionController.CurrentCellManager.BeginEdit())
            PreviewTextInput(e);
    }
}
