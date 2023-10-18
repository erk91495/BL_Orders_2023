// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using BlOrders2023.Reporting.ReportClasses;
using QuestPDF.Fluent;
using System.Diagnostics;


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

    public string GenerateWholesaleInvoice(Order order)
    {
        var report =  new WholesaleInvoice(CompanyInfo, order);
        var filePath = TempPath + Path.DirectorySeparatorChar + order.OrderID + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateWholesaleOrderPickupRecap(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new WholesaleOrderPickupRecap(CompanyInfo, orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "WholesaleOrderPickupRecap" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateWholesaleOrderTotals(IEnumerable<OrderTotalsItem> items, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new WholesaleOrderTotals(CompanyInfo, items, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "WholesaleOrderTotals" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;

    }

    public string GenerateWholesalePaymentsReport(IEnumerable<Payment> payments, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new WholesalePaymentsReport(CompanyInfo, payments, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "WholesalePaymentsReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateUnpaidInvoicesReport(IEnumerable<Order> orders)
    {
        var report = new UnpaidInvoicesReport(CompanyInfo, orders);
        var filePath = TempPath + Path.DirectorySeparatorChar + "UnpaidInvoicesReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateShippingList(Order order)
    {
        var report = new ShippingList(CompanyInfo, order);
        var filePath = TempPath + Path.DirectorySeparatorChar + "ShippingList" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateAggregateInvoiceReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new AggregateInvoiceReport(CompanyInfo, orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "AggregateInvoiceReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;

    }

    public string GenerateOutstandingBalancesReport(IEnumerable<Order> orders)
    {
        var report = new OutstandingBalancesReport(CompanyInfo, orders);
        var filePath = TempPath + Path.DirectorySeparatorChar + "OutstandingBalancesReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;

    }

    public string GenerateQuarterlySalesReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate) 
    {
        var report = new QuarterlySalesReport(CompanyInfo, orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "QuarterlySalesReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateFrozenOrdersReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new FrozenOrdersReport(CompanyInfo, orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "FrozenOrdersReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GeneratePickList(Order order)
    {
        var report = new PickList(CompanyInfo, order);
        var filePath = TempPath + Path.DirectorySeparatorChar + $"{order.OrderID}_PickList" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GeneratePalletLoadingReport(Order order, IEnumerable<Pallet> pallets) 
    {
        List<IReport> palletPages = new List<IReport>();
        foreach (var pallet in pallets)
        {
            palletPages.Add( new PalletLoadingReport(CompanyInfo, order, pallet));
        }
        var filePath = TempPath + Path.DirectorySeparatorChar + $"{order.OrderID}_PalletLoading" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        Document.Merge(palletPages).UseContinuousPageNumbers().GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateAllocationSummaryReport(IEnumerable<Order> orders, AllocatorMode mode, DateTime allocationTime)
    {
        var report = new AllocationSummaryReport(CompanyInfo, orders, mode, allocationTime);
        var filePath = TempPath + Path.DirectorySeparatorChar + $"AllocationSummary" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateAllocationDetailsReport(IEnumerable<Order> orders, IEnumerable<AllocationGroup> allocationGroups, AllocatorMode mode, DateTime allocationTime)
    {
        var report = new AllocationDetailsReport(CompanyInfo, orders, allocationGroups, mode, allocationTime);
        var filePath = TempPath + Path.DirectorySeparatorChar + $"AllocationDetails" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateCurrentInventoryReport(IEnumerable<InventoryItem> inventory)
    {
        var report = new CurrentInventoryReport(CompanyInfo, inventory);
        var filePath = TempPath + Path.DirectorySeparatorChar + $"CurrentInventory" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateInventoryDetailsReport(IEnumerable<InventoryItem> inventory, IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new InventoryDetailsReport(CompanyInfo, inventory, orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + $"InventoryDetails" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateOutOfStateSalesReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new OutOfStateSalesReport(CompanyInfo, orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + nameof(OutOfStateSalesReport) + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }
}
