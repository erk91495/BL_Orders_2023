using System.Reflection;
using System.Reflection.PortableExecutable;
using BlOrders2023.Models;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace BlOrders2023.Reporting.ReportClasses;

[System.ComponentModel.DisplayName("Shipping List")]
public class ShippingList : IReport
{
    private readonly CompanyInfo _companyInfo;
    private readonly Order _order;

    private readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
    private readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
    private readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
    private readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(8.5f);
    private readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(8.5f).SemiBold();
    private readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(8.5f);

    public ShippingList(CompanyInfo companyInfo, Order order)
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
            ///Header Row Contains invoice # Company Name and logo for first page
            headerCol.Item().ShowOnce().AlignCenter().PaddingBottom(10).Row(row =>
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
                    col.Item().AlignCenter().Text("Shipping List").Style(titleStyle);
                });

                row.RelativeItem(2).AlignMiddle().AlignRight().Column(column =>
                {
                    //Invoice Number
                    column.Item().Text($"Invoice #{_order.OrderID}").SemiBold().Style(subTitleStyle);
                    if (!_order.PO_Number.IsNullOrEmpty())
                    {
                        column.Item().Text($"PO: {_order.PO_Number}").Style(subTitleStyle);
                    }

                    column.Item().Text($"{_order.Customer.CustomerName}").Style(subTitleStyle);
                    column.Item().Text($"{_order.Customer.Address}").Style(normalTextStyle);
                    column.Item().Text($"{_order.Customer.CityStateZip()}").Style(normalTextStyle);

                });

            });


            headerCol.Item().SkipOnce().AlignCenter().PaddingBottom(10).Row(row =>
            {

                row.RelativeItem(3).AlignLeft().Column(col =>
                {
                    col.Item().AlignLeft().Text("Shipping List").Style(titleStyle);
                });

                row.RelativeItem(2).AlignMiddle().AlignRight().Column(column =>
                {
                    //Invoice Number
                    column.Item().Text($"Invoice #{_order.OrderID}").SemiBold().Style(subTitleStyle);
                    if (!_order.PO_Number.IsNullOrEmpty())
                    {
                        column.Item().Text($"PO: {_order.PO_Number}").Style(subTitleStyle);
                    }

                    column.Item().Text($"{_order.Customer.CustomerName}").Style(subTitleStyle);

                });

            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        var numberOfColumns = 4;
        container.Column(column => {

            var idList = _order.ShippingItems.Select(i => i.ProductID).Distinct().OrderBy(i => i);
            foreach (var id in idList)
            {
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(column =>
                    {
                        for(var i = 0; i < numberOfColumns; i++)
                        {
                            column.RelativeColumn(2);
                            column.RelativeColumn(2);
                            column.RelativeColumn(2);
                        }
                    });

                    table.Header(header =>
                    {
                        for (var i = 0; i < numberOfColumns; i++)
                        {
                            header.Cell().Element(CellStyle).AlignRight().Text("Product ID").Style(tableHeaderStyle);
                            header.Cell().Element(CellStyle).AlignCenter().Text("Quantity").Style(tableHeaderStyle);
                            header.Cell().Element(CellStyle).AlignLeft().Text("Net Wt").Style(tableHeaderStyle);
                        }

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).BorderBottom(1).BorderColor(Colors.Black).AlignMiddle();
                        }
                    });

                    foreach (var item in _order.ShippingItems.Where(i => i.ProductID == id))
                    {

                        table.Cell().BorderLeft(1).BorderColor(Colors.Grey.Lighten2).Element(CellStyle).AlignRight().Text($"{item.ProductID}").Style(tableTextStyle);
                        table.Cell().Element(CellStyle).AlignCenter().Text($"{item.QuanRcvd}").Style(tableTextStyle);
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Element(CellStyle).AlignLeft().Text($"{item.PickWeight:F2}").Style(tableTextStyle);
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2).AlignMiddle();
                        }
                    }

                    var remainder = _order.ShippingItems.Where(i => i.ProductID == id).Count() % numberOfColumns;
                    if(remainder > 0)
                    {
                        for (var i = 0; i < numberOfColumns - remainder; i++)
                        {
                            table.Cell().Element(CellStyle);
                            table.Cell().Element(CellStyle);
                            table.Cell().Element(CellStyle);
                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                            }
                        }
                    }


                    for (var i = 0; i < numberOfColumns - 1; i++)
                    {
                        table.Cell().Element(FooterCellStyle);
                        table.Cell().Element(FooterCellStyle);
                        table.Cell().Element(FooterCellStyle);
                    }

                    table.Cell().Element(FooterCellStyle).PaddingRight(1).AlignRight().Text("Total: ").Style(tableHeaderStyle);
                    table.Cell().Element(FooterCellStyle).AlignCenter().Text($"{_order.ShippingItems.Where(i => i.ProductID == id).Sum(i => i.QuanRcvd)}").Style(tableHeaderStyle);
                    table.Cell().Element(FooterCellStyle).AlignCenter().Text($"{_order.ShippingItems.Where(i => i.ProductID == id).Sum(i => i.PickWeight):F2}").Style(tableHeaderStyle);
                    static IContainer FooterCellStyle(IContainer container)
                    {
                        return container.BorderTop(1).BorderColor(Colors.Black).PaddingVertical(2);
                    }
                });
            }
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);

                });

                table.Cell().Element(CellStyle).PaddingRight(1).AlignRight().Text("Order Totals: ").Style(tableHeaderStyle);
                table.Cell().Element(CellStyle).Text($"{_order.ShippingItems.Sum(i => i.QuanRcvd)}").Style(tableHeaderStyle);
                table.Cell().Element(CellStyle).Text($"{_order.ShippingItems.Sum(i => i.PickWeight)}").Style(tableHeaderStyle);

                static IContainer CellStyle(IContainer container)
                {
                    return container.PaddingTop(3).BorderTop(2f).BorderColor(Colors.Black).PaddingVertical(2);
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
