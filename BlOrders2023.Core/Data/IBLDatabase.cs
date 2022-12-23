using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data
{
    public interface IBLDatabase
    {
        public IOrderTable Orders { get; }
        public IWholesaleCustomerTable Customers { get; }
        public IProductsTable Products { get; }
    }
}
