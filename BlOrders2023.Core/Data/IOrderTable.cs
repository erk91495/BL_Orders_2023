using BlOrders2023.Models;

namespace BlOrders2023.Core.Data;

public interface IOrderTable
{
    Task<IEnumerable<Order>> GetAsync(bool tracking = true);
    Task<IEnumerable<Order>> GetAsync(int orderID, bool tracking = true);
    Task<IEnumerable<Order>> GetAsync(IEnumerable<int> ids, bool tracking = true);
    IEnumerable<Order> Get(int orderID);
    IEnumerable<Order> GetFrozenOrdersByPickupDate(DateTimeOffset startDate, DateTimeOffset endDate);
    Task<IEnumerable<Order>> GetFrozenOrdersByPickupDateAsync(DateTimeOffset startDate, DateTimeOffset endDate);
    Task<IEnumerable<Order>> GetByPickupDateAsync(DateTimeOffset startDate, DateTimeOffset endDate);
    IEnumerable<Order> GetNonFrozenByPickupDate(DateTimeOffset startDate, DateTimeOffset endDate);
    Task<IEnumerable<Order>> GetNonFrozenByPickupDateAsync(DateTimeOffset startDate, DateTimeOffset endDate);
    IEnumerable<Order> GetByPickupDateThenName(DateTimeOffset startDate, DateTimeOffset endDate);
    Task<IEnumerable<Order>> GetByPickupDateThenNameAsync(DateTimeOffset startDate, DateTimeOffset endDate);
    Task<IEnumerable<Order>> GetByCustomerIDAndPickupDateAsync(IEnumerable<int> CustomerIDs, DateTimeOffset startDate, DateTimeOffset endDate);
    IEnumerable<Order> GetUnpaidInvoices(WholesaleCustomer customer);
    IEnumerable<Order> GetUnpaidInvoicedInvoices(WholesaleCustomer customer);
    Order Reload(Order orderID);
    Task<Order> UpsertAsync(Order order);
    Task<int> UpsertAsync(IEnumerable<Order> orders);
    Task DeleteAsync(Order order);
    Order Upsert(Order order, bool overwrite=false);
    void Delete(Order order);
    Task<IEnumerable<Order>> GetUnpaidInvoicedInvoicesAsync(WholesaleCustomer customer = null);
    IEnumerable<Order> GetOutOfStateOrders(DateTimeOffset startDate, DateTimeOffset endDate);
    Task<IEnumerable<Order>> GetOutOfStateOrdersAsync(DateTimeOffset startDate, DateTimeOffset endDate);
}
