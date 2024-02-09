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
public class ProductCategoryTotalsReport : IReport
{

    private readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
    private readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
    private readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
    private readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(9);
    private readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9).SemiBold();
    private readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(9);
    private readonly CompanyInfo _companyInfo;
    private readonly IEnumerable<ProductCategory> _categories;
    private readonly IEnumerable<OrderItem> _items;
    private readonly DateTime _reportDate = DateTime.Now;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;

    public ProductCategoryTotalsReport(CompanyInfo companyInfo,  IEnumerable<OrderItem> items, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        _companyInfo = companyInfo;
        _items = items;
        _startDate = startDate;
        _endDate = endDate;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(20);
            page.Size(PageSizes.Letter);

            page.Header().Height(100).Background(Colors.Grey.Lighten1);
            page.Header().Element(ComposeHeader);

            page.Content().Background(Colors.Grey.Lighten3);
            page.Content().Element(ComposeContent);

            page.Footer().Height(20).Background(Colors.Grey.Lighten1);
            page.Footer().Element(ComposeFooter);

        });
    }

    private void ComposeHeader(IContainer container)
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

    private void ComposeContent(IContainer container)
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

    private void ComposeFooter(IContainer container)
    {
        container.AlignBottom().Column(column =>
        {
            column.Item().AlignBottom().AlignRight().Row(footer =>
            {
                footer.RelativeItem().AlignLeft().Text(time =>
                {
                    time.Span("Printed: ").Style(subTitleStyle).Style(smallFooterStyle);
                    time.Span($"{_reportDate.ToString():d}").Style(smallFooterStyle);
                });

                footer.RelativeItem().AlignRight().Text(page =>
                {
                    page.Span("pg. ").Style(smallFooterStyle);
                    page.CurrentPageNumber().Style(smallFooterStyle);
                    page.Span(" of ").Style(smallFooterStyle);
                    page.TotalPages().Style(smallFooterStyle);
                });
            });

        });

    }
}
