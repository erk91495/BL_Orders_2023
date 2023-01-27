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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views
{
    /// <summary>
    /// A page to display products
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
        }
        #endregion Constructors

        #region Methods

        #endregion Methods
    }
}
