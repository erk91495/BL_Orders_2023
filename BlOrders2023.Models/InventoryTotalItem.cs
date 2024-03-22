using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;

public class InventoryTotalItem : ObservableObject
{
    private int productID;
    private int quantity;
    private int manualAdjustments;
    private int total;
    private int lastAdjustment;
    private short? sortIndex;

    [Key]
    public int ProductID
    {
        get => productID; set => SetProperty(ref productID, value);
    }
    public int Quantity
    {
        get => quantity; set => SetProperty(ref quantity, value);
    }
    public int ManualAdjustments
    {
        get => manualAdjustments; set => SetProperty(ref manualAdjustments, value);
    }
    public int Total
    {
        get => total; set => SetProperty(ref total, value);
    }
    public int LastAdjustment
    {
        get => lastAdjustment; set => SetProperty(ref lastAdjustment, value);
    }
    public short? SortIndex
    {
        get => sortIndex; set => SetProperty(ref sortIndex, value);
    }
    [ForeignKey(nameof(ProductID))]
    public virtual Product Product { get; set; }
}
