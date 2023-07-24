using BlOrders2023.Core.Contracts;
using BlOrders2023.Models.Enums;

namespace BlOrders2023.Core.Helpers;
internal class OrderAllocatorConfiguration : IAllocatorConfig
{
    public AllocationType AllocationType { get; set; }
    public List<int> IDs { get; set; }
}
