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
    public ObservableCollection<LiveInventoryItem> Inventory
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
    private ObservableCollection<LiveInventoryItem> _inventory = new();
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

            var inventory = await Task.Run(() => table.GetUnshippedInventoryItemsAsync());
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
            var inventory = await Task.Run(() => table.GetUnshippedInventoryItemsAsync());

            await dispatcherQueue.EnqueueAsync(() =>
            {
                Inventory = new(inventory);
                IsLoading = false;
                OnPropertyChanged(nameof(Inventory));
            });
        }
    }

    internal async void SaveItem(LiveInventoryItem p)
    {
        await _db.Inventory.InsertLiveInventoryItemAsync(p);
    }

    internal async Task DeleteItem(Product p)
    {
        
    }
   
    internal async Task AddInventoryItemAsync(LiveInventoryItem item)
    {
        if(!await _db.Inventory.DuplicateInventoryCheck(item.Scanline))
        {
            await _db.Inventory.InsertLiveInventoryItemAsync(item);
            Inventory.Add(item);
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
    #endregion Methods

}
