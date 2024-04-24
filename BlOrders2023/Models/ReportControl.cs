using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;
using BlOrders2023.ViewModels.ReportControls;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using System.Reflection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.Models;

public class ReportControl<T> : ReportControlBase
    where T : IReport
{
    private IReportControlViewModel<T> ViewModel { get; set; }

    public override Type ReportType => ViewModel.ReportType;

    public override string ReportName => ViewModel.ReportName;

    public override ReportCategory ReportCategory => ViewModel.ReportCategories;

    public override List<ReportPrompts.PromptTypes> Prompts => ViewModel.Prompts;

    public ReportControl()
    {
        ViewModel = App.GetService<IReportControlViewModel<T>>();

        //var displayNameAtt = (typeof(T).GetCustomAttribute(typeof(DisplayNameAttribute), true) as DisplayNameAttribute);
        //var NameRun = new HyperlinkButton()
        //{
        //    Content = "• " + (displayNameAtt != null ? displayNameAtt.DisplayName : nameof(T)),
        //};
        //NameRun.Click += base.Hyperlink_Click;
        //var res = Assembly.GetExecutingAssembly().GetManifestResourceStream("BlOrders2023.Assets.FileIcon.png");
        //var imgURI = new Uri("ms-resource:///Assets/FileIcon.png");
        //Image img = new Image() {Stretch = Microsoft.UI.Xaml.Media.Stretch.Uniform, Height = 200};
        //var bmpImage = new BitmapImage();
        //bmpImage.SetSource(res.AsRandomAccessStream());
        //img.Source = bmpImage;
        //var expander = new Expander()
        //{
        //    Header = NameRun,
        //    Content = img,
        //};

        //Content = expander;
    }

    public async override Task<object?[]> GetData(object[] userInputs) => await ViewModel.GetData(userInputs);
}
