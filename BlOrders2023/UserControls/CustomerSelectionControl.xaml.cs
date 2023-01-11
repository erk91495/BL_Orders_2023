// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.
using Microsoft.UI.Xaml.Controls;
using BlOrders2023.ViewModels;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls
{
    public sealed partial class CustomerSelectionControl : ContentDialog
    {
        #region Properties
        public CustomerSelectionViewModel ViewModel { get; }
        #endregion Properties
        #region Fields
        #endregion Fields
        #region Constructors
        public CustomerSelectionControl()
        {
            ViewModel = App.GetService<CustomerSelectionViewModel>();
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
        #endregion Methods

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            base.Hide();
        }
    }
}
