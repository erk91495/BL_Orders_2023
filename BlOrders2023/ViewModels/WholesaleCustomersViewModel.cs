using System.Collections.ObjectModel;

using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Data;
using BlOrders2023.Core.Models;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

namespace BlOrders2023.ViewModels;

public class WholesaleCustomersViewModel : ObservableRecipient, INavigationAware
{
    #region Properties
    public ObservableCollection<WholesaleCustomer> Customers { get; set; }
    #endregion Properties
    #region Fields
    private readonly IBLDatabase _db = App.GetNewDatabase();
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    #endregion Fields

    public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

    public WholesaleCustomersViewModel()
    {
        Customers = new();
        _ = LoadCustomers();
    }

    public void OnNavigatedTo(object parameter)
    {
        
    }

    public void OnNavigatedFrom()
    {
    }

    /// <summary>
    /// Queries the database for a list of all _products and adds them to SuggestedProducts
    /// </summary>
    public async Task LoadCustomers()
    {
        await dispatcherQueue.EnqueueAsync(() =>
        {
            Customers.Clear();
        });

        IWholesaleCustomerTable table = _db.Customers;
        var customers = await Task.Run(() => table.GetAsync());

        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        });
    }
}
