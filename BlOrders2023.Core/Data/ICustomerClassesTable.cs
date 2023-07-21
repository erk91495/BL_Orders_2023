using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Data;
public interface ICustomerClassesTable
{
    Task<IEnumerable<CustomerClass>> GetCustomerClassesAsync(string query = null, bool asNoTracking = false);

    Task<int> UpsertAsync(CustomerClass custClass);
}
