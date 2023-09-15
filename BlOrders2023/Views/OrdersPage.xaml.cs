using BlOrders2023.Models;
using BlOrders2023.UserControls;
using BlOrders2023.ViewModels;
using BlOrders2023.Reporting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.System;
using QuestPDF.Previewer;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Windows.Storage;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using BlOrders2023.Models.Enums;
using BlOrders2023.Services;
using System.Drawing.Printing;
using System.Media;
using BlOrders2023.Helpers;

namespace BlOrders2023.Views;

/// <summary>
///  A Page for viewing a list of orders
/// </summary>
public sealed partial class OrdersPage : Page
{
    
    #region Properties
    /// <summary>
    /// Gets the view model for the Orders Page
    /// </summary>
    public OrdersPageViewModel ViewModel
    {
        get;
    }
    #endregion Properties

    #region Fields
    private readonly ReportGenerator reportGenerator;
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Initializes an instance of OrdersPage
    /// </summary>
    public OrdersPage()
    {
        ViewModel = App.GetService<OrdersPageViewModel>();
        reportGenerator = new();
        InitializeComponent();       
    }
    #endregion Constructors

    #region Methods
    /// <summary>
    /// Retrieve the list of orders when the user navigates to the page. 
    /// </summary>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel.Orders.Clear();
        ViewModel.LoadOrders();
    }

    #region Navigation
    /// <summary>
    /// Navigates to the OrderDetailsPage passing in the currently selected order as a parameter
    /// </summary>
    private void NavigateToOrderDetailsPage()=>
        Frame.Navigate(typeof(OrderDetailsPage), ViewModel.SelectedOrder);

    /// <summary>
    /// Navigates to the FillOrdersPage passing in the current selected order id as a parameter
    /// </summary>
    private void NavigateToFillOrdersPage() =>
        Frame.Navigate(typeof(FillOrdersPage), ViewModel.SelectedOrder!.OrderID);

    #endregion Navigation

    #region Event Handlers

    /// <summary>
    /// Creates an email about the currently selected invoice. 
    /// </summary>
    private async void EmailButton_Click(object _sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedOrder != null)
            await Helpers.Helpers.SendEmailAsync(ViewModel.SelectedOrder.Customer.Email ?? String.Empty);
    }

    #region Pane Buttons
    private void EditOrder_Click(object _sender, RoutedEventArgs e) => NavigateToOrderDetailsPage();

    private void FillOrder_Click(object _sender, RoutedEventArgs e) => NavigateToFillOrdersPage();

    private async void PrintInvoice_Click(object _sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedOrder.OrderStatus == Models.Enums.OrderStatus.Ordered)
        {
            var filePath = reportGenerator.GeneratePickList(ViewModel.SelectedOrder);

            PrinterSettings printSettings = new();
            var printer = new PDFPrinterService(filePath);
            await printer.PrintPdfAsync(printSettings);

            ViewModel.SelectedOrder.OrderStatus = Models.Enums.OrderStatus.Filling;
            _ = ViewModel.SaveCurrentOrderAsync();
        }
        else if (ViewModel.SelectedOrder.CanPrintInvoice)
        {
            await PrintInvoiceAsync();           
        }
        else if (ViewModel.SelectedOrder.OrderStatus >= OrderStatus.Invoiced)
        {
            await PrintInvoiceAsync();
        }
    }

    #endregion Pane Buttons

    #region Orders Grid
    /// <summary>
    /// Handles doubleclick events for the orders datagrid 
    /// </summary>
    /// <param name="sender">the object sending the event</param>
    /// <param name="e">event args for the doubletaped event</param>
    private void Order_OrdersGrid_DoubleTapped(object _sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)=>
        NavigateToOrderDetailsPage();

    /// <summary>
    /// Handles right click events for the orders datagrid
    /// </summary>
    /// <param name="sender">the object sending the event</param>
    /// <param name="e">event args for the righttapped event</param>>
    private void OrdersGrid_RightTapped(object _sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        var element = (e.OriginalSource as FrameworkElement);
        if (element != null)
        {
            ViewModel.SelectedOrder = (Order)element.DataContext;
        }
    }

    #region Menu Flyouts
    /// <summary>
    /// Handles the click event for the details for the flyout menu
    /// </summary>
    /// <param name="sender">the object sending the event</param>
    /// <param name="e">event args for the click event</param>
    private void MenuFlyoutViewDetails_Click(object _sender, RoutedEventArgs e) =>
        NavigateToOrderDetailsPage();

    /// <summary>
    /// Handles the click event for the New Order flyout item
    /// </summary>
    /// <param name="sender">the object sending the event</param>
    /// <param name="e">event args for the click event</param>
    private void MenuFlyoutNewOrderClick(object _sender, RoutedEventArgs e)
    {
        if (ViewModel?.SelectedOrder?.Customer != null)
        {
            WholesaleCustomer customer = ViewModel.SelectedOrder.Customer;
            CreateNewOrder(customer);
        }
    }

    /// <summary>
    /// Handles the click event for the Fill Order flyout item
    /// </summary>
    /// <param name="sender">the object sending the event</param>
    /// <param name="e">event args for the click event</param>
    private void MenuFlyoutFillOrderClick(object _sender, RoutedEventArgs e) =>
        NavigateToFillOrdersPage();
    #endregion Menu Flyouts
    #endregion Orders Grid

    /// <summary>
    /// Handles TextChanged events for the search box. Updates the filter value for the view model and refreshes filter
    /// </summary>
    /// <param name="sender">the object sending the event</param>
    /// <param name="e">event args for the textchanged event</param>
    private void SeachBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox? textBox = sender as TextBox;
        if (OrdersGrid.View.Filter == null)
        {
            OrdersGrid.View.Filter = ViewModel.FilterOrders;
        }
        if (textBox != null && textBox.Text != null)
        {
            ViewModel.FilterText = textBox.Text;
            OrdersGrid.View.RefreshFilter();
        }
    }

    /// <summary>
    /// Handles the New Order button clicked by prompting the user to select a customer. Triggered by the split button
    /// </summary>
    /// <param name="sender">the sender of the event</param>
    /// <param name="e">the event args</param>
    private void NewOrderBtn_Clicked(SplitButton sender, SplitButtonClickEventArgs e)
    {
        NewOrderBtn_Clicked(sender as object, new RoutedEventArgs());
    }

    /// <summary>
    /// Handles the New Order button clicked by prompting the user to select a customer. called from the split button flyout
    /// </summary>
    /// <param name="sender">the sender of the event</param>
    /// <param name="e">the event args</param>
    private async void NewOrderBtn_Clicked(object _sender, RoutedEventArgs e)
    {
        CustomerSelectionDialog dialog = new(XamlRoot);
        WholesaleCustomer? selectedCustomer = null;
        var res = await dialog.ShowAsync();
        if(res == ContentDialogResult.Primary)
        {
            if (dialog.ViewModel != null && dialog.ViewModel.SelectedCustomer != null)
            {
                selectedCustomer = dialog.ViewModel.SelectedCustomer;
            }
        }
        else if (res == ContentDialogResult.Secondary)
        {
            CustomerDataInputDialog control = new(new WholesaleCustomer())
            {
                XamlRoot = XamlRoot,
            };
            var result = await control.ShowAsync();
            if(result == ContentDialogResult.Primary && control.GetCustomer() != null)
            {
                selectedCustomer = control.GetCustomer();
            }
        }

        if (selectedCustomer != null)
        {
            CreateNewOrder(selectedCustomer);
        }
    }

    /// <summary>
    /// Handles the New Customer button clicked by prompting the user to create a customer. called from the split button flyout
    /// </summary>
    /// <param name="sender">the sender of the event</param>
    /// <param name="e">the event args</param>
    private void NewCustomerBtn_Click(object _sender, RoutedEventArgs e)
    {
        CustomerDataInputDialog dialog = new(new WholesaleCustomer()) 
        {
            XamlRoot = XamlRoot,
        };
        _ = dialog.ShowAsync();
    }

    #endregion Event Handlers

    /// <summary>
    /// Creates a new order for the given customer
    /// </summary>
    /// <param name="customer"></param>
    private void CreateNewOrder(WholesaleCustomer customer)
    {
        if (customer != null)
        {
            Order order = new(customer);
            ViewModel.Orders.Add(order);
            Frame.Navigate(typeof(OrderDetailsPage), order);
        }
    }

    private async Task PrintInvoiceAsync()
    {
        PrinterSettings printSettings = new();
        var printInvoice = false;
        if (ViewModel.SelectedOrder.CanPrintInvoice)
        {
            //Invoice already printed print copy
            if (ViewModel.SelectedOrder.OrderStatus > OrderStatus.Filled)
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
            else if (ViewModel.SelectedOrder.OrderStatus == OrderStatus.Filling)
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
            var filePath = reportGenerator.GenerateWholesaleInvoice(ViewModel.SelectedOrder);

            var printer = new PDFPrinterService(filePath);
            await printer.PrintPdfAsync(printSettings);

            if (ViewModel.SelectedOrder.OrderStatus == OrderStatus.Filling || ViewModel.SelectedOrder.OrderStatus == OrderStatus.Filled)
            {
                ViewModel.SelectedOrder.OrderStatus = OrderStatus.Invoiced;
                _ = ViewModel.SaveCurrentOrderAsync();
            }
        }
    }
    public async Task<bool> TrySaveCurrentOrderAsync()
    {
        try
        {
            await ViewModel.SaveCurrentOrderAsync();
            return true;
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

    private void OrdersGrid_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grids.GridSelectionChangedEventArgs e)
    {
        switch (ViewModel.SelectedOrder.OrderStatus)
        {
            case Models.Enums.OrderStatus.Ordered:
                Order_BtnPrintInvoice.Content = "Print Order";
                break;
            case Models.Enums.OrderStatus.Filling:
            case Models.Enums.OrderStatus.Filled:
                Order_BtnPrintInvoice.Content = "Print Invoice";
                break;
            case Models.Enums.OrderStatus.Invoiced:
            case Models.Enums.OrderStatus.Complete:
                Order_BtnPrintInvoice.Content = "Reprint Invoice";
                break;
            default:
                break;
        }
    }

    private void OrdersGrid_CopyGridCellContent(object sender, Syncfusion.UI.Xaml.Grids.GridCopyPasteCellEventArgs e)
    {
        DatagridCellCopy.CopyGridCellContent(sender, e);
    }
    #endregion Methods
}
