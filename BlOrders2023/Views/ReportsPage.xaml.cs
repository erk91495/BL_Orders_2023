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
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using BlOrders2023.UserControls;
using BlOrders2023.ViewModels;
using Microsoft.Graphics.Canvas.Text;
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
    #endregion Fields

    #region Constructors
    public ReportsPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<ReportsPageViewModel>();


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
            string reportPath = string.Empty;
            if(control.ReportType == typeof(WholesaleInvoice))
            {
                SingleValueInputDialog dialog = new SingleValueInputDialog()
                {
                    Title = "Order ID",
                    Prompt = "Please Enter An Order ID",
                    XamlRoot = XamlRoot,
                    PrimaryButtonText = "Submit",
                    ValidateValue = ValidateOrderID,
                };
                await dialog.ShowAsync();
                var res = int.TryParse(dialog.Value ?? "", out int id);
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
                        reportPath = ReportGenerator.GenerateWholesaleInvoice(order);
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
            else if(control.ReportType == typeof(WholesaleOrderPickupRecap))
            {
                var dateTuple = await ShowDateRangeSelectionAsync();

                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    DateTimeOffset startDate = (DateTimeOffset)dateTuple.Item1;
                    DateTimeOffset endDate = (DateTimeOffset)dateTuple.Item2;
                    var values = ViewModel.GetOrderTotals(startDate, endDate);
                    reportPath = ReportGenerator.GenerateWholesaleOrderTotals(values, startDate, endDate);
                    
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
                    reportPath = ReportGenerator.GenerateWholesaleOrderPickupRecap(orders, startDate, endDate);
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
                    reportPath = ReportGenerator.GenerateWholesaleOrderTotals(values, startDate, endDate);
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
                    reportPath = ReportGenerator.GenerateWholesalePaymentsReport(values, startDate, endDate);
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
                    ValidateValue = ValidateOrderID,
                };
                await dialog.ShowAsync();
                var res = int.TryParse(dialog.Value ?? "", out int id);
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
                        reportPath = ReportGenerator.GenerateShippingList(order);
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
            else
            {
                throw new Exception("Report Type Not Found ask the programmer if they forgot something");
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
        return int.TryParse(value, out int id);
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
                return (dialog.StartDate, dialog.EndDate);
            }
        }
        return (null, null);
    }
    #endregion Methods
}
