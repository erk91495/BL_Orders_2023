using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;
[Table("tbl_RecipeMasterIngredientsMap")]
public class RecipeMasterIngredientsMap
{
    #region Fields
    private Guid _ID;
    private Guid _MasterIngredientID;
    private Guid _RecipeID;
    private decimal _Quantity;
    #endregion Fields

    #region Properties
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ID
    {
        get => _ID;
        set => _ID = value;
    }
    public Guid MasterIngredientID
    {
        get => _MasterIngredientID;
        set => _MasterIngredientID = value;
    }
    public Guid RecipeID
    {
        get => _RecipeID;
        set => _RecipeID = value;
    }
    public decimal Quantity
    {
        get => _Quantity;
        set => _Quantity = value;
    }

    [ForeignKey(nameof(MasterIngredientID))]
    public virtual MasterIngredient MasterIngredient { get; set; }
    [ForeignKey (nameof(RecipeID))]
    public virtual Recipe Recipe { get; set; }
    #endregion Properties
}
