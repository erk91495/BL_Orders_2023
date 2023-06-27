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

    internal IEnumerable<Order> GetUnpaidInvoices(WholesaleCustomer customer, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetUnpaidInvoices(customer, startDate, endDate);
    }

    internal IEnumerable<Payment> GetWholesalePayments(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Payments.GetPayments(startDate.Date,endDate.Date);
    }
}
