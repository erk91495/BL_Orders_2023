using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Contracts.Services;
internal interface IAllocatorService
{

    public Task<bool> AllocateOrders(); 
}
