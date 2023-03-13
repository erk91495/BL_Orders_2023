using BlOrders2023.Models;
using BlOrders2023.ViewModels;
using BlOrders2023.Core.Services;

using Microsoft.UI.Xaml.Controls;

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

    private void Scanline_TextChanged(object sender, TextChangedEventArgs e)
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
                BarcodeInterpreter.ParseBarcode(bc,ref item);
            }
        }
        
    }
}
