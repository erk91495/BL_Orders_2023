using System.Collections.ObjectModel;

using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Dispatching;

namespace BlOrders2023.ViewModels;

public class WholesaleCustomersViewModel : ObservableRecipient, INavigationAware
{
    #region Properties
    public ObservableCollection<WholesaleCustomer> Customers { get; set; }
    public WholesaleCustomer SelectedCustomer { get; set; }

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
    private IBLDatabase _db = App.GetNewDatabase();
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private bool _isLoading = true;
    #endregion Fields

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
        IsLoading = true;
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
            IsLoading = false;
        });
       
    }

    internal void SaveSelectedCustomer(bool overwrite = false)
    {
        _db.Customers.Upsert(SelectedCustomer, overwrite);
    }

    internal void Reload()
    {
        _db = App.GetNewDatabase();
        _ = LoadCustomers();
    }


    /// <summary>
    /// Queries the database for a list of Customers that match the given string
    /// </summary>
    /// <param name="query">A string for Customers to match</param>
    public async void QueryCustomers(string? query = null)
    {
        IsLoading = true;
        await dispatcherQueue.EnqueueAsync(() =>
        {
            Customers.Clear();
        });

        IWholesaleCustomerTable table = App.GetNewDatabase().Customers;
        var customers = await Task.Run(() => table.GetAsync(query));

        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
            IsLoading = false;
        });
    }
}
