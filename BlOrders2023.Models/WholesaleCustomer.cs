using BlOrders2023.Models.Enums;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;

[Table("tblCustomerWholesale")]
public class WholesaleCustomer
{
    #region Properties
    [Key]
    public int CustID { get; set; }
    public string CustomerName { get; set; } = "";
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string BillingCustomerName { get; set; } = "";
    public string? BillingAddress { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingState { get; set; }
    public string? BillingZipCode { get; set; }
    public string Phone { get; set; }
    public string? PhoneExt { get; set; }
    public string? Phone_2 { get; set; }
    public string? Phone2Ext { get; set; }
    public string? Buyer { get; set; }
    public string? LastPurchase { get; set; }
    public string? Fax { get; set; }
    // -1 is grocer 0 is gift
    //public short Grocer { get; set; }
    public bool Inactive { get; set; }
    public string? Email { get; set; }
    //[Column("Gift-Grocer")]
    //public string? Gift_Grocer { get; set; }
    public bool? SingleProdPerPallet { get; set; }
    public int? CustomerClassID { get; set; }
    //private bool? isGrocer { get; set; }
    public bool UseSameAddress { get; set; }
    
    [Column("isGrocer")]
    public CustomerAllocationType AllocationType { get; set; }
    
    [ForeignKey("CustomerClassID")]
    public virtual CustomerClass CustomerClass
    {
        get { return _customerClass; }
        set
        {
            CustomerClassID = value.ID;
            _customerClass = value;
        }
    }

    [Required]
    public bool COD { get; set;}
    public virtual List<Order> Orders { get; set; } = new();

    public virtual WholesaleCustomerNote? Note { get; set; } 
    #endregion Properties

    #region Fields
    private CustomerClass _customerClass;
    #endregion Fields

    #region Constructors
    public WholesaleCustomer()
    {
        CustomerName = "";
        Phone = "";
        AllocationType = CustomerAllocationType.Grocer;
        Inactive = false;
        SingleProdPerPallet = false;
        _customerClass = new();
    }
    #endregion Constructors

    #region Methods
    public string CityStateZip()
    {
        return string.Format("{0} {1}  {2}", City, State, ZipCode);
    }

    public string BillingCityStateZip()
    {
        return string.Format("{0} {1}  {2}", BillingCity, BillingState, BillingZipCode);
    }

    public string PhoneString()
    {
        if(!Phone.IsNullOrEmpty()) {
            if (PhoneExt != null)
            {
                return string.Format("{0:(###)###-####} x.{1}", Convert.ToInt64(Phone), PhoneExt);
            }
            else
            {
                return string.Format("{0:(###)###-####}", Convert.ToInt64(Phone));
            }
        }else
        {
            return string.Empty;
        }
    }
    public string Phone2String()
    {
        if (!Phone_2.IsNullOrEmpty())
        {
            if (Phone2Ext != null)
            {
                return string.Format("{0:(###)###-####} x.{1}", Convert.ToInt64(Phone_2), Phone2Ext);
            }
            else
            {
                return string.Format("{0:(###)###-####}", Convert.ToInt64(Phone_2));
            }
        }
        else
        {
            return string.Empty;
        }
    }

    public string FaxString()
    {
        if(!Fax.IsNullOrEmpty()) 
        {
            return string.Format("{0:(###)###-####}", Convert.ToInt64(Fax));
        }
        else
        {
            return string.Empty;
        }
    }

    public override string ToString()
    {
        return CustomerName;
    }
    #endregion Methods
}
