using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using CommunityToolkit.WinUI;
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

namespace BlOrders2023.Dialogs;
public sealed partial class BillOfLadingDataInputDialog : ContentDialog
{
    #region Fields
    private List<Order> _orders;
    #endregion Fields

    #region Properties
    public DateTimeOffset? AppointmentDate { get; set; } = null;
    public DateTimeOffset? AppointmentTime { get; set; } = null;

    public ObservableCollection<BillOfLadingItem> Items { get; set; } = new();
    public string CarrierName { get; set; }
    public string TrailerNumber { get; set; }
    public string TrailerSeal { get; set; }
    #endregion Properties

    #region Constructors
    public BillOfLadingDataInputDialog(IEnumerable<Order> orders)
    {
        this.InitializeComponent();
        _orders = orders.ToList();
        _ = LoadItemsAsync();
    }

    private async Task LoadItemsAsync()
    {
        await DispatcherQueue.EnqueueAsync( () =>
        {
            foreach(var item in _orders.SelectMany(o => o.Items))
            {
                var foundItem = Items.Where(i => i.ProductID == item.ProductID).FirstOrDefault();
                if(foundItem == null)
                {
                    var tempItem = new BillOfLadingItem()
                    {
                        ProductID = item.ProductID,
                        ProductName = item.Product.ProductName,
                        NumberOfCases = item.QuantityReceived,
                        NetWt = (decimal)item.PickWeight,
                    };
                    Items.Add(tempItem);
                }
                else
                {
                    foundItem.NumberOfCases += item.QuantityReceived;
                    foundItem.NetWt += (decimal)item.PickWeight;
                }
            }
        });
    }
    #endregion Constructors
}
