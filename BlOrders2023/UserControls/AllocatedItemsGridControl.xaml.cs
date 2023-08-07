using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using BlOrders2023.ViewModels.Converters;
using CommunityToolkit.WinUI.UI.Controls;
using CommunityToolkit.WinUI.UI.Controls.Primitives;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.Data;
using Syncfusion.UI.Xaml.DataGrid;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls;
public sealed partial class AllocatedItemsGridControl : UserControl
{

    #region Properties
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(List<OrderItem>), typeof(AllocatedItemsGridControl), new PropertyMetadata(null));
    public List<OrderItem> Items
    {
        get => (List<OrderItem>)GetValue(ItemsProperty);
        set 
        {
            SetValue(ItemsProperty, value);
            OnItemsChanged();
        }
    }

    //public static readonly DependencyProperty GroupsProperty = DependencyProperty.Register("Groups", typeof(IEnumerable<List<int>>), typeof(AllocatedItemsGridControl), new PropertyMetadata(null));
    //public IEnumerable<List<int>> Groups
    //{
    //    get => (IEnumerable<List<int>>)GetValue(GroupsProperty);
    //    set
    //    {
    //        SetValue(GroupsProperty, value);
    //        OnItemsChanged();
    //    }
    //}
    #endregion Properties

    #region Fields
    #endregion Fields

    #region Constructors
    public AllocatedItemsGridControl()
    {
        this.InitializeComponent();
    }
    #endregion Constructors

    #region Methods
    private void OnItemsChanged()
    {
        MainStack.Children.Clear();
        foreach(var Ids in App.GetNewDatabase().Allocation.GetAllocationGroups().Select(e => e.ProductIDs)) {
            SfDataGrid d = new SfDataGrid()
            {
                AutoGenerateColumns= false,
                ItemsSource = Items.Where(e => Ids.Contains(e.ProductID)),
                IsReadOnly=false,

            };

            GridTextColumn ProductIDColumn = new GridTextColumn()
            { 
                HeaderText="Product", 
                MappingName = "ProductID",
                IsReadOnly=true,
            };

            GridTextColumn OrderedColumn = new GridTextColumn()
            {
                HeaderText = "Ordered",
                MappingName = "Quantity",
                IsReadOnly=true,
            };

            GridNumericColumn AllocatedColumn = new GridNumericColumn()
            {
                HeaderText = "Allocated",
                MappingName = "QuanAllocated",
                //DisplayBinding = new Binding() { Path = new("QuanAllocated"), Converter = new FloatToStringConverter()},
                //ValueBinding = new Binding() { Path = new("QuanAllocated"), Mode=BindingMode.TwoWay, Converter= new FloatToDecimalConverter() },
                IsReadOnly = false,
                AllowEditing = true,
                UseBindingValue = true,
            };

            d.Columns.Add(ProductIDColumn);
            d.Columns.Add(OrderedColumn);
            d.Columns.Add(AllocatedColumn);
            if(d.ItemsSource is IEnumerable<OrderItem> source && !source.IsNullOrEmpty())
            {
                MainStack.Children.Add(d);
            }
        }
    }

    private void cellEditEnded(object? sender, DataGridCellEditEndedEventArgs e)
    {
        if(sender is DataGrid dg)
        {
            if(e.Column.Header.ToString() == ("Allocated") && dg.SelectedItem is OrderItem item)
            {
                var cell = e.Column.GetCellContent(e.Row) as TextBlock;
                if (cell != null && !int.TryParse(cell.Text, out _))
                {
                    //todo validate input
                }
            }
        }
    }
    #endregion Methods
}
