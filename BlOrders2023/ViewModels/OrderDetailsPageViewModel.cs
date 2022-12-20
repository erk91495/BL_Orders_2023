using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.ViewModels
{
    public class OrderDetailsPageViewModel : ObservableRecipient
    {
        private Order? _order;

        public Order? CurrentOrder
        {
            get => _order;
            set
            {
                SetProperty(ref _order, value);
                OnPropertyChanged(nameof(CurrentOrder));
            }
        }
        public OrderDetailsPageViewModel()
        {
           
        }
    }
}
