using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;
[Table("tbl_ProductCategories")]
public class ProductCategory : ObservableValidator
{
    #region Fields
    private int _categoryID;
    private string _categoryName;
    private bool _showTotalsOnReport;
    private int _displayIndex;
    private ObservableCollection<Product> products;
    #endregion Fields

    #region Properties
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

    public bool ShowTotalsOnReport
    {
        get => ShowTotalsOnReport;
        set => SetProperty(ref _showTotalsOnReport, value);
    }

    public int DisplayIndex
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
