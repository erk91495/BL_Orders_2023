using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;


[Table("tbl_Inventory_(Boxed)")]
public class InventoryItem: ObservableObject
{
    private int productID;
    private string? productName;
    private short quantityOnHand;
    private short adjustmentQuantity;
    private short sortIndex;

    [Key]
    [Required]
    public int ProductID
    {
        get => productID; 
        set => SetProperty(ref productID, value);
    }

    [Required(AllowEmptyStrings = false)]
    [MaxLength(30)]
    public string? ProductName
    {
        get => productName; 
        set => SetProperty(ref productName, value);
    }

    [Required]
    [ConcurrencyCheck]
    public short QuantityOnHand
    {
        get => quantityOnHand; 
        set => SetProperty(ref quantityOnHand, value);
    }

    [Required]
    public short AdjustmentQuantity
    {
        get => adjustmentQuantity; 
        set => SetProperty(ref adjustmentQuantity, value);
    }

    public short SortIndex
    {
        get => sortIndex; 
        set => SetProperty(ref sortIndex, value);
    }

}
