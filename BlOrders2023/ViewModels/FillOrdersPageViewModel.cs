using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Exceptions;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Dispatching;
using Newtonsoft.Json.Bson;
using ServiceStack.DataAnnotations;
using System.Collections.ObjectModel;
using Windows.UI.Text;

namespace BlOrders2023.ViewModels;

public class FillOrdersPageViewModel : ObservableRecipient, INavigationAware
{
    #region Properties
    public bool HasOrder => _order != null;
    public WholesaleCustomer Customer { get; set; }
    public Order? Order { get => _order; set => _order = value; }
    public ObservableCollection<ShippingItem> Items { get; set; }
    public ObservableCollection<Order> FillableOrders { get; set; }
    public ObservableCollection<Order> FillableOrdersMasterList { get; set; }
    #endregion Properties

    #region Fields
    private Order? _order;
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private readonly IBLDatabase _orderDB = App.GetNewDatabase();
    #endregion Fields

    #region Consturctors
    public FillOrdersPageViewModel()
    {
        Customer = new();
        _order = null;
        Items = new();
        FillableOrders = new();
        FillableOrdersMasterList = new();

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
        IOrderTable table = _orderDB.Orders;
        var order = await Task.Run(() => table.GetAsync(orderID));

        await dispatcherQueue.EnqueueAsync(() =>
        {
            _order = order.First();
            Customer = _order.Customer;
            Items = new ObservableCollection<ShippingItem>( _order.ShippingItems.ToList());
            //ReCalculateOrderdVsReceived();
            OnAllPropertiesChanged();  
        });

    }

    private void OnAllPropertiesChanged()
    {
        OnPropertyChanged(nameof(Customer));
        OnPropertyChanged(nameof(Order));
        OnPropertyChanged(nameof(Items));
        OnPropertyChanged(nameof(HasOrder));
        OnPropertyChanged(nameof(FillableOrders));
        OnPropertyChanged(nameof(FillableOrdersMasterList));
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
        var product = _orderDB.Products.Get(item.ProductID).FirstOrDefault();
        if(product == null)
        {
            throw new ProductNotFoundException(string.Format("Product {0} Not Found", item.ProductID), item.ProductID);
        }
        else
        {
            item.Product = product;
        }

        var duplicate = await _orderDB.ShipDetails.IsDuplicateScanline(item.Scanline);
        if (!duplicate)
        {
            Items.Add(item);
            _order?.ShippingItems.Add(item);
            IncremantOrderedItem(item);
            await _orderDB.Orders.UpsertAsync(_order);
            
        }
        else
        {
            throw new DuplicateBarcodeException("Duplicate Scanline", item.Scanline);
        }
    }

    internal async Task DeleteShippingItemAsync(ShippingItem item)
    {
        if(!Items.Remove(item))
        {
            if (!_order!.ShippingItems.Remove(item))
            {
                //throw new Exception();
            }
            else
            {
                DecrementOrderedItem(item);
                await _orderDB.Orders.UpsertAsync(_order);
            }
            OnPropertyChanged(nameof(Items));
        }

    }


    private void IncremantOrderedItem(ShippingItem item)
    {
        var ordered = _order!.Items.Where(e => e.ProductID == item.ProductID).FirstOrDefault();
        if (ordered == null)
        {
            
            OrderItem orderItem = new(item.Product, _order)
            {
                Quantity = 0,

            };
            _order.Items.Add(orderItem);
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
            if(ordered.QuantityReceived <= 0 && ordered.Quantity <=0)
            {
                _order.Items.Remove(ordered);
                _orderDB.Orders.Upsert(_order);
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

    internal async Task SaveOrderAsync() => await _orderDB.Orders.UpsertAsync(_order);
    #endregion Methods


}
