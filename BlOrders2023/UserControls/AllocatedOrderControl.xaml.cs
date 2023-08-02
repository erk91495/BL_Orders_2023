using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using BlOrders2023.UserControls.ViewModels;
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
public sealed partial class AllocatedOrderControl : UserControl
{
    #region Properties
    public static readonly DependencyProperty OrderProperty = DependencyProperty.Register("Order", typeof(Order), typeof(AllocatedOrderControl), new PropertyMetadata(null));
    public Order Order
    {
        get => (Order)GetValue(OrderProperty);
        set => SetValue(OrderProperty, value);
    }
    #endregion Properties

    #region Fields
    #endregion Fields

    #region Constructors
    public AllocatedOrderControl()
    {
        this.InitializeComponent();
    }
    #endregion Constructors

    #region Methods
    #endregion  Methods
}
