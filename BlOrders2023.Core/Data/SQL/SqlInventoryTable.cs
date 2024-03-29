using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlOrders2023.Core.Data.SQL;
internal class SqlInventoryTable : IInventoryTable
{
    #region Properties
    #endregion Properties
    #region Fields
    /// <summary>
    /// The DB context for the Bl orders database
    /// </summary>
    private readonly SqlBLOrdersDBContext _db;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Creates a new instance of the InventoryAdjustments Table
    /// </summary>
    /// <param name="db">The Db context for the ordres database</param>
    public SqlInventoryTable(SqlBLOrdersDBContext db)
    {
        _db = db;
    }
    #endregion Constructors

    #region Methods

    #region Inventory Adjustments
    public IEnumerable<InventoryAdjustmentItem> GetInventoryAdjustments(IEnumerable<int> ids = null)
    {
        if (ids != null)
        {
            return _db.InventoryAdjustments.Where(i => ids.Contains(i.ProductID)).AsNoTracking().OrderBy(i => i.SortIndex).ToList();
        }
        else
        {
            return _db.InventoryAdjustments.OrderBy(i => i.SortIndex).AsNoTracking().ToList();
        }
    }
    public async Task<IEnumerable<InventoryAdjustmentItem>> GetInventoryAdjutmentsAsync(IEnumerable<int> ids = null)
    {
        if (ids != null)
        {
            return await _db.InventoryAdjustments.Where(i => ids.Contains(i.ProductID)).AsNoTracking().OrderBy(i => i.SortIndex).ToListAsync();
        }
        else
        {
            return await _db.InventoryAdjustments.OrderBy(i => i.SortIndex).AsNoTracking().ToListAsync();
        }
    }
    public async Task UpsertAdjustmentAsync(InventoryAdjustmentItem item)
    {
        _db.Update(item);
        await _db.SaveChangesAsync();
    }

    public async Task UpsertAdjustmentsAsync(IEnumerable<InventoryAdjustmentItem> inventory)
    {
        foreach (InventoryAdjustmentItem item in inventory)
        {
            _db.Update(item);
        }
        await _db.SaveChangesAsync();
    }
    public async Task AdjustInventoryAsync(InventoryTotalItem item)
    {
        var res = await _db.Database.ExecuteSqlAsync($"[dbo].[usp_AdjustInventory] {item.ProductID}, {item.LastAdjustment}");
        if (res == 0)
        {
            _db.InventoryAdjustments.Add(new InventoryAdjustmentItem()
            {
                ProductID = item.ProductID,
                ManualAdjustments = item.LastAdjustment,
                LastAdjustment = item.LastAdjustment,
                SortIndex = null,
            });
            await _db.SaveChangesAsync();
        }
    }

    public async Task AdjustInventoryAsync(int ProductID, int LastAdjustment)
    {
        var res = await _db.Database.ExecuteSqlAsync($"[dbo].[usp_AdjustInventory] {ProductID}, {LastAdjustment}");
        if (res == 0)
        {
            _db.InventoryAdjustments.Add(new InventoryAdjustmentItem()
            {
                ProductID = ProductID,
                ManualAdjustments = LastAdjustment,
                LastAdjustment = LastAdjustment,
                SortIndex = null,
            });
            await _db.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<InventoryAuditItem>> GetInventoryAuditLogAsync()
    {
        return await _db.InventoryAuditItems.ToListAsync();
    }
    #endregion Inventory Adjustments

    #region Live Inventory Items

    public IEnumerable<LiveInventoryItem> GetInventoryItems(IEnumerable<int> ids = null)
    {
        if( ids != null)
        {
            return _db.LiveInventoryItems.Where(i => ids.Contains(i.ProductID)).ToList();
        }
        else
        {
            return _db.LiveInventoryItems.ToList();
        }
    }

    public async Task<IEnumerable<LiveInventoryItem>> GetInventoryItemsAsync(IEnumerable<int> ids = null)
    {
        if (ids != null)
        {
            return await _db.LiveInventoryItems.Where(i => ids.Contains(i.ProductID)).ToListAsync();
        }
        else
        {
            return await _db.LiveInventoryItems.ToListAsync();
        }
    }

    public IEnumerable<LiveInventoryItem> GetUnshippedInventoryItems(IEnumerable<int> ids = null)
    {
        if (ids != null)
        {
            return _db.LiveInventoryItems.Where(i => !i.RemovedFromInventory && ids.Contains(i.ProductID)).ToList();
        }
        else
        {
            return _db.LiveInventoryItems.Where(i => !i.RemovedFromInventory).ToList();
        }
    }

    public async Task<IEnumerable<LiveInventoryItem>> GetUnshippedInventoryItemsAsync(IEnumerable<int> ids = null)
    {
        if (ids != null)
        {
            return await _db.LiveInventoryItems.Where(i => !i.RemovedFromInventory && ids.Contains(i.ProductID)).ToListAsync();
        }
        else
        {
            return await _db.LiveInventoryItems.Where(i => !i.RemovedFromInventory).ToListAsync();
        }
    }

    public bool InsertLiveInventoryItem(LiveInventoryItem item)
    {
        _db.LiveInventoryItems.Add(item);
        return _db.SaveChanges() == 1;
    }

    public async Task<bool> InsertLiveInventoryItemAsync(LiveInventoryItem item)
    {
        await _db.LiveInventoryItems.AddAsync(item);
        return await _db.SaveChangesAsync() == 1;
    }

    public bool DeleteLiveInventoryItem(LiveInventoryItem item)
    {
        _db.LiveInventoryItems.Remove(item);
        return _db.SaveChanges() == 1;
    }

    public async Task<bool> DeleteLiveInventoryItemAsync(LiveInventoryItem item)
    {
        _db.LiveInventoryItems.Remove(item);
        return await _db.SaveChangesAsync() == 1;
    }

    public void UpsertLiveInventoryItem(LiveInventoryItem item)
    {
        _db.LiveInventoryItems.Update(item);
        _db.SaveChanges();
    }

    public async Task UpsertLiveInventoryItemAsync(LiveInventoryItem item)
    {
        _db.LiveInventoryItems.Update(item);
        await _db.SaveChangesAsync();
    }

    public async Task<LiveInventoryItem?> DuplicateInventoryCheck(string scanline)
    {
        var item = await _db.LiveInventoryItems.FromSql($"[dbo].[usp_DuplicateInventoryScanlineCheck] {scanline}").ToListAsync();
        return item.FirstOrDefault(defaultValue: null);
    }

    public async Task<LiveInventoryItem?> FindLiveInventoryItem(ShippingItem shippingItem)
    {
        var res = await _db.LiveInventoryItems.FromSql($"[dbo].[usp_InLiveInventory] {shippingItem.ProductID}, {shippingItem.PackDate}, {shippingItem.PackageSerialNumber}, {shippingItem.Scanline}").ToListAsync();
        return res.FirstOrDefault();
    }

    public async Task ZeroLiveInventoryAsync()
    {
        await _db.Database.ExecuteSqlAsync($"[dbo].[usp_ZeroLiveInventory]");
    }
    #endregion Live Inventory Items

    #region Live Invetory Logging
    public async Task<IEnumerable<LiveInventoryRemovalReason>> GetLiveInventoryRemovalReasonsAsync()
    {
        return await _db.LiveInventoryRemovalReasons.AsNoTrackingWithIdentityResolution().ToListAsync();
    }

    public IEnumerable<LiveInventoryRemovalReason> GetLiveInventoryRemovalReasons()
    {
        return _db.LiveInventoryRemovalReasons.AsNoTrackingWithIdentityResolution().ToList();
    }

    public async Task<IEnumerable<LiveInventoryRemovalLogItem>> GetLiveInventoryRemovalLogItemsAsync()
    {
        return await _db.LiveInventoryRemovalLogItems.ToListAsync();
    }

    public IEnumerable<LiveInventoryRemovalLogItem> GetLiveInventoryRemovalLogItems()
    {
        return _db.LiveInventoryRemovalLogItems.ToList();
    }

    public async Task InsertLiveInventoryRemovalLogItemAsync(LiveInventoryRemovalLogItem removalEntry)
    {
        await _db.LiveInventoryRemovalLogItems.AddAsync(removalEntry);
        _db.SaveChanges();
    }
    #endregion Live Invetory Logging

    #region Inventory Total Items
    public IEnumerable<InventoryTotalItem> GetInventoryTotalItems(IEnumerable<int> ids = null) 
    {
        if(ids != null)
        {
            return _db.InventoryTotalItems.FromSql($"[dbo].[usp_GetInventoryTotals]").Where(i => ids.Contains(i.ProductID)).ToList();
        }
        else
        {
            var res = _db.InventoryTotalItems.FromSql($"[dbo].[usp_GetInventoryTotals]").AsNoTracking().ToList();
            return res;
        }
        
    }

    public async Task<IEnumerable<InventoryTotalItem>> GetInventoryTotalItemsAsync(IEnumerable<int> ids = null)
    {
        if (ids != null)
        {
            return await Task.Run(() => _db.InventoryTotalItems.FromSql($"[dbo].[usp_GetInventoryTotals]").AsNoTracking().Where(i => ids.Contains(i.ProductID)).ToList());
        }
        else
        {
            return await Task.Run(() => _db.InventoryTotalItems.FromSql($"[dbo].[usp_GetInventoryTotals]").AsNoTracking().ToList());
        }
        
    }
    #endregion Inventory Total Items



    public async Task<Dictionary<int, int>> GetAllocatedNotReceivedTotalsAsync()
    {
        var result = new Dictionary<int, int>();
        var items = await _db.Orders
                        .Where(o => o.OrderStatus < OrderStatus.Filled && o.Allocated == true)
                        .SelectMany(o => o.Items)
                        .GroupBy(i => i.ProductID)
            .ToListAsync();
        foreach (var group in items)
        {
            result.Add(group.Key, group.Sum(o => o.QuanAllocated - o.QuantityReceived > 0 ? o.QuanAllocated - o.QuantityReceived : 0));
        }
        return result;
    
    }

    public Dictionary<int, int> GetAllocatedNotReceivedTotals()
    {
        var result = new Dictionary<int, int>();
        var items = _db.Orders
                        .Where(o => o.OrderStatus < OrderStatus.Filled && o.Allocated == true)
                        .SelectMany(o => o.Items)
                        .GroupBy(i => i.ProductID)
            .ToList();
        foreach (var group in items)
        {
            result.Add(group.Key, group.Sum(o => o.QuanAllocated - o.QuantityReceived > 0 ? o.QuanAllocated - o.QuantityReceived : 0));
        }
        return result;

    }
    #endregion Methods
}
