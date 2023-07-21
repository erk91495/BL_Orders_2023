// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BlOrders2023.Models;
using BlOrders2023.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomerClassesPage : Page
    {
        #region Properties
        #endregion Properties

        #region Fields
        CustomerClassesPageViewModel ViewModel { get; }
        #endregion Fields

        #region Constructors
        public CustomerClassesPage()
        {
            ViewModel = App.GetService<CustomerClassesPageViewModel>();
            this.InitializeComponent();
        }
        #endregion Constructors

        #region Methods
        #endregion Methods

        private void CustomerClassesGrid_AddNewRowInitiating(object sender, Syncfusion.UI.Xaml.DataGrid.AddNewRowInitiatingEventArgs e)
        {

        }
        private void CustomerClassesGrid_CurrentCellValidating(object sender, Syncfusion.UI.Xaml.DataGrid.CurrentCellValidatingEventArgs e)
        {
            if (e.RowData is CustomerClass custClass)
            {

                switch (e.Column.MappingName)
                {
                    case "Class":
                        {
                            if (custClass.Class.IsNullOrEmpty())
                            {
                                e.IsValid = false;
                                e.ErrorMessage = "The Class cannot be empty";
                            }
                            else if(custClass.Class.Length > 30)
                            {
                                e.IsValid = false;
                                e.ErrorMessage = "30 character max length";
                            }
                            break;
                        }
                    case "DiscountPercent":
                        {
                            if(custClass.DiscountPercent > 100)
                            {
                                e.IsValid = false;
                                e.ErrorMessage = "The discount cannont be larger than 100%";
                            }
                            else if(custClass.DiscountPercent < 0)
                            {
                                e.IsValid = false;
                                e.ErrorMessage = "The discount cannont be less than 0%";
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        private void CustomerClassesGrid_RecordDeleted(object sender, Syncfusion.UI.Xaml.DataGrid.RecordDeletedEventArgs e)
        {

        }

        private void CustomerClassesGrid_CurrentCellValidated(object sender, Syncfusion.UI.Xaml.DataGrid.CurrentCellValidatedEventArgs e)
        {
            if (e.RowData is CustomerClass p)
            {
                Collection<ValidationResult> result = new();
                ValidationContext context = new(p);
                if (Validator.TryValidateObject(p, context, result, true))
                {
                    ViewModel.SaveItem(p);
                }
            }
        }
    }
}
