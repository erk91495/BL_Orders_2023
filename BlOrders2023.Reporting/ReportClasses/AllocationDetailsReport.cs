﻿using BlOrders2023.Models.Enums;
using BlOrders2023.Models;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Allocation Details Report")]
public class AllocationDetailsReport : ReportBase
{
    #region Fields
    private readonly new TextStyle tableTextStyle = TextStyle.Default.FontSize(11).SemiBold();
    private readonly new TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9);
    private readonly TextStyle itemTableTextStyle = TextStyle.Default.FontSize(9);

    private readonly IEnumerable<Order> _orders;
    private readonly IEnumerable<AllocationGroup> _allocationGroups;
    private readonly AllocatorMode _allocationMode;
    
    private readonly DateTime _allocationTime = DateTime.Now;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;
    #endregion Fields

    #region Constructors
    public AllocationDetailsReport(CompanyInfo companyInfo, IEnumerable<Order> Orders, IEnumerable<AllocationGroup>groups, AllocatorMode mode, DateTime AllocationTime)
        :base(companyInfo)
    {
        _orders = Orders;
        _allocationTime = AllocationTime;
        _allocationGroups = groups;
        _allocationMode = mode;
    }
    #endregion Constructors

    #region Methods
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
        container.Column(headerCol =>
        {
            ///Header Row Contains invoice # Company Name and logo
            headerCol.Item().AlignCenter().PaddingBottom(10).Row(row =>
            {

                //Logo
                var res = Assembly.GetExecutingAssembly().GetManifestResourceStream("BlOrders2023.Reporting.Assets.Images.BLLogo.bmp");
                row.RelativeItem(1).AlignLeft().AlignMiddle().Height(50).Image(res).FitHeight();

                row.RelativeItem(3).AlignCenter().Column(col =>
                {
                    col.Item().AlignCenter().Text($"{_companyInfo.ShortCompanyName} Allocation Details Report").Style(titleStyle);
                });

                row.RelativeItem(2).AlignMiddle().AlignRight().Column(column =>
                {
                    column.Item().Text($"Allocation Time: {_allocationTime.ToShortDateString()} {_allocationTime.ToShortTimeString()} ");
                    column.Item().Text($"Allocation Type: {_allocationMode}");
                });

            });

        });
    }

    protected override void ComposeContent(IContainer container)
    {

        container.Column(column => {
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
                    header.Cell().Element(CellStyle).Text("Order ID").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).Text("Customer").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Total Ordered").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Total Allocated").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var order in _orders)
                {
                    table.Cell().Element(MainTableCellStyle).Text($"{order.OrderID}").Style(tableTextStyle);
                    table.Cell().Element(MainTableCellStyle).Text($"{order.Customer.CustomerName}").Style(tableTextStyle);
                    table.Cell().Element(MainTableCellStyle).Text($"{order.TotalOrderedAllocated}").Style(tableTextStyle);
                    table.Cell().Element(MainTableCellStyle).Text($"{order.TotalAllocated}").Style(tableTextStyle);
                    table.Cell().ColumnSpan(4).Inlined(inlined => 
                    {
                        inlined.Spacing(10);
                        inlined.BaselineTop();
                        foreach (var Ids in _allocationGroups.Select(e => e.ProductIDs))
                        {
                            var groupedItems = order.Items.Where(i => Ids.Contains(i.ProductID)).OrderBy(i => i.ProductID);
                            if(!groupedItems.IsNullOrEmpty()){
                                inlined.Item().Border(1).Table(itemTable =>
                                {
                                    itemTable.ColumnsDefinition(column =>
                                    {
                                        column.ConstantColumn(30);
                                        column.ConstantColumn(50);
                                        column.ConstantColumn(50);
                                    });

                                    itemTable.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("ID").Style(tableHeaderStyle);
                                        header.Cell().Element(CellStyle).Text("Ordered").Style(tableHeaderStyle);
                                        header.Cell().Element(CellStyle).Text("Allocated").Style(tableHeaderStyle);

                                        static IContainer CellStyle(IContainer container)
                                        {
                                            return container.DefaultTextStyle(x => x.SemiBold()).BorderBottom(1).BorderColor(Colors.Black).AlignCenter();
                                        }
                                    });
                                    foreach(var item in groupedItems)
                                    {
                                        itemTable.Cell().Element(ItemTableCellStyle).Text($"{item.ProductID}").Style(itemTableTextStyle);
                                        itemTable.Cell().Element(ItemTableCellStyle).Text($"{item.Quantity}").Style(itemTableTextStyle);
                                        itemTable.Cell().Element(ItemTableCellStyle).Text($"{item.QuanAllocated}").Style(itemTableTextStyle);
                                    }
                                    static IContainer ItemTableCellStyle(IContainer container)
                                    {
                                        return container.BorderLeft(1).BorderRight(1).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2).AlignCenter();
                                    }
                                });
                            }
                        }
                    });
                    static IContainer MainTableCellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).PaddingRight(1).AlignRight().Text("Total: ").Style(tableTextStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.TotalOrderedAllocated)}").Style(tableTextStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(i => i.TotalAllocated)}").Style(tableTextStyle);
                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.BorderTop(1).BorderColor(Colors.Black).PaddingVertical(2);
                }
            });
        });
    }
    #endregion Methods
}
