﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using BlOrders2023.Exceptions;

namespace BlOrders2023.ViewModels;
public class InventoryPageViewModel : ObservableRecipient
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
    public InventoryPageViewModel()
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

            var inventory = await Task.Run(() => table.GetInventoryTotalItems());
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
            var inventory = await Task.Run(() => table.GetInventoryTotalItems());

            await dispatcherQueue.EnqueueAsync(() =>
            {
                Inventory = new(inventory);
                IsLoading = false;
                OnPropertyChanged(nameof(Inventory));
            });
        }

    }

    internal async Task AddInventoryItemAsync(LiveInventoryItem item)
    {
        var foundItem = await _db.Inventory.DuplicateInventoryCheck(item.Scanline);
        if (foundItem == null)
        {
            await _db.Inventory.InsertLiveInventoryItemAsync(item);
        }
        else if(foundItem.RemovedFromInventory == true)
        {
            item.RemovedFromInventory = false;
            await _db.Inventory.UpsertLiveInventoryItemAsync(item);
        }
        else
        {
            throw new DuplicateBarcodeException();
        }
    }

    internal async Task ZeroLiveInventoryAsync()
    {
        await _db.Inventory.ZeroLiveInventoryAsync();
    }

    internal void VerifyProduct(LiveInventoryItem item)
    {
        var product = _db.Products.GetByALU(item.Scanline) ?? _db.Products.Get(item.ProductID).FirstOrDefault();
        if (product == null)
        {
            throw new ProductNotFoundException(string.Format("Product {0} Not Found", item.ProductID), item.ProductID);
        }
    }

    internal async Task<IEnumerable<LiveInventoryRemovalReason>> GetRemovalResons() => await _db.Inventory.GetLiveInventoryRemovalReasonsAsync();
    internal async Task TryRemoveItem(LiveInventoryItem item, LiveInventoryRemovalReason reason)
    {
        var foundItem = await _db.Inventory.DuplicateInventoryCheck(item.Scanline);
        if(foundItem != null && !foundItem.RemovedFromInventory)
        {
            foundItem.RemovedFromInventory = true;
            await _db.Inventory.UpsertLiveInventoryItemAsync(foundItem);
            var removalEntry = new LiveInventoryRemovalLogItem()
            {
                RemovalReasonID = reason.ID,
                LiveInventoryID = foundItem.ID,
                Scanline = foundItem.Scanline,
            };
            await _db.Inventory.InsertLiveInventoryRemovalLogItemAsync(removalEntry);
        }
        else
        {
            throw new ProductNotFoundException($"The scanline {item.Scanline} was not found in inventory");
        }
    }
    #endregion Methods

}
