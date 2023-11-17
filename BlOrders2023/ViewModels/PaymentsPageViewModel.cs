using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using CommunityToolkit.WinUI;

namespace BlOrders2023.ViewModels;
public class PaymentsPageViewModel: ObservableRecipient
{
    #region Properties
    public ObservableCollection<Payment> Payments { get; set; }
    public ObservableCollection<WholesaleCustomer> Customers { get; set; }
    public ObservableCollection<PaymentMethod> PaymentMethods { get; private set; }

    public ObservableCollection<Order> UnpaidInvoices { get; set; }
    /// <summary>
    /// Gets or sets the selected Order.
    /// </summary>
    public Payment SelectedPayment
    {
        get => _selectedPayment!;
        set
        {
            SetProperty(ref _selectedPayment, value);
            OnPropertyChanged(nameof(SelectedPayment));
        }
    }

    /// <summary>
    /// Gets or sets a value that specifies whether orders are being loaded.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            SetProperty(ref _isLoading, value);
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    public bool IsLoaded => !_isLoading;
    #endregion Properties

    #region Fields
    private Payment? _selectedPayment;
    private bool _isLoading;
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private readonly IBLDatabase _db = App.GetNewDatabase();
    #endregion Fields

    #region Constructors
    public PaymentsPageViewModel()
    {
        _isLoading = false;
        Payments = new();
        Customers = new();
        PaymentMethods = new();
        UnpaidInvoices = new();
    }
    #endregion Constructors

    #region Methods
    /// <summary>
    /// Submits a query to the data source.
    /// </summary>
    public async Task QueryPayments(string? query = null)
    {
        if (!string.IsNullOrWhiteSpace(query))
        {
            IsLoading = true;
            Payments.Clear();

            IPaymentsTable table = _db.Payments;

            var results = await table.GetPaymentsAsync(query);
            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (Payment o in results)
                {
                    Payments.Add(o);
                }
                OnPropertyChanged(nameof(Payments));
            });
            IsLoading = false;
        }
        else
        {
            IsLoading = true;
            Payments.Clear();

            IPaymentsTable table = _db.Payments;

            var results = await table.GetPaymentsAsync(null);
            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (Payment o in results)
                {
                    Payments.Add(o);
                }
                OnPropertyChanged(nameof(Payments));
            });
            IsLoading = false;
        }
    }

    public async Task QueryCustomers()
    {
            Customers.Clear();

            IWholesaleCustomerTable table = _db.Customers;

            var results = await table.GetAsync();
            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (WholesaleCustomer o in results)
                {
                    Customers.Add(o);
                }
                OnPropertyChanged(nameof(Customers));
            });
    }

    internal async Task QueryPaymentMethods()
    {
        PaymentMethods.Clear();

        IPaymentsTable table = _db.Payments;

        var results = await table.GetPaymentMethodsAsync();
        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (PaymentMethod o in results)
            {
                PaymentMethods.Add(o);
            }
            OnPropertyChanged(nameof(PaymentMethods));
        });
    }

    internal async Task SavePayment(Payment payment)
    {
        await _db.Payments.UpsertPaymentAsync(payment);
    }

    internal async Task QueryUnpaidInvoices()
    {
        UnpaidInvoices.Clear();

        IOrderTable table = _db.Orders;

        var results = await table.GetUnpaidInvoicedInvoicesAsync();
        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (Order o in results)
            {
                UnpaidInvoices.Add(o);
            }
            OnPropertyChanged(nameof(UnpaidInvoices));
        });
    }

    internal async Task SaveOrder(Order order)
    {
        await _db.Orders.UpsertAsync(order);
    }
    #endregion Methods
}
