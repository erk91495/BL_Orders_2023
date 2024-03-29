using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;
[Table("tbl_LiveInventoryRemovalReason")]
public class LiveInventoryRemovalReason : ObservableObject
{
    #region Fields
    private Guid _id;
    private string _removalReason;
    #endregion Fields

    #region Properties
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ID
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string RemovalReason
    {
        get => _removalReason;
        set => SetProperty(ref _removalReason, value);
    }
    #endregion Properties
}
