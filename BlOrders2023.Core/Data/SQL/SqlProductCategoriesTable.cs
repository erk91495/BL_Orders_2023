using Microsoft.EntityFrameworkCore;
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
    public async Task<IEnumerable<ProductCategory>> GetAsync()
    {
        var res = await _db.ProductCategories.ToListAsync();
        return res;
    }
    public async Task<IEnumerable<ProductCategory>> GetForReportsAsync() => await _db.ProductCategories.Where(c => c.ShowTotalsOnReports == true).ToListAsync();
    public async Task UpsertAsync(ProductCategory category)
    {
        _db.ProductCategories.Update(category);
        await _db.SaveChangesAsync();
    }
    #endregion Methods
}
