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
internal class ProductionSummaryReportViewModel : IReportViewModel<ProductionSummaryReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Production;

    public List<PromptTypes> Prompts => [PromptTypes.DateRange];

    public string ReportDescription => "Gets a summary of products produced for the given date range";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
        DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
        var res = await _db.Inventory.GetInventoryItemsByScanDateAsync(startDate, endDate);
        return [res, startDate, endDate];
    }
}
