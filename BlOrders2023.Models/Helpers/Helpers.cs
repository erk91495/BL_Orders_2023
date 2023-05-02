using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models.Helpers
{
    public static class Helpers
    {
        public static decimal CalculateCustomerPrice(Product product, WholesaleCustomer customer)
        {
            return decimal.Round(product.WholesalePrice * ((100M - customer.CustomerClass.DiscountPercent) / 100M),2);
        }
    }
}
