using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;

namespace BlOrders2023.ViewModels
{

    public class CustomerSelectionViewModel
    {
        #region Properties
        public ObservableCollection<WholesaleCustomer> SuggestedCustomers
        {
            get => _suggestedCustomers;
        }
        public WholesaleCustomer? SelectedCustomer { get; set; }
        #endregion Properties

        #region Fields
        private ObservableCollection<WholesaleCustomer> _suggestedCustomers;
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        #endregion Fields

        #region Constructors
        public CustomerSelectionViewModel()
        {
            _suggestedCustomers= new();
            SelectedCustomer = new();
            _ = LoadCustomers();
        }
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

            IWholesaleCustomerTable table = App.BLDatabase.Customers;
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

            IWholesaleCustomerTable table = App.BLDatabase.Customers;
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


}
