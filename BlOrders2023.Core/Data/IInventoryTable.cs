﻿using System;
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
    public Task<IEnumerable<LiveInventoryItem>> GetInventoryItemsAsync(DateTimeOffset startDate, DateTimeOffset endDate);
    public Task<IEnumerable<LiveInventoryItem>> GetInventoryItemsAsync(IEnumerable<int> ids = null);
    public IEnumerable<LiveInventoryItem> GetUnshippedInventoryItems(IEnumerable<int> ids = null);
    public Task<IEnumerable<LiveInventoryItem>> GetUnshippedInventoryItemsAsync(IEnumerable<int> ids = null);
    public Task<IEnumerable<InventoryAdjustmentItem>> GetInventoryAdjutmentsAsync(IEnumerable<int> ids = null);
    public IEnumerable<InventoryAdjustmentItem> GetInventoryAdjustments(IEnumerable<int> ids = null);
    public IEnumerable<InventoryTotalItem> GetInventoryTotalItems(IEnumerable<int> ids = null);
    public Task<IEnumerable<InventoryTotalItem>> GetInventoryTotalItemsAsync(IEnumerable<int> ids = null);
    public Task UpsertAdjustmentAsync(InventoryAdjustmentItem item);
    public Task UpsertAdjustmentsAsync(IEnumerable<InventoryAdjustmentItem> inventory);
    public Task AdjustInventoryAsync(InventoryTotalItem item, string? reason);
    public Task AdjustInventoryAsync(int id, int adjustment, string? reason);
    public bool InsertLiveInventoryItem(LiveInventoryItem item);
    public Task<bool> InsertLiveInventoryItemAsync(LiveInventoryItem item);
    public bool DeleteLiveInventoryItem(LiveInventoryItem item);
    public Task<bool> DeleteLiveInventoryItemAsync(LiveInventoryItem item);
    public void UpsertLiveInventoryItem(LiveInventoryItem item);
    public Task UpsertLiveInventoryItemAsync(LiveInventoryItem item);
    public Task<LiveInventoryItem?> FindLiveInventoryItem(ShippingItem shippingItem);
    public Task<Dictionary<int, int>> GetAllocatedNotReceivedTotalsAsync();
    public Dictionary<int, int> GetAllocatedNotReceivedTotals();
    public Task<IEnumerable<InventoryAuditItem>> GetInventoryAuditLogAsync();
    public Task<LiveInventoryItem?> DuplicateInventoryCheck(string scanline);
    public Task ZeroLiveInventoryAsync();
    public IEnumerable<LiveInventoryRemovalLogItem> GetLiveInventoryRemovalLogItems();
    public Task<IEnumerable<LiveInventoryRemovalLogItem>> GetLiveInventoryRemovalLogItemsAsync();
    public IEnumerable<LiveInventoryRemovalReason> GetLiveInventoryRemovalReasons();
    public Task<IEnumerable<LiveInventoryRemovalReason>> GetLiveInventoryRemovalReasonsAsync();
    public Task InsertLiveInventoryRemovalLogItemAsync(LiveInventoryRemovalLogItem removalEntry);
    public Task<IEnumerable<LiveInventoryItem>> GetInventoryItemsByScanDateAsync(DateTimeOffset startDate, DateTimeOffset endDate);
}
