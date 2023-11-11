using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Contracts.Services;
using BlOrders2023.Core.Contracts;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Core.Services;
using BlOrders2023.Exceptions;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using BlOrders2023.Reporting;
using BlOrders2023.Services;
using BlOrders2023.UserControls;
using BlOrders2023.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AllocatorPage : Page
{
    #region Fields
    private readonly NavigationService _navigationService;
    #endregion Fields

    #region Properties
    public AllocatorPageViewModel ViewModel { get; }
    #endregion Properties

    #region Constructors

    public AllocatorPage()
    {
        ViewModel = App.GetService<AllocatorPageViewModel>();
        this.InitializeComponent();
        _navigationService = App.GetService<INavigationService>() as NavigationService;
    }
    #endregion Constructors

    #region Methods

    public async Task ShowAllocationTypeSelectionAync()
    {
        StackPanel mainStack = new()
        {
            Orientation = Orientation.Vertical,
        };

        TextBlock propmt = new()
        {
            Text = "Select an Allocation Mode",
        };

        mainStack.Children.Add(propmt);

        ComboBox comboBox = new ComboBox()
        {
            ItemsSource = Enum.GetValues(typeof(AllocatorMode)),
            SelectedIndex = 0,
        };
        mainStack.Children.Add(comboBox);
        ContentDialog dialog = new()
        {
            XamlRoot = XamlRoot,
            Content = mainStack,
            PrimaryButtonText = "Allocate",
            CloseButtonText = "Cancel",
        };
        var result = await dialog.ShowAsync();
        if(result == ContentDialogResult.Primary)
        {
            if (comboBox.SelectedItem is AllocatorMode mode)
            {
                ViewModel.AllocatorMode = mode;
            }
            //-1  will be the value if TEST mode type is selected during debug, but TEST is not compiled into Release Builds
            if (((int)ViewModel.AllocatorConfig.AllocatorMode) == -1)
            {
                ViewModel.AllocatorConfig.IDs = new() { 67662, 67663, 68664 };
                await StartAllocationAsync();
            }

            else
            {
                var dateTuple = await ShowDateRangeSelectionAsync();
                if (dateTuple.Item1 != null && dateTuple.Item2 != null)
                {
                    var ids = await ViewModel.GetOrdersIDToAllocateAsync(dateTuple.Item1, dateTuple.Item2, ViewModel.AllocatorConfig.AllocatorMode);

                    ViewModel.AllocatorConfig.IDs = ids.ToList();
                    await StartAllocationAsync();
                }
                else
                {
                    TryGoBack();
                }
            }

        }
        else
        {
            TryGoBack();
        }
    }

    private async Task StartAllocationAsync()
    {
        try
        {
            await ViewModel.StartAllocationAsync();
        }
        catch (AllocationFailedException e)
        {
            Debug.WriteLine(e.Message);
            ContentDialog d = new()
            {
                XamlRoot = XamlRoot,
                Title = "Allocation Failed",
                Content = e.Message,
                CloseButtonText = "Ok",
            };
            await d.ShowAsync();
#if !DEBUG
            TryGoBack();
#endif //!DEBUG
        }
    }

    private async Task<(DateTimeOffset?, DateTimeOffset?)> ShowDateRangeSelectionAsync()
    {
        DateRangeSelectionDialog dialog = new()
        {
            XamlRoot = XamlRoot
        };
        var res = await dialog.ShowAsync();
        if (res == ContentDialogResult.Primary)
        {
            if (dialog.StartDate == null || dialog.EndDate == null)
            {
                ContentDialog d = new()
                {
                    XamlRoot = XamlRoot,
                    Title = "Error",
                    Content = $"Please select a date range",
                    PrimaryButtonText = "ok",
                };
                await d.ShowAsync();
            }
            else
            {
                //set the time so we pick up everything within the range
                DateTime sDate = dialog.StartDate.Value.Date;
                DateTime eDate = dialog.EndDate.Value.Date;
                DateTimeOffset startDate = new(sDate.Year, sDate.Month, sDate.Day, 0, 0, 0, 0, new());
                DateTimeOffset endDate = new(eDate.Year, eDate.Month, eDate.Day, 23, 59, 59, 999, new());
                return (startDate, endDate);
            }
        }
        return (null, null);
    }

    private void AllocatorPage_Loaded(object sender, RoutedEventArgs e)
    {
        _ = ShowAllocationTypeSelectionAync();
    }

    private void TryGoBack()
    {
        if (_navigationService != null)
        {
            if (_navigationService.CanGoBack)
            {
                _navigationService.GoBack();
            }
        }
    }

    private async void btn_SavePrint_Click(object sender, RoutedEventArgs e)
    {
        //Want to lock out save buttons to prevent double clicks
        btn_Save.IsEnabled = false;
        btn_SavePrint.IsEnabled = false;
        rng_SavePrint.Visibility = Visibility.Visible;
        await SaveAllocation();
        await PrintAllocationSummary();
        rng_SavePrint.Visibility = Visibility.Collapsed;
        btn_Save.IsEnabled = true;
        btn_SavePrint.IsEnabled = true;
    }

    private async void btn_Save_Click(object sender, RoutedEventArgs e)
    {
        //Want to lock out save buttons to prevent double clicks
        btn_Save.IsEnabled = false;
        btn_SavePrint.IsEnabled = false;
        rng_Save.Visibility = Visibility.Visible;
        await SaveAllocation();
        rng_Save.Visibility = Visibility.Collapsed;
        //Want to lock out save button to prevent double clicks
        btn_Save.IsEnabled = true;
        btn_SavePrint.IsEnabled = true;
    }

    private async Task SaveAllocation()
    {
        try
        {
            await ViewModel.AllocatorService.SaveAllocationAsync();
            ContentDialog d = new()
            {
                XamlRoot = XamlRoot,
                Title = "Allocation Saved",
                Content = $"The Allocation Has Been Saved",
                PrimaryButtonText = "OK",
                DefaultButton = ContentDialogButton.Primary,

            };
            await d.ShowAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _ = ex;
            ContentDialog d = new()
            {
                XamlRoot = XamlRoot,
                Title = "Database Write Conflict",
                Content = $"The an order was modified before your changes could be saved.\r\n" +
                $"Please re-run allocation",
                SecondaryButtonText = "Continue",
                DefaultButton = ContentDialogButton.None,

            };
            await d.ShowAsync();
        }
        catch (DbUpdateException ex)
        {
            ContentDialog d = new()
            {
                XamlRoot = XamlRoot,
                Title = "DbUpdateException",
                Content = $"An error occured while trying to save your Order. Please contact your system administrator\r\n" +
                $"Details:\r\n{ex.Message}\r\n{ex.InnerException!.Message}",
                SecondaryButtonText = "Continue",
                DefaultButton = ContentDialogButton.None,

            };
            await d.ShowAsync();
        }

    }
    
    private async Task PrintAllocationSummary()
    {
        var filePath = await ViewModel.AllocatorService.GenerateAllocationSummary();
        LauncherOptions options = new()
        {
            ContentType = "application/pdf"
        };
        _ = Launcher.LaunchUriAsync(new Uri(filePath), options);
    }

    private async Task PrintAllocationDetails()
    {
        var filePath = await ViewModel.AllocatorService.GenerateAllocationDetails();
        LauncherOptions options = new()
        {
            ContentType = "application/pdf"
        };
        _ = Launcher.LaunchUriAsync(new Uri(filePath), options);
    }

    private void AllocatedItemsGridControl_ValidateCell(object sender, Syncfusion.UI.Xaml.DataGrid.CurrentCellValidatingEventArgs e)
    {
        if(e.Column.MappingName == "QuanAllocated")
        {
            
            if((double)e.NewValue % 1 == 0 )
            {
                var newVal = (int)(double) (e.NewValue);
                var oldVal = (int)decimal.Truncate(decimal.Parse(e.OldValue.ToString()));
                if (e.RowData is OrderItem item)
                {
                    var inventoryItem = ViewModel.CurrentInventory.Where(i => i.ProductID == item.ProductID).FirstOrDefault();
                    if (inventoryItem != null && inventoryItem.QuantityOnHand >= newVal - oldVal) 
                    {
                        e.IsValid = true;
                        ViewModel.UpdateInventory(inventoryItem, (newVal - oldVal) * -1);

                    }
                    else
                    {
                        e.IsValid = false;
                        e.ErrorMessage = "Insufficent inventory";
                    }
                }
            }
            else
            {
                e.IsValid = false;
                e.ErrorMessage = "Value must be a whole number";
            }
        }
    }

    private async void PrintFlyoutButton_Click(object sender, RoutedEventArgs e)
    {
        if((MenuFlyoutItem)sender == btn_PrintSummary)
        {
            await PrintAllocationSummary();
        }
        else if ((MenuFlyoutItem)sender == btn_PrintDetails) 
        {
            await PrintAllocationDetails();
        }
    }
    #endregion Methods
}
