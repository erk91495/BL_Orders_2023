﻿using BlOrders2023.Activation;
using BlOrders2023.Contracts.Services;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Data;
using BlOrders2023.Core.Data.SQL;
using BlOrders2023.Core.Services;
using BlOrders2023.Helpers;
using BlOrders2023.Models;
using BlOrders2023.Services;
using BlOrders2023.ViewModels;
using BlOrders2023.Views;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace BlOrders2023;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    private static IBLDatabase BLDatabase { get;  set; }
    public static DbContextOptions<BLOrdersDBContext> DBOptions { get; private set; }

    public App()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt+QHFqUUdrXVNbdV5dVGpAd0N3RGlcdlR1fUUmHVdTRHRcQllhTX5bdEZjXH9deHM=;Mgo+DSMBPh8sVXJ1S0d+WFBPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSXpRckVrW3xedXRQQWc=;ORg4AjUWIQA/Gnt2VFhhQlVFfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5XdEFjUX1dcHFVQWVb;MTgyMjYyN0AzMjMxMmUzMTJlMzQzMVdnYlQ1Z1VXd0FCaG4zRmNDVWpNN1lwVy9NcndYamRmNm9jS2pLMHV5alk9;MTgyMjYyOEAzMjMxMmUzMTJlMzQzMW1xVDNQVVBjeVFhOFlLUWZSZ0M1c2FWVDlpWHk4d2c2SnB6VUUvQzQ0T3c9;NRAiBiAaIQQuGjN/V0d+XU9Ad1RDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31TckdnWXdcc3RQR2hYVg==;MTgyMjYzMEAzMjMxMmUzMTJlMzQzMWxPUk5raFp4ZlFwMTdmcUlMWHFlTTUxdGZrUWRqUjhoZ3hBMWYrUTRwUDA9;MTgyMjYzMUAzMjMxMmUzMTJlMzQzMVVnWkQ1RnBFa2haR0pRZHNDSkFMRy9IakErNFV1SmFZVmpCOGdXRjg1cjQ9;Mgo+DSMBMAY9C3t2VFhhQlVFfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5XdEFjUX1dcHFWQGhb;MTgyMjYzM0AzMjMxMmUzMTJlMzQzMU55MDcrTXpmaUxZVjVTRHdEaVE4NlJXcW9Oa292YWZZaUtvUkVUTjhOUkk9;MTgyMjYzNEAzMjMxMmUzMTJlMzQzMUdDcFJ2dVhVV3ZSVGdPdzlnU2kranZxMW9OblU2N2VXNkZ6cjNUVlVRMjQ9;MTgyMjYzNUAzMjMxMmUzMTJlMzQzMWxPUk5raFp4ZlFwMTdmcUlMWHFlTTUxdGZrUWRqUjhoZ3hBMWYrUTRwUDA9");
        InitializeComponent();
        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers
           

            // Services
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddTransient<FillOrdersPageViewModel>();
            services.AddTransient<FillOrdersPage>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<OrdersPageViewModel>();
            services.AddTransient<OrdersPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<OrderDetailsPage>();
            services.AddTransient<OrderDetailsPageViewModel>();
            services.AddTransient<ProductsPage>();
            services.AddTransient<ProductsPageViewModel>();
            services.AddTransient<CustomerSelectionViewModel>();
            services.AddTransient<CustomerDataInputControlViewModel>();
            services.AddTransient<ShippingItemDataInputControlViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        var dbOptions = new DbContextOptionsBuilder<BLOrdersDBContext>();
        dbOptions.UseLazyLoadingProxies()
               .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
               .EnableSensitiveDataLogging()
               .UseSqlServer(connectionString: "Data Source=ERIC-PC; Database=New_Bl_Orders;Integrated Security=true; Trust Server Certificate=true");


        App.DBOptions = dbOptions.Options;
        await App.GetService<IActivationService>().ActivateAsync(args);

    }
    public static IBLDatabase GetNewDatabase()
    {
        return new SqlBLOrdersDatabase(App.DBOptions);
    }
}
