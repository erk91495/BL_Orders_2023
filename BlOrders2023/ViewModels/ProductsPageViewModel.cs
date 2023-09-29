using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Composition;
using Microsoft.UI.Dispatching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.ViewModels;

public class ProductsPageViewModel : ObservableRecipient
{
    #region Properties
    public ObservableCollection<Product> Products 
    { 
        get => _products;
        set
        {
            foreach (var item in _products)
            {
                item.PropertyChanged -= Product_PropertyChanged;
            }
            SetProperty(ref _products, value);
            foreach (var item in value)
            {
                item.PropertyChanged += Product_PropertyChanged;
            }
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

            var products = await Task.Run(() => table.GetIncludeInactiveAsync());
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
            var products = await Task.Run(() => table.GetIncludeInactiveAsync());

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
        await table.DeleteAsync(p);
    }


    private void Product_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        //Have to watch for these property changes because checkboxes dont validate
        if((e.PropertyName == "FixedPrice" || e.PropertyName == "Inactive" || e.PropertyName == "IsCredit") && sender is Product p)
        {
            SaveItem(p);
        }
    }

    internal static async Task<bool> ProductIDExists(int productId)
    {
        return await App.GetNewDatabase().Products.IdExists(productId);
    }
    #endregion Methods
}
