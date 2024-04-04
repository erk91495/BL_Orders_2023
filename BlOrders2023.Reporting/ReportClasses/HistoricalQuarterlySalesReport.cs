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
[System.ComponentModel.DisplayName("Historical Quarterly Sales Report")]
public class HistoricalQuarterlySalesReport : ReportBase
{
    private readonly IEnumerable<IEnumerable<ProductTotalsItem>> _orders;
    protected readonly new TextStyle tableHeaderStyle = TextStyle.Default.FontSize(8).SemiBold();
    protected readonly new TextStyle tableTextStyle = TextStyle.Default.FontSize(8);
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;

    public HistoricalQuarterlySalesReport(CompanyInfo companyInfo, IEnumerable<IEnumerable<ProductTotalsItem>> orders, DateTimeOffset startDate, DateTimeOffset endDate)
        : base(companyInfo)
    {
        _orders = orders;
        _startDate = startDate;
        _endDate = endDate;
    }

    public override void Compose(IDocumentContainer container) 
    {
        container.Page(page =>
        {
            page.Margin(20);
            page.Size(PageSizes.Letter.Landscape());

            page.Header().Height(100).Background(Colors.Grey.Lighten1);
            page.Header().Element(ComposeHeader);

            page.Content().Background(Colors.Grey.Lighten3);
            page.Content().Element(ComposeContent);

            page.Footer().Height(20).Background(Colors.Grey.Lighten1);
            page.Footer().Element(ComposeFooter);

        });
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
                    column.RelativeColumn(2);
                    column.RelativeColumn(13);

                    column.RelativeColumn(3);
                    column.RelativeColumn(3);
                    column.RelativeColumn(3);

                    column.RelativeColumn(4);
                    column.RelativeColumn(4);
                    column.RelativeColumn(4);

                    column.RelativeColumn(4);
                    column.RelativeColumn(4);
                    column.RelativeColumn(4);
                });

                table.Header(header =>
                {
                    header.Cell().ColumnSpan(2).Element(CellStyle);

                    header.Cell().ColumnSpan(3).Element(CellStyle).AlignCenter().Text($"{_startDate.Year - 2}").Style(tableHeaderStyle);

                    header.Cell().ColumnSpan(3).Element(CellStyle).AlignCenter().Text($"{_startDate.Year - 1}").Style(tableHeaderStyle);

                    header.Cell().ColumnSpan(3).Element(CellStyle).AlignCenter().Text($"{_startDate.Year}").Style(tableHeaderStyle);

                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).Element(CellStyle).Text("ID").Style(tableHeaderStyle);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).Element(CellStyle).Text("Product Name").Style(tableHeaderStyle);
                                 
                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).Element(CellStyle).AlignCenter().Text($"Sold").Style(tableHeaderStyle);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).Element(CellStyle).Text($"Ext Price").Style(tableHeaderStyle);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).Element(CellStyle).Text($"Net Wt").Style(tableHeaderStyle);

                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).Element(CellStyle).AlignCenter().Text($"Sold").Style(tableHeaderStyle);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).Element(CellStyle).Text($"Ext Price").Style(tableHeaderStyle);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).Element(CellStyle).Text($"Net Wt").Style(tableHeaderStyle);

                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).Element(CellStyle).AlignCenter().Text($"Sold").Style(tableHeaderStyle);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).Element(CellStyle).Text($"Ext Price").Style(tableHeaderStyle);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Black).Element(CellStyle).Text($"Net Wt").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold());
                    }
                });

                var orderItems = _orders.ElementAt(0);
                // -1 year
                var orderItemsOff1 = _orders.ElementAt(1);
                // -2 years
                var orderItemsOff2 = _orders.ElementAt(2);

                for(var i = 0; i < orderItems.Count(); i++)
                {
                    var item = orderItems.ElementAt(i);
                    var item1Off = orderItemsOff1.ElementAt(i);
                    var item2Off = orderItemsOff2.ElementAt(i);

                    float? yoyReceived;
                    float? yoyPrice;
                    float? yoyWeight;
                    table.Cell().Element(CellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                    table.Cell().BorderRight(1).BorderColor(Colors.Black).Element(CellStyle).Text($"{item.ProductName}").Style(tableTextStyle);

                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item2Off.QuantityReceived}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item2Off.ExtPrice:C}").Style(tableTextStyle);
                    table.Cell().BorderRight(1).BorderColor(Colors.Black).Element(CellStyle).Text($"{item2Off.NetWeight:N2}").Style(tableTextStyle);
                    try
                    {
                        yoyReceived = (((float)item1Off.QuantityReceived / (float)item2Off.QuantityReceived) - 1f) * 100f;
                    }
                    catch(DivideByZeroException)
                    {
                        yoyReceived = null;
                    }

                    try
                    {
                        yoyPrice = (((float)item1Off.ExtPrice / (float)item2Off.ExtPrice) - 1f) * 100f;
                    }
                    catch (DivideByZeroException)
                    {
                        yoyPrice = null;
                    }

                    try
                    {
                        yoyWeight = (((float)item1Off.NetWeight / (float)item2Off.NetWeight) - 1f) * 100f;
                    }
                    catch (DivideByZeroException)
                    {
                        yoyWeight = null;
                    }

                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item1Off.QuantityReceived} ({yoyReceived:N0})").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item1Off.ExtPrice:C} ({yoyPrice:N0})").Style(tableTextStyle);
                    table.Cell().BorderRight(1).BorderColor(Colors.Black).Element(CellStyle).Text($"{item1Off.NetWeight:N2} ({yoyWeight:N0})").Style(tableTextStyle);

                    try
                    {
                        yoyReceived = (((float)item.QuantityReceived / (float)item1Off.QuantityReceived) - 1f) * 100f;
                    }
                    catch (DivideByZeroException)
                    {
                        yoyReceived = null;
                    }

                    try
                    {
                        yoyPrice = (((float)item.ExtPrice / (float)item1Off.ExtPrice) - 1f) * 100f;
                    }
                    catch (DivideByZeroException)
                    {
                        yoyPrice = null;
                    }

                    try
                    {
                        yoyWeight = (((float)item.NetWeight / (float)item1Off.NetWeight) - 1f) * 100f;
                    }
                    catch (DivideByZeroException)
                    {
                        yoyWeight = null;
                    }

                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item.QuantityReceived} ({yoyReceived:N0})").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.ExtPrice:C} ({yoyPrice:N0})").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.NetWeight:N2} ({yoyWeight:N0})").Style(tableTextStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).Text($"Total: ").Style(tableHeaderStyle);
                
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).Text($"{orderItemsOff2.Sum(o => o.ExtPrice):C}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{orderItemsOff2.Sum(o => o.ExtPrice):C}").Style(tableHeaderStyle);

                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).Text($"{orderItemsOff1.Sum(o => o.ExtPrice):C}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{orderItemsOff1.Sum(o => o.ExtPrice):C}").Style(tableHeaderStyle);

                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).Text($"{orderItems.Sum(o => o.ExtPrice):C}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{orderItems.Sum(o => o.ExtPrice):C}").Style(tableHeaderStyle);

                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.PaddingVertical(2);
                }

            });
        });
    }
}
