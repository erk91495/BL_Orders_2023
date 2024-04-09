using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;
[Table("tbl_DiscountProductMap")]
public class DiscountProductMap
{
    public Guid ID { get; set; }
    public int ProductID { get; set; }
    public Guid DiscountID { get; set; }

    [ForeignKey(nameof(DiscountID))]
    public virtual Discount Discount { get; set; }
    [ForeignKey(nameof (ProductID))]
    public virtual Product Product { get; set; }
}
