﻿using System;
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
[System.ComponentModel.DisplayName("Outstanding Balances")]
public class OutstandingBalancesReport : ReportBase
{
    private readonly IEnumerable<Order> _orders;
    private readonly DateTime _reportDate = DateTime.Now;

    public OutstandingBalancesReport(CompanyInfo companyInfo, IEnumerable<Order> orders)
        :base(companyInfo)
    {
        _orders = orders;
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
                row.RelativeItem(1).AlignLeft().AlignMiddle().Height(75).Image(res).FitHeight();

                row.RelativeItem(3).AlignCenter().Column(col =>
                {
                    col.Item().AlignCenter().Text(_companyInfo.LongCompanyName).Style(titleStyle);
                    col.Item().AlignCenter().Text("Outstanding Balances").Style(titleStyle);
                });

            });
            headerCol.Item().Text($"As of: {_reportDate.ToString():d}").Style(subTitleStyle);
        });
    }

    protected override void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            //Items
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(4);
                    column.RelativeColumn(1);

                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Pickup Date").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Order ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Customer Name").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Balance Due").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var order in _orders)
                {
                    table.Cell().Element(CellStyle).Text($"{order.PickupDate.ToString("M/d/yy")}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{order.OrderID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{order.Customer.CustomerName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{order.BalanceDue:C}").Style(tableTextStyle);
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }

                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).Text($"Total: ").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.BalanceDue):C}").Style(tableHeaderStyle);

                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.PaddingVertical(2);
                }

            });
        });
    }
}
