using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using Microsoft.UI.Dispatching;
using CommunityToolkit.WinUI;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlOrders2023.ViewModels;

public class DiscountManagerPageViewModel : ViewModelBase
{
    #region Fields
    private readonly  IBLDatabase _db = App.GetNewDatabase();
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private bool _isLoading;
    private ObservableCollection<Discount> discounts = new();
    private ObservableCollection<Product> products = new();
    private ObservableCollection<WholesaleCustomer> customers = new();
    #endregion Fields
    #region Properties
    public ObservableCollection<Discount> Discounts
    {
        get => discounts; 
        private set => SetProperty(ref discounts, value);
    }
    public ObservableCollection<Product> Products
    {
        get => products; 
        set => SetProperty(ref products, value);
    }
    public ObservableCollection<WholesaleCustomer> Customers
    {
        get => customers; 
        set => SetProperty(ref customers, value);
    }
    public Discount? SelectedDiscount { get; set; }
    public bool IsLoading
    {
        get => _isLoading; 
        set => SetProperty(ref _isLoading, value);
    }
    #endregion Properties
    #region Constructors
    public DiscountManagerPageViewModel() : base()
    {
    }
    #endregion Constructors

    #region Methods
    public async Task LoadData()
    {
        await QueryCustomers();
        await QueryProducts();
        await QueryDiscounts();
    }

    public async Task QueryDiscounts()
    {
        IsLoading = true;
        Discounts.Clear();
        IDiscountTable table = _db.Discounts;

        var results = await table.GetDiscountsAsync();
        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (var o in results)
            {
                Discounts.Add(o);
            }
            OnPropertyChanged(nameof(Discounts));
        });
        IsLoading = false;
    }

    public async Task QueryProducts()
    {
        IsLoading = true;
        Discounts.Clear();
        IProductsTable table = _db.Products;

        var results = await table.GetAsync();
        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (var o in results)
            {
                Products.Add(o);
            }
            OnPropertyChanged(nameof(Products));
        });
        IsLoading = false;
    }

    public async Task QueryCustomers()
    {
        IsLoading = true;
        Discounts.Clear();
        IWholesaleCustomerTable table = _db.Customers;

        var results = await table.GetAsync();
        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (var o in results)
            {
                Customers.Add(o);
            }
            OnPropertyChanged(nameof(Customers));
        });
        IsLoading = false;
    }

    internal void SaveDiscountAsync(Discount discount)
    {
        _db.Discounts.UpsertAsync(discount);
    }
    #endregion Methods
}
