using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    #region Fields
    private IEnumerable<LiveInventoryRemovalReason> _removalReasons;
    private ObservableCollection<string> _scanlines = new();
    private LiveInventoryRemovalReason selectedReason;
    #endregion Fields

    #region Properties
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

    public ObservableCollection<string> Scanlines
    {
        get => _scanlines;
        set => _scanlines = value;
    }
    #endregion Properties

    #region Constructors
    public LiveInventoryRemovalDialog()
    {
        this.InitializeComponent();
    }
    #endregion Constructors

    #region Methods
    private void TryEnablePrimaryButton()
    {
        IsPrimaryButtonEnabled = !Scanlines.IsNullOrEmpty() && SelectedReason != null;
    }

    private void BarcodeInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox box)
        {

            var scanlineText = box.Text;
            if (scanlineText.Contains('\r'))
            {
                var scanline = scanlineText.Split('\r').First().Trim();
                box.Text = box.Text[(scanline.Length + 1)..];
                if (!Scanlines.Contains(scanline))
                {
                    Scanlines.Add(scanline);
                    TryEnablePrimaryButton();
                }
            }
        }

    }
    #endregion Methods
}
