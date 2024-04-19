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
using System.Security.AccessControl;
using Microsoft.UI.Xaml.Media.Animation;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ReportsPage : Page
{
    private enum PromptTypes
    {
        None,
        OrderID,
        Date,
        DateRange,
        Customer,
        Customers,
        CustomersAndOrders,
        BillOfLading,
        ProductCategories,
        AuditTrail,
    }

    private readonly Dictionary<Type,PromptTypes[]> ReportPromptsMap = new() 
    {
        {typeof(WholesaleInvoice), [PromptTypes.OrderID] },
        {typeof(WholesaleOrderPickupRecap), [PromptTypes.DateRange]},
        {typeof(WholesaleOrderTotals), [PromptTypes.DateRange]},
        {typeof(WholesalePaymentsReport), [PromptTypes.DateRange]},
        {typeof(ShippingList), [PromptTypes.OrderID]},
        {typeof(UnpaidInvoicesReport), [PromptTypes.Customer]},
        {typeof(AggregateInvoiceReport), [PromptTypes.Customers, PromptTypes.DateRange] },
        {typeof(OutstandingBalancesReport), [PromptTypes.None]},
        {typeof(QuarterlySalesReport), [PromptTypes.DateRange]},
        {typeof(FrozenOrdersReport), [PromptTypes.DateRange]},
        {typeof(PickList), [PromptTypes.OrderID]},
        {typeof(PalletLoadingReport), [PromptTypes.OrderID]},
        {typeof(CurrentInventoryReport), [PromptTypes.None]},
        {typeof(InventoryDetailsReport), [PromptTypes.DateRange]},
        {typeof(OutOfStateSalesReport), [PromptTypes.DateRange]},
        {typeof(BillOfLadingReport), [PromptTypes.BillOfLading]},
        {typeof(ShippingItemAuditReport), [PromptTypes.AuditTrail]},
        {typeof(ProductCategoryTotalsReport), [PromptTypes.DateRange]},
        {typeof(ProductCategoryDetailsReport), [PromptTypes.DateRange, PromptTypes.ProductCategories]},
        {typeof(WholesaleInvoiceTotalsReport), [PromptTypes.DateRange, PromptTypes.Customer]},
        {typeof(HistoricalQuarterlySalesReport), [PromptTypes.DateRange]},
        {typeof(HistoricalProductCategoryTotalsReport), [PromptTypes.DateRange]},
        {typeof(YieldStudyReport), [PromptTypes.Date]},
    };



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
            object?[] args = [null];
            if(control.ReportType == typeof(WholesaleInvoice))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(new List<PromptTypes>() {PromptTypes.OrderID,});
                if(!userInputs.Any(i => i == null))
                {
                    int id = (int)userInputs[0];
                    var order = ViewModel.GetOrder(id);
                    if (order != null)
                    {
                        var toTotal = ViewModel.GetTotalsCategories();
                        args= [order, toTotal];
                    }
                }
            }
            else if(control.ReportType == typeof(WholesaleOrderPickupRecap))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(PromptTypes.DateRange);
                if(!userInputs.Any(item => item == null))
                {
                    DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
                    DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
                    
                    var values = ViewModel.GetOrderTotals(startDate, endDate);
                    
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
                    args = [orders, startDate, endDate];
                }
            }
            else if(control.ReportType == typeof(WholesaleOrderTotals))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(PromptTypes.DateRange);
                
                if(!userInputs.Any(item => item == null))
                {
                    DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
                    DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
                    
                    var values = ViewModel.GetOrderTotals(startDate, endDate);
                    args = [values, startDate, endDate];
                }
            }
            else if(control.ReportType == typeof(WholesalePaymentsReport))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(PromptTypes.DateRange);

                if (!userInputs.Any(item => item == null))
                {
                    DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
                    DateTimeOffset endDate = (DateTimeOffset)userInputs[1];

                    var values = ViewModel.GetWholesalePayments(startDate, endDate);
                    args = [values, startDate, endDate];
                }
            }
            else if(control.ReportType == typeof(ShippingList))
            {
                var userInputs = await GetUserInput(PromptTypes.OrderID);
                if(!userInputs.Any(i => i == null))
                {
                    int id = (int)userInputs[0];
                    var order = ViewModel.GetOrder(id);
                    if (order != null)
                    {
                        args = [order];
                    }
                }
            }
            else if(control.ReportType == typeof(UnpaidInvoicesReport))
            {
                spinner.IsVisible = true;
                var res = await GetUserInput(PromptTypes.Customer);
                if (!res.Any(i => i == null))
                {
                    var customer = (WholesaleCustomer) res[0];
                    var values = ViewModel.GetUnpaidInvoicedInvoices(customer);
                    args = [values];
                }
            }
            else if(control.ReportType == typeof(AggregateInvoiceReport))
            {
                var userInputs = await GetUserInput(new List<PromptTypes>() {PromptTypes.Customers, PromptTypes.DateRange});
                if(!userInputs.Any(i => i == null)) {
                    DateTimeOffset startDate = (DateTimeOffset)userInputs[1];
                    DateTimeOffset endDate = (DateTimeOffset)userInputs[2];
                    IEnumerable<int> ids = ((List<WholesaleCustomer>)userInputs[0]).Select(c => c.CustID);
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
                    args = [values, startDate, endDate];
                }
            }
            else if(control.ReportType == typeof(OutstandingBalancesReport))
            {
                spinner.IsVisible = true;
                args = [ await ViewModel.GetOutstandingOrdersAsync() ];
            }
            else if(control.ReportType == typeof(QuarterlySalesReport))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(PromptTypes.DateRange);
                if (!userInputs.Any(i=> i ==null))
                {
                    DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
                    DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
                    var values = await ViewModel.GetProductTotalsAsync(startDate, endDate);
                    args = [ values, startDate, endDate ];
                }
            }
            else if(control.ReportType == typeof(FrozenOrdersReport))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(PromptTypes.DateRange);
                if (!userInputs.Any(i => i == null))
                {
                    DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
                    DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
                    var values = ViewModel.GetFrozenOrders(startDate, endDate);
                    args = [ values, startDate, endDate ];
                }
            }
            else if (control.ReportType == typeof(PickList))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(PromptTypes.OrderID);
                if(!userInputs.Any(i => i == null))
                {
                    var order = await ViewModel.GetOrderAsync((int)userInputs[0]);
                    if (order != null)
                    {
                        args = [ order ];
                    }
                } 
            }
            else if(control.ReportType == typeof(PalletLoadingReport))
            {
                var userInputs = await GetUserInput(PromptTypes.OrderID);
                if (!userInputs.Any(i => i == null))
                {
                    spinner.IsVisible = true;
                    var id = (int)userInputs[0];
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

                            var result = await printDialog.ShowAsync();
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
                            args = [order, pallets];
                        }
                    }                    
                }
            }
            else if( control.ReportType == typeof(CurrentInventoryReport))
            {
                spinner.IsVisible = true;
                args = [ ViewModel.GetInventoryTotals() ];
            }
            else if(control.ReportType == typeof(InventoryDetailsReport))
            {
                var userInput = await GetUserInput(PromptTypes.DateRange);

                if(!userInput.Any(i => i == null))
                {
                    DateTimeOffset startDate = (DateTimeOffset)userInput[0];
                    DateTimeOffset endDate = (DateTimeOffset)userInput[1];
                    spinner.IsVisible = true;
                    var orders = ViewModel.GetNonFrozenOrdersByPickupDate(startDate, endDate);
                    var currentInventory = ViewModel.GetInventoryTotals();
                    var notFilled = ViewModel.GetAllocatedNotReceivedTotals();
                    args = [currentInventory, orders, notFilled, startDate, endDate];
                }

            }
            else if(control.ReportType == typeof(OutOfStateSalesReport))
            {
                var userInput = await GetUserInput(PromptTypes.DateRange);

                if (!userInput.Any(i => i == null))
                {
                    DateTimeOffset startDate = (DateTimeOffset)userInput[0];
                    DateTimeOffset endDate = (DateTimeOffset)userInput[1];
                    spinner.IsVisible = true;
                    var orders = ViewModel.GetOutOfStateOrders(startDate, endDate);
                    var currentInventory = ViewModel.GetInventory();
                    args = [orders, startDate, endDate];
                }
            }
            else if(control.ReportType == typeof(BillOfLadingReport))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(PromptTypes.BillOfLading);
                
                if(!userInputs.Any(i => i == null))
                {
                    args = userInputs;
                }

            }
            else if(control.ReportType == typeof(ShippingItemAuditReport))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(PromptTypes.AuditTrail);

                if (!userInputs.Any(i => i == null))
                {
                    args = userInputs;
                }

            }
            else if(control.ReportType == typeof(ProductCategoryTotalsReport))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(PromptTypes.DateRange);

                if (!userInputs.Any( i => i  == null))
                {
                    DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
                    DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
                    var values = await ViewModel.GetOrdersByPickupDateAsync(startDate, endDate);
                    var orderItems = values.SelectMany(o => o.Items);
                    if(!orderItems.IsNullOrEmpty())
                    {
                        args = [orderItems, startDate, endDate];
                    }
                    
                }
            }
            else if (control.ReportType == typeof(ProductCategoryDetailsReport))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(new List<PromptTypes>() {PromptTypes.DateRange, PromptTypes.ProductCategories });

                if (!userInputs.Any(i => i == null))
                {
                    DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
                    DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
                    var items = (List<ProductCategory>)userInputs[2];
                    List<ProductCategory> categories = items.ToList();
                    
                    var values = await ViewModel.GetOrdersByPickupDateAsync(startDate, endDate);
                    var orderItems = values.SelectMany(o => o.Items.Where(i => categories.Contains(i.Product.Category)));
                    if (!orderItems.IsNullOrEmpty())
                    {
                        var products = ViewModel.GetProducts();
                         args = [orderItems, products, startDate, endDate];

                    }
                    


                }
            }
            else if(control.ReportType == typeof(WholesaleInvoiceTotalsReport))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(new List<PromptTypes>() {PromptTypes.DateRange, PromptTypes.Customer });

                if (!userInputs.Any(i => i == null))
                {
                    DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
                    DateTimeOffset endDate = (DateTimeOffset)userInputs[1];
                    WholesaleCustomer customer = (WholesaleCustomer)userInputs[2];
                   
                        List<int> ids = new List<int>(){ customer.CustID };
                        var orders = await ViewModel.GetOrdersByCustomerIdAndPickupDateAsync(ids, startDate, endDate);
                        if(!orders.IsNullOrEmpty())
                        {
                            
                            args = [customer, orders, startDate, endDate];
                        }                   
                }
            }
            else if (control.ReportType == typeof(HistoricalQuarterlySalesReport))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(PromptTypes.DateRange);

                if (!userInputs.Any(i => i == null))
                {
                    DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
                    DateTimeOffset endDate = (DateTimeOffset)userInputs[1];

                    var current = await ViewModel.GetProductTotalsAsync(startDate, endDate);
                    var off1Year = await ViewModel.GetProductTotalsAsync(startDate.AddYears(-1), endDate.AddYears(-1));
                    var off2Year = await ViewModel.GetProductTotalsAsync(startDate.AddYears(-2), endDate.AddYears(-2));

                    var allItems = new List<IEnumerable<ProductTotalsItem>> { current, off1Year, off2Year };

                    args = [allItems, startDate, endDate];
                }
            }
            else if(control.ReportType == typeof(HistoricalProductCategoryTotalsReport))
            {
                spinner.IsVisible = true;
                var userInputs = await GetUserInput(PromptTypes.DateRange);

                if (!userInputs.Any(i => i == null))
                {
                    DateTimeOffset startDate = (DateTimeOffset)userInputs[0];
                    DateTimeOffset endDate = (DateTimeOffset)userInputs[1];

                    var values = await ViewModel.GetOrdersByPickupDateAsync(startDate, endDate);
                    var current = values.SelectMany(o => o.Items);

                    values = await ViewModel.GetOrdersByPickupDateAsync(startDate.AddYears(-1), endDate.AddYears(-1));
                    var off1Year = values.SelectMany(o => o.Items);

                    values = await ViewModel.GetOrdersByPickupDateAsync(startDate.AddYears(-2), endDate.AddYears(-2));
                    var off2Year = values.SelectMany(o => o.Items);

                    var categories = ViewModel.GetProductCategories();

                    var allItems = new List<IEnumerable<OrderItem>> { current, off1Year, off2Year };

                    args = [categories, allItems, startDate, endDate];
                }
            }
            else if(control.ReportType == typeof(YieldStudyReport))
            {
                var userInput = await GetUserInput(PromptTypes.Date);
                if(!userInput.Any(i => i == null))
                {
                    DateTimeOffset date = (DateTimeOffset)userInput[0];
                    var items = await ViewModel.GetLiveInventoryItems(date);
                    args = [items, date.DateTime];
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

            if (!args.Any(i => i == null))
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
}
