﻿using CommunityToolkit.Mvvm.ComponentModel;
using BlOrders2023.Models;
using BlOrders2023.Core.Data;
using System.Collections.ObjectModel;
using Microsoft.UI.Dispatching;
using CommunityToolkit.WinUI;
using Microsoft.IdentityModel.Tokens;
using BlOrders2023.Views;

namespace BlOrders2023.ViewModels;

public class OrdersPageViewModel : ObservableRecipient
{

    #region Properties
    public string DatabaseName => _db.DbConnection.Database.Replace('_',' ');

    /// <summary>
    /// Gets or sets the text for the Orders filter
    /// </summary>
    public string FilterText { get; set; } = "";

    /// <summary>
    /// Gets or sets the selected Order.
    /// </summary>
    public Order SelectedOrder
    {
        get => _selectedOrder!;
        set
        {
            SetProperty(ref _selectedOrder, value);
            OnPropertyChanged(nameof(SelectedOrder));
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

    /// <summary>
    /// Gets the orders to display.
    /// </summary>
    public ObservableCollection<Order> Orders { get; private set; } = new ObservableCollection<Order>();
    #endregion Properties

    #region Fields
    /// <summary>
    /// True if the page is loading orders from the database
    /// </summary>
    private bool _isLoading;

    /// <summary>
    /// Gets the currently selected Order from the datagrid 
    /// </summary>
    private Order? _selectedOrder;

    /// <summary>
    /// Gets the unfiltered collection of all orders. 
    /// </summary>
    private List<Order> MasterOrdersList { get; set; } = new List<Order>();

    /// <summary>
    /// Gets the dispatcher Queue for the current thread
    /// </summary>
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private readonly IBLDatabase _db = App.GetNewDatabase();
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the OrderListPageViewModel class.
    /// </summary>
    public OrdersPageViewModel() => IsLoading = false;

    #endregion Constructors

    #region Methods

    #region Queries
    /// <summary>
    /// Retrieves orders from the data source.
    /// </summary>
    public async Task LoadOrders()
    {
        await dispatcherQueue.EnqueueAsync(() =>
        {
            IsLoading = true;
            Orders.Clear();
            MasterOrdersList.Clear();
        });

        IOrderTable table = _db.Orders;
        var orders = await table.GetAsync();

        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (var order in orders)
            {
                Orders.Add(order);
                MasterOrdersList.Add(order);
            }
            IsLoading = false;
        });
    }

    /// <summary>
    /// Submits a query to the data source.
    /// </summary>
    public async Task QueryOrders(string query)
    {
        if (!string.IsNullOrWhiteSpace(query))
        {
            IsLoading = true;
            Orders.Clear();

            IOrderTable table = _db.Orders;

            var results = await table.GetAsync();
            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (Order o in results)
                {
                    Orders.Add(o);
                }
            });
            IsLoading = false;
        }
    }

    public async Task<WholesaleCustomer?> GetCustomerAsync(int custID)
    {
        var res = await _db.Customers.GetAsync(custID, true);
        if(!res.IsNullOrEmpty())
        {
            return res.FirstOrDefault();
        }
        else
        {   
            return null;
        }
    }
    #endregion Queries

    #region Filtering
    public bool FilterOrders(object o)
    {
        if (o is Order order)
        {
            if (FilterText.IsNullOrEmpty())
            {
                return true;
            }
            else
            {
                var nullableChecks = order.PO_Number != null && order.PO_Number.Contains(FilterText, StringComparison.CurrentCultureIgnoreCase);
                nullableChecks = nullableChecks || (order.Customer.Phone_2 != null && order.Customer.Phone_2.Contains(FilterText, StringComparison.CurrentCultureIgnoreCase));
                if (order.OrderID.ToString().Contains(FilterText, StringComparison.CurrentCultureIgnoreCase)
                    || order.CustID.ToString().Contains(FilterText, StringComparison.CurrentCultureIgnoreCase)
                    || order.Customer.CustomerName.Contains(FilterText, StringComparison.CurrentCultureIgnoreCase)
                    || order.Customer.Phone.Contains(FilterText, StringComparison.CurrentCultureIgnoreCase)
                    || nullableChecks)
                {
                    return true;
                }
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    #endregion Filtering

    /// <summary>
    /// Saves changes to the current Order
    /// </summary>
    public async Task SaveCurrentOrderAsync()
    {
        await _db.Orders.UpsertAsync(SelectedOrder);
    }

    internal IEnumerable<ProductCategory> GetTotalsCategories() => _db.ProductCategories.GetForReports();

    #endregion Methods
}