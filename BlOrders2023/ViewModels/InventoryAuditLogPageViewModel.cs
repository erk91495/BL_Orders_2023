using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

namespace BlOrders2023.ViewModels;
public class InventoryAuditLogPageViewModel : ViewModelBase
{
    #region Fields
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private readonly IBLDatabase _db = App.GetNewDatabase();
    private bool _isLoading = false;
    #endregion Fields

    #region Properties
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
    public ObservableCollection<InventoryAuditItem> AuditLog {get; set;} = new();
    #endregion Properties

    #region Constructors
    public InventoryAuditLogPageViewModel() 
    {
        _ = QueryAuditLog();
    }
    #endregion Constructors

    #region Methods
    public async Task QueryAuditLog(string? query = null)
    {
        if (!string.IsNullOrWhiteSpace(query))
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                IsLoading = true;
                AuditLog.Clear();
            });

            var table = _db.Inventory;

            var inventory = await Task.Run(() => table.GetInventoryAuditLogAsync());
            await dispatcherQueue.EnqueueAsync(() =>
            {
                AuditLog = new(inventory);
                IsLoading = false;
                OnPropertyChanged(nameof(AuditLog));
            });
        }
        else
        {
            await dispatcherQueue.EnqueueAsync(() =>
            {
                IsLoading = true;
                AuditLog.Clear();
            });

            var table = _db.Inventory;
            var inventory = await Task.Run(() => table.GetInventoryAuditLogAsync());

            await dispatcherQueue.EnqueueAsync(() =>
            {
                AuditLog = new(inventory);
                IsLoading = false;
                OnPropertyChanged(nameof(AuditLog));
            });
        }
    }
    #endregion Methods
}
