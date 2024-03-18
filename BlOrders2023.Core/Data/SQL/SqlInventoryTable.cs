using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
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
    public IEnumerable<InventoryAdjustmentItem> GetInventoryAdjustments(IEnumerable<int> ids = null)
    {
        if(ids != null)
        {
            return _db.InventoryAdjustments.Where( i => ids.Contains(i.ProductID)).AsNoTracking().OrderBy(i => i.SortIndex).ToList();
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
    
    public IEnumerable<InventoryTotalItem> GetInventoryTotalItems(IEnumerable<int> ids = null) 
    {
        if(ids != null)
        {
            return _db.InventoryTotalItems.FromSql($"[dbo].[usp_GetInventoryTotals]").Where(i => ids.Contains(i.ProductID)).ToList();
        }
        else
        {
            return _db.InventoryTotalItems.FromSql($"[dbo].[usp_GetInventoryTotals]").ToList();
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

    public async Task AdjustInventoryAsync(InventoryTotalItem item)
    {
        var res = await _db.Database.ExecuteSqlAsync($"[dbo].[usp_AdjustInventory] {item.ProductID}, {item.LastAdjustment}");
        if(res == 0)
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

    public async Task<LiveInventoryItem?> FindLiveInventoryItem(ShippingItem shippingItem)
    {
        var res = await _db.LiveInventoryItems.FromSql($"[dbo].[usp_InLiveInventory] {shippingItem.ProductID}, {shippingItem.PackDate}, {shippingItem.PackageSerialNumber}").ToListAsync();
        return res.FirstOrDefault();
    }
    #endregion Methods
}
