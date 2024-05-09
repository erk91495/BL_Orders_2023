using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Production Summary Report")]
public class ProductionSummaryReport : ReportBase
{
    private readonly IEnumerable<LiveInventoryItem> _items;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;
    public ProductionSummaryReport(CompanyInfo companyInfo, IEnumerable<LiveInventoryItem> items, DateTimeOffset startDate, DateTimeOffset endDate) : base(companyInfo)
    {
        _items = items;
        _startDate = startDate;
        _endDate = endDate;
    }

    public override void Compose(IDocumentContainer container)
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

    protected override void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem(2).AlignCenter().Text($"{_companyInfo.ShortCompanyName} Inventory Details").Style(titleStyle);
            row.RelativeItem(1).AlignRight().Column(column =>
            {
                column.Item().Text($"From: {_startDate.ToString("M/d/yy")}").Style(subTitleStyle);
                column.Item().Text($"To: {_endDate.ToString("M/d/yy")}").Style(subTitleStyle);
            });
        });
    }

    protected override void ComposeContent(IContainer container)
    {
        container.Column(column => {
            //Items
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(1);
                    column.RelativeColumn(1);
                    column.RelativeColumn(10);
                    column.RelativeColumn(3);
                    column.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle);
                    header.Cell().Element(CellStyle).AlignLeft().Text("ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Product Name").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Quantity").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Net Weight").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });


                foreach (var items in _items.GroupBy(i => i.ScanDate.Value.Date))
                {
                    table.Cell().ColumnSpan(5).Element(GroupHeader).Text($"Scanned On: {items.Key:d/M/yy}").Style(tableTextStyle);

                    static IContainer GroupHeader(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).BorderBottom(1).BorderColor(Colors.Black);
                    }

                    foreach(var item in items.GroupBy(i => i.ProductID).OrderBy(i => i.Key))
                    {
                        table.Cell();
                        table.Cell().Element(CellStyle).Text($"{item.Key}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).AlignCenter().Text($"{item.First().Product.ProductName}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Count()} cs.").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Sum(i => i.NetWeight):N2} lbs").Style(tableTextStyle);
                    }

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }

                }

                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"Totals:").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignCenter().Text($"{_items.Count()} cs.").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignCenter().Text($"{_items.Sum(p => p.NetWeight):N2} lbs.").Style(tableHeaderStyle);


                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.PaddingVertical(2);
                }

            });
        });
    }
}
