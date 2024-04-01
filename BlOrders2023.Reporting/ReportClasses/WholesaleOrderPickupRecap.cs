using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;


namespace BlOrders2023.Reporting.ReportClasses;

[System.ComponentModel.DisplayName("Wholesale Order Pickup Recap")]
public class WholesaleOrderPickupRecap : ReportBase
{
    private readonly IEnumerable<Order> _orders;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;

    public WholesaleOrderPickupRecap(CompanyInfo companyInfo, IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
        :base(companyInfo)
    {
        _orders = orders;
        _startDate = startDate;
        _endDate = endDate;
    }

    protected override void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem(2).AlignCenter().Text($"{_companyInfo.ShortCompanyName} Wholesale Order Pickup Recap").Style(titleStyle);
            row.RelativeItem(1).AlignRight().Column( column =>
            {
                column.Item().Text($"From: {_startDate:M/d/yy}").Style(subTitleStyle);
                column.Item().Text($"To: {_endDate:M/d/yy}").Style(subTitleStyle);
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
                    column.RelativeColumn(8);
                    column.RelativeColumn(2);
                    column.RelativeColumn(4);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);

                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).AlignLeft().Text("Customer Name").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).AlignCenter().Text("Customer ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Phone").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Order ID").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).AlignCenter().Text("Pickup Date").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).AlignCenter().Text("Pickup Time").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Cases Ordered").Style(tableHeaderStyle);


                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var order in _orders)
                {
                    table.Cell().Element(CellStyle).AlignLeft().Text($"{order.Customer.CustomerName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{order.Customer.CustID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{order.Customer.PhoneString()}").Style(tableTextStyle);

                    table.Cell().Element(CellStyle).AlignCenter().Text($"{order.OrderID}").Style(tableTextStyle);

                    table.Cell().Element(CellStyle).AlignCenter().Text($"{order.PickupDate:M/d/yyyy}").Style(tableTextStyle);
                    if(order.Shipping == Models.Enums.ShippingType.Delivery)
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text($"Delivery").Style(tableTextStyle).Italic();
                    }
                    else
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text($"{order.PickupTime:t}").Style(tableTextStyle);
                    }
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{order.TotalGiven}").Style(tableTextStyle);
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }

                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).Text($"Total:").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignCenter().Text($"{_orders.Sum(order => order.TotalGiven)}").Style(tableHeaderStyle);
                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.BorderTop(1).BorderColor(Colors.Black).PaddingVertical(2);
                }
            });
        });
    }
}
