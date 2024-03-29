using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using BlOrders2023.Dialogs.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
using Windows.Globalization.NumberFormatting;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Dialogs;
public sealed partial class ProductDataInputDialog : ContentDialog
{

    #region Fields
    private ProductDataInputDialogViewModel ViewModel { get; }
    #endregion Fields

    public event PropertyChangedEventHandler? PropertyChanged;

    #region Properties
    public Product Product
    {
        get => ViewModel.Product; 
        private set => ViewModel.Product = value;
    }

    #endregion Properties
    public ProductDataInputDialog(Product prod)
    {
        this.InitializeComponent();
        ViewModel = App.GetService<ProductDataInputDialogViewModel>();
        Product = prod;


        IncrementNumberRounder rounder = new IncrementNumberRounder();
        rounder.Increment = 0.01;
        rounder.RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp;

        DecimalFormatter formatter = new DecimalFormatter();
        formatter.IntegerDigits = 1;
        formatter.FractionDigits = 2;
        formatter.NumberRounder = rounder;
        WholesalePriceBox.NumberFormatter = formatter;
        KetteringPriceBox.NumberFormatter = formatter;
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        //if(e.AddedItems.First() is Box selecteBox) 
        //{
        //    ViewModel.Box = selecteBox;
        //}
    }
}
