﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using BlOrders2023.Core.Contracts;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Data;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Exceptions;
using BlOrders2023.Models;
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
    private IBLDatabase _db;
    private Dictionary<int, float> ordered = new();
    private IList<Order> _orders;
    private IEnumerable<InventoryItem> _ineventory;
    private ReadOnlyDictionary<int, int> _startingInventory;
    private Dictionary<int, int> _remainingInventory = new();
    #endregion Fields

    #region Constructors
    public OrderAllocator( IBLDatabase db)
    {
        _db = db;
    }
    #endregion Constructors

    public async Task<bool> Allocate(IAllocatorConfig config)
    {
        //give me a sec
        await Task.Delay(1000);
        if (config is not OrderAllocatorConfiguration || config == null) throw new ArgumentNullException("Invalid Configuration");

        //Task<IEnumerable<Order>> ordersTask = _db.Orders.GetAsync();
        //Task<IEnumerable<InventoryItem>> inventoryTask = _db.Inventory.GetInventoryAsync();
        //await Task.WhenAll(ordersTask, inventoryTask);

        //_orders = ordersTask.Result.ToList();
        //_ineventory = inventoryTask.Result;


        var temporder = await _db.Orders.GetAsync(config.IDs);
        _orders = temporder.ToList();
        _ineventory = await _db.Inventory.GetInventoryAsync();

        CalculateTotalOrdered();
        CalculateInventory();


        //for debug and testing. do what you want
        if (config.AllocationType == Models.Enums.AllocatorMode.Test)
        {
            AllocateGrocer();
        }
        else if(config.AllocationType == Models.Enums.AllocatorMode.Grocer)
        {
            AllocateGrocer();
        }
        else if (config.AllocationType == Models.Enums.AllocatorMode.Gift)
        {
            AllocateGift();
        }
        else if (config.AllocationType == Models.Enums.AllocatorMode.Both)
        {
            AllocateGift();
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

    private void AllocateGift() => throw new NotImplementedException();

    private void MarkOrdersAllocated()
    {
        foreach(var o in _orders)
        {
            o.Allocated = true;
        }
    }


    private void CalculateInventory()
    {
        foreach(var item in _ineventory)
        {
            _remainingInventory.Add(item.ProductID, item.QuantityOnHand);
        }
        _startingInventory = new(_remainingInventory);
    }

    private bool AllocateGrocer()
    {


        //TODO: decide how we are going to get the groups
        List<int> productIDs = new(){ 610, 613, 615, 617, 619, 621, 623, 625, 627 };
        for (int idIndex = 0; idIndex < productIDs.Count; idIndex++)
        {
            int currentProductID = productIDs[idIndex];
            int totalExtraNeeded = 0;

            //First Make Sure the key is inventory orderd lists
            if (_remainingInventory.ContainsKey(currentProductID) && ordered.ContainsKey(currentProductID))
            {
                float portion = (float)_remainingInventory[currentProductID] / (float)ordered[currentProductID];
                portion = portion > 1 ? 1 : portion;
                // Give Portion to each order
                for (int orderIndex = 0; orderIndex < _orders.Count(); orderIndex++)
                {
                    Order currentOrder = _orders[orderIndex];
                    var currentItemIndex = currentOrder.Items.IndexOf(currentOrder.Items.Where(i => i.ProductID == currentProductID).FirstOrDefault());
                    var currentOrderItem = currentOrder.Items.Where(i => i.ProductID == currentProductID).FirstOrDefault();

                    if (currentOrderItem != null)
                    {
                        int quantToGive = (int)(currentOrderItem.Quantity * portion);
                        int extra = (int)currentOrderItem.Quantity - quantToGive;
                        totalExtraNeeded += extra;
                        _remainingInventory[currentProductID] -= quantToGive;
                        currentOrderItem.QuanAllocated += quantToGive;
                        currentOrderItem.ExtraNeeded += extra;
                        currentOrderItem.Allocated = true;
                    }
                    //_orders[orderIndex].Items[currentItemIndex] = currentOrderItem;
                }// end for each order



                //Give Exra to each order
                for (int orderIndex = 0; orderIndex < _orders.Count(); orderIndex++)
                {
                    Order currentOrder = _orders[orderIndex];
                    var currentOrderItem = currentOrder.Items.Where(i => i.ProductID == currentProductID).FirstOrDefault();
                    if(currentOrderItem != null){
                        //If I can't portion up set portion to 0 it should sort itself out when we try to go down
                        float extraPortion = 0;
                        if ((idIndex + 1) < (productIDs.Count - 1) && totalExtraNeeded != 0)
                        {
                            extraPortion = _remainingInventory[productIDs[idIndex + 1]] / totalExtraNeeded;
                        }


                        extraPortion = extraPortion > 1 ? 1 : extraPortion;
                        int quantToGive = (int)(currentOrderItem.ExtraNeeded * extraPortion);
                        int extra = (int)(currentOrderItem.ExtraNeeded) - quantToGive;
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

        return true;
    }

    private void CalculateTotalOrdered()
    {
        foreach (var id in _orders.SelectMany(o => o.Items).Select(i => i.ProductID).Distinct())
        {
            ordered.Add(id, _orders.SelectMany(o => o.Items).Where(i => i.ProductID == id).Sum(i => i.Quantity));
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
