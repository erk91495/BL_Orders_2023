using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Data;
public  interface IRecipeTable
{
    public IEnumerable<Recipe> GetRecipes(Guid? ID = null);
    public Task<IEnumerable<Recipe>> GetRecipesAsync(Guid? ID = null);
    public IEnumerable<MasterIngredient> GetMasterIngredients(Guid? ID = null);
    public Task<IEnumerable<MasterIngredient>> GetMasterIngredientsAsync(Guid? ID = null);
    public IEnumerable<Ingredient> GetIngredients(Guid? ID = null);
    public Task<IEnumerable<Ingredient>> GetIngredientsAsync(Guid? ID = null);
    public bool Upsert(Recipe recipe);
    public Task<bool> UpsertAsync(Recipe recipe);
}
