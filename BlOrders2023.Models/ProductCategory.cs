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
}
