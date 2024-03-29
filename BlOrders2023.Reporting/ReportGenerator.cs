// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using BlOrders2023.Reporting.ReportClasses;
using QuestPDF.Fluent;
using System.Diagnostics;
using System.Security.AccessControl;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Reporting;

public class ReportGenerator
{
    private readonly string TempPath = Path.GetTempPath() + "BLOrders2023";
    private CompanyInfo CompanyInfo;

    public ReportGenerator(CompanyInfo companyInfo)
    {
        CompanyInfo = companyInfo;
        if(Directory.Exists(TempPath)) 
        {
            //This is where we cleanup the files in the tempdir
            foreach(var file in Directory.GetFiles(TempPath))
            {
                try
                {
                    File.Delete(file);
                }
                //the file is probably open so just log it and move on
                catch( Exception e)                    
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }
        else
        {
            Directory.CreateDirectory(TempPath);
        }
    }

    public async Task<string> GenerateWholesaleInvoice(Order order, IEnumerable<ProductCategory> categoriesToTotal)
    {
        var report =  new WholesaleInvoice(CompanyInfo, order, categoriesToTotal);
        var filePath = $"{TempPath}{Path.DirectorySeparatorChar}_{order.OrderID}_{order.Customer.CustomerName}_{DateTime.Now.ToFileTime()}.pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateWholesaleOrderPickupRecap(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new WholesaleOrderPickupRecap(CompanyInfo, orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "WholesaleOrderPickupRecap" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateWholesaleOrderTotals(IEnumerable<OrderTotalsItem> items, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new WholesaleOrderTotals(CompanyInfo, items, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "WholesaleOrderTotals" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;

    }

    public async Task<string> GenerateWholesalePaymentsReport(IEnumerable<Payment> payments, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new WholesalePaymentsReport(CompanyInfo, payments, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "WholesalePaymentsReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateUnpaidInvoicesReport(IEnumerable<Order> orders)
    {
        var report = new UnpaidInvoicesReport(CompanyInfo, orders);
        var filePath = TempPath + Path.DirectorySeparatorChar + "UnpaidInvoicesReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateShippingList(Order order)
    {
        var report = new ShippingList(CompanyInfo, order);
        var filePath = TempPath + Path.DirectorySeparatorChar + "ShippingList" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateAggregateInvoiceReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new AggregateInvoiceReport(CompanyInfo, orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "AggregateInvoiceReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;

    }

    public async Task<string> GenerateOutstandingBalancesReport(IEnumerable<Order> orders)
    {
        var report = new OutstandingBalancesReport(CompanyInfo, orders);
        var filePath = TempPath + Path.DirectorySeparatorChar + "OutstandingBalancesReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;

    }

    public async Task<string> GenerateQuarterlySalesReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate) 
    {
        var report = new QuarterlySalesReport(CompanyInfo, orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "QuarterlySalesReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateFrozenOrdersReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new FrozenOrdersReport(CompanyInfo, orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "FrozenOrdersReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GeneratePickList(Order order)
    {
        var report = new PickList(CompanyInfo, order);
        var filePath = TempPath + Path.DirectorySeparatorChar + $"{order.OrderID}_PickList" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GeneratePalletLoadingReport(Order order, IEnumerable<Pallet> pallets) 
    {
        List<IReport> palletPages = new List<IReport>();
        foreach (var pallet in pallets)
        {
            palletPages.Add( new PalletLoadingReport(CompanyInfo, order, pallet));
        }
        var filePath = TempPath + Path.DirectorySeparatorChar + $"{order.OrderID}_PalletLoading" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await Task.Run(() => 
        {
            Document.Merge(palletPages).UseContinuousPageNumbers().GeneratePdf(filePath);
        });
        return filePath;
    }

    public async Task<string> GenerateAllocationSummaryReport(IEnumerable<Order> orders, AllocatorMode mode, DateTime allocationTime)
    {
        var report = new AllocationSummaryReport(CompanyInfo, orders, mode, allocationTime);
        var filePath = TempPath + Path.DirectorySeparatorChar + $"AllocationSummary" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateAllocationDetailsReport(IEnumerable<Order> orders, IEnumerable<AllocationGroup> allocationGroups, AllocatorMode mode, DateTime allocationTime)
    {
        var report = new AllocationDetailsReport(CompanyInfo, orders, allocationGroups, mode, allocationTime);
        var filePath = TempPath + Path.DirectorySeparatorChar + $"AllocationDetails" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateCurrentInventoryReport(IEnumerable<InventoryTotalItem> inventory)
    {
        var report = new CurrentInventoryReport(CompanyInfo, inventory);
        var filePath = TempPath + Path.DirectorySeparatorChar + $"CurrentInventory" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateInventoryDetailsReport(IEnumerable<InventoryTotalItem> inventory, IEnumerable<Order> orders, Dictionary<int,int> allocatedNotReceived,DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new InventoryDetailsReport(CompanyInfo, inventory, orders, allocatedNotReceived, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + $"InventoryDetails" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateOutOfStateSalesReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new OutOfStateSalesReport(CompanyInfo, orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + nameof(OutOfStateSalesReport) + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateBillOfLadingReport(IEnumerable<Order> orders, IEnumerable<BillOfLadingItem> items, WholesaleCustomer customer, string carrierName, string trailerNumber, string trailerSeal, DateTime? appointmentDate)
    {
        var report = new BillOfLadingReport(CompanyInfo, orders, items, customer, carrierName, trailerNumber, trailerSeal, appointmentDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + nameof(BillOfLadingReport) + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateShippingItemAuditReport(IEnumerable<ShippingItem> items, ShippingItem item, IList<string> fieldsToMatch, DateTime? startDate, DateTime? endDate)
    {
        var report = new ShippingItemAuditReport(CompanyInfo, items, item, fieldsToMatch, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + nameof(ShippingItemAuditReport) + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateProductCategoryTotalsReport(IEnumerable<OrderItem> items, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new ProductCategoryTotalsReport(CompanyInfo, items, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + nameof(ProductCategoryTotalsReport) + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateProductCategoryDetailsReport(IEnumerable<OrderItem> items, IEnumerable<Product> products, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new ProductCategoryDetailsReport(CompanyInfo, items, products, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + nameof(ProductCategoryDetailsReport) + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    public async Task<string> GenerateWholesaleInvoiceTotalsReport(WholesaleCustomer customer, IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new WholesaleInvoiceTotalsReport(CompanyInfo, customer, orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + nameof(ProductCategoryDetailsReport) + "_" + DateTime.Now.ToFileTime() + ".pdf";
        await GernerateReportAsync(report, filePath);
        return filePath;
    }

    private async Task GernerateReportAsync(IReport report, string filepath)
    {
        await Task.Run(() => report.GeneratePdf(filepath));
    }
}
