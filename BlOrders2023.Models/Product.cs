using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;

[Table("tblProducts")]
public class Product : ObservableObject
{
    #region Fields
    private int _productID;
    private string? _productName;
    private short? _noPerCase;
    private decimal _wholesalePrice;
    private decimal _kettteringPrice;
    private string? _UPCCode;
    private string? _KPCCode;
    private bool? _fixedPrice;
    private string? _companyCode;
    private string? _ALUCode;
    private bool _inactive;
    #endregion Fields

    #region Properties
    [Key]
    [Required]
    public int ProductID
    {
        get => _productID; 
        set => SetProperty(ref _productID, value);
    }
    [MinLength(1)]
    [MaxLength(40)]
    public string? ProductName
    {
        get => _productName; 
        set => SetProperty(ref _productName, value);
    }
    public short? NoPerCase
    {
        get => _noPerCase; 
        set => SetProperty(ref _noPerCase, value);
    }
    [Column("Price_(Wholesale)")]
    public decimal WholesalePrice
    {
        get => _wholesalePrice; 
        set => SetProperty(ref _wholesalePrice, value);
    }

    public decimal KetteringPrice
    {
        get => _kettteringPrice;
        set => SetProperty(ref _kettteringPrice, value);
    }
    [MaxLength(15)]
    public string? UPCCode
    {
        get => _UPCCode; 
        set => SetProperty(ref _UPCCode, value);
    }
    [MaxLength(15)]
    public string? KPCCode
    {
        get => _KPCCode; 
        set => SetProperty(ref _KPCCode, value);
    }

    public bool? FixedPrice
    {
        get => _fixedPrice; 
        set => SetProperty(ref _fixedPrice, value);
    }
    //public string? KrogerDeptNo { get; set; }
    [StringLength(8)]
    public string? CompanyCode
    {
        get => _companyCode; 
        set => SetProperty(ref _companyCode, value);
    }

    [MaxLength(50)]
    public string? ALUCode
    {
        get => _ALUCode;
        set => SetProperty(ref _ALUCode, value);
    }
    [Required]
    public bool Inactive
    {
        get => _inactive;
        set => SetProperty(ref _inactive, value);
    }

    #endregion Properties
    public Product()
    {
    }
    public Product(Product product)
    {
        ProductID = product.ProductID;
        ProductName = product.ProductName;
        NoPerCase = product.NoPerCase;
        WholesalePrice = product.WholesalePrice;
        KetteringPrice = product.KetteringPrice;
        UPCCode = product.UPCCode;
        KPCCode = product.KPCCode;
        FixedPrice = product.FixedPrice;
        CompanyCode = product.CompanyCode;
        ALUCode = product.ALUCode;
        Inactive = product.Inactive;
    }
    public override string ToString()
    {
        return ProductName ?? string.Empty;
    }
}
