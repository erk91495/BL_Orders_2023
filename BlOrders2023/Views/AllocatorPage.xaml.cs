using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
    public DataTable AllocationTable { get; set; }
    #endregion Properties

    #region Constructors

    public AllocatorPage()
    {
        ViewModel = App.GetService<AllocatorPageViewModel>();
        this.InitializeComponent();
        _navigationService = App.GetService<INavigationService>() as NavigationService;
        AllocationTable = new("Allocation");
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
                ViewModel.AllocatorConfig.AllocationType = mode;
            }
            var dateTuple = await ShowDateRangeSelectionAsync();
            if (dateTuple.Item1 != null && dateTuple.Item2 != null)
            {
                ViewModel.AllocatorConfig.IDs = new() {67662,67663};
                await StartAllocationAsync();
                
                ViewModel.AllocatedOrders = new(ViewModel.AllocatorService.Orders);

            }
            else
            {
                throw new Exception("InvalidDate");
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
            TryGoBack();
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

    private void CreateAllocationTable()
    {
    }
    #endregion Methods


}
