using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Pallet Loading Report")]
public class PalletLoadingReport :IReport
{
    #region Fields
    private readonly Order _order;
    private readonly Pallet _pallet;
    private readonly CompanyInfo _companyInfo;

    private readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).FontColor(Colors.Black);
    private readonly TextStyle poStyle = TextStyle.Default.FontSize(28).FontColor(Colors.Black);
    private readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(18).FontColor(Colors.Black);
    private readonly TextStyle giantTextSize = TextStyle.Default.FontSize(30).SemiBold().FontColor(Colors.Black);
    private readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(18);
    private readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(15);
    private readonly TextStyle largeTableTextStyle = TextStyle.Default.FontSize(20).SemiBold();
    private readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(8.5f).SemiBold();
    private readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(8.5f);
    #endregion Fields
    #region Properties
    #endregion Properties

    public PalletLoadingReport(CompanyInfo companyInfo, Order order, Pallet pallet)
    {
        _companyInfo = companyInfo;
        _order = order;
        _pallet = pallet;
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
            headerCol.Item().AlignCenter().PaddingBottom(10).Row(row =>
            {
                row.RelativeItem(1).Text($"Pallet: {_pallet.PalletIndex} of {_pallet.TotalPallets}").Style(titleStyle);
                row.RelativeItem(3).Column( col =>
                {
                    col.Item().AlignCenter().Text($"{_companyInfo.ShortCompanyName} Pallet Loading Report").Style(subTitleStyle);
                    col.Item().AlignCenter().Text($"{_order.Customer.AllocationType} Order").Style(subTitleStyle);
                });
                FontManager.RegisterFontFromEmbeddedResource("BlOrders2023.Reporting.Assets.Fonts.IDAutomationHC39M_Free.ttf");
                //barcode
                row.RelativeItem(1).AlignRight().Text($"*{_order.OrderID}*").FontFamily("IDAutomationHC39M").FontSize(18);
            });
            headerCol.Item().AlignRight().Column(dateCol =>
            {
                dateCol.Item().AlignRight().Text($"{_order.Shipping}: {_order.PickupDate.ToShortDateString()}").Style(giantTextSize);
                dateCol.Item().AlignRight().ShowIf(_order.Shipping == Models.Enums.ShippingType.Pickup).Text($"{_order.Shipping} Time: {_order.PickupTime.ToShortTimeString()}").Style(giantTextSize);
            });
            headerCol.Item().LineHorizontal(0.5f);
            headerCol.Item().PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Text($"{_order.Customer.CustomerName}").Style(giantTextSize);
                row.RelativeItem().AlignRight().Column(col => {
                    col.Item().Text($"Order ID: {_order.OrderID}").Style(normalTextStyle).SemiBold();
                    col.Item().BorderTop(12).BorderColor(Colors.Transparent).ShowIf(!_order.PO_Number.IsNullOrEmpty()).Text($"PO: {_order.PO_Number}").Style(poStyle).SemiBold();
                });
            });
        });
    }

    private void ComposeContent(IContainer container)
    {

        container.Column(column => {
            column.Item().Table(table => {
                table.ColumnsDefinition(column =>
                {
                    column.ConstantColumn(350);
                    column.ConstantColumn(55);
                    column.ConstantColumn(50);

                });

                table.Header(header =>
                {
                    header.Cell();
                    header.Cell();
                    header.Cell().Text("Quantity");
                });

                foreach (var product in _pallet.Items.Keys)
                {
                    table.Cell().Element(CellStyle).Text($"{product.ProductName}").Style(tableTextStyle);
                    table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Element(CellStyle).AlignCenter().Text($"{product.ProductID}").Style(largeTableTextStyle);
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{_pallet.Items.GetValueOrDefault(product)}").Style(largeTableTextStyle);
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.PaddingVertical(2);
                    }
                }
                table.Cell();
                table.Cell().Element(FooterCellStyle).PaddingRight(1).AlignCenter().Text("Total: ").Style(largeTableTextStyle);
                table.Cell().Element(FooterCellStyle).AlignCenter().Text($"{_pallet.Items.Values.Sum()}").Style(largeTableTextStyle);
                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.BorderTop(1).BorderColor(Colors.Black).PaddingVertical(2);
                }
            });

        });
            
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignBottom().Column(column =>
        {

            column.Item().AlignBottom().PaddingBottom(5).Column(memoCol =>
            {
                memoCol.Item().Text("Memo:").Style(tableHeaderStyle);
                memoCol.Item().Border(1).PaddingLeft(4).PaddingRight(4).MinHeight(48).ExtendHorizontal().Text($"{_order.Memo}").FontSize(11);
            });

            column.Item().Row(row =>
            {
                row.RelativeItem().Text($"Order Taken By: {_order.TakenBy}").Style(smallFooterStyle).Italic();
                row.RelativeItem().Text($"Total Orderd: {_order.TotalOrdered}").Style(smallFooterStyle);
                var totalReceived = _order.TotalGiven;
                row.RelativeItem().Text($"Total Allocated: {totalReceived}").Style(smallFooterStyle);

            });
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
