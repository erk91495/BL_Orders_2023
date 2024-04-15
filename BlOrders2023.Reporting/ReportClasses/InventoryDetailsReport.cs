using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Inventory Details Report")]
public class InventoryDetailsReport : ReportBase
{
    private readonly IEnumerable<InventoryTotalItem> _items;
    private readonly IEnumerable<Order> _orders;
    private readonly Dictionary<int, int> _allocatedNotReceived;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;

    public InventoryDetailsReport(CompanyInfo companyInfo, IEnumerable<InventoryTotalItem> items, IEnumerable<Order> orders, Dictionary<int,int> allocatedNotReceived, DateTimeOffset startDate, DateTimeOffset endDate)
        :base(companyInfo)
    {
        _items = items;
        _orders = orders;
        _allocatedNotReceived = allocatedNotReceived;
        _startDate = startDate;
        _endDate = endDate;
    }

    public override void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(20);
            page.MarginTop(0.75f, Unit.Inch);
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
                    column.RelativeColumn(2);
                    column.RelativeColumn(6);
                    //column.RelativeColumn(2);
                    //column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Product ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Product Name").Style(tableHeaderStyle);
                    //header.Cell().Element(CellStyle).Text("Produced Inventory").Style(tableHeaderStyle);
                    //header.Cell().Element(CellStyle).Text("Manual Adjustments").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Inventory Totals").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Ordered Non Allocated").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Allocated Not Filled").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Available Inventory").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black).AlignCenter();
                    }
                });
                
                var totalProduced = 0;
                var totalAdjustments = 0;
                var totalInventoryTotals = 0;
                var totalNonAllocated = 0;
                var totalNotFilled = 0;
                var totalAvailable = 0;
                foreach (var item in _items.OrderBy(i => i.SortIndex))
                {
                    var allOrderItems = _orders.SelectMany(o => o.Items.Where(i => i.ProductID == item.ProductID));
                    var produced = item.Quantity;
                    var adjustments = item.ManualAdjustments;
                    var inventoryTotal = produced + adjustments;
                    var nonAllocated = (int)_orders.Where(o => o.Allocated != true && o.OrderStatus <= Models.Enums.OrderStatus.Filling).SelectMany(o => o.Items.Where(o => o.ProductID == item.ProductID)).Sum(i => i.Quantity);
                    var notFilled = _allocatedNotReceived.ContainsKey(item.ProductID) ? _allocatedNotReceived[item.ProductID] : 0;
                    var availibleInventory = inventoryTotal - notFilled - nonAllocated;

                    totalProduced += produced;
                    totalAdjustments += adjustments;
                    totalInventoryTotals += inventoryTotal;
                    totalNonAllocated += nonAllocated;
                    totalNotFilled += notFilled;
                    totalAvailable += availibleInventory;

                    table.Cell().Element(CellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.Product.ProductName}").Style(tableTextStyle);
                    //table.Cell().Element(CellStyle).Text($"{produced}").Style(tableTextStyle);
                    //table.Cell().Element(CellStyle).Text($"{adjustments}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{inventoryTotal}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{nonAllocated}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{notFilled}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{availibleInventory}").Style(tableTextStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().PaddingVertical(2);
                    }

                }

                table.Cell().Element(FooterCellStyle).Text($"Totals:").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle);
                //table.Cell().Element(FooterCellStyle).Text($"{totalProduced}").Style(tableHeaderStyle);
                //table.Cell().Element(FooterCellStyle).Text($"{totalAdjustments}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{totalInventoryTotals}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{totalNonAllocated}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{totalNotFilled}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{totalAvailable}").Style(tableHeaderStyle);


                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.PaddingVertical(2).AlignCenter();
                }

            });
        });
    }
}
