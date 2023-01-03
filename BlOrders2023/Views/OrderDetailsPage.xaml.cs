// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BlOrders2023.Models;
using BlOrders2023.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using BlOrders2023.Models.Enums;
using Windows.Globalization.NumberFormatting;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrderDetailsPage : Page
    {
        public OrderDetailsPageViewModel ViewModel { get; }

        public OrderDetailsPage()
        {
            ViewModel = App.GetService<OrderDetailsPageViewModel>();
            this.InitializeComponent();
            PickupTime.MinTime = new DateTime(1800, 1, 1, 0, 0, 0, 0);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var order = e.Parameter as Order;
            ViewModel.CurrentOrder = order;
            base.OnNavigatedTo(e);
        }
        private async void EmailButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProductEntryBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ViewModel.QueryProducts(sender.Text);
            }
        }

        private void SetMemoTotalFormatter()
        {
            IncrementNumberRounder rounder = new();
            rounder.Increment = 0.25;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp;

            DecimalFormatter formatter = new();
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 2;
            formatter.NumberRounder = rounder;
            MemoTotal.NumberFormatter = formatter;
        }

        private void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if(RadioPickUp.IsChecked == true)
            {
                PickupTimeStack.Visibility = Visibility.Visible;
            }
            else
            {
                PickupTimeStack.Visibility = Visibility.Collapsed;
            }
        }
    }
}
