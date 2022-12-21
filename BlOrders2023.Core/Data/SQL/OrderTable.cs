using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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
                .Take(100)
                .Include(order => order.Items)
                .Include(order => order.Customer)
                .Include(order => order.ShippingItems)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Order>> GetAsync(int orderID) =>
            await _db.Orders
                .Include(order => order.Items)
                .Include(order => order.Customer)
                .Include(order => order.ShippingItems)
                .Where(order => order.OrderID == orderID)
                .ToListAsync();


        public Task<Order> UpsertAsync(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
