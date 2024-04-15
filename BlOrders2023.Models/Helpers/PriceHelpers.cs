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

            return ProcessDiscount(discounts, customerPrice);
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

    private static decimal ProcessDiscount(IEnumerable<Discount> discounts, decimal price)
    {
        //Apply Largest set price first and then apply percent discounts
        var workingPrice = price;
        var percentTotal = (decimal)discounts.Where(d => d.Type == Enums.DiscountTypes.PercentOff).Sum(d => d.Modifier);
        var setPrice = discounts.Where(d => d.Type == Enums.DiscountTypes.SetPrice).OrderByDescending(d => d.Modifier).FirstOrDefault();

        if (setPrice != null)
        {
            workingPrice = (decimal)setPrice.Modifier;
        }

        if(percentTotal > 0)
        {
            workingPrice =  workingPrice * (1 - (percentTotal / 100m));
        }
        return workingPrice;
    }
}
