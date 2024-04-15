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
[System.ComponentModel.DisplayName("Current Inventory")]
public  class CurrentInventoryReport : ReportBase
{
    private readonly IEnumerable<InventoryTotalItem> _items;

    public CurrentInventoryReport(CompanyInfo companyInfo, IEnumerable<InventoryTotalItem> items)
        : base(companyInfo)
    {
        _items = items;
    }

    public override void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(20);
            page.MarginLeft(0.75f, Unit.Inch);
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
            row.RelativeItem(2).AlignCenter().Text($"{_companyInfo.ShortCompanyName} Current Inventory").Style(titleStyle);
            row.RelativeItem(1).AlignRight().Column(column =>
            {
                column.Item().Text($"From: {_reportDate.ToString("M/d/yy")}").Style(subTitleStyle);
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
                    column.RelativeColumn(6);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);

                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Product ID").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).Text("Product Name").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Quantity").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Manual Adjustment").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Total Invetory").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Last Adjustment").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in _items.OrderBy(i => i.SortIndex))
                {
                    table.Cell().Element(CellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.Product.ProductName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.Quantity}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.ManualAdjustments}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.Total}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.LastAdjustment}").Style(tableTextStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }

                }

                table.Cell().Element(FooterCellStyle).Text($"").Style(tableTextStyle);
                table.Cell().Element(FooterCellStyle).Text($"Totals:").Style(tableTextStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_items.Sum(i => i.Quantity)}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_items.Sum(i => i.ManualAdjustments)}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_items.Sum(i => i.Total)}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle);
                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.PaddingVertical(2);
                }

            });
        });
    }
}
