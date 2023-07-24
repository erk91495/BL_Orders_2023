using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Data;
using BlOrders2023;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.IdentityModel.Tokens;
using CommunityToolkit.WinUI;

namespace BlOrders2023.ViewModels
{
    public class CustomerClassesPageViewModel : ObservableRecipient
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value that specifies whether orders are being loaded.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public ObservableCollection<CustomerClass> CustomerClasses
        {
            get => customerClasses; 
            set 
            {
                SetProperty(ref customerClasses, value);
                OnPropertyChanged(nameof(CustomerClasses));
            }
        }
        #endregion Properties

        #region Fields
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private bool _isLoading;
        private ObservableCollection<CustomerClass> customerClasses = new();

        #endregion Fields

        #region Constructors
        public CustomerClassesPageViewModel()
        {
            _ = LoadClassesAsync();
        }
        #endregion Constructors

        #region Methods
        private async Task LoadClassesAsync(string? query = null)
        {

            await dispatcherQueue.EnqueueAsync(() =>
            {
                IsLoading = true;
                CustomerClasses.Clear();
            });

            var    classes = await App.GetNewDatabase().CustomerClasses.GetCustomerClassesAsync(query);

            await dispatcherQueue.EnqueueAsync(() =>
            {
                CustomerClasses = new(classes);
                IsLoading = false;
            });
            

        }

        internal async void SaveItem(CustomerClass custClass)
        {
            await App.GetNewDatabase().CustomerClasses.UpsertAsync(custClass);
        }
        #endregion Methods
    }
}
