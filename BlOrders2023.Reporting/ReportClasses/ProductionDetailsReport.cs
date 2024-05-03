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
[System.ComponentModel.DisplayName("Production Details Report")]
public class ProductionDetailsReport : ReportBase
{
    private readonly IEnumerable<LiveInventoryItem> _items;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;
    public ProductionDetailsReport(CompanyInfo companyInfo, IEnumerable<LiveInventoryItem> items, DateTimeOffset startDate, DateTimeOffset endDate) : base(companyInfo)
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
                    column.RelativeColumn(10);
                    column.RelativeColumn(10);
                    column.RelativeColumn(3);
                    column.RelativeColumn(3);
                    column.RelativeColumn(4);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).AlignLeft().Text("ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Product Name").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Scanline").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Pack Date").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Net Weight").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Scan Date").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });


                foreach (var item in _items)
                {
                    //var totalWeight = 0f;
                    //var totalCases = 0;
                    table.Cell().Element(CellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Product.ProductName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Scanline}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item.PackDate:MM/dd/yy}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item.NetWeight:N2} lbs").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item.ScanDate:MM/dd/yy h:m:s tt}").Style(tableTextStyle);
                    //foreach(var item in group)
                    //{
                    //    table.Cell().ColumnSpan(2);
                    //    table.Cell().Element(CellStyle).Text($"{item.Scanline}").Style(tableTextStyle);
                    //    table.Cell().Element(CellStyle).Text($"{item.NetWeight:N2}").Style(tableTextStyle);
                    //    totalWeight += item.NetWeight ?? 0;
                    //    totalCases ++;
                    //}
                    //table.Cell().Element(FooterCellStyle);
                    //table.Cell().Element(FooterCellStyle).Text($"Sub Total:").Style(tableHeaderStyle);
                    //table.Cell().Element(FooterCellStyle).Text($"{totalCases} cs.").Style(tableHeaderStyle);
                    //table.Cell().Element(FooterCellStyle).Text($"{totalWeight:N2} lbs.").Style(tableHeaderStyle);


                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }

                }

                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"Totals:").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignCenter().Text($"{_items.Count()} cs.").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{_items.Sum(p => p.NetWeight):N2} lbs.").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle);


                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.PaddingVertical(2).AlignRight();
                }

            });
        });
    }
}
