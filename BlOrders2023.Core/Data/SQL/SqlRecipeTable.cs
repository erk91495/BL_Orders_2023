using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Data.SQL;
internal class SqlRecipeTable : IRecipeTable
{
    #region Fields
    private readonly SqlBLOrdersDBContext _db;
    #endregion Fields

    #region Properties
    #endregion Properties
    
    #region Constructors
    public SqlRecipeTable(SqlBLOrdersDBContext db)
    {
        _db = db;
    }
    #endregion Constructors

    #region Methods
    public IEnumerable<Ingredient> GetIngredients(Guid? ID = null)
    {
        if (ID != null)
        {
            return _db.Ingredients.Where(i => i.ID == ID).ToList();
        }
        else
        {
            return _db.Ingredients.ToList();
        }
    }
    public async Task<IEnumerable<Ingredient>> GetIngredientsAsync(Guid? ID = null)
    {
        if(ID != null)
        {
            return await _db.Ingredients.Where(i => i.ID == ID).ToListAsync();
        }
        else
        {
            return await _db.Ingredients.ToListAsync();
        }
    }
    public IEnumerable<MasterIngredient> GetMasterIngredients(Guid? ID = null)
    {
        if(ID != null)
        {
            return _db.MasterIngredients.Where(i => i.ID == ID).ToList();
        }
        else
        {
            return _db.MasterIngredients.ToList();
        }
    }
    public async Task<IEnumerable<MasterIngredient>> GetMasterIngredientsAsync(Guid? ID = null)
    {
        if(ID != null)
        {
            return await _db.MasterIngredients.Where(i => i.ID != ID).ToListAsync();
        }
        else
        {
            return await _db.MasterIngredients.ToListAsync();
        }
    }
    public IEnumerable<Recipe> GetRecipes(Guid? ID = null)
    {
        if(ID != null)
        {
            return _db.Recipes.Where(i => i.ID == ID).ToList();
        }
        else
        {
            return _db.Recipes.ToList();
        }
    }
    public async Task<IEnumerable<Recipe>> GetRecipesAsync(Guid? ID = null)
    {
        if(ID != null)
        {
            return await _db.Recipes.Where(i => i.ID == ID).ToListAsync();
        }
        else
        {
            return await _db.Recipes.ToListAsync();
        }
    }
    public bool Upsert(Recipe recipe) 
    {
        return _db.SaveChanges() != 0;
    }
    public async Task<bool> UpsertAsync(Recipe recipe)
    {
        return await _db.SaveChangesAsync() != 0;   
    }
    #endregion Methods
}
