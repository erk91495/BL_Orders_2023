using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using BlOrders2023.Models.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Category Totals Report")]
public class ProductCategoryTotalsReport : ReportBase
{
    private readonly IEnumerable<ProductCategory> _categories;
    private readonly IEnumerable<OrderItem> _items;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;

    public ProductCategoryTotalsReport(CompanyInfo companyInfo,  IEnumerable<OrderItem> items, DateTimeOffset startDate, DateTimeOffset endDate)
        : base(companyInfo)
    {
        _items = items;
        _startDate = startDate;
        _endDate = endDate;
    }

    protected override void ComposeHeader(IContainer container)
    {
        container.Column(headerCol =>
        {
            ///Header Row Contains invoice # Company Name and logo
            headerCol.Item().AlignCenter().PaddingBottom(10).Row(row =>
            {

                //Logo
                var res = Assembly.GetExecutingAssembly().GetManifestResourceStream("BlOrders2023.Reporting.Assets.Images.BLLogo.bmp");
                row.RelativeItem(1).AlignLeft().AlignMiddle().Height(75).Image(res).FitHeight();

                row.RelativeItem(3).AlignCenter().Column(col =>
                {
                    col.Item().AlignCenter().Text($"{_companyInfo.ShortCompanyName} Turkeys, Inc.").Style(titleStyle);
                    col.Item().AlignCenter().Text($"Product Category Totals Report").Style(titleStyle);
                });

                row.RelativeItem(1).AlignRight().Column(column =>
                {
                    column.Item().Text($"From: {_startDate.ToString("M/d/yy")}").Style(subTitleStyle);
                    column.Item().Text($"To: {_endDate.ToString("M/d/yy")}").Style(subTitleStyle);
                });
            });
        });
    }

    protected override void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            //Items
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(6);
                    column.RelativeColumn(2);
                    column.RelativeColumn(5);
                    column.RelativeColumn(4);
                    column.RelativeColumn(5);
                    column.RelativeColumn(4);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Category").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Quantity").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Pounds.").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("% of Total lbs. Sales").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Dollars").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("% of Total $ Sales").Style(tableHeaderStyle);


                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                var groupedItems = _items.GroupBy(i => i.Product.Category).OrderBy(i => i.Key != null ? i.Key.DisplayIndex : int.MaxValue );
                var totalDollars = _items.Sum(i => i.GetTotalPrice );
                var totalPounds = _items.Sum(i => i.PickWeight );
                foreach (var category in groupedItems)
                {
                    if(category.Key == null)
                    {
                        table.Cell().Element(CellStyle).Text($"Uncategorized").Style(tableTextStyle);
                    }
                    else
                    {
                        table.Cell().Element(CellStyle).Text($"{category.Key.CategoryName}").Style(tableTextStyle);
                    }
                    table.Cell().Element(CellStyle).AlignRight().Text($"{category.Sum(i => i.QuantityReceived)}").Style(tableTextStyle);
                    var totalWeight = category.Sum(i => i.PickWeight);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{totalWeight:N2}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{(totalWeight / totalPounds) * 100f:N2}%").Style(tableTextStyle);
                    var totalPrice = category.Sum(i => i.GetTotalPrice);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{totalPrice:C}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{(totalPrice / totalDollars) * 100m:N2}%").Style(tableTextStyle);
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }

                table.Cell().Element(FooterCellStyle).Text("Totals:").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{_items.Sum(i => i.QuantityReceived)}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{totalPounds:N2}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{totalDollars:C}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle);


                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.PaddingVertical(2).BorderTop(1).BorderColor(Colors.Black);
                }

            });
        });
    }
}
