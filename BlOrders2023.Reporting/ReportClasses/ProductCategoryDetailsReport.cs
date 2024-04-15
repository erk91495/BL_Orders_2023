using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using BlOrders2023.Models.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BlOrders2023.Reporting.ReportClasses;
[System.ComponentModel.DisplayName("Category Details Report")]
public class ProductCategoryDetailsReport : ReportBase
{
    private readonly IEnumerable<OrderItem> _items;
    private readonly IEnumerable<Product> _products;
    private readonly DateTimeOffset _startDate;
    private readonly DateTimeOffset _endDate;
    private readonly decimal _totalDollars;
    private readonly float _totalPounds;

    public ProductCategoryDetailsReport(CompanyInfo companyInfo,  IEnumerable<OrderItem> items, IEnumerable<Product> products, DateTimeOffset startDate, DateTimeOffset endDate)
    :base(companyInfo)
    {
        _items = items;
        _products = products;
        _startDate = startDate;
        _endDate = endDate;
        _totalDollars = _items.Sum(i => i.GetTotalPrice);
        _totalPounds = _items.Sum(i => i.PickWeight);
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
                    col.Item().AlignCenter().Text($"Product Category Details Report").Style(titleStyle);
                });

                row.RelativeItem(1).AlignRight().Column(column =>
                {
                    column.Item().Text($"From: {_startDate.ToString("M/d/yy")}").Style(subTitleStyle);
                    column.Item().Text($"To: {_endDate.ToString("M/d/yy")}").Style(subTitleStyle);
                });
            });
        });
    }

    protected override void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            //Items
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(column =>
                {
                    column.RelativeColumn(1);
                    column.RelativeColumn(2);
                    column.RelativeColumn(8);
                    column.RelativeColumn(2);
                    column.RelativeColumn(4);
                    column.RelativeColumn(3);
                    column.RelativeColumn(4);
                    column.RelativeColumn(3);
                });

                table.Header(header =>
                {
                    header.Cell().ColumnSpan(3).Element(CellStyle).Text("Category").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Quantity").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Pounds.").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("% of Total lbs. Sales").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("Dollars").Style(tableHeaderStyle);
                    header.Cell().Element(CellStyle).AlignRight().Text("% of Total $ Sales").Style(tableHeaderStyle);


                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                var groupedItems = _items.GroupBy(i => i.Product.Category).OrderBy(i => i.Key != null ? i.Key.DisplayIndex : int.MaxValue );
                foreach (var category in groupedItems)
                {
                    if(category.Key == null)
                    {
                        table.Cell().ColumnSpan(3).Element(CategoryCellStyle).Text($"Uncategorized").Style(tableTextStyle).SemiBold();
                    }
                    else
                    {
                        table.Cell().ColumnSpan(3).Element(CategoryCellStyle).Text($"{category.Key.CategoryName}").Style(tableTextStyle).SemiBold();
                    }
                    table.Cell().Element(CategoryCellStyle).AlignRight().Text($"{category.Sum(i => i.QuantityReceived)}").Style(tableTextStyle).SemiBold();
                    var totalWeight = category.Sum(i => i.PickWeight);
                    table.Cell().Element(CategoryCellStyle).AlignRight().Text($"{totalWeight:N2}").Style(tableTextStyle).SemiBold();
                    table.Cell().Element(CategoryCellStyle).AlignRight().Text($"{(totalWeight / _totalPounds) * 100f:N2}%").Style(tableTextStyle).SemiBold();
                    var totalPrice = category.Sum(i => i.GetTotalPrice);
                    table.Cell().Element(CategoryCellStyle).AlignRight().Text($"{totalPrice:C}").Style(tableTextStyle).SemiBold();
                    table.Cell().Element(CategoryCellStyle).AlignRight().Text($"{(totalPrice / _totalDollars) * 100m:N2}%").Style(tableTextStyle).SemiBold();
                    
                    static IContainer CategoryCellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Black).PaddingTop(4);
                    }
                    var groupedByProduct = category.GroupBy(i => i.Product).OrderBy(p => p.Key.ProductID);
                    var productsByCategory = _products.Where(p => p.Category == category.Key).OrderBy(p => p.ProductID);
                    foreach (var product in productsByCategory)
                    {
                        var items = groupedByProduct.SingleOrDefault(p => p.Key.ProductID == product.ProductID);
                        if (items == null)
                        {
                            AddCells(ref table, product, 0,0,0, product == groupedByProduct.Last().Key);
                        }
                        else
                        {
                            var itemtotalReceived = items.Sum(i => i.QuantityReceived);
                            var itemTotalWeight = items.Sum(i => i.PickWeight);
                            var itemTotalPrice = items.Sum(i => i.GetTotalPrice);
                            AddCells(ref table, product, itemtotalReceived, itemTotalWeight, itemTotalPrice, product == groupedByProduct.Last().Key);
                        }

                    }
                }

                table.Cell().ColumnSpan(3).Element(FooterCellStyle).Text("Totals:").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{_items.Sum(i => i.QuantityReceived)}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{_totalPounds:N2}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle);
                table.Cell().Element(FooterCellStyle).AlignRight().Text($"{_totalDollars:C}").Style(tableHeaderStyle);
                table.Cell().Element(FooterCellStyle);


                static IContainer FooterCellStyle(IContainer container)
                {
                    return container.PaddingVertical(2).BorderTop(1).BorderColor(Colors.Black);
                }

            });
        });
    }

    private void AddCells(ref TableDescriptor table, Product product, int totalReceived, float totalWeight, decimal totalPrice, bool last)
    {
        if (last)//dont want a bottom border on the last row
        {
            table.Cell().Element(CellStyle);
            table.Cell().Element(CellStyle).Text($"{product.ProductID}").Style(tableTextStyle);
            table.Cell().Element(CellStyle).Text($"{product.ProductName}").Style(tableTextStyle);
            table.Cell().Element(CellStyle).AlignRight().Text($"{totalReceived}").Style(tableTextStyle);
            
            table.Cell().Element(CellStyle).AlignRight().Text($"{totalWeight:N2}").Style(tableTextStyle);
            table.Cell().Element(CellStyle).AlignRight().Text($"{(totalWeight / _totalPounds) * 100f:N2}%").Style(tableTextStyle);

            table.Cell().Element(CellStyle).AlignRight().Text($"{totalPrice:C}").Style(tableTextStyle);
            table.Cell().Element(CellStyle).AlignRight().Text($"{(totalPrice / _totalDollars) * 100m:N2}%").Style(tableTextStyle);

            static IContainer CellStyle(IContainer container)
            {
                return container;
            }
        }
        else
        {
            table.Cell().Element(CellStyle);
            table.Cell().Element(CellStyle).Text($"{product.ProductID}").Style(tableTextStyle);
            table.Cell().Element(CellStyle).Text($"{product.ProductName}").Style(tableTextStyle);
            table.Cell().Element(CellStyle).AlignRight().Text($"{totalReceived}").Style(tableTextStyle);

            table.Cell().Element(CellStyle).AlignRight().Text($"{totalWeight:N2}").Style(tableTextStyle);
            table.Cell().Element(CellStyle).AlignRight().Text($"{(totalWeight / _totalPounds) * 100f:N2}%").Style(tableTextStyle);

            table.Cell().Element(CellStyle).AlignRight().Text($"{totalPrice:C}").Style(tableTextStyle);
            table.Cell().Element(CellStyle).AlignRight().Text($"{(totalPrice / _totalDollars) * 100m:N2}%").Style(tableTextStyle);
            static IContainer CellStyle(IContainer container)
            {
                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
            }
        }
    }
}
