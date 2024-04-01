using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using static QuestPDF.Helpers.Colors;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Bill Of Lading Report")]
public class BillOfLadingReport : ReportBase
{
    private readonly IEnumerable<Order> _orders;
    private readonly IEnumerable<BillOfLadingItem> _billOfLadingItems;
    private readonly WholesaleCustomer _customer;
    private readonly string _carrierName;
    private readonly string _trailerNumber;
    private readonly string _trailerSeal;
    private readonly DateTime? _appointmentTime;


    public BillOfLadingReport(CompanyInfo companyInfo, IEnumerable<Order> orders, 
        IEnumerable<BillOfLadingItem> items, WholesaleCustomer customer,
        string carrier, string trailerNumber, string trailerSeal, DateTime? appointmentTime)
        : base(companyInfo)
    {
        _orders = orders;
        _billOfLadingItems = items;
        _customer = customer;
        _carrierName = carrier;
        _trailerNumber = trailerNumber;
        _trailerSeal = trailerSeal;
        _appointmentTime = appointmentTime;
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
                row.RelativeItem(1).AlignLeft().AlignMiddle().Height(75).Image(res).FitHeight();

                row.RelativeItem(3).AlignCenter().Column(col =>
                {
                    col.Item().AlignCenter().Text($"Bill Of Lading").Style(titleStyle);
                });

                row.RelativeItem(1).AlignMiddle().AlignRight().Column(column =>
                {

                });

            });

        });
    }

    protected override void ComposeContent(IContainer container)
    {
        container.Column(mainCol => 
        {
            mainCol.Item().Row(row => 
            {
                //Ship To:
                row.RelativeItem(10).Border(1).ExtendHorizontal().AlignCenter().Column(column =>
                {

                    column.Item().Background(Grey.Lighten3).Border(1).PaddingLeft(3).Text("Ship To:");
                    column.Item().PaddingLeft(3).Text($"{_customer.CustomerName}").Style(subTitleStyle);
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().PaddingLeft(3).Column(column =>
                        {
                            column.Item().Text($"{_customer.Address}").Style(normalTextStyle);
                            column.Item().Text($"{_customer.CityStateZip()}").Style(normalTextStyle);
                        });
                    });
                });

                row.RelativeItem(1);
                //Shipper:
                row.RelativeItem(10).Column(shipperCol =>
                {
                    shipperCol.Item().Border(1).ExtendHorizontal().AlignCenter().Column(column =>
                    {
                        column.Item().Background(Grey.Lighten3).Border(1).PaddingLeft(3).Text("Shipper:");
                        column.Item().PaddingLeft(3).Text($"{_companyInfo.LongCompanyName.ToUpper()}").Style(subTitleStyle);
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().PaddingLeft(3).Column(column =>
                            {
                                column.Item().Text($"{_companyInfo.StreetAddress.ToUpper()}").Style(normalTextStyle);
                                column.Item().Text($"{_companyInfo.City.ToUpper()}, {_companyInfo.State.ToUpper()} {_companyInfo.ShortZipCode}").Style(normalTextStyle);
                                column.Item().Text($"{_companyInfo.PhoneString()}").Style(normalTextStyle);

                            });
                        });
                    });
                });
            });

            mainCol.Item().Height(14);

            mainCol.Item().Row(row =>
            {
                //Bill To:
                row.RelativeItem(10).Border(1).ExtendHorizontal().AlignCenter().Column(column =>
                {
                    column.Item().Background(Grey.Lighten3).Border(1).PaddingLeft(3).Text("Bill To:");
                    column.Item().PaddingLeft(3).Text($"{_customer.BillingCustomerName}").Style(subTitleStyle);
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().PaddingLeft(3).Column(column =>
                        {
                            column.Item().Text($"{_customer.BillingAddress}").Style(normalTextStyle);
                            column.Item().Text($"{_customer.BillingCityStateZip()}").Style(normalTextStyle);
                            column.Item().Text($"{_customer.Buyer}").Style(normalTextStyle);
                            if (!_customer.Email.IsNullOrEmpty())
                            {
                                column.Item().Text($"{_customer.Email.Trim()}").Style(normalTextStyle);
                            }
                            column.Item().Text($"{_customer.PhoneString()}").Style(normalTextStyle);

                        });
                    });
                });

                row.RelativeItem(1);

                row.RelativeItem(10);
            });

            mainCol.Item().PaddingTop(12).Row(poRow =>
            {
                if(_appointmentTime.HasValue)
                {
                    poRow.RelativeItem().Text($"Appointment Time: {_appointmentTime.Value.ToString("MM/dd/yy h:m tt)}")}").Style(subTitleStyle);
                } 
            });

            mainCol.Item().PaddingTop(2).Row(poRow => 
            {
                poRow.RelativeItem(10);
                poRow.RelativeItem(1);
                poRow.RelativeItem(10).AlignLeft().Text($"PO(s): {string.Join(",", _orders.Where(o => !o.PO_Number.IsNullOrEmpty()).Select(o => o.PO_Number))}").Style(subTitleStyle).FontSize(15);
            });

            mainCol.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);
                    column.RelativeColumn(6);
                    column.RelativeColumn(2);
                    column.RelativeColumn(2);

                });

                table.Header(header =>
                {
                    

                    header.Cell().Element(CellStyle).Text("# Of Pallets").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Vendor Product Code").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("# Of Cases").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Product Name").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).Text("Gross Wt. (lbs)").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Net Wt. (lbs)").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                // step 3
                foreach (var item in _billOfLadingItems)
                {

                    //they dont want to see zeros so hide them
                    var pallets = item.NumberOfPallets != 0 ? item.NumberOfPallets.ToString() : string.Empty;
                    table.Cell().Element(CellStyle).Text($"{pallets}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.NumberOfCases}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.ProductName}").Style(tableTextStyle);

                    table.Cell().Element(CellStyle).Text($"{item.GrossWt:N2}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.NetWt:N2}").Style(tableTextStyle);


                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Grey.Lighten2).PaddingVertical(2);
                    }
                }

                table.Cell().Element(FooterCellStyle).Text($"{_billOfLadingItems.Sum(i => i.NumberOfPallets)}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_billOfLadingItems.Sum(i => i.NumberOfCases)}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_billOfLadingItems.Sum(i => i.GrossWt):N2}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).Text($"{_billOfLadingItems.Sum(i => i.NetWt):N2}").Style(tableHeaderStyle);

                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.BorderBottom(1).BorderTop(1).BorderColor(Black).PaddingVertical(2);
                }
            });
            mainCol.Item().ShowIf(_orders.Any(o => o.Frozen != true)).PaddingTop(12).AlignCenter().AlignMiddle().Text($"KEEP REFRIGERATED AT 28-32° F").Style(subTitleStyle);
            mainCol.Item().ShowIf(_orders.Any(o => o.Frozen == true)).PaddingTop(12).AlignCenter().AlignMiddle().Text($"KEEP FROZEN").Style(subTitleStyle);

            mainCol.Item().PaddingTop(12).Row(row =>
            {
                row.RelativeItem().Text($"Delivery Trailer #: {_trailerNumber ?? string.Empty}");
                row.RelativeItem().Text($"Trailer Seal #: {_trailerSeal ?? string.Empty}");
            });
            

            mainCol.Item().EnsureSpace().PaddingTop(12).Row(row =>
            {
                row.RelativeItem().Column(signCol =>
                {
                    signCol.Item().Text($"Shipper: {_companyInfo.LongCompanyName.ToUpper()}").Bold();
                    signCol.Item().Row(row =>
                    {
                        row.AutoItem().Text($"Shipped By: ");
                        row.AutoItem().Text($"                                                                              ").Underline(true);
                        row.ConstantItem(25);
                        row.AutoItem().Text($"Date: ");
                        row.AutoItem().Text($"                                                                              ").Underline(true);
                    });
                    
                    signCol.Item().PaddingTop(12).Text($"Carrier: {(_carrierName ?? string.Empty).ToUpper()}").Bold();
                    signCol.Item().Row(row =>
                    {
                        row.AutoItem().Text($"Received By: ");
                        row.AutoItem().Text($"                                                                              ").Underline(true);
                        row.ConstantItem(20);
                        row.AutoItem().Text($"Date: ");
                        row.AutoItem().Text($"                                                                              ").Underline(true);
                    });

                    signCol.Item().PaddingTop(12).Text($"Cosignee (To): {_customer.CustomerName.ToUpper()}").Bold();
                    signCol.Item().Row(row =>
                    {
                        row.AutoItem().Text($"Received By: ");
                        row.AutoItem().Text($"                                                                              ").Underline(true);
                        row.ConstantItem(20);
                        row.AutoItem().Text($"Date: ");
                        row.AutoItem().Text($"                                                                              ").Underline(true);
                    });
                });
            });
        });
    }
}
