using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls;
public sealed partial class ProductDataInputDialog : ContentDialog, INotifyPropertyChanged
{

    #region Fields
    private Product _product;
    private Dictionary<string,string> _errors;
    #endregion Fields

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    #region Properties
    public Product Product
    {
        get => _product;
        set
        {
            _product = value;
            OnPropertyChanged(nameof(Product));
        }
    }

    public bool HasErrors => !_errors.IsNullOrEmpty();
    #endregion Properties
    public ProductDataInputDialog(Product prod)
    {
        this.InitializeComponent();
        _errors = new();
        Product = prod;
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public IEnumerable<ValidationResult> GetErrors()
    {
        Collection<ValidationResult> result = new();
        ValidationContext context = new(Product);
        var res = Validator.TryValidateObject(Product, context, result, true);
        return result;
    }

    public string GetErrorMessage(string? propertyName)
    {
        return GetErrors().ToString();
    }
}
