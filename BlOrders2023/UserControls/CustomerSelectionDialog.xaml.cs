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

namespace BlOrders2023.UserControls
{
    public sealed partial class CustomerSelectionDialog : ContentControl
    {
        #region Properties
        public CustomerSelectionDialogViewModel ViewModel { get; }
        #endregion Properties
        #region Fields
        readonly ContentDialog _dialog;
        #endregion Fields
        #region Events
        public event EventHandler? SelectionChoose;
        #endregion Events
        #region Constructors
        public CustomerSelectionDialog(XamlRoot root)
        {
            _dialog = new()
            {
                XamlRoot = root,
                Title = "Select A Customer",
                Content = this,
                PrimaryButtonText = "Create An Order",
                SecondaryButtonText = "New Customer",
                CloseButtonText = "Cancel",
                FlowDirection = FlowDirection.LeftToRight,
                IsPrimaryButtonEnabled = false
            };
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

        public async void ShowAsync()
        {
            var result = await _dialog.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                SelectionChoose?.Invoke(this, new EventArgs());
            }
            else if (result == ContentDialogResult.Secondary)
            {
                CustomerDataInputDialog control = new(new WholesaleCustomer())
                {
                    XamlRoot = XamlRoot,
                };                
                await control.ShowAsync();
            }
            else
            {
                return;
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
                _dialog.IsPrimaryButtonEnabled = true;
                CustomerSelection.Text = ViewModel.SelectedCustomer.CustomerName;
            }
            else
            {
                _dialog.IsPrimaryButtonEnabled = false;
            }
        }
        #endregion Methods
    }
}
