using BlOrders2023.Models;

namespace BlOrders2023.Core.Data;

public interface IOrderTable
{
    Task<IEnumerable<Order>> GetAsync(bool tracking = true);
    Task<IEnumerable<Order>> GetAsync(int orderID, bool tracking = true);
    Task<IEnumerable<Order>> GetAsync(IEnumerable<int> ids, bool tracking = true);
    IEnumerable<Order> Get(int orderID);
    IEnumerable<Order> GetFrozenOrdersByPickupDate(DateTimeOffset startDate, DateTimeOffset endDate);
    IEnumerable<Order> GetByPickupDate(DateTimeOffset startDate, DateTimeOffset endDate);
    IEnumerable<Order> GetNonFrozenByPickupDate(DateTimeOffset startDate, DateTimeOffset endDate);
    IEnumerable<Order> GetByPickupDateThenName(DateTimeOffset startDate, DateTimeOffset endDate);
    Task<IEnumerable<Order>> GetByCustomerIDAndPickupDateAsync(IEnumerable<int> CustomerIDs, DateTimeOffset startDate, DateTimeOffset endDate);
    IEnumerable<Order> GetUnpaidInvoices(WholesaleCustomer customer);
    Order Reload(Order orderID);
    Task<Order> UpsertAsync(Order order);
    Task<int> UpsertAsync(IEnumerable<Order> orders);
    Task DeleteAsync(Order order);
    Order Upsert(Order order, bool overwrite=false);
    void Delete(Order order);
    Task<IEnumerable<Order>> GetUnpaidInvoicesAsync();
}
