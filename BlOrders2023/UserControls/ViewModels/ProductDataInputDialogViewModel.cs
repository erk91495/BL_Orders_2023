using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using Castle.Core.Resource;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;

namespace BlOrders2023.UserControls.ViewModels;
public class ProductDataInputDialogViewModel: ObservableValidator
{
    #region Fields
    private Product _product;
    #endregion Fields

    #region Properties
    public Product Product
    {
        get => _product;
        set
        {
            _product = value;
            OnAllPropertiesChanged();
        }
    }

    private void OnAllPropertiesChanged()
    {
        OnPropertyChanged(nameof(Product));
        OnPropertyChanged(nameof(ProductID));
        OnPropertyChanged(nameof(ProductName));
        OnPropertyChanged(nameof(WholesalePrice));
        OnPropertyChanged(nameof(KetteringPrice));
        OnPropertyChanged(nameof(UPCCode));
        OnPropertyChanged(nameof(KPCCode));
        OnPropertyChanged(nameof(ALUCode));
        OnPropertyChanged(nameof(FixedPrice));
        OnPropertyChanged(nameof(CompanyCode));
        OnPropertyChanged(nameof(Inactive));
        OnPropertyChanged(nameof(IsCredit));
        OnPropertyChanged(nameof(BoxID));
        OnPropertyChanged(nameof(Box));
        OnPropertyChanged(nameof(BoxID));
        OnPropertyChanged(nameof(PalletHeight));
    }

    public int ProductID
    {
        get => _product.ProductID;
        set
        {
            _product.ProductID = value;
            ValidateProperty(_product.ProductID, nameof(ProductID));
            OnPropertyChanged();
        }
    }

    public string? ProductName
    {
        get => _product.ProductName;
        set
        {
            _product.ProductName = value;
            ValidateProperty(_product.ProductName, nameof(ProductName));
            OnPropertyChanged();
        }
    }

    public decimal WholesalePrice
    {
        get => _product.WholesalePrice;
        set
        {
            _product.WholesalePrice = value;
            ValidateProperty(_product.WholesalePrice, nameof(WholesalePrice));
            OnPropertyChanged();
        }
    }

    public decimal KetteringPrice
    {
        get => _product.KetteringPrice;
        set
        {
            _product.KetteringPrice = value;
            ValidateProperty(_product.KetteringPrice, nameof(KetteringPrice));
            OnPropertyChanged();
        }
    }

    public string? UPCCode
    {
        get => _product.UPCCode;
        set
        {
            _product.UPCCode = value;
            ValidateProperty(_product.UPCCode, nameof(UPCCode));
            OnPropertyChanged();
        }
    }

    public string? KPCCode
    {
        get => _product.KPCCode;
        set
        {
            _product.KPCCode = value;
            ValidateProperty(_product.KPCCode, nameof(KPCCode));
            OnPropertyChanged();
        }
    }

    public string? ALUCode
    {
        get => _product.ALUCode;
        set
        {
            _product.ALUCode = value;
            ValidateProperty(_product.ALUCode, nameof(ALUCode));
            OnPropertyChanged();
        }
    }

    public bool? FixedPrice
    {
        get => _product.FixedPrice;
        set
        {
            _product.FixedPrice = value;
            ValidateProperty(_product.FixedPrice, nameof(FixedPrice));
            OnPropertyChanged();
        }
    }

    public string? CompanyCode
    {
        get => _product.CompanyCode;
        set
        {
            _product.CompanyCode = value;
            ValidateProperty(_product.CompanyCode, nameof(CompanyCode));
            OnPropertyChanged();
        }
    }

    public bool Inactive
    {
        get => _product.Inactive;
        set
        {
            _product.Inactive = value;
            ValidateProperty(_product.Inactive, nameof(Inactive));
            OnPropertyChanged();
        }
    }

    public bool IsCredit
    {
        get => _product.IsCredit;
        set
        {
            _product.IsCredit = value;
            ValidateProperty(_product.IsCredit, nameof(IsCredit));
            OnPropertyChanged();
        }
    }

    public int? BoxID
    {
        get => _product.BoxID;
        set
        {
            _product.BoxID = value;
            ValidateProperty(_product.BoxID, nameof(BoxID));
            OnPropertyChanged();
        }
    }

    public Box? Box => _product.Box;

    public int? PalletHeight
    {
        get => _product?.PalletHeight;
        set
        {
            _product.PalletHeight = value;
            ValidateProperty(_product.PalletHeight, nameof(PalletHeight));
            OnPropertyChanged();
        }
    }
    #endregion Properties

    #region Methods
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
    #endregion Methods
}
