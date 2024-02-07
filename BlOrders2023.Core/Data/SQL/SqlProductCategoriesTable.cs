using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Data.SQL;
internal class SqlProductCategoriesTable : IProductCategoriesTable
{
    #region Fields
    private readonly SqlBLOrdersDBContext _db;
    #endregion Fields
    #region Constructors
    public SqlProductCategoriesTable(SqlBLOrdersDBContext db)
    {
        _db = db;
    }
    #endregion Constructors

    #region Methods
    public IEnumerable<ProductCategory> Get() => _db.ProductCategories.ToList();
    public async Task<IEnumerable<ProductCategory>> GetAsync() => await _db.ProductCategories.ToListAsync();
    public async Task<IEnumerable<ProductCategory>> GetForReportsAsync() => await _db.ProductCategories.Where(c => c.ShowTotalsOnReport == true).ToListAsync();
    public async Task UpsertAsync(ProductCategory category)
    {
        _db.ProductCategories.Update(category);
        await _db.SaveChangesAsync();
    }
    #endregion Methods
}
