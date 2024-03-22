using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;


[Table("tbl_InventoryAdjustments")]
public class InventoryAdjustmentItem: ObservableObject
{
    private int _productID;
    private int _manualAdjustment;
    private int _lastAdjustment;
    private short? _sortIndex;

    [Key]
    [Required]
    public int ProductID
    {
        get => _productID; 
        set => SetProperty(ref _productID, value);
    }

    [Required]
    public int ManualAdjustments
    {
        get => _manualAdjustment; 
        set => SetProperty(ref _manualAdjustment, value);
    }

    public int LastAdjustment
    {
        get => _lastAdjustment;
        set => SetProperty(ref _lastAdjustment, value);
    }
    public short? SortIndex
    {
        get => _sortIndex; 
        set => SetProperty(ref _sortIndex, value);
    }

    [ForeignKey(nameof(ProductID))]
    public virtual Product Product { get; set;}
}
