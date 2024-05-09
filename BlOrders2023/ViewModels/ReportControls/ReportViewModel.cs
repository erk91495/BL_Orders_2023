using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using System.Reflection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.ViewModels.ReportControls;

public class ReportViewModel<T> : ReportViewBase
    where T : IReport
{
    private IReportViewModel<T> ViewModel
    {
        get; set;
    }

    public override Type ReportType => ViewModel.ReportType;

    public override string ReportName => ViewModel.ReportName;

    public override string ReportDescription => ViewModel.ReportDescription;

    public override ReportCategory ReportCategory => ViewModel.ReportCategories;

    public override List<PromptTypes> Prompts => ViewModel.Prompts;

    public ReportViewModel()
    {
        ViewModel = App.GetService<IReportViewModel<T>>();
    }

    public async override Task<object?[]> GetData(object[] userInputs) => await ViewModel.GetData(userInputs);
}
