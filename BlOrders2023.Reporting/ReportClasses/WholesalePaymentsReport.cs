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

[System.ComponentModel.DisplayName("Wholesale Payments")]
public class WholesalePaymentsReport : ReportBase
{
    private readonly IEnumerable<Payment> _payments;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;

    public WholesalePaymentsReport(CompanyInfo companyInfo, IEnumerable<Payment> payments, DateTimeOffset startDate, DateTimeOffset endDate)
        : base(companyInfo)
    {
        _payments = payments;
        _startDate = startDate;
        _endDate = endDate;
    }

    protected override void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem(2).AlignCenter().Text($"{_companyInfo.ShortCompanyName} Wholesale Payments").Style(titleStyle);
            row.RelativeItem(1).AlignRight().Column(column =>
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
                    column.RelativeColumn(2);
                    column.RelativeColumn(6);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(1);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);

                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Payment Date").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).Text("Customer").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Order ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Payment ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Type").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Check Number").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Amount").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var payment in _payments)
                {
                    table.Cell().Element(CellStyle).Text($"{payment.PaymentDate?.ToString("M/d/yy")}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{payment.Customer.CustomerName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{payment.OrderId}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{payment.PaymentID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{payment.PaymentMethod.Method}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{payment.CheckNumber ?? string.Empty}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{payment.PaymentAmount:C}").Style(tableTextStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }
                //Add At bottom of grid
                table.Cell().Element(FooterCellStyle).Text("");
                table.Cell().Element(FooterCellStyle).Text("");
                table.Cell().Element(FooterCellStyle).Text("");
                table.Cell().Element(FooterCellStyle).Text("");
                table.Cell().Element(FooterCellStyle).Text("");
                table.Cell().Element(FooterCellStyle).AlignRight().Text("Total: ").Style(tableHeaderStyle);

                table.Cell().Element(FooterCellStyle).Text($"{_payments.Sum(p => p.PaymentAmount):C}").Style(tableHeaderStyle);

                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.BorderTop(1).BorderColor(Colors.Black).PaddingVertical(2);
                }


            });
        });
    }
}
