﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BlOrders2023.Dialogs;

public partial class SingleValueInputDialog : ContentDialog, INotifyPropertyChanged
{
    private string? _value;
    private bool isValid = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string? Prompt { get; set; }

    public string? Value 
    { 
        get => _value; 
        set 
        { 
            _value = value;
            if(ValidateValue != null)
            {
                isValid = ValidateValue(_value);
                OnPropertyChanged(nameof(notValid));

            }
        } 
    }
    private bool notValid => !isValid;
    public ValidationMethod? ValidateValue { get; set; }

    public delegate bool ValidationMethod(string? value);
    public SingleValueInputDialog()
    {
        InitializeComponent();
        
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        //If enter is used to submit the value binding does not get triggered in time.
        if (InputBox.Text != Value)
        {
            Value = InputBox.Text;
        }
        if (!isValid)
        {
            args.Cancel = true;
        }

    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}