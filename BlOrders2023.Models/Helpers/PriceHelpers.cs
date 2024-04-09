using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Models.Helpers;

public static class PriceHelpers
{
    public static decimal CalculateCustomerPrice(Product product, WholesaleCustomer customer)
    {
        var customerPrice = GetCustomerPrice(product, customer);
        var discounts = product.Discounts.Where(d => d.Customers.Contains(customer) || d.Customers.IsNullOrEmpty());
        if (!discounts.IsNullOrEmpty())
        {
            return ProcessDiscount(discounts.First(), customerPrice);
        }
        else
        {
            return customerPrice;
        }

    }

    private static decimal GetCustomerPrice(Product product, WholesaleCustomer customer)
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

    private static decimal ProcessDiscount(Discount discount, decimal price)
    {
        switch (discount.Type)
        {
            case Enums.DiscountTypes.SetPrice:
                return (decimal)discount.Modifier;
            case Enums.DiscountTypes.PercentOff:
                return price * ((decimal)discount.Modifier / 100m);
            case Enums.DiscountTypes.DollarsOff:
                return price - (decimal)discount.Modifier > 0 ? price - (decimal)discount.Modifier : 0;
            default:
                return price;
        }
    }
}
