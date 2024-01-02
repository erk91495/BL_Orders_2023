using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.DataGrid;

namespace BlOrders2023.UserControls.ViewModels;

public interface IGridEditorViewModel
{
    public bool CanSave { get; set; }

    public ObservableCollection<object> Items{ get; set; }
    public Type ItemSourceType { get; set; }
    public void ValidateItems(object sender, CurrentCellValidatingEventArgs e);
    public void Save(ContentDialog sender, ContentDialogButtonClickEventArgs args);
}
