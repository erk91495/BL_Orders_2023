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
[System.ComponentModel.DisplayName("Inventory Details Report")]
public class InventoryDetailsReport : IReport
{
    private readonly CompanyInfo _copanyInfo;
    private readonly IEnumerable<InventoryItem> _items;
    private readonly IEnumerable<Order> _orders;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;
    private readonly DateTime _reportDate = DateTime.Now;

    public static readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
    public static readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
    public static readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
    public static readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(9);
    public static readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9).SemiBold();
    public static readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(9);

    public InventoryDetailsReport(CompanyInfo companyInfo, IEnumerable<InventoryItem> items, IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        _copanyInfo = companyInfo;
        _items = items;
        _orders = orders;
        _startDate = startDate;
        _endDate = endDate;
    }

    public void Compose(IDocumentContainer container)
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

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem(2).AlignCenter().Text($"{_copanyInfo.ShortCompanyName} Inventory Details").Style(titleStyle);
            row.RelativeItem(1).AlignRight().Column(column =>
            {
                column.Item().Text($"From: {_startDate.ToString("M/d/yy")}").Style(subTitleStyle);
                column.Item().Text($"To: {_endDate.ToString("M/d/yy")}").Style(subTitleStyle);
            });
        });
    }

    private void ComposeContent(IContainer container)
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
                    column.RelativeColumn(2);

                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Product ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Product Name").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Ordered").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Allocated").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Received").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Allocated - Received").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Available Inventory").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in _items.OrderBy(i => i.SortIndex))
                {
                    var allOrderItems = _orders.SelectMany(o => o.Items.Where(i => i.ProductID == item.ProductID));
                    var totalOrdered = allOrderItems.Sum(i => i.Quantity);
                    var totalAllocated = allOrderItems.Sum(i => i.QuanAllocated);
                    var totalReceived = allOrderItems.Sum(i => i.QuantityReceived);
                    table.Cell().Element(CellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.ProductName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{totalOrdered}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{totalAllocated}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{totalReceived}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{totalAllocated - totalReceived}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.QuantityOnHand}").Style(tableTextStyle);


                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
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
