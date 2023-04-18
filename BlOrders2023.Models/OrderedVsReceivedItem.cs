using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models
{
    public class OrderedVsReceivedItem : INotifyPropertyChanged
    {
        private int productID;
        private int ordered;
        private int received;
        private float weight;

        public int ProductID
        {
            get => productID;
            set
            {
                productID = value;
                OnPropertyChanged(nameof(ProductID));
            }
        }
        public int Ordered
        {
            get => ordered;
            set
            {
                ordered = value;
                OnPropertyChanged(nameof(ordered));
            }
        }
        public int Received 
        { 
            get => received; 
            set 
            { 
                received = value;
                OnPropertyChanged(nameof(Received));
            } 
        }

        public float Weight
        {
            get => weight;
            set
            {
                weight = value;
                OnPropertyChanged(nameof(Weight));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
