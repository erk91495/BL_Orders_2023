using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;

namespace BlOrders2023.Core.Data.SQL;

internal class SqlShipDetailsTable : IShipDetailsTable
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
    /// Creates a new instance of the OrderTable
    /// </summary>
    /// <param name="db">The Db context for the ordres database</param>
    public SqlShipDetailsTable(SqlBLOrdersDBContext db)
    {
        _db = db;
    }

    public async Task DeleteAsync(ShippingItem item)
    {
        _db.ShippingItems.Remove(item);
        await _db.SaveChangesAsync();
    }
    #endregion Constructors

    public async Task<IEnumerable<ShippingItem>> GetAsync()
    {
        return await _db.ShippingItems.ToListAsync();
    }

    public IEnumerable<OrderTotalsItem> GetOrderTotals(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return _db.OrderTotalsItems.FromSql($"[dbo].[uspRptWholesaleOrderTotals] {startDate.Date.ToShortDateString()}, { endDate.Date.ToShortDateString()}").ToList();
    }

    public async Task<bool> IsDuplicateScanline(string scanline)
    {
        //var item = await _db.ShippingItems.Where(item => item.Scanline == scanline).FirstOrDefaultAsync();
        var item = await _db.ShippingItems.FromSql($"[dbo].[usp_DuplicateScanlineCheck] {scanline}").ToListAsync();
        return item.FirstOrDefault() != null;
    }

    public async Task UpsertAsync(List<ShippingItem> items)
    {
        if (items.Count > 0)
        {
            //var orderId = items.FirstOrDefault().OrderID;
            _db.UpdateRange(items);
            await _db.SaveChangesAsync();
        }
    }

    public async Task UpsertAsync(ShippingItem item)
    {
        _db.Update(item);
        await _db.SaveChangesAsync();
    }
}
