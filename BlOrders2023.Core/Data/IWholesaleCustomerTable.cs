using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data
{
    internal interface IWholesaleCustomerTable
    {
        Task<IEnumerable<WholesaleCustomer>> GetAsync();
        Task<IEnumerable<WholesaleCustomer>> GetAsync(int orderID);
        Task<WholesaleCustomer> UpsertAsync(WholesaleCustomer order);
        Task DeleteAsync(WholesaleCustomer order);
    }
}
