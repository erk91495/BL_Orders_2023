using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Core.Data.SQL;
internal class SqlCustomerClassesTable : ICustomerClassesTable
{
    #region Fields
    /// <summary>
    /// The DB context for the Bl orders database
    /// </summary>
    private readonly SqlBLOrdersDBContext _db;
    #endregion Fields

    #region Constructors
    public SqlCustomerClassesTable(SqlBLOrdersDBContext db)
    {
        _db = db;
    }
    #endregion Constructors


    public async Task<IEnumerable<CustomerClass>> GetCustomerClassesAsync(string query = null, bool asNoTracking = false)
    {

        if (query.IsNullOrEmpty())
        {
            if (asNoTracking)
            {
                return await _db.CustomerClasses.AsNoTracking().ToListAsync();
            }
            else
            {
                return await _db.CustomerClasses.ToListAsync();
            }
        }
        else
        {
            if (asNoTracking)
            {
                return await _db.CustomerClasses.Where(c => c.ID.ToString().Contains(query) || c.Class.Contains(query))
                                                .AsNoTracking()
                                                .ToListAsync();
            }
            else
            {
                return await _db.CustomerClasses.Where(c => c.ID.ToString().Contains(query) || c.Class.Contains(query))
                                                .ToListAsync();
            }
        }

    }

    public async Task<int> UpsertAsync(CustomerClass custClass)
    {
        var exists = await _db.CustomerClasses.FirstOrDefaultAsync(p => p == custClass);
        if (exists == null)
        {
            _db.CustomerClasses.Add(custClass);
        }
        else
        {
            //TODO: Concurrency checks maybe here
            _db.Entry(exists).CurrentValues.SetValues(custClass);
        }
        return await _db.SaveChangesAsync();
    }
}
