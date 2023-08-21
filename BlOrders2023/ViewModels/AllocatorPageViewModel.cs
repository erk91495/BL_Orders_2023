using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Contracts;
using CommunityToolkit.Mvvm.ComponentModel;
using BlOrders2023.Models;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Core.Services;
using BlOrders2023.Exceptions;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BlOrders2023.ViewModels;
public class AllocatorPageViewModel : ObservableRecipient, INavigationAware
{

    #region Fields
    private ObservableCollection<Order> allocatedOrders;
    private ObservableCollection<InventoryItem> _currentInventory;
    #endregion Fields

    #region Properties
    public IAllocatorConfig AllocatorConfig { get; }
    public IAllocatorService AllocatorService { get; }
    public ObservableCollection<Order> AllocatedOrders
    {
        get => allocatedOrders; 
        set 
        {
            allocatedOrders = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<InventoryItem> CurrentInventory
    {
        get => _currentInventory;
        set
        {
            _currentInventory = value;
            OnPropertyChanged();
        }
    }
    #endregion Properties

    #region Constructors
    public AllocatorPageViewModel()
    {
        AllocatorConfig = new OrderAllocatorConfiguration();
        AllocatorService = new OrderAllocator(App.GetNewDatabase());
        AllocatedOrders = new();
        CurrentInventory = new();
    }
    #endregion Constructors

    #region Methods
    public void OnNavigatedFrom() { }
    public void OnNavigatedTo(object parameter) { }

    public async Task StartAllocationAsync()
    {
        if (AllocatorConfig == null || AllocatorService == null) throw new InvalidOperationException();
        await AllocatorService.Allocate(AllocatorConfig);
        AllocatedOrders = new(AllocatorService.Orders);
        CurrentInventory = new(AllocatorService.Inventory);
    }

    internal void UpdateInventory(InventoryItem inventoryItem, int value)
    {
        var index = CurrentInventory.IndexOf(inventoryItem);
        if (index >= 0)
        {
            CurrentInventory[index].QuantityOnHand += (short)value;
            OnPropertyChanged(nameof(CurrentInventory));
            OnPropertyChanged(nameof(AllocatedOrders));
        }
    }
    #endregion Methods

}