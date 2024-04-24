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
internal class ShippingItemAuditReportViewModel : IReportControlViewModel<ShippingItemAuditReport>
{
    public ReportCategory ReportCategories => ReportCategory.Miscellaneous;

    public List<PromptTypes> Prompts => [PromptTypes.AuditTrail];

    public async Task<object?[]> GetData(object[] userInputs)
    {
        return await Task.Run(object[]() => userInputs);
    }
}
