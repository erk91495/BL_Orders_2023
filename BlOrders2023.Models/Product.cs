using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;

[Table("tblProducts")]
public class Product
{
    [Key]
    [Required]
    public int ProductID { get; set; }
    [MinLength(1)]
    [MaxLength(40)]
    public string? ProductName { get; set; }
    public short? NoPerCase { get; set; }
    [Column("Price_(Wholesale)")]
    public decimal WholesalePrice { get; set; }
    public string? UPCCode { get; set; }
    public string? KPCCode { get; set; }
    public bool? FixedPrice { get; set; }
    //public string? KrogerDeptNo { get; set; }
    [StringLength(8)]
    public string? CompanyCode { get; set; }
    public Product()
    {
    }
    public Product(Product product)
    {
        ProductID = product.ProductID;
        ProductName = product.ProductName;
        NoPerCase = product.NoPerCase;
        WholesalePrice = product.WholesalePrice;
        UPCCode = product.UPCCode;
        KPCCode = product.KPCCode;
        FixedPrice = product.FixedPrice;
        CompanyCode = product.CompanyCode;
    }
    public override string ToString()
    {
        return ProductName ?? string.Empty;
    }
}
