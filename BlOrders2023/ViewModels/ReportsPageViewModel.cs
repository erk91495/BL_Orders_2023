using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using BlOrders2023.UserControls;
using CommunityToolkit.Mvvm.ComponentModel;
using QuestPDF.Infrastructure;
using System.Collections.ObjectModel;

namespace BlOrders2023.ViewModels;

public class ReportsPageViewModel : ObservableRecipient
{
    public ObservableCollection<ReportControl> ReportsList { get; set; }
    public ReportsPageViewModel()
    {
        ReportsList = new();
        foreach(Type t in Reports.AvalibleReports)
        {
            ReportsList.Add(new(t));
        }
    }
}
