using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;
using BlOrders2023.Contracts.Services;
using BlOrders2023.Models;
using BlOrders2023.Services;
using BlOrders2023.UserControls;
using BlOrders2023.ViewModels;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.DataGrid;
using Syncfusion.UI.Xaml.Grids.ScrollAxis;
using Syncfusion.XPS;
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
    private bool _Modified = false;
    public InventoryPageViewModel ViewModel { get; }

    public InventoryPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<InventoryPageViewModel>();
        InventoryGrid.CellRenderers.Remove("TextBox");
        InventoryGrid.CellRenderers.Add("TextBox", new GridCellNumericRendererExt());
    }


    private void InventoryGrid_CurrentCellValidating(object sender, CurrentCellValidatingEventArgs e)
    {
        if (e.RowData is InventoryItem item)
        {

            if (e.OldValue != e.NewValue)
            {
                _Modified = true;
                switch (e.Column.MappingName)
                {
                    case "AdjustmentQuantity":
                        {
                            if(! int.TryParse(e.NewValue.ToString(), out _))
                            {
                                e.ErrorMessage = "Adjustment Quantity Must Be A Whole Number";
                                e.IsValid = false;
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
    }

    private void InventoryGrid_CurrentCellValidated(object sender, CurrentCellValidatedEventArgs e)
    {
        if (e.RowData is InventoryItem p)
        {
            Collection<ValidationResult> result = new();
            ValidationContext context = new(p);
            Validator.TryValidateObject(p, context, result, true);

        }
    }

    private async void btn_Cancel_Click(object sender, RoutedEventArgs e)
    {
        if(_Modified)
        {
            ContentDialog diag = new()
            {
                XamlRoot = XamlRoot,
                Title = "Unsaved Edits",
                Content = "Are you sure you want to discard all unsaved changes?",
                PrimaryButtonText = "Discard",
                SecondaryButtonText = "Cancel",
            };
            var res = await diag.ShowAsync();
            if(res == ContentDialogResult.Primary)
            {
                if (App.GetService<INavigationService>() is NavigationService navigation && navigation.CanGoBack)
                {
                    navigation.GoBack();
                }
            }

        }
        else
        {
            if (App.GetService<INavigationService>() is NavigationService navigation && navigation.CanGoBack)
            {
                navigation.GoBack();
            }
        }
    }

    private async void btn_Save_Click(object sender, RoutedEventArgs e)
    {
        if (_Modified)
        {
            await ViewModel.SaveAllAsync();
            ContentDialog contentDialog = new()
            {
                XamlRoot = XamlRoot,
                Content = "Inventory Saved",
                PrimaryButtonText = "Ok",
                DefaultButton = ContentDialogButton.Primary,
            };
            await contentDialog.ShowAsync();
        }
        else
        {
            await ViewModel.SaveAllAsync();
            ContentDialog contentDialog = new()
            {
                XamlRoot = XamlRoot,
                Content = "No changes were made",
                PrimaryButtonText = "Ok",
                DefaultButton = ContentDialogButton.Primary,
            };
            await contentDialog.ShowAsync();
        }
    }

}
