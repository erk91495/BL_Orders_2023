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
internal class OutstandingBalancesReportViewModel : IReportViewModel<OutstandingBalancesReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Sales;

    public List<PromptTypes> Prompts => [PromptTypes.None];

    public string ReportDescription => "Gets all orders with outstanding balances.";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        return [await _db.Orders.GetUnpaidInvoicedInvoicesAsync()];
    }
}
