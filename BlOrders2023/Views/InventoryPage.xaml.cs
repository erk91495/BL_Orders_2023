using System.Diagnostics;
using System.Media;
using BlOrders2023.Contracts.Services;
using BlOrders2023.Exceptions;
using BlOrders2023.Helpers;
using BlOrders2023.Models;
using BlOrders2023.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class InventoryPage : Page
{  
    public InventoryPageViewModel ViewModel { get; }

    public InventoryPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<InventoryPageViewModel>();
        InventoryGrid.CellRenderers.Remove("TextBox");
        InventoryGrid.CellRenderers.Add("TextBox", new GridCellTextBoxRendererExt());
    }

    private void AdjustInventoryFlyout_Click(object sender, RoutedEventArgs e)
    {
        var s = App.GetService<INavigationService>();
        s.NavigateTo(typeof(InventoryAdjustmentsPageViewModel).FullName!, null);
    }

    private void InventoryAuditFlyout_Click(object sender, RoutedEventArgs e)
    {
        var s = App.GetService<INavigationService>();
        s.NavigateTo(typeof(InventoryAuditLogPageViewModel).FullName!, null);
    }

    private void EnableScanningFlyout_Click(object sender, RoutedEventArgs e)
    {
        Scanline.IsEnabled = true;
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
                        App.LogWarningMessage($"Could not parse _scanline {s} at {location}\r\nAI: {ai}");
                        await ShowLockedoutDialog(e.Message,
                            $"Could not parse _scanline {s} at {location}\r\nAI: {ai}");
                    }
                    catch (UnknownBarcodeFormatException e)
                    {
                        Debug.WriteLine(e.ToString());
                        App.LogWarningMessage($"{e.Message}");
                        await ShowLockedoutDialog("UnknownBarcodeFormatException", $"{e.Message}");
                    }

                try
                {
                    await ViewModel.AddInventoryItemAsync(item);
                }
                catch (DuplicateBarcodeException e) 
                {
                    var s = e.Data["Scanline"];
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
}
