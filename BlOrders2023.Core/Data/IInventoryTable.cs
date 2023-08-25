using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Data;
public interface IInventoryTable
{
    public Task<IEnumerable<InventoryItem>> GetInventoryAsync(IEnumerable<int> ids = null);
    public IEnumerable<InventoryItem> GetInventory(IEnumerable<int> ids = null);
    public Task UpsertAsync(InventoryItem item);
    public Task UpsertAsync(IEnumerable<InventoryItem> inventory);
}
