// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BlOrders2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using BlOrders2023.Reporting.ReportClasses;
using QuestPDF.Fluent;
using System.IO;
using Windows.System;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Reporting
{
    public static class ReportGenerator
    {
        public static string GenerateWholesaleInvoice(Order order)
        {
            var report =  new WholesaleInvoice(order);

            Directory.CreateDirectory(Path.GetTempPath() + "\\BLOrders2023");
            var filePath = Path.GetTempPath() + "BLOrders2023\\" + order.OrderID + "_" + DateTime.Now.ToFileTime() + ".pdf";
            report.GeneratePdf(filePath);
            return filePath;
        }

        public static string GenerateWholesaleOrderPickupRecap(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var report = new WholesaleOrderPickupRecap(orders, startDate, endDate);
            Directory.CreateDirectory(Path.GetTempPath() + "\\BLOrders2023");
            var filePath = Path.GetTempPath() + "BLOrders2023\\" + "WholesaleOrderPickupRecap" + "_" + DateTime.Now.ToFileTime() + ".pdf";
            report.GeneratePdf(filePath);
            return filePath;
        }

        public static string GenerateWholesaleOrderTotals(IEnumerable<OrderTotalsItem> items, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var report = new WholesaleOrderTotals(items, startDate, endDate);
            Directory.CreateDirectory(Path.GetTempPath() + "\\BLOrders2023");
            var filePath = Path.GetTempPath() + "BLOrders2023\\" + "WholesaleOrderTotals" + "_" + DateTime.Now.ToFileTime() + ".pdf";
            report.GeneratePdf(filePath);
            return filePath;

        }

        public static string GenerateWholesalePaymentsReport(IEnumerable<Payment> payments, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var report = new WholesalePaymentsReport(payments, startDate, endDate);
            Directory.CreateDirectory(Path.GetTempPath() + "\\BLOrders2023");
            var filePath = Path.GetTempPath() + "BLOrders2023\\" + "WholesalePaymentsReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
            report.GeneratePdf(filePath);
            return filePath;
        }

        public static string GenerateUnpaidInvoicesReport(IEnumerable<Order> orders)
        {
            var report = new UnpaidInvoicesReport(orders);
            Directory.CreateDirectory(Path.GetTempPath() + "\\BLOrders2023");
            var filePath = Path.GetTempPath() + "BLOrders2023\\" + "UnpaidInvoicesReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
            report.GeneratePdf(filePath);
            return filePath;
        }

        public static string GenerateShippingList(Order order)
        {
            var report = new ShippingList(order);
            Directory.CreateDirectory(Path.GetTempPath() + "\\BLOrders2023");
            var filePath = Path.GetTempPath() + "BLOrders2023\\" + "ShippingList" + "_" + DateTime.Now.ToFileTime() + ".pdf";
            report.GeneratePdf(filePath);
            return filePath;
        }

        public static string GenerateAggregateInvoiceReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var report = new AggregateInvoiceReport(orders, startDate, endDate);
            Directory.CreateDirectory(Path.GetTempPath() + "\\BLOrders2023");
            var filePath = Path.GetTempPath() + "BLOrders2023\\" + "AggregateInvoiceReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
            report.GeneratePdf(filePath);
            return filePath;

        }

        public static string GenerateOutstandingBalancesReport(IEnumerable<Order> orders)
        {
            var report = new OutstandingBalancesReport(orders);
            Directory.CreateDirectory(Path.GetTempPath() + "\\BLOrders2023");
            var filePath = Path.GetTempPath() + "BLOrders2023\\" + "OutstandingBalancesReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
            report.GeneratePdf(filePath);
            return filePath;

        }

        public static string GenerateQuarterlySalesReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate) 
        {
            var report = new QuarterlySalesReport(orders, startDate, endDate);
            Directory.CreateDirectory(Path.GetTempPath() + "\\BLOrders2023");
            var filePath = Path.GetTempPath() + "BLOrders2023\\" + "QuarterlySalesReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
            report.GeneratePdf(filePath);
            return filePath;
        }

        public static string GenerateFrozenOrdersReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var report = new FrozenOrdersReport(orders, startDate, endDate);
            Directory.CreateDirectory(Path.GetTempPath() + "\\BLOrders2023");
            var filePath = Path.GetTempPath() + "BLOrders2023\\" + "FrozenOrdersReport" + "_" + DateTime.Now.ToFileTime() + ".pdf";
            report.GeneratePdf(filePath);
            return filePath;
        }
    }
}
