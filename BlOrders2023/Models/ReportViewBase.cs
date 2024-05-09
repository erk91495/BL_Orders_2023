using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using BlOrders2023.Reporting;

namespace BlOrders2023.Models;
public abstract  class ReportViewBase
{
    public abstract Type ReportType { get; }
    public abstract string ReportName { get; }
    public abstract string ReportDescription { get; }
    public abstract ReportCategory ReportCategory { get; }
    public abstract List<PromptTypes> Prompts { get; }
    public abstract Task<object?[]> GetData(object[] userInputs);
    public event EventHandler? ReportSelected;
    protected void Hyperlink_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        //var dialog = new ContentDialog()
        //{
        //    Content = "temp",
        //    XamlRoot = sender.XamlRoot,
        //};
        //await dialog.ShowAsync();
        ReportSelected?.Invoke(this, EventArgs.Empty);
    }
}
