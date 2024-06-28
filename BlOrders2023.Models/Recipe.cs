﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;
[Table("tbl_Recipes")]
public class Recipe
{
    #region Fields
    private Guid _ID;
    private string _Name = string.Empty;
    #endregion Fields

    #region Properties
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ID
    {
        get => _ID; set => _ID = value;
    }

    public string Name
    {
        get => _Name; set => _Name = value;
    }

    public IEnumerable<MasterIngredient> MasterIngredients { get; }
    #endregion Properties
}