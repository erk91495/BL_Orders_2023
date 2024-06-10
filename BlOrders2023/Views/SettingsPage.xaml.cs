using BlOrders2023.Contracts.Services;
using BlOrders2023.Helpers;
using BlOrders2023.Dialogs;
using BlOrders2023.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace BlOrders2023.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
        AddPageSelctionItems();
    }

    private void AddPageSelctionItems()
    {
        PageSelectionCombo.Items.Add("Orders Page");
        PageSelectionCombo.Items.Add("Settings Page");
        PageSelectionCombo.Items.Add("Fill Orders Page");
        PageSelectionCombo.Items.Add("Inventory Page");
        var settings = App.GetService<ILocalSettingsService>();
        var startPage = settings.ReadSetting<string>(LocalSettingsKeys.StartupPage);
        if(startPage == typeof(OrdersPageViewModel).FullName)
        {
            PageSelectionCombo.SelectedValue = "Orders Page";
        }
        else if (startPage == typeof(SettingsViewModel).FullName) 
        {
            PageSelectionCombo.SelectedValue = "Settings Page";
        }
        else if (startPage == typeof(FillOrdersPageViewModel).FullName)
        {
            PageSelectionCombo.SelectedValue = "Fill Orders Page";
        }
        else if (startPage == typeof(InventoryPageViewModel).FullName)
        {
            PageSelectionCombo.SelectedValue = "Inventory Page";
        }
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        var settingsService = App.GetService<ILocalSettingsService>();
        settingsService.SaveSetting(LocalSettingsKeys.DBConnectionString, DatabaseSettings.ConnectionString);
        settingsService.SaveSetting(LocalSettingsKeys.DBSettingsSet, true);
        base.OnNavigatingFrom(e);
    }

    private async void HyperlinkButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        IssueSubmitterInputDialog dialog = new() { XamlRoot = XamlRoot};
        await dialog.ShowAsync();
    }

    private async void PageSelectionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var settings = App.GetService<ILocalSettingsService>();
        if(sender is ComboBox box)
        {
            switch (box.SelectedValue)
            {
                case "Orders Page":
                    {
                        await settings.SaveSettingAsync(LocalSettingsKeys.StartupPage, typeof(OrdersPageViewModel).FullName);
                        break;
                    }
                case "Settings Page":
                    {
                        await settings.SaveSettingAsync(LocalSettingsKeys.StartupPage, typeof(SettingsViewModel).FullName);
                        break;
                    }
                case "Fill Orders Page":
                    {
                        await settings.SaveSettingAsync(LocalSettingsKeys.StartupPage, typeof(FillOrdersPageViewModel).FullName);
                        break;
                    }
                case "Inventory Page":
                    {
                        await settings.SaveSettingAsync(LocalSettingsKeys.StartupPage, typeof(InventoryPageViewModel).FullName);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
