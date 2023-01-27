using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.ViewModels
{
    public class ProductsPageViewModel : ObservableRecipient
    {
        #region Properties
        public ObservableCollection<Product> Products { get; set; }
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
        /// <summary>
        /// Gets the dispatcher Queue for the current thread
        /// </summary>
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private bool _isLoading;
        #endregion Fields

        #region Constructors
        public ProductsPageViewModel()
        {
            Products = new ObservableCollection<Product>();
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

                IProductsTable table = App.BLDatabase.Products;

                var products = await Task.Run(table.GetAsync);
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    Products = new(products);
                    IsLoading = false;
                });  
            }
            else
            {
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    IsLoading = true;
                    Products.Clear();
                });

                IProductsTable table = App.BLDatabase.Products;
                var products = await Task.Run(table.GetAsync);

                await dispatcherQueue.EnqueueAsync(() =>
                {
                    Products =new (products);
                    IsLoading = false;
                    OnPropertyChanged(nameof(Products));
                });
            }
        }
        #endregion Methods
    }
}
