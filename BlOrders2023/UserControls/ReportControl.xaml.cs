using BlOrders2023.Reporting.ReportClasses;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls;

public sealed partial class ReportControl : UserControl
{
    public Type ReportType;

    public event EventHandler? ReportSelected;
    public ReportControl(Type report)
    {
        if(!typeof(IReport).IsAssignableFrom(report))
        {
            throw new ArgumentException();
        }
        this.InitializeComponent();
        ReportType = report;
        var displayNameAtt = (ReportType.GetCustomAttribute(typeof(DisplayNameAttribute), true) as DisplayNameAttribute);
        if( displayNameAtt != null )
        {
            ReportNameRun.Text = displayNameAtt.DisplayName;
        }
        else
        {
            ReportNameRun.Text = ReportType.Name;
        }
    }

    private async void Hyperlink_Click(Microsoft.UI.Xaml.Documents.Hyperlink sender, Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args)
    {
        var dialog = new ContentDialog()
        {
            Content = "temp",
            XamlRoot = sender.XamlRoot,
        };
        await dialog.ShowAsync();
        //ReportSelected?.Invoke(this, EventArgs.Empty);
    }
}
