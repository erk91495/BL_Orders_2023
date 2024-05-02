using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;

namespace BlOrders2023.ViewModels.ReportControls;
internal class UnpaidInvoicesReportViewModel : IReportViewModel<UnpaidInvoicesReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Sales;

    public List<PromptTypes> Prompts => [PromptTypes.Customer];

    public string ReportDescription => "Gets all unpaid invoices for the given customer";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        var customer = (WholesaleCustomer)userInputs[0];
        var values = await _db.Orders.GetUnpaidInvoicedInvoicesAsync(customer);
        return [values];
    }
}
