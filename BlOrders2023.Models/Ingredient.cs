using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Models;
[Table("tbl_Ingredients")]
public class Ingredient
{
    private Guid _ID;
    private string _Name;
    private int? _ProductID;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ID
    {
        get => _ID;
        set => _ID = value;
    }
    public string Name
    {
        get => _Name;
        set => _Name = value;
    }
    public int? ProductID
    {
        get => _ProductID;
        set => _ProductID = value;
    }

    public virtual IEnumerable<MasterIngredient>? MasterIngredients { get; set; };

    [ForeignKey(nameof(ProductID))]
    public virtual Product? Product { get; set; }
}
