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
internal class HistoricalQuarterlySalesReportViewModel : IReportViewModel<HistoricalQuarterlySalesReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Sales;

    public List<PromptTypes> Prompts => [PromptTypes.DateRange];

    public string ReportDescription => "Gets sales totals for the given date range compared to the previous two years.";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
        DateTimeOffset endDate = (DateTimeOffset)userInputs[1];

        var current = await _db.Reports.GetProductSalesTotalsAsync(startDate, endDate);
        var off1Year = await _db.Reports.GetProductSalesTotalsAsync(startDate.AddYears(-1), endDate.AddYears(-1));
        var off2Year = await _db.Reports.GetProductSalesTotalsAsync(startDate.AddYears(-2), endDate.AddYears(-2));

        var allItems = new List<IEnumerable<ProductTotalsItem>> { current, off1Year, off2Year };

        return [allItems, startDate, endDate];
    }
}
