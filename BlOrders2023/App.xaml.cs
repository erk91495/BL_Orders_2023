using BlOrders2023.Activation;
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
using BlOrders2023.UserControls.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using QuestPDF.Infrastructure;

using Windows.ApplicationModel;

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

    public static DbContextOptions<BLOrdersDBContext>? DBOptions { get; private set; }

    public App()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NGaF5cXmVCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdgWXlceXRSQmBZUU1+XUc=");
        QuestPDF.Settings.License = LicenseType.Community;

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
            services.AddTransient<WholesaleCustomersViewModel>();
            services.AddTransient<WholesaleCustomersPage>();
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
            services.AddTransient<ReportsPage>();
            services.AddTransient<ReportsPageViewModel>();
            services.AddTransient<CustomerSelectionDialogViewModel>();
            services.AddTransient<CustomerDataInputControlViewModel>();
            services.AddTransient<ShippingItemDataInputControlViewModel>();
            services.AddTransient<MultipleCustomerSelectionDialogViewModel>();
            services.AddTransient<CustomerClassesPageViewModel>();
            services.AddTransient<CustomerClassesPage>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        var message = $"{e.Message}";
        System.Windows.Forms.MessageBox.Show(message, $"{"AppDisplayName".GetLocalized()} Unhandled Exception", System.Windows.Forms.MessageBoxButtons.OK);
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        var localsettings = App.GetService<ILocalSettingsService>();
        //var dbServer = await localsettings.ReadSettingAsync<string>(LocalSettingsKeys.DatabaseServer);
        var dbServer = "ERIC-PC";
        var dbName = await localsettings.ReadSettingAsync<string>(LocalSettingsKeys.DatabaseName);
        var dbOptions = new DbContextOptionsBuilder<BLOrdersDBContext>();
        dbOptions.UseLazyLoadingProxies()
               .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
               .EnableSensitiveDataLogging()
               .EnableDetailedErrors()
               .UseSqlServer(connectionString: $"Data Source={dbServer}; Database={dbName};Integrated Security=true; Trust Server Certificate=true");
        App.DBOptions = dbOptions.Options;

        var SupportedDBVersion = new Version(0, 0, 1);
        var DBVersion = App.GetNewDatabase().dbVersion;
        if (!SupportedDBVersion.Equals(DBVersion))
        {
            var version = Windows.ApplicationModel.Package.Current.Id.Version;
            var versionString = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            var message = "The application version you are running is not compatible with the current Database version\r\n" +
                $"Please install the latest version of the application\r\nApplication Version: {versionString}\r\n"+
                $"Supported Database Version: {SupportedDBVersion}. Actual Database Version: {DBVersion}";
            System.Windows.Forms.MessageBox.Show(message, $"{"AppDisplayName".GetLocalized()} DatabaseVersionMismatch", System.Windows.Forms.MessageBoxButtons.OK);
            Exit();
        }
        await App.GetService<IActivationService>().ActivateAsync(args);

    }
    public static IBLDatabase GetNewDatabase()
    {
        return new SqlBLOrdersDatabase(App.DBOptions);
    }
}
