using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using Microsoft.IdentityModel.Tokens;
using static BlOrders2023.Models.ReportPrompts;

namespace BlOrders2023.ViewModels.ReportControls;
internal class WholesaleInvoiceTotalsReportViewModel : IReportControlViewModel<WholesaleInvoiceTotalsReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Orders;

    public List<PromptTypes> Prompts => [PromptTypes.DateRange, PromptTypes.Customer];

    public async Task<object?[]> GetData(object[] userInputs)
    {
        DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
        DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
        WholesaleCustomer customer = (WholesaleCustomer)userInputs[2];

        List<int> ids = new List<int>() { customer.CustID };
        var orders = await _db.Orders.GetByCustomerIDAndPickupDateAsync(ids, startDate, endDate);
        if (!orders.IsNullOrEmpty())
        {
            return [customer, orders, startDate, endDate];
        }
        return [];
    }
}
