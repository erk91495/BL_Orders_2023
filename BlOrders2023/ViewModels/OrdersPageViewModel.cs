using CommunityToolkit.Mvvm.ComponentModel;
using BlOrders2023.Core.Services;
using Microsoft.EntityFrameworkCore;
using BlOrders2023.Models;
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

namespace BlOrders2023.ViewModels;

public class OrdersPageViewModel : ObservableRecipient
{
    private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    /// <summary>
    /// Initializes a new instance of the OrderListPageViewModel class.
    /// </summary>
    public OrdersPageViewModel() => IsLoading = false;

    /// <summary>
    /// Gets the unfiltered collection of all orders. 
    /// </summary>
    private List<Order> MasterOrdersList { get; } = new List<Order>();

    /// <summary>
    /// Gets the orders to display.
    /// </summary>
    public ObservableCollection<Order> Orders { get; private set; } = new ObservableCollection<Order>();

    private bool _isLoading;

    private Order? _selectedOrder;

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

        var builder = new DbContextOptionsBuilder<BLOrdersDBContext>();
        builder.UseSqlServer(connectionString: "Data Source=ERIC-PC; Database=New_Bl_Orders;Integrated Security=true; Trust Server Certificate=true",
            opts => opts.CommandTimeout(300));
        var cont = new BLOrdersDBContext(builder.Options);

        OrderTable table = new(cont);

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
            var builder = new DbContextOptionsBuilder<BLOrdersDBContext>();
            builder.UseSqlServer(connectionString: "Data Source=ERIC-PC; Database=New_Bl_Orders;Integrated Security=true; Trust Server Certificate=true",
                opts => opts.CommandTimeout(300));
            var cont = new BLOrdersDBContext(builder.Options);

            OrderTable table = new(cont);

            var results = await Task.Run(table.GetAsync);
            //var results = await App.Repository.Orders.GetAsync(query);
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
}
