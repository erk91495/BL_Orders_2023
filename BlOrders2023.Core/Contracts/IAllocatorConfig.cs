using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models.Enums;

namespace BlOrders2023.Core.Contracts;
public interface IAllocatorConfig
{
    public AllocatorMode AllocationMode { get; set; }
    public List<int> IDs { get; set; }
    public Dictionary<int, float> ForcedPortions
    {
        get; set;
    }

}
