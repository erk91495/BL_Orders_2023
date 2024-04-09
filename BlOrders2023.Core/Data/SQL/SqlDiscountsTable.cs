using System;
using System.Collections.Generic;
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
    public Task<Discount> GetDiscountsAsync() => throw new NotImplementedException();
    public Task<Discount> GetDiscountsAsync(Product product) => throw new NotImplementedException();
    public Task<Discount> GetDiscountsAsync(WholesaleCustomer customer) => throw new NotImplementedException();
    public Task<Discount> GetDiscountsAsync(Product product, WholesaleCustomer customer) => throw new NotImplementedException();
    #endregion Products
}
