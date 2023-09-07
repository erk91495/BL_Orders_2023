using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls;
public sealed partial class PaymentDataInputControl : ContentDialog, INotifyPropertyChanged
{
    #region Fields
    private WholesaleCustomer? selectedCustomer;
    private Payment? _payment;
    #endregion Fields

    #region Properties
    public event PropertyChangedEventHandler? PropertyChanged;
    public IEnumerable<WholesaleCustomer> Customers { get; set; }
    public DateTimeOffset PaymentDate { get; set; } 
    public double? PaymentAmount { get; set; }
    public IList<PaymentMethod> PaymentMethods { get; set; }
    public PaymentMethod SelectedPaymentMethod { get ; set; } 
    public string? CheckNumber { get; set; }
    public string? Notes { get; set; }
    public int? InvoiceNumber { get; set; }
    public ObservableCollection<Order> Orders { get; set; }
    public ObservableCollection<Order> OrdersMasterList{ get; private set;}
    public WholesaleCustomer? SelectedCustomer
    {
        get => selectedCustomer;
        private set
        {
            selectedCustomer = value;
            OnPropertyChanged();
        }
    }
    #endregion Properties

    #region Constructors
    public PaymentDataInputControl(IEnumerable<Order> unpaidOrders, IList<PaymentMethod> paymentMethods)
    {
        OrdersMasterList = new(unpaidOrders);
        Orders = new(unpaidOrders);
        PaymentDate = DateTime.Today;
        PaymentMethods = paymentMethods;
        SelectedPaymentMethod = PaymentMethods.First();
        _payment = null;
        this.InitializeComponent();
        Loaded += PaymentDataInputControl_Loaded;
    }
    #endregion Constructors

    #region Methods
    private void PaymentDataInputControl_Loaded(object sender, RoutedEventArgs e)
    {
        if(_payment != null)
        {
            var res = PaymentAmountBox.Focus(FocusState.Programmatic);
        }
    }
    private void QueryOrders(string? query = null)
    {
        Orders.Clear();
        if (!query.IsNullOrEmpty())
        {
            foreach (var order in OrdersMasterList.Where(o => o.OrderID.ToString().Contains(query!, StringComparison.CurrentCultureIgnoreCase)).OrderBy(o => o.OrderID))
            {
                Orders.Add(order);
            }
        }
        else
        {
            foreach (var order in OrdersMasterList.OrderBy(o => o.OrderID))
            {
                Orders.Add(order);
            }
        }
        InvoiceSelectionBox.ItemsSource = Orders;
        OnPropertyChanged(nameof(Orders));

        if(Orders.IsNullOrEmpty())
        {
            InvoiceSelectionBox.ItemsSource = new string[] { "No results found" };
        }
    }
    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void AutoSuggestBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if(sender is AutoSuggestBox autoBox)
        {
            if(autoBox.Text.IsNullOrEmpty())
            {
                autoBox.IsSuggestionListOpen = true;
            }
        }
    }

    private void AutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is AutoSuggestBox autoBox)
        {
            autoBox.IsSuggestionListOpen = false;
        }
    }

    private void PaymentMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(sender is ComboBox combo && combo.SelectedItem is PaymentMethod method)
        {
            SelectedPaymentMethod = method;
        }
    }
    /// <summary>
    /// This event gets fired when:
    ///     * a user presses Enter while focus is in the TextBox
    ///     * a user clicks or tabs to and invokes the query button (defined using the QueryIcon API)
    ///     * a user presses selects (clicks/taps/presses Enter) a suggestion
    /// </summary>
    /// <param name="sender">The AutoSuggestBox that fired the event.</param>
    /// <param name="args">The args contain the QueryText, which is the text in the TextBox,
    /// and also ChosenSuggestion, which is only non-null when a user selects an item in the errorMessages.</param>
    private void InvoiceSelectionBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        Order FoundOrder = null;
        if (args.ChosenSuggestion != null && args.ChosenSuggestion is Order o)
        {

            //User selected an item, take an action           
            FoundOrder = o;

        }
        else if (!string.IsNullOrEmpty(args.QueryText))
        {
            var id = sender.Text.Trim();
            var result = Int32.TryParse(id, out var orderID);
            var toAdd = Orders.FirstOrDefault(order => order.OrderID == orderID);
            if (result && toAdd != null)
            {
                //The text matched a productcode
                FoundOrder = toAdd;
            }
            else
            {
                FoundOrder = null;
                return;
            }
        }

        if (FoundOrder != null)
        {
            InvoiceNumber = FoundOrder.OrderID;
            CustomerSelectionBox.Text = FoundOrder.Customer.CustomerName;
            SelectedCustomer = FoundOrder.Customer;
        }
    }

    private void InvoiceSelectionBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        QueryOrders(sender.Text);
    }

    public Payment? GetPayment()
    {
        if(_payment == null)
        {
            _payment = new();
        }
        if(SelectedCustomer != null && InvoiceNumber != null)
        {
            _payment.OrderId = InvoiceNumber;
            _payment.CustId = SelectedCustomer.CustID;
            _payment.PaymentAmount = (decimal?)PaymentAmount;
            _payment.PaymentDate = PaymentDate.DateTime;
            _payment.PaymentMethodID = SelectedPaymentMethod.PaymentMethodID;
            _payment.Notes = Notes;
            _payment.CheckNumber = CheckNumber;
            return _payment;
        }
        else
        {
            return null;
        }

    }

    public void SetPayment(Payment payment)
    {
        _payment = payment;
        //TODO: make this work with ui
        CustomerSelectionBox.Text = payment.Customer.CustomerName;
        InvoiceSelectionBox.Text = payment.OrderId.ToString();
        InvoiceNumber = payment.OrderId;
        SelectedCustomer = payment.Customer;
        PaymentAmount = (double?)payment.PaymentAmount;
        PaymentDate = new((DateTime)payment.PaymentDate);
        PaymentMethodCombo.SelectedIndex = PaymentMethods.IndexOf(payment.PaymentMethod);
        Notes = payment.Notes;
        CheckNumber = payment.CheckNumber;

        //Lock Out changes for invoice number
        InvoiceSelectionBox.IsEnabled = false;
    }
    #endregion Methods


}



