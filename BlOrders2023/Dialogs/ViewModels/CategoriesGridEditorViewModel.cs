using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.Identity.Client;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.DataGrid;
using Syncfusion.UI.Xaml.DataGrid.Renderers;

namespace BlOrders2023.Dialogs.ViewModels;
public class CategoriesGridEditorViewModel : ObservableValidator, IGridEditorViewModel<ProductCategory>
{

    #region Fields
    private ObservableCollection<ProductCategory> items = new();
    private bool _canSave = false;
    private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private Collection<int> touchedItems = new();
    private bool _canAddItems = true;
    #endregion Fields
    #region Properties
    public ObservableCollection<ProductCategory> Items
    {
        get => items;
        set => items = value;
    }
    public Type ItemSourceType
    {
        get; set;
    }
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
    public CategoriesGridEditorViewModel()
    {
        _ = QueryCategoriesAsync();
    }
    #endregion Constructors
    #region Methods
    public void ValidateItems(object sender, CurrentCellValidatingEventArgs e)
    {
        if (e.RowData is ProductCategory category)
        {
            if (e.OldValue != e.NewValue)
            {
                Collection<ValidationResult> result = new();
                ValidationContext context = new(category);
                context.MemberName = e.Column.MappingName;
                var type = category.GetType().GetProperty(e.Column.MappingName).PropertyType;
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
                    if (!touchedItems.Contains(category.CategoryID))
                    {
                        touchedItems.Add(category.CategoryID);
                    }
                }
            }
        }
    }

    private async Task QueryCategoriesAsync()
    {
        await dispatcherQueue.EnqueueAsync(() =>
        {
            items.Clear();
        });

        IProductCategoriesTable table = App.GetNewDatabase().ProductCategories;

        var categories = await Task.Run(() => table.GetAsync());
        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (var item in categories)
            {
                items.Add(item);
            }
            OnPropertyChanged(nameof(Items));
        });
    }

    public async Task SaveAsync()
    {
        if (CanSave)
        {
            foreach (var id in touchedItems)
            {
                await App.GetNewDatabase().ProductCategories.UpsertAsync(items.First(b => b.CategoryID == id));
            }
        }
    }

    public void MapColumns(SfDataGrid datagrid)
    {
        datagrid.CurrentCellValueChanged += Datagrid_CurrentCellValueChanged;
        //Bug DF Ticket 553474 makes me override this
        datagrid.CellRenderers.Remove("CheckBox");
        datagrid.CellRenderers.Add("CheckBox", new GridCellCheckBoxRendererExt());
        datagrid.Columns.Clear();
        //datagrid.Columns.Add(new GridTextColumn() { HeaderText = "ID", MappingName = "ID", AllowEditing = false });
        datagrid.Columns.Add(new GridTextColumn() { HeaderText = "Category Name", MappingName = "CategoryName", ColumnWidthMode = Syncfusion.UI.Xaml.Grids.ColumnWidthMode.SizeToCells });
        var column = new GridCheckBoxColumn() { HeaderText = "Show Totals On Reports", MappingName = "ShowTotalsOnReports", DataValidationMode = Syncfusion.UI.Xaml.Grids.GridValidationMode.InView };
        datagrid.Columns.Add(column);
        datagrid.Columns.Add(new GridNumericColumn() { HeaderText = "Display Index", MappingName = "DisplayIndex" });    
    }

    private void Datagrid_CurrentCellValueChanged(object? sender, CurrentCellValueChangedEventArgs e)
    {
        if(e.Record is ProductCategory category){
            if (!touchedItems.Contains(category.CategoryID))
            {
                touchedItems.Add(category.CategoryID);
            }
            if (!category.HasErrors)
            {
                CanSave = true;
            }
            else
            {
                CanSave = false;
            }
        }
        
    }
    #endregion Methods

}
public class GridCellCheckBoxRendererExt : GridCellCheckBoxRenderer
{
    protected override void OnEditElementUnloaded(object sender, RoutedEventArgs e)
    {

    }
}
