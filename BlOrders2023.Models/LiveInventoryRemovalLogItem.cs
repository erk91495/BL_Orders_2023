using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;
[Table("tbl_LiveInventoryRemovalLog")]
public class LiveInventoryRemovalLogItem : ObservableObject
{
    #region Fields
    private Guid _id;
    private string _scanline;
    private Guid _removalReasonID;
    private int? _liveInvetoryID;
    private LiveInventoryRemovalReason _removalReason;
    #endregion Fields

    #region Properties
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id
    {
        get => _id;
        set => _id = value;
    }

    public string Scanline
    {
        get => _scanline;
        set => _scanline = value;
    }
    public Guid RemovalReasonID
    {
        get => _removalReasonID;
        set => _removalReasonID = value;
    }
    public int? LiveInventoryID
    {
        get => _liveInvetoryID;
        set => _liveInvetoryID = value;
    }

    [ForeignKey(nameof(RemovalReasonID))]
    public virtual LiveInventoryRemovalReason RemovalReason
    {
        get => _removalReason; 
        set => SetProperty(ref _removalReason, value);
    }
    #endregion Properties

}
