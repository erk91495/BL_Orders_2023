using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data
{
    public interface IWholesaleCustomerTable
    {
        Task<IEnumerable<WholesaleCustomer>> GetAsync(string query = null);
        Task<IEnumerable<WholesaleCustomer>> GetAsync(int customerID, bool asNoTracking = false);
        Task<IEnumerable<WholesaleCustomer>> GetIncludeInavtiveAsync(string query = null);
        IEnumerable<WholesaleCustomer> Get(string query = null);
        IEnumerable<WholesaleCustomer> Get(int customerID, bool asNoTracking = false);
        Task<WholesaleCustomer> UpsertAsync(WholesaleCustomer customer);
        WholesaleCustomer Upsert(WholesaleCustomer customer, bool overwrite = false);
        Task DeleteAsync(WholesaleCustomer order);
        Task<CustomerClass> GetDefaultCustomerClassAsync();
        void Reload();
    }
}
