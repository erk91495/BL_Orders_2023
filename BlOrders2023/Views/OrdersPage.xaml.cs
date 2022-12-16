using BlOrders2023.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.ApplicationModel.Email;

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
        //if (ViewModel.SelectedOrder != null && ViewModel.SelectedOrder.Customer != null)
        //{
        //    var emailMessage = new EmailMessage
        //    {
        //        Body = $"Dear {ViewModel.SelectedOrder.Customer.CustomerName},",
        //        Subject = "A message from Bowman & Landes Turkeys about order " +
        //        $"#{ViewModel.SelectedOrder.OrderID} placed on " +
        //        $"{ViewModel.SelectedOrder.OrderDate.ToString("MM/dd/yyyy")}"
        //    };

        //    if (!string.IsNullOrEmpty(ViewModel.SelectedOrder.Customer.Email))
        //    {
        //        var emailRecipient = new EmailRecipient(ViewModel.SelectedOrder.Customer.Email);
        //        emailMessage.To.Add(emailRecipient);
        //    }
        //    await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        //}
    }
}
