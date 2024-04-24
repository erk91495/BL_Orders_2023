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
internal class HistoricalProductCategoryTotalsReportViewModel : IReportControlViewModel<HistoricalProductCategoryTotalsReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Sales;

    public List<PromptTypes> Prompts => [PromptTypes.DateRange];

    public async Task<object?[]> GetData(object[] userInputs)
    {
        DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
        DateTimeOffset endDate = (DateTimeOffset)userInputs[1];

        var values = await _db.Orders.GetByPickupDateAsync(startDate, endDate);
        var current = values.SelectMany(o => o.Items);
        values = await _db.Orders.GetByPickupDateAsync(startDate.AddYears(-1), endDate.AddYears(-1));
        var off1Year = values.SelectMany(o => o.Items);
        values = await _db.Orders.GetByPickupDateAsync(startDate.AddYears(-2), endDate.AddYears(-2));
        var off2Year = values.SelectMany(o => o.Items);
        var categories = await _db.ProductCategories.GetAsync();

        var allItems = new List<IEnumerable<OrderItem>> { current, off1Year, off2Year };
        return [categories, allItems, startDate, endDate];
    }
}
