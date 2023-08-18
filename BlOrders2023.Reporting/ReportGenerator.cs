// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BlOrders2023.Models;
using BlOrders2023.Reporting.ReportClasses;
using QuestPDF.Fluent;
using System.Diagnostics;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Reporting;

public class ReportGenerator
{
    private readonly string TempPath = Path.GetTempPath() + "BLOrders2023";

    public ReportGenerator()
    {
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
        var report =  new WholesaleInvoice(order);
        var filePath = TempPath + Path.DirectorySeparatorChar + order.OrderID + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateWholesaleOrderPickupRecap(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new WholesaleOrderPickupRecap(orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "WholesaleOrderPickupRecap" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateWholesaleOrderTotals(IEnumerable<OrderTotalsItem> items, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new WholesaleOrderTotals(items, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "WholesaleOrderTotals" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;

    }

    public string GenerateWholesalePaymentsReport(IEnumerable<Payment> payments, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new WholesalePaymentsReport(payments, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "WholesalePaymentsReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateUnpaidInvoicesReport(IEnumerable<Order> orders)
    {
        var report = new UnpaidInvoicesReport(orders);
        var filePath = TempPath + Path.DirectorySeparatorChar + "UnpaidInvoicesReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateShippingList(Order order)
    {
        var report = new ShippingList(order);
        var filePath = TempPath + Path.DirectorySeparatorChar + "ShippingList" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateAggregateInvoiceReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new AggregateInvoiceReport(orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "AggregateInvoiceReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;

    }

    public string GenerateOutstandingBalancesReport(IEnumerable<Order> orders)
    {
        var report = new OutstandingBalancesReport(orders);
        var filePath = TempPath + Path.DirectorySeparatorChar + "OutstandingBalancesReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;

    }

    public string GenerateQuarterlySalesReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate) 
    {
        var report = new QuarterlySalesReport(orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "QuarterlySalesReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GenerateFrozenOrdersReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        var report = new FrozenOrdersReport(orders, startDate, endDate);
        var filePath = TempPath + Path.DirectorySeparatorChar + "FrozenOrdersReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GeneratePickList(Order order)
    {
        var report = new PickList(order);
        var filePath = TempPath + Path.DirectorySeparatorChar + $"{order.OrderID}_PickList" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        report.GeneratePdf(filePath);
        return filePath;
    }

    public string GeneratePalletLoadingReport(Order order, IEnumerable<Pallet> pallets) 
    {
        List<IReport> palletPages = new List<IReport>();
        foreach (var pallet in pallets)
        {
            palletPages.Add( new PalletLoadingReport(order, pallet));
        }
        var filePath = TempPath + Path.DirectorySeparatorChar + $"{order.OrderID}_PalletLoading" + "_" + DateTime.Now.ToFileTime() + ".pdf";
        Document.Merge(palletPages).GeneratePdf(filePath);
        return filePath;
    }
}
