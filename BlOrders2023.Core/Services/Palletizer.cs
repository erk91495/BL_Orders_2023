using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Models;
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Core.Services;
public class Palletizer
{
    private enum BoxType
    {
        Unknown,
        BBox,
        CBox,
        NonGMO_BBox,
        NonGMO_CBox,
        BreastBox,
        NonGMOBreastBox,
    }

    #region Properties
    public PalletizerConfig Config { get; set; }
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
    public async Task<IEnumerable<Pallet>> PalletizeAsync()
    {
        IEnumerable<Pallet> result;
        if(_currentOrder.Customer.SingleProdPerPallet == false)
        {
            result = await GenerateFullPalletsAsync();
        }
        else
        {
            result = await GenerateSingleProductPalletsAsync();
        }
        return result;
    }

    private async Task<IEnumerable<Pallet>> GenerateFullPalletsAsync()
    {
        return await Task.Run(GenerateFullPallets);
    }

    private IEnumerable<Pallet> GenerateFullPallets()
    {
        List<Pallet> pallets = new();
        Dictionary<Product, int> remainder = new();
        List<List<OrderItem>> GroupedByBox = new()
        {
            _currentOrder.Items.Where(i => GetBoxType(i.Product) == BoxType.BBox).ToList(),
            _currentOrder.Items.Where(i => GetBoxType(i.Product) == BoxType.CBox).ToList(),
            _currentOrder.Items.Where(i => GetBoxType(i.Product) == BoxType.BreastBox).ToList(),
            _currentOrder.Items.Where(i => GetBoxType(i.Product) == BoxType.Unknown).ToList()
        };

        foreach(var group in GroupedByBox)
        {
            Dictionary<Product, int> groupRemainder = new();
            if(! group.IsNullOrEmpty()){
                //All items are grouped by box type so we can just grab the first one
                var maxPerPallet = GetMaxBoxesPerPallet(group[0].Product);
                foreach(var item in group)
                {
                    var quanNeeded = (int)(item.Allocated == true ? item.QuanAllocated ?? 0 : item.Quantity);
                    //Make Full Pallets
                    for(var i = 0; i < (int)(quanNeeded / maxPerPallet); i++)
                    {
                        Pallet p = new(_currentOrder.OrderID);
                        p.Items.Add(item.Product, maxPerPallet);
                        pallets.Add(p);
                    }

                    //Make Remainder Pallet
                    if(quanNeeded % maxPerPallet != 0)
                    {
                        groupRemainder.Add(item.Product, quanNeeded % maxPerPallet);
                    }

                }
                //try to combine as many pallets as you can for each group
                pallets = pallets.Concat(CombinePallets(ref groupRemainder, maxPerPallet)).ToList();

                foreach(var item in groupRemainder)
                {
                    remainder.Add(item.Key, item.Value);
                }

            }

        }
        //last chance to combine pallets 
        pallets = pallets.Concat(CombinePallets(ref remainder, Config.MixedBoxesPerPallet)).ToList();
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
        //try to combine as many pallets as you can for each group
        List<Pallet> pallets = new();
        Pallet pallet = new(_currentOrder.OrderID);
        var totalNeeded = remainder.Values.Sum();
        var enumerator = remainder.Keys.GetEnumerator();
        enumerator.MoveNext();
        var remainingPalletSpace = maxPerPallet;
        while (totalNeeded >= remainingPalletSpace && enumerator.Current != null)
        {
            if (remainder[enumerator.Current] < remainingPalletSpace)
            {
                //Add items to pallet
                pallet.Items.Add(enumerator.Current, remainder[enumerator.Current]);
                remainingPalletSpace -= remainder[enumerator.Current];
                totalNeeded -= remainder[enumerator.Current];
                //get next item
                remainder.Remove(enumerator.Current);
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
                remainder[enumerator.Current] -= remainingPalletSpace;
                if(remainder[enumerator.Current] <= 0)
                {
                    remainder.Remove(enumerator.Current);
                    enumerator.MoveNext();
                }
                remainingPalletSpace = maxPerPallet;

            }
        }

        return pallets;
    }

    private async Task<IEnumerable<Pallet>> GenerateSingleProductPalletsAsync()
    {
        return await Task.Run(GenerateSingleProductPallets);
    }
    private IEnumerable<Pallet> GenerateSingleProductPallets()
    {
        List<Pallet> pallets = new List<Pallet>();
        foreach(var item in _currentOrder.Items)
        {
            var remainingQuantity = (int)item.Quantity;
            if (_currentOrder.Allocated == true && item.Allocated == true)
            {
                remainingQuantity = (int)item.QuanAllocated;
            }
            var maxPerPallet = GetMaxBoxesPerPallet(item.Product);
            
            while (remainingQuantity > 0)
            {
                if(remainingQuantity > maxPerPallet)
                {
                    remainingQuantity -= maxPerPallet;
                    Pallet pallet = new(_currentOrder.OrderID);
                    pallet.Items.Add(item.Product,maxPerPallet);
                    pallets.Add(pallet);
                }
                else
                {
                    //Make a Pallet For the remainder
                    Pallet pallet = new(_currentOrder.OrderID);
                    pallet.Items.Add(item.Product, remainingQuantity);
                    pallets.Add(pallet);
                    remainingQuantity = 0;
                }
            }
        }
        NumberPallets(pallets);
        return pallets;
    }

    private void NumberPallets(IEnumerable<Pallet> pallets)
    {
        var palletNumber = 1;
        foreach (Pallet pallet in pallets)
        {
            pallet.PalletIndex = palletNumber++;
            pallet.TotalPallets = pallets.Count();
        }
    }

    private int GetMaxBoxesPerPallet(Product product)
    {
        var boxType = GetBoxType(product);
        switch(boxType)
        {
            case BoxType.BBox:
                return Config.BBoxesPerPallet;
            case BoxType.CBox:
                return Config.CBoxesPerPallet;
            case BoxType.BreastBox:
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
