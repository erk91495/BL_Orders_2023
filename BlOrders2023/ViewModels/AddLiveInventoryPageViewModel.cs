using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Exceptions;
using BlOrders2023.Helpers;
using BlOrders2023.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace BlOrders2023.ViewModels;
class AddLiveInventoryPageViewModel : ViewModelBase
{
    #region Fields
    private IBLDatabase _db = App.GetNewDatabase();
    private ObservableCollection<LiveInventoryItem> _scannedItems = new();
    private bool _isLoading;
    #endregion Fields

    #region Properties
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            SetProperty(ref _isLoading, value);
            OnPropertyChanged(nameof(IsLoading));
        }
    }
    public ObservableCollection<LiveInventoryItem> ScannedItems
    {
        get => _scannedItems;
        set => SetProperty(ref _scannedItems, value);
    }

    internal async Task SaveItems()
    {
    }

    internal void VerifyProduct(LiveInventoryItem item)
    {
        var product = _db.Products.GetByALU(item.Scanline) ?? _db.Products.Get(item.ProductID).FirstOrDefault();
        if (product == null)
        {
            throw new ProductNotFoundException(string.Format("Product {0} Not Found", item.ProductID), item.ProductID);
        }
    }
    #endregion Properties

    #region Methods
    #endregion Methods
}

