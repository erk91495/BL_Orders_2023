using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Syncfusion.UI.Xaml.Calendar;
using System.ComponentModel.DataAnnotations;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.IdentityModel.Tokens;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BlOrders2023.UserControls.Dialogs;
public enum InputTypes
{
    [Description("Scanline")]
    Scanline,
    [Description("ID and Serial")]
    Serial,
}

public sealed partial class AuditDataInputDialog : ContentDialog, INotifyPropertyChanged
{
    private InputTypes _inputType;
    private DateTime _packDate;
    private string _serial;
    private int _productID;
    private string _scanline;
    private DateTimeOffsetRange _dateRange;

    public event PropertyChangedEventHandler? PropertyChanged;
    public string Scanline
    {
        get => _scanline;
        set
        {
            _scanline = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSubmit));
        }
    }
    public int ProductID
    {
        get => _productID;
        set
        {
            _productID = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSubmit));
        }
    }
    public string Serial
    {
        get => _serial;
        set
        {
            _serial = value;
            OnPropertyChanged();
        }
    }
    public DateTime PackDate
    {
        get => _packDate;
        set
        {
            _packDate = value;
            OnPropertyChanged();
        }
    }

    public InputTypes InputType
    {
        get => _inputType; 
        set 
        {
            if(value != _inputType)
            {
                _inputType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSubmit));
            }
        }
    }

    public List<string> GetCheckedBoxes()
    {
        var boxes = new List<string>();
        if (ckbProductID.IsChecked == true)
        {
            boxes.Add("ProductID");
        }
        if (ckbSerial.IsChecked == true)
        {
            boxes.Add("Serial");
        }
        if (ckbPackDate.IsChecked == true)
        {
            boxes.Add("PackDate");
        }
        if (ckbScanline.IsChecked == true)
        {
            boxes.Add("Scanline");
        }
        if (ckbDateRange.IsChecked == true)
        {
            boxes.Add("DateRange");
        }
        return boxes;
    }

    public DateTimeOffsetRange DateRange
    {
        get => _dateRange;
        set
        {
            _dateRange = value;
            OnPropertyChanged();
        }
    }

    public bool CanSubmit => AllNeededInfoinput();

    private IList<InputTypes> _inputTypesList = Enum.GetValues(typeof(InputTypes)).Cast<InputTypes>().ToList();

    public IList<InputTypes> InputTypeList => _inputTypesList;

    public AuditDataInputDialog()
    {
        this.InitializeComponent();
        InputTypeCombo.SelectedIndex = 0;
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool AllNeededInfoinput()
    {
        var dateRangeOk = ckbDateRange.IsChecked == true ? DateRange != null && DateRange.StartDate != null && DateRange.EndDate != null : true;
        var scanlineOk = InputType == InputTypes.Scanline ? !Scanline.IsNullOrEmpty() : true;
        var serialOk = InputType == InputTypes.Serial ? ProductID > 0 : true;
        return dateRangeOk && scanlineOk && serialOk;
    }

    private void ckbDateRange_Checked(object sender, RoutedEventArgs e)
    {
        OnPropertyChanged(nameof(CanSubmit));
    }

    private void InputTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}