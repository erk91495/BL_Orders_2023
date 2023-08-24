using System;
using System.Collections.Generic;
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
    /// Creates a new instance of the Inventory Table
    /// </summary>
    /// <param name="db">The Db context for the ordres database</param>
    public SqlInventoryTable(SqlBLOrdersDBContext db)
    {
        _db = db;
    }
    #endregion Constructors

    #region Methods
    public IEnumerable<InventoryItem> GetInventory(IEnumerable<int> ids = null)
    {
        if(ids != null)
        {
            return _db.Inventory.Where( i => ids.Contains(i.ProductID)).OrderBy(i => i.SortIndex).AsTracking().ToList();
        }
        else
        {
            return _db.Inventory.OrderBy(i => i.SortIndex).AsTracking().ToList();
        }
    }
    public async Task<IEnumerable<InventoryItem>> GetInventoryAsync(IEnumerable<int> ids = null)
    {
        if (ids != null)
        {
            return await _db.Inventory.Where(i => ids.Contains(i.ProductID)).OrderBy(i => i.SortIndex).AsTracking().ToListAsync();
        }
        else
        {
            return await _db.Inventory.OrderBy(i => i.SortIndex).AsTracking().ToListAsync();
        }
    }

    public async Task UpsertAsync(InventoryItem item)
    {
        _db.Update(item);
        await _db.SaveChangesAsync();
    }

    public async Task UpsertAsync(IEnumerable<InventoryItem> inventory)
    {
        foreach (InventoryItem item in inventory)
        {
            _db.Update(item);
        }
        await _db.SaveChangesAsync();
    }
    #endregion Methods
}
