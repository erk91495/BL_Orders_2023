﻿using BlOrders2023.Contracts.Services;
using BlOrders2023.Helpers;
using BlOrders2023.ViewModels;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using Windows.System;

namespace BlOrders2023.Views;

/// <summary>
/// The shell page for navigation and holding other pages
/// </summary>
public sealed partial class ShellPage : Page
{
    #region Properties
    /// <summary>
    /// The viewmodel for the shell page
    /// </summary>
    public ShellViewModel ViewModel
    {
        get;
    }
    #endregion Properties

    #region Fields
    #endregion Fields

    #region Constructors
    /// <summary>
    /// Initializes and instance of the ShellPage
    /// </summary>
    /// <param name="viewModel">A view model for the page</param>
    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);

        //TODO: Set the _title bar icon by updating /Assets/WindowIcon.ico.
        // A custom _title bar is required for full window theme and Mica support.
        // https://docs.microsoft.com/windows/apps/develop/_title-bar?tabs=winui3#full-customization
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        AppTitleBarText.Text = "AppDisplayName".GetLocalized();
        NavigationFrame.Navigated += NavigationFrame_Navigated;
    }
    #endregion Constructors

    #region Methods
    /// <summary>
    /// Called when the page is loaded.
    /// </summary>
    /// <param name="sender">The page that was loaded</param>
    /// <param name="e">The event args</param>
    private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);

        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
    }

    /// <summary>
    /// Called when the main window is activated
    /// </summary>
    /// <param name="sender">the main window</param>
    /// <param name="args">the event args for the main window</param>
    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        var resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";

        AppTitleBarText.Foreground = (SolidColorBrush)App.Current.Resources[resource];
        App.AppTitlebar = AppTitleBarText as UIElement;
    }

    /// <summary>
    /// Called when 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        //AppTitleBar.Margin = new Thickness()
        //{
        //    Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
        //    Top = AppTitleBar.Margin.Top,
        //    Right = AppTitleBar.Margin.Right,
        //    Bottom = AppTitleBar.Margin.Bottom
        //};
    }

    /// <summary>
    /// Created a new keyboard accelerator with the given key and modifiers
    /// </summary>
    /// <param name="key">The key to bind to</param>
    /// <param name="modifiers">modifiers to bind to the key</param>
    /// <returns></returns>
    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    /// <summary>
    /// Handles Keyboard acceleratiors
    /// </summary>
    /// <param name="sender">the accelerator  invoked</param>
    /// <param name="args">The event args</param>
    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        var navigationService = App.GetService<INavigationService>();

        var result = navigationService.GoBack();

        args.Handled = result;
    }

    private void NavigateToProducts()
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService?.Frame?.Navigate(typeof(ProductsPage));
    }

    private void MenuFlyoutProducts_Click(object sender, RoutedEventArgs e)
    {
        NavigateToProducts();
    }

    private void MenuFlyoutCustomerClasses_Click(object sender, RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService?.Frame?.Navigate(typeof(CustomerClassesPage));
    }

    private void MenuFlyoutItemAllocation_Click(object sender, RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService?.Frame?.Navigate(typeof(AllocatorPage));
    }

    private void MenuFlyoutInventory_Click(object sender, RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService?.Frame?.Navigate(typeof(InventoryPage));
    }

    private void MenuFlyoutPayments_Click(object sender, RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService?.Frame?.Navigate(typeof(PaymentsPage));
    }

    private void MenuFlyoutSettings_Click(object sender, RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService?.Frame?.Navigate(typeof(SettingsPage));
    }

    private void NavigationFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        DatabaseName.Text = App.GetNewDatabase().DbConnection.Database.Replace('_', ' ');
    }
    #endregion Methods
}
