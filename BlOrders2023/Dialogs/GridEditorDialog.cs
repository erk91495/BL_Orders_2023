using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using BlOrders2023.Dialogs.ViewModels;
using Syncfusion.UI.Xaml.DataGrid;
using Syncfusion.UI.Xaml.Grids;
using CommunityToolkit.WinUI.UI.Controls;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Dialogs;

public sealed partial class GridEditorDialog<T> : ContentDialog
    where T : class
{
    public IGridEditorViewModel<T> ViewModel { get; }
    private SfDataGrid DataGrid { get; set; }

    public GridEditorDialog()
    {
        ViewModel = App.GetService<IGridEditorViewModel<T>>();
        this.InitializeComponent();
        DataGrid.Loaded += DataGrid_Loaded;
        ViewModel.MapColumns(DataGrid);
        if (ViewModel.CanAddItems)
        {
            this.DataGrid.AddNewRowPosition = Syncfusion.UI.Xaml.DataGrid.AddNewRowPosition.Top;
        }
        
    }

    private void DataGrid_Loaded(object sender, RoutedEventArgs e)
    {
        DataGrid.ColumnSizer.ResetAutoCalculationforAllColumns();
        DataGrid.ColumnSizer.Refresh();
    }

    private void InitializeComponent()
    {
        PrimaryButtonText = "Save";
        CloseButtonText = "Cancel";
        IsPrimaryButtonEnabled = true;
        Resources["ContentDialogMaxWidth"] = 1200.0;
        
        // Create and configure the SfDataGrid
        DataGrid = new()
        {
            AutoGenerateColumns = false,
            EditorSelectionBehavior = EditorSelectionBehavior.MoveLast,
            EditTrigger = EditTrigger.OnTap,
            AllowEditing = true,
            SelectionUnit = GridSelectionUnit.Row,
        };
        DataGrid.CurrentCellValidating += ViewModel.ValidateItems;

        // Bind ItemsSource to ViewModel.Items
        DataGrid.SetBinding(SfDataGrid.ItemsSourceProperty, new Binding { Path = new PropertyPath("Items"), Source = ViewModel });

        // Create a grid layout for the ContentDialog
        Grid grid = new Grid();
        grid.Children.Add(DataGrid);

        // Set Content to the grid layout
        Content = grid;

        // Hook up event handlers
        PrimaryButtonClick += GridEditorDialog_PrimaryButtonClick;
    }

    private async void GridEditorDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        try
        {
            await ViewModel.SaveAsync();
        }
        catch (DbUpdateException e)
        {
            App.LogException(e);
        }
    }
}
