using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;
[Table("tbl_InventoryAuditLog")]
public class InventoryAuditItem : ObservableObject
{
    #region Fields
    private int _id;
    private DateTime _transactionDate;
    private string? _workstationName;
    private string? _userName;
    private int _productID;
    private int _startingQuantity;
    private int _adjustmentQuantity;
    private int _endingQuantity;
    #endregion Fields

    #region Properties
    public int ID { get =>  _id; set =>  SetProperty(ref _id, value); }
    public DateTime TransactionDate { get => _transactionDate; set => SetProperty(ref _transactionDate, value); }
    public string? WorkstationName { get => _workstationName; set => SetProperty(ref _workstationName, value); }
    public string? UserName { get => _userName; set => SetProperty(ref _userName, value);}
    public int ProductID { get => _productID; set => SetProperty(ref _productID, value);}
    public int StartingQuantity { get => _startingQuantity; set => SetProperty(ref _startingQuantity, value);}
    public int AdjustmentQuantity { get => _adjustmentQuantity; set => SetProperty(ref _adjustmentQuantity, value); }
    public int EndingQuantity { get => _endingQuantity; set => SetProperty(ref _endingQuantity, value);}
    #endregion Properties
}
