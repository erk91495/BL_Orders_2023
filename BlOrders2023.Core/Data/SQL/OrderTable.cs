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
        /// Deletes the given order from the database
        /// </summary>
        /// <param name="order">The order to be deleted</param>
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
                //.Take(5000)
                //.Include(order => order.Items)
                .Include(order => order.Customer)
                //.Include(order => order.ShippingItems)
                //.AsNoTracking()
                .AsTracking(QueryTrackingBehavior.TrackAll)
                .ToListAsync();
        
        /// <summary>
        /// Gets the order matching the given OrderID
        /// </summary>
        /// <param name="orderID">The ID of the order to get</param>
        /// <returns>An order with the given id</returns>
        public async Task<IEnumerable<Order>> GetAsync(int orderID) =>
            await _db.Orders
                .Include(order => order.Items)
                .Include(order => order.Customer)
                .Include(order => order.ShippingItems)
                .Where(order => order.OrderID == orderID)
                .ToListAsync();

        /// <summary>
        /// Updates the database context with the given order. If the order does not exist it will be added to the db
        /// </summary>
        /// <param name="order">The order to be added or updated</param>
        /// <returns>the updated order</returns>
        public async Task<Order> UpsertAsync(Order order)
        {
            var exists = await _db.Orders.FirstOrDefaultAsync(_order => order.OrderID == _order.OrderID);
            if(exists == null) 
            {
                _db.Orders.Add(order); 
            }
            else
            {
                //TODO: Concurrency checks maybe here
                _db.Entry(exists).CurrentValues.SetValues(order);
            }
            await _db.SaveChangesAsync();
            return order;
        }
        #endregion Methods
    }
}
