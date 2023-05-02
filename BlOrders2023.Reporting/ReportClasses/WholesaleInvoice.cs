using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using BlOrders2023.Models;
using System.Reflection;
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlOrders2023.Reporting.ReportClasses
{
    internal class WholesaleInvoice : IDocument
    {

        private Order _order { get; set; }

        private readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
        private readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
        private readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
        private readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(9);
        private readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9).SemiBold();

        public WholesaleInvoice (Order order)
        {
            _order = order;
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
                        page.Footer().AlignBottom().AlignRight().Text(x =>
                        {
                            x.Span("Printed on: ").Style(subTitleStyle);
                            x.Span($"{DateTime.Now.ToString():d}").FontSize(12);

                            x.Span("page");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();


                        });

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
                    row.RelativeItem(1).AlignLeft().AlignMiddle().Height(75).Image(res, ImageScaling.FitHeight);

                    row.RelativeItem(3).AlignCenter().Column(col =>
                    {
                        col.Item().AlignCenter().Text("Bowman & Landes Turkeys, Inc.").Style(titleStyle);
                        col.Item().AlignCenter().Text("6490 East Ross Road, New Carlisle, Ohio 45344").Style(subTitleStyle);
                        col.Item().AlignCenter().Text("Phone: 937-845-9466          Fax: 937-845-9998");
                    });

                    row.RelativeItem(1).AlignMiddle().AlignRight().Column(column =>
                    {   //Invoice Number
                        column.Item().Text($"Invoice #{_order.OrderID}").SemiBold().FontSize(15);
                        if (!_order.PO_Number.IsNullOrEmpty())
                        {
                            column.Item().Text($"PO: {_order.PO_Number}").Style(subTitleStyle);
                        }
                    });

                });

             });
        }

        private void ComposeContent(IContainer container)
        {
            
            container.Column(column => {

                //Bill To Shup To
                column.Item().MinimalBox().AlignBottom().PaddingBottom(5).Row(row =>
                {
                    //Padding 
                    row.RelativeItem(1);

                    //Ship To:
                    row.RelativeItem(9).Border(1).ExtendHorizontal().AlignCenter().Column(column =>
                    {
                        column.Item().Background(Colors.Grey.Lighten3).Border(1).PaddingLeft(3).Text("Ship To:");
                        column.Item().PaddingLeft(3).Text($"{_order.Customer.CustomerName}").Style(subTitleStyle);
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().PaddingLeft(3).Column(column =>
                            {
                                column.Item().Text($"{_order.Customer.Address}").Style(normalTextStyle);
                                column.Item().Text($"{_order.Customer.CityStateZip()}").Style(normalTextStyle);
                                column.Item().Text($"{_order.Customer.Buyer}").Style(normalTextStyle);
                                column.Item().Text($"{_order.Customer.Email.Trim()}").Style(normalTextStyle);
                                column.Item().Text($"{_order.Customer.Phone}").Style(normalTextStyle);

                            });
                        });
                    });

                    //Padding 
                    row.RelativeItem(4);

                    //Bill To:
                    row.RelativeItem(9).Border(1).ExtendHorizontal().AlignCenter().Column(column =>
                    {

                        column.Item().Background(Colors.Grey.Lighten3).Border(1).PaddingLeft(3).Text("Bill To:");
                        column.Item().PaddingLeft(3).Text($"{_order.Customer.CustomerName}").Style(subTitleStyle);
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().PaddingLeft(3).Column(column =>
                            {
                                column.Item().Text($"{_order.Customer.BillingAddress}").Style(normalTextStyle);
                                column.Item().Text($"{_order.Customer.BillingCityStateZip()}").Style(normalTextStyle);
                            });
                        });
                    });

                    //Padding 
                    row.RelativeItem(1);

                });

                //Pickup Date & PO#
                column.Item().Column(column =>
                {
                    if (_order.Shipping == Models.Enums.ShippingType.Pickup)
                    {
                        column.Item().Text(text =>
                        {
                            text.Span("Pickup: ").Style(subTitleStyle);
                            text.Span($"{_order.PickupDate.ToShortDateString():d} {_order.PickupTime.ToShortTimeString():d}").FontSize(12);
                        });
                    }
                    else
                    {
                        column.Item().Text(text =>
                        {
                            text.Span("Deliver By: ").Style(subTitleStyle);
                            text.Span($"{_order.PickupDate.ToShortDateString():d}").FontSize(12);
                        });
                    }
                });


                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(column =>
                    {
                        column.RelativeColumn(1);
                        column.RelativeColumn(2);
                        column.RelativeColumn(4);
                        column.RelativeColumn(1);
                        column.RelativeColumn(1);
                        column.RelativeColumn(1);
                        column.RelativeColumn(1);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("#").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Product ID").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Description").Style(tableHeaderStyle);

                        header.Cell().Element(CellStyle).Text("Weight").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Ordered").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Received").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).AlignRight().Text("Unit Price").Style(tableHeaderStyle);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    // step 3
                    foreach (var item in _order.Items)
                    {
                        table.Cell().Element(CellStyle).Text($"{_order.Items.IndexOf(item) + 1}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text(item.Product.ProductName).Style(tableTextStyle);

                        table.Cell().Element(CellStyle).Text($"{item.PickWeight}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{item.Quantity}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{item.QuanRcvd}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.ActualCustPrice.ToString("C")}").Style(tableTextStyle);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                        }
                    }

                });
            });
        }

    }
}
