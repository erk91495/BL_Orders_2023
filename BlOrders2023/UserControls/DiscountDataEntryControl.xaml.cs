using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using Syncfusion.UI.Xaml.Calendar;
using System.ComponentModel;
using System.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls;

public sealed partial class DiscountDataEntryControl : ContentDialog, INotifyDataErrorInfo
{
    #region Fields
    private Discount _discount;
    private IEnumerable<DiscountTypes> DiscountTypes => Enum.GetValues(typeof(DiscountTypes)).Cast<DiscountTypes>();
    private IEnumerable<Product> Products;
    private IEnumerable<WholesaleCustomer> Customers;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    #endregion Fields

    #region Properties
    public DateTimeOffsetRange? DateRange
    {
        get 
        {
            if(Discount.StartDate != null && Discount.EndDate != null)
            {
                return new(new((DateTime)Discount.StartDate), new((DateTime)Discount.EndDate));
            }
            else
            {
                return null;
            }
        }
        set
        {
            if(value != null && value.StartDate != null && value.EndDate != null) 
            {
                if(value.StartDate.HasValue)
                {
                    Discount.StartDate = value.StartDate.Value.DateTime;
                }

                if (value.StartDate.HasValue)
                {
                    Discount.EndDate = value.EndDate.Value.DateTime;
                }
            }
            else
            {
                Discount.StartDate = null;
                Discount.EndDate = null;
            }
        }
    }

    public Discount Discount
    {
        get => _discount;
        set => _discount = value;
    }

    public bool HasErrors => throw new NotImplementedException();

    #endregion Properties

    public DiscountDataEntryControl(IEnumerable<Product> products, IEnumerable<WholesaleCustomer> customers, Discount? discount = null)
    {
        _discount = discount ?? new();
        Products = products;
        Customers = customers;
        this.InitializeComponent();
        ProductsCombo.Loaded += ProductsCombo_Loaded;
        CustomersCombo.Loaded += CustomersCombo_Loaded;
    }

    private void ProductsCombo_Loaded(object sender, RoutedEventArgs e)
    {
        foreach (var product in Discount.Products)
        {
            ProductsCombo.SelectedItems.Add(product);
        }
        ProductsCombo.SelectionChanged += SfComboBox_ProductSelectionChanged;
    }
    private void CustomersCombo_Loaded(object sender, RoutedEventArgs e)
    {
        foreach (var customer in Discount.Customers)
        {
            CustomersCombo.SelectedItems.Add(customer);
        }
        CustomersCombo.SelectionChanged += SfComboBox_CustomerSelectionChanged;
    }

    private void SfComboBox_ProductSelectionChanged(object sender, Syncfusion.UI.Xaml.Editors.ComboBoxSelectionChangedEventArgs e)
    {
        foreach( Product product in e.AddedItems)
        {
            Discount.Products.Add(product);
        }
        foreach (Product product in e.RemovedItems)
        {
            Discount.Products.Remove(product);
        }
    }

    private void SfComboBox_CustomerSelectionChanged(object sender, Syncfusion.UI.Xaml.Editors.ComboBoxSelectionChangedEventArgs e)
    {
        foreach (WholesaleCustomer product in e.AddedItems)
        {
            Discount.Customers.Add(product);
        }
        foreach (WholesaleCustomer product in e.RemovedItems)
        {
            Discount.Customers.Remove(product);
        }
    }

    public IEnumerable GetErrors(string? propertyName) => throw new NotImplementedException();
}
