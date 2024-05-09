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
[System.ComponentModel.DisplayName("Inventory Audit Log Report")]
public class InventoryAdjustmentsAuditReport : ReportBase
{
    private readonly IEnumerable<InventoryAuditItem> _items;

    public InventoryAdjustmentsAuditReport(CompanyInfo info, IEnumerable<InventoryAuditItem> items):base(info)
    {
        _items = items;
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
                    col.Item().AlignCenter().Text("Inventory Audit Log Report").Style(titleStyle);
                });

                row.RelativeItem(1).AlignRight().Column(column =>
                {

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
                    column.RelativeColumn(3);
                    column.RelativeColumn(3);
                    column.RelativeColumn(3);
                    column.RelativeColumn(3);
                    column.RelativeColumn(3);
                    column.RelativeColumn(8);
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Product ID").Style(tableHeaderStyle);

                    header.Cell().Element(CellStyle).Text("Workstation").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("User Name").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Starting Quan.").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Adjustment").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Ending Quan.").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).Text("Reason").Style(tableHeaderStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in _items)
                {
                    table.Cell().Element(CellStyle).Text($"{item.ProductID}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.WorkstationName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.UserName}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.StartingQuantity}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.AdjustmentQuantity}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.EndingQuantity}").Style(tableTextStyle);
                    table.Cell().Element(CellStyle).Text($"{item.AdjustmentReason}").Style(tableTextStyle);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                    }

                }
            });
        });
    }
}
