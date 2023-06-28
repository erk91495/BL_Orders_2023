using BlOrders2023.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Reporting.ReportClasses
{
    [System.ComponentModel.DisplayName("Unpaid Invoices")]
    public class UnpaidInvoicesReport : IReport
    {

        private readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
        private readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
        private readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
        private readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(9);
        private readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9).SemiBold();
        private readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(9);

        private IEnumerable<Order> _orders;
        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;    

        public UnpaidInvoicesReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            _orders = orders;
            _startDate = startDate;
            _endDate = endDate;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(10);

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
                        col.Item().AlignCenter().Text("Bowman & Landes Turkeys, Inc.").Style(titleStyle);
                        col.Item().AlignCenter().Text("6490 Ross Road, New Carlisle, Ohio 45344").Style(subTitleStyle);
                        col.Item().AlignCenter().Text("Phone: 937-845-9466          Fax: 937-845-9998");
                        col.Item().AlignCenter().Text("www.bowmanlandes.com");
                    });

                });
                headerCol.Item().Row(row => row.RelativeItem(1).Text($"Outstanding Balance Report for: {_orders.First().Customer.CustomerName}") );

            });
        }

        private void ComposeContent(IContainer container)
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

                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Invoice Number").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Pickup Date").Style(tableHeaderStyle);
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
                        table.Cell().Element(CellStyle).Text($"{order.PickupDate.ToString("M/d/yy")}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.GetInvoiceTotal()}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.GetTotalPayments()}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.GetBalanceDue()}").Style(tableTextStyle);
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                        }
                    }
                    table.Footer(footer =>
                    {
                        footer.Cell().Element(CellStyle).Text("Totals:");
                        footer.Cell().Element(CellStyle).Text("");                        
                        footer.Cell().Element(CellStyle).Text($"{_orders.Sum(o => o.GetInvoiceTotal())}").Style(tableHeaderStyle);
                        footer.Cell().Element(CellStyle).Text($"{_orders.Sum(o => o.GetBalanceDue())}");
                        footer.Cell().Element(CellStyle).Text("");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                        }
                    });

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
}
