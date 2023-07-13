using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

namespace BlOrders2023.UserControls.ViewModels;
internal class MultipleCustomerSelectionDialogViewModel
{
    #region Properties
    public ObservableCollection<WholesaleCustomer> SuggestedCustomers
    {
        get => _suggestedCustomers;
    }
    public Collection<WholesaleCustomer>? SelectedCustomers
    {
        get; set;
    }
    #endregion Properties

    #region Fields
    private ObservableCollection<WholesaleCustomer> _suggestedCustomers;
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    public MultipleCustomerSelectionDialogViewModel()
    {
        _suggestedCustomers = new();
        SelectedCustomers = new();
        _ = LoadCustomers();
    }
    #endregion Fields

    #region Constructors

    #endregion Constructors

    #region Methods
    /// <summary>
    /// Clears and Reloads the Suggested customers from the database
    /// </summary>
    /// <returns></returns>
    public async Task LoadCustomers()
    {

        await dispatcherQueue.EnqueueAsync(() =>
        {
            SuggestedCustomers.Clear();
        });

        IWholesaleCustomerTable table = App.GetNewDatabase().Customers;
        var customers = await Task.Run(() => table.GetAsync());

        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (var customer in customers)
            {
                SuggestedCustomers.Add(customer);
            }
        });

    }

    /// <summary>
    /// Queries the database for a list of Customers that match the given string
    /// </summary>
    /// <param name="query">A string for Customers to match</param>
    public async void QueryCustomers(string? query = null)
    {
        await dispatcherQueue.EnqueueAsync(() =>
        {
            SuggestedCustomers.Clear();
        });

        IWholesaleCustomerTable table = App.GetNewDatabase().Customers;
        var products = await Task.Run(() => table.GetAsync(query));

        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (var product in products)
            {
                SuggestedCustomers.Add(product);
            }
        });
    }
    #endregion Methods
}
