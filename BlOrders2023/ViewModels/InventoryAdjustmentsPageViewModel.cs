using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

namespace BlOrders2023.ViewModels;
public class InventoryAdjustmentsPageViewModel : ViewModelBase
{
    #region Properties
    public ObservableCollection<InventoryTotalItem> Inventory
    {
        get => _inventory;
        set => SetProperty(ref _inventory, value);
    }
    /// <summary>
    /// Gets or sets a value that specifies whether orders are being loaded.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            SetProperty(ref _isLoading, value);
            OnPropertyChanged(nameof(IsLoading));
        }
    }
    #endregion Properties

    #region Fields
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private bool _isLoading;
    private ObservableCollection<InventoryTotalItem> _inventory = new();
    private IBLDatabase _db;
    #endregion Fields

    #region Constructors
    public InventoryAdjustmentsPageViewModel()
    {
        _db = App.GetNewDatabase();
        _inventory = new();
        _ = QueryInventory();
    }
    #endregion Constructors

    #region Methods
    public async Task QueryInventory(string? query = null)
    {
        if (!string.IsNullOrWhiteSpace(query))
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                IsLoading = true;
                Inventory.Clear();
            });

            var table = _db.Inventory;

            var inventory = await Task.Run(() => table.GetInventoryTotalItemsAsync());
            await dispatcherQueue.EnqueueAsync(() =>
            {
                Inventory = new(inventory);
                IsLoading = false;
                OnPropertyChanged(nameof(Inventory));
            });
        }
        else
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                IsLoading = true;
                Inventory.Clear();
            });

            var table = _db.Inventory;
            var inventory = await Task.Run(() => table.GetInventoryTotalItemsAsync());

            await dispatcherQueue.EnqueueAsync(() =>
            {
                Inventory = new(inventory);
                IsLoading = false;
                OnPropertyChanged(nameof(Inventory));
            });
        }
    }

    internal async void SaveItem(InventoryAdjustmentItem p)
    {
        await _db.Inventory.UpsertAdjustmentAsync(p);
    }

    internal async Task DeleteItem(Product p)
    {

    }

    private void CalculateAdjustments()
    {
        foreach (var item in Inventory)
        {
            item.ManualAdjustments += item.LastAdjustment;
        }
    }

    internal void ClearAdjustmentQuantity()
    {
        foreach (var item in Inventory)
        {
            item.LastAdjustment = 0;
        }
    }

    internal async Task SaveAllAsync()
    {
        ///CalculateAdjustments();
        foreach (var item in Inventory)
        {
            await _db.Inventory.AdjustInventoryAsync(item);
        }
    }

    internal async Task SaveAsync(InventoryTotalItem item)
    {
        await _db.Inventory.AdjustInventoryAsync(item);
    }
    #endregion Methods

}
