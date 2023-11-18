using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.IdentityModel.Tokens;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls;
public class CustomerChangedEventArgs : EventArgs
{
    public WholesaleCustomer? Customer { get; }
    public CustomerChangedEventArgs(WholesaleCustomer? customer)
    {
        Customer = customer;
    }
}


public sealed partial class CustomerOrderSelectionDialog : ContentDialog, INotifyPropertyChanged
{
    #region Fields
    private ObservableCollection<WholesaleCustomer> _customersMasterList = new();
    private ObservableCollection<WholesaleCustomer> _suggestedCustomers = new();
    private ObservableCollection<Order> _suggestedOrders = new();
    private WholesaleCustomer? _selectedCustomer;
    private ObservableCollection<Order> selectedOrders = new();

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<CustomerChangedEventArgs>? CustomerChanged;
    #endregion Fields

    #region Properties
    public ObservableCollection<WholesaleCustomer> SuggestedCustomers
    {
        get => _suggestedCustomers;
        set
        {
            _suggestedCustomers = value;
            OnPropertyChanged();
        }
    }

    public WholesaleCustomer? SelectedCustomer
    {
        get => _selectedCustomer;
        set 
        {
            _selectedCustomer = value;
            _suggestedOrders.Clear();
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Order> SuggestedOrders
    {
        get => _suggestedOrders;
        set
        {
            _suggestedOrders = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Order> SelectedOrders
    {
        get => selectedOrders;    
        set 
        {
            selectedOrders = value;
            OnPropertyChanged();
        }
    }
    #endregion Properties

    #region Constructors
    public CustomerOrderSelectionDialog(IEnumerable<WholesaleCustomer> customers)
    {
        this.InitializeComponent();
        foreach(var customer in customers)
        {
            _customersMasterList.Add(customer);
            _suggestedCustomers.Add(customer);
        }
    }
    #endregion Constructors

    #region Methods
    private void CustomerSelection_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            var text = sender.Text;
            if (text.IsNullOrEmpty())
            {
                SuggestedCustomers = _customersMasterList;
            }
            else
            {
                SuggestedCustomers = _customersMasterList.Where(c => c.CustomerName.Contains(text, StringComparison.CurrentCultureIgnoreCase) || c.CustID.ToString().Contains(text, StringComparison.CurrentCultureIgnoreCase) ).ToObservableCollection();
            }
        }
    }

    /// <summary>
    /// Triggers when a customer has been selected or when the user submits a name using the return key
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void CustomerSelection_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion != null && args.ChosenSuggestion is WholesaleCustomer c)
        {
            //User selected an item, take an action
            SelectedCustomer = c;
            OnCustomerChanged();
        }
        else
        {
            var name = args.QueryText.Trim();
            // If we dont find something the result will be null
            SelectedCustomer = SuggestedCustomers.FirstOrDefault(cust => cust.CustomerName.Equals(name, StringComparison.CurrentCultureIgnoreCase) || cust.Phone == name);
            OnCustomerChanged();
        }
        if (SelectedCustomer != null)
        {
            IsPrimaryButtonEnabled = true;
            CustomerSelection.Text = SelectedCustomer.CustomerName;
        }
        else
        {
            IsPrimaryButtonEnabled = false;
        }
    }

    private void OrderSelection_GotFocus(object sender, RoutedEventArgs e)
    {
        if (OrderSelection.SelectedItems.IsNullOrEmpty())
        {
            OrderSelection.IsDropDownOpen = true;
        }
    }

    private void OrderSelection_LostFocus(object sender, RoutedEventArgs e)
    {
        OrderSelection.IsDropDownOpen = false;
    }

    private void OnPropertyChanged()
    {
        // Broadcast all properties.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    private void OnCustomerChanged()
    {
        CustomerChanged?.Invoke(this, new(_selectedCustomer));
    }
    #endregion Methods
}
