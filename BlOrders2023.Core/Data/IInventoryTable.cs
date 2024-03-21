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
    public IEnumerable<LiveInventoryItem> GetInventoryItems(IEnumerable<int> ids = null);
    public Task<IEnumerable<LiveInventoryItem>> GetInventoryItemsAsync(IEnumerable<int> ids = null);
    public IEnumerable<LiveInventoryItem> GetUnshippedInventoryItems(IEnumerable<int> ids = null);
    public Task<IEnumerable<LiveInventoryItem>> GetUnshippedInventoryItemsAsync(IEnumerable<int> ids = null);
    public Task<IEnumerable<InventoryAdjustmentItem>> GetInventoryAdjutmentsAsync(IEnumerable<int> ids = null);
    public IEnumerable<InventoryAdjustmentItem> GetInventoryAdjustments(IEnumerable<int> ids = null);
    public IEnumerable<InventoryTotalItem> GetInventoryTotalItems(IEnumerable<int> ids = null);
    public Task<IEnumerable<InventoryTotalItem>> GetInventoryTotalItemsAsync(IEnumerable<int> ids = null);
    public Task UpsertAdjustmentAsync(InventoryAdjustmentItem item);
    public Task UpsertAdjustmentsAsync(IEnumerable<InventoryAdjustmentItem> inventory);
    public Task AdjustInventoryAsync(InventoryTotalItem item);
    public Task AdjustInventoryAsync(int id, int adjustment);
    public bool InsertLiveInventoryItem(LiveInventoryItem item);
    public Task<bool> InsertLiveInventoryItemAsync(LiveInventoryItem item);
    public bool DeleteLiveInventoryItem(LiveInventoryItem item);
    public Task<bool> DeleteLiveInventoryItemAsync(LiveInventoryItem item);
    public Task<LiveInventoryItem?> FindLiveInventoryItem(ShippingItem shippingItem);
    public Task<Dictionary<int, int>> GetAllocatedNotReceivedTotalsAsync();
    public Dictionary<int, int> GetAllocatedNotReceivedTotals();
    public Task<IEnumerable<InventoryAuditItem>> GetInventoryAuditLogAsync();
    public Task<bool> DuplicateInventoryCheck(string scanline);
}
