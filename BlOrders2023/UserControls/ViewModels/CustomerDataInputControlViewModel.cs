using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Core.Data;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace BlOrders2023.UserControls.ViewModels;

public class CustomerDataInputControlViewModel : ObservableValidator
{
    #region Properties
    
    [Required]
    [MinLength(1)]
    [MaxLength(30)]
    [CustomValidation(typeof(CustomerDataInputControlViewModel), nameof(ValidateCustomerName))]
    [Display(Name = "Customer Name")]
    public string CustomerName
    {
        get => Customer.CustomerName;
        set
        {
            Customer.CustomerName = value.Trim().ToUpper();
            ValidateProperty(Customer.CustomerName, nameof(CustomerName));
            OnPropertyChanged();
        }
    }

    [MaxLength(20)]
    public string? Buyer
    {
        get => Customer.Buyer;
        set
        {
            if (value.IsNullOrEmpty())
            {
                value = null;
            }
            Customer.Buyer = value;
            ValidateProperty(value, nameof(Buyer));
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
            ValidateProperty(value, nameof(Phone));
            OnPropertyChanged();
        }
    }
    [MaxLength(4)]
    [Display(Name = "Phone ext.")]
    public string? PhoneExt
    {
        get => Customer.PhoneExt;
        set
        {
            if (value.IsNullOrEmpty())
            {
                value = null;
            }
            Customer.PhoneExt = value;
            
            OnPropertyChanged();

        }
    }

    [Phone]
    [Display(Name = "Phone 2")]
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
            ValidateProperty(value, nameof(Phone_2));
            OnPropertyChanged();
        }
    }

    [MaxLength(4)]
    [Display(Name = "Phone 2 ext.")]
    public string? Phone2Ext
    {
        get => Customer.Phone2Ext;
        set
        {
            if (value.IsNullOrEmpty())
            {
                value = null;
            }
            Customer.Phone2Ext = value;
            ValidateProperty(value, nameof(Phone2Ext));
            OnPropertyChanged();

        }
    }

    //We currently may use the email field for multiple emails
    //[EmailAddress]
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
            ValidateProperty(value, nameof(Email));
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
            ValidateProperty(value, nameof(Fax));
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
            ValidateProperty(value, nameof(Address));
            OnPropertyChanged();
            if (Customer.UseSameAddress)
            {
                BillingAddress = value;
            }
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
            ValidateProperty(value, nameof(City));
            OnPropertyChanged();
            if (Customer.UseSameAddress)
            {
                BillingCity = value;
            }
        }
    }

    [Required]
    [MaxLength(2)]
    public string? State
    {
        get => Customer.State;
        set
        {
            //dont use OnPropertyChanged() it causes stack overflow
            Customer.State = value;
            ValidateProperty(value, nameof(State));
            OnPropertyChanged(nameof(State));
            if (Customer.UseSameAddress)
            {
                BillingState = value;
            }
        }
    }

    [Required]
    [MaxLength(10)]
    [MinLength(1)]
    [Display(Name = "Zip Code")]
    public string? ZipCode
    {
        get => Customer.ZipCode;
        set
        {
            Customer.ZipCode = value;
            ValidateProperty(value, nameof(ZipCode));
            OnPropertyChanged();
            if (Customer.UseSameAddress)
            {
                BillingZipCode = value;
            }
        }
    }

    [Required]
    [MaxLength(30)]
    [MinLength(1)]
    [Display(Name = "Address")]
    public string? BillingAddress
    {
        get => Customer.BillingAddress;
        set
        {
            Customer.BillingAddress = value;
            ValidateProperty(value, nameof(BillingAddress));
            OnPropertyChanged();
        }
    }

    [Required]
    [MaxLength(20)]
    [MinLength(1)]
    [Display(Name = "City")]
    public string? BillingCity
    {
        get => Customer.BillingCity;
        set
        {
            Customer.BillingCity = value;
            ValidateProperty(value, nameof(BillingCity));
            OnPropertyChanged();
        }
    }

    [Required]
    [MaxLength(2)]
    [MinLength(1)]
    [Display(Name = "State")]
    public string? BillingState
    {
        get => Customer.BillingState;
        set
        {
            Customer.BillingState = value;
            ValidateProperty(value, nameof(BillingState));
            OnPropertyChanged(nameof(BillingState));
        }
    }

    [Required]
    [MaxLength(10)]
    [MinLength(1)]
    [Display(Name = "Zip Code")]
    public string? BillingZipCode
    {
        get => Customer.BillingZipCode;
        set
        {
            Customer.BillingZipCode = value;
            ValidateProperty(value, nameof(BillingZipCode));
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
            ValidateProperty(value, nameof(CustomerClass));
        }
    }

    public bool UseSameAddress
    {
        get => Customer.UseSameAddress;
        set
        {
            Customer.UseSameAddress = value;
            OnPropertyChanged();
            if (value)
            {
                BillingAddress = Address;
                BillingCity = City;
                BillingState = State;
                BillingZipCode = ZipCode;
            }
            
        }
    }

    public bool CheckIfUnique { get; set; }
    public ObservableCollection<CustomerClass> Classes { get; } = new();

    public static IEnumerable<CustomerAllocationType> AllocationTypes => Enum.GetValues(typeof(CustomerAllocationType)).Cast<CustomerAllocationType>();
    public WholesaleCustomer Customer => _customer;
    #endregion Properties
    #region Fields
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    
    private WholesaleCustomer _customer;

   
    #endregion Fields
    #region Constructors
    public CustomerDataInputControlViewModel()
    {
        ErrorsChanged += HasErrorsChanged;
        _customer = new WholesaleCustomer();
        _ = GetClassesAsync();
    }
    #endregion Constructors

    #region Methods
    public async Task GetClassesAsync()
    {
        var db = App.GetNewDatabase();
        var classes = await Task.Run(() => db.CustomerClasses.GetCustomerClassesAsync(asNoTracking: true));
        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (var custClass in classes)
            {
                Classes.Add(custClass);
            }
            OnPropertyChanged(nameof(Classes));
            //Added so that the UI shows the selected item after the items list is populated
            OnPropertyChanged(nameof(CustomerClass));
        });
    }

    internal bool SaveCustomer(bool overwrite = false)
    {
        ValidateAllProperties();
        if (!HasErrors)
        {
            var db = App.GetNewDatabase();
            db.Customers.Upsert(Customer, overwrite);
            return true;
        }
        return false;
    }
    internal void SetCustomer(WholesaleCustomer customer)
    {
        //var entityOrder = _db.Customers.Get(customer.CustID).FirstOrDefault();
        //if (entityOrder != null)
        //{
        //    //We want to track changes so get it from the db context
        //    _customer = entityOrder;
        //}
        //else
        //{
        //    //Must be a new Order
        //    _customer = customer;
        //}
        _customer = customer;
    }
    #region Validators

    private void HasErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
    {
        OnPropertyChanged(nameof(GetErrorMessage));
        OnPropertyChanged(nameof(VisibleIfError));
    }

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

    public static ValidationResult? ValidateCustomerName(string? value, ValidationContext context)
    {
        if (context.ObjectInstance is CustomerDataInputControlViewModel vm && vm.CheckIfUnique)
        {
            if (value != null)
            {
                //IDK if this is to many calls to the db, but we will just have to see
                var db = App.GetNewDatabase();
                var names = db.Customers.Get().Select(c => c.CustomerName).ToHashSet();
                if (!names.Contains(value))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("A customer with the given name already exists");
        }
        else
        {
            return ValidationResult.Success;
        }
        
    }
    #endregion Validators
    #endregion Methods
}
