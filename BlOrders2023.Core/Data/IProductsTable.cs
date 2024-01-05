using BlOrders2023.Models;

namespace BlOrders2023.Core.Data;

public interface IProductsTable
{
    Task<IEnumerable<Product>> GetAsync(int? productID = null, bool tracking=true);
    IEnumerable<Product> Get(int? productID, bool tracking=true);
    Task<IEnumerable<Product>> GetAsync(string value, bool tracking=true);
    Task<Product> UpsertAsync(Product product);
    Task DeleteAsync(Product product);
    Task<bool> IdExists(int productID);
    Product? GetByALU(string upc);
    Task<IEnumerable<Product>> GetIncludeInactiveAsync(int? ProductID = null, bool tracking = true);
    IEnumerable<Product> GetIncludeInactive(int? ProductID = null, bool tracking = true);
}
