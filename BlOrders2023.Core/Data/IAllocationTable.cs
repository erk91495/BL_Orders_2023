using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;

namespace BlOrders2023.Core.Data;
public interface IAllocationTable
{
    public Task<IEnumerable<int>> GetAllocatableOrderIDsAsync(DateTimeOffset item1, DateTimeOffset item2, AllocatorMode mode);
    IEnumerable<AllocationGroup> GetAllocationGroups();
    public Task<IEnumerable<AllocationGroup>> GetAllocationGroupsAsync();
}
