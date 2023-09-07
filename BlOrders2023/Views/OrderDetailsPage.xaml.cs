// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BlOrders2023.Models;
using BlOrders2023.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using BlOrders2023.Models.Enums;
using Windows.Globalization.NumberFormatting;
using Syncfusion.UI.Xaml.DataGrid;
using System.Reflection;
using Syncfusion.UI.Xaml.Data;
using Syncfusion.UI.Xaml.Grids.ScrollAxis;
using BlOrders2023.Contracts.Services;
using BlOrders2023.Services;
using Microsoft.UI.Dispatching;
using System.Media;
using Microsoft.EntityFrameworkCore;
using BlOrders2023.Reporting;
using System.Drawing.Printing;
using BlOrders2023.UserControls;
using CommunityToolkit.WinUI;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views;

/// <summary>
/// A page for viewing and editing Orders
/// </summary>
public sealed partial class OrderDetailsPage : Page
{
    #region Properties
    /// <summary>
    /// The viewmodel for the Order details page
    /// </summary>
    public OrderDetailsPageViewModel ViewModel { get; }
    #endregion Properties

    #region Fields
    private OrderItem? _doomed;
    private bool _deleteOrder;
    private bool _canLeave = false;
    private readonly ReportGenerator reportGenerator;
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Initializes an instance of the OrderDetails Page
    /// </summary>
    public OrderDetailsPage()
    {
        ViewModel = App.GetService<OrderDetailsPageViewModel>();
        reportGenerator = new();
        this.InitializeComponent();
        SetMemoTotalFormatter();
        //SetMemoWeightFormatter();
        PickupTime.MinTime = new DateTime(1800, 1, 1, 0, 0, 0, 0);
        OrderedItems.PreviewKeyDown += OrderedItems_PreviewKeyDown;
        this.DataContext = this;
    }

    #endregion Constructors
    #region Methods
    /// <summary>
    /// Handles NavigatedTo events
    /// </summary>
    /// <param name="e">the navigation envent args</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
    }
    /// <summary>
    /// Handles NavigatingFrom events
    /// </summary>
    /// <param name="e">the navigation envent args</param>
    protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        if (!_canLeave)
        {
            e.Cancel = true;
            if (_deleteOrder)
            {
                ViewModel.DeleteCurrentOrder();
                _canLeave = true;
                e.Cancel = false;
                Frame.Navigate(e.SourcePageType, e.Parameter);
            }
            else
            {
                if(await TrySaveCurrentOrderAsync())
                {
                    //Order was saved or discarded 
                    _canLeave = true;
                    e.Cancel = false;
                    Frame.Navigate(e.SourcePageType, e.Parameter);
                }
            }
        }
        base.OnNavigatingFrom(e);
    }

    /// <summary>
    /// Attempts to focus the last row and edit the Quantity ordered Column
    /// </summary>
    private void FocusEditLastCell()
    {
        var res = OrderedItems.Focus(FocusState.Keyboard);

        //TODO: Handle Empty row
        var rowIndex = OrderedItems.ResolveToRowIndex(ViewModel.Items.Last());
        var columnIndex = OrderedItems.Columns.IndexOf(OrderedItems.Columns.FirstOrDefault(c => c.HeaderText.ToString() == "Quantity Ordered"));
        var rowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);
        OrderedItems.MoveCurrentCell(rowColumnIndex);
        res = OrderedItems.SelectionController.CurrentCellManager.BeginEdit();
    }

    /// <summary>
    /// Sets the Formatter for the MemoTotal field
    /// </summary>
    private void SetMemoTotalFormatter()
    {
        IncrementNumberRounder rounder = new()
        {
            Increment = 0.01,
            RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
        };

        DecimalFormatter formatter = new()
        {
            IntegerDigits = 1,
            FractionDigits = 2,
            NumberRounder = rounder
        };
        MemoTotal.NumberFormatter = formatter;
    }

    /// <summary>
    /// Sets the Formatter for the MemoWeight field
    /// </summary>
    //private void SetMemoWeightFormatter()
    //{
    //    IncrementNumberRounder rounder = new()
    //    {
    //        Increment = 0.01,
    //        RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
    //    };

    //    DecimalFormatter formatter = new()
    //    {
    //        IntegerDigits = 1,
    //        FractionDigits = 2,
    //        NumberRounder = rounder
    //    };
    //    MemoWeight.NumberFormatter = formatter;
    //}

    public async Task<bool> TrySaveCurrentOrderAsync()
    {
        //change focus to write any changes
        OrderNumber.Focus(FocusState.Programmatic);
        ViewModel.ValidateProperties();
        if (!ViewModel.HasErrors)
        {
            try
            {
                ViewModel.SaveCurrentOrder();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _ = ex;
                ContentDialog dialog = new()
                {
                    Title = "Database Write Conflict",
                    Content = $"The order was modified before your changes could be saved.\r\n" +
                    $"What would you like to do with your changes?",
                    XamlRoot = this.XamlRoot,
                    PrimaryButtonText = "Overwrite",
                    SecondaryButtonText = "Discard",
                    CloseButtonText = "Cancel",
                };
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    ViewModel.SaveCurrentOrder(true);
                    return true;
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    ViewModel.ReloadOrder();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (DbUpdateException ex)
            {
                ContentDialog dialog = new()
                {
                    Title = "DbUpdateException",
                    Content = $"An error occured while trying to save your order. Please contact your system administrator\r\n" +
                    $"Details:\r\n{ex.Message}\r\n{ex.InnerException!.Message}",
                    XamlRoot = this.XamlRoot,
                    PrimaryButtonText = "Ok",
                };
                await dialog.ShowAsync();
                return false;
            }
        }
        else
        {
            List<string?> errorMessages = new();
            foreach (var error in ViewModel.GetErrors())
            {
                errorMessages.Add(error.ErrorMessage);
            }
            ContentDialog dialog = new()
            {
                Title = "Input Error",
                Content = $"Please correct all errors before saving the order\r\n\r\nErrors:\r\n{string.Join("\r\n", errorMessages)}",
                XamlRoot = this.XamlRoot,
                PrimaryButtonText = "Ok",
            };
            await dialog.ShowAsync();
            return false;
        }
    }

    #region Events Handlers
    /// <summary>
    /// Handles click events for the email hyperlink button. Creates a mailto: event
    /// </summary>
    /// <param name="sender">the hplerlink button that was clicked</param>
    /// <param name="e">the click event args</param>
    private async void EmailButton_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.Customer.Email != null)
            await Helpers.Helpers.SendEmailAsync(ViewModel.Customer.Email);
    }

    /// <summary>
    /// Handles text changed events for the Product entry box. Requeries the database for _products matching the given input
    /// </summary>
    /// <param name="sender">the product entry autosuggestbox</param>
    /// <param name="args">the text changed event args</param>
    private void ProductEntryBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            _ = ViewModel.QueryProducts(sender.Text);
        }
        else if (args.Reason == AutoSuggestionBoxTextChangeReason.SuggestionChosen)
        {
            
        }
    }
    /// <summary>
    /// Handles Selection changed for the shipping type radio buttons. Hides or shows the pickup time timepicker
    /// </summary>
    /// <param name="sender">A radio button</param>
    /// <param name="e">the selection changed event args</param>
    private void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        if (RadioPickUp.IsChecked == true)
        {
            ViewModel.Shipping = ShippingType.Pickup;
            PickupTimeStack.Visibility = Visibility.Visible;
        }
        else
        {
            ViewModel.Shipping = ShippingType.Delivery;
            ViewModel.PickupTime = new(ViewModel.PickupTime.Year, ViewModel.PickupTime.Month, ViewModel.PickupTime.Day, 0, 0, 0, 0);
            PickupTimeStack.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Deletes the given row from the datagrid
    /// </summary>
    /// <param name="sender">a delete button</param>
    /// <param name="e">the event args for the click event</param>
    private void DeleteRow_Click(object sender, RoutedEventArgs e)
    {
        if (_doomed != null)
        {
            if(_doomed.QuantityReceived == 0)
            {
                ViewModel.Items.Remove(_doomed);
            }
            else
            {
                ContentDialog dialog = new ContentDialog()
                {
                    XamlRoot = XamlRoot,
                    Title = "Error",
                    Content = "Quantity received must be 0 before this item can be removed",
                    PrimaryButtonText = "Ok",
                };
                _ = dialog.ShowAsync();
            }
        }
        _doomed = null;
    }

    /// <summary>
    /// Tracks pointer movements so we know which item to delete from the datagrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OrderedItems_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (OrderedItems != null)
        {
            var visualcontainer = OrderedItems.GetType()?.GetProperty("VisualContainer", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(OrderedItems) as VisualContainer;
            if (visualcontainer == null)
            {
                return;
            }

            var point = e.GetCurrentPoint(visualcontainer).Position;
            var rowColumnIndex = visualcontainer.PointToCellRowColumnIndex(point);
            var recordIndex = OrderedItems.ResolveToRecordIndex(rowColumnIndex.RowIndex);
            if (recordIndex < 0)
            {
                return;
            }

            //When the rowindex is zero , the row will be header row
            if (!rowColumnIndex.IsEmpty)
            {
                if (OrderedItems.View.TopLevelGroup != null)
                {
                    // Get the current row record while grouping
                    var record = OrderedItems.View.TopLevelGroup.DisplayElements[recordIndex];
                    if (record is RecordEntry r)
                    {
                        _doomed = r.Data as OrderItem;
                    }
                }
                else
                {
                    //Gets the column from ColumnsCollection by resolving the corresponding column index from  GridVisibleColumnIndex                      
                    //var gridColumn = OrderedItems.Columns[OrderedItems.ResolveToGridVisibleColumnIndex(rowColumnIndex.ColumnIndex)];
                    //For getting the record, need to resolve the corresponding record index from row index                     
                    _doomed = OrderedItems.View.Records[OrderedItems.ResolveToRecordIndex(rowColumnIndex.RowIndex)].Data as OrderItem;
                }

            }
        }
    }

    private async void DeleteOrderFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new()
        {
            Title = "Delete Order",
            Content = "Are you sure you want to delete this entire Order.\r\nThis action cannot be undone.",
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Delete Order",
            IsPrimaryButtonEnabled = true,
            XamlRoot= XamlRoot,
        };

        var res = await dialog.ShowAsync();
        if (res == ContentDialogResult.Primary)
        {
            _deleteOrder = true;
            if (App.GetService<INavigationService>() is NavigationService nav)
            {
                if (nav.CanGoBack)
                {
                    nav.GoBack();
                }
                else
                {
                    Frame.Navigate(typeof(OrdersPage));
                }
            }
        }
    }

    private async void SaveOrderFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        _ = await TrySaveCurrentOrderAsync();
    }

    /// <summary>
    /// Called when a key is pressed down on the datagrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OrderedItems_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if(e.Key == Windows.System.VirtualKey.Enter)
        {
            var rowindex = OrderedItems.SelectionController.CurrentCellManager.CurrentCell.RowIndex;
            var columnIndex = OrderedItems.SelectionController.CurrentCellManager.CurrentCell.ColumnIndex;
            if (columnIndex < OrderedItems.Columns.Count -1)
            {
                var rowColumnIndex = new RowColumnIndex(rowindex, columnIndex + 1);
                //OrderedItems.MoveCurrentCell(rowColumnIndex);
                //Dont need Count -1 becuase there is a header row
            }else if (rowindex < ViewModel.Items.Count)
            {
                var rowColumnIndex = new RowColumnIndex(rowindex + 1, 2);
                //OrderedItems.MoveCurrentCell(rowColumnIndex);
            }
        }
        if(e.Key == Windows.System.VirtualKey.Tab)
        {
            //OrderedItems.ClearSelections(false);    
            //DispatcherQueue.TryEnqueue(() => { ProductEntryBox.Focus(FocusState.Programmatic); });
            //var res = ProductEntryBox.Focus(FocusState.Programmatic);
        }
    }

    /// <summary>
    /// This event gets fired when:
    ///     * a user presses Enter while focus is in the TextBox
    ///     * a user clicks or tabs to and invokes the query button (defined using the QueryIcon API)
    ///     * a user presses selects (clicks/taps/presses Enter) a suggestion
    /// </summary>
    /// <param name="sender">The AutoSuggestBox that fired the event.</param>
    /// <param name="args">The args contain the QueryText, which is the text in the TextBox,
    /// and also ChosenSuggestion, which is only non-null when a user selects an item in the errorMessages.</param>
    private async void ProductEntryBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        Product? productToAdd = null;
        if (args.ChosenSuggestion != null && args.ChosenSuggestion is Product p)
        {

            //User selected an item, take an action           
            productToAdd = p;

        }
        else if (!string.IsNullOrEmpty(args.QueryText))
        {
            var id = sender.Text.Trim();
            ProductEntryBox.Text = null;
            var result = Int32.TryParse(id, out var prodcode);
            var toAdd = ViewModel.SuggestedProducts.FirstOrDefault(prod => prod.ProductID == prodcode);
            if (result && toAdd != null)
            {
                //The text matched a productcode
                productToAdd = toAdd;
            }
            else
            {
                productToAdd = null;
                return;
            }
        }

        if(productToAdd != null)
        {

            if (ViewModel.OrderItemsContains(productToAdd.ProductID))
            {
                productToAdd = null;
                ContentDialog dialog = new()
                {
                    Title = "Duplicate Product",
                    Content = string.Format("Product ID: {0} already exists on the Order \n", productToAdd?.ProductID),
                    CloseButtonText = "Ok",
                    XamlRoot = XamlRoot,
                };
                SystemSounds.Exclamation.Play();
                await dialog.ShowAsync();
            }
            else
            {
                var quantity = await PromptQuantityOrderdAsync(productToAdd.ProductID, productToAdd.ProductName ?? "null");
                if (quantity != int.MaxValue)
                {
                    ViewModel.AddItem(productToAdd, quantity);
                }
            }

            DispatcherQueue.TryEnqueue(async () => await ResetProductEntryBox());
        }
    }

    private void OrderNavigation_Click(object sender, RoutedEventArgs e)
    {
        //TODO: fix so you can naigate more than once
        if(sender is AppBarButton b)
        {
            if(b == NextOrderButton && ViewModel.HasNextOrder)
            {
                var s = App.GetService<INavigationService>();
                s.NavigateTo(typeof(OrderDetailsPageViewModel).FullName!, ViewModel.GetNextOrder());
                //ViewModel.SaveCurrentOrderAsync();
                //ViewModel.ChangeOrder(ViewModel.GetNextOrderID());
            }
            if(b == PreviousOrderButton && ViewModel.HasPreviousOrder)
            {
                var s = App.GetService<INavigationService>();
                s.NavigateTo(typeof(OrderDetailsPageViewModel).FullName!, ViewModel.GetPreviousOrder());
                //Frame.Navigate(typeof(OrderDetailsPage), ViewModel.GetPreviousOrderID());
                //ViewModel.SaveCurrentOrderAsync();
                //ViewModel.ChangeOrder(ViewModel.GetPreviousOrderID());
            }
        }
    }

    private void ProductEntryBox_GotFocus(object _sender, RoutedEventArgs e)
    {
        ProductEntryBox.IsSuggestionListOpen = true;
    }

    private void ProductEntryBox_LostFocus(object _sender, RoutedEventArgs e)
    {
        ProductEntryBox.IsSuggestionListOpen = false;
    }

    private async void StatusFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog contentDialog = new ContentDialog()
        {
            XamlRoot = XamlRoot,
            Title = "WARNING",
            Content = "You are about to change the status of this Order. Are you sure you know what you are doing?",
            PrimaryButtonText = "Continue",
            CloseButtonText = "Cancel",
        };
        SystemSounds.Exclamation.Play();
        var res = await contentDialog.ShowAsync();
        if (res == ContentDialogResult.Primary)
        {
            OrderStatusCombo.IsEnabled = true;
        }
    }

    #endregion Event Handlers

    #endregion Methods

    private void PrintOrderFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        _ = PrintOrderAsync();
    }

    private void PrintInvoiceFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        _= PrintInvoiceAsync();
    }

    private async Task PrintInvoiceAsync()
    {
        var printInvoice = false;
        if(ViewModel.CanPrintInvoice) {
            //Invoice already printed print copy
            if (ViewModel.OrderStatus > OrderStatus.Filled) 
            {
                ContentDialog contentDialog= new ContentDialog()
                {
                    XamlRoot = XamlRoot,
                    Content = "This invoice has already been printed. To print a copy press Reprint",
                    PrimaryButtonText = "Reprint",
                    CloseButtonText = "Cancel",
                };
                var res = await contentDialog.ShowAsync();
                if(res == ContentDialogResult.Primary)
                {
                    printInvoice = true;
                }
            }
            //Print invoice
            else
            {
                printInvoice = true;
            }
        }
        else if( ViewModel.OrderStatus == OrderStatus.Filling)
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
                printInvoice= true;
            }
        }

        if (printInvoice)
        {
            var filePath = reportGenerator.GenerateWholesaleInvoice(ViewModel.Order);

            PrinterSettings printSettings = new();
            printSettings.Copies = 2;
            var printer = new PDFPrinterService(filePath);
            await printer.PrintPdfAsync(printSettings);

            if (ViewModel.OrderStatus == OrderStatus.Filling || ViewModel.OrderStatus == OrderStatus.Filled)
            {
                ViewModel.OrderStatus = OrderStatus.Invoiced;
                _ = ViewModel.SaveCurrentOrderAsync();
            }
        }
    }

    private async Task PrintOrderAsync()
    {
        if (ViewModel.CanPrintOrder)
        {
            if (ViewModel.IsNewOrder)
            {
                ViewModel.SaveCurrentOrder();
            }
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
                _ = ViewModel.SaveCurrentOrderAsync();
            }
        }
    }

    private async Task<int> PromptQuantityOrderdAsync(int ProductID, string ProductName)
    {
        SingleValueInputDialog inputControl = new()
        {
            XamlRoot = XamlRoot,
            Title = $"{ProductID}  {ProductName}",
            PrimaryButtonText = "Submit",
            Prompt = "Quantity?",
            ValidateValue = delegate (string? value)
            {
                if (value != null)
                {
                    if (int.TryParse(value, out var result) && result != 0)
                    {
                        return true;
                    }
                }
                return false;
            },
        };
        var res = await inputControl.ShowAsync();
        if (res == ContentDialogResult.Primary && inputControl.Value != null)
        {
            return int.Parse(inputControl?.Value!);
        }
        else
        {
            //User canceled out
            return int.MaxValue;
        }
    }

    private async Task ResetProductEntryBox()
    {
        await ViewModel.QueryProducts();
        ProductEntryBox.Text = null;
        ProductEntryBox.IsSuggestionListOpen = false;
    }

    private void PickupDate_SelectedDateChanging(object sender, Syncfusion.UI.Xaml.Editors.DateChangingEventArgs e)
    {
        if(e.NewDate == null)
        {
            e.Cancel = true;
        }
    }
}