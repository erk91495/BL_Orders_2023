using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Newtonsoft.Json.Bson;
using System.Collections.ObjectModel;

namespace BlOrders2023.ViewModels;

public class FillOrdersPageViewModel : ObservableRecipient, INavigationAware
{
    #region Properties
    public WholesaleCustomer Customer { get; set; }
    public Order Order { get => _order; set => _order = value; }
    #endregion Properties

    #region Fields
    private Order _order;
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    #endregion Fields

    #region Consturctors
    public FillOrdersPageViewModel()
    {
        Customer = new();
        _order = new();

    }
    #endregion Constructors

    #region Methods

    public void OnNavigatedTo(object parameter)
    {
        var orderID = parameter as int?;
        if (orderID != null)
        {
            _ = LoadOrder((int)orderID);
        }
    }

    public async Task LoadOrder(int orderID)
    {
        IOrderTable table = App.BLDatabase.Orders;
        var order = await Task.Run(() => table.GetAsync(orderID));

        await dispatcherQueue.EnqueueAsync(() =>
        {
            _order = order.First();
            Customer = _order.Customer;
            OnAllPropertiesChanged();  
        });

    }

    private void OnAllPropertiesChanged()
    {
        OnPropertyChanged(nameof(Customer));
        OnPropertyChanged(nameof(Order));
    }

    public void OnNavigatedFrom()
    {
       
    }
    #endregion Methods


}
