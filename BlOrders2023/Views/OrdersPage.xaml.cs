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

    #region Constructors
    /// <summary>
    /// Initializes an instance of OrdersPage
    /// </summary>
    public OrdersPage()
    {
        ViewModel = App.GetService<OrdersPageViewModel>();
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
    private async void EmailButton_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedOrder != null)
            await Helpers.Helpers.SendEmailAsync(ViewModel.SelectedOrder.Customer.Email ?? String.Empty);
    }

    #region Pane Buttons
    private void EditOrder_Click(object sender, RoutedEventArgs e) => NavigateToOrderDetailsPage();

    private void FillOrder_Click(object sender, RoutedEventArgs e) => NavigateToFillOrdersPage();

    private void PrintInvoice_Click(object sender, RoutedEventArgs e)
    {
        ReportGenerator g = new();
        var pdf = g.GenerateWholesaleInvoice(ViewModel.SelectedOrder);
        Directory.CreateDirectory(Path.GetTempPath() + "\\BLOrders2023");
        var filePath = Path.GetTempPath() + "BLOrders2023\\" + ViewModel.SelectedOrder.OrderID + "_"+  DateTime.Now.ToFileTime() + ".pdf";
        pdf.GeneratePdf(filePath);
        var options = new LauncherOptions();
        options.ContentType = "application/pdf";
        _ = Launcher.LaunchUriAsync(new Uri(filePath), options);
    }

    #endregion Pane Buttons

    #region Orders Grid
    /// <summary>
    /// Handles doubleclick events for the orders datagrid 
    /// </summary>
    /// <param name="sender">the object sending the event</param>
    /// <param name="e">event args for the doubletaped event</param>
    private void Order_OrdersGrid_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)=>
        NavigateToOrderDetailsPage();

    /// <summary>
    /// Handles right click events for the orders datagrid
    /// </summary>
    /// <param name="sender">the object sending the event</param>
    /// <param name="e">event args for the righttapped event</param>>
    private void OrdersGrid_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        var element = (e.OriginalSource as FrameworkElement);
        if (element != null)
        {
            ViewModel.SelectedOrder = element.DataContext as Order;
        }
    }

    #region Menu Flyouts
    /// <summary>
    /// Handles the click event for the details for the flyout menu
    /// </summary>
    /// <param name="sender">the object sending the event</param>
    /// <param name="e">event args for the click event</param>
    private void MenuFlyoutViewDetails_Click(object sender, RoutedEventArgs e) =>
        NavigateToOrderDetailsPage();

    /// <summary>
    /// Handles the click event for the New Order flyout item
    /// </summary>
    /// <param name="sender">the object sending the event</param>
    /// <param name="e">event args for the click event</param>
    private void MenuFlyoutNewOrderClick(object sender, RoutedEventArgs e)
    {
        CreateNewOrder(ViewModel?.SelectedOrder?.Customer);
    }

    /// <summary>
    /// Handles the click event for the Fill Order flyout item
    /// </summary>
    /// <param name="sender">the object sending the event</param>
    /// <param name="e">event args for the click event</param>
    private void MenuFlyoutFillOrderClick(object sender, RoutedEventArgs e) =>
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
    private void NewOrderBtn_Clicked(object sender, RoutedEventArgs e)
    {
        CustomerSelectionControl dialog = new(XamlRoot);
        dialog.SelectionChoose += CustomerSelectionControl_SelectionChoose;
        dialog.showAsync();

    }

    /// <summary>
    /// Handles the New Customer button clicked by prompting the user to create a customer. called from the split button flyout
    /// </summary>
    /// <param name="sender">the sender of the event</param>
    /// <param name="e">the event args</param>
    private void NewCustomerBtn_Click(object sender, RoutedEventArgs e)
    {
        CustomerDataInputControl dialog = new(XamlRoot);
        _ = dialog.ShowAsync();
    }

    /// <summary>
    /// The callback for the customer selection dialog. Gets the customer from the dialog and then create a new order for the customer
    /// </summary>
    /// <param name="o">The CustomerSelectionControl</param>
    /// <param name="args">the event args</param>
    private void CustomerSelectionControl_SelectionChoose(object? o, EventArgs args)
    {
        if (o is CustomerSelectionControl control 
            && control.ViewModel != null 
            && control.ViewModel.SelectedCustomer != null)
        {
            CreateNewOrder(control.ViewModel.SelectedCustomer);
        }
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
            Order order = new Order(customer);
            ViewModel.Orders.Add(order);
            Frame.Navigate(typeof(OrderDetailsPage), order);
        }
    }



    #endregion Methods
}
