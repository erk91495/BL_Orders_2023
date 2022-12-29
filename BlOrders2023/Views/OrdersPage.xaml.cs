using BlOrders2023.Models;
using BlOrders2023.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.System;

namespace BlOrders2023.Views;

public sealed partial class OrdersPage : Page
{
    public OrdersPageViewModel ViewModel
    {
        get;
    }

    public OrdersPage()
    {
        ViewModel = App.GetService<OrdersPageViewModel>();
        InitializeComponent();        
    }

    /// <summary>
    /// Retrieve the list of orders when the user navigates to the page. 
    /// </summary>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (ViewModel.Orders.Count < 1)
        {
            ViewModel.LoadOrders();
        }
    }

    /// <summary>
    /// Creates an email about the currently selected invoice. 
    /// </summary>
    private async void EmailButton_Click(object sender, RoutedEventArgs e)
    {
        if(ViewModel.SelectedOrder != null)
            await Launcher.LaunchUriAsync( new Uri(String.Format("mailto:{0}", ViewModel.SelectedOrder.Customer.Email)));
    }

    private void Order_OrdersGrid_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)=>
        Frame.Navigate(typeof(OrderDetailsPage), ViewModel.SelectedOrder);


    private void OrdersGrid_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        var element = (e.OriginalSource as FrameworkElement);
        if (element != null)
        {
            ViewModel.SelectedOrder = element.DataContext as Order;
        }
    }

    private void MenuFlyoutViewDetails_Click(object sender, RoutedEventArgs e) =>
        Frame.Navigate(typeof(OrderDetailsPage), ViewModel.SelectedOrder);

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
}
