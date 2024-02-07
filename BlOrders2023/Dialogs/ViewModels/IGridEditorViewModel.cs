using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.DataGrid;

namespace BlOrders2023.Dialogs.ViewModels;

public interface IGridEditorViewModel<T> : INotifyPropertyChanged
{
    public bool CanAddItems { get; set; }
    public bool CanSave { get; }
    public ObservableCollection<T> Items{ get; set; }
    public Type ItemSourceType { get; set; }
    public void MapColumns(SfDataGrid datagrid);
    public void ValidateItems(object sender, CurrentCellValidatingEventArgs e);
    public Task SaveAsync();
}
