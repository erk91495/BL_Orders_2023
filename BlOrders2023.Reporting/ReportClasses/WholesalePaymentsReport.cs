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
public class WholesalePaymentsReport : IReport
{
    private readonly CompanyInfo _companyInfo;
    private readonly IEnumerable<Payment> _payments;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;

    public static readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
    public static readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
    public static readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
    public static readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(9);
    public static readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9).SemiBold();
    public static readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(9);

    public WholesalePaymentsReport(CompanyInfo companyInfo, IEnumerable<Payment> payments, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        _companyInfo = companyInfo;
        _payments = payments;
        _startDate = startDate;
        _endDate = endDate;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(20);
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
            row.RelativeItem(2).AlignCenter().Text($"{_companyInfo.ShortCompanyName} Wholesale Payments").Style(titleStyle);
            row.RelativeItem(1).AlignRight().Column(column =>
            {
                column.Item().Text($"From: {_startDate:M/d/yy}").Style(subTitleStyle);
                column.Item().Text($"To: {_endDate:M/d/yy}").Style(subTitleStyle);
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
                    table.Cell().Element(CellStyle).Text($"{payment.CheckNumber ?? String.Empty}").Style(tableTextStyle);
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

    private void ComposeFooter(IContainer container)
    {
        container.AlignBottom().Column(column =>
        {
            column.Item().AlignBottom().AlignRight().Row(footer =>
            {
                footer.RelativeItem().AlignLeft().Text(time =>
                {
                    time.Span("Printed: ").Style(subTitleStyle).Style(smallFooterStyle);
                    time.Span($"{DateTime.Now.ToString():d}").Style(smallFooterStyle);
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
