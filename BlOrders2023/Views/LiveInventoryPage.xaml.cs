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
public sealed partial class LiveInventoryPage : Page
{  
    public LiveInventoryPageViewModel ViewModel { get; }

    public LiveInventoryPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<LiveInventoryPageViewModel>();
        InventoryGrid.CellRenderers.Remove("TextBox");
        InventoryGrid.CellRenderers.Add("TextBox", new GridCellTextBoxRendererExt());
    }
}
