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
[System.ComponentModel.DisplayName("Frozen Orders Report")]
public class FrozenOrdersReport : ReportBase
{
    private readonly new TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9);

    private readonly IEnumerable<Order> _orders;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;
    
    public FrozenOrdersReport(CompanyInfo companyInfo, IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
        :base(companyInfo)
    {
        _orders = orders;
        _startDate = startDate;
        _endDate = endDate;
    }

    protected override void ComposeHeader(IContainer container)
    {
        container.Column(headerCol =>
        {
            ///Header Row Contains invoice # Company Name and logo
            headerCol.Item().AlignCenter().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(10).Row(row =>
            {

                //Logo

                row.RelativeItem(1);

                row.RelativeItem(5).AlignCenter().Text($"{_companyInfo.ShortCompanyName} Frozen Orders Report").Style(titleStyle);

                row.RelativeItem(1).AlignRight().Column(column =>
                {
                    column.Item().Text($"From: {_startDate.ToString("M/d/yy")}").Style(subTitleStyle);
                    column.Item().Text($"To: {_endDate.ToString("M/d/yy")}").Style(subTitleStyle);
                });

            });
        });
    }

    protected override void ComposeContent(IContainer container)
    {

        container.Column(column => {
            foreach (var order in _orders)
            {
                column.Item().Row(row =>
                {
                    row.AutoItem().Text($"{order.OrderID}    ").Style(subTitleStyle);
                    row.RelativeItem(3).Text($"{order.Customer.CustomerName}").Style(subTitleStyle);
                    row.RelativeItem(3).AlignRight().Text($"{order.Shipping} {order.PickupDate.ToString("M/d/yy")}").Style(subTitleStyle);
                });

                column.Item().Shrink().PaddingLeft(60).PaddingRight(200).Table(table =>
                {
                    table.ColumnsDefinition(column =>
                    {
                        column.RelativeColumn(1);
                        column.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Product ID").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Quantity Ordered").Style(tableHeaderStyle);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                        }
                    });

                    foreach (var item in order.Items)
                    {

                        table.Cell().Element(CellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{item.Quantity}").Style(tableTextStyle);
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                        }
                    }

                    table.Cell().Element(FooterCellStyle).PaddingRight(1).AlignRight().Text("Total: ").Style(tableTextStyle);
                    table.Cell().Element(FooterCellStyle).Text($"{order.ShippingItems.Sum(i => i.QuanRcvd)}").Style(tableTextStyle);
                    //table.Cell().Element(FooterCellStyle).Text($"{_order.ShippingItems.Where(i => i.ProductID == id).Sum(i => i.PickWeight):F2}").Style(tableTextStyle);
                    static IContainer FooterCellStyle(IContainer container)
                    {
                        return container.BorderTop(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(2);
                    }
                });
            }

            column.Item().AlignRight().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);

                });

                table.Cell().Element(CellStyle).PaddingRight(1).AlignRight().Text("Report Totals: ").Style(tableHeaderStyle);
                table.Cell().Element(CellStyle).Text($"{_orders.SelectMany(o => o.Items).Sum(i => i.Quantity)}").Style(tableHeaderStyle);
                //table.Cell().Element(MainTableCellStyle).Text($"{_order.ShippingItems.Sum(i => i.PickWeight)}").Style(tableHeaderStyle);

                static IContainer CellStyle(IContainer container)
                {
                    return container.PaddingTop(3).BorderTop(2f).BorderColor(Colors.Black).PaddingVertical(2);
                }
            });

        });
    }
}
