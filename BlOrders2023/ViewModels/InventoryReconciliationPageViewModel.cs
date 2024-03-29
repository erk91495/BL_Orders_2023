using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Exceptions;
using BlOrders2023.Models;
using Windows.Graphics.Printing.PrintSupport;

namespace BlOrders2023.ViewModels;
public class InventoryReconciliationPageViewModel : ViewModelBase
{
    #region Fields
    private IBLDatabase _db = App.GetNewDatabase();
    private ObservableCollection<InventoryReconciliationItem> _reconciliationItems = new();
    private ObservableCollection<LiveInventoryItem> _scannedItems = new();
    private ObservableCollection<InventoryReconciliationItem> _selectedItems = new();
    private bool _isLoading;
    #endregion Fields

    #region Properties

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            SetProperty(ref _isLoading, value);
            OnPropertyChanged(nameof(IsLoading));
        }
    }
    public ObservableCollection<InventoryReconciliationItem> ReconciliationItems
    {
        get => _reconciliationItems;
        set => SetProperty(ref _reconciliationItems, value);
    }
    public ObservableCollection<LiveInventoryItem> ScannedItems
    {
        get => _scannedItems;
        set => SetProperty(ref _scannedItems, value);
    }
    public ObservableCollection<InventoryReconciliationItem> SelectedItems
    {
        get => _selectedItems;
        set => SetProperty(ref _selectedItems, value);
    }

    internal async Task ReconcileAsync()
    {
        ReconciliationItems.Clear();
        var productIDs = ScannedItems.Select(i => i.ProductID);
        var inventory = await _db.Inventory.GetUnshippedInventoryItemsAsync(productIDs);
        var nochange = inventory.Where(p => ScannedItems.Any(scanned => scanned.Scanline == p.Scanline));
        var removed = inventory.Where(p => ScannedItems.All(scanned => scanned.Scanline != p.Scanline));
        var added = ScannedItems.Where(scanned => inventory.All(p => scanned.Scanline != p.Scanline));
        foreach (var item in added)
        {
            ReconciliationItems.Add(new () {LiveInventoryItem = item, InventoryReconciliationAction = Models.Enums.InventoryReconciliationAction.Added});
        }
        foreach (var item in nochange)
        {
            ReconciliationItems.Add(new() { LiveInventoryItem = item, InventoryReconciliationAction = Models.Enums.InventoryReconciliationAction.None });
        }
        foreach (var item in removed)
        {
            ReconciliationItems.Add(new() { LiveInventoryItem = item, InventoryReconciliationAction = Models.Enums.InventoryReconciliationAction.Removed });
        }
    }

    internal async Task SaveSelected()
    {
        foreach(var item in SelectedItems)
        {
            if(item.InventoryReconciliationAction == Models.Enums.InventoryReconciliationAction.Added)
            {
                item.LiveInventoryItem.RemovedFromInventory = false;
            }
            else if(item.InventoryReconciliationAction == Models.Enums.InventoryReconciliationAction.Removed)
            {
                item.LiveInventoryItem.RemovedFromInventory = true; 
            }
            await _db.Inventory.UpsertLiveInventoryItemAsync(item.LiveInventoryItem);
        }
    }

    internal void VerifyProduct(LiveInventoryItem item)
    {
        var product = _db.Products.GetByALU(item.Scanline) ?? _db.Products.Get(item.ProductID).FirstOrDefault();
        if (product == null)
        {
            throw new ProductNotFoundException(string.Format("Product {0} Not Found", item.ProductID), item.ProductID);
        }
    }
    #endregion Properties
}
