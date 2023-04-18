using BlOrders2023.Core.Data;
using BlOrders2023.Helpers;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design.Behavior;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlOrders2023.ViewModels
{
    public class ShippingItemDataInputControlViewModel : ObservableValidator
    {
        #region Properties
        public ShippingItem ShippingItem { get; set; }
        [Required]
        public string? PackDate
        {
            get => _packDate;
            set
            {
                SetProperty(ref _packDate, value);
                ValidateProperty(value, nameof(PackDate));
            }
        }
        [Required]
        public float? PickWeight
        {
            get => _pickWeight;
            set
            {
                SetProperty(ref _pickWeight, value);
                ValidateProperty(value, nameof(PickWeight));
            }
        }
        [Required]
        [MinLength(6)]
        public string? SerialNumber
        {
            get => _serialNumber;
            set
            {
                SetProperty(ref _serialNumber, value);
                ValidateProperty(value, nameof(SerialNumber));
            }
        }

        public int? ProductID
        {
            get
            {
                return SelectedProduct != null ? SelectedProduct.ProductID : 0;
            }
        }

        public ObservableCollection<Product> SuggestedProducts { get => _suggestedProducts; }

        public bool IsProductSelected { get => SelectedProduct != null; }

        public Product? SelectedProduct 
        { 
            get => _selectedProduct;
            set 
            {
                SetProperty(ref _selectedProduct, value);
                OnAllPropertiesChanged();
            } 
        }
        #endregion Properties

        #region Fields
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private ObservableCollection<Product> _suggestedProducts;
        private string? _packDate;
        private float? _pickWeight;
        private string? _serialNumber;
        private Product? _selectedProduct;

        #endregion Fields

        #region Constructors
        public ShippingItemDataInputControlViewModel()
        {
            _suggestedProducts = new ObservableCollection<Product>();
            ShippingItem = new ShippingItem();
            _ = LoadProducts();
            ValidateAllProperties();
        }

        public ShippingItemDataInputControlViewModel(Product? selectedProduct)
        {
            SelectedProduct = selectedProduct;
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Queries the database for a list of all _products and adds them to SuggestedProducts
        /// </summary>
        public async Task LoadProducts()
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

        public async Task QueryProducts(string text)
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                _suggestedProducts.Clear();
            });

            IProductsTable table = App.BLDatabase.Products;
            var products = await Task.Run(() => table.GetAsync(text));

            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (var product in products)
                {
                    _suggestedProducts.Add(product);
                }
            });
        }

        public void OnAllPropertiesChanged()
        {
            OnPropertyChanged(nameof(IsProductSelected));
            OnPropertyChanged(nameof(PackDate));
            OnPropertyChanged(nameof(ProductID));
            OnPropertyChanged(nameof(SerialNumber));
            OnPropertyChanged(nameof(PickWeight));
            
        }

        public ShippingItem GetShippingItem()
        {
            var item = new ShippingItem()
            {
                Product = _selectedProduct,
                PackageSerialNumber = SerialNumber,
                PackDate = DateTime.ParseExact(PackDate , "yy/MM/dd", null),
                ProductID = (int)ProductID,
                PickWeight = PickWeight,
                ScanDate = DateTime.Now,
                QuanRcvd = 1,
            };
            BarcodeInterpreter.SynthesizeBarcode(ref item);
            return item;
        }
        #endregion Methods

    }
}
