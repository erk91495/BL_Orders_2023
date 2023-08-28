using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;

namespace BlOrders2023.Core.Contracts.Services;
public interface IAllocatorService
{
    public IEnumerable<Order> Orders { get; }
    public IEnumerable<InventoryItem> Inventory { get; }
    public Task<bool> AllocateAsync(IAllocatorConfig config);
    public Task<IEnumerable<int>> GetOrdersIDToAllocateAsync(DateTimeOffset item1, DateTimeOffset item2, AllocatorMode mode);
    public Task SaveAllocationAsync();
}
