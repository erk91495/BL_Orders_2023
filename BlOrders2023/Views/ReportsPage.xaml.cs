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
using BlOrders2023.ViewModels;
using Microsoft.Graphics.Canvas.Text;
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
    #endregion Properties

    #region Fields
    private readonly ReportGenerator reportGenerator;
    #endregion Fields

    #region Constructors
    public ReportsPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<ReportsPageViewModel>();
        reportGenerator = new();


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
            var reportPath = string.Empty;
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
                        var order = ViewModel.GetOrder(id);
                        if (order != null)
                        {
                            reportPath = reportGenerator.GenerateWholesaleInvoice(order);
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
                    var values = ViewModel.GetOrderTotals(startDate, endDate);
                    reportPath = reportGenerator.GenerateWholesaleOrderTotals(values, startDate, endDate);
                    
                    ContentDialog d = new()
                    {
                        XamlRoot = XamlRoot,
                        Title = "Wholesale Order Pickup Recap",
                        Content = "Would you like that by date or alphabetical?",
                        PrimaryButtonText = "Date",
                        SecondaryButtonText = "Alphabetical",
                    };
                    var res = await d.ShowAsync();


                    var orders = res == ContentDialogResult.Primary ? ViewModel.GetOrdersByPickupDate(startDate,endDate) : ViewModel.GetOrdersByPickupDateThenName(startDate, endDate);
                    reportPath = reportGenerator.GenerateWholesaleOrderPickupRecap(orders, startDate, endDate);
                }
            }
            else if(control.ReportType == typeof(WholesaleOrderTotals))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();
                
                if(dateTuple.Item1 != null && dateTuple.Item2 != null) 
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    var values = ViewModel.GetOrderTotals(startDate, endDate);
                    reportPath = reportGenerator.GenerateWholesaleOrderTotals(values, startDate, endDate);
                }
            }
            else if(control.ReportType == typeof(WholesalePaymentsReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    var values = ViewModel.GetWholesalePayments(startDate, endDate);
                    reportPath = reportGenerator.GenerateWholesalePaymentsReport(values, startDate, endDate);
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
                            reportPath = reportGenerator.GenerateShippingList(order);
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
                    customer = custDialog.ViewModel.SelectedCustomer;
                    var values = ViewModel.GetUnpaidInvoices(customer);
                    reportPath = reportGenerator.GenerateUnpaidInvoicesReport(values);
                    
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
                            reportPath = reportGenerator.GenerateAggregateInvoiceReport(values, startDate, endDate);
                        }

                    }
                }
            }
            else if(control.ReportType == typeof(OutstandingBalancesReport))
            {
                var values = await ViewModel.GetOutstandingOrdersAsync();
                reportPath = reportGenerator.GenerateOutstandingBalancesReport(values);
            }
            else if(control.ReportType == typeof(QuarterlySalesReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    var values = ViewModel.GetOrdersByPickupDate(startDate, endDate);
                    reportPath = reportGenerator.GenerateQuarterlySalesReport(values, startDate, endDate);
                }
            }
            else if(control.ReportType == typeof(FrozenOrdersReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    var values = ViewModel.GetFrozenOrders(startDate, endDate);
                    reportPath = reportGenerator.GenerateFrozenOrdersReport(values, startDate, endDate);
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
                        var order = ViewModel.GetOrder(id);
                        if (order != null)
                        {
                            reportPath = reportGenerator.GeneratePickList(order);
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
                        var order = ViewModel.GetOrder(id);
                        if (order != null)
                        {
                            Palletizer palletizer = new Palletizer(new PalletizerConfig(), order);
                            IEnumerable<Pallet> pallets = await palletizer.PalletizeAsync();
                            reportPath = reportGenerator.GeneratePalletLoadingReport(order, pallets);
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
                var currentInventory = ViewModel.GetInventory();
                reportPath = reportGenerator.GenerateCurrentInventoryReport(currentInventory);
            }
            else if(control.ReportType == typeof(InventoryDetailsReport))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    var orders = ViewModel.GetNonFrozenOrdersByPickupDate(startDate, endDate);
                    var currentInventory = ViewModel.GetInventory();
                    reportPath = reportGenerator.GenerateInventoryDetailsReport(currentInventory, orders, startDate, endDate);
                }

            }
            else
            {
                ContentDialog d = new()
                {
                    XamlRoot = XamlRoot,
                    Title = "Error",
                    Content = $"Sorry the person writing ths progam made a mistake because the Report Type \"{control.ReportType}\" " +
                    $"was not found. \r\n Please ask the programmer nicely if they forgot something.",
                    PrimaryButtonText = "ok",
                };
                await d.ShowAsync();
            }

            if (reportPath != string.Empty)
            {
                LauncherOptions options = new()
                {
                    ContentType = "application/pdf"
                };
                _ = Launcher.LaunchUriAsync(new Uri(reportPath), options);
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
                    Title = "Error",
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
