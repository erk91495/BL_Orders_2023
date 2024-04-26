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
internal class WholesaleOrderPickupRecapViewModel : IReportViewModel<WholesaleOrderPickupRecap>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Orders;

    public List<PromptTypes> Prompts => [PromptTypes.DateRange, PromptTypes.OrderByDateOrAlphabetical];

    public string ReportDescription => "Gets the orders to be picked up during the given date range. Can be ordered by pickup date or alphabetical.";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
        DateTimeOffset endDate = (DateTimeOffset)userInputs[1];


        //var values = _db.ShipDetails.GetOrderTotals(startDate, endDate);

        var orders = (bool)userInputs[2] ? await _db.Orders.GetByPickupDateAsync(startDate, endDate) : _db.Orders.GetByPickupDateThenName(startDate, endDate);
        return [orders, startDate, endDate];
    }
}
