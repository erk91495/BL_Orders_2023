using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Services;
using BlOrders2023.Dialogs.ViewModels;
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

namespace BlOrders2023.Dialogs;
public sealed partial class IssueSubmitterInputDialog : ContentDialog
{
    private IssueSubmitterDialogViewModel ViewModel
    {
        get;
    }
    public IssueSubmitterInputDialog()
    {
        this.InitializeComponent();
        IsPrimaryButtonEnabled = false;
        ViewModel = App.GetService<IssueSubmitterDialogViewModel>();
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        IssueSubmitter s = new IssueSubmitter();
        Collection<string> tags = new();
        if(IssueTypeCombo.SelectedIndex == 0)
        {
            tags.Add("bug");
        }
        else
        {
            tags.Add("enhancement");
        }
        var combinedDescription = $"Submitter: {ViewModel.Submitter.Trim()}\r\n{ViewModel.Description.Trim()}";
        s.SubmitIssueAsync(ViewModel.Title, combinedDescription, tags);
    }
}
