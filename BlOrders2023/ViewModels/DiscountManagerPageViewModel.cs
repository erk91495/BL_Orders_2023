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
    #endregion Fields
    #region Properties
    public ObservableCollection<Discount> Discounts { get; set; } = new();
    public bool IsLoading
    {
        get => _isLoading; 
        set => SetProperty(ref _isLoading, value);
    }
    #endregion Properties

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
}
