using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using BlOrders2023.UserControls.ViewModels;

namespace BlOrders2023.UserControls
{
    public partial class SingleValueInputControl : ContentDialog
    {
        SingleValueInputControlViewModel ViewModel { get;}
        public string? Prompt { get; set; }

        public string? Value { get; set; }
        public SingleValueInputControl()
        {
            InitializeComponent();
            ViewModel = App.GetService<SingleValueInputControlViewModel>();
        }
    }
}