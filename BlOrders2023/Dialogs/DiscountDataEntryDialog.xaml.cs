using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using Syncfusion.UI.Xaml.Calendar;
using System.ComponentModel;
using System.Collections;
using BlOrders2023.Dialogs.ViewModels;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls;

public sealed partial class DiscountDataEntryDialog : ContentDialog
{
    #region Fields
    private IEnumerable<DiscountTypes> DiscountTypes => Enum.GetValues(typeof(DiscountTypes)).Cast<DiscountTypes>();
    DiscountDataEntryDialogViewModel ViewModel { get; }
    public ObservableCollection<Product> Products;
    public ObservableCollection<WholesaleCustomer> Customers;
    #endregion Fields

    #region Properties

    #endregion Properties

    public DiscountDataEntryDialog(IEnumerable<Product> products, IEnumerable<WholesaleCustomer> customers, Discount? discount = null)
    {
        ViewModel = App.GetService<DiscountDataEntryDialogViewModel>();
        ViewModel.Discount = discount ?? new();
        Products = (System.Collections.ObjectModel.ObservableCollection<Product>)products;
        Customers = (System.Collections.ObjectModel.ObservableCollection<WholesaleCustomer>)customers;
        this.InitializeComponent();
        ViewModel.ErrorsChanged += ViewModel_ErrorsChanged;
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        ProductsCombo.Loaded += ProductsCombo_Loaded;
        CustomersCombo.Loaded += CustomersCombo_Loaded;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(ViewModel.Type)) 
        {
            switch (ViewModel.Type)
            {
                case Models.Enums.DiscountTypes.SetPrice:
                    ModifierBox.Header = "Price";
                    ModifierBox.CustomFormat = "C2";
                    break;
                case Models.Enums.DiscountTypes.PercentOff:
                    ModifierBox.Header = "Percent Off";
                    ModifierBox.CustomFormat = "N4";
                    break;
                default:
                    ModifierBox.Header="Modifier";
                    ModifierBox.CustomFormat="N4";
                    break;
            }
        }
    }

    private void ViewModel_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = !ViewModel.HasErrors;
    }

    private void ProductsCombo_Loaded(object sender, RoutedEventArgs e)
    {
        foreach (var product in ViewModel.Discount.Products)
        {
            ProductsCombo.SelectedItems.Add(product);
        }
        ProductsCombo.SelectionChanged += SfComboBox_ProductSelectionChanged;
    }

    private void CustomersCombo_Loaded(object sender, RoutedEventArgs e)
    {
        foreach (var customer in ViewModel.Discount.Customers)
        {
            CustomersCombo.SelectedItems.Add(customer);
        }
        CustomersCombo.SelectionChanged += SfComboBox_CustomerSelectionChanged;
    }

    private void SfComboBox_ProductSelectionChanged(object sender, Syncfusion.UI.Xaml.Editors.ComboBoxSelectionChangedEventArgs e)
    {
        foreach( Product product in e.AddedItems)
        {
            ViewModel.Discount.Products.Add(product);
        }
        foreach (Product product in e.RemovedItems)
        {
            ViewModel.Discount.Products.Remove(product);
        }
    }

    private void SfComboBox_CustomerSelectionChanged(object sender, Syncfusion.UI.Xaml.Editors.ComboBoxSelectionChangedEventArgs e)
    {
        foreach (WholesaleCustomer product in e.AddedItems)
        {
            ViewModel.Discount.Customers.Add(product);
        }
        foreach (WholesaleCustomer product in e.RemovedItems)
        {
            ViewModel.Discount.Customers.Remove(product);
        }
    }
}
