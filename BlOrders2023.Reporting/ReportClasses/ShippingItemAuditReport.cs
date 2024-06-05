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
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Audit Trail Report")]
public class ShippingItemAuditReport : ReportBase
{

    #region Fields
    private readonly new TextStyle tableTextStyle = TextStyle.Default.FontSize(8);
    private readonly new TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9);

    private readonly IEnumerable<ShippingItem> _shippingItems;
    private readonly DateTime? _startDate = null;
    private readonly DateTime? _endDate = null;
    private readonly ShippingItem _itemToMatch;
    private readonly IList<string> _fieldsToMatch;
    #endregion Fields

    #region Constructors
    public ShippingItemAuditReport(CompanyInfo companyInfo, IEnumerable<ShippingItem> items, ShippingItem item, IList<string> fieldsToMatch, DateTime? startDate, DateTime? endDate)
        :base(companyInfo)
    {
        _shippingItems = items;
        _itemToMatch = item;
        _fieldsToMatch = fieldsToMatch;
        _startDate = startDate;
        _endDate = endDate;
    }
    #endregion Constructors

    #region Methods
    public override void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(20);
            page.MarginTop(0.75f, Unit.Inch);
            page.Size(PageSizes.Letter.Landscape());

            page.Header().Height(75).Background(Colors.Grey.Lighten1);
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
                row.RelativeItem(2).AlignLeft().AlignMiddle().Height(50).Image(res).FitHeight();

                row.RelativeItem(3).AlignCenter().Column(col =>
                {
                    col.Item().AlignCenter().Text($"{_companyInfo.ShortCompanyName} Audit Trail Report").Style(titleStyle);
                });

                row.RelativeItem(5).AlignMiddle().AlignRight().Column(column =>
                {
                    column.Item().Text($"Match: {_itemToMatch.ProductID} : {_itemToMatch.Scanline}");
                    if(!_fieldsToMatch.IsNullOrEmpty())
                    {
                        var fieldsToMatchString ="on fields: ";
                        foreach (var item in _fieldsToMatch)
                        {
                            fieldsToMatchString += item;
                            fieldsToMatchString += " ";
                        }
                        column.Item().Text(fieldsToMatchString);

                    }
                    if(_startDate != null && _endDate != null)
                    {
                        column.Item().Text($"From: {_startDate.Value.ToString("M/d/yy")}").Style(subTitleStyle);
                        column.Item().Text($"To: {_endDate.Value.ToString("M/d/yy")}").Style(subTitleStyle);
                    }
                });

            });

        });
    }

    protected override void ComposeContent(IContainer container)
    {
        var groupedItems = _shippingItems.GroupBy(i => i.Order?.Customer);
        container.Column(column => {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(1.5f);
                    column.RelativeColumn(1.5f);
                    column.RelativeColumn(1);
                    column.RelativeColumn(11);
                    column.RelativeColumn(12);
                    column.RelativeColumn(4);
                    column.RelativeColumn(5);
                    column.RelativeColumn(3);
                    column.RelativeColumn(3);
                });

                //table.Header(header =>
                //{
                //    static IContainer CategoryCellStyle(IContainer container)
                //    {
                //        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                //    }
                //});

                foreach (var group in groupedItems)
                {
                    table.Cell().ColumnSpan(9).Element(SubHeaderCellStyle).Text($"{group.Key}").Style(tableHeaderStyle);
                    var groupedByID = group.ToList().GroupBy(i => i.OrderID);
                    foreach (var order in groupedByID )
                    {
                        table.Cell().ColumnSpan(1);
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"{order.Key}").Style(tableHeaderStyle);
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"ID").Style(tableTextStyle);
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"Product Name").Style(tableTextStyle);
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"Scanline").Style(tableTextStyle);
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"Pack Wt.").Style(tableTextStyle).AlignRight();
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"LotCode").Style(tableTextStyle).AlignCenter();
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"Pack Date").Style(tableTextStyle);
                        table.Cell().ColumnSpan(1).Element(SubHeaderCellStyle).Text($"Scan Date").Style(tableTextStyle);
                        float totalWt = 0;
                        foreach (var item in order)
                        {
                            //Acts like a tab
                            table.Cell().ColumnSpan(2);
                            table.Cell().Element(MainTableCellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                            table.Cell().Element(MainTableCellStyle).Text($"{item.Product.ProductName}").Style(tableTextStyle);
                            table.Cell().Element(MainTableCellStyle).Text($"{item.Scanline}").Style(tableTextStyle);
                            table.Cell().Element(MainTableCellStyle).Text($"{item.PickWeight:N2}").Style(tableTextStyle).AlignRight();
                            table.Cell().Element(MainTableCellStyle).Text($"{item.LotCode:N2}").Style(tableTextStyle).AlignCenter();
                            totalWt += item.PickWeight ?? 0;
                            var packDateString = item.PackDate.HasValue ? item.PackDate.Value.ToShortDateString() : string.Empty;
                            table.Cell().Element(MainTableCellStyle).Text($"{packDateString}").Style(tableTextStyle);
                            var scanDateString = item.ScanDate.HasValue ? item.ScanDate.Value.ToShortDateString() : string.Empty;
                            table.Cell().Element(MainTableCellStyle).Text($"{scanDateString}").Style(tableTextStyle);
                        }
                        table.Cell().ColumnSpan(5);
                        table.Cell().Element(SubTotalCellStyle).Text($"{totalWt:N2}").Style(tableTextStyle).SemiBold().AlignRight();
                        table.Cell().ColumnSpan(3);
                        static IContainer SubTotalCellStyle(IContainer container)
                        {
                            return container.PaddingVertical(2);
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
        //            header.Cell().Element(CategoryCellStyle).Text("ID").Style(tableHeaderStyle);
        //            header.Cell().Element(CategoryCellStyle).Text("Product Name").Style(tableHeaderStyle);
        //            header.Cell().Element(CategoryCellStyle).Text("Scanline").Style(tableHeaderStyle);
        //            header.Cell().Element(CategoryCellStyle).Text("Packed On").Style(tableHeaderStyle);
        //            header.Cell().Element(CategoryCellStyle).Text("Sent To").Style(tableHeaderStyle);
        //            header.Cell().Element(CategoryCellStyle).Text("Order ID").Style(tableHeaderStyle);
        //            header.Cell().Element(CategoryCellStyle).Text("Scan Date").Style(tableHeaderStyle);


        //            static IContainer CategoryCellStyle(IContainer container)
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
    #endregion Methods
}
