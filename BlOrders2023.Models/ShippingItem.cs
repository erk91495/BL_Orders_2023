using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;

[Table("tbl_ShipDetails")]
public class ShippingItem : ObservableObject
{
    #region  Fields
    private int sD_ID;
    private int orderID;
    private Order order = null!;
    private int productID;
    private int? quanRcvd;
    private float? pickWeight;
    private bool? consolidated;
    private string? scanline;
    private string? packageSerialNumber;
    private DateTime? scanDate;
    private DateTime? packDate;
    private Product product = null!;
    private IBarcode? barcode;
    private int? liveInventoryID;
    #endregion Fields

    #region Properties
    [Key]
    public int SD_ID
    {
        get => sD_ID; 
        set => SetProperty(ref sD_ID, value);
    }
    public int OrderID
    {
        get => orderID; 
        set => SetProperty(ref orderID, value);
    }
    [ForeignKey("OrderID")]
    [JsonIgnore]
    public virtual Order Order
    {
        get => order; 
        set => SetProperty(ref order, value);
    }
    [Column("ProdID")]
    public int ProductID
    {
        get => productID; 
        set => SetProperty(ref productID, value);
    }
    [Range(1,1000)]
    public int? QuanRcvd
    {
        get => quanRcvd; 
        set => SetProperty(ref quanRcvd, value);
    }
    public float? PickWeight
    {
        get => pickWeight; 
        set => SetProperty(ref pickWeight, value);
    }
    public bool? Consolidated
    {
        get => consolidated; 
        set => SetProperty(ref consolidated, value);
    }
    public string? Scanline
    {
        get => scanline; 
        set => SetProperty(ref scanline, value);
    }
    public string? PackageSerialNumber
    {
        get => packageSerialNumber; 
        set => SetProperty(ref packageSerialNumber, value);
    }
    [Column("OrderDate")]
    public DateTime? ScanDate
    {
        get => scanDate; 
        set => SetProperty(ref scanDate, value);
    }
    [Column("PackageDate")]
    public DateTime? PackDate
    {
        get => packDate; 
        set => SetProperty(ref packDate, value);
    }
    [ForeignKey("ProductID")]
    public virtual Product Product
    {
        get => product; 
        set => SetProperty(ref product, value);
    }
    public int? LiveInventoryID
    {
        get => liveInventoryID;
        set => SetProperty( ref liveInventoryID, value);
    }

    [ForeignKey(nameof(LiveInventoryID))]
    public virtual LiveInventoryItem? LiveInventoryItem { get; set; }

    [NotMapped]
    public IBarcode? Barcode
    {
        get => barcode; 
        set => SetProperty(ref barcode, value);
    }
    #endregion Properties

    #region Methods
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
               PackDate == item.PackDate &&
               LiveInventoryID == item.LiveInventoryID;
               
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
        hash.Add(LiveInventoryID);
        return hash.ToHashCode();
    }
    #endregion Methods
}
