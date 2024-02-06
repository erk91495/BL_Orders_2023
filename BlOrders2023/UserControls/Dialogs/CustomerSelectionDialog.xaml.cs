// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.
using Microsoft.UI.Xaml.Controls;
using BlOrders2023.UserControls.ViewModels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.CodeDom;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using BlOrders2023.Models;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls;

public sealed partial class CustomerSelectionDialog : ContentDialog
{
    #region Properties
    public CustomerSelectionDialogViewModel ViewModel { get; }
    #endregion Properties
    #region Fields
    //readonly ContentDialog _dialog;
    #endregion Fields
    #region Constructors
    public CustomerSelectionDialog(XamlRoot root)
    {
        XamlRoot = root;
        ViewModel = App.GetService<CustomerSelectionDialogViewModel>();
        this.InitializeComponent();
    }
    #endregion Constructors

    #region Methods
    private void CustomerSelection_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel.QueryCustomers(sender.Text);
        }
    }

    /// <summary>
    /// Triggers when a customer has been selected or when the user submits a name using the return key
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void CustomerSelection_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion != null && args.ChosenSuggestion is WholesaleCustomer c)
        {
            //User selected an item, take an action
            ViewModel.SelectedCustomer = c;
        }
        else
        {
            var name = args.QueryText.Trim();
            // If we dont find something the result will be null
            ViewModel.SelectedCustomer = ViewModel.SuggestedCustomers.FirstOrDefault(cust => cust.CustomerName.Equals(name, StringComparison.CurrentCultureIgnoreCase) || cust.Phone == name);
        }
        if(ViewModel.SelectedCustomer!= null)
        {
            IsPrimaryButtonEnabled = true;
            CustomerSelection.Text = ViewModel.SelectedCustomer.CustomerName;
        }
        else
        {
            IsPrimaryButtonEnabled = false;
        }
    }
    #endregion Methods
}
