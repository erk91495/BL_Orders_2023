using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Pick List")]
public class PickList : IReport
{
    private readonly CompanyInfo _companyInfo;
    private readonly Order _order;
    private readonly DateTime _ReportDate = DateTime.Now;

    private readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).Bold().FontColor(Colors.Black);
    private readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
    private readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
    private readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(10f);
    private readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(10f).SemiBold();
    private readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(8f);

    public PickList(CompanyInfo companyInfo, Order order)
    {
        _companyInfo = companyInfo;
        _order = order;
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

                row.RelativeItem(3).AlignMiddle().AlignCenter().Column(col =>
                {
                    col.Item().Text($"{_companyInfo.ShortCompanyName} Wholesale Order Pick List").Style(titleStyle);
                    col.Item().PaddingTop(5).Row(row =>
                    {
                        //Invoice Number
                        row.RelativeItem().AlignCenter().Text($"Order ID: {_order.OrderID}").FontSize(14);
                        row.RelativeItem().ShowIf(!_order.PO_Number.IsNullOrEmpty()).AlignCenter().Text($"PO: {_order.PO_Number ?? ""}").FontSize(14);

                    });

                });

                row.RelativeItem(2).AlignMiddle().AlignRight().Column(column =>
                {
                    FontManager.RegisterFontFromEmbeddedResource("BlOrders2023.Reporting.Assets.Fonts.IDAutomationHC39M_Free.ttf");
                    //barcode
                    column.Item().Text($"*{_order.OrderID}*").FontFamily("IDAutomationHC39M").FontSize(18);
                    //Invoice Number
                    //column.Item().Text($"Order #{_order.OrderID}").SemiBold().FontSize(14);
                    //if (!_order.PO_Number.IsNullOrEmpty())
                    //{
                    //    column.Item().Text($"PO: {_order.PO_Number}").SemiBold().FontSize(13);
                    //}
                });

            });

            headerCol.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Black);

        });
    }

    private void ComposeContent(IContainer container)
    {

        container.Column(column => {

            column.Item().Row(row => 
            {
                row.RelativeItem().Column(custColumn =>
                {
                    custColumn.Item().Text($"{_order.Customer.CustomerName}").Style(subTitleStyle);
                    custColumn.Item().Text($"{_order.Customer.Address}").Style(normalTextStyle);
                    custColumn.Item().Text($"{_order.Customer.CityStateZip()}").Style(normalTextStyle);
                    custColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text(Formatters.PhoneFormatter(_order.Customer.Phone)).Style(normalTextStyle);
                        if( !_order.Customer.Phone_2.IsNullOrEmpty()) 
                        {
                            row.RelativeItem().Text(Formatters.PhoneFormatter(_order.Customer.Phone_2)).Style(normalTextStyle);
                        }
                    });

                    if(!_order.Customer.Fax.IsNullOrEmpty())
                    { 
                        custColumn.Item().Row(row =>
                        {
                            row.AutoItem().MinimalBox().Text("Fax: ").SemiBold().Style(normalTextStyle);
                            row.RelativeItem().MinimalBox().AlignLeft().Text(Formatters.PhoneFormatter(_order.Customer.Fax)).Style(normalTextStyle);
                        });
                    }


                    custColumn.Item().Row(row =>
                    {
                        row.AutoItem().MinimalBox().Text("Customer Class: ").SemiBold().Style(normalTextStyle);
                        row.RelativeItem().MinimalBox().AlignLeft().Text($"{_order.Customer.CustomerClass.Class}").Style(normalTextStyle);
                    });

                    custColumn.Item().Row(row =>
                    {
                        row.AutoItem().MinimalBox().Text("Buyer: ").SemiBold().Style(normalTextStyle);
                        row.RelativeItem().MinimalBox().AlignLeft().Text($"{_order.Customer.Buyer}").Style(normalTextStyle);
                    });

                });

                row.RelativeItem(1.25f).Column(pickupColumn =>
                {
                    pickupColumn.Item().Table(table => 
                    {
                        table.ColumnsDefinition(column =>
                        {
                            column.RelativeColumn(1);
                            column.RelativeColumn(1.5f);
                            column.RelativeColumn(1.5f);
                            column.RelativeColumn(1);
                            column.RelativeColumn(1);
                            column.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("Order Date");

                            header.Cell().Element(HeaderCellStyle).Text($"{_order.Shipping} Date");
                            header.Cell().Element(HeaderCellStyle).Text($"{_order.Shipping} Time");
                            header.Cell().Element(HeaderCellStyle).Text("Taken By");
                            header.Cell().Element(HeaderCellStyle).Text("Shipping");
                            header.Cell().Element(HeaderCellStyle).Text("");

                            static IContainer HeaderCellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold().FontSize(8)).PaddingVertical(2).AlignCenter();
                            }
                        });

                        table.Cell().Element(CellStyle).Text($"{_order.OrderDate:d}");
                        table.Cell().Element(CellStyle).Text($"{_order.PickupDate:d}");
                        table.Cell().Element(CellStyle).Text($"{_order.PickupTime:t}");
                        table.Cell().Element(CellStyle).Text($"{_order.TakenBy}");
                        table.Cell().Element(CellStyle).Text($"{_order.Shipping}");
                        var frozen = (_order.Frozen ?? false) ? "Frozen" : "Fresh";
                        table.Cell().Element(CellStyle).Text($"{ frozen}").SemiBold();
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.FontSize(8)).AlignCenter();
                        }


                    });
                });
            });
            column.Item().PaddingVertical(5).LineHorizontal(.5f).LineColor(Colors.Black);
            column.Item().Table(itemsTable =>
            {
                itemsTable.ColumnsDefinition(def =>
                {
                    def.RelativeColumn(2);
                    def.RelativeColumn(8);
                    def.RelativeColumn(4);
                    if(_order.Allocated == true)
                    {
                        def.RelativeColumn(4);
                    }
                });

                itemsTable.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Product ID").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).Text("Name").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Ordered").Style(tableHeaderStyle);
                    if (_order.Allocated == true)
                    {
                        header.Cell().Element(CellStyle).Text("Allocated").Style(tableHeaderStyle);
                    }

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in _order.Items.OrderBy(i => i.ProductID))
                {

                    itemsTable.Cell().Element(CellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                    itemsTable.Cell().Element(CellStyle).Text($"{item.Product.ProductName}").Style(tableTextStyle);
                    itemsTable.Cell().Element(CellStyle).Text($"{item.Quantity}").Style(tableTextStyle);
                    if (_order.Allocated == true)
                    {
                        itemsTable.Cell().Element(CellStyle).Text($"{item.QuanAllocated}").Style(tableTextStyle);
                    }
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }
                itemsTable.Cell().Element(FooterCellStyle).Text($"").Style(tableTextStyle);
                itemsTable.Cell().Element(FooterCellStyle).AlignRight().PaddingRight(2).Text($"Totals:").Style(tableTextStyle);
                itemsTable.Cell().Element(FooterCellStyle).Text($"{_order.Items.Sum(i => i.Quantity)}").Style(tableTextStyle);
                if (_order.Allocated == true)
                {
                    itemsTable.Cell().Element(FooterCellStyle).Text($"{_order.Items.Sum(i => i.QuanAllocated)}").Style(tableTextStyle);
                }
                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).BorderTop(1).BorderColor(Colors.Black).PaddingVertical(2);
                }
            });


        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignBottom().Column(column =>
        {
            column.Item().PaddingBottom(5).Column(memoCol =>
            {
                memoCol.Item().Text("Memo:").Style(tableHeaderStyle);
                memoCol.Item().Border(1).PaddingLeft(4).PaddingRight(4).MinHeight(48).ExtendHorizontal().Text($"{_order.Memo}").FontSize(11);
            });
            column.Item().AlignBottom().AlignRight().Row(footer =>
            {
                footer.RelativeItem().AlignLeft().Text(time =>
                {
                    time.Span("Printed: ").Style(subTitleStyle).Style(smallFooterStyle);
                    time.Span($"{_ReportDate.ToString():d}").Style(smallFooterStyle);
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
