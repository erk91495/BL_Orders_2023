// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

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
using BlOrders2023.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Dispatching;
using Syncfusion.UI.Xaml.DataGrid;
using Windows.Globalization.NumberFormatting;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views
{
    /// <summary>
    /// A page to display _products
    /// </summary>
    public sealed partial class ProductsPage : Page
    {
        #region Properties
        public ProductsPageViewModel ViewModel { get; }
        #endregion Properties

        #region Fields
        #endregion Fields

        #region Constructors
        public ProductsPage()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<ProductsPageViewModel>();
            ColumnWholesalePrice.NumberFormatter = new DecimalFormatter();
        }
        #endregion Constructors

        #region Methods

        #endregion Methods

        private void ProductsGrid_RecordDeleted(object sender, Syncfusion.UI.Xaml.DataGrid.RecordDeletedEventArgs e)
        {
            
        }

        private void ProductsGrid_CurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.DataGrid.CurrentCellValueChangedEventArgs e)
        {
            if (e.Record is Product p) 
            {
                ViewModel.SaveItem(p);
            }
        }
    }
}
