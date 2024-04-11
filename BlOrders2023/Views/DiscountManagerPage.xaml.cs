using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using BlOrders2023.ViewModels;
using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.UserControls;
using BlOrders2023.Models;
using Castle.Core.Resource;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DiscountManagerPage : Page
{
    #region Fields
    private DiscountManagerPageViewModel ViewModel;
    #endregion Fields
    public DiscountManagerPage()
    {
        ViewModel = App.GetService<DiscountManagerPageViewModel>();
        this.InitializeComponent();
    }

    private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new DiscountDataEntryControl(ViewModel.Products,ViewModel.Customers, ViewModel.SelectedDiscount)
        {
            XamlRoot = XamlRoot,
            //Content = new DiscountDataEntryControl(ViewModel.SelectedDiscount),
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Cancel",
        };
        var res = await dialog.ShowAsync();
        if(res == ContentDialogResult.Primary)
        {
            var index = ViewModel.Discounts.IndexOf(ViewModel.SelectedDiscount);
            ViewModel.Discounts[index] = dialog.Discount;
            await TrySaveDiscountAsync(dialog.Discount);
        }
    }

    private async void AppBarButton_Click(object sender, RoutedEventArgs e)
    {
        var discount = new Discount();
        var dialog = new DiscountDataEntryControl(ViewModel.Products, ViewModel.Customers, discount)
        {
            XamlRoot = XamlRoot,
            //Content = new DiscountDataEntryControl(ViewModel.SelectedDiscount),
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Cancel",
        };
        var res = await dialog.ShowAsync();
        if (res == ContentDialogResult.Primary)
        {
            ViewModel.Discounts.Add(discount);
            await TrySaveDiscountAsync(discount);
        }
    }

    private async Task TrySaveDiscountAsync(Discount discount)
    {
        try
        {
            ViewModel.SaveDiscountAsync(discount);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _ = ex;
            App.LogException(ex);
            ContentDialog d = new()
            {
                XamlRoot = XamlRoot,
                Title = "Database Write Conflict",
                Content = $"Your changes could not be saved",
                SecondaryButtonText = "Continue",
                DefaultButton = ContentDialogButton.None,

            };
            await d.ShowAsync();
        }
        catch (DbUpdateException ex)
        {
            App.LogException(ex);
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

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        _ = ViewModel.LoadData();
    }
}
