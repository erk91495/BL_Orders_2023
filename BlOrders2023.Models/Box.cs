using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Models;

[Table("tbl_Boxes")]
public class Box : ObservableObject
{
    #region Fields
    private int _id;
    private string _boxName;
    private int _ti_Hi;
    private double _boxLength;
    private double _boxWidth;
    private double _boxHeight;

    [Range(1, int.MaxValue)]
    public int ID
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }
    [MaxLength(100)]
    public string BoxName
    {
        get => _boxName;
        set => SetProperty(ref _boxName, value);
    }
    public int Ti_Hi
    {
        get => _ti_Hi;
        set => SetProperty(ref _ti_Hi, value);
    }
    public double BoxLength
    {
        get => _boxLength;
        set => SetProperty(ref _boxLength, value);
    }
    public double BoxWidth
    {
        get => _boxWidth;
        set => SetProperty(ref _boxWidth, value);
    }
    public double BoxHeight
    {
        get => _boxHeight;
        set => SetProperty(ref _boxHeight, value);
    }
    #endregion Fields

    #region Properties
    #endregion Properties

    #region Constructors
    #endregion Constructors

    #region Methods
    public override string ToString() => BoxName;

    public override bool Equals(object? obj) => ID == (obj as Box).ID;
    #endregion Methods
}
