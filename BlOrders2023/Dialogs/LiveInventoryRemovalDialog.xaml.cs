using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using Microsoft.IdentityModel.Tokens;
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
public sealed partial class LiveInventoryRemovalDialog : ContentDialog
{
    private IEnumerable<LiveInventoryRemovalReason> _removalReasons;
    private string scanline;
    private LiveInventoryRemovalReason selectedReason;

    public LiveInventoryRemovalDialog()
    {
        this.InitializeComponent();
    }

    public LiveInventoryRemovalReason SelectedReason
    {
        get => selectedReason; 
        set 
        {
            selectedReason = value;
            TryEnablePrimaryButton();
        }
    }
    public IEnumerable<LiveInventoryRemovalReason> RemovalReasons
    {
        get => _removalReasons;
        set => _removalReasons = value;
    }
    public string Scanline
    {
        get => scanline; 
        set
        {
            scanline = value;
            TryEnablePrimaryButton();
        }
    }

    private void TryEnablePrimaryButton()
    {
        IsPrimaryButtonEnabled = !scanline.IsNullOrEmpty() && SelectedReason != null;
    }
}
