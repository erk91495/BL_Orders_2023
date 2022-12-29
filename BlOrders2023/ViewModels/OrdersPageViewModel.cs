using CommunityToolkit.Mvvm.ComponentModel;
using BlOrders2023.Core.Services;
using Microsoft.EntityFrameworkCore;
using BlOrders2023.Models;
using System;
using System.Linq;
using BlOrders2023.Core.Data.SQL;
using BlOrders2023.Core.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.UI.Dispatching;
using CommunityToolkit.WinUI;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace BlOrders2023.ViewModels;

public class OrdersPageViewModel : ObservableRecipient
{

    #region Properties
    /// <summary>
    /// Gets or sets the text for the Orders filter
    /// </summary>
    public string FilterText { get; set; } = "";

    /// <summary>
    /// Gets or sets the selected order.
    /// </summary>
    public Order? SelectedOrder
    {
        get => _selectedOrder;
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
    #endregion Properties

    #region Fields
    /// <summary>
    /// True if the page is loading orders from the database
    /// </summary>
    private bool _isLoading;

    /// <summary>
    /// Gets the currently selected order from the datagrid 
    /// </summary>
    private Order? _selectedOrder;

    /// <summary>
    /// Gets the unfiltered collection of all orders. 
    /// </summary>
    private List<Order> MasterOrdersList { get; set; } = new List<Order>();

    /// <summary>
    /// Gets the orders to display.
    /// </summary>
    public ObservableCollection<Order> Orders { get; private set; } = new ObservableCollection<Order>();

    /// <summary>
    /// Gets the dispatcher Queue for the current thread
    /// </summary>
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
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
    public async void LoadOrders()
    {
        await dispatcherQueue.EnqueueAsync(() =>
        {
            IsLoading = true;
            Orders.Clear();
            MasterOrdersList.Clear();
        });

        IOrderTable table = App.BLDatabase.Orders;
        var orders = await Task.Run(table.GetAsync);

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
    public async void QueryOrders(string query)
    {
        if (!string.IsNullOrWhiteSpace(query))
        {
            IsLoading = true;
            Orders.Clear();

            IOrderTable table = App.BLDatabase.Orders;

            var results = await Task.Run(table.GetAsync);
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
    #endregion Queries

    #region Filtering
    public bool FilterOrders(object o)
    {
        Order? order = o as Order;

        if (order != null)
        {
            if (FilterText.IsNullOrEmpty())
            {
                return true;
            }
            else
            {
                if (order.OrderID.ToString().Contains(FilterText, StringComparison.CurrentCultureIgnoreCase) ||
                    order.CustID.ToString().Contains(FilterText, StringComparison.CurrentCultureIgnoreCase) ||
                    order.Customer.CustomerName.Contains(FilterText, StringComparison.CurrentCultureIgnoreCase)
                    //order.PO_Number.Contains(FilterText, StringComparison.CurrentCultureIgnoreCase)
                    )
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

    #endregion Methods
}