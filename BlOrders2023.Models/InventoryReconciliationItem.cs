using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;
public class InventoryReconciliationItem : ObservableObject
{
    #region Fields
    private LiveInventoryItem _liveInventoryItem;
    private InventoryReconciliationAction _inventoryReconciliationAction;
    #endregion Fields

    #region Properties
    public LiveInventoryItem LiveInventoryItem
    {
        get => _liveInventoryItem;
        set => SetProperty(ref _liveInventoryItem, value);
    }

    public InventoryReconciliationAction InventoryReconciliationAction
    {
        get => _inventoryReconciliationAction;
        set => SetProperty(ref _inventoryReconciliationAction, value);
    }
    #endregion Properties
}
 