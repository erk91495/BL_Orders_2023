﻿using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using BlOrders2023.Dialogs.ViewModels;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Converters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

using Windows.Globalization.NumberFormatting;

namespace BlOrders2023.Dialogs
{
    public sealed partial class ShippingItemDataInputDialog : ContentControl
    {
        #region Properties
        ShippingItemDataInputControlViewModel ViewModel { get; }
        #endregion Properties

        #region Fields
        private readonly ContentDialog _dialog;
        #endregion Fields

        #region Constructors
        public ShippingItemDataInputDialog(XamlRoot root)
        {
            InitializeComponent();
            ViewModel = App.GetService<ShippingItemDataInputControlViewModel>();

            _dialog = new()
            {
                XamlRoot = root,
                Content = this,
                PrimaryButtonText = "Add Product",
                CloseButtonText = "Cancel",
                FlowDirection = FlowDirection.LeftToRight
            };

            Binding b = new()
            {
                Source = ViewModel,
                Path = new PropertyPath("HasErrors"),
                Converter = new BoolNegationConverter(),
                Mode = BindingMode.OneWay
            };
            _dialog.SetBinding(ContentDialog.IsPrimaryButtonEnabledProperty, b);


            IncrementNumberRounder NetWtRounder = new IncrementNumberRounder()
            {
                Increment = .001,
                RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp,
            };

            DecimalFormatter NetWtFormatter = new()
            {
                FractionDigits = 2,
                NumberRounder = NetWtRounder,
            };
            NetWeight.NumberFormatter = NetWtFormatter;
            
        }
        #endregion Constructors

        private void ProductSelection_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                _ = ViewModel.QueryProducts(sender.Text);
            }
            else if (args.Reason == AutoSuggestionBoxTextChangeReason.SuggestionChosen)
            {
                ProductSelection.Text = null;
            }
        }

        private void ProductSelection_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ProductSelection.IsSuggestionListOpen = false;
            Product productToAdd = new();
            if (args.ChosenSuggestion != null && args.ChosenSuggestion is Product p)
            {

                //User selected an item, take an action
                productToAdd = p;
                ViewModel.SelectedProduct = productToAdd;
                ProductSelection.IsSuggestionListOpen = false;
                DispatcherQueue.EnqueueAsync(() => NetWeight.Focus(FocusState.Programmatic));

            }
            else if (!string.IsNullOrEmpty(args.QueryText))
            {
                var id = sender.Text.Trim();
                bool result = Int32.TryParse(id, out int prodcode);
                var toAdd = ViewModel.SuggestedProducts.FirstOrDefault(prod => prod.ProductID == prodcode);

                if (result && toAdd != null)
                {
                    //The text matched a productcode
                    productToAdd = toAdd;
                    ViewModel.SelectedProduct = productToAdd;

                }
                else
                {
                    productToAdd = null!;
                }

                ProductSelection.Text = null;
                ProductSelection.IsSuggestionListOpen = false;
                DispatcherQueue.EnqueueAsync(() => NetWeight.Focus(FocusState.Programmatic));
            }

        }

        public async Task<ShippingItem?> ShowAsync()
        {
            var res = await _dialog.ShowAsync();
            if(res == ContentDialogResult.Primary)
            {
                return ViewModel.GetShippingItem();
            }
            return null;
        }
    }
}