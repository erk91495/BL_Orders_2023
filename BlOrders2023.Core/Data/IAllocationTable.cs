﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Data;
public interface IAllocationTable
{
    public Task<IEnumerable<AllocationGroup>> GetAllocationGroupsAsync();
}
