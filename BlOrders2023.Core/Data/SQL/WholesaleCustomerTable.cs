using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data.SQL
{
    public class WholesaleCustomerTable : IWholesaleCustomerTable
    {
        private readonly BLOrdersDBContext _db;

        public WholesaleCustomerTable(BLOrdersDBContext dB)
        {
            _db = dB;
        }

        public Task DeleteAsync(WholesaleCustomer order)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WholesaleCustomer> Get(string query = null)
        {
            if (query.IsNullOrEmpty())
            {
                return  _db.Customers
                    .ToList();
            }
            else
            {
                return  _db.Customers
                    .Where(c => c.CustomerName.Contains(query) ||
                                c.CustID.ToString().Contains(query))
                    .ToList();
            }
        }

        public IEnumerable<WholesaleCustomer> Get(int customerID)
        {
                return _db.Customers
                    .Where(c => c.CustID == customerID)
                    .ToList();
        }

        public async Task<IEnumerable<WholesaleCustomer>> GetAsync(string query = null)
        {
            if (query.IsNullOrEmpty())
            {
                return await _db.Customers
                    .ToListAsync();
            }
            else
            {
                return await _db.Customers
                    .Where(c => c.CustomerName.Contains(query) || 
                                c.CustID.ToString().Contains(query))
                    .ToListAsync();
            }
        }


        public async Task<IEnumerable<WholesaleCustomer>> GetAsync(int customerID)=>
            await _db.Customers.Include(c => c.Orders).Where(c => c.CustID == customerID).ToListAsync();

        public async Task<IEnumerable<CustomerClass>> GetCustomerClassesAsync() =>
            await _db.CustomerClasses.ToListAsync();

        public async Task<CustomerClass> GetDefaultCustomerClassAsync() =>
           await _db.CustomerClasses.FirstAsync();

        public WholesaleCustomer Upsert(WholesaleCustomer customer, bool overwrite = false)
        {
            _db.Update(customer);
            //TODO: may be a cleaner way of doing this but it works for now
            if (overwrite)
            {
                foreach (var entry in _db.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
                {
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }
            }
            int res = _db.SaveChanges();
            return customer;
        }

        public Task<WholesaleCustomer> UpsertAsync(WholesaleCustomer order)
        {
            throw new NotImplementedException();
        }
    }
}
