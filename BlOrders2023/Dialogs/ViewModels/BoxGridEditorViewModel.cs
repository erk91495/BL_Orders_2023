using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023.Helpers;
using BlOrders2023.Models;
using BlOrders2023.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using ServiceStack.DataAnnotations;
using Syncfusion.UI.Xaml.Data;
using Syncfusion.UI.Xaml.DataGrid;
using Windows.Security.Authentication.Web.Core;

namespace BlOrders2023.Dialogs.ViewModels;

public class BoxGridEditorViewModel : ObservableValidator, IGridEditorViewModel<Box>
{
    #region Fields
    private ObservableCollection<Box> items = new();
    private bool _canSave = false;
    private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private Collection<int> touchedItems = new();
    private bool _canAddItems = true;
    #endregion Fields
    #region Properties
    public ObservableCollection<Box> Items
    {
        get => items;
        set => items = value;
    }
    public Type ItemSourceType { get; set; }
    public bool CanSave
    {
        get => _canSave;
        private set => SetProperty(ref _canSave, value, nameof(CanSave));
    }
    public bool CanAddItems
    {
        get => _canAddItems;
        set => SetProperty(ref _canAddItems, value);
    }

    #endregion Properties
    #region Constructors
    public BoxGridEditorViewModel()
    {
        _ = QueryBoxesAsync();
    }
    #endregion Constructors
    #region Methods
    public void ValidateItems(object sender, CurrentCellValidatingEventArgs e)
    {
        if (e.RowData is Box box)
        {
            if(e.OldValue != e.NewValue){
                Collection<ValidationResult> result = new();
                ValidationContext context = new(box);
                context.MemberName = e.Column.MappingName;
                var type = box.GetType().GetProperty(e.Column.MappingName).PropertyType;
                var newVal = Convert.ChangeType(e.NewValue, type);
                if (newVal == null || !Validator.TryValidateProperty(newVal, context, result))
                {
                    e.IsValid = false;
                    e.ErrorMessage = result.First().ErrorMessage;
                    CanSave = false;
                }
                else
                {
                    CanSave = true;
                    if(!touchedItems.Contains(box.ID))
                    {
                        touchedItems.Add(box.ID);
                    }
                }
            }
        }
    }

    private async Task QueryBoxesAsync()
    {
        await dispatcherQueue.EnqueueAsync(() =>
        {
            items.Clear();
        });

        IBoxTable table = App.GetNewDatabase().Boxes;

        var boxes = await Task.Run(() => table.GetAsync());
        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach(var item in boxes)
            {
                items.Add(item);
            }
            OnPropertyChanged(nameof(Items));
        });
    }

    public async Task SaveAsync()
    {
        if(CanSave)
        {
            foreach(var id in touchedItems)
            {
                await App.GetNewDatabase().Boxes.UpsertAsync(items.Where(b => b.ID == id).First());
            }
        }
    }

    public void MapColumns(SfDataGrid datagrid)
    {
        datagrid.Columns.Clear();
        //datagrid.Columns.Add(new GridTextColumn() { HeaderText = "ID", MappingName = "ID", AllowEditing = false });
        datagrid.Columns.Add(new GridTextColumn() { HeaderText = "Box Name", MappingName = "BoxName", ColumnWidthMode = Syncfusion.UI.Xaml.Grids.ColumnWidthMode.SizeToCells});
        datagrid.Columns.Add(new GridNumericColumn() { HeaderText = "Ti Hi", MappingName = "Ti_Hi"});
        datagrid.Columns.Add(new GridNumericColumn() { HeaderText = "Box Length", MappingName = "BoxLength"});
        datagrid.Columns.Add(new GridNumericColumn() { HeaderText = "Box Width", MappingName = "BoxWidth" });
        datagrid.Columns.Add(new GridNumericColumn() { HeaderText = "Box Height", MappingName = "BoxHeight" });
    }
    #endregion Methods
}
