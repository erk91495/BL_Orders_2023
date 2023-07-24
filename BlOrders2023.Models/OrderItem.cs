using Microsoft.IdentityModel.Tokens;
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
public class OrderItem
{

    #region Properties
    public int OrderID { get; set; }
    [ForeignKey("OrderID")]
    [JsonIgnore]
    public virtual Order Order { get; set; } = null!;
    public int ProductID { get; set; }
    [ConcurrencyCheck]
    public float Quantity { get; set; }
    public float? PickWeight => Order.ShippingItems.Where(i => i.ProductID == ProductID).Sum(i => i.PickWeight);
    public decimal ActualCustPrice { get; set; }
    //public float QuanRcvd { get; set; }
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrdDtl_ID { get; set; }
    public DateTime? ProdEntryDate { get; set; }
    [ForeignKey("ProductID")]
    public virtual Product Product { get; set; } = null!;

    public int QuantityReceived => CalcQuantityReceived();
    public float? QuanAllocated { get; set; }

    //Not in DB used for allocation
    public int ExtraNeeded { get; set; }
    #endregion Properties

    #region Fields
    #endregion Fields

    #region Constructors
    public OrderItem()
    {

    }
    public OrderItem(Product product, Order order)
    {
        
        Product = product;
        Order = order;
        ProductID = product.ProductID;
        Quantity = 0;
        ActualCustPrice = Helpers.Helpers.CalculateCustomerPrice(product, order.Customer);
        //QuanRcvd = 0;
        ProdEntryDate = DateTime.Now;
        ExtraNeeded = 0;
    }
    #endregion Constructors 

    #region Methods
    private int CalcQuantityReceived()
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

    public decimal GetTotalPrice()
    {
        if(Product.FixedPrice == true)
        {
            return ActualCustPrice;
        }
        return decimal.Round(ActualCustPrice * (decimal)(PickWeight ?? 0),2);
    }

    public override string ToString()
    {
        return Product.ToString();
    }
    #endregion Methods
}
