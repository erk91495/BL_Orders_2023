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
        var discounts = product.Discounts.Where(d => d.Customers.Any(c => c.CustID == customer.CustID) || d.Customers.IsNullOrEmpty());
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
        var activeDiscouns = discounts.Where(d => (d.Inactive == false) &&
                                                  ((d.StartDate == null && d.EndDate == null) || 
                                                  (DateTime.Today.Date >= d.StartDate.Value.Date  && DateTime.Today.Date <= d.EndDate.Value.Date)));
        //Apply Largest set price first and then apply percent discounts
        var workingPrice = price;
        var percentTotal = (decimal)activeDiscouns.Where(d => d.Type == Enums.DiscountTypes.PercentOff).Sum(d => d.Modifier);
        var setPrice = activeDiscouns.Where(d => d.Type == Enums.DiscountTypes.SetPrice).OrderByDescending(d => d.Modifier).FirstOrDefault();

        if (setPrice != null)
        {
            workingPrice = (decimal)setPrice.Modifier;
        }

        if(percentTotal > 0)
        {
            workingPrice *= (1 - (percentTotal / 100m));
        }
        return workingPrice;
    }
}
