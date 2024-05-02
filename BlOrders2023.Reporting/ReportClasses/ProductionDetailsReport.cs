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
                    column.RelativeColumn(2);
                    column.RelativeColumn(7);
                    column.RelativeColumn(2);
                    column.RelativeColumn(4);
                    column.RelativeColumn(4);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).AlignLeft().Text("Product ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Product Name").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Scanline").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Net Weight").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("ScanDate").Style(tableHeaderStyle);

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
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Scanline}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.NetWeight:N2}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.ScanDate:D}").Style(tableTextStyle);
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
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"Totals:").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{_items.Count()}").Style(tableHeaderStyle);
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
