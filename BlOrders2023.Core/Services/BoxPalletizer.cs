using BlOrders2023.Core.Contracts.Services;
using BlOrders2023.Core.Helpers;
using BlOrders2023.Models;
using Microsoft.IdentityModel.Tokens;

namespace BlOrders2023.Core.Services;
public class BoxPalletizer : PalletizerBase
{
    #region Fields
    private readonly Order _currentOrder;
    private readonly PalletizerConfig _config;
    private readonly List<Pallet> _pallets = [];
    #endregion Fields

    #region Properties
    public override Order CurrentOrder => _currentOrder;
    #endregion Properties

    #region Constructors
    public BoxPalletizer(PalletizerConfig config, Order order)
    {
        _currentOrder = order;
        _config = config;
    }
    #endregion Constructors

    #region Methods
    public async override Task<IEnumerable<Pallet>> PalletizeAsync()
    {
        return await Task.Run(GeneratePallets);
    }

    public IEnumerable<Pallet> GeneratePallets()
    {
        if (_config.SingleItemPerPallet)
        {
            return GenerateSingleProductPallets();
        }
        else
        {
            return GenreateMixedPallets();
        }


    
    }

    private IEnumerable<Pallet> GenreateMixedPallets()
    {
        Dictionary<Product, int> remainder = new();


        List<List<OrderItem>> GroupedByBox = new();
        foreach (var boxType in _currentOrder.Items.Select(i => i.Product.Box).Distinct())
        {
            GroupedByBox.Add(_currentOrder.Items.Where(i => i.Product.Box == boxType && i.Product.IsCredit != true).ToList());
        }

        foreach (var group in GroupedByBox)
        {
            Dictionary<Product, int> groupRemainder = new();
            if (!group.IsNullOrEmpty())
            {
                foreach (var item in group)
                {
                    var quanNeeded = (int)(item.Allocated == true ? item.QuanAllocated : item.Quantity);
                    var maxPerPallet = GetMaxBoxesPerPallet(item.Product);

                    //Make Full Pallets
                    for (var i = 0; i < (int)(quanNeeded / maxPerPallet); i++)
                    {
                        Pallet p = new(_currentOrder.OrderID);
                        p.Items.Add(item.Product, maxPerPallet);
                        _pallets.Add(p);
                    }

                    //Make Remainder Pallet
                    if (quanNeeded % maxPerPallet != 0)
                    {
                        groupRemainder.Add(item.Product, quanNeeded % maxPerPallet);
                    }
                }
                CombineLargestWithSmallest(ref groupRemainder, false);
                foreach (var item in groupRemainder)
                {
                    remainder.Add(item.Key, item.Value);
                }
            }
        }
        //We no longer want to add pallets togeter if they are differnt boxes
        //CombineLargestWithSmallest(ref remainder, false);

        foreach (var item in remainder)
        {
            Pallet p = new(_currentOrder.OrderID);
            p.Items.Add(item.Key,item.Value);
            _pallets.Add(p);
        }
        NumberPallets(_pallets);
        return _pallets;
    }
    private void CombineLargestWithSmallest(ref Dictionary<Product,int> remainder, bool fullPalletsOnly)
    {
        //need at least 2 to combine
        if(remainder.Count >= 2){
            List<Product> keys = remainder.OrderByDescending(p => p.Value).Select(p => p.Key).ToList();
            var LargestPair = remainder.FirstOrDefault(p => p.Key == keys.First());
            var SmallestPair = remainder.FirstOrDefault(p => p.Key == keys.Last());
            remainder.Remove(LargestPair.Key);
            keys.Remove(LargestPair.Key);
            var boxesNeeded = GetMaxBoxesPerPallet(LargestPair.Key) - LargestPair.Value;
            Pallet currentPallet = new(_currentOrder.OrderID);
            currentPallet.Items.Add(LargestPair.Key, LargestPair.Value);
            while(keys.Count > 0) 
            {
                if(SmallestPair.Value < boxesNeeded)
                {
                    currentPallet.Items.Add(SmallestPair.Key, SmallestPair.Value);
                    boxesNeeded -= SmallestPair.Value;
                    remainder.Remove(SmallestPair.Key);
                    keys.Remove(SmallestPair.Key);
                    SmallestPair = remainder.FirstOrDefault(p => p.Key == keys.Last());
                }
                else if(SmallestPair.Value > boxesNeeded)
                {//TODO NEED CHECKS BEFORE CHANGING LARGEST AND SMALLEST PAIR
                    currentPallet.Items.Add(SmallestPair.Key, boxesNeeded);
                    remainder[SmallestPair.Key] = SmallestPair.Value - boxesNeeded;
                    _pallets.Add(currentPallet);
                    remainder.Remove(LargestPair.Key);
                    keys.Remove(LargestPair.Key);
                    LargestPair = remainder.FirstOrDefault(p => p.Key == keys.First());
                    boxesNeeded = GetMaxBoxesPerPallet(LargestPair.Key) - LargestPair.Value;
                    currentPallet = new(_currentOrder.OrderID);
                }
                else
                {
                    currentPallet.Items.Add(SmallestPair.Key, SmallestPair.Value);
                    remainder.Remove(SmallestPair.Key);
                    keys.Remove(SmallestPair.Key);
                    remainder.Remove(LargestPair.Key);
                    keys.Remove(LargestPair.Key);
                    SmallestPair = remainder.FirstOrDefault(p => p.Key == keys.Last());
                    LargestPair = remainder.FirstOrDefault(p => p.Key == keys.First());
                    boxesNeeded = GetMaxBoxesPerPallet(LargestPair.Key);
                    _pallets.Add(currentPallet);
                    currentPallet= new(_currentOrder.OrderID);
                }
            }
            if (fullPalletsOnly)
            {
                if(currentPallet.Items.Count > 0)
                {
                    foreach(var item in currentPallet.Items)
                    {
                        remainder.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                _pallets.Add(currentPallet);
            }
        }
    }

    protected override int GetMaxBoxesPerPallet(Product product)
    {
        //Cant multiply null values and having 0 boxes per pallet is bad
        if (product.PalletHeight.GetValueOrDefault(0) != 0 && product.Box != null)
        {
            return product.PalletHeight.Value * product.Box.Ti_Hi;
        }
        else
        {
            return _config.MixedBoxesPerPallet;
        }
    }
    #endregion Methods
}
