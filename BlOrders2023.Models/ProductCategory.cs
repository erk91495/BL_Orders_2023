using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;
[Table("tbl_ProductCategories")]
public class ProductCategory : ObservableValidator
{
    #region Fields
    private int _categoryID;
    private string _categoryName;
    private bool _showTotalsOnReports;
    private short _displayIndex;
    private ObservableCollection<Product> products;
    #endregion Fields

    #region Properties
    [Key]
    public int CategoryID
    {
        get => _categoryID;
        set => SetProperty(ref _categoryID, value);
    }

    [MinLength(1)]
    public string CategoryName
    {
        get => _categoryName;
        set => SetProperty(ref _categoryName, value);
    }

    public bool ShowTotalsOnReports
    {
        get => _showTotalsOnReports;
        set => SetProperty(ref _showTotalsOnReports, value);
    }

    public short DisplayIndex
    {
        get => _displayIndex;
        set => SetProperty(ref _displayIndex, value);
    }
    public virtual ObservableCollection<Product> Products 
    { 
        get => products;
        set => SetProperty(ref products, value);
    }


    #endregion Properties
    #region Methods
    public override bool Equals(object? obj)
    {
        if(obj is ProductCategory category)
        {
            return category.CategoryID == this.CategoryID;
        }
        return false;
    }
    public static bool operator ==(ProductCategory? obj1, ProductCategory? obj2)
    {
        if(obj1 is null && obj2 is null) return true;
        if(obj1 is not null && obj2 is not null) return obj1.CategoryID == obj2.CategoryID;
        return false;
    }
    public static bool operator !=(ProductCategory? obj1, ProductCategory? obj2) => !(obj1 == obj2);
    #endregion Methods
}
