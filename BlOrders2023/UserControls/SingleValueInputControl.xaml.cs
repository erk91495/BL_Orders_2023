using Microsoft.UI.Xaml.Controls;
using BlOrders2023.UserControls.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BlOrders2023.UserControls
{
    public partial class SingleValueInputControl : ContentDialog, INotifyPropertyChanged
    {
        private string? _value;
        private bool isValid = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        SingleValueInputControlViewModel ViewModel { get; }
        public string? Prompt { get; set; }

        public string? Value 
        { 
            get => _value; 
            set 
            { 
                _value = value;
                if(ValidateValue != null)
                {
                    isValid = ValidateValue(_value);
                    OnPropertyChanged(nameof(notValid));

                }
            } 
        }
        private bool notValid { get => !isValid; }
        public ValidationMethod? ValidateValue { get; set; }

        public delegate bool ValidationMethod(string? value);
        public SingleValueInputControl()
        {
            InitializeComponent();
            ViewModel = App.GetService<SingleValueInputControlViewModel>();
            
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

            if (!isValid)
            {
                args.Cancel = true;
            }

        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}