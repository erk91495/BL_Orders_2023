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
using BlOrders2023.Models.Enums;
using Syncfusion.UI.Xaml.Data;

namespace BlOrders2023.ViewModels;
public class AllocatorPageViewModel : ObservableRecipient, INavigationAware
{

    #region Fields
    private ObservableCollection<Order> allocatedOrders;
    private ObservableCollection<InventoryTotalItem> _currentInventory;
    #endregion Fields

    #region Properties
    public IAllocatorConfig AllocatorConfig { get; }
    public IAllocatorService AllocatorService { get; }
    public ObservableCollection<Order> AllocatedOrders
    {
        get => allocatedOrders;
        set => SetProperty(ref allocatedOrders, value);
    }

    public ObservableCollection<InventoryTotalItem> CurrentInventory
    {
        get => _currentInventory;
        set => SetProperty(ref _currentInventory, value);

    }

    public AllocatorMode AllocatorMode
    {
        get => AllocatorConfig.AllocatorMode; 
        set
        {
            AllocatorConfig.AllocatorMode = value;
            OnPropertyChanged();
        }

    }
    #endregion Properties

    #region Constructors
    public AllocatorPageViewModel()
    {
        AllocatorConfig = new OrderAllocatorConfiguration();
        AllocatorService = new OrderAllocator(App.GetNewDatabase());
        allocatedOrders = new();
        _currentInventory = new();
    }
    #endregion Constructors

    #region Methods
    public void OnNavigatedFrom() { }
    public void OnNavigatedTo(object parameter) { }

    public async Task StartAllocationAsync()
    {
        if (AllocatorConfig == null || AllocatorService == null) throw new InvalidOperationException();
        await AllocatorService.AllocateAsync(AllocatorConfig);
        AllocatedOrders = new(AllocatorService.Orders);
        CurrentInventory = new(AllocatorService.Inventory);
    }

    internal void UpdateInventory(InventoryTotalItem inventoryItem, int value)
    {
        var index = CurrentInventory.IndexOf(inventoryItem);
        if (index >= 0)
        {   
            CurrentInventory[index].Total += (short)value;
        }
    }

    internal async Task<IEnumerable<int>> GetOrdersIDToAllocateAsync(DateTimeOffset? item1, DateTimeOffset? item2, AllocatorMode mode)
    {
        if(item1 != null && item2 != null)
        {
            return await AllocatorService.GetOrdersIDToAllocateAsync((DateTimeOffset)item1, (DateTimeOffset)item2, mode);
        }
        if(item1 == null)
        {
            throw new ArgumentNullException(nameof(item1));
        }
        else
        {
            throw new ArgumentNullException(nameof(item2)); 
        }
    }
    #endregion Methods

}