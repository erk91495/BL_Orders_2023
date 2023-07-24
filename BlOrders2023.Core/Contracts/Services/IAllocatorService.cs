using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Core.Contracts.Services;
public interface IAllocatorService
{
    public Task<bool> Allocate(IAllocatorConfig config);
}
