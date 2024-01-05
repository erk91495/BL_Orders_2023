using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;

namespace BlOrders2023.Core.Data.SQL;
public class SqlBoxTable : IBoxTable
{
    #region Fields
    /// <summary>
    /// The DB context for the Bl orders database
    /// </summary>
    private readonly SqlBLOrdersDBContext _db;
    #endregion Fields

    #region Properties
    #endregion Properties

    #region Constructors
    public SqlBoxTable(SqlBLOrdersDBContext db)
    {
        _db = db;
    }
    #endregion Constructors

    #region Methods
    public IEnumerable<Box> Get(IEnumerable<int> ids = null, bool asNoTracking = false)
    {
        if (ids == null)
        {
            if (asNoTracking)
            {
                return _db.Boxes.AsNoTrackingWithIdentityResolution().ToList();
            }
            else
            {
                return _db.Boxes.ToList();
            }
        }
        else
        {
            if (asNoTracking)
            {
                return _db.Boxes.Where(b => ids.Contains(b.ID)).AsNoTrackingWithIdentityResolution().ToList();
            }
            else
            {
                return _db.Boxes.Where(b => ids.Contains(b.ID)).ToList();
            }
        }
    }
    public async Task<IEnumerable<Box>> GetAsync(IEnumerable<int> ids = null, bool asNoTracking = false)
    {
        if (ids == null)
        {
            if (asNoTracking)
            {
                return await _db.Boxes.AsNoTrackingWithIdentityResolution().ToListAsync();
            }
            else
            {
                return await _db.Boxes.ToListAsync();
            }
        }
        else
        {
            if (asNoTracking)
            {
                return await _db.Boxes.Where(b => ids.Contains(b.ID)).AsNoTrackingWithIdentityResolution().ToListAsync();
            }
            else
            {
                return await _db.Boxes.Where(b => ids.Contains(b.ID)).ToListAsync();
            }
        }
    }
    public async Task UpsertAsync(Box box)
    {
        _db.Boxes.Update(box);
        await _db.SaveChangesAsync();
    }
    #endregion Properties
}
