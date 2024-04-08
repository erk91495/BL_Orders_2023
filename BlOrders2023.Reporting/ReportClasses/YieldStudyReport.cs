using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Yield Study Report")]
public class YieldStudyReport : ReportBase
{
    private readonly IEnumerable<LiveInventoryItem> _inventoryItems;
    private readonly DateTime _productionDate;

    public YieldStudyReport(CompanyInfo companyInfo, IEnumerable<LiveInventoryItem> items, DateTime productionDate) : base(companyInfo)
    {
        _inventoryItems = items;
        _productionDate = productionDate;
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
                    col.Item().AlignCenter().Text($"{_companyInfo.ShortCompanyName} Turkeys, Inc.").Style(titleStyle);
                    col.Item().AlignCenter().Text("Yield Study Analysis").Style(titleStyle);
                });

                row.RelativeItem(1).AlignRight().Column(column =>
                {
                    column.Item().Text($"Packed On: {_productionDate:M/d/yyyy}").Style(subTitleStyle);
                });
            });
        });
    }

    protected override void ComposeContent(IContainer container)
    {
        container.Column(column => {
            //Items
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(2);
                    column.RelativeColumn(7);
                    column.RelativeColumn(2);
                    column.RelativeColumn(4);
                    column.RelativeColumn(4);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).AlignLeft().Text("Product ID").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Product Name").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Cases").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Net Weight").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Avg. Case Weight").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                var groupedItems = _inventoryItems.GroupBy(p => p.Product);
                foreach (var group in groupedItems)
                {


                    //var totalWeight = 0f;
                    //var totalCases = 0;
                    table.Cell().Element(CellStyle).Text($"{group.Key.ProductID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{group.Key.ProductName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{group.Count()} cs.").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{group.Sum(i => i.NetWeight):N2}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{group.Average(i => i.NetWeight):N2}").Style(tableTextStyle);
                    //foreach(var item in group)
                    //{
                    //    table.Cell().ColumnSpan(2);
                    //    table.Cell().Element(CellStyle).Text($"{item.Scanline}").Style(tableTextStyle);
                    //    table.Cell().Element(CellStyle).Text($"{item.NetWeight:N2}").Style(tableTextStyle);
                    //    totalWeight += item.NetWeight ?? 0;
                    //    totalCases ++;
                    //}
                    //table.Cell().Element(FooterCellStyle);
                    //table.Cell().Element(FooterCellStyle).Text($"Sub Total:").Style(tableHeaderStyle);
                    //table.Cell().Element(FooterCellStyle).Text($"{totalCases} cs.").Style(tableHeaderStyle);
                    //table.Cell().Element(FooterCellStyle).Text($"{totalWeight:N2} lbs.").Style(tableHeaderStyle);


                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }

                }

                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"Totals:").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{_inventoryItems.Count()} cs.").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{_inventoryItems.Sum(p => p.NetWeight):N2} lbs.").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle);


                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.PaddingVertical(2).AlignRight();
                }

            });
        });
    }
    
}
