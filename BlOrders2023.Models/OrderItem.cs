using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlOrders2023.Models;

[Table("tblOrderDetails_2")]
public class OrderItem : ObservableObject
{
    #region Fields
    private int quanAllocated;
    private int orderID;
    private Order order = null!;
    private int productID;
    private float quantity;
    private decimal actualCustPrice;
    private int ordDtl_ID;
    private DateTime? prodEntryDate;
    private Product product = null!;
    private int extraNeeded;
    private bool? allocated;
    private int given;
    #endregion Fields

    #region Properties
    public int OrderID
    {
        get => orderID; 
        set => SetProperty(ref orderID, value);
    }
    [ForeignKey("OrderID")]
    [JsonIgnore]
    public virtual Order Order
    {
        get => order; 
        set => SetProperty(ref order, value);
    }
    public int ProductID
    {
        get => productID; 
        set => SetProperty(ref productID, value);
    }
    [ConcurrencyCheck]
    public float Quantity
    {
        get => Product.IsCredit ? 0 : quantity; 
        set
        {
            SetProperty(ref quantity, value);
            OnPropertyChanged(nameof(Given));
        }
    }
    public decimal ActualCustPrice
    {
        get => actualCustPrice; 
        set => SetProperty(ref actualCustPrice, value);
    }
    //public float QuanRcvd { get; set; }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrdDtl_ID
    {
        get => ordDtl_ID; 
        set => SetProperty(ref ordDtl_ID, value);
    }
    public DateTime? ProdEntryDate
    {
        get => prodEntryDate; 
        set => SetProperty(ref prodEntryDate, value);
    }
    [ForeignKey("ProductID")]
    public virtual Product Product
    {
        get => product; 
        set => SetProperty(ref product, value);
    }

    public int QuanAllocated
    {
        get => quanAllocated; 
        set
        { 
            SetProperty(ref quanAllocated, value);
            OnPropertyChanged(nameof(Given));
        }
    }

    public bool? Allocated
    {
        get => allocated; 
        set => SetProperty(ref allocated, value);
    }

    //Not in DB used for allocation
    [NotMapped]
    public int ExtraNeeded
    {
        get => extraNeeded; 
        set => SetProperty( ref extraNeeded, value);
    }

    /// <summary>
    /// If the item is allocated returns QuanAllocated else returns Quantity
    /// </summary>
    [NotMapped]
    public int Given => Allocated != true ?  (int)quantity : quanAllocated;

    [NotMapped]
    public float? PickWeight => Order.ShippingItems.Where(i => i.ProductID == ProductID).Sum(i => i.PickWeight ?? 0);
    [NotMapped]
    public int QuantityReceived => CalcQuantityReceived();
    #endregion Properties

    #region Constructors
    public OrderItem()
    {
        Quantity = 0;
        ProdEntryDate = DateTime.Now;
        ExtraNeeded = 0;
        Allocated = false;
    }

    public OrderItem(Product product, Order order) : this()
    {
        
        Product = product;
        Order = order;
        ProductID = product.ProductID;
        ActualCustPrice = Helpers.PriceHelpers.CalculateCustomerPrice(product, order.Customer);
    }
    #endregion Constructors 

    #region Methods
    private int CalcQuantityReceived()
    {
        if (Product.IsCredit)
        {
            return 0;
        }
        else
        {
            if (Order != null && !Order.ShippingItems.IsNullOrEmpty())
            {
                return Order.ShippingItems.Where(item => item.ProductID == ProductID).Sum(item => item.QuanRcvd ?? 0);
            }
            else
            {
                return 0;
            }
        }
    }

    public decimal GetTotalPrice
    {
        get
        {
            if(Product.FixedPrice == true)
            {
                return ActualCustPrice * QuantityReceived;
            }
            if(Product.IsCredit)
            {
                return ActualCustPrice;
            }
            return decimal.Round(ActualCustPrice * (decimal)(PickWeight ?? 0),2);
        }
    }

    public override string ToString()
    {
        return Product.ToString();
    }
    #endregion Methods
}
