//#define DAN_IS_LOADING_PALLETS
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Models;
using BlOrders2023.Models.Enums;
using Microsoft.IdentityModel.Tokens;
using Nito.Collections;
using ServiceStack;

namespace BlOrders2023.Core.Services;
public class Palletizer : PalletizerBase
{
    #region Properties
    public PalletizerConfig Config { get; set; }
    public override Order CurrentOrder => _currentOrder;
    #endregion Properties

    #region Fields
    private readonly Order _currentOrder;
    #endregion Fields

    #region Constructors

    public Palletizer(PalletizerConfig config, Order order)
    {
        Config = config;
        _currentOrder = order;
    }
    #endregion Constructors

    #region Methods
    public async override Task<IEnumerable<Pallet>> PalletizeAsync()
    {
        IEnumerable<Pallet> result;
        if(_currentOrder.Customer.SingleProdPerPallet == false)
        {
#if DAN_IS_LOADING_PALLETS
            result = await GenerateSeperateFullPalletsAsync();
#else //DAN_IS_LOADING_PALLETS
            result = await GenerateFullPalletsAsync();
#endif //DAN_IS_LOADING_PALLETS
        }
        else
        {
            result = await GenerateSingleProductPalletsAsync();
        }
        return result;
    }
    private async Task<IEnumerable<Pallet>> GenerateSeperateFullPalletsAsync()
    {
        return await Task.Run(GenerateSeperateFullPallets);
    }
    private IEnumerable<Pallet> GenerateSeperateFullPallets()
    {
        List<Pallet> pallets = new();
        List<Pallet> partialPallet = new();
        Dictionary<Product, int> remainder = new();


        List<List<OrderItem>> GroupedByBox = new();
        foreach (var boxType in Enum.GetValues<BoxType>())
        {
            GroupedByBox.Add(_currentOrder.Items.Where(i => GetBoxType(i.Product) == boxType && i.Product.IsCredit != true).ToList());
        }

        foreach (var group in GroupedByBox)
        {
            Dictionary<Product, int> groupRemainder = new();
            if (!group.IsNullOrEmpty())
            {
                var maxPerPallet = GetMaxBoxesPerPallet(group[0].Product);
                var boxType = GetBoxType(group[0].Product);
                if (boxType != BoxType.Unknown)
                {
                    foreach (var item in group)
                    {
                        var quanNeeded = (int)(item.Allocated == true ? item.QuanAllocated : item.Quantity);
                        //Make Full Pallets
                        for (var i = 0; i < (int)(quanNeeded / maxPerPallet); i++)
                        {
                            Pallet p = new(_currentOrder.OrderID);
                            p.Items.Add(item.Product, maxPerPallet);
                            pallets.Add(p);
                        }

                        //Make Remainder Pallet
                        if (quanNeeded % maxPerPallet != 0)
                        {
                            groupRemainder.Add(item.Product, quanNeeded % maxPerPallet);
                        }
                    }
                    //try to combine as many _pallets as you can for each group
                    pallets = pallets.Concat(CombinePalletsBetter(ref groupRemainder, maxPerPallet)).ToList();
                    if(groupRemainder.Count > 0) {
                        Pallet groupPallet = new(_currentOrder.OrderID);
                        foreach (var item in groupRemainder)
                        {
                            var spaceOnPallet = maxPerPallet - groupPallet.Items.Values.Sum(q => q);
                            if (spaceOnPallet >= item.Value)
                            {
                                groupPallet.Items.Add(item.Key, item.Value);
                            }
                            else
                            {
                                groupPallet.Items.Add(item.Key, spaceOnPallet);
                                pallets.Add(groupPallet);
                                groupPallet = new(_currentOrder.OrderID);
                                groupPallet.Items.Add(item.Key, item.Value - spaceOnPallet);
                            }
                        }
                        partialPallet.Add(groupPallet);
                    }
                }
                else
                {
                    Pallet p = new Pallet(_currentOrder.OrderID);
                    foreach (var item in group)
                    {
                        var quanNeeded = (int)(item.Allocated == true ? item.QuanAllocated : item.Quantity);
                        p.Items.Add(item.Product, quanNeeded);
                    }
                    pallets.Add(p);
                }
            }
        }

        CombineBCPallets(ref pallets, partialPallet);

        NumberPallets(pallets);
        return pallets;
    }

    private void CombineBCPallets(ref List<Pallet> pallets, List<Pallet> partials)
    {
        var groupedPallets = new List<Pallet>[3]{new(), new(), new()};
        foreach(var pallet in partials)
        {
            var currentType = GetBoxType(pallet.Items.First().Key);

            switch(currentType) 
            {
                case BoxType.BBox:
                case BoxType.CBox:
                    groupedPallets[0].Add(pallet);
                    break;

                case BoxType.NonGMO_BBox:
                case BoxType.NonGMO_CBox:
                    groupedPallets[1].Add(pallet);
                    break;

                default:
                    pallets.Add(pallet);
                    break;
            }
        }

        var groupedList = groupedPallets[0];
        if (!groupedList.IsNullOrEmpty())
        {
            if (groupedList.Count > 2)
            {
                throw new Exception("Too Many Partial Pallets To Combine");
            }
            else
            {
                if(groupedList.Count == 2)
                {
                    if(groupedList[0].Items.Values.Sum(o => o) + groupedList[1].Items.Values.Sum(o => o) <= Config.MixedBoxesPerPallet)
                    {
                        Pallet p = new(_currentOrder.OrderID);
                        foreach(var item in groupedList[0].Items)
                        {
                            p.Items.Add(item.Key,item.Value);
                        }
                        foreach (var item in groupedList[1].Items)
                        {
                            p.Items.Add(item.Key, item.Value);
                        }
                        pallets.Add(p);
                    }
                    else
                    {
                        pallets.Add(groupedList[0]);
                        pallets.Add(groupedList[1]);
                    }
                }
                else
                {
                    pallets.Add(groupedPallets[0][0]);
                }
            }
        }

        groupedList = groupedPallets[1];
        if (!groupedList.IsNullOrEmpty())
        {
            
            if (groupedList.Count > 2)
            {
                throw new Exception("Too Many Partial Pallets To Combine");
            }
            else
            {
                if (groupedList.Count == 2)
                {
                    if (groupedList[0].Items.Values.Sum(o => o) + groupedList[1].Items.Values.Sum(o => o) <= Config.MixedBoxesPerPallet)
                    {
                        Pallet p = new(_currentOrder.OrderID);
                        foreach (var item in groupedList[0].Items)
                        {
                            p.Items.Add(item.Key, item.Value);
                        }
                        foreach (var item in groupedList[1].Items)
                        {
                            p.Items.Add(item.Key, item.Value);
                        }
                        pallets.Add(p);
                    }
                    else
                    {
                        pallets.Add(groupedList[0]);
                        pallets.Add(groupedList[1]);
                    }
                }
                else
                {
                    pallets.Add(groupedList[0]);
                }
            }
        }
    }

    private async Task<IEnumerable<Pallet>> GenerateFullPalletsAsync()
    {
        return await Task.Run(GenerateFullPallets);
    }

    private IEnumerable<Pallet> GenerateFullPallets()
    {
        List<Pallet> pallets = new();
        Dictionary<Product, int> remainder = new();


        List<List<OrderItem>> GroupedByBox = new();
        foreach (var boxType in Enum.GetValues<BoxType>())
        {
            GroupedByBox.Add(_currentOrder.Items.Where(i => GetBoxType(i.Product) == boxType && i.Product.IsCredit != true).ToList());
        }

        foreach(var group in GroupedByBox)
        {
            Dictionary<Product, int> groupRemainder = new();
            if(!group.IsNullOrEmpty()){
                //All items are grouped by box type so we can just grab the first one
                
                var boxType = GetBoxType(group[0].Product);
                if(boxType != BoxType.Unknown || true)
                {
                    foreach (var item in group)
                    {
                        var maxPerPallet = GetMaxBoxesPerPallet(item.Product);
                        var quanNeeded = (int)(item.Allocated == true ? item.QuanAllocated : item.Quantity);
                        //Make Full Pallets
                        for (var i = 0; i < (int)(quanNeeded / maxPerPallet); i++)
                        {
                            Pallet p = new(_currentOrder.OrderID);
                            p.Items.Add(item.Product, maxPerPallet);
                            pallets.Add(p);
                        }

                        //Make Remainder Pallet
                        if (quanNeeded % maxPerPallet != 0)
                        {
                            groupRemainder.Add(item.Product, quanNeeded % maxPerPallet);
                        }

                    }

                    //try to combine as many _pallets as you can for each group
                    pallets = pallets.Concat(CombinePalletsBetter(ref groupRemainder, 50)).ToList();
                    foreach (var item in groupRemainder)
                    {
                        remainder.Add(item.Key, item.Value);
                    }
                }
                //All Unknown Items Go On The same Pallet
                else
                {
                    Pallet p = new Pallet(_currentOrder.OrderID);
                    foreach (var item in group)
                    {
                        var quanNeeded = (int)(item.Allocated == true ? item.QuanAllocated : item.Quantity);
                        p.Items.Add(item.Product, quanNeeded);
                    }
                    pallets.Add(p);
                }
            }

        }

        //these _pallets are already to big to combine
        foreach(var item in remainder.Where(i => i.Value > Config.MixedBoxesPerPallet))
        {
            Pallet p = new Pallet(_currentOrder.OrderID);
            p.Items.Add(item.Key, item.Value);
            remainder.Remove(item.Key);
            pallets.Add(p);
        }

        //last chance to combine _pallets 
        pallets = pallets.Concat(CombinePalletsBetter(ref remainder, Config.MixedBoxesPerPallet)).ToList();
        if(!remainder.IsNullOrEmpty()){
            Pallet lastPallet = new(_currentOrder.OrderID);
            foreach(var item in remainder)
            {
                lastPallet.Items.Add(item.Key, item.Value);
            }
            pallets.Add(lastPallet);
        }

        NumberPallets(pallets);
        return pallets;

    }

    private IEnumerable<Pallet> CombinePallets(ref Dictionary<Product,int> remainder, int maxPerPallet)
    {
        //try to combine as many _pallets as you can for each group
        List<Pallet> pallets = new();
        Pallet pallet = new(_currentOrder.OrderID);
        var sortedRemainder = remainder.OrderByDescending(p => p.Value).ToDictionary();
        var totalNeeded = sortedRemainder.Values.Sum();
        var enumerator = sortedRemainder.Keys.GetEnumerator();
        enumerator.MoveNext();
        var remainingPalletSpace = maxPerPallet;
        while (totalNeeded >= remainingPalletSpace && enumerator.Current != null)
        {
            if (sortedRemainder[enumerator.Current] < remainingPalletSpace)
            {
                //Add items to pallet
                pallet.Items.Add(enumerator.Current, sortedRemainder[enumerator.Current]);
                remainingPalletSpace -= sortedRemainder[enumerator.Current];
                totalNeeded -= sortedRemainder[enumerator.Current];
                //get next item
                sortedRemainder.Remove(enumerator.Current);
                enumerator.MoveNext();
            }
            else
            {
                //Fill out pallet and create another one
                pallet.Items.Add(enumerator.Current, remainingPalletSpace);
                pallets.Add(pallet);
                pallet = new(_currentOrder.OrderID);

                totalNeeded -= remainingPalletSpace;
                //decement the item
                sortedRemainder[enumerator.Current] -= remainingPalletSpace;
                if(sortedRemainder[enumerator.Current] <= 0)
                {
                    sortedRemainder.Remove(enumerator.Current);
                    enumerator.MoveNext();
                }
                remainingPalletSpace = maxPerPallet;

            }
        }

        return pallets;
    }

    private IEnumerable<Pallet> CombinePalletsBetter(ref Dictionary<Product, int> remainder, int maxPerPallet) 
    {
        //try to combine as many _pallets as you can for each group
        List<Pallet> pallets = new();
        Pallet pallet = new(_currentOrder.OrderID);
        var remainingPalletSpace = maxPerPallet;
        var sortedRemainder = remainder.OrderByDescending(p => p.Value).ToDictionary();
        var deque = new Deque<KeyValuePair<Product, int>>(sortedRemainder.Count());
        foreach( var kvp in sortedRemainder)
        {
            deque.AddToBack(kvp);
        }
        KeyValuePair<Product, int>? frontItem = null;
        KeyValuePair<Product, int>? backItem = null;
        if (!deque.IsNullOrEmpty())
        {
            frontItem = deque.RemoveFromFront();
        }
        if (!deque.IsNullOrEmpty())
        {
            backItem = deque.RemoveFromBack();
        }
        while(deque.Count > 0 && frontItem != null && backItem != null)
        {
            if(frontItem != null)
            {
                if (remainingPalletSpace > frontItem.Value.Value)
                {
                    pallet.Items.Add(frontItem.Value.Key, frontItem.Value.Value);
                    remainingPalletSpace -= frontItem.Value.Value;
                }
                else
                {
                    throw new Exception("Full pallet made it into combine??");
                }
                while(remainingPalletSpace > 0 && backItem != null)
                {
                    if(remainingPalletSpace > backItem.Value.Value)
                    {
                        pallet.Items.Add(backItem.Value.Key, backItem.Value.Value);
                        remainingPalletSpace -= backItem.Value.Value;
                        if (!deque.IsNullOrEmpty())
                        {
                            backItem = deque.RemoveFromBack();
                        }
                        else
                        {
                            backItem = null;
                        }
                        
                    }
                    else
                    {
                        pallet.Items.Add(backItem.Value.Key, remainingPalletSpace);
                        backItem = new(backItem.Value.Key, backItem.Value.Value - remainingPalletSpace);
                        remainingPalletSpace = 0;
                        
                    }
                }
                pallets.Add(pallet);
                pallet = new(_currentOrder.OrderID);
                remainingPalletSpace = maxPerPallet;
                if (!deque.IsNullOrEmpty())
                {
                    frontItem = deque.RemoveFromFront();
                }
                else
                {
                    frontItem = null;
                }
            }

        }

        //clean up we need to get all of the remainder and give them back
        remainder.Clear();
        if (!pallet.Items.IsNullOrEmpty())
        {
            foreach(var item in pallet.Items) 
            {
                remainder.Add(item.Key, item.Value);
            }
        }
        if(frontItem != null)
        {
            remainder.Add(frontItem.Value.Key, frontItem.Value.Value);
        }
        if (backItem != null)
        {
            remainder.Add(backItem.Value.Key, backItem.Value.Value);
        }

        return pallets;
    }

    private async Task<IEnumerable<Pallet>> GenerateSingleProductPalletsAsync()
    {
        return await Task.Run(GenerateSingleProductPallets);
    }

    protected override int GetMaxBoxesPerPallet(Product product)
    {
        var boxType = GetBoxType(product);
        switch(boxType)
        {
            case BoxType.BBox:
            case BoxType.NonGMO_BBox:
                return Config.BBoxesPerPallet;
            case BoxType.CBox:
            case BoxType.NonGMO_CBox:
                return Config.CBoxesPerPallet;
            case BoxType.BreastBox:
            case BoxType.NonGMOBreastBox:
                return Config.BreastBoxesPerPallet;
            case BoxType.Unknown:
            default:
                return 50;
        }
    }

    private BoxType GetBoxType(Product product)
    {
        switch (product.ProductID)
        {
            case 610:
            case 613:
            case 615:
            case 617:
                return BoxType.BBox;
            case 612:
            case 614:
            case 616:
            case 812:
            case 816:
                return BoxType.NonGMO_BBox;

            case 619:
            case 621:
            case 623:
            case 625:
            case 627:
                return BoxType.CBox;
            case 620:
            case 624:
            case 626:
            case 820:
            case 824:
            case 826:
                return BoxType.NonGMO_CBox;

            case 654:
            case 657:
            case 659:
            case 744:
            case 749:
            case 41:
            case 42:
            case 854:
            case 857:
            case 859:
                return BoxType.BreastBox;

            case 656:
            case 658:
            case 856:
            case 858:
                return BoxType.NonGMOBreastBox;

            default:
                return BoxType.Unknown;
        }
    }
#endregion Methods
}
