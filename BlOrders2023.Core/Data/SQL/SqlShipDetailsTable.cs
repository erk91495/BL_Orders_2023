using BlOrders2023.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
    public IEnumerable<ShippingItem> GetShippingItems(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return _db.ShippingItems.Where(i => i.PackDate != null &&  i.PackDate.Value.Date >= startDate.Date &&
                                       i.PackDate.Value.Date <= endDate.Date)
                                .Include(i => i.Product)
                                .Include(i => i.Order)
                                .ThenInclude( i => i.Customer);
    }

    public ShippingItem? Get(string scanline)
    {
        return _db.ShippingItems.Where(s => s.Scanline == scanline).FirstOrDefault();
    }

    public ShippingItem? Get(int productID, string serial)
    {
        if (serial.IsNullOrEmpty())
        {
            return _db.ShippingItems.Where(s => s.ProductID == productID).FirstOrDefault();
        }
        else
        {
            return _db.ShippingItems.Where(s => s.ProductID == productID && s.PackageSerialNumber == serial).FirstOrDefault();
        }
        
    }

    public IEnumerable<ShippingItem> GetShippingItems(ShippingItem item, bool? matchProductID, bool? matchSerial, bool? matchPackDate, bool? matchScanline, DateTime? startDate, DateTime? endDate)
    {
        var predicate = PredicateBuilder.New<ShippingItem>(true);
        if(matchProductID == true)
        {
            predicate.And(i => i.ProductID ==  item.ProductID);
        }
        if(matchSerial == true)
        {
            predicate.And(i => i.PackageSerialNumber == item.PackageSerialNumber);
        }
        if (matchPackDate == true && startDate == null && endDate == null)
        {
            predicate.And(i => i.PackDate.GetValueOrDefault().Date ==  item.PackDate.GetValueOrDefault().Date);   
        }
        if (matchScanline == true)
        {
            predicate.And(i => i.Scanline == item.Scanline);
        }
        if(startDate != null && endDate != null)
        {
            predicate.And(i => i.PackDate >= startDate && i.PackDate <= endDate);
        }

        var items = _db.ShippingItems.Where(predicate).ToList();
        return items;
    }
}
