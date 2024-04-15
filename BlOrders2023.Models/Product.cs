using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Numerics;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;

[Table("tblProducts")]
public class Product : ObservableObject
{
    #region Fields
    private int _productID;
    private string? _productName;
    //private short? _noPerCase;
    private decimal _wholesalePrice;
    private decimal _kettteringPrice;
    private string? _UPCCode;
    private string? _KPCCode;
    private bool? _fixedPrice;
    private string? _companyCode;
    private string? _ALUCode;
    private bool _inactive;
    private bool _isCredit;
    private int? _boxID;
    private Box? _box;
    private int? _palletHeight;
    private int? _categoryID;
    private ProductCategory? _category;
    #endregion Fields

    #region Properties
    [Key]
    [Range(1, int.MaxValue)]
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

    //public short? NoPerCase
    //{
    //    get => _noPerCase; 
    //    set => SetProperty(ref _noPerCase, value);
    //}

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
    [Required]
    public bool IsCredit
    {
        get => _isCredit;
        set => SetProperty(ref _isCredit, value);
    }

    
    public int? BoxID
    {
        get => _boxID;
        set => SetProperty(ref _boxID, value);
    }

    [ForeignKey(nameof(BoxID))]
    [JsonIgnore]
    public virtual Box? Box
    {
        get => _box;
        set => SetProperty(ref _box, value);
    }
    [Range(0, int.MaxValue)]
    public int? PalletHeight
    {
        get => _palletHeight;
        set => SetProperty( ref _palletHeight, value);
    }

    public int? CategoryID
    {
        get => _categoryID; 
        set => SetProperty(ref _categoryID, value);
    }

    [ForeignKey(nameof(CategoryID))]
    [JsonIgnore]
    public virtual ProductCategory? Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    public virtual IEnumerable<Discount> Discounts { get; set; }
    #endregion Properties
    public Product()
    {
    }
    public Product(Product product)
    {
        ProductID = product.ProductID;
        ProductName = product.ProductName;
        //NoPerCase = product.NoPerCase;
        WholesalePrice = product.WholesalePrice;
        KetteringPrice = product.KetteringPrice;
        UPCCode = product.UPCCode;
        KPCCode = product.KPCCode;
        FixedPrice = product.FixedPrice;
        CompanyCode = product.CompanyCode;
        ALUCode = product.ALUCode;
        Inactive = product.Inactive;
        IsCredit = product.IsCredit;
        BoxID = product.BoxID;
        Box = product.Box;
        CategoryID = product.CategoryID;
        Category = product.Category;
    }
    public override string ToString()
    {
        return ProductName ?? string.Empty;
    }
}
