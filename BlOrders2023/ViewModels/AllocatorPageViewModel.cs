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

namespace BlOrders2023.ViewModels;
public class AllocatorPageViewModel : ObservableRecipient, INavigationAware
{

    #region Fields
    private ObservableCollection<Order> allocatedOrders;
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
    #endregion Properties

    #region Constructors
    public AllocatorPageViewModel()
    {
        AllocatorConfig = new OrderAllocatorConfiguration();
        AllocatorService = new OrderAllocator(App.GetNewDatabase());
        AllocatedOrders = new List<Order>();
    }
    #endregion Constructors

    #region Methods
    public void OnNavigatedFrom() { }
    public void OnNavigatedTo(object parameter) { }

    public async Task StartAllocationAsync()
    {
        if (AllocatorConfig == null || AllocatorService == null) throw new InvalidOperationException();
        await AllocatorService.Allocate(AllocatorConfig);
    }
    #endregion Methods

}