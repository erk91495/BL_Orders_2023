using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Models;

namespace BlOrders2023.Core.Services;
public abstract class PalletizerBase : IPalletizer
{
    public abstract Order CurrentOrder { get; }

    public abstract Task<IEnumerable<Pallet>> PalletizeAsync();

    protected void NumberPallets(IEnumerable<Pallet> pallets)
    {
        var palletNumber = 1;
        foreach (Pallet pallet in pallets)
        {
            pallet.PalletIndex = palletNumber++;
            pallet.TotalPallets = pallets.Count();
        }
    }

    protected abstract int GetMaxBoxesPerPallet(Product product);

    protected IEnumerable<Pallet> GenerateSingleProductPallets()
    {
        List<Pallet> pallets = new List<Pallet>();
        foreach (var item in CurrentOrder.Items)
        {
            var remainingQuantity = (int)item.Quantity;
            if (CurrentOrder.Allocated == true && item.Allocated == true)
            {
                remainingQuantity = (int)item.QuanAllocated;
            }
            var maxPerPallet = GetMaxBoxesPerPallet(item.Product);

            while (remainingQuantity > 0)
            {
                if (remainingQuantity > maxPerPallet)
                {
                    remainingQuantity -= maxPerPallet;
                    Pallet pallet = new(CurrentOrder.OrderID);
                    pallet.Items.Add(item.Product, maxPerPallet);
                    pallets.Add(pallet);
                }
                else
                {
                    //Make a Pallet For the remainder
                    Pallet pallet = new(CurrentOrder.OrderID);
                    pallet.Items.Add(item.Product, remainingQuantity);
                    pallets.Add(pallet);
                    remainingQuantity = 0;
                }
            }
        }
        NumberPallets(pallets);
        return pallets;
    }
}
