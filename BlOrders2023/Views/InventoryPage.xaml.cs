using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;
using BlOrders2023.Contracts.Services;
using BlOrders2023.Helpers;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Services;
using BlOrders2023.UserControls;
using BlOrders2023.ViewModels;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.DataGrid;
using Syncfusion.UI.Xaml.Grids.ScrollAxis;
using Syncfusion.XPS;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
}
