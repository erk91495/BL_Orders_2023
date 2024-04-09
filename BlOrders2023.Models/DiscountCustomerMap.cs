using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;
[Table("tbl_DiscountCustomerMap")]
public class DiscountCustomerMap
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ID { get; set; }
    public int CustomerID { get; set; }
    public Guid DiscountID { get; set; }

    [ForeignKey("DiscountID")]
    public virtual Discount Discount { get; set; }
    [ForeignKey("CustomerID")]
    public virtual WholesaleCustomer Customer { get; set; }
}
