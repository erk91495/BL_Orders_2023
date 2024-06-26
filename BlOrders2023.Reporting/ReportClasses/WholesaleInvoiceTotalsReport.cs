﻿using System.Reflection;
using System.Reflection.PortableExecutable;
using BlOrders2023.Models;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Wholesale Invoice Totals")]
public class WholesaleInvoiceTotalsReport : ReportBase
{
    public static readonly TextStyle tableSubHeaderStyle = TextStyle.Default.FontSize(7).SemiBold();
    public static readonly TextStyle tableSubTextStyle = TextStyle.Default.FontSize(7);

    private readonly WholesaleCustomer _customer;
    private readonly IEnumerable<Order> _orders;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;

    public WholesaleInvoiceTotalsReport(CompanyInfo company, WholesaleCustomer customer, IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
        :base(company)
    {
        _customer = customer;
        _orders = orders;
        _startDate = startDate;
        _endDate = endDate;
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
                    col.Item().AlignCenter().Text($"{_companyInfo.StreetAddress}, {_companyInfo.City}, {_companyInfo.State} {_companyInfo.ShortZipCode}").Style(subTitleStyle);
                    col.Item().AlignCenter().Text($"Phone: {_companyInfo.PhoneString()}          Fax: {_companyInfo.FaxString()}");
                    col.Item().AlignCenter().Text(_companyInfo.Website);
                });

            });
            headerCol.Item().Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Invoice Report for:");
                    row.RelativeItem().Text($"From: {_startDate:d} To: {_endDate:d}");
                });
                column.Item().PaddingLeft(3).Text($"{_customer.CustomerName}").Style(subTitleStyle);
                column.Item().Row(row =>
                {
                    row.RelativeItem().PaddingLeft(3).Column(column =>
                    {
                        column.Item().Text($"{_customer.Address}").Style(normalTextStyle);
                        column.Item().Text($"{_customer.CityStateZip()}").Style(normalTextStyle);
                    });
                });
            });
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
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Invoice Number").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Order Status").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Pickup/Delivery Date").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Invoice Total").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Payments").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Balance Due").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var order in _orders)
                {
                    table.Cell().Element(CellStyle).Text($"{order.OrderID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{order.OrderStatus}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{order.PickupDate.ToString("M/d/yy")}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{order.InvoiceTotal:C}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{order.TotalPayments:C}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{order.BalanceDue:C}").Style(tableTextStyle);
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }

                    if (!order.Payments.IsNullOrEmpty())
                    {
                        table.Cell();
                        table.Cell().Element(SubHeaderCellStyle).Text("Payment ID").Style(tableSubHeaderStyle);
                        table.Cell().Element(SubHeaderCellStyle).Text("Payment Amount").Style(tableSubHeaderStyle);
                        table.Cell().Element(SubHeaderCellStyle).Text("Payment Method").Style(tableSubHeaderStyle);
                        table.Cell().Element(SubHeaderCellStyle).Text("").Style(tableSubHeaderStyle);
                        table.Cell();
                        static IContainer SubHeaderCellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Black);
                        }
                    }

                    foreach (var payment in order.Payments)
                    {
                        table.Cell().ColumnSpan(1).Element(SubCellStyle);
                        table.Cell().ColumnSpan(1).Element(SubCellStyle).Text($"{payment.PaymentID}").Style(tableSubTextStyle); ;
                        table.Cell().ColumnSpan(1).Element(SubCellStyle).Text($"{payment.PaymentAmount}").Style(tableSubTextStyle);
                        table.Cell().ColumnSpan(1).Element(SubCellStyle).Text($"{payment.PaymentMethod}").Style(tableSubTextStyle);
                        if(payment.PaymentMethodID == 2 || payment.PaymentMethodID == 4) // check
                        {
                            table.Cell().ColumnSpan(2).Element(SubCellStyle).Text($"{payment.CheckNumber}").Style(tableSubTextStyle);
                        }
                        else
                        {
                            table.Cell().ColumnSpan(2).Element(SubCellStyle);
                        }
                        
                        static IContainer SubCellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                        }
                    }
                }
                table.Cell().Element(FooterCellStyle).Text("Totals:").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text("").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text("").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.InvoiceTotal):C}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.TotalPayments):C}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.BalanceDue):C}").Style(tableHeaderStyle);

                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.PaddingVertical(2);
                }

            });
        });
    }
}
