
using System.Reflection;
using System.Xml;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Allocation Summary Report")]
public class AllocationSummaryReport : ReportBase
{
    #region Fields
    private readonly new TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9);

    private readonly IEnumerable<Order> _orders;
    private readonly AllocatorMode _allocationMode;
    private readonly DateTime _allocationTime = DateTime.Now;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;
    #endregion Fields

    #region Constructors
    public AllocationSummaryReport(CompanyInfo companyInfo, IEnumerable<Order> Orders, AllocatorMode mode, DateTime AllocationTime) : base(companyInfo)
    {
        _orders = Orders;
        _allocationMode = mode;
        _allocationTime = AllocationTime;
    }
    #endregion Constructors

    #region Methods
    public override void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(20);
            page.MarginLeft(0.75f, Unit.Inch);
            page.Size(PageSizes.Letter);

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
                    col.Item().AlignCenter().Text($"{_companyInfo.ShortCompanyName} Allocation Summary Report").Style(titleStyle);
                });

                row.RelativeItem(2).AlignMiddle().AlignRight().Column(column =>
                {
                    column.Item().Text($"Allocation Time: {_allocationTime.ToShortDateString()} {_allocationTime.ToShortTimeString()} ");
                    column.Item().Text($"Allocation Type: {_allocationMode}");
                });

            });

        });
    }

    protected override void ComposeContent(IContainer container)
    {

        container.Column(column => {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(2);
                    column.RelativeColumn(8);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);

                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Order ID").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).Text("Customer").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Total Ordered").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Total Allocated").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var order in _orders)
                {
                    var quanOrdered = order.TotalOrderedAllocated;
                    table.Cell().Element(CellStyle).Text($"{order.OrderID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{order.Customer.CustomerName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{quanOrdered}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{order.TotalAllocated}").Style(tableTextStyle);
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).PaddingRight(1).AlignRight().Text("Total: ").Style(tableTextStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.TotalOrderedAllocated)}").Style(tableTextStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(i => i.TotalAllocated)}").Style(tableTextStyle);
                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.BorderTop(1).BorderColor(Colors.Black).PaddingVertical(2);
                }
            });


        });
    }
    #endregion Methods
}
