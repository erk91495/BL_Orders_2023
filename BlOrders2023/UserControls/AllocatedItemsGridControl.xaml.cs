using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using CommunityToolkit.WinUI.UI.Controls;
using CommunityToolkit.WinUI.UI.Controls.Primitives;
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
            DataGrid d = new DataGrid()
            {
                AutoGenerateColumns= false,
                ItemsSource = Items.Where(e => Ids.Contains( e.ProductID)),
                Style = (Style)App.Current.Resources["AllocationDataGridStyle"],
                ColumnHeaderStyle = (Style)App.Current.Resources["AllocationDataGridColumnHeaderStyle"],
                RowStyle = (Style)App.Current.Resources["AllocationDataGridRowStyle"],
                CellStyle = (Style)App.Current.Resources["AllocationDataGridCellStyle"],
            };

            DataGridColumn ProductIDColumn = new DataGridTextColumn()
            { 
                Header="Product", 
                Binding = new(){ Path = new("ProductID"), Mode=BindingMode.TwoWay},
            };

            DataGridColumn OrderedColumn = new DataGridTextColumn()
            {
                Header = "Ordered",
                Binding = new() { Path = new("Quantity"), Mode = BindingMode.TwoWay },
            };

            DataGridColumn AllocatedColumn = new DataGridTextColumn()
            {
                Header = "Allocated",
                Binding = new() { Path = new("QuanAllocated"), Mode = BindingMode.TwoWay },
            };

            d.Columns.Add(ProductIDColumn);
            d.Columns.Add(OrderedColumn);
            d.Columns.Add(AllocatedColumn);
            if(!d.ItemsSource.ToList<OrderItem>().IsNullOrEmpty())
            {
                MainStack.Children.Add(d);
            }
        }
    }
    #endregion  Methods
}
