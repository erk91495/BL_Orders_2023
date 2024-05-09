using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using BlOrders2023.Services;
using BlOrders2023.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class InventoryAuditLogPage : Page
{
    #region Properties
    public InventoryAuditLogPageViewModel ViewModel { get; }
    #endregion Properties
    public InventoryAuditLogPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<InventoryAuditLogPageViewModel>();
    }

    private async void PrintButton_Click(object sender, RoutedEventArgs e)
    {
        var items = AuditGrid.View.Records.Select(e => e.Data as InventoryAuditItem).ToList();
        var reportGenerator = new ReportGenerator(App.GetNewDatabase().CompanyInfo);
        var report = reportGenerator.GetReport(typeof(InventoryAdjustmentsAuditReport),[items]);
        
        var filePath = await reportGenerator.GenerateReportPDFAsync(report);
        Windows.System.LauncherOptions options = new()
        {
            ContentType = "application/pdf"
        };
        _ = Windows.System.Launcher.LaunchUriAsync(new Uri(filePath), options);

    }
}
