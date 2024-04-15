using Microsoft.EntityFrameworkCore;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Data.SQL;
internal class SqlDiscountsTable : IDiscountTable
{
    #region Fields
    /// <summary>
    /// The DB context for the Bl orders database
    /// </summary>
    private readonly SqlBLOrdersDBContext _db;
    #endregion Fields

    #region Constructors
    public SqlDiscountsTable(SqlBLOrdersDBContext db)
    {
        _db = db;
    }
    #endregion Constructors

    #region Products
    public async Task<IEnumerable<Discount>> GetDiscountsAsync() => await _db.Discounts.ToListAsync();
    public async Task<IEnumerable<Discount>> GetDiscountsAsync(Product product) => await _db.Discounts.Where(d => d.Products.Contains(product)).ToListAsync();
    public async Task<IEnumerable<Discount>> GetDiscountsAsync(WholesaleCustomer customer) => await _db.Discounts.Where(d => d.Customers.Contains(customer)).ToListAsync();
    public async Task<IEnumerable<Discount>> GetDiscountsAsync(Product product, WholesaleCustomer customer) => await _db.Discounts.Where(d => d.Customers.Contains(customer) && d.Products.Contains(product)).ToListAsync();
    public async Task<bool> UpsertAsync(Discount discount)
    {
        _db.Update(discount);
        return await _db.SaveChangesAsync() == 1;
    }
    #endregion Products
}
