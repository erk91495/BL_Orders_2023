﻿using BlOrders2023.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.ViewModels;

public class ReportsPageViewModel : ObservableRecipient
{

    public Order? GetOrder(int orderID)
    {
        return App.GetNewDatabase().Orders.Get(orderID).FirstOrDefault();
    }

    internal IEnumerable<Order> GetOrdersByPickupDate(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetByPickupDate(startDate, endDate);
    }

    internal IEnumerable<Order> GetOrdersByPickupDateThenName(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return App.GetNewDatabase().Orders.GetByPickupDateThenName(startDate, endDate);
    }
}
