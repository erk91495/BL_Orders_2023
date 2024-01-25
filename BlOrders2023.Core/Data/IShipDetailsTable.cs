using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data;

public interface IShipDetailsTable
{
    public Task<IEnumerable<ShippingItem>> GetAsync();
    public Task<bool> IsDuplicateScanline(string scanline);
    public Task UpsertAsync(List<ShippingItem> items);
    public Task UpsertAsync(ShippingItem item);
    public Task DeleteAsync(ShippingItem item);
    public IEnumerable<OrderTotalsItem> GetOrderTotals(DateTimeOffset startDate, DateTimeOffset endDate);
    public IEnumerable<ShippingItem> GetShippingItems(DateTimeOffset startDate, DateTimeOffset endDate);
}
