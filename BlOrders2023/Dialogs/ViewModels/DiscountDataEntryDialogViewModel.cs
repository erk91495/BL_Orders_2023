using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Contracts.ViewModels;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using Castle.Core.Resource;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;
using Syncfusion.UI.Xaml.Calendar;

namespace BlOrders2023.Dialogs.ViewModels;
public class DiscountDataEntryDialogViewModel : ObservableValidator
{
    #region Fields
    private Discount _discount;
    private DiscountTypes _type;
    private string? _description;

    public DiscountDataEntryDialogViewModel()
    {
        _discount = new();
        _description = string.Empty;
        ErrorsChanged += HasErrorsChanged;
        PropertyChanged += DiscountDataEntryDialogViewModel_PropertyChanged;
    }

    private void DiscountDataEntryDialogViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        CanSave = HasErrors;
    }

    private double modifier;
    #endregion Fields

    #region Properties
    public Discount Discount
    {
        get => _discount;
        internal set 
        {
            SetProperty(ref _discount, value);
            Modifier = _discount.Modifier;
            Description = _discount.Description;
            Type = Discount.Type;
        }
    }
    public ObservableCollection<Product> Products
    {
        get => _discount.Products;
        set => _discount.Products = value;
    }
    public ObservableCollection<WholesaleCustomer> Customers
    {
        get => _discount.Customers;
        set => _discount.Customers = value;
    }

    public DateTimeOffsetRange? DateRange
    {
        get
        {
            if (Discount.StartDate != null && Discount.EndDate != null)
            {
                return new(new((DateTime)Discount.StartDate), new((DateTime)Discount.EndDate));
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (value != null && value.StartDate != null && value.EndDate != null)
            {
                if (value.StartDate.HasValue)
                {
                    Discount.StartDate = value.StartDate.Value.DateTime;
                }

                if (value.StartDate.HasValue)
                {
                    Discount.EndDate = value.EndDate.Value.DateTime;
                }
            }
            else
            {
                Discount.StartDate = null;
                Discount.EndDate = null;
            }
        }
    }

    public DiscountTypes Type 
    { 
        get => _type;
        set
        {
            SetProperty(ref _type, value);
            _discount.Type = value;
            ValidateProperty(Modifier,nameof(Modifier));
        }
    }

    [MinLength(1)]
    [MaxLength(128)]
    public string? Description
    {
        get => _description; 
        set 
        {
            if(value != null)
            {
                value = value.Trim();
            }
            SetProperty(ref _description, value);
            _discount.Description = _description;
            ValidateProperty(_description, nameof(Description));
        }
    }

    [CustomValidation(typeof(DiscountDataEntryDialogViewModel), nameof(ValidateModifier))]
    public double Modifier
    {
        get => modifier; 
        set
        { 
            SetProperty(ref modifier, value);
            _discount.Modifier = modifier;
            ValidateProperty(modifier, nameof(Modifier));
        }

    }

    public bool Inactive
    {
        get => _discount.Inactive;
        set
        {
            _discount.Inactive = value;
            ValidateProperty(value, nameof(Inactive));
        }
    }

    public bool CanSave { get; set; } = false;
    #endregion Properties

    #region Methods
    public static ValidationResult? ValidateModifier(string? value, ValidationContext context)
    {
        var result = ValidationResult.Success;
        if (context.ObjectInstance is DiscountDataEntryDialogViewModel vm)
        {
            switch (vm.Type)
            {
                case DiscountTypes.SetPrice:
                    if(!double.TryParse(value, out var res))
                    {
                        result = new ValidationResult("Set price must be a valid number");
                    }
                    else if(res < 0)
                    {
                        result = new ValidationResult("Set price cannot be negative");
                    }
                    break;
                case DiscountTypes.PercentOff:
                    if (!double.TryParse(value, out res))
                    {
                        result = new ValidationResult("Percent off must be a valid number");
                    }
                    else if (res >= 100)
                    {
                        result = new ValidationResult("Percent cannot be greater than 100");
                    }
                    else if(res <= -100)
                    {
                        result = new ValidationResult("Percent cannot be less than 100");
                    }
                    break;
            }
        }
        return result;
    }
    #endregion Methods

    #region Validation 
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
    #endregion Validation
}
