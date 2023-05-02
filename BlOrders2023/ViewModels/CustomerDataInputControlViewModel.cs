using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.ViewModels
{
    public class CustomerDataInputControlViewModel : ObservableValidator
    {
        #region Properties
        [Required]
        [MinLength(1)]
        [MaxLength(30)]
        public string? CustomerName
        {
            get => _customerName;
            set => SetProperty(ref _customerName, value, true);
        }

        [Required]
        [MinLength(1)]
        public string? Phone
        {
            get => _phone;
            set
            {
                SetProperty(ref _phone, value, true);
                ValidateProperty(value, nameof(Phone));
            }
        }

        public CustomerClass CustomerClass 
        { 
            get => _customerClass;
            set
            {
                SetProperty(ref _customerClass, value);
                ValidateProperty(value, nameof(CustomerClass));
            }
        }

        public ObservableCollection<CustomerClass> Classes { get; } = new();

        #endregion Properties
        #region Fields
        private string? _customerName;
        private string? _phone;
        private CustomerClass _customerClass;
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        public WholesaleCustomer Customer { get; set; } = new();
        #endregion Fields
        #region Constructors
        public CustomerDataInputControlViewModel()
        {
            Customer = new WholesaleCustomer();
            CustomerClass = Customer.CustomerClass;
            _ = GetClassesAsync();
            ValidateAllProperties();
        }

        public async Task GetClassesAsync()
        {
            var classes = await Task.Run(() => App.GetNewDatabase().Customers.GetCustomerClassesAsync());
            await dispatcherQueue.EnqueueAsync(() =>
            {
                foreach (var custClass in classes)
                {
                    Classes.Add(custClass);
                }
            });
        }

        #endregion Constructors
        #region Methods
        #endregion Methods
    }
}
