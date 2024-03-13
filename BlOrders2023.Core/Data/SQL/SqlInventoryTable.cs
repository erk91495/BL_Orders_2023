using System;
using System.Collections.Generic;
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
            return _db.InventoryAdjustments.Where( i => ids.Contains(i.ProductID)).OrderBy(i => i.SortIndex).AsTracking().ToList();
        }
        else
        {
            return _db.InventoryAdjustments.OrderBy(i => i.SortIndex).AsTracking().ToList();
        }
    }
    public async Task<IEnumerable<InventoryAdjustmentItem>> GetInventoryAdjutmentsAsync(IEnumerable<int> ids = null)
    {
        if (ids != null)
        {
            return await _db.InventoryAdjustments.Where(i => ids.Contains(i.ProductID)).OrderBy(i => i.SortIndex).AsTracking().ToListAsync();
        }
        else
        {
            return await _db.InventoryAdjustments.OrderBy(i => i.SortIndex).AsTracking().ToListAsync();
        }
    }

    public IEnumerable<LiveInventoryItem> GetInventoryItems(IEnumerable<int> ids = null)
    {
        if( ids != null)
        {
            return _db.ScannerInventoryItems.Where(i => ids.Contains(i.ProductID)).ToList();
        }
        else
        {
            return _db.ScannerInventoryItems.ToList();
        }
    }

    public async Task<IEnumerable<LiveInventoryItem>> GetInventoryItemsAsync(IEnumerable<int> ids = null)
    {
        if (ids != null)
        {
            return await _db.ScannerInventoryItems.Where(i => ids.Contains(i.ProductID)).ToListAsync();
        }
        else
        {
            return await _db.ScannerInventoryItems.ToListAsync();
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
            return await Task.Run(() => _db.InventoryTotalItems.FromSql($"[dbo].[usp_GetInventoryTotals]").Where(i => ids.Contains(i.ProductID)).AsNoTracking().ToList());
        }
        else
        {
            return await Task.Run(() => _db.InventoryTotalItems.FromSql($"[dbo].[usp_GetInventoryTotals]").AsNoTracking().ToList());
        }
        
    }

    public bool InsertScannerInventoryItem(LiveInventoryItem item)
    {
        _db.ScannerInventoryItems.Add(item);
        return _db.SaveChanges() == 1;
    }
    public async Task<bool> InsertScannerInventoryItemAsync(LiveInventoryItem item)
    {
        await _db.ScannerInventoryItems.AddAsync(item);
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

    public bool DeleteScannerInventoryItem(LiveInventoryItem item)
    {
        _db.ScannerInventoryItems.Remove(item);
        return _db.SaveChanges() == 1;
    }
    public async Task<bool> DeleteScannerInventoryItemAsync(LiveInventoryItem item)
    {
        _db.ScannerInventoryItems.Remove(item);
        return await _db.SaveChangesAsync() == 1;
    }

    public async Task AdjustInventoryAsync(InventoryTotalItem item)
    {
        var res = await _db.Database.ExecuteSqlAsync($"[dbo].[usp_AdjustInventory] {item.ProductID}, {item.LastAdjustment}");
    }
    #endregion Methods
}
