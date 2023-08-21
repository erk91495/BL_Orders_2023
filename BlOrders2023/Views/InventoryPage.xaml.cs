using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using BlOrders2023.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.DataGrid;
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
    }


    private void InventoryGrid_CurrentCellValidating(object sender, CurrentCellValidatingEventArgs e)
    {
        if (e.RowData is InventoryItem item)
        {

            switch (e.Column.MappingName)
            {
                default:
                    {
                        break;
                    }
            }
        }
    }

    private void InventoryGrid_CurrentCellValidated(object sender, CurrentCellValidatedEventArgs e)
    {
        if (e.RowData is InventoryItem p)
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
