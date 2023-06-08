using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.UI.Xaml.Controls;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;


namespace BlOrders2023.Reporting.ReportClasses
{
    [System.ComponentModel.DisplayName("Wholesale Order Pickup Recap")]
    internal class WholesaleOrderPickupRecap : IReport
    {
        private IEnumerable<Order> _orders;

        public static readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
        public static readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
        public static readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
        public static readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(9);
        public static readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9).SemiBold();
        public static readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(9);
        WholesaleOrderPickupRecap(IEnumerable<Order> orders)
        {
            _orders = orders;
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
            container.Row(row =>
            {
                row.RelativeItem().AlignCenter().Text("B & L Wholesale Order Pickup Recap").Style(titleStyle);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(column => {
                // step 3
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

                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Customer Name").Style(tableHeaderStyle);

                        header.Cell().Element(CellStyle).Text("Customer ID").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Phone").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Order ID").Style(tableHeaderStyle);

                        header.Cell().Element(CellStyle).Text("Pickup Date").Style(tableHeaderStyle);

                        header.Cell().Element(CellStyle).Text("Pickup Time").Style(tableHeaderStyle);


                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    // step 3
                    foreach (var order in _orders)
                    {
                        table.Cell().Element(CellStyle).Text($"{order.Customer.CustomerName}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.Customer.CustID}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.Customer.Phone}").Style(tableTextStyle);

                        table.Cell().Element(CellStyle).Text($"{order.OrderID}").Style(tableTextStyle);

                        table.Cell().Element(CellStyle).Text($"{order.PickupDate}").Style(tableTextStyle);

                        table.Cell().Element(CellStyle).Text($"{order.PickupTime}").Style(tableTextStyle);

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
