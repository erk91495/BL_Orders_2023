using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BlOrders2023.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using ServiceStack;

namespace BlOrders2023.Models;
[Table("tblOrdersWholesale")]
public class Order : ObservableObject
{
    #region Fields
    private int _orderID;
    private DateTime _orderDate;
    private int _custID;
    private WholesaleCustomer _customer;
    private string _takenBy;
    private DateTime _pickupDate;
    private DateTime _pickupTime;
    private ShippingType _shipping;
    private byte _boxed;
    private bool? _frozen;
    //private bool? _shipped;
    //private short? _filled;
    //private short? _filledBox;
    private string? _memo;
    private float? _memoWeight;
    private decimal? _memoTotl;
    //private short? _net;
    private string? _poNumber;
    //private bool? _printed;
    //private bool? _oKToProcess;
    private bool? _paid;
    private bool? _allocated;
    private OrderStatus _orderStatus;
    private ObservableCollection<OrderItem> _items;
    private List<ShippingItem> _shippingItems = new();
    private List<Payment> _payments = new();
    private bool _palletTicketPrinted = false;
    #endregion Fields

    #region Properties
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderID
    {
        get => _orderID; 
        set => SetProperty(ref _orderID,value);
    }
    public DateTime OrderDate
    {
        get => _orderDate; 
        set => SetProperty(ref _orderDate, value);
    }
    public int CustID
    {
        get => _custID; 
        set => SetProperty(ref _custID, value);
    }

    [ForeignKey("CustID")]
    [JsonIgnore]
    public virtual WholesaleCustomer Customer
    {
        get => _customer; 
        set => SetProperty(ref _customer, value);
    }
    public string TakenBy
    {
        get => _takenBy; 
        set => SetProperty(ref _takenBy, value);
    }
    public DateTime PickupDate
    {
        get => _pickupDate; 
        set => SetProperty(ref _pickupDate, value);
    }
    public DateTime PickupTime
    {
        get => _pickupTime; 
        set => SetProperty(ref _pickupTime, value);
    }
    public ShippingType Shipping
    {
        get => _shipping; 
        set => SetProperty(ref _shipping, value);
    }
    public byte Boxed
    {
        get => _boxed; 
        set => SetProperty(ref _boxed, value);
    }
    public bool? Frozen
    {
        get => _frozen; 
        set => SetProperty(ref _frozen, value);
    }
    //public bool? Shipped
    //{
    //    get => _shipped; 
    //    set => SetProperty(ref _shipped, value);
    //}
    //public short? Filled
    //{
    //    get => _filled; 
    //    set => SetProperty(ref _filled, value);
    //}
    //public short? FilledBox
    //{
    //    get => _filledBox; 
    //    set => SetProperty(ref _filledBox, value);
    //}
    public string? Memo
    {
        get => _memo; 
        set => SetProperty(ref _memo, value);
    }
    public float? Memo_Weight
    {
        get => _memoWeight; 
        set => SetProperty(ref _memoWeight, value);
    }
    public decimal? Memo_Totl
    {
        get => _memoTotl; 
        set => SetProperty(ref _memoTotl, value);
    }
    //public short? Net
    //{
    //    get => _net; 
    //    set => SetProperty(ref _net, value);
    //}
    public string? PO_Number
    {
        get => _poNumber; 
        set => SetProperty(ref _poNumber, value);
    }
    //public bool? Printed
    //{
    //    get => _printed; 
    //    set => SetProperty(ref _printed, value);
    //}
    //public bool? OKToProcess
    //{
    //    get => _oKToProcess; 
    //    set => SetProperty(ref _oKToProcess, value);
    //}
    public bool? Paid
    {
        get => _paid;
        set => SetProperty(ref _paid, value);
    }
    public bool? Allocated
    {
        get => _allocated; 
        set => SetProperty(ref _allocated, value);
    }
    public OrderStatus OrderStatus
    {
        get => _orderStatus; 
        set => SetProperty(ref _orderStatus, value);
    }
    public virtual ObservableCollection<OrderItem> Items
    {
        get => _items; 
        set    
        {
            if (_items != null)
            {
                _items.CollectionChanged -= Items_CollectionChanged;
            }
            
            
            if(value != null)
            {
                value.CollectionChanged += Items_CollectionChanged;
            }
            SetProperty(ref _items, value);
        }
    }
    public virtual List<ShippingItem> ShippingItems
    {
        get => _shippingItems; 
        set => SetProperty(ref _shippingItems, value);
    }
    public virtual List<Payment> Payments
    {
        get => _payments; 
        set => SetProperty(ref _payments, value);
    }
    //Todo should this be a helper class?
    public bool CanFillOrder => (OrderStatus == OrderStatus.Ordered || OrderStatus == OrderStatus.Filling || OrderStatus == OrderStatus.Filled);
    public bool CanEditOrder => OrderStatus == OrderStatus.Ordered;
    public bool CanPrintInvoice =>  OrderStatus >= OrderStatus.Filling;
    public bool CanPrintOrder => OrderStatus == OrderStatus.Ordered;
    public bool AllItemsReceived
    {
        get
        {
            if(Items.IsNullOrEmpty()) return false;
            if (Allocated ?? false)
            {
                return Items.All(i => i.QuanAllocated == i.QuantityReceived);
            }
            else
            {
                return Items.All(i => i.Quantity == i .QuantityReceived);
            }
        }
    }

    public bool PalletTicketPrinted
    {
        get => _palletTicketPrinted;
        set => SetProperty(ref _palletTicketPrinted, value);
    }
    #endregion Properties

    #region Fields
    #endregion Fields

    #region Constructors
    public Order()
    {
        Customer = null!;
        TakenBy = string.Empty;
        Memo_Totl = 0M;
        Memo_Weight = 0;
        OrderDate = DateTime.Now;
        Frozen = false;
        //Set the date for today so that sql will accept the time
        _pickupDate = DateTime.Today;
        _pickupTime = DateTime.Today.AddHours(12);
        OrderStatus = OrderStatus.Ordered;
        Items = new();
        Allocated = false;
    }

    public Order(WholesaleCustomer customer)
        : this()
    {
        Customer = customer;
        CustID = customer.CustID;
    }
    #endregion Constructors


    #region Methods
    public decimal GetInvoiceTotal()
    {
        decimal total = 0;
        if (!Items.IsNullOrEmpty())
        {
            foreach(var item in Items)
            {
                total += item.GetTotalPrice;
            }
        }
        total += Memo_Totl ?? 0;
        return decimal.Round(total,2);
    }

    public int GetTotalOrdered()
    {
            if (Items.IsNullOrEmpty())
            {
                return 0;
            }
            else
            {
                var total =  Items.Where(i => i.Product.IsCredit != true).Sum(item => (int)item.Quantity);
                return total;
            }
    }

    public int GetTotalAllocated()
    {
            if (Items.IsNullOrEmpty())
            {
                return 0;
            }
            else
            {
                var total = Items.Sum(item => (int)(item.QuanAllocated));
                return total;
            }
    }

    public int GetTotalOrderedAllocated()
    {
        if (Items.IsNullOrEmpty())
        {
            return 0;
        }
        else
        {
            var total = Items.Where(i => i.Allocated == true).Sum(item => (int)(item.QuanAllocated));
            return total;
        }
    }

    public decimal GetTotalPayments()
    {
        if (Payments.IsNullOrEmpty())
        {
            return 0;
        }
        else
        {
            return Payments.Sum(p => p.PaymentAmount ?? 0);
        }
    }

    public decimal GetBalanceDue()
    {
        return GetInvoiceTotal() - GetTotalPayments();
    }

    private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
        {
            foreach (INotifyPropertyChanged added in e.NewItems)
            {
                added.PropertyChanged += ShippingItemOnPropertyChanged;
            }
        }
        if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
        {
            foreach (INotifyPropertyChanged removed in e.OldItems)
            {
                removed.PropertyChanged -= ShippingItemOnPropertyChanged;
            }
        }

        OnPropertyChanged(nameof(GetTotalAllocated));
        OnPropertyChanged(nameof(GetTotalOrdered));
    }

    private void ShippingItemOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(GetTotalAllocated));
        OnPropertyChanged(nameof(GetTotalOrdered));
    }

    public override string? ToString()
    {
        return OrderID.ToString();
    }


    #endregion Methods
}
