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
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        [JsonIgnore]
        public Order order { get; set; } = new Order();
        public int ProductID { get; set; }
        public float Quantity { get; set; }
        public float? PickWeight { get; set; }
        public decimal ActualCustPrice { get; set; }
        public float? QuanRcvd { get; set; }
        public string? Scanline { get; set; }
        public int? PackageSerialNumber { get; set; }
        [Key]
        public int OrdDtl_ID { get; set; }
        public DateTime? ProdEntryDate { get; set; }

    }
}
