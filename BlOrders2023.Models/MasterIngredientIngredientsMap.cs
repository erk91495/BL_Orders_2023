using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;
[Table("MasterIngredientIngredientsMap")]
public class MasterIngredientIngredientsMap
{
    #region Fields
    private Guid _ID;
    private Guid _MasterIngredientID;
    private Guid _IngredientID;
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
    public Guid IngredientID
    {
        get => _IngredientID;
        set => _IngredientID = value;
    }

    [ForeignKey(nameof(MasterIngredientID))]
    public virtual MasterIngredient MasterIngredient
    {
        get; set;
    }
    [ForeignKey(nameof(IngredientID))]
    public virtual Ingredient Ingredient
    {
        get; set;
    }
    #endregion Properties
}
