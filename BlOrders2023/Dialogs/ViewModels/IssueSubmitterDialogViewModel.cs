using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using BlOrders2023.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;

namespace BlOrders2023.Dialogs.ViewModels;
internal class IssueSubmitterDialogViewModel : ObservableValidator
{
    private string _title;
    private string _submitter;
    private string _description;

    [Required]
    [MinLength(1)]
    public string Title
    {
        get => _title;
        set
        {
            ValidateProperty(value, nameof(Title));
            SetProperty(ref _title, value);
        }
    }

    [Required]
    [MinLength(1)]
    public string Submitter
    {
        get => _submitter;
        set
        {
            ValidateProperty(value, nameof(Submitter));
            SetProperty(ref _submitter, value);
        }
    }

    [Required]
    [MinLength(1)]
    public string Description
    {
        get => _description; 
        set 
        {
            ValidateProperty(value, nameof(Description));
            SetProperty(ref _description, value);
        }
    }

    public IssueSubmitterDialogViewModel()
    {
        ErrorsChanged += HasErrorsChanged;
    }

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
