using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.ViewModels;

public class ReportsPageViewModel : ObservableRecipient
{

    public Order? GetOrder(int orderID)
    {
        return App.GetNewDatabase().Orders.Get(orderID).FirstOrDefault();
    }

    internal IEnumerable<Order> GetOrdersByPickupDate(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetByPickupDate(startDate, endDate);
    }

    internal IEnumerable<Order> GetOrdersByPickupDateThenName(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetByPickupDateThenName(startDate, endDate);
    }

    internal IEnumerable<OrderTotalsItem> GetOrderTotals(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().ShipDetails.GetOrderTotals(startDate, endDate);
    }

    internal IEnumerable<Order> GetUnpaidInvoices(WholesaleCustomer customer)
    {
        return App.GetNewDatabase().Orders.GetUnpaidInvoices(customer);
    }

    internal IEnumerable<Payment> GetWholesalePayments(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Payments.GetPayments(startDate.Date,endDate.Date);
    }
    internal async Task<IEnumerable<Order>> GetOrdersByCustomerIdAndPickupDateAsync(IEnumerable<int>  custIds, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await App.GetNewDatabase().Orders.GetByCustomerIDAndPickupDateAsync(custIds, startDate, endDate);
    }

    internal async Task<IEnumerable<Order>> GetOutstandingOrdersAsync()
    {
        return await App.GetNewDatabase().Orders.GetUnpaidInvoicesAsync();
    }
}
