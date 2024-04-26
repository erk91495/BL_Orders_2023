using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using static BlOrders2023.Models.ReportPrompts;

namespace BlOrders2023.ViewModels.ReportControls;
internal class InventoryDetailsReportViewModel : IReportViewModel<InventoryDetailsReport>
{
    private IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Inventory;

    public List<PromptTypes> Prompts => [PromptTypes.DateRange];

    public string ReportDescription => "Gets scanned inventory, adjustments, and ordered totals for the given date range.";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
        DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
        var orders = await _db.Orders.GetNonFrozenByPickupDateAsync(startDate, endDate);
        var currentInventory = await _db.Inventory.GetInventoryTotalItemsAsync();
        var notFilled = await _db.Inventory.GetAllocatedNotReceivedTotalsAsync();
        return [currentInventory, orders, notFilled, startDate, endDate];
    }
}
