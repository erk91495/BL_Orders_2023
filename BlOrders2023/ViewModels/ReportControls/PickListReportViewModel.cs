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
internal class PickListReportViewModel : IReportViewModel<PickList>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Orders;

    public List<PromptTypes> Prompts => [PromptTypes.OrderID];

    public string ReportDescription => "Groups items by type for picking.";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        var order = (await _db.Orders.GetAsync((int)userInputs[0])).FirstOrDefault();
        if (order != null)
        {
            return [order];
        }
        return [];
    }
}
