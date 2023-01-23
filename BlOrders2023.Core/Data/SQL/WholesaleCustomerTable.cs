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
                                c.CustID.ToString().Contains(query)
                                
                            )
                    .ToListAsync();
            }
        }


        public async Task<IEnumerable<WholesaleCustomer>> GetAsync(int customerID)=>
            await _db.Customers.Include(c => c.orders).Where(c => c.CustID == customerID).ToListAsync();

        public async Task<CustomerClass> GetDefaultCustomerClassasync() =>
           await _db.CustomerClasses.FirstAsync();
        

        public Task<WholesaleCustomer> UpsertAsync(WholesaleCustomer order)
        {
            throw new NotImplementedException();
        }
    }
}
