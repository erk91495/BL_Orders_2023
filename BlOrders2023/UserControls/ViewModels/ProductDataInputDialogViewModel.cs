﻿using System.ComponentModel.DataAnnotations;
using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using CommunityToolkit.WinUI;
using System.Collections.ObjectModel;

namespace BlOrders2023.UserControls.ViewModels;
public class ProductDataInputDialogViewModel: ObservableValidator
{
    #region Fields
    private Product _product;
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
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
        OnPropertyChanged(nameof(PalletHeight));
    }

    [Required]
    [Range(1,int.MaxValue)]
    public int? ProductID
    {
        get => _product.ProductID != 0 ? _product.ProductID : null;
        set
        {
            _product.ProductID = value ?? 0;
            ValidateProperty(value, nameof(ProductID));
            OnPropertyChanged();
        }
    }

    [Required]
    [MinLength(1)]
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

    [Required]
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

    public ObservableCollection<Box> Boxes = new();

    #endregion Properties

    #region Constructors
    public ProductDataInputDialogViewModel()
    {
        ErrorsChanged += HasErrorsChanged;
        _ = GetBoxesAsync();
    }
    #endregion Constructors

    #region Methods

    public async Task GetBoxesAsync()
    {
        var db = App.GetNewDatabase();
        var classes = await Task.Run(() => db.Boxes.GetAsync(asNoTracking: true));
        await dispatcherQueue.EnqueueAsync(() =>
        {
            foreach (var custClass in classes)
            {
                Boxes.Add(custClass);
            }
            OnPropertyChanged(nameof(Box));
        });
    }

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
