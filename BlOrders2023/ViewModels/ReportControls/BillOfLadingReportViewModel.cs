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
internal class BillOfLadingReportViewModel : IReportViewModel<BillOfLadingReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Orders;

    public List<PromptTypes> Prompts => [PromptTypes.BillOfLading];

    public string ReportDescription => "Generates a bill of lading for the given order(s).";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        return await Task.Run(object[]() => userInputs);
    }
}
