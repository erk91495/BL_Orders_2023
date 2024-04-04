using System;
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
[System.ComponentModel.DisplayName("Quarterly Sales Report")]
public class QuarterlySalesReport : ReportBase
{
    private readonly IEnumerable<Order> _orders;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;

    public QuarterlySalesReport(CompanyInfo companyInfo, IEnumerable<Order> orders, DateTimeOffset startDate, DateTimeOffset endDate)
        :base(companyInfo)
    {
        _orders = orders;
        _startDate  = startDate;
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
                    col.Item().AlignCenter().Text("Quarterly Sales Report").Style(titleStyle);
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
        container.Column(column =>
        {
            //Items
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(1);
                    column.RelativeColumn(4);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Product ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Product Name").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Quantity Received").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Extended Price").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Net Wt").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                var orderItems = _orders.SelectMany(o => o.Items);
                var productsIDs = orderItems.Select(i => i.ProductID).Distinct().OrderBy(i => i);
                foreach (var id in productsIDs)
                {
                    var matchingItems = orderItems.Where(item => item.ProductID == id);
                    table.Cell().Element(CellStyle).Text($"{id}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{matchingItems.First().Product.ProductName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{matchingItems.Sum(i => i.QuantityReceived)}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{matchingItems.Sum(i => i.GetTotalPrice):C}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{matchingItems.Sum(i => i.PickWeight):N2}").Style(tableTextStyle);
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }

                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).Text($"Total: ").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.InvoiceTotal):C}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{orderItems.Sum(o => o.PickWeight):N2}").Style(tableHeaderStyle);

                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.PaddingVertical(2);
                }

            });
        });
    }
}
