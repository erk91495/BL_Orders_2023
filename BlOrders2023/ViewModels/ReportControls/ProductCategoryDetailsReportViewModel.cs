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
internal class ProductCategoryDetailsReportViewModel : IReportViewModel<ProductCategoryDetailsReport>
{
    private readonly IBLDatabase _db = App.GetNewDatabase();
    public ReportCategory ReportCategories => ReportCategory.Products;

    public List<PromptTypes> Prompts => [PromptTypes.DateRange, PromptTypes.ProductCategories];

    public string ReportDescription => "Gets detailed product totals by category.";

    public async Task<object?[]> GetData(object[] userInputs)
    {
        DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
        DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
        var items = (List<ProductCategory>)userInputs[2];
        List<ProductCategory> categories = items.ToList();

        var values = await _db.Orders.GetByPickupDateAsync(startDate, endDate);
        var orderItems = values.SelectMany(o => o.Items.Where(i => categories.Contains(i.Product.Category)));
        if (!orderItems.IsNullOrEmpty())
        {
            var products = await _db.Products.GetAsync();
            return [orderItems, products, startDate, endDate];

        }
        return [];
    }
}
