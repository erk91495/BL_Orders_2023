using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BlOrders2023.Models;
using BlOrders2023.UserControls;
using BlOrders2023.ViewModels;
using CommunityToolkit.WinUI;
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

namespace BlOrders2023.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PaymentsPage : Page
{
    #region Properties
    public PaymentsPageViewModel ViewModel
    {
        get;
    }
    #endregion Properties

    public PaymentsPage()
    {
        ViewModel = App.GetService<PaymentsPageViewModel>();
        this.InitializeComponent();
    }

    /// <summary>
    /// Retrieve the list of orders when the user navigates to the page. 
    /// </summary>
    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        await ViewModel.QueryPayments();
        await ViewModel.QueryCustomers();
        await ViewModel.QueryPaymentMethods();
        await ViewModel.QueryUnpaidInvoices();
        await DispatcherQueue.EnqueueAsync(() => PaymentsGrid.View.Refresh());
    }

    private async void NewPaymentButton_Click(object sender, RoutedEventArgs e)
    {
        var done = false;
        var OneAdded = false;
        while (!done) {
            PaymentDataInputControl paymentDialog = new(ViewModel.UnpaidInvoices, ViewModel.PaymentMethods)
            {
                XamlRoot = XamlRoot,
                SecondaryButtonText = "Save and Create New"
            };
            var res = await paymentDialog.ShowAsync();
            if(res == ContentDialogResult.Primary || res == ContentDialogResult.Secondary)
            {
                var payment = paymentDialog.GetPayment();
                await ViewModel.SavePayment(payment);
                await SaveOrder(payment.Order);
                OneAdded = true;
                if(res == ContentDialogResult.Primary)
                {
                    done = true;
                }
            }
            else
            {
                done = true;
            }
        }
        if (OneAdded)
        {
            _ = ViewModel.QueryPayments();
        }
    }
    private async Task SaveOrder(Order order)
    {
        if (order.TotalPayments >= order.InvoiceTotal)
        {
            order.OrderStatus = Models.Enums.OrderStatus.Complete;
            order.Paid = true;
        }
        else
        {
            order.OrderStatus = Models.Enums.OrderStatus.Invoiced;
            order.Paid = false;
        }
        await ViewModel.SaveOrder(order);
    }

    private async void MenuFlyoutEdit_Click(object sender, RoutedEventArgs e)
    {
        if(ViewModel.SelectedPayment != null)
        { 
            PaymentDataInputControl inputControl = new(ViewModel.UnpaidInvoices, ViewModel.PaymentMethods)
            {
                XamlRoot= XamlRoot
            };
            inputControl.SetPayment(ViewModel.SelectedPayment);
            var res = await inputControl.ShowAsync();
            if(res == ContentDialogResult.Primary)
            {
                ViewModel.SelectedPayment = inputControl.GetPayment();
                await ViewModel.SavePayment(ViewModel.SelectedPayment);
                var index = ViewModel.Payments.IndexOf(ViewModel.SelectedPayment);
                ViewModel.Payments[index] = ViewModel.SelectedPayment;
                await SaveOrder(ViewModel.SelectedPayment.Order);
                PaymentsGrid.View.Refresh();
            }
        }
    }
}
