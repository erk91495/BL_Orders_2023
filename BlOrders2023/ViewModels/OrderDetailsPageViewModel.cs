using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
namespace BlOrders2023.ViewModels;

public class OrderDetailsPageViewModel : ObservableValidator, INavigationAware
{
    #region Properties
    public bool HasChanges 
    { 
        get; 
        set; 
    } = false;

    public ObservableCollection<OrderItem> Items
    {
        get => _items;
        set
        {
            if (_items != value)
            {
                value.CollectionChanged += LineItems_Changed;
            }

            if (_items != null)
            {
                _items.CollectionChanged -= LineItems_Changed;
            }

            _items = value;
            OnPropertyChanged();
        }

    }

    /// <summary>
    /// Gets a list of _products for the auto suggest box
    /// </summary>
    public ObservableCollection<Product> SuggestedProducts => _suggestedProducts;

    public int OrderID
    {
        get => _order.OrderID;
        set
        {
            if(value != _order.OrderID)
            {
                HasChanges = true;
                _order.OrderID = value;
                OnPropertyChanged();
            }

        }
    }

    public WholesaleCustomer Customer
    {
        get => _order.Customer;
        private set => _order.Customer = value;
    }

    public OrderStatus OrderStatus
    {
        get => _order.OrderStatus;
        set 
        {
            if (value != _order.OrderStatus)
            {
                HasChanges = true;
                _order.OrderStatus = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanPrintInvoice));
                OnPropertyChanged(nameof(CanPrintOrder));
                OnPropertyChanged(nameof(CanDeleteItems));
                OnPropertyChanged(nameof(CanAddItems));
                //check if we have errors for loading in a new order
                if(!HasErrors){
                    SaveCurrentOrder();
                }
            }
        }
    }

    public string? PO_Number
    {
        get => _order.PO_Number;
        set
        {
            if (value != _order.PO_Number)
            {
                HasChanges = true;
                if (value.IsNullOrEmpty())
                {
                    _order.PO_Number = null;
                }
                else
                {
                    _order.PO_Number = value;
                }
                OnPropertyChanged();
            }
        }
    }

    [Required]
    [CustomValidation(typeof(OrderDetailsPageViewModel), nameof(ValidateShipping), ErrorMessage = "A Shipping Type Must Be Selected")]
    public ShippingType Shipping
    {
        get => _order.Shipping;
        set
        {
            if (value != _order.Shipping)
            {
                HasChanges = true;
                _order.Shipping = value;
                CheckValidation(value, nameof(Shipping));
                OnPropertyChanged();
            }
        }
    }

    [Required]
    [Display(Name = "Taken By")]
    public string TakenBy
    {
        get => _order.TakenBy;
        set
        {
            if (value != _order.TakenBy)
            {
                HasChanges = true;
                _order.TakenBy = value.Trim();
                CheckValidation(value, nameof(TakenBy));
                OnPropertyChanged();
            }
        }
    }

    //Need to handle opening an old order before i can re enable this
    //[CustomValidation(typeof(OrderDetailsPageViewModel), nameof(ValidatePickupDate),ErrorMessage = "Pickup\\Delivery Date cannot be in the past")]
    public DateTime PickupDate
    {
        get => _order.PickupDate;
        set
        {
            if (value != _order.PickupDate)
            {
                HasChanges = true;
                _order.PickupDate = value;
                CheckValidation(value, nameof(PickupDate));
                OnPropertyChanged();
            }
        }
    }

    public DateTime PickupTime
    {
        get => _order.PickupTime;
        set
        {
            if (value != _order.PickupTime)
            {
                HasChanges = true;
                _order.PickupTime = value;
                OnPropertyChanged();
            }
        }
    }

    public bool? Frozen
    {
        get => _order.Frozen;
        set
        {
            if (value != _order.Frozen)
            {
                HasChanges = true;
                _order.Frozen = value;
                OnPropertyChanged();
            }
        }
    }

    [MaxLength(255)]
    public string? Memo
    {
        get => _order.Memo;
        set
        {
            if (value != _order.Memo)
            {
                HasChanges = true;
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
            }
        }
    }

    public decimal? Memo_Totl
    {
        get => _order.Memo_Totl;
        set
        {
            if (value != _order.Memo_Totl)
            {
                HasChanges = true;
                _order.Memo_Totl = value;
                OnPropertyChanged();
            }
        }
    }

    public float? Memo_Weight
    {
        get => _order.Memo_Weight;
        set
        {
            if (value != _order.Memo_Weight)
            {
                HasChanges = true;
                _order.Memo_Weight = value;
                OnPropertyChanged();
            }
        }
    }

    public bool? Allocated
    {
        get => _order.Allocated;
        set
        {
            if (value != _order.Allocated)
            {
                HasChanges = true;
                _order.Allocated = value;
                OnPropertyChanged();
            }
        }
    }

    public Order Order => _order;

    public bool HasNextOrder { get; set; } = false;
    public bool HasPreviousOrder { get; set; } = false;

    public bool CanAddItems => !HasErrors && _order.OrderStatus == OrderStatus.Ordered;
    public bool CanDeleteItems => _order.OrderStatus < OrderStatus.Filling;

    public bool CanPrintInvoice => _order.CanPrintInvoice;
    public bool CanPrintOrder => _order.CanPrintOrder;
    public bool AllItemsScanned => _order.AllItemsReceived;
    public bool IsNewOrder {get; set;} = true;
    #endregion Properties

    #region Fields
    private Order _order;
    private readonly ObservableCollection<Product> _suggestedProducts;
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private ObservableCollection<OrderItem> _items;
    private int _currentOrderIndex;
    private readonly IBLDatabase _db = App.GetNewDatabase();
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Creates a new instance of OrderDetailsPageViewModel
    /// </summary>
    public OrderDetailsPageViewModel()
    {
        _currentOrderIndex = 0;
        _order = new();
        _suggestedProducts = new();
        _items = new();
        LoadProducts();
    }
    #endregion Constructors

    #region Methods

    /// <summary>
    /// Checks if _items contains a product with the given id
    /// </summary>
    /// <param name="id">The id of the Prodcut to check for</param>
    /// <returns></returns>
    public bool OrderItemsContains(int id)
    {
        return _items.FirstOrDefault(i => i!.ProductID == id, null) != null;
    }
    /// <summary>
    /// Notifies anyone listening to this object that a line item changed. 
    /// </summary>
    private void LineItems_Changed(object? _sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Items));
    }

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is Order order)
        {
            var entityOrder = _db.Orders.Get(order.OrderID).FirstOrDefault();
            if (entityOrder != null)
            {
                //We want to track changes so get it from the db context
                _order = entityOrder;
                IsNewOrder = false;
            }
            else
            {
                //Must be a new Order
                _order = order;
                IsNewOrder = true;
                if (_order.Customer.COD)
                {
                    Memo = "Cash on Delivery";
                }
            }
            _items = new ObservableCollection<OrderItem>(_order.Items);
            _currentOrderIndex = _order.Customer.Orders.OrderBy(o => o.OrderID).ToList().IndexOf(_order);
            HasNextOrder = _currentOrderIndex < _order.Customer.Orders.Count - 1;
            HasPreviousOrder = _currentOrderIndex > 0;

            OnAllPropertiesChanged();
        }
        //Validate this to disable the product entry box on a new order
        ValidateProperty(Shipping, nameof(Shipping));
    }

    public void OnNavigatedFrom()
    {

    }

    public void AddItem(Product p, int? quantity = null, decimal? actualCustomerPrice = null)
    {
        var tracked = _db.Products.Get(p.ProductID, true).First();
        OrderItem item = new(tracked, _order);
        if(quantity != null) 
        {
            item.Quantity = (float)quantity;
        }
        if(actualCustomerPrice != null)
        {
            item.ActualCustPrice = (decimal)actualCustomerPrice;
        }
        Items.Add(item);
        HasChanges = true;
    }

    private void OnAllPropertiesChanged()
    {
        OnPropertyChanged(nameof(Items));
        OnPropertyChanged(nameof(OrderID));
        OnPropertyChanged(nameof(Customer));
        OnPropertyChanged(nameof(OrderStatus));
        OnPropertyChanged(nameof(PO_Number));
        OnPropertyChanged(nameof(Shipping));
        OnPropertyChanged(nameof(TakenBy));
        OnPropertyChanged(nameof(PickupDate));
        OnPropertyChanged(nameof(PickupTime));
        OnPropertyChanged(nameof(Frozen));
        OnPropertyChanged(nameof(Memo));
        OnPropertyChanged(nameof(Memo_Totl));
        OnPropertyChanged(nameof(HasNextOrder));
        OnPropertyChanged(nameof(HasPreviousOrder));
    }

    public int? GetNextOrderID()
    {
        if (HasNextOrder)
        {
            return _order.Customer.Orders.OrderBy(o => o.OrderID).ToList()[_currentOrderIndex + 1].OrderID;
        }
        return null;
    }
    public Order? GetNextOrder()
    {
        if (HasNextOrder)
        {
            return _order.Customer.Orders.OrderBy(o => o.OrderID).ToList()[_currentOrderIndex + 1];
        }
        return null;
    }

    public int? GetPreviousOrderID()
    {
        if (HasPreviousOrder)
        {
            return _order.Customer.Orders.OrderBy(o => o.OrderID).ToList()[_currentOrderIndex - 1].OrderID;
        }
        return null;
    }

    public Order? GetPreviousOrder()
    {
        if (HasPreviousOrder)
        {
            return _order.Customer.Orders.OrderBy(o => o.OrderID).ToList()[_currentOrderIndex - 1];
        }
        return null;
    }
    internal void ReloadOrder()
    {
        _order = _db.Orders.Reload(_order);
        _items = new ObservableCollection<OrderItem>(_order.Items);
        _currentOrderIndex = _order.Customer.Orders.OrderBy(o => o.OrderID).ToList().IndexOf(_order);
        HasNextOrder = _currentOrderIndex < _order.Customer.Orders.Count - 1;
        HasPreviousOrder = _currentOrderIndex > 0;
        OnAllPropertiesChanged();
    }



    #region Queries
    /// <summary>
    /// Queries the database for a list of all _products and adds them to SuggestedProducts
    /// </summary>
    public async void LoadProducts()
    {
        await dispatcherQueue.EnqueueAsync(() =>
        {
            SuggestedProducts.Clear();
        });

        IProductsTable table = _db.Products;
        var products = await Task.Run(() => table.GetAsync((int?)null, false));

        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (var product in products)
            {
                SuggestedProducts.Add(product);
            }
        });
    }

    /// <summary>
    /// Queries the database for a list of _products that match the given string
    /// </summary>
    /// <param name="query">A string for _products to match</param>
    public async Task QueryProducts(string query = null)
    {
        if (query.IsNullOrEmpty())
        {
            LoadProducts();
        }
        else
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                SuggestedProducts.Clear();
            });

            IProductsTable table = _db.Products;
            var products = await Task.Run(() => table.GetAsync(query, false));

            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (var product in products)
                {
                    SuggestedProducts.Add(product);
                }
            });
        }
    }
    /// <summary>
    /// Saves changes to the current Order
    /// </summary>
    public async Task SaveCurrentOrderAsync()
    {
        if(HasChanges){
            HasChanges = false;
            _order.Items = Items;
            await _db.Orders.UpsertAsync(_order);
            if (IsNewOrder)
            {
                IsNewOrder = false;
                ReloadOrder();
            }
        }
    }

    /// <summary>
    /// Saves changes to the current Order
    /// </summary>
    public void SaveCurrentOrder(bool overwrite = false)
    {
        if(HasChanges) {
            HasChanges = false;
            _order.Items = Items;
            _db.Orders.Upsert(_order, overwrite);
            if (IsNewOrder)
            {
                IsNewOrder = false;
                // Want to pick up DB generated OrderID
                ReloadOrder();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameter"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void BeforeNavigatedFrom(object? parameter)
    {
        throw new NotImplementedException();
    }

    internal async Task DeleteCurrentOrderAsync()
    {
        await _db.Orders.DeleteAsync(_order);
    }

    internal void DeleteCurrentOrder()
    {
        _db.Orders.Delete(_order);
    }
    #endregion Queries

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
            if(errors.IsNullOrEmpty())
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
        OnPropertyChanged(nameof(CanAddItems));
    }

    public static ValidationResult? ValidateShipping(ShippingType shipping)
    {
        var isValid = shipping != ShippingType.NoType;
        return isValid ? ValidationResult.Success : new ValidationResult("Shipping must be selected");
    }

    public static ValidationResult? ValidatePickupDate(DateTime dateTime)
    {
        var isValid = dateTime.CompareTo(DateTime.Today) >= 0;
        return isValid ? ValidationResult.Success : new ValidationResult("Pickup Date cannot be in the past");
    }

    public void ValidateProperties()
    {
        ValidateAllProperties();
    }

    public void ResetAllocation()
    {
            List<OrderItem> itemsCopy = new (Items);
            foreach(var item in itemsCopy) 
            {
            //is credit items are not allocated so do nothing for them
                if(!item.Product.IsCredit){
                    if (item.QuantityReceived == 0 && item.Quantity == 0)
                    {
                        Items.Remove(item);
                    }
                    else
                    {
                        item.Allocated = null;
                        item.QuanAllocated = 0;
                    }
                }
            }
            Order.Allocated = false;
    }
    #endregion Validators
    #endregion Methods
}
