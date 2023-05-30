// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using BlOrders2023.ViewModels;
using CommunityToolkit.WinUI.UI.Converters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls
{
    public sealed partial class CustomerDataInputControl : ContentDialog
    {
        #region Properties
        CustomerDataInputControlViewModel ViewModel { get; }
        #endregion Properties

        #region Fields
        //private ContentDialog _dialog;
        #endregion Fields

        #region Constructors
        public CustomerDataInputControl()
        {
            this.InitializeComponent();
            var enumValues = Enum.GetNames(typeof(States));
            StateComboBox.ItemsSource = enumValues;
            BillingStateComboBox.ItemsSource = enumValues;
            ViewModel = App.GetService<CustomerDataInputControlViewModel>();
            ViewModel.ErrorsChanged += ViewModel_ErrorsChanged;
        }

        private void ViewModel_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            if (ViewModel.HasErrors)
            {
                IsPrimaryButtonEnabled = false;
            }
            else
            {
                IsPrimaryButtonEnabled = true;
            }
        }
        #endregion Constructors

        #region Methods
        #endregion Methods

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
        }

        //private void SfMaskedTextBox_ValueChanged(object sender, Syncfusion.UI.Xaml.Editors.MaskedTextBoxValueChangedEventArgs e)
        //{
        //    23int i = 0;
        //}
    }
}
