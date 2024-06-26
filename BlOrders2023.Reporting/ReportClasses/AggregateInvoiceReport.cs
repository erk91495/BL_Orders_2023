﻿using System.Reflection;
using BlOrders2023.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Aggregate Invoice")]
public class AggregateInvoiceReport : ReportBase
{

    private readonly IEnumerable<Order> _orders;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;

    public AggregateInvoiceReport(CompanyInfo companyInfo, IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate) : base(companyInfo)
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
                    col.Item().AlignCenter().Text(_companyInfo.LongCompanyName).Style(titleStyle);
                    col.Item().AlignCenter().Text($"{_companyInfo.StreetAddress}, {_companyInfo.City}, {_companyInfo.State} {_companyInfo.ShortZipCode}").Style(subTitleStyle);
                    col.Item().AlignCenter().Text($"Phone: {_companyInfo.PhoneString()}          Fax: {_companyInfo.FaxString()}");
                    col.Item().AlignCenter().Text(_companyInfo.Website);
                });

                row.RelativeItem(1).AlignRight().Column(column =>
                {
                    column.Item().Text($"From: {_startDate.ToString("M/d/yy")}").Style(subTitleStyle);
                    column.Item().Text($" To:  {_endDate.ToString("M/d/yy")}").Style(subTitleStyle);
                });

            });

        });
    }

    protected override void ComposeContent(IContainer container)
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
                        column.RelativeColumn(2);
                    });

                    invoicesTable.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Invoice Number").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Invoice Total").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Balance Due").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Pickup Date").Style(tableHeaderStyle);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach(Order order in _orders)
                    {
                        invoicesTable.Cell().Element(CellStyle).Text($"{order.OrderID}").Style(tableTextStyle);
                        invoicesTable.Cell().Element(CellStyle).Text($"{order.InvoiceTotal:C}").Style(tableTextStyle);
                        invoicesTable.Cell().Element(CellStyle).Text($"{order.BalanceDue:C}").Style(tableTextStyle);
                        invoicesTable.Cell().Element(CellStyle).Text($"{order.PickupDate.ToString("M/d/yy")}").Style(tableTextStyle);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                        }
                    }

                    invoicesTable.Cell().Element(FooterCellStyle).Text("Total").Style(tableHeaderStyle);
                    invoicesTable.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.InvoiceTotal):C}").Style(tableHeaderStyle);
                    invoicesTable.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.BalanceDue):C}").Style(tableHeaderStyle);
                    invoicesTable.Cell().Element(FooterCellStyle).Text("").Style(tableHeaderStyle);

                    static IContainer FooterCellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderColor(Colors.Black);
                    }

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
                var groupedItems = aggregateItems.GroupBy(i => new { i.ProductID, i.ActualCustPrice }).OrderBy(a => a.Key.ProductID);
                
                // step 3
                foreach (var group in groupedItems)
                {
                    table.Cell().Element(CellStyle).Text($"{group.First().ProductID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{group.Sum(i => i.Quantity)}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{group.Sum(i => i.QuantityReceived)}").Style(tableTextStyle);

                    table.Cell().Element(CellStyle).Text(group.First().Product.ProductName).Style(tableTextStyle);

                    table.Cell().Element(CellStyle).Text($"{group.Sum(i => i.PickWeight):N2}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{group.First().ActualCustPrice:C}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{group.Sum(i => i.GetTotalPrice):C}").Style(tableTextStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }

                table.Cell().Element(FooterCellStyle).Text("Total: ").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{aggregateItems.Sum(i => i.Quantity)}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{aggregateItems.Sum(i => i.QuantityReceived)}").Style(tableHeaderStyle);
                table.Cell();
                table.Cell().Element(FooterCellStyle).Text($"{aggregateItems.Sum(i => i.PickWeight):N2}").Style(tableHeaderStyle);
                table.Cell();
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{_orders.Sum(o => o.InvoiceTotal):C}").Style(tableHeaderStyle);

                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderColor(Colors.Black);
                }

            });
        });
    }

}
