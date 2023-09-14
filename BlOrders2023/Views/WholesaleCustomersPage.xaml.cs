using BlOrders2023.Helpers;
using BlOrders2023.Models;
using BlOrders2023.UserControls;
using BlOrders2023.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BlOrders2023.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class WholesaleCustomersPage : Page
{
    #region Properties
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

    private async void MenuFlyoutEdit_Click(object sender, RoutedEventArgs e)
    {

        CustomerDataInputDialog control = new(ViewModel.SelectedCustomer, false)
        {
            XamlRoot = XamlRoot,
        };
        await control.ShowAsync();
        ViewModel.Reload();
    }

    private void MenuFlyoutAdd_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedCustomer != null) 
        {
            CreateNewOrder(ViewModel.SelectedCustomer);
        }
    }

    private void DataGrid_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        var element = (e.OriginalSource as FrameworkElement);
        if (element != null)
        {
            ViewModel.SelectedCustomer = (WholesaleCustomer)element.DataContext;
        }
    }

    public async Task<bool> TrySaveSelectedCustomerAsync()
    {
        try
        {
            ViewModel.SaveSelectedCustomer();
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _ = ex;
            ContentDialog dialog = new()
            {
                Title = "Database Write Conflict",
                Content = $"The customer was modified before your changes could be saved.\r\n" +
                $"What would you like to do with your changes?",
                XamlRoot = this.XamlRoot,
                PrimaryButtonText = "Overwrite",
                SecondaryButtonText = "Discard",
                CloseButtonText = "Cancel",
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ViewModel.SaveSelectedCustomer(true);
                return true;
            }
            else if (result == ContentDialogResult.Secondary)
            {
                ViewModel.Reload();
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

    private void CustomerSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        ViewModel.QueryCustomers(CustomerSearch.Text);
    }

    /// <summary>
    /// Handles the New Customer button clicked by prompting the user to create a customer. called from the split button flyout
    /// </summary>
    /// <param name="sender">the sender of the event</param>
    /// <param name="e">the event args</param>
    private async void btn_NewCustomer_Click(object sender, RoutedEventArgs e)
    {
        CustomerDataInputDialog dialog = new(new WholesaleCustomer(), true)
        {
            XamlRoot = XamlRoot,
        };
        await dialog.ShowAsync();

        ViewModel.Reload();
    }

    /// <summary>
    /// Creates a new Order for the given customer
    /// </summary>
    /// <param name="customer"></param>
    private void CreateNewOrder(WholesaleCustomer customer)
    {
        if (customer != null)
        {
            Order order = new(customer);
            Frame.Navigate(typeof(OrderDetailsPage), order);
        }
    }

    private void CustomersGrid_CopyingRowClipboardContent(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridRowClipboardEventArgs e)
    {
            DatagridCellCopy.CopyGridCellContent(sender, e);
    }
}