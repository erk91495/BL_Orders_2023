using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;
[Table("tbl_Discounts")]
public class Discount : ObservableObject
{
    #region Fields
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    private Guid _id;
    private string _description;
    private DiscountTypes _type;
    private double _modifier;
    private DateTime? _startDate;
    private DateTime? _endDate;
    private bool _inactive = false;
    #endregion Field

    #region Properties
    public Guid ID
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    [MaxLength(128)]
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public DiscountTypes Type
    {
        get => _type;
        set => SetProperty(ref _type, value);
    }

    public double Modifier
    {
        get => _modifier;
        set => SetProperty(ref _modifier, value);
    }

    public DateTime? StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTime? EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    public bool Inactive
    {
        get => _inactive; set => _inactive = value;
    }
    public virtual IEnumerable<DiscountProductMap> DiscountProductMaps { get; set; }
    public virtual ObservableCollection<Product> Products { get; set; } = new();
    public virtual IEnumerable<DiscountCustomerMap> DiscountCustomerMaps { get; set; }
    public virtual ObservableCollection<WholesaleCustomer> Customers { get; set; } = new();
    #endregion Properties

}
