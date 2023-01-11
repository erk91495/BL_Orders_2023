using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{
    [Table("tblOrderDetails_2")]
    public class OrderItem
    {
        public OrderItem()
        {

        }
        public OrderItem(Product product, Order order)
        {
            Product = product;
            Order = order;
            ProductID = product.ProductID;
            Quantity = 0;
            PickWeight = 0;
            ActualCustPrice = product.WholesalePrice * (new Decimal(100) - order.Customer.CustomerClass.DiscountPercent) / new Decimal(100);
            QuanRcvd = 0;
            ProdEntryDate = DateTime.Now;
        }
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        [JsonIgnore]
        public virtual Order Order { get; set; } = new Order();
        public int ProductID { get; set; }
        [ConcurrencyCheck]
        public float Quantity { get; set; }
        public float? PickWeight { get; set; }
        public decimal ActualCustPrice { get; set; }
        public float QuanRcvd { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrdDtl_ID { get; set; }
        public DateTime? ProdEntryDate { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }
}
