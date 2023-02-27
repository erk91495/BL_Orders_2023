using BlOrders2023.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace BlOrders2023.Views;

public sealed partial class FillOrdersPage : Page
{
    public FillOrdersPageViewModel ViewModel
    {
        get;
    }

    public FillOrdersPage()
    {
        ViewModel = App.GetService<FillOrdersPageViewModel>();
        InitializeComponent();
    }
}
