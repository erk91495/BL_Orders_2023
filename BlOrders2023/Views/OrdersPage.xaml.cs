using BlOrders2023.ViewModels;

using Microsoft.UI.Xaml.Controls;

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
}
