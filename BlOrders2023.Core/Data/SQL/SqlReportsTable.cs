using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;

namespace BlOrders2023.Core.Data.SQL;
public class SqlReportsTable : IReportsTable
{
    #region Fields
    /// <summary>
    /// The DB context for the Bl orders database
    /// </summary>
    private readonly SqlBLOrdersDBContext _db;
    #endregion Fields

    #region Constructors
    public SqlReportsTable(SqlBLOrdersDBContext db)
    {
        _db = db;
    }
    #endregion Constructors
    
    #region Methods
    public IEnumerable<ProductTotalsItem> GetProductSalesTotals(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return _db.ProductTotalsItems.FromSql($"[dbo].[usp_GetProductTotals] {startDate}, {endDate}").AsNoTracking().ToList();
    }

    public async Task<IEnumerable<ProductTotalsItem>> GetProductSalesTotalsAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await _db.ProductTotalsItems.FromSql($"[dbo].[usp_GetProductTotals] {startDate}, {endDate}").AsNoTracking().ToListAsync();
    }
    #endregion Methods
}
