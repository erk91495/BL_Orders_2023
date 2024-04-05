using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Historical Product Category Totals Report")]
public class HistoricalProductCategoryTotalsReport : ReportBase
{
    private readonly IEnumerable<ProductCategory?> _categories;
    private readonly IEnumerable<IEnumerable<OrderItem>> _items;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;

    protected readonly new TextStyle tableTextStyle = TextStyle.Default.FontSize(7);
    protected readonly new TextStyle tableHeaderStyle = TextStyle.Default.FontSize(7).SemiBold();


    public HistoricalProductCategoryTotalsReport(CompanyInfo companyInfo, IEnumerable<ProductCategory> categories, IEnumerable<IEnumerable<OrderItem>> items, DateTimeOffset startDate, DateTimeOffset endDate)
        : base(companyInfo)
    {
        _categories = categories.Union(new List<ProductCategory>() {null } );
        _items = items;
        _startDate = startDate;
        _endDate = endDate;
    }

    public override void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(20);
            page.Size(PageSizes.Letter.Landscape());

            page.Header().Height(100).Background(Colors.Grey.Lighten1);
            page.Header().Element(ComposeHeader);

            page.Content().Background(Colors.Grey.Lighten3);
            page.Content().Element(ComposeContent);

            page.Footer().Height(20).Background(Colors.Grey.Lighten1);
            page.Footer().Element(ComposeFooter);

        });
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
                    col.Item().AlignCenter().Text($"Historical Product Category Totals Report").Style(titleStyle);
                });

                row.RelativeItem(1).AlignRight().Column(column =>
                {
                    column.Item().Text($"From: {_startDate:M/d/yy}").Style(subTitleStyle);
                    column.Item().Text($"To: {_endDate:M/d/yy}").Style(subTitleStyle);
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
                    column.RelativeColumn(16);

                    column.RelativeColumn(4);
                    column.RelativeColumn(8);
                    column.RelativeColumn(8);

                    column.RelativeColumn(4);
                    column.RelativeColumn(8);
                    column.RelativeColumn(6);
                    column.RelativeColumn(8);
                    column.RelativeColumn(6);

                    column.RelativeColumn(4);
                    column.RelativeColumn(8);
                    column.RelativeColumn(6);
                    column.RelativeColumn(8);
                    column.RelativeColumn(6);
                });
                
                table.Header(header =>
                {
                    header.Cell();

                    header.Cell().ColumnSpan(3).AlignCenter().Text($"{_startDate.Year - 2}").Style(tableHeaderStyle);
                    
                    header.Cell().ColumnSpan(5).AlignCenter().Text($"{_startDate.Year - 1}").Style(tableHeaderStyle);
                    
                    header.Cell().ColumnSpan(5).AlignCenter().Text($"{_startDate.Year}").Style(tableHeaderStyle);

                    header.Cell().BorderRight(1).BorderColor(Colors.Black).Element(CellStyle).Text("Category").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).AlignRight().Text("Quant.").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Pounds").Style(tableHeaderStyle);
                    header.Cell().BorderRight(1).BorderColor(Colors.Black).Element(CellStyle).AlignRight().PaddingRight(8).Text("Dollars").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).AlignRight().Text("Quant.").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Pounds").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("YOY lbs.").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().PaddingRight(4).Text("Dollars").Style(tableHeaderStyle);
                    header.Cell().BorderRight(1).BorderColor(Colors.Black).Element(CellStyle).AlignRight().PaddingRight(4).Text("YOY Dollars").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).AlignRight().Text("Quant.").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Pounds").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("YOY lbs.").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().PaddingRight(4).Text("Dollars").Style(tableHeaderStyle); 
                    header.Cell().BorderRight(1).BorderColor(Colors.Black).Element(CellStyle).AlignRight().PaddingRight(4).Text("YOY Dollars").Style(tableHeaderStyle);


                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

         
                foreach (var category in _categories)
                {
                    var items = _items.ElementAt(0).Where(i => i.Product.Category == category).OrderBy(i => i.Product.Category != null ? i.Product.Category.DisplayIndex : int.MaxValue);
                    var items1Off = _items.ElementAt(1).Where(i => i.Product.Category == category).OrderBy(i => i.Product.Category != null ? i.Product.Category.DisplayIndex : int.MaxValue);
                    var items2Off = _items.ElementAt(2).Where(i => i.Product.Category == category).OrderBy(i => i.Product.Category != null ? i.Product.Category.DisplayIndex : int.MaxValue);

                    var totalWeight = items.Sum(i => i.PickWeight);
                    var totalPrice = items.Sum(i => i.GetTotalPrice);

                    var totalWeight1Off = items1Off.Sum(i => i.PickWeight);
                    var totalPrice1Off = items1Off.Sum(i => i.GetTotalPrice);

                    var totalWeight2Off = items2Off.Sum(i => i.PickWeight);
                    var totalPrice2Off = items2Off.Sum(i => i.GetTotalPrice);

                    float yoyLbs;
                    float yoyDollars;


                    if (category == null)
                    {
                        table.Cell().BorderRight(1).BorderColor(Colors.Black).Element(CellStyle).Text($"Uncategorized").Style(tableTextStyle);
                    }
                    else
                    {
                        table.Cell().BorderRight(1).BorderColor(Colors.Black).Element(CellStyle).Text($"{category.CategoryName}").Style(tableTextStyle);
                    }


                    table.Cell().Element(CellStyle).AlignRight().Text($"{items2Off.Sum(i => i.QuantityReceived)}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{totalWeight2Off:N2}").Style(tableTextStyle);
                    table.Cell().BorderRight(1).BorderColor(Colors.Black).Element(CellStyle).PaddingRight(4).AlignRight().Text($"{totalPrice2Off:C}").Style(tableTextStyle);

                    try
                    {
                        yoyLbs = (((float)totalWeight1Off / (float)totalWeight2Off) - 1) * 100.0f;
                    }
                    catch (DivideByZeroException)
                    {
                        yoyLbs = float.NaN;
                    }
                    try
                    {
                        yoyDollars = (((float)totalPrice1Off / (float)totalPrice2Off) - 1) * 100.0f;
                    }
                    catch (DivideByZeroException)
                    {
                        yoyDollars = float.NaN;
                    }

                    table.Cell().Element(CellStyle).AlignRight().Text($"{items1Off.Sum(i => i.QuantityReceived)}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{totalWeight1Off:N2}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{yoyLbs:N2}%").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{totalPrice1Off:C}").Style(tableTextStyle);
                    table.Cell().BorderRight(1).BorderColor(Colors.Black).Element(CellStyle).AlignRight().PaddingRight(4).Text($"{yoyDollars:N2}%").Style(tableTextStyle);

                    totalWeight = items.Sum(i => i.PickWeight);
                    totalPrice = items.Sum(i => i.GetTotalPrice);
                    try
                    {
                        yoyLbs = (((float)totalWeight / (float)totalWeight1Off) -1) * 100.0f;
                    }
                    catch (DivideByZeroException)
                    {
                        yoyLbs = float.NaN;
                    }
                    try
                    {
                        yoyDollars = (((float)totalPrice / (float)totalPrice1Off) -1) * 100.0f;
                    }
                    catch (DivideByZeroException)
                    {
                        yoyDollars = float.NaN;
                    }
                    table.Cell().Element(CellStyle).AlignRight().Text($"{items.Sum(i => i.QuantityReceived)}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{totalWeight:N2}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{yoyLbs:N2}%").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{totalPrice:C}").Style(tableTextStyle);
                    table.Cell().BorderRight(1).BorderColor(Colors.Black).Element(CellStyle).AlignRight().PaddingRight(4).Text($"{yoyDollars:N2}%").Style(tableTextStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }

                var totalDollars = _items.ElementAt(0).Sum(i => i.GetTotalPrice);
                var totalPounds = _items.ElementAt(0).Sum(i => i.PickWeight);

                var totalDollars1Off = _items.ElementAt(1).Sum(i => i.GetTotalPrice);
                var totalPounds1Off = _items.ElementAt(1).Sum(i => i.PickWeight);

                var totalDollars2Off = _items.ElementAt(2).Sum(i => i.GetTotalPrice);
                var totalPounds2Off = _items.ElementAt(2).Sum(i => i.PickWeight);

                table.Cell().Element(FooterCellStyle).Text("Totals:").Style(tableHeaderStyle);

                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{_items.ElementAt(2).Sum(i => i.QuantityReceived)}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{totalPounds2Off:N2}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().PaddingRight(4).Text($"{totalDollars2Off:C}").Style(tableHeaderStyle);

                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{_items.ElementAt(1).Sum(i => i.QuantityReceived)}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{totalPounds1Off:N2}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{totalDollars1Off:C}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle);

                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{_items.ElementAt(0).Sum(i => i.QuantityReceived)}").Style(tableHeaderStyle);
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
