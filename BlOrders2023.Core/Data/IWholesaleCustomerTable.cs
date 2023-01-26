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
        Task<IEnumerable<WholesaleCustomer>> GetAsync(int customerID);
        Task<WholesaleCustomer> UpsertAsync(WholesaleCustomer order);
        Task DeleteAsync(WholesaleCustomer order);
        Task<CustomerClass> GetDefaultCustomerClassAsync();

        Task<IEnumerable<CustomerClass>> GetCustomerClassesAsync();
    }
}
