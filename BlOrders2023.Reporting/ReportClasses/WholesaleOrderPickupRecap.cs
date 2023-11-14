using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;


namespace BlOrders2023.Reporting.ReportClasses
{
    [System.ComponentModel.DisplayName("Wholesale Order Pickup Recap")]
    public class WholesaleOrderPickupRecap : IReport
    {
        private readonly CompanyInfo _companyInfo;
        private IEnumerable<Order> _orders;
        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;

        public static readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
        public static readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
        public static readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
        public static readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(9);
        public static readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9).SemiBold();
        public static readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(9);

        public WholesaleOrderPickupRecap(CompanyInfo companyInfo, IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            _companyInfo = companyInfo;
            _orders = orders;
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
                row.RelativeItem(2).AlignCenter().Text($"{_companyInfo.ShortCompanyName} Wholesale Order Pickup Recap").Style(titleStyle);
                row.RelativeItem(1).AlignRight().Column( column =>
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
                        column.RelativeColumn(8);
                        column.RelativeColumn(2);
                        column.RelativeColumn(2);
                        column.RelativeColumn(2);
                        column.RelativeColumn(2);
                        column.RelativeColumn(2);
                        column.RelativeColumn(2);

                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Customer Name").Style(tableHeaderStyle);

                        header.Cell().Element(CellStyle).Text("Customer ID").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Phone").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Order ID").Style(tableHeaderStyle);

                        header.Cell().Element(CellStyle).Text("Pickup Date").Style(tableHeaderStyle);

                        header.Cell().Element(CellStyle).Text("Pickup Time").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Cases Ordered").Style(tableHeaderStyle);


                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var order in _orders)
                    {
                        table.Cell().Element(CellStyle).Text($"{order.Customer.CustomerName}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.Customer.CustID}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.Customer.Phone}").Style(tableTextStyle);

                        table.Cell().Element(CellStyle).Text($"{order.OrderID}").Style(tableTextStyle);

                        table.Cell().Element(CellStyle).Text($"{order.PickupDate.ToString("M/d/yyyy")}").Style(tableTextStyle);
                        if(order.Shipping == Models.Enums.ShippingType.Delivery)
                        {
                            table.Cell().Element(CellStyle).Text($"Delivery").Style(tableTextStyle).Italic();
                        }
                        else
                        {
                            table.Cell().Element(CellStyle).Text($"{order.PickupTime.ToString("t")}").Style(tableTextStyle);
                        }
                        table.Cell().Element(CellStyle).Text($"{order.GetTotalGiven()}").Style(tableTextStyle);
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
