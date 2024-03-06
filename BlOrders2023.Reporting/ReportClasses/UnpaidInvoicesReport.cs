using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

namespace BlOrders2023.Reporting.ReportClasses
{
    [System.ComponentModel.DisplayName("Unpaid Invoices")]
    public class UnpaidInvoicesReport : IReport
    {

        private readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
        private readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
        private readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
        private readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(9);
        private readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(9).SemiBold();
        private readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(9);
        private readonly CompanyInfo _companyInfo;
        private IEnumerable<Order> _orders;
        private DateTime _reportDate = DateTime.Now;

        public UnpaidInvoicesReport(CompanyInfo companyInfo, IEnumerable<Order> orders)
        {
            _companyInfo = companyInfo;
            _orders = orders;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Size(PageSizes.Letter); page.Margin(10);

                page.Header().Height(100).Background(Colors.Grey.Lighten1);
                page.Header().Element(ComposeHeader);

                page.Content().Background(Colors.Grey.Lighten3);
                page.Content().Element(ComposeContent);

                page.Footer().Height(20).Background(Colors.Grey.Lighten1);
                page.Footer().Element(ComposeFooter);

            });
        }

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
                        col.Item().AlignCenter().Text(_companyInfo.LongCompanyName).Style(titleStyle);
                        col.Item().AlignCenter().Text($"{_companyInfo.StreetAddress}, {_companyInfo.City}, {_companyInfo.State} {_companyInfo.ShortZipCode}").Style(subTitleStyle);
                        col.Item().AlignCenter().Text($"Phone: {_companyInfo.PhoneString()}          Fax: {_companyInfo.FaxString()}");
                        col.Item().AlignCenter().Text(_companyInfo.Website);
                    });

                });
                headerCol.Item().Column(column => 
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Outstanding Balance Report for:");
                        row.RelativeItem().Text($"As of: {_reportDate.ToString():d}");
                    });
                    if(!_orders.IsNullOrEmpty())
                    {
                        column.Item().PaddingLeft(3).Text($"{_orders.First().Customer.CustomerName}").Style(subTitleStyle);
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().PaddingLeft(3).Column(column =>
                            {
                                column.Item().Text($"{_orders.First().Customer.Address}").Style(normalTextStyle);
                                column.Item().Text($"{_orders.First().Customer.CityStateZip()}").Style(normalTextStyle);
                            });
                        });
                    }
                    else
                    {
                        column.Item().Text($"No unpaid invoices").Style(normalTextStyle);
                    }
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                //Items
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(column =>
                    {
                        column.RelativeColumn(2);
                        column.RelativeColumn(2);
                        column.RelativeColumn(2);
                        column.RelativeColumn(2);
                        column.RelativeColumn(2);

                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Invoice Number").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Pickup/Delivery Date").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Invoice Total").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Payments").Style(tableHeaderStyle);
                        header.Cell().Element(CellStyle).Text("Balance Due").Style(tableHeaderStyle);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var order in _orders)
                    {
                        table.Cell().Element(CellStyle).Text($"{order.OrderID}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.PickupDate.ToString("M/d/yy")}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.InvoiceTotal:C}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.TotalPayments:C}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).Text($"{order.BalanceDue:C}").Style(tableTextStyle);
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                        }
                    }
                    table.Cell().Element(FooterCellStyle).Text("Totals:").Style(tableHeaderStyle);
                    table.Cell().Element(FooterCellStyle).Text("").Style(tableHeaderStyle);                        
                    table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.InvoiceTotal):C}").Style(tableHeaderStyle);
                    table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.TotalPayments):C}").Style(tableHeaderStyle);
                    table.Cell().Element(FooterCellStyle).Text($"{_orders.Sum(o => o.BalanceDue):C}").Style(tableHeaderStyle);

                    static IContainer FooterCellStyle(IContainer container)
                    {
                        return container.PaddingVertical(2);
                    }

                });
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
    }
}
