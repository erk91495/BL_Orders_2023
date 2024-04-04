using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Data;
public interface IReportsTable
{
    public IEnumerable<ProductTotalsItem> GetProductSalesTotals(DateTimeOffset startDate, DateTimeOffset endDate);
    public Task<IEnumerable<ProductTotalsItem>> GetProductSalesTotalsAsync(DateTimeOffset startDate, DateTimeOffset endDate);
}
