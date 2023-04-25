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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Reporting
{
    public class ReportGenerator
    {
        public IDocument GenerateWholesaleInvoice(Order order)
        {
            return new WholesaleInvoice();
        }
    }
}
