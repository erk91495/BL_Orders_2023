using BlOrders2023.Models;
using BlOrders2023.ViewModels;
using BlOrders2023.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using BlOrders2023.Exceptions;
using System.Diagnostics;
using System.Media;
using Microsoft.IdentityModel.Tokens;
using BlOrders2023.UserControls;
using Microsoft.UI.Dispatching;
using CommunityToolkit.WinUI;
using Microsoft.EntityFrameworkCore;
using BlOrders2023.Models.Enums;
using BlOrders2023.Reporting;
using BlOrders2023.Services;
using System.Drawing.Printing;
using BlOrders2023.Core.Services;
using Syncfusion.UI.Xaml.DataGrid;
using Microsoft.UI.Xaml.Input;
using NLog;

namespace BlOrders2023.Views;

public sealed partial class FillOrdersPage : Page
{
    #region Fields
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    private readonly ReportGenerator reportGenerator;
    private Task SaveOrderTask = null;
    #endregion Fields
    public FillOrdersPageViewModel ViewModel
    {
        get;
    }

    public FillOrdersPage()
    {
        ViewModel = App.GetService<FillOrdersPageViewModel>();
        InitializeComponent();
        reportGenerator = new(App.CompanyInfo);
        OrderedItems.SelectionController = new FillOrdersGridSelectionController(OrderedItems);
    }

    private async void Scanline_TextChanged(object sender, TextChangedEventArgs args)
    {
        if(sender is TextBox box )
        {

            var scanlineText = box.Text;
            if (scanlineText.Contains('\r'))
            {
                var scanline = scanlineText.Split('\r').First().Trim();
                box.Text = box.Text[(scanline.Length + 1)..];
                if (RemoveItemCheckBox.IsChecked != true)
                {
                    ShippingItem item = new()
                    {
                        QuanRcvd = 1,
                        ScanDate = DateTime.Now,
                        Scanline = scanline,
                        Order = ViewModel.Order,
                    };
                    try
                    {
                        //interpreter has no concept of dbcontext and cannot track items
                        BarcodeInterpreter.ParseBarcode(ref item);
                        //item.Product = product;
                        await AddShippingItemAsync(item);
                    }
                    catch (ProductNotFoundException e)
                    {
                        Debug.WriteLine(e.ToString());
                        var prodCode = e.Data["ProductID"];
                        App.LogWarningMessage($"Product ID: {prodCode} was not found in the database\r\n");
                        await ShowLockedoutDialog("Product Not Found",
                            $"Product ID: {prodCode} was not found in the database\r\n");
                    }
                    catch (InvalidBarcodeExcption e)
                    {
                        Debug.WriteLine(e.ToString());
                        var ai = e.Data["AI"];
                        var s = e.Data["Scanline"];
                        var location = e.Data["Location"];
                        App.LogWarningMessage($"Could not parse scanline {s} at {location}\r\nAI: {ai}");
                        await ShowLockedoutDialog(e.Message,
                            $"Could not parse scanline {s} at {location}\r\nAI: {ai}");
                    }
                    catch (UnknownBarcodeFormatException e)
                    {
                        Debug.WriteLine(e.ToString());
                        App.LogWarningMessage($"{e.Message}");
                        await ShowLockedoutDialog("UnknownBarcodeFormatException", $"{e.Message}");
                    }

                }
                else // remove items is checked
                {
                    var item = ViewModel.Items.Where(i => i.Scanline == scanline).FirstOrDefault();
                    if(item != null)
                    {
                        await ViewModel.DeleteShippingItemAsync(item);
                        await TrySaveOrderAsync();
                        box.Focus(FocusState.Programmatic);
                    }
                    else
                    {
                        await ShowLockedoutDialog("Product Not Found", $"No item with the scanline {scanline} was found on order {ViewModel.Order.OrderID}" +
                            $" Nothing was removed");
                    }
                }
            }
        }
    }

    private async Task AddShippingItemAsync(ShippingItem item)
    {
        try
        {   
            var isFixedBarcode = item.Barcode?.GetType() == typeof(GTIN14Barcode);
            //if(isFixedBarcode)
            //{
            //    item.Scanline += "_" + DateTime.Now.ToString("ddMMMMyyyyHHmmssff");
            //}
            await ViewModel.ReceiveItemAsync(item,!isFixedBarcode);
            //only resize to the first item b/c its slow when i have lots of items O(n)
            if(ViewModel.Order.ShippingItems.Count <= 1)
            {
                OrderedItems.ColumnSizer.ResetAutoCalculationforAllColumns();
                OrderedItems.ColumnSizer.Refresh(); 
            }
            OrderedVsReceivedGrid.View.Refresh();
            if (ViewModel.Order?.OrderStatus <= OrderStatus.Ordered)
            {
                ViewModel.OrderStatus = OrderStatus.Filling;
            }
            if (ViewModel.Order.AllItemsReceived)
            {
                ViewModel.OrderStatus = OrderStatus.Filled;
            }
            await TrySaveOrderAsync();
        }
        catch (DuplicateBarcodeException e)
        {
            Debug.WriteLine(e.ToString());
            var s = e.Data["Scanline"];
            ContentDialog d = new()
            {
                XamlRoot = XamlRoot,
                Title = e.Message,
                Content = $"Duplicate Scanline {s}\r\nWould you like to modify and override?",
                PrimaryButtonText = "Modify",
                SecondaryButtonText = "Cancel",
                DefaultButton = ContentDialogButton.None,

            };
            d.PreviewKeyDown += LockOutKeyPresses;
            SystemSounds.Exclamation.Play();
            var res = await d.ShowAsync();
            if(res == ContentDialogResult.Primary)
            {

                SingleValueInputDialog inputControl = new()
                {
                    XamlRoot = XamlRoot,
                    PrimaryButtonText = "Submit",
                    Prompt = "Enter the new weight",
                    ValidateValue = delegate (string? value)
                    {
                        if (value != null)
                        {
                            if (value.Replace(".",string.Empty).Length <= 6 && float.TryParse(value, out var result))
                            {
                                return true;
                            }
                        }
                        return false;
                    },
                };
                res = await inputControl.ShowAsync();
                if(res == ContentDialogResult.Primary && !inputControl.Value.IsNullOrEmpty())
                {
                    item.PickWeight = float.Parse(inputControl?.Value!);
                    //Add underscore so when an invoice is printed we can see manual overrides
                    item.PackageSerialNumber += '_';

                }
                else
                {
                    //User canceled out
                    return;
                }
                BarcodeInterpreter.UpdateBarcode(ref item);
                await AddShippingItemAsync(item);
            }
        }
    }

    private async Task ShowLockedoutDialog(string title, string content)
    {
        ContentDialog d = new()
        {
            XamlRoot = XamlRoot,
            Title = title,
            Content = content,
            SecondaryButtonText = "Continue",
            DefaultButton = ContentDialogButton.None,
        };
        d.PreviewKeyDown += LockOutKeyPresses;
        SystemSounds.Exclamation.Play();
        await d.ShowAsync();

    }

    private void LockOutKeyPresses(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        e.Handled = true;
    }

    private void OrderLookup_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel.QueryFillableOrders(sender.Text);
        }
        else if (args.Reason == AutoSuggestionBoxTextChangeReason.SuggestionChosen)
        {
            OrderLookup.Text = null;
            ViewModel.QueryFillableOrders(string.Empty);
            
        }
    }

    private async void OrderLookup_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var input = args.QueryText;
        if (!ViewModel.FillableOrdersMasterList.Where(e => e.OrderID.ToString().Equals(input)).IsNullOrEmpty())
        {
            await ViewModel.LoadOrder(int.Parse(input));
            RemoveItemCheckBox.IsChecked = false;
        }
        else if (args.ChosenSuggestion is Order o)
        {
            await ViewModel.LoadOrder(o.OrderID);
            RemoveItemCheckBox.IsChecked = false;
        }
        else
        {
            await ShowLockedoutDialog("Order Not Found", $"No order found for {input}");
        }
        Scanline.Focus(FocusState.Programmatic);
    }

    private async void OrderedItems_RecordDeleted(object sender, Syncfusion.UI.Xaml.DataGrid.RecordDeletedEventArgs e)
    {
        if (e.Items.FirstOrDefault() is ShippingItem)
        {
            foreach (ShippingItem item in e.Items.Cast<ShippingItem>())
            {
                await ViewModel.DeleteShippingItemAsync(item);
                OrderedVsReceivedGrid.View.Refresh();
            }
            await TrySaveOrderAsync();
            OrderedItems.View.Refresh();
        }
    }

    private async void ManualProductAdd_Click(object sender, RoutedEventArgs e)
    {
        ShippingItemDataInputDialog dialog = new(XamlRoot);
        var result = await dialog.ShowAsync();
        if (result != null)
        {
            result.Order = ViewModel.Order;
            await AddShippingItemAsync(result);
        }
    }

    private async void DeleteAll_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new()
        {
            XamlRoot = XamlRoot,
            Title = "Confirm Delete",
            Content = "Are you sure you want to delete all shipping items from this order? These changes cannot be undone.",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
        };
        var result = await dialog.ShowAsync();
        if(result == ContentDialogResult.Primary){
            await ViewModel.DeleteAllShippingItemsAsync();
            await TrySaveOrderAsync();
            OrderedVsReceivedGrid.View.Refresh();
        }
    }

    private void PrintOrderFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        DispatcherQueue.EnqueueAsync(async () => await PrintOrderAsync());
    }

    private void PrintInvoiceFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        DispatcherQueue.EnqueueAsync(async () => await PrintInvoiceAsync());
    }

    private void PrintPalletTicketsFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        DispatcherQueue.EnqueueAsync(async () => await PrintPalletTicketsAsync());
    }

    private async Task PrintInvoiceAsync()
    {
        var printInvoice = false;
        PrinterSettings printSettings = new();
        if (ViewModel.CanPrintInvoice)
        {
            //Invoice already printed print copy
            if (ViewModel.OrderStatus > OrderStatus.Filled)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    XamlRoot = XamlRoot,
                    Content = "This invoice has already been printed. To print a copy press Reprint",
                    PrimaryButtonText = "Reprint",
                    CloseButtonText = "Cancel",
                };
                var res = await contentDialog.ShowAsync();
                if (res == ContentDialogResult.Primary)
                {
                    printInvoice = true;
                    printSettings.Copies = 1;
                }
            }
            //Not all items on order
            else if (!ViewModel.Order.AllItemsReceived)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    XamlRoot = XamlRoot,
                    Content = "All items ordered have not been received. Would you still like to print?",
                    PrimaryButtonText = "Print",
                    CloseButtonText = "Cancel",
                };
                SystemSounds.Asterisk.Play();
                var res = await contentDialog.ShowAsync();
                if (res == ContentDialogResult.Primary)
                {
                    printInvoice = true;
                    printSettings.Copies = 2;
                }
            }
            //Print invoice
            else
            {
                printInvoice = true;
                printSettings.Copies = 2;
            }
        }
        

        if (printInvoice)
        {
            var filePath = reportGenerator.GenerateWholesaleInvoice(ViewModel.Order);

            var printer = new PDFPrinterService(filePath);
            await printer.PrintPdfAsync(printSettings);

            filePath = reportGenerator.GenerateShippingList(ViewModel.Order);
            Windows.System.LauncherOptions options = new()
            {
                ContentType = "application/pdf"
            };
            _ = Windows.System.Launcher.LaunchUriAsync(new Uri(filePath), options);


            if (ViewModel.OrderStatus == OrderStatus.Filling || ViewModel.OrderStatus == OrderStatus.Filled)
            {
                ViewModel.OrderStatus = OrderStatus.Invoiced;
                _ = TrySaveOrderAsync();
            }
        }
    }

    private async Task PrintOrderAsync()
    {
        if (ViewModel.CanPrintOrder)
        {
            if (ViewModel.OrderStatus > OrderStatus.Ordered)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    XamlRoot = XamlRoot,
                    Content = "This Order has already been printed. To print a copy press continue",
                    PrimaryButtonText = "Continue",
                    CloseButtonText = "Cancel",
                };
                var res = await contentDialog.ShowAsync();
                if (res != ContentDialogResult.Primary)
                {
                    return;
                }
            }
            var filePath = reportGenerator.GeneratePickList(ViewModel.Order);

            //Windows.System.LauncherOptions options = new()
            //{
            //    ContentType = "application/pdf"
            //};
            //_ = Windows.System.Launcher.LaunchUriAsync(new Uri(filePath), options);

            PrinterSettings printSettings = new();
            printSettings.Copies = 1;
            var printer = new PDFPrinterService(filePath);
            await printer.PrintPdfAsync(printSettings);

            if (ViewModel.OrderStatus == OrderStatus.Ordered)
            {
                ViewModel.OrderStatus = OrderStatus.Filling;
                _ = TrySaveOrderAsync();
            }
        }
    }

    private async Task PrintPalletTicketsAsync()
    {
        if(ViewModel.Order != null)
        {
            var print = false;
            if(ViewModel.Order.Allocated != true)
            {

                var dialog = new ContentDialog()
                {
                    XamlRoot = XamlRoot,
                    Title = "Print Confirmation",
                    Content = "This order has not yet been allocated. Are you sure you want to print pallet tickets?",
                    PrimaryButtonText = "Print",
                    CloseButtonText = "Cancel",
                };

                var result = await dialog.ShowAsync();
                if(result == ContentDialogResult.Primary)
                {
                    print = true;
                }
            }
            else
            {
                print = true;
            }

            if (print && ViewModel.Order.PalletTicketPrinted)
            {
                var dialog = new ContentDialog()
                {
                    XamlRoot = XamlRoot,
                    Title = "Print Confirmation",
                    Content = "Pallet tickets have already been printed for this order. Do you want to reprint the pallet tickets?",
                    PrimaryButtonText = "Reprint",
                    CloseButtonText = "Cancel",
                };
                var result = await dialog.ShowAsync();
                if (result != ContentDialogResult.Primary)
                {
                    print = false;
                }
            }

            if (print)
            {
                Palletizer palletizer = new(new(), ViewModel.Order);
                var pallets = await palletizer.PalletizeAsync();
                var filePath = reportGenerator.GeneratePalletLoadingReport(ViewModel.Order, pallets);

                PrinterSettings printSettings = new();
                printSettings.Copies = 1;
                printSettings.Duplex = Duplex.Simplex;
                var printer = new PDFPrinterService(filePath);
                await printer.PrintPdfAsync(printSettings);

                ViewModel.Order.PalletTicketPrinted = true;
                await ViewModel.SaveOrderAsync();
            }
        }
    }

    private void OrderLookup_GotFocus(object sender, RoutedEventArgs e)
    {
        OrderLookup.IsSuggestionListOpen = true;
    }

    private void OrderLookup_LostFocus(object sender, RoutedEventArgs e)
    {
        OrderLookup.IsSuggestionListOpen = false;
    }

    private void MemoBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (!ViewModel.HasErrors)
        {
            _ = TrySaveOrderAsync();
        }
        else
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info($"Didn't save memo. Order Had Errors {ViewModel.GetErrors()}");
        }
    }

    private async Task TrySaveOrderAsync()
    {
        try
        {
            await ViewModel.SaveOrderAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _ = ex;
            await ShowLockedoutDialog("Database Write Conflict", $"The Order was modified before your changes could be saved.\r\n" +
                $"Please re-open the Order to get all changes");
        }
        catch (DbUpdateException ex)
        {
            await ShowLockedoutDialog("DbUpdateException", $"An error occured while trying to save your Order. Please contact your system administrator\r\n" +
                $"Details:\r\n{ex.Message}\r\n{ex.InnerException!.Message}");
        }

    }
    private async Task WaitTen()
    {
        await Task.Delay(10000);
    }

    private void RemoveItemCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if(RemoveItemCheckBox.IsChecked == true) 
        {
            Scanline.PlaceholderText = "Scan to remove a product...";
        }
        else
        {
            Scanline.PlaceholderText = "Scan to add a product...";
        }
    }

    private void OrderedItems_CopyGridCellContent(object sender, Syncfusion.UI.Xaml.Grids.GridCopyPasteCellEventArgs e)
    {
        DatagridCellCopy.CopyGridCellContent(sender,e);
    }

    private async void OrderedItems_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
    {
        await TrySaveOrderAsync();
        OrderedVsReceivedGrid.View.Refresh();

    }
}

internal class FillOrdersGridSelectionController : GridSelectionController
{
    public FillOrdersGridSelectionController(SfDataGrid dataGrid) : base(dataGrid)
    {
    }

    KeyRoutedEventArgs keyEventArgs = null;
    public override bool HandleKeyDown(KeyRoutedEventArgs args)
    {

        if (args.Key == Windows.System.VirtualKey.Delete)
        {
            keyEventArgs = args;
            args.Handled = true;
            ContentDialog dialog = new()
            {
                XamlRoot = DataGrid.XamlRoot,
                Title = "Confirm Delete",
                Content = "Are you sure you want to delete the selected shipping item(s) from this order? These changes cannot be undone.",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
            };

            dialog.PrimaryButtonClick += DeleteDialog_PrimaryButtonClick;
            _ = dialog.ShowAsync();
            return false;
        }

        return base.HandleKeyDown(args);

    }

    private void DeleteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        base.HandleKeyDown(keyEventArgs);
    }

}