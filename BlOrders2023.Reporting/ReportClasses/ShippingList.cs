using BlOrders2023.Models;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BlOrders2023.Reporting.ReportClasses
{
    [System.ComponentModel.DisplayName("Shipping List")]
    public class ShippingList : IReport
    {

        private readonly Order _order;

        private readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
        private readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
        private readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
        private readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(8.5f);
        private readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(8.5f).SemiBold();
        private readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(8.5f);

        public ShippingList(Order order)
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
                    row.RelativeItem(1).AlignLeft().AlignMiddle().Height(75).Image(res).FitHeight();

                    row.RelativeItem(3).AlignCenter().Column(col =>
                    {
                        col.Item().AlignCenter().Text("Bowman & Landes Turkeys, Inc.").Style(titleStyle);
                        col.Item().AlignCenter().Text("6490 East Ross Road, New Carlisle, Ohio 45344").Style(subTitleStyle);
                        col.Item().AlignCenter().Text("Phone: 937-845-9466          Fax: 937-845-9998");
                        col.Item().AlignCenter().Text("www.bowmanlandes.com");
                        col.Item().AlignCenter().Text("Shipping List").Style(subTitleStyle);
                    });

                    row.RelativeItem(1).AlignMiddle().AlignRight().Column(column =>
                    {
                        //Invoice Number
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

                var idList = _order.ShippingItems.Select(i => i.ProductID).Distinct().OrderBy(i => i);
                foreach (var id in idList)
                {
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(column =>
                        {
                            column.RelativeColumn(2);
                            column.RelativeColumn(2);
                            column.RelativeColumn(2);

                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Product ID").Style(tableHeaderStyle);

                            header.Cell().Element(CellStyle).Text("Quantity").Style(tableHeaderStyle);
                            header.Cell().Element(CellStyle).Text("Net Wt").Style(tableHeaderStyle);

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var item in _order.ShippingItems.Where(i => i.ProductID == id))
                        {

                            table.Cell().Element(CellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                            table.Cell().Element(CellStyle).Text($"{item.QuanRcvd}").Style(tableTextStyle);
                            table.Cell().Element(CellStyle).Text($"{item.PickWeight:F2}").Style(tableTextStyle);
                        }
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                        }

                        table.Footer(footer =>
                        {
                            footer.Cell().Element(CellStyle).AlignRight().Text("Total:").Style(tableHeaderStyle);

                            footer.Cell().Element(CellStyle).Text($"{_order.ShippingItems.Where(i => i.ProductID == id).Sum(i => i.QuanRcvd)}").Style(tableHeaderStyle);
                            footer.Cell().Element(CellStyle).Text($"{_order.ShippingItems.Where(i => i.ProductID == id).Sum(i => i.PickWeight):F2}").Style(tableHeaderStyle);
                        });
                    });
                }
                
            
            });
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
                        time.Span($"{DateTime.Now.ToString():d}").Style(smallFooterStyle);
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
}
