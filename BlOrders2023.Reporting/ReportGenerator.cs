// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using BlOrders2023.Reporting.ReportClasses;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Fluent;
using System.Diagnostics;
using System.Reflection;
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

    public IReport GetWholesaleInvoice(Order order, IEnumerable<ProductCategory> categoriesToTotal)
    {
        return new WholesaleInvoice(CompanyInfo, order, categoriesToTotal);
    }

    private IReport GetWholesaleOrderPickupRecap(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return new WholesaleOrderPickupRecap(CompanyInfo, orders, startDate, endDate);
    }

    private IReport GetWholesaleOrderTotals(IEnumerable<OrderTotalsItem> items, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return new WholesaleOrderTotals(CompanyInfo, items, startDate, endDate);
    }
    private IReport GetWholesalePaymentsReport(IEnumerable<Payment> payments, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return new WholesalePaymentsReport(CompanyInfo, payments, startDate, endDate);
    }

    private IReport GetUnpaidInvoicesReport(IEnumerable<Order> orders)
    {
        return new UnpaidInvoicesReport(CompanyInfo, orders);
    }

    private IReport GetShippingList(Order order)
    {
        return new ShippingList(CompanyInfo, order);
    }

    private IReport GetAggregateInvoiceReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return new AggregateInvoiceReport(CompanyInfo, orders, startDate, endDate);
    }

    private IReport GetOutstandingBalancesReport(IEnumerable<Order> orders)
    {
        return new OutstandingBalancesReport(CompanyInfo, orders);
    }

    private IReport GetQuarterlySalesReport(IEnumerable<ProductTotalsItem> items, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return new QuarterlySalesReport(CompanyInfo, items, startDate, endDate);
    }

    private IReport GetFrozenOrdersReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return new FrozenOrdersReport(CompanyInfo, orders, startDate, endDate);
    }

    private IReport GetPickList(Order order)
    {
        return new PickList(CompanyInfo, order);
    }

    private IReport GetPalletLoadingReport(Order order, IEnumerable<Pallet> pallets)
    {
        return new PalletLoadingReport(CompanyInfo, order, pallets);
    }

    private IReport GetAllocationSummaryReport(IEnumerable<Order> orders, AllocatorMode mode, DateTime allocationTime)
    {
        return new AllocationSummaryReport(CompanyInfo, orders, mode, allocationTime);
    }

    private IReport GetAllocationDetailsReport(IEnumerable<Order> orders, IEnumerable<AllocationGroup> allocationGroups, AllocatorMode mode, DateTime allocationTime)
    {
        return new AllocationDetailsReport(CompanyInfo, orders, allocationGroups, mode, allocationTime);
    }

    private IReport GetCurrentInventoryReport(IEnumerable<InventoryTotalItem> inventory)
    {
        return new CurrentInventoryReport(CompanyInfo, inventory); 
    }

    private IReport GetInventoryDetailsReport(IEnumerable<InventoryTotalItem> inventory, IEnumerable<Order> orders, Dictionary<int, int> allocatedNotReceived, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return new InventoryDetailsReport(CompanyInfo, inventory, orders, allocatedNotReceived, startDate, endDate);
    }

    private IReport GetOutOfStateSalesReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return new OutOfStateSalesReport(CompanyInfo, orders, startDate, endDate);
    }

    private IReport GetBillOfLadingReport(IEnumerable<Order> orders, IEnumerable<BillOfLadingItem> items, WholesaleCustomer customer, string carrierName, string trailerNumber, string trailerSeal, DateTime? appointmentDate)
    {
        return new BillOfLadingReport(CompanyInfo, orders, items, customer, carrierName, trailerNumber, trailerSeal, appointmentDate);
    }

    private IReport GetShippingItemAuditReport(IEnumerable<ShippingItem> items, ShippingItem item, IList<string> fieldsToMatch, DateTime? startDate, DateTime? endDate)
    {
        return new ShippingItemAuditReport(CompanyInfo, items, item, fieldsToMatch, startDate, endDate);
    }

    private IReport GetProductCategoryTotalsReport(IEnumerable<OrderItem> items, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return new ProductCategoryTotalsReport(CompanyInfo, items, startDate, endDate);
    }

    private IReport GetProductCategoryDetailsReport(IEnumerable<OrderItem> items, IEnumerable<Product> products, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return new ProductCategoryDetailsReport(CompanyInfo, items, products, startDate, endDate);
    }

    private IReport GetWholesaleInvoiceTotalsReport(WholesaleCustomer customer, IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return new WholesaleInvoiceTotalsReport(CompanyInfo, customer, orders, startDate, endDate);
    }

    private IReport GetHistoricalQuarterlySalesReport(List<IEnumerable<ProductTotalsItem>> totals, DateTime startDate, DateTime endDate)
    {
        return new HistoricalQuarterlySalesReport(CompanyInfo, totals, startDate, endDate);
    }

    private IReport GetHistoricalProductCategoryTotalsReport(IEnumerable<ProductCategory> categories, IEnumerable<IEnumerable<OrderItem>>items, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return new HistoricalProductCategoryTotalsReport(CompanyInfo, categories, items, startDate, endDate);
    }

    private IReport GetYieldStudyReport(IEnumerable<LiveInventoryItem> items, string lotCode)
    {
        return new YieldStudyReport(CompanyInfo, items, lotCode);
    }

    public IReport GetReport(Type type, params object[] args)
    {
        object[] companyArgs = [CompanyInfo];
        companyArgs = [.. companyArgs, .. args];
        return (IReport)Activator.CreateInstance(type, companyArgs);
    }

    public async Task<string> GenerateReportPDFAsync(IReport report, string filepath = null)
    {
        var path = filepath;
        if(path.IsNullOrEmpty())
        {
            path = TempPath + Path.DirectorySeparatorChar + report.GetFileName() + ".pdf";
        }
        await Task.Run(() => report.GeneratePdf(path));
        return path;
    }

    public async Task<string> GenerateReportPDFAsync(IEnumerable<IReport> reports, bool continuousPageNumers = false, string filepath = null)
    {
        if(reports.IsNullOrEmpty()) throw new ArgumentNullException(nameof(reports));
        if(reports.Count() <= 1)
        {
            return await GenerateReportPDFAsync(reports.First(), filepath);
        }
        else{

            var firstReport = reports.First();
            var reportType = firstReport.GetType();
            var path = filepath;
            if (filepath.IsNullOrEmpty())
            {
                path = TempPath + Path.DirectorySeparatorChar + firstReport.GetFileName() + ".pdf";
            }
            await Task.Run(() =>
            {
                if(continuousPageNumers)
                {
                    Document.Merge(reports).UseContinuousPageNumbers().GeneratePdf(path);
                }
                else
                {
                    Document.Merge(reports).UseOriginalPageNumbers().GeneratePdf(path);
                }
            });
            return path;
        }
    }


}
