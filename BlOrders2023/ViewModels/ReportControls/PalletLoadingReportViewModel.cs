using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Data;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Core.Services;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace BlOrders2023.ViewModels.ReportControls;
internal class PalletLoadingReportViewModel : IReportViewModel<PalletLoadingReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Orders;

    public List<PromptTypes> Prompts => [PromptTypes.OrderID];

    public string ReportDescription => "Groups items by pallet for the given order.";

    public async Task<object?[]> GetData(object[] userInputs) 
    {
        var id = (int)userInputs[0];
        var order = (await _db.Orders.GetAsync(id)).FirstOrDefault();
        if (order != null)
        {
            IPalletizer palletizer = new BoxPalletizer(new PalletizerConfig() { SingleItemPerPallet = order.Customer.SingleProdPerPallet ?? false }, order);
            var pallets = await palletizer.PalletizeAsync();
            return [order, pallets];
        }
        return [];
    }
}
