using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.DataGrid;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls;
public partial class AllocatedOrderCardControl : UserControl, INotifyPropertyChanged
{
    #region Properties
    public event EventHandler<CurrentCellValidatingEventArgs>? ValidateCell;
    public event PropertyChangedEventHandler? PropertyChanged;

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<OrderItem>), typeof(AllocatedOrderCardControl), new PropertyMetadata(null));
    public static readonly DependencyProperty OrderIdProperty = DependencyProperty.Register("OrderID", typeof(int), typeof(AllocatedOrderCardControl), new PropertyMetadata(null));
    public static readonly DependencyProperty TotalOrderedProperty = DependencyProperty.Register("TotalOrdered", typeof(int), typeof(AllocatedOrderCardControl), new PropertyMetadata(null));
    public static readonly DependencyProperty TotalAllocatedProperty = DependencyProperty.Register("TotalAllcoated", typeof(int), typeof(AllocatedOrderCardControl), new PropertyMetadata(null));

    public static AllocatedOrderCardControl control;

    public ObservableCollection<OrderItem> Items
    {
        get => (ObservableCollection<OrderItem>)GetValue(ItemsProperty);
        set
        {
            SetValue(ItemsProperty, value);
            //GenerateAllocatedItemsTables();
            OnPropertyChanged();
        }
    }

    public int OrderID
    {
        get => (int)GetValue(OrderIdProperty);
        set
        {
            SetValue(OrderIdProperty, value);
            OnPropertyChanged();
        }
    }

    public int TotalOrdered
    {
        get => (int)GetValue(TotalOrderedProperty);
        set
        {
            SetValue(TotalOrderedProperty, value);
            OnPropertyChanged();
        }
    }

    public int TotalAllocated
    {
        get => (int)GetValue(TotalAllocatedProperty);
        set
        {
            SetValue(TotalAllocatedProperty, value);
            OnPropertyChanged();
        }
    }
    #endregion Properties

    #region Fields
    #endregion Fields

    #region Constructors
    public AllocatedOrderCardControl()
    {
        this.InitializeComponent();

    }
    #endregion Constructors

    #region Methods
    private void GenerateAllocatedItemsTables()
    {
        MainStack.Children.Clear();
        foreach (var Ids in App.GetNewDatabase().Allocation.GetAllocationGroups().Select(e => e.ProductIDs))
        {
            SfDataGrid d = new SfDataGrid()
            {
                AutoGenerateColumns = false,
                ItemsSource = Items.Where(e => Ids.Contains(e.ProductID)),
                IsReadOnly = false,
            };
            d.CurrentCellValidating += ValidateCell;

            GridTextColumn ProductIDColumn = new GridTextColumn()
            {
                HeaderText = "Product",
                MappingName = "ProductID",
                IsReadOnly = true,
            };

            GridTextColumn OrderedColumn = new GridTextColumn()
            {
                HeaderText = "Ordered",
                MappingName = "Quantity",
                IsReadOnly = true,
            };

            GridNumericColumn AllocatedColumn = new FloatGridNumericColumn()
            {
                HeaderText = "Allocated",
                MappingName = "QuanAllocated",
                DataValidationMode = Syncfusion.UI.Xaml.Grids.GridValidationMode.InView,
                IsReadOnly = false,
                AllowEditing = true,
            };

            d.Columns.Add(ProductIDColumn);
            d.Columns.Add(OrderedColumn);
            d.Columns.Add(AllocatedColumn);
            if (d.ItemsSource is IEnumerable<OrderItem> source && !source.IsNullOrEmpty())
            {
                MainStack.Children.Add(d);
            }
        }
    }


    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion Methods
}
