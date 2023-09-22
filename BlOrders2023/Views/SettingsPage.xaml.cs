﻿using BlOrders2023.Contracts.Services;
using BlOrders2023.Helpers;
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
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        var settingsService = App.GetService<ILocalSettingsService>();
        settingsService.SaveSetting(LocalSettingsKeys.DBConnectionString, DatabaseSettings.ConnectionString);
        base.OnNavigatingFrom(e);
    }
}
