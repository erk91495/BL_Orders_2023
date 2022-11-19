using Bl_Orders_2023.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Bl_Orders_2023.Views;

public sealed partial class FillOrdersPage : Page
{
    public FillOrdersViewModel ViewModel
    {
        get;
    }

    public FillOrdersPage()
    {
        ViewModel = App.GetService<FillOrdersViewModel>();
        InitializeComponent();
    }
}
