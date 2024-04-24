﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using CommunityToolkit.WinUI;
using static BlOrders2023.Models.ReportPrompts;

namespace BlOrders2023.ViewModels.ReportControls;
internal class WholesaleOrderTotalsViewModel : IReportControlViewModel<WholesaleOrderTotals>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Orders;

    public List<PromptTypes> Prompts => [PromptTypes.DateRange];

    public async Task<object?[]> GetData(object[] userInputs)
    {
        return await Task.Run( object?[]() =>
        { 
            DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
            DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
            var values = _db.ShipDetails.GetOrderTotals(startDate, endDate);
            return [values, startDate, endDate];
        });
    }
}
