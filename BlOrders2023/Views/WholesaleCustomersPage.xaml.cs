using BlOrders2023.Models;
using BlOrders2023.UserControls;
using BlOrders2023.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BlOrders2023.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class WholesaleCustomersPage : Page
{
    #region Properties
    WholesaleCustomer SelectedCustomer { get; set; }
    #endregion Properties
    #region Fields
    #endregion Fields

    public WholesaleCustomersViewModel ViewModel
    {
        get;
    }

    public WholesaleCustomersPage()
    {
        ViewModel = App.GetService<WholesaleCustomersViewModel>();
        InitializeComponent();
    }

    private async void MenuFlyoutEdit_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

        CustomerDataInputControl control = new(SelectedCustomer, false)
        {
            XamlRoot = XamlRoot,
        };
        await control.ShowAsync();
    }

    private void MenuFlyoutAdd_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void DataGrid_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        var element = (e.OriginalSource as FrameworkElement);
        if (element != null)
        {
            SelectedCustomer = (WholesaleCustomer)element.DataContext;
        }
    }
}