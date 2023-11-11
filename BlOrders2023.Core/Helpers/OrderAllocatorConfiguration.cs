using System.ComponentModel;
using BlOrders2023.Core.Contracts;
using BlOrders2023.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BlOrders2023.Core.Helpers;
public class OrderAllocatorConfiguration : IAllocatorConfig

{

    public AllocatorMode AllocatorMode { get; set; }
    public List<int> IDs { get; set; }
    public Dictionary<int,float> ForcedPortions { get; set; }
}
