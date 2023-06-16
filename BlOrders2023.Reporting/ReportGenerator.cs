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
using Microsoft.UI.Xaml.Controls;

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
            var filePath = Path.GetTempPath() + "BLOrders2023\\" + "WholesaleOrderPickupRecap" + "_" + DateTime.Now.ToFileTime() + ".pdf";
            report.GeneratePdf(filePath);
            return filePath;

        }
    }
}
