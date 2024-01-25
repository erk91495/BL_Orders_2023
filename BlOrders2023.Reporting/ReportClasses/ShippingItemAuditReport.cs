using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models.Enums;
using BlOrders2023.Models;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using System.Reflection;
using System.Diagnostics;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Audit Trail Report")]
public class ShippingItemAuditReport :IReport
{

    #region Fields
    private readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
    private readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).FontColor(Colors.Black);
    private readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
    private readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(8);
    private readonly TextStyle itemTableTextStyle = TextStyle.Default.FontSize(9);
    private readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9);
    private readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(9);
    private readonly CompanyInfo _companyInfo;
    private readonly IEnumerable<ShippingItem> _shippingItems;
    private readonly DateTime _reportDate = DateTime.Now;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;
    #endregion Fields

    #region Constructors
    public ShippingItemAuditReport(CompanyInfo companyInfo, IEnumerable<ShippingItem> items)
    {
        _companyInfo = companyInfo;
        _shippingItems = items;
    }
    #endregion Constructors

    #region Methods
    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(20);
            page.MarginTop(0.75f, Unit.Inch);
            page.Size(PageSizes.Letter.Landscape());

            page.Header().Height(100).Background(Colors.Grey.Lighten1);
            page.Header().Element(ComposeHeader);

            page.Content().Background(Colors.Grey.Lighten3);
            page.Content().Element(ComposeContent);

            page.Footer().Height(20).Background(Colors.Grey.Lighten1);
            page.Footer().Element(ComposeFooter);

        });
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    private void ComposeHeader(IContainer container)
    {
        container.Column(headerCol =>
        {
            ///Header Row Contains invoice # Company Name and logo
            headerCol.Item().AlignCenter().PaddingBottom(10).Row(row =>
            {

                //Logo
                var res = Assembly.GetExecutingAssembly().GetManifestResourceStream("BlOrders2023.Reporting.Assets.Images.BLLogo.bmp");
                row.RelativeItem(1).AlignLeft().AlignMiddle().Height(50).Image(res).FitHeight();

                row.RelativeItem(3).AlignCenter().Column(col =>
                {
                    col.Item().AlignCenter().Text($"{_companyInfo.ShortCompanyName} Audit Trail Report").Style(titleStyle);
                });

                row.RelativeItem(2).AlignMiddle().AlignRight().Column(column =>
                {
                });

            });

        });
    }

    private void ComposeContent(IContainer container)
    {
        var groupedItems = _shippingItems.GroupBy(i => i.Order.Customer);
        container.Column(column => {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(1);
                    column.RelativeColumn(11);
                    column.RelativeColumn(12);
                    column.RelativeColumn(4);
                    column.RelativeColumn(4);
                });

                //table.Header(header =>
                //{
                //    static IContainer CellStyle(IContainer container)
                //    {
                //        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                //    }
                //});

                foreach (var group in groupedItems)
                {
                    table.Cell().ColumnSpan(7).Element(SubHeaderCellStyle).Text($"{group.Key}").Style(tableHeaderStyle);
                    var groupedByID = group.ToList().GroupBy(i => i.OrderID);
                    foreach (var order in groupedByID )
                    {
                        table.Cell().ColumnSpan(1);
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"{order.Key}").Style(tableHeaderStyle);
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"ID").Style(tableTextStyle);
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"Product Name").Style(tableTextStyle);
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"Scanline").Style(tableTextStyle);
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"Pack Date").Style(tableTextStyle);
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"Scan Date").Style(tableTextStyle);
                        foreach (var item in order)
                        {
                            //Acts like a tab
                            table.Cell().ColumnSpan(2);
                            table.Cell().Element(MainTableCellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                            table.Cell().Element(MainTableCellStyle).Text($"{item.Product.ProductName}").Style(tableTextStyle);
                            table.Cell().Element(MainTableCellStyle).Text($"{item.Scanline}").Style(tableTextStyle);
                            var packDateString = item.PackDate.HasValue ? item.PackDate.Value.ToShortDateString() : string.Empty;
                            table.Cell().Element(MainTableCellStyle).Text($"{packDateString}").Style(tableTextStyle);
                            var scanDateString = item.ScanDate.HasValue ? item.ScanDate.Value.ToShortDateString() : string.Empty;
                            table.Cell().Element(MainTableCellStyle).Text($"{scanDateString}").Style(tableTextStyle);
                        }

                    }
                    static IContainer MainTableCellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }

                static IContainer SubHeaderCellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(2);
                }

                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.BorderTop(1).BorderColor(Colors.Black).PaddingVertical(2);
                }
            });


        });

        //container.Column(column => {
        //    column.Item().Table(table =>
        //    {
        //        table.ColumnsDefinition(column =>
        //        {
        //            column.RelativeColumn(1);
        //            column.RelativeColumn(11);
        //            column.RelativeColumn(12);
        //            column.RelativeColumn(4);
        //            column.RelativeColumn(9);
        //            column.RelativeColumn(2);
        //            column.RelativeColumn(4);
        //        });

        //        table.Header(header =>
        //        {
        //            header.Cell().Element(CellStyle).Text("ID").Style(tableHeaderStyle);
        //            header.Cell().Element(CellStyle).Text("Product Name").Style(tableHeaderStyle);
        //            header.Cell().Element(CellStyle).Text("Scanline").Style(tableHeaderStyle);
        //            header.Cell().Element(CellStyle).Text("Packed On").Style(tableHeaderStyle);
        //            header.Cell().Element(CellStyle).Text("Sent To").Style(tableHeaderStyle);
        //            header.Cell().Element(CellStyle).Text("Order ID").Style(tableHeaderStyle);
        //            header.Cell().Element(CellStyle).Text("Scan Date").Style(tableHeaderStyle);


        //            static IContainer CellStyle(IContainer container)
        //            {
        //                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
        //            }
        //        });

        //        foreach (var item in _shippingItems)
        //        {
        //            table.Cell().Element(MainTableCellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
        //            table.Cell().Element(MainTableCellStyle).Text($"{item.Product.ProductName}").Style(tableTextStyle);
        //            table.Cell().Element(MainTableCellStyle).Text($"{item.Scanline}").Style(tableTextStyle);
        //            var packDateString = item.PackDate.HasValue ? item.PackDate.Value.ToShortDateString() : string.Empty;
        //            table.Cell().Element(MainTableCellStyle).Text($"{packDateString}").Style(tableTextStyle);
        //            table.Cell().Element(MainTableCellStyle).Text($"{item.Order.Customer.CustomerName}").Style(tableTextStyle);
        //            table.Cell().Element(MainTableCellStyle).Text($"{item.OrderID}").Style(tableTextStyle);
        //            var scanDateString = item.ScanDate.HasValue ? item.ScanDate.Value.ToShortDateString() : string.Empty;
        //            table.Cell().Element(MainTableCellStyle).Text($"{scanDateString}").Style(tableTextStyle);
        //            static IContainer MainTableCellStyle(IContainer container)
        //            {
        //                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
        //            }
        //        }

        //        static IContainer FooterCellStyle(IContainer container)
        //        {
        //            return container.BorderTop(1).BorderColor(Colors.Black).PaddingVertical(2);
        //        }
        //    });


        //});
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
                    time.Span($"{_reportDate.ToString():d}").Style(smallFooterStyle);
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
    #endregion Methods
}
