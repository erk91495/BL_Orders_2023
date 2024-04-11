using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Data;
public interface IDiscountTable
{
    public Task<IEnumerable<Discount>> GetDiscountsAsync();
    public Task<IEnumerable<Discount>> GetDiscountsAsync(Product product);
    public Task<IEnumerable<Discount>> GetDiscountsAsync(WholesaleCustomer customer);
    public Task<IEnumerable<Discount>> GetDiscountsAsync(Product product, WholesaleCustomer customer);
    public Task<bool> UpsertAsync(Discount discount);
}
