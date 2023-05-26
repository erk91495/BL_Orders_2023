using BlOrders2023.Models;
using BlOrders2023.ViewModels;
using BlOrders2023.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using BlOrders2023.Core.Exceptions;
using System.Diagnostics;
using Windows.Management.Update;
using System.Media;
using BlOrders2023.Contracts.ViewModels;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.IdentityModel.Tokens;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using BlOrders2023.UserControls;
using Microsoft.UI.Dispatching;
using CommunityToolkit.WinUI;

namespace BlOrders2023.Views;

public sealed partial class FillOrdersPage : Page
{
    #region Fields
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    #endregion Fields
    public FillOrdersPageViewModel ViewModel
    {
        get;
    }

    public FillOrdersPage()
    {
        ViewModel = App.GetService<FillOrdersPageViewModel>();
        InitializeComponent();

        //TODO: Is this the way to handle setting the default focus?
        dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low,
        new DispatcherQueueHandler(() =>
        {
            OrderLookup.Focus(FocusState.Programmatic);
        }));
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
                ShippingItem item = new()
                {
                    QuanRcvd = 1,
                    ScanDate = DateTime.Now,
                };
                try
                {
                    //interpreter has no concept of dbcontext and cannot track items
                    BarcodeInterpreter.ParseBarcode(bc, ref item);
                    var product = App.GetNewDatabase().Products.Get(item.ProductID, false).FirstOrDefault();
                    if (product != null)
                    {
                        item.Product = product;
                        await AddShippingItemAsync(item);
                    }
                    else
                    {
                        throw new ProductNotFoundException(String.Format("Product {0} Not Found", item.ProductID), item.ProductID);
                    }
                    
                }
                catch (ProductNotFoundException e)
                {
                    Debug.WriteLine(e.ToString());
                    var prodCode = e.Data["ProductID"];
                    await ShowLockedoutDialog("Product Not Found", 
                        String.Format("Product ID: {0} was not found in the database\r\n", prodCode)); 
                } catch (InvalidBarcodeExcption e)
                {
                    Debug.WriteLine(e.ToString());
                    var ai = e.Data["AI"];
                    var s = e.Data["Scanline"];
                    var location = e.Data["Location"];
                    await ShowLockedoutDialog(e.Message,
                        String.Format("Could not parse scanline {0} at {1}\r\nAI: {2}", s, location, ai));
                }

                
            }
        }
    }

    private async Task AddShippingItemAsync(ShippingItem item)
    {
        try
        {
            await ViewModel.ReceiveItemAsync(item);
            OrderedItems.ColumnSizer.ResetAutoCalculationforAllColumns();
            OrderedItems.ColumnSizer.Refresh();
        }
        catch (DuplicateBarcodeException e)
        {
            Debug.WriteLine(e.ToString());
            var s = e.Data["Scanline"];
            await ShowLockedoutDialog(e.Message,
                String.Format("Duplicate Scanline {0}", s));
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

    private void LockOutKeyPresses(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        e.Handled = true;
    }

    private void OrderLookup_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel.QueryFillableOrders(sender.Text);
        }
        else if (args.Reason == AutoSuggestionBoxTextChangeReason.SuggestionChosen)
        {
            OrderLookup.Text = null;
        }
    }

    private async void OrderLookup_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var input = args.QueryText;
        if (!ViewModel.FillableOrders.Where(e => e.OrderID.ToString().Equals(input)).IsNullOrEmpty())
        {
            await ViewModel.LoadOrder(int.Parse(input));
        }
        else if (args.ChosenSuggestion is Order o)
        {
            await ViewModel.LoadOrder(o.OrderID);
        }
    }

    private async void OrderedItems_RecordDeleted(object sender, Syncfusion.UI.Xaml.DataGrid.RecordDeletedEventArgs e)
    {
        if (e.Items.FirstOrDefault() is ShippingItem)
        {
            foreach (ShippingItem item in e.Items.Cast<ShippingItem>())
            {
                await ViewModel.DeleteShippingItemAsync(item);
            }
        }
    }

    private async void ManualProductAdd_Click(object sender, RoutedEventArgs e)
    {
        ShippingItemDataInputControl dialog = new(XamlRoot);
        var result = await dialog.ShowAsync();
        if (result != null)
        {
            await AddShippingItemAsync(result);
        }
    }

    private async void DeleteAll_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.DeleteAllShippingItemsAsync();
    }
}
