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

internal class SqlProductsTable : IProductsTable
{

    private readonly SqlBLOrdersDBContext _db;

    public SqlProductsTable(SqlBLOrdersDBContext db)
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
                return _db.Products.Where(p => !p.Inactive).ToList();
            }
            else
            {
                return _db.Products.Where(product => !product.Inactive && product.ProductID == productID).ToList();
            }
        }
        else
        {
            if (productID == null)
            {
                return _db.Products.Where(p => !p.Inactive).AsNoTrackingWithIdentityResolution().ToList();
            }
            else
            {
                return _db.Products.Where(product => !product.Inactive && product.ProductID == productID).AsNoTrackingWithIdentityResolution().ToList();
            }
        }
    }

    public async Task<IEnumerable<Product>> GetAsync(int? ProductID = null, bool tracking = true)
    {
        if (tracking)
        {
            if(ProductID == null)
            {
                return await _db.Products.Where(p => !p.Inactive).OrderBy(product => product.ProductID).ToListAsync();
            }
            else
            {
                return await _db.Products.Where(p => !p.Inactive && p.ProductID == ProductID).ToListAsync();
            }
        }
        else
        {
            if (ProductID == null)
            {
                return await _db.Products.Where(p => !p.Inactive).OrderBy(product => product.ProductID).AsNoTrackingWithIdentityResolution().ToListAsync();
            }
            else
            {
                return await _db.Products.Where(p => !p.Inactive && p.ProductID == ProductID).AsNoTrackingWithIdentityResolution().ToListAsync();
            }
        }
        
    }

    public async Task<IEnumerable<Product>> GetIncludeInactiveAsync(int? ProductID = null, bool tracking = true)
    {
        if (tracking)
        {
            if (ProductID == null)
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
                return await _db.Products.OrderBy(product => product.ProductID).AsNoTrackingWithIdentityResolution().ToListAsync();
            }
            else
            {
                return await _db.Products.Where(p => p.ProductID == ProductID).AsNoTrackingWithIdentityResolution().ToListAsync();
            }
        }

    }

    public async Task<IEnumerable<Product>> GetAsync(string value, bool tracking) 
    {
        if (tracking)
        {
            if (value.IsNullOrEmpty())
            {
                return await _db.Products.Where(p => !p.Inactive).ToListAsync();
            }
            else
            {
                return await _db.Products.Where(product =>
                    !product.Inactive &&
                    (product.ProductName.Contains(value) || product.ProductID.ToString().Contains(value)))
                    .ToListAsync();
            }
        }
        else
        {
            if (value.IsNullOrEmpty())
            {
                return await _db.Products.Where(p => !p.Inactive).AsNoTrackingWithIdentityResolution().ToListAsync();
            }
            else
            {
                return await _db.Products.Where(product =>
                    !product.Inactive &&
                    (product.ProductName.Contains(value) || product.ProductID.ToString().Contains(value)))
                    .AsNoTrackingWithIdentityResolution()
                    .ToListAsync();
            }
        }
    }

    public Product? GetByALU(string alu)
    {
        var result = _db.Products.Where(p => !p.Inactive && string.Equals(p.ALUCode, alu)).ToList();
        return result.FirstOrDefault();
    }

    public async Task<bool> IdExists(int productID)
    {
        var result = await _db.Products.FromSql<Product>($"[dbo].[usp_ProductIDExists] {productID}").AsNoTrackingWithIdentityResolution().ToListAsync();
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
        var res = await _db.SaveChangesAsync();
        return product;
    }
}
