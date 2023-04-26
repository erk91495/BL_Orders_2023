using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using BlOrders2023.Models;
using System.Reflection;
using System.Net;

namespace BlOrders2023.Reporting.ReportClasses
{
    internal class WholesaleInvoice : IDocument
    {

        private Order _order { get; set; }

        public WholesaleInvoice (Order order)
        {
            _order = order;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
                    {
                        page.Margin(50);

                        page.Header().Height(100).Background(Colors.Grey.Lighten1);
                        page.Header().Element(ComposeHeader);

                        page.Content().Background(Colors.Grey.Lighten3);
                        page.Content().Element(ComposeContent);

                        page.Footer().Height(20).Background(Colors.Grey.Lighten1);
                        page.Footer().AlignBottom().AlignRight().Text(x =>
                        {
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
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
            var subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
            var normalTextStyle = TextStyle.Default.FontSize(10);
            container.Column(headerCol => 
            { 
                headerCol.Item().AlignCenter().Row(row =>
                {
                    row.RelativeItem(1).AlignLeft();
                    row.RelativeItem(3).AlignCenter().Column(col =>
                    {
                        col.Item().AlignCenter().Text("Bowman & Landes Turkeys, Inc.").Style(titleStyle);
                        col.Item().AlignCenter().Text("6490 East Ross Road, New Carlisle, Ohio 45344").Style(subTitleStyle);
                        col.Item().AlignCenter().Text("Phone: 937-845-9466          Fax: 937-845-9998");
                    });
                
                    //Logo
                    var res = Assembly.GetExecutingAssembly().GetManifestResourceStream("BlOrders2023.Reporting.Assets.Images.BLLogo.bmp");
                    row.RelativeItem(1).AlignRight().Height(75).Image(res, ImageScaling.FitHeight);
                });
                headerCol.Item().Row(row =>
                {


                    row.RelativeItem(5).ExtendHorizontal().AlignCenter().Column(column =>
                    {
                        column.Item().Text($"{_order.Customer.CustomerName}").Style(subTitleStyle);
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text($"{_order.Customer.City}, {_order.Customer.State}");
                                column.Item().Text($"{_order.Customer.Email.Trim()}");
                                column.Item().Text($"{_order.Customer.Address}");
                                column.Item().Text($"{_order.Customer.Phone}");
                            });
                        });
                    });

                    //Invoice Number
                    row.RelativeItem(2).AlignRight().Column(column =>
                    {
                        column.Item().Text($"Invoice #{_order.OrderID}").Style(subTitleStyle);

                        column.Item().Text(text =>
                        {
                            text.Span("Issue date: ").Style(subTitleStyle);
                            text.Span($"{DateTime.Now.ToShortDateString():d}").FontSize(12);
                        });

                        column.Item().Text(text =>
                        {
                            text.Span("Due date: ").Style(subTitleStyle);
                            text.Span($"{DateTime.Now.AddDays(40).ToShortDateString():d}").FontSize(12);
                        });
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(1);
                    column.RelativeColumn(2);
                    column.RelativeColumn(5);
                    column.RelativeColumn(1);
                    column.RelativeColumn(1);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#");
                    header.Cell().Element(CellStyle).Text("Product ID");
                    header.Cell().Element(CellStyle).Text("Product");

                    header.Cell().Element(CellStyle).AlignRight().Text("Weight");
                    header.Cell().Element(CellStyle).AlignRight().Text("Quantity");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                // step 3
                foreach (var item in _order.ShippingItems)
                {
                    table.Cell().Element(CellStyle).Text($"{_order.ShippingItems.IndexOf(item) + 1}");
                    table.Cell().Element(CellStyle).Text($"{item.ProductID}");
                    table.Cell().Element(CellStyle).Text(item.Product.ProductName);

                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.PickWeight}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.QuanRcvd}");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                }
            });
        }

    }
}
