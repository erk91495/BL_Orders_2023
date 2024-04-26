using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using static BlOrders2023.Models.ReportPrompts;

namespace BlOrders2023.ViewModels.ReportControls;
internal class WholesaleInvoiceReportViewModel : IReportViewModel<WholesaleInvoice>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Invoicing;
    public List<PromptTypes> Prompts => [PromptTypes.OrderID];

    public string ReportDescription => "Get the invoice for the given order number.";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        var id = (int)userInputs[0];
        var order = (await _db.Orders.GetAsync(id)).FirstOrDefault();
        if (order != null)
        {
            var toTotal = await _db.ProductCategories.GetForReportsAsync();
            return [order, toTotal];
        }
        else
        {
            return [];
        }
    }
}
