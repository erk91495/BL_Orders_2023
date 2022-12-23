using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;

namespace BlOrders2023.ViewModels
{
    public class OrderDetailsPageViewModel : ObservableRecipient
    {
        private Order? _order;
        private ObservableCollection<Product> _allProducts;
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public Order? CurrentOrder
        {
            get => _order;
            set
            {
                SetProperty(ref _order, value);
                OnPropertyChanged(nameof(CurrentOrder));
            }
        }

        public ObservableCollection<Product> AllProducts
        {
            get => _allProducts;
        }

        public OrderDetailsPageViewModel()
        {
            _allProducts = new(); 
            LoadProducts();
        }

        public async void LoadProducts()
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                AllProducts.Clear();
            });

            IProductsTable table = App.BLDatabase.Products;
            var products = await Task.Run(table.GetAsync);

            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (var product in products)
                {
                    AllProducts.Add(product);
                }
            });
        }

        public async void QueryProducts(string query)
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                AllProducts.Clear();
            });

            IProductsTable table = App.BLDatabase.Products;
            var products = await Task.Run(() => table.GetAsync(query));

            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (var product in products)
                {
                    AllProducts.Add(product);
                }
            });
        }

        internal async void UpdateProductSuggestions(string text)
        {
            AllProducts.Clear();

            if(!string.IsNullOrEmpty(text))
            {
                var suggestions = await App.BLDatabase.Products.GetAsync(text);

                foreach (var product in suggestions)
                {
                    AllProducts.Add(product);
                }
            }
        }
    }
}
