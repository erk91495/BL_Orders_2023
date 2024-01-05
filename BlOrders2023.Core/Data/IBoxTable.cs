using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Data;
public interface IBoxTable
{
    public Task<IEnumerable<Box>> GetAsync(IEnumerable<int> ids = null, bool asNoTracking = false);
    public IEnumerable<Box> Get(IEnumerable<int> ids = null, bool asNoTracking = false);
    public Task UpsertAsync(Box box);
}
