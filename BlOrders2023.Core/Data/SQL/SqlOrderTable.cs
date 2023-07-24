using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Data.SQL;

internal class SqlOrderTable : IOrderTable
{
    #region Properties
    #endregion Properties
    #region Fields
    /// <summary>
    /// The DB context for the Bl orders database
    /// </summary>
    private readonly SqlBLOrdersDBContext _db;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Creates a new instance of the OrderTable
    /// </summary>
    /// <param name="db">The Db context for the ordres database</param>
    public SqlOrderTable(SqlBLOrdersDBContext db)
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
    public void Delete(Order order)
    {
        var match = _db.Orders.FirstOrDefault(_order => _order.OrderID == order.OrderID);
        if (match != null)
        {
            _db.Orders.Remove(match);
        }
        _db.SaveChanges();
    }

    /// <summary>
    /// Deletes the given Order from the database
    /// </summary>
    /// <param name="order">The Order to be deleted</param>
    /// <returns></returns>
    public async Task DeleteAsync(Order order)
    {
        var match = await _db.Orders.FirstOrDefaultAsync(_order => _order.OrderID == order.OrderID);
        if (match != null)
        {
            _db.Orders.Remove(match);
        }
        await _db.SaveChangesAsync();
    }

    public IEnumerable<Order> Get(int orderID)
    {
        return _db.Orders
            .Include(order => order.Items)
            .Include(order => order.Customer)
            .Include(order => order.ShippingItems)
            .Where(order => order.OrderID == orderID)
            .ToList();
    }

    public IEnumerable<Order> GetFrozenOrdersByPickupDate(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return _db.Orders
            .Where(o => o.Frozen == true && o.PickupDate >= startDate && o.PickupDate <= endDate)
            .OrderBy(o => o.PickupDate)
            .ThenBy(o => o.PickupTime)
            .ToList();
    }

    /// <summary>
    /// Gets all of the Orders from the database
    /// </summary>
    /// <returns>An IEnumerable<Order> of all of the database orders</returns>
    public async Task<IEnumerable<Order>> GetAsync(bool tracking = true)
    {
        if (tracking)
        {
            return await _db.Orders
            .OrderByDescending(order => order.PickupDate)
            .ThenBy(order => order.OrderID)
            .Where(order => order.PickupDate.Year >= DateTime.Now.Year - 2)
            //.Include(Order => Order.Items)
            //.Include(order => order.Customer)
            //.Include(Order => Order.ShippingItems)
            .ToListAsync();
        }
        else
        {
            return await _db.Orders
            .OrderByDescending(order => order.PickupDate)
            .ThenBy(order => order.OrderID)
            .Where(order => order.PickupDate.Year >= DateTime.Now.Year - 2)
            .Include(Order => Order.Items)
            .Include(order => order.Customer)
            .Include(Order => Order.ShippingItems)
            .AsNoTracking()
            .ToListAsync();
        }
    }
        

    /// <summary>
    /// Gets the Order matching the given OrderID
    /// </summary>
    /// <param name="orderID">The ID of the Order to get</param>
    /// <returns>An Order with the given id</returns>
    public async Task<IEnumerable<Order>> GetAsync(int orderID, bool tracking = true)
    {
        if (tracking){
            return await _db.Orders
            .Include(order => order.Items)
            .Include(order => order.Customer)
            .Include(order => order.ShippingItems)
            .Where(order => order.OrderID == orderID)
            .ToListAsync();
        }
        else
        {
            return await _db.Orders
            .Include(order => order.Items)
            .Include(order => order.Customer)
            .Include(order => order.ShippingItems)
            .Where(order => order.OrderID == orderID)
            .AsNoTracking()
            .ToListAsync();
        }
    }
        

    public IEnumerable<Order> GetByPickupDate(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return _db.Orders
            .Where(o => o.PickupDate >= startDate && o.PickupDate <= endDate)
            .OrderBy(o => o.PickupDate)
            .ThenBy(o => o.PickupTime)
            .ToList();
    }

    public async Task<IEnumerable<Order>> GetByCustomerIDAndPickupDateAsync(IEnumerable<int> CustomerIDs, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await _db.Orders
            .Where(o => CustomerIDs.Contains(o.CustID) && o.PickupDate >= startDate && o.PickupDate <= endDate)
            .ToListAsync();
    }

    public IEnumerable<Order> GetUnpaidInvoices(WholesaleCustomer customer)
    {
        return _db.Orders
            .Where(o => 
                o.CustID == customer.CustID 
                && o.OrderStatus == Models.Enums.OrderStatus.Invoiced)
            .ToList();
    }

    public async Task<IEnumerable<Order>> GetUnpaidInvoicesAsync()
    {
        return await _db.Orders.Where(o => o.OrderStatus == Models.Enums.OrderStatus.Invoiced).ToListAsync();
    }

    public IEnumerable<Order> GetByPickupDateThenName(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return _db.Orders
            .Where(o => o.PickupDate >= startDate && o.PickupDate <= endDate)
            .OrderBy(o => o.PickupDate)
            .ThenBy(o => o.Customer.CustomerName)
            .ThenBy(o => o.PickupTime)
            .ToList();
    }

    public Order Reload(Order order)
    {
        _db.ChangeTracker.Clear();
        return _db.Orders.Where(o => o.OrderID == order.OrderID).First();
        
    }

    /// <summary>
    /// Updates the database context with the given Order. If the Order does not exist it will be added to the db
    /// </summary>
    /// <param name="order"></param> The order to be updated
    /// <param name="overwrite"></param> setting this flag to true clobbers the current database values
    /// <returns></returns>
    public Order Upsert(Order order, bool overwrite = false)
    {
        _db.Update(order);
        //TODO: may be a cleaner way of doing this but it works for now
        if (overwrite)
        {
            foreach (var entry in _db.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
            {
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
            }
        }
        var res = _db.SaveChanges();
        return order;
    }

    /// <summary>
    /// Updates the database context with the given Order. If the Order does not exist it will be added to the db
    /// </summary>
    /// <param name="order">The Order to be added or updated</param>
    /// <returns>the updated Order</returns>
    public async Task<Order> UpsertAsync(Order order)
    {
        _db.Update(order);

        _ = await _db.SaveChangesAsync();
        return order;
    }


    #endregion Methods
}
