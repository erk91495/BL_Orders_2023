using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Exceptions;
using BlOrders2023.Helpers;
using BlOrders2023.Models;
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
public sealed partial class InventoryReconciliationPage : Page
{
    public InventoryReconciliationPageViewModel ViewModel { get; }

    public InventoryReconciliationPage()
    {
        ViewModel = App.GetService<InventoryReconciliationPageViewModel>();
        this.InitializeComponent();
    }

    private async void Scanline_TextChanged(object sender, TextChangedEventArgs args)
    {
        if (sender is TextBox box)
        {
            var scanlineText = box.Text;
            if (scanlineText.Contains('\r'))
            {
                var scanline = scanlineText.Split('\r').First().Trim();
                box.Text = box.Text[(scanline.Length + 1)..];

                LiveInventoryItem item = new()
                {
                    ScanDate = DateTime.Now,
                    Scanline = scanline,
                };
                try
                {
                    //interpreter has no concept of dbcontext and cannot track items
                    BarcodeInterpreter.ParseBarcode(ref item);
                    ViewModel.VerifyProduct(item);
                }
                catch (ProductNotFoundException e)
                {
                    Debug.WriteLine(e.ToString());
                    var prodCode = e.Data["ProductID"];
                    App.LogWarningMessage($"Product ID: {prodCode} was not found in the database\r\n");
                    await ShowLockedoutDialog("Product Not Found",
                        $"Product ID: {prodCode} was not found in the database\r\n");
                }
                catch (InvalidBarcodeExcption e)
                {
                    Debug.WriteLine(e.ToString());
                    var ai = e.Data["AI"];
                    var s = e.Data["Scanline"];
                    var location = e.Data["Location"];
                    App.LogWarningMessage($"Could not parse scanline {s} at {location}\r\nAI: {ai}");
                    await ShowLockedoutDialog(e.Message,
                        $"Could not parse scanline {s} at {location}\r\nAI: {ai}");
                }
                catch (UnknownBarcodeFormatException e)
                {
                    Debug.WriteLine(e.ToString());
                    App.LogWarningMessage($"{e.Message}");
                    await ShowLockedoutDialog("UnknownBarcodeFormatException", $"{e.Message}");
                }

                if(!ViewModel.ScannedItems.Where(i => i.Scanline == item.Scanline).Any())
                {
                    ViewModel.ScannedItems.Add(item);
                }
                else
                {
                    var s = item.Scanline;
                    await ShowLockedoutDialog("Duplicate Scanline", $"Duplicate Scanline {s}\r\n");
                }
            }
        }
    }
    private async Task ShowLockedoutDialog(string title, string content)
    {
        ContentDialog d = new()
        {
            XamlRoot = XamlRoot,
            Title = title,
            Content = content,
            SecondaryButtonText = "Continue",
            DefaultButton = ContentDialogButton.None,
        };
        d.PreviewKeyDown += LockOutKeyPresses;
        SystemSounds.Exclamation.Play();
        await d.ShowAsync();
    }

    private void LockOutKeyPresses(object sender, KeyRoutedEventArgs e)
    {
        e.Handled = true;
    }

    internal async void BeginReconciliation_Click(object sender, RoutedEventArgs e)
    {
        var allItemsMatch = await ViewModel.ReconcileAsync();
        if (allItemsMatch)
        {
            var dialog = new ContentDialog()
            {
                XamlRoot = XamlRoot,
                Title = "No Changes",
                Content = "All scanned items were found in inventory",
                PrimaryButtonText = "ok",
            };
            await dialog.ShowAsync();
        }
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.SelectedItems = GetSelectedItems();
        await ViewModel.SaveSelected();
        var dialog = new ContentDialog()
        {
            XamlRoot = XamlRoot,
            Content = "Inventory Saved",
            PrimaryButtonText = "Ok",
        };
        _ =  await dialog.ShowAsync();
    }

    private ObservableCollection<InventoryReconciliationItem> GetSelectedItems()
    {
        var result = new ObservableCollection<InventoryReconciliationItem>();
        foreach (var item in ReconciledItemsGrid.SelectedItems)
        {
            result.Add((InventoryReconciliationItem) item);
        }
        return result;
    }
}
