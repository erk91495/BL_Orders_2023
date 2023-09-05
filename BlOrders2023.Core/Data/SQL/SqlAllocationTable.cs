using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
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

    public async Task<IEnumerable<int>> GetAllocatableOrderIDsAsync(DateTimeOffset item1, DateTimeOffset item2, AllocatorMode allocationMode)
    {
        if(allocationMode == AllocatorMode.Both)
        {
            return await _db.Orders.Where(o => o.OrderStatus == Models.Enums.OrderStatus.Ordered &&
                                           o.PickupDate >= item1 &&
                                           o.PickupDate <= item2 &&
                                           o.Allocated != true &&
                                           o.Frozen != true)
                               .AsNoTrackingWithIdentityResolution()
                               .Select(o => o.OrderID)
                               .ToListAsync();
        }
        else
        {
            CustomerAllocationType allocatorType;
            if(allocationMode == AllocatorMode.Gift)
            {
                allocatorType = CustomerAllocationType.Gift;
            }
            else
            {
                allocatorType = CustomerAllocationType.Grocer;
            }
            return await _db.Orders.Where(o => o.OrderStatus == Models.Enums.OrderStatus.Ordered &&
                                               o.PickupDate >= item1 && 
                                               o.PickupDate <= item2 && 
                                               o.Allocated != true &&
                                               o.Frozen != true && 
                                               o.Customer.AllocationType == allocatorType)
                                   .AsNoTrackingWithIdentityResolution()
                                   .Select(o => o.OrderID)
                                   .ToListAsync();
        }
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
