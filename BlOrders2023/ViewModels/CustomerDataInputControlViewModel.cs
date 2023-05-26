using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace BlOrders2023.ViewModels
{
    public class CustomerDataInputControlViewModel : ObservableValidator
    {
        #region Properties
        
        [Required]
        [MinLength(1)]
        [MaxLength(30)]
        [Display(Name = "Customer Name")]
        public string CustomerName
        {
            get => Customer.CustomerName;
            set
            {
                Customer.CustomerName = value.Trim();
                CheckValidation(Customer.CustomerName, nameof(CustomerName));
                OnPropertyChanged();
            }
        }

        [MaxLength(20)]
        public string? Buyer
        {
            get => Customer.Buyer;
            set
            {
                Customer.Buyer = value;
                CheckValidation(value, nameof(Buyer));
                OnPropertyChanged();
            }
        }

        [Required]
        [Phone]
        public string Phone
        {
            get => Customer.Phone;
            set
            {
                Customer.Phone = value; 
                CheckValidation(value, nameof(Phone));
                OnPropertyChanged();
            }
        }

        [Phone]
        public string? Phone_2
        {
            get => Customer.Phone_2;
            set
            {
                if (value.IsNullOrEmpty())
                {
                    value = null;
                }
                Customer.Phone_2 = value;
                CheckValidation(value, nameof(Phone_2));
                OnPropertyChanged();
            }
        }

        [EmailAddress]
        public string? Email
        {
            get => Customer.Email;
            set
            {
                if (value.IsNullOrEmpty())
                {
                    value = null;
                }
                Customer.Email = value;
                CheckValidation(value, nameof(Email));
                OnPropertyChanged();
            }
        }

        [Phone]
        public string? Fax
        {
            get => Customer.Fax;
            set
            {
                if (value.IsNullOrEmpty())
                {
                    value = null;
                }
                Customer.Fax = value;
                CheckValidation(value, nameof(Fax));
                OnPropertyChanged();
            }
        }

        [Required]
        [MaxLength(30)]
        public string? Address
        {
            get => Customer.Address;
            set
            {
                Customer.Address = value;
                CheckValidation(value, nameof(Address));
                OnPropertyChanged();
            }
        }

        [Required]
        [MaxLength(20)]
        public string? City
        {
            get => Customer.City;
            set
            {
                Customer.City = value;
                CheckValidation(value, nameof(City));
                OnPropertyChanged();
            }
        }

        [Required]
        [MaxLength(2)]
        public string? State
        {
            get => Customer.State;
            set
            {
                CheckValidation(value, nameof(State));
                OnPropertyChanged();
            }
        }

        [Required]
        [MaxLength(10)]
        public string? ZipCode
        {
            get => Customer.ZipCode;
            set
            {
                Customer.ZipCode = value;
                CheckValidation(value, nameof(ZipCode));
                OnPropertyChanged();
            }
        }

        [Required]
        [MaxLength(30)]
        public string? BillingAddress
        {
            get => Customer.BillingAddress;
            set
            {
                Customer.BillingAddress = value;
                CheckValidation(value, nameof(BillingAddress));
                OnPropertyChanged();
            }
        }

        [Required]
        [MaxLength(20)]
        public string? BillingCity
        {
            get => Customer.BillingCity;
            set
            {
                Customer.BillingCity = value;
                CheckValidation(value, nameof(BillingCity));
                OnPropertyChanged();
            }
        }

        [Required]
        [MaxLength(2)]
        public string? BillingState
        {
            get => Customer.BillingState;
            set
            {
                Customer.BillingState = value;
                CheckValidation(value, nameof(BillingState));
                OnPropertyChanged();
            }
        }

        [Required]
        [MaxLength(10)]
        public string? BillingZipCode
        {
            get => Customer.BillingZipCode;
            set
            {
                Customer.BillingZipCode = value;
                CheckValidation(value, nameof(BillingZipCode));
                OnPropertyChanged();
            }
        }

        [Required]
        [Display(Name = "Customer Class")]
        public CustomerClass CustomerClass 
        { 
            get => Customer.CustomerClass;
            set
            {
                Customer.CustomerClass = value;
                CheckValidation(value, nameof(CustomerClass));
            }
        }

        public ObservableCollection<CustomerClass> Classes { get; } = new();

        public static IEnumerable<AllocationType> AllocationTypes { get => Enum.GetValues(typeof(AllocationType)).Cast<AllocationType>(); }

        #endregion Properties
        #region Fields
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        public WholesaleCustomer Customer { get; set; }
        #endregion Fields
        #region Constructors
        public CustomerDataInputControlViewModel()
        {
            Customer = new WholesaleCustomer();
            _ = GetClassesAsync();
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
                OnPropertyChanged(nameof(classes));
                //Added so that the UI shows the selected item after the items list is populated
                OnPropertyChanged(nameof(CustomerClass));
            });
        }

        #endregion Constructors

        #region Methods

        #region Validators
        public string GetErrorMessage(string name)
        {
            var errors = GetErrors(name);
            var firstError = errors.FirstOrDefault();
            if (firstError != null)
            {
                return firstError.ErrorMessage ?? "Error";
            }
            return string.Empty;
        }

        public bool HasError(string name)
        {
            if (HasErrors)
            {
                var errors = GetErrors(name);
                if (errors.IsNullOrEmpty())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public Visibility VisibleIfError(string name)
        {
            if (HasError(name))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        private void CheckValidation(object? value, string propertyName = null!)
        {
            ValidateProperty(value, propertyName);
            OnPropertyChanged(nameof(GetErrorMessage));
            OnPropertyChanged(nameof(VisibleIfError));
        }
        #endregion Validators
        #endregion Methods
    }
}
