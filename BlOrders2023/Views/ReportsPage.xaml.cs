// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Core.Services;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using BlOrders2023.UserControls;
using BlOrders2023.Dialogs;
using BlOrders2023.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using QuestPDF.Fluent;
using Syncfusion.UI.Xaml.Data;
using Windows.System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.ComponentModel;
using BlOrders2023.Core.Contracts.Services;
using Castle.Core.Resource;
using Syncfusion.UI.Xaml.Editors;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ReportsPage : Page
{
    #region Properties
    public ReportsPageViewModel ViewModel { get; }
    public ObservableCollection<ReportControl> ReportsList { get; set; }

    public bool IsSpinnerVisible { get; set; }
    #endregion Properties

    #region Fields
    private readonly ReportGenerator reportGenerator;
    #endregion Fields

    #region Constructors
    public ReportsPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<ReportsPageViewModel>();
        reportGenerator = new(App.CompanyInfo);


        ReportsList = new();
        foreach (Type t in Reports.AvalibleReports)
        {

            ReportControl control = new(t);
            control.ReportSelected += ReportControl_Click;
            ReportsList.Add(control);
        }

    }
    #endregion Constructors

    #region Methods
    public async void ReportControl_Click(object? sender, EventArgs e)
    {
        if(sender is ReportControl control)
        {
            List<IReport>? report = null;
            if(control.ReportType == typeof(WholesaleInvoice))
            {
                SingleValueInputDialog dialog = new SingleValueInputDialog()
                {
                    Title = "Order ID",
                    Prompt = "Please Enter An Order ID",
                    XamlRoot = XamlRoot,
                    PrimaryButtonText = "Submit",
                    SecondaryButtonText = "Cancel",
                    ValidateValue = ValidateOrderID,
                };
                var result = await dialog.ShowAsync();
                if(result == ContentDialogResult.Primary){
                    var res = int.TryParse(dialog.Value ?? "", out var id);
                    if (!res)
                    {
                        ContentDialog d = new()
                        {
                            XamlRoot = XamlRoot,
                            Title = "User Error",
                            Content = $"Invalid Order ID {dialog.Value}.\r\n " +
                            $"Please enter a numeric value",
                            PrimaryButtonText = "ok",
                        };
                        await d.ShowAsync();
                    }
                    else
                    {
                        spinner.IsVisible = true;
                        var order = ViewModel.GetOrder(id);
                        if (order != null)
                        {
                            var toTotal = ViewModel.GetTotalsCategories();
                            report = new() { reportGenerator.GetWholesaleInvoice(order, toTotal) };
                        }
                        else
                        {
                            ContentDialog d = new()
                            {
                                XamlRoot = XamlRoot,
                                Title = "Error",
                                Content = $"No order with the order id {id} found.",
                                PrimaryButtonText = "ok",
                            };
                            await d.ShowAsync();
                        }
                    }
                }
            }
            else if(control.ReportType == typeof(WholesaleOrderPickupRecap))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    spinner.IsVisible = true;
                    var values = ViewModel.GetOrderTotals(startDate, endDate);
                    report = new() { reportGenerator.GetWholesaleOrderTotals(values, startDate, endDate) };
                    
                    ContentDialog d = new()
                    {
                        XamlRoot = XamlRoot,
                        Title = "Wholesale Order Pickup Recap",
                        Content = "Would you like that by date or alphabetical?",
                        PrimaryButtonText = "Date",
                        SecondaryButtonText = "Alphabetical",
                    };
                    var res = await d.ShowAsync();


                    var orders = res == ContentDialogResult.Primary ? await ViewModel.GetOrdersByPickupDateAsync(startDate,endDate) : ViewModel.GetOrdersByPickupDateThenName(startDate, endDate);
                    report = new() { reportGenerator.GetWholesaleOrderPickupRecap(orders, startDate, endDate) };
                }
            }
            else if(control.ReportType == typeof(WholesaleOrderTotals))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();
                
                if(dateTuple.Item1 != null && dateTuple.Item2 != null) 
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    spinner.IsVisible = true;
                    var values = ViewModel.GetOrderTotals(startDate, endDate);
                    report = new() { reportGenerator.GetWholesaleOrderTotals(values, startDate, endDate) };
                }
            }
            else if(control.ReportType == typeof(WholesalePaymentsReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = new DateTimeOffset(dateTuple.Item1.Value.Year, dateTuple.Item1.Value.Month, dateTuple.Item1.Value.Day, 0, 0, 0,TimeSpan.Zero);
                    DateTimeOffset endDate = new DateTimeOffset(dateTuple.Item2.Value.Year, dateTuple.Item2.Value.Month, dateTuple.Item2.Value.Day, 23, 59, 59, TimeSpan.Zero);
                    spinner.IsVisible = true;
                    var values = ViewModel.GetWholesalePayments(startDate, endDate);
                    report = new() { reportGenerator.GetWholesalePaymentsReport(values, startDate, endDate) };
                }
            }
            else if(control.ReportType == typeof(ShippingList))
            {
                SingleValueInputDialog dialog = new SingleValueInputDialog()
                {
                    Title = "Order ID",
                    Prompt = "Please Enter An Order ID",
                    XamlRoot = XamlRoot,
                    PrimaryButtonText = "Submit",
                    SecondaryButtonText = "Cancel",
                    ValidateValue = ValidateOrderID,
                };
                var result = await dialog.ShowAsync();
                if(result == ContentDialogResult.Primary)
                {
                    var res = int.TryParse(dialog.Value ?? "", out var id);
                    if (!res)
                    {
                        ContentDialog d = new()
                        {
                            XamlRoot = XamlRoot,
                            Title = "User Error",
                            Content = $"Invalid Order ID {dialog.Value}.\r\n " +
                            $"Please enter a numeric value",
                            PrimaryButtonText = "ok",
                        };
                        await d.ShowAsync();
                    }
                    else
                    {
                        var order = ViewModel.GetOrder(id);
                        if (order != null)
                        {
                            report = new() { reportGenerator.GetShippingList(order) };
                        }
                        else
                        {
                            ContentDialog d = new()
                            {
                                XamlRoot = XamlRoot,
                                Title = "Error",
                                Content = $"No order with the order id {id} found.",
                                PrimaryButtonText = "ok",
                            };
                            await d.ShowAsync();
                        }
                    }
                }
            }
            else if(control.ReportType == typeof(UnpaidInvoicesReport))
            {
                WholesaleCustomer customer;
                CustomerSelectionDialog custDialog = new(XamlRoot)
                {
                    PrimaryButtonText = "Select Customer",
                    SecondaryButtonText = ""
                };
                var res = await custDialog.ShowAsync();
                if (res == ContentDialogResult.Primary && custDialog.ViewModel != null && custDialog.ViewModel.SelectedCustomer != null)
                {
                    spinner.IsVisible = true;
                    customer = custDialog.ViewModel.SelectedCustomer;
                    var values = ViewModel.GetUnpaidInvoicedInvoices(customer);
                    report = new() { reportGenerator.GetUnpaidInvoicesReport(values) };
                    
                }
            }
            else if(control.ReportType == typeof(AggregateInvoiceReport))
            {
                MultipleCustomerSelectionDialog dialog = new(XamlRoot);
                var res = await dialog.ShowAsync();
                if(res == ContentDialogResult.Primary)
                {
                    if(dialog.Customers.IsNullOrEmpty())
                    {
                        ContentDialog d = new()
                        {
                            XamlRoot = XamlRoot,
                            Title = "Error",
                            Content = $"No Customers Selected",
                            PrimaryButtonText = "ok",
                        };
                        await d.ShowAsync();
                    }
                    else
                    {
                        var dateTuple = await ShowDateRangeSelectionAsync();
                        if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                        {
                            DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                            DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                            IEnumerable<int> ids = dialog.Customers!.Select(c => c.CustID);
                            spinner.IsVisible = true;
                            var values = await ViewModel.GetOrdersByCustomerIdAndPickupDateAsync(ids, startDate, endDate);
                            if(values.Any(o => o.Paid == true))
                            {
                                ContentDialog d = new()
                                {
                                    XamlRoot = XamlRoot,
                                    Title = "Warning",
                                    Content = $"One or more invoices on this report have already been marked paid",
                                    PrimaryButtonText = "ok",
                                };
                                await d.ShowAsync();
                            }
                            report = new() { reportGenerator.GetAggregateInvoiceReport(values, startDate, endDate) };
                        }

                    }
                }
            }
            else if(control.ReportType == typeof(OutstandingBalancesReport))
            {
                spinner.IsVisible = true;
                var values = await ViewModel.GetOutstandingOrdersAsync();
                report = new() { reportGenerator.GetOutstandingBalancesReport(values) };
            }
            else if(control.ReportType == typeof(QuarterlySalesReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    spinner.IsVisible = true;
                    var values = await ViewModel.GetProductTotalsAsync(startDate, endDate);
                    report = new() {  reportGenerator.GetQuarterlySalesReport(values, startDate, endDate) };
                }
            }
            else if(control.ReportType == typeof(FrozenOrdersReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    spinner.IsVisible = true;
                    var values = ViewModel.GetFrozenOrders(startDate, endDate);
                    report = new() { reportGenerator.GetFrozenOrdersReport(values, startDate, endDate) };
                }
            }
            else if (control.ReportType == typeof(PickList))
            {
                SingleValueInputDialog dialog = new SingleValueInputDialog()
                {
                    Title = "Order ID",
                    Prompt = "Please Enter An Order ID",
                    XamlRoot = XamlRoot,
                    PrimaryButtonText = "Submit",
                    SecondaryButtonText = "Cancel",
                    ValidateValue = ValidateOrderID,
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var res = int.TryParse(dialog.Value ?? "", out var id);
                    if (!res)
                    {
                        ContentDialog d = new()
                        {
                            XamlRoot = XamlRoot,
                            Title = "User Error",
                            Content = $"Invalid Order ID {dialog.Value}.\r\n " +
                            $"Please enter a numeric value",
                            PrimaryButtonText = "ok",
                        };
                        await d.ShowAsync();
                    }
                    else
                    {
                        spinner.IsVisible = true;
                        var order = await ViewModel.GetOrderAsync(id);
                        if (order != null)
                        {
                            report = new() { reportGenerator.GetPickList(order) };
                        }
                        else
                        {
                            ContentDialog d = new()
                            {
                                XamlRoot = XamlRoot,
                                Title = "Error",
                                Content = $"No order with the order id {id} found.",
                                PrimaryButtonText = "ok",
                            };
                            await d.ShowAsync();
                        }
                    }
                }
            }
            else if(control.ReportType == typeof(PalletLoadingReport))
            {
                SingleValueInputDialog dialog = new SingleValueInputDialog()
                {
                    Title = "Order ID",
                    Prompt = "Please Enter An Order ID",
                    XamlRoot = XamlRoot,
                    PrimaryButtonText = "Submit",
                    SecondaryButtonText = "Cancel",
                    ValidateValue = ValidateOrderID,
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var res = int.TryParse(dialog.Value ?? "", out var id);
                    if (!res)
                    {
                        ContentDialog d = new()
                        {
                            XamlRoot = XamlRoot,
                            Title = "User Error",
                            Content = $"Invalid Order ID {dialog.Value}.\r\n " +
                            $"Please enter a numeric value",
                            PrimaryButtonText = "ok",
                        };
                        await d.ShowAsync();
                    }
                    else
                    {
                        spinner.IsVisible = true;
                        var order = await ViewModel.GetOrderAsync(id);
                        if (order != null)
                        {
                            var print = false;
                            if (order.Allocated != true)
                            {

                                var printDialog = new ContentDialog()
                                {
                                    XamlRoot = XamlRoot,
                                    Title = "Print Confirmation",
                                    Content = "This order has not yet been allocated. Are you sure you want to print pallet tickets?",
                                    PrimaryButtonText = "Print",
                                    CloseButtonText = "Cancel",
                                };

                                result = await printDialog.ShowAsync();
                                if (result == ContentDialogResult.Primary)
                                {
                                    print = true;
                                }


                            }
                            else
                            {
                                print = true;
                            }

                            if(print) 
                            {
                                IPalletizer palletizer = new BoxPalletizer(new PalletizerConfig() { SingleItemPerPallet = order.Customer.SingleProdPerPallet ?? false }, order);
                                IEnumerable<Pallet> pallets = await palletizer.PalletizeAsync();
                                report = reportGenerator.GetPalletLoadingReport(order, pallets);
                            }
                        }
                        else
                        {
                            ContentDialog d = new()
                            {
                                XamlRoot = XamlRoot,
                                Title = "Error",
                                Content = $"No order with the order id {id} found.",
                                PrimaryButtonText = "ok",
                            };
                            await d.ShowAsync();
                        }
                    }
                }
            }
            else if( control.ReportType == typeof(CurrentInventoryReport))
            {
                spinner.IsVisible = true;
                var currentInventory = ViewModel.GetInventoryTotals();
                report = new() { reportGenerator.GetCurrentInventoryReport(currentInventory) };
            }
            else if(control.ReportType == typeof(InventoryDetailsReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    spinner.IsVisible = true;
                    var orders = ViewModel.GetNonFrozenOrdersByPickupDate(startDate, endDate);
                    var currentInventory = ViewModel.GetInventoryTotals();
                    var notFilled = ViewModel.GetAllocatedNotReceivedTotals();
                    report = new() { reportGenerator.GetInventoryDetailsReport(currentInventory, orders, notFilled, startDate, endDate) };
                }

            }
            else if(control.ReportType == typeof(OutOfStateSalesReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    spinner.IsVisible = true;
                    var orders = ViewModel.GetOutOfStateOrders(startDate, endDate);
                    var currentInventory = ViewModel.GetInventory();
                    report = new() { reportGenerator.GetOutOfStateSalesReport(orders, startDate, endDate) };
                }
            }
            else if(control.ReportType == typeof(BillOfLadingReport))
            {
                var customers = ViewModel.GetWholesaleCustomers();
                var dialog = new CustomerOrderSelectionDialog(customers)
                {
                    XamlRoot = XamlRoot
                };
                dialog.CustomerChanged += CustomerOrderSelectionDialog_CustomerChanged;
                var res = await dialog.ShowAsync();
                if(res == ContentDialogResult.Primary)
                {
                    var orders = dialog.SelectedOrders;
                    var customer = dialog.SelectedCustomer;
                    var billOfLadingInput = new BillOfLadingDataInputDialog(orders)
                    {
                        XamlRoot = XamlRoot
                    };
                    res = await billOfLadingInput.ShowAsync();
                    if(res == ContentDialogResult.Primary)
                    {
                        var items = billOfLadingInput.Items;
                        var carrier = billOfLadingInput.CarrierName;
                        var trailerNumber = billOfLadingInput.TrailerNumber;
                        var trailerSeal = billOfLadingInput.TrailerSeal;
                        DateTime? appointmentDate = null;
                        if(billOfLadingInput.AppointmentDate != null && billOfLadingInput.AppointmentTime != null)
                        {
                            var inputDate = billOfLadingInput.AppointmentDate.Value;
                            var inputTime = billOfLadingInput.AppointmentTime.Value;
                            appointmentDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, inputTime.Hour, inputTime.Minute, inputTime.Second);
                        }
                        report = new() { reportGenerator.GetBillOfLadingReport(orders,items,customer,carrier,trailerNumber,trailerSeal, appointmentDate) };
                    }
                }

            }
            else if(control.ReportType == typeof(ShippingItemAuditReport))
            {
                spinner.IsVisible = true;
                var dialog = new AuditDataInputDialog(){ XamlRoot = XamlRoot };
                var res = await dialog.ShowAsync();
                if(res == ContentDialogResult.Primary)
                {
                    var fieldsToMatch = dialog.GetCheckedBoxes();
                    ShippingItem? item = null;
                    if (dialog.InputType == InputTypes.Scanline)
                    {
                        item = ViewModel.GetShippingItem(dialog.Scanline);
                    }
                    else if (dialog.InputType == InputTypes.Serial && dialog.ProductID != null)
                    {
                        item = ViewModel.GetShippingItem((int)dialog.ProductID, dialog.Serial);
                    }
                    if(item != null)
                    {
                        DateTime? startDate = null;
                        DateTime? endDate = null;
                        if (fieldsToMatch.Contains("DateRange"))
                        {
                            DateTime sDate = dialog.DateRange.StartDate.Value.Date;
                            DateTime eDate = dialog.DateRange.EndDate.Value.Date;
                            startDate = new DateTime(sDate.Year, sDate.Month, sDate.Day, 0, 0, 0, 0, DateTimeKind.Local);
                            endDate = new DateTime(eDate.Year, eDate.Month, eDate.Day, 23, 59, 59, 999, DateTimeKind.Local);
                        }
                        var items = ViewModel.GetShippingItems(item,fieldsToMatch.Contains("ProductID"),fieldsToMatch.Contains("Serial"),
                            fieldsToMatch.Contains("PackDate"),fieldsToMatch.Contains("Scanline"),startDate,endDate);
                        if(!items.IsNullOrEmpty())
                        {
                            report = new() { reportGenerator.GetShippingItemAuditReport(items, item, fieldsToMatch, startDate, endDate) };
                        }
                        else
                        {
                            ContentDialog d = new()
                            {
                                XamlRoot = XamlRoot,
                                Title = "Error",
                                Content = $"This error shouldnt happen, but it did",
                                PrimaryButtonText = "ok",
                            };
                            await d.ShowAsync();
                        }
                        
                    }
                    else
                    {
                        ContentDialog d = new()
                        {
                            XamlRoot = XamlRoot,
                            Title = "Error",
                            Content = $"No products were found matching the given criteria",
                            PrimaryButtonText = "ok",
                        };
                        await d.ShowAsync();
                    }
                }

            }
            else if(control.ReportType == typeof(ProductCategoryTotalsReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    var values = await ViewModel.GetOrdersByPickupDateAsync(startDate, endDate);
                    var orderItems = values.SelectMany(o => o.Items);
                    if(orderItems.IsNullOrEmpty())
                    {
                        ContentDialog d = new()
                        {
                            XamlRoot = XamlRoot,
                            Title = "Error",
                            Content = $"No records found for the given date range",
                            PrimaryButtonText = "ok",
                        };
                        await d.ShowAsync();
                    }
                    else
                    {
                        report = new() { reportGenerator.GetProductCategoryTotalsReport(orderItems, startDate, endDate) };
                    }
                    
                }
            }
            else if (control.ReportType == typeof(ProductCategoryDetailsReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    var categories  = ViewModel.GetProductCategories();
                    var categorySelect = new MultiSelectListBox(categories.Cast<object>().ToObservableCollection());
                    ContentDialog diag = new()
                    {
                        Title = "Select Categories",
                        XamlRoot = XamlRoot,
                        Content = categorySelect,
                        PrimaryButtonText = "Next",
                        CloseButtonText = "Cancel",
                    };
                    if(await diag.ShowAsync() == ContentDialogResult.Primary)
                    {
                        var categoriesToLookup = categories.Where(i => categorySelect.SelectedItems.Contains(i.CategoryName)).ToList();
                        DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                        DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                        //var startDate = DateTimeOffset.Now.AddDays(-100);
                        //var endDate = DateTimeOffset.Now;
                        spinner.IsVisible = true;
                        var values = await ViewModel.GetOrdersByPickupDateAsync(startDate, endDate);
                        var orderItems = values.SelectMany(o => o.Items.Where(i => categoriesToLookup.Contains(i.Product.Category)));
                        if (orderItems.IsNullOrEmpty())
                        {
                            ContentDialog d = new()
                            {
                                XamlRoot = XamlRoot,
                                Title = "Error",
                                Content = $"No records found for the given date range",
                                PrimaryButtonText = "ok",
                            };
                            await d.ShowAsync();
                        }
                        else
                        {
                            var products = ViewModel.GetProducts();
                            report = new() { reportGenerator.GetProductCategoryDetailsReport(orderItems, products, startDate, endDate) };

                        }
                    }


                }
            }
            else if(control.ReportType == typeof(WholesaleInvoiceTotalsReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {

                    CustomerSelectionDialog custDialog = new(XamlRoot)
                    {
                        PrimaryButtonText = "Select Customer",
                        SecondaryButtonText = ""
                    };
                    var res = await custDialog.ShowAsync();
                    if (res == ContentDialogResult.Primary && custDialog.ViewModel != null && custDialog.ViewModel.SelectedCustomer != null)
                    {
                        var customer = custDialog.ViewModel.SelectedCustomer;
                        List<int> ids = new List<int>(){ customer.CustID };
                        var orders = await ViewModel.GetOrdersByCustomerIdAndPickupDateAsync(ids, dateTuple.Item1.Value, dateTuple.Item2.Value);
                        if(!orders.IsNullOrEmpty())
                        {
                            spinner.IsVisible=true;
                            report = new() { reportGenerator.GetWholesaleInvoiceTotalsReport(customer,orders,dateTuple.Item1.Value,dateTuple.Item2.Value) };
                        }
                        else
                        {
                            ContentDialog d = new()
                            {
                                XamlRoot = XamlRoot,
                                Title = "No Orders",
                                Content = $"No orders found for the given date range.",
                                PrimaryButtonText = "ok",
                            };
                            await d.ShowAsync();
                        }
                    }
                }
            }
            else if (control.ReportType == typeof(HistoricalQuarterlySalesReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    spinner.IsVisible = true;
                    
                    var current = await ViewModel.GetProductTotalsAsync(startDate, endDate);
                    var off1Year = await ViewModel.GetProductTotalsAsync(startDate.AddYears(-1), endDate.AddYears(-1));
                    var off2Year = await ViewModel.GetProductTotalsAsync(startDate.AddYears(-2), endDate.AddYears(-2));

                    var allItems = new List<IEnumerable<ProductTotalsItem>> { current, off1Year, off2Year };

                    report = new() { reportGenerator.GetHistoricalQuarterlySalesReport(allItems, startDate.Date, endDate.Date) };
                }
            }
            else if(control.ReportType == typeof(HistoricalProductCategoryTotalsReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    spinner.IsVisible = true;

                    var values = await ViewModel.GetOrdersByPickupDateAsync(startDate, endDate);
                    var current = values.SelectMany(o => o.Items);

                    values = await ViewModel.GetOrdersByPickupDateAsync(startDate.AddYears(-1), endDate.AddYears(-1));
                    var off1Year = values.SelectMany(o => o.Items);

                    values = await ViewModel.GetOrdersByPickupDateAsync(startDate.AddYears(-2), endDate.AddYears(-2));
                    var off2Year = values.SelectMany(o => o.Items);

                    var categories = ViewModel.GetProductCategories();

                    var allItems = new List<IEnumerable<OrderItem>> { current, off1Year, off2Year };

                    report = new() { reportGenerator.GetHistoricalProductCategoryTotalsReport(categories, allItems, startDate.Date, endDate.Date) };
                }
            }
            else if(control.ReportType == typeof(YieldStudyReport))
            {
                var datePicker = new SfDatePicker()
                {
                    PlaceholderText = "Production Date..."
                };
                var dialog = new ContentDialog()
                {
                    XamlRoot = XamlRoot,
                    Content = datePicker,
                    PrimaryButtonText = "ok"
                };
                var res = await dialog.ShowAsync();
                if(res == ContentDialogResult.Primary && datePicker.SelectedDate != null)
                {
                    var date = datePicker.SelectedDate.Value;
                    date = new(date.Year, date.Month, date.Day,0,0,0,TimeSpan.FromHours(-4));
                    var items = await ViewModel.GetLiveInventoryItems(date);

                    report = new() { reportGenerator.GetYieldStudyReport(items, date.DateTime) };
                }
            }
            else
            {
                ContentDialog d = new()
                {
                    XamlRoot = XamlRoot,
                    Title = "Error",
                    Content = $"Sorry the person writing ths progam made a mistake because the Report Type \"{control.ReportType}\" " +
                    $"was not found. \r\n Please nicely ask the programmer if they forgot something.",
                    PrimaryButtonText = "ok",
                };
                await d.ShowAsync();
            }

            if (!report.IsNullOrEmpty())
            {
                var filePath = await reportGenerator.GenerateReportPDFAsync(report);
                LauncherOptions options = new()
                {
                    ContentType = "application/pdf"
                };
                _ = Launcher.LaunchUriAsync(new Uri(filePath), options);
            }
            spinner.IsVisible = false;

        }
    }

    private async void CustomerOrderSelectionDialog_CustomerChanged(object? sender, CustomerChangedEventArgs e)
    {
        if(sender is CustomerOrderSelectionDialog dialog && e.Customer != null)
        {
            var orders =  await ViewModel.GetOrdersByCustomerIdAndPickupDateAsync(new List<int>(){e.Customer.CustID}, DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
            dialog.SuggestedOrders.Clear();
            foreach (var order in orders.Where(o => o.OrderStatus == Models.Enums.OrderStatus.Invoiced))
            {
                dialog.SuggestedOrders.Add(order);
            }
        }
    }

    private bool ValidateOrderID(string? value)
    {
        return int.TryParse(value, out _);
    }

    private async Task<(DateTimeOffset?, DateTimeOffset?)> ShowDateRangeSelectionAsync()
    {
        DateRangeSelectionDialog dialog = new()
        {
            XamlRoot = XamlRoot
        };
        var res = await dialog.ShowAsync();
        if( res == ContentDialogResult.Primary)
        {
            if (dialog.StartDate == null || dialog.EndDate == null)
            {
                ContentDialog d = new()
                {
                    XamlRoot = XamlRoot,
                    Title = "Date",
                    Content = $"Please select a date range",
                    PrimaryButtonText = "ok",
                };
                await d.ShowAsync();
            }
            else
            {
                //set the time so we pick up everything within the range
                DateTime sDate = dialog.StartDate.Value.Date;
                DateTime eDate = dialog.EndDate.Value.Date;
                DateTimeOffset startDate = new(sDate.Year, sDate.Month, sDate.Day, 0, 0, 0, 0, new());
                DateTimeOffset endDate = new(eDate.Year, eDate.Month, eDate.Day, 23, 59, 59, 999, new());
                return (startDate, endDate);
            }
        }
        return (null, null);
    }
    #endregion Methods
}
