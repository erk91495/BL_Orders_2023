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

namespace BlOrders2023.ViewModels.ReportControls;
internal class ProductCategoryTotalsReportViewModel : IReportViewModel<ProductCategoryTotalsReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Products;

    public List<PromptTypes> Prompts => [PromptTypes.DateRange];

    public string ReportDescription => "Gets product sales totals by category.";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
        DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
        var values = await _db.Orders.GetByPickupDateAsync(startDate, endDate);
        var orderItems = values.SelectMany(o => o.Items);
        if (!orderItems.IsNullOrEmpty())
        {
            return [orderItems, startDate, endDate];
        }
        return [];
    }
}
