using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualBasic;
using System.Text.Json.Serialization;

namespace BlOrders2023.Models;
[Table("tblOrdersWholesale")]
public class Order
{ 
    [Key]
    public int OrderID { get; set; }
    public DateTime OrderDate { get; set; }
    public int CustID { get; set; }

    [ForeignKey("CustID")]
    [JsonIgnore]
    public WholesaleCustomer Customer { get; set; } = new WholesaleCustomer();
    public string TakenBy { get; set; } = ""; 
    public DateTime PickupDate { get; set; }
    public DateTime PickupTime { get; set; }
    public byte Shipping { get; set; }
    public byte Boxed { get; set; }
    public short? Frozen { get; set; }
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

    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
}
