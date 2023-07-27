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
        public virtual Order order { get; set; } = null!;
        [Column("ProdID")]
        public int ProductID { get; set; }
        public int? QuanRcvd { get; set; }
        public float? PickWeight { get; set; }
        public bool? Consolidated { get; set; }
        public string? Scanline { get; set; }
        public string? PackageSerialNumber { get; set; }
        [Column("OrderDate")]
        public DateTime? ScanDate { get; set; }
        [Column("PackageDate")]
        public DateTime? PackDate { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            return obj is ShippingItem item &&
                   SD_ID == item.SD_ID &&
                   OrderID == item.OrderID &&
                   ProductID == item.ProductID &&
                   QuanRcvd == item.QuanRcvd &&
                   PickWeight == item.PickWeight &&
                   Consolidated == item.Consolidated &&
                   Scanline == item.Scanline &&
                   PackageSerialNumber == item.PackageSerialNumber &&
                   ScanDate == item.ScanDate &&
                   PackDate == item.PackDate;
                   
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(SD_ID);
            hash.Add(OrderID);
            hash.Add(ProductID);
            hash.Add(QuanRcvd);
            hash.Add(PickWeight);
            hash.Add(Consolidated);
            hash.Add(Scanline);
            hash.Add(PackageSerialNumber);
            hash.Add(ScanDate);
            hash.Add(PackDate);
            return hash.ToHashCode();
        }
    }
}
