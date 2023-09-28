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
    private int orderID;
    private DateTime orderDate;
    private int custID;
    private WholesaleCustomer customer = null!;
    private string takenBy = "";
    private DateTime pickupDate;
    private DateTime pickupTime;
    private ShippingType shipping;
    private byte boxed;
    private bool? frozen;
    private bool? shipped;
    private short? filled;
    private short? filledBox;
    private string? memo;
    private float? memo_Weight;
    private decimal? memo_Totl;
    private short? net;
    private string? pO_Number;
    private bool? printed;
    private bool? oKToProcess;
    private bool? paid;
    private bool? allocated;
    private OrderStatus orderStatus;
    private ObservableCollection<OrderItem> items;
    private List<ShippingItem> shippingItems = new();
    private List<Payment> payments = new();
    #region Properties
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderID
    {
        get => orderID; 
        set => SetProperty(ref orderID,value);
    }
    public DateTime OrderDate
    {
        get => orderDate; 
        set => SetProperty(ref orderDate, value);
    }
    public int CustID
    {
        get => custID; 
        set => SetProperty(ref custID, value);
    }

    [ForeignKey("CustID")]
    [JsonIgnore]
    public virtual WholesaleCustomer Customer
    {
        get => customer; 
        set => SetProperty(ref customer, value);
    }
    public string TakenBy
    {
        get => takenBy; 
        set => SetProperty(ref takenBy, value);
    }
    public DateTime PickupDate
    {
        get => pickupDate; 
        set => SetProperty(ref pickupDate, value);
    }
    public DateTime PickupTime
    {
        get => pickupTime; 
        set => SetProperty(ref pickupTime, value);
    }
    public ShippingType Shipping
    {
        get => shipping; 
        set => SetProperty(ref shipping, value);
    }
    public byte Boxed
    {
        get => boxed; 
        set => SetProperty(ref boxed, value);
    }
    public bool? Frozen
    {
        get => frozen; 
        set => SetProperty(ref frozen, value);
    }
    public bool? Shipped
    {
        get => shipped; 
        set => SetProperty(ref shipped, value);
    }
    public short? Filled
    {
        get => filled; 
        set => SetProperty(ref filled, value);
    }
    public short? FilledBox
    {
        get => filledBox; 
        set => SetProperty(ref filledBox, value);
    }
    public string? Memo
    {
        get => memo; 
        set => SetProperty(ref memo, value);
    }
    public float? Memo_Weight
    {
        get => memo_Weight; 
        set => SetProperty(ref memo_Weight, value);
    }
    public decimal? Memo_Totl
    {
        get => memo_Totl; 
        set => SetProperty(ref memo_Totl, value);
    }
    public short? Net
    {
        get => net; 
        set => SetProperty(ref net, value);
    }
    public string? PO_Number
    {
        get => pO_Number; 
        set => SetProperty(ref pO_Number, value);
    }
    public bool? Printed
    {
        get => printed; 
        set => SetProperty(ref printed, value);
    }
    public bool? OKToProcess
    {
        get => oKToProcess; 
        set => SetProperty(ref oKToProcess, value);
    }
    public bool? Paid
    {
        get => paid; 
        set => SetProperty(ref paid, value);
    }
    public bool? Allocated
    {
        get => allocated; 
        set => SetProperty(ref allocated, value);
    }
    public OrderStatus OrderStatus
    {
        get => orderStatus; 
        set => SetProperty(ref orderStatus, value);
    }
    public virtual ObservableCollection<OrderItem> Items
    {
        get => items; 
        set    
        {
            if (items != null)
            {
                items.CollectionChanged -= Items_CollectionChanged;
            }
            
            
            if(value != null)
            {
                value.CollectionChanged += Items_CollectionChanged;
            }
            SetProperty(ref items, value);
        }
    }
    public virtual List<ShippingItem> ShippingItems
    {
        get => shippingItems; 
        set => SetProperty(ref shippingItems, value);
    }
    public virtual List<Payment> Payments
    {
        get => payments; 
        set => SetProperty(ref payments, value);
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
    #endregion Properties

    #region Fields
    #endregion Fields

    #region Constructors
    public Order()
    {
        Memo_Totl = 0M;
        Memo_Weight = 0;
        OrderDate = DateTime.Now;
        Frozen = false;
        //Set the date for today so that sql will accept the time
        PickupDate = DateTime.Today;
        PickupTime = DateTime.Today.AddHours(12);
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
                var total =  Items.Sum(item => (int)item.Quantity);
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
