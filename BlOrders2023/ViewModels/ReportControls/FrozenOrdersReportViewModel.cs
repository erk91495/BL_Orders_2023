﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;

namespace BlOrders2023.ViewModels.ReportControls;
internal class FrozenOrdersReportViewModel : IReportViewModel<FrozenOrdersReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Orders;

    public List<PromptTypes> Prompts => [PromptTypes.DateRange];

    public string ReportDescription => "Gets frozen orders for the given date range.";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
        DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
        var values = await _db.Orders.GetFrozenOrdersByPickupDateAsync(startDate, endDate);
        return [values, startDate, endDate];
    }
}
