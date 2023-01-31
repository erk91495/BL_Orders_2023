using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data.SQL
{
    public class ProductsTable : IProductsTable
    {

        private readonly BLOrdersDBContext _db;

        public ProductsTable(BLOrdersDBContext db)
        {
            _db = db;
        }
        public Task DeleteAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetAsync() =>
                await _db.Products
                .OrderBy(product => product.ProductID)
                .ToListAsync();


        public Task<IEnumerable<Product>> GetAsync(int productID)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetAsync(string value) =>
            await _db.Products.Where(product =>
                product.ProductName.Contains(value) ||
                product.ProductID.ToString().Contains(value))
                .ToListAsync();

        public async Task<Product> UpsertAsync(Product product)
        {
            var exists = await _db.Products.FirstOrDefaultAsync(p => product.ProductID == p.ProductID);
            if (exists == null)
            {
                _db.Products.Add(product);
            }
            else
            {
                //TODO: Concurrency checks maybe here
                _db.Entry(exists).CurrentValues.SetValues(product);
            }
            int res = await _db.SaveChangesAsync();
            return product;
        }
    }
}
