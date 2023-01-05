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
using BlOrders2023.Helpers;
using Syncfusion.UI.Xaml.DataGrid;
using System.Reflection;
using Syncfusion.UI.Xaml.Data;
using WinUIEx;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views
{
    /// <summary>
    /// A page for viewing and editing Orders
    /// </summary>
    public sealed partial class OrderDetailsPage : Page
    {
        #region Properties
        /// <summary>
        /// The viewmodel for the order details page
        /// </summary>
        public OrderDetailsPageViewModel ViewModel { get; }
        #endregion Properties

        #region Fields
        private OrderItem? _doomed;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Initializes an instance of the OrderDetails Page
        /// </summary>
        public OrderDetailsPage()
        {
            ViewModel = App.GetService<OrderDetailsPageViewModel>();
            this.InitializeComponent();
            PickupTime.MinTime = new DateTime(1800, 1, 1, 0, 0, 0, 0);
        }
        #endregion Constructors
        #region Methods
        /// <summary>
        /// Handles NavigatedTo events
        /// </summary>
        /// <param name="e">the navigation envent args</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        /// <summary>
        /// Handles NavigatingFrom events
        /// </summary>
        /// <param name="e">the navigation envent args</param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            OrderNumber.Focus(FocusState.Programmatic);
            ViewModel.SaveCurrentOrder();
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Sets the Formatter for the MemoTotal field
        /// </summary>
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

        #region Events Handlers
        /// <summary>
        /// Handles click events for the email hyperlink button. Creates a mailto: event
        /// </summary>
        /// <param name="sender">the hplerlink button that was clicked</param>
        /// <param name="e">the click event args</param>
        private async void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Customer.Email != null)
                await Helpers.Helpers.SendEmailAsync(ViewModel.Customer.Email);
        }

        /// <summary>
        /// Handles text changed events for the Product entry box. Requeries the database for products matching the given input
        /// </summary>
        /// <param name="sender">the product entry autosuggestbox</param>
        /// <param name="args">the text changed event args</param>
        private void ProductEntryBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ViewModel.QueryProducts(sender.Text);
            }
        }
        /// <summary>
        /// Handles Selection changed for the shipping type radio buttons. Hides or shows the pickup time timepicker
        /// </summary>
        /// <param name="sender">A radio button</param>
        /// <param name="e">the selection changed event args</param>
        private void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (RadioPickUp.IsChecked == true)
            {
                PickupTimeStack.Visibility = Visibility.Visible;
            }
            else
            {
                PickupTimeStack.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Deletes the given row from the datagrid
        /// </summary>
        /// <param name="sender">a delete button</param>
        /// <param name="e">the event args for the click event</param>
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (_doomed != null)
            {
                ViewModel.Items.Remove(_doomed);
            }
            _doomed = null;
        }


        #endregion Event Handlers

        #endregion Methods


        /// <summary>
        /// Tracks pointer movements so we know which item to delete from the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderedItems_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var visualcontainer = OrderedItems.GetType().GetProperty("VisualContainer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(OrderedItems) as VisualContainer;
            if (visualcontainer == null)
            {
                return;
            }
                
            var point = e.GetCurrentPoint(visualcontainer).Position;
            var rowColumnIndex = visualcontainer.PointToCellRowColumnIndex(point);
            var recordIndex = OrderedItems.ResolveToRecordIndex(rowColumnIndex.RowIndex);
            if (recordIndex < 0)
            {
                return;
            }

            //When the rowindex is zero , the row will be header row
            if (!rowColumnIndex.IsEmpty)
            {
                if (OrderedItems.View.TopLevelGroup != null)
                {
                    // Get the current row record while grouping
                    var record = OrderedItems.View.TopLevelGroup.DisplayElements[recordIndex];
                    if (record.GetType() == typeof(RecordEntry))
                    {
                        _doomed = (record as RecordEntry).Data as OrderItem;
                    }
                }
                else
                {
                    //Gets the column from ColumnsCollection by resolving the corresponding column index from  GridVisibleColumnIndex                      
                    var gridColumn = OrderedItems.Columns[OrderedItems.ResolveToGridVisibleColumnIndex(rowColumnIndex.ColumnIndex)];
                    //For getting the record, need to resolve the corresponding record index from row index                     
                    _doomed = OrderedItems.View.Records[OrderedItems.ResolveToRecordIndex(rowColumnIndex.RowIndex)].Data as OrderItem;
                }

            }
        }
    }
}
