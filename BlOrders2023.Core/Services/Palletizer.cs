using System;
using System.Collections.Generic;
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
        BreastBox
    }

    #region Properties
    public PalletizerConfig Config { get; set; }
    #endregion Properties

    #region Fields
    private readonly Order _currentOrder;
    #endregion Fields

    #region Constructors

    Palletizer(PalletizerConfig config, Order order)
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

    public async Task<IEnumerable<Pallet>> GenerateFullPalletsAsync()
    {
        List<Pallet> pallets = new List<Pallet>();
        List<List<OrderItem>> GroupedByBox = new()
        {
            _currentOrder.Items.Where(i => GetBoxType(i.Product) == BoxType.BBox).ToList(),
            _currentOrder.Items.Where(i => GetBoxType(i.Product) == BoxType.CBox).ToList(),
            _currentOrder.Items.Where(i => GetBoxType(i.Product) == BoxType.BreastBox).ToList(),
            _currentOrder.Items.Where(i => GetBoxType(i.Product) == BoxType.Unknown).ToList()
        };

        foreach(var group in GroupedByBox)
        {
            if(! group.IsNullOrEmpty()){
                var totalNeeded = (int)group.Sum(i => i.Allocated == true ? i.QuanAllocated : i.Quantity );
                var itemIndex = 0;
                
                //should all be in the same group
                var palletRemaining = GetMaxBoxesPerPallet(group[0].Product);
                Pallet currentPallet = new(_currentOrder.OrderID);
                while (totalNeeded > 0) 
                {
                    var currentItem = group[itemIndex];
                    var received = (int) (currentItem.Allocated == true ? currentItem.QuanAllocated : currentItem.Quantity);
                    if(received <= palletRemaining )
                    {
                        currentPallet.Items.Add(currentItem.Product, received);
                        palletRemaining -= received;
                        itemIndex++;
                    }
                    else
                    {
                        currentPallet.Items.Add(currentItem.Product, palletRemaining);
                        pallets.Add(currentPallet);
                        currentPallet = new(_currentOrder.OrderID);
                        palletRemaining = GetMaxBoxesPerPallet(group[0].Product);
                    }
                }
            }

        }

        return pallets;

    }

    public async Task<IEnumerable<Pallet>> GenerateSingleProductPalletsAsync()
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
        var palletNumber = 1;
        foreach(Pallet pallet in pallets)
        {
            pallet.PalletIndex = palletNumber++;
        }
        return pallets;
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
            case 612:
            case 614:
            case 616:
                return BoxType.BBox;

            case 619:
            case 621:
            case 623:
            case 625:
            case 627:
            case 620:
            case 624:
            case 626:
                return BoxType.CBox;

            case 654:
            case 657:
            case 659:
            case 656:
            case 658:
            case 744:
            case 749:
            case 41:
            case 42:
                return BoxType.BreastBox;

            default:
                return BoxType.Unknown;
        }
    }
    #endregion Methods
}
