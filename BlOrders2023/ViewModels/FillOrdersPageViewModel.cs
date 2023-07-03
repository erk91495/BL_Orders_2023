using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Exceptions;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Dispatching;
using Newtonsoft.Json.Bson;
using System.Collections.ObjectModel;
using Windows.UI.Text;

namespace BlOrders2023.ViewModels;

public class FillOrdersPageViewModel : ObservableRecipient, INavigationAware
{
    #region Properties
    public bool HasOrder { get => _order != null; }
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

    public async void OnNavigatedTo(object parameter)
    {
        var orderID = parameter as int?;
        await LoadFillableOrders();
        if (orderID != null)
        {
            await LoadOrder((int)orderID); 
        }
    }

    public void OnNavigatedFrom() { }

    private async Task LoadFillableOrders()
    {
        IOrderTable table = App.GetNewDatabase().Orders;
        var orders = await Task.Run(() => table.GetAsync());
        FillableOrders.Clear();
        FillableOrdersMasterList.Clear();
        foreach (var order in orders.Where( e => e.OrderStatus == Models.Enums.OrderStatus.Ordered ||
                                            e.OrderStatus ==  Models.Enums.OrderStatus.Filling || e.OrderStatus == Models.Enums.OrderStatus.Filling).ToList())
        {
            FillableOrders.Add(order);
            FillableOrdersMasterList.Add(order);
        }
    }

    public async Task LoadOrder(int orderID)
    {
        IOrderTable table = App.GetNewDatabase().Orders;
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

        //Calculate ordered
        foreach(var item in _order!.Items)
        {
            //If i would just build an observable dictionary i wouldnt have to do this 
            if (OrderedVsReceivedItems.Where(e => e.ProductID == item.ProductID).FirstOrDefault() != null)
            {
                OrderedVsReceivedItems.Where(e => e.ProductID == item.ProductID).First().Ordered += (int)item.Quantity;
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
        //Calculate Received
        foreach(var item in _order.ShippingItems)
        {
           IncrementReceivedItem(item);
        }
        OnPropertyChanged(nameof(OrderedVsReceivedItems));
    }

    private void IncrementReceivedItem(ShippingItem item)
    {
        if (OrderedVsReceivedItems.Where(e => e.ProductID == item.ProductID).FirstOrDefault() != null)
        {
            OrderedVsReceivedItems.Where(e => e.ProductID == item.ProductID).First().Received += (int)(item.QuanRcvd ?? 0);
            OrderedVsReceivedItems.Where(e => e.ProductID == item.ProductID).First().Weight += item.PickWeight ?? 0;
        }
        else
        {
            OrderedVsReceivedItem ovsr = new()
            {
                ProductID = item.ProductID,
                Ordered = 0,
                Received = (int)(item.QuanRcvd ?? 0),
                Weight = item.PickWeight ?? 0,
            };
            OrderedVsReceivedItems.Add(ovsr);
        }
        OnPropertyChanged(nameof(OrderedVsReceivedItems));
        
    }
    private void DecrementReceivedItem(ShippingItem item)
    {
        var foundItem = OrderedVsReceivedItems.Where(e => e.ProductID == item.ProductID).FirstOrDefault();
        if (foundItem != null)
        {
            // pre-decrement should make this so that when i get to zero the item is removed from the list
            if(--foundItem.Received <= 0 && foundItem.Ordered <= 0)
            {
                OrderedVsReceivedItems.Remove(foundItem);
            }
            else
            {
                foundItem.Weight -= item.PickWeight ?? 0;
            }
        }
        OnPropertyChanged(nameof(OrderedVsReceivedItems));
    }

    private void OnAllPropertiesChanged()
    {
        OnPropertyChanged(nameof(Customer));
        OnPropertyChanged(nameof(Order));
        OnPropertyChanged(nameof(Items));
        OnPropertyChanged(nameof(HasOrder));
        OnPropertyChanged(nameof(FillableOrders));
        OnPropertyChanged(nameof(FillableOrdersMasterList));
        OnPropertyChanged(nameof(OrderedVsReceivedItems));
    }

    internal void QueryFillableOrders(string text)
    {
        FillableOrders.Clear();
        var orders = FillableOrdersMasterList.Where(o => o.OrderID.ToString().Contains(text) || 
                                                    o.Customer.CustomerName.Contains(text, StringComparison.CurrentCultureIgnoreCase)).ToList();
        if (!orders.IsNullOrEmpty())
        {
            foreach (var order in orders)
            {
                FillableOrders.Add(order);
            }
        }
    }

    internal async Task ReceiveItemAsync(ShippingItem item)
    {
        bool duplicate = await App.GetNewDatabase().ShipDetails.IsDuplicateScanline(item.Scanline);
        if (!duplicate)
        {
            Items.Add(item);
            _order?.ShippingItems.Add(item);
            IncremantOrderedItem(item);
            await App.GetNewDatabase().Orders.UpsertAsync(_order);
            IncrementReceivedItem(item);
            
            
        }
        else
        {
            throw new DuplicateBarcodeException("Duplicate Scanline", item.Scanline);
        }
    }

    internal async Task DeleteShippingItemAsync(ShippingItem item)
    {
        Items.Remove(item);
        if (!_order!.ShippingItems.Remove(item))
        {
            throw new Exception();
        }
        
        DecrementOrderedItem(item);
        await App.GetNewDatabase().ShipDetails.DeleteAsync(item);
        DecrementReceivedItem(item);


    }


    private void IncremantOrderedItem(ShippingItem item)
    {
        var ordered = _order!.Items.Where(e => e.ProductID == item.ProductID).FirstOrDefault();
        if (ordered == null)
        {
            OrderItem orderItem = new(item.Product, _order)
            {
                Quantity = 0,
                PickWeight = item.PickWeight,
            };
            _order.Items.Add(orderItem);
        }
        else
        {
            ordered.PickWeight += item.PickWeight;
        }
    }

    private void DecrementOrderedItem(ShippingItem item)
    {
        var ordered = _order!.Items.Where(e => e.ProductID == item.ProductID).FirstOrDefault();
        if (ordered == null)
        {
            throw new Exception("Cannot Delete Item. Item does not exist");
        }
        else
        {
            ordered.PickWeight -= item.PickWeight;
            if(ordered.QuantityReceived <= 0 && ordered.Quantity <=0)
            {
                _order.Items.Remove(ordered);
            }
        }
    }


    internal Task DeleteAllShippingItemsAsync()
    {
        throw new NotImplementedException();
        //need to handle order items
        //Items.Clear();
        //_order.ShippingItems.Clear();
        //await App.GetNewDatabase().ShipDetails.UpsertAsync(_order.ShippingItems);
        //ReCalculateOrderdVsReceived();

    }
    #endregion Methods


}
