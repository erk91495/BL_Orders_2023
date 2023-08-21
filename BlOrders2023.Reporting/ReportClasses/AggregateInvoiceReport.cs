using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Windows.Media.AppBroadcasting;
using Windows.System.RemoteSystems;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Aggregate Invoice")]
public class AggregateInvoiceReport : IReport
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
    public AggregateInvoiceReport(IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
    {
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

                row.RelativeItem(1).AlignRight().Column(column =>
                {
                    column.Item().Text($"From: {_startDate.ToString("M/d/yy")}").Style(subTitleStyle);
                    column.Item().Text($" To:  {_endDate.ToString("M/d/yy")}").Style(subTitleStyle);
                });

            });

        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(mainColumn =>{
            mainColumn.Item().Row(row => {
                row.RelativeItem(2).Border(1).PaddingLeft(1).Table(invoicesTable =>
                {
                    invoicesTable.ColumnsDefinition(column =>
                    {
                        column.RelativeColumn(2);
                        column.RelativeColumn(2);
                        column.RelativeColumn(2);
                    });

                    invoicesTable.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Invoice Number").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Invoice Total").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Pickup Date").Style(tableHeaderStyle);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach(Order order in _orders)
                    {
                        invoicesTable.Cell().Element(CellStyle).Text($"{order.OrderID}").Style(tableTextStyle);
                        invoicesTable.Cell().Element(CellStyle).Text($"{order.GetInvoiceTotal():C}").Style(tableTextStyle);
                        invoicesTable.Cell().Element(CellStyle).Text($"{order.PickupDate.ToString("M/d/yy")}").Style(tableTextStyle);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                        }
                    }

                    invoicesTable.Footer(footer =>
                    {
                        footer.Cell().Element(CellStyle).Text("Total").Style(tableHeaderStyle);
                        footer.Cell().Element(CellStyle).Text($"{_orders.Sum(o => o.GetInvoiceTotal()):C}").Style(tableHeaderStyle);
                        footer.Cell().Element(CellStyle).Text("").Style(tableHeaderStyle);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderColor(Colors.Black);
                        }
                    });

                });
                row.ConstantItem(10);
                row.RelativeItem(2).Border(1).PaddingLeft(1).Table(customersTable =>
                {
                    customersTable.ColumnsDefinition(column =>
                    {
                        column.RelativeColumn(2);
                    });

                    customersTable.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Customer Name").Style(tableHeaderStyle);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var customerName in _orders.Select(o => o.Customer.CustomerName).Distinct())
                    {
                        customersTable.Cell().Element(CellStyle).Text($"{customerName}").Style(tableTextStyle);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                        }
                    }
                });
            });

            //Items
            mainColumn.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(8);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Product ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Ordered").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Received").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).Text("Description").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).Text("Net Weight").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Unit Price").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Total Price").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });
                var aggregateItems = _orders.SelectMany(o => o.Items);
                var groupedItems = aggregateItems.GroupBy(i => new { i.ProductID, i.ActualCustPrice });
                
                // step 3
                foreach (var group in groupedItems)
                {
                    table.Cell().Element(CellStyle).Text($"{group.First().ProductID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{group.Sum(i => i.Quantity)}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{group.Sum(i => i.QuantityReceived)}").Style(tableTextStyle);

                    table.Cell().Element(CellStyle).Text(group.First().Product.ProductName).Style(tableTextStyle);

                    table.Cell().Element(CellStyle).Text($"{group.Sum(i => i.PickWeight):N2}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{group.First().ActualCustPrice:C}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{group.Sum(i => i.GetTotalPrice()):C}").Style(tableTextStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }

                table.Footer(footer =>
                {
                    footer.Cell().Element(CellStyle).Text("Total: ").Style(tableHeaderStyle);
                    footer.Cell().Element(CellStyle).Text($"{aggregateItems.Sum(i => i.QuantityReceived)}").Style(tableHeaderStyle);
                    footer.Cell().Element(CellStyle).Text($"{aggregateItems.Sum(i => i.Quantity)}").Style(tableHeaderStyle);
                    footer.Cell();
                    footer.Cell().Element(CellStyle).Text($"{aggregateItems.Sum(i => i.PickWeight):N2}").Style(tableHeaderStyle);
                    footer.Cell();
                    footer.Cell().Element(CellStyle).AlignRight().Text($"{_orders.Sum(o => o.GetInvoiceTotal()):C}").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderColor(Colors.Black);
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
