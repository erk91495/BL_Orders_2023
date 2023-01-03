using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;

namespace BlOrders2023.ViewModels
{
    public class OrderDetailsPageViewModel : ObservableRecipient, INavigationAware
    {
        #region Properties
        /// <summary>
        /// Gets or sets the current Order
        /// </summary>
        public Order CurrentOrder
        {
            get => _order;
            set
            {
                SetProperty(ref _order, value);
                OnPropertyChanged(nameof(CurrentOrder));
            }
        }

        /// <summary>
        /// Gets a list of products for the auto suggest box
        /// </summary>
        public ObservableCollection<Product> SuggestedProducts
        {
            get => _suggestedProducts;
        }
        #endregion Properties
        #region Fields
        private Order _order;
        private ObservableCollection<Product> _suggestedProducts;
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Creates a new instance of OrderDetailsPageViewModel
        /// </summary>
        public OrderDetailsPageViewModel()
        {
            _order = new();
            _suggestedProducts = new(); 
            LoadProducts();
        }
        #endregion Constructors

        #region Methods

        public void OnNavigatedTo(object parameter)
        {
            
        }

        public void OnNavigatedFrom()
        {
            SaveCurrentOrder();
        }

        #region Queries
        /// <summary>
        /// Queries the database for a list of all products and adds them to SuggestedProducts
        /// </summary>
        public async void LoadProducts()
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                SuggestedProducts.Clear();
            });

            IProductsTable table = App.BLDatabase.Products;
            var products = await Task.Run(table.GetAsync);

            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (var product in products)
                {
                    SuggestedProducts.Add(product);
                }
            });
        }

        /// <summary>
        /// Queries the database for a list of products that match the given string
        /// </summary>
        /// <param name="query">A string for products to match</param>
        public async void QueryProducts(string query)
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                SuggestedProducts.Clear();
            });

            IProductsTable table = App.BLDatabase.Products;
            var products = await Task.Run(() => table.GetAsync(query));

            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (var product in products)
                {
                    SuggestedProducts.Add(product);
                }
            });
        }
        /// <summary>
        /// Saves changes to the current order
        /// </summary>
        public async void SaveCurrentOrder()
        {
           await App.BLDatabase.Orders.UpsertAsync(CurrentOrder);
        }
        #endregion Queries
        #endregion Methods
    }
}
