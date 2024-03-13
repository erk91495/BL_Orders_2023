using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;
[Table("tbl_LiveInventory")]
public class LiveInventoryItem : ObservableObject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }
    public string LotCode { get; set; }
    public int ProductID { get; set; }
    public DateTime? PackDate { get; set; }
    public float? NetWeight { get; set; }
    public string? SerialNumber { get; set; }
    public string Scanline { get; set; }
    public DateTime? ScanDate { get; set; }

    [ForeignKey(nameof(LotCode))]
    public virtual LotCode Lot { get; set; }
    [ForeignKey(nameof(ProductID))]
    public virtual Product Product { get; set; }
}
