using System;
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

namespace BlOrders2023.ViewModels;
public class InventoryPageViewModel : ObservableRecipient
{
    #region Properties
    public ObservableCollection<InventoryItem> Inventory
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
    private ObservableCollection<InventoryItem> _inventory;
    #endregion Fields

    #region Constructors
    public InventoryPageViewModel()
    {
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

            var table = App.GetNewDatabase().Inventory;

            var inventory = await Task.Run(() => table.GetInventoryAsync());
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

            var table = App.GetNewDatabase().Inventory;
            var inventory = await Task.Run(() => table.GetInventoryAsync());

            await dispatcherQueue.EnqueueAsync(() =>
            {
                Inventory = new(inventory);
                IsLoading = false;
                OnPropertyChanged(nameof(Inventory));
            });
        }
    }

    internal async void SaveItem(InventoryItem p)
    {
        var table = App.GetNewDatabase().Inventory;
        await table.UpsertAsync(p);
    }

    internal async Task DeleteItem(Product p)
    {
        
    }
    #endregion Methods

}
