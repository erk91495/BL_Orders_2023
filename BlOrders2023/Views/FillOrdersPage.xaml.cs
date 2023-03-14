using BlOrders2023.Models;
using BlOrders2023.ViewModels;
using BlOrders2023.Helpers;

using Microsoft.UI.Xaml.Controls;
using BlOrders2023.Core.Exceptions;
using System.Diagnostics;
using Windows.Management.Update;
using System.Media;

namespace BlOrders2023.Views;

public sealed partial class FillOrdersPage : Page
{
    public FillOrdersPageViewModel ViewModel
    {
        get;
    }

    public FillOrdersPage()
    {
        ViewModel = App.GetService<FillOrdersPageViewModel>();
        InitializeComponent();
    }

    private async void Scanline_TextChanged(object sender, TextChangedEventArgs args)
    {
        if(sender is TextBox box )
        {
            String scanlineText = box.Text;
            if (scanlineText.EndsWith('\r'))
            {
                var scanline = scanlineText;
                box.Text = null;
                GS1_128Barcode bc = new()
                {
                    Scanline = scanline,
                };
                ShippingItem item = new();
                try
                {
                    BarcodeInterpreter.ParseBarcode(bc, ref item);
                }catch (ProductNotFoundException e)
                {
                    Debug.WriteLine(e.ToString());
                    var prodCode = e.Data["ProductID"];
                    ContentDialog d = new()
                    {
                        XamlRoot = XamlRoot,
                        Title = "Product Not Found",
                        Content = String.Format("Product ID: {0} was not found in the database\r\n", prodCode),
                        SecondaryButtonText = "Continue",
                        DefaultButton = ContentDialogButton.None,
                        
                    };
                    d.PreviewKeyDown += LockOutKeyPresses;
                    SystemSounds.Exclamation.Play();
                    await d.ShowAsync();
                    
                }
            }
        }
    }

    private void LockOutKeyPresses(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        e.Handled = true;
    }
}
