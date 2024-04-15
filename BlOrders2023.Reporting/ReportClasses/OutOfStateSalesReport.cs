using System.Reflection;
using BlOrders2023.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Out Of State Sales")]
public class OutOfStateSalesReport : ReportBase
{
    private readonly IEnumerable<Order> _orders;
    private readonly DateTime _reportDate = DateTime.Now;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;
    public  OutOfStateSalesReport(CompanyInfo companyInfo, IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
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
            headerCol.Item().AlignCenter().PaddingBottom(10).Row(row =>
            {

                //Logo
                var res = Assembly.GetExecutingAssembly().GetManifestResourceStream("BlOrders2023.Reporting.Assets.Images.BLLogo.bmp");
                row.RelativeItem(1).AlignLeft().AlignMiddle().Height(75).Image(res).FitHeight();

                row.RelativeItem(3).AlignCenter().Column(col =>
                {
                    col.Item().AlignCenter().Text($"{_companyInfo.ShortCompanyName} Turkeys, Inc.").Style(titleStyle);
                    col.Item().AlignCenter().Text("Out Of State Sales Report").Style(titleStyle);
                });

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
        var groupedOrders = _orders.GroupBy(o => o.CustID);

        container.Column(column =>
        {
            //Items
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(2);
                    column.RelativeColumn(5);
                    column.RelativeColumn(6);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Order ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Customer Name").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Address").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Date").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Invoice Total").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                var orderItems = _orders.SelectMany(o => o.Items);
                var productsIDs = orderItems.Select(i => i.ProductID).Distinct().OrderBy(i => i);
                foreach (var group in groupedOrders)
                {
                    foreach (var order in group)
                    {
                        table.Cell().Element(CellStyle).Text($"{order.OrderID}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.Customer.BillingCustomerName}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.Customer.BillingAddress}{order.Customer.BillingCityStateZip()}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.PickupDate.ToShortDateString()}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.InvoiceTotal:N2}").Style(tableTextStyle);
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                        }
                    }
                    table.Cell().Element(SubHeaderCellStyle);
                    table.Cell().Element(SubHeaderCellStyle);
                    table.Cell().Element(SubHeaderCellStyle);
                    table.Cell().Element(SubHeaderCellStyle).AlignTop().Text($"Total:").Style(tableTextStyle).SemiBold();
                    table.Cell().Element(SubHeaderCellStyle).AlignTop().Text($"{group.ToList().Sum(o => o.InvoiceTotal):N2}").Style(tableTextStyle).SemiBold();
                    static IContainer SubHeaderCellStyle(IContainer container)
                    {
                        return container.PaddingVertical(4);
                    }

                }

                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).Text($"Report Total: ").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.InvoiceTotal):C}").Style(tableHeaderStyle);

                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.PaddingVertical(2);
                }

            });
        });
    }

}
