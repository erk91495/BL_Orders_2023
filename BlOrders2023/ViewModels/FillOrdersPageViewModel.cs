using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Core.Exceptions;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Newtonsoft.Json.Bson;
using System.Collections.ObjectModel;

namespace BlOrders2023.ViewModels;

public class FillOrdersPageViewModel : ObservableRecipient, INavigationAware
{
    #region Properties
    public bool hasOrder { get => _order != null; }
    public WholesaleCustomer Customer { get; set; }
    public Order? Order { get => _order; set => _order = value; }
    public ObservableCollection<ShippingItem> Items { get; set; }

    public ObservableCollection<Order> FillableOrders { get; set; }
    public ObservableCollection<Order> FillableOrdersMasterList { get; set; }

    public ObservableCollection<OrderedVsReceivedItem> OrderedVsReceivedItems { get; set; }
    #endregion Properties

    #region Fields
    private Order? _order;
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    #endregion Fields

    #region Consturctors
    public FillOrdersPageViewModel()
    {
        Customer = new();
        _order = null;
        Items = new();
        FillableOrders = new();
        FillableOrdersMasterList = new();
        OrderedVsReceivedItems = new();
    }
    #endregion Constructors

    #region Methods

    public void OnNavigatedTo(object parameter)
    {
        var orderID = parameter as int?;
        _ = LoadFillableOrders();
        if (orderID != null)
        {
            _ = LoadOrder((int)orderID);
            
        }
    }

    public void OnNavigatedFrom() { }

    private async Task LoadFillableOrders()
    {
        IOrderTable table = App.BLDatabase.Orders;
        var orders = await Task.Run(() => table.GetAsync());
        FillableOrders.Clear();
        FillableOrdersMasterList.Clear();
        foreach (var order in orders.Where( e => e.OrderStatus == Models.Enums.OrderStatus.Ordered ||
                                            e.OrderStatus ==  Models.Enums.OrderStatus.Filling).ToList())
        {
            FillableOrders.Add(order);
            FillableOrdersMasterList.Add(order);
        }
    }

    public async Task LoadOrder(int orderID)
    {
        IOrderTable table = App.BLDatabase.Orders;
        var order = await Task.Run(() => table.GetAsync(orderID));

        await dispatcherQueue.EnqueueAsync(() =>
        {
            _order = order.First();
            Customer = _order.Customer;
            Items = new ObservableCollection<ShippingItem>( _order.ShippingItems);

            ReCalculateOrderdVsReceived();
            OnAllPropertiesChanged();  
        });

    }

    private void ReCalculateOrderdVsReceived()
    {
        OrderedVsReceivedItems.Clear();

        foreach(var item in _order!.Items)
        {
            //If i would just build an observable dictionary i wouldnt have to do this 
            if (OrderedVsReceivedItems.Where(e => e.ProductID == item.ProductID).FirstOrDefault() != null)
            {
                OrderedVsReceivedItems.Where(e => e.ProductID == item.ProductID).First().Ordered += (int)item.QuanRcvd;
            }
            else
            {
                OrderedVsReceivedItem ovsr = new()
                {
                    ProductID = item.ProductID,
                    Ordered = (int)item.Quantity,
                    Received = 0
                };
                OrderedVsReceivedItems.Add(ovsr);
            }
        }
        foreach(var item in _order.ShippingItems)
        {
            if (OrderedVsReceivedItems.Where(e => e.ProductID == item.ProductID).FirstOrDefault() != null)
            {
                OrderedVsReceivedItems.Where(e => e.ProductID == item.ProductID).First().Received += (int)(item.QuanRcvd ?? 0);
            }
            else
            {
                OrderedVsReceivedItem ovsr = new()
                {
                    ProductID = item.ProductID,
                    Ordered = 0,
                    Received = (int)(item.QuanRcvd ?? 0)
                };
                OrderedVsReceivedItems.Add(ovsr);
            }
        }
        OnPropertyChanged(nameof(OrderedVsReceivedItems));
    }

    private void IncrementReceivedItem(ShippingItem item)
    {
        if (OrderedVsReceivedItems.Where(e => e.ProductID == item.ProductID).FirstOrDefault() != null)
        {
            OrderedVsReceivedItems.Where(e => e.ProductID == item.ProductID).First().Received += (int)(item.QuanRcvd ?? 0);
        }
        else
        {
            OrderedVsReceivedItem ovsr = new()
            {
                ProductID = item.ProductID,
                Ordered = 0,
                Received = (int)(item.QuanRcvd ?? 0)
            };
            OrderedVsReceivedItems.Add(ovsr);
        }
        OnPropertyChanged(nameof(OrderedVsReceivedItems));
        
    }

    private void OnAllPropertiesChanged()
    {
        OnPropertyChanged(nameof(Customer));
        OnPropertyChanged(nameof(Order));
        OnPropertyChanged(nameof(Items));
        OnPropertyChanged(nameof(hasOrder));
        OnPropertyChanged(nameof(FillableOrders));
        OnPropertyChanged(nameof(FillableOrdersMasterList));
        OnPropertyChanged(nameof(OrderedVsReceivedItems));
    }

    internal void QueryFillableOrders(string text)
    {
        FillableOrders.Clear();
        var orders = FillableOrdersMasterList.Where(o => o.OrderID.ToString().Contains(text) || 
                                                    o.Customer.CustomerName.Contains(text, StringComparison.CurrentCultureIgnoreCase)).ToList();
        foreach (var order in orders)
        {
            FillableOrders.Add(order);
        }
    }

    internal async Task ReceiveItemAsync(ShippingItem item)
    {
        bool duplicate = await App.BLDatabase.ShipDetails.IsDuplicateScanline(item.Scanline);
        if (!duplicate)
        {
            Items.Add(item);
            IncrementReceivedItem(item);
        }
        else
        {
            throw new DuplicateBarcodeException("Duplicate Scanline", item.Scanline);
        }
    }
    #endregion Methods


}
