// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BlOrders2023.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls
{
    public sealed partial class CustomerDataInputControl : ContentControl
    {
        #region Properties
        CustomerDataInputControlViewModel ViewModel { get; }
        #endregion Properties

        #region Fields
        private ContentDialog _dialog;
        #endregion Fields

        #region Constructors
        public CustomerDataInputControl(XamlRoot root)
        {
            this.InitializeComponent();
            _dialog = new();
            _dialog.XamlRoot = root;
            _dialog.Content = this;
            _dialog.PrimaryButtonText = "Create Customer";
            _dialog.CloseButtonText = "Cancel";
            _dialog.FlowDirection = FlowDirection.LeftToRight;
            _dialog.IsPrimaryButtonEnabled = false;
            ViewModel = App.GetService<CustomerDataInputControlViewModel>();
        }
        #endregion Constructors

        #region Methods

        public async Task<int?> ShowAsync()
        {
            await _dialog.ShowAsync();
            return 0;
        }
        #endregion Methods
    }
}
