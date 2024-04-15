using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace BlOrders2023.Models;
[Keyless]
public class ProductTotalsItem : ObservableObject
{
    #region Fields
    private int _productID;
    private string _productName;
    private int _quantityReceived;
    private double _extPrice;
    private double _netWeight;
    #endregion Fields

    #region Properties
    public int ProductID
    {
        get => _productID;
        set => SetProperty(ref _productID, value);
    }
    public string ProductName
    {
        get => _productName;
        set => SetProperty(ref _productName, value);
    }
    public int QuantityReceived
    {
        get => _quantityReceived;
        set => SetProperty(ref _quantityReceived, value);
    }
    public double ExtPrice
    {
        get => _extPrice;
        set => SetProperty(ref _extPrice, value);
    }
    public double NetWeight
    {
        get => _netWeight;
        set => SetProperty(ref _netWeight, value);
    }
    #endregion Properties
}
