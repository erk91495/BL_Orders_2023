using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Contracts.Services;
using BlOrders2023.Helpers;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using BlOrders2023.Services;
using BlOrders2023.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.DataGrid;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class InventoryAdjustmentsPage : Page
{

    private bool _CanLeave = false;
    private HashSet<int> _modifiedItems = new();
    private bool _Modified => !_modifiedItems.IsNullOrEmpty();
    public InventoryAdjustmentsPageViewModel ViewModel
    {
        get;
    }

    public InventoryAdjustmentsPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<InventoryAdjustmentsPageViewModel>();
        InventoryGrid.CellRenderers.Remove("TextBox");
        InventoryGrid.CellRenderers.Add("TextBox", new GridCellTextBoxRendererExt());
    }

    protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {

        if (!_CanLeave && _Modified)
        {
            e.Cancel = true;
            var dialog = new ContentDialog()
            {
                XamlRoot = XamlRoot,
                Title = "Unsaved Changes",
                Content = "You have unsaved changes. Do you want to discard your changes",
                PrimaryButtonText = "Discard",
                CloseButtonText = "Cancel",
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                _CanLeave = true;
                e.Cancel = false;
                Frame.Navigate(e.SourcePageType, e.Parameter);
            }
            else
            {
                e.Cancel = true;
            }
        }
        base.OnNavigatingFrom(e);
    }

    private void InventoryGrid_CurrentCellValidating(object sender, CurrentCellValidatingEventArgs e)
    {
        if (e.RowData is InventoryTotalItem item)
        {

            if (e.NewValue != null && e.OldValue != e.NewValue)
            {
                _modifiedItems.Add(item.ProductID);
                switch (e.Column.MappingName)
                {
                    case "LastAdjustment":
                        {
                            if (!int.TryParse(e.NewValue.ToString(), out _))
                            {
                                e.ErrorMessage = "Adjustment Quantity Must Be A Whole Number";
                                e.IsValid = false;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }
    }

    private void InventoryGrid_CurrentCellValidated(object sender, CurrentCellValidatedEventArgs e)
    {
        if (e.RowData is InventoryTotalItem p)
        {
            Collection<ValidationResult> result = new();
            ValidationContext context = new(p);
            Validator.TryValidateObject(p, context, result, true);
        }
    }

    private async void btn_Cancel_Click(object sender, RoutedEventArgs e)
    {
        if (_Modified)
        {
            ContentDialog diag = new()
            {
                XamlRoot = XamlRoot,
                Title = "Unsaved Edits",
                Content = "Are you sure you want to discard all unsaved changes?",
                PrimaryButtonText = "Discard",
                SecondaryButtonText = "Cancel",
            };
            var res = await diag.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                if (App.GetService<INavigationService>() is NavigationService navigation && navigation.CanGoBack)
                {
                    navigation.GoBack();
                }
            }

        }
        else
        {
            if (App.GetService<INavigationService>() is NavigationService navigation && navigation.CanGoBack)
            {
                navigation.GoBack();
            }
        }
    }

    private async void btn_Save_Click(object sender, RoutedEventArgs e)
    {
        if (_Modified)
        {
            foreach (var id in _modifiedItems)
            {
                var item = ViewModel.Inventory.Where(i => i.ProductID == id).First();
                await ViewModel.SaveAsync(item);
                //do all of this to make the data look pretty. it doesnt get saved b/c its from a usp and not a table
                item.ManualAdjustments += item.LastAdjustment;
                item.Total += item.LastAdjustment;
            }
            var generator = new ReportGenerator(App.CompanyInfo);
            var report = generator. GetReport(typeof(CurrentInventoryReport), [ViewModel.Inventory]);
            ReportPrinterService printerService = new(report);
            _ = printerService.PrintAsync();
            ContentDialog contentDialog = new()
            {
                XamlRoot = XamlRoot,
                Content = "Inventory Adjustments Saved",
                PrimaryButtonText = "Ok",
                DefaultButton = ContentDialogButton.Primary,
            };
            await contentDialog.ShowAsync();
            await ViewModel.QueryInventory();
            ViewModel.ClearAdjustmentQuantity();
            foreach (var id in _modifiedItems)
            {
                await ViewModel.SaveAsync(ViewModel.Inventory.Where(i => i.ProductID == id).First());
            }
            _modifiedItems.Clear();
        }
        else
        {
            ContentDialog contentDialog = new()
            {
                XamlRoot = XamlRoot,
                Content = "No changes were made",
                PrimaryButtonText = "Ok",
                DefaultButton = ContentDialogButton.Primary,
            };
            await contentDialog.ShowAsync();
        }
    }
}

