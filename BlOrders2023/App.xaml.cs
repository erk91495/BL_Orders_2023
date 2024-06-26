﻿using BlOrders2023.Activation;
using BlOrders2023.Contracts.Services;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Data;
using BlOrders2023.Core.Data.SQL;
using BlOrders2023.Core.Services;
using BlOrders2023.Models;
using BlOrders2023.Services;
using BlOrders2023.ViewModels;
using BlOrders2023.Views;
using BlOrders2023.Dialogs.ViewModels;
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
using NLog;
using System.Media;
using Microsoft.Extensions.Configuration;
using BlOrders2023.Reporting.ReportClasses;
using BlOrders2023.ViewModels.ReportControls;

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

    //Works but is kinda ugly look at how GridEditorDialog works instead
    //public static T GetService<T>(Type serviceType)
    //where T : class
    //{
    //    if ((App.Current as App)!.Host.Services.GetService(serviceType) is not T service)
    //    {
    //        throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
    //    }

    //    return service;
    //}

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
            services.AddTransient<LiveInventoryPage>();
            services.AddTransient<LiveInventoryPageViewModel>();
            services.AddTransient<PaymentsPage>();
            services.AddTransient<PaymentsPageViewModel>();
            services.AddTransient<ProductDataInputDialogViewModel>();
            services.AddTransient<IGridEditorViewModel<Box>,BoxGridEditorViewModel>();
            services.AddTransient<IssueSubmitterDialogViewModel>();
            services.AddTransient<IGridEditorViewModel<ProductCategory>,CategoriesGridEditorViewModel>();
            services.AddTransient<InventoryAdjustmentsPage>();
            services.AddTransient<InventoryAdjustmentsPageViewModel>();
            services.AddTransient<InventoryAuditLogPage>();
            services.AddTransient<InventoryAuditLogPageViewModel>();
            services.AddTransient<InventoryReconciliationPage>();
            services.AddTransient<InventoryReconciliationPageViewModel>();
            services.AddTransient<AddLiveInventoryPage>();
            services.AddTransient<AddLiveInventoryPageViewModel>();
            services.AddTransient<DiscountManagerPage>();
            services.AddTransient<DiscountManagerPageViewModel>();
            services.AddTransient<DiscountDataEntryDialogViewModel>();

            //ReportControl
            services.AddTransient<IReportViewModel<AggregateInvoiceReport>,AggregateInvoiceReportViewModel>();
            services.AddTransient<IReportViewModel<BillOfLadingReport>, BillOfLadingReportViewModel>();
            services.AddTransient<IReportViewModel<CurrentInventoryReport>, CurrentInventoryReportViewModel>();
            services.AddTransient<IReportViewModel<FrozenOrdersReport>, FrozenOrdersReportViewModel>();
            services.AddTransient<IReportViewModel<HistoricalProductCategoryTotalsReport>, HistoricalProductCategoryTotalsReportViewModel>();
            services.AddTransient<IReportViewModel<HistoricalQuarterlySalesReport>, HistoricalQuarterlySalesReportViewModel>();
            services.AddTransient<IReportViewModel<InventoryDetailsReport>, InventoryDetailsReportViewModel>();
            services.AddTransient<IReportViewModel<OutOfStateSalesReport>, OutOfStateSalesReportViewModel>();
            services.AddTransient<IReportViewModel<OutstandingBalancesReport>, OutstandingBalancesReportViewModel>();
            services.AddTransient<IReportViewModel<PalletLoadingReport>, PalletLoadingReportViewModel>();
            services.AddTransient<IReportViewModel<PickList>, PickListReportViewModel>();
            services.AddTransient<IReportViewModel<ProductCategoryDetailsReport>, ProductCategoryDetailsReportViewModel>();
            services.AddTransient<IReportViewModel<ProductCategoryTotalsReport>, ProductCategoryTotalsReportViewModel>();
            services.AddTransient<IReportViewModel<QuarterlySalesReport>, QuarterlySalesReportViewModel>();
            services.AddTransient<IReportViewModel<ShippingItemAuditReport>, ShippingItemAuditReportViewModel>();
            services.AddTransient<IReportViewModel<ShippingList>, ShippingListReportViewModel>();
            services.AddTransient<IReportViewModel<UnpaidInvoicesReport>, UnpaidInvoicesReportViewModel>();
            services.AddTransient<IReportViewModel<WholesaleInvoice>, WholesaleInvoiceReportViewModel>();
            services.AddTransient<IReportViewModel<WholesaleInvoiceTotalsReport>, WholesaleInvoiceTotalsReportViewModel>();
            services.AddTransient<IReportViewModel<WholesaleOrderPickupRecap>, WholesaleOrderPickupRecapViewModel>();
            services.AddTransient<IReportViewModel<WholesaleOrderTotals>, WholesaleOrderTotalsViewModel>();
            services.AddTransient<IReportViewModel<WholesalePaymentsReport>, WholesalePaymentReportViewModel>();
            services.AddTransient<IReportViewModel<YieldStudyReport>, YieldStudyReportViewModel>();
            services.AddTransient<IReportViewModel<ProductionDetailsReport> , ProductionDetailsReportViewModel>();
            services.AddTransient<IReportViewModel<ProductionSummaryReport>, ProductionSummaryReportViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
        }).
        ConfigureAppConfiguration( app => app.AddJsonFile("apis.json") ).
        Build();

        UnhandledException += App_UnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        var config =  GetService<IConfiguration>();
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(config["APIkeys:Syncfusion"]);
        QuestPDF.Settings.License = LicenseType.Community;

#if DEBUG
        var db = App.GetNewDatabase();
        if (db.DbConnection.DataSource == "BL4" && db.DbConnection.Database == "BL_Enterprise")
        {
            var message = "You are running a production DB with a debug application version.";
            System.Windows.Forms.MessageBox.Show(message, $"WARNING", System.Windows.Forms.MessageBoxButtons.OK);
        }
#endif // DEBUG
    }

    private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        SystemSounds.Exclamation.Play();
        if(e.ExceptionObject is Exception ex)
        {
            LogException(e.ExceptionObject as Exception);
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

    private async void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        SystemSounds.Exclamation.Play();
        LogException(e.Exception);
        if(e.Exception is SqlException || e.Exception is VersionMismatchException)
        {
            if (App.GetService<INavigationService>() is NavigationService nav)
            {
                e.Handled = true;
                nav.NavigateTo(typeof(SettingsViewModel).FullName!);
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
                await dialog.ShowAsync();
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
        LogInfoMessage("Application Launched");
        base.OnLaunched(args);
        await App.GetService<IActivationService>().ActivateAsync(args);
        try
        {
            var db = App.GetNewDatabase();
            var DBVersion = db.dbVersion;
            var SupportedDBVersion = new Version(2, 0, 3);
            if (!SupportedDBVersion.Equals(DBVersion))
            {
                var version = Package.Current.Id.Version;
                var versionString = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
                var message = "The application version you are running is not compatible with the current Database version\r\n" +
                    $"Please install the latest version of the application\r\nApplication Version: {versionString}\r\n" +
                    $"Supported Database Version: {SupportedDBVersion}. Actual Database Version: {DBVersion}";
                LogErrorMessage(message);
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

    public static void LogException(Exception e)
    {
        var logger = LogManager.GetCurrentClassLogger();
        logger.Error(e, e.Message);
    }
    public static void LogInfoMessage(string message)
    {
        var logger = LogManager.GetCurrentClassLogger();
        logger.Info(message);
    }
    public static void LogErrorMessage(string message)
    {
        var logger = LogManager.GetCurrentClassLogger();
        logger.Error(message);
    }

    public static void LogWarningMessage(string message)
    {
        var logger = LogManager.GetCurrentClassLogger();
        logger.Warn(message);
    }

    public static IBLDatabase GetNewDatabase()
    {
        return new SqlBLOrdersDatabase(DBOptions);
    }

    public static CompanyInfo CompanyInfo => App.GetNewDatabase().CompanyInfo;
}
