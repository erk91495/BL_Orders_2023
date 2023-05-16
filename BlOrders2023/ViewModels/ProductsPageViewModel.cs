using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Composition;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.ViewModels
{
    public class ProductsPageViewModel : ObservableRecipient
    {
        #region Properties
        public ObservableCollection<Product> Products 
        { 
            get => _products;
            set
            {
                if (_products != value)
                {
                    value.CollectionChanged += LineItems_Changed;
                }

                if (_products != null)
                {
                    _products.CollectionChanged -= LineItems_Changed;
                }

                _products = value;
                OnPropertyChanged();
            }
        }
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
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private bool _isLoading;
        private ObservableCollection<Product> _products;
        #endregion Fields

        #region Constructors
        public ProductsPageViewModel()
        {
            _products = new();
            _ = QueryProducts();
        }
        #endregion Constructors

        #region Methods
        public async Task QueryProducts(string? query = null)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    IsLoading = true;
                    Products.Clear();
                });

                IProductsTable table = App.GetNewDatabase().Products;

                var products = await Task.Run(() => table.GetAsync());
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    Products = new(products);
                    IsLoading = false;
                    OnPropertyChanged(nameof(Products));
                });
            }
            else
            {
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    IsLoading = true;
                    Products.Clear();
                });

                IProductsTable table = App.GetNewDatabase().Products;
                var products = await Task.Run(() => table.GetAsync());

                await dispatcherQueue.EnqueueAsync(() =>
                {
                    Products = new(products);
                    IsLoading = false;
                    OnPropertyChanged(nameof(Products));
                });
            }
        }

        internal async void SaveItem(Product p)
        {
            IProductsTable table = App.GetNewDatabase().Products;
            var products = await Task.Run(() => table.UpsertAsync(p));
        }

        internal async Task DeleteItem(Product p) 
        {
            IProductsTable table = App.GetNewDatabase().Products;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Notifies anyone listening to this object that a line item changed. 
        /// </summary>
        private void LineItems_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Products));
        }

        internal async Task<bool> productIDExists(int productId)
        {
            return await App.GetNewDatabase().Products.IdExists(productId);
        }
        #endregion Methods
    }
}
