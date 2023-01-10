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
        #region Properties
        #endregion Properties
        #region Fields
        /// <summary>
        /// The DB context for the Bl orders database
        /// </summary>
        private readonly BLOrdersDBContext _db;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Creates a new instance of the OrderTable
        /// </summary>
        /// <param name="db">The Db context for the ordres database</param>
        public OrderTable(BLOrdersDBContext db)
        {
            _db = db;
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Deletes the given Order from the database
        /// </summary>
        /// <param name="order">The Order to be deleted</param>
        /// <returns></returns>
        public Task DeleteAsync(Order order)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all of the Orders from the database
        /// </summary>
        /// <returns>An IEnumerable<Order> of all of the database orders</returns>
        public async Task<IEnumerable<Order>> GetAsync() =>
            await _db.Orders
                .OrderByDescending(order => order.PickupDate)
                .ThenBy(order => order.OrderID)
                //.Take(100)
                //.Include(Order => Order.Items)
                //.Include(order => order.Customer)
                //.Include(Order => Order.ShippingItems)
                .ToListAsync();
        
        /// <summary>
        /// Gets the Order matching the given OrderID
        /// </summary>
        /// <param name="orderID">The ID of the Order to get</param>
        /// <returns>An Order with the given id</returns>
        public async Task<IEnumerable<Order>> GetAsync(int orderID) =>
            await _db.Orders
                .Include(order => order.Items)
                .Include(order => order.Customer)
                .Include(order => order.ShippingItems)
                .Where(order => order.OrderID == orderID)
                .ToListAsync();

        /// <summary>
        /// Updates the database context with the given Order. If the Order does not exist it will be added to the db
        /// </summary>
        /// <param name="order">The Order to be added or updated</param>
        /// <returns>the updated Order</returns>
        public async Task<Order> UpsertAsync(Order order)
        {
            var exists = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(_order => order.OrderID == _order.OrderID);
            if(exists == null) 
            {
                _db.Orders.Add(order); 
            }
            else
            {
                //TODO: Concurrency checks maybe here
                _db.Entry(exists).CurrentValues.SetValues(order);
                foreach(var item in order.Items)
                {
                    _db.Entry(item).CurrentValues.SetValues(item);
                }

            }
            if(_db.ChangeTracker.HasChanges() == false)
            {
                var i = 0;
            }
            int res =  await _db.SaveChangesAsync();
            return order;
        }
        #endregion Methods
    }
}
