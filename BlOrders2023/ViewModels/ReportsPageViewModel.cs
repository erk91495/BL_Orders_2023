using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.ViewModels;

public class ReportsPageViewModel : ObservableRecipient
{

    internal Order? GetOrder(int orderID)
    {
        return App.GetNewDatabase().Orders.Get(orderID).FirstOrDefault();
    }

    internal IEnumerable<Order> GetOrdersByPickupDate(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetByPickupDate(startDate, endDate);
    }

    internal IEnumerable<Order> GetNonFrozenOrdersByPickupDate(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetNonFrozenByPickupDate(startDate, endDate);
    }

    internal IEnumerable<Order> GetOrdersByPickupDateThenName(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetByPickupDateThenName(startDate, endDate);
    }

    internal IEnumerable<OrderTotalsItem> GetOrderTotals(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().ShipDetails.GetOrderTotals(startDate, endDate);
    }

    internal IEnumerable<Order> GetUnpaidInvoicedInvoices(WholesaleCustomer customer)
    {
        return App.GetNewDatabase().Orders.GetUnpaidInvoicedInvoices(customer);
    }

    internal IEnumerable<Payment> GetWholesalePayments(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Payments.GetPayments(startDate.DateTime,endDate.DateTime);
    }
    internal async Task<IEnumerable<Order>> GetOrdersByCustomerIdAndPickupDateAsync(IEnumerable<int>  custIds, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await App.GetNewDatabase().Orders.GetByCustomerIDAndPickupDateAsync(custIds, startDate, endDate);
    }

    internal async Task<IEnumerable<Order>> GetOutstandingOrdersAsync()
    {
        return await App.GetNewDatabase().Orders.GetUnpaidInvoicedInvoicesAsync();
    }

    internal IEnumerable<Product> GetProducts()
    {
        return App.GetNewDatabase().Products.Get(null, false);
    }
    internal IEnumerable<Order> GetFrozenOrders(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetFrozenOrdersByPickupDate(startDate.Date, endDate.Date);
    }

    internal IEnumerable<InventoryItem> GetInventory()
    {
        return App.GetNewDatabase().Inventory.GetInventory();
    }

    internal IEnumerable<Order> GetOutOfStateOrders(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetOutOfStateOrders(startDate, endDate);
    }

    internal IEnumerable<WholesaleCustomer> GetWholesaleCustomers()
    {
        return App.GetNewDatabase().Customers.Get();
    }
}
