using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Exceptions;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Newtonsoft.Json.Bson;
using ServiceStack.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Windows.UI.Text;

namespace BlOrders2023.ViewModels;

public class FillOrdersPageViewModel : ObservableValidator, INavigationAware
{
    #region Properties
    public bool HasOrder => _order != null;
    public WholesaleCustomer Customer { get; set; }
    public Order? Order { get => _order; set => _order = value; }
    public IEnumerable<OrderItem>? SortedOrderItems => _order?.Items.Where(i => i.Product.IsCredit != true).OrderBy(i => i.ProductID);
    public ObservableCollection<ShippingItem> Items { get; set; }
    public ObservableCollection<Order> FillableOrders { get; set; }
    public ObservableCollection<Order> FillableOrdersMasterList { get; set; }
    public OrderStatus? OrderStatus
    {
        get => _order == null ? null : _order.OrderStatus; 
        set
        {
            _order!.OrderStatus = (OrderStatus)value!;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanPrintInvoice));
            OnPropertyChanged(nameof(CanPrintOrder));
        }
    }

    [MaxLength(255)]
    public string? Memo
    {
        get => _order?.Memo;
        set
        {
            if (value.IsNullOrEmpty())
            {
                _order.Memo = null;
            }
            else
            {
                _order.Memo = value?.Trim();
            }
            CheckValidation(Memo, nameof(Memo));
            OnPropertyChanged();
            if (!HasErrors)
            {
                _ = SaveOrderAsync();
            }
        }
    }

    public bool CanPrintInvoice => _order?.CanPrintInvoice ?? false;
    public bool CanPrintOrder => _order?.CanPrintOrder ?? false;

    public int TotalOrdered =>  _order != null ? (int)(_order.Items.Sum(i => i.Quantity)) : 0;
    public int TotalReceived => _order != null ? _order.ShippingItems.Sum(i => i.QuanRcvd) ?? 0 : 0;
    #endregion Properties

    #region Fields
    private Order? _order;
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private readonly IBLDatabase _orderDB = App.GetNewDatabase();
    private readonly SemaphoreSlim _SaveSemaphore = new(1);
    private readonly SemaphoreSlim _AddItemSemaphore = new(1);
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
                                            e.OrderStatus ==  Models.Enums.OrderStatus.Filling || e.OrderStatus == Models.Enums.OrderStatus.Filled).OrderBy(o => o.PickupDate).ToList())
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
        OnPropertyChanged(nameof(CanPrintInvoice));
        OnPropertyChanged(nameof(CanPrintOrder));
        OnPropertyChanged(nameof(Memo));
        OnPropertyChanged(nameof(SortedOrderItems));
        OnPropertyChanged(nameof(OrderStatus));
        OnPropertyChanged(nameof(TotalOrdered));
        OnPropertyChanged(nameof(TotalReceived));
    }

    internal void QueryFillableOrders(string text)
    {
        FillableOrders.Clear();
        List<Order> orders;
        if (text.IsNullOrEmpty())
        {
            orders = FillableOrdersMasterList.ToList();
        }
        else
        {
             orders = FillableOrdersMasterList.Where(o => o.OrderID.ToString().Contains(text) || 
                                                        o.Customer.CustomerName.Contains(text, StringComparison.CurrentCultureIgnoreCase)).OrderBy(o => o.PickupDate).ToList();
        }
        if (!orders.IsNullOrEmpty())
        {
            foreach (var order in orders)
            {
                FillableOrders.Add(order);
            }
        }
    }

    internal async Task ReceiveItemAsync(ShippingItem item, bool checkDuplicate = true)
    {
        await _AddItemSemaphore.WaitAsync();
        await _SaveSemaphore.WaitAsync();
        try
        {
            //Mainly for canned stuff where upc is the scanline
            //A USP could make this 1 query instead of 2
            var product = _orderDB.Products.GetByALU(item.Scanline) ?? _orderDB.Products.Get(item.ProductID).FirstOrDefault();
            if (product == null)
            {
                throw new ProductNotFoundException(string.Format("Product {0} Not Found", item.ProductID), item.ProductID);
            }
            else
            {
                item.Product = product;
                item.ProductID = product.ProductID;
            }

            if(checkDuplicate)
            {
                var duplicate = await _orderDB.ShipDetails.IsDuplicateScanline(item.Scanline);
                if (duplicate)
                {
                    throw new DuplicateBarcodeException("Duplicate Scanline", item.Scanline);
                } 
            }
            Items.Add(item);
            _order?.ShippingItems.Add(item);
            IncremantOrderedItem(item);
            OnPropertyChanged(nameof(TotalReceived));
            //await SaveOrderAsync();
        }
        finally
        {
            _SaveSemaphore.Release();
            _AddItemSemaphore.Release();
        }
    }

    internal async Task DeleteShippingItemAsync(ShippingItem item)
    {
        Items.Remove(item);
        await Task.Run(() => 
        {

            if (!_order!.ShippingItems.Remove(item))
            {
                //throw new Exception();
            }
            else
            {
                DecrementOrderedItem(item);
            }
        });
        OnPropertyChanged(nameof(SortedOrderItems));
        OnPropertyChanged(nameof(Items));
        OnPropertyChanged(nameof(TotalReceived));
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
            orderItem.IncrementQuanRcvd(1);
            orderItem.IncrementPickWeight(item.PickWeight ?? 0);
        }
        else
        {
            ordered.IncrementQuanRcvd(1);
            ordered.IncrementPickWeight(item.PickWeight ?? 0);

        }
    }

    private async void DecrementOrderedItem(ShippingItem item)
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
                await dispatcherQueue.EnqueueAsync(() => 
                {
                    _order.Items.Remove(ordered);
                });

            }
            else
            {
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    ordered.IncrementQuanRcvd(-1);
                    ordered.IncrementPickWeight(-1 * item.PickWeight ?? 0);
                });   
            }
        }
    }


    internal async Task DeleteAllShippingItemsAsync()
    {
        Items.Clear();
        await Task.Run(() =>
        {
            _order.ShippingItems.Clear();
            var itemsCopy = new ObservableCollection<OrderItem>(_order.Items);
            foreach (var item in itemsCopy)
            {
                if(!item.Product.IsCredit && item.Quantity == 0 && item.QuantityReceived ==0 && item.QuanAllocated == 0)
                {
                    _order.Items.Remove(item);
                }
            }
        });
        OnPropertyChanged(nameof(Items));
    }

    internal async Task SaveOrderAsync()
    {
        await _SaveSemaphore.WaitAsync();
        try
        {
            await _orderDB.Orders.UpsertAsync(_order);
        }
        finally
        {
            _SaveSemaphore.Release();
        }
    }

    #region Validators
    public string GetErrorMessage(string name)
    {
        var errors = GetErrors(name);
        var firstError = errors.FirstOrDefault();
        if (firstError != null)
        {
            return firstError.ErrorMessage ?? "Error";
        }
        return string.Empty;


    }

    public bool HasError(string name)
    {
        if (HasErrors)
        {
            var errors = GetErrors(name);
            if (errors.IsNullOrEmpty())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    public Visibility VisibleIfError(string name)
    {
        if (HasError(name))
        {
            return Visibility.Visible;
        }
        else
        {
            return Visibility.Collapsed;
        }
    }

    private void CheckValidation(object? value, string propertyName = null!)
    {
        ValidateProperty(value, propertyName);
        OnPropertyChanged(nameof(GetErrorMessage));
        OnPropertyChanged(nameof(VisibleIfError));
    }

    public void ValidateProperties()
    {
        ValidateAllProperties();
    }
    #endregion Validators
    #endregion Methods


}
