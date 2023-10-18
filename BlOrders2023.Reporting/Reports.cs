using BlOrders2023.Reporting.ReportClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Reporting;

public static class Reports
{
    public static readonly Collection<Type> AvalibleReports = new()
    { 
        typeof(WholesaleInvoice), 
        typeof(WholesaleOrderPickupRecap),
        typeof(WholesaleOrderTotals),
        typeof(WholesalePaymentsReport),
        typeof(ShippingList),
        typeof(UnpaidInvoicesReport),
        typeof(AggregateInvoiceReport),
        typeof(OutstandingBalancesReport),
        typeof(QuarterlySalesReport),
        typeof(FrozenOrdersReport),
        typeof(PickList),
        typeof(PalletLoadingReport),
        typeof(CurrentInventoryReport),
        typeof(InventoryDetailsReport),
        typeof(OutOfStateSalesReport),
    }; 
}
