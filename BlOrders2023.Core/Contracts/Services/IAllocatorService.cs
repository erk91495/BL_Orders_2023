using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Contracts.Services;
public interface IAllocatorService
{
    public IEnumerable<Order> Orders { get; }
    public Dictionary<int, int> Inventory { get; }
    public Task<bool> Allocate(IAllocatorConfig config);
}
