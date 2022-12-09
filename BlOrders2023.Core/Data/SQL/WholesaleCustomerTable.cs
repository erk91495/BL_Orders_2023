using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<WholesaleCustomer>> GetAsync() =>
            await _db.Customers
                .Include(customer => customer.orders)
                .AsNoTracking()
                .ToListAsync();


        public Task<IEnumerable<WholesaleCustomer>> GetAsync(int orderID)
        {
            throw new NotImplementedException();
        }

        public Task<WholesaleCustomer> UpsertAsync(WholesaleCustomer order)
        {
            throw new NotImplementedException();
        }
    }
}
