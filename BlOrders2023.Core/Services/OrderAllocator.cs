using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BlOrders2023.Core.Contracts;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Data;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Exceptions;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Core.Services;
public class OrderAllocator : IAllocatorService
{
    #region Properties
    public IEnumerable<Order> Orders => _orders;
    public Dictionary<int, int> Inventory => _remainingInventory;
    #endregion Properties

    #region Fields
    private readonly IBLDatabase _db;
    private readonly Dictionary<int, float> ordered = new();
    private IList<Order> _orders;
    private IEnumerable<InventoryItem> _inventory;
    private ReadOnlyDictionary<int, int> _startingInventory;
    private readonly Dictionary<int, int> _remainingInventory = new();
    private IEnumerable<AllocationGroup> _allocationGroups;
    #endregion Fields

    #region Constructors
    public OrderAllocator( IBLDatabase db)
    {
        _db = db;
    }
    #endregion Constructors

    public async Task<bool> Allocate(IAllocatorConfig config)
    {
        if (config is not OrderAllocatorConfiguration || config == null) throw new ArgumentNullException("Invalid Configuration");

        //Task<IEnumerable<Order>> ordersTask = _db.Orders.GetAsync();
        //Task<IEnumerable<InventoryItem>> inventoryTask = _db.Inventory.GetInventoryAsync();
        //await Task.WhenAll(ordersTask, inventoryTask);

        //_orders = ordersTask.Result.ToList();
        //_inventory = inventoryTask.Result;


        var temporder = await _db.Orders.GetAsync(config.IDs);
        _orders = temporder.ToList();
        _inventory = await _db.Inventory.GetInventoryAsync();
        _allocationGroups = await _db.Allocation.GetAllocationGroupsAsync();

        
        CalculateInventory();


        //for debug and testing. do what you want
        if (config.AllocationType == Models.Enums.AllocatorMode.Test)
        {
            CalculateTotalOrdered(CustomerAllocationType.Grocer);
            AllocateGrocer();
        }
        else if(config.AllocationType == Models.Enums.AllocatorMode.Grocer)
        {
            CalculateTotalOrdered(CustomerAllocationType.Grocer);
            AllocateGrocer();
        }
        else if (config.AllocationType == Models.Enums.AllocatorMode.Gift)
        {
            CalculateTotalOrdered(CustomerAllocationType.Gift);
            AllocateGift();
        }
        else if (config.AllocationType == Models.Enums.AllocatorMode.Both)
        {
            CalculateTotalOrdered(CustomerAllocationType.Gift);
            AllocateGift();
            CalculateTotalOrdered(CustomerAllocationType.Grocer);
            AllocateGrocer();
        }
        else
        {
            throw new NotImplementedException();
        }
        
        MarkOrdersAllocated();
#if DEBUG
        if(Debugger.IsAttached)
        {
            PrintAllocatedOrders();
        }
#endif //DEBUG
        return true;
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
                    foreach(var order in _orders.Where(o => o.Customer.AllocationType == Models.Enums.CustomerAllocationType.Gift))
                    {
                        var orderedItem = order.Items.Where(e => e.ProductID == currentProductID).FirstOrDefault();
                        if (orderedItem != null)
                        {
                            var quantityNeeded = orderedItem.Quantity;
                            if(quantityNeeded >= _remainingInventory[currentProductID])
                            {
                                orderedItem.QuanAllocated = quantityNeeded;
                                orderedItem.Allocated = true;
                            }
                            //Up one
                            else if (idIndex + 1 < productIDs.Count && quantityNeeded >= _remainingInventory[productIDs[idIndex + 1]])
                            {
                                orderedItem = order.Items.Where(e => e.ProductID == productIDs[idIndex + 1]).FirstOrDefault();
                                if(orderedItem != null)
                                {
                                    orderedItem.QuanAllocated += quantityNeeded;
                                }
                                else
                                {
                                    OrderItem item = new()
                                    {
                                        ProductID = productIDs[idIndex + 1],
                                        Quantity = 0,
                                        QuanAllocated = quantityNeeded,
                                        Allocated = true
                                    };
                                }
                            }
                            //down one
                            else if (idIndex > 0 && quantityNeeded >= _remainingInventory[productIDs[idIndex -1 ]])
                            {
                                orderedItem = order.Items.Where(e => e.ProductID == productIDs[idIndex - 1]).FirstOrDefault();
                                if (orderedItem != null)
                                {
                                    orderedItem.QuanAllocated += quantityNeeded;
                                }
                                else
                                {
                                    OrderItem item = new()
                                    {
                                        ProductID = productIDs[idIndex - 1],
                                        Quantity = 0,
                                        QuanAllocated = quantityNeeded,
                                        Allocated = true
                                    };
                                }
                            }
                            else
                            {
                                throw new AllocationFailedException($"Could Not Allocate {currentProductID}. Insufficent Quantity: Needed {quantityNeeded}");
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
            _remainingInventory.Add(item.ProductID, item.QuantityOnHand);
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
                var totalExtraNeeded = 0;

                //First Make Sure the key is inventory orderd lists
                if (_remainingInventory.ContainsKey(currentProductID) && ordered.ContainsKey(currentProductID))
                {
                    var portion = (float)_remainingInventory[currentProductID] / (float)ordered[currentProductID];
                    portion = portion > 1 ? 1 : portion;
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



                    //Give Exra to each order
                    for (var orderIndex = 0; orderIndex < grocerOrders.Count(); orderIndex++)
                    {
                        var currentOrder = grocerOrders[orderIndex];
                        var currentOrderItem = currentOrder.Items.Where(i => i.ProductID == currentProductID).FirstOrDefault();
                        if(currentOrderItem != null){
                            //If I can't portion up set portion to 0 it should sort itself out when we try to go down
                            float extraPortion = 0;
                            if ((idIndex + 1) < (productIDs.Count - 1) && totalExtraNeeded != 0)
                            {
                                extraPortion = _remainingInventory[productIDs[idIndex + 1]] / totalExtraNeeded;
                            }


                            extraPortion = extraPortion > 1 ? 1 : extraPortion;
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
                                    var newItem = new OrderItem()
                                    {
                                        ProductID = productIDs[idIndex + 1],
                                        QuanAllocated = quantToGive,
                                        Quantity = 0,
                                        Allocated = true,
                                    };
                                    currentOrder.Items.Add(newItem);
                                }
                                _remainingInventory[productIDs[idIndex + 1]] -= quantToGive;
                            }
                            if (extra > 0)
                            {

                                if (idIndex + 2 < productIDs.Count && extra < _remainingInventory[productIDs[idIndex + 2]])
                                {
                                    //go up two
                                    if (!(currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex + 2]).IsNullOrEmpty()))
                                    {
                                        currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex + 2]).First().QuanAllocated += quantToGive;
                                        currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex + 2]).First().ExtraNeeded = 0;
                                    }
                                    else
                                    {
                                        var newItem = new OrderItem()
                                        {
                                            ProductID = productIDs[idIndex + 2],
                                            QuanAllocated = quantToGive,
                                            Quantity = 0,
                                            ExtraNeeded = 0,
                                        };
                                        currentOrder.Items.Add(newItem);
                                    }
                                    _remainingInventory[productIDs[idIndex + 2]] -= extra;
                                }
                                //go down one
                                else if (idIndex - 1 >= 0 && extra < _remainingInventory[productIDs[idIndex - 1]])
                                {
                                    if (!(currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex - 1]).IsNullOrEmpty()))
                                    {
                                        currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex - 1]).First().QuanAllocated += quantToGive;
                                        currentOrder.Items.Where(i => i.ProductID == productIDs[idIndex - 1]).First().ExtraNeeded = 0;
                                    }
                                    else
                                    {
                                        var newItem = new OrderItem()
                                        {
                                            ProductID = productIDs[idIndex - 1],
                                            QuanAllocated = quantToGive,
                                            Quantity = 0,
                                            ExtraNeeded = 0,
                                        };
                                        currentOrder.Items.Add(newItem);
                                    }
                                    _remainingInventory[productIDs[idIndex - 1]] -= extra;
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
        var ordesByAllocationType = _orders.Where(o => o.Customer.AllocationType == allocationType);
        foreach (var id in ordesByAllocationType.SelectMany(o => o.Items).Select(i => i.ProductID).Distinct())
        {
            ordered.Add(id, ordesByAllocationType.SelectMany(o => o.Items).Where(i => i.ProductID == id).Sum(i => i.Quantity));
        }
    }

    private void PrintAllocatedOrders()
    {
        Debug.WriteLine($"Remaining Inventory:");
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
}
