using System.Collections.ObjectModel;
using System.Diagnostics;
using BlOrders2023.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.Data;
using Syncfusion.UI.Xaml.DataGrid;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls;
public sealed partial class AllocatedItemsGridControl : UserControl
{

    #region Properties
    public event EventHandler<CurrentCellValidatingEventArgs>? ValidateCell;

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<OrderItem>), typeof(AllocatedItemsGridControl), new PropertyMetadata(null));

    public ObservableCollection<OrderItem> Items
    {
        get => (ObservableCollection<OrderItem>)GetValue(ItemsProperty);
        set 
        {
            SetValue(ItemsProperty, value);
            OnItemsChanged();
        }
    }

    #endregion Properties

    #region Fields
    #endregion Fields

    #region Constructors
    public AllocatedItemsGridControl()
    {
        this.InitializeComponent();
        this.DataContext = Items;
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
                ItemsSource = Items.Where(e => Ids.Contains(e.ProductID)).OrderBy(i => i.ProductID),
                IsReadOnly=false,
            };
            d.CurrentCellValidating += ValidateCell;

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
            if(d.ItemsSource is IEnumerable<OrderItem> source && !source.IsNullOrEmpty())
            {
                MainStack.Children.Add(d);
            }
        }
    }
    #endregion Methods
}
