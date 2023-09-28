using BlOrders2023.Activation;
using BlOrders2023.Contracts.Services;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Data;
using BlOrders2023.Core.Data.SQL;
using BlOrders2023.Core.Services;
using BlOrders2023.Models;
using BlOrders2023.Services;
using BlOrders2023.ViewModels;
using BlOrders2023.Views;
using BlOrders2023.UserControls.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using QuestPDF.Infrastructure;

using Windows.ApplicationModel;
using Microsoft.UI.Dispatching;
using Microsoft.Data.SqlClient;
using BlOrders2023.Exceptions;
using CommunityToolkit.WinUI;
using System.Diagnostics;

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
	
	public static UIElement? AppTitlebar { get; set; }
	
    public static DbContextOptions<SqlBLOrdersDBContext>? DBOptions 
    { 
        get
        {
            var localsettings = App.GetService<ILocalSettingsService>();
            var dbOptions = new DbContextOptionsBuilder<SqlBLOrdersDBContext>();
            dbOptions.UseLazyLoadingProxies()
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
                   .EnableSensitiveDataLogging()
                   .EnableDetailedErrors()
                   .UseSqlServer(connectionString: localsettings.ReadSetting<string>(Helpers.LocalSettingsKeys.DBConnectionString));
            return dbOptions.Options;
        }
    }

    public App()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF5cXmVCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdgWXZfeXRSQmFdVEB/V0U=");
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
            services.AddTransient<AllocatorPage>();
            services.AddTransient<AllocatorPageViewModel>();
            services.AddTransient<InventoryPage>();
            services.AddTransient<InventoryPageViewModel>();
            services.AddTransient<PaymentsPage>();
            services.AddTransient<PaymentsPageViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        Build();

        UnhandledException += App_UnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


        var db = App.GetNewDatabase();
#if DEBUG
        if (db.DbConnection.DataSource == "BL4" && db.DbConnection.Database == "BL_Enterprise")
        {
            var message = "You are running a production DB with a debug application version.";
            System.Windows.Forms.MessageBox.Show(message, $"WARNING", System.Windows.Forms.MessageBoxButtons.OK);
        }
#endif // DEBUG
    }

    private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        if(e.ExceptionObject is Exception ex)
        {
            var message = $"{ex.ToString()} \r\n";
            var title = $"{"AppDisplayName".GetLocalized()} {nameof(e)}";
            try
            {
                if (MainWindow.Content is ShellPage shell)
                {
                    ContentDialog dialog = new ContentDialog()
                    {
                        XamlRoot = shell.XamlRoot,
                        Content = message,
                        Title = title,
                        PrimaryButtonText = "ok"
                    };
                    _ = dialog.ShowAsync();
                }
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show(message, title, System.Windows.Forms.MessageBoxButtons.OK);
            }
        }
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        if(e.Exception is SqlException || e.Exception is VersionMismatchException)
        {
            if (App.GetService<INavigationService>() is NavigationService nav)
            {
                nav.NavigateTo(typeof(SettingsViewModel).FullName!);
                e.Handled = true;
            }
        }

        var message = $"{e.Message} \r\n";
        var title = $"{"AppDisplayName".GetLocalized()} {nameof(e)}";
        try{
            if (MainWindow.Content is ShellPage shell)
            {
                ContentDialog dialog = new ContentDialog()
                {
                    XamlRoot = shell.XamlRoot,
                    Content = message,
                    Title = title,
                    PrimaryButtonText = "ok"
                };
                _ = dialog.ShowAsync();
            }
        }
        catch (Exception)
        {
            System.Windows.Forms.MessageBox.Show(message, title, System.Windows.Forms.MessageBoxButtons.OK);
        }

        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
        if (!e.Handled)
        {
            Exit();
        }
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        await App.GetService<IActivationService>().ActivateAsync(args);
        try
        {
            var db = App.GetNewDatabase();
            var DBVersion = db.dbVersion;
            var SupportedDBVersion = new Version(1, 0, 4);
            if (!SupportedDBVersion.Equals(DBVersion))
            {
                var version = Package.Current.Id.Version;
                var versionString = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
                var message = "The application version you are running is not compatible with the current Database version\r\n" +
                    $"Please install the latest version of the application\r\nApplication Version: {versionString}\r\n" +
                    $"Supported Database Version: {SupportedDBVersion}. Actual Database Version: {DBVersion}";
                System.Windows.Forms.MessageBox.Show(message, $"{"AppDisplayName".GetLocalized()} DatabaseVersionMismatch", System.Windows.Forms.MessageBoxButtons.OK);
                Exit();
            }
        }
        catch (Exception ex)
        {
            //System.Windows.Forms.MessageBox.Show(ex.Message, $"{"AppDisplayName".GetLocalized()} DatabaseVersionMismatch", System.Windows.Forms.MessageBoxButtons.OK);
            await DispatcherQueue.GetForCurrentThread().EnqueueAsync(() =>
            {
                if (App.GetService<INavigationService>() is NavigationService nav)
                {
                    nav.NavigateTo(typeof(SettingsViewModel).FullName!);
                }
            });
        }
    }

    public static IBLDatabase GetNewDatabase()
    {
        return new SqlBLOrdersDatabase(DBOptions);
    }

    public static CompanyInfo CompanyInfo => App.GetNewDatabase().CompanyInfo;
}
