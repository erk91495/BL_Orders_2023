using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using BlOrders2023.Exceptions;
using BlOrders2023.Helpers;
using BlOrders2023.Models;
using System.Diagnostics;
using System.Media;
using BlOrders2023.ViewModels;
using Microsoft.EntityFrameworkCore;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddLiveInventoryPage : Page
    {
        #region Fields
        private AddLiveInventoryPageViewModel ViewModel { get; }
        private bool hasChanges = false;
        private bool canLeave = false;
        #endregion Fields

        public AddLiveInventoryPage()
        {
            ViewModel = App.GetService<AddLiveInventoryPageViewModel>();
            this.InitializeComponent();
            this.Unloaded += OrderDetailsPage_Unloaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.MainWindow.Closed += AddLiveInventoryPage_Closed;
        }

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if(!canLeave){
                if (hasChanges)
                {
                    e.Cancel = true;
                    ContentDialog dialog = new ContentDialog()
                    {
                        XamlRoot = XamlRoot,
                        Title = "Unsaved Changes",
                        Content = "Are you sure you want to leave? Unsaved changes will be discarded.",
                        PrimaryButtonText = "Leave",
                        CloseButtonText = "Cancel",
                    };

                    var res = await dialog.ShowAsync();
                    if(res == ContentDialogResult.Primary)
                    {
                        e.Cancel = false;
                        canLeave = true;
                        Frame.Navigate(e.SourcePageType, e.Parameter);
                    }
                }
            }
        }


        private void OrderDetailsPage_Unloaded(object sender, RoutedEventArgs e)
        {
            App.MainWindow.Closed -= AddLiveInventoryPage_Closed;
        }

        private async void AddLiveInventoryPage_Closed(object sender, WindowEventArgs args)
        {
            if (hasChanges)
            {
                args.Handled = true;
                ContentDialog dialog = new ContentDialog()
                {
                    XamlRoot = XamlRoot,
                    Title = "Close Application",
                    Content = "Are you sure you want to exit the application? Unsaved changes will be discarded.",
                    PrimaryButtonText = "Exit",
                    CloseButtonText = "Cancel",
                };

                var res = await dialog.ShowAsync();
                if (res == ContentDialogResult.Primary)
                {
                    if (sender is MainWindow window)
                    {
                        App.MainWindow.Closed -= AddLiveInventoryPage_Closed;
                        App.MainWindow.Close();
                    }
                }
            }
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


                        if (!ViewModel.ScannedItems.Where(i => i.Scanline == item.Scanline).Any())
                        {
                            ViewModel.ScannedItems.Add(item);
                            hasChanges = true;
                        }
                        else
                        {
                            var s = item.Scanline;
                            await ShowLockedoutDialog("Duplicate Scanline", $"Duplicate Scanline {s}\r\n");
                        }
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
                        App.LogWarningMessage($"Could not parse Scanline {s} at {location}\r\nAI: {ai}");
                        await ShowLockedoutDialog(e.Message,
                            $"Could not parse Scanline {s} at {location}\r\nAI: {ai}");
                    }
                    catch (UnknownBarcodeFormatException e)
                    {
                        Debug.WriteLine(e.ToString());
                        App.LogWarningMessage($"{e.Message}");
                        await ShowLockedoutDialog("UnknownBarcodeFormatException", $"{e.Message}");
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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                b.IsEnabled = false;
                Scanline.IsEnabled = false; 
            }
            try
            {
                await ViewModel.SaveItems();
                ContentDialog d = new()
                {
                    XamlRoot = XamlRoot,
                    Title = "Saved",
                    Content = $"The Items have been saved",
                    PrimaryButtonText = "OK",
                    DefaultButton = ContentDialogButton.Primary,

                };
                await d.ShowAsync();
                hasChanges = false;
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
    }
}
