using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.Calendar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls
{
    public sealed partial class DateRangeSelectionDialog : ContentDialog, INotifyPropertyChanged
    {
        private DateTimeOffsetRange? dateRange;

        public DateTimeOffsetRange? DateRange
        {
            get => dateRange;
            set
            {
                dateRange = value;
            }
        }

        public DateTimeOffset? StartDate
        {
            get
            {
                if (dateRange != null)
                {
                    return dateRange.StartDate;
                }
                else
                {
                    return null;
                }
            }
        }
        public DateTimeOffset? EndDate
        {
            get
            {
                if (dateRange != null)
                {
                    return dateRange.EndDate;
                }
                else
                {
                    return null;
                }
            }
        }
        public DateRangeSelectionDialog()
        {
            this.InitializeComponent();
            this.Title = "Date Selection";
            this.PrimaryButtonText = "Select";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
