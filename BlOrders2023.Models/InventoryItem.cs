using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlOrders2023.Models;

[Table("tbl_Inventory_(Boxed)")]
public class InventoryItem
{

    [Key]
    [Required]
    public int ProductID
    {
        get; set;
    }

    //[Required(AllowEmptyStrings=false)]
    //[MaxLength(30)]
    //public string? ProductName
    //{
    //    get; set;
    //}

    [Required]
    [ConcurrencyCheck]
    public short QuantityOnHand
    {
        get; set;
    }

    [Required]
    public short AdjustmentQuantity
    {
        get; set;
    }

    public short SortIndex
    {
        get; set;
    }

}
