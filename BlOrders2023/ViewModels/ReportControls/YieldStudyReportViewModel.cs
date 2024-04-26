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
internal class YieldStudyReportViewModel : IReportViewModel<YieldStudyReport>
{
    public readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories =>  ReportCategory.Products;

    public List<PromptTypes> Prompts => [PromptTypes.Date];

    public string ReportDescription => "Gets products produced and yeilds  for the given date.";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        DateTimeOffset date = (DateTimeOffset)userInputs[0];
        var items = await _db.Inventory.GetInventoryItemsAsync(date,date);
        return [items, date.DateTime];
    }
}
