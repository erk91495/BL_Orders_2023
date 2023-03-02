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
using Syncfusion.UI.Xaml.Grids.ScrollAxis;
using System.Windows.Forms.VisualStyles;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BlOrders2023.Contracts.Services;
using BlOrders2023.Services;
using Microsoft.UI.Dispatching;
using System.Media;
using CommunityToolkit.Common;
using Windows.Media.Devices;

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
        /// The viewmodel for the Order details page
        /// </summary>
        public OrderDetailsPageViewModel ViewModel { get; }
        #endregion Properties

        #region Fields
        private OrderItem? _doomed;
        private bool _deleteOrder;
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Initializes an instance of the OrderDetails Page
        /// </summary>
        public OrderDetailsPage()
        {
            ViewModel = App.GetService<OrderDetailsPageViewModel>();
            this.InitializeComponent();
            SetMemoTotalFormatter();
            SetMemoWeightFormatter();
            PickupTime.MinTime = new DateTime(1800, 1, 1, 0, 0, 0, 0);
            OrderedItems.PreviewKeyDown += OrderedItems_PreviewKeyDown;
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
            if (_deleteOrder)
            {
                ViewModel.DeleteCurrentOrder();
            }
            else 
            { 
                //change focus to write any changes
                OrderNumber.Focus(FocusState.Programmatic);
                //Task.Run(() => ViewModel.SaveCurrentOrder()).GetResultOrDefault();
                
            }
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Attempts to focus the last row and edit the Quantity ordered Column
        /// </summary>
        private void FocusEditLastCell()
        {
            var res = OrderedItems.Focus(FocusState.Keyboard);

            //TODO: Handle Empty row
            var rowIndex = OrderedItems.ResolveToRowIndex(ViewModel.Items.Last());
            var columnIndex = OrderedItems.Columns.IndexOf(OrderedItems.Columns.FirstOrDefault(c => c.HeaderText.ToString() == "Quantity Ordered"));
            var rowColumnIndex = new RowColumnIndex(rowIndex, columnIndex);
            OrderedItems.MoveCurrentCell(rowColumnIndex);
            res = OrderedItems.SelectionController.CurrentCellManager.BeginEdit();
        }

        /// <summary>
        /// Sets the Formatter for the MemoTotal field
        /// </summary>
        private void SetMemoTotalFormatter()
        {
            IncrementNumberRounder rounder = new();
            rounder.Increment = 0.01;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp;

            DecimalFormatter formatter = new();
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 2;
            formatter.NumberRounder = rounder;
            MemoTotal.NumberFormatter = formatter;
        }

        /// <summary>
        /// Sets the Formatter for the MemoWeight field
        /// </summary>
        private void SetMemoWeightFormatter()
        {
            IncrementNumberRounder rounder = new();
            rounder.Increment = 0.01;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp;

            DecimalFormatter formatter = new();
            formatter.IntegerDigits = 1;
            formatter.FractionDigits = 2;
            formatter.NumberRounder = rounder;
            MemoWeight.NumberFormatter = formatter;
        }

        #region Events Handlers

        /// <summary>
        /// Triggers when the collection changes
        /// </summary>
        /// <param name="sender">the data grid</param>
        /// <param name="e">event args</param>
        /// <exception cref="NotImplementedException"></exception>
        private void Records_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //If we added an item then enqueue a task to focus it and edit the quantity 
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                dispatcherQueue.TryEnqueue(() => FocusEditLastCell());
            }
        }

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
        /// Handles text changed events for the Product entry box. Requeries the database for _products matching the given input
        /// </summary>
        /// <param name="sender">the product entry autosuggestbox</param>
        /// <param name="args">the text changed event args</param>
        private void ProductEntryBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ViewModel.QueryProducts(sender.Text);
            }
            else if (args.Reason == AutoSuggestionBoxTextChangeReason.SuggestionChosen)
            {
                ProductEntryBox.Text = null;
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

        /// <summary>
        /// Tracks pointer movements so we know which item to delete from the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderedItems_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (OrderedItems != null)
            {
                var visualcontainer = OrderedItems.GetType()?.GetProperty("VisualContainer", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(OrderedItems) as VisualContainer;
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
                        if (record is RecordEntry r)
                        {
                            _doomed = r.Data as OrderItem;
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

        private async void DeleteOrderFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Delete Order",
                Content = "Are you sure you want to delete this entire order.\r\nThis action cannot be undone.",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Delete Order",
                IsPrimaryButtonEnabled = true,
                XamlRoot= XamlRoot,
            };

            var res = await dialog.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                _deleteOrder = true;
                NavigationService nav = App.GetService<INavigationService>() as NavigationService;
                if (nav != null)
                {
                    if (nav.CanGoBack)
                    {
                        nav.GoBack();
                    }
                    else
                    {
                        Frame.Navigate(typeof(OrdersPage));
                    }
                }
            }
        }

        /// <summary>
        /// Called when the ordereditems are loaded 
        /// </summary>
        /// <param name="sender">the sfdatagrid</param>
        /// <param name="e">the event args</param>
        private void OrderedItems_Loaded(object sender, RoutedEventArgs e)
        {
            OrderedItems.View.Records.CollectionChanged += Records_CollectionChanged;
        }

        /// <summary>
        /// Called when a key is pressed down on the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OrderedItems_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                var rowindex = OrderedItems.SelectionController.CurrentCellManager.CurrentCell.RowIndex;
                var columnIndex = OrderedItems.SelectionController.CurrentCellManager.CurrentCell.ColumnIndex;
                if (columnIndex < OrderedItems.Columns.Count -1)
                {
                    var rowColumnIndex = new RowColumnIndex(rowindex, columnIndex + 1);
                    //OrderedItems.MoveCurrentCell(rowColumnIndex);
                    //Dont need Count -1 becuase there is a header row
                }else if (rowindex < ViewModel.Items.Count)
                {
                    var rowColumnIndex = new RowColumnIndex(rowindex + 1, 2);
                    //OrderedItems.MoveCurrentCell(rowColumnIndex);
                }
            }
        }

        /// <summary>
        /// This event gets fired when:
        ///     * a user presses Enter while focus is in the TextBox
        ///     * a user clicks or tabs to and invokes the query button (defined using the QueryIcon API)
        ///     * a user presses selects (clicks/taps/presses Enter) a suggestion
        /// </summary>
        /// <param name="sender">The AutoSuggestBox that fired the event.</param>
        /// <param name="args">The args contain the QueryText, which is the text in the TextBox,
        /// and also ChosenSuggestion, which is only non-null when a user selects an item in the list.</param>
        private async void ProductEntryBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

            ProductEntryBox.IsSuggestionListOpen = false;
            Product productToAdd = new();
            if (args.ChosenSuggestion != null && args.ChosenSuggestion is Product p)
            {

                //User selected an item, take an action
                productToAdd = p;
                if (productToAdd != null && !ViewModel.OrderItemsContains(productToAdd.ProductID))
                {
                    ViewModel.addItem(productToAdd);
                }
                ProductEntryBox.IsSuggestionListOpen = false;

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
                }
                else
                {
                    productToAdd = null;
                    return;
                }
                if (productToAdd != null && !ViewModel.OrderItemsContains(productToAdd.ProductID))
                {
                    ViewModel.addItem(productToAdd);
                }
                else if (productToAdd != null)
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "Duplicate Product",
                        Content = String.Format("Product ID: {0} already exists on the Order \n", productToAdd.ProductID),
                        CloseButtonText = "Ok",
                        XamlRoot = XamlRoot,
                    };
                    SystemSounds.Exclamation.Play();
                    await dialog.ShowAsync();
                }
                ProductEntryBox.Text = null;
                ProductEntryBox.IsSuggestionListOpen = false;
            }
        }


        /// <summary>
        /// Resets the ProductEntryBox after an item is choosen
        /// </summary>
        /// <param name="sender">Product Entry Box</param>
        /// <param name="args">event args</param>
        private void ProductEntryBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            //ProductEntryBox.Text = null;
        }

        private async void OrderNavigation_Click(object sender, RoutedEventArgs e)
        {
            //TODO: fix so you can naigate more than once
            if(sender is AppBarButton b)
            {
                if(b == NextOrderButton && ViewModel.HasNextOrder)
                {
                    var s = App.GetService<INavigationService>();
                    s.NavigateTo(typeof(OrderDetailsPageViewModel).FullName!, ViewModel.GetNextOrder());
                    //ViewModel.SaveCurrentOrder();
                    //ViewModel.ChangeOrder(ViewModel.GetNextOrderID());
                }
                if(b == PreviousOrderButton && ViewModel.HasPreviousOrder)
                {
                    var s = App.GetService<INavigationService>();
                    s.NavigateTo(typeof(OrderDetailsPageViewModel).FullName!, ViewModel.GetPreviousOrder());
                    //Frame.Navigate(typeof(OrderDetailsPage), ViewModel.GetPreviousOrderID());
                    //ViewModel.SaveCurrentOrder();
                    //ViewModel.ChangeOrder(ViewModel.GetPreviousOrderID());
                }
            }
        }

        #endregion Event Handlers

        #endregion Methods
    }
}