using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace BlOrders2023.Core.Data.SQL;

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

    public IEnumerable<Product> Get(int? productID = null, bool tracking = true)
    {
        if (tracking)
        {
            if (productID == null)
            {
                return _db.Products.ToList();
            }
            else
            {
                return _db.Products.Where(product => product.ProductID == productID).ToList();
            }
        }
        else
        {
            if (productID == null)
            {
                return _db.Products.AsNoTracking().ToList();
            }
            else
            {
                return _db.Products.Where(product => product.ProductID == productID).AsNoTracking().ToList();
            }
        }
    }

    public async Task<IEnumerable<Product>> GetAsync(int? ProductID = null, bool tracking = true)
    {
        if (tracking)
        {
            if(ProductID == null)
            {
                return await _db.Products.OrderBy(product => product.ProductID).ToListAsync();
            }
            else
            {
                return await _db.Products.Where(p => p.ProductID == ProductID).ToListAsync();
            }
        }
        else
        {
            if (ProductID == null)
            {
                return await _db.Products.OrderBy(product => product.ProductID).AsNoTracking().ToListAsync();
            }
            else
            {
                return await _db.Products.Where(p => p.ProductID == ProductID).AsNoTracking().ToListAsync();
            }
        }
        
    }

    public async Task<IEnumerable<Product>> GetAsync(string value, bool tracking) 
    {
        if (tracking)
        {
            if (value.IsNullOrEmpty())
            {
                return await _db.Products.ToListAsync();
            }
            else
            {
                return await _db.Products.Where(product =>
                    product.ProductName.Contains(value) ||
                    product.ProductID.ToString().Contains(value))
                    .ToListAsync();
            }
        }
        else
        {
            if (value.IsNullOrEmpty())
            {
                return await _db.Products.AsNoTracking().ToListAsync();
            }
            else
            {
                return await _db.Products.Where(product =>
                    product.ProductName.Contains(value) ||
                    product.ProductID.ToString().Contains(value))
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
    }


    public async Task<bool> IdExists(int productID)
    {
        var result = await _db.Products.FromSql<Product>($"[dbo].[usp_ProductIDExists] {productID}").AsNoTracking().ToListAsync();
        if (result.IsNullOrEmpty()){
            return false;
        }
        else
        {
            return true;
        }
    }

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
