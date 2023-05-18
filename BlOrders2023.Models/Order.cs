using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BlOrders2023.Models.Enums;
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Models;
[Table("tblOrdersWholesale")]
public class Order
{ 
    public Order() 
    {
        Memo_Totl = 0M;
        Memo_Weight = 0;
        OrderDate = DateTime.Now;
        Frozen = false;
        //Set the date for today so that sql will accept the time
        PickupDate = DateTime.Now;
        PickupTime = DateTime.Today;
    }
    
    public Order(WholesaleCustomer customer)
        : this()
    {
        Customer = customer;
        CustID = customer.CustID;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderID { get; set; }
    public DateTime OrderDate { get; set; }
    public int CustID { get; set; }

    [ForeignKey("CustID")]
    [JsonIgnore]
    public virtual WholesaleCustomer Customer { get; set; } = null!;
    public string TakenBy { get; set; } = ""; 
    public DateTime PickupDate { get; set; }
    public DateTime PickupTime { get; set; }
    public ShippingType Shipping { get; set; }
    public byte Boxed { get; set; }
    public bool? Frozen { get; set; }
    public bool? Shipped { get; set; }
    public short? Filled { get; set; }
    public short? FilledBox { get; set; }
    public string? Memo { get; set; }
    public float? Memo_Weight { get; set; }
    public decimal? Memo_Totl { get; set; }
    public short? Net { get; set; }
    public string? PO_Number { get; set; }
    public bool? Printed { get; set; }
    public bool? OKToProcess { get; set; }
    public bool? Paid { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public virtual List<OrderItem> Items { get; set; } = new();
    public virtual List<ShippingItem> ShippingItems { get; set; } = new();

    public decimal GetInvoiceTotal()
    {
        decimal total = 0;
        if (!Items.IsNullOrEmpty())
        {
            foreach(var item in Items)
            {
                total += item.GetTotalPrice();
            }
        }
        total += Memo_Totl ?? 0;
        return total;
    }
}
