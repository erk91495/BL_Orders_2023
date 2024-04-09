using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    #endregion Products
}
