using BlOrders2023.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Reporting.ReportClasses;

[System.ComponentModel.DisplayName("Wholesale Order Totals")]
public class WholesaleOrderTotals : ReportBase
{
    private readonly IEnumerable<OrderTotalsItem> _items;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;

    public WholesaleOrderTotals(CompanyInfo companyInfo, IEnumerable<OrderTotalsItem> items, DateTimeOffset startDate, DateTimeOffset endDate)
        :base(companyInfo)
    {
        _items = items;
        _startDate = startDate;
        _endDate = endDate;
    }

    protected override void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem(2).AlignCenter().Text($"{_companyInfo.ShortCompanyName} Wholesale Order Totals").Style(titleStyle);
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
                    column.RelativeColumn(8);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);

                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Product ID").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).Text("Product Name").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Quantity Ordered").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Quantity Received").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in _items)
                {
                    table.Cell().Element(CellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.ProductName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.TotalQuantity ?? 0}").Style(tableTextStyle);

                    table.Cell().Element(CellStyle).Text($"{item.TotalReceived ?? 0}").Style(tableTextStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }
            });
        });
    }
}
