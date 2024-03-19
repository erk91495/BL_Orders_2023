using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Azure.Identity;
using BlOrders2023.Core.Contracts;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Data;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Exceptions;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using BlOrders2023.Reporting;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Core.Services;
public class OrderAllocator : IAllocatorService
{
    #region Properties
    public IEnumerable<Order> Orders => _orders;
    public IEnumerable<InventoryTotalItem> Inventory => _inventory;

    public DateTime AllocationTime => _allocationTime;
    #endregion Properties

    #region Fields
    private readonly IBLDatabase _db;
    private readonly Dictionary<int, float> ordered = new();
    private IList<Order> _orders;
    private IEnumerable<InventoryTotalItem> _inventory;
    private Dictionary<int,int> _totalNotAllocated;
    private ReadOnlyDictionary<int, int> _startingInventory;
    private readonly Dictionary<int, int> _remainingInventory = new();
    private DateTime _allocationTime = DateTime.Now;
    private IEnumerable<AllocationGroup> _allocationGroups;
    private IAllocatorConfig _config;
    #endregion Fields

    #region Constructors
    public OrderAllocator(IBLDatabase db)
    {
        _db = db;
    }
    #endregion Constructors

    public async Task<bool> AllocateAsync(IAllocatorConfig config)
    {
        if (config is not OrderAllocatorConfiguration || config == null) throw new ArgumentNullException("Invalid Configuration");
        _allocationTime = DateTime.Now;
        _config = config;
        //Task<IEnumerable<Order>> ordersTask = _db.Orders.GetAsync();
        ////ordersTask.Start();
        //Task<IEnumerable<InventoryAdjustmentItem>> inventoryTask = _db.InventoryAdjustments.GetAsync();
        ////inventoryTask.Start();
        //Task<IEnumerable<AllocationGroup>> allocationGroupsTask = _db.Allocation.GetAllocationGroupsAsync();
        ////allocationGroupsTask.Start();
        //await Task.WhenAll(ordersTask, inventoryTask, allocationGroupsTask);

        //_orders = (IList<Order>)await ordersTask;
        //_inventory = await inventoryTask;
        //_allocationGroups = await allocationGroupsTask;

        _totalNotAllocated = await _db.Allocation.GetAllocatedNotReceivedTotals();
        var temporder = await _db.Orders.GetAsync(_config.IDs);
        _orders = temporder.ToList();
        _inventory = await _db.Inventory.GetInventoryTotalItemsAsync();
        _allocationGroups = await _db.Allocation.GetAllocationGroupsAsync();

        
        CalculateInventory();



        if(_config.AllocatorMode == AllocatorMode.Grocer)
        {
            CalculateTotalOrdered(CustomerAllocationType.Grocer);
            AllocateGrocer();
        }
        else if (_config.AllocatorMode == AllocatorMode.Gift)
        {
            CalculateTotalOrdered(CustomerAllocationType.Gift);
            AllocateGift();
        }
        else if (_config.AllocatorMode == AllocatorMode.Both)
        {
            CalculateTotalOrdered(CustomerAllocationType.Gift);
            AllocateGift();
            CalculateTotalOrdered(CustomerAllocationType.Grocer);
            AllocateGrocer();
        }
#if DEBUG
        //for debug and testing. do what you want
        else if (_config.AllocatorMode == AllocatorMode.Test)
        {
            CalculateTotalOrdered(CustomerAllocationType.Gift);
            AllocateGift();
            CalculateTotalOrdered(CustomerAllocationType.Grocer);
            AllocateGrocer();
        }
#endif //DEBUG
        else
        {
            throw new NotImplementedException();
        }
        
        MarkOrdersAllocated();
        UpdateRemainingInventory();
#if DEBUG
        if(Debugger.IsAttached)
        {
            PrintAllocatedOrders();
        }
#endif //DEBUG
        return true;
    }

    public async Task SaveAllocationAsync()
    {
        await _db.Orders.UpsertAsync(Orders);
        //TODO VALIDATE BUT WE DONT NEED TO ADJUST INVETORY ANYMORE JUST HANDE HOW WE CALC IT 
        //foreach(var item in _inventory) 
        //{
        //    await _db.Inventory.UpsertAdjustmentAsync(item);
        //}
    }
    private void UpdateRemainingInventory()
    {
        foreach(var key in _remainingInventory.Keys)
        {
            _inventory.Where(p => p.ProductID == key).FirstOrDefault().Total = (short)_remainingInventory[key];
        }
    }


    private void AllocateGift()
    {
        foreach (var group in _allocationGroups)
        {
            var productIDs = group.ProductIDs;
            for (var idIndex = 0; idIndex < productIDs.Count; idIndex++)
            {
                var currentProductID = productIDs[idIndex];

                //First Make Sure the key is inventory and orderd lists
                if (_remainingInventory.ContainsKey(currentProductID))
                {
                    foreach(var order in _orders.Where(o => o.Customer.AllocationType == CustomerAllocationType.Gift))
                    {
                        var orderedItem = order.Items.Where(e => e.ProductID == currentProductID).FirstOrDefault();
                        if (orderedItem != null)
                        {
                            var quantityNeeded = orderedItem.Quantity;
                            if(quantityNeeded <= _remainingInventory[currentProductID])
                            {
                                orderedItem.QuanAllocated += (int)quantityNeeded;
                                _remainingInventory[currentProductID] -= (int)quantityNeeded;
                                orderedItem.Allocated = true;
                            }
                            //Up one
                            else if (idIndex + 1 < productIDs.Count && quantityNeeded <= _remainingInventory[productIDs[idIndex + 1]])
                            {
                                orderedItem = order.Items.Where(e => e.ProductID == productIDs[idIndex + 1]).FirstOrDefault();
                                if(orderedItem != null)
                                {
                                    orderedItem.QuanAllocated += (int)quantityNeeded;
                                    
                                }
                                else
                                {
                                    var product = _db.Products.Get(productIDs[idIndex + 1],false).First();
                                    OrderItem item = new(product,order)
                                    {
                                        QuanAllocated = (int)quantityNeeded,
                                        Allocated = true
                                    };
                                    order.Items.Add(item);
                                }
                                _remainingInventory[productIDs[idIndex + 1]] -= (int)quantityNeeded;
                            }
                            //down one
                            else if (idIndex > 0 && quantityNeeded <= _remainingInventory[productIDs[idIndex -1 ]])
                            {
                                orderedItem = order.Items.Where(e => e.ProductID == productIDs[idIndex - 1]).FirstOrDefault();
                                if (orderedItem != null)
                                {
                                    orderedItem.QuanAllocated += (int)quantityNeeded;
                                }
                                else
                                {
                                    var product = _db.Products.Get(productIDs[idIndex - 1], true).First();
                                    OrderItem item = new(product, order)
                                    {
                                        QuanAllocated = (int)quantityNeeded,
                                        Allocated = true
                                    };
                                    order.Items.Add(item);
                                }
                                _remainingInventory[productIDs[idIndex - 1]] -= (int)quantityNeeded;
                            }
                            else
                            {
                                throw new AllocationFailedException($"Could Not AllocateAsync {currentProductID}. Insufficent Quantity: Needed {quantityNeeded}");
                            }
                        }
                    }

                }// end for each order

                else
                {
                    //TODO: fixme
                    if (ordered.ContainsKey(currentProductID))
                    {
                        Debug.WriteLine($"Item not in inventory: {currentProductID}");
                        throw new AllocationFailedException($"Item not in inventory: {currentProductID}");
                    }
                }

            }// end for each product id
        }

    }

    private void MarkOrdersAllocated()
    {
        foreach(var o in _orders)
        {
            o.Allocated = true;
        }
    }


    private void CalculateInventory()
    {
        foreach(var item in _inventory)
        {
            var total = item.Total - ( _totalNotAllocated.ContainsKey(item.ProductID) ? _totalNotAllocated[item.ProductID] : 0);
            _remainingInventory.Add(item.ProductID, total);
        }
        _startingInventory = new(_remainingInventory);
    }

    private bool AllocateGrocer()
    {
        var grocerOrders = _orders.Where(o => o.Customer.AllocationType == CustomerAllocationType.Grocer).ToList();
        foreach(var group in _allocationGroups)
        {
            var productIDs = group.ProductIDs;
            for (var idIndex = 0; idIndex < productIDs.Count; idIndex++)
            {
                var currentProductID = productIDs[idIndex];
                var totalExtraNeeded = 0f;

                //First Make Sure the key is inventory orderd lists
                if (_remainingInventory.ContainsKey(currentProductID) && ordered.ContainsKey(currentProductID))
                {
                    var portion = (float)_remainingInventory[currentProductID] / (float)ordered[currentProductID];
                    if (portion > 1)
                    {
                        portion = 1;
                    }
                    else if (portion < 0)
                    {
                        portion = 0;
                    }
                    // Give Portion to each order
                    for (var orderIndex = 0; orderIndex < grocerOrders.Count(); orderIndex++)
                    {
                        var currentOrder = grocerOrders[orderIndex];
                        var currentItemIndex = currentOrder.Items.IndexOf(currentOrder.Items.Where(i => i.ProductID == currentProductID).FirstOrDefault());
                        var currentOrderItem = currentOrder.Items.Where(i => i.ProductID == currentProductID).FirstOrDefault();

                        if (currentOrderItem != null)
                        {
                            var quantToGive = (int)(currentOrderItem.Quantity * portion);
                            var extra = (int)currentOrderItem.Quantity - quantToGive;
                            totalExtraNeeded += extra;
                            _remainingInventory[currentProductID] -= quantToGive;
                            currentOrderItem.QuanAllocated += quantToGive;
                            currentOrderItem.ExtraNeeded += extra;
                            currentOrderItem.Allocated = true;
                        }
                        //grocerOrders[orderIndex].Items[currentItemIndex] = currentOrderItem;
                    }// end for each order


                    float extraPortion = 0;
                    if ((idIndex + 1) < (productIDs.Count) && totalExtraNeeded != 0)
                    {
                        extraPortion = _remainingInventory[productIDs[idIndex + 1]] / totalExtraNeeded;
                    }

                    if (extraPortion > 1)
                    {
                        extraPortion = 1;
                    }
                    else if (extraPortion < 0)
                    {
                        extraPortion = 0;
                    }
                    //Give Exra to each order
                    for (var orderIndex = 0; orderIndex < grocerOrders.Count(); orderIndex++)
                    {
                        var currentOrder = grocerOrders[orderIndex];
                        var currentOrderItem = currentOrder.Items.Where(i => i.ProductID == currentProductID).FirstOrDefault();
                        if(currentOrderItem != null){

                            var quantToGive = (int)(currentOrderItem.ExtraNeeded * extraPortion);
                            var extra = (int)(currentOrderItem.ExtraNeeded) - quantToGive;
                            if (quantToGive > 0)
                            {
                                //up one
                                if(!(currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex + 1]).IsNullOrEmpty()))
                                {
                                    currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex + 1]).First().QuanAllocated += quantToGive;
                                }
                                else
                                {
                                    var product = _db.Products.Get(productIDs[idIndex + 1], true).First();
                                    var item = new OrderItem(product, currentOrder)
                                    {
                                        QuanAllocated = quantToGive,
                                        Quantity = 0,
                                        Allocated = true,
                                    };
                                    currentOrder.Items.Add(item);
                                }
                                _remainingInventory[productIDs[idIndex + 1]] -= quantToGive;
                            }

                            if (extra > 0)
                            {
                                //go down one
                                if (idIndex - 1 >= 0 && extra <= _remainingInventory[productIDs[idIndex - 1]])
                                {
                                    if (!(currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex - 1]).IsNullOrEmpty()))
                                    {
                                        currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex - 1]).First().QuanAllocated += extra;
                                        currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex - 1]).First().ExtraNeeded = 0;
                                    }
                                    else
                                    {
                                        var product = _db.Products.Get(productIDs[idIndex -1], true).First();
                                        var newItem = new OrderItem(product, currentOrder)
                                        {
                                            QuanAllocated = extra,
                                            Quantity = 0,
                                            ExtraNeeded = 0,
                                            Allocated = true,
                                        };
                                        currentOrder.Items.Add(newItem);
                                    }
                                    _remainingInventory[productIDs[idIndex - 1]] -= extra;
                                }
                                //go up two
                                else if (idIndex + 2 < productIDs.Count && extra <= _remainingInventory[productIDs[idIndex + 2]])
                                {
                                    
                                    if (!(currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex + 2]).IsNullOrEmpty()))
                                    {
                                        currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex + 2]).First().QuanAllocated += extra;
                                        currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex + 2]).First().ExtraNeeded = 0;
                                    }
                                    else
                                    {
                                        var product = _db.Products.Get(productIDs[idIndex + 2], true).First();
                                        var newItem = new OrderItem(product, currentOrder)
                                        {
                                            QuanAllocated = extra,
                                            Quantity = 0,
                                            ExtraNeeded = 0,
                                            Allocated = true,
                                        };
                                        currentOrder.Items.Add(newItem);
                                    }
                                    _remainingInventory[productIDs[idIndex + 2]] -= extra;
                                }
                                else
                                {
    #if DEBUG
                                    PrintAllocatedOrders();
    #endif //DEBUG
                                    throw new AllocationFailedException($"Could Not Replace {currentProductID} on Order {currentOrder.OrderID}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    //TODO: fixme
                        if (ordered.ContainsKey(currentProductID))
                        {
                            Debug.WriteLine($"Item not in inventory: {currentProductID}");
                            throw new AllocationFailedException($"Item not in inventory: {currentProductID}");
                        }
                }

            }// end for each product id
        }
        return true;
    }

    private void CalculateTotalOrdered(CustomerAllocationType allocationType)
    {
        ordered.Clear();
        var ordesByAllocationType = _orders.Where(o => o.Customer.AllocationType == allocationType);
        foreach (var id in ordesByAllocationType.SelectMany(o => o.Items).Select(i => i.ProductID).Distinct())
        {
            ordered.Add(id, ordesByAllocationType.SelectMany(o => o.Items).Where(i => i.ProductID == id).Sum(i => i.Quantity));
        }
    }

    private void PrintAllocatedOrders()
    {
        Debug.WriteLine($"Remaining InventoryAdjustments:");
        foreach (var id in _remainingInventory.Keys)
        {
            Debug.WriteLine($"{id} : {_remainingInventory[id]}");
        }
        
        foreach(var o in  _orders)
        {
            Debug.WriteLine($"Order ID {o.OrderID}");
            foreach(var item in o.Items) 
            {
                Debug.WriteLine($"Product ID: {item.ProductID} Ordered: {item.Quantity} | Allocated: {item.QuanAllocated}");
            }
        }
        
    }

    public async Task<IEnumerable<int>> GetOrdersIDToAllocateAsync(DateTimeOffset item1, DateTimeOffset item2, AllocatorMode mode)
    {
        return await _db.Allocation.GetAllocatableOrderIDsAsync(item1, item2, mode);
    }

    public async Task<string> GenerateAllocationSummary() 
    {
        ReportGenerator generator = new(_db.CompanyInfo);
        return await Task.Run(() => generator.GenerateAllocationSummaryReport(Orders, _config.AllocatorMode, AllocationTime));
    }

    public async Task<string> GenerateAllocationDetails()
    {
        ReportGenerator generator = new(_db.CompanyInfo);
        return await Task.Run(() => generator.GenerateAllocationDetailsReport(Orders, _allocationGroups, _config.AllocatorMode, AllocationTime));
    }
}
