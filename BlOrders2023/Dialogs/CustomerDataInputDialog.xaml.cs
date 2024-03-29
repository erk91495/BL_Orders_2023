using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using BlOrders2023.Dialogs.ViewModels;
using CommunityToolkit.WinUI.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Dialogs;

public sealed partial class CustomerDataInputDialog : ContentDialog
{
    #region Properties
    CustomerDataInputControlViewModel ViewModel { get; }

    #endregion Properties

    #region Fields
    #endregion Fields

    #region Constructors
    public CustomerDataInputDialog(WholesaleCustomer customer, bool CheckIfUnique = true)
    {
        this.InitializeComponent();
        var enumValues = Enum.GetNames(typeof(States));
        StateComboBox.ItemsSource = enumValues;
        BillingStateComboBox.ItemsSource = enumValues;
        ViewModel = App.GetService<CustomerDataInputControlViewModel>();
        ViewModel.SetCustomer(customer);
        ViewModel.ErrorsChanged += ViewModel_ErrorsChanged;
        ViewModel.CheckIfUnique = CheckIfUnique;
        if (!CheckIfUnique)
        {
            PrimaryButtonText = "Update Customer";
        }
    }

    private void ViewModel_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        if (ViewModel.HasErrors)
        {
            IsPrimaryButtonEnabled = false;
        }
        else
        {
            IsPrimaryButtonEnabled = true;
        }
    }
    #endregion Constructors

    #region Methods
    #endregion Methods

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debugger.Break();
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        try
        {
            //if we didn't save we have validation issues
            if (!ViewModel.SaveCustomer())
            {
                args.Cancel = true;
            }
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _ = ex;
            FailureInfoBar.Message = $"The customer was modified before your changes could be saved.\r\n";
            FailureInfoBar.IsOpen = true;
            args.Cancel = true;
        }
        catch (DbUpdateException ex)
        {
            FailureInfoBar.Message = $"An error occured while trying to save your order. Please contact your system administrator\r\n" +
                $"Details:\r\n{ex.Message}\r\n{ex.InnerException!.Message}";
            FailureInfoBar.IsOpen = true;
            args.Cancel = true;

        }

    }

    public WholesaleCustomer GetCustomer() 
    {
        return ViewModel.Customer;
    }
}
