using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using BlOrders2023.Dialogs.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Dialogs;
public sealed partial class MultipleCustomerSelectionDialog : ContentDialog
{
    #region Properties
    public List<WholesaleCustomer>? Customers { get; set; }
    private MultipleCustomerSelectionDialogViewModel ViewModel { get; }
    #endregion Properties

    public MultipleCustomerSelectionDialog(XamlRoot xamlRoot)
    {
        XamlRoot = xamlRoot;
        this.InitializeComponent();
        ViewModel = App.GetService<MultipleCustomerSelectionDialogViewModel>();
    }

    #region Methods

    #endregion Methods

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (CustomerSelection.SelectedItems != null)
        {
            Customers = CustomerSelection.SelectedItems.OfType<WholesaleCustomer>().ToList();
        }
    }

    private void CustomerSelection_GotFocus(object sender, RoutedEventArgs e)
    {
        if(CustomerSelection.SelectedItems.IsNullOrEmpty())
        {
            CustomerSelection.IsDropDownOpen = true;
        }
    }

    private void CustomerSelection_LostFocus(object sender, RoutedEventArgs e)
    {
        CustomerSelection.IsDropDownOpen = false;
    }
}
