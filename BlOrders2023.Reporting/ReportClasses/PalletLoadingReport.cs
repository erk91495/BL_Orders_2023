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
public class PalletLoadingReport :IReport
{
    #region Properties
    #endregion Properties

    #region Fields
    private readonly Order _order;
    private readonly Pallet _pallet;

    private readonly TextStyle titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Black);
    private readonly TextStyle subTitleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Black);
    private readonly TextStyle normalTextStyle = TextStyle.Default.FontSize(9);
    private readonly TextStyle tableTextStyle = TextStyle.Default.FontSize(8.5f);
    private readonly TextStyle tableHeaderStyle = TextStyle.Default.FontSize(8.5f).SemiBold();
    private readonly TextStyle smallFooterStyle = TextStyle.Default.FontSize(8.5f);
    #endregion Fields

    public PalletLoadingReport(Order order, Pallet pallet)
    {
        _order = order;
        _pallet = pallet;
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
            headerCol.Item().AlignCenter().PaddingBottom(10).Row(row =>
            {
                row.AutoItem().Text($"{_order.OrderID}-{_pallet.PalletIndex} of {_pallet.TotalPallets}").Style(titleStyle);
                row.RelativeItem().Column( col =>
                {
                    col.Item().Text($"Bowman & Landes Pallet Loading Report").Style(subTitleStyle);
                    col.Item().Text($"{_order.Customer.AllocationType} Order").Style(subTitleStyle);
                });
                FontManager.RegisterFontFromEmbeddedResource("BlOrders2023.Reporting.Assets.Fonts.IDAutomationHC39M_Free.ttf");
                //barcode
                row.RelativeItem().Text($"*{_order.OrderID}*").FontFamily("IDAutomationHC39M").FontSize(18);
            });

        });
    }

    private void ComposeContent(IContainer container)
    {

        container.Column(column => {
            column.Item().Row(row =>
            {
                row.RelativeItem().Text($"{_order.Customer.CustomerName}").Style(titleStyle);
                row.RelativeItem().Column(col =>{ 
                    col.Item().Text($"Order ID: {_order.OrderID}").Style(normalTextStyle).SemiBold();
                    col.Item().ShowIf(!_order.PO_Number.IsNullOrEmpty()).Text($"PO: {_order.PO_Number}").Style(normalTextStyle);
                });
            });
            column.Item().Table(table => {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);

                });

                foreach (var product in _pallet.Items.Keys)
                {

                    table.Cell().Element(CellStyle).Text($"{product.ProductID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{product.ProductName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{_pallet.Items.GetValueOrDefault(product)}").Style(tableTextStyle);
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }
                }
                table.Cell();
                table.Cell().Element(FooterCellStyle).PaddingRight(1).AlignRight().Text("Total: ").Style(tableTextStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_pallet.Items.Values.Sum()}").Style(tableTextStyle);
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
