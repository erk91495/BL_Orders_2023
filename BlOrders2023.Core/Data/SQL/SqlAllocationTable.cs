using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using Microsoft.EntityFrameworkCore;

namespace BlOrders2023.Core.Data.SQL;
internal class SqlAllocationTable : IAllocationTable
{
    #region Properties
    #endregion Properties

    #region Fields
    /// <summary>
    /// The DB context for the Bl orders database
    /// </summary>
    private readonly SqlBLOrdersDBContext _db;
    #endregion Fields

    #region Constructors
    public SqlAllocationTable(SqlBLOrdersDBContext db)
    {
        _db = db;
    }

    public IEnumerable<AllocationGroup> GetAllocationGroups() => _db.AllocationGroups.ToList();
    #endregion Constructors

    #region Methods
    public async Task<IEnumerable<AllocationGroup>> GetAllocationGroupsAsync()
    {
        return await _db.AllocationGroups.ToListAsync();
    }
    #endregion Methods

}
