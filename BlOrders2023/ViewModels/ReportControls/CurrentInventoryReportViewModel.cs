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
internal class CurrentInventoryReportViewModel : IReportViewModel<CurrentInventoryReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Inventory;

    public List<PromptTypes> Prompts => [PromptTypes.None];

    public string ReportDescription => "Gets the current inventory and adjustments.";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        return [await _db.Inventory.GetInventoryTotalItemsAsync()];
    }
}
