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
    [Table("tbl_ShipDetails")]
    public class ShippingItem
    {

        [Key]
        public int SD_ID { get; set; }
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        [JsonIgnore]
        public virtual Order order { get; set; } = new Order();
        [Column("ProdID")]
        public int ProductID { get; set; }
        public int? QuanRcvd { get; set; }
        public float? PickWeight { get; set; }
        public bool? Consolidated { get; set; }
        public string? Scanline { get; set; }
        public int? PackageSerialNumber { get; set; }
        [Column("OrderDate")]
        DateTime? ScanDate { get; set; }
        [Column("PackageDate")]
        DateTime? PackDate { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }

    }
}
