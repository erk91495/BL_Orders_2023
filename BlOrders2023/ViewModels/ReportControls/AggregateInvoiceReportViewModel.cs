using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using static BlOrders2023.Models.ReportPrompts;
using BlOrders2023.Core.Data;
using BlOrders2023.Reporting.ReportClasses;

namespace BlOrders2023.ViewModels.ReportControls;
internal class AggregateInvoiceReportViewModel : IReportControlViewModel<AggregateInvoiceReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();

    public ReportCategory ReportCategories => ReportCategory.Orders;
    public List<PromptTypes> Prompts => [PromptTypes.Customers, PromptTypes.DateRange];

    public async Task<object?[]> GetData(object[] userInputs)
    {
        DateTimeOffset startDate = (DateTimeOffset)userInputs[1];
        DateTimeOffset endDate = (DateTimeOffset)userInputs[2];
        IEnumerable<int> ids = ((List<WholesaleCustomer>)userInputs[0]).Select(c => c.CustID);
        var values = await _db.Orders.GetByCustomerIDAndPickupDateAsync(ids, startDate, endDate);
        if (values.Any(o => o.Paid == true))
        {

            //TODO: NEED TO SHOW THIS DIALOG SOMEHOW
            //ContentDialog d = new()
            //{
            //    XamlRoot = XamlRoot,
            //    Title = "Warning",
            //    Content = $"One or more invoices on this report have already been marked paid",
            //    PrimaryButtonText = "ok",
            //};
            //await d.ShowAsync();
        }
        return [values, startDate, endDate];
    }
}
