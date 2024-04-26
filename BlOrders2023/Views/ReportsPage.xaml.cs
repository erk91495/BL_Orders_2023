// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.ObjectModel;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Core.Services;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using BlOrders2023.UserControls;
using BlOrders2023.Dialogs;
using BlOrders2023.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.Data;
using Windows.System;
using BlOrders2023.Core.Contracts.Services;
using Syncfusion.UI.Xaml.Editors;
using static BlOrders2023.Models.ReportPrompts;
using System.Reflection.Metadata;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Data;
using System.Diagnostics;

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
    public bool IsSpinnerVisible { get; set; }
    #endregion Properties

    #region Fields
    private readonly ReportGenerator reportGenerator;
    private ObservableCollection<ReportViewBase> ReportsList { get; set; }
    private ObservableCollection<ReportGroup> Grouped { get; set; } = new();
    #endregion Fields

    #region Constructors
    public ReportsPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<ReportsPageViewModel>();
        reportGenerator = new(App.CompanyInfo);
        InitializeReports();

    }

    private void InitializeReports()
    {
            ReportsList = new();
            foreach (Type t in Reports.AvalibleReports)
            {
                var constructedType = typeof(ReportViewModel<>).MakeGenericType([t]);
                var control = (ReportViewBase)Activator.CreateInstance(constructedType);
                ReportsList.Add(control);
            }

            foreach(var category in ReportsList.Select(i => i.ReportCategory).Distinct())
            {
                var group = new ReportGroup() {Category = category};
                group.Items = ReportsList.Where(i => i.ReportCategory == category).OrderBy(r => r.ReportName).ToObservableCollection();
                Grouped.Add(group);
            }
    }
    #endregion Constructors

    #region Methods
    private async Task<object?[]> GetUserInput(IEnumerable<PromptTypes> propmpts)
    {
        List<object?> results = new();
        foreach (var prompt in propmpts)
        {
            var result = await GetUserInput(prompt);
            if(results.Any(x => x == null))
            {
                return [null];
            }
            else
            {
                foreach(var item in result)
                {
                    results.Add(item);
                }
            }
        }
        return results.ToArray();
    } 

    private async Task<object?[]> GetUserInput(PromptTypes prompt)
    {
        switch (prompt)
        {
            case PromptTypes.OrderID:
                return [await ShowOrderIDInputDialog()];
            case PromptTypes.Date:
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
                    if (res == ContentDialogResult.Primary && datePicker.SelectedDate != null)
                    {
                        var date = datePicker.SelectedDate.Value;
                        date = new(date.Year, date.Month, date.Day, 0, 0, 0, TimeSpan.FromHours(-4));
                        return [date];
                    }
                    return [null];
                }
            case PromptTypes.DateRange:
                return Array.ConvertAll(await ShowDateRangeSelectionAsync(), item => (object?)item);
            case PromptTypes.Customer:
                {
                    CustomerSelectionDialog custDialog = new(XamlRoot)
                    {
                        PrimaryButtonText = "Select Customer",
                        SecondaryButtonText = ""
                    };
                    var res = await custDialog.ShowAsync();
                    if (res == ContentDialogResult.Primary && custDialog.ViewModel != null && custDialog.ViewModel.SelectedCustomer != null)
                    {
                        return [custDialog.ViewModel.SelectedCustomer];
                    }
                    return[null];
                }
            case PromptTypes.Customers:
                {
                    MultipleCustomerSelectionDialog dialog = new(XamlRoot);
                    var res = await dialog.ShowAsync();
                    if (res == ContentDialogResult.Primary)
                    {
                        if (dialog.Customers.IsNullOrEmpty())
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
                            return [dialog.Customers];
                        }
                    }
                    return [null];
                }
            case PromptTypes.CustomersAndOrders:
                {
                    var customers = ViewModel.GetWholesaleCustomers();
                    var dialog = new CustomerOrderSelectionDialog(customers)
                    {
                        XamlRoot = XamlRoot
                    };
                    dialog.CustomerChanged += CustomerOrderSelectionDialog_CustomerChanged;
                    var res = await dialog.ShowAsync();
                    if (res == ContentDialogResult.Primary)
                    {
                        var orders = dialog.SelectedOrders;
                        var customer = dialog.SelectedCustomer;
                        return [orders, customer];
                    }
                    return [null];
                }
            case PromptTypes.BillOfLading:
                {
                    var customers = ViewModel.GetWholesaleCustomers();
                    var dialog = new CustomerOrderSelectionDialog(customers)
                    {
                        XamlRoot = XamlRoot
                    };
                    dialog.CustomerChanged += CustomerOrderSelectionDialog_CustomerChanged;
                    var res = await dialog.ShowAsync();
                    if (res == ContentDialogResult.Primary)
                    {
                        var orders = dialog.SelectedOrders;
                        var customer = dialog.SelectedCustomer;
                        var billOfLadingInput = new BillOfLadingDataInputDialog(orders)
                        {
                            XamlRoot = XamlRoot
                        };
                        res = await billOfLadingInput.ShowAsync();
                        if (res == ContentDialogResult.Primary)
                        {
                            var items = billOfLadingInput.Items;
                            var carrier = billOfLadingInput.CarrierName;
                            var trailerNumber = billOfLadingInput.TrailerNumber;
                            var trailerSeal = billOfLadingInput.TrailerSeal;
                            DateTime? appointmentDate = null;
                            if (billOfLadingInput.AppointmentDate != null && billOfLadingInput.AppointmentTime != null)
                            {
                                var inputDate = billOfLadingInput.AppointmentDate.Value;
                                var inputTime = billOfLadingInput.AppointmentTime.Value;
                                appointmentDate = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day, inputTime.Hour, inputTime.Minute, inputTime.Second);
                            }
                            return [orders, items, customer, carrier, trailerNumber, trailerSeal, appointmentDate];
                        }
                    }
                    return [null];
                }
            case PromptTypes.ProductCategories:
                {
                    var categories = ViewModel.GetProductCategories();
                    var categorySelect = new MultiSelectListBox(categories.Cast<object>().ToObservableCollection());
                    ContentDialog diag = new()
                    {
                        Title = "Select Categories",
                        XamlRoot = XamlRoot,
                        Content = categorySelect,
                        PrimaryButtonText = "Next",
                        CloseButtonText = "Cancel",
                    };
                    if (await diag.ShowAsync() == ContentDialogResult.Primary)
                    {
                        return[categories.Where(i => categorySelect.SelectedItems.Contains(i.CategoryName)).ToList()];
                    }
                    return[null];
                }
            case PromptTypes.AuditTrail:
                {
                    var dialog = new AuditDataInputDialog() { XamlRoot = XamlRoot };
                    var res = await dialog.ShowAsync();
                    if (res == ContentDialogResult.Primary)
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
                        if (item != null)
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
                            var items = ViewModel.GetShippingItems(item, fieldsToMatch.Contains("ProductID"), fieldsToMatch.Contains("Serial"),
                                fieldsToMatch.Contains("PackDate"), fieldsToMatch.Contains("Scanline"), startDate, endDate);
                            if (!items.IsNullOrEmpty())
                            {
                                return [items, item, fieldsToMatch, startDate, endDate];
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
                    return [null];
                }
            case PromptTypes.OrderByDateOrAlphabetical:
                {
                    ContentDialog d = new()
                    {
                        XamlRoot = XamlRoot,
                        Title = "Format",
                        Content = "Would you like that by date or alphabetical?",
                        PrimaryButtonText = "Date",
                        SecondaryButtonText = "Alphabetical",
                    };
                    var res = await d.ShowAsync();
                    return [res == ContentDialogResult.Primary];
                }
            default:
                return [null];
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

    private async Task<int?> ShowOrderIDInputDialog()
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
            return null;
            }
            else
            {
                return id;
            }
        }
        return null;
    }

    private async Task<DateTimeOffset?[]> ShowDateRangeSelectionAsync()
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
                return [startDate, endDate];
            }
        }
        return [null,null];
    }
    #endregion Methods

    private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is ReportViewBase control)
        {
            var prompts = control.Prompts;
            var userInputs = await GetUserInput(prompts);
            object?[]? args = null;
            if (!(userInputs.Any(i => i == null) && prompts.FirstOrDefault() != PromptTypes.None))
            {
                args = await control.GetData(userInputs);
            }
            if (!(args.IsNullOrEmpty() || args.Any(i => i == null)))
            {
                var report = reportGenerator.GetReport(control.ReportType, args);
                var filePath = await reportGenerator.GenerateReportPDFAsync(report);
                LauncherOptions options = new()
                {
                    ContentType = "application/pdf"
                };
                _ = Launcher.LaunchUriAsync(new Uri(filePath), options);
            }
            else
            {
                ContentDialog dialog = new ContentDialog()
                {
                    XamlRoot = XamlRoot,
                    Title = "Error",
                    Content = "No data was found for the given input",
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
            spinner.IsVisible = false;

        }
    }
}
