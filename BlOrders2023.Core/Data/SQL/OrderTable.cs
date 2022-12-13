using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BlOrders2023.Core.Data.SQL
{
    public class OrderTable : IOrderTable
    {

        private readonly BLOrdersDBContext _db;

        public OrderTable(BLOrdersDBContext db)
        {
            _db = db;
        }

        public Task DeleteAsync(Order order)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Order>> GetAsync() =>
            await _db.Orders
                .OrderByDescending(order => order.PickupDate)
                .ThenBy(order => order.OrderID)
                .Take(5)
                .Include(order => order.Items)
                .Include(order => order.Customer)
                .AsNoTracking()
                .ToListAsync();

        public Task<IEnumerable<Order>> GetAsync(int orderID)
        {
            throw new NotImplementedException();
        }

        public Task<Order> UpsertAsync(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
