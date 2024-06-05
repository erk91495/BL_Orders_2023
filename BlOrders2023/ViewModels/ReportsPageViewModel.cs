using System.Windows.Forms.Design.Behavior;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.ViewModels;

public class ReportsPageViewModel : ObservableRecipient
{

    internal Order? GetOrder(int orderID)
    {
        return App.GetNewDatabase().Orders.Get(orderID).FirstOrDefault();
    }

    internal async Task<Order> GetOrderAsync(int id)
    {
        var orders = await App.GetNewDatabase().Orders.GetAsync(id);
        return orders.FirstOrDefault();
    }

    internal async Task<IEnumerable<Order>> GetOrdersByPickupDateAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await App.GetNewDatabase().Orders.GetByPickupDateAsync(startDate, endDate);
    }


    internal IEnumerable<Order> GetNonFrozenOrdersByPickupDate(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetNonFrozenByPickupDate(startDate, endDate);
    }

    internal IEnumerable<Order> GetOrdersByPickupDateThenName(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetByPickupDateThenName(startDate, endDate);
    }

    internal async Task<IEnumerable<Order>> GetOrdersByPickupDateThenNameAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await App.GetNewDatabase().Orders.GetByPickupDateThenNameAsync(startDate, endDate);
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

    internal IEnumerable<InventoryAdjustmentItem> GetInventory()
    {
        return App.GetNewDatabase().Inventory.GetInventoryAdjustments();
    }

    internal IEnumerable<Order> GetOutOfStateOrders(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetOutOfStateOrders(startDate, endDate);
    }

    internal IEnumerable<WholesaleCustomer> GetWholesaleCustomers()
    {
        return App.GetNewDatabase().Customers.Get();
    }

    internal IEnumerable<ShippingItem> GetShippingItems(ShippingItem item, bool? matchProductID = null,
                                                        bool? matchSerial = null, bool? matchPackDate = null,
                                                        bool? matchScanline = null, bool? matchLot = null,
                                                        DateTime? startDate = null, DateTime? endDate = null)
    {
        return App.GetNewDatabase().ShipDetails.GetShippingItems(item, matchProductID, matchSerial, matchPackDate, matchScanline, matchLot, startDate, endDate);
    }

    internal ShippingItem? GetShippingItem(string scanline)
    {
        return App.GetNewDatabase().ShipDetails.Get(scanline);
    }

    internal ShippingItem? GetShippingItemByLot(string lotCode)
    {
        return App.GetNewDatabase().ShipDetails.GetByLot(lotCode);
    }

    internal ShippingItem? GetShippingItem(int productID, string serial)
    {
        return App.GetNewDatabase().ShipDetails.Get(productID, serial);
    }

    internal IEnumerable<ProductCategory> GetProductCategories() => App.GetNewDatabase().ProductCategories.Get();
    internal IEnumerable<ProductCategory> GetTotalsCategories() => App.GetNewDatabase().ProductCategories.GetForReports();
    internal IEnumerable<InventoryTotalItem> GetInventoryTotals() => App.GetNewDatabase().Inventory.GetInventoryTotalItems();
    internal Dictionary<int, int> GetAllocatedNotReceivedTotals() => App.GetNewDatabase().Inventory.GetAllocatedNotReceivedTotals();
    internal async Task<IEnumerable<ProductTotalsItem>> GetProductTotalsAsync(DateTimeOffset startDate, DateTimeOffset endDate) => await App.GetNewDatabase().Reports.GetProductSalesTotalsAsync(startDate, endDate);
    internal async Task<IEnumerable<LiveInventoryItem>> GetLiveInventoryItems(DateTimeOffset date)
    {
        return await App.GetNewDatabase().Inventory.GetInventoryItemsAsync(date,date);
    }
}
