using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models.Helpers;

public static class PriceHelpers
{
    public static decimal CalculateCustomerPrice(Product product, WholesaleCustomer customer)
    {
        if (customer.CustomerClass.Class.Trim().Equals("Kettering"))
        {
            return decimal.Round(product.KetteringPrice, 2);
        }
        else
        {
            return decimal.Round(product.WholesalePrice * ((100M - customer.CustomerClass.DiscountPercent) / 100M), 2);
        }
    }
}
