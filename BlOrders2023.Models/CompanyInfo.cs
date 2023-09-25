using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Models;
[Table("tbl_CompanyInfo")]
public class CompanyInfo : ObservableObject
{
    #region Fields
    private string _longCompanyName = string.Empty;
    private string _shortCompanyName = string.Empty;
    private string _streetAddress = string.Empty;
    private string _city = string.Empty;
    private string _state = string.Empty;
    private string _shortState = string.Empty;
    private string _shortZipCode = string.Empty;
    private string _longZipCode = string.Empty;
    private string _phone = string.Empty;
    private string _fax = string.Empty;
    private string _website = string.Empty;
    private string _email = string.Empty;
    #endregion Fields

    #region Properties
    [Key]
    public int ID { get; set; }

    [MaxLength(255)]
    public string LongCompanyName
    {
        get => _longCompanyName; 
        set => SetProperty(ref _longCompanyName, value);
    }

    [MaxLength(100)]
    public string ShortCompanyName
    {
        get => _shortCompanyName; 
        set => SetProperty(ref _shortCompanyName, value);
    }

    [MaxLength(255)]
    public string StreetAddress
    {
        get => _streetAddress; 
        set => SetProperty(ref _streetAddress, value);
    }

    [MaxLength(255)]
    public string City
    {
        get => _city; 
        set => SetProperty(ref _city, value);
    }

    [MaxLength(20)]
    public string State
    {
        get => _state; 
        set => SetProperty(ref _state, value);
    }

    [MaxLength(2)]
    [MinLength(2)]
    public string ShortState
    {
        get => _shortState; 
        set => SetProperty(ref _shortState, value);
    }

    [MaxLength(5)]
    [MinLength(5)]
    public string ShortZipCode
    {
        get => _shortZipCode; 
        set => SetProperty(ref _shortZipCode, value);
    }

    [MaxLength(10)]
    [MinLength(10)]
    public string LongZipCode
    {
        get => _longZipCode; 
        set => SetProperty(ref _longZipCode, value);
    }

    [MaxLength(10)]
    [MinLength(10)]
    public string Phone
    {
        get => _phone; 
        set => SetProperty(ref _phone, value);
    }

    [MaxLength(10)]
    [MinLength(10)]
    public string Fax
    {
        get => _fax; 
        set => SetProperty(ref _fax, value);
    }

    [MaxLength(255)]
    public string Website
    {
        get => _website; 
        set => SetProperty(ref _website, value);
    }

    [MaxLength(255)]
    public string Email
    {
        get => _email; 
        set => SetProperty(ref _email, value);
    }
    #endregion Properties

    #region Methods
    public string PhoneString()
    {
        if (!Phone.IsNullOrEmpty())
        {
            return string.Format("{0:(###)###-####}", Convert.ToInt64(Phone));
        }
        else
        {
            return string.Empty;
        }
    }

    public string FaxString()
    {
        if (!Fax.IsNullOrEmpty())
        {
            return string.Format("{0:(###)###-####}", Convert.ToInt64(Fax));
        }
        else
        {
            return string.Empty;
        }
    }
    #endregion Method
}
