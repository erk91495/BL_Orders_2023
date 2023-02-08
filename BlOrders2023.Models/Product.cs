using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{
    [Table("tblProducts")]
    public class Product
    {
        [Key]
        [Required]
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public short? NoPerCase { get; set; }
        [Column("Price_(Wholesale)")]
        public decimal WholesalePrice { get; set; }
        public string? UPCCode { get; set; }
        public string? KPCCode { get; set; }
        public bool? FixedPrice { get; set; }
        //public string? KrogerDeptNo { get; set; }

        public override string ToString()
        {
            return ProductName ?? string.Empty;
        }
    }
}
