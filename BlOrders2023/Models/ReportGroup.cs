using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Reporting;
using WinRT;

namespace BlOrders2023.Models
{
    class ReportGroup
    {
        public ReportCategory Category { get; set; }
        public ObservableCollection<ReportViewBase> Items { get; set; } = new();
    }
}
