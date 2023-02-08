using BlOrders2023.Models;

namespace BlOrders2023.Core.Data
{
    public interface IProductsTable
    {
        Task<IEnumerable<Product>> GetAsync();
        Task<IEnumerable<Product>> GetAsync(int productID);
        Task<IEnumerable<Product>> GetAsync(string value);
        Task<Product> UpsertAsync(Product product);
        Task DeleteAsync(Product product);

        Task<bool> IdExists(int productID);
    }
}
